using Xunit;
using WebCrawlerDemo;
using System.Collections.Generic;

namespace WebCrawlerDemo.Tests
{
    public class EmailWebCrawlerTests
    {
        [Fact]
        public void GetEmailsInPageAndChildPages_Depth0_ShouldReturnOnlyRootPageEmails()
        {
            // Arrange
            var browser = new MockWebBrowser();
            var crawler = new EmailWebCrawler();
            
            // Act
            var emails = crawler.GetEmailsInPageAndChildPages(browser, "C:/TestHtml/index.html", 0);
            
            // Assert
            Assert.Single(emails);
            Assert.Contains("nullepart@mozilla.org", emails);
        }

        [Fact]
        public void GetEmailsInPageAndChildPages_Depth1_ShouldReturnRootAndFirstLevelEmails()
        {
            // Arrange
            var browser = new MockWebBrowser();
            var crawler = new EmailWebCrawler();
            
            // Act
            var emails = crawler.GetEmailsInPageAndChildPages(browser, "C:/TestHtml/index.html", 1);
            
            // Assert
            Assert.Equal(2, emails.Count);
            Assert.Contains("nullepart@mozilla.org", emails);
            Assert.Contains("ailleurs@mozilla.org", emails);
        }

        [Fact]
        public void GetEmailsInPageAndChildPages_Depth2_ShouldReturnAllEmails()
        {
            // Arrange
            var browser = new MockWebBrowser();
            var crawler = new EmailWebCrawler();
            
            // Act
            var emails = crawler.GetEmailsInPageAndChildPages(browser, "C:/TestHtml/index.html", 2);
            
            // Assert
            Assert.Equal(3, emails.Count);
            Assert.Contains("nullepart@mozilla.org", emails);
            Assert.Contains("ailleurs@mozilla.org", emails);
            Assert.Contains("loin@mozilla.org", emails);
        }

        [Fact]
        public void GetEmailsInPageAndChildPages_MalformedHtml_ShouldParseSuccessfully()
        {
            // Arrange
            var browser = new MockWebBrowser();
            var crawler = new EmailWebCrawler();
            
            // Act
            var emails = crawler.GetEmailsInPageAndChildPages(browser, "C:/TestHtml/malformed.html", 0);
            
            // Assert
            Assert.Equal(2, emails.Count);
            Assert.Contains("test@malformed.org", emails);
            Assert.Contains("another@test.com", emails);
        }

        [Fact]
        public void GetEmailsInPageAndChildPages_WithMaxPagesPerDomain_ShouldRespectLimit()
        {
            // Arrange
            var browser = new MockWebBrowser();
            var policies = new CrawlerPolicies 
            { 
                MaxPagesPerDomain = 1,
                DelayBetweenRequestsMs = 0
            };
            var crawler = new EmailWebCrawler(policies);
            
            // Act
            var emails = crawler.GetEmailsInPageAndChildPages(browser, "C:/TestHtml/index.html", 2);
            
            // Assert
            // Devrait s arrêter après la première page
            Assert.Single(emails);
        }

        [Fact]
        public void GetEmailsInPageAndChildPages_NullBrowser_ShouldThrowArgumentNullException()
        {
            // Arrange
            var crawler = new EmailWebCrawler();
            
            // Act & Assert
            Assert.Throws<System.ArgumentNullException>(() => 
                crawler.GetEmailsInPageAndChildPages(null, "C:/TestHtml/index.html", 0));
        }

        [Fact]
        public void GetEmailsInPageAndChildPages_EmptyUrl_ShouldThrowArgumentException()
        {
            // Arrange
            var browser = new MockWebBrowser();
            var crawler = new EmailWebCrawler();
            
            // Act & Assert
            Assert.Throws<System.ArgumentException>(() => 
                crawler.GetEmailsInPageAndChildPages(browser, "", 0));
        }

        [Fact]
        public void GetEmailsInPageAndChildPages_ShouldReturnDistinctEmails()
        {
            // Arrange
            var browser = new TestBrowserWithDuplicates();
            var crawler = new EmailWebCrawler();
            
            // Act
            var emails = crawler.GetEmailsInPageAndChildPages(browser, "C:/test.html", 0);
            
            // Assert
            Assert.Single(emails);
            Assert.Contains("duplicate@test.com", emails);
        }

        [Fact]
        public void GetEmailsInPageAndChildPages_ShouldNormalizeEmailsToLowercase()
        {
            // Arrange
            var browser = new TestBrowserWithMixedCase();
            var crawler = new EmailWebCrawler();
            
            // Act
            var emails = crawler.GetEmailsInPageAndChildPages(browser, "C:/test.html", 0);
            
            // Assert
            Assert.Single(emails);
            Assert.Contains("test@example.com", emails);
        }
    }

    // Helper classes pour les tests
    internal class TestBrowserWithDuplicates : IWebBrowser
    {
        public string GetHtml(string url)
        {
            return @"
                <html>
                    <body>
                        <a href='mailto:duplicate@test.com'>Email 1</a>
                        <a href='mailto:duplicate@test.com'>Email 2</a>
                        <a href='mailto:DUPLICATE@test.com'>Email 3</a>
                    </body>
                </html>";
        }
    }

    internal class TestBrowserWithMixedCase : IWebBrowser
    {
        public string GetHtml(string url)
        {
            return @"
                <html>
                    <body>
                        <a href='mailto:Test@Example.COM'>Email</a>
                    </body>
                </html>";
        }
    }
}
