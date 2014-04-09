using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Edeems.Common.Extensions;
using System.Net.Mail;
using System.IO;

namespace Edeems.Common.Templating
{

    public static class MailMessageFactory
    {

        public static MailMessage CreateMessage(string to, string from, string inContents, IDictionary<string, string> replacementDictionary)
        {
            //do the replacements
            foreach (var item in replacementDictionary)
            {
                inContents = inContents.Replace('|' + item.Key + '|', item.Value);
            }

            //yes, I know this isn't efficient.  could be MUCH better
            var contents = inContents.Split(new string[] { "SUBJECT:" }, StringSplitOptions.None).Last().SplitLines();
            string subject = contents.First();
            string body = contents.Skip(1).Aggregate((a, b) => a + Environment.NewLine + b);

            //create and return the message
            MailMessage m = new MailMessage(from, to);
            m.Subject = subject;
            m.Body = body;

            return m;
        }

    }

}
