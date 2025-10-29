using Xunit;
using WebCrawlerDemo;

namespace WebCrawlerDemo.Tests
{
    public class CrawlerPoliciesTests
    {
        [Fact]
        public void Default_ShouldHaveReasonableDefaults()
        {
            // Act
            var policies = new CrawlerPolicies();
            
            // Assert
            Assert.Equal(1000, policies.DelayBetweenRequestsMs);
            Assert.Equal(-1, policies.MaxPagesPerDomain);
            Assert.Equal(30, policies.RequestTimeoutSeconds);
            Assert.True(policies.RespectRobotsTxt);
            Assert.NotEmpty(policies.UserAgent);
        }

        [Fact]
        public void Conservative_ShouldHaveConservativeSettings()
        {
            // Act
            var policies = CrawlerPolicies.Conservative;
            
            // Assert
            Assert.Equal(2000, policies.DelayBetweenRequestsMs);
            Assert.Equal(100, policies.MaxPagesPerDomain);
            Assert.Equal(20, policies.RequestTimeoutSeconds);
            Assert.True(policies.RespectRobotsTxt);
            Assert.Contains("Conservative", policies.UserAgent);
        }

        [Fact]
        public void Aggressive_ShouldHaveAggressiveSettings()
        {
            // Act
            var policies = CrawlerPolicies.Aggressive;
            
            // Assert
            Assert.Equal(100, policies.DelayBetweenRequestsMs);
            Assert.Equal(-1, policies.MaxPagesPerDomain);
            Assert.Equal(10, policies.RequestTimeoutSeconds);
            Assert.False(policies.RespectRobotsTxt);
            Assert.Contains("Fast", policies.UserAgent);
        }

        [Fact]
        public void CustomPolicies_ShouldBeConfigurable()
        {
            // Arrange & Act
            var policies = new CrawlerPolicies
            {
                DelayBetweenRequestsMs = 500,
                MaxPagesPerDomain = 50,
                RequestTimeoutSeconds = 15,
                UserAgent = "CustomBot/1.0",
                RespectRobotsTxt = false
            };
            
            // Assert
            Assert.Equal(500, policies.DelayBetweenRequestsMs);
            Assert.Equal(50, policies.MaxPagesPerDomain);
            Assert.Equal(15, policies.RequestTimeoutSeconds);
            Assert.Equal("CustomBot/1.0", policies.UserAgent);
            Assert.False(policies.RespectRobotsTxt);
        }

        [Fact]
        public void UserAgent_ShouldContainProjectLink()
        {
            // Act
            var policies = new CrawlerPolicies();
            
            // Assert
            Assert.Contains("github.com", policies.UserAgent.ToLower());
        }
    }
}
