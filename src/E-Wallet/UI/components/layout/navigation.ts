import { ArrowLeftRight, Bell, CreditCard, LayoutDashboard, type LucideIcon } from 'lucide-react'

export type View = 'overview' | 'cards' | 'transactions' | 'notifications'

export const navItems: { id: View; label: string; icon: LucideIcon }[] = [
  { id: 'overview', label: 'Genel Bakış', icon: LayoutDashboard },
  { id: 'cards', label: 'Kartlarım', icon: CreditCard },
  { id: 'transactions', label: 'İşlemler', icon: ArrowLeftRight },
  { id: 'notifications', label: 'Bildirimler', icon: Bell },
]

export const viewTitles: Record<View, { title: string; description: string }> = {
  overview: { title: 'Genel Bakış', description: 'Hesaplarınızın ve kartlarınızın özeti' },
  cards: { title: 'Kartlarım', description: 'Kartlarınızı yönetin, para yatırın, çekin veya transfer edin' },
  transactions: { title: 'İşlemler', description: 'Hesap hareketlerinizin tam listesi' },
  notifications: { title: 'Bildirimler', description: 'Hesabınızla ilgili tüm bildirimler' },
}
