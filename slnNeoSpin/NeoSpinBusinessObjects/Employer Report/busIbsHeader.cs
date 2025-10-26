#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;
using System.Linq;
using System.Linq.Expressions;

using System.Globalization;
using System.Collections.Generic;
#endregion

namespace NeoSpin.BusinessObjects
{
    public partial class busIbsHeader : busExtendBase
    {
        private decimal _idecPremiumAmountTotal;
        public decimal idecPremiumAmountTotal
        {
            get { return _idecPremiumAmountTotal; }
            set { _idecPremiumAmountTotal = value; }
        }
        private busJsRhicBill _ibusJsRhicBill;
        public busJsRhicBill ibusJsRhicBill
        {
            get
            {
                return _ibusJsRhicBill;
            }
            set
            {
                _ibusJsRhicBill = value;
            }
        }
        private busJsRhicRemittanceAllocation _ibusJsRhicRemittanceAllocation;
        public busJsRhicRemittanceAllocation ibusJsRhicRemittanceAllocation
        {
            get
            {
                return _ibusJsRhicRemittanceAllocation;
            }
            set
            {
                _ibusJsRhicRemittanceAllocation = value;
            }
        }

        private DateTime _idtBatchDate;
        public DateTime idtBatchDate
        {
            get
            {
                return _idtBatchDate;
            }
            set
            {
                _idtBatchDate = value;
            }
        }

        private Collection<busPerson> _iclbIBSMembers;
        public Collection<busPerson> iclbIBSMembers
        {
            get
            {
                return _iclbIBSMembers;
            }

            set
            {
                _iclbIBSMembers = value;
            }
        }

        public DateTime idtGLPostingDate { get; set; }

        public Collection<busIbsPersonSummary> iclbIbsPersonSummary { get; set; }
        public void LoadIbsPersonSummary()
        {
            iclbIbsPersonSummary = new Collection<busIbsPersonSummary>();
            DataTable ldtbList = Select<cdoIbsPersonSummary>(
                new string[1] { "ibs_header_id" },
                new object[1] { icdoIbsHeader.ibs_header_id }, null, null);
            iclbIbsPersonSummary = GetCollection<busIbsPersonSummary>(ldtbList, "icdoIbsPersonSummary");
        }

        public busDBCacheData ibusDBCacheData { get; set; }

        public void LoadIbsDetails()
        {
            icolIbsDetail = new Collection<busIbsDetail>();
            DataTable ldtbList = Select("cdoIbsHeader.LoadIBSDetails", new object[1] { icdoIbsHeader.ibs_header_id });
            foreach (DataRow adrRow in ldtbList.Rows)
            {
                busIbsDetail lbusIbsDetail = new busIbsDetail { icdoIbsDetail = new cdoIbsDetail() };
                lbusIbsDetail.ibusPlan = new busPlan { icdoPlan = new cdoPlan() };
                lbusIbsDetail.ibusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
                lbusIbsDetail.ibusPersonAccount.ibusPaymentElection = new busPersonAccountPaymentElection { icdoPersonAccountPaymentElection = new cdoPersonAccountPaymentElection() };
                lbusIbsDetail.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                lbusIbsDetail.icdoIbsDetail.LoadData(adrRow);
                lbusIbsDetail.ibusPlan.icdoPlan.LoadData(adrRow);
                lbusIbsDetail.ibusPersonAccount.icdoPersonAccount.LoadData(adrRow);
                lbusIbsDetail.ibusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.LoadData(adrRow);
                lbusIbsDetail.ibusPerson.icdoPerson.LoadData(adrRow);
                icolIbsDetail.Add(lbusIbsDetail);
                _idecPremiumAmountTotal += lbusIbsDetail.icdoIbsDetail.total_premium_amount;
            }
        }
		
		//PIR 15417
        public void LoadIbsDetailsForCorrespondance()
        {
            icolIbsDetail = new Collection<busIbsDetail>();
            DataTable ldtbList = Select("cdoIbsHeader.LoadIbsDetailsForCorrespondance", new object[1] { icdoIbsHeader.ibs_header_id });
            foreach (DataRow adrRow in ldtbList.Rows)
            {
                busIbsDetail lbusIbsDetail = new busIbsDetail { icdoIbsDetail = new cdoIbsDetail() };
                lbusIbsDetail.ibusPlan = new busPlan { icdoPlan = new cdoPlan() };
                lbusIbsDetail.ibusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
                lbusIbsDetail.ibusPersonAccount.ibusPaymentElection = new busPersonAccountPaymentElection { icdoPersonAccountPaymentElection = new cdoPersonAccountPaymentElection() };
                lbusIbsDetail.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                lbusIbsDetail.icdoIbsDetail.LoadData(adrRow);
                lbusIbsDetail.ibusPlan.icdoPlan.LoadData(adrRow);
                lbusIbsDetail.ibusPersonAccount.icdoPersonAccount.LoadData(adrRow);
                lbusIbsDetail.ibusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.LoadData(adrRow);
                lbusIbsDetail.ibusPerson.icdoPerson.LoadData(adrRow);
                icolIbsDetail.Add(lbusIbsDetail);
                _idecPremiumAmountTotal += lbusIbsDetail.icdoIbsDetail.total_premium_amount;
            }
        }

        public void LoadTotalRecordCountAndTotalPremium()
        {
            DataTable ldtbList = Select("cdoIbsHeader.LoadTotalCountAndPremium", new object[1] { icdoIbsHeader.ibs_header_id });
            if (ldtbList.Rows.Count > 0)
            {
                _idecPremiumAmountTotal = Convert.ToDecimal(ldtbList.Rows[0]["total_premium_amount"]);
                icdoIbsHeader.total_record_count = Convert.ToInt32(ldtbList.Rows[0]["total_record_count"]);
                icdoIbsHeader.total_premium_amount = idecPremiumAmountTotal;
            }
        }

        public void CreateJSRHICBill()
        {
            if (ibusJsRhicBill == null)
            {
                ibusJsRhicBill = new busJsRhicBill();
                ibusJsRhicBill.icdoJsRhicBill = new cdoJsRhicBill();
            }
            ibusJsRhicBill.icdoJsRhicBill.org_id = busOrganization.GetJSPlanOrgID(iobjPassInfo);
            ibusJsRhicBill.icdoJsRhicBill.bill_date = icdoIbsHeader.billing_month_and_year;
            ibusJsRhicBill.icdoJsRhicBill.allocated_amount = 0.00M;
            ibusJsRhicBill.icdoJsRhicBill.bill_amount = 0.00M;
            ibusJsRhicBill.icdoJsRhicBill.Insert();
        }

        public void LoadBill()
        {
            if (_ibusJsRhicBill == null)
                _ibusJsRhicBill = new busJsRhicBill();

            if (_ibusJsRhicBill.FindJsRhicBill(icdoIbsHeader.js_rhic_bill_id))
            {
                _ibusJsRhicBill.istrOrgCode = busGlobalFunctions.GetOrgCodeFromOrgId(_ibusJsRhicBill.icdoJsRhicBill.org_id);
            }
            else
            {
                //Sometimes we have bill id in IBS header but not in JSRHIC Bill table. Such cases, we get the id value in find method that causes some problem.
                _ibusJsRhicBill.icdoJsRhicBill = new cdoJsRhicBill();
            }

        }

        public void btnReadyToPost_Click()
        {
            if (_icdoIbsHeader.report_type_value == busConstant.IBSHeaderReportTypeAdjustment)
            {
                _icdoIbsHeader.report_status_value = busConstant.IBSHeaderStatusReadyPost;
                _icdoIbsHeader.Update();
            }
        }

        public void CreateAdjustmentIBSHeader()
        {
            if (icdoIbsHeader == null)
                icdoIbsHeader = new cdoIbsHeader();
            icdoIbsHeader.billing_month_and_year =
                Convert.ToDateTime(DateTime.Now.ToString("MM/yyyy", CultureInfo.CreateSpecificCulture("en-US")));
            icdoIbsHeader.report_status_value = busConstant.IBSHeaderStatusReview;
            icdoIbsHeader.report_type_value = busConstant.IBSHeaderReportTypeAdjustment;
            icdoIbsHeader.total_record_count = 0;
            icdoIbsHeader.total_premium_amount = 0.00M;
            icdoIbsHeader.ienuObjectState = ObjectState.Insert;
            //ucs - 038 addendum workflow generation
            InitiateIBSAdjustmentHeaderWorkflow();
        }

        public bool LoadCurrentAdjustmentIBSHeader()
        {
            if (icdoIbsHeader == null)
                icdoIbsHeader = new cdoIbsHeader();
            DataTable ldtbList = Select<cdoIbsHeader>(
                new string[2] { "report_status_value", "report_type_value" },
                new object[2] { busConstant.IBSHeaderStatusReview, busConstant.IBSHeaderReportTypeAdjustment }, null, "IBS_HEADER_ID DESC"); //PIR 15606 - latest header instead of any adjustment header in Review
            if (ldtbList.Rows.Count > 0)
            {
                icdoIbsHeader.LoadData(ldtbList.Rows[0]);
                return true;
            }
            else
                return false;
        }

        public bool LoadCurrentRegularIBSHeader()
        {
            if (icdoIbsHeader == null)
                icdoIbsHeader = new cdoIbsHeader();

            DataTable ldtbList = busNeoSpinBase.Select<cdoIbsHeader>(
                new string[2] { "billing_month_and_year", "report_type_value", },
                new object[2]
                    {
                        icdoIbsHeader.billing_month_and_year, busConstant.IBSHeaderReportTypeRegular
                    }, null, null);
            if (ldtbList.Rows.Count > 0)
            {
                icdoIbsHeader.LoadData(ldtbList.Rows[0]);
                return true;
            }
            return false;
        }

        public void CreateIBSRegularHeader()
        {
            icdoIbsHeader.report_status_value = busConstant.IBSHeaderStatusReview; //PIR 8164 & 8165
            icdoIbsHeader.report_type_value = busConstant.IBSHeaderReportTypeRegular;
            icdoIbsHeader.total_record_count = 0;
            icdoIbsHeader.total_premium_amount = 0.00M;
            icdoIbsHeader.Insert();
        }

        public string GetGroupHealthCoverageCodeDescription(int AintCoverageRefID)
        {
            if (AintCoverageRefID > 0)
            {
                DataTable ldtbCoverageCode = busNeoSpinBase.Select("cdoIbsHeader.GetCoverageCodeDescription",
                                                                                            new object[1] { AintCoverageRefID });
                if (ldtbCoverageCode.Rows.Count > 0)
                {
                    string lstrCoverageCodeDescription = ldtbCoverageCode.Rows[0]["CLIENT_DESCRIPTION"].ToString();
                    return lstrCoverageCodeDescription;
                }
            }
            return string.Empty;
        }
        public string GetGroupHealthCoverageCodeDescription(string astrCoverageCode, string astrRateStructureCode)
        {
            DataTable ldtbCoverageCode = busNeoSpinBase.Select("entIbsHeader.GetCoverageCodeDescByCoverageCodeNRateStructureCode",
                                                                                        new object[2] { astrCoverageCode, astrRateStructureCode });
            if (ldtbCoverageCode.Rows.Count > 0)
            {
                string lstrCoverageCodeDescription = ldtbCoverageCode.Rows[0]["CLIENT_DESCRIPTION"].ToString();
                return lstrCoverageCodeDescription;
            }

            return string.Empty;
        }
    
        public void AddIBSDetailForGHDV(int aintPersonAccountID, int aintPersonID, int aintPlanID, DateTime adatBillingDate,
            string astrPaymentMethod, string astrCoverageCode, decimal adecFeeAmount, decimal adecBuydownAmount, decimal adecMedicarePartDAmount,decimal adecMemberPremiumAmount,
            decimal adecRHICAmt, decimal adecOthrRHICAmt, decimal adecJSRHICAmount, decimal adecProviderPremiumAmt, decimal adecTotalPremiumAmt, int aintProviderOrgID,
            int aintGHDVHistoryID = 0, string astrGroupNumber = "", decimal adecPaidPremiumAmount = 0.0M, string astrCoverageCodeValue = "", string astrRateStructureCode = "", Collection<busIbsDetail> acolNegPosIBsDetail = null, int aintPayeeAccountId = 0)
        {
            /* UAT PIR 476, Including other and JS RHIC Amount */
            busIbsDetail lbusIbsDtl = CreateIBSDetailForGHDV(aintPersonAccountID, aintPersonID, aintPlanID,
                                                             adatBillingDate, astrPaymentMethod, astrCoverageCode,
                                                             adecFeeAmount, adecBuydownAmount, adecMedicarePartDAmount, adecMemberPremiumAmount, adecRHICAmt, adecOthrRHICAmt, adecJSRHICAmount,
                                                             adecProviderPremiumAmt, adecTotalPremiumAmt, aintProviderOrgID, aintGHDVHistoryID, astrGroupNumber, adecPaidPremiumAmount,
                                                             astrCoverageCodeValue, astrRateStructureCode, aintPayeeAccountId);//uat pir 1429 : post ghdv_history_id //prod pir 6076
            /* UAT PIR 476 ends here */
            if (icolIbsDetail != null)
                icolIbsDetail.Add(lbusIbsDtl);

			//IBS Adjustment creation logic changed (Affected area enroll to enroll scenario) - logged under Backlog PIR 8022 and 8111
            if (acolNegPosIBsDetail != null)
            {
                int MaxSrNo = 0;
                if(acolNegPosIBsDetail.Count > 0)
                    MaxSrNo = acolNegPosIBsDetail.Max(i => i.SrNo);
                lbusIbsDtl.SrNo = MaxSrNo + 1;
                acolNegPosIBsDetail.Add(lbusIbsDtl);
            }
        }

