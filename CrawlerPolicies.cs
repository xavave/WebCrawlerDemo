namespace WebCrawlerDemo
{
    /// <summary>
    /// Configuration des politiques de crawling responsable
    /// It�ration 4: Rate limiting et politesse
    /// </summary>
    public class CrawlerPolicies
    {
        /// <summary>
        /// D�lai minimum en millisecondes entre deux requ�tes vers le m�me domaine
        /// Valeur recommand�e: 1000ms (1 seconde) pour �tre poli
        /// </summary>
        public int DelayBetweenRequestsMs { get; set; } = 1000;

        /// <summary>
        /// Nombre maximum de pages � crawler par domaine
        /// -1 = illimit�
        /// </summary>
        public int MaxPagesPerDomain { get; set; } = -1;

        /// <summary>
        /// Timeout pour chaque requ�te HTTP en secondes
        /// </summary>
        public int RequestTimeoutSeconds { get; set; } = 30;

        /// <summary>
        /// User-Agent � utiliser pour s'identifier aupr�s des serveurs
        /// </summary>
        public string UserAgent { get; set; } = "WebCrawlerDemo/1.0 (+https://github.com/xavave/WebCrawlerDemo)";

        /// <summary>
        /// Respecter les directives robots.txt (sera impl�ment� dans l'it�ration 5)
        /// </summary>
        public bool RespectRobotsTxt { get; set; } = true;

        /// <summary>
        /// Politique conservative recommand�e pour un crawling respectueux
        /// </summary>
        public static CrawlerPolicies Conservative => new CrawlerPolicies
        {
            DelayBetweenRequestsMs = 2000,
            MaxPagesPerDomain = 100,
            RequestTimeoutSeconds = 20,
            UserAgent = "WebCrawlerDemo/1.0 (Conservative; +https://github.com/xavave/WebCrawlerDemo)",
            RespectRobotsTxt = true
        };

        /// <summary>
        /// Politique agressive pour les tests ou environnements contr�l�s
        /// </summary>
        public static CrawlerPolicies Aggressive => new CrawlerPolicies
        {
            DelayBetweenRequestsMs = 100,
            MaxPagesPerDomain = -1,
            RequestTimeoutSeconds = 10,
            UserAgent = "WebCrawlerDemo/1.0 (Fast; +https://github.com/xavave/WebCrawlerDemo)",
            RespectRobotsTxt = false
        };

        /// <summary>
        /// Politique par d�faut �quilibr�e
        /// </summary>
        public static CrawlerPolicies Default => new CrawlerPolicies();
    }
}
