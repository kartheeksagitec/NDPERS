using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeoSpin.Common
{
    public class WorkflowConstants
    {
        #region Workflow constants
        public const string WorkflowProcessInstanceStatus_NotProcessed = "UNPC";
        public const string WorkflowProcessInstanceStatus_Processed = "PROC";
        public const string WorkflowProcessInstanceStatus_InProgress = "INPC";
        public const string WorkflowProcessInstanceStatus_Terminated = "TERM";
        public const string WorkflowProcessInstanceStatus_Aborted = "ABRT";

        //Constants for Request Status
        public const string WorkflowRequestStatus_NotProcessed = "UNPC";
        public const string WorkflowRequestStatus_Processed = "PROC";
        public const string WorkflowRequestStatus_Ignored = "IGNO";
        
        //Constants for request source 
        public const string WorkflowRequestSource_Online = "ONLI";
        public const string WorkflowRequestSource_Batch = "BTCH";
        public const string WorkflowRequestSource_Scan_And_Index = "SCIN";
        
        //Constants for ActivityInstance Status
        public const string ActivityInstanceStatus_Initiated = "UNPC";
        public const string ActivityInstanceStatus_InProcess = "INPC";
        public const string ActivityInstanceStatus_Processed = "PROC";
        public const string ActivityInstanceStatus_Released = "RELE";
        public const string ActivityInstanceStatus_Suspended = "SUSP";
        public const string ActivityInstanceStatus_Resumed = "RESU";
        public const string ActivityInstanceStatus_Cancelled = "CANC";
        public const string ActivityInstanceStatus_Returned = "RETU";
        public const string ActivityInstanceStatus_ReturnedToAudit = "REAU";
        
        public const string WorkflowServiceURL = "NEOFLOW_SERVICE_TIER_URL";

        public const string MyBasketFilter_WorkPool = "WKPO";
        public const string MyBasketFilter_WorkAssigned = "ASWO";
        public const string MyBasketFilter_CompletedWork = "COWO";
        public const string MyBasketFilter_SuspendedWork = "SUWO";

        public const string WorkflowProcessType_Person = "PERS";
        public const string WorkflowProcessType_Organization = "ORGN";

        public const string Send_Reminder = "REM";
        public const string Reassign_To_User = "REAS";
        public const string Put_Back_In_Queue = "PBQ";
        public const string Move_To_Diff_Queue = "MDQ";

        public const string Checked_Value = "Y";

        //Pattern Providers
        public const string No_Provider = "NOP";
        public const string Round_Robin = "RR";
        public const string Least_Used_Resource = "LUR";
        public const string Random = "RND";
        public const string Fifo = "FIFO";
        public const string Lifo = "LIFO";
        public const string Custom = "CUST";

        //Patterns
        public const string Dispatch_Pattern_Auto = "AUTO";
        public const string Dispatch_Pattern_Manual = "MAN";

        public const string CustomActivitieNamespace = "clr-namespace:NeoFlow.Activities;assembly=NeoFlowActivities";        
        public const string ProcessMaintainance_FilePath = "ProcessMaintenanceFilePath";
        public const string ProcessMaintainance_sfwMap_NameAttribute = "sfwName";
        public const string ProcessMaintainance_sfwActivity_DisplayNameAttribute = "sfwDisplayName";
        public const string ProcessMaintainance_sfwActivity_ScreenNameAttribute = "sfwScreenName";
        public const string ProcessMaintainance_sfwForm_ModeAttribute = "sfwMode";
        public const string ProcessMaintainance_sfwForm_FocusControlIDAttribute = "sfwFocusControlID";
        public const string ProcessMaintainance_sfwForm_FormNameAttribute = "sfwFormName";
        
        public const string ProcessMaintainance_sfwParameter_ParamaterName = "sfwParamaterName";
        public const string ProcessMaintainance_sfwParameter_DataType = "sfwDataType";
        public const string ProcessMaintainance_sfwParameter_ParameterSource = "sfwParameterSource";
        public const string ProcessMaintainance_sfwParameter_ParameterValue = "sfwParameterValue";
        public const string ProcessMaintainance_sfwParameter_FieldName = "sfwFieldName";
            

        public const string XAMLFile_DisplayName = "DisplayName";
        public const string XAMLFile_BookmarkName = "BookmarkName";
        #endregion

        #region [WorkFlow Constants]

        //Constants for Request Status
        public const string WORKFLOW_REQUEST_STATUS_NOT_PROCESSED = "UNPC";

        //Constants for request source 
        public const string WORKFLOW_REQUEST_SOURCE_ONLINE = "ONLI";
        public const string WORKFLOW_REQUEST_SOURCE_SCANNING = "INDX";
        public const string FLAG_NO = "N";
        public const string FLAG_YES = "Y";

        //Constants for my basket filter.
        public const string MYBASKET_FILTER_WORKPOOL = "WKPO";

        //Constant for the tables     
        public const string REQUEST_TABLE = "SGW_BPM_REQUEST";

        //Case types       
        public const string PROCESS_NAME = "Process_Name";
        public const string PROCESS_DESCRIPTION = "Process_Description";

        //Constants for datatype of the queue rule attributes
        public const string DATATYPE_INT = "int";
        public const string DATATYPE_STRING = "string";
        public const string DATATYPE_DATETIME = "datetime";

        //Constants for filter options.
        public const string ACTIVITYINSTANCE_STATUS_UNPC_OR_RELE = "'UNPC','RTRN'";
        public const string ACTIVITYINSTANCE_STATUS_TO_REASSIGN = "'UNPC','INPC','RTRN','SUSP','RWRK','RCHK','RESU'";

        public const string OPERATOR_EQUALTO = "=";
        public const string OPERATOR_BETWEEN = "between";
        public const string BUILD_WHERE_CLAUSE_IN = "in";
        public const string BUILD_WHERE_CLAUSE_AND = " and ";

        #endregion [WorkFlow Constants]


        #region [Message Constants]

        public const int MESSAGE_ID_1 = 1;
        public const int MESSAGE_ID_2 = 2;
        public const int MESSAGE_ID_3 = 3;
        public const int MESSAGE_ID_5 = 5;

        public const int MESSAGE_ID_1520 = 1520;
        public const int MESSAGE_ID_1521 = 1521;
        public const int MESSAGE_ID_1522 = 1522;
        public const int MESSAGE_ID_1523 = 1523;
        public const int MESSAGE_ID_1524 = 1524;
        public const int MESSAGE_ID_1537 = 1537;
        public const int MESSAGE_ID_1538 = 1538;
        public const int MESSAGE_ID_1569 = 1569;
        public const int MESSAGE_ID_1573 = 1573;
        public const int MESSAGE_ID_20008002 = 20008002;
        public const int MESSAGE_ID_1582 = 1582;

        #endregion [Constants]
    }
}
