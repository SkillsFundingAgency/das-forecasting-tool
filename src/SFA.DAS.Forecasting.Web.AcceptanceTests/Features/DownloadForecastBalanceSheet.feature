Feature: DownloadForecastBalanceSheet
	As a Levy Employer logged into my Apprenticeship Account
	I want to be able to download my forecast details as a csv file
	So that I can use the forecast to explore a variety of possible future scenarios and better plan my future levy spend and apprenticeship

Background:	
	Given that I am an employer
	And I have logged into my Apprenticeship Account

Scenario: DownloadForecastBalanceSheetAC1_Forecast between payments made and 23rd of month
	 Given I'm on the Funding projection page
	 When I select download as csv
	 Then the csv should be downloaded 
	 And the downloaded filename is in the format esfaforecast_yyyymmddhhmmss
	 And column headers are downloaded
     And all of the rows have been downloaded



  Scenario: DownloadForecastBalanceSheetAC2_Forecast data is displayed correctly when forecast between payments made and 23rd of month

  Given I have generated the following projections
  
  | Date   | Funds in | Cost Of Training | Completion Payments | Future Funds |
  | Mar 18 | 1000     | 1590             | 49900               | 1000         |
  | Apr 18 | 1000     | 880              | 32200               | 1000         |
  | Jun 18 | 1000     | 1800             | 10000               | 1000         |
  | Jul 18 | 1000     | 2350             | 50000               | 1000         |
  | Aug 18 | 1000     | 850              | 45000               | 1000         |
  | Sep 18 | 1000     | 700              | 37880               | 1000         |
  | Nov 18 | 1000     | 1800             | 45000               | 1000         |
  | Dec 18 | 1000     | 1400             | 10000               | 1000         |
  | Jan 19 | 1000     | 2000             | 10000               | 1000         |
  | Feb 19 | 1000     | 1800             | 10000               | 1000         |
  | Mar 19 | 1000     | 1800             | 45000               | 1000         |
  | Apr 19 | 1000     | 2100             | 10000               | 1000         | 

  And I'm on the Funding projection page  
  When I select download as csv
  Then the csv should be downloaded 
  And the downloaded filename is in the format esfaforecast_yyyymmddhhmmss
  And column headers are downloaded
  And all of the rows have been downloaded

  Scenario: DownloadForecastBalanceSheetAC3_Forecast data is displayed correctly when forecast between 23rd of month until next payments made

  Given I have generated the following projections
  | Date   | Funds in | Cost Of Training | Completion Payments | Future Funds |
  | Mar 18 | 1000     | 1590             | 49900               | 1000         |
  | Apr 18 | 1000     | 880              | 32200               | 1000         |
  | Jun 18 | 1000     | 1800             | 10000               | 1000         |
  | Jul 18 | 1000     | 2350             | 50000               | 1000         |
  | Aug 18 | 1000     | 850              | 45000               | 1000         |
  | Sep 18 | 1000     | 700              | 37880               | 1000         |
  | Nov 18 | 1000     | 1800             | 45000               | 1000         |
  | Dec 18 | 1000     | 1400             | 10000               | 1000         |
  | Jan 19 | 1000     | 2000             | 10000               | 1000         |
  | Feb 19 | 1000     | 1800             | 10000               | 1000         |
  | Mar 19 | 1000     | 1800             | 45000               | 1000         |
  | Apr 19 | 1000     | 2100             | 10000               | 1000         | 

  And I'm on the Funding projection page 
  When I select download as csv
  Then the csv should be downloaded 
  And the downloaded filename is in the format esfaforecast_yyyymmddhhmmss
  And column headers are downloaded
  And all of the rows have been downloaded

  Scenario: DownloadForecastBalanceSheetAC4_Forecast data when negative balance

  Given I have generated the following projections
  | Date   | Funds in | Cost Of Training | Completion Payments | Future Funds |
  | Mar 18 | 1000     | 1590             | 49900               | 1000         |
  | Apr 18 | 1000     | 880              | 32200               | 1000         |
  | Jun 18 | 1000     | 1800             | 10000               | 1000         |
  | Jul 18 | 1000     | 2350             | 50000               | 1000         |
  | Aug 18 | 1000     | 850              | 45000               | 1000         |
  | Sep 18 | 1000     | 700              | 37880               | 1000         |
  | Nov 18 | 1000     | 1800             | 45000               | 1000         |
  | Dec 18 | 1000     | 1400             | 10000               | 1000         |
  | Jan 19 | 1000     | 2000             | 10000               | 1000         |
  | Feb 19 | 1000     | 1800             | 10000               | 1000         |
  | Mar 19 | 1000     | 1800             | 45000               | 1000         |
  | Apr 19 | 1000     | 2100             | 10000               | 1000         | 

  And I'm on the Funding projection page 
  When I select download as csv
  Then the csv should be downloaded 
  And the downloaded filename is in the format esfaforecast_yyyymmddhhmmss
  And column headers are downloaded
  And all of the rows have been downloaded