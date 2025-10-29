using System;
using System.Collections.Generic;

namespace WebCrawlerDemo
{
    /// <summary>
    /// Cache pour les fichiers robots.txt par domaine
    /// Itération 5: Support de robots.txt
    /// </summary>
    public class RobotsTxtCache
    {
        private readonly Dictionary<string, CachedRobotsTxt> _cache;
        private readonly IWebBrowser _browser;
        private readonly object _lockObject = new object();
        private readonly TimeSpan _cacheDuration;

        public RobotsTxtCache(IWebBrowser browser, TimeSpan? cacheDuration = null)
        {
            _browser = browser ?? throw new ArgumentNullException(nameof(browser));
            _cache = new Dictionary<string, CachedRobotsTxt>();
            _cacheDuration = cacheDuration ?? TimeSpan.FromHours(24);
        }

        /// <summary>
        /// Récupère le parser robots.txt pour un domaine donné
        /// </summary>
        public RobotsTxtParser GetRobotsTxt(string url)
        {
            if (string.IsNullOrEmpty(url))
                return new RobotsTxtParser();

            try
            {
                var uri = new Uri(url);
                var domain = $"{uri.Scheme}://{uri.Host}";
                var robotsUrl = $"{domain}/robots.txt";

                lock (_lockObject)
                {
                    // Vérifier si on a déjà le robots.txt en cache
                    if (_cache.TryGetValue(domain, out var cached))
                    {
                        // Vérifier si le cache est encore valide
                        if (DateTime.UtcNow - cached.Timestamp < _cacheDuration)
                        {
                            return cached.Parser;
                        }
                    }

                    // Télécharger le robots.txt
                    var parser = new RobotsTxtParser();
                    try
                    {
                        var robotsTxtContent = _browser.GetHtml(robotsUrl);
                        if (!string.IsNullOrEmpty(robotsTxtContent))
                        {
                            parser.Parse(robotsTxtContent);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Impossible de récupérer robots.txt pour {domain}: {ex.Message}");
                        // En cas d erreur, on crée un parser vide (tout autorisé)
                    }

                    // Mettre en cache
                    _cache[domain] = new CachedRobotsTxt
                    {
                        Parser = parser,
                        Timestamp = DateTime.UtcNow
                    };

                    return parser;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération du robots.txt pour {url}: {ex.Message}");
                return new RobotsTxtParser();
            }
        }

        /// <summary>
        /// Vérifie si une URL peut être crawlée selon robots.txt
        /// </summary>
        public bool IsAllowed(string url, string userAgent = "*")
        {
            try
            {
                var parser = GetRobotsTxt(url);
                return parser.IsAllowed(url, userAgent);
            }
            catch
            {
                // En cas d erreur, autoriser par défaut
                return true;
            }
        }

        /// <summary>
        /// Nettoie les entrées expirées du cache
        /// </summary>
        public void CleanExpiredEntries()
        {
            lock (_lockObject)
            {
                var expiredKeys = new List<string>();
                var now = DateTime.UtcNow;

                foreach (var kvp in _cache)
                {
                    if (now - kvp.Value.Timestamp >= _cacheDuration)
                    {
                        expiredKeys.Add(kvp.Key);
                    }
                }

                foreach (var key in expiredKeys)
                {
                    _cache.Remove(key);
                }
            }
        }
    }

    internal class CachedRobotsTxt
    {
        public RobotsTxtParser Parser { get; set; } = new RobotsTxtParser();
        public DateTime Timestamp { get; set; }
    }
}
