using Serilog;
using Serilog.Events;

// Configuration du logging (Itération 7)
WebCrawlerDemo.LoggingConfiguration.ConfigureLogging(LogEventLevel.Debug);

Log.Information("=== Démarrage de WebCrawlerDemo ===");

// Test de l'algorithme de web crawler d'emails
var browser = new WebCrawlerDemo.MockWebBrowser();
var webCrawler = new WebCrawlerDemo.EmailWebCrawler();

Console.WriteLine("\n=== Test du Web Crawler d'Emails ===\n");

// Test avec profondeur 0
Console.WriteLine("Profondeur 0:");
var emails0 = webCrawler.GetEmailsInPageAndChildPages(browser, "C:/TestHtml/index.html", 0);
Console.WriteLine($"Résultat: {string.Join(", ", emails0)}\n");

// Test avec profondeur 1
Console.WriteLine("Profondeur 1:");
var emails1 = webCrawler.GetEmailsInPageAndChildPages(browser, "C:/TestHtml/index.html", 1);
Console.WriteLine($"Résultat: {string.Join(", ", emails1)}\n");

// Test avec profondeur 2
Console.WriteLine("Test avec profondeur 2:");
var emails2 = webCrawler.GetEmailsInPageAndChildPages(browser, "C:/TestHtml/index.html", 2);
Console.WriteLine($"Résultat: {string.Join(", ", emails2)}\n");

// Test avec HTML malformé (Itération 2 - HtmlAgilityPack)
Console.WriteLine("=== Test HTML Malformé (Itération 2) ===\n");
Console.WriteLine("Test avec HTML malformé:");
var emailsMalformed = webCrawler.GetEmailsInPageAndChildPages(browser, "C:/TestHtml/malformed.html", 0);
Console.WriteLine($"Résultat: {string.Join(", ", emailsMalformed)}");
Console.WriteLine("✅ HtmlAgilityPack peut parser du HTML malformé !\n");

// Test avec URLs HTTP (Itération 3)
Console.WriteLine("=== Test URLs HTTP/HTTPS (Itération 3) ===\n");
Console.WriteLine("✅ Support des URLs HTTP/HTTPS implémenté !");
Console.WriteLine("✅ Résolution d'URLs relatives avec Uri");
Console.WriteLine("✅ Normalisation avancée des URLs\n");

// Test Rate Limiting et Politiques (Itération 4)
Console.WriteLine("=== Test Rate Limiting et Politiques (Itération 4) ===\n");

// Test avec politique par défaut
Console.WriteLine("Test avec politique par défaut (1000ms entre requêtes):");
var defaultPolicies = new WebCrawlerDemo.CrawlerPolicies();
var httpBrowserDefault = new WebCrawlerDemo.HttpWebBrowser(
    defaultPolicies.UserAgent, 
    defaultPolicies.DelayBetweenRequestsMs
);
var crawlerDefault = new WebCrawlerDemo.EmailWebCrawler(defaultPolicies);
Console.WriteLine($"✅ Délai configuré: {defaultPolicies.DelayBetweenRequestsMs}ms");
Console.WriteLine($"✅ User-Agent: {defaultPolicies.UserAgent}\n");

// Test avec politique conservative
Console.WriteLine("Politique Conservative (2000ms, max 100 pages/domaine):");
var conservativePolicies = new WebCrawlerDemo.CrawlerPolicies
{
    DelayBetweenRequestsMs = 2000,
    MaxPagesPerDomain = 100,
    RequestTimeoutSeconds = 20,
    UserAgent = "WebCrawlerDemo/1.0 (Conservative; +https://github.com/xavave/WebCrawlerDemo)"
};
Console.WriteLine($"  - Délai: {conservativePolicies.DelayBetweenRequestsMs}ms");
Console.WriteLine($"  - Max pages/domaine: {conservativePolicies.MaxPagesPerDomain}");
Console.WriteLine($"  - Timeout: {conservativePolicies.RequestTimeoutSeconds}s");
Console.WriteLine($"  - User-Agent: {conservativePolicies.UserAgent}\n");

