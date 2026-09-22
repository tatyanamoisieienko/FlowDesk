import { useEffect, useState } from 'react'
import { getAnfragen } from './api'
import type { AnfrageListItem } from './types'

function kurzerText(text: string, laenge = 60): string {
  if (text.length <= laenge) return text
  return `${text.slice(0, laenge).trim()}…`
}

function formatDatum(iso: string): string {
  return new Date(iso).toLocaleString('de-DE')
}

function App() {
  const [anfragen, setAnfragen] = useState<AnfrageListItem[]>([])
  const [laedt, setLaedt] = useState(true)
  const [fehler, setFehler] = useState<string | null>(null)

  useEffect(() => {
    getAnfragen()
      .then(setAnfragen)
      .catch((error: Error) => setFehler(error.message))
      .finally(() => setLaedt(false))
  }, [])

  return (
    <main>
      <header>
        <h1>Anfragen</h1>
        <button type="button">Neue Anfrage</button>
      </header>

      {laedt && <p>Anfragen werden geladen…</p>}
      {fehler && <p role="alert">{fehler}</p>}

      {!laedt && !fehler && (
        <table>
          <thead>
            <tr>
              <th>Id</th>
              <th>Titel</th>
              <th>Standort</th>
              <th>Status</th>
              <th>Priorität</th>
              <th>Erstellt am</th>
            </tr>
          </thead>
          <tbody>
            {anfragen.map((anfrage) => (
              <tr key={anfrage.id}>
                <td>{anfrage.id}</td>
                <td>{anfrage.titel ?? kurzerText(anfrage.originalText)}</td>
                <td>{anfrage.standort}</td>
                <td>{anfrage.status}</td>
                <td>{anfrage.prioritaet ?? '–'}</td>
                <td>{formatDatum(anfrage.erstelltAm)}</td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </main>
  )
}

export default App
