import type {
  AnfrageDetail,
  AnfrageErstellenRequest,
  AnfrageListItem,
  KiEntscheidungRequest,
  KiVorschlag,
  PrioritaetAktualisierenRequest,
  Stammdaten,
  StatusAktualisierenRequest,
} from './types'

const API_BASE_URL = 'http://localhost:5083'

export async function getAnfragen(): Promise<AnfrageListItem[]> {
  const response = await fetch(`${API_BASE_URL}/api/anfragen`)
  if (!response.ok) {
    throw new Error(`Anfragen konnten nicht geladen werden (Status ${response.status}).`)
  }
  return response.json()
}

export async function getStammdaten(): Promise<Stammdaten> {
  const response = await fetch(`${API_BASE_URL}/api/stammdaten`)
  if (!response.ok) {
    throw new Error(`Stammdaten konnten nicht geladen werden (Status ${response.status}).`)
  }
  return response.json()
}

export async function getAnfrage(id: number): Promise<AnfrageDetail> {
  const response = await fetch(`${API_BASE_URL}/api/anfragen/${id}`)
  if (!response.ok) {
    throw new Error(`Anfrage konnte nicht geladen werden (Status ${response.status}).`)
  }
  return response.json()
}

export async function starteKiAnalyse(id: number): Promise<KiVorschlag> {
  const response = await fetch(`${API_BASE_URL}/api/anfragen/${id}/ki-analyse`, {
    method: 'POST',
  })
  if (!response.ok) {
    throw new Error(`KI-Analyse konnte nicht durchgeführt werden (Status ${response.status}).`)
  }
  return response.json()
}

export async function sendeKiEntscheidung(id: number, entscheidung: KiEntscheidungRequest): Promise<AnfrageDetail> {
  const response = await fetch(`${API_BASE_URL}/api/anfragen/${id}/ki-entscheidung`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(entscheidung),
  })
  if (!response.ok) {
    throw new Error(`Entscheidung konnte nicht gespeichert werden (Status ${response.status}).`)
  }
  return response.json()
}

export async function aktualisiereStatus(
  id: number,
  request: StatusAktualisierenRequest,
): Promise<AnfrageDetail> {
  const response = await fetch(`${API_BASE_URL}/api/anfragen/${id}/status`, {
    method: 'PATCH',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(request),
  })
  if (!response.ok) {
    throw new Error(`Status konnte nicht aktualisiert werden (Status ${response.status}).`)
  }
  return response.json()
}

export async function aktualisierePrioritaet(
  id: number,
  request: PrioritaetAktualisierenRequest,
): Promise<AnfrageDetail> {
  const response = await fetch(`${API_BASE_URL}/api/anfragen/${id}/prioritaet`, {
    method: 'PATCH',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(request),
  })
  if (!response.ok) {
    throw new Error(`Priorität konnte nicht aktualisiert werden (Status ${response.status}).`)
  }
  return response.json()
}

export async function loescheAnfrage(id: number): Promise<void> {
  const response = await fetch(`${API_BASE_URL}/api/anfragen/${id}`, {
    method: 'DELETE',
  })
  if (!response.ok) {
    throw new Error(`Anfrage konnte nicht gelöscht werden (Status ${response.status}).`)
  }
}

export async function erstelleAnfrage(request: AnfrageErstellenRequest): Promise<void> {
  const response = await fetch(`${API_BASE_URL}/api/anfragen`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(request),
  })
  if (!response.ok) {
    throw new Error(`Anfrage konnte nicht erstellt werden (Status ${response.status}).`)
  }
}
