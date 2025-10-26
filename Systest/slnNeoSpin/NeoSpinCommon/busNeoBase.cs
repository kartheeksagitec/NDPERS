#region [Using Directives]using Sagitec.Bpm;
using Sagitec.Bpm;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using System;
using System.Collections;
using System.Data;
#endregion [Using Directives]

namespace NeoBase.Common
{
    [Serializable]
    public partial class busNeoBase : busBase
    {
        #region [Members]

        /// <summary>
        /// Error Message to be used in the screens. Override default message on button click.
        /// </summary>
        public string istrErrorMessage { get; set; }

        public int iintPersonID { get; set; }

        public static bool iblnReady { get; set; }

        /// <summary>
        /// Gets or sets AGENCY Actor
        /// </summary>
        public string istrAGENCYActor { get; set; }

        /// <summary>
        /// Holds all restricted persons
        /// </summary>
        public DataTable idtbRestrictedPersons;

        #endregion [Members]

        #region [Public Methods]

        static busNeoBase()
        {
            //You can assign your handlers to the func/action methods provided here
            //busCommunication.CheckHold = busSolCommunication.CheckHoldOnParticipantLevel;
        }

        public const string _istrTraceFormatString = "Timestamp: {0}; User Name: {1}; Object Name: {2}; Event Name: {3};";
        public const string _istrObjectName = "TraceLogForModule - {0} ";
        public const string _istrDateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";

        public static utlWhereClause GetWhereClause(object aobjValue1, object aobjValue2,
           string astrFieldName, string astrDataType, string astrOperator, string astrCondition, string astrQueryId)
        {
            utlWhereClause lobjWhereClause = new utlWhereClause();

            lobjWhereClause.iobjValue1 = aobjValue1.Clone();
            lobjWhereClause.iobjValue2 = aobjValue2.Clone();
            lobjWhereClause.istrQueryId = astrQueryId;
            lobjWhereClause.istrFieldName = astrFieldName;
            lobjWhereClause.istrDataType = astrDataType;
            lobjWhereClause.istrOperator = astrOperator;
            lobjWhereClause.istrCondition = astrCondition;

            return lobjWhereClause;
        }

        public static string DeriveMimeTypeFromFileName(string astrFileName)
        {
            string lstrMimeType = "application/octet-stream";
            string[] larrDotSplit = astrFileName.Split('.');
            if (larrDotSplit != null && larrDotSplit.Length > 0)
            {
                string lstrFileExtension = larrDotSplit[larrDotSplit.Length - 1];
                switch (lstrFileExtension.ToLower())
                {
                    case "pdf":
                        lstrMimeType = "application/pdf";
                        break;
                    case "doc":
                    case "docx":
                        lstrMimeType = "application/msword";
                        break;
                    case "html":
                        lstrMimeType = "application/iexplore";
                        break;
                }
            }

            return lstrMimeType;
        }

        /// <summary>
        /// To Log the error message from code, by passing message-id
        /// </summary>
        /// <param name="aintErrorMessageId"></param>
        //public void LogErrorMessages(int aintErrorMessageId, object[] aobjParams = null)
        //{
        //    string lstrErrorMessage = iobjPassInfo.isrvDBCache.GetMessageText(aintErrorMessageId);

        //    if (this.iarrErrors.IsNull())
        //    {
        //        iarrErrors = new ArrayList();
        //    }
        //    if (aobjParams == null)
        //    {
        //        iarrErrors.Add(AddError(aintErrorMessageId, lstrErrorMessage));
        //    }
        //    else
        //    {
        //        iarrErrors.Add(AddError(0, lstrErrorMessage.Format(aobjParams)));
        //    }
        //}


        public static string EncryptPassword(string astrPassword)
        {
            return HelperFunction.SagitecEncrypt(null, astrPassword);
        }

