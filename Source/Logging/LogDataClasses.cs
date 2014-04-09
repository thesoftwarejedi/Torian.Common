using Torian.Common.Tracing;
using System;

namespace Torian.Common.Logging
{
    partial class LogDataClassesDataContext
    {

        partial void OnCreated()
        {
            Log = new TextToTraceWriter("Linq");
        }


    }

}
