'use client'

import { useMemo, useSyncExternalStore } from 'react'
import { getSessionUser, getToken, isTokenExpired, subscribeToken } from '@/lib/auth'

const noopSubscribe = () => () => {}

export function useIsClient() {
  return useSyncExternalStore(
    noopSubscribe,
    () => true,
    () => false,
  )
}

export function useSession() {
  const rawToken = useSyncExternalStore(subscribeToken, getToken, () => null)
  const token = rawToken && !isTokenExpired(rawToken) ? rawToken : null
  const user = useMemo(() => getSessionUser(token), [token])
  return { token: user ? token : null, user }
}
