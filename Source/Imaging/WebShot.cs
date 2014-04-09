using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.Threading;

namespace Torian.Common.Imaging
{

    public class WebShot : IDisposable
    {

        private WebsitesScreenshot.WebsitesScreenshot wb;

        public WebShot()
        {
            wb = new WebsitesScreenshot.WebsitesScreenshot("Q8M810AR9FR41P0LUWAK331CU");
            //wb.NoActiveX = true;
            //wb.NoJava = true;
            //wb.NoScripts = true;
        }

        public Bitmap GenerateScreenshot(string url)
        {
            // This method gets a screenshot of the webpage
            // rendered at its full size (height and width)
            return GenerateScreenshot(url, -1, -1, 30);
        }

        public Bitmap GenerateScreenshot(string url, int width, int height, int timeoutSeconds)
        {
            if (width != -1)
            {
                wb.BrowserWidth = width;
            }

            if (height != -1)
            {
                wb.BrowserHeight = height;
            }

            wb.TimeOutSeconds = timeoutSeconds;

            Stopwatch sw = new Stopwatch();
            sw.Start();
            // Load the webpage into a WebBrowser control
            var result = wb.CaptureWebpage(url);
            if (result != WebsitesScreenshot.WebsitesScreenshot.Result.Captured)
            {
                throw new Exception("Website couldn't be snapped (" + Enum.GetName(typeof(WebsitesScreenshot.WebsitesScreenshot.Result), result) + ")");
            }

            // Get a Bitmap representation of the webpage as it's rendered in the WebBrowser control
            Bitmap bitmap = wb.GetImage();

            Torian.Common.Tracing.Trace.TraceInformation("Torian.Common.Imaging", "Captured {0} in {1}", url, sw.ElapsedMilliseconds);

            return bitmap;
        }


        #region IDisposable Members

        public void Dispose()
        {
            wb.Dispose();
        }

        #endregion
    }

}
