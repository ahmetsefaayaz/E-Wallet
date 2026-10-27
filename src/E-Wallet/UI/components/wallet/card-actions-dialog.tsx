'use client'

import { useState } from 'react'
import { ArrowDownToLine, ArrowUpFromLine, Loader2, Send } from 'lucide-react'
import { toast } from 'sonner'
import type { Card } from '@/lib/models'
import { api } from '@/lib/api'
import { endpoints } from '@/lib/config'
import { formatCurrency, maskCardNumber } from '@/lib/format'
import { useRefreshWallet } from '@/hooks/use-mark-as-read'
import { Button } from '@/components/ui/button'
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle } from '@/components/ui/dialog'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs'

export type CardAction = 'deposit' | 'withdraw' | 'transfer'

const GUID_PATTERN = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i

const actionMeta: Record<CardAction, { label: string; submit: string; success: string; icon: typeof Send }> = {
  deposit: { label: 'Para Yatır', submit: 'Yatır', success: 'Para yatırma işlemi başarılı.', icon: ArrowDownToLine },
  withdraw: { label: 'Para Çek', submit: 'Çek', success: 'Para çekme işlemi başarılı.', icon: ArrowUpFromLine },
  transfer: { label: 'Transfer', submit: 'Gönder', success: 'Transfer başarıyla gerçekleşti.', icon: Send },
}

function parseAmount(raw: string): number {
  return Number(raw.replace(',', '.'))
}

function ActionForm({
  action,
  accountId,
  card,
  balance,
  onDone,
}: {
  action: CardAction
  accountId: string
  card: Card
  balance: number
  onDone: () => void
}) {
  const [amount, setAmount] = useState('')
  const [receiverId, setReceiverId] = useState('')
  const [error, setError] = useState<string | null>(null)
  const [isSubmitting, setIsSubmitting] = useState(false)
  const refresh = useRefreshWallet()
  const meta = actionMeta[action]
  const Icon = meta.icon
  const needsFunds = action !== 'deposit'

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    const value = parseAmount(amount)
    if (!Number.isFinite(value) || value <= 0) return setError('Geçerli ve sıfırdan büyük bir tutar girin.')
    if (Math.round(value * 100) !== value * 100) return setError('En fazla 2 ondalık basamak girebilirsiniz.')
    if (needsFunds && value > balance) return setError(`Yetersiz bakiye. Kullanılabilir: ${formatCurrency(balance)}`)

    const receiver = receiverId.trim()
    if (action === 'transfer') {
      if (!GUID_PATTERN.test(receiver)) return setError('Alıcı hesap ID geçerli bir GUID formatında olmalı.')
      if (receiver.toLowerCase() === accountId.toLowerCase()) return setError('Kendi hesabınıza transfer yapamazsınız.')
    }

    setError(null)
    setIsSubmitting(true)
    try {
      const body = action === 'transfer' ? { receiverId: receiver, amount: value } : { amount: value }
      await api.post(endpoints.cardAction(accountId, card.id, action), body)
      toast.success(meta.success)
      await refresh()
      onDone()
    } catch {
      // Error toast is shown by the axios interceptor.
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <form onSubmit={handleSubmit} className="flex flex-col gap-4 pt-2" noValidate>
      {action === 'transfer' ? (
        <div className="flex flex-col gap-2">
          <Label htmlFor="receiverId">Alıcı hesap ID</Label>
          <Input
            id="receiverId"
            placeholder="xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx"
            className="font-mono text-xs"
            autoComplete="off"
            spellCheck={false}
            value={receiverId}
            onChange={(e) => {
              setReceiverId(e.target.value)
              setError(null)
            }}
          />
        </div>
      ) : null}
      <div className="flex flex-col gap-2">
        <div className="flex items-center justify-between">
          <Label htmlFor={`amount-${action}`}>Tutar (TL)</Label>
          {needsFunds ? (
            <button
              type="button"
              className="text-xs font-medium text-primary hover:underline"
              onClick={() => setAmount(String(balance))}
            >
              Tümü: {formatCurrency(balance)}
            </button>
          ) : null}
        </div>
        <Input
          id={`amount-${action}`}
          inputMode="decimal"
          placeholder="0,00"
          className="h-12 font-mono text-xl"
          value={amount}
          onChange={(e) => {
            setAmount(e.target.value.replace(/[^\d.,]/g, ''))
            setError(null)
          }}
          aria-invalid={Boolean(error)}
          aria-describedby={error ? `action-error-${action}` : undefined}
        />
      </div>
      {error ? (
        <p id={`action-error-${action}`} role="alert" className="text-sm text-destructive">
          {error}
        </p>
      ) : null}
      <Button type="submit" size="lg" className="h-10" disabled={isSubmitting}>
        {isSubmitting ? <Loader2 className="animate-spin" aria-hidden="true" /> : <Icon aria-hidden="true" />}
        {meta.submit}
      </Button>
    </form>
  )
}

export function CardActionsDialog({
  accountId,
  balance,
  card,
  initialAction,
  onClose,
}: {
  accountId: string
  balance: number
  card: Card | null
  initialAction: CardAction
  onClose: () => void
}) {
  const [action, setAction] = useState<CardAction>(initialAction)

  return (
    <Dialog open={Boolean(card)} onOpenChange={(open) => !open && onClose()}>
      <DialogContent className="sm:max-w-md">
        {card ? (
          <>
            <DialogHeader>
              <DialogTitle>Kart işlemleri</DialogTitle>
              <DialogDescription className="font-mono">
                {maskCardNumber(card.number)} · Bakiye {formatCurrency(balance)}
              </DialogDescription>
            </DialogHeader>
            <Tabs value={action} onValueChange={(value) => setAction(value as CardAction)}>
              <TabsList className="w-full">
                {(Object.keys(actionMeta) as CardAction[]).map((key) => (
                  <TabsTrigger key={key} value={key}>
                    {actionMeta[key].label}
                  </TabsTrigger>
                ))}
              </TabsList>
              {(Object.keys(actionMeta) as CardAction[]).map((key) => (
                <TabsContent key={key} value={key}>
                  <ActionForm action={key} accountId={accountId} card={card} balance={balance} onDone={onClose} />
                </TabsContent>
              ))}
            </Tabs>
          </>
        ) : null}
      </DialogContent>
    </Dialog>
  )
}
