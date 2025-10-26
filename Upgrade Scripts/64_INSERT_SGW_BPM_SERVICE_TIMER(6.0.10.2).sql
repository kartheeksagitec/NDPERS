-----------------------------------------
--Created By	:	Siddharth Sabadra
--Created On	:	12th February 2020
--Description	:	Adding Timer to SGW_BPM_SERVICE_TIMER table
------------------------------------------------------------------------------------------------------------------------ 
IF NOT EXISTS(select * from SGW_BPM_SERVICE_TIMER where ACTION_METHOD='ResumeStuckTasks') 
 INSERT into SGW_BPM_SERVICE_TIMER(ACTION_METHOD, INTERVAL, ENABLED, BPM_TRACING_FLAG, BPM_QUERY_TRACING_FLAG, BPM_QUERY_LIMIT, BPM_QUERY_LIMIT_ALERT, BPM_QUERY_HIGH_LIMIT, BPM_QUERY_HIGH_LIMIT_ALERT) values('ResumeStuckTasks',300000,'N','N','N',5000,NULL,5000,NULL);



 