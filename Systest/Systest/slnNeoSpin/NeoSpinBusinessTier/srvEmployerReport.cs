#region Using directives

using System;
using System.Data;
using NeoSpin.BusinessObjects;
using NeoSpin.CustomDataObjects;
using System.Globalization;
using System.Collections;
using Sagitec.DBUtility;
#endregion

namespace NeoSpin.BusinessTier
{
    public class srvEmployerReport : srvNeoSpin
    {
        public srvEmployerReport()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        protected override ArrayList ValidateNew(string astrFormName, Hashtable ahstParam)
        {
            ArrayList larrErrors = null;
            //iobjPassInfo.iconFramework.Open();
            try
            {
                if (astrFormName == "wfmEmployerPayrollDetailLookup")
                {
                    busEmployerPayrollDetailLookup lbusEmployerPayrollDetailLookup = new busEmployerPayrollDetailLookup();
                    larrErrors = lbusEmployerPayrollDetailLookup.ValidateNew(ahstParam);
                }
            }
            finally
            {
               // iobjPassInfo.iconFramework.Close();
            }
            return larrErrors;
        }
        public busDepositTape NewDepositTape()
        {
            busDepositTape lobjDepositTape = new busDepositTape();
            lobjDepositTape.icdoDepositTape = new cdoDepositTape();
            lobjDepositTape.icdoDepositTape.deposit_date = DateTime.Today;
            return lobjDepositTape;
        }

        public busDepositTape FindDepositTape(int Aintdeposittapeid, int aintActivityInstanceID)
        {
            busDepositTape lobjDepositTape = new busDepositTape();
            if (lobjDepositTape.FindDepositTape(Aintdeposittapeid))
            {
                lobjDepositTape.LoadDeposits();
                lobjDepositTape.LoadDepositsCountAndTotalAmount();
                if (aintActivityInstanceID > 0)
                {
                    lobjDepositTape.iintActivityInstanceID = aintActivityInstanceID;
                    lobjDepositTape.SetVisibilityForPullACH();
                }
                lobjDepositTape.LoadTotalRemittance(Aintdeposittapeid);// PIR 6905
            }
            return lobjDepositTape;
        }

        public busDepositTapeLookup LoadDepositTape(DataTable adtbSearchResult)
        {
            busDepositTapeLookup lobjDepositTape = new busDepositTapeLookup();
            lobjDepositTape.LoadDepositTape(adtbSearchResult);
            return lobjDepositTape;
        }

        public busDeposit NewDeposit(int AintDepositTapeId)
        {
            busDeposit lobjDeposit = new busDeposit();
            lobjDeposit.icdoDeposit = new cdoDeposit();
            lobjDeposit.ibusDepositTape = new busDepositTape();
            lobjDeposit.icdoDeposit.deposit_tape_id = AintDepositTapeId;
            lobjDeposit.ibusDepositTape.FindDepositTape(AintDepositTapeId);
            lobjDeposit.SetNewDepositStatus(AintDepositTapeId);
            lobjDeposit.icdoDeposit.deposit_source_value = busConstant.DepositSourceRegularDeposits;
            lobjDeposit.icdoDeposit.deposit_date = lobjDeposit.ibusDepositTape.icdoDepositTape.deposit_date == DateTime.MinValue ? DateTime.Today : lobjDeposit.ibusDepositTape.icdoDepositTape.deposit_date;
            //PIR 12243 
            //lobjDeposit.LoadOtherDeposits();
            lobjDeposit.LoadTotalsFromDetail();
            return lobjDeposit;
        }

        public busDeposit FindDeposit(int Aintdepositid)
        {
            busDeposit lobjDeposit = new busDeposit();
            if (lobjDeposit.FindDeposit(Aintdepositid))
            {
                lobjDeposit.LoadRemittances();
                lobjDeposit.LoadDepositTape();

                //PIR 12243 
                //lobjDeposit.LoadOtherDeposits();
                //this is for cor - PAY 4300 - Insurance Payments   
                //UAT - PIR 168
                lobjDeposit.LoadPerson();
                lobjDeposit.LoadPersonOrgNameByID();
                lobjDeposit.LoadOrgCodeID();
                lobjDeposit.LoadTotalsFromDetail();
                lobjDeposit.LoadTotalRemittance(lobjDeposit.icdoDeposit.deposit_tape_id);// PIR 6905
                lobjDeposit.LoadBenfitTypeValue();
            }
            return lobjDeposit;
        }

