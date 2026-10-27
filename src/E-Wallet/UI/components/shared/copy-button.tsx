'use client'

import { useState } from 'react'
import { Check, Copy } from 'lucide-react'
import { toast } from 'sonner'
import { Button } from '@/components/ui/button'
import { cn } from '@/lib/utils'

export function CopyButton({ value, label, className }: { value: string | null | undefined; label: string; className?: string }) {
  const [copied, setCopied] = useState(false)

  const handleCopy = async (event: React.MouseEvent) => {
    event.stopPropagation()
    if (!value) return
    try {
      await navigator.clipboard?.writeText(value)
      setCopied(true)
      toast.success(`${label} kopyalandı`)
      setTimeout(() => setCopied(false), 1500)
    } catch {
      toast.error('Panoya kopyalanamadı')
    }
  }

  return (
    <Button
      type="button"
      variant="ghost"
      size="icon-sm"
      className={cn('shrink-0', className)}
      onClick={handleCopy}
      disabled={!value}
      aria-label={`${label} kopyala`}
    >
      {copied ? <Check aria-hidden="true" /> : <Copy aria-hidden="true" />}
    </Button>
  )
}
