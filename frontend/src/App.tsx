import { useEffect, useState, type FormEvent } from 'react'
import { erstelleAnfrage, getAnfrage, getAnfragen, getStammdaten, starteKiAnalyse } from './api'
import type { AnfrageDetail, AnfrageListItem, KiVorschlag, Standort } from './types'

type Ansicht = 'liste' | 'formular' | 'detail'

function kurzerText(text: string, laenge = 60): string {
  if (text.length <= laenge) return text
  return `${text.slice(0, laenge).trim()}…`
}

function formatDatum(iso: string): string {
  // + 2 Stunden 
  const datum = new Date(iso)
  datum.setHours(datum.getHours() + 2)
  return datum.toLocaleString('de-DE')
}

function App() {
  const [ansicht, setAnsicht] = useState<Ansicht>('liste')
  const [anfragen, setAnfragen] = useState<AnfrageListItem[]>([])
  const [laedt, setLaedt] = useState(true)
  const [fehler, setFehler] = useState<string | null>(null)
  const [ausgewaehlteId, setAusgewaehlteId] = useState<number | null>(null)

  const ladeAnfragen = () => {
    setLaedt(true)
    setFehler(null)
    getAnfragen()
      .then(setAnfragen)
      .catch((error: Error) => setFehler(error.message))
      .finally(() => setLaedt(false))
  }

  useEffect(() => {
    ladeAnfragen()
  }, [])

  if (ansicht === 'formular') {
    return (
      <main>
        <NeueAnfrageFormular
          onAbbrechen={() => setAnsicht('liste')}
          onErstellt={() => {
            setAnsicht('liste')
            ladeAnfragen()
          }}
        />
      </main>
    )
  }

  if (ansicht === 'detail' && ausgewaehlteId !== null) {
    return (
      <main>
        <AnfrageDetailAnsicht id={ausgewaehlteId} onZurueck={() => setAnsicht('liste')} />
      </main>
    )
  }

  return (
    <main>
      <header>
        <h1>Anfragen</h1>
        <button type="button" onClick={() => setAnsicht('formular')}>
          Neue Anfrage
        </button>
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
              <tr
                key={anfrage.id}
                className="klickbar"
                onClick={() => {
                  setAusgewaehlteId(anfrage.id)
                  setAnsicht('detail')
                }}
              >
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

function NeueAnfrageFormular({
  onAbbrechen,
  onErstellt,
}: {
  onAbbrechen: () => void
  onErstellt: () => void
}) {
  const [standorte, setStandorte] = useState<Standort[]>([])
  const [standortId, setStandortId] = useState('')
  const [originalText, setOriginalText] = useState('')
  const [fehler, setFehler] = useState<string | null>(null)
  const [speichertGerade, setSpeichertGerade] = useState(false)

  useEffect(() => {
    getStammdaten()
      .then((stammdaten) => setStandorte(stammdaten.standorte))
      .catch((error: Error) => setFehler(error.message))
  }, [])

  const handleSubmit = async (event: FormEvent) => {
    event.preventDefault()

    if (!standortId) {
      setFehler('Bitte einen Standort auswählen.')
      return
    }
    if (!originalText.trim()) {
      setFehler('Bitte einen Text eingeben.')
      return
    }

    setFehler(null)
    setSpeichertGerade(true)
    try {
      await erstelleAnfrage({ originalText: originalText.trim(), standortId: Number(standortId) })
      onErstellt()
    } catch (error) {
      setFehler((error as Error).message)
      setSpeichertGerade(false)
    }
  }

  return (
    <>
      <h1>Neue Anfrage</h1>
      <form onSubmit={handleSubmit}>
        <div className="feld">
          <label htmlFor="standort">Standort</label>
          <select id="standort" value={standortId} onChange={(event) => setStandortId(event.target.value)}>
            <option value="">Bitte wählen…</option>
            {standorte.map((standort) => (
              <option key={standort.id} value={standort.id}>
                {standort.name}
              </option>
            ))}
          </select>
        </div>

        <div className="feld">
          <label htmlFor="originalText">Anfrage</label>
          <textarea
            id="originalText"
            rows={6}
            value={originalText}
            onChange={(event) => setOriginalText(event.target.value)}
          />
        </div>

        {fehler && <p role="alert">{fehler}</p>}

        <div className="aktionen">
          <button type="button" onClick={onAbbrechen}>
            Abbrechen
          </button>
          <button type="submit" disabled={speichertGerade}>
            Anfrage erstellen
          </button>
        </div>
      </form>
    </>
  )
}

function AnfrageDetailAnsicht({ id, onZurueck }: { id: number; onZurueck: () => void }) {
  const [anfrage, setAnfrage] = useState<AnfrageDetail | null>(null)
  const [laedt, setLaedt] = useState(true)
  const [fehler, setFehler] = useState<string | null>(null)
  const [kiVorschlag, setKiVorschlag] = useState<KiVorschlag | null>(null)
  const [analysiertGerade, setAnalysiertGerade] = useState(false)
  const [analyseFehler, setAnalyseFehler] = useState<string | null>(null)

  useEffect(() => {
    setLaedt(true)
    setFehler(null)
    getAnfrage(id)
      .then((daten) => {
        setAnfrage(daten)
        setKiVorschlag(daten.kiVorschlag)
      })
      .catch((error: Error) => setFehler(error.message))
      .finally(() => setLaedt(false))
  }, [id])

  const handleKiAnalyse = async () => {
    setAnalyseFehler(null)
    setAnalysiertGerade(true)
    try {
      const ergebnis = await starteKiAnalyse(id)
      setKiVorschlag(ergebnis)
    } catch (error) {
      setAnalyseFehler((error as Error).message)
    } finally {
      setAnalysiertGerade(false)
    }
  }

  return (
    <>
      <header>
        <h1>Anfrage #{id}</h1>
        <button type="button" onClick={onZurueck}>
          Zurück
        </button>
      </header>

      {laedt && <p>Anfrage wird geladen…</p>}
      {fehler && <p role="alert">{fehler}</p>}

      {anfrage && (
        <dl className="detail">
          <dt>Titel</dt>
          <dd>{anfrage.titel ?? '–'}</dd>

          <dt>Standort</dt>
          <dd>{anfrage.standort}</dd>

          <dt>Status</dt>
          <dd>{anfrage.status}</dd>

          <dt>Priorität</dt>
          <dd>{anfrage.prioritaet ?? '–'}</dd>

          <dt>Abteilungen</dt>
          <dd>{anfrage.abteilungen.length > 0 ? anfrage.abteilungen.join(', ') : '–'}</dd>

          <dt>Erstellt am</dt>
          <dd>{formatDatum(anfrage.erstelltAm)}</dd>

          <dt>Aktualisiert am</dt>
          <dd>{formatDatum(anfrage.aktualisiertAm)}</dd>

          <dt>Ursprünglicher Text</dt>
          <dd className="original-text">{anfrage.originalText}</dd>
        </dl>
      )}

      {anfrage && (
        <section className="ki-bereich">
          <h2>KI-Vorschlag</h2>

          <button type="button" onClick={handleKiAnalyse} disabled={analysiertGerade}>
            {analysiertGerade ? 'KI analysiert…' : 'Mit KI analysieren'}
          </button>

          {analyseFehler && <p role="alert">{analyseFehler}</p>}

          {kiVorschlag && (
            <dl className="detail">
              <dt>Vorgeschlagener Titel</dt>
              <dd>{kiVorschlag.vorgeschlagenerTitel ?? '–'}</dd>

              <dt>Vorgeschlagene Priorität</dt>
              <dd>{kiVorschlag.vorgeschlagenePrioritaet ?? '–'}</dd>

              <dt>Vorgeschlagene Abteilungen</dt>
              <dd>
                {kiVorschlag.vorgeschlageneAbteilungen.length > 0
                  ? kiVorschlag.vorgeschlageneAbteilungen.join(', ')
                  : '–'}
              </dd>

              <dt>Fehlende Informationen</dt>
              <dd>{kiVorschlag.fehlendeInformationen ?? '–'}</dd>

              <dt>Vorgeschlagene nächste Schritte</dt>
              <dd>{kiVorschlag.vorgeschlageneNaechsteSchritte ?? '–'}</dd>

              <dt>Manuelle Prüfung erforderlich</dt>
              <dd>{kiVorschlag.manuellePruefungErforderlich ? 'Ja' : 'Nein'}</dd>

              <dt>Prüfstatus</dt>
              <dd>{kiVorschlag.pruefstatus}</dd>
            </dl>
          )}
        </section>
      )}
    </>
  )
}

export default App
