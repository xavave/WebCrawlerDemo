using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace WebCrawlerDemo
{
    /// <summary>
    /// Implémentation du web crawler d'emails utilisant l'algorithme BFS (Breadth-First Search)
    /// pour respecter la contrainte de priorité aux pages "les plus proches"
    /// </summary>
    public class EmailWebCrawler : ITheTest
    {
        public List<string> GetEmailsInPageAndChildPages(IWebBrowser browser, string url, int maximumDepth)
        {
            if (browser == null)
                throw new ArgumentNullException(nameof(browser));
            
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("L'URL ne peut pas être vide", nameof(url));

            var distinctEmails = new HashSet<string>();
            var visitedUrls = new HashSet<string>();
            var urlsToVisit = new Queue<(string Url, int Depth)>();

            // Normaliser l'URL de départ
            string normalizedStartUrl = NormalizeUrl(url);
            urlsToVisit.Enqueue((normalizedStartUrl, 0));

            // Algorithme BFS pour explorer les pages niveau par niveau
            while (urlsToVisit.Count > 0)
            {
                var (currentUrl, currentDepth) = urlsToVisit.Dequeue();

                // Vérifier si on a déjà visité cette URL
                if (visitedUrls.Contains(currentUrl))
                    continue;

                // Vérifier la profondeur maximale (sauf si maximumDepth = -1)
                if (maximumDepth >= 0 && currentDepth > maximumDepth)
                    continue;

                visitedUrls.Add(currentUrl);

                try
                {
                    // Récupérer le HTML de la page
                    string html = browser.GetHtml(currentUrl);
                    if (string.IsNullOrEmpty(html))
                        continue;

                    // Parser le HTML comme XML pour extraire les liens
                    var doc = XDocument.Parse($"<root>{html}</root>");

                    // Extraire tous les emails de la page actuelle
                    var emailsInPage = ExtractEmailsFromHtml(doc);
                    foreach (var email in emailsInPage)
                    {
                        distinctEmails.Add(email);
                    }

                    // Si on n'a pas atteint la profondeur maximale, ajouter les liens enfants
                    if (maximumDepth == -1 || currentDepth < maximumDepth)
                    {
                        var childUrls = ExtractChildUrls(doc, currentUrl);
                        foreach (var childUrl in childUrls)
                        {
                            string normalizedChildUrl = NormalizeUrl(childUrl);
                            if (!visitedUrls.Contains(normalizedChildUrl))
                            {
                                urlsToVisit.Enqueue((normalizedChildUrl, currentDepth + 1));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log l'erreur mais continue le traitement des autres URLs
                    Console.WriteLine($"Erreur lors du traitement de {currentUrl}: {ex.Message}");
                }
            }

            return distinctEmails.ToList();
        }

        /// <summary>
        /// Extrait tous les emails des liens mailto dans le document XML
        /// </summary>
        private HashSet<string> ExtractEmailsFromHtml(XDocument doc)
        {
            var emails = new HashSet<string>();

            // Rechercher tous les éléments <a> avec un attribut href contenant "mailto:"
            var mailtoLinks = doc.Descendants("a")
                .Where(a => a.Attribute("href") != null && 
                           a.Attribute("href").Value.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase));

            foreach (var link in mailtoLinks)
            {
                string href = link.Attribute("href").Value;
                // Extraire l'email de "mailto:email@domain.com"
                string email = href.Substring(7); // Enlever "mailto:"
                
                // Nettoyer l'email (enlever les paramètres comme ?subject=...)
                int paramIndex = email.IndexOf('?');
                if (paramIndex > 0)
                {
                    email = email.Substring(0, paramIndex);
                }

                if (IsValidEmail(email))
                {
                    emails.Add(email.ToLowerInvariant());
                }
            }

            return emails;
        }

        /// <summary>
        /// Extrait toutes les URLs enfants de la page (liens href qui ne sont pas des mailto)
        /// </summary>
        private HashSet<string> ExtractChildUrls(XDocument doc, string baseUrl)
        {
            var childUrls = new HashSet<string>();

            // Rechercher tous les éléments <a> avec un attribut href qui ne sont pas des mailto
            var links = doc.Descendants("a")
                .Where(a => a.Attribute("href") != null && 
                           !a.Attribute("href").Value.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase) &&
                           !a.Attribute("href").Value.StartsWith("http", StringComparison.OrdinalIgnoreCase));

            foreach (var link in links)
            {
                string href = link.Attribute("href").Value;
                string absoluteUrl = ResolveRelativeUrl(baseUrl, href);
                if (!string.IsNullOrEmpty(absoluteUrl))
                {
                    childUrls.Add(absoluteUrl);
                }
            }

            return childUrls;
        }

        /// <summary>
        /// Résout une URL relative par rapport à une URL de base
        /// </summary>
        private string ResolveRelativeUrl(string baseUrl, string relativeUrl)
        {
            try
            {
                // Si l'URL relative commence par "./" on l'enlève
                if (relativeUrl.StartsWith("./"))
                {
                    relativeUrl = relativeUrl.Substring(2);
                }

                // Obtenir le répertoire de base
                string baseDirectory = Path.GetDirectoryName(baseUrl);
                return Path.Combine(baseDirectory, relativeUrl).Replace('\\', '/');
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Normalise une URL pour éviter les doublons
        /// </summary>
        private string NormalizeUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return url;

            // Normaliser les séparateurs de chemin
            return url.Replace('\\', '/');
        }

        /// <summary>
        /// Valide le format d'un email
        /// </summary>
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            try
            {
                // Pattern simple pour valider un email
                var emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                return Regex.IsMatch(email, emailPattern);
            }
            catch
            {
                return false;
            }
        }
    }
}