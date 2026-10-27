'use client'

import { useState } from 'react'
import { ArrowRight, Eye, EyeOff, Loader2, Lock, Mail, ShieldCheck, Zap, Bell } from 'lucide-react'
import { toast } from 'sonner'
import { api } from '@/lib/api'
import { endpoints } from '@/lib/config'
import { setToken } from '@/lib/auth'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { BrandLogo } from '@/components/shared/brand-logo'
import { ThemeToggle } from '@/components/layout/theme-toggle'

const highlights = [
  { icon: ShieldCheck, title: 'Zero Trust güvenlik', text: 'Kimliğiniz yalnızca imzalı JWT ile doğrulanır.' },
  { icon: Zap, title: 'Anında transfer', text: 'Kartlarınızdan saniyeler içinde para gönderin.' },
  { icon: Bell, title: 'Canlı bildirimler', text: 'Her işlem için anlık bilgilendirme alın.' },
]

function extractToken(data: unknown): string | null {
  if (!data || typeof data !== 'object') return null
  const record = data as Record<string, unknown>
  const candidate = record.token ?? record.accessToken ?? (record.data as Record<string, unknown> | undefined)?.token
  return typeof candidate === 'string' && candidate.length > 0 ? candidate : null
}

export function LoginScreen() {
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [showPassword, setShowPassword] = useState(false)
  const [isSubmitting, setIsSubmitting] = useState(false)

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    if (!email.trim() || !password) {
      toast.error('E-posta ve şifre alanları zorunludur.')
      return
    }
    setIsSubmitting(true)
    try {
      const response = await api.post(endpoints.login, { Email: email.trim(), Password: password })
      const token = extractToken(response?.data)
      if (!token) {
        toast.error('Sunucu yanıtında token bulunamadı.')
        return
      }
      setToken(token)
      toast.success('Giriş başarılı. Hoş geldiniz!')
    } catch {
      // Error toast is shown by the axios interceptor.
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <div className="grid min-h-dvh lg:grid-cols-2">
      <aside className="relative hidden flex-col justify-between overflow-hidden bg-sidebar p-10 text-sidebar-foreground lg:flex">
        <div
          aria-hidden="true"
          className="pointer-events-none absolute -top-32 -right-32 size-[28rem] rounded-full bg-sidebar-primary/15 blur-3xl"
        />
        <BrandLogo inverted />
        <div className="relative flex max-w-md flex-col gap-8">
          <div className="flex flex-col gap-3">
            <h1 className="text-4xl font-semibold tracking-tight text-balance text-sidebar-accent-foreground">
              Paranızı tek yerden, güvenle yönetin.
            </h1>
            <p className="text-pretty text-sidebar-foreground/70">
              Hesaplarınız, kartlarınız ve transferleriniz kurumsal düzeyde güvenlikle bir arada.
            </p>
          </div>
          <ul className="flex flex-col gap-5">
            {highlights.map(({ icon: Icon, title, text }) => (
              <li key={title} className="flex items-start gap-3">
                <span className="flex size-9 shrink-0 items-center justify-center rounded-lg bg-sidebar-accent text-sidebar-primary">
                  <Icon className="size-4" aria-hidden="true" />
                </span>
                <div>
                  <p className="text-sm font-medium text-sidebar-accent-foreground">{title}</p>
                  <p className="text-sm text-sidebar-foreground/60">{text}</p>
                </div>
              </li>
            ))}
          </ul>
        </div>
        <p className="relative text-xs text-sidebar-foreground/50">© {new Date().getFullYear()} E-Cüzdan</p>
      </aside>

      <main className="flex flex-col">
        <div className="flex items-center justify-between p-4 sm:p-6">
          <BrandLogo className="lg:invisible" />
          <ThemeToggle />
        </div>
        <div className="flex flex-1 items-center justify-center px-4 pb-16 sm:px-6">
          <div className="w-full max-w-sm rounded-2xl border bg-card p-6 shadow-sm sm:p-8">
            <div className="mb-6 flex flex-col gap-1.5">
              <h2 className="text-2xl font-semibold tracking-tight">Giriş yap</h2>
              <p className="text-sm text-muted-foreground">Cüzdanınıza erişmek için bilgilerinizi girin.</p>
            </div>
            <form onSubmit={handleSubmit} className="flex flex-col gap-4" noValidate>
              <div className="flex flex-col gap-2">
                <Label htmlFor="email">E-posta</Label>
                <div className="relative">
                  <Mail className="pointer-events-none absolute top-1/2 left-3 size-4 -translate-y-1/2 text-muted-foreground" aria-hidden="true" />
                  <Input
                    id="email"
                    type="email"
                    autoComplete="email"
                    placeholder="ornek@mail.com"
                    className="h-10 pl-9"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    required
                  />
                </div>
              </div>
              <div className="flex flex-col gap-2">
                <Label htmlFor="password">Şifre</Label>
                <div className="relative">
                  <Lock className="pointer-events-none absolute top-1/2 left-3 size-4 -translate-y-1/2 text-muted-foreground" aria-hidden="true" />
                  <Input
                    id="password"
                    type={showPassword ? 'text' : 'password'}
                    autoComplete="current-password"
                    placeholder="••••••••"
                    className="h-10 pr-10 pl-9"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    required
                  />
                  <Button
                    type="button"
                    variant="ghost"
                    size="icon-sm"
                    className="absolute top-1/2 right-1 -translate-y-1/2"
                    onClick={() => setShowPassword((v) => !v)}
                    aria-label={showPassword ? 'Şifreyi gizle' : 'Şifreyi göster'}
                  >
                    {showPassword ? <EyeOff aria-hidden="true" /> : <Eye aria-hidden="true" />}
                  </Button>
                </div>
              </div>
              <Button type="submit" size="lg" className="mt-2 h-10" disabled={isSubmitting}>
                {isSubmitting ? <Loader2 className="animate-spin" aria-hidden="true" /> : null}
                {isSubmitting ? 'Giriş yapılıyor…' : 'Giriş yap'}
                {!isSubmitting ? <ArrowRight aria-hidden="true" /> : null}
              </Button>
            </form>
          </div>
        </div>
      </main>
    </div>
  )
}
