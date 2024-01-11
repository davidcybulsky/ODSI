export interface PaymentModel {
    title: string
    amountOfMoney: number
    receiversAccountNumber: string
    issuersAccountNumber: string
    balanceBefore: number
    balanceAfter: number
}