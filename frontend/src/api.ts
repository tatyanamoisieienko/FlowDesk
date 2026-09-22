import type { AnfrageListItem } from './types'

const API_BASE_URL = 'http://localhost:5083'

export async function getAnfragen(): Promise<AnfrageListItem[]> {
  const response = await fetch(`${API_BASE_URL}/api/anfragen`)
  if (!response.ok) {
    throw new Error(`Anfragen konnten nicht geladen werden (Status ${response.status}).`)
  }
  return response.json()
}
