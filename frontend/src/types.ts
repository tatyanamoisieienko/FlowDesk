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

export interface AnfrageDetail {
  id: number
  originalText: string
  titel: string | null
  standort: string
  status: string
  prioritaet: string | null
  abteilungen: string[]
  erstelltAm: string
  aktualisiertAm: string
  kiVorschlag: KiVorschlag | null
}

export interface KiVorschlag {
  id: number
  anfrageId: number
  vorgeschlagenerTitel: string | null
  vorgeschlagenePrioritaet: string | null
  vorgeschlageneAbteilungen: string[]
  fehlendeInformationen: string | null
  vorgeschlageneNaechsteSchritte: string | null
  manuellePruefungErforderlich: boolean
  pruefstatus: string
}

export interface KiEntscheidungRequest {
  pruefstatus: 'Angenommen' | 'Geaendert' | 'Abgelehnt'
  titel?: string | null
  prioritaet?: string | null
  abteilungIds?: number[] | null
}

export interface StatusAktualisierenRequest {
  status: 'Neu' | 'InBearbeitung' | 'Erledigt'
}
