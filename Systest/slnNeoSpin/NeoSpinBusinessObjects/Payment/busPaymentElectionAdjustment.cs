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
using NeoSpin.DataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// Class NeoSpin.BusinessObjects.busPaymentElectionAdjustment:
    /// Inherited from busPaymentElectionAdjustmentGen, the class is used to customize the business object busPaymentElectionAdjustmentGen.
    /// </summary>
    [Serializable]
    public class busPaymentElectionAdjustment : busPaymentElectionAdjustmentGen
    {
        /// <summary>
        /// Method to insert a new Payment election adjustment
        /// </summary>
        /// <param name="aintIBSHeaderID">IBS Header ID</param>
        /// <param name="aintPersonAccountID">Person Account ID</param>
        /// <param name="astrIBSAdjustmentStatus">Adjustment status</param>
        /// <param name="aintPayeeAccountID">Payee account id</param>
        /// <param name="astrIBSModeofPayment">Mode of payment</param>
        /// <param name="astrAdjustmentRepaymentType">Adjustment Repayment type</param>
        /// <param name="adecTotalDueAmount">Total due amount</param>
        public int InsertNewAdjustmentPaymentElection(int aintIBSHeaderID, int aintPersonAccountID, string astrIBSAdjustmentStatus, int aintPayeeAccountID,
            string astrIBSModeofPayment, string astrAdjustmentRepaymentType, decimal adecTotalDueAmount, DateTime adtApprovedDate, int aintProviderOrgID)
        {
            icdoPaymentElectionAdjustment = new cdoPaymentElectionAdjustment();
            icdoPaymentElectionAdjustment.ibs_header_id = aintIBSHeaderID;
            icdoPaymentElectionAdjustment.person_account_id = aintPersonAccountID;
            icdoPaymentElectionAdjustment.status_value = astrIBSAdjustmentStatus;
            icdoPaymentElectionAdjustment.payee_account_id = aintPayeeAccountID;
            icdoPaymentElectionAdjustment.payment_option_value = astrIBSModeofPayment;
            icdoPaymentElectionAdjustment.repayment_type_value = astrAdjustmentRepaymentType;
            icdoPaymentElectionAdjustment.total_adjustment_amount = adecTotalDueAmount;
            icdoPaymentElectionAdjustment.monthly_amount = adecTotalDueAmount;
            icdoPaymentElectionAdjustment.approved_date = adtApprovedDate;
            icdoPaymentElectionAdjustment.provider_org_id = aintProviderOrgID;
            icdoPaymentElectionAdjustment.Insert();
            return icdoPaymentElectionAdjustment.payment_election_adjustment_id;
        }

        //Property to contain person account bus. object
        public busPersonAccount ibusPersonAccount { get; set; }
        /// <summary>
        /// Method to laod person account object
        /// </summary>
        public void LoadPersonAccount()
        {
            if (ibusPersonAccount == null)
                ibusPersonAccount = new busPersonAccount();

            ibusPersonAccount.FindPersonAccount(icdoPaymentElectionAdjustment.person_account_id);
        }

        //Property to contain payee account object
        public busPayeeAccount ibusPayeeAccount { get; set; }
        /// <summary>
        /// Method to load payee account
        /// </summary>
        public void LoadPayeeAccount()
        {
            if (ibusPayeeAccount == null)
                ibusPayeeAccount = new busPayeeAccount();

            ibusPayeeAccount.FindPayeeAccount(icdoPaymentElectionAdjustment.payee_account_id);
        }

        public override void BeforePersistChanges()
        {
            if (string.IsNullOrEmpty(icdoPaymentElectionAdjustment.status_value))
                icdoPaymentElectionAdjustment.status_value = busConstant.IBSAdjustmentStatusPending;

            if (icdoPaymentElectionAdjustment.repayment_type_value == busConstant.IBSAdjustmentRepaymentTypeLumpSum)
                icdoPaymentElectionAdjustment.monthly_amount = icdoPaymentElectionAdjustment.total_adjustment_amount;
            else if (icdoPaymentElectionAdjustment.repayment_type_value == busConstant.IBSAdjustmentRepaymentType3Installment)
                icdoPaymentElectionAdjustment.monthly_amount = Math.Round(icdoPaymentElectionAdjustment.total_adjustment_amount / 3, 2, MidpointRounding.AwayFromZero);

            base.BeforePersistChanges();
        }

        /// <summary>
        /// Method invoked when approve button is clicked
        /// </summary>
        /// <returns>Arraylist contain the object</returns>
        public ArrayList btn_Approve_Click()
        {
            ArrayList larrList = new ArrayList();
            utlError lobjError = new utlError();
            this.ValidateHardErrors(utlPageMode.All);
            if (iarrErrors.Count == 0)
            {
                if (icdoPaymentElectionAdjustment.payment_option_value == busConstant.IBSModeOfPaymentPensionCheck)
                {
                    CreatePAPITItem();
                }
                //prod pir 5855
                //add the adjustment amount to ibs person summary on approval
                //--start--//
               if (ibusLastPostedRegularIBSHeader == null)
                    LoadLastPostedRegularIBSHeader();
               if (ibusPersonAccount == null)
                   LoadPersonAccount();
                if (ibusLastPostedRegularIBSHeader == null)
                    LoadLastPostedRegularIBSHeader();
                DateTime ldtNextBenefitPaymentDate = busPayeeAccountHelper.GetLastBenefitPaymentDate().AddMonths(1);
                //as per discussion with Satya and Ragavan on March 4, 2011
                //need to update ibs person summary for Lump sum 
                //for installment we need to update ibs person summary only if last ibs billing date = next benefit payment date for pension check payment option
                if (icdoPaymentElectionAdjustment.repayment_type_value == busConstant.IBSAdjustmentRepaymentTypeLumpSum ||
                    ((icdoPaymentElectionAdjustment.repayment_type_value == busConstant.IBSAdjustmentRepaymentType3Installment ||
                    icdoPaymentElectionAdjustment.repayment_type_value == busConstant.IBSAdjustmentRepaymentTypeInstallment) &&
                    icdoPaymentElectionAdjustment.payment_option_value != busConstant.IBSModeOfPaymentPensionCheck) ||
                    ((icdoPaymentElectionAdjustment.repayment_type_value == busConstant.IBSAdjustmentRepaymentType3Installment ||
                    icdoPaymentElectionAdjustment.repayment_type_value == busConstant.IBSAdjustmentRepaymentTypeInstallment) &&
                    icdoPaymentElectionAdjustment.payment_option_value == busConstant.IBSModeOfPaymentPensionCheck &&
                    ibusLastPostedRegularIBSHeader.icdoIbsHeader.billing_month_and_year.GetFirstDayofCurrentMonth() == ldtNextBenefitPaymentDate.GetFirstDayofCurrentMonth()))
                {
                busIbsPersonSummary lobjPersonSummary = new busIbsPersonSummary();
                //PIR 23336
                int lintPersonID = ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMedicarePartD ? GetMemberPersonIDForMedicare() : ibusPersonAccount.icdoPersonAccount.person_id;

                if (lobjPersonSummary.FindIbsPersonSummaryByIbsHeaderAndPerson(ibusLastPostedRegularIBSHeader.icdoIbsHeader.ibs_header_id, lintPersonID))
                {
                    lobjPersonSummary.icdoIbsPersonSummary.adjustment_amount += icdoPaymentElectionAdjustment.monthly_amount;
                    lobjPersonSummary.icdoIbsPersonSummary.Update();
                }
                //PROD PIR 5415
                //need to insert new ibs person summary if not available
                else
                {
                    lobjPersonSummary.icdoIbsPersonSummary.adjustment_amount += icdoPaymentElectionAdjustment.monthly_amount;
                    lobjPersonSummary.icdoIbsPersonSummary.ibs_header_id = ibusLastPostedRegularIBSHeader.icdoIbsHeader.ibs_header_id;
                    lobjPersonSummary.icdoIbsPersonSummary.person_id = lintPersonID;
                    lobjPersonSummary.icdoIbsPersonSummary.Insert();
                }               
                //--end--//
                }
                icdoPaymentElectionAdjustment.approved_date = DateTime.Now;
                icdoPaymentElectionAdjustment.status_value = busConstant.IBSAdjustmentStatusApproved;
                icdoPaymentElectionAdjustment.Update();
                icdoPaymentElectionAdjustment.status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1710, icdoPaymentElectionAdjustment.status_value);
                this.EvaluateInitialLoadRules();
                larrList.Add(this);
            }
            else
            {
                foreach (utlError lobjErr in iarrErrors)
                    larrList.Add(lobjErr);
            }
            return larrList;
        }

        /// <summary>
        /// Method invoked when cancel button is clicked
        /// </summary>
        /// <returns>Arraylist contain the object</returns>
        public ArrayList btn_Cancel_Click()
        {
            ArrayList larrList = new ArrayList();
            utlError lobjError = new utlError();
            if (ibusLastPostedRegularIBSHeader == null)
                LoadLastPostedRegularIBSHeader();
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            busIbsPersonSummary lobjIBSPersonSummary = new busIbsPersonSummary();
            //PIR 23336
            int lintPersonID = ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMedicarePartD ? GetMemberPersonIDForMedicare() : ibusPersonAccount.icdoPersonAccount.person_id;

            if (lobjIBSPersonSummary.FindIbsPersonSummaryByIbsHeaderAndPerson(ibusLastPostedRegularIBSHeader.icdoIbsHeader.ibs_header_id, lintPersonID))
            {
                lobjIBSPersonSummary.icdoIbsPersonSummary.adjustment_amount += icdoPaymentElectionAdjustment.total_adjustment_amount;
                lobjIBSPersonSummary.icdoIbsPersonSummary.Update();
            }
            icdoPaymentElectionAdjustment.status_value = busConstant.IBSAdjustmentStatusCancelled;
            icdoPaymentElectionAdjustment.Update();
            icdoPaymentElectionAdjustment.status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1710, icdoPaymentElectionAdjustment.status_value);
            this.EvaluateInitialLoadRules();
            larrList.Add(this);

            return larrList;
        }

        /// <summary>
        /// Function to check whether the payee account entered is valid and is in recv. or appr. status
        /// </summary>
        /// <returns>boolean value</returns>
        public bool IsPayeeAccountInvalid()
        {
            bool lblnResult = false;
            if (icdoPaymentElectionAdjustment.payment_option_value == busConstant.IBSModeOfPaymentPensionCheck)
            {
                if (ibusPayeeAccount == null)
                    LoadPayeeAccount();
                if (ibusPersonAccount == null)
                    LoadPersonAccount();
                if (ibusPayeeAccount.ibusPayeeAccountActiveStatus == null)
                    ibusPayeeAccount.LoadActivePayeeStatusAsofNextBenefitPaymentDate();
                if (ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMedicarePartD)
                {
                    //PIR 23336
                    int lintMemberPersonID = GetMemberPersonIDForMedicare();
                    if (ibusPayeeAccount.icdoPayeeAccount.payee_perslink_id != lintMemberPersonID)
                    {
                        lblnResult = true;
                    }
                    else
                    {
                        string lstrData2 = busGlobalFunctions.GetData2ByCodeValue(2203, ibusPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value, iobjPassInfo);
                        if (lstrData2 != busConstant.PayeeAccountStatusApproved && lstrData2 != busConstant.PayeeAccountStatusReceiving)
                        {
                            lblnResult = true;
                        }
                    }
                }
                else
                {
                    if (ibusPayeeAccount.icdoPayeeAccount.payee_perslink_id != ibusPersonAccount.icdoPersonAccount.person_id)
                    {
                        lblnResult = true;
                    }
                    else
                    {
                        string lstrData2 = busGlobalFunctions.GetData2ByCodeValue(2203, ibusPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value, iobjPassInfo);
                        if (lstrData2 != busConstant.PayeeAccountStatusApproved && lstrData2 != busConstant.PayeeAccountStatusReceiving)
                        {
                            lblnResult = true;
                        }
                    }
                }
            }
            return lblnResult;
        }

        /// <summary>
        /// Function to validate monthly amount against benefit amount for payee account
        /// </summary>
        /// <returns>boolean value</returns>
        public bool IsMonthlyAmountGreaterThanBenefitAmount()
        {
            bool lblnResult = false;
            if (icdoPaymentElectionAdjustment.payment_option_value == busConstant.IBSModeOfPaymentPensionCheck)
            {
                if (ibusPayeeAccount == null)
                    LoadPayeeAccount();
                ibusPayeeAccount.LoadBenefitAmount();
                if (icdoPaymentElectionAdjustment.repayment_type_value == busConstant.IBSAdjustmentRepaymentType3Installment)
                    icdoPaymentElectionAdjustment.monthly_amount = Math.Round(icdoPaymentElectionAdjustment.total_adjustment_amount / 3, 2, MidpointRounding.AwayFromZero);
                if (ibusPayeeAccount.idecBenefitAmount < icdoPaymentElectionAdjustment.monthly_amount)
                {
                    lblnResult = true;
                }
            }
            return lblnResult;
        }

        /// <summary>
        /// Method to create PAPIT item
        /// </summary>
        public void CreatePAPITItem()
        {
            LoadPayeeAccount();
            ibusPayeeAccount.LoadNexBenefitPaymentDate();
            if (ibusPersonAccount == null)
                LoadPersonAccount();

            string lstrItemType = string.Empty;
            //int lintNoofMonths = 0;
            DataTable ldtCodeValue = iobjPassInfo.isrvDBCache.GetCodeValues(1711);
            DataRow[] ldrCodeValue = ldtCodeValue.FilterTable(busConstant.DataType.String, "data2", ibusPersonAccount.icdoPersonAccount.plan_id.ToString());

            if (ldrCodeValue != null && ldrCodeValue.Count() > 0)
            {
                lstrItemType = ldrCodeValue[0]["data1"].ToString();
                //lintNoofMonths = Convert.ToInt32(Math.Floor(icdoPaymentElectionAdjustment.total_adjustment_amount / icdoPaymentElectionAdjustment.monthly_amount));
                //if ((icdoPaymentElectionAdjustment.total_adjustment_amount % icdoPaymentElectionAdjustment.monthly_amount) == 0)
                //{
                //    lintNoofMonths -= 1;
                //}
                //decimal ldecExistingPAPITAmount = ibusPayeeAccount.LoadLatestPAPITAmount(lstrItemType);
                //ibusPayeeAccount.CreatePayeeAccountPaymentItemType(lstrItemType, (icdoPaymentElectionAdjustment.monthly_amount + ldecExistingPAPITAmount), string.Empty,
                //    icdoPaymentElectionAdjustment.provider_org_id, ibusPayeeAccount.idtNextBenefitPaymentDate, DateTime.MinValue, false);

                busPayeeAccountPaymentItemType lobjPAPIT = ibusPayeeAccount.GetLatestPayeeAccountPaymentItemTypeByVendorOrgID(lstrItemType, icdoPaymentElectionAdjustment.provider_org_id);
                if (lobjPAPIT != null)
                {
                    ibusPayeeAccount.CreatePayeeAccountPaymentItemType(lstrItemType, (icdoPaymentElectionAdjustment.monthly_amount + lobjPAPIT.icdoPayeeAccountPaymentItemType.amount),
                            string.Empty, icdoPaymentElectionAdjustment.provider_org_id, ibusPayeeAccount.idtNextBenefitPaymentDate, DateTime.MinValue, false, true);                    
                }
                else
                {
                    ibusPayeeAccount.CreatePayeeAccountPaymentItemType(lstrItemType, icdoPaymentElectionAdjustment.monthly_amount, string.Empty,
                                       icdoPaymentElectionAdjustment.provider_org_id, ibusPayeeAccount.idtNextBenefitPaymentDate, DateTime.MinValue, true);
                }
            }
        }

        public decimal idecTotalPaidAmount { get; set; }

        public void LoadTotalPaidAmount()
        {
            DataTable ldtInsuranceContribution = Select<cdoPersonAccountInsuranceContribution>(new string[2] { "person_account_id", "transaction_type_value" },
                new object[2] { icdoPaymentElectionAdjustment.person_account_id, busConstant.PersonAccountTransactionTypeIBSAdjPayment }, null, null);

            if (ldtInsuranceContribution.Rows.Count > 0)
                idecTotalPaidAmount = ldtInsuranceContribution.AsEnumerable().Sum(o => o.Field<decimal>("paid_premium_amount"));
        }

        public busIbsHeader ibusLastPostedRegularIBSHeader { get; set; }
        public void LoadLastPostedRegularIBSHeader()
        {
            ibusLastPostedRegularIBSHeader = new busIbsHeader { icdoIbsHeader = new cdoIbsHeader() };
            DataTable ldtbList = busNeoSpinBase.SelectWithOperator<cdoIbsHeader>(
              new string[2] { "report_type_value", "report_status_value" },
               new string[2] { "=", "=" },
              new object[2] { busConstant.IBSHeaderReportTypeRegular, busConstant.IBSHeaderStatusPosted }, "billing_month_and_year desc");
            if (ldtbList.Rows.Count > 0)
            {
                ibusLastPostedRegularIBSHeader.icdoIbsHeader.LoadData(ldtbList.Rows[0]);
            }
        }

        #region Correspondence

        public Collection<busIbsDetail> iclbIBSDetail { get; set; }

        public void LoadIBSDetail()
        {
            iclbIBSDetail = new Collection<busIbsDetail>();
            if (icdoPaymentElectionAdjustment.payment_election_adjustment_id > 0)
            { 
				DataTable ldtIBSDetails = Select<cdoIbsDetail>(new string[1] { enmPaymentElectionAdjustment.payment_election_adjustment_id.ToString() }, 
                                                             new object[1] { icdoPaymentElectionAdjustment.payment_election_adjustment_id }, null, "billing_month_and_year");
                iclbIBSDetail = GetCollection<busIbsDetail>(ldtIBSDetails, "icdoIbsDetail");
            }
        }

        public Collection<busIbsDetail> iclbIBSDetailGrouped { get; set; }
        public void LoadIBSDetailForCorrespondence()
        {
            if (iclbIBSDetail == null)
                LoadIBSDetail();
            iclbIBSDetailGrouped = new Collection<busIbsDetail>();
            DateTime ldtDate = DateTime.MinValue;
            busIbsDetail lobjNewDetail = new busIbsDetail { icdoIbsDetail = new cdoIbsDetail() };
            foreach (busIbsDetail lobjDetail in iclbIBSDetail)
            {
                if (ldtDate != lobjDetail.icdoIbsDetail.billing_month_and_year)
                {
                    if (lobjNewDetail.icdoIbsDetail.billing_month_and_year != DateTime.MinValue)
                    {
                        iclbIBSDetailGrouped.Add(lobjNewDetail);
                    }
                    lobjNewDetail = new busIbsDetail { icdoIbsDetail = new cdoIbsDetail() };
                    lobjNewDetail.icdoIbsDetail.billing_month_and_year = lobjDetail.icdoIbsDetail.billing_month_and_year;
                }
                if (lobjDetail.icdoIbsDetail.member_premium_amount >= 0)
                    lobjNewDetail.icdoIbsDetail.idecCorrectPremium += lobjDetail.icdoIbsDetail.member_premium_amount;
                else
                    lobjNewDetail.icdoIbsDetail.idecChargedPremium += Math.Abs(lobjDetail.icdoIbsDetail.member_premium_amount);
                ldtDate = lobjDetail.icdoIbsDetail.billing_month_and_year;
            }
            if (lobjNewDetail.icdoIbsDetail.billing_month_and_year != DateTime.MinValue)
            {
                iclbIBSDetailGrouped.Add(lobjNewDetail);
            }
        }

        public decimal idecTotalChargedPremium { get; set; }

        public decimal idecTotalCorrectPremium { get; set; }

        public override void LoadCorresProperties(string astrTemplateName)
        {
            if (iclbIBSDetailGrouped == null)
                LoadIBSDetailForCorrespondence();
            idecTotalChargedPremium = iclbIBSDetailGrouped.Sum(o => o.icdoIbsDetail.idecChargedPremium);
            idecTotalCorrectPremium = iclbIBSDetailGrouped.Sum(o => o.icdoIbsDetail.idecCorrectPremium);
        }

        public override busBase GetCorPerson()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPerson == null)
                ibusPersonAccount.LoadPerson();
            if (ibusPersonAccount.ibusPlan == null)
                ibusPersonAccount.LoadPlan();
            return ibusPersonAccount.ibusPerson;
        }

        #endregion


        //PIR 23336
        public int GetMemberPersonIDForMedicare()
        {
            DataTable ldtbList = Select<cdoPersonAccountMedicarePartDHistory>(
                                                 new string[1] { enmPersonAccountMedicarePartDHistory.person_account_id.ToString() },
                                                 new object[1] { icdoPaymentElectionAdjustment.person_account_id }, 
                                                 null, enmPersonAccountMedicarePartDHistory.person_account_medicare_part_d_history_id.ToString() + " DESC");

            return ldtbList.Rows.Count > 0 ? ldtbList.Rows[0].Field<int>(enmPersonAccountMedicarePartDHistory.member_person_id.ToString()) : 0;
        }
    }
}
