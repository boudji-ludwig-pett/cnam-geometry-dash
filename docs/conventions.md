# Conventions développement informatique

## GitFlow

Le projet suit la convention [GitFlow](https://nvie.com/posts/a-successful-git-branching-model/) reposant sur 3 branches principales :

- `main` : Contient le code de la dernière version stable et déployé en production.
- `staging` : Contient le code en cours de test avant déploiement en production, Quality Assurance (QA).
- `develop` : Contient le code en cours de développement. Les nouvelles fonctionnalités et les correctifs de bugs sont fusionnés ici régulièrement.

Idéalement, chaque nouvelle fonctionnalité ou correctif de bug est développé dans une branche dédiée à partir de `develop`, nommée `feat/<nom-de-la-fonctionnalité>` ou `fix/<nom-du-bug>`. Une fois le développement terminé, une pull request est créée pour demander une revue de code, et une fois validée, la branche est fusionnée dans `develop`, puis supprimée.

## Convention des commits

Les commits respectent la convention [Conventional Commits](https://www.conventionalcommits.org/) et [Semantic Versioning](https://semver.org/) pour la gestion des versions et des releases en fonction des commits.

Les commits doivent être **atomiques**, c'est-à-dire qu'ils respectent les 3 règles suivantes :

- Ne concernent qu'un seul sujet (une fonctionnalité, une correction de bug, etc.).
- Doivent avoir un message clair et concis.
- Ne doivent pas rendre le dépôt "incohérent" (ne bloquent pas la CI du projet).

## Pull Requests (PR)

Lorsqu'une Pull Request (PR) est créée, il est obligatoire d'ajouter un **Reviewer**. Le rôle du Reviewer est d'effectuer une **code review** pour garantir la qualité et la conformité du code soumis avant sa fusion dans la branche cible. L'approbation du Reviewer est nécessaire pour fusionner la PR.

### Processus de revue de code

1. **Code Review** :
   - Examiner le code pour détecter les erreurs ou les violations des bonnes pratiques.
   - Vérifier la clarté, la lisibilité et la maintenabilité du code.

2. **Commentaires** :
   - Les commentaires doivent suivre les [Conventional Comments](https://conventionalcomments.org/), qui permettent de structurer les retours de manière cohérente et compréhensible.

3. **Validation** :
   - Une fois tous les retours pris en compte et les corrections appliquées, le Reviewer peut approuver la PR, permettant ainsi sa fusion.
