Feature: ForecastCalculator
	In order to forecast the cost of my apprenticeship funding
	I want to be shown figures that show the affordability


Scenario Outline: Forecast adds numbers correctly
	Given I have a paybill of <paybill> and my English Fraction is <english_fraction>
	When I Have the following apprenticeships:
		| Name                    | Qty | start_date |
		| Aerospace Engineer	  | 1   | 2017-05-01 |
	
	Then the total cost should be <total_cost>, total employer contribution <total_employer_contribution> and total goverment pays <total_goverment_pays>
Examples: 
	| paybill  | english_fraction | total_cost | total_employer_contribution | total_goverment_pays |
	| 10000000 | 100              | 36000      | 3600                        | 32400                |
	| 3100000  | 75               | 36000      | 3504                        | 31680                |




Scenario Outline: Forecasting Levy Payment

	Given I have a paybill of <paybill> and my English Fraction is <english_fraction>

#	When My annual levy cost <annual_levy_cost> and mothly levy cost <monthly_levy_cost> calculation are correct
#
	Then the annual levy cost should be <annual_levy_cost> and mothly levy cost should be <monthly_levy_cost>

	Examples: 
	| paybill  | english_fraction | annual_levy_cost | monthly_levy_cost |
	| 5000000  | 100              | 11004            | 917               |
	| 13000000 | 77               | 42348            | 3529              |
	| 5000000  | 50               | 5508             | 459               |
	| 5000000  | 10               | 1104             | 92                |
	| 5000000  | 1                | 120              | 10                |
	| 7000567  | 100              | 21996            | 1833              |
	| 3000500  | 100              | 24               | 2                 |







Scenario Outline: Forecasting Levy Payment with a selected Apprenticeship

	Given I have a paybill of <paybill> and my English Fraction is <english_fraction>

		When I Have the following apprenticeships:
		| AppName                          | AppCost | AppDuration | AppStartDate | NumberOfApprentices |
		| Financial Services Administrator | 12000   | 12          | 2017-05-01   | 1                   |

	Then the annual levy cost should be <annual_levy_cost> and mothly levy cost should be <monthly_levy_cost>

	Then the total annual cost of the apprnticeship should be <appr_totalcost> and the monthly cost should be <app_monthly_cost> and the final month achievement cost should be <final_monthAchievemnt_cost> 



	Examples: 
	| paybill | english_fraction | annual_levy_cost | monthly_levy_cost | appr_totalcost | app_monthly_cost | final_monthAchievemnt_cost |
	| 5000000 | 100              | 11004            | 917               | 12000          | 8000             | £3200                      |
	











	Scenario Outline: Forecasting Non Levy Payment

	Given I have a paybill of <paybill> and my English Fraction is <english_fraction>

#	When My annual levy cost <annual_levy_cost> and mothly levy cost <monthly_levy_cost> calculation are correct
#
	Then the annual levy cost should be <annual_levy_cost> and mothly levy cost should be <monthly_levy_cost>

	Examples: 
	| paybill | english_fraction | annual_levy_cost | monthly_levy_cost |
	| 3000000 | 100              | 0                | 0                 |
	| 2000000 | 0                | 0                | 0                 |
	| 700000  | 0                | 0                | 0                 |
	| 27659   | 0                | 0                | 0                 |