        public void AddIBSDetailForMedicare(int aintPersonAccountID, int aintPersonID, int aintPlanID, DateTime adatBillingDate,
            string astrPaymentMethod, decimal adecMemberPremiumAmount, decimal adecTotalPremiumAmt, int aintProviderOrgID,decimal adecLowIncomeCredit, decimal adecLateEnrollmentPenalty,
            decimal adecMemberPremiumAmountFromRef, Collection<busIbsDetail> acolNegPosIBsDetail = null, int aintPayeeAccountId = 0)
        {
            busIbsDetail lbusIbsDtl = CreateIBSDetailForMedicarePartD(aintPersonAccountID, aintPersonID, aintPlanID, adatBillingDate,
                                                                        astrPaymentMethod, adecMemberPremiumAmount, adecTotalPremiumAmt, aintProviderOrgID,
                                                                        adecLowIncomeCredit, adecLateEnrollmentPenalty, adecMemberPremiumAmountFromRef, aintPayeeAccountId);//PIR 15786
            if (icolIbsDetail != null)
                icolIbsDetail.Add(lbusIbsDtl);

            if (acolNegPosIBsDetail != null)
            {
                int MaxSrNo = 0;
                if (acolNegPosIBsDetail.Count > 0)
                    MaxSrNo = acolNegPosIBsDetail.Max(i => i.SrNo);
                lbusIbsDtl.SrNo = MaxSrNo + 1;
                acolNegPosIBsDetail.Add(lbusIbsDtl);
            }
        }

        public void AddIBSDetailForLife(int aintPersonAccountID, int aintPersonID, int aintPlanID, DateTime adatBillingDate,
           string astrPaymentMethod, decimal adecMemberPremiumAmount, decimal adecProviderPremiumAmt, decimal adecTotalPremiumAmt,
           decimal adecLifeBasicPremiumAmt, decimal adecLifeSuppPremiumAmt, decimal adecLifeSpouseSuppPremiumAmt, decimal adecDepSuppPremiumAmt,
            decimal adecADAndDBasicRate, decimal adecADAndDSuppRate, decimal adecBasicCoverageAmount, decimal adecSuppCoverageAmount, decimal adecSpouseSuppCoverageAmount,
            decimal adecDepSuppCoverageAmount, int aintProviderOrgID, decimal adecPaidPremiumAmount = 0.0M, Collection<busIbsDetail> acolNegPosIBsDetail = null, int aintPayeeAccountId = 0)//ucs - 038 addendum. new col. for paid premium amount
        {

            busIbsDetail lbusIbsDtl = CreateIBSDetailForLife(aintPersonAccountID, aintPersonID, aintPlanID,
                                                            adatBillingDate, astrPaymentMethod, adecMemberPremiumAmount,
                                                            adecProviderPremiumAmt, adecTotalPremiumAmt, adecLifeBasicPremiumAmt,
                                                            adecLifeSuppPremiumAmt, adecLifeSpouseSuppPremiumAmt, adecDepSuppPremiumAmt,
                                                            adecADAndDBasicRate, adecADAndDSuppRate, adecBasicCoverageAmount, adecSuppCoverageAmount,
                                                            adecSpouseSuppCoverageAmount, adecDepSuppCoverageAmount, aintProviderOrgID, adecPaidPremiumAmount, aintPayeeAccountId);

            if (icolIbsDetail != null)
                icolIbsDetail.Add(lbusIbsDtl);

            if (acolNegPosIBsDetail != null)
            {
                int MaxSrNo = 0;
                if (acolNegPosIBsDetail.Count > 0)
                    MaxSrNo = acolNegPosIBsDetail.Max(i => i.SrNo);
                lbusIbsDtl.SrNo = MaxSrNo + 1;
                acolNegPosIBsDetail.Add(lbusIbsDtl);
            }
        }

        public void AddIBSDetailForLTC(int aintPersonAccountID, int aintPersonID, int aintPlanID, DateTime adatBillingDate,
            string astrPaymentMethod, decimal adecMemberPremiumAmount,
            decimal adecLtcMemberThreeYrsPremium, decimal adecLtcMemberFiveYrsPremium, decimal adecLtcSpouseThreeYrsPremium, decimal adecLtcSpouseFiveYrsPremium,
            int aintProviderOrgID, decimal adecPaidPremiumAmount = 0.0M)//ucs - 038 addendum. new col. for paid premium amount
        {
            busIbsDetail lbusIbsDtl = CreateIBSDetailForLTC(aintPersonAccountID, aintPersonID, aintPlanID,
                                                            adatBillingDate, astrPaymentMethod,
                                                            adecMemberPremiumAmount, adecLtcMemberThreeYrsPremium,
                                                            adecLtcMemberFiveYrsPremium, adecLtcSpouseThreeYrsPremium,
                                                            adecLtcSpouseFiveYrsPremium, aintProviderOrgID, adecPaidPremiumAmount);

            if (icolIbsDetail != null)
                icolIbsDetail.Add(lbusIbsDtl);
        }

        public busIbsDetail CreateIBSDetailForGHDV(int aintPersonAccountID, int aintPersonID, int aintPlanID, DateTime adatBillingDate,
            string astrPaymentMethod, string astrCoverageCode,
            decimal adecFeeAmount, decimal adecBuydownAmount, decimal adecMedicarePartDAmount, decimal adecMemberPremiumAmount, decimal adecRHICAmt,
            decimal adecOthrRHICAmount, decimal adecJSRHICAmount, decimal adecProviderPremiumAmt, decimal adecTotalPremiumAmt, int aintProviderOrgID,
            int aintGHDVHistoryID = 0, string astrGroupNumber = "", decimal adecPaidPremiumAmount = 0.0M, string astrCoverageCodeValue = "", string astrRateStructureCode = "", int aintPayeeAccountId = 0)
        {
            busIbsDetail lbusIbsDtl = new busIbsDetail();
            lbusIbsDtl.icdoIbsDetail = new cdoIbsDetail();
            lbusIbsDtl.icdoIbsDetail.person_account_id = aintPersonAccountID;
            lbusIbsDtl.icdoIbsDetail.person_id = aintPersonID;
            lbusIbsDtl.icdoIbsDetail.plan_id = aintPlanID;
            lbusIbsDtl.icdoIbsDetail.billing_month_and_year = adatBillingDate;
            lbusIbsDtl.icdoIbsDetail.mode_of_payment_value = astrPaymentMethod;
            lbusIbsDtl.icdoIbsDetail.ibs_header_id = icdoIbsHeader.ibs_header_id;

            lbusIbsDtl.icdoIbsDetail.coverage_code = astrCoverageCode;
            lbusIbsDtl.icdoIbsDetail.member_premium_amount = adecMemberPremiumAmount;
            lbusIbsDtl.icdoIbsDetail.group_health_fee_amt = adecFeeAmount;
            lbusIbsDtl.icdoIbsDetail.rhic_amount = adecRHICAmt;
            /* PIR 476, Including other and JS RHIC Amount */
            lbusIbsDtl.icdoIbsDetail.othr_rhic_amount = adecOthrRHICAmount;
            lbusIbsDtl.icdoIbsDetail.js_rhic_amount = adecJSRHICAmount;
            /* PIR 476 ends here */
            lbusIbsDtl.icdoIbsDetail.provider_premium_amount = adecProviderPremiumAmt;
            lbusIbsDtl.icdoIbsDetail.total_premium_amount = adecTotalPremiumAmt;
            //uat pir 1429 :- to insert GHDV history id and group number
            //prod pir 6076 & 6077 - Removal of person account ghdv history id
            //lbusIbsDtl.icdoIbsDetail.person_account_ghdv_history_id = aintGHDVHistoryID;
            lbusIbsDtl.icdoIbsDetail.group_number = astrGroupNumber;
            //ucs - 38 addendum, new col. paid premium amount
            lbusIbsDtl.icdoIbsDetail.paid_premium_amount = adecPaidPremiumAmount;
            lbusIbsDtl.icdoIbsDetail.provider_org_id = aintProviderOrgID;
            //prod pir 6076
            lbusIbsDtl.icdoIbsDetail.coverage_code_value = astrCoverageCodeValue;
            lbusIbsDtl.icdoIbsDetail.rate_structure_code = astrRateStructureCode;

            lbusIbsDtl.icdoIbsDetail.buydown_amount = adecBuydownAmount;
            lbusIbsDtl.icdoIbsDetail.medicare_part_d_amt = adecMedicarePartDAmount;
            lbusIbsDtl.icdoIbsDetail.detail_status_value = busConstant.IBSHeaderStatusReview; // PIR 8164 & 8165 Ignore functionality on detail
            lbusIbsDtl.icdoIbsDetail.payee_account_id_papit = aintPayeeAccountId; //PIR 8022

            lbusIbsDtl.icdoIbsDetail.ienuObjectState = ObjectState.Insert;
            return lbusIbsDtl;
        }

        public busIbsDetail CreateIBSDetailForMedicarePartD(int aintPersonAccountID, int aintPersonID, int aintPlanID, DateTime adatBillingDate,
            string astrPaymentMethod, decimal adecMemberPremiumAmount, decimal adecTotalPremiumAmount, int aintProviderOrgID, decimal adecLowIncomeCredit, decimal adecLateEnrollmentPenalty,
            decimal adecMemberPremiumAmountFromRef, int aintPayeeAccountId = 0)
        {
            busIbsDetail lbusIbsDtl = new busIbsDetail();
            lbusIbsDtl.icdoIbsDetail = new cdoIbsDetail();

            lbusIbsDtl.icdoIbsDetail.ibs_header_id = icdoIbsHeader.ibs_header_id;
            lbusIbsDtl.icdoIbsDetail.person_id = aintPersonID;
            lbusIbsDtl.icdoIbsDetail.plan_id = aintPlanID;
            lbusIbsDtl.icdoIbsDetail.billing_month_and_year = adatBillingDate;
            lbusIbsDtl.icdoIbsDetail.mode_of_payment_value = astrPaymentMethod;
            lbusIbsDtl.icdoIbsDetail.member_premium_amount = adecMemberPremiumAmount;
            lbusIbsDtl.icdoIbsDetail.total_premium_amount = adecTotalPremiumAmount;
            lbusIbsDtl.icdoIbsDetail.person_account_id = aintPersonAccountID;
            lbusIbsDtl.icdoIbsDetail.provider_org_id = aintProviderOrgID;
            lbusIbsDtl.icdoIbsDetail.detail_status_value = busConstant.IBSHeaderStatusReview; // PIR 8164 & 8165 Ignore functionality on detail

            //PIR 15786
            lbusIbsDtl.icdoIbsDetail.provider_premium_amount = adecMemberPremiumAmountFromRef;
            lbusIbsDtl.icdoIbsDetail.lis_amount = adecLowIncomeCredit;
            lbusIbsDtl.icdoIbsDetail.lep_amount = adecLateEnrollmentPenalty;
            lbusIbsDtl.icdoIbsDetail.payee_account_id_papit = aintPayeeAccountId; //PIR 8022
            
            lbusIbsDtl.icdoIbsDetail.ienuObjectState = ObjectState.Insert;
            return lbusIbsDtl;
        }

        public busIbsDetail CreateIBSDetailForLife(int aintPersonAccountID, int aintPersonID, int aintPlanID, DateTime adatBillingDate,
            string astrPaymentMethod, decimal adecMemberPremiumAmount, decimal adecProviderPremiumAmt, decimal adecTotalPremiumAmt,
            decimal adecLifeBasicPremiumAmt, decimal adecLifeSuppPremiumAmt, decimal adecLifeSpouseSuppPremiumAmt, decimal adecDepSuppPremiumAmt,
            decimal adecADAndDBasicRate, decimal adecADAndDSuppRate, decimal adecBasicCoverageAmount, decimal adecSuppCoverageAmount,
            decimal adecSpouSuppCoverageAmount, decimal adecDepSuppCoverageAmount, int aintProviderOrgID,
            decimal adecPaidPremiumAmount = 0.0M, int aintPayeeAccountId = 0)//ucs - 038 addendum : added new col. paid premium amount
        {
            busIbsDetail lbusIbsDtl = new busIbsDetail();
            lbusIbsDtl.icdoIbsDetail = new cdoIbsDetail();
            lbusIbsDtl.icdoIbsDetail.person_account_id = aintPersonAccountID;
            lbusIbsDtl.icdoIbsDetail.person_id = aintPersonID;
            lbusIbsDtl.icdoIbsDetail.plan_id = aintPlanID;
            lbusIbsDtl.icdoIbsDetail.billing_month_and_year = adatBillingDate;
            lbusIbsDtl.icdoIbsDetail.mode_of_payment_value = astrPaymentMethod;
            lbusIbsDtl.icdoIbsDetail.ibs_header_id = icdoIbsHeader.ibs_header_id;

            lbusIbsDtl.icdoIbsDetail.member_premium_amount = adecMemberPremiumAmount;
            lbusIbsDtl.icdoIbsDetail.provider_premium_amount = adecProviderPremiumAmt;
            lbusIbsDtl.icdoIbsDetail.total_premium_amount = adecTotalPremiumAmt;
            lbusIbsDtl.icdoIbsDetail.life_basic_premium_amount = adecLifeBasicPremiumAmt;
            lbusIbsDtl.icdoIbsDetail.life_supp_premium_amount = adecLifeSuppPremiumAmt;
            lbusIbsDtl.icdoIbsDetail.life_spouse_supp_premium_amount = adecLifeSpouseSuppPremiumAmt;
            lbusIbsDtl.icdoIbsDetail.life_dep_supp_premium_amount = adecDepSuppPremiumAmt;
            //ucs - 38 addendum, new col. paid premium amount
            lbusIbsDtl.icdoIbsDetail.paid_premium_amount = adecPaidPremiumAmount;
            //prod pir 4260
            lbusIbsDtl.icdoIbsDetail.ad_and_d_basic_premium_rate = adecADAndDBasicRate;
            lbusIbsDtl.icdoIbsDetail.ad_and_d_supplemental_premium_rate = adecADAndDSuppRate;
            lbusIbsDtl.icdoIbsDetail.life_basic_coverage_amount = adecBasicCoverageAmount;
            lbusIbsDtl.icdoIbsDetail.life_spouse_supp_coverage_amount = adecSpouSuppCoverageAmount;
            lbusIbsDtl.icdoIbsDetail.life_supp_coverage_amount = adecSuppCoverageAmount;
            lbusIbsDtl.icdoIbsDetail.life_dep_supp_coverage_amount = adecDepSuppCoverageAmount;
            lbusIbsDtl.icdoIbsDetail.provider_org_id = aintProviderOrgID;
            lbusIbsDtl.icdoIbsDetail.detail_status_value = busConstant.IBSHeaderStatusReview; // PIR 8164 & 8165 Ignore functionality on detail
            lbusIbsDtl.icdoIbsDetail.payee_account_id_papit = aintPayeeAccountId; //PIR 8022

            lbusIbsDtl.icdoIbsDetail.ienuObjectState = ObjectState.Insert;
            return lbusIbsDtl;
        }

