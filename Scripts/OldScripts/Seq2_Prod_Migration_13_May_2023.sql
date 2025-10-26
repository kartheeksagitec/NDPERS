--------------Created By: Bharat Reddy
--------------Purpose   : PIR 25603
--------------Date      : 03/07/2023
IF NOT EXISTS (SELECT * FROM SGS_MESSAGES WHERE MESSAGE_ID=10487)
INSERT INTO SGS_MESSAGES (MESSAGE_ID,DISPLAY_MESSAGE,SEVERITY_ID,SEVERITY_VALUE,INTERNAL_INSTRUCTIONS,EMPLOYER_INSTRUCTIONS,CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ)
VALUES(10487,'Pay Period Begin Date cannot be prior to existing record.',16,'E',NULL,NULL,'PIR 25603',GETDATE(),'PIR 25603',GETDATE(),0)
GO

--------------Created By: Vidya Fulsoundar
--------------Purpose   : PIR 25559
--------------Date      : 04/27/2023
ALTER TABLE SGT_TAX_REF_CONFIG
ADD TWO_TIP VARCHAR(MAX)

IF EXISTS (SELECT 1 FROM SGT_TAX_REF_CONFIG WHERE TAX_REF_ID=1)
UPDATE SGT_TAX_REF_CONFIG SET TWO_TIP ='<b>TIP: </b>To be accurate, submit a 2022 Form W-4P for all other pensions/annuities. Submit a new Form W-4 for yourjob(s) if you have not updated your withholding since 2019. If you have self-employment income, see page 2.',
THREE_FIRST_LINE='If your total income will be {0} or less ({1} or less if married filing jointly):',THREE_SECOND_LINE='Multiply the number of qualifying children under age {0} by {1}',
THREE_THIRD_LINE='Multiply the number of other dependents by {0}' WHERE TAX_REF_ID=1
GO

--------------Created By: sanket chougale
--------------Purpose   : PIR 25756  
--------------Date      : 3 May 2023
UPDATE  SGS_MESSAGES SET DISPLAY_MESSAGE='No valid address on file. Check will print without address.', SEVERITY_VALUE='I', MODIFIED_BY='PIR 25756', MODIFIED_DATE=GETDATE() 
WHERE MESSAGE_ID=5646 AND DISPLAY_MESSAGE = 'Payee account cannot be approved as there is no address for the payee'
