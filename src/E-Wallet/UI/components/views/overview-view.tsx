'use client'

import { ArrowRight, Landmark, Layers, Wallet } from 'lucide-react'
import type { SessionUser } from '@/lib/auth'
import { formatCurrency } from '@/lib/format'
import { Button } from '@/components/ui/button'
import { SectionHeader } from '@/components/shared/states'
import type { View } from '@/components/layout/navigation'
import {
  AccountsGate,
  AccountSwitcher,
  BalanceHero,
  CreateAccountButton,
  type AccountProps,
} from '@/components/wallet/accounts'
import { CardList } from '@/components/wallet/card-list'
import { TransactionList } from '@/components/wallet/transaction-list'

function StatTile({ icon: Icon, label, value }: { icon: typeof Wallet; label: string; value: string }) {
  return (
    <div className="flex items-center gap-4 rounded-xl border bg-card p-4">
      <span className="flex size-10 items-center justify-center rounded-lg bg-accent text-accent-foreground">
        <Icon className="size-5" aria-hidden="true" />
      </span>
      <div className="flex min-w-0 flex-col">
        <span className="text-xs text-muted-foreground">{label}</span>
        <span className="truncate font-mono text-lg font-semibold">{value}</span>
      </div>
    </div>
  )
}

export function OverviewView({
  user,
  onNavigate,
  ...accountProps
}: AccountProps & { user: SessionUser; onNavigate: (view: View) => void }) {
  const { accountsState, selectedAccount } = accountProps
  const { accounts } = accountsState
  const totalBalance = accounts.reduce((sum, account) => sum + account.balance, 0)
  const selectedIndex = accounts.findIndex((account) => account.id === selectedAccount?.id)
  const greetingName = user.name ?? user.email?.split('@')[0]

  return (
    <div className="flex flex-col gap-8">
      <div className="flex flex-wrap items-center justify-between gap-4">
        <div className="flex flex-col gap-1">
          <h2 className="text-2xl font-semibold tracking-tight text-balance">
            Merhaba{greetingName ? `, ${greetingName}` : ''}
          </h2>
          <p className="text-sm text-muted-foreground">Cüzdanınızdaki son durum aşağıda.</p>
        </div>
        <CreateAccountButton variant="outline" />
      </div>

      <AccountsGate accountsState={accountsState}>
        {selectedAccount ? (
          <>
            <div className="flex flex-col gap-4">
              <AccountSwitcher {...accountProps} />
              <BalanceHero account={selectedAccount} index={Math.max(selectedIndex, 0)} />
              <div className="grid gap-4 sm:grid-cols-3">
                <StatTile icon={Wallet} label="Toplam bakiye" value={formatCurrency(totalBalance)} />
                <StatTile icon={Landmark} label="Hesap sayısı" value={String(accounts.length)} />
                <StatTile icon={Layers} label="Para birimi" value="TRY (₺)" />
              </div>
            </div>

            <section className="flex flex-col gap-4">
              <SectionHeader
                title="Kartlarım"
                description="Seçili hesaba bağlı kartlar"
                action={
                  <Button variant="ghost" size="sm" onClick={() => onNavigate('cards')}>
                    Yönet
                    <ArrowRight aria-hidden="true" />
                  </Button>
                }
              />
              <CardList account={selectedAccount} />
            </section>

            <section className="flex flex-col gap-4">
              <SectionHeader
                title="Son işlemler"
                action={
                  <Button variant="ghost" size="sm" onClick={() => onNavigate('transactions')}>
                    Tümünü gör
                    <ArrowRight aria-hidden="true" />
                  </Button>
                }
              />
              <TransactionList accountId={selectedAccount.id} limit={5} />
            </section>
          </>
        ) : null}
      </AccountsGate>
    </div>
  )
}
