using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebCrawlerDemo
{
    /// <summary>
    /// Implémentation HTTP réelle du navigateur utilisant HttpClient
    /// Itération 3: Support des URLs HTTP/HTTPS et domaines multiples
    /// </summary>
    public class HttpWebBrowser : IWebBrowser
    {
        private readonly HttpClient _httpClient;
        private readonly string _userAgent;

        public HttpWebBrowser(string userAgent = "WebCrawlerDemo/1.0")
        {
            _userAgent = userAgent;
            
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
                // Convertir l'appel asynchrone en synchrone pour l'interface existante
                return GetHtmlAsync(url).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération de {url}: {ex.Message}");
                return null;
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