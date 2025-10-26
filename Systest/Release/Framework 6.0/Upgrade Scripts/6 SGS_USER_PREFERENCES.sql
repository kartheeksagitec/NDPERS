/****** Object:  Table [dbo].[SGS_USER_PREFERENCES]    Script Date: 13/10/2017 6:09:52 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[SGS_USER_PREFERENCES](
       [USER_PREFERENCES_ID] [int] IDENTITY(1,1) NOT NULL,
       [USER_NAME] [varchar](50) NOT NULL,
       [APPLICATION_NAME] [varchar](50) NOT NULL,
       [USE_SYSTEM_PREFERENCES] [varchar](1) NOT NULL DEFAULT ('N'),
       [CREATED_BY] [varchar](50) NOT NULL,
       [CREATED_DATE] [DateTime] NOT NULL,
       [MODIFIED_BY] [varchar](50) NOT NULL,
       [MODIFIED_DATE] [DateTime] NOT NULL,
       [UPDATE_SEQ] [int] NOT NULL,
       [INITIAL_PAGE] [varchar](50) NULL,
       [USER_PREF_VALUES] [varchar](max) NULL,
CONSTRAINT [PK_SGS_USER_PREFERENCES] PRIMARY KEY CLUSTERED 
(
       [USER_PREFERENCES_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = ON, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO



/****** Object:  Table dbo.SGS_USER_PREFERENCES  ********/
INSERT dbo.SGS_USER_PREFERENCES ([USER_NAME],[APPLICATION_NAME],[USE_SYSTEM_PREFERENCES],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ],[INITIAL_PAGE],[USER_PREF_VALUES])VALUES('!mshinde','InternalPortal','N','System','Aug 30 2017  3:39:38:850PM','sagitec','Sep  8 2017  3:58:11:513PM',1,'wfmCodeLookup','{"initial_page":"wfmCodeLookup","use_tag_list":"N","use_grid_toolbar":"N","use_grid_common_filterbox":"N","use_grid_settings":"N","use_store_state_buttons":"N","use_view_edit_for_open_button":"N","use_neogrid":"N","on_hover_tooltip_form":"N","page_size":10,"use_system_preferences":"N"}')
INSERT dbo.SGS_USER_PREFERENCES ([USER_NAME],[APPLICATION_NAME],[USE_SYSTEM_PREFERENCES],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ],[INITIAL_PAGE],[USER_PREF_VALUES])VALUES('!mshinde','MemberPortal','N','System','Sep 13 2017 11:55:07:780AM','System','Sep 13 2017 11:55:07:780AM',0,'wfmMSSHomeMaintenance','{"initial_page":"wfmMSSHomeMaintenance","use_tag_list":"N","use_grid_toolbar":"N","use_grid_common_filterbox":"N","use_grid_settings":"N","use_store_state_buttons":"N","use_view_edit_for_open_button":"N","use_neogrid":"N","on_hover_tooltip_form":"N","page_size":10,"use_system_preferences":"N"}')
INSERT dbo.SGS_USER_PREFERENCES ([USER_NAME],[APPLICATION_NAME],[USE_SYSTEM_PREFERENCES],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ],[INITIAL_PAGE],[USER_PREF_VALUES])VALUES('!mshinde','EmployerPortal','N','System','Sep 13 2017 11:55:07:780AM','System','Sep 13 2017 11:55:07:780AM',0,'wfmESSHomeCSContactMaintenance','{"initial_page":"wfmESSHomeCSContactMaintenance","use_tag_list":"N","use_grid_toolbar":"N","use_grid_common_filterbox":"N","use_grid_settings":"N","use_store_state_buttons":"N","use_view_edit_for_open_button":"N","use_neogrid":"N","on_hover_tooltip_form":"N","page_size":10,"use_system_preferences":"N"}')




/****** Object:  Table [dbo].[SGS_USER_PREF_PAGE_STATE]    Script Date: 13/10/2017 6:13:45 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[SGS_USER_PREF_PAGE_STATE](
       [USER_PREF_PAGE_STATE_ID] [int] IDENTITY(1,1) NOT NULL,
       [USER_NAME] [varchar](50) NOT NULL,
       [APPLICATION_NAME] [varchar](50) NOT NULL,
       [FORM_NAME] [varchar] (128) NOT NULL,
       [PAGE_STATE_DATA] [image] NULL,
CONSTRAINT [PK_USER_PREF_PAGE_STATE_ID] PRIMARY KEY CLUSTERED 
(
       [USER_PREF_PAGE_STATE_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = ON, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO





