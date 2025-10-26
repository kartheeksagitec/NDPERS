/*Data_Change_PIR 25049 */
/********************Purpose: PIR 25049 ******************************
*********************Created By: Vidya********************************
*********************Comments: *****************/
------------------------------------------------------------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM SGS_MESSAGES WHERE MESSAGE_ID = 10457)
BEGIN
INSERT INTO SGS_MESSAGES ([MESSAGE_ID], [DISPLAY_MESSAGE], [SEVERITY_ID], [SEVERITY_VALUE], [INTERNAL_INSTRUCTIONS], [EMPLOYER_INSTRUCTIONS], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ])
VALUES(10457, 'Plan is not linked to an active Employment Detail.', 16, 'E', NULL, NULL, 'PIR 25049', GETDATE(), 'PIR 25049', GETDATE(), 0)
END