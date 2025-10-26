#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;
using System.Data;
using System.Data.Common;
using System.Linq;
using Sagitec.ExceptionPub;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busPir : busExtendBase
    {
        public busPir()
        {
        }

        private cdoPir _icdoPir;
        public cdoPir icdoPir
        {
            get
            {
                return _icdoPir;
            }

            set
            {
                _icdoPir = value;
            }
        }

        private Collection<busPirHistory> _iclbPirHistory;
        public Collection<busPirHistory> iclbPirHistory
        {
            get
            {
                return _iclbPirHistory;
            }

            set
            {
                _iclbPirHistory = value;
            }
        }


        public Collection<busPIRAttachment> iclbPIRAttchment
        {
            get;set;
        }

        private busUser _ibusAssignedTo;
        public busUser ibusAssignedTo
        {
            get
            {
                return _ibusAssignedTo;
            }
            set
            {
                _ibusAssignedTo = value;
            }
        }

        private busUser _ibusReportedBy;
        public busUser ibusReportedBy
        {
            get
            {
                return _ibusReportedBy;
            }
            set
            {
                _ibusReportedBy = value;
            }
        }

        private busTestcase _ibusTestcase;
        public busTestcase ibusTestcase
        {
            get
            {
                return _ibusTestcase;
            }
            set
            {
                _ibusTestcase = value;
            }
        }

        public bool FindPir(int aintPirId)
        {
            bool lblnResult = false;
            if (_icdoPir == null)
            {
                _icdoPir = new cdoPir();
            }
            if (_icdoPir.SelectRow(new object[1] { aintPirId }))
            {
                lblnResult = true;

                LoadPirHistory();
            }
            return lblnResult;
        }

        public void LoadPir()
        {
            this.FindPir(_icdoPir.pir_id);
        }

        public void LoadPirHistory()
        {
            DataTable ldtbList = Select<cdoPirHistory>(
                new string[1] { "pir_id" },
                new object[1] { icdoPir.pir_id }, null, "pir_history_id desc");
            _iclbPirHistory = GetCollection<busPirHistory>(ldtbList, "icdoPirHistory");
            foreach (busPirHistory lobjPIRHistory in _iclbPirHistory)
            {
                lobjPIRHistory.LoadAssignedTo();
            }
        }

        public void LoadPIRAttachment(List<string> alstResult)
        {
            iclbPIRAttchment = new Collection<busPIRAttachment>();
            foreach (var dr in alstResult)
            {
                busPIRAttachment lobjPIRAttachment = new busPIRAttachment();
                lobjPIRAttachment.istrAttchmentName = dr;
                iclbPIRAttchment.Add(lobjPIRAttachment);
            }
        }
        public void LoadAssignedTo()
        {
            if (_ibusAssignedTo == null)
            {
                _ibusAssignedTo = new busUser();
            }
            _ibusAssignedTo.FindUser(icdoPir.assigned_to_id);
        }

        public void LoadReportedBy()
        {
            if (_ibusReportedBy == null)
            {
                _ibusReportedBy = new busUser();
            }
            _ibusReportedBy.FindUser(icdoPir.reported_by_id);
        }
        //PIR 23829	Add another field to the PIR process for Notification. Send mail to Referent ID
        public busUser ibusReferentIDUser { get; set; }
        public void LoadReferentID()
        {
            if (ibusReferentIDUser.IsNull())
                ibusReferentIDUser = new busUser { icdoUser = new cdoUser() };
            ibusReferentIDUser.FindUser(icdoPir.referent_id);
        }
        public busUser ibusLoggedUser { get; set; }

        public void LoadLoggedUser()
        {
            if (ibusLoggedUser.IsNull())
                ibusLoggedUser = new busUser { icdoUser = new cdoUser() };
            ibusLoggedUser.FindUser(iobjPassInfo.iintUserSerialID);
        }

        public string istrCreateByUserFullName { get; set; }
        public busUser ibusCreatedByUser { get; set; }
        public void LoadCreatedByUser()
        {
            if (ibusCreatedByUser.IsNull())
                ibusCreatedByUser = new busUser { icdoUser = new cdoUser() };
            if(ibusCreatedByUser.FindUserByUserName(icdoPir.created_by))
                istrCreateByUserFullName = Convert.ToString(ibusCreatedByUser.icdoUser.User_full_name);
        }
        public void LoadTestCase()
        {
            if (_ibusTestcase == null)
            {
                _ibusTestcase = new busTestcase();
            }
            _ibusTestcase.FindTestcase(_icdoPir.testcase_scenario);
        }
        public override void AfterPersistChanges()
        {
            LoadPir();
            LoadPirHistory();
            base.AfterPersistChanges();
        }
        /// <summary>
        /// //F/W Upgrade PIR - Htx screens are not loading since WSS responsive portla conversion.
        /// Implementation changed until we get F/W fix for this
        /// </summary>
        /// <returns></returns>
        public Collection<busScreen> GetScreens()
        {
            string lstrBaseMetaDataPath = SystemSettings.Instance.MetaDataPath;
            Collection<busScreen> lclbScreens = new Collection<busScreen>();
            if (!string.IsNullOrEmpty(lstrBaseMetaDataPath))
            {
                //Loading all wfm screens
                Directory.GetFiles(lstrBaseMetaDataPath, "wfm*.xml", SearchOption.AllDirectories)?.ForEach(lstrFullFileName =>
                lclbScreens.Add(new busScreen()
                {
                    istrScreenName = Path.GetFileNameWithoutExtension(lstrFullFileName)
                }));
                //Loading all htx screens
                Directory.GetFiles(lstrBaseMetaDataPath, "htx*.xml", SearchOption.AllDirectories)?.ForEach(lstrFullFileName =>
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(lstrFullFileName);
                    XmlNode lformNode = xmlDocument.FirstChild;
                    string lstrFormID = lformNode?.Attributes["ID"]?.Value;
                    if (!string.IsNullOrEmpty(lstrFormID))
                    {
                        lclbScreens.Add(new busScreen() { istrScreenName = lstrFormID });
                    }
                });
            }
            return lclbScreens.Count > 0 ? lclbScreens.OrderBy(screen=>screen.istrScreenName).ToList().ToCollection() : lclbScreens;
        }

        public override void BeforePersistChanges()
        {
            //PIR 19929  A user is not able to remove the Priority Date within a PIR screen. Added else condition
            if (!String.IsNullOrEmpty(_icdoPir.istrPriorityDate))
            {
                _icdoPir.priority_date = Convert.ToDateTime(icdoPir.istrPriorityDate.ToString());
            }
            else
            {
                _icdoPir.priority_date = DateTime.MinValue;
            }
            icdoPir.status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(40, icdoPir.status_value); //PROD PIR ID 6715
            if (busGlobalFunctions.GetData1ByCodeValue(icdoPir.status_id, icdoPir.status_value, iobjPassInfo) == busConstant.Flag_Yes && // PROD PIR ID 5634
                Convert.ToString(icdoPir.ihstOldValues["status_value"]) != icdoPir.status_value)
                SendMail();
            base.BeforePersistChanges();            
        }

        public void SendMail()
        {
            if (ibusLoggedUser.IsNull()) LoadLoggedUser();
            LoadAssignedTo();
            LoadReportedBy();
            LoadReferentID();
            if (ibusCreatedByUser.IsNull()) LoadCreatedByUser();
			//PIR 23373 Send email to created by user and email occurance only once
            try
            {
                string lstrStatus = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(40, icdoPir.status_value);
                string lstrSubject = string.Format(busGlobalFunctions.GetData1ByCodeValue(52, "SCOT", iobjPassInfo), icdoPir.pir_id.ToString());
                string lstrMessage = string.Format(busGlobalFunctions.GetData2ByCodeValue(52, "SCOT", iobjPassInfo), icdoPir.pir_description.ToString(),
                                        icdoPir.status_description.Substring(lstrStatus.IndexOf('.') + 1).Trim());
                if (ibusAssignedTo.IsNotNull() && ibusLoggedUser.icdoUser.user_serial_id != ibusAssignedTo.icdoUser.user_serial_id && ibusAssignedTo.icdoUser.user_serial_id > 0 )
                {
                    busGlobalFunctions.SendMail(ibusLoggedUser.icdoUser.email_address, ibusAssignedTo.icdoUser.email_address, lstrSubject, lstrMessage, true, true);
                }
                if (ibusReportedBy.IsNotNull() && ibusLoggedUser.icdoUser.user_serial_id != ibusReportedBy.icdoUser.user_serial_id && ibusReportedBy.icdoUser.user_serial_id > 0
                                                && ibusAssignedTo.icdoUser.user_serial_id != ibusReportedBy.icdoUser.user_serial_id)
                {
                    busGlobalFunctions.SendMail(ibusLoggedUser.icdoUser.email_address, ibusReportedBy.icdoUser.email_address, lstrSubject, lstrMessage, true, true);
                }
                if (ibusLoggedUser.icdoUser.user_serial_id != ibusCreatedByUser.icdoUser.user_serial_id 
                    && ibusAssignedTo.icdoUser.user_serial_id != ibusCreatedByUser.icdoUser.user_serial_id
					&& ibusReportedBy.icdoUser.user_serial_id != ibusCreatedByUser.icdoUser.user_serial_id)
                {
                    busGlobalFunctions.SendMail(ibusLoggedUser.icdoUser.email_address, ibusCreatedByUser.icdoUser.email_address, lstrSubject, lstrMessage, true, true);
                }
                //PIR 23829	Add another field to the PIR process for Notification. Send mail to Referent ID
                if (ibusLoggedUser.icdoUser.user_serial_id != ibusReferentIDUser.icdoUser.user_serial_id && ibusReferentIDUser.icdoUser.user_serial_id > 0 
                    && ibusAssignedTo.icdoUser.user_serial_id != ibusReferentIDUser.icdoUser.user_serial_id
                    && ibusReportedBy.icdoUser.user_serial_id != ibusReferentIDUser.icdoUser.user_serial_id
                    && ibusCreatedByUser.icdoUser.user_serial_id != ibusReferentIDUser.icdoUser.user_serial_id)
                {
                    busGlobalFunctions.SendMail(ibusLoggedUser.icdoUser.email_address, ibusReferentIDUser.icdoUser.email_address, lstrSubject, lstrMessage, true, true);
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.Publish(ex);
            }
        }

        public int GetPIRAttachmentCount()
        {
            if (this.iclbPIRAttchment.IsNotNull() && this.iclbPIRAttchment.Count > 0)
            {
                return this.iclbPIRAttchment.Count;
            }
            return 0;
        }
    }
}
