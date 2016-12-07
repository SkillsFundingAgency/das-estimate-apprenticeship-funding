Feature: ForecastCalculator
	In order to forecast the cost of my apprenticeship funding
	I want to be shown figures that show the affordability

@ignore
Scenario Outline: Forecast adds numbers correctly
	Given I have a paybill of <paybill> and my English Fraction is <english_fraction>
	When I Have the following apprenticeships:
		| AppName            | NumberOfApprentices | AppStartDate |
		| Aerospace Engineer | 1                   | 2017-05-01   |
	
	Then the total cost should be <total_cost>, total employer contribution <total_employer_contribution> and total goverment pays <total_goverment_pays>
Examples: 
	| paybill  | english_fraction | total_cost | total_employer_contribution | total_goverment_pays |
	| 10000000 | 100              | 36000      | 3600                        | 32400                |
	| 3100000  | 75               | 36000      | 3504                        | 31680                |





@BackEndTests
Scenario Outline: Forecasting Levy Payment with a large pot

	Given I have a paybill of <paybill> and my English Fraction is <english_fraction>

		When I Have the following apprenticeships:
		| AppName                          | AppCost | AppDuration | AppStartDate | NumberOfApprentices |
		| Financial Services Administrator | 12000   | 12          | 2017-05-01   | 2                   |


	When My annual levy cost <annual_levy_cost> and mothly levy cost <monthly_levy_cost> calculation are correct

	Then the annual levy cost should be <annual_levy_cost> and mothly levy cost should be <monthly_levy_cost>

	When my monthly cost <app_monthly_cost> and final month cost <final_monthAchievemnt_cost> and employer share cost <your_monthly_share> and government share cost <government_monthly_share> and emploer final month share cost <your_final_month_share> and government final month share cost <government_final_month_share> calcuation is correct

	Then the total annual cost of the apprenticeship should be <appr_totalcost>
	Then the apprenticeship monthly cost should be <app_monthly_cost>
	Then the final month achievement cost should be <final_monthAchievemnt_cost>
	Then the employer monthly contribution cost should be <your_monthly_share>
	Then the government monthly contribution cost should be <government_monthly_share>
	Then the employer final month contribution cost should be <your_final_month_share>
	Then the government final month contribution cost should be <government_final_month_share>

	Examples: 
	| paybill  | english_fraction | annual_levy_cost | monthly_levy_cost | appr_totalcost | app_monthly_cost | final_monthAchievemnt_cost | your_monthly_share | government_monthly_share | your_final_month_share | government_final_month_share |
	| 13000000 | 25               | 13752            | 1146              | 24000          | 1600             | 6400                       | 45                 | 409                      | 525                    | 4729                         |
	| 13000000 | 100              | 54996            | 4583              | 24000          | 1600             | 6400                       | 0                  | 0                        | 0                      | 0                            |	
	
	

	


@BackEndTests
Scenario Outline: Forecasting Levy Payment with small pot

	Given I have a paybill of <paybill> and my English Fraction is <english_fraction>

		When I Have the following apprenticeships:
		| AppName                          | AppCost | AppDuration | AppStartDate | NumberOfApprentices |
		| Financial Services Administrator | 12000   | 12          | 2017-05-01   | 1                   |


	When My annual levy cost <annual_levy_cost> and mothly levy cost <monthly_levy_cost> calculation are correct

	Then the annual levy cost should be <annual_levy_cost> and mothly levy cost should be <monthly_levy_cost>

	When my monthly cost <app_monthly_cost> and final month cost <final_monthAchievemnt_cost> and employer share cost <your_monthly_share> and government share cost <government_monthly_share> and emploer final month share cost <your_final_month_share> and government final month share cost <government_final_month_share> calcuation is correct

	Then the total annual cost of the apprenticeship should be <appr_totalcost>
	Then the apprenticeship monthly cost should be <app_monthly_cost>
	Then the final month achievement cost should be <final_monthAchievemnt_cost>
	Then the employer monthly contribution cost should be <your_monthly_share>
	Then the government monthly contribution cost should be <government_monthly_share>
	Then the employer final month contribution cost should be <your_final_month_share>
	Then the government final month contribution cost should be <government_final_month_share>

	Examples: 
	| paybill | english_fraction | annual_levy_cost | monthly_levy_cost | appr_totalcost | app_monthly_cost | final_monthAchievemnt_cost | your_monthly_share | government_monthly_share | your_final_month_share | government_final_month_share |
	| 5000000 | 50               | 5508             | 459               | 12000          | 800              | 3200                       | 34                 | 307                      | 274                    | 2467                         |
	| 7000000 | 40               | 8808             | 734               | 12000          | 800              | 3200                       | 6                  | 60                       | 246                    | 2220                         |
	


@BackEndTests
Scenario Outline: Forecasting Levy Payment with no pot

	Given I have a paybill of <paybill> and my English Fraction is <english_fraction>

		When I Have the following apprenticeships:
		| AppName                          | AppCost | AppDuration | AppStartDate | NumberOfApprentices |
		| Financial Services Administrator | 12000   | 12          | 2017-05-01   | 1                   |


	When My annual levy cost <annual_levy_cost> and mothly levy cost <monthly_levy_cost> calculation are correct

	Then the annual levy cost should be <annual_levy_cost> and mothly levy cost should be <monthly_levy_cost>

	When my monthly cost <app_monthly_cost> and final month cost <final_monthAchievemnt_cost> and employer share cost <your_monthly_share> and government share cost <government_monthly_share> and emploer final month share cost <your_final_month_share> and government final month share cost <government_final_month_share> calcuation is correct

	Then the total annual cost of the apprenticeship should be <appr_totalcost>
	Then the apprenticeship monthly cost should be <app_monthly_cost>
	Then the final month achievement cost should be <final_monthAchievemnt_cost>
	Then the employer monthly contribution cost should be <your_monthly_share>
	Then the government monthly contribution cost should be <government_monthly_share>
	Then the employer final month contribution cost should be <your_final_month_share>
	Then the government final month contribution cost should be <government_final_month_share>

	Examples: 
	| paybill | english_fraction | annual_levy_cost | monthly_levy_cost | appr_totalcost | app_monthly_cost | final_monthAchievemnt_cost | your_monthly_share | government_monthly_share | your_final_month_share | government_final_month_share |
	| 3000000 | 50               | 0                | 0                 | 12000          | 800              | 3200                       | 80                 | 720                      | 320                    | 2880                         |
	
	
	













