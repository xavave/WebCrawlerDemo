using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace WebCrawlerDemo
{
    /// <summary>
    /// Parser pour les fichiers robots.txt selon la RFC
    /// Itération 5: Support de robots.txt
    /// </summary>
    public class RobotsTxtParser
    {
        private readonly Dictionary<string, List<RobotRule>> _rules;
        private readonly List<string> _sitemaps;
        private int? _crawlDelay;

        public RobotsTxtParser()
        {
            _rules = new Dictionary<string, List<RobotRule>>();
            _sitemaps = new List<string>();
        }

        /// <summary>
        /// Parse le contenu d un fichier robots.txt
        /// </summary>
        public void Parse(string robotsTxtContent)
        {
            if (string.IsNullOrWhiteSpace(robotsTxtContent))
                return;

            var lines = robotsTxtContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            string currentUserAgent = "*";
            var currentRules = new List<RobotRule>();

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                
                // Ignorer les commentaires et lignes vides
                if (string.IsNullOrWhiteSpace(trimmedLine) || trimmedLine.StartsWith("#"))
                    continue;

                // Parser les directives
                var parts = trimmedLine.Split(new[] { ':' }, 2);
                if (parts.Length != 2)
                    continue;

                var directive = parts[0].Trim().ToLowerInvariant();
                var value = parts[1].Trim();

                switch (directive)
                {
                    case "user-agent":
                        // Sauvegarder les règles précédentes
                        if (currentRules.Count > 0)
                        {
                            if (!_rules.ContainsKey(currentUserAgent))
                                _rules[currentUserAgent] = new List<RobotRule>();
                            _rules[currentUserAgent].AddRange(currentRules);
                        }
                        
                        currentUserAgent = value.ToLowerInvariant();
                        currentRules = new List<RobotRule>();
                        break;

                    case "disallow":
                        currentRules.Add(new RobotRule { Type = RuleType.Disallow, Path = value });
                        break;

                    case "allow":
                        currentRules.Add(new RobotRule { Type = RuleType.Allow, Path = value });
                        break;

                    case "crawl-delay":
                        if (int.TryParse(value, out int delay))
                        {
                            _crawlDelay = delay * 1000; // Convertir en millisecondes
                        }
                        break;

                    case "sitemap":
                        _sitemaps.Add(value);
                        break;
                }
            }

            // Sauvegarder les dernières règles
            if (currentRules.Count > 0)
            {
                if (!_rules.ContainsKey(currentUserAgent))
                    _rules[currentUserAgent] = new List<RobotRule>();
                _rules[currentUserAgent].AddRange(currentRules);
            }
        }

        /// <summary>
        /// Vérifie si une URL peut être crawlée selon les règles robots.txt
        /// </summary>
        public bool IsAllowed(string url, string userAgent = "*")
        {
            if (string.IsNullOrEmpty(url))
                return false;

            try
            {
                var uri = new Uri(url);
                var path = uri.PathAndQuery;

                // Récupérer les règles applicables
                var applicableRules = GetApplicableRules(userAgent);

                // Si aucune règle, tout est autorisé
                if (applicableRules.Count == 0)
                    return true;

                // Trouver la règle la plus spécifique qui correspond
                RobotRule? matchingRule = null;
                int maxMatchLength = 0;

                foreach (var rule in applicableRules)
                {
                    if (PathMatches(path, rule.Path))
                    {
                        // La règle la plus longue (la plus spécifique) gagne
                        if (rule.Path.Length > maxMatchLength)
                        {
                            maxMatchLength = rule.Path.Length;
                            matchingRule = rule;
                        }
                    }
                }

                // Si aucune règle ne correspond, c est autorisé
                if (matchingRule == null)
                    return true;

                // Appliquer la règle trouvée
                return matchingRule.Type == RuleType.Allow;
            }
            catch
            {
                // En cas d erreur, autoriser par défaut
                return true;
            }
        }

        /// <summary>
        /// Récupère le délai de crawl en millisecondes (si spécifié)
        /// </summary>
        public int? GetCrawlDelay()
        {
            return _crawlDelay;
        }

        /// <summary>
        /// Récupère la liste des sitemaps
        /// </summary>
        public List<string> GetSitemaps()
        {
            return _sitemaps.ToList();
        }

        private List<RobotRule> GetApplicableRules(string userAgent)
        {
            var rules = new List<RobotRule>();
            var normalizedAgent = userAgent.ToLowerInvariant();

            // Chercher les règles spécifiques au user-agent
            foreach (var kvp in _rules)
            {
                if (kvp.Key == "*" || normalizedAgent.Contains(kvp.Key))
                {
                    rules.AddRange(kvp.Value);
                }
            }

            return rules;
        }

        private bool PathMatches(string path, string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
                return true;

            // Pattern vide = tout est autorisé
            if (pattern == "/")
                return true;

            // Convertir le pattern robots.txt en regex
            // * = n importe quel caractère
            // $ = fin de l URL
            var regexPattern = "^" + Regex.Escape(pattern)
                .Replace("\\*", ".*")
                .Replace("\\$", "$");

            return Regex.IsMatch(path, regexPattern, RegexOptions.IgnoreCase);
        }
    }

    public enum RuleType
    {
        Allow,
        Disallow
    }

    public class RobotRule
    {
        public RuleType Type { get; set; }
        public string Path { get; set; } = string.Empty;
    }
}
