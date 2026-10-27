'use client'

import { useState } from 'react'
import { ArrowDownToLine, ArrowUpFromLine, CreditCard, Plus, Send } from 'lucide-react'
import type { Account, Card } from '@/lib/models'
import { getErrorMessage } from '@/lib/api'
import { useCards } from '@/hooks/use-wallet'
import { Button } from '@/components/ui/button'
import { Skeleton } from '@/components/ui/skeleton'
import { EmptyState, ErrorState } from '@/components/shared/states'
import { CreditCardVisual } from '@/components/wallet/credit-card'
import { CreateCardDialog } from '@/components/wallet/create-card-dialog'
import { CardActionsDialog, type CardAction } from '@/components/wallet/card-actions-dialog'

const quickActions: { action: CardAction; label: string; icon: typeof Send }[] = [
  { action: 'deposit', label: 'Yatır', icon: ArrowDownToLine },
  { action: 'withdraw', label: 'Çek', icon: ArrowUpFromLine },
  { action: 'transfer', label: 'Gönder', icon: Send },
]

export function CardList({ account }: { account: Account }) {
  const { cards, isLoading, error, mutate } = useCards(account.id)
  const [createOpen, setCreateOpen] = useState(false)
  const [active, setActive] = useState<{ card: Card; action: CardAction } | null>(null)

  let content: React.ReactNode
  if (isLoading) {
    content = (
      <div className="grid gap-5 sm:grid-cols-2 xl:grid-cols-3">
        <Skeleton className="aspect-[1.586] rounded-2xl" />
        <Skeleton className="aspect-[1.586] rounded-2xl" />
      </div>
    )
  } else if (error && cards.length === 0) {
    content = <ErrorState message={getErrorMessage(error)} onRetry={() => void mutate()} />
  } else if (cards.length === 0) {
    content = (
      <EmptyState
        icon={CreditCard}
        title="Bu hesaba bağlı kart yok"
        description="Para yatırmak, çekmek veya transfer yapmak için hesabınıza bir kart ekleyin."
        action={
          <Button onClick={() => setCreateOpen(true)}>
            <Plus aria-hidden="true" />
            Kart Ekle
          </Button>
        }
      />
    )
  } else {
    content = (
      <ul className="grid gap-5 sm:grid-cols-2 xl:grid-cols-3">
        {cards.map((card, index) => (
          <li key={card.id} className="flex flex-col gap-3">
            <CreditCardVisual card={card} index={index} />
            <div className="grid grid-cols-3 gap-2">
              {quickActions.map(({ action, label, icon: Icon }) => (
                <Button
                  key={action}
                  variant="outline"
                  size="sm"
                  className="h-9"
                  onClick={() => setActive({ card, action })}
                  aria-label={`${label} — kart son dört hane ${card.number.slice(-4)}`}
                >
                  <Icon aria-hidden="true" />
                  {label}
                </Button>
              ))}
            </div>
          </li>
        ))}
        <li>
          <button
            type="button"
            onClick={() => setCreateOpen(true)}
            className="flex aspect-[1.586] w-full flex-col items-center justify-center gap-2 rounded-2xl border-2 border-dashed text-muted-foreground transition-colors hover:border-primary hover:text-primary"
          >
            <Plus className="size-6" aria-hidden="true" />
            <span className="text-sm font-medium">Yeni kart ekle</span>
          </button>
        </li>
      </ul>
    )
  }

  return (
    <>
      {content}
      <CreateCardDialog accountId={account.id} open={createOpen} onOpenChange={setCreateOpen} />
      <CardActionsDialog
        key={active ? `${active.card.id}-${active.action}` : 'closed'}
        accountId={account.id}
        balance={account.balance}
        card={active?.card ?? null}
        initialAction={active?.action ?? 'deposit'}
        onClose={() => setActive(null)}
      />
    </>
  )
}
