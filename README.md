# das-forecasting-tool - Forecasting tool
Forecasting and estimation tool used within Manage your Apprenticeships

### Build
![Build Status](https://sfa-gov-uk.visualstudio.com/_apis/public/build/definitions/c39e0c0b-7aff-4606-b160-3566f3bbce23/795/badge)

## Running Locally

### Prerequisites

The following needs to be installed on your local machine:

* Visual Studio 2017
* [Azure Cosmos DB Emulator](https://cosmosdbportalstorage.blob.core.windows.net/emulator/2018.04.20-1.22.0/Azure%20Cosmos%20DB.Emulator.msi)
* [Azure Function emulator version 1.0.10] (https://github.com/Azure/azure-functions-core-tools/releases/tag/1.0.10)

### Setup

* Build solution
* Publish database -> this will create a database called SFA.DAS.Forecasting.Database on your local instance

### Data Population

Running the acceptance tests will populate the database with a set of data - the connection string in the app.config file for the acceptance tests may need updating.


### Routes for test data

from localhost the following screens are available

```
/accounts/MDPP87/forecasting/estimations/start-transfer
```

```
/accounts/MDPP87/forecasting/projections
```
