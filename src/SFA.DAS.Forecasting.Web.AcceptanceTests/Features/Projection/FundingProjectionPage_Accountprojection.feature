﻿Feature: FundingProjectionPage _Accountprojection
As a Levy Employer logged into my Apprenticeship Account
I want to be able to see my current levy balance, credit and apprenticeship commitments displayed as a forecast across multiple future periods.
So that I can explore a variety of possible future scenarios and better plan my future levy spend and apprenticeships

Background:	
	Given that I am an employer
	And I have logged into my Apprenticeship Account

Scenario: FundingProjectionPageAC1: Forecast data is displayed correctly when forecast between payments made and 23rd of month
  Given I have generated the following projections
  
  | Date   | Funds in | Cost Of Training | Completion Payments | Your Contribution | Government Contribution | Future Funds |
  | Jul 18 | 21000    | 2350             | 50000               | 0                 | 0                       | 23000        |
  | Aug 18 | 45200    | 850              | 45000               | 0                 | 0                       | 1000         |
  | Sep 18 | 55000    | 700              | 37880               | 0                 | 0                       | 12000        |
  | Oct 18 | 42000    | 700              | 37880               | 0                 | 0                       | 1000         |
  | Nov 18 | 22000    | 1800             | 45000               | 0                 | 0                       | 5000         |
  | Dec 18 | 42000    | 1400             | 10000               | 0                 | 0                       | 4000         |
  | Jan 19 | 41000    | 2000             | 10000               | 0                 | 0                       | 1000         |
  | Feb 19 | 10000    | 1800             | 10000               | 0                 | 0                       | 1000         |
  | Mar 19 | 15000    | 1800             | 45000               | 0                 | 0                       | 31000        |
  | Apr 19 | 42500    | 2100             | 10000               | 0                 | 0                       | 1000         |
   
  And I'm on the Funding projection page
  When the Account projection is displayed
  Then the Account projection has the correct columns without Co-Investment
  And the first month displayed is the next calendar month
  And there are months up to 'Apr 19' displayed in the forecast
  And the data is displayed correctly in each column



  Scenario: FundingProjectionPageAC2: Forecast data is displayed correctly when forecast between 23rd of month until end of month
  Given I have generated the following projections
  
 | Date   | Funds in | Cost Of Training | Completion Payments | Your Contribution | Government Contribution | Future Funds |
 | Jul 18 | 21000    | 2350             | 50000               | 0                 | 0                       | 23000        |
 | Aug 18 | 45200    | 850              | 45000               | 0                 | 0                       | 1000         |
 | Sep 18 | 55000    | 700              | 37880               | 0                 | 0                       | 12000        |
 | Oct 18 | 42000    | 700              | 37880               | 0                 | 0                       | 1000         |
 | Nov 18 | 22000    | 1800             | 45000               | 0                 | 0                       | 5000         |
 | Dec 18 | 42000    | 1400             | 10000               | 0                 | 0                       | 4000         |
 | Jan 19 | 41000    | 2000             | 10000               | 0                 | 0                       | 1000         |
 | Feb 19 | 10000    | 1800             | 10000               | 0                 | 0                       | 1000         |
 | Mar 19 | 15000    | 1800             | 45000               | 0                 | 0                       | 31000        |
 | Apr 19 | 42500    | 2100             | 10000               | 0                 | 0                       | 1000         |

  And I'm on the Funding projection page
  When the Account projection is displayed
  Then the Account projection has the correct columns without Co-Investment
  And the first month displayed is the next calendar month
  And there are months up to 'Apr 19' displayed in the forecast
  And the data is displayed correctly in each column



  Scenario: FundingProjectionPageAC3: Forecast data is displayed correctly when forecast between 1st of month until next payments made
  Given I have generated the following projections
  
  | Date   | Funds in | Cost Of Training | Completion Payments | Your Contribution | Government Contribution | Future Funds |
  | Jul 18 | 21000    | 2350             | 50000               | 0                 | 0                       | 23000        |
  | Aug 18 | 45200    | 850              | 45000               | 0                 | 0                       | 1000         |
  | Sep 18 | 55000    | 700              | 37880               | 0                 | 0                       | 12000        |
  | Oct 18 | 42000    | 700              | 37880               | 0                 | 0                       | 1000         |
  | Nov 18 | 22000    | 1800             | 45000               | 0                 | 0                       | 5000         |
  | Dec 18 | 42000    | 1400             | 10000               | 0                 | 0                       | 4000         |
  | Jan 19 | 41000    | 2000             | 10000               | 0                 | 0                       | 1000         |
  | Feb 19 | 10000    | 1800             | 10000               | 0                 | 0                       | 1000         |
  | Mar 19 | 15000    | 1800             | 45000               | 0                 | 0                       | 31000        |
  | Apr 19 | 42500    | 2100             | 10000               | 0                 | 0                       | 1000         |   

  And I'm on the Funding projection page
  When the Account projection is displayed
  Then the Account projection has the correct columns without Co-Investment
  And the first month displayed is the next calendar month
  And there are months up to 'Apr 19' displayed in the forecast
  And the data is displayed correctly in each column

  Scenario: Hide Co-investment columns 
  Given I have generated the following projections

  | Date   | Funds in | Cost Of Training | Completion Payments | Your Contribution | Government Contribution | Future Funds |
  | Apr 18 | 14000    | 880              | 3220                | 0                 | 0                       | 32000        |
  | May 18 | 15000    | 880              | 3220                | 0                 | 0                       | 18000        |
  | Jun 18 | 91000    | 1800             | 1000                | 0                 | 0                       | 23000        |
  | Jul 18 | 21000    | 2350             | 5000                | 0                 | 0                       | 23000        |
  | Aug 18 | 45200    | 850              | 4500                | 0                 | 0                       | 1000         |
  | Sep 18 | 55000    | 700              | 3788                | 0                 | 0                       | 12000        |
  | Oct 18 | 42000    | 700              | 3788                | 0                 | 0                       | 1000         |
  | Nov 18 | 22000    | 1800             | 4500                | 0                 | 0                       | 5000         |
  | Dec 18 | 42000    | 1400             | 1000                | 0                 | 0                       | 4000         |
  | Jan 19 | 41000    | 2000             | 1000                | 0                 | 0                       | 1000         |
  | Feb 19 | 10000    | 1800             | 1000                | 0                 | 0                       | 1000         |
  | Mar 19 | 15000    | 1800             | 4500                | 0                 | 0                       | 31000        |
  | Apr 19 | 42500    | 2100             | 1000                | 0                 | 0                       | 1000         |
  And I'm on the Funding projection page
  When the Account projection is displayed
  Then the Account projection has the correct columns without Co-Investment
  And there are months up to 'Apr 19' displayed in the forecast

Scenario: FundingProjection completion payments overdue
	Given I have completion payments of £ 2401 on commitments without stop date
	And I'm on the Funding projection page
	Then I see Pending completion payments with the amount of £ 2,401