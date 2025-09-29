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
            // Using a site designed for automation avoids issues like CAPTCHAs.
            await Page.GotoAsync("https://playwright.dev/");
            
            // Handle the cookie consent banner that might overlay the search button.
            // This step is now conditional to prevent timeouts if the banner doesn't appear.
            var cookieBannerButton = Page.GetByRole(AriaRole.Button, new() { Name = "Switch to light mode" });
            if (await cookieBannerButton.IsVisibleAsync())
            {
                await cookieBannerButton.ClickAsync();
            }
            
            // Click the search button
            await Page.GetByRole(AriaRole.Button, new() { Name = "Search" }).ClickAsync();
            
            // Fill the search input that appears in the modal
            await Page.Locator("input.DocSearch-Input").FillAsync("video recording");
            
            // Wait for search results to appear
            await Page.WaitForSelectorAsync(".DocSearch-Hit a");
            
            // Assert that the first result is visible and contains the expected text
            NUnit.Framework.Assert.That(await Page.Locator(".DocSearch-Hit a").First.TextContentAsync(), Does.Contain("testOptions.video"));
            
            // NOTE: The video is saved *automatically* when the BrowserContext is closed 
            // after the test completes, due to inheriting from PageTest.
            // You don't need a Page.Video.SaveAsAsync() call unless you create the context manually.
        }
    }
}