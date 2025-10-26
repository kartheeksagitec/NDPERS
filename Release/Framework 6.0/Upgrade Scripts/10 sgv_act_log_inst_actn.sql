


/****** Object:  View [dbo].[sgv_act_log_inst_actn]    Script Date: 12/14/2017 5:00:48 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE view [dbo].[sgv_act_log_inst_actn]
as
select case when charindex('(', action_name) > 0 then left(action_name, charindex('(', action_name)-1) else action_name end + '.' +  
       (case when isnull(action_target,'') = '' then action_source else action_target end) transaction_name,
	   start_time to_start_time,
	   a.*,
	   case when time_in_ms >= b.transaction_limit then 'Y' else 'N' end slow_flag
from sgs_act_log_inst_actn a,
     sgs_act_log b



GO


