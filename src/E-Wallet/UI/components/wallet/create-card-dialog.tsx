'use client'

import { useState } from 'react'
import { Loader2 } from 'lucide-react'
import { toast } from 'sonner'
import { api } from '@/lib/api'
import { endpoints } from '@/lib/config'
import { useRefreshWallet } from '@/hooks/use-mark-as-read'
import { Button } from '@/components/ui/button'
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'

interface FormState {
  holderName: string
  number: string
  expiry: string
  cvv: string
}

const initialState: FormState = { holderName: '', number: '', expiry: '', cvv: '' }

function formatCardNumber(value: string) {
  return value
    .replace(/\D/g, '')
    .slice(0, 19)
    .replace(/(.{4})/g, '$1 ')
    .trim()
}

function formatExpiry(value: string) {
  const digits = value.replace(/\D/g, '').slice(0, 6)
  return digits.length > 2 ? `${digits.slice(0, 2)}/${digits.slice(2)}` : digits
}

function validate(form: FormState): Partial<Record<keyof FormState, string>> {
  const errors: Partial<Record<keyof FormState, string>> = {}
  if (form.holderName.trim().length < 2) errors.holderName = 'Kart sahibinin adını girin.'
  const digits = form.number.replace(/\D/g, '')
  if (digits.length < 12) errors.number = 'Kart numarası en az 12 haneli olmalı.'

  const match = /^(\d{2})\/(\d{4})$/.exec(form.expiry)
  if (!match) {
    errors.expiry = 'AA/YYYY formatında girin.'
  } else {
    const month = Number(match[1])
    const year = Number(match[2])
    const now = new Date()
    if (month < 1 || month > 12) errors.expiry = 'Geçerli bir ay girin.'
    else if (year < now.getFullYear() || (year === now.getFullYear() && month < now.getMonth() + 1))
      errors.expiry = 'Kartın süresi dolmuş.'
  }

  if (!/^\d{3,4}$/.test(form.cvv)) errors.cvv = 'CVV 3 veya 4 haneli olmalı.'
  return errors
}

export function CreateCardDialog({
  accountId,
  open,
  onOpenChange,
}: {
  accountId: string
  open: boolean
  onOpenChange: (open: boolean) => void
}) {
  const [form, setForm] = useState<FormState>(initialState)
  const [errors, setErrors] = useState<Partial<Record<keyof FormState, string>>>({})
  const [isSubmitting, setIsSubmitting] = useState(false)
  const refresh = useRefreshWallet()

  const update = (field: keyof FormState, value: string) => {
    setForm((prev) => ({ ...prev, [field]: value }))
    setErrors((prev) => ({ ...prev, [field]: undefined }))
  }

  const handleOpenChange = (next: boolean) => {
    if (!next) {
      setForm(initialState)
      setErrors({})
    }
    onOpenChange(next)
  }

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    const nextErrors = validate(form)
    setErrors(nextErrors)
    if (Object.keys(nextErrors).length > 0) return

    setIsSubmitting(true)
    try {
      await api.post(endpoints.cards(accountId), {
        card: {
          cardHolderName: form.holderName.trim(),
          cardNumber: form.number.replace(/\D/g, ''),
          cardExpiryDate: form.expiry,
          cardSecurityNumber: form.cvv,
        },
      })
      toast.success('Kartınız başarıyla eklendi.')
      await refresh()
      handleOpenChange(false)
    } catch {
      // Error toast is shown by the axios interceptor.
    } finally {
      setIsSubmitting(false)
    }
  }

  const fieldError = (field: keyof FormState) =>
    errors[field] ? (
      <p id={`${field}-error`} className="text-xs text-destructive">
        {errors[field]}
      </p>
    ) : null

  return (
    <Dialog open={open} onOpenChange={handleOpenChange}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle>Yeni kart ekle</DialogTitle>
          <DialogDescription>Kart bilgileriniz güvenli bir şekilde hesabınıza bağlanır.</DialogDescription>
        </DialogHeader>
        <form onSubmit={handleSubmit} className="flex flex-col gap-4" noValidate>
          <div className="flex flex-col gap-2">
            <Label htmlFor="holderName">Kart sahibi</Label>
            <Input
              id="holderName"
              autoComplete="cc-name"
              placeholder="Ad Soyad"
              value={form.holderName}
              onChange={(e) => update('holderName', e.target.value)}
              aria-invalid={Boolean(errors.holderName)}
              aria-describedby={errors.holderName ? 'holderName-error' : undefined}
            />
            {fieldError('holderName')}
          </div>
          <div className="flex flex-col gap-2">
            <Label htmlFor="number">Kart numarası</Label>
            <Input
              id="number"
              inputMode="numeric"
              autoComplete="cc-number"
              placeholder="0000 0000 0000 0000"
              className="font-mono"
              value={form.number}
              onChange={(e) => update('number', formatCardNumber(e.target.value))}
              aria-invalid={Boolean(errors.number)}
              aria-describedby={errors.number ? 'number-error' : undefined}
            />
            {fieldError('number')}
          </div>
          <div className="grid grid-cols-2 gap-3">
            <div className="flex flex-col gap-2">
              <Label htmlFor="expiry">Son kullanma</Label>
              <Input
                id="expiry"
                inputMode="numeric"
                autoComplete="cc-exp"
                placeholder="AA/YYYY"
                className="font-mono"
                value={form.expiry}
                onChange={(e) => update('expiry', formatExpiry(e.target.value))}
                aria-invalid={Boolean(errors.expiry)}
                aria-describedby={errors.expiry ? 'expiry-error' : undefined}
              />
              {fieldError('expiry')}
            </div>
            <div className="flex flex-col gap-2">
              <Label htmlFor="cvv">CVV</Label>
              <Input
                id="cvv"
                type="password"
                inputMode="numeric"
                autoComplete="cc-csc"
                placeholder="•••"
                className="font-mono"
                value={form.cvv}
                onChange={(e) => update('cvv', e.target.value.replace(/\D/g, '').slice(0, 4))}
                aria-invalid={Boolean(errors.cvv)}
                aria-describedby={errors.cvv ? 'cvv-error' : undefined}
              />
              {fieldError('cvv')}
            </div>
          </div>
          <DialogFooter className="mt-2">
            <Button type="button" variant="outline" onClick={() => handleOpenChange(false)}>
              Vazgeç
            </Button>
            <Button type="submit" disabled={isSubmitting}>
              {isSubmitting ? <Loader2 className="animate-spin" aria-hidden="true" /> : null}
              Kartı ekle
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  )
}
