/********************Purpose: PIR 24553******************************
*********************Created By: Sarvesh Ghante********************************
*********************Comments: *****************/
IF EXISTS (SELECT 1 FROM SGS_COR_TEMPLATES where TEMPLATE_NAME = 'SFN-58885')
BEGIN
Update SGS_COR_TEMPLATES set TEMPLATE_DESC = 'SFN 58885 Employer Purchase Remittance Statement' where TEMPLATE_NAME = 'SFN-58885'
END
GO
