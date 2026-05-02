# ECF → CSV Konverter

🌐 [English](README.en.md) · [Français](README.fr.md)

Ein Windows-Desktop-Tool zum Extrahieren von Dialogtexten aus **Empyrion Galactic Survival**-Mod-Dateien in eine CSV-Übersetzungsdatei.

---

## Wozu brauche ich das?

Empyrion-Mods enthalten Konfigurationsdateien (`.ecf`), in denen Dialogtexte, Händler-Beschriftungen und andere Spieltexte direkt als Klartext eingebettet sind, zum Beispiel:

```
C:\GAMES\Steam\steamapps\workshop\content\383120\3143225812\Content\Configuration\TokenConfig.ecf
C:\GAMES\Steam\steamapps\workshop\content\383120\3143225812\Content\Configuration\TraderNPCConfig.ecf
```

Um diese Texte in andere Sprachen übersetzen zu können, müssen sie zunächst **aus den ECF-Dateien herausgezogen** und durch eindeutige Schlüssel ersetzt werden.  
Genau das erledigt dieses Tool:

1. Es liest die ECF-Datei und sucht alle `Output:`- und `Option_*`-Einträge mit Textinhalten.
2. Jeder Text wird durch einen **eindeutigen Schlüssel** ersetzt (z. B. `txt_1A2B3C4D`).
3. Die Zuordnung `Schlüssel → Originaltext` wird in eine **CSV-Datei** geschrieben.
4. Die modifizierte ECF-Datei (mit Schlüsseln statt Klartext) wird zurückgespeichert.

---

## Workflow

```
ECF-Datei  ──►  ECF → CSV Konverter  ──►  ECF-Datei (mit Schlüsseln)
                        │
                        ▼
                   CSV-Datei
                (Schlüssel + Text)
                        │
                        ▼
               TranslateCSV (DeepL)
                        │
                        ▼
              CSV-Datei (mehrsprachig)
```

### Schritt 1 – Texte extrahieren (dieses Tool)

- ECF-Eingabedatei wählen (z. B. `TraderNPCConfig.ecf`)
- CSV-Datei angeben (neu oder bereits vorhanden)
- Schlüssel-Präfix nach Wunsch anpassen (Standard: `txt_`)
- **„Konvertierung starten"** klicken

Das Tool schreibt alle gefundenen Texte mit ihren generierten Schlüsseln in die CSV und ersetzt die Texte in der ECF-Datei durch die Schlüssel.

### Schritt 2 – Übersetzen

Die erzeugte CSV-Datei kann anschließend mit dem Tool **TranslateCSV** per **DeepL API** in beliebige Sprachen übersetzt werden.
TranslateCSV ergänzt die CSV-Datei um weitere Sprachspalten, ohne die vorhandenen Schlüssel zu verändern.

### Schritt 3 – Wiederholte Ausführung

Wird das Tool erneut auf einer bereits teilweise verarbeiteten ECF-Datei ausgeführt, erkennt es bereits vorhandene Schlüssel und überspringt diese. Nur **neu hinzugekommene Texte** werden der CSV hinzugefügt.

---

## Voraussetzungen

| Anforderung | Version |
|-------------|---------|
| Windows | 10 / 11 |
| .NET Runtime | **nicht nötig** – die `publish\ECFtoCSV.exe` ist selbst-enthalten |

---

## Starten

Einfach die fertig kompilierte Datei ausführen:

```
publish\ECFtoCSV.exe
```

Beim ersten Start sind keine Einstellungen nötig – alle Pfade und Optionen werden über die Oberfläche ausgewählt. Die Einstellungen werden automatisch gespeichert und beim nächsten Start wiederhergestellt.

---

## Oberfläche

| Feld | Bedeutung |
|------|-----------|
| **ECF-Eingabe** | Die zu verarbeitende `.ecf`-Mod-Datei |
| **ECF-Ausgabe** | Wohin die modifizierte ECF (mit Schlüsseln) gespeichert wird – Standard: Eingabedatei überschreiben |
| **CSV-Eingabe** | Die CSV-Übersetzungsdatei (wird neu erstellt, falls noch nicht vorhanden) |
| **CSV-Ausgabe** | Wohin die aktualisierte CSV gespeichert wird – Standard: Eingabedatei überschreiben |
| **Schlüssel-Präfix** | Kürzel vor jedem generierten Schlüssel, z. B. `txt_` → `txt_1A2B3C4D` |

Die Oberfläche ist in **Deutsch**, **Englisch** und **Französisch** verfügbar (Auswahl oben rechts).

---

## Einstellungen

Die Einstellungen (Dateipfade, Präfix, Sprache) werden automatisch gespeichert unter:

```
%APPDATA%\ECFtoCSV\settings.json
```

---

## Projekt

| | |
|---|---|
| Sprache | C# / .NET 10 / WPF |
| Lizenz | GNU General Public License v3.0 |
| Autor | ASTIC/TC |
