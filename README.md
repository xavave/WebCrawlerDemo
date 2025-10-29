# 🕷️ Web Crawler Demo - Email Extractor

Un algorithme de web crawling pour extraire les adresses emails (liens `mailto:`) d'une page web et de ses pages référencées, avec contrôle de la profondeur de recherche.

## 📋 Description

Ce projet implémente un web crawler qui :
- Extrait tous les emails distincts trouvés dans les liens `mailto:` d'une page HTML
- Explore récursivement les pages web référencées
- Utilise un algorithme **BFS (Breadth-First Search)** pour prioriser les pages les plus proches
- Permet de contrôler la profondeur maximale de recherche
- Gère les cycles (pages qui se référencent mutuellement)
- Garantit l'unicité des emails retournés

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

```csharp
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

// Profondeur -1 : exploration complète sans limite
emails = webCrawler.GetEmailsInPageAndChildPages(browser, "C:/TestHtml/index.html", -1);
```

## 📁 Structure du Projet

```
WebCrawlerDemo/
├── Interfaces.cs           # Définition des interfaces ITheTest et IWebBrowser
├── EmailWebCrawler.cs      # Implémentation de l'algorithme BFS
├── MockWebBrowser.cs       # Navigateur fictif pour les tests
├── Program.cs              # Point d'entrée avec exemples de tests
└── README.md               # Documentation
```

## 🔧 Technologies

- **.NET 8.0**
- **C# 12**
- **System.Xml.Linq** pour le parsing XML/HTML
- **SOLID Principles** : Séparation des responsabilités, injection de dépendances

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

Le projet inclut des tests manuels dans `Program.cs` qui valident :
- ✅ Extraction correcte des emails à différentes profondeurs
- ✅ Gestion des emails en double (unicité)
- ✅ Gestion des cycles (index → child1 → index)
- ✅ Respect de la profondeur maximale
- ✅ Exploration illimitée avec `maximumDepth = -1`

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

---

**Note** : Ce projet se concentre sur l'algorithme de crawling. Pour une utilisation en production, il faudrait ajouter :
- Gestion du HTML réel (pas uniquement XML valide)
- Support des URLs absolues et domaines multiples
- Rate limiting et politeness policies
- Gestion du robots.txt
- Tests unitaires complets
- Logging structuré

Test : dotnet run
sortie:
=== Test du Web Crawler d'Emails ===

Profondeur 0:
Résultat: nullepart@mozilla.org

Profondeur 1:
Résultat: nullepart@mozilla.org, ailleurs@mozilla.org

Profondeur 2:
Résultat: nullepart@mozilla.org, ailleurs@mozilla.org, loin@mozilla.org
