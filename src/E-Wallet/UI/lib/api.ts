import axios, { AxiosError } from 'axios'
import { toast } from 'sonner'
import { API_BASE_URL, endpoints } from '@/lib/config'
import { clearToken, getToken } from '@/lib/auth'

export const api = axios.create({
  baseURL: API_BASE_URL,
  timeout: 15000,
  headers: { 'Content-Type': 'application/json' },
})

api.interceptors.request.use((config) => {
  const token = getToken()
  if (token) config.headers.set('Authorization', `Bearer ${token}`)
  return config
})

const STATUS_FALLBACKS: Record<number, string> = {
  400: 'Geçersiz istek. Lütfen girdiğiniz bilgileri kontrol edin.',
  403: 'Bu işlem için yetkiniz bulunmuyor.',
  404: 'İstenen kayıt bulunamadı.',
  409: 'İşlem mevcut kayıtlarla çakışıyor.',
  500: 'Sunucuda beklenmeyen bir hata oluştu.',
}

export function getErrorMessage(error: unknown): string {
  if (!axios.isAxiosError(error)) return 'Beklenmeyen bir hata oluştu.'
  if (!error.response) {
    return `Sunucuya ulaşılamadı. Gateway (${API_BASE_URL}) çalışıyor ve sertifikası güvenilir mi?`
  }

  const data: unknown = error.response.data
  if (typeof data === 'string' && data.trim().length > 0 && data.length < 300) return data

  if (data && typeof data === 'object') {
    const record = data as Record<string, unknown>
    const validation = record.errors
    if (validation && typeof validation === 'object') {
      const first = Object.values(validation as Record<string, unknown>)
        .flat()
        .find((v): v is string => typeof v === 'string')
      if (first) return first
    }
    for (const key of ['detail', 'message', 'error', 'title']) {
      const value = record[key]
      if (typeof value === 'string' && value.length > 0) return value
    }
  }

  return STATUS_FALLBACKS[error.response.status] ?? `İstek başarısız oldu (${error.response.status}).`
}

api.interceptors.response.use(
  (response) => response,
  (error: AxiosError) => {
    const status = error.response?.status
    const url = error.config?.url ?? ''
    const method = (error.config?.method ?? 'get').toUpperCase()
    const isLoginRequest = url.includes(endpoints.login)

    if (status === 401 && isLoginRequest) {
      toast.error('E-posta veya şifre hatalı.', { id: 'login-failed' })
    } else if (status === 401) {
      clearToken()
      toast.error('Oturumunuzun süresi doldu. Lütfen tekrar giriş yapın.', { id: 'session-expired' })
    } else if (method !== 'GET') {
      // Read requests render their own inline error state; don't toast once per failed endpoint or poll.
      const toastId = error.response ? `${status}-${url}` : 'gateway-unreachable'
      toast.error(getErrorMessage(error), { id: toastId })
    }

    return Promise.reject(error)
  },
)

export const fetcher = (url: string) => api.get(url).then((res) => res.data as unknown)
