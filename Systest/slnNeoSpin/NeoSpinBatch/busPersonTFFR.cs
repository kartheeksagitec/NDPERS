#region Using directives
using System;
using System.Data;
using System.Collections.Generic;
using NeoSpin.CustomDataObjects;
using NeoSpin.BusinessObjects;
using System.Collections.ObjectModel;
using Sagitec.DataObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.CustomDataObjects;
using Sagitec.BusinessObjects;
using System.Linq;
#endregion



namespace NeoSpinBatch

{
    public class busPersonTFFR : busNeoSpinBatch
    {
        public busPersonTFFR()
        { }
        public Collection<busPerson> iclbPerson { get; set; }
        private Collection<busTFFRSSNOutFile> _iclbTFFRSSNOutFile;

        public Collection<busTFFRSSNOutFile> iclbTFFRSSNOutFile
        {
            get { return _iclbTFFRSSNOutFile; }
            set { _iclbTFFRSSNOutFile = value; }
        }
        /// <summary>
        /// Function to generate file for Dual Member Request File
        /// </summary>
        public void GenerateDualMemberRequestFile()
        {
            try
            {

                istrProcessName = "Loading OutBound File Data";
                idlgUpdateProcessLog("Creating TFFR OutBound File", "INFO", istrProcessName);
                DataTable ldtfileHeaderData = busBase.Select("cdoPerson.FileHeader118InUpload", new object[] { });
                foreach (DataRow ldtRow in ldtfileHeaderData.Rows)
                {
                     
                    LoadPersonDetails(Convert.ToInt32(ldtRow["FILE_HDR_ID"]));
                    busProcessOutboundFile lobjProcessOutFiles = new busProcessOutboundFile();
                    lobjProcessOutFiles.iarrParameters = new object[3];
                    lobjProcessOutFiles.iarrParameters[0] = iclbTFFRSSNOutFile;
                    lobjProcessOutFiles.CreateOutboundFile(117);
                    istrProcessName = "TFFR Outbound File Creating";
                    idlgUpdateProcessLog("TFFR Outbound File Created Successfully", "INFO", istrProcessName);
                    busFileHdr lobjFileHdr = new busFileHdr();
                    lobjFileHdr.FindFileHdr(Convert.ToInt32(ldtRow["FILE_HDR_ID"]));
                    lobjFileHdr.icdoFileHdr.status_value = busConstant.PayrollHeaderStatusProcessedWithWarnings;
                    lobjFileHdr.icdoFileHdr.Update();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void LoadPersonDetails(int aintFileHdrId)
        {
            iclbTFFRSSNOutFile = new Collection<busTFFRSSNOutFile>();
            DataTable ldtfileHeaderSSNData = busBase.Select("cdoPerson.SSNMatchFile", new object[1] { aintFileHdrId });
            foreach (DataRow ldtRow in ldtfileHeaderSSNData.Rows)
            {
                busPerson lbusPerson = new busPerson() { icdoPerson = new cdoPerson() };
                lbusPerson= lbusPerson.LoadPersonBySsn(ldtRow["RECORD_DATA"].ToString());
                if (lbusPerson.IsNotNull())
                {
                    if (lbusPerson.icdoPerson.person_id > 0)
                    {
                        lbusPerson.LoadRetirementAccount();
                        if (lbusPerson.iclbRetirementAccount.IsNotNull() && lbusPerson.iclbRetirementAccount.Any(i => i.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled || i.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementRetired || i.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetimentSuspended))
                        {
                            busTFFRSSNOutFile lobjTFFRSSNOutFile = new busTFFRSSNOutFile();

                            if (!Convert.IsDBNull(ldtRow["RECORD_DATA"]))
                                lobjTFFRSSNOutFile.ssn = ldtRow["RECORD_DATA"].ToString();
                            iclbTFFRSSNOutFile.Add(lobjTFFRSSNOutFile);
                        }
                    }
                }
            }
        }
    }
}