        public busRemittanceRefundAmount FindRemittanceRefundAmount(int aintRemittanceId)
        {
            busRemittanceRefundAmount lobjRemittanceRefundAmount = new busRemittanceRefundAmount();
            if (lobjRemittanceRefundAmount.FindRemittance(aintRemittanceId))
            {
                if (lobjRemittanceRefundAmount.icdoRemittance.refund_to_org_id.IsNotNull() && lobjRemittanceRefundAmount.icdoRemittance.refund_to_org_id != 0)
                {
                    lobjRemittanceRefundAmount.ibusOrganization = new busOrganization();
                    lobjRemittanceRefundAmount.ibusOrganization.FindOrganization(lobjRemittanceRefundAmount.icdoRemittance.refund_to_org_id);
                    lobjRemittanceRefundAmount.istrDifferentOrgCode = lobjRemittanceRefundAmount.ibusOrganization.icdoOrganization.org_code;
                }
                //PIR 16208 
                if (lobjRemittanceRefundAmount.icdoRemittance.refund_to_person_id != 0)
                {
                    lobjRemittanceRefundAmount.iintmemberperslinkid = lobjRemittanceRefundAmount.icdoRemittance.refund_to_person_id;
                }
                lobjRemittanceRefundAmount.AssignRefundAmount();
            }
            lobjRemittanceRefundAmount.LoadRemittancePersonOrOrgName();
            lobjRemittanceRefundAmount.istrOrgCodeID = busGlobalFunctions.GetOrgCodeFromOrgId(lobjRemittanceRefundAmount.icdoRemittance.org_id);
            lobjRemittanceRefundAmount.LoadPlanName();
            return lobjRemittanceRefundAmount;
        }

        public busDepositLookup LoadDeposit(DataTable adtbSearchResult)
        {
            busDepositLookup lobjDeposit = new busDepositLookup();
            lobjDeposit.LoadDeposit(adtbSearchResult);
            return lobjDeposit;
        }

        public busRemittance NewRemittance(int AintDepositID)
        {
            busRemittance lobjRemittance = new busRemittance();
            lobjRemittance.icdoRemittance = new cdoRemittance();
            lobjRemittance.ibusDeposit = new busDeposit();
            lobjRemittance.ibusDepositTape = new busDepositTape();
            lobjRemittance.icdoRemittance.deposit_id = AintDepositID;
            lobjRemittance.ibusDeposit.FindDeposit(AintDepositID);
            lobjRemittance.ibusDepositTape.FindDepositTape(lobjRemittance.ibusDeposit.icdoDeposit.deposit_tape_id);
            lobjRemittance.icdoRemittance.person_id = lobjRemittance.ibusDeposit.icdoDeposit.person_id;
            lobjRemittance.icdoRemittance.org_id = lobjRemittance.ibusDeposit.icdoDeposit.org_id;
            lobjRemittance.LoadOrgCodeID();
            lobjRemittance.LoadPersonOrgNameByID();
            lobjRemittance.LoadOtherRemittances();
            lobjRemittance.LoadTotalRemittanceAmount();
            lobjRemittance.GetPlan();
            lobjRemittance.GetRemittanceType();
            return lobjRemittance;
        }

        public busRemittance FindRemittance(int Aintremittanceid)
        {
            busRemittance lobjRemittance = new busRemittance();
            if (lobjRemittance.FindRemittance(Aintremittanceid))
            {
                lobjRemittance.LoadDepositTape();
                lobjRemittance.LoadPersonOrgNameByID();
                lobjRemittance.LoadRemittancePersonOrOrgName();
                lobjRemittance.LoadReviewRemittanceDistribution();
                lobjRemittance.LoadOrganization();
                lobjRemittance.istrOrgCodeID = lobjRemittance.ibusOrganization.icdoOrganization.org_code;
                lobjRemittance.LoadTotalRemittanceAmount();
                lobjRemittance.LoadOtherRemittances();
                lobjRemittance.LoadPlan();
            }
            return lobjRemittance;
        }

        public busRemittanceLookup LoadRemittances(DataTable adtbSearchResult)
        {
            busRemittanceLookup lobjRemittanceLookup = new busRemittanceLookup();
            lobjRemittanceLookup.LoadRemittance(adtbSearchResult);
            return lobjRemittanceLookup;
        }

        public busEmployerPayrollDetailLookup LoadPayrollDetail(DataTable adtbSearchResult)
        {
            busEmployerPayrollDetailLookup lobjPayrollDetailLookup = new busEmployerPayrollDetailLookup();
            lobjPayrollDetailLookup.LoadPayrollDetail(adtbSearchResult);
            lobjPayrollDetailLookup.LoadCommentsOfPayrollDetails(); // ESS Backlog PIR - 13416
            return lobjPayrollDetailLookup;
        }

