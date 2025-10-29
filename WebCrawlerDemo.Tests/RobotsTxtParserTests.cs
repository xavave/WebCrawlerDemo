using Xunit;
using WebCrawlerDemo;

namespace WebCrawlerDemo.Tests
{
    public class RobotsTxtParserTests
    {
        [Fact]
        public void Parse_EmptyContent_ShouldAllowEverything()
        {
            // Arrange
            var parser = new RobotsTxtParser();
            
            // Act
            parser.Parse("");
            
            // Assert
            Assert.True(parser.IsAllowed("https://example.com/page", "*"));
        }

        [Fact]
        public void Parse_DisallowAll_ShouldBlockEverything()
        {
            // Arrange
            var parser = new RobotsTxtParser();
            var robotsTxt = @"
User-agent: *
Disallow: /
";
            
            // Act
            parser.Parse(robotsTxt);
            
            // Assert
            Assert.False(parser.IsAllowed("https://example.com/page", "*"));
            Assert.False(parser.IsAllowed("https://example.com/admin", "*"));
        }

        [Fact]
        public void Parse_DisallowSpecificPath_ShouldBlockOnlyThatPath()
        {
            // Arrange
            var parser = new RobotsTxtParser();
            var robotsTxt = @"
User-agent: *
Disallow: /admin/
";
            
            // Act
            parser.Parse(robotsTxt);
            
            // Assert
            Assert.True(parser.IsAllowed("https://example.com/", "*"));
            Assert.True(parser.IsAllowed("https://example.com/public", "*"));
            Assert.False(parser.IsAllowed("https://example.com/admin/settings", "*"));
        }

        [Fact]
        public void Parse_AllowAndDisallow_MostSpecificRuleWins()
        {
            // Arrange
            var parser = new RobotsTxtParser();
            var robotsTxt = @"
User-agent: *
Disallow: /admin/
Allow: /admin/public/
";
            
            // Act
            parser.Parse(robotsTxt);
            
            // Assert
            Assert.False(parser.IsAllowed("https://example.com/admin/private", "*"));
            Assert.True(parser.IsAllowed("https://example.com/admin/public/page", "*"));
        }

        [Fact]
        public void Parse_CrawlDelay_ShouldReturnCorrectValue()
        {
            // Arrange
            var parser = new RobotsTxtParser();
            var robotsTxt = @"
User-agent: *
Crawl-delay: 5
";
            
            // Act
            parser.Parse(robotsTxt);
            
            // Assert
            Assert.Equal(5000, parser.GetCrawlDelay()); // 5 secondes = 5000ms
        }

        [Fact]
        public void Parse_Sitemap_ShouldReturnSitemapUrls()
        {
            // Arrange
            var parser = new RobotsTxtParser();
            var robotsTxt = @"
User-agent: *
Sitemap: https://example.com/sitemap.xml
Sitemap: https://example.com/sitemap2.xml
";
            
            // Act
            parser.Parse(robotsTxt);
            
            // Assert
            var sitemaps = parser.GetSitemaps();
            Assert.Equal(2, sitemaps.Count);
            Assert.Contains("https://example.com/sitemap.xml", sitemaps);
            Assert.Contains("https://example.com/sitemap2.xml", sitemaps);
        }

        [Fact]
        public void Parse_Comments_ShouldBeIgnored()
        {
            // Arrange
            var parser = new RobotsTxtParser();
            var robotsTxt = @"
# This is a comment
User-agent: *
Disallow: /admin/ # Block admin
# Another comment
";
            
            // Act
            parser.Parse(robotsTxt);
            
            // Assert
            Assert.False(parser.IsAllowed("https://example.com/admin/page", "*"));
        }

        [Fact]
        public void Parse_WildcardPattern_ShouldMatchCorrectly()
        {
            // Arrange
            var parser = new RobotsTxtParser();
            var robotsTxt = @"
User-agent: *
Disallow: /*.pdf$
";
            
            // Act
            parser.Parse(robotsTxt);
            
            // Assert
            Assert.True(parser.IsAllowed("https://example.com/page.html", "*"));
            Assert.False(parser.IsAllowed("https://example.com/document.pdf", "*"));
        }

        [Fact]
        public void Parse_MultipleUserAgents_ShouldApplyCorrectRules()
        {
            // Arrange
            var parser = new RobotsTxtParser();
            var robotsTxt = @"
User-agent: *
Disallow: /admin/

User-agent: WebCrawlerDemo
Allow: /
";
            
            // Act
            parser.Parse(robotsTxt);
            
            // Assert
            Assert.False(parser.IsAllowed("https://example.com/admin/page", "*"));
            Assert.True(parser.IsAllowed("https://example.com/admin/page", "WebCrawlerDemo"));
        }
    }
}
