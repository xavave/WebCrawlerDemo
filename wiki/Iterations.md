# Itérations du Projet WebCrawlerDemo

Ce document détaille chaque itération du projet, les fonctionnalités ajoutées, les défis rencontrés et les solutions apportées.

---

## ✅ Itération 1 : Algorithme BFS de Base avec HTML/XML Valide

**Date** : 29 octobre 2025  
**Commit** : `893543b` - "Initial commit: Email Web Crawler with BFS algorithm"  
**Statut** : ✅ Complété

### 🎯 Objectifs

- Implémenter l'algorithme BFS (Breadth-First Search) pour le crawling
- Extraire les emails des liens `mailto:` dans les pages HTML
- Gérer la profondeur de recherche configurable
- Éviter les cycles (pages qui se référencent mutuellement)
- Garantir l'unicité des emails retournés

### 📋 Fonctionnalités Implémentées

#### 1. **Algorithme BFS**
```csharp
var urlsToVisit = new Queue<(string Url, int Depth)>();
while (urlsToVisit.Count > 0)
{
    var (currentUrl, currentDepth) = urlsToVisit.Dequeue();
    // Traitement niveau par niveau
}
```

**Avantages du BFS** :
- Explore tous les liens à profondeur `n` avant de passer à `n+1`
- Respecte la contrainte de priorité aux pages "les plus proches"
- Contrôle naturel de la profondeur maximale

#### 2. **Extraction des Emails**
```csharp
var mailtoLinks = doc.Descendants("a")
    .Where(a => a.Attribute("href")?.Value.StartsWith("mailto:") == true);
```

- Extraction via parsing XML (XDocument)
- Validation avec regex
- Normalisation en minuscules pour l'unicité

#### 3. **Gestion des Cycles**
```csharp
var visitedUrls = new HashSet<string>();
if (visitedUrls.Contains(currentUrl))
    continue;
```

- HashSet pour tracker les URLs visitées
- Évite les boucles infinies (index → child1 → index)

#### 4. **Contrôle de Profondeur**
```csharp
if (maximumDepth >= 0 && currentDepth > maximumDepth)
    continue;
```

- Profondeur configurable (0, 1, 2, ...)
- Support de `-1` pour exploration illimitée

### 🏗️ Architecture

**Classes principales** :
- `ITheTest` - Interface du crawler
- `IWebBrowser` - Interface du navigateur (abstraction)
- `EmailWebCrawler` - Implémentation BFS
- `MockWebBrowser` - Navigateur de test

**Patterns utilisés** :
- ✅ **Dependency Injection** - `IWebBrowser` injecté
- ✅ **Single Responsibility** - Méthodes dédiées
- ✅ **Strategy Pattern** - Algorithme BFS encapsulé

### 📊 Résultats de Test

```
Profondeur 0: nullepart@mozilla.org
Profondeur 1: nullepart@mozilla.org, ailleurs@mozilla.org
Profondeur 2: nullepart@mozilla.org, ailleurs@mozilla.org, loin@mozilla.org
```

✅ Tous les tests passent correctement

### ⚠️ Limitations Connues

1. **HTML doit être du XML valide** - Utilise `XDocument.Parse()`
2. **Pas de support HTTP réel** - URLs locales uniquement
3. **Pas de rate limiting** - Peut surcharger un serveur
4. **Pas de gestion robots.txt** - Ne respecte pas les règles de crawling
5. **Logging minimal** - Console.WriteLine uniquement

### 📈 Métriques

- **Complexité temporelle** : O(V + E)
- **Complexité spatiale** : O(V)
- **Fichiers** : 5 (Interfaces, Crawler, Browser, Program, README)
- **Lignes de code** : ~400 lignes

### 🎓 Leçons Apprises

1. **BFS vs DFS** : BFS est clairement supérieur pour ce cas d'usage car il garantit l'exploration des pages proches en premier
2. **XML Parsing** : Assumer que HTML = XML valide est une simplification forte
3. **SOLID** : L'utilisation d'interfaces rend le code testable et extensible

---

## ✅ Itération 2 : Gestion du HTML Réel

**Date** : 29 octobre 2025  
**Commit** : `e18d968` / `815ba01` - "Iteration 2: HTML réel avec HtmlAgilityPack + Wiki documentation"  
**Statut** : ✅ Complété

### 🎯 Objectifs

