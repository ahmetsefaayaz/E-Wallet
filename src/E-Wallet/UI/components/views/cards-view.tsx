'use client'

import { AccountsGate, AccountSwitcher, BalanceHero, type AccountProps } from '@/components/wallet/accounts'
import { CardList } from '@/components/wallet/card-list'

export function CardsView(props: AccountProps) {
  const { accountsState, selectedAccount } = props
  const index = accountsState.accounts.findIndex((a) => a.id === selectedAccount?.id)

  return (
    <AccountsGate accountsState={accountsState}>
      {selectedAccount ? (
        <div className="flex flex-col gap-6">
          <AccountSwitcher {...props} />
          <BalanceHero account={selectedAccount} index={Math.max(index, 0)} />
          <CardList account={selectedAccount} />
        </div>
      ) : null}
    </AccountsGate>
  )
}
