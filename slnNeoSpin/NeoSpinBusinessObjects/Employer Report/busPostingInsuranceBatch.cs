#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using Sagitec.Common;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;
using System.Linq;
using System.IO;
using Sagitec.DBUtility;
using System.Threading.Tasks;

#endregion

namespace NeoSpin.BusinessObjects
{
    public class busPostingInsuranceBatch : busExtendBase
    {
        private busEmployerPayrollHeader _ibusEmployerPayrollHeader;
        public busEmployerPayrollHeader ibusEmployerPayrollHeader
        {
            get { return _ibusEmployerPayrollHeader; }
            set { _ibusEmployerPayrollHeader = value; }
        }

        public busSystemManagement ibusSystemManagement { get; set; }

        public DateTime PayrollPaidDate
        {
            get
            {
                /*PIR 14493 When reloading a Insurance header from screen, the system must load active members of an organization by payroll_paid_date,
                 * not as of system management batch date*/
                if (ibusEmployerPayrollHeader.IsNotNull() && ibusEmployerPayrollHeader.iblnIsReloadFromScreen)
                {
                    return ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.payroll_paid_date;
                }
                else
                {
                    if (ibusSystemManagement == null)
                    {
                        ibusSystemManagement = new busSystemManagement();
                        ibusSystemManagement.FindSystemManagement();
                    }
                }
                return new DateTime(ibusSystemManagement.icdoSystemManagement.batch_date.Year, ibusSystemManagement.icdoSystemManagement.batch_date.Month, 1);

            }
        }

        public Collection<busOrgPlan> iclbProviderOrgPlan { get; set; }

        public List<busOrgPlan> iclbOrgPlan { get; set; }

        public busDBCacheData ibusDBCacheData { get; set; }

        public Collection<busPersonAccount> iclbActiveInsuranceMembers { get; set; }
        public Collection<busPersonAccount> iclbRGroupPersonAccount { get; set; }

        public Collection<busPersonAccountGhdv> iclbPersonAccountGhdv { get; set; }
        public Collection<busPersonAccountLife> iclbPersonAccountLife { get; set; }

        public Collection<busPersonAccountGhdv> iclbRGroupPersonAccountGhdv { get; set; }
        public Collection<busPersonAccountLife> iclbRGroupPersonAccountLife { get; set; }
		//PIR 15434
        public Collection<busPersonAccountMedicarePartDHistory> iclbRGroupPersonAccountMedicare { get; set; }

        public DataTable idtbOrgPlanProviders { get; set; }
        //public DataTable idtbPALifeOptionHistory { get; set; }
        //public DataTable idtbEAPHistory { get; set; }
        //public DataTable idtbGHDVHistory { get; set; }
        //public DataTable idtbLifeHistory { get; set; }

        //dka
        public Collection<cdoPersonAccountLifeHistory> iclbPALifeOptionHistory { get; set; }
        public Collection<cdoPersonAccountEapHistory> iclbEAPHistory { get; set; }
        public Collection<cdoPersonAccountGhdvHistory> iclbGHDVHistory { get; set; }
        public Collection<cdoPersonAccountLifeHistory> iclbLifeHistory { get; set; }
        //dka

        //prod pir 933
        //dka
        public Collection<busPersonAccountDependent> iclbPersonAccountDependent { get; set;}
        //public DataTable idtPersonAccountDependent { get; set; }
        /// <summary>
        /// method to load person account dependent
        /// </summary>
        public void LoadPersonAccountDepenedents()
        {
            //dka
            //idtPersonAccountDependent = new DataTable();
            bool lblnLoadOldValues = utlPassInfo.iobjPassInfo.iblnLoadOldValues;
            try
            {
                utlPassInfo.iobjPassInfo.iblnLoadOldValues = false;
                iclbPersonAccountDependent = new Collection<busPersonAccountDependent>();
                using (var ldtbData = busBase.Select("cdoPersonAccountDependent.LoadDependentForBillingInsurance", new object[1] { PayrollPaidDate }))
                {
                    foreach (DataRow ldtrData in ldtbData.Rows)
                    {
                        busPersonAccountDependent lobjBus = new busPersonAccountDependent { icdoPersonAccountDependent = new cdoPersonAccountDependent() };
                        lobjBus.icdoPersonAccountDependent.LoadData(ldtrData);
                    }
                }
            }
            finally
            {
                utlPassInfo.iobjPassInfo.iblnLoadOldValues = lblnLoadOldValues;
            }
        }

        /// <summary>
        /// Method to Post the Insurance Payroll Header and Detail
        /// </summary>        
        public void PostInsurancePayroll(busOrganization abusOrganization)
        {
            CreatePayrollHeader(abusOrganization);
            CreatePayrollDetailCollecton(abusOrganization, true);

            if (_ibusEmployerPayrollHeader.iclbEmployerPayrollDetail.Count > 0)
            {
                _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.Insert();

                ValidatePayrollDetail();
            }
        }

        public string LoadDataForOrg(int aintOrgId)
        {
            ///RA 
            ///
            bool lblnOldLoadValues = utlPassInfo.iobjPassInfo.iblnLoadOldValues;
            utlPassInfo.iobjPassInfo.iblnLoadOldValues = false;
            try
            {
                string lstrResult = "";
                DateTime ldtPayrollPaidDate = PayrollPaidDate;

                utlPassInfo lobjOrigPassInfo = utlPassInfo.iobjPassInfo;
                //dka
                using (var ldtbPersonData = busNeoSpinBase.Select("cdoEmployerPayrollHeader.LoadLifeOptionWithOrg", new object[2] { ldtPayrollPaidDate, aintOrgId }))
                {
                    iclbPALifeOptionHistory = new Collection<cdoPersonAccountLifeHistory>();
                    cdoPersonAccountLifeHistory lobCDO = null;
                    foreach (DataRow dRow in ldtbPersonData.Rows)
                    {
                        utlPassInfo.iobjPassInfo = lobjOrigPassInfo;
                        lobCDO = new cdoPersonAccountLifeHistory();
                        lobCDO.LoadData(dRow);
                        iclbPALifeOptionHistory.Add(lobCDO);
                    };
                }

                using (var ldtbPersonData = busNeoSpinBase.Select("cdoEmployerPayrollHeader.LoadEAPHistoryWithOrg", new object[2] { ldtPayrollPaidDate, aintOrgId }))
                {
                    iclbEAPHistory = new Collection<cdoPersonAccountEapHistory>();
                    cdoPersonAccountEapHistory lobCDO = null;
                    foreach (DataRow dRow in ldtbPersonData.Rows)
                    {
                        utlPassInfo.iobjPassInfo = lobjOrigPassInfo;
                        lobCDO = new cdoPersonAccountEapHistory();
                        lobCDO.LoadData(dRow);
                        iclbEAPHistory.Add(lobCDO);
                    };
                }

                //Loading the GHDV History (Optimization)
                using (var ldtbPersonData = busNeoSpinBase.Select("cdoEmployerPayrollHeader.LoadGHDVHistoryWithOrg", new object[2] { ldtPayrollPaidDate, aintOrgId }))
                {
                    iclbGHDVHistory = new Collection<cdoPersonAccountGhdvHistory>();
                    cdoPersonAccountGhdvHistory lobCDO = null;
                    foreach (DataRow dRow in ldtbPersonData.Rows)
                    {
                        utlPassInfo.iobjPassInfo = lobjOrigPassInfo;
                        lobCDO = new cdoPersonAccountGhdvHistory();
                        lobCDO.LoadData(dRow);
                        iclbGHDVHistory.Add(lobCDO);
                    };
                }

                //Loading the Life History (Optimization)
                using (var ldtbPersonData = busNeoSpinBase.Select("cdoEmployerPayrollHeader.LoadLifeHistoryWithOrg", new object[2] { ldtPayrollPaidDate, aintOrgId }))
                {
                    iclbLifeHistory = new Collection<cdoPersonAccountLifeHistory>();
                    cdoPersonAccountLifeHistory lobCDO = null;
                    foreach (DataRow dRow in ldtbPersonData.Rows)
                    {
                        utlPassInfo.iobjPassInfo = lobjOrigPassInfo;
                        lobCDO = new cdoPersonAccountLifeHistory();
                        lobCDO.LoadData(dRow);
                        iclbLifeHistory.Add(lobCDO);
                    };
                }

                lstrResult = " Loaded Data LifeOption History : " + iclbPALifeOptionHistory.Count.ToString() +
                                         " EAPH History : " + iclbEAPHistory.Count.ToString() +
                                         " GHDV History : " + iclbGHDVHistory.Count.ToString() +
                                         " Life History : " + iclbLifeHistory.Count.ToString();

                return lstrResult;
            }
            finally
            {
                utlPassInfo.iobjPassInfo.iblnLoadOldValues = lblnOldLoadValues;
            }
        }

