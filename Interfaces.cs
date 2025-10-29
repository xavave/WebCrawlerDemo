using System.Collections.Generic;

namespace WebCrawlerDemo
{
    // Interface fournie pour le test
    public interface ITheTest
    {
        // Retourne la liste des emails distincts trouvés dans la page et ses pages enfants
        List<string> GetEmailsInPageAndChildPages(IWebBrowser browser, string url, int maximumDepth);
    }

    // Interface fournie pour le navigateur
    public interface IWebBrowser
    {
        // Retourne le HTML de la page ou null si l'URL ne peut pas être visitée
        string GetHtml(string url);
    }
}