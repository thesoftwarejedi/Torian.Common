using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.Specialized;
using Torian.Common.Extensions;

namespace Torian.Common.Templating
{

    public class EmailTemplate
    {

        public string BodyHtml { get; set; }
        public string BodyPlain { get; set; }

        public string ReplacementPrefix { get; set; }
        public string ReplacementSuffix { get; set; }

        public ListDictionary ReplacementDictionary { get; set; }

        public EmailTemplate(string htmlFile, string plainFile)
        {
            ReplacementDictionary = new ListDictionary();

            ReplacementPrefix = "{";
            ReplacementSuffix = "}";

            BodyHtml = "";
            if (htmlFile != null)
            {
                BodyHtml = File.ReadAllText(htmlFile);
            }

            BodyPlain = "";
            if (plainFile != null)
            {
                BodyPlain = File.ReadAllText(plainFile);
            }
        }

        public void ApplyReplacement()
        {
            StringBuilder sbHtml = new StringBuilder(BodyHtml);
            StringBuilder sbPlain = new StringBuilder(BodyPlain);
            foreach (var key in ReplacementDictionary.Keys)
            {
                string repl = ReplacementPrefix + key + ReplacementSuffix;
                sbHtml.Replace(repl, ReplacementDictionary[key].ToStringNullSafe() ?? "");
                sbPlain.Replace(repl, ReplacementDictionary[key].ToStringNullSafe() ?? "");
            }
            BodyHtml = sbHtml.ToStringNullSafe();
            BodyPlain = sbPlain.ToStringNullSafe();
        }

    }

}
