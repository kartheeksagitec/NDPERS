
/****** Object:  View [dbo].[sgv_database_transaction]    Script Date: 12/14/2017 5:01:52 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE view [dbo].[sgv_database_transaction] 
as
select a.*,  
       start_time to_start_time,
       case when time_in_ms >= query_limit then 'Y' else 'N' end slow_flag,
	   case when len(query) > 8000 then Convert(varchar(32),HASHBYTES('SHA2_256', substring(query, 0, 8000))) 
	   else Convert(varchar(32),HASHBYTES('SHA2_256', query), 2) end query_hash  
  from sgs_act_log_inst_query a,
       sgs_act_log b
 

GO


