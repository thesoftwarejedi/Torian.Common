using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace SerializationTesting
{
    /// <summary>
    /// Compresses and writes to an S3 bucket using multipart uploads (if output > chunk size)
    /// 
    /// min chunk size (amazon rule) is 5MB, so if set lower it will default to 5MB.
    /// 
    /// This class uses a pipeline of streams to properly handle streaming the compressed data.
    /// Calls to Flush are swallowed as they would potentially force early chunking which would 
    /// ruin the zip compression and/or upload to s3 too early.
    /// 
    /// this -> noflush -> buffered -> gzip -> noflush -> buffered -> mem -> s3
    /// 
    /// Due to bufferring the actual chunk size may end up being double the requested size, and the results are double or triple buffered.
    /// Thus the actual memory consumed could be as high as 6x the chunk size.
    /// </summary>
    public class S3MultipartFileStream : Stream
    {
        private readonly AmazonS3Client _s3;
        private readonly string _bucketName;
        private readonly string _key;
        private readonly int _chunkSize;
        private readonly Stream _noflush1;
        private readonly Stream _buffered1;
        private readonly Stream _gzip;
        private readonly Stream _noflush2;
        private readonly Stream _buffered2;
        private readonly MemoryStream _mem;

        private readonly List<PartETag> _etags;
        private string _uploadId;

        private int _partCount = 1;
        private bool _closed = false;

        public S3MultipartFileStream(AmazonS3Client s3, string bucketName, string key, int chunkSize)
        {
            _s3 = s3;
            _bucketName = bucketName;
            _key = key;
            _chunkSize = chunkSize < 5_120_001 ? 5_120_001 : chunkSize;

            _etags = new List<PartETag>();

            _mem = new MemoryStream(chunkSize + 1);
            _buffered2 = new BufferedStream(_mem, chunkSize);
            _noflush2 = new NoFlushStream(_buffered2);
            _gzip = new GZipStream(_noflush2, CompressionLevel.Optimal);
            _buffered1 = new BufferedStream(_gzip, chunkSize);
            _noflush1 = new NoFlushStream(_buffered1);

        }

        public override bool CanRead => false;
        public override bool CanSeek => false;
        public override bool CanWrite => true;
        public override long Length => throw new NotSupportedException();
        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
        public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Flush() => _noflush1.Flush();

        public override void Write(byte[] buffer, int offset, int count)
        {
            //write the first stream in our pipeline
            _noflush1.Write(buffer, offset, count);

            //if the memory contents are up to chunk size, upload a part
            if (_mem.Length > _chunkSize)
                UploadPart();
        }

        private void UploadPart()
        {
            //if this is the first part, begin upload parts
            if (_uploadId == null)
                _uploadId = _s3.InitiateMultipartUploadAsync(_bucketName, _key).ConfigureAwait(false).GetAwaiter().GetResult().UploadId;

                _mem.Position = 0;
            // upload
            var etag = _s3.UploadPartAsync(new UploadPartRequest() { BucketName = _bucketName, Key = _key, InputStream = _mem, PartNumber = _partCount, UploadId = _uploadId }).ConfigureAwait(false).GetAwaiter().GetResult().ETag;
            _etags.Add(new PartETag(_partCount, etag));
            _partCount++;
            _mem.SetLength(0);
            _mem.Position = 0;
        }

        private void FinishUpload()
        {
            //there is a chance that we never uploaded a part at all because we fell under the chunk size
            if (_uploadId == null)
            {
                _mem.Position = 0;
                _s3.PutObjectAsync(new PutObjectRequest() { BucketName = _bucketName, InputStream = _mem, Key = _key }).ConfigureAwait(false).GetAwaiter().GetResult();
            }
            else // tell amazon upload complete
                _s3.CompleteMultipartUploadAsync(new CompleteMultipartUploadRequest() { BucketName = _bucketName, Key = _key, PartETags = _etags, UploadId = _uploadId }).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public override void Close()
        {
            if (_closed) return;

            //close all but the mem stream to flush it all out
            _buffered1.Flush();
            _gzip.Flush();
            _buffered2.Flush();

            //if the memory contents are up to chunk size, upload a part
            if (_mem.Length > _chunkSize)
                UploadPart();

            FinishUpload();

            _noflush1.Close();
            _buffered1.Close();
            _gzip.Close();
            _noflush2.Close();
            _buffered2.Close();
            _mem.Close();
            base.Close();

            _closed = true;
        }

        protected override void Dispose(bool disposing)
        {
            _noflush1.Dispose();
            _buffered1.Dispose();
            _gzip.Dispose();
            _noflush2.Dispose();
            _buffered2.Dispose();
            _mem.Dispose();
            base.Dispose(disposing);
        }

        private class NoFlushStream : Stream
        {
            private Stream _inner;
            public NoFlushStream(Stream inner) => _inner = inner;
            public override bool CanRead => _inner.CanRead;
            public override bool CanSeek => _inner.CanSeek;
            public override bool CanWrite => _inner.CanWrite;
            public override long Length => throw new NotSupportedException();
            public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
            public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
            public override void SetLength(long value) => throw new NotSupportedException();
            public override void Flush() { }
            public override int Read(byte[] buffer, int offset, int count) => _inner.Read(buffer, offset, count);
            public override void Write(byte[] buffer, int offset, int count) => _inner.Write(buffer, offset, count);
            public override void Close() { _inner.Close(); base.Close(); }
        }
    }
}