        public busIbsDetail CreateIBSDetailForLTC(int aintPersonAccountID, int aintPersonID, int aintPlanID, DateTime adatBillingDate,
            string astrPaymentMethod, decimal adecMemberPremiumAmount,
            decimal adecLtcMemberThreeYrsPremium, decimal adecLtcMemberFiveYrsPremium, decimal adecLtcSpouseThreeYrsPremium, decimal adecLtcSpouseFiveYrsPremium,
            int aintProviderOrgID, decimal adecPaidPremiumAmount = 0.0M)//ucs - 038 addendum : added new col. paid premium amount
        {
            busIbsDetail lbusIbsDtl = new busIbsDetail();
            lbusIbsDtl.icdoIbsDetail = new cdoIbsDetail();
            lbusIbsDtl.icdoIbsDetail.person_account_id = aintPersonAccountID;
            lbusIbsDtl.icdoIbsDetail.person_id = aintPersonID;
            lbusIbsDtl.icdoIbsDetail.plan_id = aintPlanID;
            lbusIbsDtl.icdoIbsDetail.billing_month_and_year = adatBillingDate;
            lbusIbsDtl.icdoIbsDetail.mode_of_payment_value = astrPaymentMethod;
            lbusIbsDtl.icdoIbsDetail.ibs_header_id = icdoIbsHeader.ibs_header_id;

            lbusIbsDtl.icdoIbsDetail.member_premium_amount = adecMemberPremiumAmount;
            lbusIbsDtl.icdoIbsDetail.provider_premium_amount = adecMemberPremiumAmount;
            lbusIbsDtl.icdoIbsDetail.total_premium_amount = adecMemberPremiumAmount;
            lbusIbsDtl.icdoIbsDetail.ltc_member_three_yrs_premium_amount = adecLtcMemberThreeYrsPremium;
            lbusIbsDtl.icdoIbsDetail.ltc_member_five_yrs_premium_amount = adecLtcMemberFiveYrsPremium;
            lbusIbsDtl.icdoIbsDetail.ltc_spouse_three_yrs_premium_amount = adecLtcSpouseThreeYrsPremium;
            lbusIbsDtl.icdoIbsDetail.ltc_spouse_five_yrs_premium_amount = adecLtcSpouseFiveYrsPremium;
            //ucs - 38 addendum, new col. paid premium amount
            lbusIbsDtl.icdoIbsDetail.paid_premium_amount = adecPaidPremiumAmount;
            lbusIbsDtl.icdoIbsDetail.provider_org_id = aintProviderOrgID;
            lbusIbsDtl.icdoIbsDetail.detail_status_value = busConstant.IBSHeaderStatusReview; // PIR 8164 & 8165 Ignore functionality on detail

            lbusIbsDtl.icdoIbsDetail.ienuObjectState = ObjectState.Insert;
            return lbusIbsDtl;
        }

        public void UpdateSummaryData(string astrStatus)
        {
            if (_icolIbsDetail == null)
                LoadIbsDetails();

            decimal ldecTotal = 0;
            decimal ldecTotalJSRHIC = 0;
            foreach (busIbsDetail lbusDtl in _icolIbsDetail)
            {
                ldecTotal += lbusDtl.icdoIbsDetail.total_premium_amount;
                ldecTotalJSRHIC += lbusDtl.icdoIbsDetail.js_rhic_amount;
            }

            if (ibusJsRhicBill == null)
                LoadBill();
            if (ibusJsRhicBill.icdoJsRhicBill.js_rhic_bill_id > 0)
            {
                ibusJsRhicBill.icdoJsRhicBill.ienuObjectState = ObjectState.Update;
            }
            else
            {
                ibusJsRhicBill = new busJsRhicBill();
                ibusJsRhicBill.icdoJsRhicBill = new cdoJsRhicBill();
                ibusJsRhicBill.icdoJsRhicBill.ienuObjectState = ObjectState.Insert;
            }
            ibusJsRhicBill.icdoJsRhicBill.org_id = busOrganization.GetJSPlanOrgID(iobjPassInfo); //?? This should be retireved form membes last employer under JS plan
            ibusJsRhicBill.icdoJsRhicBill.bill_date = DateTime.Now;
            ibusJsRhicBill.icdoJsRhicBill.allocated_amount = 0.00M;
            ibusJsRhicBill.icdoJsRhicBill.bill_amount = ldecTotalJSRHIC;
            ibusJsRhicBill.UpdateDataObject(ibusJsRhicBill.icdoJsRhicBill);

            icdoIbsHeader.total_record_count = icolIbsDetail.Count;
            icdoIbsHeader.total_premium_amount = ldecTotal;
            icdoIbsHeader.report_status_value = astrStatus;
            icdoIbsHeader.js_rhic_bill_id = ibusJsRhicBill.icdoJsRhicBill.js_rhic_bill_id;
            if (icdoIbsHeader.report_status_value == busConstant.IBSHeaderStatusPosted)
            {
                icdoIbsHeader.run_date = busGlobalFunctions.GetSysManagementBatchDate().Date.AddDays(1).AddMinutes(-1);
                UpdateIBSDetailStatus(astrStatus); // PIR 8164 & 8165 Ignore functionality on detail
            }
            UpdateDataObject(icdoIbsHeader);
        }

        public bool AllocateJSRHICRemittance()
        {
            bool lblnRemittanceFound = false;
            if (ibusJsRhicRemittanceAllocation == null)
            {
                ibusJsRhicRemittanceAllocation = new busJsRhicRemittanceAllocation();
                ibusJsRhicRemittanceAllocation.icdoJsRhicRemittanceAllocation = new cdoJsRhicRemittanceAllocation();
            }
            DataTable ldtbAvlRemittance = busNeoSpinBase.Select("cdoJsRhicRemittanceAllocation.GetJSRHICRemittanceAllocationAmount", new object[0]);
            foreach (DataRow dr in ldtbAvlRemittance.Rows)
            {
                if (Convert.ToDecimal(dr["Available_Amount"]) == ibusJsRhicBill.icdoJsRhicBill.bill_amount)
                {
                    ibusJsRhicRemittanceAllocation.icdoJsRhicRemittanceAllocation.allocated_amount = Convert.ToDecimal(dr["Available_Amount"]);
                    ibusJsRhicRemittanceAllocation.icdoJsRhicRemittanceAllocation.remittance_id = Convert.ToInt32(dr["Remittance_id"]);
                    lblnRemittanceFound = true;
                }
            }
            if (lblnRemittanceFound)
            {
                ibusJsRhicRemittanceAllocation.icdoJsRhicRemittanceAllocation.ibs_header_id = icdoIbsHeader.ibs_header_id;
                ibusJsRhicRemittanceAllocation.icdoJsRhicRemittanceAllocation.js_rhic_bill_id = ibusJsRhicBill.icdoJsRhicBill.js_rhic_bill_id;
                ibusJsRhicRemittanceAllocation.icdoJsRhicRemittanceAllocation.rhic_allocation_status_value = "ALTD";
                ibusJsRhicRemittanceAllocation.icdoJsRhicRemittanceAllocation.Insert();

                //Update the Allocated Amount
                ibusJsRhicBill.icdoJsRhicBill.allocated_amount = ibusJsRhicRemittanceAllocation.icdoJsRhicRemittanceAllocation.allocated_amount;
                ibusJsRhicBill.icdoJsRhicBill.Update();
            }
            return lblnRemittanceFound;
        }

        public void GenerateGL()
        {
            if (icolIbsDetail == null)
                LoadIbsDetails();

            foreach (busIbsDetail lobjIBSdetail in icolIbsDetail)
            {
                if (lobjIBSdetail.icdoIbsDetail.plan_id == busConstant.PlanIdDental)
                {
                    if (lobjIBSdetail.icdoIbsDetail.member_premium_amount > 0.00M)
                    {
                        GenerateGLByType(lobjIBSdetail.icdoIbsDetail, busConstant.ItemTypeGroupDentalPremium, lobjIBSdetail.icdoIbsDetail.member_premium_amount);
                    }
                    else if (lobjIBSdetail.icdoIbsDetail.member_premium_amount < 0.00M)
                    {
                        GenerateReverseGLByType(lobjIBSdetail.icdoIbsDetail, busConstant.ItemTypeGroupDentalPremium, lobjIBSdetail.icdoIbsDetail.member_premium_amount);
                    }
                }
                if (lobjIBSdetail.icdoIbsDetail.plan_id == busConstant.PlanIdVision)
                {
                    if (lobjIBSdetail.icdoIbsDetail.member_premium_amount > 0.00M)
                    {
                        GenerateGLByType(lobjIBSdetail.icdoIbsDetail, busConstant.ItemTypeGroupVisionPremium, lobjIBSdetail.icdoIbsDetail.member_premium_amount);
                    }
                    else if (lobjIBSdetail.icdoIbsDetail.member_premium_amount < 0.00M)
                    {
                        GenerateReverseGLByType(lobjIBSdetail.icdoIbsDetail, busConstant.ItemTypeGroupVisionPremium, lobjIBSdetail.icdoIbsDetail.member_premium_amount);
                    }

                }
                if (lobjIBSdetail.icdoIbsDetail.plan_id == busConstant.PlanIdGroupHealth)
                {
                    if (lobjIBSdetail.icdoIbsDetail.member_premium_amount > 0.00M)
                    {
                        //Exclude the Health Fee Amount
                        GenerateGLByType(lobjIBSdetail.icdoIbsDetail, busConstant.ItemTypeGroupHealthPremium, lobjIBSdetail.icdoIbsDetail.member_premium_amount);
                    }
                    else if (lobjIBSdetail.icdoIbsDetail.member_premium_amount < 0.00M)
                    {
                        GenerateReverseGLByType(lobjIBSdetail.icdoIbsDetail, busConstant.ItemTypeGroupHealthPremium, lobjIBSdetail.icdoIbsDetail.member_premium_amount);
                    }
                    if (lobjIBSdetail.icdoIbsDetail.group_health_fee_amt > 0.00M)
                    {
                        GenerateGLByType(lobjIBSdetail.icdoIbsDetail, busConstant.ItemTypeHealthAdminFee, lobjIBSdetail.icdoIbsDetail.group_health_fee_amt);
                    }
                    else if (lobjIBSdetail.icdoIbsDetail.group_health_fee_amt < 0.00M)
                    {
                        GenerateReverseGLByType(lobjIBSdetail.icdoIbsDetail, busConstant.ItemTypeHealthAdminFee, lobjIBSdetail.icdoIbsDetail.group_health_fee_amt);
                    }

                    // PIR 11239 - add GL for buydown
                    if (lobjIBSdetail.icdoIbsDetail.buydown_amount > 0.00M)
                    {
                        GenerateGLByType(lobjIBSdetail.icdoIbsDetail, busConstant.ItemTypeBuydownAmount, lobjIBSdetail.icdoIbsDetail.buydown_amount);
                    }
                    else if (lobjIBSdetail.icdoIbsDetail.buydown_amount < 0.00M)
                    {
                        GenerateReverseGLByType(lobjIBSdetail.icdoIbsDetail, busConstant.ItemTypeBuydownAmount, lobjIBSdetail.icdoIbsDetail.buydown_amount);
                    }

                    //PIR 14529 - GL for Medicare Part D
                    if (lobjIBSdetail.icdoIbsDetail.medicare_part_d_amt > 0.00M)
                    {
                        GenerateGLByType(lobjIBSdetail.icdoIbsDetail, busConstant.ItemTypeMedicarePartDAmount, lobjIBSdetail.icdoIbsDetail.medicare_part_d_amt);
                    }

                    else if (lobjIBSdetail.icdoIbsDetail.medicare_part_d_amt < 0.00M)
                    {
                        GenerateReverseGLByType(lobjIBSdetail.icdoIbsDetail, busConstant.ItemTypeMedicarePartDAmount, lobjIBSdetail.icdoIbsDetail.medicare_part_d_amt);
                    }

                    if (lobjIBSdetail.icdoIbsDetail.othr_rhic_amount > 0.00M) //pir 8573
                    {
                        GenerateGLByType(lobjIBSdetail.icdoIbsDetail, busConstant.ItemTypeRHICAmount, lobjIBSdetail.icdoIbsDetail.othr_rhic_amount); //pir 8573
                    }
                    else if (lobjIBSdetail.icdoIbsDetail.othr_rhic_amount < 0.00M) //pir 8573
                    {
                        GenerateReverseGLByType(lobjIBSdetail.icdoIbsDetail, busConstant.ItemTypeRHICNegative, lobjIBSdetail.icdoIbsDetail.othr_rhic_amount); //pir 8573
                    }
                    if (lobjIBSdetail.icdoIbsDetail.js_rhic_amount > 0.00M)
                    {
                        GenerateGLByType(lobjIBSdetail.icdoIbsDetail, busConstant.ItemTypeJobSeriveHealthCredit, lobjIBSdetail.icdoIbsDetail.js_rhic_amount);
                    }
                    else if (lobjIBSdetail.icdoIbsDetail.js_rhic_amount < 0.00M)
                    {
                        GenerateReverseGLByType(lobjIBSdetail.icdoIbsDetail, busConstant.ItemTypeJobSeriveHealthCredit, lobjIBSdetail.icdoIbsDetail.js_rhic_amount);
                    }
                }
                if (lobjIBSdetail.icdoIbsDetail.plan_id == busConstant.PlanIdGroupLife)
                {
                    if (lobjIBSdetail.icdoIbsDetail.member_premium_amount > 0.00M)
                    {
                        GenerateGLByType(lobjIBSdetail.icdoIbsDetail, busConstant.ItemTypeGroupLifePremium, lobjIBSdetail.icdoIbsDetail.member_premium_amount);
                    }
                    else if (lobjIBSdetail.icdoIbsDetail.member_premium_amount < 0.00M)
                    {
                        GenerateReverseGLByType(lobjIBSdetail.icdoIbsDetail, busConstant.ItemTypeGroupLifePremium, lobjIBSdetail.icdoIbsDetail.member_premium_amount);
                    }
                }
                if (lobjIBSdetail.icdoIbsDetail.plan_id == busConstant.PlanIdMedicarePartD)
                {
                    if (lobjIBSdetail.icdoIbsDetail.member_premium_amount > 0.00M)
                    {
                        GenerateGLByType(lobjIBSdetail.icdoIbsDetail, busConstant.ItemTypeMedicarePremium, lobjIBSdetail.icdoIbsDetail.member_premium_amount);
                    }
                    else if (lobjIBSdetail.icdoIbsDetail.member_premium_amount < 0.00M)
                    {
                        GenerateReverseGLByType(lobjIBSdetail.icdoIbsDetail, busConstant.ItemTypeMedicarePremium, lobjIBSdetail.icdoIbsDetail.member_premium_amount);
                    }
                }

            }
        }

