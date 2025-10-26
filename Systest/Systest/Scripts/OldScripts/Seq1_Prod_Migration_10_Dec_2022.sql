
/********************Purpose: PIR 25368******************************
*********************Created By: Saylee P********************************
*********************Comments: Effective date added to Life Expectancy factor table *****************/

ALTER TABLE SGT_LIFE_EXPECTANCY_FACTOR 
ADD EFFECTIVE_DATE DATETIME DEFAULT '2010-01-01 00:00:00.000'