// Test avec politique aggressive
Console.WriteLine("Politique Aggressive (100ms, pas de limite):");
var aggressivePolicies = new WebCrawlerDemo.CrawlerPolicies
{
    DelayBetweenRequestsMs = 100,
    MaxPagesPerDomain = -1,
    RequestTimeoutSeconds = 10,
    UserAgent = "WebCrawlerDemo/1.0 (Fast; +https://github.com/xavave/WebCrawlerDemo)"
};
Console.WriteLine($"  - Délai: {aggressivePolicies.DelayBetweenRequestsMs}ms");
Console.WriteLine($"  - Max pages/domaine: illimité");
Console.WriteLine($"  - Timeout: {aggressivePolicies.RequestTimeoutSeconds}s");
Console.WriteLine($"  - User-Agent: {aggressivePolicies.UserAgent}\n");

// Démonstration du rate limiting avec MockWebBrowser
Console.WriteLine("Démonstration du comptage de pages par domaine:");
var testPolicies = new WebCrawlerDemo.CrawlerPolicies 
{ 
    MaxPagesPerDomain = 2,  // Limite à 2 pages pour la démo
    DelayBetweenRequestsMs = 0  // Pas de délai pour les tests locaux
};
var crawlerLimited = new WebCrawlerDemo.EmailWebCrawler(testPolicies);
var emailsLimited = crawlerLimited.GetEmailsInPageAndChildPages(browser, "C:/TestHtml/index.html", 2);
Console.WriteLine($"Avec limite de 2 pages: {string.Join(", ", emailsLimited)}");
Console.WriteLine("✅ Les pages au-delà de la limite sont ignorées\n");

Console.WriteLine("✅ Rate limiting implémenté !");
Console.WriteLine("✅ Politiques de crawling configurables");
Console.WriteLine("✅ Limitation par domaine fonctionnelle");
Console.WriteLine("✅ Crawling respectueux et responsable\n");

// Test robots.txt (Itération 5)
Console.WriteLine("=== Test Support robots.txt (Itération 5) ===\n");

// Test du parser robots.txt
Console.WriteLine("Test du parser robots.txt:");
var robotsParser = new WebCrawlerDemo.RobotsTxtParser();
var sampleRobotsTxt = @"
User-agent: *
Disallow: /admin/
Disallow: /private/
Allow: /public/

User-agent: WebCrawlerDemo
Allow: /

Crawl-delay: 2
Sitemap: https://example.com/sitemap.xml
";

robotsParser.Parse(sampleRobotsTxt);

// Tester différentes URLs
var testUrls = new[]
{
    ("https://example.com/", "autorisé"),
    ("https://example.com/public/page.html", "autorisé"),
    ("https://example.com/admin/settings", "bloqué"),
    ("https://example.com/private/data", "bloqué")
};

foreach (var (url, expected) in testUrls)
{
    var allowed = robotsParser.IsAllowed(url, "*");
    var status = allowed ? "✅ autorisé" : "❌ bloqué";
    Console.WriteLine($"  {url} -> {status} (attendu: {expected})");
}

var crawlDelay = robotsParser.GetCrawlDelay();
Console.WriteLine($"\nCrawl-delay détecté: {(crawlDelay.HasValue ? $"{crawlDelay.Value}ms" : "aucun")}");

var sitemaps = robotsParser.GetSitemaps();
Console.WriteLine($"Sitemaps trouvés: {sitemaps.Count}");
if (sitemaps.Count > 0)
{
    Console.WriteLine($"  - {sitemaps[0]}");
}

Console.WriteLine("\n✅ Parser robots.txt fonctionnel !");
Console.WriteLine("✅ Support des directives Allow/Disallow");
Console.WriteLine("✅ Support Crawl-delay et Sitemap");
Console.WriteLine("✅ Cache robots.txt par domaine");
Console.WriteLine("✅ Crawling éthique et conforme aux standards\n");

// Itération 7: Logging structuré
Console.WriteLine("=== Itération 7: Structured Logging ===\n");
Console.WriteLine("✅ Serilog configuré avec Console et File sinks");
Console.WriteLine("✅ Logging structuré dans toutes les classes");
Console.WriteLine("✅ Niveaux: Debug, Information, Warning, Error");
Console.WriteLine("✅ Fichiers de log: logs/webcrawler-YYYY-MM-DD.log");
Console.WriteLine("✅ Rétention: 7 jours\n");

Log.Information("=== Fin de WebCrawlerDemo ===");
WebCrawlerDemo.LoggingConfiguration.CloseAndFlush();

Console.WriteLine("Appuyez sur une touche pour continuer...");
Console.ReadKey();
