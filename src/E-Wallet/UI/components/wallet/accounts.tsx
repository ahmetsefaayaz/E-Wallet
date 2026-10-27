'use client'

import { useState } from 'react'
import { Check, Loader2, Plus, Wallet } from 'lucide-react'
import { toast } from 'sonner'
import type { Account } from '@/lib/models'
import { api, getErrorMessage } from '@/lib/api'
import { endpoints } from '@/lib/config'
import { formatCurrency, shortId } from '@/lib/format'
import { cn } from '@/lib/utils'
import type { useAccounts } from '@/hooks/use-wallet'
import { useRefreshWallet } from '@/hooks/use-mark-as-read'
import { Button } from '@/components/ui/button'
import { Skeleton } from '@/components/ui/skeleton'
import { CopyButton } from '@/components/shared/copy-button'
import { EmptyState, ErrorState } from '@/components/shared/states'

export type AccountsState = ReturnType<typeof useAccounts>

export interface AccountProps {
  accountsState: AccountsState
  selectedAccount: Account | null
  onSelectAccount: (id: string) => void
}

export function CreateAccountButton({ variant = 'default' }: { variant?: 'default' | 'outline' }) {
  const [isCreating, setIsCreating] = useState(false)
  const refresh = useRefreshWallet()

  const handleCreate = async () => {
    setIsCreating(true)
    try {
      await api.post(endpoints.accounts)
      toast.success('Yeni hesabınız oluşturuldu.')
      await refresh()
    } catch {
      // Error toast is shown by the axios interceptor.
    } finally {
      setIsCreating(false)
    }
  }

  return (
    <Button variant={variant} onClick={handleCreate} disabled={isCreating}>
      {isCreating ? <Loader2 className="animate-spin" aria-hidden="true" /> : <Plus aria-hidden="true" />}
      Yeni Hesap Aç
    </Button>
  )
}

export function AccountsGate({
  accountsState,
  children,
}: {
  accountsState: AccountsState
  children: React.ReactNode
}) {
  const { accounts, isLoading, error, mutate } = accountsState

  if (isLoading) {
    return (
      <div className="flex flex-col gap-4">
        <Skeleton className="h-44 w-full rounded-2xl" />
        <div className="grid gap-4 sm:grid-cols-3">
          <Skeleton className="h-24 rounded-xl" />
          <Skeleton className="h-24 rounded-xl" />
          <Skeleton className="h-24 rounded-xl" />
        </div>
      </div>
    )
  }

  if (error && accounts.length === 0) {
    return <ErrorState message={getErrorMessage(error)} onRetry={() => void mutate()} />
  }

  if (accounts.length === 0) {
    return (
      <EmptyState
        icon={Wallet}
        title="Henüz bir hesabınız yok"
        description="Kart ekleyip para transferi yapabilmek için önce bir cüzdan hesabı oluşturun."
        action={<CreateAccountButton />}
      />
    )
  }

  return <>{children}</>
}

export function AccountSwitcher({ accountsState, selectedAccount, onSelectAccount }: AccountProps) {
  const { accounts } = accountsState
  if (accounts.length < 2) return null

  return (
    <div role="radiogroup" aria-label="Hesap seçimi" className="flex gap-2 overflow-x-auto pb-1">
      {accounts.map((account, index) => {
        const active = account.id === selectedAccount?.id
        return (
          <button
            key={account.id}
            type="button"
            role="radio"
            aria-checked={active}
            onClick={() => onSelectAccount(account.id)}
            className={cn(
              'flex shrink-0 items-center gap-2 rounded-full border px-3.5 py-1.5 text-sm transition-colors',
              active
                ? 'border-primary bg-accent font-medium text-accent-foreground'
                : 'bg-card text-muted-foreground hover:text-foreground',
            )}
          >
            {active ? <Check className="size-3.5" aria-hidden="true" /> : null}
            Hesap {index + 1}
            <span className="font-mono text-xs opacity-70">{formatCurrency(account.balance)}</span>
          </button>
        )
      })}
    </div>
  )
}

export function BalanceHero({ account, index }: { account: Account; index: number }) {
  return (
    <section
      aria-label="Seçili hesap bakiyesi"
      className="relative overflow-hidden rounded-2xl bg-sidebar p-6 text-sidebar-foreground sm:p-8"
    >
      <div aria-hidden="true" className="pointer-events-none absolute -top-24 -right-16 size-72 rounded-full bg-sidebar-primary/20 blur-3xl" />
      <div className="relative flex flex-col gap-6 sm:flex-row sm:items-end sm:justify-between">
        <div className="flex flex-col gap-2">
          <p className="text-sm text-sidebar-foreground/70">Hesap {index + 1} · Kullanılabilir bakiye</p>
          <p className="font-mono text-4xl font-semibold tracking-tight text-sidebar-accent-foreground sm:text-5xl">
            {formatCurrency(account.balance)}
          </p>
        </div>
        <div className="flex items-center gap-1 rounded-lg bg-sidebar-accent/70 py-1 pr-1 pl-3">
          <span className="text-xs text-sidebar-foreground/60">Hesap ID</span>
          <span className="font-mono text-xs text-sidebar-accent-foreground" title={account.id}>
            {shortId(account.id)}
          </span>
          <CopyButton
            value={account.id}
            label="Hesap ID"
            className="text-sidebar-foreground hover:bg-sidebar-accent hover:text-sidebar-accent-foreground"
          />
        </div>
      </div>
    </section>
  )
}
