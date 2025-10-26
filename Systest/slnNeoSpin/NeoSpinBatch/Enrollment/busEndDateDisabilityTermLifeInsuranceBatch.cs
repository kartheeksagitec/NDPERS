#region Using directives
using System;
using System.Data;
using System.Collections.Generic;
using System.Diagnostics;
using NeoSpin.CustomDataObjects;
using NeoSpin.BusinessObjects;
using System.Collections.ObjectModel;
using Sagitec.DataObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.CustomDataObjects;
using Sagitec.BusinessObjects;
using System.Collections;
using Sagitec.CorBuilder;

#endregion

namespace NeoSpinBatch
{
    class busEndDateDisabilityTermLifeInsuranceBatch : busNeoSpinBatch
    {
        DataTable idtResultTable = new DataTable();

        private Collection<busPersonAccountLife> _iclbPersonAccountLife;
        public Collection<busPersonAccountLife> iclbPersonAccountLife
        {
            get { return _iclbPersonAccountLife; }
            set { _iclbPersonAccountLife = value; }
        }

        public void CreateCorrespondenceForEndDateDisability()
        {
            istrProcessName = "End Date Disability Term Life Insurance Batch";
            DateTime ldtProjectedEffectiveDate = new DateTime();
            DateTime ldtEmploymentEndDate = new DateTime();
            DateTime ldtIBSEndDate = new DateTime();
            _iclbPersonAccountLife = new Collection<busPersonAccountLife>();
            /// Get all records where DISABILITY LETTER SENT FLAG <> 'Y' and 
            ///                       PROJECTED PREMIUM WAIVER DATE IS NOT NULL and
            ///                       ACTUAL PREMIUM WAIVER DATE IS NULL
            idlgUpdateProcessLog("Creating correspondence for End Date Disability Term Life Insurance Batch", "INFO", istrProcessName);
            DataTable ldtbEnddateDisability = busBase.Select("cdoPersonAccountLife.EndDateDisabilityTermLifeInsuranceBatch",new object[] { });
            foreach (DataRow dr in ldtbEnddateDisability.Rows)
            {
                busPersonAccountLife lobjGroupLife = new busPersonAccountLife();
                lobjGroupLife.icdoPersonAccount = new cdoPersonAccount();
                lobjGroupLife.icdoPersonAccountLife = new cdoPersonAccountLife();
                lobjGroupLife.icdoPersonAccountLife.LoadData(dr);
                lobjGroupLife.icdoPersonAccount.LoadData(dr);
                lobjGroupLife.ibusPerson = new busPerson();
                lobjGroupLife.ibusPerson.FindPerson(lobjGroupLife.icdoPersonAccount.person_id);
                /// If the Projected Premium Waiver Date is around 2 months of Batch Run Date
                ldtProjectedEffectiveDate= lobjGroupLife.icdoPersonAccountLife.projected_premium_waiver_date.AddMonths(-2);
                /// Load Employment End Date
                if (lobjGroupLife.icdoPersonAccount.person_employment_dtl_id != 0)
                {
                    lobjGroupLife.LoadPersonEmploymentDetail();
                    if (lobjGroupLife.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date != DateTime.MinValue)
                    {
                        int lintYear=lobjGroupLife.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date.AddMonths(2).Year;
                        int lintMonth=lobjGroupLife.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date.AddMonths(2).Month;
                        int lintFirstDay=DateTime.DaysInMonth(lintYear,lintMonth);
                        ldtEmploymentEndDate = new DateTime(lintYear,lintMonth,lintFirstDay);
                    }
                }
                /// Load IBS End Date
                lobjGroupLife.LoadPaymentElection();
                if (lobjGroupLife.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date != DateTime.MinValue)
                    ldtIBSEndDate = lobjGroupLife.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date;
                /// Get Greater of Employment and IBS EndDate
                if (ldtIBSEndDate > ldtEmploymentEndDate)
                    ldtEmploymentEndDate = ldtIBSEndDate;
                
                if ((ldtEmploymentEndDate.AddMonths(10) >= iobjSystemManagement.icdoSystemManagement.batch_date) ||
                    (ldtProjectedEffectiveDate <= iobjSystemManagement.icdoSystemManagement.batch_date))
                {
                    lobjGroupLife.lstrProjectedPremiumEffectiveDate = 
                                lobjGroupLife.icdoPersonAccountLife.projected_premium_waiver_date.AddMonths(12).ToString("dd MMM yyyy");

                    //ArrayList larrList = new ArrayList();
                    //larrList.Add(lobjGroupLife);

                    Hashtable lshtTemp = new Hashtable();
                    lshtTemp.Add("FormTable", "Batch");

                    idlgUpdateProcessLog("Correspondence Creating for "+lobjGroupLife.ibusPerson.icdoPerson.FullName, "INFO", istrProcessName);
                    string lstrFileName=CreateCorrespondence("PER-0154", lobjGroupLife, lshtTemp);
                    idlgUpdateProcessLog("Updating the Disability Sent Flag", "INFO", istrProcessName);
                    UpdateDisabilitySentFlag(lobjGroupLife.icdoPersonAccount.person_account_id);
                    CreateContactTicket(lobjGroupLife.ibusPerson.icdoPerson.person_id);                    
                    _iclbPersonAccountLife.Add(lobjGroupLife);
                }
            }           
            idlgUpdateProcessLog("Correspondence Created Successfully", "INFO", istrProcessName);
            idlgUpdateProcessLog("Generating End Date Disability Term Life Insurance Report", "INFO", istrProcessName);
            GenerateReport(_iclbPersonAccountLife);
        }

