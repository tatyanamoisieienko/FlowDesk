import type { AnfrageDetail, AnfrageErstellenRequest, AnfrageListItem, Stammdaten } from './types'

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
