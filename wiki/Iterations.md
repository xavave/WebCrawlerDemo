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

## 📋 Itération 3 : Support des URLs Absolues et Domaines Multiples

**Statut** : 📋 Planifié

### 🎯 Objectifs

- Support des URLs HTTP/HTTPS complètes
- Crawler sur plusieurs domaines
- Résolution correcte des URLs relatives et absolues
- Respect des redirections HTTP

### 📋 Tâches Prévues

- [ ] Implémenter un vrai `HttpWebBrowser` avec HttpClient
- [ ] Gérer les URLs absolues (http://, https://)
- [ ] Résoudre les URLs relatives correctement avec `Uri`
- [ ] Gérer les redirections (301, 302)
- [ ] Ajouter une option pour limiter aux sous-domaines

---

## 📋 Itération 4 : Rate Limiting et Politeness Policies

**Statut** : 📋 Planifié

### 🎯 Objectifs

- Implémenter un délai entre les requêtes
- Limiter le nombre de requêtes par seconde
- Respecter les bonnes pratiques de crawling

---

## 📋 Itération 5 : Gestion du robots.txt

**Statut** : 📋 Planifié

### 🎯 Objectifs

- Parser le fichier robots.txt
- Respecter les directives User-agent
- Gérer les règles Allow/Disallow

---

## 📋 Itération 6 : Tests Unitaires Complets

**Statut** : 📋 Planifié

### 🎯 Objectifs

- Tests unitaires avec xUnit
- Tests d'intégration
- Code coverage > 80%

---

## 📋 Itération 7 : Logging Structuré

**Statut** : 📋 Planifié

### 🎯 Objectifs

- Implémenter Serilog ou NLog
- Logs structurés (JSON)
- Niveaux de log appropriés (Debug, Info, Warning, Error)

---

## 📊 Vue d'Ensemble du Progrès

| Itération | Fonctionnalité | Statut | Commit |
|-----------|---------------|--------|--------|
| 1 | Algorithme BFS de base | ✅ | 893543b |
| 2 | HTML réel | ✅ | e18d968 / 815ba01 |
| 3 | URLs absolues | 📋 | - |
| 4 | Rate limiting | 📋 | - |
| 5 | robots.txt | 📋 | - |
| 6 | Tests unitaires | 📋 | - |
| 7 | Logging structuré | 📋 | - |