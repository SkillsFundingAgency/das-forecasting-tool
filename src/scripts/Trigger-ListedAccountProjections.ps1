﻿ $connectionString = "UseDevelopmentStorage=true"
 
 #************ Projection source can be either PaymentPeriodEnd or LevyDeclaration
 
 $projectionSource = "LevyDeclaration"

  #************ Projection source can be either PaymentPeriodEnd or LevyDeclaration

 #*************************************************************
 $accountIds = @(12345
 ,67890
 , 54321
 ,09876
 )
 #*************************************************************

 foreach ($listedAccountId in $accountIds) {    
    .\Trigger-AccountProjection.ps1 -connectionString $connectionString -accountId $listedAccountId -projectionSource $projectionSource -startPeriodMonth 7 -startPeriodYear 2018
 }

 