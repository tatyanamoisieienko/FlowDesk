# FlowDesk

FlowDesk ist ein kleiner Full-Stack-Prototyp zur Verwaltung interner Anfragen in einer Organisation mit mehreren Standorten.

Die Idee ist, interne Anfragen zentral zu erfassen und mit einem KI-Vorschlag zu strukturieren. Der Benutzer kann diesen Vorschlag prüfen, ändern, annehmen oder ablehnen.

Das Projekt verwendet nur fiktive Demo-Daten und verarbeitet keine realen Patientendaten.

## Funktionen

- Übersicht aller internen Anfragen
- Neue Anfrage mit Standort und Freitext erstellen
- Detailansicht einer Anfrage
- Status ändern:
  - Neu
  - In Bearbeitung
  - Erledigt
- Priorität manuell ändern:
  - Niedrig
  - Normal
  - Hoch
- Anfragen löschen
- Sortierung nach ID
- KI-Vorschlag für eine Anfrage erstellen
- KI-Vorschlag annehmen, ändern oder ablehnen
- Titel, Priorität und zuständige Abteilungen bearbeiten
- Unklare Anfragen erkennen, bei denen eine manuelle Prüfung notwendig ist

## KI-Demo

Aktuell ist noch kein echtes LLM angebunden.

FlowDesk verwendet den regelbasierten `DemoKiVorschlagService`. Dieser analysiert den Text anhand von Regeln und Schlüsselwörtern.

Er kann Vorschläge erstellen für:

- Titel
- Priorität
- zuständige Abteilung oder mehrere Abteilungen
- fehlende Informationen
- nächste Schritte
- notwendige manuelle Prüfung

Die Logik ist über `IKiVorschlagService` vom restlichen System getrennt. Dadurch kann der Demo-Service später durch eine Implementierung mit einem echten LLM ersetzt werden, ohne den gesamten Workflow neu aufzubauen.

## Human-in-the-loop

Für mich war wichtig, dass das System eine Anfrage nicht automatisch verändert.

Zuerst wird ein separater Vorschlag erstellt. Der Benutzer prüft ihn und entscheidet danach, ob er ihn annehmen, ändern oder ablehnen möchte.

Wenn eine Anfrage zu unklar ist, zeigt das System an, dass eine manuelle Prüfung notwendig ist.

## Technologie

### Backend

- C#
- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- SQLite
- xUnit

### Frontend

- React
- TypeScript
- Vite
- CSS

## Projektstruktur

```text
FlowDesk/
├── backend/
│   ├── FlowDesk.Api/
│   └── FlowDesk.Api.Tests/
├── frontend/
└── README.md
```

## Lokaler Start

### Backend

Voraussetzung: .NET 10 SDK

```bash
cd backend/FlowDesk.Api
dotnet restore
dotnet run --launch-profile http
```

Das Backend läuft unter:

```text
http://localhost:5083
```

### Frontend

Voraussetzung: Node.js und npm

```bash
cd frontend
npm install
npm run dev
```

Das Frontend läuft unter:

```text
http://localhost:5173
```

### Tests
 
```bash
cd backend/FlowDesk.Api.Tests
dotnet test
```

## Arbeiten mit Claude Code

Für dieses Projekt habe ich Claude Code als mein wichtigstes agentisches Coding-Werkzeug verwendet.

Ich habe die Entwicklung in kleine Aufgaben aufgeteilt und vorher festgelegt, welche Dateien geändert werden dürfen und welches Ergebnis ich erwarte.

Claude Code analysierte den bestehenden Code und setzte einzelne Änderungen um.

Danach startete ich die Anwendung und überprüfte das Ergebnis selbst. Wenn etwas nicht so funktionierte oder aussah, wie ich es erwartet hatte, beschrieb ich das Problem genauer und ließ die entsprechende Stelle korrigieren.

Nach der Prüfung entschied ich selbst, ob das Ergebnis für mich passt, und führte danach Commit und Push durch.

## Was war für mich neu?

Neu war für mich der Ansatz, ein agentisches Coding-Werkzeug als wichtigen Teil des Entwicklungsprozesses zu verwenden.

Früher habe ich AI hauptsächlich für einzelne Fragen, Erklärungen oder kleinere Code-Beispiele verwendet. In diesem Projekt arbeitete Claude Code direkt mit dem bestehenden Projekt, analysierte Dateien und setzte einzelne Aufgaben um.

Ich musste lernen, Aufgaben in kleine und klare Schritte aufzuteilen und vorher festzulegen, welche Dateien geändert werden dürfen und welches Ergebnis ich erwarte.

Neu war für mich auch der Umgang mit dem KI-Vorschlag. Mir war wichtig, dass das System zuerst nur einen Vorschlag erstellt und der Benutzer danach entscheidet, was damit passiert.

## Wo gab es Schwierigkeiten?

Einige Probleme wurden erst beim Starten und Testen der Anwendung sichtbar.

Zum Beispiel zeigte die Anfragenliste nach einer Änderung nicht immer sofort die aktuellen Daten. Deshalb wurde die Liste beim Zurückkehren erneut vom Backend geladen.

Bei sehr unklaren Anfragen musste außerdem entschieden werden, dass das System keinen sicheren Vorschlag erstellen soll, wenn nicht genug Informationen vorhanden sind. Dafür gibt es die manuelle Prüfung.

Auch beim Design gab es kleine Probleme. Claude Code hatte zum Beispiel zwei unterschiedliche Button-Arten zuerst fast gleich gestaltet. Nach meiner Prüfung habe ich die Anforderungen genauer beschrieben und die Styles wurden angepasst.

Dadurch habe ich gelernt, dass die Ergebnisse eines Coding-Agents trotzdem selbst geprüft werden müssen.

## Mögliche nächste Schritte

In Zukunft würde ich eine Login-Funktion und verschiedene Benutzerrollen hinzufügen.

Zum Beispiel könnten Mitarbeiter neue Anfragen erstellen und ihre eigenen Anfragen sehen. IT-Mitarbeiter könnten die eingegangenen Anfragen bearbeiten und Status, Priorität oder zuständige Abteilung ändern.

Ich würde außerdem speichern, von welchem Mitarbeiter eine Anfrage erstellt wurde und zu welcher Abteilung diese Person gehört.

Die KI-Logik könnte zusätzlich mit einer Mitarbeiterliste verbunden werden. Wenn in einer Anfrage der Name einer Person erwähnt wird, könnte das System dadurch die passende Abteilung besser bestimmen.

Für eine größere Anzahl von Anfragen würde ich außerdem hinzufügen:

- Suche nach Anfragen
- Filter nach Städten
- weitere Sortiermöglichkeiten
- Informationen über den Ersteller einer Anfrage

Später könnte der `DemoKiVorschlagService` über das bestehende `IKiVorschlagService` durch ein echtes LLM ersetzt werden.
