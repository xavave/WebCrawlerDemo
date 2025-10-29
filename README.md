# 🕷️ Web Crawler Demo - Email Extractor

Un web crawler professionnel pour extraire les adresses emails (liens `mailto:`) d'une page web et de ses pages référencées, avec contrôle de la profondeur, rate limiting, respect des robots.txt et logging structuré.

## 📋 Description

Ce projet implémente un web crawler de niveau production qui :
- ✅ Extrait tous les emails distincts trouvés dans les liens `mailto:` d'une page HTML
- ✅ Explore récursivement les pages web référencées
- ✅ Utilise un algorithme **BFS (Breadth-First Search)** pour prioriser les pages les plus proches
- ✅ Parse du **HTML réel malformé** avec HtmlAgilityPack
- ✅ Supporte les **URLs HTTP/HTTPS** et domaines multiples
- ✅ Implémente le **rate limiting** et politiques de politesse
- ✅ Respecte les fichiers **robots.txt**
- ✅ Logging structuré avec **Serilog**
- ✅ Permet de contrôler la profondeur maximale de recherche
- ✅ Gère les cycles (pages qui se référencent mutuellement)
- ✅ Garantit l'unicité des emails retournés
- ✅ **23 tests unitaires xUnit** (100% de réussite)

## 🎯 Algorithme : BFS (Breadth-First Search)

### Pourquoi BFS ?

L'algorithme **BFS** a été choisi car il :
- ✅ Explore tous les liens à profondeur `n` avant de passer à profondeur `n+1`
- ✅ Respecte parfaitement la contrainte de priorité aux pages "les plus proches"
- ✅ Évite d'aller trop loin en profondeur avant d'avoir exploré le niveau actuel
- ✅ Garantit une exploration systématique et contrôlée

### Comparaison avec DFS

| Critère | BFS | DFS |
|---------|-----|-----|
| Ordre d'exploration | Niveau par niveau | Branche par branche |
| Pages proches | ✅ Prioritaires | ❌ Pas garanties |
| Contrôle profondeur | ✅ Naturel | ⚠️ Plus complexe |
| Mémoire | Plus élevée | Plus faible |

## 🚀 Utilisation

### Exemple de Base (MockWebBrowser)

```csharp
// Configuration du logging
LoggingConfiguration.ConfigureLogging(LogEventLevel.Information);

var browser = new MockWebBrowser();
var webCrawler = new EmailWebCrawler();

// Profondeur 0 : page de départ uniquement
var emails = webCrawler.GetEmailsInPageAndChildPages(browser, "C:/TestHtml/index.html", 0);
// Résultat : ["nullepart@mozilla.org"]

// Profondeur 1 : page de départ + pages directement référencées
emails = webCrawler.GetEmailsInPageAndChildPages(browser, "C:/TestHtml/index.html", 1);
// Résultat : ["nullepart@mozilla.org", "ailleurs@mozilla.org"]

// Profondeur 2 : page de départ + 2 niveaux de pages référencées
emails = webCrawler.GetEmailsInPageAndChildPages(browser, "C:/TestHtml/index.html", 2);
// Résultat : ["nullepart@mozilla.org", "ailleurs@mozilla.org", "loin@mozilla.org"]
```

### Exemple avec HTTP et Rate Limiting

```csharp
// Politique Conservative : crawling respectueux
var policies = CrawlerPolicies.Conservative;
var httpBrowser = new HttpWebBrowser(policies.UserAgent, policies.DelayBetweenRequestsMs);
var crawler = new EmailWebCrawler(policies);

var emails = crawler.GetEmailsInPageAndChildPages(
    httpBrowser, 
    "https://example.com", 
    maximumDepth: 2
);

// Fermer proprement le logger
LoggingConfiguration.CloseAndFlush();
```

### Politiques de Crawling