- Remplacer le parsing XML par un parser HTML robuste
- Gérer le HTML malformé (balises non fermées, attributs sans guillemets, etc.)
- Utiliser **HtmlAgilityPack** ou **AngleSharp** pour le parsing
- Maintenir la compatibilité avec l'interface existante

### 📋 Tâches

- [x] Ajouter le package NuGet HtmlAgilityPack
- [x] Refactorer `ExtractEmailsFromHtml()` pour utiliser HtmlAgilityPack
- [x] Refactorer `ExtractChildUrls()` pour utiliser HtmlAgilityPack
- [x] Tester avec du HTML réel malformé
- [x] Mettre à jour la documentation
- [x] Créer des tests pour HTML malformé

### 🔧 Changements Techniques Prévus

**Avant (XDocument)** :
```csharp
var doc = XDocument.Parse($"<root>{html}</root>");
var mailtoLinks = doc.Descendants("a");
```

**Après (HtmlAgilityPack)** :
```csharp
var doc = new HtmlDocument();
doc.LoadHtml(html);
var mailtoLinks = doc.DocumentNode.SelectNodes("//a[@href]");
```

### ⚠️ Risques et Défis

1. **Performance** - Parser HTML peut être plus lent que XML
2. **Compatibilité** - S'assurer que les tests existants passent
3. **Dépendances** - Ajout d'une dépendance externe

### 📈 Critères de Succès

- ✅ Tous les tests existants passent
- ✅ Peut parser du HTML avec des balises non fermées
- ✅ Peut parser du HTML avec des attributs sans guillemets
- ✅ Performance similaire ou meilleure

### 🎯 Résultats Obtenus

**Changements implémentés** :

1. **Migration vers HtmlAgilityPack** :
```csharp
// Avant (XDocument)
var doc = XDocument.Parse($"<root>{html}</root>");

// Après (HtmlAgilityPack)
var htmlDoc = new HtmlDocument();
htmlDoc.LoadHtml(html);
```

2. **Parsing robuste avec XPath** :
```csharp
var linkNodes = doc.DocumentNode.SelectNodes("//a[@href]");
```

3. **Test avec HTML malformé** :
- Balises non fermées : `<h1>HTML MALFORMÉ` ✅
- Attributs sans guillemets : `href=mailto:test@test.org` ✅
- Guillemets simples : `href='./link.html'` ✅

**Tests validés** :
```
Profondeur 0: nullepart@mozilla.org ✅
Profondeur 1: nullepart@mozilla.org, ailleurs@mozilla.org ✅
Profondeur 2: nullepart@mozilla.org, ailleurs@mozilla.org, loin@mozilla.org ✅
HTML Malformé: test@malformed.org, another@test.com ✅
```

### 📚 Documentation Créée

- **Wiki complet** avec 3 pages :
  - `Home.md` - Page d'accueil du wiki
  - `Iterations.md` - Documentation détaillée des itérations
  - `IA-Accelerateur-Developpeurs-Seniors.md` - Réflexion sur l'utilisation de l'IA

### 🎓 Leçons Apprises

1. **HtmlAgilityPack vs XDocument** : HtmlAgilityPack est beaucoup plus tolérant et adapté au HTML réel
2. **XPath** : Expressions XPath plus simples et lisibles que LINQ to XML
3. **Robustesse** : Peut maintenant gérer du HTML de sources variées (web scraping réel)

### 📦 Dépendances Ajoutées

- `HtmlAgilityPack` v1.12.4

---

## ✅ Itération 3 : Support des URLs HTTP/HTTPS et Domaines Multiples

**Date** : 29 octobre 2025  
**Commit** : `1c787fe` - "Iteration 3: HTTP/HTTPS URLs support"  
**Statut** : ✅ Complété

### 🎯 Objectifs

- Support des URLs HTTP/HTTPS complètes
- Crawler sur plusieurs domaines
- Résolution correcte des URLs relatives et absolues avec `Uri`
- Respect des redirections HTTP
- Normalisation avancée des URLs

### 📋 Fonctionnalités Implémentées

#### 1. **HttpWebBrowser avec HttpClient**
```csharp
public class HttpWebBrowser : IWebBrowser
{
    private readonly HttpClient _httpClient;
    
    public string? GetHtml(string url)
    {
        var response = _httpClient.GetAsync(url).Result;
        return response.Content.ReadAsStringAsync().Result;
    }
}
```

**Caractéristiques** :
- HttpClient avec redirections automatiques (max 5)
- Timeout configurable (30 secondes)
- User-Agent personnalisable
- Gestion des erreurs HTTP

