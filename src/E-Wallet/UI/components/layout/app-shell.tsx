'use client'

import { useState } from 'react'
import type { SessionUser } from '@/lib/auth'
import { useAccounts, useNotifications } from '@/hooks/use-wallet'
import { Sidebar, MobileNav } from '@/components/layout/sidebar'
import { Navbar } from '@/components/layout/navbar'
import type { View } from '@/components/layout/navigation'
import { OverviewView } from '@/components/views/overview-view'
import { CardsView } from '@/components/views/cards-view'
import { TransactionsView } from '@/components/views/transactions-view'
import { NotificationsView } from '@/components/views/notifications-view'

export function AppShell({ user }: { user: SessionUser }) {
  const [view, setView] = useState<View>('overview')
  const [selectedAccountId, setSelectedAccountId] = useState<string | null>(null)
  const accountsState = useAccounts(user.id)
  const { unreadCount } = useNotifications(user.id)

  const selectedAccount =
    accountsState.accounts.find((account) => account.id === selectedAccountId) ?? accountsState.accounts[0] ?? null

  const navigate = (next: View) => {
    setView(next)
    window.scrollTo({ top: 0 })
  }

  const accountProps = {
    accountsState,
    selectedAccount,
    onSelectAccount: setSelectedAccountId,
  }

  return (
    <div className="flex min-h-dvh">
      <Sidebar view={view} onNavigate={navigate} unreadCount={unreadCount} />
      <div className="flex min-w-0 flex-1 flex-col">
        <Navbar user={user} view={view} onNavigate={navigate} />
        <main className="mx-auto w-full max-w-6xl flex-1 px-4 pt-6 pb-28 md:px-8 md:pt-8 md:pb-12">
          {view === 'overview' ? <OverviewView user={user} onNavigate={navigate} {...accountProps} /> : null}
          {view === 'cards' ? <CardsView {...accountProps} /> : null}
          {view === 'transactions' ? <TransactionsView {...accountProps} /> : null}
          {view === 'notifications' ? <NotificationsView userId={user.id} /> : null}
        </main>
      </div>
      <MobileNav view={view} onNavigate={navigate} unreadCount={unreadCount} />
    </div>
  )
}