        public busEmployerPayrollDetail NewEmployerPayrollDetail(int aintEmployerPayrollHeaderId)
        {
            busEmployerPayrollDetail lobjPayrollDetail = new busEmployerPayrollDetail();
            lobjPayrollDetail.icdoEmployerPayrollDetail = new cdoEmployerPayrollDetail();

            lobjPayrollDetail.icdoEmployerPayrollDetail.employer_payroll_header_id = aintEmployerPayrollHeaderId;
            lobjPayrollDetail.LoadPayrollHeader();
            if (lobjPayrollDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
                lobjPayrollDetail.icdoEmployerPayrollDetail.pay_check_date = lobjPayrollDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.pay_check_date;

            // PROD PIR 933
            if (lobjPayrollDetail.icdoEmployerPayrollDetail.plan_id == busConstant.PlanIdGroupHealth &&
                lobjPayrollDetail.icdoEmployerPayrollDetail.record_type_value == busConstant.RecordTypeNegativeAdjustment)
                lobjPayrollDetail.LoadDependents();
            lobjPayrollDetail.EvaluateInitialLoadRules();
            return lobjPayrollDetail;
        }

        public busEmployerPayrollDetail FindEmployerPayrollDetail(int Aintemployerpayrolldetailid)
        {
            busEmployerPayrollDetail lobjEmployerPayrollDetail = new busEmployerPayrollDetail();
            //To Avoid Null Issues in Smart Navigation
            lobjEmployerPayrollDetail.ibusEmployerPayrollHeader = new busEmployerPayrollHeader();
            lobjEmployerPayrollDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader();
            if (lobjEmployerPayrollDetail.FindEmployerPayrollDetail(Aintemployerpayrolldetailid))
            {
                lobjEmployerPayrollDetail.LoadPayrollHeader();
                lobjEmployerPayrollDetail.LoadOrgCodeID();

                //Org to bill
                if (lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.plan_id == busConstant.PlanIdMedicarePartD)
                {
                    //lobjIbsDetail.icdoIbsDetail.ldecLowIncomeCredit = lobjIbsDetail.icdoIbsDetail.lis_amount;
                    busPersonAccountMedicarePartDHistory lobjMedicare = new busPersonAccountMedicarePartDHistory { icdoPersonAccountMedicarePartDHistory = new cdoPersonAccountMedicarePartDHistory() };
                    lobjMedicare.FindMedicareByPersonAccountID(lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.person_account_id);
                    lobjMedicare.FindPersonAccount(lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.person_account_id);

                    lobjMedicare.LoadPlanEffectiveDate();

                    //Low Income Credit Amount should be populated from Ref table. 
                    DataTable adtbCachedLowIncomeCreditRef = busGlobalFunctions.LoadLowIncomeCreditRefCacheData(iobjPassInfo);
                    DateTime ldtEffectiveDate = new DateTime();
                    var lenumList = adtbCachedLowIncomeCreditRef.AsEnumerable().OrderByDescending(i => i.Field<DateTime>("effective_date"));
                    foreach (DataRow dr in lenumList)
                    {
                        if (Convert.ToDateTime(dr["effective_date"]).Date <= lobjMedicare.idtPlanEffectiveDate.Date)
                        {
                            ldtEffectiveDate = Convert.ToDateTime(dr["effective_date"]).Date;
                            break;
                        }
                    }
                    DataTable ldtFilteredLowIncomeCredit = adtbCachedLowIncomeCreditRef.AsEnumerable().Where(i => i.Field<DateTime>("effective_date") == ldtEffectiveDate.Date).AsDataTable();

                    var lenumListFiltered = ldtFilteredLowIncomeCredit.AsEnumerable().Where(i => i.Field<Decimal>("amount") == Math.Abs(lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.lis_amount)).OrderByDescending(i => i.Field<DateTime>("effective_date"));
                    foreach (DataRow dr in lenumListFiltered)
                    {
                        if (Convert.ToDateTime(dr["effective_date"]).Date <= lobjMedicare.idtPlanEffectiveDate.Date)
                        {
                            lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.idecLow_Income_Credit = Convert.ToDecimal(dr["low_income_credit"]);
                            break;
                        }
                    }

                    lobjEmployerPayrollDetail.LoadLowIncomeCreditRef();

                    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.iintPremiumForPersonId = lobjMedicare.icdoPersonAccount.person_id;//Display Premium for on screen
                }

                lobjEmployerPayrollDetail.LoadObjectsForValidation();
                lobjEmployerPayrollDetail.LoadErrors();
                lobjEmployerPayrollDetail.LoadEmployerPurchaseAllocation();
                lobjEmployerPayrollDetail.LoadEmployerPayrollDetailError();
                lobjEmployerPayrollDetail.LoadEmployerPayrollDetailErrorLOB();

                if (lobjEmployerPayrollDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.payroll_paid_date != DateTime.MinValue)
                {
                    lobjEmployerPayrollDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.pay_period = lobjEmployerPayrollDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.payroll_paid_date.ToString("MM/yyyy", CultureInfo.CreateSpecificCulture("en-US"));
                }
                else
                {
                    lobjEmployerPayrollDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.pay_period = String.Empty;
                }
                if (lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.pay_period_date != DateTime.MinValue)
                {
                    lobjEmployerPayrollDetail.pay_period = lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.pay_period_date.ToString("MM/yyyy", CultureInfo.CreateSpecificCulture("en-US"));
                }
                else
                {
                    lobjEmployerPayrollDetail.pay_period = String.Empty;
                }
                if (lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.pay_period_end_month_for_bonus != DateTime.MinValue)
                {
                    lobjEmployerPayrollDetail.pay_end_month = lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.pay_period_end_month_for_bonus.ToString("MM/yyyy", CultureInfo.CreateSpecificCulture("en-US"));
                }
                else
                {
                    lobjEmployerPayrollDetail.pay_end_month = String.Empty;
                }
                lobjEmployerPayrollDetail.LoadPeoplesoftID();

                // PROD PIR 933
                if (lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.plan_id == busConstant.PlanIdGroupHealth &&
                    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.record_type_value == busConstant.RecordTypeNegativeAdjustment)
                {
                    lobjEmployerPayrollDetail.LoadPersonAccountDependentBillingLink();
                    lobjEmployerPayrollDetail.LoadDependents();
                }

                //ESS Backlog PIR- 12843 Added Provider org code for insurance type records
                if (lobjEmployerPayrollDetail.ibusPlan.IsInsurancePlan())
                    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_org_code = busGlobalFunctions.GetOrgCodeFromOrgId(lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_org_id);
                // ESS Backlog PIR - 13416
                lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.comments = String.Empty;
                lobjEmployerPayrollDetail.LoadEmployerPayrollDetailComments();
                lobjEmployerPayrollDetail.LoadRetiremntPlanRate();
            }
            return lobjEmployerPayrollDetail;
        }

        public busEmployerPayrollDetailError FindEmployerPayrollDetailError(int Aintemployerpayrolldetailerrorid)
        {
            busEmployerPayrollDetailError lobjEmployerPayrollDetailError = new busEmployerPayrollDetailError();
            if (lobjEmployerPayrollDetailError.FindEmployerPayrollDetailError(Aintemployerpayrolldetailerrorid))
            {
            }
            return lobjEmployerPayrollDetailError;
        }
        public busEmployerPayrollHeader NewEmployerPayrollHeader()
        {
            busEmployerPayrollHeader lobjEmployerPayrollHeader = new busEmployerPayrollHeader();
            lobjEmployerPayrollHeader.icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader();
            lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.status_value = busConstant.PayrollHeaderStatusPending;
            lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1209, lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.status_value);
            //BR-034-16
            lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.reporting_source_value = busConstant.PayrollHeaderReportingSourcePaperRpt;
            lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.balancing_status_value = busConstant.PayrollHeaderBalancingStatusNoRemittance;
            lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.received_date = DateTime.Now;
            lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.submitted_date = DateTime.Now;
            return lobjEmployerPayrollHeader;
        }
        public busEmployerPayrollHeader FindEmployerPayrollHeader(int Aintemployerpayrollheaderid)
        {
            busEmployerPayrollHeader lobjEmployerPayrollHeader = new busEmployerPayrollHeader();
            if (lobjEmployerPayrollHeader.FindEmployerPayrollHeader(Aintemployerpayrollheaderid))
            {
                lobjEmployerPayrollHeader.LoadEmployerPayrollDetail();
                lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.total_detail_record_count = lobjEmployerPayrollHeader.iclbEmployerPayrollDetail.Count;
                lobjEmployerPayrollHeader.LoadOrganization();
                lobjEmployerPayrollHeader.istrOrgCodeId = lobjEmployerPayrollHeader.ibusOrganization.icdoOrganization.org_code;
                lobjEmployerPayrollHeader.LoadEmployerRemittanceAllocation();
                lobjEmployerPayrollHeader.LoadAvailableRemittanace();

                if (lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
                {
                    lobjEmployerPayrollHeader.LoadRetirementContributionByPlan();
                }
                else if (lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
                {
                    lobjEmployerPayrollHeader.LoadDeferredCompContributionByPlan();
                }
                else if (lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeInsr)
                {
                    lobjEmployerPayrollHeader.LoadInsurancePremiumByPlan();
                }
                else if (lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypePurchases)
                {
                    lobjEmployerPayrollHeader.LoadPurchaseByPlan();
                }

                lobjEmployerPayrollHeader.LoadTotalAppliedRemittance();
                if (lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.payroll_paid_date != DateTime.MinValue)
                {
                    lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.pay_period = lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.payroll_paid_date.ToString("MM/yyyy", CultureInfo.CreateSpecificCulture("en-US"));
                }
                else
                {
                    lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.pay_period = String.Empty;
                }
                lobjEmployerPayrollHeader.LoadStatusSummary();
                lobjEmployerPayrollHeader.LoadDetailError();
                lobjEmployerPayrollHeader.LoadErrors();
                lobjEmployerPayrollHeader.LoadEmployerPayrollHeaderError();
                lobjEmployerPayrollHeader.CalculateContributionWagesInterestFromDtl();
                lobjEmployerPayrollHeader.LoadErrorLOB();

                //PIR 23999 total contribution calculated for penalty is calculated while create penalty
                if (!(lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.report_type_value == busConstant.PayrollHeaderReportTypePenalty))
                {
                    //PIR 15238 
                    if (lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
                    {
                        lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.total_contribution_calculated = lobjEmployerPayrollHeader.idecTotalContributionFromDetails;
                        lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.total_wages_calculated = lobjEmployerPayrollHeader.idecTotalWagesCalculated;
                    }
                    else if (lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
                    {
                        lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.total_contribution_calculated = lobjEmployerPayrollHeader.idecTotalContributionCalculatedForDef;
                    }
                }
                // ESS Backlog PIR - 13416
                lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.comments = String.Empty;
                lobjEmployerPayrollHeader.LoadEmployerPayrollHeaderComments();
                //lobjEmployerPayrollHeader.LoadDetailComments();
                //PIR 25909 
                if (lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PlanBenefitTypeDeferredComp)
                {
                    lobjEmployerPayrollHeader.LoadTotalContributionDiffComp();
                }   
            }
            return lobjEmployerPayrollHeader;
        }
        public busEmployerPayrollHeaderLookup LoadEmployerPayrollHeaders(DataTable adtbSearchResult)
        {
            busEmployerPayrollHeaderLookup lobjEmployerPayrollHeaderLookup = new busEmployerPayrollHeaderLookup();
            lobjEmployerPayrollHeaderLookup.LoadEmployerPayrollHeader(adtbSearchResult);
            lobjEmployerPayrollHeaderLookup.LoadCommentsOfPayrollHeaders(); // ESS Backlog PIR - 13416
            return lobjEmployerPayrollHeaderLookup;
        }

        public busEmployerRemittanceAllocation FindEmployerRemittanceAllocation(int Aintemployerremittanceallocationid)
        {
            busEmployerRemittanceAllocation lobjEmployerRemittanceAllocation = new busEmployerRemittanceAllocation();
            if (lobjEmployerRemittanceAllocation.FindEmployerRemittanceAllocation(Aintemployerremittanceallocationid))
            {
                lobjEmployerRemittanceAllocation.LoadEmployerPayrollHeader();
                lobjEmployerRemittanceAllocation.ibusEmployerPayrollHeader.LoadAvailableRemittanace();
                lobjEmployerRemittanceAllocation.LoadRemittance();
                lobjEmployerRemittanceAllocation.LoadAvailableAmount();
                lobjEmployerRemittanceAllocation.LoadDeposit();
            }
            return lobjEmployerRemittanceAllocation;
        }

        public busEmployerRemittanceAllocationLookup LoadEmployerRemittanceAllocations(DataTable adtbSearchResult)
        {
            busEmployerRemittanceAllocationLookup lobjEmployerRemittanceAllocationLookup = new busEmployerRemittanceAllocationLookup();
            lobjEmployerRemittanceAllocationLookup.LoadEmployerRemittanceAllocation(adtbSearchResult);
            return lobjEmployerRemittanceAllocationLookup;
        }

        public busEmployerRemittanceAllocation NewEmployerRemittanceAllocation(int aintEmployerPayrollHeaderId)
        {
            busEmployerRemittanceAllocation lobjEmployerRemittanceAllocation = new busEmployerRemittanceAllocation();
            lobjEmployerRemittanceAllocation.icdoEmployerRemittanceAllocation = new cdoEmployerRemittanceAllocation();
            lobjEmployerRemittanceAllocation.icdoEmployerRemittanceAllocation.employer_payroll_header_id = aintEmployerPayrollHeaderId;
            lobjEmployerRemittanceAllocation.icdoEmployerRemittanceAllocation.payroll_allocation_status_value = busConstant.Pending_Allocation;
            lobjEmployerRemittanceAllocation.LoadEmployerPayrollHeader();
            lobjEmployerRemittanceAllocation.ibusEmployerPayrollHeader.LoadAvailableRemittanace();
            lobjEmployerRemittanceAllocation.EvaluateInitialLoadRules();
            return lobjEmployerRemittanceAllocation;
        }
        public busIbsHeader NewIbsHeader()
        {
            busIbsHeader lobjIbsHeader = new busIbsHeader();
            lobjIbsHeader.icdoIbsHeader = new cdoIbsHeader();
            lobjIbsHeader.icdoIbsHeader.report_status_value = busConstant.IBSHeaderStatusReview;
            lobjIbsHeader.icdoIbsHeader.report_status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1702, lobjIbsHeader.icdoIbsHeader.report_status_value);
            return lobjIbsHeader;
        }
        public busIbsHeader FindIbsHeader(int Aintibsheaderid)
        {
            busIbsHeader lobjIbsHeader = new busIbsHeader();
            if (lobjIbsHeader.FindIbsHeader(Aintibsheaderid))
            {
                //uat pir 1702
                //lobjIbsHeader.LoadIbsDetails();
                //lobjIbsHeader.icdoIbsHeader.total_record_count = lobjIbsHeader.icolIbsDetail.Count;
                //lobjIbsHeader.icdoIbsHeader.total_premium_amount = lobjIbsHeader.idecPremiumAmountTotal;
                lobjIbsHeader.LoadTotalRecordCountAndTotalPremium();
                lobjIbsHeader.LoadJsRhicRemittanceAllocations();
                lobjIbsHeader.LoadBill();
            }
            return lobjIbsHeader;
        }
        public busIbsDetail NewIbsDetail(int aintIBSHeaderID, int aintPersonID)
        {
            busIbsDetail lobjIbsDetail = new busIbsDetail();
            lobjIbsDetail.icdoIbsDetail = new cdoIbsDetail();
            if (aintIBSHeaderID != 0)
            {
                lobjIbsDetail.icdoIbsDetail.ibs_header_id = aintIBSHeaderID;
                lobjIbsDetail.LoadIbsHeader();
            }
            if (aintPersonID != 0)
            {
                lobjIbsDetail.icdoIbsDetail.person_id = aintPersonID;
                lobjIbsDetail.LoadPerson();
            }
            // PROD PIR 933
            if (lobjIbsDetail.icdoIbsDetail.plan_id == busConstant.PlanIdGroupHealth &&
                lobjIbsDetail.icdoIbsDetail.total_premium_amount < 0M)
                lobjIbsDetail.LoadDependents();
            return lobjIbsDetail;
        }

        public busIbsDetail NewIbsDetailOnline(int aintIBSHeaderID, int aintPlanID)
        {
            busIbsDetail lobjIbsDetail = new busIbsDetail();
            lobjIbsDetail.icdoIbsDetail = new cdoIbsDetail();
            if (aintIBSHeaderID != 0)
            {
                lobjIbsDetail.icdoIbsDetail.ibs_header_id = aintIBSHeaderID;
                lobjIbsDetail.LoadIbsHeader();
            }
            lobjIbsDetail.icdoIbsDetail.plan_id = aintPlanID;
            lobjIbsDetail.icdoIbsDetail.detail_status_value = busConstant.IBSHeaderStatusReview; // PIR 8164 & 8165 Ignore functionality on detail
            lobjIbsDetail.iblnOnlineCreation = true;
            lobjIbsDetail.EvaluateInitialLoadRules();
            lobjIbsDetail.LoadPlan();
            return lobjIbsDetail;
        }

        public busIbsDetail FindIbsDetail(int Aintibsdetailid)
        {
            busIbsDetail lobjIbsDetail = new busIbsDetail();
            if (lobjIbsDetail.FindIbsDetail(Aintibsdetailid))
            {
                lobjIbsDetail.LoadIbsHeader();
                lobjIbsDetail.LoadIbsPersonSummary();
                lobjIbsDetail.LoadPerson();
                lobjIbsDetail.LoadPersonAccountPremiumFor();
                lobjIbsDetail.LoadPlan();
                lobjIbsDetail.LoadIbsPlanHistoryDetails();
                // PROD PIR 933
                if (lobjIbsDetail.icdoIbsDetail.plan_id == busConstant.PlanIdGroupHealth &&
                    lobjIbsDetail.icdoIbsDetail.total_premium_amount < 0M)
                {
                    lobjIbsDetail.LoadPersonAccountDependentBillingLink();
                    lobjIbsDetail.LoadDependents();
                }
                if (lobjIbsDetail.icdoIbsDetail.plan_id == busConstant.PlanIdMedicarePartD)
                {
                    //lobjIbsDetail.icdoIbsDetail.ldecLowIncomeCredit = lobjIbsDetail.icdoIbsDetail.lis_amount;
                    busPersonAccountMedicarePartDHistory lobjMedicare = new busPersonAccountMedicarePartDHistory { icdoPersonAccountMedicarePartDHistory = new cdoPersonAccountMedicarePartDHistory() };
                    lobjMedicare.FindMedicareByPersonAccountID(lobjIbsDetail.icdoIbsDetail.person_account_id);
                    lobjMedicare.FindPersonAccount(lobjIbsDetail.icdoIbsDetail.person_account_id);

                    lobjMedicare.LoadPlanEffectiveDate();

                    //Low Income Credit Amount should be populated from Ref table. 
                    DataTable adtbCachedLowIncomeCreditRef = busGlobalFunctions.LoadLowIncomeCreditRefCacheData(iobjPassInfo);
                    DateTime ldtEffectiveDate = new DateTime();
                    var lenumList = adtbCachedLowIncomeCreditRef.AsEnumerable().OrderByDescending(i => i.Field<DateTime>("effective_date"));
                    foreach (DataRow dr in lenumList)
                    {
                        if (Convert.ToDateTime(dr["effective_date"]).Date <= lobjIbsDetail.icdoIbsDetail.billing_month_and_year)
                        {
                            ldtEffectiveDate = Convert.ToDateTime(dr["effective_date"]).Date;
                            break;
                        }
                    }
                    DataTable ldtFilteredLowIncomeCredit = adtbCachedLowIncomeCreditRef.AsEnumerable().Where(i => i.Field<DateTime>("effective_date") == ldtEffectiveDate.Date).AsDataTable();

                    var lenumListFiltered = ldtFilteredLowIncomeCredit.AsEnumerable().Where(i => i.Field<Decimal>("amount") == Math.Abs(lobjIbsDetail.icdoIbsDetail.lis_amount)).OrderByDescending(i => i.Field<DateTime>("effective_date"));
                    foreach (DataRow dr in lenumListFiltered)
                    {
                        if (Convert.ToDateTime(dr["effective_date"]).Date <= lobjIbsDetail.icdoIbsDetail.billing_month_and_year)
                        {
                            lobjIbsDetail.icdoIbsDetail.idecLow_Income_Credit = Convert.ToDecimal(dr["low_income_credit"]);
                            break;
                        }
                    }

                    lobjIbsDetail.LoadLowIncomeCreditRef();

                    lobjIbsDetail.icdoIbsDetail.iintPremiumForPersonId = lobjMedicare.icdoPersonAccount.person_id;//Display Premium for on screen
                }
            }

            lobjIbsDetail.icdoIbsDetail.istrProviderOrgCode = busGlobalFunctions.GetOrgCodeFromOrgId(lobjIbsDetail.icdoIbsDetail.provider_org_id);

            return lobjIbsDetail;
        }

        public busIbsRemittanceAllocation FindIbsRemittanceAllocation(int Aintibsremittanceallocationid)
        {
            busIbsRemittanceAllocation lobjIbsRemittanceAllocation = new busIbsRemittanceAllocation();
            if (lobjIbsRemittanceAllocation.FindIbsRemittanceAllocation(Aintibsremittanceallocationid))
            {
            }
            return lobjIbsRemittanceAllocation;
        }

        public busJsRhicRemittanceAllocation FindJsRhicRemittanceAllocation(int Aintrhicremittanceallocationid)
        {
            busJsRhicRemittanceAllocation lobjJsRhicRemittanceAllocation = new busJsRhicRemittanceAllocation();
            if (lobjJsRhicRemittanceAllocation.FindJsRhicRemittanceAllocation(Aintrhicremittanceallocationid))
            {
            }
            return lobjJsRhicRemittanceAllocation;
        }
        public busJsRhicBill NewJsRhicBill()
        {
            busJsRhicBill lobjJsRhicBill = new busJsRhicBill();
            lobjJsRhicBill.icdoJsRhicBill = new cdoJsRhicBill();
            //lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.status_value = busConstant.PayrollHeaderStatusPending;
            //lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1209, lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.status_value);
            return lobjJsRhicBill;
        }

        public busJsRhicBill FindJsRhicBill(int Aintjsrhicbillid, int aintIbsHeaderID)
        {
            busJsRhicBill lobjJsRhicBill = new busJsRhicBill();
            lobjJsRhicBill.ibusJsRhicRemittanceAllocation = new busJsRhicRemittanceAllocation();
            lobjJsRhicBill.ibusJsRhicRemittanceAllocation.icdoJsRhicRemittanceAllocation = new cdoJsRhicRemittanceAllocation();
            if (lobjJsRhicBill.FindJsRhicBill(Aintjsrhicbillid))
            {
                lobjJsRhicBill.iintIBSHeaderID = aintIbsHeaderID;
                lobjJsRhicBill.LoadAvalilableRemittance();
                lobjJsRhicBill.LoadAllocationHistory();
                lobjJsRhicBill.LoadJsRhicRemittanceAllocations();
                lobjJsRhicBill.istrOrgCode = busGlobalFunctions.GetOrgCodeFromOrgId(lobjJsRhicBill.icdoJsRhicBill.org_id);
                lobjJsRhicBill.idecRemainingBillAmount = lobjJsRhicBill.icdoJsRhicBill.bill_amount - lobjJsRhicBill.icdoJsRhicBill.allocated_amount;
            }
            return lobjJsRhicBill;
        }

        public busIbsHeaderLookup LoadIbsHeaders(DataTable adtbSearchResult)
        {
            busIbsHeaderLookup lobjbusIbsHeaderLookup = new busIbsHeaderLookup();
            lobjbusIbsHeaderLookup.LoadIbsHeader(adtbSearchResult);
            return lobjbusIbsHeaderLookup;
        }

        public busIbsDetailLookup LoadIbsDetails(DataTable adtbSearchResult)
        {
            busIbsDetailLookup lobjIbsDetailLookup = new busIbsDetailLookup();
            lobjIbsDetailLookup.LoadIbsDetail(adtbSearchResult);
            return lobjIbsDetailLookup;
        }
        public busEmployerPurchaseAllocation NewEmployerPurchaseAllocation(int AintEmployerPayrollDetailid)
        {
            busEmployerPurchaseAllocation lobjEmployerPurchaseAllocation = new busEmployerPurchaseAllocation();
            lobjEmployerPurchaseAllocation.icdoEmployerPurchaseAllocation = new cdoEmployerPurchaseAllocation();
            lobjEmployerPurchaseAllocation.icdoEmployerPurchaseAllocation.employer_payroll_detail_id = AintEmployerPayrollDetailid;
            lobjEmployerPurchaseAllocation.LoadEmployerPayrollDetail();
            return lobjEmployerPurchaseAllocation;
        }
        public busEmployerPurchaseAllocation FindEmployerPurchaseAllocation(int aintEmployerPurchaseAllocationid)
        {
            busEmployerPurchaseAllocation lobjEmployerPurchaseAllocation = new busEmployerPurchaseAllocation();
            if (lobjEmployerPurchaseAllocation.FindEmployerPurchaseAllocation(aintEmployerPurchaseAllocationid))
            {
                lobjEmployerPurchaseAllocation.LoadPurchaseHeader();
                lobjEmployerPurchaseAllocation.LoadEmployerPayrollDetail();
            }
            return lobjEmployerPurchaseAllocation;
        }

        public busEmployerPayrollMonthlyStatement FindEmployerPayrollMonthlyStatment(int Aintemployerpayrollmonthlystatementid)
        {
            busEmployerPayrollMonthlyStatement lobjEmployerPayrollMonthlyStatement = new busEmployerPayrollMonthlyStatement();
            if (lobjEmployerPayrollMonthlyStatement.FindEmployerPayrollMonthlyStatment(Aintemployerpayrollmonthlystatementid))
            {
            }

            return lobjEmployerPayrollMonthlyStatement;
        }

        public busEmployerPayrollMonthlyStatementLookup LoadEmployerPayrollMonthlyStatements(DataTable adtbSearchResult)
        {
            busEmployerPayrollMonthlyStatementLookup lobjEmployerPayrollMonthlyStatementLookup = new busEmployerPayrollMonthlyStatementLookup();
            lobjEmployerPayrollMonthlyStatementLookup.LoadEmployerPayrollMonthlyStatments(adtbSearchResult);
            return lobjEmployerPayrollMonthlyStatementLookup;
        }
        public busEmployerPayrollMonthlyStatement NewEmployerPayrollMonthlyStatements()
        {
            busEmployerPayrollMonthlyStatement lobjEmployerPayrollMonthlyStatement = new busEmployerPayrollMonthlyStatement();
            lobjEmployerPayrollMonthlyStatement.icdoEmployerPayrollMonthlyStatement = new cdoEmployerPayrollMonthlyStatement();
            return lobjEmployerPayrollMonthlyStatement;
        }

        public busIbsPersonSummary FindIbsPersonSummary(int aintibspersonsummaryid)
        {
            busIbsPersonSummary lobjIbsPersonSummary = new busIbsPersonSummary();
            if (lobjIbsPersonSummary.FindIbsPersonSummary(aintibspersonsummaryid))
            {
                lobjIbsPersonSummary.LoadIbsHeader();
                lobjIbsPersonSummary.LoadPerson();
            }

            return lobjIbsPersonSummary;
        }

        public busEmployerPayrollDetail NewOnlineEmployerPayrollDetail(int aintEmployerPayrollHeaderId, int aintPlanID, int aintEmployerPayrollDetailId)
        {
            busEmployerPayrollDetail lobjPayrollDetail = new busEmployerPayrollDetail();
            lobjPayrollDetail.icdoEmployerPayrollDetail = new cdoEmployerPayrollDetail();

            lobjPayrollDetail.iblnOnlineCreation = true;
            lobjPayrollDetail.icdoEmployerPayrollDetail.employer_payroll_header_id = aintEmployerPayrollHeaderId;
            lobjPayrollDetail.icdoEmployerPayrollDetail.plan_id = aintPlanID;
            lobjPayrollDetail.LoadPlan();
            lobjPayrollDetail.LoadPayrollHeader();
            if(iobjPassInfo.istrSenderID == "btnReplicate")
                lobjPayrollDetail = lobjPayrollDetail.btnReplicate_Clicked(aintEmployerPayrollDetailId);
            lobjPayrollDetail.EvaluateInitialLoadRules();
            return lobjPayrollDetail;
        }

        public busPersonAccountDependentBillingLink FindPersonAccountDependentBillingLink(int aintpersonaccountdependentbillinglinkid)
        {
            busPersonAccountDependentBillingLink lobjPersonAccountDependentBillingLink = new busPersonAccountDependentBillingLink();
            if (lobjPersonAccountDependentBillingLink.FindPersonAccountDependentBillingLink(aintpersonaccountdependentbillinglinkid))
            {
            }

            return lobjPersonAccountDependentBillingLink;
        }

        public busPersonAccountDependentBillingLink NewPersonAccountDependentBillingLink()
        {
            busPersonAccountDependentBillingLink lobjPersonAccountDependentBillingLink = new busPersonAccountDependentBillingLink();
            lobjPersonAccountDependentBillingLink.icdoPersonAccountDependentBillingLink = new cdoPersonAccountDependentBillingLink();
            return lobjPersonAccountDependentBillingLink;
        }

        //PIR 8164 - Ignore button on Lookup
        public busIbsDetailLookup btnIgnoreClick(ArrayList arrSelectedObjects)
        {
            busIbsDetailLookup lbusIBSDetailLokkup = new busIbsDetailLookup();
            lbusIBSDetailLokkup.UpdateStatusToIgnore(arrSelectedObjects);
            return lbusIBSDetailLokkup;
        }
    }
}
