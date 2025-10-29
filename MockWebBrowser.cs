using System.Collections.Generic;

namespace WebCrawlerDemo
{
    /// <summary>
    /// Navigateur fictif pour les tests qui simule les fichiers HTML de l'exemple
    /// </summary>
    public class MockWebBrowser : IWebBrowser
    {
        private readonly Dictionary<string, string> _htmlPages;

        public MockWebBrowser()
        {
            _htmlPages = new Dictionary<string, string>
            {
                // HTML bien formé (version originale)
                ["C:/TestHtml/index.html"] = @"<html>
                    <h1>INDEX</h1>
                    <a href=""./child1.html"">child1</a>
                    <a href=""mailto:nullepart@mozilla.org"">Envoyer l'email nulle part</a>
                </html>",

                ["C:/TestHtml/child1.html"] = @"<html>
                    <h1>CHILD1</h1>
                    <a href=""./index.html"">index</a>
                    <a href=""./child2.html"">child2</a>
                    <a href=""mailto:ailleurs@mozilla.org"">Envoyer l'email ailleurs</a>
                    <a href=""mailto:nullepart@mozilla.org"">Envoyer l'email nulle part</a>
                </html>",

                ["C:/TestHtml/child2.html"] = @"<html>
                    <h1>CHILD2</h1>
                    <a href=""./index.html"">index</a>
                    <a href=""mailto:loin@mozilla.org"">Envoyer l'email loin</a>
                    <a href=""mailto:nullepart@mozilla.org"">Envoyer l'email nulle part</a>
                </html>",

                // HTML malformé pour tester HtmlAgilityPack (Itération 2)
                ["C:/TestHtml/malformed.html"] = @"<html>
                    <h1>HTML MALFORMÉ
                    <p>Balise non fermée
                    <a href='./index.html'>lien avec guillemets simples
                    <a href=mailto:test@malformed.org>mailto sans guillemets</a>
                    <br>
                    <div>Div sans fermeture
                    <a href=""mailto:another@test.com"">Email valide</a>
                "
            };
        }

        public string GetHtml(string url)
        {
            // Normaliser l'URL pour la recherche
            string normalizedUrl = url.Replace('\\', '/');
            
            if (_htmlPages.TryGetValue(normalizedUrl, out string html))
            {
                return html;
            }

            // Retourner null si l'URL n'existe pas (comme spécifié dans l'interface)
            return null;
        }
    }
}