
create table [dbo].[sgs_act_log_inst_actn_step_dtl](
               [actn_step_dtl_id] [int] identity(1,1) not null,
               [transaction_id] [varchar](100) not null,
               [sub_transaction_id] [varchar](20) not null,
               [call_details] [varchar](200) not null,
               [start_time] [datetime] not null,
               [end_time] [datetime] not null,
               [time_in_ms] [int] null,
               [parameters] [varchar](max) null,
               [status] [varchar](1) not null,
               [error_details] [varchar](max) null,
constraint [pk_act_log_inst_actn_step_dtl] primary key clustered 
(
               [actn_step_dtl_id] asc
)with (pad_index = off, statistics_norecompute = off, ignore_dup_key = off, allow_row_locks = on, allow_page_locks = on, fillfactor = 90) on [primary]
) on [primary] textimage_on [primary]
Go