        private void GenerateGLByType(cdoIbsDetail aobjCdoIbsDetail, string astrItemType, decimal adecGLAmount)
        {
            if (idtGLPostingDate == DateTime.MinValue)
                idtGLPostingDate = busGlobalFunctions.GetSysManagementBatchDate();
            cdoAccountReference lcdoAcccountReference = new cdoAccountReference();
            lcdoAcccountReference.plan_id = aobjCdoIbsDetail.plan_id;
            lcdoAcccountReference.source_type_value = busConstant.SourceTypeIBS;
            lcdoAcccountReference.transaction_type_value = busConstant.TransactionTypeItemLevel;
            lcdoAcccountReference.item_type_value = astrItemType;
            busGLHelper.GenerateGL(lcdoAcccountReference, aobjCdoIbsDetail.person_id, 0,
                                              aobjCdoIbsDetail.ibs_header_id,
                                              adecGLAmount,
                                              icdoIbsHeader.billing_month_and_year,
                                              idtGLPostingDate, iobjPassInfo);

            //Generate Fund Transfer GL only for RHIC
            if (astrItemType == busConstant.ItemTypeRHICAmount)
            {
                cdoAccountReference lcdoAcccountReferenceTransfer = new cdoAccountReference();
                lcdoAcccountReferenceTransfer.plan_id = aobjCdoIbsDetail.plan_id;
                lcdoAcccountReferenceTransfer.source_type_value = busConstant.SourceTypeIBS;
                lcdoAcccountReferenceTransfer.transaction_type_value = busConstant.TransactionTypeTransfer;
                lcdoAcccountReferenceTransfer.status_transition_value = string.Empty;
                lcdoAcccountReferenceTransfer.item_type_value = astrItemType;
                busGLHelper.GenerateGL(lcdoAcccountReferenceTransfer, aobjCdoIbsDetail.person_id, 0,
                                                  aobjCdoIbsDetail.ibs_header_id,
                                                  adecGLAmount,
                                                  icdoIbsHeader.billing_month_and_year,
                                                  idtGLPostingDate, iobjPassInfo);
            }

        }

        private void GenerateReverseGLByType(cdoIbsDetail aobjCdoIbsDetail, string astrItemType, decimal adecGLAmount)
        {
            if (idtGLPostingDate == DateTime.MinValue)
                idtGLPostingDate = busGlobalFunctions.GetSysManagementBatchDate();
            cdoAccountReference lcdoAcccountReference = new cdoAccountReference();
            lcdoAcccountReference.plan_id = aobjCdoIbsDetail.plan_id;
            lcdoAcccountReference.source_type_value = busConstant.SourceTypeIBS;
            lcdoAcccountReference.transaction_type_value = busConstant.TransactionTypeAllocation;            
            lcdoAcccountReference.from_item_type_value = astrItemType; // PROD PIR ID 5144
            busGLHelper.GenerateGL(lcdoAcccountReference, aobjCdoIbsDetail.person_id, 0,
                                              aobjCdoIbsDetail.ibs_header_id,
                                              adecGLAmount * -1M,
                                              icdoIbsHeader.billing_month_and_year,
                                              idtGLPostingDate, iobjPassInfo);

            //Generate Fund Transfer GL only for RHIC
            if (astrItemType == busConstant.ItemTypeRHICNegative)
            {
                cdoAccountReference lcdoAcccountReferenceTransfer = new cdoAccountReference();
                lcdoAcccountReferenceTransfer.plan_id = aobjCdoIbsDetail.plan_id;
                lcdoAcccountReferenceTransfer.source_type_value = busConstant.SourceTypeIBS;
                lcdoAcccountReferenceTransfer.transaction_type_value = busConstant.TransactionTypeTransfer;
                lcdoAcccountReferenceTransfer.status_transition_value = string.Empty;
                lcdoAcccountReferenceTransfer.item_type_value = astrItemType;
                busGLHelper.GenerateGL(lcdoAcccountReferenceTransfer, aobjCdoIbsDetail.person_id, 0,
                                                  aobjCdoIbsDetail.ibs_header_id,
                                                  adecGLAmount * -1M,
                                                  icdoIbsHeader.billing_month_and_year,
                                                  idtGLPostingDate, iobjPassInfo);
            }

        }

        public void GenerateGLForJSRHICAllocation(int aintJSOrgID, string astrItemType, decimal adecGLAmount)
        {
            if (idtGLPostingDate == DateTime.MinValue)
                idtGLPostingDate = busGlobalFunctions.GetSysManagementBatchDate();
            cdoAccountReference lcdoAcccountReference = new cdoAccountReference();
            lcdoAcccountReference.plan_id = busConstant.PlanIdGroupHealth; ;
            lcdoAcccountReference.source_type_value = busConstant.SourceTypeIBS;
            lcdoAcccountReference.transaction_type_value = busConstant.TransactionTypeItemLevel;
            lcdoAcccountReference.item_type_value = astrItemType;
            busGLHelper.GenerateGL(lcdoAcccountReference, 0, aintJSOrgID,
                                              icdoIbsHeader.ibs_header_id,
                                              adecGLAmount,
                                              icdoIbsHeader.billing_month_and_year,
                                              idtGLPostingDate, iobjPassInfo);
        }

        public void GenerateGLForReverseJSRHICAllocation(int aintJSOrgID, string astrItemType, decimal adecGLAmount)
        {
            if (idtGLPostingDate == DateTime.MinValue)
                idtGLPostingDate = busGlobalFunctions.GetSysManagementBatchDate();
            cdoAccountReference lcdoAcccountReference = new cdoAccountReference();
            lcdoAcccountReference.plan_id = busConstant.PlanIdGroupHealth; ;
            lcdoAcccountReference.source_type_value = busConstant.SourceTypeIBS;
            lcdoAcccountReference.transaction_type_value = busConstant.TransactionTypeAllocation;
            lcdoAcccountReference.item_type_value = astrItemType;
            busGLHelper.GenerateGL(lcdoAcccountReference, 0, aintJSOrgID,
                                              icdoIbsHeader.ibs_header_id,
                                              adecGLAmount,
                                              icdoIbsHeader.billing_month_and_year,
                                              idtGLPostingDate, iobjPassInfo);
        }

        public void PostIBSContributionDetails(string astrTransactionType)
        {
            foreach (busIbsDetail lobjIBSdetail in icolIbsDetail)
            {
                cdoPersonAccountInsuranceContribution lobjInsrContribution = new cdoPersonAccountInsuranceContribution();
                lobjInsrContribution.person_account_id = lobjIBSdetail.icdoIbsDetail.person_account_id;
                lobjInsrContribution.due_premium_amount = lobjIBSdetail.icdoIbsDetail.member_premium_amount;
                lobjInsrContribution.group_health_fee_amt = lobjIBSdetail.icdoIbsDetail.group_health_fee_amt;
                lobjInsrContribution.buydown_amount = lobjIBSdetail.icdoIbsDetail.buydown_amount;
                lobjInsrContribution.medicare_part_d_amt = lobjIBSdetail.icdoIbsDetail.medicare_part_d_amt;
                lobjInsrContribution.rhic_benefit_amount = lobjIBSdetail.icdoIbsDetail.rhic_amount;
                /* PIR 476, Including other and JS RHIC Amount */
                lobjInsrContribution.js_rhic_amount = lobjIBSdetail.icdoIbsDetail.js_rhic_amount;
                lobjInsrContribution.othr_rhic_amount = lobjIBSdetail.icdoIbsDetail.othr_rhic_amount;
                /* PIR 476 ends here */
                lobjInsrContribution.subsystem_ref_id = lobjIBSdetail.icdoIbsDetail.ibs_detail_id;
                lobjInsrContribution.subsystem_value = busConstant.SubSystemValueIBSDeposit;
                lobjInsrContribution.transaction_date = DateTime.Today;
                //ucs 38- addendum
                //Negative adjustments should of type Regular IBS
                if (lobjIBSdetail.icdoIbsDetail.member_premium_amount < 0 && astrTransactionType == busConstant.TransactionTypeAdjustmentIBS)
                    lobjInsrContribution.transaction_type_value = busConstant.TransactionTypeRegularIBS;
                else
                    lobjInsrContribution.transaction_type_value = astrTransactionType;
                //ucs - 038 addendum
                //post paid permium amount from ibs detail into contribution table if transaction type is adjustment ibs
                if (astrTransactionType == busConstant.TransactionTypeAdjustmentIBS)
                    lobjInsrContribution.paid_premium_amount = lobjIBSdetail.icdoIbsDetail.paid_premium_amount;
                lobjInsrContribution.effective_date = lobjIBSdetail.icdoIbsDetail.billing_month_and_year;
                lobjInsrContribution.life_basic_premium_amount = lobjIBSdetail.icdoIbsDetail.life_basic_premium_amount;
                lobjInsrContribution.life_supp_premium_amount = lobjIBSdetail.icdoIbsDetail.life_supp_premium_amount;
                lobjInsrContribution.life_spouse_supp_premium_amount = lobjIBSdetail.icdoIbsDetail.life_spouse_supp_premium_amount;
                lobjInsrContribution.life_dep_supp_premium_amount = lobjIBSdetail.icdoIbsDetail.life_dep_supp_premium_amount;
                lobjInsrContribution.ltc_member_three_yrs_premium_amount = lobjIBSdetail.icdoIbsDetail.ltc_member_three_yrs_premium_amount;
                lobjInsrContribution.ltc_member_five_yrs_premium_amount = lobjIBSdetail.icdoIbsDetail.ltc_member_five_yrs_premium_amount;
                lobjInsrContribution.ltc_spouse_three_yrs_premium_amount = lobjIBSdetail.icdoIbsDetail.ltc_spouse_three_yrs_premium_amount;
                lobjInsrContribution.ltc_spouse_five_yrs_premium_amount = lobjIBSdetail.icdoIbsDetail.ltc_spouse_five_yrs_premium_amount;
                //uat pir 1429 :- post GHDV history id and group number
                //prod pir 6076 & 6077 - Removal of person account ghdv history id
                //lobjInsrContribution.person_account_ghdv_history_id = lobjIBSdetail.icdoIbsDetail.person_account_ghdv_history_id;
                lobjInsrContribution.group_number = lobjIBSdetail.icdoIbsDetail.group_number;
                //prod pir 4260
                lobjInsrContribution.ad_and_d_basic_premium_rate = lobjIBSdetail.icdoIbsDetail.ad_and_d_basic_premium_rate;
                lobjInsrContribution.ad_and_d_supplemental_premium_rate = lobjIBSdetail.icdoIbsDetail.ad_and_d_supplemental_premium_rate;
                lobjInsrContribution.life_basic_coverage_amount = lobjIBSdetail.icdoIbsDetail.life_basic_coverage_amount;
                lobjInsrContribution.life_supp_coverage_amount = lobjIBSdetail.icdoIbsDetail.life_supp_coverage_amount;
                lobjInsrContribution.life_spouse_supp_coverage_amount = lobjIBSdetail.icdoIbsDetail.life_spouse_supp_coverage_amount;
                lobjInsrContribution.life_dep_supp_coverage_amount = lobjIBSdetail.icdoIbsDetail.life_dep_supp_coverage_amount;
                //PIR 4444
                lobjInsrContribution.provider_org_id = lobjIBSdetail.icdoIbsDetail.provider_org_id;
                //prod pir 6076
                lobjInsrContribution.coverage_code = lobjIBSdetail.icdoIbsDetail.coverage_code_value;
                lobjInsrContribution.rate_structure_code = lobjIBSdetail.icdoIbsDetail.rate_structure_code;
                lobjInsrContribution.Insert();
            }
        }

        public void PostProviderData()
        {
            foreach (busIbsDetail lobjIBSdetail in icolIbsDetail)
            {
                if (lobjIBSdetail.ibusPerson == null)
                    lobjIBSdetail.LoadPerson();
                if (lobjIBSdetail.icdoIbsDetail.plan_id == busConstant.PlanIdMedicarePartD)
                {
                    PostProviderDataForMedicarePartD(lobjIBSdetail);
                }
                else
                {
                    cdoProviderReportDataInsurance lobjProviderReportDataInsurance = new cdoProviderReportDataInsurance();
                    lobjProviderReportDataInsurance.subsystem_value = busConstant.SubSystemValueIBSDeposit;
                    lobjProviderReportDataInsurance.subsystem_ref_id = lobjIBSdetail.icdoIbsDetail.ibs_detail_id;
                    lobjProviderReportDataInsurance.person_id = lobjIBSdetail.icdoIbsDetail.person_id;
                    lobjProviderReportDataInsurance.ssn = lobjIBSdetail.ibusPerson.icdoPerson.ssn;
                    lobjProviderReportDataInsurance.first_name = lobjIBSdetail.ibusPerson.icdoPerson.first_name;
                    lobjProviderReportDataInsurance.last_name = lobjIBSdetail.ibusPerson.icdoPerson.last_name;
                    lobjProviderReportDataInsurance.plan_id = lobjIBSdetail.icdoIbsDetail.plan_id;
                    //PIR 4444
                    lobjProviderReportDataInsurance.provider_org_id = lobjIBSdetail.icdoIbsDetail.provider_org_id;

                    //TODO: to Discuss with Satya about How to handle this 
                    string lstrRecordType = busConstant.PayrollDetailRecordTypeRegular;
                    if (icdoIbsHeader.report_type_value == busConstant.IBSHeaderReportTypeAdjustment)
                    {
                        if (lobjIBSdetail.icdoIbsDetail.provider_premium_amount > 0)
                            lstrRecordType = busConstant.PayrollDetailRecordTypePositiveAdjustment;
                        else
                            lstrRecordType = busConstant.PayrollDetailRecordTypeNegativeAdjustment;
                    }
                    lobjProviderReportDataInsurance.record_type_value = lstrRecordType;
                    lobjProviderReportDataInsurance.effective_date = lobjIBSdetail.icdoIbsDetail.billing_month_and_year;
                    lobjProviderReportDataInsurance.premium_amount = lobjIBSdetail.icdoIbsDetail.provider_premium_amount;
                    lobjProviderReportDataInsurance.fee_amount = lobjIBSdetail.icdoIbsDetail.group_health_fee_amt;
                    lobjProviderReportDataInsurance.buydown_amount = lobjIBSdetail.icdoIbsDetail.buydown_amount; // PIR 11239
                    lobjProviderReportDataInsurance.medicare_part_d_amt = lobjIBSdetail.icdoIbsDetail.medicare_part_d_amt;
                    //uat pir 1429 :- post ghdv_history_id and group number
                    //prod pir 6076 & 6077 - Removal of person account ghdv history id
                    //lobjProviderReportDataInsurance.person_account_ghdv_history_id = lobjIBSdetail.icdoIbsDetail.person_account_ghdv_history_id;
                    lobjProviderReportDataInsurance.group_number = lobjIBSdetail.icdoIbsDetail.group_number;
                    //prod pir 6076
                    lobjProviderReportDataInsurance.coverage_code = lobjIBSdetail.icdoIbsDetail.coverage_code_value;
                    lobjProviderReportDataInsurance.rate_structure_code = lobjIBSdetail.icdoIbsDetail.rate_structure_code;
                    lobjProviderReportDataInsurance.Insert();
                }
            }
        }

        public void PostProviderDataForMedicarePartD(busIbsDetail lobjIBSDetail)
        {
            busPersonAccountMedicarePartDHistory lobjMedicare = new busPersonAccountMedicarePartDHistory { icdoPersonAccountMedicarePartDHistory = new cdoPersonAccountMedicarePartDHistory() };

            lobjMedicare.FindMedicareByPersonAccountID(lobjIBSDetail.icdoIbsDetail.person_account_id);
            lobjMedicare.LoadMedicarePartDMembersIBS(lobjIBSDetail.icdoIbsDetail.person_account_id);

            foreach (busPersonAccountMedicarePartDHistory lobj in lobjMedicare.iclbPersonAccountMedicarePartDMembersIBS)
            {
                lobj.FindPersonAccount(lobj.icdoPersonAccountMedicarePartDHistory.person_account_id);
                lobj.LoadPlanEffectiveDate();
                
                //Low Income Credit Amount should be populated from Ref table. 
                //Decimal ldecLowIncomeCreditAmount = 0;
                //DataTable adtbCachedLowIncomeCreditRef = busGlobalFunctions.LoadLowIncomeCreditRefCacheData(iobjPassInfo);
                //var lenumList = adtbCachedLowIncomeCreditRef.AsEnumerable().Where(i => i.Field<Decimal>("low_income_credit") == lobj.icdoPersonAccountMedicarePartDHistory.low_income_credit).OrderByDescending(i => i.Field<DateTime>("effective_date"));
                //foreach (DataRow dr in lenumList)
                //{
                //    if (Convert.ToDateTime(dr["effective_date"]).Date <= lobj.idtPlanEffectiveDate.Date)
                //    {
                //        ldecLowIncomeCreditAmount = Convert.ToDecimal(dr["amount"]);
                //        break;
                //    }
                //}
                lobj.iblnIsFromIBSBilling = true;
                lobj.LoadPersonAccount(lobj.icdoPersonAccountMedicarePartDHistory.person_id);
                lobj.GetPremiumAmountFromRef();
                cdoProviderReportDataMedicarePartD lobjProviderReportDataInsuranceMedicarePartD = new cdoProviderReportDataMedicarePartD();
                lobjProviderReportDataInsuranceMedicarePartD.subsystem_value = busConstant.SubSystemValueIBSDeposit;
                lobjProviderReportDataInsuranceMedicarePartD.subsystem_ref_id = lobjIBSDetail.icdoIbsDetail.ibs_detail_id;
                lobjProviderReportDataInsuranceMedicarePartD.person_id = lobj.icdoPersonAccountMedicarePartDHistory.person_id;
                lobjProviderReportDataInsuranceMedicarePartD.ssn = lobj.ibusPerson.icdoPerson.ssn;
                lobjProviderReportDataInsuranceMedicarePartD.first_name = lobj.ibusPerson.icdoPerson.first_name;
                lobjProviderReportDataInsuranceMedicarePartD.last_name = lobj.ibusPerson.icdoPerson.last_name;
                lobjProviderReportDataInsuranceMedicarePartD.provider_org_id = lobjIBSDetail.icdoIbsDetail.provider_org_id;
                lobjProviderReportDataInsuranceMedicarePartD.plan_id = lobjIBSDetail.icdoIbsDetail.plan_id;
                lobjProviderReportDataInsuranceMedicarePartD.effective_date = lobjIBSDetail.icdoIbsDetail.billing_month_and_year;

                string lstrRecordType = busConstant.PayrollDetailRecordTypeRegular;
                if (icdoIbsHeader.report_type_value == busConstant.IBSHeaderReportTypeAdjustment)
                {
                    if (lobjIBSDetail.icdoIbsDetail.member_premium_amount > 0)//PIR 15786
                        lstrRecordType = busConstant.PayrollDetailRecordTypePositiveAdjustment;
                    else
                        lstrRecordType = busConstant.PayrollDetailRecordTypeNegativeAdjustment;
                }
                lobjProviderReportDataInsuranceMedicarePartD.record_type_value = lstrRecordType;

                lobjProviderReportDataInsuranceMedicarePartD.premium_amount = lobjIBSDetail.icdoIbsDetail.provider_premium_amount;
                lobjProviderReportDataInsuranceMedicarePartD.lis_amount = lobjIBSDetail.icdoIbsDetail.lis_amount;
                lobjProviderReportDataInsuranceMedicarePartD.lep_amount = lobjIBSDetail.icdoIbsDetail.lep_amount;

                lobjProviderReportDataInsuranceMedicarePartD.Insert();
            }
        }

        public void CreateDepositForJSRHICNegativeAdjustment(cdoDeposit acdoDeposit)
        {
            acdoDeposit.org_id = ibusJsRhicBill.icdoJsRhicBill.org_id;
            acdoDeposit.reference_no = "IBS Header ID : " + icdoIbsHeader.ibs_header_id;
            acdoDeposit.deposit_amount = -1M * ibusJsRhicBill.icdoJsRhicBill.bill_amount;
            acdoDeposit.status_value = busConstant.DepositDetailStatusApplied;
            acdoDeposit.deposit_source_value = busConstant.DepositSourceNegativeAdjustment;
            acdoDeposit.deposit_date = DateTime.Today;
            acdoDeposit.Insert();
        }

        public void CreateRemittanceForJSRHICNegativeAdjustment(cdoDeposit acdoDeposit)
        {
            cdoRemittance lobjcdoRemittance = new cdoRemittance();
            lobjcdoRemittance.org_id = ibusJsRhicBill.icdoJsRhicBill.org_id;
            lobjcdoRemittance.plan_id = busConstant.PlanIdGroupHealth;
            lobjcdoRemittance.deposit_id = acdoDeposit.deposit_id;
            lobjcdoRemittance.remittance_type_value = busConstant.RemittanceTypeJSRHICDeposit;
            lobjcdoRemittance.remittance_amount = -1M * ibusJsRhicBill.icdoJsRhicBill.bill_amount;
            lobjcdoRemittance.applied_date = busGlobalFunctions.GetSysManagementBatchDate().Date;
            lobjcdoRemittance.Insert();
        }

        public void CreateDepositForNegativeAdjustment(cdoDeposit acdoDeposit, busIbsDetail abusIbsDetail, decimal adecAmount)
        {
            acdoDeposit.person_id = abusIbsDetail.icdoIbsDetail.person_id;
            acdoDeposit.reference_no = abusIbsDetail.icdoIbsDetail.person_account_id.ToString();
            acdoDeposit.deposit_amount = -1M * adecAmount;
            acdoDeposit.status_value = busConstant.DepositDetailStatusApplied;
            acdoDeposit.deposit_source_value = busConstant.DepositSourceNegativeAdjustment;
            acdoDeposit.deposit_date = DateTime.Today;
            acdoDeposit.Insert();
        }

        public void CreateRemittanceForNegativeAdjustment(cdoDeposit acdoDeposit, busIbsDetail abusIbsDetail)
        {
            cdoRemittance lobjcdoRemittance = new cdoRemittance();
            lobjcdoRemittance.person_id = abusIbsDetail.icdoIbsDetail.person_id;
            lobjcdoRemittance.plan_id = abusIbsDetail.icdoIbsDetail.plan_id;
            lobjcdoRemittance.deposit_id = acdoDeposit.deposit_id;
            lobjcdoRemittance.remittance_type_value = busConstant.RemittanceTypeIBSDeposit;
            lobjcdoRemittance.remittance_amount = acdoDeposit.deposit_amount;
            lobjcdoRemittance.applied_date = busGlobalFunctions.GetSysManagementBatchDate().Date;
            lobjcdoRemittance.Insert();
        }
        /*
        public void CreateRemittanceForNegativeAdjustment()
        {
            if (_icolIbsDetail == null)
                LoadIbsDetails();

            foreach (busIbsDetail lbusIbsDetail in _icolIbsDetail)
            {
                if (lbusIbsDetail.icdoIbsDetail.member_premium_amount < 0)
                {
                    cdoDeposit lcdoDeposit = new cdoDeposit();
                    CreateDepositForNegativeAdjustment(lcdoDeposit, lbusIbsDetail);
                    CreateRemittanceForNegativeAdjustment(lcdoDeposit, lbusIbsDetail);
                }
            }
        }
        */
        public void LoadDistinctMembersFromIBSDetail()
        {
			// Loading again for SFN-16789 correspondance - To load members name correctly and not the spouse name
            //if (iclbIBSMembers == null)
                iclbIBSMembers = new Collection<busPerson>();

            //Sort the Collection By Person ID
            icolIbsDetail = busGlobalFunctions.Sort("icdoIbsDetail.person_id", icolIbsDetail);

            int lintOldPersonID = 0;
            foreach (busIbsDetail lobjIbsDetail in icolIbsDetail)
            {
                if (lobjIbsDetail.icdoIbsDetail.person_id == lintOldPersonID) continue;

                if (lobjIbsDetail.ibusPerson == null)
                    lobjIbsDetail.LoadPerson();

                iclbIBSMembers.Add(lobjIbsDetail.ibusPerson);

                lintOldPersonID = lobjIbsDetail.icdoIbsDetail.person_id;
            }
        }

