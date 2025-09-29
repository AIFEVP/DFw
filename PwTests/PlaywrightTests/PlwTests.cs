using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System.IO;
using System.Threading.Tasks;

namespace PlaywrightVideoExample
{
    // Inherit from PageTest to get automatic Page/Browser/Context setup and cleanup.
    public class VideoRecordingTest : PageTest
    {
        // 1. Override ContextOptions to enable video recording for all tests in this class.
        public override BrowserNewContextOptions ContextOptions()
        {
            return new BrowserNewContextOptions
            {
                // Specify the directory where videos should be saved. 
                // Playwright will create a unique file name inside this directory.
                RecordVideoDir = "videos/", 
                
                // (Optional) Set the video resolution. Default is based on viewport.
                RecordVideoSize = new RecordVideoSize { Width = 1280, Height = 720 },
                
                // (Optional) Set the initial viewport size for the page
                ViewportSize = new ViewportSize { Width = 1280, Height = 720 }
            };
        }

        [Test]
        public async Task TestGoogleSearchAndRecordVideo()
        {
            // The 'Page' object is automatically available via inheritance from PageTest

            // 2. Perform test actions
            await Page.GotoAsync("https://www.google.com");
            await Page.FillAsync("[aria-label=\"Search\"]", "Playwright .NET video recording");
            await Page.PressAsync("[aria-label=\"Search\"]", "Enter");
            
            // Wait for search results
            await Page.WaitForSelectorAsync("#search");

            // Assert something to prove the test works
            NUnit.Framework.Assert.That(await Page.TitleAsync(), Does.Contain("Playwright .NET video recording - Google Search"));
            
            // NOTE: The video is saved *automatically* when the BrowserContext is closed 
            // after the test completes, due to inheriting from PageTest.
            // You don't need a Page.Video.SaveAsAsync() call unless you create the context manually.
        }
    }
}