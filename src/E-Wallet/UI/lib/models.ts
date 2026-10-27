export interface Account {
  id: string
  userId: string | null
  balance: number
}

export interface Card {
  id: string
  accountId: string | null
  holderName: string
  number: string
  expiry: string
}

export type TransactionDirection = 'in' | 'out' | 'neutral'

export interface Transaction {
  id: string
  amount: number
  type: string
  direction: TransactionDirection
  date: string | null
  description: string | null
}

export interface AppNotification {
  id: string
  title: string
  message: string
  type: string
  isRead: boolean
  createdAt: string | null
}

type UnknownRecord = Record<string, unknown>

function isRecord(value: unknown): value is UnknownRecord {
  return typeof value === 'object' && value !== null && !Array.isArray(value)
}

function str(value: unknown): string | null {
  if (typeof value === 'string') return value
  if (typeof value === 'number') return String(value)
  return null
}

function num(value: unknown): number {
  const parsed = typeof value === 'number' ? value : Number(value)
  return Number.isFinite(parsed) ? parsed : 0
}

function pickList(data: unknown, keys: string[]): unknown[] {
  if (Array.isArray(data)) return data
  if (!isRecord(data)) return []
  for (const key of [...keys, 'data', 'items']) {
    const value = data[key]
    if (Array.isArray(value)) return value
    if (isRecord(value)) {
      const nested = pickList(value, [])
      if (nested.length > 0) return nested
    }
  }
  return []
}

export function toAccounts(data: unknown): Account[] {
  return pickList(data, ['paginatedResult', 'accounts'])
    .filter(isRecord)
    .map((item) => ({
      id: str(item.id) ?? '',
      userId: str(item.userId),
      balance: num(item.accountBalance ?? item.balance),
    }))
    .filter((account) => account.id.length > 0)
}

export function toCards(data: unknown): Card[] {
  return pickList(data, ['cards', 'paginatedResult'])
    .filter(isRecord)
    .map((item) => ({
      id: str(item.id) ?? '',
      accountId: str(item.accountId),
      holderName: str(item.cardHolderName) ?? 'Kart Sahibi',
      number: str(item.cardNumber) ?? '',
      expiry: str(item.cardExpiryDate) ?? '--/----',
    }))
    .filter((card) => card.id.length > 0)
}

const TRANSACTION_TYPE_LABELS: Record<string, string> = {
  deposit: 'Para Yatırma',
  withdrawal: 'Para Çekme',
  withdraw: 'Para Çekme',
  transfer: 'Transfer',
}

function resolveDirection(type: string, amount: number): TransactionDirection {
  if (amount < 0) return 'out'
  const normalized = type.toLowerCase()
  if (normalized.includes('deposit') || normalized.includes('incoming') || normalized.includes('receiv')) return 'in'
  if (normalized.includes('withdraw') || normalized.includes('transfer') || normalized.includes('outgoing')) return 'out'
  return 'neutral'
}

export function transactionLabel(type: string): string {
  return TRANSACTION_TYPE_LABELS[type.toLowerCase()] ?? (type || 'İşlem')
}

export function toTransactions(data: unknown): Transaction[] {
  return pickList(data, ['transactions', 'paginatedResult'])
    .filter(isRecord)
    .map((item, index) => {
      const amount = num(item.amount ?? item.transactionAmount)
      const type = str(item.type ?? item.transactionType) ?? ''
      return {
        id: str(item.id) ?? `tx-${index}`,
        amount,
        type,
        direction: resolveDirection(type, amount),
        date: str(item.createdAt ?? item.transactionDate ?? item.date),
        description: str(item.description),
      }
    })
    .sort((a, b) => (b.date ?? '').localeCompare(a.date ?? ''))
}

export function toNotifications(data: unknown): AppNotification[] {
  return pickList(data, ['notifications', 'paginatedResult'])
    .filter(isRecord)
    .map((item) => ({
      id: str(item.id) ?? '',
      title: str(item.title) ?? 'Bildirim',
      message: str(item.message) ?? '',
      type: str(item.type) ?? '',
      isRead: item.isRead === true,
      createdAt: str(item.createdAt),
    }))
    .filter((n) => n.id.length > 0)
    .sort((a, b) => (b.createdAt ?? '').localeCompare(a.createdAt ?? ''))
}