#### 2. **Résolution d'URLs avec Uri**
```csharp
private string ResolveRelativeUrl(string baseUrl, string relativeUrl)
{
    var baseUri = new Uri(baseUrl);
    var resolvedUri = new Uri(baseUri, relativeUrl);
    return resolvedUri.ToString();
}
```

**Support** :
- URLs relatives : `./child.html`, `../parent.html`, `child.html`
- URLs absolues : `https://example.com/page.html`
- Chemins locaux : `C:/TestHtml/index.html`

#### 3. **Normalisation des URLs**
```csharp
private string NormalizeUrl(string url)
{
    // Lowercase scheme et host
    // Suppression default ports (80, 443)
    // Suppression fragments (#section)
    // Normalisation path
}
```

### 📊 Résultats de Test

✅ Support HTTP/HTTPS complet
✅ Résolution d'URLs relatives correcte
✅ Normalisation avancée (ports, fragments)
✅ Tous les tests précédents passent

### 🎓 Leçons Apprises

1. **Uri classe** : Gère automatiquement la résolution relative/absolue
2. **HttpClient** : Doit être réutilisé (singleton pattern recommandé)
3. **Normalisation** : Critique pour éviter les doublons d'URLs

---

## ✅ Itération 4 : Rate Limiting et Politeness Policies

**Date** : 29 octobre 2025  
**Commit** : `bc56821` - "Iteration 4: Rate limiting and politeness policies"  
**Statut** : ✅ Complété

### 🎯 Objectifs

- Implémenter un délai entre les requêtes par domaine
- Limiter le nombre de pages par domaine
- Créer des politiques de crawling configurables
- Respecter les bonnes pratiques de crawling éthique

### 📋 Fonctionnalités Implémentées

#### 1. **Rate Limiting par Domaine**
```csharp
private void ApplyRateLimiting(string url)
{
    var domain = ExtractDomain(url);
    var timeSinceLastRequest = DateTime.UtcNow - _lastRequestPerDomain[domain];
    var remainingDelay = _delayBetweenRequestsMs - timeSinceLastRequest.TotalMilliseconds;
    
    if (remainingDelay > 0)
        Thread.Sleep((int)remainingDelay);
}
```

**Thread-safe** avec `lock()` pour accès concurrent

#### 2. **Limitation de Pages par Domaine**
```csharp
private bool CanCrawlDomain(string url)
{
    if (_policies.MaxPagesPerDomain < 0)
        return true; // Illimité
        
    var domain = ExtractDomain(url);
    return _pagesPerDomain[domain] < _policies.MaxPagesPerDomain;
}
```

#### 3. **CrawlerPolicies Configurables**
```csharp
public class CrawlerPolicies
{
    public int DelayBetweenRequestsMs { get; set; } = 1000;
    public int MaxPagesPerDomain { get; set; } = -1;
    public int RequestTimeoutSeconds { get; set; } = 30;
    public string UserAgent { get; set; } = "WebCrawlerDemo/1.0 (+GitHub)";
    
    public static CrawlerPolicies Default { get; }      // Équilibré
    public static CrawlerPolicies Conservative { get; } // Respectueux
    public static CrawlerPolicies Aggressive { get; }   // Tests
}
```

### 📊 Politiques Prédéfinies

| Politique | Délai (ms) | Max Pages | Timeout (s) | Usage |
|-----------|-----------|-----------|-------------|-------|
| Default | 1000 | Illimité | 30 | Production standard |
| Conservative | 2000 | 100 | 20 | Sites sensibles |
| Aggressive | 100 | Illimité | 10 | Tests uniquement |

### 🎓 Leçons Apprises

1. **Rate limiting** : Essentiel pour ne pas surcharger les serveurs
2. **Thread-safety** : Lock nécessaire pour le dictionnaire de timestamps
3. **Configuration** : Politiques prédéfinies facilitent l'utilisation

---

## ✅ Itération 5 : Gestion du robots.txt

**Date** : 29 octobre 2025  
**Commit** : `018f50c` - "Iteration 5: robots.txt support"  
**Statut** : ✅ Complété

### 🎯 Objectifs

- Parser le fichier robots.txt selon la RFC
- Respecter les directives User-agent, Disallow, Allow
- Gérer Crawl-delay et Sitemap
- Cache thread-safe par domaine

### 📋 Fonctionnalités Implémentées

