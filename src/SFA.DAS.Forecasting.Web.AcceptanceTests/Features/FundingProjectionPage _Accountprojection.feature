Feature: FundingProjectionPage _Accountprojection
As a Levy Employer logged into my Apprenticeship Account
I want to be able to see my current levy balance, credit and apprenticeship commitments displayed as a forecast across multiple future periods.
So that I can explore a variety of possible future scenarios and better plan my future levy spend and apprenticeships

Background:	
	Given that I am an employer
	And I have logged into my Apprenticeship Account

Scenario: FundingProjectionPageAC1: Forecast data is displayed correctly when forecast between payments made and 23rd of month
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
  When the Account projection is displayed
  Then the Account projection has the correct columns	
  And the first month displayed is the next calendar month
  And there are months up to 'Apr 19' displayed in the forecast
#  And the data is displayed correctly in each column
#  And the first month's levy credit is shown as the calculated levy value
#  ##And the first month's balance includes the levy credit value
#  And completion payments are shown against the correct months
#
#
  Scenario: FundingProjectionPageAC2: Forecast data is displayed correctly when forecast between 23rd of month until end of month
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
  When the Account projection is displayed
  Then the Account projection has the correct columns
  And the first month displayed is the next calendar month
  And there are months up to 'Apr 19' displayed in the forecast
#  And the data is displayed correctly in each column
#  And the first month's levy credit is shown as the calculated levy value
#  #And the first month's balance uses a levy credit value of £0
#  And completion payments are shown against the correct months
#
#
  Scenario: FundingProjectionPageAC3: Forecast data is displayed correctly when forecast between 1st of month until next payments made
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
  When the Account projection is displayed
  Then the Account projection has the correct columns
  And the first month displayed is the next calendar month
  And there are months up to 'Apr 19' displayed in the forecast
#  And the data is displayed correctly in each column
#  And the first month's levy credit is shown as the calculated levy value
#  #And the first month's balance uses a levy credit value of £0
#  And completion payments are shown against the correct months
#
#
#  Scenario: FundingProjectionPageAC4: Forecast data when negative balance
#  Given I have generated the following projections
#  
#  | Date   | Funds in | Cost Of Training | Completion Payments | Future Funds |
#  | Mar 18 | 1000     | 1590             | 49900               | 1000         |
#  | Apr 18 | 1000     | 880              | 32200               | 1000         |
#  | Jun 18 | 1000     | 1800             | 10000               | 1000         |
#  | Jul 18 | 1000     | 2350             | 50000               | 1000         |
#  | Aug 18 | 1000     | 850              | 45000               | 1000         |
#  | Sep 18 | 1000     | 700              | 37880               | 1000         |
#  | Nov 18 | 1000     | 1800             | 45000               | 1000         |
#  | Dec 18 | 1000     | 1400             | 10000               | 1000         |
#  | Jan 19 | 1000     | 2000             | 10000               | 1000         |
#  | Feb 19 | 1000     | 1800             | 10000               | 1000         |
#  | Mar 19 | 1000     | 1800             | 45000               | 1000         |
#  | Apr 19 | 1000     | 2100             | 10000               | 1000         | 
#
#
#
#  And I'm on the Funding projection page
#  When I have a negative balance in a forecast month
#  Then the balance for that month is displayed correctly as £0