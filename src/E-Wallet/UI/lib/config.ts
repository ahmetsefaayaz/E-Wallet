export const API_BASE_URL = 'https://localhost:6063'

export const endpoints = {
  login: '/identity-service/login',
  accounts: '/account-service/accounts',
  cards: (accountId: string) => `/account-service/accounts/${encodeURIComponent(accountId)}/cards`,
  transactions: (accountId: string) =>
    `/account-service/accounts/${encodeURIComponent(accountId)}/transactions`,
  cardAction: (accountId: string, cardId: string, action: 'transfer' | 'deposit' | 'withdraw') =>
    `/account-service/accounts/${encodeURIComponent(accountId)}/cards/${encodeURIComponent(cardId)}/${action}`,
  notifications: (userId: string) =>
    `/notification-service/users/${encodeURIComponent(userId)}/notifications`,
  markAsRead: (notificationId: string) =>
    `/notification-service/notifications/${encodeURIComponent(notificationId)}/mark-as-read`,
} as const