        public void LoadDBCacheData()
        {
            if (ibusDBCacheData == null)
                ibusDBCacheData = new busDBCacheData();
            ibusDBCacheData.idtbCachedRateRef = busGlobalFunctions.LoadHealthRateRefCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedRateStructureRef = busGlobalFunctions.LoadHealthRateStructureCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedCoverageRef = busGlobalFunctions.LoadHealthCoverageRefCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedHealthRate = busGlobalFunctions.LoadHealthRateCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedLifeRate = busGlobalFunctions.LoadLifeRateCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedDentalRate = busGlobalFunctions.LoadDentalRateCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedHMORate = busGlobalFunctions.LoadHMORateCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedLtcRate = busGlobalFunctions.LoadLTCRateCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedVisionRate = busGlobalFunctions.LoadVisionRateCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedMedicarePartDRate = busGlobalFunctions.LoadMedicarePartDRateCacheData(iobjPassInfo);
        }

        /// <summary>
        /// Method to load Remittance Allocation based on remittance id
        /// </summary>
        /// <param name="aintRemittanceID">Remittance ID</param>
        public void LoadIbsRemittanceAllocationsByRemittanceID(int aintRemittanceID)
        {
            DataTable ldtbList = Select<cdoIbsRemittanceAllocation>(
                new string[1] { "remittance_id" },
                new object[1] { aintRemittanceID }, null, null);
            _icolIbsRemittanceAllocation = GetCollection<busIbsRemittanceAllocation>(ldtbList, "icdoIbsRemittanceAllocation");
        }

        /// <summary>
        /// Method to load Remittance allocation based on remittance id
        /// </summary>
        /// <param name="aintRemittanceID">Remittance ID</param>
        public void LoadJsRhicRemittanceAllocationsByRemittanceID(int aintRemittanceID)
        {
            DataTable ldtbList = Select<cdoJsRhicRemittanceAllocation>(
                new string[1] { "remittance_id" },
                new object[1] { aintRemittanceID }, null, null);
            _icolJsRhicRemittanceAllocation = GetCollection<busJsRhicRemittanceAllocation>(ldtbList, "icdoJsRhicRemittanceAllocation");
        }

        public Collection<busOrgPlan> iclbProviderOrgPlan { get; set; }
        public DataTable idtbPALifeOptionHistory { get; set; }
        public DataTable idtbGHDVHistory { get; set; }
        public DataTable idtbLifeHistory { get; set; }
        public DataTable idtbMedicarePartDHistory { get; set; }

        public void LoadActiveProviders()
        {
            DataTable ldtbActiveProviders = busNeoSpinBase.Select("cdoIbsHeader.LoadAllActiveProviders", new object[1] { idtBatchDate });
            iclbProviderOrgPlan = new busBase().GetCollection<busOrgPlan>(ldtbActiveProviders, "icdoOrgPlan");
        }

        public void LoadLifeOptionData()
        {
            idtbPALifeOptionHistory =
                busNeoSpinBase.Select("cdoIbsHeader.LoadLifeOption", new object[1] { idtBatchDate });
        }

        public void LoadGHDVHistory()
        {
            idtbGHDVHistory =
                busNeoSpinBase.Select("cdoIbsHeader.LoadGHDVHistory", new object[1] { idtBatchDate });
        }

        public void LoadLifeHistory()
        {
            idtbLifeHistory =
                busNeoSpinBase.Select("cdoIbsHeader.LoadLifeHistory", new object[1] { idtBatchDate });
        }

        public void LoadMedicarePartDHistory()
        {
            idtbMedicarePartDHistory =
                busNeoSpinBase.Select("cdoIbsHeader.LoadMedicarePartDHistory", new object[1] { idtBatchDate });
        }

        public busOrgPlan LoadProviderOrgPlanByProviderOrgId(int aintProviderOrgId, int aintPlanId)
        {
            busOrgPlan lbusOrgPlanToReturn = new busOrgPlan { icdoOrgPlan = new cdoOrgPlan() };
            foreach (var lbusOrgPlan in iclbProviderOrgPlan)
            {
                if ((lbusOrgPlan.icdoOrgPlan.org_id == aintProviderOrgId) && (lbusOrgPlan.icdoOrgPlan.plan_id == aintPlanId))
                {
                    if (busGlobalFunctions.CheckDateOverlapping(idtBatchDate,
                        lbusOrgPlan.icdoOrgPlan.participation_start_date,
                        lbusOrgPlan.icdoOrgPlan.participation_end_date))
                    {
                        lbusOrgPlanToReturn = lbusOrgPlan;
                        break;
                    }
                }
            }
            return lbusOrgPlanToReturn;
        }
        //Data table used for JS RHIC Report
        public DataTable idtJSRHICReportTable { get; set; }
        //Create table with specified columns for JS RHIC Report
        public DataTable CreateNewDataTableForJSRHICReport()
        {
            DataTable ldtbReportTable = new DataTable();
            DataColumn ldc1 = new DataColumn("PERSLinkID", Type.GetType("System.Int32"));
            DataColumn ldc2 = new DataColumn("FirstName", Type.GetType("System.String"));
            DataColumn ldc3 = new DataColumn("LastName", Type.GetType("System.String"));
            DataColumn ldc4 = new DataColumn("RHICAmount", Type.GetType("System.Decimal"));

            ldtbReportTable.Columns.Add(ldc1);
            ldtbReportTable.Columns.Add(ldc2);
            ldtbReportTable.Columns.Add(ldc3);
            ldtbReportTable.Columns.Add(ldc4);

            ldtbReportTable.TableName = busConstant.ReportTableName;
            return ldtbReportTable;
        }
        //Add Data to  JS RHIC Report Data table
        public void AddToNewDataRow(busIbsDetail aobjIbsDetail)
        {
            DataRow dr = idtJSRHICReportTable.NewRow();

            dr["PERSLinkID"] = aobjIbsDetail.ibusPerson.icdoPerson.person_id;
            dr["FirstName"] = aobjIbsDetail.ibusPerson.icdoPerson.first_name;
            dr["LastName"] = aobjIbsDetail.ibusPerson.icdoPerson.last_name;
            dr["RHICAmount"] = aobjIbsDetail.icdoIbsDetail.js_rhic_amount;
            idtJSRHICReportTable.Rows.Add(dr);
        }

        #region UCS - 038 addendum
        /// <summary>
        /// Method to create payment election adjustment or deposit
        /// </summary>
        public void CreateAdjustmentOrDeposit()
        {
            if (icolIbsDetail == null)
                LoadIbsDetails();
            //prod pir 6074 : new report for refund amounts
            CreateRefundDetailsTable();
            DateTime ldtLastIBSRunDate = busIBSHelper.GetLastPostedIBSBatchDate();
            int lintPersonAccountID = 0, lintProviderOrgID = 0, lintPersonID = 0;
            decimal ldecTotalPaidPremiumAmount;
            if (ibusLastPostedRegularIBSHeader == null)
                LoadLastPostedRegularIBSHeader();
            icolIbsDetail = busGlobalFunctions.Sort<busIbsDetail>("icdoIbsDetail.person_account_id,icdoIbsDetail.provider_org_id", icolIbsDetail);
            foreach (busIbsDetail lobjDetail in icolIbsDetail)
            {
				// PIR 15698
                if ((lintPersonAccountID == lobjDetail.icdoIbsDetail.person_account_id) && (lintProviderOrgID == lobjDetail.icdoIbsDetail.provider_org_id) && lintPersonID == lobjDetail.icdoIbsDetail.person_id)
                    continue;
                ldecTotalPaidPremiumAmount = 0.0M;
                IEnumerable<busIbsDetail> lenmIBSDetail = icolIbsDetail.Where(o => o.icdoIbsDetail.person_account_id == lobjDetail.icdoIbsDetail.person_account_id
                    && o.icdoIbsDetail.provider_org_id == lobjDetail.icdoIbsDetail.provider_org_id
                    && o.icdoIbsDetail.person_id == lobjDetail.icdoIbsDetail.person_id);  // PIR 15698
                if (lenmIBSDetail != null)
                {
                    ldecTotalPaidPremiumAmount = lenmIBSDetail.Sum(o => o.icdoIbsDetail.paid_premium_amount);                    
                    if (ldecTotalPaidPremiumAmount == 0)
                    {
                        //PROD PIR 5881
                        //create multiple adjustments for current month and previous months
                        IEnumerable<busIbsDetail> lenmOldIBSDetail = lenmIBSDetail.Where(o => o.icdoIbsDetail.billing_month_and_year < ldtLastIBSRunDate);
                        IEnumerable<busIbsDetail> lenmCurrentIBSDetail = lenmIBSDetail.Where(o => o.icdoIbsDetail.billing_month_and_year >= ldtLastIBSRunDate);
                        if(lenmOldIBSDetail != null)
                            CreateAdjustmentPaymentElection(lobjDetail, lenmOldIBSDetail, ldtLastIBSRunDate);
                        if(lenmCurrentIBSDetail != null)
                            CreateAdjustmentPaymentElection(lobjDetail, lenmCurrentIBSDetail, ldtLastIBSRunDate);
                    }
                    else if (ldecTotalPaidPremiumAmount < 0)
                    {
                        CreateDepositAndRemittance(lobjDetail, ldecTotalPaidPremiumAmount);
                        //prod pir 6074 : new report for refund amounts
                        AddNewRowToRefundTable(lobjDetail, Math.Abs(ldecTotalPaidPremiumAmount));
                    }
                    UpdateIBSPersonSummary(lobjDetail, lenmIBSDetail);
                    lintPersonAccountID = lobjDetail.icdoIbsDetail.person_account_id;
                    lintProviderOrgID = lobjDetail.icdoIbsDetail.provider_org_id;
                    lintPersonID = lobjDetail.icdoIbsDetail.person_id;
                    lenmIBSDetail = null;
                }
            }
            //prod pir 6074 : new report for refund details
            if (idtRefundDetails.Rows.Count > 0)
            {
                busNeoSpinBase lobjNeospinBase = new busNeoSpinBase();
                lobjNeospinBase.CreateReport("rptRemittanceRefundDetails.rpt", idtRefundDetails, string.Empty);
            }
        }

        private void UpdateIBSPersonSummary(busIbsDetail aobjDetail, IEnumerable<busIbsDetail> aenmIBSDetail)
        {
            decimal ldecTotalAdjustmentAmount = 0.0M;
            if (aenmIBSDetail.Where(o => o.icdoIbsDetail.member_premium_amount < 0).Any())
            {
                ldecTotalAdjustmentAmount = aenmIBSDetail.Where(o => o.icdoIbsDetail.member_premium_amount < 0)
                                                                .Sum(o => o.icdoIbsDetail.member_premium_amount - o.icdoIbsDetail.paid_premium_amount);
                if (ldecTotalAdjustmentAmount != 0)
                {
                    busIbsPersonSummary lobjPersonSummary = new busIbsPersonSummary();
                    if (lobjPersonSummary.FindIbsPersonSummaryByIbsHeaderAndPerson(ibusLastPostedRegularIBSHeader.icdoIbsHeader.ibs_header_id, aobjDetail.icdoIbsDetail.person_id))
                    {
                        lobjPersonSummary.icdoIbsPersonSummary.adjustment_amount += ldecTotalAdjustmentAmount;
                        lobjPersonSummary.icdoIbsPersonSummary.Update();
                    }
                    //PROD PIR 5415
                    //need to insert new ibs person summary if not available
                    else
                    {
                        lobjPersonSummary.icdoIbsPersonSummary.adjustment_amount += ldecTotalAdjustmentAmount;
                        lobjPersonSummary.icdoIbsPersonSummary.ibs_header_id = ibusLastPostedRegularIBSHeader.icdoIbsHeader.ibs_header_id;
                        lobjPersonSummary.icdoIbsPersonSummary.person_id = aobjDetail.icdoIbsDetail.person_id;
                        lobjPersonSummary.icdoIbsPersonSummary.Insert();
                    }
                }
            }
        }

        /// <summary>
        /// method to create deposit and remittance
        /// </summary>
        /// <param name="aobjDetail">ibs detail object</param>
        /// <param name="adecTotalDueAmount">total due amount</param>
        private void CreateDepositAndRemittance(busIbsDetail aobjDetail, decimal adecTotalDueAmount)
        {
            cdoDeposit lcdoDeposit = new cdoDeposit();
            CreateDepositForNegativeAdjustment(lcdoDeposit, aobjDetail, adecTotalDueAmount);
            CreateRemittanceForNegativeAdjustment(lcdoDeposit, aobjDetail);
        }

