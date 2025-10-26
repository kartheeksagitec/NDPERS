IF COL_LENGTH('sgw_bpm_request_parameter', 'PARAMETER_CODE_ID') IS NULL
BEGIN
ALTER TABLE sgw_bpm_request_parameter
ADD PARAMETER_CODE_ID int NULL
END
GO