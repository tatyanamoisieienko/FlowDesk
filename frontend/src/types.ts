export interface AnfrageListItem {
  id: number
  titel: string | null
  originalText: string
  standort: string
  status: string
  prioritaet: string | null
  erstelltAm: string
}

export interface Standort {
  id: number
  name: string
}

export interface Abteilung {
  id: number
  name: string
}

export interface Stammdaten {
  standorte: Standort[]
  abteilungen: Abteilung[]
}

export interface AnfrageErstellenRequest {
  originalText: string
  standortId: number
}
