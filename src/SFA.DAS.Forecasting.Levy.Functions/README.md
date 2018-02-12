## SFA.DAS.Forecasting.Levy.Functions

##### LevyDeclarationPreLoadHttpFunction
Gets the levy declarations for a list of AccountIds for a year month period.  

```
POST  http://localhost:7071/api/LevyDeclarationPreLoadHttpFunction

{
  "EmployerAccountIds": ["MJK9XV", "MGXLRV", "MPN4YM"],
  "PeriodYear": "2018-19",
  "PeriodMonth": 1
}
```


##### LevyDeclarationEventHttpFunction and LevyDeclarationEventServiceBusFunction
These functions will take a levy update message and add it to the queue for validation. 

```
POST http://localhost:7071/api/LevyDeclarationEventHttpFunction
Accept: application/javascript

{
  "AccountId": 11122,
  "EmpRef": "ABBA12",
  "PayrollYear": "18-19",
  "PayrollMonth": 1,
  "LevyDeclaredInMonth": 101.0,
  "CreatedDate": "2018-02-05T11:08:36.1205465+00:00"
}
```
##### LevyDeclarationEventValidatorFunction
Validates the levy declaration and add valid declaration to the _levy store queue_.
##### LevyDeclarationEventStoreFunction
Stored levy declaration.