        private void CreatePayrollHeader(busOrganization abusOrganization)
        {
            _ibusEmployerPayrollHeader = new busEmployerPayrollHeader();
            _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader();
            _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.org_id = abusOrganization.icdoOrganization.org_id;
            _ibusEmployerPayrollHeader.ibusOrganization = abusOrganization;
            _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value = busConstant.PayrollHeaderBenefitTypeInsr;
            _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.report_type_value = busConstant.PayrollHeaderReportTypeRegular;
            _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.status_value = busConstant.PayrollHeaderStatusReview;
            _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.payroll_paid_date = PayrollPaidDate;
            _ibusEmployerPayrollHeader.istrOrgCodeId = abusOrganization.icdoOrganization.org_code;
            _ibusEmployerPayrollHeader.iblnValidateDetail = true;
            _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.reporting_source_value = busConstant.PayrollHeaderReportingSourceInsuranceBatch;
            _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.balancing_status_value = busConstant.PayrollHeaderBalancingStatusNoRemittance;
            _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.interest_waiver_flag = busConstant.Flag_No;
            _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.received_date = DateTime.Now;
            _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.submitted_date = DateTime.Now;
        }

        public void CreatePayrollDetailCollecton(busOrganization abusOrganization, bool ablnSkipValidationFromBatch)
        {
            _ibusEmployerPayrollHeader.iclbEmployerPayrollDetail = new Collection<busEmployerPayrollDetail>();

            //Load all the Active Insurance Members By Org
            LoadActiveInsuranceMembersByOrg(abusOrganization);

            foreach (busPersonAccount lbusPersonAccount in iclbActiveInsuranceMembers)
            {
                //Populate Org Plan Object By Plan Id (Optimization Purpose)
                if (iclbOrgPlan != null)
                {
                    lbusPersonAccount.ibusOrgPlan =
                        iclbOrgPlan.Where(o => o.icdoOrgPlan.plan_id == lbusPersonAccount.icdoPersonAccount.plan_id).FirstOrDefault();
                }

                busEmployerPayrollDetail lobjPayrollDetail = CreateEmployerPayrollDetail(lbusPersonAccount, false, ablnSkipValidationFromBatch);
                if (lobjPayrollDetail != null)
                    _ibusEmployerPayrollHeader.iclbEmployerPayrollDetail.Add(lobjPayrollDetail);
            }

            //if (iclbRGroupPersonAccount == null)
            //Commented the above code as R Group members need to be loaded always
            //for each organization this method will get called
            LoadRGroupMembers(abusOrganization);

            //Add the Rgroup Members If Exists
            foreach (busPersonAccount lobjPersonAccount in iclbRGroupPersonAccount)
            {
                if (lobjPersonAccount.ibusPerson == null)
                    lobjPersonAccount.LoadPerson();

                if (lobjPersonAccount.ibusPlan == null)
                    lobjPersonAccount.LoadPlan();

                busEmployerPayrollDetail lobjPayrollDetail = CreateEmployerPayrollDetail(lobjPersonAccount, true, ablnSkipValidationFromBatch);
                if (lobjPayrollDetail != null)
                    _ibusEmployerPayrollHeader.iclbEmployerPayrollDetail.Add(lobjPayrollDetail);
            }
        }

        public void ValidatePayrollDetail()
        {
            //Add it into iarrChangeLog for the Validations to Happen            
            _ibusEmployerPayrollHeader.iarrChangeLog.Add(_ibusEmployerPayrollHeader.icdoEmployerPayrollHeader);

            //Add the Detail Collection to iArrChangeLog (This calls the UpdateDataObject Method and that will Insert the Detail Records)
            foreach (busEmployerPayrollDetail lobjEmployerPayrollDetail in _ibusEmployerPayrollHeader.iclbEmployerPayrollDetail)
            {
                //Assign the Employer Payroll Header Id
                lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.employer_payroll_header_id =
                    _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.employer_payroll_header_id;
                _ibusEmployerPayrollHeader.iarrChangeLog.Add(lobjEmployerPayrollDetail.icdoEmployerPayrollDetail);
            }

            //Calling the Save Operation of Header that will do the Payroll Validation
            _ibusEmployerPayrollHeader.iblnValidateDetail = true;

            long lngMemoryused = GC.GetTotalMemory(false) / 1000000;
            string lstrLogMessage = "BeforePersistChanges started " + DateTime.Now.ToString() + " Memeory used : " + lngMemoryused.ToString();
            WriteInfLog(lstrLogMessage);

            _ibusEmployerPayrollHeader.BeforePersistChanges();

            lngMemoryused = GC.GetTotalMemory(false) / 1000000;
            lstrLogMessage = "BeforePersistChanges ended " + DateTime.Now.ToString() + " Memeory used : " + lngMemoryused.ToString();
            WriteInfLog(lstrLogMessage);


            lngMemoryused = GC.GetTotalMemory(false) / 1000000;
            lstrLogMessage = "PersistChanges started " + DateTime.Now.ToString() + " Memeory used : " + lngMemoryused.ToString();
            WriteInfLog(lstrLogMessage);

            _ibusEmployerPayrollHeader.PersistChanges();

            lngMemoryused = GC.GetTotalMemory(false) / 1000000;
            lstrLogMessage = "PersistChanges ended " + DateTime.Now.ToString() + " Memeory used : " + lngMemoryused.ToString();
            WriteInfLog(lstrLogMessage);

            lngMemoryused = GC.GetTotalMemory(false) / 1000000;
            lstrLogMessage = "ValidateSoftErrors started " + DateTime.Now.ToString() + " Memeory used : " + lngMemoryused.ToString();
            WriteInfLog(lstrLogMessage);

            _ibusEmployerPayrollHeader.ValidateSoftErrors();
            
            lngMemoryused = GC.GetTotalMemory(false) / 1000000;
            lstrLogMessage = "ValidateSoftErrors ended " + DateTime.Now.ToString() + " Memeory used : " + lngMemoryused.ToString();
            WriteInfLog(lstrLogMessage);

            lngMemoryused = GC.GetTotalMemory(false) / 1000000;
            lstrLogMessage = "UpdateValidateStatus started " + DateTime.Now.ToString() + " Memeory used : " + lngMemoryused.ToString();
            WriteInfLog(lstrLogMessage);

            _ibusEmployerPayrollHeader.UpdateValidateStatus();

            lngMemoryused = GC.GetTotalMemory(false) / 1000000;
            lstrLogMessage = "UpdateValidateStatus ended " + DateTime.Now.ToString() + " Memeory used : " + lngMemoryused.ToString();
            WriteInfLog(lstrLogMessage);

            _ibusEmployerPayrollHeader.iblnSkipReloadAfterSave = true;

            lngMemoryused = GC.GetTotalMemory(false) / 1000000;
            lstrLogMessage = "AfterPersistChanges started " + DateTime.Now.ToString() + " Memeory used : " + lngMemoryused.ToString();
            WriteInfLog(lstrLogMessage);

            _ibusEmployerPayrollHeader.AfterPersistChanges();

            lngMemoryused = GC.GetTotalMemory(false) / 1000000;
            lstrLogMessage = "AfterPersistChanges ended " + DateTime.Now.ToString() + " Memeory used : " + lngMemoryused.ToString();
            WriteInfLog(lstrLogMessage);

            /*  Code Changes for performance- SH*/
            DataTable ldtCodeValue = iobjPassInfo.isrvDBCache.GetCodeValues(1922);

            //prod pir 933 : inserting person account dependent link table
            foreach (busEmployerPayrollDetail lobjEmployerPayrollDetail in _ibusEmployerPayrollHeader.iclbEmployerPayrollDetail)
            {
                if (lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.plan_id == busConstant.PlanIdGroupHealth && 
                    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.record_type_value != busConstant.PayrollDetailRecordTypeNegativeAdjustment)
                {
                    if (lobjEmployerPayrollDetail.ibusPersonAccountGhdv != null && lobjEmployerPayrollDetail.ibusPersonAccountGhdv.icdoPersonAccountGhdv != null &&
                        ldtCodeValue.AsEnumerable().Where(o=>o.Field<string>("data1") == lobjEmployerPayrollDetail.ibusPersonAccountGhdv.icdoPersonAccountGhdv.coverage_code).Any())
                    {
                        //dka
                        var lclbData = iclbPersonAccountDependent.Where(d=> d.icdoPersonAccountDependent.person_account_id == lobjEmployerPayrollDetail.ibusPersonAccountGhdv.icdoPersonAccountGhdv.person_account_id);
                        lobjEmployerPayrollDetail.InsertPersonAccountDependentBillingLink(lclbData);
                    }
                }
            }
        }

        private void WriteInfLog(string lstrInf)
        {
            string lstrLogFile = AppDomain.CurrentDomain.BaseDirectory;

            lstrLogFile += "\\InsuranceInbound" + DateTime.Today.ToString("yyyy-MM-dd") + ".txt";

            using (FileStream fs = File.Open(lstrLogFile,
            FileMode.Append, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine(lstrInf);
                }
            }
        }

