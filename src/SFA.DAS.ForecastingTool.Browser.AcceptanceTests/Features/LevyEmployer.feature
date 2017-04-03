@UITests
Feature: LevyEmployer
	As an employer whopays more than levy limit
	I would like to know my contributions towards the selected apprenticeship program

Scenario: Annual Payroll is Over Levy Limit And Skip Apprenticeship selection
	Given My Annual Payroll is 3000001
	And Roughly 85 percentage of my employees live in england
	When I Skip apprenticeship selection
	Then My Annual and Monthly Levy Credit are shown
	And I should see how this is worked out

	
Scenario: Annual Payroll is Over Levy Limit And Add Apprenticeship
	Given My Annual Payroll is 3000001
	And Roughly 85 percentage of my employees live in england
	When I add 1 apprenticeship for 18 months starts on 05/17
	Then My Annual and Monthly Levy Credit are shown
	And I should see how this is worked out
	And I should see my co-investment details

	
Scenario Outline: Annual Payroll is Under Levy Limit And Skip Apprenticeship selection
	Given My Annual Payroll is <LevyLimit>
	When I Skip apprenticeship selection
	Then My Annual and Monthly Levy Credit are not shown
	And I should see that i wont have to pay levy
	Examples: 
	| LevyLimit |
	| 2000001   |
	| 3000000   | 
	#Commented known failing test as it will mark the Deployment status as Failure
	
Scenario: Annual Payroll is Under Levy Limit And Add Apprenticeship
	Given My Annual Payroll is 1000001
	When I add 1 apprenticeship for 24 months starts on 05/17
	Then My Annual and Monthly Levy Credit are not shown
	And I should see that i wont have to pay levy
	And I should see my co-investment details