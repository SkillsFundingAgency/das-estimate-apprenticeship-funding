Feature: ForecastTool
	In order to show the correct user journey
	I want to be shown the relevant screens relating to my data


@explicit
Scenario Outline: Non Levy Payer is not shown english fraction screen
	Given I have a payroll of <payroll>
	Then my english fraction is <english_fraction>
	Examples:
	| payroll | english_fraction |
	| 3       | NA               |
	| 2999999 | NA               |
	| 3000000 | 100              |
	| 4100000 | 50              |

@explicit
Scenario: Non levy payer with no cohorts is shown empty results grid
	Given I am a non levy payer
	When I view the results page
	Then I am not shown a results grid
