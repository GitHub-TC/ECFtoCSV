# ECF → CSV Converter ![Version](https://img.shields.io/badge/version-2.0.0-blue)

A Windows desktop tool for extracting dialogue texts from **Empyrion Galactic Survival** mod files into a CSV translation file.

---

## What is this for?

Empyrion mods contain configuration files (`.ecf`) in which dialogue texts, trader labels and other in-game strings are embedded as plain text, for example:

```
C:\GAMES\Steam\steamapps\workshop\content\383120\3143225812\Content\Configuration\TokenConfig.ecf
C:\GAMES\Steam\steamapps\workshop\content\383120\3143225812\Content\Configuration\TraderNPCConfig.ecf
```

To translate these texts into other languages, they first need to be **extracted from the ECF files** and replaced with unique keys.  
That is exactly what this tool does:

1. It reads the ECF file and finds all `Output:` and `Option_*` entries containing text.
2. Each text is replaced by a **unique key** (e.g. `txt_1A2B3C4D`).
3. The mapping `key → original text` is written to a **CSV file**.
4. The modified ECF file (with keys instead of plain text) is saved back.

---

## Workflow

```
ECF file  ──►  ECF → CSV Converter  ──►  ECF file (with keys)
                       │
                       ▼
                  CSV file
              (key + source text)
                       │
                       ▼
              TranslateCSV (DeepL)
                       │
                       ▼
           CSV file (multilingual)
```

### Step 1 – Extract texts (this tool)

- Select the ECF input file (e.g. `TraderNPCConfig.ecf`)
- Specify the CSV file (new or existing)
- Adjust the key prefix if desired (default: `txt_`)
- Click **"Start conversion"**

The tool writes all found texts with their generated keys into the CSV and replaces the texts in the ECF file with the keys.

### Step 2 – Translate

The generated CSV file can then be translated into any language using the tool **TranslateCSV** via the **DeepL API**.
TranslateCSV adds additional language columns to the CSV without modifying the existing keys.

### Step 3 – Repeated execution

If the tool is run again on an ECF file that has already been partially processed, it recognises existing keys and skips them. Only **newly added texts** are appended to the CSV.

---

## Installation

1. Open the **Releases** tab on this page and find the latest version.
2. Download **`ECFtoCSV.zip`**.
3. Extract the archive to any folder.
4. Run **`ECFtoCSV.exe`** – that's it.

A .NET installation is **not required** – the program is fully self-contained.

---

---

## Interface

| Field | Description |
|-------|-------------|
| **ECF input** | The `.ecf` mod file to be processed |
| **ECF output** | Where the modified ECF (with keys) is saved – default: overwrite input file |
| **CSV input** | The CSV translation file (created automatically if it does not exist yet) |
| **CSV output** | Where the updated CSV is saved – default: overwrite input file |
| **Key prefix** | Prefix for every generated key, e.g. `txt_` → `txt_1A2B3C4D` |

The interface is available in **German**, **English** and **French** (select in the top-right corner).

---

## Settings

Settings (file paths, prefix, language) are saved automatically at:

```
%APPDATA%\ECFtoCSV\settings.json
```

---

## Project

| | |
|---|---|
| Language | C# / .NET 10 / WPF |
| License | GNU General Public License v3.0 |
| Author | ASTIC/TC |
