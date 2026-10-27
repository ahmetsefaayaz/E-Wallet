'use client'

import { formatCurrency } from '@/lib/format'
import { AccountsGate, AccountSwitcher, type AccountProps } from '@/components/wallet/accounts'
import { TransactionList } from '@/components/wallet/transaction-list'
import { SectionHeader } from '@/components/shared/states'

export function TransactionsView(props: AccountProps) {
  const { accountsState, selectedAccount } = props

  return (
    <AccountsGate accountsState={accountsState}>
      {selectedAccount ? (
        <div className="flex flex-col gap-5">
          <AccountSwitcher {...props} />
          <SectionHeader
            title="Hesap hareketleri"
            description={`Güncel bakiye: ${formatCurrency(selectedAccount.balance)}`}
          />
          <TransactionList accountId={selectedAccount.id} />
        </div>
      ) : null}
    </AccountsGate>
  )
}
