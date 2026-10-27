const currencyFormatter = new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' })
const dateTimeFormatter = new Intl.DateTimeFormat('tr-TR', { dateStyle: 'medium', timeStyle: 'short' })
const relativeFormatter = new Intl.RelativeTimeFormat('tr-TR', { numeric: 'auto' })

export function formatCurrency(value: number | null | undefined): string {
  return currencyFormatter.format(Number.isFinite(value) ? (value as number) : 0)
}

function parseDate(iso: string | null | undefined): Date | null {
  if (!iso) return null
  const date = new Date(iso)
  return Number.isNaN(date.getTime()) ? null : date
}

export function formatDateTime(iso: string | null | undefined): string {
  const date = parseDate(iso)
  return date ? dateTimeFormatter.format(date) : '—'
}

export function formatRelative(iso: string | null | undefined): string {
  const date = parseDate(iso)
  if (!date) return '—'
  const seconds = Math.round((date.getTime() - Date.now()) / 1000)
  const units: [Intl.RelativeTimeFormatUnit, number][] = [
    ['day', 86400],
    ['hour', 3600],
    ['minute', 60],
  ]
  for (const [unit, size] of units) {
    if (Math.abs(seconds) >= size) return relativeFormatter.format(Math.round(seconds / size), unit)
  }
  return 'az önce'
}

export function maskCardNumber(cardNumber: string | null | undefined): string {
  const digits = (cardNumber ?? '').replace(/\D/g, '')
  const last4 = digits.slice(-4).padStart(4, '•')
  return `•••• •••• •••• ${last4}`
}

export function shortId(id: string | null | undefined): string {
  if (!id) return '—'
  return id.length > 14 ? `${id.slice(0, 8)}…${id.slice(-4)}` : id
}
