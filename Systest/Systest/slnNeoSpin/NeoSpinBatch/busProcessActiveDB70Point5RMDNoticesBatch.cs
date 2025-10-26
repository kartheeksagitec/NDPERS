using System;
using System.Data;
using NeoSpin.BusinessObjects;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;
using System.Collections;


namespace NeoSpinBatch
{
    public class busProcessActiveDB70Point5RMDNoticesBatch : busNeoSpinBatch
    {
        public void ProcessActiveDB70Point5RMDNotices()
        {
            istrProcessName = "Process Active 73 RMD Notices - Monthly Batch";
            DateTime ldtBatchDate = iobjSystemManagement.icdoSystemManagement.batch_date;
            DateTime ldtBatchEndDate = ldtBatchDate.AddMonths(1).AddDays(-1);
            // Records Fetched as per the rule: BR-093-07
            DataTable ldtbResult = busBase.Select("cdoBenefitApplication.ActiveDBMember70Point5RMDBatch", new object[1] { ldtBatchEndDate });
            if (ldtbResult.Rows.Count > 0)
            {
                idlgUpdateProcessLog("Processing Fetched Records", "INFO", istrProcessName);
                foreach (DataRow dr in ldtbResult.Rows)
                {
                    busPersonAccountEmploymentDetail lobjPersonAccountEmploymentDetail = new busPersonAccountEmploymentDetail
                    {
                        icdoPersonAccountEmploymentDetail = new cdoPersonAccountEmploymentDetail()
                    };
                    lobjPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.LoadData(dr);

                    busPerson lobjMember = new busPerson
                    {
                        icdoPerson = new cdoPerson(),
                        ibusCurrentEmployment = new busPersonEmployment
                            {
                                icdoPersonEmployment = new cdoPersonEmployment(),
                                ibusOrganization = new busOrganization { icdoOrganization = new cdoOrganization() },
                            }
                    };
                    lobjMember.icdoPerson.LoadData(dr);

                    idlgUpdateProcessLog("Processing Person id = " + lobjMember.icdoPerson.person_id, "INFO", istrProcessName);

                    lobjMember.ibusCurrentEmployment.ibusOrganization.icdoOrganization.LoadData(dr);

                    // Generating Correspondences 
                    //ArrayList larrlist = new ArrayList();
                    //larrlist.Add(lobjMember);
                    Hashtable lhstDummyTable = new Hashtable();
                    lhstDummyTable.Add("sfwCallingForm", "Batch");
                    CreateCorrespondence("ENR-5600", lobjMember, lhstDummyTable);
                    lobjPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.db_batch_letter_sent_flag = busConstant.Flag_Yes;
                    lobjPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.Update();                    
                }
            }
            else
                idlgUpdateProcessLog("No Records Fetched", "INFO", istrProcessName);
        }
    }
}
