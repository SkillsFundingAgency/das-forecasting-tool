Feature: DownloadForecastBalanceSheet
	As a Levy Employer logged into my Apprenticeship Account
	I want to be able to download my forecast details as a csv file
	So that I can use the forecast to explore a variety of possible future scenarios and better plan my future levy spend and apprenticeship

Background:	
	Given that I am an employer
	And I have logged into my Apprenticeship Account

Scenario: AC1: DownloadForecastBalanceSheet_Forecast between payments made and 23rd of month
	 Given I'm on the Funding projection page
	 When I select download as csv
	 Then the csv should be downloaded 
	 And the downloaded filename is in the format esfaforecast_yyyymmddhhmmss
	 And column headers are downloaded
     And all of the rows have been downloaded



  Scenario: AC2 - Forecast data is displayed correctly when forecast between payments made and 23rd of month

  Given I have generated the following projections
  
  | Date   | Funds in | Cost Of Training | Completion Payments | Future Funds |
  | Jan 18 | 1000     | 0                | 0                   | 1000         |
  | Feb 18 | 1000     | 0                | 0                   | 1000         |
  | Mar 18 | 1000     | 0                | 0                   | 1000         |
  | Apr 18 | 1000     | 0                | 0                   | 1000         |
  | Jun 18 | 1000     | 0                | 0                   | 1000         |
  | Jul 18 | 1000     | 0                | 0                   | 1000         |
  | Aug 18 | 1000     | 0                | 0                   | 1000         |

  And I'm on the Funding projection page  
  When I select download as csv
  Then the csv should be downloaded 
  And the downloaded filename is in the format esfaforecast_yyyymmddhhmmss
  And column headers are downloaded
  And all of the rows have been downloaded

  Scenario: AC3 - Forecast data is displayed correctly when forecast between 23rd of month until next payments made

  Given I have generated the following projections
  | Date   | Funds in | Cost Of Training | Completion Payments | Future Funds |
  | Jan 18 | 1000     | 0                | 0                   | 1000         |
  | Feb 18 | 1000     | 0                | 0                   | 1000         |
  | Mar 18 | 1000     | 0                | 0                   | 1000         |
  | Apr 18 | 1000     | 0                | 0                   | 1000         |
  | Jun 18 | 1000     | 0                | 0                   | 1000         |
  | Jul 18 | 1000     | 0                | 0                   | 1000         |
  | Aug 18 | 1000     | 0                | 0                   | 1000         |

  When I select download as csv
  Then the csv should be downloaded 
  And the downloaded filename is in the format esfaforecast_yyyymmddhhmmss
  And column headers are downloaded
  #And all of the rows have been downloaded

  Scenario: AC4 - Forecast data when negative balance

  Given I have generated the following projections
  | Date   | Funds in | Cost Of Training | Completion Payments | Future Funds |
  | Jan 18 | 1000     | 0                | 0                   | -1000         |
  | Feb 18 | 1000     | 0                | 0                   | 1000         |
  | Mar 18 | 1000     | 0                | 0                   | 1000         |
  | Apr 18 | 1000     | 0                | 0                   | 1000         |
  | Jun 18 | 1000     | 0                | 0                   | 1000         |
  | Jul 18 | 1000     | 0                | 0                   | 1000         |
  | Aug 18 | 1000     | 0                | 0                   | 1000         |

  When I select download as csv
  Then the csv should be downloaded 
  And the downloaded filename is in the format esfaforecast_yyyymmddhhmmss
  And column headers are downloaded
  #And all of the rows have been downloaded