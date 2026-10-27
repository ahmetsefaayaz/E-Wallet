'use client'

import { useMemo } from 'react'
import useSWR from 'swr'
import { endpoints } from '@/lib/config'
import { fetcher } from '@/lib/api'
import { toAccounts, toCards, toNotifications, toTransactions } from '@/lib/models'

export function useAccounts(userId: string | null) {
  const { data, error, isLoading, mutate } = useSWR(endpoints.accounts, fetcher)
  const accounts = useMemo(() => {
    const all = toAccounts(data)
    if (!userId) return all
    const me = userId.toLowerCase()
    return all.filter((account) => !account.userId || account.userId.toLowerCase() === me)
  }, [data, userId])
  return { accounts, error, isLoading, mutate }
}

export function useCards(accountId: string | null | undefined) {
  const { data, error, isLoading, mutate } = useSWR(accountId ? endpoints.cards(accountId) : null, fetcher)
  const cards = useMemo(() => toCards(data), [data])
  return { cards, error, isLoading, mutate }
}

export function useTransactions(accountId: string | null | undefined) {
  const { data, error, isLoading, mutate } = useSWR(
    accountId ? endpoints.transactions(accountId) : null,
    fetcher,
  )
  const transactions = useMemo(() => toTransactions(data), [data])
  return { transactions, error, isLoading, mutate }
}

export function useNotifications(userId: string | null | undefined) {
  const { data, error, isLoading, mutate } = useSWR(
    userId ? endpoints.notifications(userId) : null,
    fetcher,
    { refreshInterval: 30000 },
  )
  const notifications = useMemo(() => toNotifications(data), [data])
  const unreadCount = useMemo(() => notifications.filter((n) => !n.isRead).length, [notifications])
  return { notifications, unreadCount, error, isLoading, mutate, raw: data }
}
