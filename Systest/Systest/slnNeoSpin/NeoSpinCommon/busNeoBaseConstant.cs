#region [Using Directives]
using System;
#endregion [Using Directives]

namespace NeoBase.Common
{
    [Serializable]
    public static class busNeoBaseConstants
    {
        #region [Reports]

        public const String REPORT_DATE_FORMAT = "MM/dd/yyyy";
        public const string SQL_DATE_FORMAT = "yyyy-MM-dd HH:mm:ss.fff";

        public const string IN_PROCCESS = "INPC";
        public const string BPMCOMPLETEDACTIVITY = "PROC";

        public const string DEVL_ENV_REGION_CODE = "DEVL";
        public const string PROD_REGION_CODE = "PROD";      
        #endregion [Reports]

        public const string ROLE = "Role";
        public const string NAME = "NAME";
        public const string IS_USER_HAVE_PRIVACY_SETTING_ACCESS = "IsUserHavePrivacySettingAccess";
        public const string PROCESS = "Process";
        public const string END_DATE = "EndDate";
        public const string START_DATE = "StartDate";
        public const string PROCESS_ID = "Prcs_ID";
        public const string USER_ID = "User_Id";

        #region [Common]

        public const int ZERO = 0;
        #endregion [Common]
        public static class BPM
        {
            public const string OutOfOfficeRecordStatus = "VOID";
            public const string SOURCE_ONLINE_DESCREPTION = "Online";
            public const string SOURCE_ONLINE_VALUE = "ONLI";
            public const string SOURCE_BATCH_DESCREPTION = "Batch";
            public const string SOURCE_BATCH_VALUE = "BTCH";
            public const string SOURCE_MESSAGE_FLOW_DESCREPTION = "Message Flow";
            public const string SOURCE_MESSAGE_FLOW_VALUE = "MSGF";
            public const string SOURCE_INDEXING_DESCREPTION = "Scanning & Indexing";
            public const string SOURCE_INDEXING_VALUE = "INDX";
            public const string PRIORITY_NORMAL_DESCREPTION = "Normal";
            public const string PRIORITY_NORMAL_VALUE = "NORM";
            public const string PRIORITY_HIGH_DESCREPTION = "High";
            public const string PRIORITY_HIGH_VALUE = "HIGH";
            public const string PROCESS_INSTANCE_CREATED_DATE = "PI_CREATED_DATE";
            public const string ACTIVITY_INSTANCE_START_DATE = "AI_START_DATE";
            public const string ACTIVITY_INSTANCE_END_DATE = "AI_END_DATE";
            public const string BPM_ACTIVITY_INSTANCE_PERSON_NAME = "PERSON_NAME";
            public const string BPM_ACTIVITY_INSTANCE_ORG_NAME = "ORG_NAME";
            public const string BPM_ACTIVITY_INSTANCE_COMPLETED = "PROC";
            public const string BPM_ACTIVITY_INSTANCE_INITIATED = "UNPC";
            public const string BPM_DOCUMENT_UPLOAD = "BPM_UPLD";

            /// <summary>
            /// FM 6.0.0.35 change
            /// </summary>
            public const string BTN_TERMINATE_ACTIVITY = "btnCancelActivity";
        }

        public static class Message
        {
            public const int MessageID_30100035 = 30100035;
            public const int MessageID_30100042 = 30100042;
            public const int MessageID_20007031 = 20007031;

        }

    }
}
