'use client'

import { SWRConfig } from 'swr'
import { Loader2 } from 'lucide-react'
import { fetcher } from '@/lib/api'
import { useIsClient, useSession } from '@/hooks/use-session'
import { LoginScreen } from '@/components/auth/login-screen'
import { AppShell } from '@/components/layout/app-shell'

export function WalletApp() {
  const isClient = useIsClient()
  const { token, user } = useSession()

  if (!isClient) {
    return (
      <div className="flex min-h-dvh items-center justify-center" role="status">
        <Loader2 className="size-6 animate-spin text-primary" aria-hidden="true" />
        <span className="sr-only">Yükleniyor</span>
      </div>
    )
  }

  if (!token || !user) return <LoginScreen />

  return (
    <SWRConfig
      key={token}
      value={{ provider: () => new Map(), fetcher, shouldRetryOnError: false, revalidateOnFocus: false }}
    >
      <AppShell user={user} />
    </SWRConfig>
  )
}
