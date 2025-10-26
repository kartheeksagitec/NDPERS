/****** Object:  View [dbo].[sgv_act_log_inst_actn_step]    Script Date: 12/14/2017 5:01:22 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE view [dbo].[sgv_act_log_inst_actn_step]
as
select a.*,
       start_time to_start_time,
	   case when time_in_ms >= b.appserver_limit then 'Y' else 'N' end slow_flag,
	   case when error_message is null then 'N' else 'Y' end error_flag
from sgs_act_log_inst_actn_step a,
     sgs_act_log b


GO


