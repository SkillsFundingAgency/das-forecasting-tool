## SFA.DAS.Forecasting.Payments.Functions


##### PaymentEventHttpFunction and PaymentEventServiceBusFunction

##### PaymentEventValidatorFunction

##### PaymentEventStoreFunction


### PreLoad

##### PaymentPreLoadHttpFunction
For each id creates a **PreLoadPaymentMessage** messages.

```
POST  http://localhost:7071/api/PaymentPreLoadHttpFunction

{
  "EmployerAccountIds": ["MJK9XV", "MGXLRV", "MPN4YM"],
  "PeriodYear": 2018,
  "PeriodMonth": 1
}
```

##### PaymentPreLoadCreatePaymentMessageFunction
Creates a **PaymentCreatedMessage** and sends it to functions to add _EarningDetails_
##### PaymentPreLoadAddEarningDetailsFunction
Adding earning details to the **PaymentCreatedMessage** message and sends it for validation.