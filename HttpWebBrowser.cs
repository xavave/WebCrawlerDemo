using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WebCrawlerDemo
{
    /// <summary>
    /// Implémentation HTTP réelle du navigateur utilisant HttpClient
    /// Itération 3: Support des URLs HTTP/HTTPS et domaines multiples
    /// Itération 4: Rate limiting et politiques de politesse
    /// </summary>
    public class HttpWebBrowser : IWebBrowser
    {
        private readonly HttpClient _httpClient;
        private readonly string _userAgent;
        private readonly int _delayBetweenRequestsMs;
        private readonly Dictionary<string, DateTime> _lastRequestPerDomain;
        private readonly object _lockObject = new object();

        /// <summary>
        /// Constructeur avec support du rate limiting
        /// </summary>
        /// <param name="userAgent">User-Agent à utiliser pour les requêtes</param>
        /// <param name="delayBetweenRequestsMs">Délai minimum en millisecondes entre deux requêtes vers le même domaine (défaut: 1000ms)</param>
        public HttpWebBrowser(string userAgent = "WebCrawlerDemo/1.0", int delayBetweenRequestsMs = 1000)
        {
            _userAgent = userAgent;
            _delayBetweenRequestsMs = Math.Max(0, delayBetweenRequestsMs);
            _lastRequestPerDomain = new Dictionary<string, DateTime>();
            
            // Configuration HttpClient avec redirections automatiques
            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = true,
                MaxAutomaticRedirections = 5
            };

            _httpClient = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
            
            _httpClient.DefaultRequestHeaders.Add("User-Agent", _userAgent);
        }

        public string? GetHtml(string url)
        {
            try
            {
                // Appliquer le rate limiting avant la requête
                ApplyRateLimiting(url);
                
                // Convertir l'appel asynchrone en synchrone pour l'interface existante
                return GetHtmlAsync(url).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération de {url}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Applique la politique de rate limiting basée sur le domaine
        /// Respecte un délai minimum entre deux requêtes vers le même domaine
        /// </summary>
        private void ApplyRateLimiting(string url)
        {
            if (_delayBetweenRequestsMs <= 0)
                return;

            try
            {
                if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? uri))
                    return;

                string domain = uri.Host.ToLowerInvariant();

                lock (_lockObject)
                {
                    if (_lastRequestPerDomain.TryGetValue(domain, out DateTime lastRequest))
                    {
                        // Calculer le temps écoulé depuis la dernière requête
                        var timeSinceLastRequest = DateTime.UtcNow - lastRequest;
                        var remainingDelay = _delayBetweenRequestsMs - (int)timeSinceLastRequest.TotalMilliseconds;

                        if (remainingDelay > 0)
                        {
                            // Attendre le délai restant
                            Console.WriteLine($"Rate limiting: attente de {remainingDelay}ms pour {domain}");
                            Thread.Sleep(remainingDelay);
                        }
                    }

                    // Mettre à jour le timestamp de la dernière requête
                    _lastRequestPerDomain[domain] = DateTime.UtcNow;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'application du rate limiting pour {url}: {ex.Message}");
            }
        }

        private async Task<string?> GetHtmlAsync(string url)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);
                
                // Vérifier le statut HTTP
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Erreur HTTP {(int)response.StatusCode} pour {url}");
                    return null;
                }

                // Vérifier le Content-Type (on veut du HTML/XML)
                var contentType = response.Content.Headers.ContentType?.MediaType?.ToLower();
                if (contentType != null && 
                    !contentType.Contains("html") && 
                    !contentType.Contains("xml"))
                {
                    Console.WriteLine($"Type de contenu non supporté pour {url}: {contentType}");
                    return null;
                }

                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Erreur réseau pour {url}: {ex.Message}");
                return null;
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine($"Timeout pour {url}");
                return null;
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}