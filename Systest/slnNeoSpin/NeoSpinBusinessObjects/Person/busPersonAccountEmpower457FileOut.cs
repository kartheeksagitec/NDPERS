using System;
using System.Collections.Generic;
using System.Text;
using NeoSpin.BusinessObjects;
using Sagitec.BusinessObjects;
using System.Collections;
using System.Collections.ObjectModel;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.CorBuilder;
using Sagitec.DataObjects;
using System.Data;
using Sagitec.CustomDataObjects;
using NeoSpin.CustomDataObjects;
using System.Linq;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busPersonAccountEmpower457FileOut : busFileBaseOut
    {
        private Collection<busPersonAccountDeferredComp> _iclb457Members;
        public Collection<busPersonAccountDeferredComp> iclb457Members
        {
            get { return _iclb457Members; }
            set { _iclb457Members = value; }
        }
        public DateTime ldteNextRunDate { get; set; }
        public string lstrTIAAFlag { get; set; }
        public void Load457Members(DataTable ldtb457Members)
        {
            // The same query is used in the update query too.
            ldtb457Members = busBase.Select("cdoPersonAccountDeferredComp.fleFidelity457EnrollmentOut",
                                                                                    new object[0] { });
            iclb457Members = new Collection<busPersonAccountDeferredComp>();
            foreach (DataRow dr in ldtb457Members.Rows)
            {
                busPersonAccountDeferredComp lobjPersonAccountDeferredComp = new busPersonAccountDeferredComp
                {
                    icdoPersonAccount = new cdoPersonAccount(),
                    icdoPersonAccountDeferredComp = new cdoPersonAccountDeferredComp(),
                    ibusPerson = new busPerson { icdoPerson = new cdoPerson() },
                    ibusPersonEmploymentDetail = new busPersonEmploymentDetail
                    {
                        icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail(),
                        ibusPersonEmployment = new busPersonEmployment { icdoPersonEmployment = new cdoPersonEmployment() }
                    }
                };

                lobjPersonAccountDeferredComp.icdoPersonAccount.LoadData(dr);
                lobjPersonAccountDeferredComp.icdoPersonAccountDeferredComp.LoadData(dr);
                if (!Convert.IsDBNull(dr["UPDATE_SEQ_FOR_DEFE_COMP"]))
                    lobjPersonAccountDeferredComp.icdoPersonAccountDeferredComp.update_seq = Convert.ToInt32(dr["UPDATE_SEQ_FOR_DEFE_COMP"]);
                lobjPersonAccountDeferredComp.ibusPerson.icdoPerson.LoadData(dr);

                lobjPersonAccountDeferredComp.ibusPerson.LoadPersonCurrentAddress();
                lobjPersonAccountDeferredComp.LoadAllPersonEmploymentDetails(false);

                foreach (busPersonEmploymentDetail lobjEmpDtl in lobjPersonAccountDeferredComp.iclbEmploymentDetail)
                    lobjEmpDtl.LoadPersonEmployment();

                if (lobjPersonAccountDeferredComp.iclbEmploymentDetail.Count > 0)
                {
                    //Set the Start Date as Least Employment Start Date
                    int lintLastEntry = lobjPersonAccountDeferredComp.iclbEmploymentDetail.Count - 1;
                    lobjPersonAccountDeferredComp.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.start_date =
                            lobjPersonAccountDeferredComp.iclbEmploymentDetail[lintLastEntry].ibusPersonEmployment.icdoPersonEmployment.start_date;

                    //Set the End Date as Top Employment End Date
                    lobjPersonAccountDeferredComp.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.end_date =
                            lobjPersonAccountDeferredComp.iclbEmploymentDetail[0].ibusPersonEmployment.icdoPersonEmployment.end_date;
                }

                // PIR ID 1603
                // Status Code is T when all employment records associated with the Def comp plan are end dated.
                if (lobjPersonAccountDeferredComp.iclbEmploymentDetail.Where(row => row.ibusPersonEmployment.icdoPersonEmployment.end_date == DateTime.MinValue).Any())
                {
                    string lstrMutualFundFlag = Convert.ToString(dr["MUTUAL_FUND_WINDOW_FLAG"]);
                    if (lstrMutualFundFlag == busConstant.Flag_Yes)
                        lobjPersonAccountDeferredComp.icdoPersonAccount.status_code = "M";
                    else
                        lobjPersonAccountDeferredComp.icdoPersonAccount.status_code = "A";
                }
                else
                {
                    if (lobjPersonAccountDeferredComp.ibusPerson.icolPersonAccount.IsNull()) lobjPersonAccountDeferredComp.ibusPerson.LoadPersonAccount(false);
                    if (lobjPersonAccountDeferredComp.ibusPerson.icolPersonAccount.Where(lobj => lobj.icdoPersonAccount.plan_participation_status_value.Equals(busConstant.PlanParticipationStatusRetirementRetired)).Any())
                        lobjPersonAccountDeferredComp.icdoPersonAccount.status_code = "R";
                    else
                        lobjPersonAccountDeferredComp.icdoPersonAccount.status_code = "T";

                    lobjPersonAccountDeferredComp.icdoPersonAccountDeferredComp.file_457_sent_flag = busConstant.Flag_Yes;
                    lobjPersonAccountDeferredComp.icdoPersonAccountDeferredComp.Update();
                }
                iclb457Members.Add(lobjPersonAccountDeferredComp);
            }
        }

        public void Load457MembersForEmpower(DataTable ldtb457Members)
        {
            // The same query is used in the update query too.
            
            iclb457Members = new Collection<busPersonAccountDeferredComp>();
            iclb457Members = (Collection<busPersonAccountDeferredComp>)iarrParameters[0];
            lstrTIAAFlag = iarrParameters[1].ToString();
            ldteNextRunDate = Convert.ToDateTime(iarrParameters[2]);
            
        }

        public override void InitializeFile()
        {
            if (lstrTIAAFlag == busConstant.Flag_Yes && iclb457Members != null && iclb457Members.Count > 0)
                istrFileName = "100455-01_Demo." + ldteNextRunDate.ToString(busConstant.DateFormatD8) + busConstant.FileFormattxt;
        }
        public override void FinalizeFile()
        {
            base.FinalizeFile();
            //PROD PIR : 4663 , Now client wants the full file every time except Terminated / Retired Status to be sent once. so, Mass update not needed.
            /*
            // SYSTEST PIR ID 1403
            // Perfomance issue results in mass update of flag
            DBFunction.DBNonQuery("cdoPersonAccountDeferredComp.UpdateFidelity457FileSentFlag", new object[] { }, iobjPassInfo.iconFramework,
                            iobjPassInfo.itrnFramework);*/

        }
    }
}