        public void UpdateDisabilitySentFlag(int AintPersonAccountID)
        {
            busPersonAccountLife lobjGroupLife = new busPersonAccountLife();
            if (lobjGroupLife.FindPersonAccountLife(AintPersonAccountID))
            {
                lobjGroupLife.icdoPersonAccountLife.disability_letter_sent_flag = busConstant.Flag_Yes;
                lobjGroupLife.icdoPersonAccountLife.Update();
            }
        }

        // Create Contact Ticket
        private void CreateContactTicket(int aintPersonID)
        {
            cdoContactTicket lobjContactTicket = new cdoContactTicket();
            CreateContactTicket(aintPersonID, busConstant.ContactTicketTypeInsuranceRetiree, lobjContactTicket);          
        }        

        public void GenerateReport(Collection<busPersonAccountLife> aclbPersonAccounLife)
        {
            idtResultTable = CreateNewDataTable();

            foreach (busPersonAccountLife lobjPersonAccountLife in aclbPersonAccounLife)
            {
                AddToNewDataRow(lobjPersonAccountLife);
            }

            if (idtResultTable.Rows.Count > 0)
            {                
                CreateReport("rptEndDateDisabilityTermLifeReport.rpt", idtResultTable);

                idlgUpdateProcessLog("End Date Disability Term Life Insurance Report generated succesfully", "INFO", istrProcessName);
            }
            else
            {
                idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
            }
        }   

        public DataTable CreateNewDataTable()
        {
            DataTable ldtbReportTable = new DataTable();
            DataColumn ldc1 = new DataColumn("FullName", Type.GetType("System.String"));           
            DataColumn ldc3 = new DataColumn("LastName", Type.GetType("System.String"));
            DataColumn ldc4 = new DataColumn("PERSLinkID", Type.GetType("System.Int32"));
            DataColumn ldc5 = new DataColumn("ProjectedEffectiveDate", Type.GetType("System.DateTime"));          
            ldtbReportTable.Columns.Add(ldc1);           
            ldtbReportTable.Columns.Add(ldc3);
            ldtbReportTable.Columns.Add(ldc4);
            ldtbReportTable.Columns.Add(ldc5);         
            ldtbReportTable.TableName = busConstant.ReportTableName;
            return ldtbReportTable;
        }

        public void AddToNewDataRow(busPersonAccountLife aobjPersonAccountLife)
        {
            if (aobjPersonAccountLife.ibusPerson == null)
                aobjPersonAccountLife.LoadPerson();
            DataRow dr = idtResultTable.NewRow();
            dr["FullName"] = aobjPersonAccountLife.ibusPerson.icdoPerson.FullName;           
            dr["LastName"] = aobjPersonAccountLife.ibusPerson.icdoPerson.last_name;
            dr["PERSLinkID"] = aobjPersonAccountLife.ibusPerson.icdoPerson.person_id;
            dr["ProjectedEffectiveDate"] = aobjPersonAccountLife.icdoPersonAccountLife.projected_premium_waiver_date.AddMonths(12);
            idtResultTable.Rows.Add(dr);
        }
    }
}
