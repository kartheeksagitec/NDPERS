
/********************Purpose: PIR 18274 correspondance to be available on PersonMaintenance ******************************
*********************Created By: Abhijeet M ********************************
*********************Comments: correspondance to be available on PersonMaintenance *****************/
IF NOT EXISTS(SELECT 1 FROM SGS_COR_TEMPLATES WHERE TEMPLATE_ID=559 AND TEMPLATE_NAME='PER-0160' AND  ASSOCIATED_FORMS='wfmFlexCompMaintenance;wfmPersonMaintenance;')
BEGIN
	UPDATE SGS_COR_TEMPLATES SET ASSOCIATED_FORMS='wfmFlexCompMaintenance;wfmPersonMaintenance;' WHERE TEMPLATE_ID=559 AND TEMPLATE_NAME='PER-0160'
END

IF NOT EXISTS(SELECT 1 FROM SGS_COR_TEMPLATES WHERE TEMPLATE_ID=551 AND TEMPLATE_NAME='PAY-4026' AND  ASSOCIATED_FORMS='wfmFlexCompMaintenance;wfmPersonMaintenance;')
BEGIN
	UPDATE SGS_COR_TEMPLATES SET ASSOCIATED_FORMS='wfmFlexCompMaintenance;wfmPersonMaintenance;' WHERE TEMPLATE_ID=551 AND TEMPLATE_NAME='PAY-4026'
END