        public static string DecryptPassword(string astrPassword)
        {
            return HelperFunction.SagitecDecrypt(null, astrPassword);
        }
        /// <summary>
        /// Override this method in your class if you have a scenario where the autoEnclosure template is based on a other entity
        /// than the parent template is based on.
        /// And pass the appropriate BusinessObject to busBase.GetCorrespondenceObject in case of autoEnclosure in that method.
        /// </summary>
        /// <param name="aobjCorrespondenceInfo"></param>
        /// <param name="ahstQueryBookmarks"></param>
        /// <returns></returns>
        //public virtual busBase GetCorrespondenceObject(utlCorresPondenceInfo aobjCorrespondenceInfo, Hashtable ahstQueryBookmarks, busGenerateCommunicationParams abusGenerateCommunicationParams = null)
        //{
        //    return busBase.GetCorrespondenceObject(aobjCorrespondenceInfo, this,
        //        ahstQueryBookmarks);
        //}

        /// <summary>
        /// Overriden to validate hard errors on main as well as child objects.
        /// </summary>
        /// <returns></returns>
        //protected ArrayList ValidateAllHardErrors()
        //{
        //    if (this.iarrErrors.IsNullOrEmpty())
        //    {
        //        this.iarrErrors = new ArrayList();
        //    }
        //    foreach (busMainBase lbusLobjBase in this.iarrBusChangeLog)
        //    {
        //        if (lbusLobjBase is busBase)
        //        {
        //            ((busBase)lbusLobjBase).iarrErrors = new ArrayList();
        //            ((busBase)lbusLobjBase).ValidateHardErrors(iobjPassInfo.ienmPageMode);
        //            this.iarrErrors.AddRange(((busBase)lbusLobjBase).iarrErrors);
        //        }
        //    }

        //    return this.iarrErrors;
        //}

        /// <summary>
        /// Resets the object properies to default 
        /// </summary>
        public virtual void ResetObjectProperties()
        {

        }

        public override void LoadLookupChild(DataRow adtrChild)
        {
            base.LoadLookupChild(adtrChild);
        }

        public bool IsDevelopmentRegion
        {
            get
            {
                return busNeoBaseConstants.DEVL_ENV_REGION_CODE.Equals(utlPassInfo.iobjPassInfo.isrvDBCache.GetDBSystemManagement().Rows[0]["REGION_VALUE"].ToString(), StringComparison.CurrentCultureIgnoreCase);
            }
        }

        public bool IsNonProductionRegion
        {
            get
            {
                return busNeoBaseConstants.PROD_REGION_CODE.Equals(utlPassInfo.iobjPassInfo.isrvDBCache.GetDBSystemManagement().Rows[0]["REGION_VALUE"].ToString(), StringComparison.CurrentCultureIgnoreCase);
            }
        }

        public void InitializeExtendedObjects()
        {
            bool iblnTemp = Sagitec.Bpm.BpmHelper.iblnDebugMode;
            utlPassInfo.iobjPassInfo = BPMDBHelper.CreatePassInfo("Bpm Service");
            try
            {
                busBpmCaseInstance lbusBpmCaseInstance = ClassMapper.GetObject<busBpmCaseInstance>();
                busBpmProcessInstance lbusBpmProcessInstance = ClassMapper.GetObject<busBpmProcessInstance>();
                busBpmActivityInstance lbusBpmActivityInstance = ClassMapper.GetObject<busBpmActivityInstance>();
                busBpmRequest ExtendedEmptyObject = ClassMapper.GetObject<busBpmRequest>();
                busBpmTimerActivityInstanceDetails lbusBpmTimerActivityInstanceDetails = ClassMapper.GetObject<busBpmTimerActivityInstanceDetails>();
                busBpmProcessEscalationInstance lbusBpmProcessEscalationInstance = ClassMapper.GetObject<busBpmProcessEscalationInstance>();
                busBpmEscalationInstance lbusBpmEscalationInstance = ClassMapper.GetObject<busBpmEscalationInstance>();
            }
            finally
            {
                BPMDBHelper.FreePassInfo(utlPassInfo.iobjPassInfo);
            }
        }

        #endregion [Public Methods]
    }
}