        private busEmployerPayrollDetail CreateEmployerPayrollDetail(busPersonAccount abusPersonAccount, bool ablnIsRgroupRetiree, bool ablnSkipSoftErrorQueries)
        {

            //PIR 2028 : Skip to Payroll Detail if the Entry is already Exists (Reload Insurance Scenario)
            if (ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.employer_payroll_header_id > 0 && abusPersonAccount.icdoPersonAccount.plan_id != busConstant.PlanIdMedicarePartD)
            {
                Object lobjResult = DBFunction.DBExecuteScalar("cdoEmployerPayrollHeader.IsDetailEntryExistsByPersonPlanForHeader",
                    new object[3] 
                            { 
                            ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.employer_payroll_header_id, 

                            abusPersonAccount.icdoPersonAccount.person_id, 
                            abusPersonAccount.icdoPersonAccount.plan_id 
                            }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

                //busNeoSpinBase.Select("cdoEmployerPayrollHeader.IsDetailEntryExistsByPersonPlanForHeader",
                //                      new object[3]
                //                          {
                //                              ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.employer_payroll_header_id,
                //                              abusPersonAccount.icdoPersonAccount.person_id,
                //                              abusPersonAccount.icdoPersonAccount.plan_id
                //                          });


                if ((lobjResult != null) && (Convert.ToInt32(lobjResult) > 0))
                {
                    return null;
                }
            }
            //PIR 16617
            if (ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.employer_payroll_header_id > 0 && abusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMedicarePartD)
            {
                Object lobjResult = DBFunction.DBExecuteScalar("cdoEmployerPayrollHeader.IsDetailEntryExistsByPersonPlanForHeaderMedicare",
                    new object[3] 
                            { 
                            ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.employer_payroll_header_id, 

                            abusPersonAccount.icdoPersonAccount.person_account_id, 
                            abusPersonAccount.icdoPersonAccount.plan_id 
                            }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

                if ((lobjResult != null) && (Convert.ToInt32(lobjResult) > 0))
                {
                    return null;
                }
            }
			
			//PIR 15434
            int lintMemberPersonId = 0;
            string lstrSSN = string.Empty;
			//Org to bill
            decimal ldecLateEnrollmentPenalty = 0M;
            decimal ldecLowIncomeCredit = 0M;
            if (abusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMedicarePartD && iclbRGroupPersonAccountMedicare.IsNotNull())
            {
                //Org to bill
                busPersonAccountMedicarePartDHistory lbusPersonAccountMedicarePartDHistory = iclbRGroupPersonAccountMedicare.Where(i => i.icdoPersonAccountMedicarePartDHistory.person_account_id == abusPersonAccount.icdoPersonAccount.person_account_id).FirstOrDefault();
                if (lbusPersonAccountMedicarePartDHistory.IsNotNull())
                {
                    lintMemberPersonId = lbusPersonAccountMedicarePartDHistory.icdoPersonAccountMedicarePartDHistory.member_person_id;
                    busPerson lbusPerson = new busPerson();
                    lbusPerson.FindPerson(lintMemberPersonId);
                    lstrSSN = lbusPerson.icdoPerson.ssn;
                }
            }

            busEmployerPayrollDetail lobjPayrollDetail = new busEmployerPayrollDetail();
            lobjPayrollDetail.icdoEmployerPayrollDetail = new cdoEmployerPayrollDetail();
            lobjPayrollDetail.ibusEmployerPayrollHeader = _ibusEmployerPayrollHeader;
            lobjPayrollDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader = _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader;
            lobjPayrollDetail.icdoEmployerPayrollDetail.employer_payroll_header_id = _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.employer_payroll_header_id;
            lobjPayrollDetail.ibusPerson = abusPersonAccount.ibusPerson;
            lobjPayrollDetail.icdoEmployerPayrollDetail.person_id = abusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMedicarePartD ? lintMemberPersonId : abusPersonAccount.ibusPerson.icdoPerson.person_id;
            lobjPayrollDetail.icdoEmployerPayrollDetail.record_type_value = busConstant.PayrollDetailRecordTypeRegular;
            lobjPayrollDetail.icdoEmployerPayrollDetail.ssn = abusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMedicarePartD ? lstrSSN : abusPersonAccount.ibusPerson.icdoPerson.ssn;
            lobjPayrollDetail.icdoEmployerPayrollDetail.plan_id = abusPersonAccount.ibusPlan.icdoPlan.plan_id;
            lobjPayrollDetail.ibusPlan = abusPersonAccount.ibusPlan;
            lobjPayrollDetail.icdoEmployerPayrollDetail.first_name = abusPersonAccount.ibusPerson.icdoPerson.first_name;
            lobjPayrollDetail.icdoEmployerPayrollDetail.last_name = abusPersonAccount.ibusPerson.icdoPerson.last_name;
            lobjPayrollDetail.icdoEmployerPayrollDetail.pay_period_date = PayrollPaidDate;
            lobjPayrollDetail.icdoEmployerPayrollDetail.suppress_warnings_flag = busConstant.Flag_No;
            lobjPayrollDetail.icdoEmployerPayrollDetail.status_value = busConstant.PayrollDetailStatusReview;
            lobjPayrollDetail.ibusPersonAccount = abusPersonAccount;
            lobjPayrollDetail.ibusDBCacheData = ibusDBCacheData;
            if (ablnSkipSoftErrorQueries)
            {
                lobjPayrollDetail.iblnClearSoftErrors = false;
                lobjPayrollDetail.iblnLoadSoftErrors = false;
            }
            //PIR 7705
            decimal ldecHSAAmt = 0.00M;
            decimal ldecHSAVendorAmt = 0.00M;

            decimal ldecFeeAmt = 0.00M;
            decimal ldecRHICAmt = 0.00M;
            decimal ldecBuydownAmt = 0.00M;
            decimal ldecMedicarePartDAmt = 0.00M;
            /* UAT PIR 476, Including other and JS RHIC Amount */
            decimal ldecOthrRHICAmt = 0.00M;
            decimal ldecJSRHICAmt = 0.00M;
            /* UAT PIR 476 ends here */
            //uat pir 1429 : to post ghdv_history_id
            int lintGHDVHistoryID = 0;
            string lstrGroupNumber = string.Empty;
            //prod pir 6076
            string lstrCoverageCodeValue = string.Empty, lstrRateStructureCode = string.Empty;
            //uat pir 1344
            decimal ldecEmprSharePremium = 0.00M, ldecEmprShareFee = 0.00M, ldecEmprShareRHICAmt = 0.00M, ldecEmprShareOtherRHICAmt = 0.00M, ldecEmprShareJSRHICAmt = 0.00M;
            decimal ldecEmprShareBuydown = 0.00M;
            decimal ldecEmprShareMedicarePartDAmt = 0.00M;
            if (ibusEmployerPayrollHeader.ibusOrganization == null)
                ibusEmployerPayrollHeader.LoadOrganization();
            //Loading the GHDV Object (Optimization)            
            if (lobjPayrollDetail.ibusPlan.IsGHDVPlan())
            {
                if (ablnIsRgroupRetiree)
                {
                    if (iclbRGroupPersonAccountGhdv != null)
                    {
                        lobjPayrollDetail.ibusPersonAccountGhdv =
                            iclbRGroupPersonAccountGhdv.Where(
                                o => o.icdoPersonAccountGhdv.person_account_id == abusPersonAccount.icdoPersonAccount.person_account_id).FirstOrDefault();
                    }
                }
                else
                {
                    if (iclbPersonAccountGhdv != null)
                    {
                        lobjPayrollDetail.ibusPersonAccountGhdv =
                            iclbPersonAccountGhdv.Where(
                                o => o.icdoPersonAccountGhdv.person_account_id == abusPersonAccount.icdoPersonAccount.person_account_id).FirstOrDefault();
                    }
                }

                if (lobjPayrollDetail.ibusPersonAccountGhdv == null)
                {
                    lobjPayrollDetail.ibusPersonAccountGhdv = new busPersonAccountGhdv();
                    lobjPayrollDetail.ibusPersonAccountGhdv.FindGHDVByPersonAccountID(abusPersonAccount.icdoPersonAccount.person_account_id);
                }

                lobjPayrollDetail.ibusPersonAccountGhdv.icdoPersonAccount = abusPersonAccount.icdoPersonAccount;

                //Loading the History Object                
                //dka
                if ((iclbGHDVHistory != null) && (iclbGHDVHistory.Count > 0))
                {
                    var lclbData = iclbGHDVHistory.Where(d => d.person_account_ghdv_id == lobjPayrollDetail.ibusPersonAccountGhdv.icdoPersonAccountGhdv.person_account_ghdv_id);
                    //Dont initialize the collection as the LoadHistoryByDate will load based on Null Check
                    if (lclbData != null && lclbData.Count() > 0)
                    {
                        lobjPayrollDetail.ibusPersonAccountGhdv.iclbPersonAccountGHDVHistory = new Collection<busPersonAccountGhdvHistory>();
                        foreach (var lobjCDO in lclbData)
                        {
                            busPersonAccountGhdvHistory lobjBus = new busPersonAccountGhdvHistory();
                            lobjBus.icdoPersonAccountGhdvHistory = lobjCDO;
                            lobjPayrollDetail.ibusPersonAccountGhdv.iclbPersonAccountGHDVHistory.Add(lobjBus);
                        }
                    }
                }
                
                busPersonAccountGhdvHistory lbusHistory = lobjPayrollDetail.ibusPersonAccountGhdv.LoadHistoryByDate(PayrollPaidDate);
                lobjPayrollDetail.ibusPersonAccountGhdv = lbusHistory.LoadGHDVObject(lobjPayrollDetail.ibusPersonAccountGhdv);

                lobjPayrollDetail.ibusPersonAccountGhdv.ibusPerson = abusPersonAccount.ibusPerson;
                lobjPayrollDetail.ibusPersonAccountGhdv.ibusPlan = abusPersonAccount.ibusPlan;
                lobjPayrollDetail.ibusPersonAccountGhdv.ibusPaymentElection = abusPersonAccount.ibusPaymentElection;
                lobjPayrollDetail.ibusPersonAccountGhdv.ibusOrgPlan = abusPersonAccount.ibusOrgPlan;
                //We must load all the associated provider for the given employer org plan object.
                //IBS Loads the default one since we dont have employment over there.
                if ((idtbOrgPlanProviders != null) && (lobjPayrollDetail.ibusPersonAccountGhdv.ibusOrgPlan != null)
                    && (lobjPayrollDetail.ibusPersonAccountGhdv.ibusOrgPlan.icdoOrgPlan.org_plan_id > 0) && (iclbProviderOrgPlan != null))
                {
                    DataRow[] larrRow = idtbOrgPlanProviders.FilterTable(busConstant.DataType.Numeric,
                                                                   "org_plan_id",
                                                                   lobjPayrollDetail.ibusPersonAccountGhdv.ibusOrgPlan.icdoOrgPlan.org_plan_id);
                    if (larrRow != null && larrRow.Length > 0)
                    {
                        lobjPayrollDetail.ibusPersonAccountGhdv.ibusOrgPlan.iclbOrgPlanProvider =
                            GetCollection<busOrgPlanProvider>(larrRow, "icdoOrgPlanProvider");
                    }
                    //Null check required
                    if (lobjPayrollDetail.ibusPersonAccountGhdv.ibusOrgPlan.iclbOrgPlanProvider != null)
                    {
                        lobjPayrollDetail.ibusPersonAccountGhdv.ibusProviderOrgPlan =
                            LoadProviderOrgPlanByProvider(lobjPayrollDetail.ibusPersonAccountGhdv.ibusOrgPlan.iclbOrgPlanProvider,
                            lobjPayrollDetail.ibusPersonAccountGhdv.icdoPersonAccount.plan_id);
                    }
                }
            }
			
			//PIR 15434
            //Loading the Medicare Object (Optimization)            
            if (lobjPayrollDetail.icdoEmployerPayrollDetail.plan_id == busConstant.PlanIdMedicarePartD)
            {
                if (ablnIsRgroupRetiree)
                {
                    if (iclbRGroupPersonAccountMedicare != null)
                    {
                        lobjPayrollDetail.ibusPersonAccountMedicare =
                            iclbRGroupPersonAccountMedicare.Where(
                                o => o.icdoPersonAccountMedicarePartDHistory.person_account_id == abusPersonAccount.icdoPersonAccount.person_account_id).FirstOrDefault();
                    }
                }
                //else
                //{
                //    if (iclbPersonAccountme != null)
                //    {
                //        lobjPayrollDetail.ibusPersonAccountGhdv =
                //            iclbPersonAccountGhdv.Where(
                //                o => o.icdoPersonAccountGhdv.person_account_id == abusPersonAccount.icdoPersonAccount.person_account_id).FirstOrDefault();
                //    }
                //}

                if (lobjPayrollDetail.ibusPersonAccountMedicare == null)
                {
                    lobjPayrollDetail.ibusPersonAccountMedicare = new busPersonAccountMedicarePartDHistory();
                    lobjPayrollDetail.ibusPersonAccountMedicare.FindMedicareByPersonAccountID(abusPersonAccount.icdoPersonAccount.person_account_id);
                }

                lobjPayrollDetail.ibusPersonAccountMedicare.icdoPersonAccount = abusPersonAccount.icdoPersonAccount;

                //Loading the History Object                
                //dka
                //if ((iclbGHDVHistory != null) && (iclbGHDVHistory.Count > 0))
                //{
                //    var lclbData = iclbGHDVHistory.Where(d => d.person_account_ghdv_id == lobjPayrollDetail.ibusPersonAccountGhdv.icdoPersonAccountGhdv.person_account_ghdv_id);
                //    //Dont initialize the collection as the LoadHistoryByDate will load based on Null Check
                //    if (lclbData != null && lclbData.Count() > 0)
                //    {
                //        lobjPayrollDetail.ibusPersonAccountGhdv.iclbPersonAccountGHDVHistory = new Collection<busPersonAccountGhdvHistory>();
                //        foreach (var lobjCDO in lclbData)
                //        {
                //            busPersonAccountGhdvHistory lobjBus = new busPersonAccountGhdvHistory();
                //            lobjBus.icdoPersonAccountGhdvHistory = lobjCDO;
                //            lobjPayrollDetail.ibusPersonAccountGhdv.iclbPersonAccountGHDVHistory.Add(lobjBus);
                //        }
                //    }
                //}

                //busPersonAccountGhdvHistory lbusHistory = lobjPayrollDetail.ibusPersonAccountGhdv.LoadHistoryByDate(PayrollPaidDate);
                //lobjPayrollDetail.ibusPersonAccountGhdv = lbusHistory.LoadGHDVObject(lobjPayrollDetail.ibusPersonAccountGhdv);

                lobjPayrollDetail.ibusPersonAccountMedicare.ibusPerson = abusPersonAccount.ibusPerson;
                lobjPayrollDetail.ibusPersonAccountMedicare.ibusPlan = abusPersonAccount.ibusPlan;
                lobjPayrollDetail.ibusPersonAccountMedicare.ibusPaymentElection = abusPersonAccount.ibusPaymentElection;
                lobjPayrollDetail.ibusPersonAccountMedicare.ibusOrgPlan = abusPersonAccount.ibusOrgPlan;
                //We must load all the associated provider for the given employer org plan object.
                //IBS Loads the default one since we dont have employment over there.
                if ((idtbOrgPlanProviders != null) && (lobjPayrollDetail.ibusPersonAccountMedicare.ibusOrgPlan != null)
                    && (lobjPayrollDetail.ibusPersonAccountMedicare.ibusOrgPlan.icdoOrgPlan.org_plan_id > 0) && (iclbProviderOrgPlan != null))
                {
                    DataRow[] larrRow = idtbOrgPlanProviders.FilterTable(busConstant.DataType.Numeric,
                                                                   "org_plan_id",
                                                                   lobjPayrollDetail.ibusPersonAccountMedicare.ibusOrgPlan.icdoOrgPlan.org_plan_id);
                    if (larrRow != null && larrRow.Length > 0)
                    {
                        lobjPayrollDetail.ibusPersonAccountMedicare.ibusOrgPlan.iclbOrgPlanProvider =
                            GetCollection<busOrgPlanProvider>(larrRow, "icdoOrgPlanProvider");
                    }
                    //Null check required
                    if (lobjPayrollDetail.ibusPersonAccountMedicare.ibusOrgPlan.iclbOrgPlanProvider != null)
                    {
                        lobjPayrollDetail.ibusPersonAccountMedicare.ibusProviderOrgPlan =
                            LoadProviderOrgPlanByProvider(lobjPayrollDetail.ibusPersonAccountMedicare.ibusOrgPlan.iclbOrgPlanProvider,
                            lobjPayrollDetail.ibusPersonAccountMedicare.icdoPersonAccount.plan_id);
                    }
                }
                DataTable adtbCachedLowIncomeCreditRef = busGlobalFunctions.LoadLowIncomeCreditRefCacheData(iobjPassInfo);
                Decimal ldecLowIncomeCreditAmount = 0;
                var lenumList = adtbCachedLowIncomeCreditRef.AsEnumerable().Where(i => i.Field<Decimal>("low_income_credit") == lobjPayrollDetail.ibusPersonAccountMedicare.icdoPersonAccountMedicarePartDHistory.low_income_credit).OrderByDescending(i => i.Field<DateTime>("effective_date"));
                foreach (DataRow dr in lenumList)
                {
                    if (Convert.ToDateTime(dr["effective_date"]).Date <= DateTime.Now)
                    {
                        ldecLowIncomeCreditAmount = Convert.ToDecimal(dr["amount"]);
                        break;
                    }
                }
                ldecLateEnrollmentPenalty = lobjPayrollDetail.ibusPersonAccountMedicare.icdoPersonAccountMedicarePartDHistory.late_enrollment_penalty;
                ldecLowIncomeCredit = ldecLowIncomeCreditAmount;

                lobjPayrollDetail.icdoEmployerPayrollDetail.person_account_id = abusPersonAccount.icdoPersonAccount.person_account_id;

            }

            if (lobjPayrollDetail.icdoEmployerPayrollDetail.plan_id == busConstant.PlanIdEAP)
            {
                lobjPayrollDetail.ibusPersonAccountEAP = new busPersonAccountEAP { icdoPersonAccount = abusPersonAccount.icdoPersonAccount };

                //dka
                //Loading the History Object
                if ((iclbEAPHistory != null) && (iclbEAPHistory.Count > 0))
                {
                    var lclbData = iclbEAPHistory.Where(d => d.person_account_id == abusPersonAccount.icdoPersonAccount.person_account_id);
                    //Dont initialize the collection as the LoadHistoryByDate will load based on Null Check
                    if (lclbData != null && lclbData.Count() > 0)
                    {
                        lobjPayrollDetail.ibusPersonAccountEAP.iclbEAPHistory = new Collection<busPersonAccountEAPHistory>();
                        foreach (var lobjCDO in lclbData)
                        {
                            busPersonAccountEAPHistory lobjBus = new busPersonAccountEAPHistory();
                            lobjBus.icdoPersonAccountEapHistory = lobjCDO;
                            lobjPayrollDetail.ibusPersonAccountEAP.iclbEAPHistory.Add(lobjBus);
                        }
                    }
                }

                busPersonAccountEAPHistory lbusHistory = lobjPayrollDetail.ibusPersonAccountEAP.LoadHistoryByDate(PayrollPaidDate);
                lobjPayrollDetail.ibusPersonAccountEAP = lbusHistory.LoadEAPObject(lobjPayrollDetail.ibusPersonAccountEAP);

                lobjPayrollDetail.ibusPersonAccountEAP.ibusPerson = abusPersonAccount.ibusPerson;
                lobjPayrollDetail.ibusPersonAccountEAP.ibusPlan = abusPersonAccount.ibusPlan;
                lobjPayrollDetail.ibusPersonAccountEAP.ibusOrgPlan = abusPersonAccount.ibusOrgPlan;
                lobjPayrollDetail.ibusPersonAccountEAP.ibusPaymentElection = abusPersonAccount.ibusPaymentElection;

                if (iclbProviderOrgPlan != null)
                {
                    lobjPayrollDetail.ibusPersonAccountEAP.ibusProviderOrgPlan =
                        LoadProviderOrgPlanByProviderOrgId(
                            lobjPayrollDetail.ibusPersonAccountEAP.icdoPersonAccount.provider_org_id,
                            lobjPayrollDetail.ibusPersonAccountEAP.icdoPersonAccount.plan_id);
                }

            }

            //Loading the Life Object           
            if (lobjPayrollDetail.icdoEmployerPayrollDetail.plan_id == busConstant.PlanIdGroupLife)
            {
                if (ablnIsRgroupRetiree)
                {
                    if (iclbRGroupPersonAccountLife != null)
                    {
                        lobjPayrollDetail.ibusPersonAccountLife =
                            iclbRGroupPersonAccountLife.Where(
                                o => o.icdoPersonAccountLife.person_account_id == abusPersonAccount.icdoPersonAccount.person_account_id).FirstOrDefault();
                    }
                }
                else
                {
                    if (iclbPersonAccountLife != null)
                    {
                        lobjPayrollDetail.ibusPersonAccountLife =
                            iclbPersonAccountLife.Where(
                                o => o.icdoPersonAccountLife.person_account_id == abusPersonAccount.icdoPersonAccount.person_account_id).FirstOrDefault();
                    }
                }

                if (lobjPayrollDetail.ibusPersonAccountLife == null)
                {
                    lobjPayrollDetail.ibusPersonAccountLife = new busPersonAccountLife();
                    lobjPayrollDetail.ibusPersonAccountLife.FindPersonAccountLife(abusPersonAccount.icdoPersonAccount.person_account_id);
                }

                lobjPayrollDetail.ibusPersonAccountLife.icdoPersonAccount = abusPersonAccount.icdoPersonAccount;

                //dka
                if ((iclbPALifeOptionHistory != null) && (iclbPALifeOptionHistory.Count > 0))
                {
                    var lclbData = iclbPALifeOptionHistory.Where(d => d.person_account_id == abusPersonAccount.icdoPersonAccount.person_account_id);
                    if (lclbData != null && lclbData.Count() > 0)
                    {
                        //Loading the Life Option Data
                        lobjPayrollDetail.ibusPersonAccountLife.LoadLifeOptionDataFromHistory(lclbData);
                    }
                    else
                    {
                        //For R Group, we are not loading full collection upfront
                        lobjPayrollDetail.ibusPersonAccountLife.LoadLifeOptionDataFromHistory(PayrollPaidDate);
                    }
                }
                else
                {
                    //For R Group, we are not loading full collection upfront
                    lobjPayrollDetail.ibusPersonAccountLife.LoadLifeOptionDataFromHistory(PayrollPaidDate);
                }

                //Member Age Calcualtion Needs Person Object. (Optimization)
                lobjPayrollDetail.ibusPersonAccountLife.ibusPerson = abusPersonAccount.ibusPerson;
                lobjPayrollDetail.ibusPersonAccountLife.ibusPlan = abusPersonAccount.ibusPlan;
                lobjPayrollDetail.ibusPersonAccountLife.ibusPaymentElection = abusPersonAccount.ibusPaymentElection;
                lobjPayrollDetail.ibusPersonAccountLife.ibusOrgPlan = abusPersonAccount.ibusOrgPlan;

                if ((idtbOrgPlanProviders != null) && (lobjPayrollDetail.ibusPersonAccountLife.ibusOrgPlan != null)
                    && (lobjPayrollDetail.ibusPersonAccountLife.ibusOrgPlan.icdoOrgPlan.org_plan_id > 0) && (iclbProviderOrgPlan != null))
                {
                    DataRow[] larrRow = idtbOrgPlanProviders.FilterTable(busConstant.DataType.Numeric,
                                                                   "org_plan_id",
                                                                   lobjPayrollDetail.ibusPersonAccountLife.ibusOrgPlan.icdoOrgPlan.org_plan_id);
                    if (larrRow != null && larrRow.Length > 0)
                    {
                        lobjPayrollDetail.ibusPersonAccountLife.ibusOrgPlan.iclbOrgPlanProvider =
                            GetCollection<busOrgPlanProvider>(larrRow, "icdoOrgPlanProvider");
                    }
                    //Null Check Required
                    if (lobjPayrollDetail.ibusPersonAccountLife.ibusOrgPlan.iclbOrgPlanProvider != null)
                    {
                        lobjPayrollDetail.ibusPersonAccountLife.ibusProviderOrgPlan =
                            LoadProviderOrgPlanByProvider(lobjPayrollDetail.ibusPersonAccountLife.ibusOrgPlan.iclbOrgPlanProvider,
                            lobjPayrollDetail.ibusPersonAccountLife.icdoPersonAccount.plan_id);
                    }
                }

                //dka
                //Loading the History Object
                if ((iclbLifeHistory != null) && (iclbLifeHistory.Count > 0))
                {
                    var lclbData = iclbLifeHistory.Where(d=> d.person_account_id == abusPersonAccount.icdoPersonAccount.person_account_id);
                    //Dont initialize the collection as the LoadHistoryByDate will load based on Null Check
                    if (lclbData != null && lclbData.Count() > 0)
                    {
                        lobjPayrollDetail.ibusPersonAccountLife.iclbPersonAccountLifeHistory = new Collection<busPersonAccountLifeHistory>();
                        foreach (var lobjCDO in lclbData)
                        {
                            busPersonAccountLifeHistory lobjBus = new busPersonAccountLifeHistory();
                            lobjBus.icdoPersonAccountLifeHistory = lobjCDO;
                            lobjPayrollDetail.ibusPersonAccountLife.iclbPersonAccountLifeHistory.Add(lobjBus);
                        }
                    }
                }
            }

            //Loading the LTC Object           
            if (lobjPayrollDetail.icdoEmployerPayrollDetail.plan_id == busConstant.PlanIdLTC)
            {
                lobjPayrollDetail.ibusPersonAccountLtc = new busPersonAccountLtc { icdoPersonAccount = abusPersonAccount.icdoPersonAccount };

                //Member Age Calcualtion Needs Person Object. (Optimization)
                lobjPayrollDetail.ibusPersonAccountLtc.ibusPerson = abusPersonAccount.ibusPerson;
                lobjPayrollDetail.ibusPersonAccountLtc.ibusPlan = abusPersonAccount.ibusPlan;
                lobjPayrollDetail.ibusPersonAccountLtc.ibusPaymentElection = abusPersonAccount.ibusPaymentElection;
                lobjPayrollDetail.ibusPersonAccountLtc.ibusOrgPlan = abusPersonAccount.ibusOrgPlan;

                if ((idtbOrgPlanProviders != null) && (lobjPayrollDetail.ibusPersonAccountLtc.ibusOrgPlan != null)
                    && (lobjPayrollDetail.ibusPersonAccountLtc.ibusOrgPlan.icdoOrgPlan.org_plan_id > 0) && (iclbProviderOrgPlan != null))
                {
                    DataRow[] larrRow = idtbOrgPlanProviders.FilterTable(busConstant.DataType.Numeric,
                                                                   "org_plan_id",
                                                                   lobjPayrollDetail.ibusPersonAccountLtc.ibusOrgPlan.icdoOrgPlan.org_plan_id);
                    
                    if (larrRow != null && larrRow.Length > 0)
                    {
                        lobjPayrollDetail.ibusPersonAccountLtc.ibusOrgPlan.iclbOrgPlanProvider =
                            GetCollection<busOrgPlanProvider>(larrRow, "icdoOrgPlanProvider");
                    }
                    //Null Check Required
                    if (lobjPayrollDetail.ibusPersonAccountLtc.ibusOrgPlan.iclbOrgPlanProvider != null)
                    {
                        lobjPayrollDetail.ibusPersonAccountLtc.ibusProviderOrgPlan =
                            LoadProviderOrgPlanByProvider(lobjPayrollDetail.ibusPersonAccountLtc.ibusOrgPlan.iclbOrgPlanProvider,
                            lobjPayrollDetail.ibusPersonAccountLtc.icdoPersonAccount.plan_id);
                    }
                }

            }

            /* UAT PIR 476, Including other and JS RHIC Amount */
                lobjPayrollDetail.icdoEmployerPayrollDetail.premium_amount =
                    busRateHelper.GetInsurancePremiumAmount(ibusEmployerPayrollHeader.ibusOrganization, PayrollPaidDate,
                                                            abusPersonAccount.icdoPersonAccount.person_account_id,
                                                            abusPersonAccount.icdoPersonAccount.plan_id, ref ldecFeeAmt, ref ldecBuydownAmt, ref ldecMedicarePartDAmt,
                                                                           ref ldecRHICAmt, ref ldecOthrRHICAmt, ref ldecJSRHICAmt, ref ldecHSAAmt, ref ldecHSAVendorAmt, lobjPayrollDetail.ibusPersonAccountLife,
                                                                           lobjPayrollDetail.ibusPersonAccountGhdv,
                                                                           lobjPayrollDetail.ibusPersonAccountLtc,
                                                                           lobjPayrollDetail.ibusPersonAccountEAP
                                                                           ,lobjPayrollDetail.ibusPersonAccountMedicare  //PIR 15434
                                                                           , iobjPassInfo,
                                                                           ibusDBCacheData, ref lintGHDVHistoryID, ref lstrGroupNumber,
                                                                           ref lstrCoverageCodeValue, ref lstrRateStructureCode); //prod pir 6076
        
            
                //uat pir 1344
            //--Start--//
            ldecEmprSharePremium = ldecEmprShareFee = ldecEmprShareRHICAmt = ldecEmprShareOtherRHICAmt = ldecEmprShareJSRHICAmt = 0.0m;
            if (ablnIsRgroupRetiree && lobjPayrollDetail.ibusPlan.IsGHDVPlan() &&
                !string.IsNullOrEmpty(lobjPayrollDetail.ibusPersonAccountGhdv.icdoPersonAccountGhdv.cobra_type_value) &&
                abusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id > 0 &&
                abusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share > 0 &&
                abusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share < 100)
            {
                ldecEmprSharePremium = Math.Round(lobjPayrollDetail.icdoEmployerPayrollDetail.premium_amount *
                                           abusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share / 100, 2, MidpointRounding.AwayFromZero);
                ldecEmprShareFee = Math.Round(ldecFeeAmt *
                    abusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share / 100, 2, MidpointRounding.AwayFromZero);
                ldecEmprShareBuydown = Math.Round(ldecBuydownAmt *
                    abusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share / 100, 2, MidpointRounding.AwayFromZero);
                ldecEmprShareRHICAmt = Math.Round(ldecRHICAmt *
                    abusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share / 100, 2, MidpointRounding.AwayFromZero);
                ldecEmprShareOtherRHICAmt = Math.Round(ldecOthrRHICAmt *
                    abusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share / 100, 2, MidpointRounding.AwayFromZero);
                ldecEmprShareJSRHICAmt = Math.Round(ldecJSRHICAmt *
                    abusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share / 100, 2, MidpointRounding.AwayFromZero);
                ldecEmprShareMedicarePartDAmt = Math.Round(ldecMedicarePartDAmt *
                    abusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share / 100, 2, MidpointRounding.AwayFromZero); //PIR 14271

                lobjPayrollDetail.icdoEmployerPayrollDetail.premium_amount = ldecEmprSharePremium;
                ldecFeeAmt = ldecEmprShareFee;
                ldecBuydownAmt = ldecEmprShareBuydown;
                ldecRHICAmt = ldecEmprShareRHICAmt;
                ldecOthrRHICAmt = ldecEmprShareOtherRHICAmt;
                ldecJSRHICAmt = ldecEmprShareJSRHICAmt;
                ldecMedicarePartDAmt = ldecEmprShareMedicarePartDAmt;//PIR 14271
            }
            //--End--//
            //Include the Fee Amount
            lobjPayrollDetail.icdoEmployerPayrollDetail.premium_amount += ldecFeeAmt;
            lobjPayrollDetail.icdoEmployerPayrollDetail.premium_amount -= ldecBuydownAmt;
            lobjPayrollDetail.icdoEmployerPayrollDetail.group_health_fee_amount = ldecFeeAmt;
            lobjPayrollDetail.icdoEmployerPayrollDetail.buydown_amount = ldecBuydownAmt; // PIR 11239
            lobjPayrollDetail.icdoEmployerPayrollDetail.medicare_part_d_amt = ldecMedicarePartDAmt;//PIR 14271
            lobjPayrollDetail.icdoEmployerPayrollDetail.rhic_benefit_amount = ldecRHICAmt;
            lobjPayrollDetail.icdoEmployerPayrollDetail.othr_rhic_amount = ldecOthrRHICAmt;
            lobjPayrollDetail.icdoEmployerPayrollDetail.js_rhic_amount = ldecJSRHICAmt;
            lobjPayrollDetail.icdoEmployerPayrollDetail.premium_amount -= ldecRHICAmt; //need to reduce rhic amount from premium amount
            lobjPayrollDetail.icdoEmployerPayrollDetail.premium_amount += ldecMedicarePartDAmt;
            /* UAT PIR 476 ends here */
            //PIR 7705 - Premium AMount = Provider Premium Amount + Fee Amount + HSA Amount
            lobjPayrollDetail.icdoEmployerPayrollDetail.premium_amount += ldecHSAAmt;
            lobjPayrollDetail.icdoEmployerPayrollDetail.hsa_amount = ldecHSAAmt;
            lobjPayrollDetail.icdoEmployerPayrollDetail.vendor_amount = ldecHSAVendorAmt;
            //uat pir 1429 : to post ghdv_history_id and group number
            //prod pir 6076 & 6077 - Removal of person account ghdv history id
            //lobjPayrollDetail.icdoEmployerPayrollDetail.person_account_ghdv_history_id = lintGHDVHistoryID;
            if (string.IsNullOrEmpty(lstrGroupNumber))
            {
                if (ibusEmployerPayrollHeader.ibusOrganization == null)
                    ibusEmployerPayrollHeader.LoadOrganization();
                lobjPayrollDetail.icdoEmployerPayrollDetail.group_number = ibusEmployerPayrollHeader.ibusOrganization.icdoOrganization.org_code;
            }
            else
            {
                lobjPayrollDetail.icdoEmployerPayrollDetail.group_number = lstrGroupNumber;
            }
            //prod pir 6076
            lobjPayrollDetail.icdoEmployerPayrollDetail.coverage_code = lstrCoverageCodeValue;
            lobjPayrollDetail.icdoEmployerPayrollDetail.rate_structure_code = lstrRateStructureCode;

            if (lobjPayrollDetail.icdoEmployerPayrollDetail.plan_id == busConstant.PlanIdGroupLife)
            {
                if (ablnIsRgroupRetiree)
                {
                    //For R Group Life Plan, Include only Org To Bill Amount                   
                    if (lobjPayrollDetail.ibusPersonAccountLife.ibusPaymentElection != null)
                    {
                        decimal ldecFinalPremiumAmount = 0.00M;
                        if (lobjPayrollDetail.ibusPersonAccountLife.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id > 0)
                        {
                            ldecFinalPremiumAmount += lobjPayrollDetail.ibusPersonAccountLife.idecLifeBasicPremiumAmt;
                            lobjPayrollDetail.icdoEmployerPayrollDetail.basic_premium = lobjPayrollDetail.ibusPersonAccountLife.idecLifeBasicPremiumAmt;
                        }

                        if (lobjPayrollDetail.ibusPersonAccountLife.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_supplemental_org_id > 0)
                        {
                            ldecFinalPremiumAmount += lobjPayrollDetail.ibusPersonAccountLife.idecLifeSupplementalPremiumAmount;
                            lobjPayrollDetail.icdoEmployerPayrollDetail.supplemental_premium = lobjPayrollDetail.ibusPersonAccountLife.idecLifeSupplementalPremiumAmount;
                        }
                        lobjPayrollDetail.icdoEmployerPayrollDetail.premium_amount = ldecFinalPremiumAmount;
                        //R Group doesnt offer these premium
                        lobjPayrollDetail.icdoEmployerPayrollDetail.spouse_premium = 0.00M;
                        lobjPayrollDetail.icdoEmployerPayrollDetail.dependent_premium = 0.00M;
                    }
                }
                else
                {
                    lobjPayrollDetail.icdoEmployerPayrollDetail.basic_premium = lobjPayrollDetail.ibusPersonAccountLife.idecLifeBasicPremiumAmt;
                    lobjPayrollDetail.icdoEmployerPayrollDetail.supplemental_premium = lobjPayrollDetail.ibusPersonAccountLife.idecLifeSupplementalPremiumAmount;
                    lobjPayrollDetail.icdoEmployerPayrollDetail.spouse_premium = lobjPayrollDetail.ibusPersonAccountLife.idecSpouseSupplementalPremiumAmt;
                    lobjPayrollDetail.icdoEmployerPayrollDetail.dependent_premium = lobjPayrollDetail.ibusPersonAccountLife.idecDependentSupplementalPremiumAmt;
                }
                lobjPayrollDetail.icdoEmployerPayrollDetail.life_ins_age = lobjPayrollDetail.ibusPersonAccountLife.icdoPersonAccountLife.Life_Insurance_Age;
                //prod pir 4260
                lobjPayrollDetail.icdoEmployerPayrollDetail.ad_and_d_basic_premium_rate = lobjPayrollDetail.ibusPersonAccountLife.idecADAndDBasicRate;
                lobjPayrollDetail.icdoEmployerPayrollDetail.ad_and_d_supplemental_premium_rate = lobjPayrollDetail.ibusPersonAccountLife.idecADAndDSupplementalRate;
                lobjPayrollDetail.icdoEmployerPayrollDetail.life_basic_coverage_amount = lobjPayrollDetail.ibusPersonAccountLife.idecBasicCoverageAmount;
                lobjPayrollDetail.icdoEmployerPayrollDetail.life_supp_coverage_amount = lobjPayrollDetail.ibusPersonAccountLife.idecSuppCoverageAmount;
                lobjPayrollDetail.icdoEmployerPayrollDetail.life_spouse_supp_coverage_amount = lobjPayrollDetail.ibusPersonAccountLife.idecSpouseSuppCoverageAmount;
                lobjPayrollDetail.icdoEmployerPayrollDetail.life_dep_supp_coverage_amount = lobjPayrollDetail.ibusPersonAccountLife.idecDepSuppCoverageAmount;
            }

            if (lobjPayrollDetail.icdoEmployerPayrollDetail.plan_id == busConstant.PlanIdLTC)
            {
                foreach (var lbusLtcOption in lobjPayrollDetail.ibusPersonAccountLtc.iclbLtcOptionMember)
                {
                    if (lbusLtcOption.icdoPersonAccountLtcOption.level_of_coverage_value == busConstant.LTCLevelOfCoverage3YRS)
                    {
                        lobjPayrollDetail.icdoEmployerPayrollDetail.ltc_member_three_yrs_premium = lbusLtcOption.idecMonthlyPremium;
                    }
                    else if (lbusLtcOption.icdoPersonAccountLtcOption.level_of_coverage_value == busConstant.LTCLevelOfCoverage5YRS)
                    {
                        lobjPayrollDetail.icdoEmployerPayrollDetail.ltc_member_five_yrs_premium = lbusLtcOption.idecMonthlyPremium;
                    }
                }

                foreach (var lbusLtcOption in lobjPayrollDetail.ibusPersonAccountLtc.iclbLtcOptionSpouse)
                {
                    if (lbusLtcOption.icdoPersonAccountLtcOption.level_of_coverage_value == busConstant.LTCLevelOfCoverage3YRS)
                    {
                        lobjPayrollDetail.icdoEmployerPayrollDetail.ltc_spouse_three_yrs_premium = lbusLtcOption.idecMonthlyPremium;
                    }
                    else if (lbusLtcOption.icdoPersonAccountLtcOption.level_of_coverage_value == busConstant.LTCLevelOfCoverage5YRS)
                    {
                        lobjPayrollDetail.icdoEmployerPayrollDetail.ltc_spouse_five_yrs_premium = lbusLtcOption.idecMonthlyPremium;
                    }
                }
            }
            if (ablnIsRgroupRetiree)
                lobjPayrollDetail.icdoEmployerPayrollDetail.rgroup_retiree_flag = busConstant.Flag_Yes;

            //PIR 4444
            if (lobjPayrollDetail.ibusPlan.IsGHDVPlan())
            {
                if (lobjPayrollDetail.ibusPersonAccountGhdv.ibusProviderOrgPlan != null)
                    lobjPayrollDetail.icdoEmployerPayrollDetail.provider_org_id = lobjPayrollDetail.ibusPersonAccountGhdv.ibusProviderOrgPlan.icdoOrgPlan.org_id;
            }
			//PIR 15434
            else if (lobjPayrollDetail.icdoEmployerPayrollDetail.plan_id == busConstant.PlanIdMedicarePartD)
            {
                if (lobjPayrollDetail.ibusPersonAccountMedicare.ibusProviderOrgPlan != null)
                    lobjPayrollDetail.icdoEmployerPayrollDetail.provider_org_id = lobjPayrollDetail.ibusPersonAccountMedicare.ibusProviderOrgPlan.icdoOrgPlan.org_id;
            }
            else if (lobjPayrollDetail.icdoEmployerPayrollDetail.plan_id == busConstant.PlanIdGroupLife)
            {
                if (lobjPayrollDetail.ibusPersonAccountLife.ibusProviderOrgPlan != null)
                    lobjPayrollDetail.icdoEmployerPayrollDetail.provider_org_id = lobjPayrollDetail.ibusPersonAccountLife.ibusProviderOrgPlan.icdoOrgPlan.org_id;
            }
            else if (lobjPayrollDetail.icdoEmployerPayrollDetail.plan_id == busConstant.PlanIdLTC)
            {
                if (lobjPayrollDetail.ibusPersonAccountLtc.ibusProviderOrgPlan != null)
                    lobjPayrollDetail.icdoEmployerPayrollDetail.provider_org_id = lobjPayrollDetail.ibusPersonAccountLtc.ibusProviderOrgPlan.icdoOrgPlan.org_id;
            }
            else if (lobjPayrollDetail.icdoEmployerPayrollDetail.plan_id == busConstant.PlanIdEAP)
            {
                if (lobjPayrollDetail.ibusPersonAccountEAP.ibusProviderOrgPlan != null)
                    lobjPayrollDetail.icdoEmployerPayrollDetail.provider_org_id = lobjPayrollDetail.ibusPersonAccountEAP.ibusProviderOrgPlan.icdoOrgPlan.org_id;
            }

            //Org to bill
            if (lobjPayrollDetail.icdoEmployerPayrollDetail.plan_id == busConstant.PlanIdMedicarePartD)
            {
                lobjPayrollDetail.icdoEmployerPayrollDetail.lep_amount = ldecLateEnrollmentPenalty; 
                lobjPayrollDetail.icdoEmployerPayrollDetail.lis_amount = ldecLowIncomeCredit;

                //PIR 18807 - LEP amount is already added to the premium while calculating from GetInsurancePremiumAmount function. 
                lobjPayrollDetail.icdoEmployerPayrollDetail.premium_amount = lobjPayrollDetail.icdoEmployerPayrollDetail.premium_amount -
                                                                             lobjPayrollDetail.icdoEmployerPayrollDetail.lis_amount;
            }

            lobjPayrollDetail.icdoEmployerPayrollDetail.ienuObjectState = ObjectState.Insert;
            return lobjPayrollDetail;
        }

        private busOrgPlan LoadProviderOrgPlanByProvider(Collection<busOrgPlanProvider> aclbOrgPlanProvider, int aintPlanId)
        {
            busOrgPlan lbusOrgPlanToReturn = new busOrgPlan { icdoOrgPlan = new cdoOrgPlan() };
            foreach (busOrgPlanProvider lbusOPProvider in aclbOrgPlanProvider)
            {
                foreach (var lbusOrgPlan in iclbProviderOrgPlan)
                {
                    if ((lbusOrgPlan.icdoOrgPlan.org_id == lbusOPProvider.icdoOrgPlanProvider.provider_org_id) &&
                       (lbusOrgPlan.icdoOrgPlan.plan_id == aintPlanId))
                    {
                        if (busGlobalFunctions.CheckDateOverlapping(PayrollPaidDate, lbusOrgPlan.icdoOrgPlan.participation_start_date,
                            lbusOrgPlan.icdoOrgPlan.participation_end_date))
                        {
                            lbusOrgPlanToReturn = lbusOrgPlan;
                            break;
                        }
                    }
                }
                if (lbusOrgPlanToReturn.icdoOrgPlan.org_plan_id > 0) break;
            }
            return lbusOrgPlanToReturn;
        }

        private busOrgPlan LoadProviderOrgPlanByProviderOrgId(int aintProviderOrgID, int aintPlanId)
        {
            busOrgPlan lbusOrgPlanToReturn = new busOrgPlan { icdoOrgPlan = new cdoOrgPlan() };
            foreach (var lbusOrgPlan in iclbProviderOrgPlan)
            {
                if ((lbusOrgPlan.icdoOrgPlan.org_id == aintProviderOrgID) &&
                   (lbusOrgPlan.icdoOrgPlan.plan_id == aintPlanId))
                {
                    if (busGlobalFunctions.CheckDateOverlapping(PayrollPaidDate, lbusOrgPlan.icdoOrgPlan.participation_start_date,
                        lbusOrgPlan.icdoOrgPlan.participation_end_date))
                    {
                        lbusOrgPlanToReturn = lbusOrgPlan;
                        break;
                    }
                }
            }
            return lbusOrgPlanToReturn;
        }

        public void LoadRGroupMembers(busOrganization abusOrganization)
        {
            iclbRGroupPersonAccount = new Collection<busPersonAccount>();
            iclbRGroupPersonAccountGhdv = new Collection<busPersonAccountGhdv>();
            iclbRGroupPersonAccountLife = new Collection<busPersonAccountLife>();
            iclbRGroupPersonAccountMedicare = new Collection<busPersonAccountMedicarePartDHistory>();

            DataTable ldtbList = Select("cdoEmployerPayrollHeader.LoadRGroupMembersByOrg",
                                        new object[2] { abusOrganization.icdoOrganization.org_id, PayrollPaidDate });
            foreach (DataRow ldrRow in ldtbList.Rows)
            {
                var lbusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
                lbusPersonAccount.icdoPersonAccount.LoadData(ldrRow);

                lbusPersonAccount.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                lbusPersonAccount.ibusPerson.icdoPerson.LoadData(ldrRow);

                lbusPersonAccount.ibusPlan = new busPlan { icdoPlan = new cdoPlan() };
                lbusPersonAccount.ibusPlan.icdoPlan.LoadData(ldrRow);

                //uat pir - 1344
                lbusPersonAccount.ibusPaymentElection = new busPersonAccountPaymentElection { icdoPersonAccountPaymentElection = new cdoPersonAccountPaymentElection() };
                lbusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.LoadData(ldrRow);

                iclbRGroupPersonAccount.Add(lbusPersonAccount);

                //Adding the GHDV Object (Optimization)
                var lbusPersonAccountGhdv = new busPersonAccountGhdv { icdoPersonAccountGhdv = new cdoPersonAccountGhdv() };
                lbusPersonAccountGhdv.icdoPersonAccountGhdv.LoadData(ldrRow);
                iclbRGroupPersonAccountGhdv.Add(lbusPersonAccountGhdv);
				
				//PIR 15434	
                //Adding the medicare Object (Optimization)
                var lbusPersonAccountMedicare = new busPersonAccountMedicarePartDHistory { icdoPersonAccountMedicarePartDHistory = new cdoPersonAccountMedicarePartDHistory() };
                lbusPersonAccountMedicare.icdoPersonAccountMedicarePartDHistory.LoadData(ldrRow);
                iclbRGroupPersonAccountMedicare.Add(lbusPersonAccountMedicare);

                //Adding the Life Object (Optimization)
                var lbusPersonAccountLife = new busPersonAccountLife { icdoPersonAccountLife = new cdoPersonAccountLife() };
                lbusPersonAccountLife.icdoPersonAccountLife.LoadData(ldrRow);
                lbusPersonAccountLife.ibusPaymentElection = new busPersonAccountPaymentElection()
                {
                    icdoPersonAccountPaymentElection = new cdoPersonAccountPaymentElection()
                };
                lbusPersonAccountLife.ibusPaymentElection.icdoPersonAccountPaymentElection.LoadData(ldrRow);
                iclbRGroupPersonAccountLife.Add(lbusPersonAccountLife);
            }
        }

        private void LoadActiveInsuranceMembersByOrg(busOrganization abusOrganization)
        {
            //Loading Complete Activte Insurance Members List (Optimization Purpose)
            bool lblnLoadOldValues = utlPassInfo.iobjPassInfo.iblnLoadOldValues;
            try
            {
                utlPassInfo.iobjPassInfo.iblnLoadOldValues = false;
                DataTable ldtbActiveInsuranceMembers =
                    busNeoSpinBase.Select("cdoEmployerPayrollHeader.LoadActiveInsuranceMemberByOrg",
                                          new object[2] { PayrollPaidDate, abusOrganization.icdoOrganization.org_id });
                iclbActiveInsuranceMembers = new Collection<busPersonAccount>();
                iclbPersonAccountGhdv = new Collection<busPersonAccountGhdv>();
                iclbPersonAccountLife = new Collection<busPersonAccountLife>();
                if (iclbAllPlans == null)
                    LoadAllPlans();
                foreach (DataRow ldrRow in ldtbActiveInsuranceMembers.Rows)
                {
                    //Loading Person Account from the Query Result (Optimization)
                    var lbusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
                    lbusPersonAccount.icdoPersonAccount.LoadData(ldrRow);

                    lbusPersonAccount.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                    lbusPersonAccount.ibusPerson.icdoPerson.LoadData(ldrRow);

                    lbusPersonAccount.ibusPlan = iclbAllPlans.FirstOrDefault(i => i.icdoPlan.plan_id == lbusPersonAccount.icdoPersonAccount.plan_id);

                    lbusPersonAccount.ibusPaymentElection = new busPersonAccountPaymentElection { icdoPersonAccountPaymentElection = new cdoPersonAccountPaymentElection() };
                    lbusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.LoadData(ldrRow);

                    lbusPersonAccount.ibusPersonEmploymentDetail = new busPersonEmploymentDetail
                    {
                        icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail()
                    };
                    lbusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.LoadData(ldrRow);

                    lbusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment = new busPersonEmployment
                    {
                        icdoPersonEmployment = new cdoPersonEmployment()
                    };
                    lbusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment = new busPersonEmployment
                    {
                        icdoPersonEmployment = new cdoPersonEmployment()
                    };
                    lbusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.LoadData(ldrRow);

                    iclbActiveInsuranceMembers.Add(lbusPersonAccount);

                    //Adding the GHDV Object (Optimization)
                    var lbusPersonAccountGhdv = new busPersonAccountGhdv { icdoPersonAccountGhdv = new cdoPersonAccountGhdv() };
                    lbusPersonAccountGhdv.icdoPersonAccountGhdv.LoadData(ldrRow);
                    iclbPersonAccountGhdv.Add(lbusPersonAccountGhdv);

                    //Adding the Life Object (Optimization)
                    var lbusPersonAccountLife = new busPersonAccountLife { icdoPersonAccountLife = new cdoPersonAccountLife() };
                    lbusPersonAccountLife.icdoPersonAccountLife.LoadData(ldrRow);
                    iclbPersonAccountLife.Add(lbusPersonAccountLife);
                }
            }
            finally
            {
                utlPassInfo.iobjPassInfo.iblnLoadOldValues = lblnLoadOldValues;
            }
        }

        public void LoadDBCacheData()
        {
            ibusDBCacheData = new busDBCacheData();
            ibusDBCacheData.idtbCachedRateRef = busGlobalFunctions.LoadHealthRateRefCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedRateStructureRef = busGlobalFunctions.LoadHealthRateStructureCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedCoverageRef = busGlobalFunctions.LoadHealthCoverageRefCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedHealthRate = busGlobalFunctions.LoadHealthRateCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedLifeRate = busGlobalFunctions.LoadLifeRateCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedDentalRate = busGlobalFunctions.LoadDentalRateCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedEapRate = busGlobalFunctions.LoadEAPRateCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedHMORate = busGlobalFunctions.LoadHMORateCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedLtcRate = busGlobalFunctions.LoadLTCRateCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedVisionRate = busGlobalFunctions.LoadVisionRateCacheData(iobjPassInfo);
        }

        public void LoadActiveProviders()
        {
            DataTable ldtbActiveProviders = busNeoSpinBase.Select("cdoEmployerPayrollHeader.LoadAllActiveProviders", new object[1] { PayrollPaidDate });
            iclbProviderOrgPlan = new busBase().GetCollection<busOrgPlan>(ldtbActiveProviders, "icdoOrgPlan");
        }

        public void LoadAllOrgPlanProviders()
        {
            idtbOrgPlanProviders = busNeoSpinBase.Select("cdoEmployerPayrollHeader.LoadAllOrgPlanProviders", new object[0] { });
        }

       /*
        public void LoadLifeOptionData()
        {
            idtbPALifeOptionHistory =
                busNeoSpinBase.Select("cdoEmployerPayrollHeader.LoadLifeOption", new object[1] { PayrollPaidDate });
        }

        public void LoadEAPHistory()
        {
            idtbEAPHistory =
                busNeoSpinBase.Select("cdoEmployerPayrollHeader.LoadEAPHistory", new object[1] { PayrollPaidDate });
        }

        public void LoadGHDVHistory()
        {
            idtbGHDVHistory =
                busNeoSpinBase.Select("cdoEmployerPayrollHeader.LoadGHDVHistory", new object[1] { PayrollPaidDate });
        }

        public void LoadLifeHistory()
        {
            idtbLifeHistory =
                busNeoSpinBase.Select("cdoEmployerPayrollHeader.LoadLifeHistory", new object[1] { PayrollPaidDate });
        }
        */

        public Collection<busPlan> iclbAllPlans { get; set; }
        public void LoadAllPlans()
        {
            DataTable ldtbList = iobjPassInfo.isrvDBCache.GetCacheData("sgt_plan", null);
            iclbAllPlans = GetCollection<busPlan>(ldtbList, "icdoPlan");
        }
    }
}
