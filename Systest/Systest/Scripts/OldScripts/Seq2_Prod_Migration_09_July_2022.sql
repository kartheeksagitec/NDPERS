/********************PIR 24923******************************
*********************Created By: Vidya F********************************
*********************Comments: *****************/
IF NOT EXISTS (SELECT 1 FROM [dbo].[SGS_MESSAGES] WITH(NOLOCK) WHERE MESSAGE_ID = 10454)
BEGIN
INSERT INTO SGS_MESSAGES([MESSAGE_ID],[DISPLAY_MESSAGE],[SEVERITY_ID],[SEVERITY_VALUE],[INTERNAL_INSTRUCTIONS],[EMPLOYER_INSTRUCTIONS],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
VALUES (10454,'Record already exists for the same month.  Please wait to update until the next payment has been made or contact NDPERS.',16,'E',NULL,NULL,'PIR 24923',getdate(),'PIR 24923',GETDATE(),0)
END
GO

/********************PIR 24911******************************
*********************Created By: Vidya F********************************
*********************Comments: *****************/
IF  EXISTS (SELECT 1 FROM [dbo].[SGT_PAYMENT_ITEM_TYPE] WITH(NOLOCK) WHERE PAYMENT_ITEM_TYPE_ID=111 and SPECIAL_TAX_TREATMENT_CODE_VALUE='NOTX')
BEGIN
UPDATE SGT_PAYMENT_ITEM_TYPE SET SPECIAL_TAX_TREATMENT_CODE_VALUE='FTTX' WHERE PAYMENT_ITEM_TYPE_ID=111
END
GO

