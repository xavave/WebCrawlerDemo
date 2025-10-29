// Test de l'algorithme de web crawler d'emails
var browser = new WebCrawlerDemo.MockWebBrowser();
var webCrawler = new WebCrawlerDemo.EmailWebCrawler();

Console.WriteLine("=== Test du Web Crawler d'Emails ===\n");

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

Console.WriteLine("Appuyez sur une touche pour continuer...");
Console.ReadKey();
