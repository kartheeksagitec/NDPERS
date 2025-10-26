using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Linq.Expressions;
using Sagitec.ExceptionPub;
using NeoSpin.DataObjects;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busDailyPeopleSoftFileOut : busFileBaseOut
    {

        busBase lobjBase = new busBase();
        static string istrUserId = "PERSLink Batch";
        public Collection<busDailyPeopleSoftFile> iclbDailyPeopleSoft { get; set; }
        DataTable ldtPeopleSoftFlagUpdate { get; set; }


        public void LoadPeopleSoftMembers(DataTable ldtPeopleSoftMembers)
        {
            ldtPeopleSoftMembers = busBase.Select("cdoPersonAccount.fleDailyPeopleSoftPlanFileOut", new object[0] { });

            iclbDailyPeopleSoft = new Collection<busDailyPeopleSoftFile>();

            ldtPeopleSoftFlagUpdate = ldtPeopleSoftMembers;

            foreach (DataRow dr in ldtPeopleSoftMembers.Rows)
            {
                int lintOrgID = 0;
                busDailyPeopleSoftFile lobjDailyPeopleSoftFile = new busDailyPeopleSoftFile();

                //Employee ID
                if (!Convert.IsDBNull(dr["ssn"]))
                    lobjDailyPeopleSoftFile.employee_id = dr["ssn"].ToString();

                //Employee Record Number
                lobjDailyPeopleSoftFile.employee_record_number = string.Empty;

                if (!Convert.IsDBNull(dr["employer_org_id"]))
                    lintOrgID = Convert.ToInt32(dr["employer_org_id"]);

                //Business Unit
                lobjDailyPeopleSoftFile.LoadOrganization(lintOrgID);
                lobjDailyPeopleSoftFile.business_unit = lobjDailyPeopleSoftFile.ibusOrganization.icdoOrganization.business_unit;

                //Plan Type
                if (!Convert.IsDBNull(dr["plan_type"]))
                    lobjDailyPeopleSoftFile.plan_type = dr["plan_type"].ToString();

                //Benefit Plan
                if (!Convert.IsDBNull(dr["benefit_plan"]))
                    lobjDailyPeopleSoftFile.benefit_plan = dr["benefit_plan"].ToString();

                //Coverage Begin Date
                if (!Convert.IsDBNull(dr["coverage_begin_date"]))
                    lobjDailyPeopleSoftFile.coverage_begin_date = Convert.ToDateTime(dr["coverage_begin_date"]);

                //Deduction Begin Date
                if (!Convert.IsDBNull(dr["deduction_begin_date"]))
                    lobjDailyPeopleSoftFile.deduction_begin_date = Convert.ToDateTime(dr["deduction_begin_date"]);

                //Coverage Election
                if (!Convert.IsDBNull(dr["coverage_election_value"]))
                    lobjDailyPeopleSoftFile.coverage_election = dr["coverage_election_value"].ToString();

                //Election Date
                if (!Convert.IsDBNull(dr["election_date"]))
                    lobjDailyPeopleSoftFile.election_date = Convert.ToDateTime(dr["election_date"]);

                //Coverage Code -- will need to calculate based on coverage code in the enrollment data table
                if (!Convert.IsDBNull(dr["coverage_code"]))
                    lobjDailyPeopleSoftFile.coverage_code = dr["coverage_code"].ToString();

                //Flat Amount / Annual Pledge
                if (!Convert.IsDBNull(dr["pretax_amount"]))
                    lobjDailyPeopleSoftFile.flat_amount = Convert.ToDecimal(dr["pretax_amount"]);

                // Direct Deposit - Only for Flex Comp
                if (!Convert.IsDBNull(dr["direct_deposit_flag"]))
                    lobjDailyPeopleSoftFile.direct_deposit = dr["direct_deposit_flag"].ToString();

                //Inside Mail - Only for Flex Comp
                if (!Convert.IsDBNull(dr["inside_mail_flag"]))
                    lobjDailyPeopleSoftFile.inside_mail = dr["inside_mail_flag"].ToString();


                //Company
                lobjDailyPeopleSoftFile.company = lobjDailyPeopleSoftFile.ibusOrganization.icdoOrganization.company_for_people_soft_file;

                //Calculation Routine

                //Person ID
                if (!Convert.IsDBNull(dr["ndpers_member_id"]))
                    lobjDailyPeopleSoftFile.person_id = Convert.ToInt32(dr["ndpers_member_id"]);


                iclbDailyPeopleSoft.Add(lobjDailyPeopleSoftFile);
            }
        }

        public override void InitializeFile()
        {
             istrFileName = "Daily_PeopleSoft_File_Out.txt";
        }

        public override void FinalizeFile()
        {
            DBFunction.StoreProcessLog(100, "Create Outbound File", "INFO", "Started finalize file", istrUserId, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            try
            {
                if (iclbDailyPeopleSoft.Count > 0)
                {
                    foreach (DataRow dr in ldtPeopleSoftFlagUpdate.Rows)
                    {
                        busEnrollmentData lobjEnrollmentData = new busEnrollmentData();
                        lobjEnrollmentData.icdoEnrollmentData = new doEnrollmentData();
                        lobjEnrollmentData.icdoEnrollmentData.LoadData(dr);
                        lobjEnrollmentData.icdoEnrollmentData.peoplesoft_file_sent_flag = busConstant.Flag_Yes;
                        lobjEnrollmentData.icdoEnrollmentData.Update();
                    }
                }
            }
            catch (Exception e)
            {
                DBFunction.StoreProcessLog(100, "An error occured while updating record." + "Error Message : " + e, "ERR", "Error in finalize file", istrUserId, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            }
        }

    }
}
