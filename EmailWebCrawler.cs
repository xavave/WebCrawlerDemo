using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace WebCrawlerDemo
{
    /// <summary>
    /// Implémentation du web crawler d'emails utilisant l'algorithme BFS (Breadth-First Search)
    /// pour respecter la contrainte de priorité aux pages "les plus proches".
    /// Version 2: Utilise HtmlAgilityPack pour parser du HTML réel (pas uniquement XML valide)
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

                    // Parser le HTML réel avec HtmlAgilityPack (supporte HTML malformé)
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(html);

                    // Extraire tous les emails de la page actuelle
                    var emailsInPage = ExtractEmailsFromHtml(htmlDoc);
                    foreach (var email in emailsInPage)
                    {
                        distinctEmails.Add(email);
                    }

                    // Si on n'a pas atteint la profondeur maximale, ajouter les liens enfants
                    if (maximumDepth == -1 || currentDepth < maximumDepth)
                    {
                        var childUrls = ExtractChildUrls(htmlDoc, currentUrl);
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
        /// Extrait tous les emails des liens mailto dans le document HTML
        /// Utilise HtmlAgilityPack pour supporter le HTML réel (malformé)
        /// </summary>
        private HashSet<string> ExtractEmailsFromHtml(HtmlDocument doc)
        {
            var emails = new HashSet<string>();

            // Rechercher tous les éléments <a> avec un attribut href contenant "mailto:"
            // XPath: sélectionne tous les liens <a> qui ont un attribut href
            var linkNodes = doc.DocumentNode.SelectNodes("//a[@href]");
            
            if (linkNodes == null)
                return emails;

            foreach (var link in linkNodes)
            {
                string? href = link.GetAttributeValue("href", null);
                
                if (string.IsNullOrEmpty(href))
                    continue;

                // Vérifier si c'est un lien mailto
                if (!href.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase))
                    continue;

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
        /// Utilise HtmlAgilityPack pour supporter le HTML réel (malformé)
        /// </summary>
        private HashSet<string> ExtractChildUrls(HtmlDocument doc, string baseUrl)
        {
            var childUrls = new HashSet<string>();

            // Rechercher tous les éléments <a> avec un attribut href
            var linkNodes = doc.DocumentNode.SelectNodes("//a[@href]");
            
            if (linkNodes == null)
                return childUrls;

            foreach (var link in linkNodes)
            {
                string? href = link.GetAttributeValue("href", null);
                
                if (string.IsNullOrEmpty(href))
                    continue;

                // Ignorer les liens mailto et les liens HTTP/HTTPS (pour l'instant)
                if (href.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase) ||
                    href.StartsWith("http:", StringComparison.OrdinalIgnoreCase) ||
                    href.StartsWith("https:", StringComparison.OrdinalIgnoreCase))
                    continue;

                string? absoluteUrl = ResolveRelativeUrl(baseUrl, href);
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
        private string? ResolveRelativeUrl(string baseUrl, string relativeUrl)
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