# ECF → CSV Convertisseur ![Version](https://img.shields.io/badge/version-2.0.0-blue)

Un outil de bureau Windows pour extraire les textes de dialogue des fichiers de mod **Empyrion Galactic Survival** dans un fichier CSV de traduction.

---

## Installation

1. Ouvrir l'onglet **Releases** de cette page et trouver la dernière version.
2. Télécharger **`ECFtoCSV.zip`**.
3. Extraire l'archive dans le dossier de votre choix.
4. Lancer **`ECFtoCSV.exe`** – c'est tout.

Aucune installation de .NET n'est **requise** – le programme est entièrement autonome.

---

## À quoi sert cet outil ?

Les mods Empyrion contiennent des fichiers de configuration (`.ecf`) dans lesquels les textes de dialogue, les étiquettes de commerçants et autres textes du jeu sont intégrés en clair.  
Ces fichiers se trouvent dans le **répertoire de scénario** du mod – pour les installations Steam Workshop, généralement à :

```
C:\GAMES\Steam\steamapps\workshop\content\383120\<IDScénario>\Content\Configuration\TokenConfig.ecf
C:\GAMES\Steam\steamapps\workshop\content\383120\<IDScénario>\Content\Configuration\TraderNPCConfig.ecf
```

Sur les **serveurs Empyrion**, le répertoire de scénario se trouve généralement directement dans le dossier d'installation d'Empyrion, p. ex. :

```
<ServeurEmpyrion>\Saves\Games\<NomScénario>\Mods\<NomMod>\Content\Configuration\
```

Pour traduire ces textes dans d'autres langues, ils doivent d'abord être **extraits des fichiers ECF** et remplacés par des clés uniques.  
C'est exactement ce que fait cet outil :

1. Il lit le fichier ECF et recherche toutes les entrées `Output:` et `Option_*` contenant du texte.
2. Chaque texte est remplacé par une **clé unique** (p. ex. `txt_1A2B3C4D`).
3. La correspondance `clé → texte original` est écrite dans un **fichier CSV**.
4. Le fichier ECF modifié (avec des clés à la place du texte) est réenregistré.

---

## Flux de travail

```
Fichier ECF  ──►  ECF → CSV Convertisseur  ──►  Fichier ECF (avec clés)
                           │
                           ▼
                      Fichier CSV
                  (clé + texte source)
                           │
                           ▼
                 TranslateCSV (DeepL)
                           │
                           ▼
              Fichier CSV (multilingue)
```

### Étape 1 – Extraire les textes (cet outil)

- Sélectionner le fichier ECF d'entrée (p. ex. `TraderNPCConfig.ecf`)
- Indiquer le fichier CSV (nouveau ou existant)
- Ajuster le préfixe de clé si souhaité (par défaut : `txt_`)
- Cliquer sur **« Lancer la conversion »**

L'outil écrit tous les textes trouvés avec leurs clés générées dans le CSV et remplace les textes dans le fichier ECF par les clés.

### Étape 2 – Traduire

Le fichier CSV généré peut ensuite être traduit dans n'importe quelle langue à l'aide de l'outil **TranslateCSV** via l'**API DeepL**.
TranslateCSV ajoute des colonnes de langue supplémentaires au CSV sans modifier les clés existantes.

### Étape 3 – Exécution répétée

Si l'outil est relancé sur un fichier ECF déjà partiellement traité, il reconnaît les clés existantes et les ignore. Seuls les **textes nouvellement ajoutés** sont ajoutés au CSV.

---

## Interface

| Champ | Description |
|-------|-------------|
| **ECF entrée** | Le fichier mod `.ecf` à traiter |
| **ECF sortie** | Où le fichier ECF modifié (avec clés) est enregistré – par défaut : écraser le fichier d'entrée |
| **CSV entrée** | Le fichier CSV de traduction (créé automatiquement s'il n'existe pas encore) |
| **CSV sortie** | Où le CSV mis à jour est enregistré – par défaut : écraser le fichier d'entrée |
| **Préfixe de clé** | Préfixe pour chaque clé générée, p. ex. `txt_` → `txt_1A2B3C4D` |

L'interface est disponible en **allemand**, **anglais** et **français** (sélection en haut à droite).

---

## Paramètres

Les paramètres (chemins de fichiers, préfixe, langue) sont sauvegardés automatiquement dans :

```
%APPDATA%\ECFtoCSV\settings.json
```

---

## Projet

| | |
|---|---|
| Langage | C# / .NET 10 / WPF |
| Licence | GNU General Public License v3.0 |
| Auteur | ASTIC/TC |
