'use client'

import { LogOut, Shield } from 'lucide-react'
import type { SessionUser } from '@/lib/auth'
import { clearToken } from '@/lib/auth'
import { Button } from '@/components/ui/button'
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuGroup,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu'
import { BrandLogo } from '@/components/shared/brand-logo'
import { ThemeToggle } from '@/components/layout/theme-toggle'
import { NotificationBell } from '@/components/notifications/notification-bell'
import { viewTitles, type View } from '@/components/layout/navigation'

function initials(user: SessionUser): string {
  const source = user.name ?? user.email ?? '?'
  return source
    .split(/[\s@._-]+/)
    .filter(Boolean)
    .slice(0, 2)
    .map((part) => part[0]?.toUpperCase() ?? '')
    .join('')
}

export function Navbar({ user, view, onNavigate }: { user: SessionUser; view: View; onNavigate: (view: View) => void }) {
  const { title, description } = viewTitles[view]

  return (
    <header className="sticky top-0 z-30 flex h-16 items-center gap-3 border-b bg-background/85 px-4 backdrop-blur md:px-8">
      <BrandLogo className="md:hidden" />
      <div className="hidden min-w-0 flex-1 flex-col md:flex">
        <h1 className="truncate text-lg font-semibold tracking-tight">{title}</h1>
        <p className="truncate text-xs text-muted-foreground">{description}</p>
      </div>
      <div className="ml-auto flex items-center gap-1">
        <ThemeToggle />
        <NotificationBell userId={user.id} onViewAll={() => onNavigate('notifications')} />
        <DropdownMenu>
          <DropdownMenuTrigger
            render={<Button variant="ghost" className="ml-1 h-9 gap-2 px-1.5" aria-label="Kullanıcı menüsü" />}
          >
            <span className="flex size-7 items-center justify-center rounded-full bg-primary text-xs font-semibold text-primary-foreground">
              {initials(user)}
            </span>
            <span className="hidden max-w-40 truncate text-sm font-medium lg:inline">{user.name ?? user.email}</span>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="end" className="w-64">
            <DropdownMenuGroup>
              <DropdownMenuLabel className="flex flex-col gap-0.5">
                <span className="truncate text-sm font-medium text-foreground">{user.name ?? 'Kullanıcı'}</span>
                <span className="truncate text-xs font-normal">{user.email}</span>
              </DropdownMenuLabel>
            </DropdownMenuGroup>
            {user.roles.length > 0 ? (
              <div className="flex items-center gap-1.5 px-2 pb-2 text-xs text-muted-foreground">
                <Shield className="size-3.5" aria-hidden="true" />
                {user.roles.join(', ')}
              </div>
            ) : null}
            <DropdownMenuSeparator />
            <DropdownMenuItem onClick={clearToken} variant="destructive">
              <LogOut aria-hidden="true" />
              Çıkış yap
            </DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
      </div>
    </header>
  )
}
