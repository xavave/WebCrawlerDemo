# WebCrawlerDemo Wiki

Bienvenue sur le wiki du projet **WebCrawlerDemo** ! Ce wiki documente l'évolution du projet à travers différentes itérations.

## 📚 Contenu

- **[Iterations](Iterations)** - Détails de chaque itération du projet
- **[IA comme Accélérateur pour Développeurs Seniors](IA-Accelerateur-Developpeurs-Seniors)** - Réflexion sur l'utilisation de l'IA dans le développement
- **[Développeur Senior IA-Augmenté Pour Serensia](Developpeur-Senior-IA-Augmente-Pour-Serensia)** - Proposition de valeur Serensia

## 🎯 Objectif du Projet

Implémenter un algorithme de web crawling pour extraire les adresses emails (liens `mailto:`) d'une page web et de ses pages référencées, avec contrôle de la profondeur de recherche.

## 🔄 Approche de Développement

Ce projet suit une approche **itérative et incrémentale**, où chaque itération ajoute une fonctionnalité ou amélioration spécifique :

1. ✅ **Itération 1** - Algorithme BFS de base avec HTML/XML valide
2. ✅ **Itération 2** - Gestion du HTML réel avec HtmlAgilityPack
3. ✅ **Itération 3** - Support des URLs HTTP/HTTPS et domaines multiples
4. ✅ **Itération 4** - Rate limiting et politeness policies
5. ✅ **Itération 5** - Support robots.txt (RFC-compliant)
6. ✅ **Itération 6** - Tests unitaires complets (23 tests xUnit)
7. ✅ **Itération 7** - Logging structuré avec Serilog

🎉 **Toutes les itérations sont complétées !** Le projet est maintenant de **niveau production**.

## 🛠️ Technologies

- **.NET 8.0** / **C# 12**
- **Algorithme BFS** (Breadth-First Search)
- **HtmlAgilityPack 1.12.4** - Parsing HTML robuste
- **Serilog 4.3.0** - Logging structuré
- **xUnit 2.9.3** - Tests unitaires (23 tests, 100% réussite)
- **SOLID Principles** - Architecture maintenable

## 📊 Statistiques du Projet

- **Commits** : 10+ commits documentés
- **Tests** : 23 tests unitaires (100% de réussite)
- **Fichiers** : 12 fichiers de code principal + 3 fichiers de tests
- **Fonctionnalités** : 7 itérations complètes
- **Documentation** : README complet + Wiki avec 4 pages
- **Niveau** : Production-ready avec observabilité complète

## 📖 Navigation

Consultez la page **[Iterations](Iterations)** pour voir les détails techniques de chaque étape du développement.