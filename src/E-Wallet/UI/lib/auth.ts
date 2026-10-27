const TOKEN_KEY = 'ewallet_token'
const AUTH_EVENT = 'ewallet:auth-changed'

const NAME_ID_CLAIM = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'
const EMAIL_CLAIM = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'
const NAME_CLAIM = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'
const ROLE_CLAIM = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'

export interface SessionUser {
  id: string | null
  email: string | null
  name: string | null
  roles: string[]
}

type JwtPayload = Record<string, unknown>

export function getToken(): string | null {
  if (typeof window === 'undefined') return null
  try {
    return window.localStorage.getItem(TOKEN_KEY)
  } catch {
    return null
  }
}

export function setToken(token: string) {
  try {
    window.localStorage.setItem(TOKEN_KEY, token)
  } finally {
    window.dispatchEvent(new Event(AUTH_EVENT))
  }
}

export function clearToken() {
  try {
    window.localStorage.removeItem(TOKEN_KEY)
  } finally {
    window.dispatchEvent(new Event(AUTH_EVENT))
  }
}

export function subscribeToken(callback: () => void) {
  window.addEventListener(AUTH_EVENT, callback)
  window.addEventListener('storage', callback)
  return () => {
    window.removeEventListener(AUTH_EVENT, callback)
    window.removeEventListener('storage', callback)
  }
}

export function decodeJwt(token: string | null | undefined): JwtPayload | null {
  try {
    const segment = token?.split('.')?.[1]
    if (!segment) return null
    const base64 = segment.replace(/-/g, '+').replace(/_/g, '/')
    const padded = base64 + '='.repeat((4 - (base64.length % 4)) % 4)
    const binary = atob(padded)
    const bytes = Uint8Array.from(binary, (char) => char.charCodeAt(0))
    const parsed: unknown = JSON.parse(new TextDecoder().decode(bytes))
    return parsed && typeof parsed === 'object' ? (parsed as JwtPayload) : null
  } catch {
    return null
  }
}

function readClaim(payload: JwtPayload, keys: string[]): string | null {
  for (const key of keys) {
    const value = payload?.[key]
    if (typeof value === 'string' && value.length > 0) return value
    if (Array.isArray(value) && typeof value[0] === 'string') return value[0]
  }
  return null
}

function readClaimList(payload: JwtPayload, keys: string[]): string[] {
  for (const key of keys) {
    const value = payload?.[key]
    if (typeof value === 'string') return [value]
    if (Array.isArray(value)) return value.filter((v): v is string => typeof v === 'string')
  }
  return []
}

export function isTokenExpired(token: string): boolean {
  const exp = decodeJwt(token)?.exp
  return typeof exp === 'number' && exp * 1000 <= Date.now()
}

export function getSessionUser(token: string | null): SessionUser | null {
  if (!token) return null
  const payload = decodeJwt(token)
  if (!payload) return null
  return {
    // .NET emits the user id under the long nameidentifier claim, not `sub`.
    id: readClaim(payload, [NAME_ID_CLAIM]),
    email: readClaim(payload, [EMAIL_CLAIM, 'email']),
    name: readClaim(payload, [NAME_CLAIM, 'unique_name', 'name']),
    roles: readClaimList(payload, [ROLE_CLAIM, 'role']),
  }
}
