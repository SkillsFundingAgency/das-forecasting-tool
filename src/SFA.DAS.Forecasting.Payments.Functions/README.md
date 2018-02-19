## SFA.DAS.Forecasting.Payments.Functions


##### PaymentEventHttpFunction and PaymentEventServiceBusFunction

##### PaymentEventValidatorFunction

##### PaymentEventStoreFunction


### PreLoad

##### PaymentPreLoadHttpFunction
For each EmployerAccountId creates a **PreLoadPaymentMessage** messages.

```
POST  http://localhost:7071/api/PaymentPreLoadHttpFunction

{
  "EmployerAccountIds": ["MN4YKL", "MGXLRV", "MPN4YM"],
  "PeriodYear": 2017,
  "PeriodMonth": 5,
  "PeriodId": "16-17R10"
}
```

##### GetEmployerPaymentFunction
Will read all payments for an employer and cache them.
Will send a message to GetEarningDetailsFunction.

##### GetEarningDetailsFunction
Gets EarningDetails for all payments for an employer from the ProviderEventsAPI.
Will send a message to CreatePaymentMessageFunction

##### CreatePaymentMessageFunction
Will create **PaymentCreatedMessage** and put on the validation queue.