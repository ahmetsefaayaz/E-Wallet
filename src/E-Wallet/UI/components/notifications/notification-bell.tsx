'use client'

import { Bell, CheckCheck } from 'lucide-react'
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
import { useNotifications } from '@/hooks/use-wallet'
import { useMarkAsRead } from '@/hooks/use-mark-as-read'
import { formatRelative } from '@/lib/format'
import { cn } from '@/lib/utils'

export function NotificationBell({ userId, onViewAll }: { userId: string | null; onViewAll: () => void }) {
  const { notifications, unreadCount, isLoading } = useNotifications(userId)
  const markAsRead = useMarkAsRead(userId)
  const preview = notifications.slice(0, 6)

  return (
    <DropdownMenu>
      <DropdownMenuTrigger
        render={
          <Button
            variant="ghost"
            size="icon"
            className="relative"
            aria-label={unreadCount > 0 ? `Bildirimler, ${unreadCount} okunmamış` : 'Bildirimler'}
          />
        }
      >
        <Bell aria-hidden="true" />
        {unreadCount > 0 ? (
          <span
            aria-hidden="true"
            className="absolute -top-0.5 -right-0.5 flex min-w-4.5 items-center justify-center rounded-full bg-destructive px-1 text-[10px] leading-4.5 font-semibold text-white"
          >
            {unreadCount > 9 ? '9+' : unreadCount}
          </span>
        ) : null}
      </DropdownMenuTrigger>
      <DropdownMenuContent align="end" className="w-80 max-w-[calc(100vw-2rem)]">
        <DropdownMenuGroup>
          <DropdownMenuLabel className="flex items-center justify-between">
            <span>Bildirimler</span>
            {unreadCount > 0 ? (
              <span className="text-xs font-normal text-muted-foreground">{unreadCount} okunmamış</span>
            ) : null}
          </DropdownMenuLabel>
        </DropdownMenuGroup>
        <DropdownMenuSeparator />
        <div className="max-h-80 overflow-y-auto">
          {isLoading ? (
            <p className="px-3 py-6 text-center text-sm text-muted-foreground">Yükleniyor…</p>
          ) : preview.length === 0 ? (
            <div className="flex flex-col items-center gap-2 px-3 py-8 text-center">
              <CheckCheck className="size-5 text-muted-foreground" aria-hidden="true" />
              <p className="text-sm text-muted-foreground">Henüz bildiriminiz yok.</p>
            </div>
          ) : (
            preview.map((notification) => (
              <DropdownMenuItem
                key={notification.id}
                onClick={() => {
                  if (!notification.isRead) void markAsRead(notification.id)
                }}
                className="flex items-start gap-2.5 py-2.5"
              >
                <span
                  aria-hidden="true"
                  className={cn(
                    'mt-1.5 size-2 shrink-0 rounded-full',
                    notification.isRead ? 'bg-transparent' : 'bg-primary',
                  )}
                />
                <span className="flex min-w-0 flex-1 flex-col gap-0.5">
                  <span className={cn('truncate text-sm', !notification.isRead && 'font-semibold')}>
                    {notification.title}
                  </span>
                  <span className="line-clamp-2 text-xs text-muted-foreground">{notification.message}</span>
                  <span className="text-[11px] text-muted-foreground/80">{formatRelative(notification.createdAt)}</span>
                </span>
                {!notification.isRead ? <span className="sr-only">Okunmamış</span> : null}
              </DropdownMenuItem>
            ))
          )}
        </div>
        <DropdownMenuSeparator />
        <DropdownMenuItem onClick={onViewAll} className="justify-center text-sm font-medium text-primary">
          Tüm bildirimleri gör
        </DropdownMenuItem>
      </DropdownMenuContent>
    </DropdownMenu>
  )
}
