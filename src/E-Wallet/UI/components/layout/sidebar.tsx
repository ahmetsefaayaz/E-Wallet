'use client'

import { LogOut, ShieldCheck } from 'lucide-react'
import { clearToken } from '@/lib/auth'
import { cn } from '@/lib/utils'
import { BrandLogo } from '@/components/shared/brand-logo'
import { navItems, type View } from '@/components/layout/navigation'

export function Sidebar({
  view,
  onNavigate,
  unreadCount,
}: {
  view: View
  onNavigate: (view: View) => void
  unreadCount: number
}) {
  return (
    <aside className="sticky top-0 hidden h-dvh w-64 shrink-0 flex-col border-r border-sidebar-border bg-sidebar text-sidebar-foreground md:flex">
      <div className="px-5 py-5">
        <BrandLogo inverted />
      </div>
      <nav aria-label="Ana menü" className="flex-1 px-3">
        <ul className="flex flex-col gap-1">
          {navItems.map(({ id, label, icon: Icon }) => {
            const active = view === id
            return (
              <li key={id}>
                <button
                  type="button"
                  onClick={() => onNavigate(id)}
                  aria-current={active ? 'page' : undefined}
                  className={cn(
                    'flex w-full items-center gap-3 rounded-lg px-3 py-2.5 text-sm font-medium transition-colors',
                    active
                      ? 'bg-sidebar-accent text-sidebar-accent-foreground'
                      : 'text-sidebar-foreground/70 hover:bg-sidebar-accent/60 hover:text-sidebar-accent-foreground',
                  )}
                >
                  <Icon className={cn('size-4', active && 'text-sidebar-primary')} aria-hidden="true" />
                  <span className="flex-1 text-left">{label}</span>
                  {id === 'notifications' && unreadCount > 0 ? (
                    <span className="rounded-full bg-sidebar-primary px-2 py-0.5 text-[11px] font-semibold text-sidebar-primary-foreground">
                      {unreadCount}
                      <span className="sr-only"> okunmamış</span>
                    </span>
                  ) : null}
                </button>
              </li>
            )
          })}
        </ul>
      </nav>
      <div className="flex flex-col gap-3 p-3">
        <div className="flex items-start gap-2.5 rounded-lg bg-sidebar-accent/60 p-3">
          <ShieldCheck className="mt-0.5 size-4 shrink-0 text-sidebar-primary" aria-hidden="true" />
          <p className="text-xs leading-relaxed text-sidebar-foreground/70">
            Oturumunuz uçtan uca şifreli JWT ile korunmaktadır.
          </p>
        </div>
        <button
          type="button"
          onClick={clearToken}
          className="flex items-center gap-3 rounded-lg px-3 py-2.5 text-sm font-medium text-sidebar-foreground/70 transition-colors hover:bg-sidebar-accent/60 hover:text-sidebar-accent-foreground"
        >
          <LogOut className="size-4" aria-hidden="true" />
          Çıkış yap
        </button>
      </div>
    </aside>
  )
}

export function MobileNav({
  view,
  onNavigate,
  unreadCount,
}: {
  view: View
  onNavigate: (view: View) => void
  unreadCount: number
}) {
  return (
    <nav
      aria-label="Mobil menü"
      className="fixed inset-x-0 bottom-0 z-40 border-t bg-card/95 pb-[env(safe-area-inset-bottom)] backdrop-blur md:hidden"
    >
      <ul className="grid grid-cols-4">
        {navItems.map(({ id, label, icon: Icon }) => {
          const active = view === id
          return (
            <li key={id}>
              <button
                type="button"
                onClick={() => onNavigate(id)}
                aria-current={active ? 'page' : undefined}
                className={cn(
                  'relative flex w-full flex-col items-center gap-1 py-2.5 text-[11px] font-medium',
                  active ? 'text-primary' : 'text-muted-foreground',
                )}
              >
                <Icon className="size-5" aria-hidden="true" />
                {label}
                {id === 'notifications' && unreadCount > 0 ? (
                  <span className="absolute top-1.5 left-1/2 ml-2 size-2 rounded-full bg-destructive" aria-hidden="true" />
                ) : null}
              </button>
            </li>
          )
        })}
      </ul>
    </nav>
  )
}
