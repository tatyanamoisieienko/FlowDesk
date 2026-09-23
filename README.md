# FlowDesk

FlowDesk ist ein kleiner Full-Stack-Prototyp zur Verwaltung interner Anfragen in einer Organisation mit mehreren Standorten.

Die Idee ist, unstrukturierte interne Anfragen zentral zu erfassen und sie anschließend durch einen KI-gestützten Vorschlag zu strukturieren. Der Vorschlag kann von einem Benutzer geprüft, übernommen, geändert oder abgelehnt werden.

Das Projekt verwendet ausschließlich fiktive Demo-Daten und verarbeitet keine realen Patienten- oder Gesundheitsdaten.

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
- KI-Vorschlag für eine Anfrage erzeugen
- KI-Vorschlag prüfen:
  - Annehmen
  - Ändern
  - Ablehnen
- Titel, Priorität und zuständige Abteilungen vor der Übernahme bearbeiten
- Erkennen unklarer Anfragen, bei denen eine manuelle Prüfung erforderlich ist

## KI-Demo

Aktuell ist noch kein externes LLM angebunden.

Stattdessen verwendet FlowDesk einen regelbasierten `DemoKiVorschlagService`. Dieser analysiert den ursprünglichen Text anhand definierter Schlüsselwörter und erstellt daraus einen Vorschlag für:

- Titel
- Priorität
- zuständige Abteilungen
- fehlende Informationen
- nächste Schritte
- erforderliche manuelle Prüfung

Dabei können auch mehrere Abteilungen gleichzeitig erkannt werden.

Die KI-Logik ist über `IKiVorschlagService` vom restlichen System getrennt. Dadurch kann der Demo-Service später durch eine Implementierung mit einem echten LLM ersetzt werden, ohne den gesamten Workflow neu aufzubauen.

## Human-in-the-loop

Ein wichtiger Teil des Projekts war für mich, dass ein KI-Vorschlag nicht automatisch die eigentliche Anfrage verändert.

Die Analyse wird zunächst separat als Vorschlag gespeichert. Anschließend entscheidet der Benutzer, ob der Vorschlag:

- übernommen,
- vorher verändert oder
- abgelehnt

werden soll.

Bei sehr unklaren Anfragen wird zusätzlich eine manuelle Prüfung verlangt.

Damit bleibt die endgültige Entscheidung beim Benutzer und nicht beim automatisierten System.

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

Das Backend läuft anschließend unter:

```text
http://localhost:5083
```

Beim ersten Start wird die lokale SQLite-Datenbank automatisch erstellt und mit Demo-Daten gefüllt.

### Frontend

Voraussetzung: Node.js und npm

```bash
cd frontend
npm install
npm run dev
```

Das Frontend läuft anschließend unter:

```text
http://localhost:5173
```

### Tests

```bash
cd backend/FlowDesk.Api.Tests
dotnet test
```

## Arbeiten mit Claude Code

Für dieses Projekt habe ich Claude Code als agentisches Coding-Werkzeug verwendet.

Dabei habe ich Claude nicht einfach den kompletten Auftrag auf einmal umsetzen lassen. Ich habe die Entwicklung in kleinere Aufgaben aufgeteilt und den bestehenden Stand zwischen den einzelnen Schritten immer wieder getestet und überprüft.

Mein Workflow war ungefähr:

1. Anforderungen für den nächsten kleinen Schritt festlegen
2. Claude Code den bestehenden Code analysieren lassen
3. Änderung implementieren lassen
4. Anwendung selbst starten und Verhalten testen
5. Ergebnis und Code überprüfen
6. Probleme oder unpassendes Verhalten konkret zurückgeben
7. Änderung korrigieren und erneut testen
8. funktionierenden Stand committen

Claude Code war damit das Hauptwerkzeug für die Implementierung, die Entscheidungen über Verhalten, Datenmodell und Workflow sowie die Kontrolle der Ergebnisse habe ich selbst übernommen.