```csharp
// Politique par défaut (équilibrée)
var defaultPolicies = CrawlerPolicies.Default;
// - Délai: 1000ms entre requêtes
// - Max pages: illimité
// - Respecte robots.txt

// Politique conservative (très respectueuse)
var conservative = CrawlerPolicies.Conservative;
// - Délai: 2000ms entre requêtes
// - Max pages: 100 par domaine
// - Respecte robots.txt

// Politique aggressive (rapide, tests uniquement)
var aggressive = CrawlerPolicies.Aggressive;
// - Délai: 100ms entre requêtes
// - Max pages: illimité
// - Ignore robots.txt

// Politique personnalisée
var customPolicies = new CrawlerPolicies
{
    DelayBetweenRequestsMs = 500,
    MaxPagesPerDomain = 50,
    RequestTimeoutSeconds = 15,
    UserAgent = "MonCrawler/1.0",
    RespectRobotsTxt = true
};
```

## 📁 Structure du Projet

```
WebCrawlerDemo/
├── Interfaces.cs              # ITheTest et IWebBrowser
├── EmailWebCrawler.cs         # Algorithme BFS avec robots.txt
├── MockWebBrowser.cs          # Navigateur fictif pour les tests
├── HttpWebBrowser.cs          # Navigateur HTTP avec rate limiting
├── CrawlerPolicies.cs         # Politiques de crawling configurables
├── RobotsTxtParser.cs         # Parser RFC-compliant pour robots.txt
├── RobotsTxtCache.cs          # Cache thread-safe pour robots.txt
├── LoggingConfiguration.cs    # Configuration Serilog centralisée
├── Program.cs                 # Démonstration de toutes les itérations
├── WebCrawlerDemo.Tests/      # 23 tests unitaires xUnit
│   ├── EmailWebCrawlerTests.cs
│   ├── RobotsTxtParserTests.cs
│   └── CrawlerPoliciesTests.cs
├── logs/                      # Logs Serilog (rotation quotidienne)
└── README.md                  # Cette documentation
```

## 🔧 Technologies

- **.NET 8.0**
- **C# 12** avec nullable reference types
- **HtmlAgilityPack 1.12.4** - Parsing HTML robuste (supporte HTML malformé)
- **Serilog 4.3.0** - Logging structuré
  - Serilog.Sinks.Console 6.0.0
  - Serilog.Sinks.File 6.0.0
- **xUnit 2.9.3** - Framework de tests unitaires
- **SOLID Principles** - Architecture maintenable et extensible

## 📊 Exemple de Test

### Structure HTML de Test

**index.html**
```html
<html>
  <h1>INDEX</h1>
  <a href="./child1.html">child1</a>
  <a href="mailto:nullepart@mozilla.org">Envoyer l'email nulle part</a>
</html>
```

**child1.html**
```html
<html>
  <h1>CHILD1</h1>
  <a href="./index.html">index</a>
  <a href="./child2.html">child2</a>
  <a href="mailto:ailleurs@mozilla.org">Envoyer l'email ailleurs</a>
  <a href="mailto:nullepart@mozilla.org">Envoyer l'email nulle part</a>
</html>
```

**child2.html**
```html
<html>
  <h1>CHILD2</h1>
  <a href="./index.html">index</a>
  <a href="mailto:loin@mozilla.org">Envoyer l'email loin</a>
  <a href="mailto:nullepart@mozilla.org">Envoyer l'email nulle part</a>
</html>
```

### Résultats Attendus

```
Profondeur 0: nullepart@mozilla.org
Profondeur 1: nullepart@mozilla.org, ailleurs@mozilla.org
Profondeur 2: nullepart@mozilla.org, ailleurs@mozilla.org, loin@mozilla.org
```

## ⚡ Caractéristiques Techniques

### Gestion des Cycles
Le crawler détecte et évite les cycles grâce à un `HashSet<string>` qui stocke les URLs déjà visitées.

```csharp
var visitedUrls = new HashSet<string>();
if (visitedUrls.Contains(currentUrl))
    continue;
```

### Extraction des Emails
Les emails sont extraits des liens `mailto:` et validés avec une expression régulière :

```csharp
var mailtoLinks = doc.Descendants("a")
    .Where(a => a.Attribute("href")?.Value.StartsWith("mailto:") == true);
```

### Résolution des URLs Relatives
Le crawler gère correctement les URLs relatives (`./child1.html`) :

