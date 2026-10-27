import { Nfc } from 'lucide-react'
import type { Card } from '@/lib/models'
import { maskCardNumber } from '@/lib/format'
import { cn } from '@/lib/utils'

const tones = [
  'from-[oklch(0.3_0.06_262)] to-[oklch(0.22_0.05_262)]',
  'from-[oklch(0.5_0.1_195)] to-[oklch(0.33_0.08_215)]',
  'from-[oklch(0.42_0.06_240)] to-[oklch(0.26_0.05_262)]',
]

export function CreditCardVisual({ card, index = 0, className }: { card: Card; index?: number; className?: string }) {
  return (
    <div
      className={cn(
        'relative flex aspect-[1.586] w-full flex-col justify-between overflow-hidden rounded-2xl bg-gradient-to-br p-5 text-white shadow-md',
        tones[index % tones.length],
        className,
      )}
    >
      <div aria-hidden="true" className="pointer-events-none absolute -right-10 -bottom-16 size-48 rounded-full bg-white/10" />
      <div aria-hidden="true" className="pointer-events-none absolute -top-12 -left-8 size-36 rounded-full bg-white/5" />
      <div className="relative flex items-center justify-between">
        <span className="text-sm font-semibold tracking-wide">E-Cüzdan</span>
        <Nfc className="size-5 opacity-80" aria-hidden="true" />
      </div>
      <div className="relative flex flex-col gap-4">
        <span aria-hidden="true" className="h-7 w-10 rounded-md bg-gradient-to-br from-amber-200 to-amber-400/80" />
        <p className="font-mono text-lg tracking-[0.18em] sm:text-xl" aria-label={`Kart numarası son dört hane ${card.number.slice(-4)}`}>
          {maskCardNumber(card.number)}
        </p>
        <div className="flex items-end justify-between gap-4 text-xs">
          <div className="flex min-w-0 flex-col gap-0.5">
            <span className="text-[10px] tracking-wider text-white/60 uppercase">Kart Sahibi</span>
            <span className="truncate text-sm font-medium uppercase">{card.holderName}</span>
          </div>
          <div className="flex shrink-0 flex-col items-end gap-0.5">
            <span className="text-[10px] tracking-wider text-white/60 uppercase">SKT</span>
            <span className="font-mono text-sm">{card.expiry}</span>
          </div>
        </div>
      </div>
    </div>
  )
}
