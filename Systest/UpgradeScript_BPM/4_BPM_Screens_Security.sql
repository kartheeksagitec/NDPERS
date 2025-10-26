DECLARE @cnt INT
BEGIN
SELECT @cnt = count(*) FROM [dbo].[SGS_RESOURCES] WHERE RESOURCE_ID=2063 AND RESOURCE_DESCRIPTION='BPM - My Basket';
if @cnt = 0
Begin
INSERT INTO [dbo].[SGS_RESOURCES] ([RESOURCE_ID],[RESOURCE_TYPE_ID],[RESOURCE_TYPE_VALUE],[RESOURCE_DESCRIPTION],[CREATED_BY],[CREATED_DATE]
,[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
VALUES
(2063,12,'F','BPM - My Basket','BPM_UPGRADE',GETDATE(),'BPM_UPGRADE',GETDATE(),0)
END
ELSE
PRINT 'RESOURCE_ID 2063 already exists in SGS_RESOURCES table';
END
--------------------------------------------------------------------------------------------------------------
/* Insert query to assign New Resources to all ROLES */
---------------------------------------------------------------------------------------------------------------

IF NOT EXISTS(SELECT 1 FROM [dbo].[SGS_SECURITY] WHERE RESOURCE_ID = 2063)
BEGIN
INSERT INTO [dbo].[SGS_SECURITY]
SELECT ROL.ROLE_ID, RES.RESOURCE_ID, 11, 0, 'BPM_UPGRADE', GETDATE(), 'BPM_UPGRADE', GETDATE(), 0
FROM [dbo].[SGS_ROLES] ROL, [dbo].[SGS_RESOURCES] RES
WHERE NOT EXISTS (SELECT ROLE_ID
FROM [dbo].[SGS_SECURITY] SEC
WHERE ROL.ROLE_ID = SEC.ROLE_ID
AND RES.RESOURCE_ID = SEC.RESOURCE_ID) AND RES.RESOURCE_ID = 2063
END

------------------------------------------------------------------------------------------------------------------------------------------

BEGIN
SELECT @cnt = count(*) FROM [dbo].[SGS_RESOURCES] WHERE RESOURCE_ID=2064 AND RESOURCE_DESCRIPTION='BPM - Reassign Work';
if @cnt = 0
Begin
INSERT INTO [dbo].[SGS_RESOURCES] ([RESOURCE_ID],[RESOURCE_TYPE_ID],[RESOURCE_TYPE_VALUE],[RESOURCE_DESCRIPTION],[CREATED_BY],[CREATED_DATE]
,[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
VALUES
(2064,12,'F','BPM - Reassign Work','BPM_UPGRADE',GETDATE(),'BPM_UPGRADE',GETDATE(),0)
END
ELSE
PRINT 'RESOURCE_ID 2064 alredy exists in SGS_RESOURCES table';
END

--------------------------------------------------------------------------------------------------------------
/* Insert query to assign New Resources to all ROLES */
---------------------------------------------------------------------------------------------------------------
IF NOT EXISTS(SELECT 1 FROM [dbo].[SGS_SECURITY] WHERE RESOURCE_ID = 2064)
BEGIN
INSERT INTO [dbo].[SGS_SECURITY]
SELECT ROL.ROLE_ID, RES.RESOURCE_ID, 11, 0, 'BPM_UPGRADE', GETDATE(), 'BPM_UPGRADE', GETDATE(), 0
FROM [dbo].[SGS_ROLES] ROL, [dbo].[SGS_RESOURCES] RES
WHERE NOT EXISTS (SELECT ROLE_ID
FROM [dbo].[SGS_SECURITY] SEC
WHERE ROL.ROLE_ID = SEC.ROLE_ID
AND RES.RESOURCE_ID = SEC.RESOURCE_ID) AND RES.RESOURCE_ID = 2064
END
--------------------------------------------------------------------------------------------------------------------------------------------

BEGIN
SELECT @cnt = count(*) FROM [dbo].[SGS_RESOURCES] WHERE RESOURCE_ID=2065 AND RESOURCE_DESCRIPTION='BPM - Case Initiation';
if @cnt = 0
Begin
INSERT INTO [dbo].[SGS_RESOURCES] ([RESOURCE_ID],[RESOURCE_TYPE_ID],[RESOURCE_TYPE_VALUE],[RESOURCE_DESCRIPTION],[CREATED_BY],[CREATED_DATE]
,[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
VALUES
(2065,12,'F','BPM - Case Initiation','BPM_UPGRADE',GETDATE(),'BPM_UPGRADE',GETDATE(),0)
END
ELSE
PRINT 'RESOURCE_ID 2065 alredy exists in SGS_RESOURCES table';
END

--------------------------------------------------------------------------------------------------------------
/* Insert query to assign New Resources to all ROLES */
---------------------------------------------------------------------------------------------------------------
IF NOT EXISTS(SELECT 1 FROM [dbo].[SGS_SECURITY] WHERE RESOURCE_ID = 2065)
BEGIN
INSERT INTO [dbo].[SGS_SECURITY]
SELECT ROL.ROLE_ID, RES.RESOURCE_ID, 11, 0, 'BPM_UPGRADE', GETDATE(), 'BPM_UPGRADE', GETDATE(), 0
FROM [dbo].[SGS_ROLES] ROL, [dbo].[SGS_RESOURCES] RES
WHERE NOT EXISTS (SELECT ROLE_ID
FROM [dbo].[SGS_SECURITY] SEC
WHERE ROL.ROLE_ID = SEC.ROLE_ID
AND RES.RESOURCE_ID = SEC.RESOURCE_ID) AND RES.RESOURCE_ID = 2065
END

-----------------------------------------------------------------------------------------------------------------------------------------------
BEGIN
SELECT @cnt = count(*) FROM [dbo].[SGS_RESOURCES] WHERE RESOURCE_ID=2066 AND RESOURCE_DESCRIPTION='BPM - Case Configuration';
if @cnt = 0
Begin
INSERT INTO [dbo].[SGS_RESOURCES] ([RESOURCE_ID],[RESOURCE_TYPE_ID],[RESOURCE_TYPE_VALUE],[RESOURCE_DESCRIPTION],[CREATED_BY],[CREATED_DATE]
,[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
VALUES
(2066,12,'F','BPM - Case Configuration','BPM_UPGRADE',GETDATE(),'BPM_UPGRADE',GETDATE(),0)
END
ELSE
PRINT 'RESOURCE_ID 2066 alredy exists in SGS_RESOURCES table';
END

--------------------------------------------------------------------------------------------------------------
/* Insert query to assign New Resources to all ROLES */
---------------------------------------------------------------------------------------------------------------
IF NOT EXISTS(SELECT 1 FROM [dbo].[SGS_SECURITY] WHERE RESOURCE_ID = 2066)
BEGIN
INSERT INTO [dbo].[SGS_SECURITY]
SELECT ROL.ROLE_ID, RES.RESOURCE_ID, 11, 0, 'BPM_UPGRADE', GETDATE(), 'BPM_UPGRADE', GETDATE(), 0
FROM [dbo].[SGS_ROLES] ROL, [dbo].[SGS_RESOURCES] RES
WHERE NOT EXISTS (SELECT ROLE_ID
FROM [dbo].[SGS_SECURITY] SEC
WHERE ROL.ROLE_ID = SEC.ROLE_ID
AND RES.RESOURCE_ID = SEC.RESOURCE_ID) AND RES.RESOURCE_ID = 2066
END
----------------------------------------------------------------------------------------------------------------------------------

BEGIN
SELECT @cnt = count(*) FROM [dbo].[SGS_RESOURCES] WHERE RESOURCE_ID=2067 AND RESOURCE_DESCRIPTION='BPM - Case Instance';
if @cnt = 0
Begin
INSERT INTO [dbo].[SGS_RESOURCES] ([RESOURCE_ID],[RESOURCE_TYPE_ID],[RESOURCE_TYPE_VALUE],[RESOURCE_DESCRIPTION],[CREATED_BY],[CREATED_DATE]
,[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
VALUES
(2067,12,'F','BPM - Case Instance','BPM_UPGRADE',GETDATE(),'BPM_UPGRADE',GETDATE(),0)
END
ELSE
PRINT 'RESOURCE_ID 2067 alredy exists in SGS_RESOURCES table';
END

--------------------------------------------------------------------------------------------------------------
/* Insert query to assign New Resources to all ROLES */
---------------------------------------------------------------------------------------------------------------
IF NOT EXISTS(SELECT 1 FROM [dbo].[SGS_SECURITY] WHERE RESOURCE_ID = 2067)
BEGIN
INSERT INTO [dbo].[SGS_SECURITY]
SELECT ROL.ROLE_ID, RES.RESOURCE_ID, 11, 0, 'BPM_UPGRADE', GETDATE(), 'BPM_UPGRADE', GETDATE(), 0
FROM [dbo].[SGS_ROLES] ROL, [dbo].[SGS_RESOURCES] RES
WHERE NOT EXISTS (SELECT ROLE_ID
FROM [dbo].[SGS_SECURITY] SEC
WHERE ROL.ROLE_ID = SEC.ROLE_ID
AND RES.RESOURCE_ID = SEC.RESOURCE_ID) AND RES.RESOURCE_ID = 2067
END
-------------------------------------------------------------------------------------------------------------------------------------

BEGIN
SELECT @cnt = count(*) FROM [dbo].[SGS_RESOURCES] WHERE RESOURCE_ID=2068 AND RESOURCE_DESCRIPTION='BPM - Process Configuration';
if @cnt = 0
Begin
INSERT INTO [dbo].[SGS_RESOURCES] ([RESOURCE_ID],[RESOURCE_TYPE_ID],[RESOURCE_TYPE_VALUE],[RESOURCE_DESCRIPTION],[CREATED_BY],[CREATED_DATE]
,[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
VALUES
(2068,12,'F','BPM - Process Configuration','BPM_UPGRADE',GETDATE(),'BPM_UPGRADE',GETDATE(),0)
END
ELSE
PRINT 'RESOURCE_ID 2068 alredy exists in SGS_RESOURCES table';
END

--------------------------------------------------------------------------------------------------------------
/* Insert query to assign New Resources to all ROLES */
---------------------------------------------------------------------------------------------------------------
IF NOT EXISTS(SELECT 1 FROM [dbo].[SGS_SECURITY] WHERE RESOURCE_ID = 2068)
BEGIN
INSERT INTO [dbo].[SGS_SECURITY]
SELECT ROL.ROLE_ID, RES.RESOURCE_ID, 11, 0, 'BPM_UPGRADE', GETDATE(), 'BPM_UPGRADE', GETDATE(), 0
FROM [dbo].[SGS_ROLES] ROL, [dbo].[SGS_RESOURCES] RES
WHERE NOT EXISTS (SELECT ROLE_ID
FROM [dbo].[SGS_SECURITY] SEC
WHERE ROL.ROLE_ID = SEC.ROLE_ID
AND RES.RESOURCE_ID = SEC.RESOURCE_ID) AND RES.RESOURCE_ID = 2068
END
----------------------------------------------------------------------------------------------------------------------------

BEGIN
SELECT @cnt = count(*) FROM [dbo].[SGS_RESOURCES] WHERE RESOURCE_ID=2069 AND RESOURCE_DESCRIPTION='BPM - Escalation';
if @cnt = 0
Begin
INSERT INTO [dbo].[SGS_RESOURCES] ([RESOURCE_ID],[RESOURCE_TYPE_ID],[RESOURCE_TYPE_VALUE],[RESOURCE_DESCRIPTION],[CREATED_BY],[CREATED_DATE]
,[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
VALUES
(2069,12,'F','BPM - Escalation','BPM_UPGRADE',GETDATE(),'BPM_UPGRADE',GETDATE(),0)
END
ELSE
PRINT 'RESOURCE_ID 2069 alredy exists in SGS_RESOURCES table';
END

--------------------------------------------------------------------------------------------------------------
/* Insert query to assign New Resources to all ROLES */
---------------------------------------------------------------------------------------------------------------
IF NOT EXISTS(SELECT 1 FROM [dbo].[SGS_SECURITY] WHERE RESOURCE_ID = 2069)
BEGIN
INSERT INTO [dbo].[SGS_SECURITY]
SELECT ROL.ROLE_ID, RES.RESOURCE_ID, 11, 0, 'BPM_UPGRADE', GETDATE(), 'BPM_UPGRADE', GETDATE(), 0
FROM [dbo].[SGS_ROLES] ROL, [dbo].[SGS_RESOURCES] RES
WHERE NOT EXISTS (SELECT ROLE_ID
FROM [dbo].[SGS_SECURITY] SEC
WHERE ROL.ROLE_ID = SEC.ROLE_ID
AND RES.RESOURCE_ID = SEC.RESOURCE_ID) AND RES.RESOURCE_ID = 2069
END
-------------------------------------------------------------------------------------------------------------------------------
BEGIN
SELECT @cnt = count(*) FROM [dbo].[SGS_RESOURCES] WHERE RESOURCE_ID=2070 AND RESOURCE_DESCRIPTION='BPM - Process Escalations';
if @cnt = 0
Begin
INSERT INTO [dbo].[SGS_RESOURCES] ([RESOURCE_ID],[RESOURCE_TYPE_ID],[RESOURCE_TYPE_VALUE],[RESOURCE_DESCRIPTION],[CREATED_BY],[CREATED_DATE]
,[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
VALUES
(2070,12,'F','BPM - Process Escalations','BPM_UPGRADE',GETDATE(),'BPM_UPGRADE',GETDATE(),0)
END
ELSE
PRINT 'RESOURCE_ID 2070 alredy exists in SGS_RESOURCES table';
END

--------------------------------------------------------------------------------------------------------------
/* Insert query to assign New Resources to all ROLES */
---------------------------------------------------------------------------------------------------------------
IF NOT EXISTS(SELECT 1 FROM [dbo].[SGS_SECURITY] WHERE RESOURCE_ID = 2070)
BEGIN
INSERT INTO [dbo].[SGS_SECURITY]
SELECT ROL.ROLE_ID, RES.RESOURCE_ID, 11, 0, 'BPM_UPGRADE', GETDATE(), 'BPM_UPGRADE', GETDATE(), 0
FROM [dbo].[SGS_ROLES] ROL, [dbo].[SGS_RESOURCES] RES
WHERE NOT EXISTS (SELECT ROLE_ID
FROM [dbo].[SGS_SECURITY] SEC
WHERE ROL.ROLE_ID = SEC.ROLE_ID
AND RES.RESOURCE_ID = SEC.RESOURCE_ID) AND RES.RESOURCE_ID = 2070
END
-----------------------------------------------------------------------------------------------------------------------------------------

BEGIN
SELECT @cnt = count(*) FROM [dbo].[SGS_RESOURCES] WHERE RESOURCE_ID=2071 AND RESOURCE_DESCRIPTION='BPM - Process Instance';
if @cnt = 0
Begin
INSERT INTO [dbo].[SGS_RESOURCES] ([RESOURCE_ID],[RESOURCE_TYPE_ID],[RESOURCE_TYPE_VALUE],[RESOURCE_DESCRIPTION],[CREATED_BY],[CREATED_DATE]
,[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
VALUES
(2071,12,'F','BPM - Process Instance','BPM_UPGRADE',GETDATE(),'BPM_UPGRADE',GETDATE(),0)
END
ELSE
PRINT 'RESOURCE_ID 2071 alredy exists in SGS_RESOURCES table';
END

--------------------------------------------------------------------------------------------------------------
/* Insert query to assign New Resources to all ROLES */
---------------------------------------------------------------------------------------------------------------
IF NOT EXISTS(SELECT 1 FROM [dbo].[SGS_SECURITY] WHERE RESOURCE_ID = 2071)
BEGIN
INSERT INTO [dbo].[SGS_SECURITY]
SELECT ROL.ROLE_ID, RES.RESOURCE_ID, 11, 0, 'BPM_UPGRADE', GETDATE(), 'BPM_UPGRADE', GETDATE(), 0
FROM [dbo].[SGS_ROLES] ROL, [dbo].[SGS_RESOURCES] RES
WHERE NOT EXISTS (SELECT ROLE_ID
FROM [dbo].[SGS_SECURITY] SEC
WHERE ROL.ROLE_ID = SEC.ROLE_ID
AND RES.RESOURCE_ID = SEC.RESOURCE_ID) AND RES.RESOURCE_ID = 2071
END
-----------------------------------------------------------------------------------------------------------------------------------------------
BEGIN
SELECT @cnt = count(*) FROM [dbo].[SGS_RESOURCES] WHERE RESOURCE_ID=2072 AND RESOURCE_DESCRIPTION='BPM - Events';
if @cnt = 0
Begin
INSERT INTO [dbo].[SGS_RESOURCES] ([RESOURCE_ID],[RESOURCE_TYPE_ID],[RESOURCE_TYPE_VALUE],[RESOURCE_DESCRIPTION],[CREATED_BY],[CREATED_DATE]
,[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
VALUES
(2072,12,'F','BPM - Events','BPM_UPGRADE',GETDATE(),'BPM_UPGRADE',GETDATE(),0)
END
ELSE
PRINT 'RESOURCE_ID 2072 alredy exists in SGS_RESOURCES table';
END

--------------------------------------------------------------------------------------------------------------
/* Insert query to assign New Resources to all ROLES */
---------------------------------------------------------------------------------------------------------------
IF NOT EXISTS(SELECT 1 FROM [dbo].[SGS_SECURITY] WHERE RESOURCE_ID = 2072)
BEGIN
INSERT INTO [dbo].[SGS_SECURITY]
SELECT ROL.ROLE_ID, RES.RESOURCE_ID, 11, 0, 'BPM_UPGRADE', GETDATE(), 'BPM_UPGRADE', GETDATE(), 0
FROM [dbo].[SGS_ROLES] ROL, [dbo].[SGS_RESOURCES] RES
WHERE NOT EXISTS (SELECT ROLE_ID
FROM [dbo].[SGS_SECURITY] SEC
WHERE ROL.ROLE_ID = SEC.ROLE_ID
AND RES.RESOURCE_ID = SEC.RESOURCE_ID) AND RES.RESOURCE_ID = 2072
END
----------------------------------------------------------------------------------------------------------------------------------------------------
BEGIN
SELECT @cnt = count(*) FROM [dbo].[SGS_RESOURCES] WHERE RESOURCE_ID=2073 AND RESOURCE_DESCRIPTION='BPM - Requests';
if @cnt = 0
Begin
INSERT INTO [dbo].[SGS_RESOURCES] ([RESOURCE_ID],[RESOURCE_TYPE_ID],[RESOURCE_TYPE_VALUE],[RESOURCE_DESCRIPTION],[CREATED_BY],[CREATED_DATE]
,[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
VALUES
(2073,12,'F','BPM - Requests','BPM_UPGRADE',GETDATE(),'BPM_UPGRADE',GETDATE(),0)
END
ELSE
PRINT 'RESOURCE_ID 2073 alredy exists in SGS_RESOURCES table';
END

--------------------------------------------------------------------------------------------------------------
/* Insert query to assign New Resources to all ROLES */
---------------------------------------------------------------------------------------------------------------
IF NOT EXISTS(SELECT 1 FROM [dbo].[SGS_SECURITY] WHERE RESOURCE_ID = 2073)
BEGIN
INSERT INTO [dbo].[SGS_SECURITY]
SELECT ROL.ROLE_ID, RES.RESOURCE_ID, 11, 0, 'BPM_UPGRADE', GETDATE(), 'BPM_UPGRADE', GETDATE(), 0
FROM [dbo].[SGS_ROLES] ROL, [dbo].[SGS_RESOURCES] RES
WHERE NOT EXISTS (SELECT ROLE_ID
FROM [dbo].[SGS_SECURITY] SEC
WHERE ROL.ROLE_ID = SEC.ROLE_ID
AND RES.RESOURCE_ID = SEC.RESOURCE_ID) AND RES.RESOURCE_ID = 2073
END
----------------------------------------------------------------------------------------------------------------------------------------------
BEGIN
SELECT @cnt = count(*) FROM [dbo].[SGS_RESOURCES] WHERE RESOURCE_ID=2074 AND RESOURCE_DESCRIPTION='BPM - Receive Document';
if @cnt = 0
Begin
INSERT INTO [dbo].[SGS_RESOURCES] ([RESOURCE_ID],[RESOURCE_TYPE_ID],[RESOURCE_TYPE_VALUE],[RESOURCE_DESCRIPTION],[CREATED_BY],[CREATED_DATE]
,[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
VALUES
(2074,12,'F','BPM - Receive Document','BPM_UPGRADE',GETDATE(),'BPM_UPGRADE',GETDATE(),0)
END
ELSE
PRINT 'RESOURCE_ID 2074 alredy exists in SGS_RESOURCES table';
END

--------------------------------------------------------------------------------------------------------------
/* Insert query to assign New Resources to all ROLES */
---------------------------------------------------------------------------------------------------------------
IF NOT EXISTS(SELECT 1 FROM [dbo].[SGS_SECURITY] WHERE RESOURCE_ID = 2074)
BEGIN
INSERT INTO [dbo].[SGS_SECURITY]
SELECT ROL.ROLE_ID, RES.RESOURCE_ID, 11, 0, 'BPM_UPGRADE', GETDATE(), 'BPM_UPGRADE', GETDATE(), 0
FROM [dbo].[SGS_ROLES] ROL, [dbo].[SGS_RESOURCES] RES
WHERE NOT EXISTS (SELECT ROLE_ID
FROM [dbo].[SGS_SECURITY] SEC
WHERE ROL.ROLE_ID = SEC.ROLE_ID
AND RES.RESOURCE_ID = SEC.RESOURCE_ID) AND RES.RESOURCE_ID = 2074
END

---------------------------------------------------------------------------------------------------------------------------------------
BEGIN
SELECT @cnt = count(*) FROM [dbo].[SGS_RESOURCES] WHERE RESOURCE_ID=2075 AND RESOURCE_DESCRIPTION='BPM - Case Mapping';
if @cnt = 0
Begin
INSERT INTO [dbo].[SGS_RESOURCES] ([RESOURCE_ID],[RESOURCE_TYPE_ID],[RESOURCE_TYPE_VALUE],[RESOURCE_DESCRIPTION],[CREATED_BY],[CREATED_DATE]
,[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
VALUES
(2075,12,'F','BPM - Case Mapping','BPM_UPGRADE',GETDATE(),'BPM_UPGRADE',GETDATE(),0)
END
ELSE
PRINT 'RESOURCE_ID 2075 alredy exists in SGS_RESOURCES table';
END

--------------------------------------------------------------------------------------------------------------
/* Insert query to assign New Resources to all ROLES */
---------------------------------------------------------------------------------------------------------------
IF NOT EXISTS(SELECT 1 FROM [dbo].[SGS_SECURITY] WHERE RESOURCE_ID = 2075)
BEGIN
INSERT INTO [dbo].[SGS_SECURITY]
SELECT ROL.ROLE_ID, RES.RESOURCE_ID, 11, 0, 'BPM_UPGRADE', GETDATE(), 'BPM_UPGRADE', GETDATE(), 0
FROM [dbo].[SGS_ROLES] ROL, [dbo].[SGS_RESOURCES] RES
WHERE NOT EXISTS (SELECT ROLE_ID
FROM [dbo].[SGS_SECURITY] SEC
WHERE ROL.ROLE_ID = SEC.ROLE_ID
AND RES.RESOURCE_ID = SEC.RESOURCE_ID) AND RES.RESOURCE_ID = 2075
END
-----------------------------------------------------------------------------------------------------------------------------------------------------
BEGIN
SELECT @cnt = count(*) FROM [dbo].[SGS_RESOURCES] WHERE RESOURCE_ID=2076 AND RESOURCE_DESCRIPTION='BPM - Server Management';
if @cnt = 0
Begin
INSERT INTO [dbo].[SGS_RESOURCES] ([RESOURCE_ID],[RESOURCE_TYPE_ID],[RESOURCE_TYPE_VALUE],[RESOURCE_DESCRIPTION],[CREATED_BY],[CREATED_DATE]
,[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
VALUES
(2076,12,'F','BPM - Server Management','BPM_UPGRADE',GETDATE(),'BPM_UPGRADE',GETDATE(),0)
END
ELSE
PRINT 'RESOURCE_ID 2076 alredy exists in SGS_RESOURCES table';
END

--------------------------------------------------------------------------------------------------------------
/* Insert query to assign New Resources to all ROLES */
---------------------------------------------------------------------------------------------------------------
IF NOT EXISTS(SELECT 1 FROM [dbo].[SGS_SECURITY] WHERE RESOURCE_ID = 2076)
BEGIN
INSERT INTO [dbo].[SGS_SECURITY]
SELECT ROL.ROLE_ID, RES.RESOURCE_ID, 11, 0, 'BPM_UPGRADE', GETDATE(), 'BPM_UPGRADE', GETDATE(), 0
FROM [dbo].[SGS_ROLES] ROL, [dbo].[SGS_RESOURCES] RES
WHERE NOT EXISTS (SELECT ROLE_ID
FROM [dbo].[SGS_SECURITY] SEC
WHERE ROL.ROLE_ID = SEC.ROLE_ID
AND RES.RESOURCE_ID = SEC.RESOURCE_ID) AND RES.RESOURCE_ID = 2076
END
-------------------------------------------------------------------------------------------------------------------------------------------------
BEGIN
SELECT @cnt = count(*) FROM [dbo].[SGS_RESOURCES] WHERE RESOURCE_ID=2077 AND RESOURCE_DESCRIPTION='BPM - Dashboard';
if @cnt = 0
Begin
INSERT INTO [dbo].[SGS_RESOURCES] ([RESOURCE_ID],[RESOURCE_TYPE_ID],[RESOURCE_TYPE_VALUE],[RESOURCE_DESCRIPTION],[CREATED_BY],[CREATED_DATE]
,[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
VALUES
(2077,12,'F','BPM - Dashboard','BPM_UPGRADE',GETDATE(),'BPM_UPGRADE',GETDATE(),0)
END
ELSE
PRINT 'RESOURCE_ID 2077 alredy exists in SGS_RESOURCES table';
END

--------------------------------------------------------------------------------------------------------------
/* Insert query to assign New Resources to all ROLES */
---------------------------------------------------------------------------------------------------------------
IF NOT EXISTS(SELECT 1 FROM [dbo].[SGS_SECURITY] WHERE RESOURCE_ID = 2077)
BEGIN
INSERT INTO [dbo].[SGS_SECURITY]
SELECT ROL.ROLE_ID, RES.RESOURCE_ID, 11, 0, 'BPM_UPGRADE', GETDATE(), 'BPM_UPGRADE', GETDATE(), 0
FROM [dbo].[SGS_ROLES] ROL, [dbo].[SGS_RESOURCES] RES
WHERE NOT EXISTS (SELECT ROLE_ID
FROM [dbo].[SGS_SECURITY] SEC
WHERE ROL.ROLE_ID = SEC.ROLE_ID
AND RES.RESOURCE_ID = SEC.RESOURCE_ID) AND RES.RESOURCE_ID = 2077
END
----------------------------------------------------------------------------------------------------------------------------------------

BEGIN
SELECT @cnt = count(*) FROM [dbo].[SGS_RESOURCES] WHERE RESOURCE_ID=2078 AND RESOURCE_DESCRIPTION='BPM - User Dashboard';
if @cnt = 0
Begin
INSERT INTO [dbo].[SGS_RESOURCES] ([RESOURCE_ID],[RESOURCE_TYPE_ID],[RESOURCE_TYPE_VALUE],[RESOURCE_DESCRIPTION],[CREATED_BY],[CREATED_DATE]
,[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
VALUES
(2078,12,'F','BPM - User Dashboard','BPM_UPGRADE',GETDATE(),'BPM_UPGRADE',GETDATE(),0)
END
ELSE
PRINT 'RESOURCE_ID 2078 alredy exists in SGS_RESOURCES table';
END

--------------------------------------------------------------------------------------------------------------
/* Insert query to assign New Resources to all ROLES */
---------------------------------------------------------------------------------------------------------------
IF NOT EXISTS(SELECT 1 FROM [dbo].[SGS_SECURITY] WHERE RESOURCE_ID = 2078)
BEGIN
INSERT INTO [dbo].[SGS_SECURITY]
SELECT ROL.ROLE_ID, RES.RESOURCE_ID, 11, 0, 'BPM_UPGRADE', GETDATE(), 'BPM_UPGRADE', GETDATE(), 0
FROM [dbo].[SGS_ROLES] ROL, [dbo].[SGS_RESOURCES] RES
WHERE NOT EXISTS (SELECT ROLE_ID
FROM [dbo].[SGS_SECURITY] SEC
WHERE ROL.ROLE_ID = SEC.ROLE_ID
AND RES.RESOURCE_ID = SEC.RESOURCE_ID) AND RES.RESOURCE_ID = 2078
END
-------------------------------------------------------------------------------------------------------------------------
BEGIN
SELECT @cnt = count(*) FROM [dbo].[SGS_RESOURCES] WHERE RESOURCE_ID=2079 AND RESOURCE_DESCRIPTION='BPM - Supervisor Dashboard';
if @cnt = 0
Begin
INSERT INTO [dbo].[SGS_RESOURCES] ([RESOURCE_ID],[RESOURCE_TYPE_ID],[RESOURCE_TYPE_VALUE],[RESOURCE_DESCRIPTION],[CREATED_BY],[CREATED_DATE]
,[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
VALUES
(2079,12,'F','BPM - Supervisor Dashboard','BPM_UPGRADE',GETDATE(),'BPM_UPGRADE',GETDATE(),0)
END
ELSE
PRINT 'RESOURCE_ID 2079 alredy exists in SGS_RESOURCES table';
END

--------------------------------------------------------------------------------------------------------------
/* Insert query to assign New Resources to all ROLES */
---------------------------------------------------------------------------------------------------------------
IF NOT EXISTS(SELECT 1 FROM [dbo].[SGS_SECURITY] WHERE RESOURCE_ID = 2079)
BEGIN
INSERT INTO [dbo].[SGS_SECURITY]
SELECT ROL.ROLE_ID, RES.RESOURCE_ID, 11, 0, 'BPM_UPGRADE', GETDATE(), 'BPM_UPGRADE', GETDATE(), 0
FROM [dbo].[SGS_ROLES] ROL, [dbo].[SGS_RESOURCES] RES
WHERE NOT EXISTS (SELECT ROLE_ID
FROM [dbo].[SGS_SECURITY] SEC
WHERE ROL.ROLE_ID = SEC.ROLE_ID
AND RES.RESOURCE_ID = SEC.RESOURCE_ID) AND RES.RESOURCE_ID = 2079
END
------------------------------------------------------------------------------------------------------------------------------------------

BEGIN
SELECT @cnt = count(*) FROM [dbo].[SGS_RESOURCES] WHERE RESOURCE_ID=2080 AND RESOURCE_DESCRIPTION='BPM - Out Of Office';
if @cnt = 0
Begin
INSERT INTO [dbo].[SGS_RESOURCES] ([RESOURCE_ID],[RESOURCE_TYPE_ID],[RESOURCE_TYPE_VALUE],[RESOURCE_DESCRIPTION],[CREATED_BY],[CREATED_DATE]
,[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
VALUES
(2080,12,'F','BPM - Out Of Office','BPM_UPGRADE',GETDATE(),'BPM_UPGRADE',GETDATE(),0)
END
ELSE
PRINT 'RESOURCE_ID 2080 alredy exists in SGS_RESOURCES table';
END

--------------------------------------------------------------------------------------------------------------
/* Insert query to assign New Resources to all ROLES */
---------------------------------------------------------------------------------------------------------------
IF NOT EXISTS(SELECT 1 FROM [dbo].[SGS_SECURITY] WHERE RESOURCE_ID = 2080)
BEGIN
INSERT INTO [dbo].[SGS_SECURITY]
SELECT ROL.ROLE_ID, RES.RESOURCE_ID, 11, 0, 'BPM_UPGRADE', GETDATE(), 'BPM_UPGRADE', GETDATE(), 0
FROM [dbo].[SGS_ROLES] ROL, [dbo].[SGS_RESOURCES] RES
WHERE NOT EXISTS (SELECT ROLE_ID
FROM [dbo].[SGS_SECURITY] SEC
WHERE ROL.ROLE_ID = SEC.ROLE_ID
AND RES.RESOURCE_ID = SEC.RESOURCE_ID) AND RES.RESOURCE_ID = 2080
END
----------------------------------------------------------------------------------------------------------------------------------------------------
BEGIN
SELECT @cnt = count(*) FROM [dbo].[SGS_RESOURCES] WHERE RESOURCE_ID=2081 AND RESOURCE_DESCRIPTION='BPM - Failed Activity';
if @cnt = 0
Begin
INSERT INTO [dbo].[SGS_RESOURCES] ([RESOURCE_ID],[RESOURCE_TYPE_ID],[RESOURCE_TYPE_VALUE],[RESOURCE_DESCRIPTION],[CREATED_BY],[CREATED_DATE]
,[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
VALUES
(2081,12,'F','BPM - Failed Activity','BPM_UPGRADE',GETDATE(),'BPM_UPGRADE',GETDATE(),0)
END
ELSE
PRINT 'RESOURCE_ID 2081 alredy exists in SGS_RESOURCES table';
END

--------------------------------------------------------------------------------------------------------------
/* Insert query to assign New Resources to all ROLES */
---------------------------------------------------------------------------------------------------------------
IF NOT EXISTS(SELECT 1 FROM [dbo].[SGS_SECURITY] WHERE RESOURCE_ID = 2081)
BEGIN
INSERT INTO [dbo].[SGS_SECURITY]
SELECT ROL.ROLE_ID, RES.RESOURCE_ID, 11, 0, 'BPM_UPGRADE', GETDATE(), 'BPM_UPGRADE', GETDATE(), 0
FROM [dbo].[SGS_ROLES] ROL, [dbo].[SGS_RESOURCES] RES
WHERE NOT EXISTS (SELECT ROLE_ID
FROM [dbo].[SGS_SECURITY] SEC
WHERE ROL.ROLE_ID = SEC.ROLE_ID
AND RES.RESOURCE_ID = SEC.RESOURCE_ID) AND RES.RESOURCE_ID = 2081
END
-----------------------------------------------------------------------------------------------------------------------------------------------

BEGIN
SELECT @cnt = count(*) FROM [dbo].[SGS_RESOURCES] WHERE RESOURCE_ID=2082 AND RESOURCE_DESCRIPTION='BPM - Performance Test';
if @cnt = 0
Begin
INSERT INTO [dbo].[SGS_RESOURCES] ([RESOURCE_ID],[RESOURCE_TYPE_ID],[RESOURCE_TYPE_VALUE],[RESOURCE_DESCRIPTION],[CREATED_BY],[CREATED_DATE]
,[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
VALUES
(2082,12,'F','BPM - Performance Test','BPM_UPGRADE',GETDATE(),'BPM_UPGRADE',GETDATE(),0)
END
ELSE
PRINT 'RESOURCE_ID 2082 alredy exists in SGS_RESOURCES table';
END

--------------------------------------------------------------------------------------------------------------
/* Insert query to assign New Resources to all ROLES */
---------------------------------------------------------------------------------------------------------------
IF NOT EXISTS(SELECT 1 FROM [dbo].[SGS_SECURITY] WHERE RESOURCE_ID = 2082)
BEGIN
INSERT INTO [dbo].[SGS_SECURITY]
SELECT ROL.ROLE_ID, RES.RESOURCE_ID, 11, 0, 'BPM_UPGRADE', GETDATE(), 'BPM_UPGRADE', GETDATE(), 0
FROM [dbo].[SGS_ROLES] ROL, [dbo].[SGS_RESOURCES] RES
WHERE NOT EXISTS (SELECT ROLE_ID
FROM [dbo].[SGS_SECURITY] SEC
WHERE ROL.ROLE_ID = SEC.ROLE_ID
AND RES.RESOURCE_ID = SEC.RESOURCE_ID) AND RES.RESOURCE_ID = 2082
END
------------------------------------------------------------------------------------------------------------------------------
BEGIN
SELECT @cnt = count(*) FROM [dbo].[SGS_RESOURCES] WHERE RESOURCE_ID=2083 AND RESOURCE_DESCRIPTION='BPM - Service Mapping';
if @cnt = 0
Begin
INSERT INTO [dbo].[SGS_RESOURCES] ([RESOURCE_ID],[RESOURCE_TYPE_ID],[RESOURCE_TYPE_VALUE],[RESOURCE_DESCRIPTION],[CREATED_BY],[CREATED_DATE]
,[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
VALUES
(2083,12,'F','BPM - Service Mapping','BPM_UPGRADE',GETDATE(),'BPM_UPGRADE',GETDATE(),0)
END
ELSE
PRINT 'RESOURCE_ID 2083 alredy exists in SGS_RESOURCES table';
END

--------------------------------------------------------------------------------------------------------------
/* Insert query to assign New Resources to all ROLES */
---------------------------------------------------------------------------------------------------------------
IF NOT EXISTS(SELECT 1 FROM [dbo].[SGS_SECURITY] WHERE RESOURCE_ID = 2083)
BEGIN
INSERT INTO [dbo].[SGS_SECURITY]
SELECT ROL.ROLE_ID, RES.RESOURCE_ID, 11, 0, 'BPM_UPGRADE', GETDATE(), 'BPM_UPGRADE', GETDATE(), 0
FROM [dbo].[SGS_ROLES] ROL, [dbo].[SGS_RESOURCES] RES
WHERE NOT EXISTS (SELECT ROLE_ID
FROM [dbo].[SGS_SECURITY] SEC
WHERE ROL.ROLE_ID = SEC.ROLE_ID
AND RES.RESOURCE_ID = SEC.RESOURCE_ID) AND RES.RESOURCE_ID = 2083
END
--------------------------------------------------------------------------------------------------------------------------------------------------------

BEGIN
SELECT @cnt = count(*) FROM [dbo].[SGS_RESOURCES] WHERE RESOURCE_ID=2084 AND RESOURCE_DESCRIPTION='BPM - Monitoring';
if @cnt = 0
Begin
INSERT INTO [dbo].[SGS_RESOURCES] ([RESOURCE_ID],[RESOURCE_TYPE_ID],[RESOURCE_TYPE_VALUE],[RESOURCE_DESCRIPTION],[CREATED_BY],[CREATED_DATE]
,[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
VALUES
(2084,12,'F','BPM - Monitoring','BPM_UPGRADE',GETDATE(),'BPM_UPGRADE',GETDATE(),0)
END
ELSE
PRINT 'RESOURCE_ID 2084 alredy exists in SGS_RESOURCES table';
END

--------------------------------------------------------------------------------------------------------------
/* Insert query to assign New Resources to all ROLES */
---------------------------------------------------------------------------------------------------------------
IF NOT EXISTS(SELECT 1 FROM [dbo].[SGS_SECURITY] WHERE RESOURCE_ID = 2084)
BEGIN
INSERT INTO [dbo].[SGS_SECURITY]
SELECT ROL.ROLE_ID, RES.RESOURCE_ID, 11, 0, 'BPM_UPGRADE', GETDATE(), 'BPM_UPGRADE', GETDATE(), 0
FROM [dbo].[SGS_ROLES] ROL, [dbo].[SGS_RESOURCES] RES
WHERE NOT EXISTS (SELECT ROLE_ID
FROM [dbo].[SGS_SECURITY] SEC
WHERE ROL.ROLE_ID = SEC.ROLE_ID
AND RES.RESOURCE_ID = SEC.RESOURCE_ID) AND RES.RESOURCE_ID = 2084
END

-----------------------------------------------------------------------------------------------------------------

UPDATE [dbo].[SGS_SECURITY]  SET SECURITY_VALUE = 5 WHERE RESOURCE_ID = 2063 AND ROLE_ID = 124
UPDATE [dbo].[SGS_SECURITY]  SET SECURITY_VALUE = 5 WHERE RESOURCE_ID = 2064 AND ROLE_ID = 134
UPDATE [dbo].[SGS_SECURITY]  SET SECURITY_VALUE = 5 WHERE RESOURCE_ID = 2065 AND ROLE_ID = 124
UPDATE [dbo].[SGS_SECURITY]  SET SECURITY_VALUE = 5 WHERE RESOURCE_ID = 2066 AND ROLE_ID = 20
UPDATE [dbo].[SGS_SECURITY]  SET SECURITY_VALUE = 5 WHERE RESOURCE_ID = 2067 AND ROLE_ID = 20
UPDATE [dbo].[SGS_SECURITY]  SET SECURITY_VALUE = 5 WHERE RESOURCE_ID = 2068 AND ROLE_ID = 20
UPDATE [dbo].[SGS_SECURITY]  SET SECURITY_VALUE = 5 WHERE RESOURCE_ID = 2069 AND ROLE_ID = 20
UPDATE [dbo].[SGS_SECURITY]  SET SECURITY_VALUE = 5 WHERE RESOURCE_ID = 2070 AND ROLE_ID = 20
UPDATE [dbo].[SGS_SECURITY]  SET SECURITY_VALUE = 5 WHERE RESOURCE_ID = 2071 AND ROLE_ID = 20
UPDATE [dbo].[SGS_SECURITY]  SET SECURITY_VALUE = 5 WHERE RESOURCE_ID = 2072 AND ROLE_ID = 20
UPDATE [dbo].[SGS_SECURITY]  SET SECURITY_VALUE = 5 WHERE RESOURCE_ID = 2073 AND ROLE_ID = 20
UPDATE [dbo].[SGS_SECURITY]  SET SECURITY_VALUE = 5 WHERE RESOURCE_ID = 2077 AND ROLE_ID = 134
UPDATE [dbo].[SGS_SECURITY]  SET SECURITY_VALUE = 5 WHERE RESOURCE_ID = 2079 AND ROLE_ID = 134
UPDATE [dbo].[SGS_SECURITY]  SET SECURITY_VALUE = 5 WHERE RESOURCE_ID = 2080 AND ROLE_ID = 20
UPDATE [dbo].[SGS_SECURITY]  SET SECURITY_VALUE = 5 WHERE RESOURCE_ID = 2081 AND ROLE_ID = 20
UPDATE [dbo].[SGS_SECURITY]  SET SECURITY_VALUE = 5 WHERE RESOURCE_ID = 2082 AND ROLE_ID = 20
UPDATE [dbo].[SGS_SECURITY]  SET SECURITY_VALUE = 5 WHERE RESOURCE_ID = 2083 AND ROLE_ID = 20
UPDATE [dbo].[SGS_SECURITY]  SET SECURITY_VALUE = 5 WHERE RESOURCE_ID = 2084 AND ROLE_ID = 20