#### 1. **RobotsTxtParser RFC-compliant**
```csharp
public class RobotsTxtParser
{
    public void Parse(string robotsTxtContent);
    public bool IsAllowed(string url, string userAgent);
    public int? GetCrawlDelay();
    public List<string> GetSitemaps();
}
```

**Directives supportées** :
- `User-agent:` - Ciblage par crawler
- `Disallow:` - Chemins interdits
- `Allow:` - Chemins autorisés (priorité sur Disallow)
- `Crawl-delay:` - Délai minimum entre requêtes
- `Sitemap:` - URLs des sitemaps
- Commentaires (`#`)
- Wildcards (`*` et `$`)

#### 2. **RobotsTxtCache Thread-Safe**
```csharp
public class RobotsTxtCache
{
    public RobotsTxtParser GetRobotsTxt(string url);
    public bool IsAllowed(string url, string userAgent);
}
```

**Caractéristiques** :
- Cache par domaine avec expiration (24h)
- Thread-safe avec `lock()`
- Téléchargement automatique de `/robots.txt`

#### 3. **Intégration au Crawler**
```csharp
if (_policies.RespectRobotsTxt && !_robotsCache.IsAllowed(url, userAgent))
{
    Log.Information("URL bloquée par robots.txt: {Url}", url);
    continue;
}
```

### 📊 Résultats de Test

✅ Parser complet fonctionnel
✅ Règles Allow/Disallow respectées
✅ Crawl-delay et Sitemap extraits
✅ Cache par domaine opérationnel
✅ Multi-user-agents géré correctement

### 🎓 Leçons Apprises

1. **RFC robots.txt** : Règles complexes avec priorités (plus spécifique gagne)
2. **Cache** : Évite de télécharger robots.txt à chaque requête
3. **Éthique** : Respecter robots.txt est fondamental pour un crawler responsable

---

## ✅ Itération 6 : Tests Unitaires Complets (xUnit)

**Date** : 29 octobre 2025  
**Commits** : `b4a22b3`, `f851efa` - "Iteration 6: xUnit tests + fixes"  
**Statut** : ✅ Complété

### 🎯 Objectifs

- Tests unitaires avec xUnit
- Coverage de tous les composants principaux
- Tests des edge cases
- 100% de réussite des tests

### 📋 Tests Implémentés

#### **EmailWebCrawlerTests.cs** (10 tests)
- ✅ Depth 0/1/2 - Emails extraits corrects
- ✅ HTML malformé - Parser robuste
- ✅ Emails distincts - Unicité garantie
- ✅ Normalisation lowercase
- ✅ MaxPagesPerDomain - Limite respectée
- ✅ Validation paramètres (null, empty)

#### **RobotsTxtParserTests.cs** (10 tests)
- ✅ Empty content - Tout autorisé
- ✅ Disallow all - Tout bloqué
- ✅ Allow/Disallow - Règle spécifique gagne
- ✅ Crawl-delay - Valeur extraite
- ✅ Sitemap - URLs extraites
- ✅ Commentaires - Ignorés (ligne + inline)
- ✅ Wildcards - Patterns * et $
- ✅ Multi-user-agents - Priorité correcte

#### **CrawlerPoliciesTests.cs** (3 tests)
- ✅ Default policy - Valeurs équilibrées
- ✅ Conservative policy - Valeurs respectueuses
- ✅ Aggressive policy - Valeurs rapides

### 📊 Résultats

```
Série de tests réussie.
Nombre total de tests : 23
     Réussi(s) : 23
 Durée totale : 0,49 secondes
```

### 🐛 Bugs Corrigés

1. **Commentaires inline** - Parser ne gérait pas `# comment` après directive
2. **Multi-user-agents** - Règles wildcard pas priorisées correctement
3. **Domain limiting** - Chemins locaux non détectés comme domaine

### 🎓 Leçons Apprées

1. **TDD** : Tests révèlent bugs subtils dans la logique
2. **Arrange-Act-Assert** : Pattern clair et maintenable
3. **Edge cases** : Tests de validation essentiels

---

## ✅ Itération 7 : Structured Logging avec Serilog

**Date** : 29 octobre 2025  
**Commit** : `2f513cf` - "Iteration 7: Structured logging with Serilog"  
**Statut** : ✅ Complété

### 🎯 Objectifs

- Implémenter Serilog pour logging structuré
- Console et File sinks
- Niveaux de log appropriés (Debug, Info, Warning, Error)
- Logs JSON structurés dans fichiers
- Rotation quotidienne des logs

