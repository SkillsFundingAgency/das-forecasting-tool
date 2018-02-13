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
	 #And column headers are downloaded
  #   And all of the rows have been downloaded