        /// <summary>
        ///  Method to create payment election adjustment
        /// </summary>
        /// <param name="aobjDetail">ibs detail object</param>
        /// <param name="aenmIBSDetail">collection of ibs detail for a person account id</param>
        /// <param name="adtLastIBSRunDate">latest ibs run date</param>       
        private void CreateAdjustmentPaymentElection(busIbsDetail aobjDetail, IEnumerable<busIbsDetail> aenmIBSDetail, DateTime adtLastIBSRunDate)
        {
            busPaymentElectionAdjustment lobjPaymentElection = new busPaymentElectionAdjustment();
            busPayeeAccount lobjPayeeAccount = new busPayeeAccount();
            int lintOldRecordCount = aenmIBSDetail.Where(o => o.icdoIbsDetail.billing_month_and_year < adtLastIBSRunDate)
                                                    .Select(o => o.icdoIbsDetail.billing_month_and_year).Distinct().Count();
            string lstrPaymentMethod = string.Empty;
            string lstrIBSAdjustmentStatus = lintOldRecordCount == 0 ? busConstant.IBSAdjustmentStatusApproved :
                                                    busConstant.IBSAdjustmentStatusPending;
            int lintPaymentElectionAdj = 0;
            decimal ldecTotalDueAmount = 0.0M, ldecTotalPAPITAmount = 0.0M;
            decimal ldecTotalPositiveDueAmount = aenmIBSDetail.Where(o => o.icdoIbsDetail.member_premium_amount > 0).Sum(o => o.icdoIbsDetail.member_premium_amount);
            decimal ldecTotalPositivePaidAmount = aenmIBSDetail.Where(o => o.icdoIbsDetail.member_premium_amount > 0).Sum(o => o.icdoIbsDetail.paid_premium_amount);
            ldecTotalDueAmount = ldecTotalPositiveDueAmount - ldecTotalPositivePaidAmount;
            //PROD PIR 5389
            //need to create papit only if the positive adj billing month and year is not equal to next benefit payment date
            DateTime ldtPostiveAdjBillingMonthYear = aenmIBSDetail.Where(o => o.icdoIbsDetail.member_premium_amount > 0)
                                                                    .Select(o => o.icdoIbsDetail.billing_month_and_year)
                                                                    .FirstOrDefault();
            /*if (!aenmIBSDetail.Where(o => Math.Abs(o.icdoIbsDetail.paid_premium_amount) > 0).Any())
            {
                decimal ldecTotalNegativeAdjustmentAmount = aenmIBSDetail.Where(o => o.icdoIbsDetail.member_premium_amount < 0).Sum(o => o.icdoIbsDetail.member_premium_amount);
                busIbsPersonSummary lobjPersonSummary = new busIbsPersonSummary();
                if (lobjPersonSummary.FindIbsPersonSummaryByIbsHeaderAndPerson(ibusLastPostedRegularIBSHeader.icdoIbsHeader.ibs_header_id, aobjDetail.icdoIbsDetail.person_id))
                {
                    lobjPersonSummary.icdoIbsPersonSummary.adjustment_amount += ldecTotalNegativeAdjustmentAmount;
                    lobjPersonSummary.icdoIbsPersonSummary.Update();
                }
            }*/
            if (aobjDetail.ibusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.payee_account_id > 0)
            {
                lobjPayeeAccount.FindPayeeAccount(aobjDetail.ibusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.payee_account_id);
                lobjPayeeAccount.LoadBenefitAmount();
                ldecTotalPAPITAmount = lobjPayeeAccount.LoadTotalPAPITAmountForInsuranceRecv();
                if (aobjDetail.ibusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.payment_method_value == busConstant.IBSModeOfPaymentPensionCheck &&
                    (ldecTotalDueAmount + ldecTotalPAPITAmount) > lobjPayeeAccount.idecBenefitAmount)
                {
                    lstrPaymentMethod = busConstant.IBSModeOfPaymentPersonalCheck;
                }
                else
                {
                    lstrPaymentMethod = aobjDetail.ibusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.payment_method_value;
                }
            }
            else
                lstrPaymentMethod = aobjDetail.ibusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.payment_method_value;
            if (ldecTotalDueAmount > 0)
            {
                if (string.IsNullOrEmpty(lstrPaymentMethod))
                    lstrPaymentMethod = busConstant.IBSModeOfPaymentPersonalCheck;
                DateTime ldtApprovedDate = DateTime.MinValue;
                if (lstrIBSAdjustmentStatus == busConstant.IBSAdjustmentStatusApproved)
                    ldtApprovedDate = DateTime.Now;
                lintPaymentElectionAdj = lobjPaymentElection.InsertNewAdjustmentPaymentElection(aobjDetail.icdoIbsDetail.ibs_header_id,
                                                                        aobjDetail.icdoIbsDetail.person_account_id,
                                                                        lstrIBSAdjustmentStatus,
                                                                        aobjDetail.ibusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.payee_account_id,
                                                                        lstrPaymentMethod,
                                                                        busConstant.IBSAdjustmentRepaymentTypeLumpSum,
                                                                        ldecTotalDueAmount,
                                                                        ldtApprovedDate, aobjDetail.icdoIbsDetail.provider_org_id);
                //prod pir 5855
                //add the adjustment amount to ibs person summary on approval
                //--start--//
                if (lstrIBSAdjustmentStatus == busConstant.IBSAdjustmentStatusApproved)
                {
                    if (ibusLastPostedRegularIBSHeader == null)
                        LoadLastPostedRegularIBSHeader();
                    busIbsPersonSummary lobjPersonSummary = new busIbsPersonSummary();
                    if (lobjPersonSummary.FindIbsPersonSummaryByIbsHeaderAndPerson(ibusLastPostedRegularIBSHeader.icdoIbsHeader.ibs_header_id, aobjDetail.icdoIbsDetail.person_id))
                    {
                        lobjPersonSummary.icdoIbsPersonSummary.adjustment_amount += ldecTotalDueAmount;
                        lobjPersonSummary.icdoIbsPersonSummary.Update();
                    }
                    //PROD PIR 5415
                    //need to insert new ibs person summary if not available
                    else
                    {
                        lobjPersonSummary.icdoIbsPersonSummary.adjustment_amount += ldecTotalDueAmount;
                        lobjPersonSummary.icdoIbsPersonSummary.ibs_header_id = ibusLastPostedRegularIBSHeader.icdoIbsHeader.ibs_header_id;
                        lobjPersonSummary.icdoIbsPersonSummary.person_id = aobjDetail.icdoIbsDetail.person_id;
                        lobjPersonSummary.icdoIbsPersonSummary.Insert();
                    }
                }
                //--end--//
                if (lstrPaymentMethod == busConstant.IBSModeOfPaymentPensionCheck && lstrIBSAdjustmentStatus == busConstant.IBSAdjustmentStatusApproved &&
                    lobjPayeeAccount != null && lobjPayeeAccount.icdoPayeeAccount !=null && lobjPayeeAccount.icdoPayeeAccount.payee_account_id > 0)
                {
                    //PROD PIR 5389
                    //need to create papit only if the positive adj billing month and year is not equal to next benefit payment date
                    if (lobjPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                        lobjPayeeAccount.LoadNexBenefitPaymentDate();
                    if (ldtPostiveAdjBillingMonthYear != null && ldtPostiveAdjBillingMonthYear != DateTime.MinValue && 
                        ldtPostiveAdjBillingMonthYear != lobjPayeeAccount.idtNextBenefitPaymentDate)
                    {
                        lobjPaymentElection.FindPaymentElectionAdjustment(lintPaymentElectionAdj);
                        lobjPaymentElection.CreatePAPITItem();
                    }
                }
                foreach (busIbsDetail lobjDetail in aenmIBSDetail)
                {
                    lobjDetail.icdoIbsDetail.payment_election_adjustment_id = lintPaymentElectionAdj;
                    lobjDetail.icdoIbsDetail.Update();
                }
                if (lintOldRecordCount > 0)
                {
                    IntiateIBSWorkflow(aobjDetail.icdoIbsDetail.person_id, busConstant.MapProcessIBSAdjustment);
                }
                //PROD Pir - 4642
                else
                {
                    IntiateIBSWorkflow(aobjDetail.icdoIbsDetail.person_id, busConstant.Map_Process_IBS_PaymentElection_Adjustment);
                }
            }
            lobjPayeeAccount = null;
            lobjPaymentElection = null;
        }

        private void IntiateIBSWorkflow(int aintPersonID, int aintProcessID)
        {
            busWorkflowHelper.InitiateBpmRequest(aintProcessID, aintPersonID, 0, 0, iobjPassInfo, busConstant.WorkflowProcessSource_Batch);
;        }

        private void InitiateIBSAdjustmentHeaderWorkflow()
        {
            busWorkflowHelper.InitiateBpmRequest(busConstant.MapProcessIBSAdjustmentHeader, 0, 0, 0, iobjPassInfo);

        }

        /// <summary>
        /// Method to update paid premium amount in positive adjustment row
        /// </summary>
        public void UpdatePaidPremiumAmount()
        {
            if (icolIbsDetail == null)
                LoadIbsDetails();

            IEnumerable<busIbsDetail> lenmPaidNegativeIBSDetail = icolIbsDetail.Where(o => o.icdoIbsDetail.paid_premium_amount < 0 && o.icdoIbsDetail.member_premium_amount < 0);
            IEnumerable<busIbsDetail> lenmPositiveIBSDetail = icolIbsDetail.Where(o => o.icdoIbsDetail.member_premium_amount > 0);
            decimal ldecAvailableAmount = 0.0M;
            int lintPreviousPersonAccountID = 0, lintProviderOrgID = 0, lintPersonID = 0;
            foreach (busIbsDetail lobjDetail in lenmPositiveIBSDetail)
            {
                if (lintPreviousPersonAccountID != lobjDetail.icdoIbsDetail.person_account_id || lintProviderOrgID != lobjDetail.icdoIbsDetail.provider_org_id || lintPersonID != lobjDetail.icdoIbsDetail.person_id)
                {
                    // PIR 15698
                    ldecAvailableAmount = Math.Abs(lenmPaidNegativeIBSDetail.Where(o => o.icdoIbsDetail.person_account_id == lobjDetail.icdoIbsDetail.person_account_id &&
                                                                    o.icdoIbsDetail.provider_org_id == lobjDetail.icdoIbsDetail.provider_org_id && o.icdoIbsDetail.person_id == lobjDetail.icdoIbsDetail.person_id) 
                                                                    .Sum(o => o.icdoIbsDetail.paid_premium_amount));
                }
                if (ldecAvailableAmount > 0)
                {
                    if (lobjDetail.icdoIbsDetail.member_premium_amount > ldecAvailableAmount)
                    {
                        lobjDetail.icdoIbsDetail.paid_premium_amount = ldecAvailableAmount;
                    }
                    else
                    {
                        lobjDetail.icdoIbsDetail.paid_premium_amount = lobjDetail.icdoIbsDetail.member_premium_amount;
                    }
                    lobjDetail.icdoIbsDetail.Update();
                    ldecAvailableAmount -= lobjDetail.icdoIbsDetail.paid_premium_amount;
                }
                lintPreviousPersonAccountID = lobjDetail.icdoIbsDetail.person_account_id;
                lintProviderOrgID = lobjDetail.icdoIbsDetail.provider_org_id;
                lintPersonID = lobjDetail.icdoIbsDetail.person_id;
            }

            //forcefully loading since we are updating ibs detail in the above code
            LoadIbsDetails();
        }

        public busIbsHeader ibusLastPostedRegularIBSHeader { get; set; }
        public void LoadLastPostedRegularIBSHeader()
        {
            ibusLastPostedRegularIBSHeader = new busIbsHeader { icdoIbsHeader = new cdoIbsHeader() };
            DataTable ldtbList = busNeoSpinBase.SelectWithOperator<cdoIbsHeader>(
              new string[3] { "report_type_value", "report_status_value", "ibs_header_id" },
               new string[3] { "=", "=", "<>" },
              new object[3] { busConstant.IBSHeaderReportTypeRegular, busConstant.IBSHeaderStatusPosted, icdoIbsHeader.ibs_header_id }, "billing_month_and_year desc");
            if (ldtbList.Rows.Count > 0)
            {
                ibusLastPostedRegularIBSHeader.icdoIbsHeader.LoadData(ldtbList.Rows[0]);
            }
        }

        #endregion

        //PROD PIR 5389
        /// <summary>
        /// method to update paid premium amounts if contribution came after enrollment change
        /// </summary>
        public void UpdatePaidPremiumAmountForNegativeAdj()
        {
            if (icolIbsDetail == null)
                LoadIbsDetails();
            foreach (busIbsDetail lobjDetail in icolIbsDetail)
            {
                if (lobjDetail.icdoIbsDetail.member_premium_amount < 0)
                {
                    //As per latest email 01/21/2011 from Satya, created date logic to be removed and subsystem value check to be removed
                    DataTable ldtbList = SelectWithOperator<cdoPersonAccountInsuranceContribution>(
                                            new string[2] { "person_account_id", "effective_date" },
                                            new string[2] { "=", "=" },
                                            new object[2] { lobjDetail.ibusPersonAccount.icdoPersonAccount.person_account_id,
                                                            lobjDetail.icdoIbsDetail.billing_month_and_year}, "HEALTH_INSURANCE_CONTRIBUTION_ID");
                    if (ldtbList.Rows.Count > 0)
                    {
                        lobjDetail.icdoIbsDetail.paid_premium_amount = -1 * ldtbList.AsEnumerable()
                                                                                .Where(o=>o.Field<string>("subsystem_value") == busConstant.SubSystemValueIBSDeposit ||
                                                                                    o.Field<string>("subsystem_value") == busConstant.SubSystemValueIBSPayment)
                                                                                    .Sum(o => o.Field<decimal>("paid_premium_amount"));
                        lobjDetail.icdoIbsDetail.Update();
                    }
                }
            }
        }

        //datatable to contain data for Refund details report
        public DataTable idtRefundDetails { get; set; }
        /// <summary>
        /// method to initialize the report table
        /// </summary>
        public void CreateRefundDetailsTable()
        {
            idtRefundDetails = new DataTable();
            DataColumn ldc1 = new DataColumn("person_id", Type.GetType("System.Int32"));
            DataColumn ldc2 = new DataColumn("last_name", Type.GetType("System.String"));
            DataColumn ldc3 = new DataColumn("first_name", Type.GetType("System.String"));
            DataColumn ldc4 = new DataColumn("deposit_amount", Type.GetType("System.Decimal"));
            idtRefundDetails.Columns.Add(ldc1);
            idtRefundDetails.Columns.Add(ldc2);
            idtRefundDetails.Columns.Add(ldc3);
            idtRefundDetails.Columns.Add(ldc4);
        }
        /// <summary>
        /// method to add new rows to report table
        /// </summary>
        /// <param name="aobjDetail">ibs detail bus. object</param>
        /// <param name="adecTotalPaidPremiumAmount">deposit total amount</param>
        private void AddNewRowToRefundTable(busIbsDetail aobjDetail, decimal adecTotalPaidPremiumAmount)
        {
            DataRow ldrNewRow = idtRefundDetails.NewRow();
            if (aobjDetail.ibusPerson == null)
                aobjDetail.LoadPerson();
            ldrNewRow["person_id"] = aobjDetail.icdoIbsDetail.person_id;
            ldrNewRow["last_name"] = aobjDetail.ibusPerson.icdoPerson.last_name;
            ldrNewRow["first_name"] = aobjDetail.ibusPerson.icdoPerson.first_name;
            ldrNewRow["deposit_amount"] = adecTotalPaidPremiumAmount;

            idtRefundDetails.Rows.Add(ldrNewRow);
            idtRefundDetails.AcceptChanges();
        }

        public void UpdatePAPITItems()
        {
            if (icolIbsDetail == null)
                LoadIbsDetails();

            foreach (busIbsDetail lobjDetail in icolIbsDetail)
            {
                //Medicare Part D
                if (lobjDetail.icdoIbsDetail.plan_id == busConstant.PlanIdMedicarePartD)
                {
                    busPersonAccountMedicarePartDHistory lobjMedicare = new busPersonAccountMedicarePartDHistory();

                    lobjMedicare.FindPersonAccount(lobjDetail.icdoIbsDetail.person_account_id);
                    lobjMedicare.FindMedicareByPersonAccountID(lobjDetail.icdoIbsDetail.person_account_id);

                    if (lobjDetail.ibusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.ihstOldValues.Count > 0)
                    {
                        lobjMedicare.iintOldPayeeAccountID = Convert.ToInt32(lobjDetail.ibusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.ihstOldValues["payee_account_id"]);
                    }
                    lobjMedicare.LoadOrgPlan(lobjMedicare.icdoPersonAccountMedicarePartDHistory.start_date);
                    lobjMedicare.LoadProviderOrgPlan(lobjMedicare.icdoPersonAccountMedicarePartDHistory.start_date);

                    if (lobjMedicare.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                    {
                        lobjMedicare.LoadActiveProviderOrgPlan(lobjMedicare.GetLatestDate(lobjMedicare.icdoPersonAccount.current_plan_start_date, 
                                        lobjDetail.ibusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date,
                                        lobjMedicare.idtNextBenefiPaymentDate));

                        lobjMedicare.GetMonthlyPremiumAmountForMedicarePartD(lobjMedicare.GetLatestDate(lobjMedicare.icdoPersonAccount.current_plan_start_date,
                                        lobjDetail.ibusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date,
                                        lobjMedicare.idtNextBenefiPaymentDate));

                        lobjDetail.ibusPersonAccount.ibusPaymentElection.ManagePayeeAccountPaymentItemType(lobjDetail.ibusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.payee_account_id,
                                            lobjMedicare.iintOldPayeeAccountID, lobjMedicare.icdoPersonAccountMedicarePartDHistory.MonthlyPremiumAmount, lobjMedicare.icdoPersonAccount.plan_id,
                                            lobjMedicare.GetLatestDate(lobjMedicare.icdoPersonAccount.current_plan_start_date, lobjDetail.ibusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date,
                                            lobjMedicare.idtNextBenefiPaymentDate), lobjDetail.ibusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.payment_method_value, lobjMedicare.ibusProviderOrgPlan.icdoOrgPlan.org_id,
                                            lobjDetail.icdoIbsDetail.person_account_id,true); //15575

                    }

                }

                //GHDV
                if (lobjDetail.icdoIbsDetail.plan_id == busConstant.PlanIdGroupHealth
                    || lobjDetail.icdoIbsDetail.plan_id == busConstant.PlanIdDental
                    || lobjDetail.icdoIbsDetail.plan_id == busConstant.PlanIdVision)
                {
                    busPersonAccountGhdv lobjGHDV = new busPersonAccountGhdv();

                    lobjGHDV.FindGHDVByPersonAccountID(lobjDetail.icdoIbsDetail.person_account_id);
                    lobjGHDV.FindPersonAccount(lobjDetail.icdoIbsDetail.person_account_id);

                    if (lobjDetail.ibusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.ihstOldValues != null)
                    {
                        lobjGHDV.iintOldPayeeAccountID = Convert.ToInt32(lobjDetail.ibusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.ihstOldValues["payee_account_id"]);
                    }
                    lobjGHDV.DetermineEnrollmentAndLoadObjects(lobjGHDV.icdoPersonAccount.current_plan_start_date_no_null, false);
                    lobjGHDV.LoadOrgPlan(lobjGHDV.idtPlanEffectiveDate);
                    lobjGHDV.LoadProviderOrgPlan(lobjGHDV.idtPlanEffectiveDate);
                    if (lobjGHDV.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                    {
                        if (lobjGHDV.icdoPersonAccount.person_employment_dtl_id > 0)
                        {
                            lobjGHDV.LoadOrgPlan(lobjGHDV.GetLatestDate(lobjGHDV.icdoPersonAccount.current_plan_start_date, lobjDetail.ibusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date,
                                            lobjGHDV.idtNextBenefiPaymentDate));
                            lobjGHDV.LoadProviderOrgPlan(lobjGHDV.GetLatestDate(lobjGHDV.icdoPersonAccount.current_plan_start_date, lobjDetail.ibusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date,
                                            lobjGHDV.idtNextBenefiPaymentDate));
                        }
                        else
                        {
                            lobjGHDV.LoadActiveProviderOrgPlan(lobjGHDV.GetLatestDate(lobjGHDV.icdoPersonAccount.current_plan_start_date, lobjDetail.ibusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date,
                                                lobjGHDV.idtNextBenefiPaymentDate));
                        }
                        lobjGHDV.CalculatePremiumAmount(lobjGHDV.GetLatestDate(lobjGHDV.icdoPersonAccount.current_plan_start_date, lobjDetail.ibusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date,
                                            lobjGHDV.idtNextBenefiPaymentDate));

                        lobjDetail.ibusPersonAccount.ibusPaymentElection.ManagePayeeAccountPaymentItemType(lobjDetail.ibusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.payee_account_id,
                                            lobjGHDV.iintOldPayeeAccountID, lobjGHDV.icdoPersonAccountGhdv.MonthlyPremiumAmount - lobjGHDV.icdoPersonAccountGhdv.total_rhic_amount, lobjGHDV.icdoPersonAccount.plan_id,
                                            lobjGHDV.GetLatestDate(lobjGHDV.icdoPersonAccount.current_plan_start_date, lobjDetail.ibusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date,
                                            lobjGHDV.idtNextBenefiPaymentDate), lobjDetail.ibusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.payment_method_value, lobjGHDV.ibusProviderOrgPlan.icdoOrgPlan.org_id, IsMedicare: false);

                        lobjGHDV.RecalculatePremiumBasedOnPlanEffectiveDate();
                    }
                }

                //Life
                if (lobjDetail.icdoIbsDetail.plan_id == busConstant.PlanIdGroupLife)
                {
                    busPersonAccountLife lobjLife = new busPersonAccountLife();
                    lobjLife.FindPersonAccountLife(lobjDetail.icdoIbsDetail.person_account_id);
                    lobjLife.FindPersonAccount(lobjDetail.icdoIbsDetail.person_account_id);
                    lobjLife.LoadLifeOptionData();

                    if (lobjLife.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                    {
                        if (lobjLife.icdoPersonAccount.person_employment_dtl_id > 0)
                        {
                            lobjLife.LoadOrgPlan(lobjLife.GetLatestDate(lobjLife.icdoPersonAccount.current_plan_start_date, lobjDetail.ibusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date,
                                            lobjLife.idtNextBenefiPaymentDate));
                            lobjLife.LoadProviderOrgPlan(lobjLife.GetLatestDate(lobjLife.icdoPersonAccount.current_plan_start_date, lobjDetail.ibusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date,
                                            lobjLife.idtNextBenefiPaymentDate));
                        }
                        else
                        {
                            lobjLife.LoadActiveProviderOrgPlan(lobjLife.GetLatestDate(lobjLife.icdoPersonAccount.current_plan_start_date, lobjDetail.ibusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date,
                                            lobjLife.idtNextBenefiPaymentDate));
                        }
                        lobjLife.GetMonthlyPremiumAmount(lobjLife.GetLatestDate(lobjLife.icdoPersonAccount.current_plan_start_date, lobjDetail.ibusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date,
                                    lobjLife.idtNextBenefiPaymentDate));

                        lobjDetail.ibusPersonAccount.ibusPaymentElection.ManagePayeeAccountPaymentItemType(lobjDetail.ibusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.payee_account_id,
                                    lobjLife.iintOldPayeeAccountID, lobjLife.idecTotalMonthlyPremium, lobjLife.icdoPersonAccount.plan_id,
                                    lobjLife.GetLatestDate(lobjLife.icdoPersonAccount.current_plan_start_date, lobjDetail.ibusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date,
                                    lobjLife.idtNextBenefiPaymentDate), lobjDetail.ibusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.payment_method_value, lobjLife.ibusProviderOrgPlan.icdoOrgPlan.org_id, IsMedicare: false);

                        lobjLife.GetMonthlyPremiumAmount();
                    }
                }

                //LTC
                if (lobjDetail.icdoIbsDetail.plan_id == busConstant.PlanIdLTC)
                {
                    busPersonAccountLtc lobjLTC = new busPersonAccountLtc();

                    lobjLTC.FindPersonAccount(lobjDetail.icdoIbsDetail.person_account_id);
                    lobjLTC.LoadLtcOptionUpdateMember();
                    lobjLTC.LoadLtcOptionUpdateSpouse();

                    if (lobjLTC.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                    {
                        if (lobjLTC.icdoPersonAccount.person_employment_dtl_id > 0)
                        {
                            lobjLTC.LoadOrgPlan(lobjLTC.GetLatestDate(lobjLTC.icdoPersonAccount.current_plan_start_date, lobjDetail.ibusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date,
                                                lobjLTC.idtNextBenefiPaymentDate));
                            lobjLTC.LoadProviderOrgPlan(lobjLTC.GetLatestDate(lobjLTC.icdoPersonAccount.current_plan_start_date, lobjDetail.ibusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date,
                                                lobjLTC.idtNextBenefiPaymentDate));
                        }
                        else
                        {
                            lobjLTC.LoadActiveProviderOrgPlan(lobjLTC.GetLatestDate(lobjLTC.icdoPersonAccount.current_plan_start_date, lobjDetail.ibusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date,
                                                lobjLTC.idtNextBenefiPaymentDate));
                        }
                        lobjLTC.GetMonthlyPremiumAmount(lobjLTC.GetLatestDate(lobjLTC.icdoPersonAccount.current_plan_start_date, lobjDetail.ibusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date,
                                                lobjLTC.idtNextBenefiPaymentDate));

                        lobjDetail.ibusPersonAccount.ibusPaymentElection.ManagePayeeAccountPaymentItemType(lobjDetail.ibusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.payee_account_id,
                                                lobjLTC.iintOldPayeeAccountID, lobjLTC.idecTotalMonthlyPremium, lobjLTC.icdoPersonAccount.plan_id,
                                                lobjLTC.GetLatestDate(lobjLTC.icdoPersonAccount.current_plan_start_date, lobjDetail.ibusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date,
                                                lobjLTC.idtNextBenefiPaymentDate), lobjDetail.ibusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.payment_method_value, lobjLTC.ibusProviderOrgPlan.icdoOrgPlan.org_id,IsMedicare: false);

                        lobjLTC.GetMonthlyPremiumAmount();
                    }
                }
            }
        }

        // PIR 8164 & 8165 Ignore functionality on detail
        public void UpdateIBSDetailStatus(string astrDetailStatus)
        {
            DBFunction.DBNonQuery("cdoIbsDetail.UpdateIBSDetailStatus", new object[2] { 
                                    astrDetailStatus, icdoIbsHeader.ibs_header_id },
                                    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
        }
		//PIR 24918 Update IBS detail to ignore
        public void UpdateIBSDetailStatusReviewAndValidToIgnore(int aintPersonAccountId, DateTime adtmPayPeriod)
        {
            DBFunction.DBNonQuery("cdoIbsDetail.UpdateIBSDetailStatusToIgnore", new object[2] {
                                    aintPersonAccountId, adtmPayPeriod },
                                    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
        }
    }
}
