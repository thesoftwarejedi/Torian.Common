using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace Torian.Common.IO
{

    public class FileInfoWrapper
    {
        private FileInfo TheFileInfo
        {
            get; set;
        }

        private byte[] _checksum;
        public byte[] CheckSum
        {
            get { return _checksum ?? (_checksum = MD5.Create().ComputeHash(TheFileInfo.OpenRead())); }
        }

        FileInfoWrapper(FileInfo fi)
        {
            TheFileInfo = fi;
        }

    }

}
