Feature: ForecastCalculator
	In order to forecast the cost of my apprenticeship funding
	I want to be shown figures that show the affordability


Scenario Outline: Forecast adds numbers correctly
	Given I have a paybill of <paybill> and my English Fraction is <english_fraction>
	When I Have the following apprenticeships:
		| Name                    | Qty | start_date |
		| Actuarial Technician	  | 2   | 2017-05-01 |
	
	Then the total cost should be <total_cost>, total employer contribution <total_employer_contribution> and total goverment pays <total_goverment_pays>
Examples: 
	| paybill | english_fraction | total_cost | total_employer_contribution | total_goverment_pays |
	| 200000  | 75               | 36000     | 3600                       | 32400                |
	| 3100000 | 75               | 36000     | 3504                       | 31680                |
	 