```csharp
if (relativeUrl.StartsWith("./"))
    relativeUrl = relativeUrl.Substring(2);
string absoluteUrl = Path.Combine(baseDirectory, relativeUrl);
```

### Unicité des Résultats
Un `HashSet<string>` garantit qu'aucun email n'est retourné en double, même s'il apparaît sur plusieurs pages.

## 📈 Complexité

- **Temps** : O(V + E)
  - V = nombre de pages visitées
  - E = nombre de liens explorés
  
- **Espace** : O(V)
  - Stockage des URLs visitées
  - Queue pour le parcours BFS

## 🏃 Exécution

```bash
# Cloner le repository
git clone https://github.com/[username]/WebCrawlerDemo.git

# Naviguer dans le projet
cd WebCrawlerDemo

# Restaurer les dépendances
dotnet restore

# Exécuter l'application
dotnet run
```

## 🧪 Tests

### Tests Unitaires (xUnit)

**23 tests unitaires avec 100% de réussite** couvrant :

#### EmailWebCrawlerTests (10 tests)
- ✅ Extraction correcte des emails à différentes profondeurs (0, 1, 2)
- ✅ Gestion du HTML malformé avec HtmlAgilityPack
- ✅ Gestion des emails en double (unicité)
- ✅ Normalisation des emails en minuscules
- ✅ Respect de la limite de pages par domaine
- ✅ Gestion des emails distincts sur plusieurs pages
- ✅ Validation des paramètres (null, empty)

#### RobotsTxtParserTests (10 tests)
- ✅ Parsing de robots.txt vide (tout autorisé)
- ✅ Directive Disallow: / (tout bloqué)
- ✅ Directive Disallow avec chemins spécifiques
- ✅ Combinaison Allow/Disallow (règle la plus spécifique gagne)
- ✅ Support Crawl-delay
- ✅ Support Sitemap
- ✅ Gestion des commentaires (ligne complète et inline)
- ✅ Patterns avec wildcards (* et $)
- ✅ Multi-user-agents (règles spécifiques prioritaires)

#### CrawlerPoliciesTests (3 tests)
- ✅ Politique Default (valeurs équilibrées)
- ✅ Politique Conservative (respectueuse)
- ✅ Politique Aggressive (tests uniquement)

### Exécution des Tests

```bash
# Exécuter tous les tests
dotnet test

# Avec output détaillé
dotnet test --logger "console;verbosity=detailed"

# Avec coverage (si configuré)
dotnet test --collect:"XPlat Code Coverage"
```

### Résultat des Tests

```
Série de tests réussie.
Nombre total de tests : 23
     Réussi(s) : 23
 Durée totale : 0,49 secondes
```

## 🎓 Principes de Design

### SOLID Principles

- **Single Responsibility** : Chaque méthode a une responsabilité unique
- **Open/Closed** : Extensible via l'interface `IWebBrowser`
- **Liskov Substitution** : `MockWebBrowser` peut être remplacé par toute implémentation de `IWebBrowser`
- **Interface Segregation** : Interfaces ciblées et minimales
- **Dependency Inversion** : Le crawler dépend de l'abstraction `IWebBrowser`, pas d'une implémentation concrète

### Clean Code

- Nommage expressif des variables et méthodes
- Méthodes courtes et focalisées
- Documentation XML pour toutes les méthodes publiques
- Gestion appropriée des erreurs

## 📝 Notes Techniques

