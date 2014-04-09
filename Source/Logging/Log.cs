using System;
using System.Transactions;
using System.IO;

namespace Torian.Common.Logging
{

    public static class Log
    {

        public static void LogMessage(string category, LogLevel level, string message)
        {
            try
            {
                using (new TransactionScope(TransactionScopeOption.Suppress, new TimeSpan(0, 0, 3))) using (var ctx = new LogDataClassesDataContext())
                {
                    var l = new LogEntry
                    {
                        Category = category,
                        LogLevel = (short)level,
                        Text = message,
                        CreatedDate = DateTime.Now
                    };
                    ctx.LogEntries.InsertOnSubmit(l);
                    ctx.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                //we should never hard error!
                try
                {
                    File.AppendAllText("c:\\edeems_log_error.txt", "oops: " + ex + Environment.NewLine + Environment.NewLine);
                }
                catch (Exception)
                {
                    //well shit
                }
            }
        }
    }

    public enum LogLevel
    {
        Info,
        Warn,
        Error
    }

}
