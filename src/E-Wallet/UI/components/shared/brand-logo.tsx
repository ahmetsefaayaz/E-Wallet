import { Wallet } from 'lucide-react'
import { cn } from '@/lib/utils'

export function BrandLogo({ className, inverted = false }: { className?: string; inverted?: boolean }) {
  return (
    <div className={cn('flex items-center gap-2.5', className)}>
      <span
        className={cn(
          'flex size-9 items-center justify-center rounded-lg',
          inverted ? 'bg-sidebar-primary text-sidebar-primary-foreground' : 'bg-primary text-primary-foreground',
        )}
      >
        <Wallet className="size-5" aria-hidden="true" />
      </span>
      <span className="flex flex-col leading-none">
        <span className="text-base font-semibold tracking-tight">E-Cüzdan</span>
        <span className={cn('text-[11px]', inverted ? 'text-sidebar-foreground/60' : 'text-muted-foreground')}>
          Kurumsal Dijital Cüzdan
        </span>
      </span>
    </div>
  )
}