- Le HTML est traité comme du XML valide (hypothèse de l'exercice)
- Les emails sont normalisés en minuscules pour l'unicité
- Les URLs absolues HTTP/HTTPS sont ignorées (focus sur les liens locaux)
- Les paramètres dans les liens `mailto:` (ex: `?subject=...`) sont automatiquement retirés

## 🤝 Contribution

Ce projet est un exercice algorithmique démontrant l'implémentation d'un web crawler avec BFS. Les contributions sont bienvenues pour :
- Améliorer la robustesse du parsing HTML
- Ajouter des tests unitaires (xUnit)
- Optimiser les performances
- Étendre les fonctionnalités

## 📄 Licence

Ce projet est fourni à des fins éducatives et de démonstration.

## 👨‍💻 Auteur

Développé comme démonstration d'algorithme de web crawling avec contrôle de profondeur.

## 📊 Itérations du Projet

Ce projet a été développé en **7 itérations** progressives :

### Itération 1 : BFS de base
- ✅ Algorithme BFS pour exploration niveau par niveau
- ✅ Parsing XML avec System.Xml.Linq
- ✅ Gestion des cycles et profondeur

### Itération 2 : HTML réel avec HtmlAgilityPack
- ✅ Support du HTML malformé
- ✅ Parser robuste (balises non fermées, attributs sans guillemets)
- ✅ XPath pour extraction des liens

### Itération 3 : URLs HTTP/HTTPS
- ✅ HttpClient pour requêtes réelles
- ✅ Résolution d'URLs relatives avec Uri
- ✅ Normalisation avancée des URLs
- ✅ Support domaines multiples

### Itération 4 : Rate Limiting et Politiques
- ✅ Délai configurable entre requêtes par domaine
- ✅ Limitation de pages par domaine
- ✅ 3 politiques préconfigurées (Default/Conservative/Aggressive)
- ✅ User-Agent personnalisable

### Itération 5 : Support robots.txt
- ✅ Parser RFC-compliant (Allow/Disallow/Crawl-delay/Sitemap)
- ✅ Cache thread-safe par domaine (24h)
- ✅ Gestion multi-user-agents
- ✅ Patterns avec wildcards

### Itération 6 : Tests Unitaires
- ✅ 23 tests xUnit avec 100% de réussite
- ✅ Arrange-Act-Assert pattern
- ✅ Tests de tous les composants principaux
- ✅ Validation des edge cases

### Itération 7 : Structured Logging
- ✅ Serilog avec Console et File sinks
- ✅ Logs structurés JSON dans fichiers
- ✅ 4 niveaux (Debug/Info/Warning/Error)
- ✅ Rotation quotidienne, rétention 7 jours
- ✅ Traçabilité complète de toutes les opérations

## 📈 Fonctionnalités de Production

Ce crawler est **prêt pour la production** avec :

- 🛡️ **Sécurité** : Respect des robots.txt, rate limiting, timeouts
- 🔍 **Observabilité** : Logging structuré avec Serilog
- ✅ **Qualité** : 23 tests unitaires, SOLID principles
- ⚡ **Performance** : Cache robots.txt, normalisation URLs
- 🎯 **Robustesse** : Gestion des erreurs, HTML malformé
- 🔧 **Configuration** : Politiques personnalisables

## 📝 Logging

Les logs sont écrits dans `logs/webcrawler-YYYY-MM-DD.log` avec :

```
[2025-10-29 21:10:07.514 +01:00 INF] Début du crawling - URL: "https://example.com"
[2025-10-29 21:10:07.607 +01:00 DBG] Cache robots.txt initialisé
[2025-10-29 21:10:07.626 +01:00 DBG] Emails trouvés - Nombre: 3, Emails: ["a@test.com", "b@test.com"]
[2025-10-29 21:10:07.629 +01:00 INF] Crawling terminé - Emails: 3, Pages: 5
```

---

## 🎓 Développement Augmenté par l'IA

Ce projet a été développé en utilisant **GitHub Copilot** comme assistant de développement senior, démontrant :
- 🚀 Développement itératif rapide (7 itérations complètes)
- ✅ Code de qualité production avec tests
- 📚 Documentation complète (README + Wiki)
- 🏗️ Architecture SOLID et Clean Code
- 🔄 Intégration continue (Git + GitHub)

**Voir le Wiki** pour plus de détails sur la méthodologie de développement augmenté par l'IA.

Test : dotnet run
sortie:
```
[21:10:07 INF] === Démarrage de WebCrawlerDemo ===

=== Test du Web Crawler d'Emails ===

Profondeur 0:
[21:10:07 INF] Début du crawling - URL: C:/TestHtml/index.html, Profondeur max: 0
[21:10:07 DBG] Emails trouvés - URL: C:/TestHtml/index.html, Nombre: 1
[21:10:07 INF] Crawling terminé - Emails trouvés: 1, Pages visitées: 1
Résultat: nullepart@mozilla.org
```
