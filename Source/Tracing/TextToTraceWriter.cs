using System.Text;
using System.IO;

namespace Torian.Common.Tracing
{

    public class TextToTraceWriter : TextWriter
    {

        private string SwitchName { get; set; }

        public TextToTraceWriter(string switchName)
        {
            SwitchName = switchName;
        }

        public override Encoding Encoding
        {
            get { return Encoding.Default; }
        }

        public override void WriteLine(string value)
        {
            Trace.TraceInformation(SwitchName, value);
        }

        public override void Write(string value)
        {
            Trace.TraceInformation(SwitchName, value);
        }

    }

}
