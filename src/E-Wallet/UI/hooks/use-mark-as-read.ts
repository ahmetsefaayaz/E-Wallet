'use client'

import { useCallback } from 'react'
import { useSWRConfig } from 'swr'
import { api } from '@/lib/api'
import { endpoints } from '@/lib/config'

export function useMarkAsRead(userId: string | null | undefined) {
  const { mutate } = useSWRConfig()

  return useCallback(
    async (notificationId: string) => {
      if (!userId) return
      try {
        await api.put(endpoints.markAsRead(notificationId))
      } catch {
        // Error toast is shown by the axios interceptor.
      } finally {
        await mutate(endpoints.notifications(userId))
      }
    },
    [mutate, userId],
  )
}

export function useRefreshWallet() {
  const { mutate } = useSWRConfig()
  return useCallback(
    () =>
      mutate(
        (key) =>
          typeof key === 'string' &&
          (key.startsWith('/account-service') || key.startsWith('/notification-service')),
      ),
    [mutate],
  )
}
