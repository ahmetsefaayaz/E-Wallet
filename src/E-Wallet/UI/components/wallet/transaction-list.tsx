'use client'

import { ArrowDownLeft, ArrowLeftRight, ArrowUpRight, Receipt } from 'lucide-react'
import { getErrorMessage } from '@/lib/api'
import { transactionLabel } from '@/lib/models'
import { formatCurrency, formatDateTime } from '@/lib/format'
import { cn } from '@/lib/utils'
import { useTransactions } from '@/hooks/use-wallet'
import { Skeleton } from '@/components/ui/skeleton'
import { EmptyState, ErrorState } from '@/components/shared/states'

const directionStyles = {
  in: { icon: ArrowDownLeft, chip: 'bg-success/12 text-success', amount: 'text-success', sign: '+' },
  out: { icon: ArrowUpRight, chip: 'bg-destructive/10 text-destructive', amount: 'text-foreground', sign: '−' },
  neutral: { icon: ArrowLeftRight, chip: 'bg-muted text-muted-foreground', amount: 'text-foreground', sign: '' },
} as const

export function TransactionList({ accountId, limit }: { accountId: string; limit?: number }) {
  const { transactions, isLoading, error, mutate } = useTransactions(accountId)
  const items = limit ? transactions.slice(0, limit) : transactions

  if (isLoading) {
    return (
      <div className="flex flex-col gap-2 rounded-xl border bg-card p-3">
        {Array.from({ length: 4 }, (_, i) => (
          <Skeleton key={i} className="h-14 w-full rounded-lg" />
        ))}
      </div>
    )
  }

  if (error && transactions.length === 0) {
    return <ErrorState message={getErrorMessage(error)} onRetry={() => void mutate()} />
  }

  if (items.length === 0) {
    return (
      <EmptyState
        icon={Receipt}
        title="Henüz işlem yok"
        description="Kartlarınızla yaptığınız para yatırma, çekme ve transfer işlemleri burada listelenir."
      />
    )
  }

  return (
    <ul className="divide-y rounded-xl border bg-card">
      {items.map((tx) => {
        const style = directionStyles[tx.direction]
        const Icon = style.icon
        return (
          <li key={tx.id} className="flex items-center gap-3 px-4 py-3.5">
            <span className={cn('flex size-10 shrink-0 items-center justify-center rounded-full', style.chip)}>
              <Icon className="size-4" aria-hidden="true" />
            </span>
            <div className="flex min-w-0 flex-1 flex-col">
              <span className="truncate text-sm font-medium">{tx.description ?? transactionLabel(tx.type)}</span>
              <span className="text-xs text-muted-foreground">{formatDateTime(tx.date)}</span>
            </div>
            <span className={cn('shrink-0 font-mono text-sm font-semibold', style.amount)}>
              {style.sign}
              {formatCurrency(Math.abs(tx.amount))}
            </span>
          </li>
        )
      })}
    </ul>
  )
}