### 📋 Fonctionnalités Implémentées

#### 1. **LoggingConfiguration Centralisé**
```csharp
public static class LoggingConfiguration
{
    public static void ConfigureLogging(LogEventLevel minimumLevel = LogEventLevel.Information)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Is(minimumLevel)
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}")
            .WriteTo.File(
                path: "logs/webcrawler-.log",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7)
            .CreateLogger();
    }
}
```

#### 2. **Logging dans Toutes les Classes**

**EmailWebCrawler** :
```csharp
Log.Information("Début du crawling - URL: {Url}, Profondeur max: {MaxDepth}", url, maximumDepth);
Log.Debug("Emails trouvés - URL: {Url}, Nombre: {Count}, Emails: {@Emails}", url, count, emails);
Log.Warning("Limite de pages atteinte - URL: {Url}, Domaine: {Domain}", url, domain);
Log.Error(ex, "Erreur lors du traitement de l'URL: {Url}", url);
```

**HttpWebBrowser** :
```csharp
Log.Debug("Récupération de la page - URL: {Url}", url);
Log.Debug("Rate limiting - Domaine: {Domain}, Attente: {Delay}ms", domain, delay);
```

**RobotsTxtParser** :
```csharp
Log.Debug("Parsing robots.txt - Taille: {Size} caractères", size);
```

#### 3. **Niveaux de Log Utilisés**

| Niveau | Usage | Exemples |
|--------|-------|----------|
| **Debug** | Détails techniques | URL normalisée, liens enfants, cache |
| **Information** | Événements importants | Début/fin crawling, résultats |
| **Warning** | Situations anormales | Limite atteinte, HTML vide |
| **Error** | Erreurs avec exceptions | Erreurs HTTP, parsing |

### 📊 Résultats

**Console Output** :
```
[21:10:07 INF] Début du crawling - URL: C:/TestHtml/index.html
[21:10:07 DBG] Emails trouvés - Nombre: 1
[21:10:07 WRN] Limite de pages atteinte - Domaine: c:/testhtml
[21:10:07 INF] Crawling terminé - Emails: 3, Pages: 3
```

**Fichier logs/webcrawler-20251029.log** :
```json
[2025-10-29 21:10:07.514 +01:00 INF] Début du crawling - URL: "C:/TestHtml/index.html" {"Application":"WebCrawlerDemo"}
[2025-10-29 21:10:07.626 +01:00 DBG] Emails trouvés - Nombre: 1, Emails: ["nullepart@mozilla.org"] {"Application":"WebCrawlerDemo"}
```

### 📈 Caractéristiques

- ✅ **Rotation quotidienne** - Nouveau fichier chaque jour
- ✅ **Rétention 7 jours** - Nettoyage automatique
- ✅ **Logs structurés JSON** - Facilite parsing et analyse
- ✅ **Métadonnées** - Application, timestamp, timezone
- ✅ **Observabilité complète** - Traçabilité de toutes les opérations

### 🎓 Leçons Apprises

1. **Serilog** : Flexible et puissant pour logging structuré
2. **Niveaux appropriés** : Debug pour détails, Info pour événements, Warning pour anomalies
3. **JSON structuré** : Facilite analyse avec outils (ELK, Splunk, etc.)
4. **Production-ready** : Rotation et rétention essentielles

---

## 📊 Vue d'Ensemble du Progrès

| Itération | Fonctionnalité | Statut | Commit |
|-----------|---------------|--------|--------|
| 1 | Algorithme BFS de base | ✅ | 893543b |
| 2 | HTML réel (HtmlAgilityPack) | ✅ | e18d968 |
| 3 | URLs HTTP/HTTPS | ✅ | 1c787fe |
| 4 | Rate limiting et politiques | ✅ | bc56821 |
| 5 | Support robots.txt | ✅ | 018f50c |
| 6 | Tests unitaires xUnit (23 tests) | ✅ | b4a22b3 |
| 7 | Logging structuré (Serilog) | ✅ | 2f513cf |

## 🎉 Projet Complet !

Le WebCrawlerDemo est maintenant un **crawler de niveau production** avec :
- 🛡️ Sécurité et éthique (robots.txt, rate limiting)
- � Observabilité (logging structuré)
- ✅ Qualité (23 tests unitaires, SOLID)
- ⚡ Performance (cache, normalisation)
- 🎯 Robustesse (HTML malformé, erreurs)
- 🔧 Configuration (politiques personnalisables)