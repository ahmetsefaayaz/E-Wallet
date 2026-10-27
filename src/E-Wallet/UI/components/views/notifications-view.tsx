'use client'

import { useState } from 'react'
import { ArrowDownToLine, ArrowUpFromLine, Bell, Check, CreditCard, Loader2, Send } from 'lucide-react'
import type { AppNotification } from '@/lib/models'
import { getErrorMessage } from '@/lib/api'
import { formatDateTime, formatRelative } from '@/lib/format'
import { cn } from '@/lib/utils'
import { useNotifications } from '@/hooks/use-wallet'
import { useMarkAsRead } from '@/hooks/use-mark-as-read'
import { Button } from '@/components/ui/button'
import { Skeleton } from '@/components/ui/skeleton'
import { Tabs, TabsList, TabsTrigger } from '@/components/ui/tabs'
import { EmptyState, ErrorState } from '@/components/shared/states'

type Filter = 'all' | 'unread'

function iconFor(type: string) {
  const t = type.toLowerCase()
  if (t.includes('card')) return CreditCard
  if (t.includes('deposit')) return ArrowDownToLine
  if (t.includes('withdraw')) return ArrowUpFromLine
  if (t.includes('transfer')) return Send
  return Bell
}

function NotificationRow({ notification, onMark }: { notification: AppNotification; onMark: (id: string) => Promise<void> }) {
  const [pending, setPending] = useState(false)
  const Icon = iconFor(notification.type)

  const handleMark = async () => {
    setPending(true)
    await onMark(notification.id)
    setPending(false)
  }

  return (
    <li className={cn('flex items-start gap-3 px-4 py-4', !notification.isRead && 'bg-accent/40')}>
      <span
        className={cn(
          'flex size-10 shrink-0 items-center justify-center rounded-full',
          notification.isRead ? 'bg-muted text-muted-foreground' : 'bg-primary text-primary-foreground',
        )}
      >
        <Icon className="size-4" aria-hidden="true" />
      </span>
      <div className="flex min-w-0 flex-1 flex-col gap-0.5">
        <div className="flex flex-wrap items-center gap-2">
          <p className={cn('text-sm', notification.isRead ? 'font-medium' : 'font-semibold')}>{notification.title}</p>
          {!notification.isRead ? (
            <span className="rounded-full bg-primary/10 px-2 py-0.5 text-[10px] font-semibold text-primary">Yeni</span>
          ) : null}
        </div>
        <p className="text-sm break-words text-muted-foreground">{notification.message}</p>
        <time
          className="text-xs text-muted-foreground/80"
          dateTime={notification.createdAt ?? undefined}
          title={formatDateTime(notification.createdAt)}
        >
          {formatRelative(notification.createdAt)}
        </time>
      </div>
      {!notification.isRead ? (
        <Button variant="ghost" size="sm" onClick={handleMark} disabled={pending} className="shrink-0">
          {pending ? <Loader2 className="animate-spin" aria-hidden="true" /> : <Check aria-hidden="true" />}
          <span className="hidden sm:inline">Okundu</span>
          <span className="sr-only sm:hidden">Okundu olarak işaretle</span>
        </Button>
      ) : null}
    </li>
  )
}

export function NotificationsView({ userId }: { userId: string | null }) {
  const { notifications, unreadCount, isLoading, error, mutate } = useNotifications(userId)
  const markAsRead = useMarkAsRead(userId)
  const [filter, setFilter] = useState<Filter>('all')
  const visible = filter === 'unread' ? notifications.filter((n) => !n.isRead) : notifications

  return (
    <div className="flex flex-col gap-5">
      <Tabs value={filter} onValueChange={(value) => setFilter(value as Filter)}>
        <TabsList>
          <TabsTrigger value="all">Tümü ({notifications.length})</TabsTrigger>
          <TabsTrigger value="unread">Okunmamış ({unreadCount})</TabsTrigger>
        </TabsList>
      </Tabs>

      {isLoading ? (
        <div className="flex flex-col gap-2 rounded-xl border bg-card p-3">
          {Array.from({ length: 4 }, (_, i) => (
            <Skeleton key={i} className="h-16 w-full rounded-lg" />
          ))}
        </div>
      ) : error && notifications.length === 0 ? (
        <ErrorState message={getErrorMessage(error)} onRetry={() => void mutate()} />
      ) : visible.length === 0 ? (
        <EmptyState
          icon={Bell}
          title={filter === 'unread' ? 'Okunmamış bildirim yok' : 'Henüz bildiriminiz yok'}
          description="Kart ve para hareketlerinizle ilgili bildirimler burada görünecek."
        />
      ) : (
        <ul className="divide-y overflow-hidden rounded-xl border bg-card">
          {visible.map((notification) => (
            <NotificationRow key={notification.id} notification={notification} onMark={markAsRead} />
          ))}
        </ul>
      )}
    </div>
  )
}
