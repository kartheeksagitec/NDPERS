#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Data;
using Sagitec.Common;
using Sagitec.CustomDataObjects;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;
using System.Linq;
using System.Collections.Generic;

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busServicePurchaseHeader : busServicePurchaseHeaderGen
    {
        private Collection<busServicePurchaseAmortizationSchedule> _iclbServicePurchaseAmortizationSchedule;
        public Collection<busServicePurchaseAmortizationSchedule> iclbServicePurchaseAmortizationSchedule
        {

            get
            {
                return _iclbServicePurchaseAmortizationSchedule;
            }
            set
            {
                _iclbServicePurchaseAmortizationSchedule = value;
            }
        }

        private Collection<busServicePurchaseAmortizationSchedule> _iclbServicePurchaseFutureSchedule;
        public Collection<busServicePurchaseAmortizationSchedule> iclbServicePurchaseFutureSchedule
        {

            get
            {
                return _iclbServicePurchaseFutureSchedule;
            }
            set
            {
                _iclbServicePurchaseFutureSchedule = value;
            }
        }

        private Collection<busServicePurchasePaymentAllocation> _iclbAvailableRemittances;
        public Collection<busServicePurchasePaymentAllocation> iclbAvailableRemittances
        {
            get
            {
                return _iclbAvailableRemittances;
            }
            set
            {
                _iclbAvailableRemittances = value;
            }
        }
        private Collection<busServicePurchasePaymentAllocation> _iclbUnPostedPaymentAllocation;
        public Collection<busServicePurchasePaymentAllocation> iclbUnPostedPaymentAllocation
        {
            get
            {
                return _iclbUnPostedPaymentAllocation;
            }
            set
            {
                _iclbUnPostedPaymentAllocation = value;
            }
        }

        private Collection<busServicePurchasePaymentAllocation> _iclbAllocatedRemittance;
        public Collection<busServicePurchasePaymentAllocation> iclbAllocatedRemittance
        {
            get
            {
                return _iclbAllocatedRemittance;
            }
            set
            {
                _iclbAllocatedRemittance = value;
            }
        }

        private Collection<busServicePurchasePaymentAllocation> _iclbAllocatedPaymentPurchaseList;
        public Collection<busServicePurchasePaymentAllocation> iclbAllocatedPaymentPurchaseList
        {
            get
            {
                return _iclbAllocatedPaymentPurchaseList;
            }
            set
            {
                _iclbAllocatedPaymentPurchaseList = value;
            }
        }

        private busServicePurchaseDetail _ibusPrimaryServicePurchaseDetail;
        public busServicePurchaseDetail ibusPrimaryServicePurchaseDetail
        {
            get
            {
                return _ibusPrimaryServicePurchaseDetail;
            }
            set
            {
                _ibusPrimaryServicePurchaseDetail = value;
            }
        }

        private busPerson _ibusPerson;
        public busPerson ibusPerson
        {
            get
            {
                return _ibusPerson;
            }
            set
            {
                _ibusPerson = value;
            }
        }

        private busPlan _ibusPlan;
        public busPlan ibusPlan
        {
            get
            {
                return _ibusPlan;
            }
            set
            {
                _ibusPlan = value;
            }
        }

        //Get Last Paid Date for correspondence PUR-8300
        public string idtLastPaidDate
        {
            get
            {
                DateTime ldtLastPaidDate = DateTime.MinValue;
                DataTable ldtbGetLastPaidDate = Select<cdoServicePurchasePaymentAllocation>(new string[1] { "service_purchase_header_id" },
                new object[1] { icdoServicePurchaseHeader.service_purchase_header_id }, null, "payment_date desc");
                if (ldtbGetLastPaidDate.Rows.Count > 0)
                {
                    if (ldtbGetLastPaidDate.Rows[0]["payment_date"].ToString() != null)
                    {
                        ldtLastPaidDate = Convert.ToDateTime(ldtbGetLastPaidDate.Rows[0]["payment_date"]);
                    }
                }
                return ldtLastPaidDate.ToString(busConstant.DateFormatLongDate);
            }
        }

        public bool _lblnInstallmentPreTaxConditionFailed;
        public bool lblnInstallmentPreTaxConditionFailed
        {
            get { return _lblnInstallmentPreTaxConditionFailed; }
            set { _lblnInstallmentPreTaxConditionFailed = value; }
        }

        public bool _lblnIsFirstPaymentIsRHICLumpSum;
        public bool lblnIsFirstPaymentIsRHICLumpSum
        {
            get { return _lblnIsFirstPaymentIsRHICLumpSum; }
            set { _lblnIsFirstPaymentIsRHICLumpSum = value; }
        }

        private decimal _idecPayOffAmount;
        public decimal idecPayOffAmount
        {
            get { return _idecPayOffAmount; }
            set { _idecPayOffAmount = value; }
        }

        private decimal _idecExpectedInstallAmtOldValue;
        public decimal idecExpectedInstallAmtOldValue
        {
            get { return _idecExpectedInstallAmtOldValue; }
            set { _idecExpectedInstallAmtOldValue = value; }
        }

        private decimal _iintNoOfPaymentOldValue;
        public decimal iintNoOfPaymentOldValue
        {
            get { return _iintNoOfPaymentOldValue; }
            set { _iintNoOfPaymentOldValue = value; }
        }

        private int _iintTotalNoOfFuturePayments;
        public int iintTotalNoOfFuturePayments
        {
            get { return _iintTotalNoOfFuturePayments; }
            set { _iintTotalNoOfFuturePayments = value; }
        }

        public bool _iblnIsExpectedPaymentAmountTooSmall;
        public bool iblnIsExpectedPaymentAmountTooSmall
        {
            get { return _iblnIsExpectedPaymentAmountTooSmall; }
            set { _iblnIsExpectedPaymentAmountTooSmall = value; }
        }

        private decimal _idecExpectedInstallmentAmount;
        public decimal idecExpectedInstallmentAmount
        {
            get { return _idecExpectedInstallmentAmount; }
            set { _idecExpectedInstallmentAmount = value; }
        }

        public string idecPayOffAmountFormatted
        {
            get
            {
                return _idecPayOffAmount.ToString(".00");
            }
        }

        //This boolean property will determine whether the user is changed the No of Payments or Expected Installment Amount
        public bool iblnIsPaymentElectionChanged;
        public decimal HigherPurchaseCost
        {
            get
            {
                // In case of "Consolidated Service Purchase" the Total purchase cost in header is equal to 
                // "Total Purchase cost" in Detail or "Refund Plus Interest" in detail whichever is higher.
                //
                if (icdoServicePurchaseHeader.service_purchase_type_value == busConstant.Service_Purchase_Type_Consolidated_Purchase)
                {
                    return Math.Max(TotalPurchaseCost,
                                    ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.total_refund_and_interest);
                }
                //PIR-11115 (PIR-10003)
                if (icdoServicePurchaseHeader.service_purchase_type_value == busConstant.Service_Purchase_Type_USERRA_Military_Service)
                {
                    decimal ldecHigherPurchaseCost = 0;
                    LoadServicePurchaseDetail();
                    if (ibusPrimaryServicePurchaseDetail.iclbServicePurchaseDetailUserra != null)
                    {
                        foreach (busServicePurchaseDetailUserra ibusServicePurchaseDetailUserra in ibusPrimaryServicePurchaseDetail.iclbServicePurchaseDetailUserra)
                        {
                            ldecHigherPurchaseCost += ibusServicePurchaseDetailUserra.icdoServicePurchaseDetailUserra.TotalPurchaseCost; 		// Service Purchase Backlog PIR-10003 (PIR-11115)
                        }
                        return ldecHigherPurchaseCost;
                    }
                }
                return TotalPurchaseCost;
            }
        }       
        //PIR 488
        //BR - 044 - 68           
        public decimal idecContractInterestAmount
        {
            get
            {
                decimal ldecContractInterestAmount = 0.00M;
                ldecContractInterestAmount = icdoServicePurchaseHeader.total_contract_amount - HigherPurchaseCost;
                return ldecContractInterestAmount;
            }
        }
        private ObjectState ienuServicePurchaseHeaderObjectState = ObjectState.None;
        public decimal TotalPurchaseCost
        {
            get
            {
                if (ibusPrimaryServicePurchaseDetail == null)
                    LoadServicePurchaseDetail();
                return ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.total_purchase_cost;
            }
        }

        public decimal TotalTimeToPurchase
        {
            get
            {
                if (ibusPrimaryServicePurchaseDetail == null)
                    LoadServicePurchaseDetail();
                //For Sick Leave, fraction of months are deemed to be a full month
                if (icdoServicePurchaseHeader.service_purchase_type_value == busConstant.Service_Purchase_Type_Unused_Sick_Leave)
                {
                    return Math.Ceiling(ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.total_time_to_purchase);
                }

                return Math.Round(ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.total_time_to_purchase, MidpointRounding.AwayFromZero);
            }
        }
		//PIR 17204
        public decimal idecTotalMemberPSC
        {
            get
            {
                //PIR 25929 Load Total PSC when generating the corr so that correct PSC is being shown on screen
                //This includes the Total time to purchase as it is already posted on Retirement contributions before generating PUR-8313
                ibusPersonAccount.LoadTotalPSC();
                return Math.Round(ibusPersonAccount.icdoPersonAccount.Total_PSC, MidpointRounding.AwayFromZero);
            }
        }
        //PIR 26044 : Bookmark for PUR-8303 (show total PSC upto current date + total time to purchase in months)
        public decimal idecTotalMemberPSCForScreen
        {
            get
            {
                ibusPersonAccount.LoadTotalPSC();
                if (icdoServicePurchaseHeader.action_status_value == busConstant.Service_Purchase_Action_Status_Paid_In_Full)
                {
                    return (Math.Round(ibusPersonAccount.icdoPersonAccount.Total_PSC, MidpointRounding.AwayFromZero));
                }
                else
                {
                    return ((Math.Round(ibusPersonAccount.icdoPersonAccount.Total_PSC, MidpointRounding.AwayFromZero)) + TotalTimeToPurchase);
                }

            }
        }

        //PIR 26044 : Current total PSC and total VSC to be loaded and shown on screen dynamically
        public string idecCurrentTotalMemberPSCForScreen
        {
            get
            {
                ibusPersonAccount.LoadTotalPSC();
                if (ibusPersonAccount.icdoPersonAccount.Total_PSC < 0)
                    return String.Format("{0} Years {1} Months", Math.Ceiling(ibusPersonAccount.icdoPersonAccount.Total_PSC / 12).ToString(),
                                     Math.Round((ibusPersonAccount.icdoPersonAccount.Total_PSC % 12), 4, MidpointRounding.AwayFromZero).ToString());

                return String.Format("{0} Years {1} Months", Math.Floor(ibusPersonAccount.icdoPersonAccount.Total_PSC / 12).ToString(),
                                         Math.Round((ibusPersonAccount.icdoPersonAccount.Total_PSC % 12), 4, MidpointRounding.AwayFromZero).ToString());
            }
        }
        public string idecCurrentTotalMemberVSCForScreen
        {
            get
            {
                ibusPersonAccount.LoadTotalVSC(ablnExcludeTFFRTIAA: true);
                if (ibusPersonAccount.icdoPersonAccount.Total_VSC < 0)
                    return String.Format("{0} Years {1} Months", Math.Ceiling(ibusPersonAccount.icdoPersonAccount.Total_VSC / 12).ToString(),
                                     Math.Round((ibusPersonAccount.icdoPersonAccount.Total_VSC % 12), 4, MidpointRounding.AwayFromZero).ToString());

                return String.Format("{0} Years {1} Months", Math.Floor(ibusPersonAccount.icdoPersonAccount.Total_VSC / 12).ToString(),
                                     Math.Round((ibusPersonAccount.icdoPersonAccount.Total_VSC % 12), 4, MidpointRounding.AwayFromZero).ToString());
            }
        }

        // Prem : This variable is inserted in the cdo to reduce the dependency on this field in other
        // screens since it was initially part of do, do to changes in the requirement we have made 
        // it as derived field to reduce the impact across other areas that access this field.

        //Changes Done By : Jeeva S        
        //PIR 628
        //Date of Purchase is the Expected Payment Start Date till the first payment comes.
        //Once the First Payment comes in, that will be the payment start date
        //Since business logic involved here, i movied this property from DO to BUS object.
        private DateTime _expected_payment_start_date;
        public DateTime expected_payment_start_date
        {
            get
            {
                _expected_payment_start_date = GetFirstPaymentDate();
                if (_expected_payment_start_date == DateTime.MinValue)
                {
                    _expected_payment_start_date = icdoServicePurchaseHeader.date_of_purchase;
                    //When there is no payment made, and date of purchase also past date, we have generate schedule based on the start date of today
                    if (_expected_payment_start_date.Date < DateTime.Now.Date)
                        _expected_payment_start_date = DateTime.Now;
                }

                if (_expected_payment_start_date.Day <= 15)
                {
                    _expected_payment_start_date = new DateTime(_expected_payment_start_date.Year, _expected_payment_start_date.Month, 15);
                }
                else
                {
                    _expected_payment_start_date = _expected_payment_start_date.AddMonths(1);
                    _expected_payment_start_date = new DateTime(_expected_payment_start_date.Year, _expected_payment_start_date.Month, 15);
                }
                return _expected_payment_start_date;
            }
        }

        // This derived property is used for testing a business rule under "Employer Purchase" in BR-044-038
        public int MemberAgePlusPSCServiceCredit
        {
            get
            {
                int lintResult = icdoServicePurchaseHeader.current_age_year_part;

                //Get the Total PSC
                if (ibusPrimaryServicePurchaseDetail != null)
                {
                    if (ibusPrimaryServicePurchaseDetail.ibusServicePurchaseHeader == null)
                        ibusPrimaryServicePurchaseDetail.ibusServicePurchaseHeader = this;
                    if (ibusPersonAccount == null)
                        LoadPersonAccount();

                    if (ibusPersonAccount.icdoPersonAccount.person_account_id > 0)
                    {
                        if (ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.IsNotNull() && ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.psc > 0)
                            lintResult += busGlobalFunctions.Round(ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.psc / 12);
                        else
                            lintResult += busGlobalFunctions.Round(ibusPersonAccount.icdoPersonAccount.Total_PSC_in_Years); //PIR 18575 - For existing purchases
                    }
                }
                return lintResult;
            }
        }
		//PIR 8912
        public string istrDBPlan
        {
            get
            {
                if (ibusPlan.IsNull()) LoadPlan();
                if (ibusPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueDB || ibusPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueHB)       //PIR 25920  New DC plan
                    return "Benefit";
                if (ibusPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueDC)
                    return "Contribution";
                return string.Empty;
            }
        }      

        //Property to conatin Person Account retirement details which will be used to calculate FAS
        public busPersonAccountRetirement ibusPersonAccountRetirement { get; set; }

        //Variable to contain Page Mode
        public bool iblnPageNewMode = false;
        public DateTime retirement_date_from_portal { get; set; }
        public DateTime termination_date_from_portal { get; set; }
        #region What If Amortization Schedule Properties
        private int _iintWhatIfNoOfPayments;
        public int iintWhatIfNoOfPayments
        {
            get { return _iintWhatIfNoOfPayments; }
            set { _iintWhatIfNoOfPayments = value; }
        }

        private decimal _idecWhatIfExpextedInstallmentAmount;
        public decimal idecWhatIfExpextedInstallmentAmount
        {
            get { return _idecWhatIfExpextedInstallmentAmount; }
            set { _idecWhatIfExpextedInstallmentAmount = value; }
        }

        private string _istrWhatIfPaymentFrequency;
        public string istrWhatIfPaymentFrequency
        {
            get { return _istrWhatIfPaymentFrequency; }
            set { _istrWhatIfPaymentFrequency = value; }
        }

        private decimal _idecWhatIfPaymentAmount;
        public decimal idecWhatIfPaymentAmount
        {
            get { return _idecWhatIfPaymentAmount; }
            set { _idecWhatIfPaymentAmount = value; }
        }

        #endregion

        public ArrayList btnPaymentElectionSave_Click()
        {
            //UAT PIR 934 : Fire the hard Error Validation
            if (!(icdoServicePurchaseHeader.payment_frequency_value == busConstant.ServicePurchasePaymentFrequencyValueMonthly ||
                icdoServicePurchaseHeader.payment_frequency_value == busConstant.ServicePurchasePaymentFrequencyValueOneTimeLumpSumAmt))
            {
                icdoServicePurchaseHeader.number_of_payments = 0;
            }
            ArrayList larrList = new ArrayList();
            ReassignPaymentElectionFields();
            LoadAmortizationSchedule();
            //Setting this Property true becaise even if other fields (Payment Frequency, Pre Tax, Payroll Deductiom changed, we need to persist)
            iblnIsPaymentElectionChanged = true;
            UpdatePaymentElection();
            //PIR 23827 - Fire the hard error validation after calculating number of payments
            ValidateHardErrors(utlPageMode.All);
            if (iarrErrors.Count > 0)
            {
               return iarrErrors;
            }            
            //Update the Workflow Process Instance Parameter (Payroll Deduction)            
            if (ibusBaseActivityInstance.IsNotNull())
                //SetProcessInstanceParameters();
                SetCaseInstanceParameters();
            larrList.Add(this);
            return larrList;
        }

        public ArrayList btnValidate_Click()
        {
            ArrayList larrList = new ArrayList();
            ValidateSoftErrors();
            UpdateValidateStatus();
            larrList.Add(this);
            return larrList;
        }

        public ArrayList btnApprove_Click()
        {
            ArrayList larrList = new ArrayList();
            if (iobjPassInfo.istrUserID == icdoServicePurchaseHeader.created_by)
            {
                utlError lobjError = new utlError();
                lobjError = AddError(1173, "");
                larrList.Add(lobjError);
                return larrList;
            }
            icdoServicePurchaseHeader.action_status_value = busConstant.Service_Purchase_Action_Status_Approved;
            icdoServicePurchaseHeader.Update();
            larrList.Add(this);
            return larrList;
        }

        public void btnClosed_Click()
        {
            decimal idecAdjustedPSC = 0.00M;
            decimal idecAdjustedVSC = 0.00M;
            PostServicePurchaseAdjstClosingOfContract(ref idecAdjustedPSC, ref idecAdjustedVSC);
            icdoServicePurchaseHeader.prorated_psc = icdoServicePurchaseHeader.prorated_psc + idecAdjustedPSC;
            icdoServicePurchaseHeader.prorated_vsc = icdoServicePurchaseHeader.prorated_vsc + idecAdjustedVSC;
            icdoServicePurchaseHeader.service_purchase_adjustment_fraction_psc = idecAdjustedPSC;
            icdoServicePurchaseHeader.service_purchase_adjustment_fraction_vsc = idecAdjustedVSC;
            icdoServicePurchaseHeader.action_status_value = busConstant.Service_Purchase_Action_Status_Closed;
            icdoServicePurchaseHeader.Update();
        }

        public ArrayList btnRecalculate_Click()
        {
            ArrayList larr = new ArrayList();
            _ibusPrimaryServicePurchaseDetail.ibusServicePurchaseHeader = this;
            _ibusPrimaryServicePurchaseDetail.CalculateTimeToPurchaseForUnUsedSickLeave();
            _ibusPrimaryServicePurchaseDetail.CalculateRetirementandRHICCostForUnUsedSickLeave();
            _ibusPrimaryServicePurchaseDetail.CalculateTotalPurchaseCostForUnUsedSickLeave();
            //PIR 775 : Update the Total Contract Amount after Click the Recaluclate Button
            CalculateTotalContractAmount();
            larr.Add(this);
            return larr;
        }

        public ArrayList btnRecalculateAndSave_Click()
        {
            ArrayList larr = new ArrayList();
            _ibusPrimaryServicePurchaseDetail.ibusServicePurchaseHeader = this;
            _ibusPrimaryServicePurchaseDetail.CalculateTimeToPurchaseForUnUsedSickLeave();
            _ibusPrimaryServicePurchaseDetail.CalculateRetirementandRHICCostForUnUsedSickLeave();
            _ibusPrimaryServicePurchaseDetail.CalculateTotalPurchaseCostForUnUsedSickLeave();
            //PIR 775 : Update the Total Contract Amount after Click the Recaluclate Button
            CalculateTotalContractAmount();

            //Persist the data After doing calculation
            _ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.Update();
            icdoServicePurchaseHeader.Update();

            larr.Add(this);
            return larr;
        }

        public ArrayList btnGenerateWhatIfSchedule_Click()
        {
            ArrayList larrList = new ArrayList();
            utlError lobjError = new utlError();
            _iclbServicePurchaseFutureSchedule = busServicePurchaseAmortizationSchedule.CalculateWhatIfAmortizationSchedule(this, iobjPassInfo);
            larrList.Add(this);
            return larrList;
        }

        public ArrayList btnApplyRemittance_Click()
        {
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();

            if (iclbAvailableRemittances == null)
            {
                iclbAvailableRemittances = new Collection<busServicePurchasePaymentAllocation>();
            }
            if (iclbAllocatedPaymentPurchaseList == null)
                LoadAllocatedPurchaseList();
            // Check to see if there are trying to allocate more money than what is available from the remittance
            foreach (busServicePurchasePaymentAllocation lobjServicePurchasePaymentAllocation in iclbAvailableRemittances)
            {
                if (lobjServicePurchasePaymentAllocation.icdoServicePurchasePaymentAllocation.applied_amount > lobjServicePurchasePaymentAllocation.icdoServicePurchasePaymentAllocation.remittance_amount)
                {
                    lobjError = AddError(busConstant.Message_Id_Service_Purchase_Applied_Amount_Greater_Than_Remittance_Amount, "");
                    alReturn.Add(lobjError);
                    return alReturn;
                }

                if (lobjServicePurchasePaymentAllocation.icdoServicePurchasePaymentAllocation.applied_amount > 0 && string.IsNullOrEmpty(lobjServicePurchasePaymentAllocation.icdoServicePurchasePaymentAllocation.service_purchase_payment_class_value))
                {
                    lobjError = AddError(busConstant.Message_Id_Service_Purchase_Applied_Amount_Entered_But_Payment_Class_Not_Selected, "");
                    alReturn.Add(lobjError);
                    return alReturn;
                }
                // If payment class is "Employer Lumpsum" the payor should be employer
                if (!(string.IsNullOrEmpty(lobjServicePurchasePaymentAllocation.icdoServicePurchasePaymentAllocation.service_purchase_payment_class_value))
                && lobjServicePurchasePaymentAllocation.icdoServicePurchasePaymentAllocation.service_purchase_payment_class_value == busConstant.Service_Purchase_Class_Employer_LumpSum
                    && icdoServicePurchaseHeader.payor_value != busConstant.Service_Purchase_Payor_Employer)
                {
                    lobjError = AddError(busConstant.Message_Id_Service_Purchase_Payment_Class_Employer_Lumpsum_But_Payor_Is_Not_Employer, "");
                    alReturn.Add(lobjError);
                    return alReturn;
                }

                // If payor is employer then "payment class" should be Employer LumpSum or Employer Userra Lumpsum
                if (icdoServicePurchaseHeader.payor_value == busConstant.Service_Purchase_Payor_Employer
                    && (!(string.IsNullOrEmpty(lobjServicePurchasePaymentAllocation.icdoServicePurchasePaymentAllocation.service_purchase_payment_class_value))
                    && !(lobjServicePurchasePaymentAllocation.icdoServicePurchasePaymentAllocation.service_purchase_payment_class_value == busConstant.Service_Purchase_Class_Employer_LumpSum
                    || lobjServicePurchasePaymentAllocation.icdoServicePurchasePaymentAllocation.service_purchase_payment_class_value == busConstant.Service_Purchase_Class_Employer_USERRA_LumpSum)))
                {
                    lobjError = AddError(busConstant.Message_Id_Service_Purchase_Payment_Class_Employer_Lumpsum_But_Payor_Is_Not_Employer, "");
                    alReturn.Add(lobjError);
                    return alReturn;
                }
                if (((!(string.IsNullOrEmpty(lobjServicePurchasePaymentAllocation.icdoServicePurchasePaymentAllocation.service_purchase_payment_class_value)))
                    && (lobjServicePurchasePaymentAllocation.icdoServicePurchasePaymentAllocation.service_purchase_payment_class_value != busConstant.Service_Purchase_Class_Employer_Installment_PreTax)) && ((icdoServicePurchaseHeader.pre_tax == busConstant.Flag_Yes) && (icdoServicePurchaseHeader.payroll_deduction == busConstant.Flag_Yes)))
                {
                    lblnInstallmentPreTaxConditionFailed = true;
                }
                if (((!(string.IsNullOrEmpty(lobjServicePurchasePaymentAllocation.icdoServicePurchasePaymentAllocation.service_purchase_payment_class_value)))
                    && (lobjServicePurchasePaymentAllocation.icdoServicePurchasePaymentAllocation.service_purchase_payment_class_value != busConstant.Service_Purchase_Class_Employer_RHIC_Lumpsum)) && ((icdoServicePurchaseHeader.pre_tax == busConstant.Flag_Yes) && (icdoServicePurchaseHeader.payroll_deduction == busConstant.Flag_Yes))
                    && (iclbAllocatedPaymentPurchaseList.Count == 0))
                {
                    _lblnIsFirstPaymentIsRHICLumpSum = true;
                }

                //PIR 752 BR - 109
                if (lobjServicePurchasePaymentAllocation.icdoServicePurchasePaymentAllocation.applied_amount > RoundNormal(_idecPayOffAmount, 2))
                {
                    lobjError = AddError(1148, "");
                    alReturn.Add(lobjError);
                    return alReturn;
                }

                // PIR 728
                if (lobjServicePurchasePaymentAllocation.icdoServicePurchasePaymentAllocation.service_purchase_payment_class_value == busConstant.Service_Purchase_Class_Employer_Rollover)
                {
                    if (CheckMemberEnrolledInJobService())
                    {
                        lobjError = AddError(1138, "");
                        alReturn.Add(lobjError);
                        return alReturn;
                    }
                }

                //UAT PIR 765 - Allow Down Payment only in the first month of payment (less than 15th). Need not to be in the first payment
                if (lobjServicePurchasePaymentAllocation.icdoServicePurchasePaymentAllocation.service_purchase_payment_class_value == busConstant.Service_Purchase_Class_Employer_Down_Payment)
                {
                    DateTime ldtFirstPaymentDate = GetFirstPaymentDate();
                    if (ldtFirstPaymentDate != DateTime.MinValue)
                    {
                        if (ldtFirstPaymentDate.Day <= 15)
                        {
                            ldtFirstPaymentDate = new DateTime(ldtFirstPaymentDate.Year, ldtFirstPaymentDate.Month, 15);
                        }
                        else
                        {
                            ldtFirstPaymentDate = ldtFirstPaymentDate.AddMonths(1);
                            ldtFirstPaymentDate = new DateTime(ldtFirstPaymentDate.Year, ldtFirstPaymentDate.Month, 15);
                        }

                        if (lobjServicePurchasePaymentAllocation.icdoServicePurchasePaymentAllocation.payment_date > ldtFirstPaymentDate)
                        {
                            lobjError = AddError(1170, "");
                            alReturn.Add(lobjError);
                            return alReturn;
                        }
                    }
                }

            }
            ApplyRemittance();
            ValidateSoftErrors();
            UpdateValidateStatus();
            LoadAvailableRemittances();
            LoadUnPostedPaymentAllcoation();
            LoadAmortizationSchedule();
            alReturn.Add(this);
            return alReturn;
        }

        private void ApplyRemittance()
        {
            // Make sure if amount is entered then that record is updated in the database.
            foreach (busServicePurchasePaymentAllocation lobjServicePurchasePaymentAllocation in iclbAvailableRemittances)
            {
                if (lobjServicePurchasePaymentAllocation.icdoServicePurchasePaymentAllocation.applied_amount > 0)
                {
                    lobjServicePurchasePaymentAllocation.icdoServicePurchasePaymentAllocation.Insert();
                    iclbAllocatedPaymentPurchaseList.Add(lobjServicePurchasePaymentAllocation);
                }
            }
        }

        public bool MemberIdIsNotValid()
        {
            bool lblnResult = false;
            if (ibusPerson == null)
                LoadPerson();
            if (ibusPerson.icdoPerson.person_id == 0)
            {
                lblnResult = true;
            }
            return lblnResult;
        }
        public bool IsMemberIsTFFRdual() => busNeoSpinBase.Select("cdoPersonTffrTiaaService.GetMemberTFFRdual", new object[1] { icdoServicePurchaseHeader.person_id }).Rows.Count > 0;
      
        public bool MaxTermForPaymentFrequency()
        {
            // Go ahead and get all the code values tied to the payment frequency selected by the user
            if (icdoServicePurchaseHeader.number_of_payments > 0 && icdoServicePurchaseHeader.payment_frequency_value != "")
            {
                cdoCodeValue lobjCodeValue =
                    busGlobalFunctions.GetCodeValueDetails(busConstant.Service_Purchase_Payment_Frequency_Code_Id,
                                                           icdoServicePurchaseHeader.payment_frequency_value);

                // Get the max term applicable for the payment frequency from the data1 value and see if the user entered
                // value is greater than the max term, then we need to go ahead and throw the error/warning.
                int lintMaxTerm = Convert.ToInt32(lobjCodeValue.data1);
                if (icdoServicePurchaseHeader.number_of_payments > lintMaxTerm)
                    return true;
            }

            return false;
        }

        //UAT PIR 1051 : Warning when the Rollover payment selected, Payment Amount may not exceed Retirement Portion
        public bool IsPaymentAmountExceedsRetPortionForRollover()
        {
            bool lblnResult = false;

            if (iclbAllocatedPaymentPurchaseList == null)
                LoadAllocatedPurchaseList();

            if (ibusPrimaryServicePurchaseDetail == null)
                LoadServicePurchaseDetail();

            foreach (busServicePurchasePaymentAllocation lobjPurchasePayment in _iclbAllocatedPaymentPurchaseList)
            {
                if (lobjPurchasePayment.icdoServicePurchasePaymentAllocation.service_purchase_payment_class_value == busConstant.Service_Purchase_Class_Employer_Rollover)
                {
                    if (lobjPurchasePayment.icdoServicePurchasePaymentAllocation.applied_amount > ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.retirement_purchase_cost)
                    {
                        lblnResult = true;
                        break;
                    }
                }
            }
            return lblnResult;
        }

        public override int PersistChanges()
        {
            int lintResult = 0;
            lintResult = base.PersistChanges();

            // Only in the insert mode, we have to set the headerid value in detail object before inserting it.
            // This code change has been made to accommodate the scenario of "Copy Purchase" and to make sure even when foreign key constraint
            // is implemented in the database our code will work fine.
            if (ienuServicePurchaseHeaderObjectState == ObjectState.Insert)
            {
                if (ibusPrimaryServicePurchaseDetail.iclbServicePurchaseDetailConsolidated != null)
                {
                    foreach (busServicePurchaseDetailConsolidated lobjServicePurchaseDetailConsolidated in ibusPrimaryServicePurchaseDetail.iclbServicePurchaseDetailConsolidated)
                    {
                        lobjServicePurchaseDetailConsolidated.icdoServicePurchaseDetailConsolidated.service_purchase_detail_id =
                            ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.service_purchase_detail_id;

                        UpdateDataObject(lobjServicePurchaseDetailConsolidated.icdoServicePurchaseDetailConsolidated);
                    }
                }

                if (ibusPrimaryServicePurchaseDetail.iclbServicePurchaseDetailUserra != null)
                {
                    foreach (busServicePurchaseDetailUserra lobjServicePurchaseDetailUserra in ibusPrimaryServicePurchaseDetail.iclbServicePurchaseDetailUserra)
                    {
                        lobjServicePurchaseDetailUserra.icdoServicePurchaseDetailUserra.service_purchase_detail_id =
                            ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.service_purchase_detail_id;

                        UpdateDataObject(lobjServicePurchaseDetailUserra.icdoServicePurchaseDetailUserra);
                    }
                }

                // UAT PIR 718/727
                if (icdoServicePurchaseHeader.service_purchase_type_value == busConstant.Service_Purchase_Type_Consolidated_Purchase &&
                    icdoServicePurchaseHeader.is_created_from_portal != busConstant.Flag_Yes) //PIR 19270
                    InsertServicePurchaseHeaderConsolidated();
            }
            ValidateHardErrors(utlPageMode.All);
            return lintResult;
        }

        // UAT PIR 718/727
        public void InsertServicePurchaseHeaderConsolidated()
        {
            if (ibusPerson.IsNull())
                LoadPerson();
            if (ibusPrimaryServicePurchaseDetail.IsNull())
                LoadServicePurchaseDetail();
            if (ibusPerson.iclbRetirementAccount.IsNull())
                ibusPerson.LoadRetirementAccount();

            // Check whether there exists a Withdrawned Person Account exists for the same plan.
            busPersonAccount lobjPersonAccount = ibusPerson.iclbRetirementAccount.Where(
                                            o => o.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementWithDrawn).FirstOrDefault();
            if (lobjPersonAccount != null && lobjPersonAccount.icdoPersonAccount.person_account_id != 0)
            {
                if (ibusPerson.iclbPayeeAccount == null)
                    ibusPerson.LoadPayeeAccount(true);
                // Get the valid Refund Payee Account of this Person.               
                foreach (busPayeeAccount lbusPayeeAccount in ibusPerson.iclbPayeeAccount)
                {
                    if (lbusPayeeAccount.ibusPlan.IsNull())
                        lbusPayeeAccount.LoadPlan();
                    //PIR 23779
                    if (lbusPayeeAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdJobService ||
                        lbusPayeeAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdMain ||
                        lbusPayeeAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdMain2020 ||
                        lbusPayeeAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdHP ||
                        lbusPayeeAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdLE ||
                        lbusPayeeAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdLEWithoutPS ||
                        lbusPayeeAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdBCILawEnf ||
                        lbusPayeeAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdStatePublicSafety || //PIR 25729
                        lbusPayeeAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdNG ||
                        lbusPayeeAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdJudges ||
                        lbusPayeeAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdPriorJudges)
                    {
                        if (lbusPayeeAccount.ibusPayeeAccountActiveStatus.IsNull())
                            lbusPayeeAccount.LoadActivePayeeStatusAsofNextBenefitPaymentDate();
                        if (lbusPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value != busConstant.PayeeAccountStatusCancelled)
                        {
                            // Get All Refund Payee Account of that Member
                            string lstrRefund = busGlobalFunctions.GetData2ByCodeValue(2216, lbusPayeeAccount.icdoPayeeAccount.benefit_option_value, iobjPassInfo);
                            if (lstrRefund == busConstant.Refund)
                            {
                                // UAT PIR ID 262
                                // Get All the 'InPayment' and 'PaidInFull' Consolidated Purchases for the Person.
                                bool lblnIsRefundPayeeAccountNotUsed = true;
                                DataTable ldtbAllConsolidatedPurchase = Select("cdoServicePurchaseDetailConsolidated.GetConsolidatedInpaymentorPaidInfullPurchaseByPerson",
                                                                        new object[2] { icdoServicePurchaseHeader.person_id,
                                                                            lbusPayeeAccount.icdoPayeeAccount.payee_account_id });
                                foreach (DataRow ldtrRow1 in ldtbAllConsolidatedPurchase.Rows)
                                {
                                    busServicePurchaseDetailConsolidated lobjConsolidated = new busServicePurchaseDetailConsolidated
                                    {
                                        icdoServicePurchaseDetailConsolidated = new cdoServicePurchaseDetailConsolidated()
                                    };
                                    lobjConsolidated.icdoServicePurchaseDetailConsolidated.LoadData(ldtrRow1);
                                    if ((busGlobalFunctions.CheckDateOverlapping(lobjConsolidated.icdoServicePurchaseDetailConsolidated.service_purchase_start_date,
                                                                                 lobjConsolidated.icdoServicePurchaseDetailConsolidated.service_purchase_end_date,
                                                                                 lbusPayeeAccount.icdoPayeeAccount.benefit_begin_date,
                                                                                 lbusPayeeAccount.icdoPayeeAccount.benefit_end_date)))
                                    {
                                        lblnIsRefundPayeeAccountNotUsed = false;
                                        break;
                                    }
                                }
                                // Consolidated Purchase of 'InPayment' and 'PaidInFull' exists for the same period.
                                // Refund Purchase should overlap the Date of Purchase.
                                if (lblnIsRefundPayeeAccountNotUsed)
                                {
                                    // Inserts the service purchase detail consolidated record
                                    int lintContributionMonths = 0;
                                    decimal ldecRefundInterest = 0.00M;
                                    if (lbusPayeeAccount.ibusBenefitAccount == null)
                                        lbusPayeeAccount.LoadBenfitAccount();
                                    if (lbusPayeeAccount.iclbPaymentDetails == null)
                                        lbusPayeeAccount.LoadPaymentDetails();

                                    if (lbusPayeeAccount.ibusApplication == null)
                                        lbusPayeeAccount.LoadApplication();

                                    if (lbusPayeeAccount.ibusApplication.iclbBenefitApplicationPersonAccounts == null)
                                        lbusPayeeAccount.ibusApplication.LoadBenefitApplicationPersonAccount();
                                    busBenefitApplicationPersonAccount lobjBAPA = lbusPayeeAccount.ibusApplication.iclbBenefitApplicationPersonAccounts
                                                                                  .Where(o => o.icdoBenefitApplicationPersonAccount.is_application_person_account_flag == busConstant.Flag_Yes)
                                                                                  .FirstOrDefault();
                                    if (lobjBAPA != null)
                                    {

                                        if (lobjBAPA.ibusPersonAccount == null)
                                            lobjBAPA.LoadPersonAccount();

                                        if (lobjBAPA.ibusPersonAccount.ibusPersonAccountRetirement == null)
                                            lobjBAPA.ibusPersonAccount.LoadPersonAccountRetirement();

                                        if (lobjBAPA.ibusPersonAccount.ibusPersonAccountRetirement.ibusPARetirementHistory == null)
                                        {
                                            //lobjBAPA.ibusPersonAccount.ibusPersonAccountRetirement.LoadPreviousEnrolledHistory();
                                            lintContributionMonths = lobjBAPA.ibusPersonAccount.ibusPersonAccountRetirement.LoadPreviousEnrolledHistoryAndCalculateContribMonths();
                                            if(!iblnIsMissingVSCContributions)
                                                IsMissingVSCContributionsForGivenPeriod(lobjBAPA.icdoBenefitApplicationPersonAccount.person_account_id);
                                        }

                                        //PIR: 1962: Interest Amount Calculated for Consolidated Purchase for Refund From Refund Date till Date of Purchase.
                                        if ((lbusPayeeAccount.idecpaidgrossamount > 0)
                                            && (icdoServicePurchaseHeader.date_of_purchase >= lbusPayeeAccount.icdoPayeeAccount.benefit_begin_date))
                                        {
                                            ldecRefundInterest = CalculateInterestForRefundAmount(lbusPayeeAccount.icdoPayeeAccount.benefit_begin_date,
                                                icdoServicePurchaseHeader.expiration_date, lbusPayeeAccount.idecpaidgrossamount); // UAT PIR ID 1050
                                        }
                                        cdoServicePurchaseDetailConsolidated lobjSPDConsolidated = new cdoServicePurchaseDetailConsolidated
                                        {
                                            service_purchase_detail_id = ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.service_purchase_detail_id,
                                            service_credit_type_value = busConstant.Service_Purchase_Type_Previous_Pers_Employment,
                                            service_purchase_start_date = lobjBAPA.ibusPersonAccount.ibusPersonAccountRetirement.ibusPARetirementHistory.icdoPersonAccountRetirementHistory.start_date,
                                            service_purchase_end_date = lobjBAPA.ibusPersonAccount.ibusPersonAccountRetirement.ibusPARetirementHistory.icdoPersonAccountRetirementHistory.end_date,
                                            calculated_time_to_purchase = lintContributionMonths,
                                            refund_date = lbusPayeeAccount.icdoPayeeAccount.benefit_begin_date,
                                            refund_with_interest = lbusPayeeAccount.idecpaidgrossamount + ldecRefundInterest,
                                            org_id = lbusPayeeAccount.ibusBenefitAccount.icdoBenefitAccount.retirement_org_id,
                                            payee_account_id = lbusPayeeAccount.icdoPayeeAccount.payee_account_id
                                        };
                                        lobjSPDConsolidated.Insert();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        //PIR 25420 Check if VSC is missing or not for months between given dates
        public void IsMissingVSCContributionsForGivenPeriod(int lintPersonAccountId)
        {
            DataTable ldtbHistory = Select<cdoPersonAccountRetirementHistory>(
                                    new string[2] { "PERSON_ACCOUNT_ID", "PLAN_PARTICIPATION_STATUS_VALUE" },
                                    new object[2] { lintPersonAccountId, busConstant.PlanParticipationStatusRetirementEnrolled },
                                    null, "PERSON_ACCOUNT_RETIREMENT_HISTORY_ID ASC");
            Collection<busPersonAccountRetirementHistory> lclbRetHist = GetCollection<busPersonAccountRetirementHistory>(ldtbHistory, "icdoPersonAccountRetirementHistory");

            DataTable ldtbRetirementContributions = Select<cdoPersonAccountRetirementContribution>(
                                    new string[1] { "PERSON_ACCOUNT_ID" },
                                    new object[1] { lintPersonAccountId },
                                    null, "RETIREMENT_CONTRIBUTION_ID ASC");
            Collection<busPersonAccountRetirementContribution> lclbRetCont = GetCollection<busPersonAccountRetirementContribution>(ldtbRetirementContributions, "icdoPersonAccountRetirementContribution");
            iblnIsMissingVSCContributions = false;
            if (ldtbHistory.Rows.Count > 0)
            {
                foreach (busPersonAccountRetirementHistory lobjPARetHist in lclbRetHist)
                {
                    if (iblnIsMissingVSCContributions)
                        break;
                    DateTime ldtmCurrentDate = lobjPARetHist.icdoPersonAccountRetirementHistory.start_date.GetFirstDayofCurrentMonth();
                    DateTime ldtmEndDate = lobjPARetHist.icdoPersonAccountRetirementHistory.end_date.GetFirstDayofCurrentMonth();
                    while (ldtmCurrentDate <= ldtmEndDate)
                    {
                        if (lclbRetCont.Where(i => i.icdoPersonAccountRetirementContribution.effective_date.Month == ldtmCurrentDate.Month &&
                                                i.icdoPersonAccountRetirementContribution.effective_date.Year == ldtmCurrentDate.Year).Select(i => i.icdoPersonAccountRetirementContribution.vested_service_credit).Sum() <= 0)
                        {
                            iblnIsMissingVSCContributions = true;
                            break;
                        }
                        ldtmCurrentDate = ldtmCurrentDate.AddMonths(1);
                    }
                }
            }
        }
        public bool iblnIsMissingVSCContributions { get; set; }

        //PIR: 1962: Interest Amount Calculated for Consolidated Purchase for Refund From Refund Date till Date of Purchase.
        public decimal CalculateInterestForRefundAmount(DateTime adtBeginDate, DateTime adtEndDate, decimal adecRefundAmount)
        {
            decimal ldecMemberInterest = 0.00M;
            if (adtBeginDate != DateTime.MinValue && adtEndDate != DateTime.MinValue)
            {
                adtBeginDate = new DateTime(adtBeginDate.Year, adtBeginDate.Month, 1);
                adtEndDate = new DateTime(adtEndDate.Year, adtEndDate.Month, 1);
                //Calculating Actuarial Percentage                      
                DateTime ldtInterestDate = adtBeginDate;
                while (ldtInterestDate <= adtEndDate)
                {
                    decimal ldecInterestRate = busGlobalFunctions.GetCodeValueDetailsfromData2(busConstant.ServicePurchaseContractInterestCodeId, ldtInterestDate, iobjPassInfo);
                    ldecMemberInterest += Convert.ToDecimal(adecRefundAmount * ldecInterestRate) / 12;
                    ldtInterestDate = ldtInterestDate.AddMonths(1);
                }
                ldecMemberInterest = Math.Round(ldecMemberInterest, 2, MidpointRounding.AwayFromZero);
            }
            return ldecMemberInterest;
        }

        public override void BeforePersistChanges()
        {
            if ((icdoServicePurchaseHeader.service_purchase_header_id == 0 && ibusPrimaryServicePurchaseDetail.IsNotNull()
                && ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.IsNotNull() && ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.vsc == 0
                && ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.psc == 0)) //|| icdoServicePurchaseHeader.action_status_value == busConstant.Service_Purchase_Action_Status_Pending)//PIR 26307 - VSC/PSC should be calculated only once
            {
                ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.vsc = icdoServicePurchaseHeader.total_vsc;
                ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.psc = ibusPersonAccount.icdoPersonAccount.Total_PSC;
            }
            //PIR 24243
            if (icdoServicePurchaseHeader.overridden_final_average_salary > 0.0M)
            {
                icdoServicePurchaseHeader.final_average_salary = 0.0M;
            }
                //Method to calculate FAS
                //Load PersonAccountRetirement bus obj using person id and Plan id, then initialize benefit calculation bus obj and calculte FAS
                if (icdoServicePurchaseHeader.final_average_salary == 0.0M && icdoServicePurchaseHeader.overridden_final_average_salary == 0.0M)
                {
                    btnRefreshFAS_Click();
                    iblnPageNewMode = false;
                }
            // Store the Object State in Service Purchase Header to be used in Validate Soft Errors            
            ienuServicePurchaseHeaderObjectState = icdoServicePurchaseHeader.ienuObjectState;

            CalculateDateOfExpiration();
            PopulateUserNameForSuppressWarning();
            SetCurrentInterestRate();

            //Now Framework 4.0, will autmaticaly insert/update the Object at BeforePersistChanges based on the MainCDO
            base.BeforePersistChanges();
        }

        //If Both Payment Frequency and Expected Installment Amount Changed, we need to set the zero to one field. 
        //Also, if the value changes in any one field, we need to set zero for other field.
        private void ReassignPaymentElectionFields()
        {
            if (icdoServicePurchaseHeader.ihstOldValues.Count > 0)
            {
                if (Convert.ToDecimal(icdoServicePurchaseHeader.ihstOldValues["expected_installment_amount"]) != 0.00M)
                {
                    idecExpectedInstallAmtOldValue = Convert.ToDecimal(icdoServicePurchaseHeader.ihstOldValues["expected_installment_amount"]);
                }
                if (Convert.ToInt32(icdoServicePurchaseHeader.ihstOldValues["number_of_payments"]) != 0)
                {
                    iintNoOfPaymentOldValue = Convert.ToDecimal(icdoServicePurchaseHeader.ihstOldValues["number_of_payments"]);
                }

                if (idecExpectedInstallAmtOldValue != icdoServicePurchaseHeader.expected_installment_amount)
                {
                    iblnIsPaymentElectionChanged = true;
                    //Expected Installment Amount changed other than empty or zero, clear the number of payments. so that, amortization schedule will put the data.
                    if (icdoServicePurchaseHeader.expected_installment_amount > 0)
                    {
                        icdoServicePurchaseHeader.number_of_payments = 0;
                    }
                }
                else if (iintNoOfPaymentOldValue != icdoServicePurchaseHeader.number_of_payments)
                {
                    iblnIsPaymentElectionChanged = true;
                    if (icdoServicePurchaseHeader.number_of_payments != 0)
                    {
                        icdoServicePurchaseHeader.expected_installment_amount = 0;
                    }

                }
                else if ((idecExpectedInstallAmtOldValue != icdoServicePurchaseHeader.expected_installment_amount)
                         && (iintNoOfPaymentOldValue != icdoServicePurchaseHeader.number_of_payments))
                {
                    iblnIsPaymentElectionChanged = true;
                    if ((icdoServicePurchaseHeader.expected_installment_amount > 0) && (icdoServicePurchaseHeader.number_of_payments != 0))
                    {
                        icdoServicePurchaseHeader.expected_installment_amount = 0;
                    }
                }
                else if ((idecExpectedInstallAmtOldValue == icdoServicePurchaseHeader.expected_installment_amount)
                         && (iintNoOfPaymentOldValue == icdoServicePurchaseHeader.number_of_payments))
                {
                    //When Both Exists, and No Change, we calculate based on the No of Payments. (This way we can calcuate delay payments also without any issues)
                    if ((icdoServicePurchaseHeader.expected_installment_amount > 0) && (icdoServicePurchaseHeader.number_of_payments > 0))
                    {
                        icdoServicePurchaseHeader.expected_installment_amount = 0;
                    }
                }
            }
        }
        // This method will go and get the current interest rate as stored in the code value table.
        public void SetCurrentInterestRate()
        {
            if (icdoServicePurchaseHeader.service_purchase_type_value == busConstant.Service_Purchase_Type_USERRA_Military_Service)
                icdoServicePurchaseHeader.contract_interest = 0.00M; //PIR 17946 - Interest should not be charged on USERRA purchases
            else
                icdoServicePurchaseHeader.contract_interest = GetCurrentInterestRateFromCodes();
        }

        private decimal GetCurrentInterestRateFromCodes()
        {
            decimal ldecInterestRate = 0;
            string lstrData1Value = busGlobalFunctions.GetData1ByCodeValue(
                busConstant.SystemConstantsAndVariablesCodeID, busConstant.Service_Purchase_Contract_Interest, iobjPassInfo);

            if (!String.IsNullOrEmpty(lstrData1Value))
            {
                ldecInterestRate = Convert.ToDecimal(lstrData1Value);
            }
            return ldecInterestRate;
        }

        private int GetServicePurchaseDateOfExpiration()
        {
            int lintServicePurchaseDateOfExpiration = 0;
            string lstrData1Value = busGlobalFunctions.GetData1ByCodeValue(
                busConstant.SystemConstantsAndVariablesCodeID, busConstant.Service_Purchase_Date_Of_Expiration, iobjPassInfo);

            if (!String.IsNullOrEmpty(lstrData1Value))
            {
                lintServicePurchaseDateOfExpiration = Convert.ToInt32(lstrData1Value);
            }
            return lintServicePurchaseDateOfExpiration;
        }

        public void CalculateDateOfExpiration()
        {
            if (icdoServicePurchaseHeader.date_of_purchase != System.DateTime.MinValue)
            {
                icdoServicePurchaseHeader.expiration_date = icdoServicePurchaseHeader.date_of_purchase.AddDays(GetServicePurchaseDateOfExpiration());
            }
        }

        private void PopulateUserNameForSuppressWarning()
        {
            // set the name of the user who has suppressed the warning.
            // first we have to check whether the object is in update mode or insert mode, based on that
            // we have to either 
            if (icdoServicePurchaseHeader.ienuObjectState == ObjectState.Insert)
            {
                if (icdoServicePurchaseHeader.suppress_warnings_flag != null && icdoServicePurchaseHeader.suppress_warnings_flag == busConstant.Flag_Yes)
                {
                    icdoServicePurchaseHeader.suppressed_by = iobjPassInfo.istrUserID;
                }
            }
            else if (icdoServicePurchaseHeader.ienuObjectState == ObjectState.Update)
            {
                string lstrOldSuppressWarningsFlag =
                    icdoServicePurchaseHeader.ihstOldValues["suppress_warnings_flag"].ToString();
                if (lstrOldSuppressWarningsFlag != icdoServicePurchaseHeader.suppress_warnings_flag)
                {
                    if (icdoServicePurchaseHeader.suppress_warnings_flag == busConstant.Flag_Yes)
                    {
                        icdoServicePurchaseHeader.suppressed_by = iobjPassInfo.istrUserID;
                    }
                    else
                    {
                        icdoServicePurchaseHeader.suppressed_by = "";
                    }
                }
            }
        }

        public bool IsInstallmentAmtModified()
        {
            // We should not allow the user to modify the expected installment amount when the pre-tax checkbox is checked.
            if (icdoServicePurchaseHeader.ienuObjectState == ObjectState.Update)
            {
                if (icdoServicePurchaseHeader.pre_tax == busConstant.Flag_Yes)
                {
                    decimal ldecOldExpectedInstallmentAmount =
                        Convert.ToDecimal(icdoServicePurchaseHeader.ihstOldValues["expected_installment_amount"]);

                    if (ldecOldExpectedInstallmentAmount != icdoServicePurchaseHeader.expected_installment_amount)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override void AfterPersistChanges()
        {
            // Now go ahead and calculate the value of Total Time to Purchase to be saved in the SGT_Service_Purchase_Detail table.
            // We do this by loading all the consolidated detail records for the current Service_Purchase_Detail_Id, this
            // is done to avoid doing complicated calculations when there is a change in date from/date to and time to purchase..
            // whenever there is some change in the consolidated, we need to go ahead and recalculate all the values in Header
            // and detail as well.

            //After Loading the Schedule, Update the Total Contract Amount            
            ibusPrimaryServicePurchaseDetail.RecomputeCalculatedFields();
            ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.Update();

            //Update the Total Contract Amount
            ibusPrimaryServicePurchaseDetail.ibusServicePurchaseHeader.CalculateTotalContractAmount();
            ibusPrimaryServicePurchaseDetail.ibusServicePurchaseHeader.icdoServicePurchaseHeader.Update();

            LoadAvailableRemittances();
            LoadUnPostedPaymentAllcoation();
            UpdatePaymentElection();
            LoadPerson();
            LoadPlan();
            LoadInPaymnetServicePurchases();//PIR 20022
            base.AfterPersistChanges();
        }

        public void CalculateCurrentAge()
        {
            if (icdoServicePurchaseHeader.date_of_purchase != System.DateTime.MinValue)
            {
                if (ibusPerson == null)
                {
                    LoadPerson();
                }
                if (ibusPerson.icdoPerson.date_of_birth != System.DateTime.MinValue)
                {
                    int lintYears;
                    int lintMonths;
                    DateTime ldtFrom = new DateTime(ibusPerson.icdoPerson.date_of_birth.Year, ibusPerson.icdoPerson.date_of_birth.Month, 1);
                    DateTime ldtTo = new DateTime(icdoServicePurchaseHeader.date_of_purchase.Year, icdoServicePurchaseHeader.date_of_purchase.Month, 1);
                    HelperFunction.GetMonthSpan(ldtFrom, ldtTo, out lintYears, out lintMonths);
                    icdoServicePurchaseHeader.current_age_year_part = lintYears;
                    icdoServicePurchaseHeader.current_age_month_part = lintMonths;
                }
            }
        }

        //If the First Payment is not made, then only calculate the Total Contract Amount. PIR 711
        public void CalculateTotalContractAmount()
        {
            if (icdoServicePurchaseHeader.service_purchase_header_id > 0)
            {
                if (!busServicePurchaseAmortizationSchedule.IsPaymentMade(icdoServicePurchaseHeader.service_purchase_header_id))
                {
                    // Total Contract Amount is equal to principle amount plus sum of interest the member will pay over
                    // the term of amortization schedule to fulfill the service purchase.
                    decimal ldecTotalContractAmount = TotalPurchaseCost;

                    //If the Payment Frequency is OneTime Lumpsum, we should not add the interest amount into Total Contract Amount
                    if (iclbServicePurchaseAmortizationSchedule != null && iclbServicePurchaseAmortizationSchedule.Count > 0)
                    {
                        foreach (busServicePurchaseAmortizationSchedule lobjServicePurchaseAmortizationSchedule in iclbServicePurchaseAmortizationSchedule)
                        {
                            ldecTotalContractAmount = ldecTotalContractAmount +
                                                      lobjServicePurchaseAmortizationSchedule.
                                                          icdoServicePurchaseAmortizationSchedule.interest_in_payment_amount;
                        }
                    }

                    icdoServicePurchaseHeader.total_contract_amount = ldecTotalContractAmount;
                }
            }
        }

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            if (aenmPageMode == utlPageMode.New)
                iblnPageNewMode = true;
            //Reload Person Account Based on New Plan
            LoadPersonAccount();

            CalculateCurrentAge();

            //If Both Payment Frequency and Expected Installment Amount Changed, we need to set the zero to one field. 
            //Also, if the value changes in any one field, we need to set zero for other field to recalculate the Amortization Schedule
            ReassignPaymentElectionFields();

            //This method will be called here to determine the Expected Payment Amount is less than Interest Amount.
            LoadAmortizationSchedule();

            //Load Allocated remittance collection for validation purpose
            LoadAllocatedPurchaseList();

            if (ibusPersonAccount == null)
                LoadPersonAccount();

            if (ibusPlan == null)
                LoadPlan();

            base.BeforeValidate(aenmPageMode);
        }

        public string Current_Age
        {
            get
            {
                return string.Format("{0:00}", icdoServicePurchaseHeader.current_age_year_part) + " " + string.Format("{0:00}", icdoServicePurchaseHeader.current_age_month_part);
            }
        }

        public void LoadServicePurchaseDetail()
        {
            if (ibusPrimaryServicePurchaseDetail == null)
            {
                ibusPrimaryServicePurchaseDetail = new busServicePurchaseDetail();
                ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail = new cdoServicePurchaseDetail();

                DataTable ldtbList = Select<cdoServicePurchaseDetail>(
                    new string[1] { "service_purchase_header_id" },
                    new object[1] { icdoServicePurchaseHeader.service_purchase_header_id }, null, null);
                if (ldtbList.Rows.Count > 0)
                {
                    ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.LoadData(ldtbList.Rows[0]);
                    ibusPrimaryServicePurchaseDetail.ibusServicePurchaseHeader = this;

                    // Depending on the service credit type of the header, we need to load the appropriate collection.
                    switch (icdoServicePurchaseHeader.service_purchase_type_value)
                    {
                        case busConstant.Service_Purchase_Type_Consolidated_Purchase:
                            ibusPrimaryServicePurchaseDetail.LoadServicePurchaseDetailConsolidated();
                            break;
                        case busConstant.Service_Purchase_Type_USERRA_Military_Service:
                            ibusPrimaryServicePurchaseDetail.LoadServicePurchaseDetailUSERRA();
                            break;
                    }
                }
            }
        }

        // PIR 20022
        public Collection<busServicePurchaseHeader> iclbMemberInPaymentServicePurchaseList { get; set; }
        public decimal idecTotalInPaymentService { get; set; }
        public decimal idecTotalTimeToPurchase { get; set; }
        public decimal idecRemainingService { get; set; }
        public decimal idecAdditionalServicePSC { get; set; }
        public decimal idecAdditionalServiceVSC { get; set; }

        public string idecAdditionalServicePSC_formatted { get; set; }
        public string idecAdditionalServiceVSC_formatted { get; set; }
        /// <summary>
        /// PIR 20022 Modify service purchase calculation screen to account for purchases "in payment"
        /// </summary>
        public void LoadInPaymnetServicePurchases()
        {
            if (ibusPerson == null)
                LoadPerson();
            if (ibusPerson.iclbServicePurchaseHeader == null)
                ibusPerson.LoadServicePurchase(false);
            iclbMemberInPaymentServicePurchaseList = new Collection<busServicePurchaseHeader>();
			//PIR 26151
            this.idecAdditionalServicePSC = 0;
            this.idecAdditionalServiceVSC = 0;
            foreach (busServicePurchaseHeader lobjServicePurchaseHeader in ibusPerson.iclbServicePurchaseHeader)
            {
                if (//lobjServicePurchaseHeader.icdoServicePurchaseHeader.plan_id == this.icdoServicePurchaseHeader.plan_id &&
                    lobjServicePurchaseHeader.icdoServicePurchaseHeader.service_purchase_type_value == busConstant.Service_Purchase_Type_Consolidated_Purchase &&
                    lobjServicePurchaseHeader.icdoServicePurchaseHeader.action_status_value == busConstant.Service_Purchase_Action_Status_In_Payment)
                {
                    busServicePurchaseHeader lTempServicePurchaseHeader = new BusinessObjects.busServicePurchaseHeader { icdoServicePurchaseHeader = new cdoServicePurchaseHeader() };
                    if (lobjServicePurchaseHeader.ibusPrimaryServicePurchaseDetail == null)
                        lobjServicePurchaseHeader.LoadOnlyConsolidatedServicePurchaseDetail();

                    lTempServicePurchaseHeader.icdoServicePurchaseHeader.service_purchase_header_id = lobjServicePurchaseHeader.icdoServicePurchaseHeader.service_purchase_header_id;
                    lTempServicePurchaseHeader.idecTotalTimeToPurchase = Math.Round(lobjServicePurchaseHeader.ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.total_time_to_purchase, MidpointRounding.AwayFromZero);

                    lTempServicePurchaseHeader.idecRemainingService = Math.Round(lobjServicePurchaseHeader.ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.total_time_to_purchase
                                                - lobjServicePurchaseHeader.icdoServicePurchaseHeader.prorated_psc, 4, MidpointRounding.AwayFromZero);
                    lTempServicePurchaseHeader.idecTotalInPaymentService = lobjServicePurchaseHeader.ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.total_time_to_purchase;

                    iclbMemberInPaymentServicePurchaseList.Add(lTempServicePurchaseHeader);
                    this.idecAdditionalServicePSC += lTempServicePurchaseHeader.idecRemainingService;
                    this.idecAdditionalServiceVSC += lTempServicePurchaseHeader.idecRemainingService;

                }
            }
            //formatting
            if (idecAdditionalServicePSC < 0)
                idecAdditionalServicePSC_formatted = String.Format("{0} Years {1} Months", Math.Ceiling(idecAdditionalServicePSC / 12).ToString(),
                                 Math.Round((idecAdditionalServicePSC % 12), 6, MidpointRounding.AwayFromZero).ToString());
            else
                idecAdditionalServicePSC_formatted = String.Format("{0} Years {1} Months", Math.Floor(idecAdditionalServicePSC / 12).ToString(),
                                     Math.Round((idecAdditionalServicePSC % 12), 6, MidpointRounding.AwayFromZero).ToString());

            if (idecAdditionalServiceVSC < 0)
                idecAdditionalServiceVSC_formatted = String.Format("{0} Years {1} Months", Math.Ceiling(idecAdditionalServiceVSC / 12).ToString(),
                                 Math.Round((idecAdditionalServiceVSC % 12), 6, MidpointRounding.AwayFromZero).ToString());
            else
                idecAdditionalServiceVSC_formatted = String.Format("{0} Years {1} Months", Math.Floor(idecAdditionalServiceVSC / 12).ToString(),
                                     Math.Round((idecAdditionalServiceVSC % 12), 6, MidpointRounding.AwayFromZero).ToString());

        }
        public void LoadOnlyConsolidatedServicePurchaseDetail()
        {
            if (ibusPrimaryServicePurchaseDetail == null)
            {
                ibusPrimaryServicePurchaseDetail = new busServicePurchaseDetail();
                ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail = new cdoServicePurchaseDetail();

                DataTable ldtbList = Select<cdoServicePurchaseDetail>(
                    new string[1] { "service_purchase_header_id" },
                    new object[1] { icdoServicePurchaseHeader.service_purchase_header_id }, null, null);
                if (ldtbList.Rows.Count > 0)
                {
                    ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.LoadData(ldtbList.Rows[0]);
                    ibusPrimaryServicePurchaseDetail.ibusServicePurchaseHeader = this;
                    //ibusPrimaryServicePurchaseDetail.LoadServicePurchaseDetailConsolidated();
                }
            }
        }
        public void LoadAvailableRemittances()
        {
            iclbAvailableRemittances = new Collection<busServicePurchasePaymentAllocation>();
            busRemittance lobjRemittance;
            busDeposit lobjDeposit;
            decimal ldecTotalAvailableAmountForAllocation = 0.0M;

            DataTable ldtbList = busNeoSpinBase.Select("cdoServicePurchasePaymentAllocation.GetRemittanceForPerson", new object[2] { icdoServicePurchaseHeader.person_id, icdoServicePurchaseHeader.plan_id });
            if (ldtbList.Rows.Count > 0)
            {
                Collection<busServicePurchasePaymentAllocation> lclbTotalAvailableRemittances = GetCollection<busServicePurchasePaymentAllocation>(ldtbList,
                                                                                               "icdoServicePurchasePaymentAllocation");

                foreach (busServicePurchasePaymentAllocation lobjServicePurchasePaymentAllocation in lclbTotalAvailableRemittances)
                {
                    lobjRemittance = new busRemittance { icdoRemittance = new cdoRemittance() };
                    lobjRemittance.FindRemittance(lobjServicePurchasePaymentAllocation.icdoServicePurchasePaymentAllocation.remittance_id);

                    lobjServicePurchasePaymentAllocation.icdoServicePurchasePaymentAllocation.service_purchase_header_id
                        = icdoServicePurchaseHeader.service_purchase_header_id;

                    //Assign the Payroll Header Object to get the Pay Off Amount
                    lobjServicePurchasePaymentAllocation.ibusServicePurchaseHeader = this;

                    // Find the Available remittance amount after reducing the allocated amount + refund amount if any
                    lobjServicePurchasePaymentAllocation.icdoServicePurchasePaymentAllocation.remittance_amount =
                        busEmployerReportHelper.GetRemittanceAvailableAmount(lobjServicePurchasePaymentAllocation.icdoServicePurchasePaymentAllocation.remittance_id);

                    lobjServicePurchasePaymentAllocation.icdoServicePurchasePaymentAllocation.ienuObjectState = ObjectState.Insert;

                    //PIR 710: if the Remaining balance amount is zero, skip this entry from display
                    if (lobjServicePurchasePaymentAllocation.icdoServicePurchasePaymentAllocation.remittance_amount > 0)
                    {
                        iclbAvailableRemittances.Add(lobjServicePurchasePaymentAllocation);
                    }
                }
            }
        }

        //This method will update the No of Payments and Expected Installment Amount only if the user changes any one of the field.
        public void UpdatePaymentElection()
        {
            if ((iclbServicePurchaseAmortizationSchedule != null) && (_iintTotalNoOfFuturePayments > 0) && (iblnIsPaymentElectionChanged))
            {
                if (icdoServicePurchaseHeader.service_purchase_type_value == busConstant.Service_Purchase_Type_Unused_Sick_Leave)
                {
                    if (ibusPrimaryServicePurchaseDetail != null)
                    {
                        //UAT PIR 726 : if the FAS gets changed, purchase cost also got changed. so, we must update the detail table too.
                        ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.Update();
                    }
                }
                icdoServicePurchaseHeader.number_of_payments = _iintTotalNoOfFuturePayments;
                icdoServicePurchaseHeader.expected_installment_amount = idecExpectedInstallmentAmount;
                icdoServicePurchaseHeader.Update();
                iblnIsPaymentElectionChanged = false;
            }
        }

        public void LoadAmortizationSchedule()
        {
            _iblnIsExpectedPaymentAmountTooSmall = false;

            //UAT PIR 1070 : If the Number of Payments is blank or zero (for Grant Free), system thorws an Null exception. To handle that, we initialize the Collection here.
            if (iclbServicePurchaseAmortizationSchedule == null)
                iclbServicePurchaseAmortizationSchedule = new Collection<busServicePurchaseAmortizationSchedule>();

            // Go and get the amortization schedule only for service purchase when the Status is approved
            // and the expected payment amount or number of periods has been mentioned.
            if ((icdoServicePurchaseHeader.action_status_value != busConstant.Service_Purchase_Action_Status_Pending) ||
                (icdoServicePurchaseHeader.action_status_value != busConstant.Service_Purchase_Action_Status_Void))
            {
                if ((icdoServicePurchaseHeader.expected_installment_amount > 0 || icdoServicePurchaseHeader.number_of_payments > 0))
                {
                    iclbServicePurchaseAmortizationSchedule =
                        busServicePurchaseAmortizationSchedule.CalculateAmortizationSchedule(this, iobjPassInfo,
                                                                                             ref
                                                                                                 _iblnIsExpectedPaymentAmountTooSmall);
                }
            }
        }

        public void LoadPerson()
        {
            DataTable ldtbList = Select<cdoPerson>(
             new string[1] { "person_id" },
             new object[1] { icdoServicePurchaseHeader.person_id }, null, null);

            // TODO check to see whether we have to throw exception when person is not found
            if (ldtbList.Rows.Count > 0)
            {
                if (ibusPerson == null)
                {
                    ibusPerson = new busPerson();
                    ibusPerson.icdoPerson = new cdoPerson();
                }

                ibusPerson.icdoPerson.LoadData(ldtbList.Rows[0]);
            }
        }

        public void LoadPlan()
        {
            if (ibusPlan == null || ibusPlan.icdoPlan.plan_id != icdoServicePurchaseHeader.plan_id)
            {
                DataTable ldtbList = Select<cdoPlan>(
                 new string[1] { "plan_id" },
                 new object[1] { icdoServicePurchaseHeader.plan_id }, null, null);
                if (ldtbList.Rows.Count > 0)
                {
                    if (ibusPlan == null)
                    {
                        ibusPlan = new busPlan();
                        ibusPlan.icdoPlan = new cdoPlan();
                    }
                    ibusPlan.icdoPlan.LoadData(ldtbList.Rows[0]);
                }
            }
        }

        public Collection<busNotes> iclbNotes { get; set; }
        public void LoadNotes()
        {
            DataTable ldtbList = Select<cdoNotes>(
                new string[1] { "person_id" },
                new object[1] { icdoServicePurchaseHeader.person_id }, null, null);
            iclbNotes = GetCollection<busNotes>(ldtbList, "icdoNotes");
        }

        public override bool ValidateSoftErrors()
        {
            if (icdoServicePurchaseHeader.service_purchase_header_id != 0)
            {
                if (ibusSoftErrors == null)
                {
                    LoadErrors();
                }
                iblnClearSoftErrors = true;
                ibusSoftErrors.iblnClearError = true;
                ibusSoftErrors.DeleteErrors();
                iblnClearSoftErrors = false;
                ibusSoftErrors.iblnClearError = false;
                if (ibusPrimaryServicePurchaseDetail != null)
                {
                    ibusPrimaryServicePurchaseDetail.ibusServicePurchaseHeader = this;
                    ibusPrimaryServicePurchaseDetail.iblnHeaderValidating = true;
                    ibusPrimaryServicePurchaseDetail.ValidateSoftErrors();
                }
                return base.ValidateSoftErrors();
            }
            return false;
        }

        public bool CheckIsPlanNameValidforJudgesConversion()
        {
            if (icdoServicePurchaseHeader.plan_id != 0)
            {
                if (ibusPlan == null)
                    LoadPlan();
                if (ibusPlan != null)
                {
                    if (!(ibusPlan.icdoPlan.plan_code == busConstant.Plan_Code_Main || ibusPlan.icdoPlan.plan_code == busConstant.Plan_Code_Main_2020 || ibusPlan.icdoPlan.plan_code == busConstant.Plan_Code_Judges)) //PIR 20232
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        public bool IsMemberhasWagesAndServiceCredit()
        {
            bool lblnIsPersonHasPSCandWages = false;
            if (iobjPassInfo.idictParams.ContainsKey("FormName") &&
                iobjPassInfo.idictParams["FormName"] != null &&
                iobjPassInfo.idictParams["FormName"].Equals("wfmServicePurchaseHeaderMaintenance")
                && ibusPersonAccount.IsNotNull() && ibusPersonAccount.icdoPersonAccount.person_account_id > 0 && icdoServicePurchaseHeader.suppress_warnings_flag !=busConstant.Flag_Yes)
            {
                lblnIsPersonHasPSCandWages = ibusPersonAccount.LoadMemberWithWagesAndServiceCredit().Rows.Count > 0;
            }
            return lblnIsPersonHasPSCandWages;
        }

        //Load Remittance Allocation
        public void LoadPaymentAllocated()
        {
            DataTable ldtbList = Select<cdoServicePurchasePaymentAllocation>(
            new string[1] { "service_purchase_header_id" },
            new object[1] { icdoServicePurchaseHeader.service_purchase_header_id }, null, null);
            _iclbAllocatedRemittance = GetCollection<busServicePurchasePaymentAllocation>(ldtbList, "icdoServicePurchasePaymentAllocation");
        }
        /// <summary>
        /// Post GL account
        /// </summary>                        
        /// <param name="adecPreTaxEE"></param>        
        /// <param name="adecPostTaxEE"></param>
        /// <param name="adecPostTaxER"></param>
        /// <param name="adecPostTaxEERHIC"></param>
        /// <param name="adecPreTaxERRHIC"></param>
        /// <param name="adecPreTaxER"></param>
        /// <param name="aintPostDirection"></param> +1 for posting / -1 for reveral
        public void PostGLAccount(decimal adecPreTaxEE, decimal adecPostTaxEE, decimal adecPostTaxER,
            decimal adecPostTaxEERHIC, decimal adecPreTaxERRHIC, decimal adecPreTaxER, int aintPostDirection, bool ablnIsFromEmpReporting = false, int aintOrgId = 0)
        {
            decimal ldecUSERRA_EE = 0;
            decimal ldecUSERRA_ER = 0;
            decimal ldecUSERRA_RHIC_EE = 0;
            decimal ldecUSERRA_RHIC_ER = 0;

            decimal ldecPurchase_EE = 0;
            decimal ldecPurchase_ER = 0;
            decimal ldecPurchase_RHIC_EE = 0;
            decimal ldecPurchase_RHIC_ER = 0;
            if (icdoServicePurchaseHeader.service_purchase_type_value == busConstant.Service_Purchase_Type_USERRA_Military_Service)
            {
                ldecUSERRA_EE = adecPreTaxEE + adecPostTaxEE;
                ldecUSERRA_ER = adecPreTaxER + adecPostTaxER;
                ldecUSERRA_RHIC_EE = adecPostTaxEERHIC;
                ldecUSERRA_RHIC_ER = adecPreTaxERRHIC;

			//PIR 938 GL Changes
                if (ablnIsFromEmpReporting)
                {
                    decimal ldecTotalUserra = ldecUSERRA_EE + ldecUSERRA_ER + ldecUSERRA_RHIC_EE + ldecUSERRA_RHIC_ER;
                    if (ldecTotalUserra > 0)
                        GenerateGLByItemTypeFromEmpReporting(busConstant.ItemTypePurchase, aintPostDirection, ldecTotalUserra, aintOrgId);
                }
                if (ldecUSERRA_EE > 0)
                    GenerateGLByItemType(busConstant.ItemTypeUSERRAEE, aintPostDirection, ldecUSERRA_EE);

                if (ldecUSERRA_ER > 0)
                    GenerateGLByItemType(busConstant.ItemTypeUSERRAER, aintPostDirection, ldecUSERRA_ER);

                if (ldecUSERRA_RHIC_EE > 0)
                    GenerateGLByItemType(busConstant.ItemTypeUSERRARHICEE, aintPostDirection, ldecUSERRA_RHIC_EE);

                if (ldecUSERRA_RHIC_ER > 0)
                    GenerateGLByItemType(busConstant.ItemTypeUSERRARHICER, aintPostDirection, ldecUSERRA_RHIC_ER);
            }
            else
            {
                ldecPurchase_EE = adecPreTaxEE + adecPostTaxEE;
                ldecPurchase_ER = adecPreTaxER + adecPostTaxER;
                ldecPurchase_RHIC_EE = adecPostTaxEERHIC;
                ldecPurchase_RHIC_ER = adecPreTaxERRHIC;
                if (ablnIsFromEmpReporting)
                {
                    decimal ldecTotalPurchase = ldecPurchase_EE + ldecPurchase_ER + ldecPurchase_RHIC_EE + ldecPurchase_RHIC_ER;
                    if (ldecTotalPurchase > 0)
                        GenerateGLByItemTypeFromEmpReporting(busConstant.ItemTypePurchase, aintPostDirection, ldecTotalPurchase, aintOrgId);
                }
                if (ldecPurchase_EE > 0)
                    GenerateGLByItemType(busConstant.ItemTypePurchaseEE, aintPostDirection, ldecPurchase_EE);

                if (ldecPurchase_ER > 0)
                    GenerateGLByItemType(busConstant.ItemTypePurchaseER, aintPostDirection, ldecPurchase_ER);

                if (ldecPurchase_RHIC_EE > 0)
                    GenerateGLByItemType(busConstant.ItemTypePurchaseRHICEE, aintPostDirection, ldecPurchase_RHIC_EE, true);

                if (ldecPurchase_RHIC_ER > 0)
                    GenerateGLByItemType(busConstant.ItemTypePurchaseRHICEE, aintPostDirection, ldecPurchase_RHIC_ER, true);
            }
        }

        private void GenerateGLByItemTypeFromEmpReporting(string astrItemType, int aintPostDirection, decimal adecAmount, int aintOrgId)
        {
            cdoAccountReference lcdoFundTransferReference = new cdoAccountReference();
            lcdoFundTransferReference.plan_id = icdoServicePurchaseHeader.plan_id;
            lcdoFundTransferReference.source_type_value = busConstant.SourceTypePurchase;
            lcdoFundTransferReference.transaction_type_value = busConstant.TransactionTypeTransfer;
            lcdoFundTransferReference.item_type_value = astrItemType;
            if (aintPostDirection == 1)
                lcdoFundTransferReference.status_transition_value = String.Empty;
            else
                lcdoFundTransferReference.status_transition_value = busConstant.StatusTransitionAppliedToNSF;
            busGLHelper.GenerateGL(lcdoFundTransferReference,
            icdoServicePurchaseHeader.person_id, 0, icdoServicePurchaseHeader.service_purchase_header_id,
            adecAmount, DateTime.Now, DateTime.Now, iobjPassInfo, aintOrgId);
        }

        private void GenerateGLByItemType(string astrItemType, int aintPostDirection, decimal adecAmount, bool ablnInsertExtraTransferGL = false)
        {
            cdoAccountReference lcdoAcccountReference = new cdoAccountReference();
            lcdoAcccountReference.plan_id = icdoServicePurchaseHeader.plan_id;
            lcdoAcccountReference.source_type_value = busConstant.SourceTypePurchase;
            lcdoAcccountReference.transaction_type_value = busConstant.TransactionTypeAllocation;
            lcdoAcccountReference.from_item_type_value = busConstant.FromItemTypePurchases;
            lcdoAcccountReference.to_item_type_value = astrItemType;
            if (aintPostDirection == 1)
                lcdoAcccountReference.status_transition_value = String.Empty;
            else
                lcdoAcccountReference.status_transition_value = busConstant.StatusTransitionAppliedToNSF;

            ArrayList larrError = busGLHelper.GenerateGL(lcdoAcccountReference,
                icdoServicePurchaseHeader.person_id,
                0, icdoServicePurchaseHeader.service_purchase_header_id,
                adecAmount,
                DateTime.Now,
                DateTime.Now, iobjPassInfo);
            if (larrError.Count > 0)
            {
                utlError lutlError = (utlError)larrError[0];
                throw new Exception(lutlError.istrDisplayMessage);
            }
            else
            {
                //UAT PIR 1061 - Generate Fund Transfer GL Also
                cdoAccountReference lcdoFundTransferReference = new cdoAccountReference();
                lcdoFundTransferReference.plan_id = icdoServicePurchaseHeader.plan_id;
                lcdoFundTransferReference.source_type_value = busConstant.SourceTypePurchase;
                lcdoFundTransferReference.transaction_type_value = busConstant.TransactionTypeTransfer;
                lcdoFundTransferReference.item_type_value = astrItemType;
                if (aintPostDirection == 1)
                    lcdoFundTransferReference.status_transition_value = String.Empty;
                else
                    lcdoFundTransferReference.status_transition_value = busConstant.StatusTransitionAppliedToNSF;

                //For some of the Bucket like Purchase EE, Purchsae ER doesnt have Fund Transfer GL.
                //So, Generate GL method will handle that by itself, but, we dont need to throw any exception as such here.
                busGLHelper.GenerateGL(lcdoFundTransferReference,
                icdoServicePurchaseHeader.person_id, 0, icdoServicePurchaseHeader.service_purchase_header_id,
                adecAmount, DateTime.Now, DateTime.Now, iobjPassInfo);
				//PIR 11171
                if(ablnInsertExtraTransferGL)
                {
                    cdoAccountReference lcdoFundTranReference = new cdoAccountReference();
                    lcdoFundTranReference.plan_id = icdoServicePurchaseHeader.plan_id;
                    lcdoFundTranReference.source_type_value = busConstant.SourceTypePurchase;
                    lcdoFundTranReference.transaction_type_value = busConstant.TransactionTypeTransfer;
                    lcdoFundTranReference.item_type_value = busConstant.ItemTypePurchaseRHIC;
                    lcdoFundTranReference.status_transition_value = String.Empty;
                   
                    busGLHelper.GenerateGL(lcdoFundTranReference,
                    icdoServicePurchaseHeader.person_id, 0, icdoServicePurchaseHeader.service_purchase_header_id,
                    adecAmount, DateTime.Now, DateTime.Now, iobjPassInfo);

                    cdoAccountReference lcdoFundTranForPurchase = new cdoAccountReference();
                    lcdoFundTranForPurchase.plan_id = icdoServicePurchaseHeader.plan_id;
                    lcdoFundTranForPurchase.source_type_value = busConstant.SourceTypeRemittance;
                    lcdoFundTranForPurchase.transaction_type_value = busConstant.TransactionTypeTransfer;
                    lcdoFundTranForPurchase.item_type_value = busConstant.ItemTypePurchase;
                    lcdoFundTranForPurchase.status_transition_value = String.Empty;

                    busGLHelper.GenerateGL(lcdoFundTranForPurchase,
                    icdoServicePurchaseHeader.person_id, 0, icdoServicePurchaseHeader.service_purchase_header_id,
                    adecAmount, DateTime.Now, DateTime.Now, iobjPassInfo);
                }
            }
        }
        public decimal RoundServiceToPersist(decimal adecValue)
        {
            return Math.Round(adecValue, 6, MidpointRounding.AwayFromZero);
        }
        public decimal RoundServiceToUse(decimal adecValue)
        {
            return Math.Round(adecValue, 4, MidpointRounding.AwayFromZero);
        }
        public decimal RoundNormal(decimal adecValue, int adecDecimal)
        {
            return Math.Round(adecValue, adecDecimal, MidpointRounding.AwayFromZero);
        }

        protected busPersonAccount _ibusPersonAccount;
        public busPersonAccount ibusPersonAccount
        {
            get
            {
                return _ibusPersonAccount;
            }
            set
            {
                _ibusPersonAccount = value;
            }
        }

        public void LoadPersonAccount()
        {
            if (ibusPerson == null)
                LoadPerson();

            ibusPersonAccount = ibusPerson.LoadActivePersonAccountByPlan(icdoServicePurchaseHeader.plan_id);
            if ((icdoServicePurchaseHeader.service_purchase_type_value == busConstant.Service_Purchase_Type_Consolidated_Purchase && icdoServicePurchaseHeader.action_status_value == busConstant.Service_Purchase_Action_Status_Pending) ||
                (icdoServicePurchaseHeader.service_purchase_type_value != busConstant.Service_Purchase_Type_Consolidated_Purchase))
            {

                icdoServicePurchaseHeader.total_psc = 0.00M;
                icdoServicePurchaseHeader.total_vsc = 0.00M;
                //UAT PIR 1467 : Loading Service Credit from current Person Account as well suspended Payee Accounts for RTW Members
                if (ibusPerson.iclbPayeeAccount == null)
                    ibusPerson.LoadPayeeAccount();

                foreach (busPayeeAccount lbusPayeeAccount in ibusPerson.iclbPayeeAccount)
                {
                    if (lbusPayeeAccount.ibusPayeeAccountActiveStatus == null)
                        lbusPayeeAccount.LoadActivePayeeStatusAsofNextBenefitPaymentDate();
                }

                var lenuList = ibusPerson.iclbPayeeAccount.Where(i => i.ibusPayeeAccountActiveStatus.IsSuspendedForRetirementOrDisability());
                foreach (busPayeeAccount lbusPayeeAccount in lenuList)
                {
                    if (lbusPayeeAccount.ibusApplication == null)
                        lbusPayeeAccount.LoadApplication();

                    if (lbusPayeeAccount.ibusApplication.ibusPersonAccount == null)
                        lbusPayeeAccount.ibusApplication.LoadPersonAccountByApplication();

                    lbusPayeeAccount.ibusApplication.ibusPersonAccount.LoadTotalPSC();
                    icdoServicePurchaseHeader.total_psc += lbusPayeeAccount.ibusApplication.ibusPersonAccount.icdoPersonAccount.Total_PSC;

                    //lbusPayeeAccount.ibusApplication.ibusPersonAccount.LoadTotalVSC(ablnExcludeTFFRTIAA: true);
                    //icdoServicePurchaseHeader.total_vsc += lbusPayeeAccount.ibusApplication.ibusPersonAccount.icdoPersonAccount.Total_VSC;
                }
                ibusPersonAccount.LoadTotalPSC();
                icdoServicePurchaseHeader.total_psc += ibusPersonAccount.icdoPersonAccount.Total_PSC;
                ibusPersonAccount.LoadTotalVSC(ablnExcludeTFFRTIAA: true);
                //icdoServicePurchaseHeader.total_vsc += ibusPersonAccount.icdoPersonAccount.Total_VSC;
                //Maik mail dated February 10, 2018, - Consider Approved and Tentative TIAA/TFFR service from SGT_PERSON_TFFR_TIAA_SERVICE
                decimal adecTFFRService = 0.00M;
                decimal adecTIAAService = 0.00M;
                decimal adecTFFRTentativeService = 0.00M;
                decimal adecTIAATentativeService = 0.00M;
                ibusPersonAccount.LoadTFFRTIAAService(ref adecTFFRService, ref adecTIAAService, ref adecTFFRTentativeService, ref adecTIAATentativeService);
                //icdoServicePurchaseHeader.total_vsc += adecTFFRService + adecTIAAService + adecTFFRTentativeService + adecTIAATentativeService;
                ibusPersonAccount.icdoPersonAccount.Total_VSC += adecTFFRService + adecTIAAService + adecTFFRTentativeService + adecTIAATentativeService;
                icdoServicePurchaseHeader.total_vsc = ibusPerson.GetTotalVSCForPerson(icdoServicePurchaseHeader.plan_id == busConstant.PlanIdJobService, DateTime.MinValue, true, false);
                icdoServicePurchaseHeader.total_vsc += icdoServicePurchaseHeader.free_or_dual_service;
                LoadInPaymnetServicePurchases();
                icdoServicePurchaseHeader.total_vsc += this.idecAdditionalServiceVSC;
            }
            else
            {
                if (ibusPrimaryServicePurchaseDetail.IsNull())
                    LoadServicePurchaseDetail();
                icdoServicePurchaseHeader.total_vsc = ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.vsc;
                icdoServicePurchaseHeader.total_psc = ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.psc;
                ibusPersonAccount.icdoPersonAccount.Total_PSC = icdoServicePurchaseHeader.total_psc;
            }
        }

        public void GetDistributedService(ref decimal adecPSCToPost, ref decimal adecVSCToPost)
        {
            decimal ldecPSCToPost, ldecVSCToPost;
            if (icdoServicePurchaseHeader.service_purchase_type_value == busConstant.Service_Purchase_Type_Consolidated_Purchase)
            {
                if (_ibusPrimaryServicePurchaseDetail.iclbServicePurchaseDetailConsolidated == null)
                    _ibusPrimaryServicePurchaseDetail.LoadServicePurchaseDetailConsolidated();
                foreach (busServicePurchaseDetailConsolidated lbusDetailConsolidate in _ibusPrimaryServicePurchaseDetail.iclbServicePurchaseDetailConsolidated)
                {
                    ldecPSCToPost = 0;
                    ldecVSCToPost = 0;
                    GetServiceToPost(icdoServicePurchaseHeader.plan_id, lbusDetailConsolidate.icdoServicePurchaseDetailConsolidated.service_credit_type_value,
                        icdoServicePurchaseHeader.payor_value,
                        RoundServiceToPersist(Convert.ToDecimal(lbusDetailConsolidate.icdoServicePurchaseDetailConsolidated.calculated_time_to_purchase)),
                        ref ldecPSCToPost, ref ldecVSCToPost);
                    adecPSCToPost += ldecPSCToPost;
                    adecVSCToPost += ldecVSCToPost;
                }
            }
            else if (icdoServicePurchaseHeader.service_purchase_type_value == busConstant.Service_Purchase_Type_USERRA_Military_Service)
            {
                GetServiceToPost(icdoServicePurchaseHeader.plan_id, icdoServicePurchaseHeader.service_purchase_type_value,
                    icdoServicePurchaseHeader.payor_value, RoundServiceToPersist(Convert.ToDecimal(_ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.total_time_to_purchase)),
                    ref adecPSCToPost, ref adecVSCToPost);
            }
            else if (icdoServicePurchaseHeader.service_purchase_type_value == busConstant.Service_Purchase_Type_Unused_Sick_Leave)
            {
                GetServiceToPost(icdoServicePurchaseHeader.plan_id, icdoServicePurchaseHeader.service_purchase_type_value,
                    icdoServicePurchaseHeader.payor_value, RoundServiceToPersist(Convert.ToDecimal(_ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.total_time_to_purchase)),
                    ref adecPSCToPost, ref adecVSCToPost);
            }
        }
        // Date used in posting codes. Specially batches
        private DateTime _idatRunDate;
        public DateTime idatRunDate
        {
            get
            {
                return _idatRunDate;
            }
            set
            {
                _idatRunDate = value;
            }
        }

        //This method will be called when Grant Free Flag Checked. 
        public void PostPersonAccountRetirement(decimal adecPreTaxEE, decimal adecPostTaxEE, decimal adecPostTaxER, decimal adecPostTaxEERHIC,
            decimal adecPreTaxERRHIC, decimal adecPreTaxER, decimal adecEREEPickup, int aintPostDirection, ref decimal adecPSCToPost, ref decimal adecVSCToPost,
            DateTime adatPayment)
        {
            busServicePurchasePaymentAllocation lobjServicePurchasePaymentAllocation = new busServicePurchasePaymentAllocation();
            lobjServicePurchasePaymentAllocation.icdoServicePurchasePaymentAllocation = new cdoServicePurchasePaymentAllocation();

            PostPersonAccountRetirement(
                adecPreTaxEE, adecPostTaxEE, adecPostTaxER, adecPostTaxEERHIC, adecPreTaxERRHIC, adecPreTaxER, adecEREEPickup,
                aintPostDirection, ref adecPSCToPost,
                ref adecVSCToPost, adatPayment, lobjServicePurchasePaymentAllocation);

        }

        /// <summary>
        /// Post the money and service to person account retirement
        /// </summary>
        /// <param name="adecPreTaxEE"></param>
        /// <param name="adecPostTaxEE"></param>
        /// <param name="adecPostTaxER"></param>
        /// <param name="adecPostTaxEERHIC"></param>
        /// <param name="adecPreTaxERRHIC"></param>
        /// <param name="adecPreTaxER"></param>
        /// <param name="aintPostDirection"></param>
        /// <param name="adecPSCToPost"></param>
        /// <param name="adecVSCToPost"></param>
        public void PostPersonAccountRetirement(decimal adecPreTaxEE, decimal adecPostTaxEE, decimal adecPostTaxER, decimal adecPostTaxEERHIC,
            decimal adecPreTaxERRHIC, decimal adecPreTaxER, decimal adecEREEPickup, int aintPostDirection, ref decimal adecPSCToPost, ref decimal adecVSCToPost,
            DateTime adatPayment, busServicePurchasePaymentAllocation aobjServicePurchasePaymentAllocation)
        {
            decimal ldecTotalPSC = 0;
            decimal ldecTotalVSC = 0;
            decimal ldecPayOffAmountByAllocation = 0;
            adecPSCToPost = 0;
            adecVSCToPost = 0;

            //decimal ldecFreeServiceToPost = 0;
            if (_ibusPersonAccount == null)
                LoadPersonAccount();

            if (_ibusPrimaryServicePurchaseDetail == null)
                LoadServicePurchaseDetail();

            if (_ibusPerson == null)
                LoadPerson();

            if (_iclbServicePurchaseAmortizationSchedule == null)
                LoadAmortizationSchedule();

            decimal ldecTotalContractAmt = TotalPurchaseCost;
            GetDistributedService(ref ldecTotalPSC, ref ldecTotalVSC);

            //PSC to post            
            if ((icdoServicePurchaseHeader.grant_free_flag == "Y") || (IsFreeServiceOnly()))
            {
                adecPSCToPost = ldecTotalPSC;
            }
            else if (icdoServicePurchaseHeader.service_purchase_type_value == busConstant.Service_Purchase_Type_Unused_Sick_Leave) //UAT PIR 726
            {
                if (ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.ready_for_posting_flag == busConstant.Flag_Yes && idecPayOffAmount <= 0)
                {
                    adecPSCToPost = ldecTotalPSC;
                }
                else
                {
                    adecPSCToPost = 0.00M;
                }
            }
            else if (icdoServicePurchaseHeader.service_purchase_type_value == busConstant.Service_Purchase_Type_USERRA_Military_Service)
            {
                //UAT PIR 828 - Post the Full Service Credit only when paid in full goes.
                bool lblnPaidInFull = (icdoServicePurchaseHeader.paid_rhic_cost_amount + icdoServicePurchaseHeader.paid_retirement_ee_cost_amount +
                       icdoServicePurchaseHeader.paid_retirement_er_cost_amount +
                       (adecPreTaxEE + adecPostTaxEE + adecPostTaxER + adecPostTaxEERHIC + adecPreTaxERRHIC + adecPreTaxER) >= ldecTotalContractAmt);
                if (lblnPaidInFull)
                {
                    adecPSCToPost = ldecTotalPSC;
                }
                else
                {
                    adecPSCToPost = 0.00M;
                }
            }
            else
            {
                //Get The Payoff Amount By Allocation
                ldecPayOffAmountByAllocation = GetPayOffAmountByAllocation(aobjServicePurchasePaymentAllocation.icdoServicePurchasePaymentAllocation.service_purchase_payment_allocation_id);

                //If Pay Off Amount not zero (To Avoid Divide By Zero Exception)
                if (ldecPayOffAmountByAllocation > 0)
                    adecPSCToPost = ((ldecTotalPSC - icdoServicePurchaseHeader.paid_pension_service_credit) *
                                     (aobjServicePurchasePaymentAllocation.icdoServicePurchasePaymentAllocation.applied_amount / ldecPayOffAmountByAllocation));

            }

            // PIR 755 : Adjusting the Service Credit if it exceeds the Total Service Credit
            if (adecPSCToPost + icdoServicePurchaseHeader.paid_pension_service_credit > ldecTotalPSC)
            {
                adecPSCToPost = adecPSCToPost - (adecPSCToPost + icdoServicePurchaseHeader.paid_pension_service_credit - ldecTotalPSC);
            }


            //VSC to post
            if ((icdoServicePurchaseHeader.grant_free_flag == "Y") || (IsFreeServiceOnly()))
            {
                adecVSCToPost = ldecTotalVSC;
            }
            else if (icdoServicePurchaseHeader.service_purchase_type_value == busConstant.Service_Purchase_Type_Unused_Sick_Leave) //UAT PIR 726
            {
                if (ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.ready_for_posting_flag == busConstant.Flag_Yes && idecPayOffAmount <= 0)
                {
                    adecVSCToPost = ldecTotalVSC;
                }
                else
                {
                    adecVSCToPost = 0.00M;
                }
            }
            else if (icdoServicePurchaseHeader.service_purchase_type_value == busConstant.Service_Purchase_Type_USERRA_Military_Service)
            {
                //UAT PIR 828 - Post the Full Service Credit only when paid in full goes.
                bool lblnPaidInFull = (icdoServicePurchaseHeader.paid_rhic_cost_amount + icdoServicePurchaseHeader.paid_retirement_ee_cost_amount +
                       icdoServicePurchaseHeader.paid_retirement_er_cost_amount +
                       (adecPreTaxEE + adecPostTaxEE + adecPostTaxER + adecPostTaxEERHIC + adecPreTaxERRHIC + adecPreTaxER) >= ldecTotalContractAmt);
                if (lblnPaidInFull)
                {
                    adecVSCToPost = ldecTotalVSC;
                    //PIR 1871 - We are only posting PSC when VSC is already posted for a period in case of Paid USERRA.
                    if (_ibusPrimaryServicePurchaseDetail.iclbServicePurchaseDetailUserra == null)
                        _ibusPrimaryServicePurchaseDetail.LoadServicePurchaseDetailUSERRA();
                    if (adecVSCToPost > 0 && _ibusPrimaryServicePurchaseDetail.iclbServicePurchaseDetailUserra.Count > 0)
                    {
                        foreach (busServicePurchaseDetailUserra lbusUserra in ibusPrimaryServicePurchaseDetail.iclbServicePurchaseDetailUserra)
                        {
                            DataTable ldtblist = Select("cdoServicePurchaseHeader.IsVSCAlreadyPosted",
                                new object[2] { _ibusPersonAccount.icdoPersonAccount.person_account_id, lbusUserra.icdoServicePurchaseDetailUserra.missed_salary_month });

                            if (ldtblist.Rows.Count > 0)
                            {
                                adecVSCToPost--;
                            }
                        }
                        if (adecVSCToPost < 0) adecVSCToPost = 0;
                    }
                }
                else
                {
                    adecVSCToPost = 0.00M;
                }
            }
            else
            {
                //If Pay Off Amount not zero (To Avoid Divide By Zero Exception)
                if (ldecPayOffAmountByAllocation > 0)
                    adecVSCToPost = ((ldecTotalVSC - icdoServicePurchaseHeader.paid_vesting_service_credit) *
                                     (aobjServicePurchasePaymentAllocation.icdoServicePurchasePaymentAllocation.applied_amount / ldecPayOffAmountByAllocation));
            }

            // PIR 755 : Adjusting the Service Credit if it exceeds the Total Service Credit
            if (adecVSCToPost + icdoServicePurchaseHeader.paid_vesting_service_credit > ldecTotalVSC)
            {
                adecVSCToPost = adecVSCToPost - (adecVSCToPost + icdoServicePurchaseHeader.paid_vesting_service_credit - ldecTotalVSC);
            }

            // Free and dual service is not needded to be posted.
            adecPreTaxEE *= aintPostDirection;
            adecPreTaxER *= aintPostDirection;
            adecPostTaxEE *= aintPostDirection;
            adecPostTaxER *= aintPostDirection;
            adecPostTaxEERHIC *= aintPostDirection;
            adecPreTaxERRHIC *= aintPostDirection;
            adecEREEPickup *= aintPostDirection;

            if ((adecPreTaxEE != 0) ||
                (adecPostTaxEE != 0) ||
                (adecPostTaxER != 0) ||
                (adecPostTaxEERHIC != 0) ||
                (adecPreTaxERRHIC != 0) ||
                (adecPreTaxER != 0) ||
                (adecPSCToPost != 0) ||
                (adecVSCToPost != 0) ||
                (adecEREEPickup != 0))
            {
                if (_ibusPrimaryServicePurchaseDetail.iclbServicePurchaseDetailConsolidated == null)
                    _ibusPrimaryServicePurchaseDetail.LoadServicePurchaseDetailConsolidated();
                //?? We need to creat Person Account for not open accounts and then post to newly created accounts
                //?? nothing in UCS so wait till PIR
                string lstrTransactionType = busConstant.TransactionTypeServicePurchase;
                if (icdoServicePurchaseHeader.service_purchase_type_value == busConstant.Service_Purchase_Type_USERRA_Military_Service)
                    lstrTransactionType = busConstant.TransactionTypeRegularPayroll; // USERRA is treated as payroll so SubSystemValueEmployerReporting is being used.

                //PIR 1871 - For GRANT_FREE_FLAG = 'Y', we are inserting only VSC as 1 for each month.
                if ((icdoServicePurchaseHeader.service_purchase_type_value == busConstant.Service_Purchase_Type_USERRA_Military_Service) &&
                     (icdoServicePurchaseHeader.grant_free_flag == busConstant.Flag_Yes))
                {
                    DateTime ldtStartDate = _ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.userra_active_duty_start_date.GetFirstDayofCurrentMonth();
                    DateTime ldtEndDate = _ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.userra_active_duty_end_date.GetFirstDayofCurrentMonth();
                    if (ldtStartDate != DateTime.MinValue && ldtEndDate != DateTime.MinValue)
                    {
                        while (ldtStartDate <= ldtEndDate)
                        {
                            DataTable ldtbContList = Select("cdoServicePurchaseHeader.IsContributionAlreadyPosted",
                           new object[2] { _ibusPersonAccount.icdoPersonAccount.person_account_id, ldtStartDate });

                            if (ldtbContList.Rows.Count == 0)
                            {
                                _ibusPersonAccount.PostRetirementContribution(busConstant.SubSystemValueServicePurchase, icdoServicePurchaseHeader.service_purchase_header_id,
                              idatRunDate, ldtStartDate, 0, 0, 0, lstrTransactionType, 0, adecPostTaxER, adecPostTaxEE, adecPreTaxER, adecPreTaxEE,
                              adecPostTaxEERHIC, adecPreTaxERRHIC, adecEREEPickup, 0, 0, 1, adecPSCToPost);
                            }
                            ldtStartDate = ldtStartDate.AddMonths(1);
                        }
                    }
                }
                //PIR 1871 - For SEAS and LOA type details, VSC and PSC in Retirement contribution should not exceed one per month. 
                else if (icdoServicePurchaseHeader.service_purchase_type_value == busConstant.Service_Purchase_Type_Consolidated_Purchase && (_ibusPrimaryServicePurchaseDetail.iclbServicePurchaseDetailConsolidated.Any(i => (i.icdoServicePurchaseDetailConsolidated.service_credit_type_value == busConstant.Service_Purchase_Type_Leave_Of_Absence || i.icdoServicePurchaseDetailConsolidated.service_credit_type_value == busConstant.Service_Purchase_Type_Seasonal))))
                {

                    decimal tempVSC = adecVSCToPost;
                    decimal tempPSC = adecPSCToPost;
                    _ibusPersonAccount.PostRetirementContribution(busConstant.SubSystemValueServicePurchase, icdoServicePurchaseHeader.service_purchase_header_id,
                                    idatRunDate, adatPayment, 0, 0, 0, lstrTransactionType, 0, adecPostTaxER, adecPostTaxEE, adecPreTaxER, adecPreTaxEE,
                                                         adecPostTaxEERHIC, adecPreTaxERRHIC, adecEREEPickup, 0, 0, 0, 0);
                    foreach (busServicePurchaseDetailConsolidated lbusCons in _ibusPrimaryServicePurchaseDetail.iclbServicePurchaseDetailConsolidated)
                    {
                        if (lbusCons.icdoServicePurchaseDetailConsolidated.service_credit_type_value == busConstant.Service_Purchase_Type_Leave_Of_Absence || lbusCons.icdoServicePurchaseDetailConsolidated.service_credit_type_value == busConstant.Service_Purchase_Type_Seasonal)
                        {
                            DateTime ldtStartDate = lbusCons.icdoServicePurchaseDetailConsolidated.service_purchase_start_date.GetFirstDayofCurrentMonth();
                            DateTime ldtEndDate = lbusCons.icdoServicePurchaseDetailConsolidated.service_purchase_end_date.GetFirstDayofCurrentMonth();
                            if (ldtStartDate != DateTime.MinValue && ldtEndDate != DateTime.MinValue)
                            {
                                while (ldtStartDate <= ldtEndDate)
                                {
                                    DataTable ldtbContributions = Select("cdoServicePurchaseHeader.IsContributionLessThanOne",
                                                                   new object[2] { _ibusPersonAccount.icdoPersonAccount.person_account_id, ldtStartDate });
                                    decimal PostedVSC = 0, PostedPSC = 0;
                                    if (ldtbContributions.Rows.Count > 0)
                                    {
                                        PostedVSC = Convert.ToDecimal(ldtbContributions.Rows[0]["TOTAL_VSC"]);
                                        PostedPSC = Convert.ToDecimal(ldtbContributions.Rows[0]["TOTAL_PSC"]);
                                    }
                                    if (PostedPSC < 1 && PostedVSC < 1 && tempVSC > 0 && tempPSC > 0)
                                    {
                                        DataTable ldtbList = Select("cdoServicePurchaseHeader.GetMonthlyVSCAndPSC", new object[5]{tempVSC.ToString(), tempPSC.ToString() ,_ibusPersonAccount.icdoPersonAccount.person_account_id,
                                                                                                                                 icdoServicePurchaseHeader.service_purchase_header_id,ldtStartDate,});
                                        if (ldtbList.Rows.Count > 0)
                                        {
                                            decimal ldecVSCToPostMonthly = Convert.ToDecimal(ldtbList.Rows[0]["VSC_To_POST"]);
                                            decimal ldecPSCToPostMonthly = Convert.ToDecimal(ldtbList.Rows[0]["PSC_To_POST"]);
                                            decimal VSC = Convert.ToDecimal(ldtbList.Rows[0]["POSTED_VSC"]) + tempVSC;
                                            decimal PSC = Convert.ToDecimal(ldtbList.Rows[0]["POSTED_PSC"]) + tempPSC;
                                            if (VSC < 1 && PSC < 1)
                                            {
                                                _ibusPersonAccount.PostRetirementContribution(busConstant.SubSystemValueServicePurchase, icdoServicePurchaseHeader.service_purchase_header_id,
                                                                    idatRunDate, ldtStartDate, 0, 0, 0, lstrTransactionType, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, ldecVSCToPostMonthly, ldecPSCToPostMonthly);
                                                tempVSC = 0;
                                                tempPSC = 0;
                                                break;

                                            }
                                            else
                                            {
                                                _ibusPersonAccount.PostRetirementContribution(busConstant.SubSystemValueServicePurchase, icdoServicePurchaseHeader.service_purchase_header_id,
                                                                      idatRunDate, ldtStartDate, 0, 0, 0, lstrTransactionType, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, ldecVSCToPostMonthly, ldecPSCToPostMonthly);
                                                ldtStartDate = ldtStartDate.AddMonths(1);
                                                tempVSC = tempVSC - ldecVSCToPostMonthly;
                                                tempPSC = tempPSC - ldecPSCToPostMonthly;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ldtStartDate = ldtStartDate.AddMonths(1);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (tempVSC > 0 && tempPSC > 0)
                                _ibusPersonAccount.PostRetirementContribution(busConstant.SubSystemValueServicePurchase, icdoServicePurchaseHeader.service_purchase_header_id,
                                  idatRunDate, adatPayment, 0, 0, 0, lstrTransactionType, 0, 0, 0, 0, 0,
                                  0, 0, 0, 0, 0, tempVSC, tempPSC);

                            tempVSC = 0;
                            tempPSC = 0;
                        }

                    }

                }
                else
                {
                    _ibusPersonAccount.PostRetirementContribution(busConstant.SubSystemValueServicePurchase, icdoServicePurchaseHeader.service_purchase_header_id,
                              idatRunDate, adatPayment, 0, 0, 0, lstrTransactionType, 0, adecPostTaxER, adecPostTaxEE, adecPreTaxER, adecPreTaxEE,
                              adecPostTaxEERHIC, adecPreTaxERRHIC, adecEREEPickup, 0, 0, adecVSCToPost, adecPSCToPost);
                }

                // Create salary records in contributions for USERRA
                if ((icdoServicePurchaseHeader.service_purchase_type_value == busConstant.Service_Purchase_Type_USERRA_Military_Service) &&
                    (icdoServicePurchaseHeader.grant_free_flag != "Y"))
                {
                    //UAT PIR 828 - Post only when paid in full                    
                    bool lblnPaidInFull = ((icdoServicePurchaseHeader.paid_rhic_cost_amount + icdoServicePurchaseHeader.paid_retirement_ee_cost_amount +
                        icdoServicePurchaseHeader.paid_retirement_er_cost_amount +
                        (adecPreTaxEE + adecPostTaxEE + adecPostTaxER + adecPostTaxEERHIC + adecPreTaxERRHIC + adecPreTaxER) >= ldecTotalContractAmt) &&
                        (icdoServicePurchaseHeader.paid_vesting_service_credit + adecVSCToPost >= ldecTotalVSC));

                    if (lblnPaidInFull)
                    {
                        if (_ibusPrimaryServicePurchaseDetail.iclbServicePurchaseDetailUserra == null)
                            _ibusPrimaryServicePurchaseDetail.LoadServicePurchaseDetailUSERRA();
                        foreach (busServicePurchaseDetailUserra lbusUSERRADtl in _ibusPrimaryServicePurchaseDetail.iclbServicePurchaseDetailUserra)
                        {
                            if (lbusUSERRADtl.icdoServicePurchaseDetailUserra.posted_flag != "Y")
                            {   // USERRA is treated as payroll so SubSystemValueEmployerReporting is being used.
                                _ibusPersonAccount.PostRetirementContribution(busConstant.SubSystemValueServicePurchase, icdoServicePurchaseHeader.service_purchase_header_id,
                                    idatRunDate, lbusUSERRADtl.icdoServicePurchaseDetailUserra.missed_salary_month, lbusUSERRADtl.icdoServicePurchaseDetailUserra.missed_salary_month.Month,
                                    lbusUSERRADtl.icdoServicePurchaseDetailUserra.missed_salary_month.Year, 0, busConstant.TransactionTypeRegularPayroll,
                                    aintPostDirection * lbusUSERRADtl.icdoServicePurchaseDetailUserra.missed_salary_amount, 0, 0, 0, 0,
                                    0, 0, 0, 0, 0, 0, 0);
                                lbusUSERRADtl.icdoServicePurchaseDetailUserra.posted_flag = "Y";
                                lbusUSERRADtl.icdoServicePurchaseDetailUserra.service_purchase_payment_allocation_id =
                                    aobjServicePurchasePaymentAllocation.icdoServicePurchasePaymentAllocation.service_purchase_payment_allocation_id;
                                lbusUSERRADtl.icdoServicePurchaseDetailUserra.Update();

                                //UAT PIR 828 : Post PEP Adjustmebt
                                busPostingVestedERContributionBatch lobjVestedERCont = new busPostingVestedERContributionBatch();
                                if ((icdoServicePurchaseHeader.plan_id == 1) ||
                                     (icdoServicePurchaseHeader.plan_id == busConstant.PlanIdMain2020) || // PIR 20232
                                    (icdoServicePurchaseHeader.plan_id == 2) ||
                                    (icdoServicePurchaseHeader.plan_id == 3) ||
                                    (icdoServicePurchaseHeader.plan_id == 20) ||
                                    (icdoServicePurchaseHeader.plan_id == busConstant.PlanIdBCILawEnf) || //pir 7943
                                    (icdoServicePurchaseHeader.plan_id == busConstant.PlanIdStatePublicSafety)) //PIR 25729
                                {
                                    if (ibusPersonEmploymentDetail == null)
                                        LoadLatestEmploymentDetail();
                                    lobjVestedERCont.CalculatePEPAdjustment(
                                        icdoServicePurchaseHeader.plan_id,
                                        icdoServicePurchaseHeader.person_id,
                                        lbusUSERRADtl.icdoServicePurchaseDetailUserra.missed_salary_month,
                                        icdoServicePurchaseHeader.service_purchase_header_id,
                                        string.Empty,
                                        ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id,
                                        idatRunDate, _ibusPersonAccount.icdoPersonAccount.person_account_id, busConstant.SubSystemValueServicePurchase);
                                }
                            }
                        }
                    }
                }
                icdoServicePurchaseHeader.paid_rhic_cost_amount += adecPostTaxEERHIC + adecPreTaxERRHIC;
                icdoServicePurchaseHeader.paid_retirement_er_cost_amount += adecPostTaxER + adecPreTaxER + adecEREEPickup;
                icdoServicePurchaseHeader.paid_retirement_ee_cost_amount += adecPreTaxEE + adecPostTaxEE;
                icdoServicePurchaseHeader.paid_pension_service_credit += adecPSCToPost;
                icdoServicePurchaseHeader.paid_vesting_service_credit += adecVSCToPost;
                icdoServicePurchaseHeader.paid_contract_amount_used = ldecTotalContractAmt;
                GetPersonAccountAndInitiateWorkflow(); //PIR - 9529 - Need to initiate appropriate WFLs if additional contribution is posted after payee account is setup
                //Judge Conversion
                if (_ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.judges_conversion_flag == "Y")
                {
                    // Take only this service out from MAIN plan

                    // Idnetify the Main account in concern.
                    //?? This is the best I can think off.
                    //?? UCS should have explained this
                    busPersonAccount lbusPersonAccountMain = null;
                    if (_ibusPerson.iclbRetirementAccount == null)
                        _ibusPerson.LoadRetirementAccount();

                    if (_ibusPrimaryServicePurchaseDetail.iclbServicePurchaseDetailConsolidated == null)
                        _ibusPrimaryServicePurchaseDetail.LoadServicePurchaseDetailConsolidated();

                    foreach (busPersonAccount lbusPersonAccount in _ibusPerson.iclbRetirementAccount)
                    {
                        if (lbusPersonAccount.IsRetirementPlanOpen())
                        {
                            foreach (busServicePurchaseDetailConsolidated lbusJudgeConversionDtl in _ibusPrimaryServicePurchaseDetail.iclbServicePurchaseDetailConsolidated)
                            {
                                if ((lbusJudgeConversionDtl.icdoServicePurchaseDetailConsolidated.service_purchase_start_date <= lbusPersonAccount.icdoPersonAccount.end_date_no_null) &&
                                    (lbusJudgeConversionDtl.icdoServicePurchaseDetailConsolidated.service_purchase_end_date >= lbusPersonAccount.icdoPersonAccount.start_date))
                                {
                                    lbusPersonAccountMain = lbusPersonAccount;
                                    break;
                                }

                            }
                        }
                        if (lbusPersonAccountMain != null)
                            break;
                    }
                    if (lbusPersonAccountMain == null)
                        throw new Exception("No person account for Main plan is available");
                    lbusPersonAccountMain.PostRetirementContribution(busConstant.SubSystemValueServicePurchase, icdoServicePurchaseHeader.service_purchase_header_id,
                         idatRunDate, adatPayment, 0, 0, 0, busConstant.TransactionTypeServicePurchase, 0, 0, 0, 0, 0,
                         0, 0, 0, 0, 0, -1 * adecVSCToPost, -1 * adecPSCToPost);
                }
            }
            // Update hdr status
            //?? Again need to see if I am taking right amount fields
			//service purchase PIR-10439 repurposed under PIR-9224
            if ((icdoServicePurchaseHeader.paid_rhic_cost_amount + icdoServicePurchaseHeader.paid_retirement_ee_cost_amount + icdoServicePurchaseHeader.paid_retirement_er_cost_amount == 0) &&
                (icdoServicePurchaseHeader.paid_pension_service_credit == 0) &&
                (icdoServicePurchaseHeader.paid_vesting_service_credit == 0))
                icdoServicePurchaseHeader.action_status_value = busConstant.Service_Purchase_Action_Status_Approved;
            else
            { 
                decimal idecServicePurchaseCloseVariance = Convert.ToDecimal(busGlobalFunctions.GetData1ByCodeValue(busConstant.SystemConstantsAndVariablesCodeID, busConstant.ServicePurchaseCloseVariance, iobjPassInfo));


                if ((icdoServicePurchaseHeader.grant_free_flag == "Y") || IsFreeServiceOnly() ||
                    ((  
                    (Math.Round((icdoServicePurchaseHeader.paid_rhic_cost_amount + icdoServicePurchaseHeader.paid_retirement_ee_cost_amount + icdoServicePurchaseHeader.paid_retirement_er_cost_amount), 2, MidpointRounding.AwayFromZero)) >=
                    (_ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.retirement_purchase_cost +
                    _ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.rhic_purchase_cost))
                    && ((icdoServicePurchaseHeader.paid_vesting_service_credit >= ldecTotalVSC) || (Math.Abs(icdoServicePurchaseHeader.paid_vesting_service_credit - ldecTotalVSC) <= idecServicePurchaseCloseVariance)) &&
                    ((icdoServicePurchaseHeader.paid_pension_service_credit >= ldecTotalPSC) || (Math.Abs(icdoServicePurchaseHeader.paid_pension_service_credit - ldecTotalPSC) <= idecServicePurchaseCloseVariance))))
                {
                    decimal adecAdjustedPSC = 0.00M;
                    decimal adecAdjustedVSC = 0.00M;
                    adecAdjustedPSC = ldecTotalPSC - icdoServicePurchaseHeader.paid_pension_service_credit;
                    adecAdjustedVSC = ldecTotalVSC - icdoServicePurchaseHeader.paid_vesting_service_credit;

                    if ((adecAdjustedPSC > 0) || (adecAdjustedVSC > 0))
                    {
                        ibusPersonAccount.PostRetirementContribution(busConstant.SubSystemValueServicePurchase, icdoServicePurchaseHeader.service_purchase_header_id, DateTime.Now, DateTime.Now, 0, 0, 0, busConstant.TransactionTypePurchaseAdjustment, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, adecAdjustedVSC, adecAdjustedPSC);
                    }

                    icdoServicePurchaseHeader.action_status_value = busConstant.Service_Purchase_Action_Status_Paid_In_Full;

                }
                else
                    icdoServicePurchaseHeader.action_status_value = busConstant.Service_Purchase_Action_Status_In_Payment;
            }
            icdoServicePurchaseHeader.Update();
        }
        protected Collection<busServiceCreditPlanFormulaRef> _iclbCachedFormulaRef;
        /// <summary>
        /// //PIR - 9529 - Need to initiate appropriate WFLs if additional contribution is posted after payee account setup
        /// </summary>
        public void GetPersonAccountAndInitiateWorkflow()
        {
            if (_ibusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementRetired ||
                _ibusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetimentSuspended ||
                _ibusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusPreRetirementDeath)
            {

                if (_ibusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusPreRetirementDeath)
                {
                    CheckConditionForPreRetirementWorkflow();
                }
                else
                {
                    CheckConditionsForWorkflow();
                }
            }

        }
        private void CheckConditionForPreRetirementWorkflow()
        {
            //Loading member person
            if (_ibusPersonAccount.ibusPerson == null)
                _ibusPersonAccount.LoadPerson();
            //Loading member persons' benefit applications
            if (_ibusPersonAccount.ibusPerson.iclbBenefitApplication == null)
                _ibusPersonAccount.ibusPerson.LoadBenefitApplication();
            //Finding preretirementdeath benefit application not in denied, deferred, or canceled status
            busBenefitApplication lobjPreRetrApplication = _ibusPersonAccount.ibusPerson.iclbBenefitApplication
                                                                .Where(o => o.icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath &&
                                                                    o.icdoBenefitApplication.action_status_value != busConstant.ApplicationActionStatusDenied &&
                                                                    o.icdoBenefitApplication.action_status_value != busConstant.ApplicationActionStatusDeferred &&
                                                                    o.icdoBenefitApplication.action_status_value != busConstant.ApplicationActionStatusCancelled)
                                                                .FirstOrDefault();
            if (lobjPreRetrApplication != null)
            {
                //Loading benefitapplicationpersonaccounts of the preretirementdeath benefit application not in denied, deferred, or canceled status
                if (lobjPreRetrApplication.iclbBenefitApplicationPersonAccounts == null)
                    lobjPreRetrApplication.LoadBenefitApplicationPersonAccount();
                busBenefitApplicationPersonAccount lobjBAPA = lobjPreRetrApplication.iclbBenefitApplicationPersonAccounts
                                                                .Where(o => o.icdoBenefitApplicationPersonAccount.is_application_person_account_flag == busConstant.Flag_Yes)
                                                                .FirstOrDefault();
                if (lobjPreRetrApplication != null && lobjBAPA != null &&
                    lobjBAPA.icdoBenefitApplicationPersonAccount.person_account_id == _ibusPersonAccount.icdoPersonAccount.person_account_id)
                {
                    lobjPreRetrApplication.LoadPayeeAccount();
                    foreach (busPayeeAccount lobjPayeeAccount in lobjPreRetrApplication.iclbPayeeAccount)
                    {
                        lobjPayeeAccount.LoadActivePayeeStatusAsofNextBenefitPaymentDate();
                        //laoding the data 2 for PAS
                        DataTable ldtbStatus = iobjPassInfo.isrvDBCache.GetCodeDescription(2203, lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value);
                        lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 = ldtbStatus.Rows.Count > 0 ? ldtbStatus.Rows[0]["data2"].ToString()
                                                                                                                                            : string.Empty;

                        if ((lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 == busConstant.PayeeAccountStatusApproved &&
                        _ibusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetimentSuspended)
                        || lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 == busConstant.PayeeAccountStatusReceiving
                        || (lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 == busConstant.PayeeAccountStatusReview))
                        {
                            if (!busWorkflowHelper.IsActiveInstanceAvailable(lobjPayeeAccount.icdoPayeeAccount.payee_perslink_id,
                                busConstant.Map_Initialize_Process_Beneficiary_Auto_Refund_Workflow))
                            {
                                InitiateWorkflow(lobjPreRetrApplication.icdoBenefitApplication.member_person_id, lobjPayeeAccount.icdoPayeeAccount.payee_perslink_id, lobjPayeeAccount.icdoPayeeAccount.application_id,
                                    busConstant.Map_Initialize_Process_Beneficiary_Auto_Refund_Workflow);
                                lobjPayeeAccount.CreateReviewPayeeAccountStatus();
                                lobjPayeeAccount.iblnAddionalContributionsIndicatorFlag = true;
                                lobjPayeeAccount.ValidateSoftErrors();
                                lobjPayeeAccount.UpdateValidateStatus();
                            }
                        }
                    }
                }
            }
        }

        private void CheckConditionsForWorkflow()
        {
            //Loading the person
            if (_ibusPersonAccount.ibusPerson == null)
                _ibusPersonAccount.LoadPerson();
            //Loading member payee account
            _ibusPersonAccount.ibusPerson.LoadMemberPayeeAccount();
            if (_ibusPersonAccount.ibusPerson.ibusMemberPayeeAccount != null &&
                _ibusPersonAccount.ibusPerson.ibusMemberPayeeAccount.icdoPayeeAccount.payee_account_id > 0)
            {
                if (_ibusPersonAccount.ibusPerson.ibusMemberPayeeAccount.ibusApplication == null)
                    _ibusPersonAccount.ibusPerson.ibusMemberPayeeAccount.LoadApplication();
                if (_ibusPersonAccount.ibusPerson.ibusMemberPayeeAccount.ibusApplication.iclbBenefitApplicationPersonAccounts == null)
                    _ibusPersonAccount.ibusPerson.ibusMemberPayeeAccount.ibusApplication.LoadBenefitApplicationPersonAccount();
                busBenefitApplicationPersonAccount lobjBAPA = ibusPersonAccount.ibusPerson.ibusMemberPayeeAccount.ibusApplication.iclbBenefitApplicationPersonAccounts
                    .Where(o => o.icdoBenefitApplicationPersonAccount.is_application_person_account_flag == busConstant.Flag_Yes).FirstOrDefault();
                if (lobjBAPA != null && lobjBAPA.icdoBenefitApplicationPersonAccount.person_account_id == _ibusPersonAccount.icdoPersonAccount.person_account_id)
                {
                    //loading active payee account status
                    _ibusPersonAccount.ibusPerson.ibusMemberPayeeAccount.LoadActivePayeeStatusAsofNextBenefitPaymentDate();
                    //laoding the data 2 for PAS
                    DataTable ldtbStatus = iobjPassInfo.isrvDBCache.GetCodeDescription(2203,
                        _ibusPersonAccount.ibusPerson.ibusMemberPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value);
                    _ibusPersonAccount.ibusPerson.ibusMemberPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 =
                        ldtbStatus.Rows.Count > 0 ? ldtbStatus.Rows[0]["data2"].ToString() : string.Empty;
                    //checking whether PAS is in Receiving or Approved
                    if ( _ibusPersonAccount.ibusPerson.ibusMemberPayeeAccount.icdoPayeeAccount.benefit_account_type_value != busConstant.ApplicationBenefitTypeRefund &&
                        ((_ibusPersonAccount.ibusPerson.ibusMemberPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 == busConstant.PayeeAccountStatusApproved &&
                            (_ibusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled ||
                            _ibusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetimentSuspended)) // PROD PIR 8312
                        || _ibusPersonAccount.ibusPerson.ibusMemberPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 == busConstant.PayeeAccountStatusReceiving
                        || (_ibusPersonAccount.ibusPerson.ibusMemberPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 == busConstant.PayeeAccountStatusReview)))
                    {
                        if (!busWorkflowHelper.IsActiveInstanceAvailable(_ibusPersonAccount.icdoPersonAccount.person_id, busConstant.Map_Recalculate_Pension_and_RHIC_Benefit))
                        {
                            InitiateWorkflow(_ibusPersonAccount.ibusPerson.icdoPerson.person_id, 0,
                                                _ibusPersonAccount.ibusPerson.ibusMemberPayeeAccount.icdoPayeeAccount.payee_account_id,
                                                busConstant.Map_Recalculate_Pension_and_RHIC_Benefit);
                            _ibusPersonAccount.ibusPerson.ibusMemberPayeeAccount.CreateReviewPayeeAccountStatus();
                            _ibusPersonAccount.ibusPerson.ibusMemberPayeeAccount.iblnAddionalContributionsIndicatorFlag = true;
                            _ibusPersonAccount.ibusPerson.ibusMemberPayeeAccount.ValidateSoftErrors();
                            _ibusPersonAccount.ibusPerson.ibusMemberPayeeAccount.UpdateValidateStatus();
                        }
                    }
                }
            }
            else
            {
                DataTable ldtbFinalDisOrRetCalculationsExcCanceled = busBase.Select("cdoPersonAccountAdjustment.GetRetOrDisFinalCalculations",
                                                                        new object[1] { _ibusPersonAccount.ibusPerson.icdoPerson.person_id });

                Collection<busBenefitCalculation> lclcFinalDisOrRetCalculationsExcCanceled = GetCollection<busBenefitCalculation>(ldtbFinalDisOrRetCalculationsExcCanceled, "icdoBenefitCalculation");
                if (_ibusPersonAccount.ibusPerson.iclbRetirementAccount.IsNull())
                    _ibusPersonAccount.ibusPerson.LoadRetirementAccount();
                if (lclcFinalDisOrRetCalculationsExcCanceled.Count > 0 && !(_ibusPersonAccount.ibusPerson
                                                                                    .iclbRetirementAccount
                                                                                    .Where(i => i.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled)
                                                                                    .Any()))
                {
                    InitiateWorkflow(_ibusPersonAccount.icdoPersonAccount.person_id,
                            0,
                            0,
                            busConstant.Map_Recalculate_Pension_and_RHIC_Benefit);
                }
            }
        }
        public void InitiateWorkflow(int aintPersonID, int aintBeneficiaryID, int aintReferenceID, int aintTypeID)
        {
            if (!busWorkflowHelper.IsActiveInstanceAvailable(aintPersonID, aintTypeID))
            {
                Dictionary<string, object> ldctParams = new Dictionary<string, object>();
                ldctParams["additional_parameter1"] = aintBeneficiaryID > 0 ? aintBeneficiaryID.ToString() : "";
                busWorkflowHelper.InitiateBpmRequest(aintTypeID, aintPersonID, 0, aintReferenceID, iobjPassInfo, busConstant.WorkflowProcessSource_Batch, adictInstanceParameters: ldctParams);

            }
        }
        public void GetServiceToPost(int aintPlanId, string astrServicePurchaseType, string astrPayorType, decimal ldecServiceToPost,
            ref decimal adecPSCToPost, ref decimal adecVSCToPost)
        {
            if (_iclbCachedFormulaRef == null)
            {
                DataTable ldtbList = iobjPassInfo.isrvDBCache.GetCacheData("sgt_service_credit_plan_formula_ref", null);
                _iclbCachedFormulaRef = GetCollection<busServiceCreditPlanFormulaRef>(ldtbList, "icdoServiceCreditPlanFormulaRef");
            }
            bool lblnFound = false;
            foreach (busServiceCreditPlanFormulaRef lbusFormulaRef in _iclbCachedFormulaRef)
            {
                if ((lbusFormulaRef.icdoServiceCreditPlanFormulaRef.service_credit_type_value == astrServicePurchaseType) &&
                    (lbusFormulaRef.icdoServiceCreditPlanFormulaRef.payor_value == astrPayorType) &&
                    (lbusFormulaRef.icdoServiceCreditPlanFormulaRef.plan_id == aintPlanId))
                {
                    lblnFound = true;
                    if (lbusFormulaRef.icdoServiceCreditPlanFormulaRef.applicable_to_pension_flag == "Y")
                        adecPSCToPost = ldecServiceToPost;
                    if (lbusFormulaRef.icdoServiceCreditPlanFormulaRef.applicable_to_vesting_flag == "Y")
                        adecVSCToPost = ldecServiceToPost;
                    break;
                }
            }

            //PIR 716, 756 : Ignore the PSC for Grant Free USERRa Purchase.
            //As of now, we are handling hard coded way for this scenario
            if ((icdoServicePurchaseHeader.service_purchase_type_value == busConstant.Service_Purchase_Type_USERRA_Military_Service) &&
                (icdoServicePurchaseHeader.grant_free_flag == busConstant.Flag_Yes))
            {
                adecPSCToPost = 0;
            }


            if (!lblnFound)
                throw new Exception("No data found in Formula Reference." +
                    " Plan ID = " + aintPlanId.ToString() +
                    " Payor = " + astrPayorType +
                    " Service Purchase Type = " + astrServicePurchaseType);
        }
        /// <summary>
        /// Distributes remittance to the various buckets as explained by UCS-044
        /// This method sums up them so send in the assigned variables for current payments
        /// 
        /// </summary>
        /// <param name="abusRemittance"></param>
        /// <returns></returns>
        public decimal GetDistributedRemittance(string astrPaymentClass, decimal adecPaymentAmt, ref decimal adecPreTaxEE, ref decimal adecPostTaxEE, ref decimal adecPostTaxER,
            ref decimal adecPostTaxEERHIC, ref decimal adecPreTaxERRHIC, ref decimal adecPreTaxER, ref decimal adecEREEPickup)
        {
            adecPreTaxEE = 0;
            adecPostTaxEE = 0;
            adecPostTaxER = 0;
            adecPostTaxEERHIC = 0;
            adecPreTaxERRHIC = 0;
            adecPreTaxER = 0;
            adecEREEPickup = 0;

            if (_ibusPersonAccount == null)
                LoadPersonAccount();

            if (_ibusPrimaryServicePurchaseDetail == null)
                LoadServicePurchaseDetail();

            decimal ldecReturn = 0;
            decimal ldecRetirementCost = 0;
            decimal ldecRHICCost = 0;

            // Cost split
            if (icdoServicePurchaseHeader.service_purchase_type_value == busConstant.Service_Purchase_Type_Unused_Sick_Leave)
            {
                ldecRetirementCost = _ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.retirement_cost_for_sick_leave_purchase;
                ldecRHICCost = _ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.rhic_cost_for_sick_leave_purchase;
            }
            else
            { //?? Is this right?
                ldecRetirementCost = _ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.retirement_purchase_cost;
                ldecRHICCost = _ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.rhic_purchase_cost;
            }
            decimal ldecRHICToBePaid = ldecRHICCost - icdoServicePurchaseHeader.paid_rhic_cost_amount;

            //Allocating buckets
            if (astrPaymentClass == busConstant.Service_Purchase_Class_Employer_Rollover)
                adecPreTaxEE += adecPaymentAmt;
            else if (astrPaymentClass == busConstant.Service_Purchase_Class_Employer_Installment_PreTax)
            {
                if (_ibusPersonAccount.ibusPlan == null)
                    _ibusPersonAccount.LoadPlan();
                if ((icdoServicePurchaseHeader.payor_value == busConstant.Service_Purchase_Payor_Employer_And_Employee) &&
                    (icdoServicePurchaseHeader.service_purchase_type_value == busConstant.Service_Purchase_Type_USERRA_Military_Service) &&
                    (_ibusPersonAccount.ibusPlan.IsDCRetirementPlan() || _ibusPersonAccount.ibusPlan.IsHBRetirementPlan()))
                {
                    adecPreTaxEE += adecPaymentAmt; //There were difference code before.. removed now.. //UAT PIR 1178
                }
                else
                    adecPreTaxEE += adecPaymentAmt;
            }
            else if ((astrPaymentClass == busConstant.Service_Purchase_Class_Employer_Installment_PostTax) ||
                (astrPaymentClass == busConstant.Service_Purchase_Class_Employer_Down_Payment))
            {
                if (_ibusPersonAccount.ibusPlan == null)
                    _ibusPersonAccount.LoadPlan();
                if ((icdoServicePurchaseHeader.payor_value == busConstant.Service_Purchase_Payor_Employer_And_Employee) &&
                    (icdoServicePurchaseHeader.service_purchase_type_value == busConstant.Service_Purchase_Type_USERRA_Military_Service) &&
                    ((astrPaymentClass == busConstant.Service_Purchase_Class_Employer_Installment_PostTax) ||
                    (astrPaymentClass == busConstant.Service_Purchase_Class_Employer_Down_Payment)) && //UAT PIR 743
                    (_ibusPersonAccount.ibusPlan.IsDBRetirementPlan()))
                {
                    //20081116 BR1 101 -- rest are implicit
                    adecPostTaxEE += adecPaymentAmt;
                }
                else if ((icdoServicePurchaseHeader.payor_value == busConstant.Service_Purchase_Payor_Employer_And_Employee) &&
                    (icdoServicePurchaseHeader.service_purchase_type_value == busConstant.Service_Purchase_Type_USERRA_Military_Service) &&
                    (_ibusPersonAccount.ibusPlan.IsDCRetirementPlan() || _ibusPersonAccount.ibusPlan.IsHBRetirementPlan()))
                {
                    adecPostTaxEE += adecPaymentAmt;
                }
                else
                {
                    adecPostTaxEE += adecPaymentAmt;
                    if (ldecRHICCost > 0)
                    { //?? Is EE RHIC correct
                        //?? Service_Purchase_Class_Employer_RHIC_Lumpsum -- Employer word here troubled me hell.
                        decimal ldecPostTaxEERHIC = adecPaymentAmt * (ldecRHICCost / (ldecRHICCost + ldecRetirementCost));
                        if (adecPostTaxEERHIC + ldecPostTaxEERHIC > ldecRHICToBePaid)
                            ldecPostTaxEERHIC = ldecRHICToBePaid - adecPostTaxEERHIC;
                        adecPostTaxEERHIC += ldecPostTaxEERHIC;
                        adecPostTaxEE -= ldecPostTaxEERHIC;
                    }
                    else
                        adecPostTaxEERHIC += 0;
                }
            }
            else if (astrPaymentClass == busConstant.Service_Purchase_Class_Employer_RHIC_Lumpsum)
                adecPostTaxEERHIC += adecPaymentAmt;
            else if (astrPaymentClass == busConstant.Service_Purchase_Class_Employer_USERRA_LumpSum)
            {
                decimal ldecPreTaxERRHIC = adecPaymentAmt;
                if (adecPreTaxERRHIC + ldecPreTaxERRHIC > ldecRHICToBePaid)
                    ldecPreTaxERRHIC = ldecRHICToBePaid - adecPreTaxERRHIC;
                adecPreTaxERRHIC += ldecPreTaxERRHIC;

                decimal ldecPreTaxER = adecPaymentAmt - ldecPreTaxERRHIC;
                if (adecPaymentAmt - ldecPreTaxERRHIC > 0)
                {
                    decimal ldecERCostAmtToBePaid = _ibusPrimaryServicePurchaseDetail.GetUSERRAERCostAmt()
                        - icdoServicePurchaseHeader.paid_retirement_er_cost_amount;
                    if (adecPreTaxER + ldecPreTaxER > ldecERCostAmtToBePaid)
                        ldecPreTaxER = ldecERCostAmtToBePaid - adecPreTaxER;
                    adecPreTaxER += ldecPreTaxER;
                }
                if (adecPaymentAmt - ldecPreTaxERRHIC - ldecPreTaxER > 0)
                {
                    adecPreTaxEE += adecPaymentAmt - ldecPreTaxERRHIC - ldecPreTaxER;
                }
            }
            else if (astrPaymentClass == busConstant.Service_Purchase_Class_Employer_LumpSum)
            {
                decimal ldecPreTaxERRHIC = adecPaymentAmt;
                if (adecPreTaxERRHIC + ldecPreTaxERRHIC > ldecRHICToBePaid)
                    ldecPreTaxERRHIC = ldecRHICToBePaid - adecPreTaxERRHIC;
                adecPreTaxERRHIC += ldecPreTaxERRHIC;
                adecPreTaxER = adecPaymentAmt - ldecPreTaxERRHIC;
            }
            else if (astrPaymentClass == busConstant.Service_Purchase_Class_Employer_USERRA_Installment) //UAT PIR 1178 - Enhancement
            {
                //BR 102
                decimal ldecERCostAmtToBePaid = _ibusPrimaryServicePurchaseDetail.GetUSERRAERCostAmt() -
                    icdoServicePurchaseHeader.paid_retirement_er_cost_amount;

                // Should be OK to call this method for performance 
                decimal ldecRHIC = adecPaymentAmt *
                    (_ibusPrimaryServicePurchaseDetail.GetUSERRARHICCostAmt() /
                    (_ibusPrimaryServicePurchaseDetail.GetUSERRARHICCostAmt() +
                    _ibusPrimaryServicePurchaseDetail.GetUSERRAERCostAmt()));
                decimal ldecER = adecPaymentAmt *
                    (_ibusPrimaryServicePurchaseDetail.GetUSERRAERCostAmt() /
                    (_ibusPrimaryServicePurchaseDetail.GetUSERRARHICCostAmt() +
                    _ibusPrimaryServicePurchaseDetail.GetUSERRAERCostAmt()));

                if (ldecRHIC + adecPreTaxERRHIC + adecPostTaxEERHIC > ldecRHICToBePaid)
                    ldecRHIC = ldecRHICToBePaid - (adecPreTaxERRHIC + adecPostTaxEERHIC);
                adecPreTaxERRHIC += ldecRHIC;

                if (adecPostTaxER + adecPreTaxER + ldecER > ldecERCostAmtToBePaid)
                    ldecER = ldecERCostAmtToBePaid - (adecPostTaxER + adecPreTaxER);
                adecPreTaxER += ldecER;
            }
            else if (astrPaymentClass == busConstant.Service_Purchase_Class_Employer_USERRA_EE_Pickup) //UAT PIR 1178 - Enhancement
            {
                adecEREEPickup += adecPaymentAmt;
            }
            if (adecPostTaxEERHIC + adecPreTaxERRHIC > ldecRHICToBePaid)
                throw new Exception("Paid RHIC amount exceeds balance RHIC amount.");
            return ldecReturn;
        }


        ////BR - 044 - 12
        ////Load Plan by member s contributing status from employment detail
        public Collection<cdoPlan> LoadPlanByContributingStatus()
        {
            bool lblnIsPlanJudges = false;
            Collection<cdoPlan> lclbPlan = new Collection<cdoPlan>();

            if (ibusPerson.iclbRetirementAccount == null)
                ibusPerson.LoadRetirementAccount();

            //Exclude withdrawn and transferred DC status person accounts
            var lenuList = ibusPerson.iclbRetirementAccount.Where(i => ((!i.IsWithDrawn()) && (!i.IsTransferredDC())));
            foreach (busPersonAccount lbusPersonAccount in lenuList)
            {
                if (lbusPersonAccount.iclbAccountEmploymentDetail == null)
                    lbusPersonAccount.LoadPersonAccountEmploymentDetails();

                foreach (busPersonAccountEmploymentDetail lbusPAEmpDetail in lbusPersonAccount.iclbAccountEmploymentDetail)
                {
                    if (lbusPAEmpDetail.ibusEmploymentDetail == null)
                        lbusPAEmpDetail.LoadPersonEmploymentDetail();

                    if (lbusPAEmpDetail.ibusEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusContributing)
                    {
                        if (!lclbPlan.Any(i => i.plan_id == lbusPersonAccount.icdoPersonAccount.plan_id))
                        {
                            if (lbusPersonAccount.ibusPlan == null)
                                lbusPersonAccount.LoadPlan();
                            lclbPlan.Add(lbusPersonAccount.ibusPlan.icdoPlan);

                            //if Contributing plan is judges load all plan
                            if (lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdJudges)
                            {
                                lblnIsPlanJudges = true;
                                break;
                            }
                        }
                    }
                }

                if (lblnIsPlanJudges) break;
            }

            //For Judges, we need to add all enrolled retirement plans
            if (lblnIsPlanJudges)
            {
                lclbPlan = new Collection<cdoPlan>();
                foreach (busPersonAccount lbusPersonAccount in ibusPerson.iclbRetirementAccount)
                {
                    if (lbusPersonAccount.ibusPlan == null)
                        lbusPersonAccount.LoadPlan();

                    if (!lclbPlan.Any(i => i.plan_id == lbusPersonAccount.icdoPersonAccount.plan_id))
                    {
                        lclbPlan.Add(lbusPersonAccount.ibusPlan.icdoPlan);
                    }

                }
            }
            return lclbPlan;
        }

        public Collection<cdoPersonEmploymentDetail> LoadMemberTypeByContributingStatus()
        {
            Collection<cdoPersonEmploymentDetail> lclbPersonEmpDetail = new Collection<cdoPersonEmploymentDetail>();

            if (ibusPerson.iclbRetirementAccount == null)
                ibusPerson.LoadRetirementAccount();
            //PIR 18363 - Dual Member service purchase benefit calculation issue - 
            //Load member type by plan
            var lenuList = iblnLoadMemberTypeByPlan ? ibusPerson
                                                        .iclbRetirementAccount
                                                        .Where(i => i.icdoPersonAccount.plan_id == icdoServicePurchaseHeader.plan_id && 
                                                                ((!i.IsWithDrawn()) && (!i.IsTransferredDC()))) : 
                                                        ibusPerson
                                                        .iclbRetirementAccount
                                                        .Where(i => ((!i.IsWithDrawn()) && (!i.IsTransferredDC())));

            foreach (busPersonAccount lbusPersonAccount in lenuList)
            {
                if (lbusPersonAccount.iclbAccountEmploymentDetail == null)
                    lbusPersonAccount.LoadPersonAccountEmploymentDetails();

                foreach (busPersonAccountEmploymentDetail lbusPAEmpDetail in lbusPersonAccount.iclbAccountEmploymentDetail)
                {
                    if (lbusPAEmpDetail.ibusEmploymentDetail == null)
                        lbusPAEmpDetail.LoadPersonEmploymentDetail();

                    if (lbusPAEmpDetail.ibusEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusContributing)
                    {
                        if (lbusPAEmpDetail.ibusEmploymentDetail.icdoPersonEmploymentDetail.derived_member_type_value.IsNotNull())
                        {
                            if (!lclbPersonEmpDetail.Any(i => i.derived_member_type_value == lbusPAEmpDetail.ibusEmploymentDetail.icdoPersonEmploymentDetail.derived_member_type_value))
                            {
                                lclbPersonEmpDetail.Add(lbusPAEmpDetail.ibusEmploymentDetail.icdoPersonEmploymentDetail);
                            }
                        }
                    }
                }
            }
            return lclbPersonEmpDetail;
        }

        //BR-044-11
        //Get employment detail id where end date is null
        public bool CheckMemberEnrolledInDBOrDC()
        {
            //check if plan id DB or DC
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled)
            {
                return true;
            }
            return false;
        }

        //BR-044-11
        //For DC Plan, if payor is not Employer or Employer/Employee, throw an Warning.
        public bool CheckPayorIsEmployerOrEmployerEmployeeForDCPlan()
        {
            //check if plan id DB or DC
            string lstrRetirementTypeValue = busGlobalFunctions.GetRetirementTypeValue(icdoServicePurchaseHeader.plan_id);
            //check if payor is neither employer nor employee for plan DC only
            if (lstrRetirementTypeValue == busConstant.PlanRetirementTypeValueDC)
            {
                if ((icdoServicePurchaseHeader.payor_value != busConstant.Service_Purchase_Payor_Employer) &&
                    (icdoServicePurchaseHeader.payor_value != busConstant.Service_Purchase_Payor_Employer_And_Employee))
                {
                    return false;
                }
            }
            return true;
        }

        //BR-044-31
        public bool CheckMemberTerminatedDateLessThanPurchaseDate()
        {
            DateTime ldtTerminatedDate = ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.termination_date.AddMonths(1);
            ldtTerminatedDate = Convert.ToDateTime(ldtTerminatedDate.Month + "/15/" + ldtTerminatedDate.Year);
            if (icdoServicePurchaseHeader.date_of_purchase > ldtTerminatedDate)
            {
                return false;
            }
            return true;
        }

        //BR-044-47
        public bool CheckifUSSERAContributiingInJobServicePlan()
        {
            if (_ibusPerson == null)
                LoadPerson();

            if (_ibusPerson.iclbRetirementAccount == null)
                _ibusPerson.LoadRetirementAccount();

            foreach (busPersonAccount lobjPersonAccount in _ibusPerson.iclbRetirementAccount)
            {
                if (busGlobalFunctions.CheckDateOverlapping(DateTime.Now, lobjPersonAccount.icdoPersonAccount.start_date, lobjPersonAccount.icdoPersonAccount.end_date))
                {
                    lobjPersonAccount.LoadPersonAccountEmploymentDetails();

                    foreach (busPersonAccountEmploymentDetail lobjPersonAccountEmploymentDetail in lobjPersonAccount.iclbAccountEmploymentDetail)
                    {
                        if (lobjPersonAccountEmploymentDetail.ibusPersonAccount == null)
                            lobjPersonAccountEmploymentDetail.LoadPersonAccount();
                        if (lobjPersonAccountEmploymentDetail.ibusEmploymentDetail == null)
                            lobjPersonAccountEmploymentDetail.LoadPersonEmploymentDetail();
                        if ((lobjPersonAccountEmploymentDetail.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdJobService)
                            && (lobjPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.election_value == busConstant.PlanOptionStatusValueEnrolled)
                            && (lobjPersonAccountEmploymentDetail.ibusEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusContributing))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public busServicePurchaseAmortizationSchedule GetAmortizationScheduleByPaymentAllocation(int aintPaymentAllocationID)
        {
            busServicePurchaseAmortizationSchedule lbusScheduleResult = new busServicePurchaseAmortizationSchedule();
            lbusScheduleResult.icdoServicePurchaseAmortizationSchedule = new cdoServicePurchaseAmortizationSchedule();

            foreach (busServicePurchaseAmortizationSchedule lbusSchdule in _iclbServicePurchaseAmortizationSchedule)
            {
                if (lbusSchdule.icdoServicePurchaseAmortizationSchedule.iintSerPurPaymentAllocationID == aintPaymentAllocationID)
                {
                    lbusScheduleResult = lbusSchdule;
                    break;
                }
            }
            return lbusScheduleResult;
        }

        public busServicePurchasePaymentAllocation GetLastPaidPaymentAllocation()
        {
            busServicePurchasePaymentAllocation lbusLastPaymentAllocation = new busServicePurchasePaymentAllocation();
            lbusLastPaymentAllocation.icdoServicePurchasePaymentAllocation = new cdoServicePurchasePaymentAllocation();

            //Get the Last Posted Payment Date
            if (_iclbAllocatedRemittance == null)
                LoadPaymentAllocated();

            if (_iclbAllocatedRemittance.Count > 0)
            {
                //Sort the Allocated Payments by Payment Date
                iclbAllocatedRemittance = busGlobalFunctions.Sort("icdoServicePurchasePaymentAllocation.payment_date desc,icdoServicePurchasePaymentAllocation.service_purchase_payment_allocation_id desc", iclbAllocatedRemittance);

                lbusLastPaymentAllocation = iclbAllocatedRemittance[0];
            }

            return lbusLastPaymentAllocation;
        }

        public int GetMonthsToBeAddedToDeriveNextDueDate(string aobjPaymentTypeValue)
        {
            cdoCodeValue lobjCodeValue = busGlobalFunctions.GetCodeValueDetails(busConstant.Service_Purchase_Payment_Frequency_Code_Id,
                                                aobjPaymentTypeValue);

            int lintMonthsToBeAddedToDeriveNextDueDate = Convert.ToInt32(lobjCodeValue.data2);
            return lintMonthsToBeAddedToDeriveNextDueDate;
        }

        /// <summary>
        /// Method to Get the First Payment Date
        /// </summary>
        /// <returns></returns>
        private DateTime GetFirstPaymentDate()
        {
            DateTime ldtResult = DateTime.MinValue;

            DataTable ldtbList = Select<cdoServicePurchasePaymentAllocation>(
            new string[1] { "service_purchase_header_id" },
            new object[1] { icdoServicePurchaseHeader.service_purchase_header_id }, null, "PAYMENT_DATE");

            if (ldtbList.Rows.Count > 0)
            {
                ldtResult = Convert.ToDateTime(ldtbList.Rows[0]["payment_date"]);
            }

            return ldtResult;
        }

        /// <summary>
        /// to check for Service type USSERA
        /// member employmnet status should not be other than CONT or TERM
        /// </summary>
        /// <returns></returns>
        public bool IsMemberGotEmploymentStatusOtherThanCONTOrTERM()
        {
            if (icdoServicePurchaseHeader.plan_id != 0)
            {
                if (ibusPersonAccount == null)
                    LoadPersonAccount();
                if (ibusPersonAccount.icdoPersonAccount.person_account_id > 0)
                {
                    if (ibusPersonAccount.iclbAccountEmploymentDetail == null)
                        ibusPersonAccount.LoadPersonAccountEmploymentDetails();

                    bool lblnContTermEntryFound = false;
                    foreach (busPersonAccountEmploymentDetail lbusPAEmpDetail in ibusPersonAccount.iclbAccountEmploymentDetail)
                    {
                        if (lbusPAEmpDetail.ibusEmploymentDetail == null)
                            lbusPAEmpDetail.LoadPersonEmploymentDetail();

                        if (lbusPAEmpDetail.ibusEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusContributing ||
                            lbusPAEmpDetail.ibusEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusTerminated)
                        {
                            lblnContTermEntryFound = true;
                            break;
                        }
                    }

                    if (!lblnContTermEntryFound) return false;
                }
            }
            return true;
        }

        //Load Allocated Purchase Payment List for Person id
        public void LoadAllocatedPurchaseList()
        {
            iclbAllocatedPaymentPurchaseList = new Collection<busServicePurchasePaymentAllocation>();
            DataTable ldtbList = Select<cdoServicePurchasePaymentAllocation>(
            new string[1] { "service_purchase_header_id" },
            new object[1] { icdoServicePurchaseHeader.service_purchase_header_id }, null, "Payment_date");
            _iclbAllocatedPaymentPurchaseList = GetCollection<busServicePurchasePaymentAllocation>(ldtbList, "icdoServicePurchasePaymentAllocation");
        }

        //PIR 418
        //BR-044-13 
        public bool CheckIfPersonEmploymnetIsNotLOAForUSSERA()
        {
            if (ibusPerson == null)
                LoadPerson();

            if (ibusPrimaryServicePurchaseDetail == null)
                LoadServicePurchaseDetail();

            ibusPerson.LoadPersonAccountEmploymentDetailEnrolled();
            foreach (busPersonAccountEmploymentDetail lobjPersonAccountEmployment in ibusPerson.icolEnrolledPersonAccountEmploymentDetail)
            {
                lobjPersonAccountEmployment.LoadPersonEmploymentDetail();
                if (lobjPersonAccountEmployment.icdoPersonAccountEmploymentDetail.plan_id == icdoServicePurchaseHeader.plan_id)
                {
                    if (busGlobalFunctions.CheckDateOverlapping(DateTime.Now, lobjPersonAccountEmployment.ibusEmploymentDetail.icdoPersonEmploymentDetail.start_date, lobjPersonAccountEmployment.ibusEmploymentDetail.icdoPersonEmploymentDetail.end_date))
                    {
                        if ((lobjPersonAccountEmployment.ibusEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusLOA)
                                || (lobjPersonAccountEmployment.ibusEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusLOAM)
                                || (lobjPersonAccountEmployment.ibusEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusFMLA)
                                || (lobjPersonAccountEmployment.ibusEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusTerminated))
                        {
                            return false;
                        }

                    }
                }
            }
            return true;
        }

        public bool CheckIfDateUSERRAStartDateIsEntered()
        {
            if (ibusPrimaryServicePurchaseDetail.IsNull()) LoadServicePurchaseDetail();
            return ((icdoServicePurchaseHeader.grant_free_flag == busConstant.Flag_Yes) && (ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.userra_active_duty_end_date == DateTime.MinValue || ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.userra_active_duty_start_date == DateTime.MinValue)) ?
                true : false;
        }

        //BR - 044 - 29  & 28
        public bool CheckLimitExceeds415Limit()
        {
            string istrStartMonth;
            string istrEndMonth;
            DateTime idtStartDate = DateTime.MinValue;
            DateTime idtEndDate = DateTime.MinValue;
            if (icdoServicePurchaseHeader.service_purchase_type_value == busConstant.Service_Purchase_Type_Unused_Sick_Leave)
            {
                decimal ldecTotalLimitCalculated = 0.00M;

                ldecTotalLimitCalculated = ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.retirement_purchase_cost;

                //get post tax contribution and post tax service purchase contribution for calender year.
                if (ibusPersonAccount == null)
                    LoadPersonAccount();

                if (ibusPersonAccount.icdoPersonAccount.person_account_id != 0)
                {

                    DateTime ldate = DateTime.Now;

                    istrStartMonth = Convert.ToString(ldate.Year) + "01";
                    istrEndMonth = Convert.ToString(ldate.Year + 1) + "12";
                    idtStartDate = new DateTime(ldate.Year, 1, 1);
                    idtEndDate = new DateTime(ldate.Year, 12, 1);

                    DataTable ldtbLoadYTDDetail = Select("cdoPersonAccountRetirement.LoadFYTDSummary",
                              new object[3] { ibusPersonAccount.icdoPersonAccount.person_account_id, istrStartMonth, istrEndMonth });

                    if (ldtbLoadYTDDetail.Rows.Count > 0)
                    {
                        if (!String.IsNullOrEmpty(ldtbLoadYTDDetail.Rows[0]["POST_TAX_EE_AMOUNT"].ToString()))
                        { ldecTotalLimitCalculated += Convert.ToDecimal(ldtbLoadYTDDetail.Rows[0]["POST_TAX_EE_AMOUNT"]); }
                        if (!String.IsNullOrEmpty(ldtbLoadYTDDetail.Rows[0]["POST_TAX_EE_SER_PUR_CONT"].ToString()))
                        { ldecTotalLimitCalculated += Convert.ToDecimal(ldtbLoadYTDDetail.Rows[0]["POST_TAX_EE_SER_PUR_CONT"]); }
                    }
                }

                DataTable ldtbCodeValue = iobjPassInfo.isrvDBCache.GetCodeValues(357);
                if (ldtbCodeValue.Rows.Count > 0)
                {
                    foreach (DataRow dr in ldtbCodeValue.Rows)
                    {
                        string ldtUpperLimitDate = dr["data1"].ToString();
                        string ldtLowerLimitDate = dr["data2"].ToString();
                        decimal ReportedAmountLimit = Convert.ToDecimal(dr["data3"].ToString());

                        if ((!String.IsNullOrEmpty(ldtLowerLimitDate))
                            && (!String.IsNullOrEmpty(ldtUpperLimitDate)))
                        {
                            if (busGlobalFunctions.CheckDateOverlapping(idtStartDate, idtEndDate, Convert.ToDateTime(ldtUpperLimitDate), Convert.ToDateTime(ldtLowerLimitDate)))
                            {
                                if (ReportedAmountLimit != 0.00M)
                                {
                                    if (ldecTotalLimitCalculated >= ReportedAmountLimit)
                                    {
                                        return false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }

        //PIR - 456
        //BR-044-59
        //FW Upgrade PIR ID :	15758, 15759 Null Reference exception when click on save button.
        public bool CheckCheckedPreTaxIsUnchecked()
        {
            if (icdoServicePurchaseHeader.ihstOldValues.Count > 0)
            {
                if (!String.IsNullOrEmpty(Convert.ToString(icdoServicePurchaseHeader.ihstOldValues["pre_tax"])))
                {
                    if (icdoServicePurchaseHeader.ihstOldValues["pre_tax"].ToString() == busConstant.Flag_Yes)
                    {
                        if ((icdoServicePurchaseHeader.pre_tax == busConstant.Flag_No) || (IsExpectedInstallAmountOrNoOfPaymentsChanged()))
                        {
                            if (iclbAllocatedPaymentPurchaseList == null)
                                LoadAllocatedPurchaseList();
                            foreach (busServicePurchasePaymentAllocation lobjPurchasePayment in _iclbAllocatedPaymentPurchaseList)
                            {
                                if (lobjPurchasePayment.icdoServicePurchasePaymentAllocation.service_purchase_payment_class_value == busConstant.Service_Purchase_Class_Employer_Installment_PreTax)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }

        private bool IsExpectedInstallAmountOrNoOfPaymentsChanged()
        {
            if (icdoServicePurchaseHeader.ihstOldValues.Count > 0)
            {
                if (!String.IsNullOrEmpty(icdoServicePurchaseHeader.ihstOldValues["expected_installment_amount"].ToString()))
                {
                    if (icdoServicePurchaseHeader.ihstOldValues["expected_installment_amount"].ToString() !=
                        icdoServicePurchaseHeader.expected_installment_amount.ToString())
                    {
                        return true;
                    }
                }

                if (!String.IsNullOrEmpty(icdoServicePurchaseHeader.ihstOldValues["number_of_payments"].ToString()))
                {
                    if (icdoServicePurchaseHeader.ihstOldValues["number_of_payments"].ToString() != icdoServicePurchaseHeader.number_of_payments.ToString())
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        //PIR - Load UnPosted Payment Allocation
        public void LoadUnPostedPaymentAllcoation()
        {
            _iclbUnPostedPaymentAllocation = new Collection<busServicePurchasePaymentAllocation>();

            DataTable ldtbList = Select<cdoServicePurchasePaymentAllocation>(
            new string[1] { "service_purchase_header_id" },
            new object[1] { icdoServicePurchaseHeader.service_purchase_header_id }, null, "payment_date");
            _iclbUnPostedPaymentAllocation = GetCollection<busServicePurchasePaymentAllocation>(ldtbList, "icdoServicePurchasePaymentAllocation");
            Collection<busServicePurchasePaymentAllocation> lclbResult = new Collection<busServicePurchasePaymentAllocation>();
            foreach (busServicePurchasePaymentAllocation lobjPaymentAllocation in _iclbUnPostedPaymentAllocation)
            {
                if (lobjPaymentAllocation.icdoServicePurchasePaymentAllocation.posted_flag != busConstant.Flag_Yes)
                {
                    lclbResult.Add(lobjPaymentAllocation);
                }
            }
            _iclbUnPostedPaymentAllocation = lclbResult;
        }

        public void ReverseRetirementContribution(decimal adecPreTaxEE, decimal adecPostTaxEE, decimal adecPostTaxER,
            decimal adecPostTaxEERHIC, decimal adecPreTaxERRHIC, decimal adecPreTaxER, int aintPostDirection, decimal adecPSCToPost,
            decimal adecVSCToPost, DateTime adatPayment, busServicePurchasePaymentAllocation aobjSerPurPayAllocation)
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();

            if (ibusPrimaryServicePurchaseDetail == null)
                LoadServicePurchaseDetail();

            if (_ibusPerson == null)
                LoadPerson();

            adecPreTaxEE *= aintPostDirection;
            adecPreTaxER *= aintPostDirection;
            adecPostTaxEE *= aintPostDirection;
            adecPostTaxER *= aintPostDirection;
            adecPostTaxEERHIC *= aintPostDirection;
            adecPreTaxERRHIC *= aintPostDirection;
            adecPSCToPost *= aintPostDirection;
            adecVSCToPost *= aintPostDirection;

            if ((adecPreTaxEE != 0) ||
                (adecPostTaxEE != 0) ||
                (adecPostTaxER != 0) ||
                (adecPostTaxEERHIC != 0) ||
                (adecPreTaxERRHIC != 0) ||
                (adecPreTaxER != 0) ||
                (adecPSCToPost != 0) ||
                (adecVSCToPost != 0))
            {
                string lstrTransactionType = busConstant.TransactionTypeServicePurchase;

                //USSERA
                if (icdoServicePurchaseHeader.service_purchase_type_value == busConstant.Service_Purchase_Type_USERRA_Military_Service)
                    lstrTransactionType = busConstant.TransactionTypeRegularPayroll;

                //Reverse the Contribution
                _ibusPersonAccount.PostRetirementContribution(busConstant.SubSystemValueServicePurchase, icdoServicePurchaseHeader.service_purchase_header_id,
                  DateTime.Now, adatPayment, 0, 0, 0, lstrTransactionType, 0, adecPostTaxER, adecPostTaxEE, adecPreTaxER, adecPreTaxEE,
                  adecPostTaxEERHIC, adecPreTaxERRHIC, 0, 0, 0, adecVSCToPost, adecPSCToPost);

                //Reverse the Userra Purchase
                if ((icdoServicePurchaseHeader.service_purchase_type_value == busConstant.Service_Purchase_Type_USERRA_Military_Service) &&
                   (icdoServicePurchaseHeader.grant_free_flag != "Y"))
                {
                    if (ibusPrimaryServicePurchaseDetail.iclbServicePurchaseDetailUserra == null)
                        ibusPrimaryServicePurchaseDetail.LoadServicePurchaseDetailUSERRA();

                    foreach (busServicePurchaseDetailUserra lbusUSERRADtl in ibusPrimaryServicePurchaseDetail.iclbServicePurchaseDetailUserra)
                    {
                        if ((lbusUSERRADtl.icdoServicePurchaseDetailUserra.posted_flag == "Y") &&
                            (lbusUSERRADtl.icdoServicePurchaseDetailUserra.service_purchase_payment_allocation_id == aobjSerPurPayAllocation.icdoServicePurchasePaymentAllocation.service_purchase_payment_allocation_id))
                        {   // USERRA is treated as payroll so SubSystemValueEmployerReporting is being used.
                            _ibusPersonAccount.PostRetirementContribution(busConstant.SubSystemValueServicePurchase, icdoServicePurchaseHeader.service_purchase_header_id, DateTime.Now, adatPayment, lbusUSERRADtl.icdoServicePurchaseDetailUserra.missed_salary_month.Month,
                                lbusUSERRADtl.icdoServicePurchaseDetailUserra.missed_salary_month.Year, 0, lstrTransactionType,
                                aintPostDirection * lbusUSERRADtl.icdoServicePurchaseDetailUserra.missed_salary_amount, 0, 0, 0, 0,
                                0, 0, 0, 0, 0, 0, 0);
                            lbusUSERRADtl.icdoServicePurchaseDetailUserra.posted_flag = "N";
                            lbusUSERRADtl.icdoServicePurchaseDetailUserra.service_purchase_payment_allocation_id = 0;
                            lbusUSERRADtl.icdoServicePurchaseDetailUserra.Update();
                        }
                    }
                }

                icdoServicePurchaseHeader.paid_rhic_cost_amount += (adecPostTaxEERHIC + adecPreTaxERRHIC);
                icdoServicePurchaseHeader.paid_retirement_er_cost_amount += adecPostTaxER + adecPreTaxER;
                icdoServicePurchaseHeader.paid_retirement_ee_cost_amount += adecPreTaxEE + adecPostTaxEE;
                icdoServicePurchaseHeader.paid_pension_service_credit += adecPSCToPost;
                icdoServicePurchaseHeader.paid_vesting_service_credit += adecVSCToPost;

                //Judges Conversion
                if (ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.judges_conversion_flag == "Y")
                {
                    busPersonAccount lbusPersonAccountMain = null;
                    if (ibusPerson.iclbRetirementAccount == null)
                        ibusPerson.LoadRetirementAccount();

                    if (ibusPrimaryServicePurchaseDetail.iclbServicePurchaseDetailConsolidated == null)
                        ibusPrimaryServicePurchaseDetail.LoadServicePurchaseDetailConsolidated();

                    foreach (busPersonAccount lbusPersonAccount in ibusPerson.iclbRetirementAccount)
                    {
                        if (lbusPersonAccount.IsRetirementPlanOpen())
                        {
                            foreach (busServicePurchaseDetailConsolidated lbusJudgeConversionDtl in ibusPrimaryServicePurchaseDetail.iclbServicePurchaseDetailConsolidated)
                            {
                                if ((lbusJudgeConversionDtl.icdoServicePurchaseDetailConsolidated.service_purchase_start_date <= lbusPersonAccount.icdoPersonAccount.end_date_no_null) &&
                                    (lbusJudgeConversionDtl.icdoServicePurchaseDetailConsolidated.service_purchase_end_date >= lbusPersonAccount.icdoPersonAccount.start_date))
                                {
                                    lbusPersonAccountMain = lbusPersonAccount;
                                    break;
                                }
                            }
                        }
                        if (lbusPersonAccountMain != null)
                            break;
                    }
                    if (lbusPersonAccountMain == null)
                        throw new Exception("No person account for Main plan is available");
                    lbusPersonAccountMain.PostRetirementContribution(busConstant.SubSystemValueServicePurchase, icdoServicePurchaseHeader.service_purchase_header_id, adatPayment, adatPayment, 0, 0, 0, busConstant.TransactionTypeServicePurchase, 0, 0, 0, 0, 0,
                         0, 0, 0, 0, 0, -1 * adecVSCToPost, -1 * adecPSCToPost);
                }

                //check if all payment got reverted back, then change the status to APPROVED
                //else changes the status to In Payment

                aobjSerPurPayAllocation.Delete();

                //Reload the Payment Allocation
                LoadPaymentAllocated();
                bool lblnIsPostedPaymentEntryExists = false;
                foreach (busServicePurchasePaymentAllocation lobjSPPayAllocation in _iclbAllocatedRemittance)
                {
                    if (lobjSPPayAllocation.icdoServicePurchasePaymentAllocation.posted_flag == busConstant.Flag_Yes)
                    {
                        lblnIsPostedPaymentEntryExists = true;
                        break;
                    }
                }

                icdoServicePurchaseHeader.action_status_value = busConstant.Service_Purchase_Action_Status_Approved;
                if (lblnIsPostedPaymentEntryExists)
                    icdoServicePurchaseHeader.action_status_value = busConstant.Service_Purchase_Action_Status_In_Payment;
                icdoServicePurchaseHeader.Update();
            }
        }

        public decimal GetPayOffAmountByAllocation(int aintAllocationID)
        {
            decimal ldecPayOffAmount = 0;
            if (_iclbServicePurchaseAmortizationSchedule == null)
                LoadAmortizationSchedule();

            foreach (busServicePurchaseAmortizationSchedule lobjSchedule in _iclbServicePurchaseAmortizationSchedule)
            {
                if (lobjSchedule.icdoServicePurchaseAmortizationSchedule.iintSerPurPaymentAllocationID == aintAllocationID)
                {
                    ldecPayOffAmount = lobjSchedule.icdoServicePurchaseAmortizationSchedule.idecPayOffAmountActualValue;
                    break;
                }
            }
            return ldecPayOffAmount;
        }

        //BR - 044 - 60 PIR - 725
        public bool CheckOrgPlanPreTaxChecked()
        {
            bool lblnOrgPlanPreTaxChecked = false;
            busOrganization lobjOrganization = GetEmployer();
            if (lobjOrganization.icdoOrganization.org_id != 0)
            {
                //pir 1106
                if (lobjOrganization.icdoOrganization.pre_tax_purchase == busConstant.Flag_Yes)
                    lblnOrgPlanPreTaxChecked = true;
                //if (lobjOrganization.iclbOrgPlan == null)
                //    lobjOrganization.LoadOrgPlan();

                //foreach (busOrgPlan lobjOrgPlan in lobjOrganization.iclbOrgPlan)
                //{
                //    if (lobjOrgPlan.icdoOrgPlan.plan_id == icdoServicePurchaseHeader.plan_id)
                //    {
                //        if (busGlobalFunctions.CheckDateOverlapping(DateTime.Now, lobjOrgPlan.icdoOrgPlan.participation_start_date, lobjOrgPlan.icdoOrgPlan.participation_end_date))
                //        {
                //            if (lobjOrgPlan.icdoOrgPlan.pre_tax_purchase == busConstant.Flag_Yes)
                //            {
                //                lblnOrgPlanPreTaxChecked = true;
                //                break;
                //            }
                //        }
                //    }
                //}
            }
            return lblnOrgPlanPreTaxChecked;
        }

        //BR - 044 - 87 PIR - 730
        public bool CheckOrgPlanEarlyRetirementIncentiveChecked()
        {
            busOrganization lobjOrganization = GetEmployer();
            if (lobjOrganization.icdoOrganization.org_id != 0)
            {
                if (lobjOrganization.icdoOrganization.early_retirement_incentive_purchase_agreement != busConstant.Flag_Yes)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Get Employer under which Person employed
        /// and participated in same plan as selected for service purchase
        /// </summary>
        /// <returns></returns>
        public busOrganization GetEmployer()
        {
            busOrganization lobjOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };

            if (ibusPersonAccount == null)
                LoadPersonAccount();

            if (ibusPersonAccount.iclbAccountEmploymentDetail == null)
                ibusPersonAccount.LoadPersonAccountEmploymentDetails();

            foreach (busPersonAccountEmploymentDetail lobjPersonAccountEmploymentDetail in ibusPersonAccount.iclbAccountEmploymentDetail)
            {
                if (lobjPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.election_value == busConstant.PlanOptionStatusValueEnrolled)
                {
                    if (lobjPersonAccountEmploymentDetail.ibusEmploymentDetail == null)
                        lobjPersonAccountEmploymentDetail.LoadPersonEmploymentDetail();
                    //PIR-12944
                    if (busGlobalFunctions.CheckDateOverlapping(DateTime.Now.AddDays(-45), lobjPersonAccountEmploymentDetail.ibusEmploymentDetail.icdoPersonEmploymentDetail.start_date, lobjPersonAccountEmploymentDetail.ibusEmploymentDetail.icdoPersonEmploymentDetail.end_date))
                    {
                        if (lobjPersonAccountEmploymentDetail.ibusEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusContributing)
                        {
                            if (lobjPersonAccountEmploymentDetail.ibusEmploymentDetail.ibusPersonEmployment == null)
                                lobjPersonAccountEmploymentDetail.ibusEmploymentDetail.LoadPersonEmployment();

                            if (lobjPersonAccountEmploymentDetail.ibusEmploymentDetail.ibusPersonEmployment.ibusOrganization == null)
                                lobjPersonAccountEmploymentDetail.ibusEmploymentDetail.ibusPersonEmployment.LoadOrganization();

                            lobjOrganization = lobjPersonAccountEmploymentDetail.ibusEmploymentDetail.ibusPersonEmployment.ibusOrganization;
                            break;
                        }
                    }
                }
            }
            return lobjOrganization;
        }

        //BR - 044 - 84 check member enrolled in Job Service
        public bool CheckMemberEnrolledInJobService()
        {
            if (_ibusPerson == null)
                LoadPerson();

            if (ibusPerson.IsMemberEnrolledInPlan(busConstant.PlanIdJobService))
            {
                return true;
            }
            return false;
        }

        //Adjust PSC when status is updated to 'close'
        public void PostServicePurchaseAdjstClosingOfContract(ref decimal adecAdjustedPSC, ref decimal adecAdjustedVSC)
        {
            adecAdjustedPSC = 0.00M;
            adecAdjustedVSC = 0.00M;
            if (ibusPrimaryServicePurchaseDetail == null)
                LoadServicePurchaseDetail();

            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (_ibusPrimaryServicePurchaseDetail.iclbServicePurchaseDetailConsolidated == null)
                _ibusPrimaryServicePurchaseDetail.LoadServicePurchaseDetailConsolidated();
            if (ibusPersonAccount.icdoPersonAccount.person_account_id > 0)
            {
                DateTime adteffectiveDate = DateTime.Now;
                adecAdjustedPSC = Math.Ceiling(Convert.ToDecimal(icdoServicePurchaseHeader.prorated_psc)) - Convert.ToDecimal(icdoServicePurchaseHeader.prorated_psc);
                adecAdjustedVSC = Math.Ceiling(Convert.ToDecimal(icdoServicePurchaseHeader.prorated_vsc)) - Convert.ToDecimal(icdoServicePurchaseHeader.prorated_vsc);
                //PIR 1871 - Rounding off latest month's contribution to 1 for SEAS and LOA type of Consolidated Purchase.                            
                var lenumLastDetailConsolidated = _ibusPrimaryServicePurchaseDetail.iclbServicePurchaseDetailConsolidated.OrderBy(i => i.icdoServicePurchaseDetailConsolidated.service_purchase_consolidated_detail_id).LastOrDefault();
                if (lenumLastDetailConsolidated.IsNotNull() && (lenumLastDetailConsolidated.icdoServicePurchaseDetailConsolidated.service_credit_type_value == busConstant.Service_Purchase_Type_Leave_Of_Absence || lenumLastDetailConsolidated.icdoServicePurchaseDetailConsolidated.service_credit_type_value == busConstant.Service_Purchase_Type_Seasonal))
                {
                    DateTime adtlatestDate = lenumLastDetailConsolidated.icdoServicePurchaseDetailConsolidated.service_purchase_end_date;
                    DataTable ldtbLatestPayment = Select("cdoServicePurchaseHeader.GetLatestSEASOrLOAHeader",
                                                   new object[3] { ibusPersonAccount.icdoPersonAccount.person_account_id, icdoServicePurchaseHeader.service_purchase_header_id, adtlatestDate });
                    if (ldtbLatestPayment.Rows.Count > 0)
                    {

                        if (Convert.ToDecimal(ldtbLatestPayment.Rows[0]["POSTED_VSC"]) < 1 && Convert.ToDecimal(ldtbLatestPayment.Rows[0]["POSTED_PSC"]) < 1)
                        {
                            adteffectiveDate = adtlatestDate;
                            adecAdjustedVSC = 1.00M - Convert.ToDecimal(ldtbLatestPayment.Rows[0]["POSTED_VSC"]);
                            adecAdjustedPSC = 1.00M - Convert.ToDecimal(ldtbLatestPayment.Rows[0]["POSTED_PSC"]);
                        }
                    }


                }
                if (adecAdjustedVSC > 0 && adecAdjustedPSC > 0)
                {
                    ibusPersonAccount.PostRetirementContribution(busConstant.SubSystemValueServicePurchase, icdoServicePurchaseHeader.service_purchase_header_id, DateTime.Now, adteffectiveDate, 0, 0, 0, busConstant.TransactionTypePurchaseAdjustment, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, adecAdjustedVSC, adecAdjustedPSC);
                }
            }
        }

        //Adjust PSC when status is updated to 'close'
        public void PostServicePurchaseAdjstOnReversal()
        {
            if (ibusPrimaryServicePurchaseDetail == null)
                LoadServicePurchaseDetail();

            if (ibusPersonAccount == null)
                LoadPersonAccount();

            if (ibusPersonAccount.icdoPersonAccount.person_account_id > 0)
            {
                if ((icdoServicePurchaseHeader.service_purchase_adjustment_fraction_psc > 0)
                    || (icdoServicePurchaseHeader.service_purchase_adjustment_fraction_vsc > 0))
                {
                    ibusPersonAccount.PostRetirementContribution(busConstant.SubSystemValueServicePurchase, icdoServicePurchaseHeader.service_purchase_header_id, DateTime.Now, DateTime.Now, 0, 0, 0, busConstant.TransactionTypePurchaseAdjustment, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, icdoServicePurchaseHeader.service_purchase_adjustment_fraction_psc * -1, icdoServicePurchaseHeader.service_purchase_adjustment_fraction_vsc * -1);
                }
            }
        }

        /// <summary>
        /// This Method will return true if any USERRa Detail Entry Entered.
        /// This will be used for the validation of when the user enters the detail, GRANT Free can not be checked
        /// </summary>
        /// <returns></returns>
        public bool IsUSERRAMilitaryServiceEntered()
        {
            bool lblnResult = false;
            if (icdoServicePurchaseHeader.service_purchase_type_value == busConstant.Service_Purchase_Type_USERRA_Military_Service)
            {
                if ((_ibusPrimaryServicePurchaseDetail != null) && (_ibusPrimaryServicePurchaseDetail.iclbServicePurchaseDetailUserra != null))
                {
                    if (_ibusPrimaryServicePurchaseDetail.iclbServicePurchaseDetailUserra.Count > 0)
                    {
                        lblnResult = true;
                    }
                }
            }
            return lblnResult;
        }

        //to reverse the status from Closed to In Payment
        //reverse the adjusted fraction of PSC and VSC that got posted when status changed to Closed
        public void btnReverse_Click()
        {
            PostServicePurchaseAdjstOnReversal();
            icdoServicePurchaseHeader.action_status_value = busConstant.Service_Purchase_Action_Status_In_Payment;
            icdoServicePurchaseHeader.prorated_psc = icdoServicePurchaseHeader.prorated_psc - icdoServicePurchaseHeader.service_purchase_adjustment_fraction_psc;
            icdoServicePurchaseHeader.prorated_vsc = icdoServicePurchaseHeader.prorated_vsc - icdoServicePurchaseHeader.service_purchase_adjustment_fraction_vsc;
            icdoServicePurchaseHeader.delinquent_letter1_sent_flag = busConstant.Flag_No; //PIR 17283
            icdoServicePurchaseHeader.delinquent_letter2_sent_flag = busConstant.Flag_No;
            icdoServicePurchaseHeader.Update();
        }

        private bool _iblnIsFirstPaymentIsLessThanInterestAmt;
        public bool iblnIsFirstPaymentIsLessThanInterestAmt
        {
            get { return _iblnIsFirstPaymentIsLessThanInterestAmt; }
            set { _iblnIsFirstPaymentIsLessThanInterestAmt = value; }
        }

        private bool _iblnIsFirstPaymentIsMoreThanInterestAmt;
        public bool iblnIsFirstPaymentIsMoreThanInterestAmt
        {
            get { return _iblnIsFirstPaymentIsMoreThanInterestAmt; }
            set { _iblnIsFirstPaymentIsMoreThanInterestAmt = value; }
        }

        //Check first Payment made is less than or greater than the interest amount
        public void SetBoolValueAfterComparingFirstPaymentAndInterestAmt()
        {
            if (iclbServicePurchaseAmortizationSchedule == null)
                LoadAmortizationSchedule();
            if (iclbAllocatedRemittance == null)
                LoadAllocatedPurchaseList();
            if ((iclbServicePurchaseAmortizationSchedule.Count > 0)
                && (iclbAllocatedRemittance.Count > 0))
            {
                if (iclbServicePurchaseAmortizationSchedule[0].icdoServicePurchaseAmortizationSchedule.interest_in_payment_amount > iclbAllocatedRemittance[0].icdoServicePurchasePaymentAllocation.applied_amount)
                    _iblnIsFirstPaymentIsLessThanInterestAmt = true;
                if (iclbServicePurchaseAmortizationSchedule[0].icdoServicePurchaseAmortizationSchedule.interest_in_payment_amount < iclbAllocatedRemittance[0].icdoServicePurchasePaymentAllocation.applied_amount)
                    _iblnIsFirstPaymentIsMoreThanInterestAmt = true;
            }
        }

        /// <summary>
        /// This Method will return true if only free service added in consolidated purchase. This will almost equal to Grant Free.
        /// </summary>
        /// <returns></returns>
        public bool IsFreeServiceOnly()
        {
            bool lblnResult = false;
            if (icdoServicePurchaseHeader.service_purchase_type_value == busConstant.Service_Purchase_Type_Consolidated_Purchase)
            {
                if (ibusPrimaryServicePurchaseDetail == null)
                    LoadServicePurchaseDetail();

                if (ibusPrimaryServicePurchaseDetail.iclbServicePurchaseDetailConsolidated == null)
                    ibusPrimaryServicePurchaseDetail.LoadServicePurchaseDetailConsolidated();

                if (ibusPrimaryServicePurchaseDetail.iclbServicePurchaseDetailConsolidated.Count > 0)
                {
                    lblnResult = true;
                    foreach (busServicePurchaseDetailConsolidated lbusSPDConsolidated in ibusPrimaryServicePurchaseDetail.iclbServicePurchaseDetailConsolidated)
                    {
                        if (lbusSPDConsolidated.icdoServicePurchaseDetailConsolidated.service_credit_type_value != busConstant.Service_Purchase_Type_Additional_Free_Service)
                        {
                            lblnResult = false;
                            break;
                        }
                    }
                }
            }
            return lblnResult;
        }

        // UID-055 and BR-056-36
        public bool IsValidCalculationIdSelected()
        {
            if (icdoServicePurchaseHeader.be_with_service_purchase_calc_id != 0)
            {
                if (IsBenefitEstimateExists(icdoServicePurchaseHeader.be_with_service_purchase_calc_id, busConstant.ApplicationBenefitTypeRetirement))
                    return true;
                return false;
            }
            if (icdoServicePurchaseHeader.be_without_service_purchase_calc_id != 0)
            {
                if (IsBenefitEstimateExists(icdoServicePurchaseHeader.be_without_service_purchase_calc_id, busConstant.ApplicationBenefitTypeRetirement))
                    return true;
                return false;
            }
            if (icdoServicePurchaseHeader.be_death_with_service_purchase_calc_id != 0)
            {
                if (IsBenefitEstimateExists(icdoServicePurchaseHeader.be_death_with_service_purchase_calc_id, busConstant.ApplicationBenefitTypePreRetirementDeath))
                    return true;
                return false;
            }
            if (icdoServicePurchaseHeader.be_death_without_service_purchase_calc_id != 0)
            {
                if (IsBenefitEstimateExists(icdoServicePurchaseHeader.be_death_without_service_purchase_calc_id, busConstant.ApplicationBenefitTypePreRetirementDeath))
                    return true;
                return false;
            }
            return true;
        }

        // Method to Check whether the Benefit Calculation Exists
        public bool IsBenefitEstimateExists(int aintBenefitCalculationID, string astrBenefitAccountType)
        {
            DataTable ldtbResult = Select<cdoBenefitCalculation>(
                                    new string[3] { "BENEFIT_CALCULATION_ID", "BENEFIT_ACCOUNT_TYPE_VALUE", "CALCULATION_TYPE_VALUE" },
                                    new object[3] { aintBenefitCalculationID, astrBenefitAccountType, busConstant.CalculationTypeEstimate }, null, null);
            if (ldtbResult.Rows.Count > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Method to load remittance allocation based on Remittance ID
        /// </summary>
        /// <param name="aintRemittanceID">Remittance id</param>
        public void LoadAllocatedPurchaseListByRemittanceID(int aintRemittanceID)
        {
            iclbAllocatedPaymentPurchaseList = new Collection<busServicePurchasePaymentAllocation>();
            DataTable ldtbList = Select<cdoServicePurchasePaymentAllocation>(
            new string[1] { "remittance_id" },
            new object[1] { aintRemittanceID }, null, null);
            _iclbAllocatedPaymentPurchaseList = GetCollection<busServicePurchasePaymentAllocation>(ldtbList, "icdoServicePurchasePaymentAllocation");
        }

        #region Properties for Correspondence

        //UCS - 044 Correspondence
        private busServicePurchaseAmortizationSchedule _ibusLastPaidSchedule;
        public busServicePurchaseAmortizationSchedule ibusLastPaidSchedule
        {
            get { return _ibusLastPaidSchedule; }
            set { _ibusLastPaidSchedule = value; }
        }

        //UCS - 044 Correspondence
        private busServicePurchaseAmortizationSchedule _ibusCurrentSchedule;
        public busServicePurchaseAmortizationSchedule ibusCurrentSchedule
        {
            get { return _ibusCurrentSchedule; }
            set { _ibusCurrentSchedule = value; }
        }

        //UCS - 044 Correspondence - Payment due date for current record
        private DateTime _idtLastPaymentDueDate;
        public DateTime idtLastPaymentDueDate
        {
            get { return _idtLastPaymentDueDate; }
            set { _idtLastPaymentDueDate = value; }
        }

        //UCS - 044 Correspondence
        public decimal idecRHICPortionMinusPaymentAmount
        {
            get
            {
                return Math.Round(ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.rhic_purchase_cost -
                    ibusLastPaidSchedule.icdoServicePurchaseAmortizationSchedule.payment_amount, 2, MidpointRounding.AwayFromZero);
            }
        }

        //UCS - 044 Correspondence
        private busPersonEmploymentDetail _ibusPersonEmploymentDetail;
        public busPersonEmploymentDetail ibusPersonEmploymentDetail
        {
            get { return _ibusPersonEmploymentDetail; }
            set { _ibusPersonEmploymentDetail = value; }
        }

        //Cor PUR-8004
        public string istr2MonthsAfterEmplDtlStartDate
        {
            get
            {
                if (_ibusPersonEmploymentDetail == null)
                    LoadLatestEmploymentDetail();
                string lstr2MonthsAfterEmplDtlStartDate = String.Empty;
                lstr2MonthsAfterEmplDtlStartDate = _ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date.AddMonths(2).ToString("MMMM");
                return lstr2MonthsAfterEmplDtlStartDate;
            }
        }

        //Cor PUR-8004
        public string istr1MonthAfterEmplDtlStartDate
        {
            get
            {
                if (_ibusPersonEmploymentDetail == null)
                    LoadLatestEmploymentDetail();
                string lstr1MonthsAfterEmplDtlStartDate = String.Empty;
                lstr1MonthsAfterEmplDtlStartDate = _ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date.AddMonths(1).ToString("MMMM");
                return lstr1MonthsAfterEmplDtlStartDate;
            }
        }

        //UCS - 044 - Property to load all the system constants, code id 52
        private Collection<cdoCodeValue> _iclbVSCThresholdMonths;
        public Collection<cdoCodeValue> iclbVSCThresholdMonths
        {
            get { return _iclbVSCThresholdMonths; }
            set { _iclbVSCThresholdMonths = value; }
        }

        //UCS - 044 - Property to return Number of threshold months based on plan
        public string istrNumberofMonths
        {
            get
            {
                if (_iclbVSCThresholdMonths == null)
                    LoadVSCThresholdMonths();
                foreach (cdoCodeValue lobjCodeValue in iclbVSCThresholdMonths)
                {
                    if (icdoServicePurchaseHeader.plan_id == busConstant.PlanIdHP && lobjCodeValue.code_value == busConstant.VSCThresholdHP)
                        return lobjCodeValue.data1;
                    else if (icdoServicePurchaseHeader.plan_id == busConstant.PlanIdJudges && lobjCodeValue.code_value == busConstant.VSCThresholdJudges)
                        return lobjCodeValue.data1;
                    else if ((icdoServicePurchaseHeader.plan_id == busConstant.PlanIdMain ||
                       icdoServicePurchaseHeader.plan_id == busConstant.PlanIdMain2020 || // PIR 20232
                        icdoServicePurchaseHeader.plan_id == busConstant.PlanIdLE ||
                       icdoServicePurchaseHeader.plan_id == busConstant.PlanIdNG ||
                       icdoServicePurchaseHeader.plan_id == busConstant.PlanIdStatePublicSafety || //PIR 25729
                        icdoServicePurchaseHeader.plan_id == busConstant.PlanIdBCILawEnf) && //pir 7943 
                        lobjCodeValue.code_value == busConstant.VSCThresholdMLN)
                        return lobjCodeValue.data1;
                    else if ((icdoServicePurchaseHeader.plan_id == busConstant.PlanIdDC ||
                       icdoServicePurchaseHeader.plan_id == busConstant.PlanIdDC2025 || // PIR 25920
                       icdoServicePurchaseHeader.plan_id == busConstant.PlanIdDC2020) && lobjCodeValue.code_value == busConstant.VSCThresholdDC) //PIR 20232
                        return lobjCodeValue.data1;
                }
                return string.Empty;
            }
        }

        //UCS - 044 - Property to contain Retirement rates for the plan
        private cdoPlanRetirementRate _icdoPlanRetirementRate;
        public cdoPlanRetirementRate icdoPlanRetirementRate
        {
            get { return _icdoPlanRetirementRate; }
            set { _icdoPlanRetirementRate = value; }
        }

        //UCS - 044 - Property containing Employee contribution percentage
        public decimal idecEEPercentage
        {
            get
            {
                if (icdoPlanRetirementRate == null)
                    LoadPlanRetirementRate();
                if (ibusPerson.IsNull()) LoadPerson(); //PIR 14656
                return (!string.IsNullOrEmpty(ibusPerson.icdoPerson.db_addl_contrib) && ibusPerson.icdoPerson.db_addl_contrib.ToUpper() == busConstant.Flag_Yes) ?
                    icdoPlanRetirementRate.ee_post_tax + icdoPlanRetirementRate.ee_pre_tax + icdoPlanRetirementRate.ee_emp_pickup +
                    icdoPlanRetirementRate.addl_ee_post_tax + icdoPlanRetirementRate.addl_ee_pre_tax + icdoPlanRetirementRate.addl_ee_emp_pickup :
                    icdoPlanRetirementRate.ee_post_tax + icdoPlanRetirementRate.ee_pre_tax + icdoPlanRetirementRate.ee_emp_pickup;
            }
        }

        //UCS - 044 - Property containing Employer contribution percentage
        public decimal idecERPercentage
        {
            get
            {
                if (icdoPlanRetirementRate == null)
                    LoadPlanRetirementRate();
                return icdoPlanRetirementRate.er_post_tax;
            }
        }

        //UCS - 044 - Property to contain first payment which is less than interest amt
        public decimal idecFirstPaymentLessThanInterest
        {
            get
            {
                busServicePurchaseAmortizationSchedule lobjFirstPayment = new busServicePurchaseAmortizationSchedule();
                lobjFirstPayment = GetFirstPaymentRowLessThanInterest();
                if (lobjFirstPayment != null)
                    return lobjFirstPayment.icdoServicePurchaseAmortizationSchedule.payment_amount;
                else
                    return 0.0M;
            }
        }

        //UCS - 044 - Property to contain first paymentdate where payment amt is less than interest amt
        public DateTime idtFirstPaymentLessThanInterest
        {
            get
            {
                busServicePurchaseAmortizationSchedule lobjFirstPayment = new busServicePurchaseAmortizationSchedule();
                lobjFirstPayment = GetFirstPaymentRowLessThanInterest();
                if (lobjFirstPayment != null)
                    return lobjFirstPayment.icdoServicePurchaseAmortizationSchedule.payment_date;
                else
                    return DateTime.MinValue;
            }
        }

        //UCS - 044 - Property to get check amount
        public decimal idecCheckAmount
        {
            get
            {
                decimal ldecCheckAmount = 0.0M;
                if (iclbAvailableRemittances == null)
                    LoadAvailableRemittances();
                foreach (busServicePurchasePaymentAllocation lobjPaymentAllocation in iclbAvailableRemittances)
                {
                    ldecCheckAmount = lobjPaymentAllocation.icdoServicePurchasePaymentAllocation.remittance_amount;
                    break;
                }
                return ldecCheckAmount;
            }
        }

        //UCS - 044 - Property which returns difference of pay off amount and purchase cost - rhic portion
        public decimal idecDiffPayoffAmtRHICPortion
        {
            get
            {
                return Math.Round(idecPayOffAmount - ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.rhic_purchase_cost, 2, MidpointRounding.AwayFromZero);
            }
        }

        // UCS - 044 - This derived property is used to display the  TotalTimeToPurchase in custom formated
        public string TotalTimeToPurchase_formatted
        {
            get
            {
                if (TotalTimeToPurchase < 0)
                    return String.Format("{0} Years {1} Months", Math.Ceiling(TotalTimeToPurchase / 12).ToString(),
                                     Math.Round((TotalTimeToPurchase % 12), 4, MidpointRounding.AwayFromZero).ToString());

                return String.Format("{0} Years {1} Months", Math.Floor(TotalTimeToPurchase / 12).ToString(),
                                     Math.Round((TotalTimeToPurchase % 12), 4, MidpointRounding.AwayFromZero).ToString());
            }
        }

        //UCS - 044 - Property which returns difference of total purchase cost and down payment
        public decimal idecDiffTotalPurchaseCostDownPayment
        {
            get
            {
                return Math.Round(TotalPurchaseCost - icdoServicePurchaseHeader.down_payment, 2, MidpointRounding.AwayFromZero);
            }
        }

        //ucs-044-Payment amt for Rollover pay class
        public decimal idecPaymentAmtforRollover
        {
            get
            {
                if (iclbAllocatedRemittance == null)
                    LoadPaymentAllocated();
                foreach (busServicePurchasePaymentAllocation lobjPayment in iclbAllocatedRemittance)
                {
                    if (lobjPayment.icdoServicePurchasePaymentAllocation.payment_date != DateTime.MinValue &&
                        lobjPayment.icdoServicePurchasePaymentAllocation.service_purchase_payment_class_value == busConstant.Service_Purchase_Class_Employer_Rollover)
                        return lobjPayment.icdoServicePurchasePaymentAllocation.applied_amount;
                }
                return 0.0M;
            }
        }

        //Property to check whether the current schedule is the last one
        public int iintCheckFinalPaymentDue
        {
            get
            {
                if (ibusCurrentSchedule == null)
                    LoadCurrentSchedule();
                if (ibusCurrentSchedule.icdoServicePurchaseAmortizationSchedule.idecPayOffAmount > 0.00M &&
                    ibusCurrentSchedule.icdoServicePurchaseAmortizationSchedule.principle_balance <= ibusCurrentSchedule.icdoServicePurchaseAmortizationSchedule.expected_payment_amount)
                    return 1;
                else
                    return 0;
            }
        }

        //Property to check whether payment amt > interet due
        public int iintComparePaymentAmtInterestDue
        {
            get
            {
                if (ibusLastPaidSchedule == null)
                    LoadLastPaidSchedule();

                if (ibusLastPaidSchedule.icdoServicePurchaseAmortizationSchedule.payment_amount >
                    ibusLastPaidSchedule.icdoServicePurchaseAmortizationSchedule.interest_in_payment_amount)
                    return 1;
                else if (ibusLastPaidSchedule.icdoServicePurchaseAmortizationSchedule.payment_amount <
                    ibusLastPaidSchedule.icdoServicePurchaseAmortizationSchedule.interest_in_payment_amount)
                    return 0;

                return -1;
            }
        }

        //Property to compare Payment Amount & Purchase Cost RHIC portion
        public int iintComparePaymentAmtPurchaseCostRHICPortion
        {
            get
            {
                if (ibusLastPaidSchedule == null)
                    LoadLastPaidSchedule();

                if (ibusLastPaidSchedule.icdoServicePurchaseAmortizationSchedule.payment_amount <
                    ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.rhic_purchase_cost)
                    return 1;

                return -1;
            }
        }

        //Property to check Payment Class is LumpSum
        public int iintCheckLumpSum
        {
            get
            {
                if (ibusLastPaidSchedule == null)
                    LoadLastPaidSchedule();

                if (ibusLastPaidSchedule.icdoServicePurchaseAmortizationSchedule.istrPaymentClass != null &&
                    (ibusLastPaidSchedule.icdoServicePurchaseAmortizationSchedule.istrPaymentClass.Contains("Lump Sum") ||
                    ibusLastPaidSchedule.icdoServicePurchaseAmortizationSchedule.istrPaymentClass.Contains("Lumpsum")))
                    return 1;
                return 0;
            }
        }

        //Property to check Payment Election
        public int iintCheckPaymentElection
        {
            get
            {
                if (icdoServicePurchaseHeader.payment_frequency_value == busConstant.ServicePurchasePaymentFrequencyValueOneTimeLumpSumAmt)
                    return 1;
                else
                    return 0;
            }
        }

        //Property to compare pay off amount and purchase cost - rhic portion
        public int iintComparePayoffAmountRHICPortion
        {
            get
            {
                if (idecPayOffAmount > ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.rhic_purchase_cost)
                    return 1;
                else
                    return 0;
            }
        }

        //property to return rounded prorated vsc
        public decimal idecProratedVSC
        {
            get
            {
                if (icdoServicePurchaseHeader != null)
                    return Math.Round(icdoServicePurchaseHeader.prorated_vsc, MidpointRounding.AwayFromZero);
                return 0;
            }
        }

        #endregion

        #region Methods for Correspondence
        // PIR - 9262 
        public override busBase GetCorOrganization()
        {
            if (ibusPerson.IsNull()) LoadPerson();
            if (ibusPerson.icolPersonEmployment.IsNull()) ibusPerson.LoadPersonEmployment();
            return ibusPerson.icolPersonEmployment[0].ibusOrganization; 
        }
		
        public override busBase GetCorPerson()
        {
            if (_ibusPerson == null)
                LoadPerson();
            return _ibusPerson;
        }

        public bool iblnShowTableUSERRA { get; set; }

        //Method to load properties related to Correspondence
        public override void LoadCorresProperties(string astrTemplateName)
        {
            LoadLastPaidSchedule();
            LoadCurrentSchedule();
            LoadLastPaymentDueDate();
            LoadLatestEmploymentDetail();
            LoadVSCThresholdMonths();
            LoadPlanRetirementRate();
            LoadOrganizationForDate();
            if (icdoServicePurchaseHeader.service_purchase_type_value != busConstant.Service_Purchase_Type_USERRA_Military_Service &&
                ibusPrimaryServicePurchaseDetail.iclbServicePurchaseDetailUserra == null)
                ibusPrimaryServicePurchaseDetail.LoadServicePurchaseDetailUSERRA();
            if (ibusPrimaryServicePurchaseDetail.iclbServicePurchaseDetailUserra.Count > 0)
                iblnShowTableUSERRA = true;
        }

        //Method to return the Last Paid Schedule
        public void LoadLastPaidSchedule()
        {
            if (_ibusLastPaidSchedule == null)
            {
                busServicePurchasePaymentAllocation lbusLastPaymentAllocation = GetLastPaidPaymentAllocation();

                _ibusLastPaidSchedule = GetAmortizationScheduleByPaymentAllocation(
                                                        lbusLastPaymentAllocation.icdoServicePurchasePaymentAllocation.service_purchase_payment_allocation_id);
            }
        }

        //method to load latest employment detail
        public void LoadLatestEmploymentDetail()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            _ibusPersonEmploymentDetail = ibusPersonAccount.GetLatestEmploymentDetail();

        }

        //Method to return the Schedule which need to be paid
        public void LoadCurrentSchedule()
        {
            if (ibusCurrentSchedule.IsNull())
                ibusCurrentSchedule = new busServicePurchaseAmortizationSchedule { icdoServicePurchaseAmortizationSchedule = new cdoServicePurchaseAmortizationSchedule() };
            if (iclbServicePurchaseAmortizationSchedule == null || iclbServicePurchaseAmortizationSchedule.Count == 0)//Code corrected
                LoadAmortizationSchedule();
            foreach (busServicePurchaseAmortizationSchedule lobjCurrentSchedule in iclbServicePurchaseAmortizationSchedule)
            {
                if (lobjCurrentSchedule.icdoServicePurchaseAmortizationSchedule.payment_date == DateTime.MinValue)
                {
                    ibusCurrentSchedule = lobjCurrentSchedule;
                    break;
                }
            }
        }

        public void LoadLastPaymentDueDate()
        {
            /*
            if (ibusCurrentSchedule == null)
            {
                ibusCurrentSchedule = new busServicePurchaseAmortizationSchedule();
                ibusCurrentSchedule.icdoServicePurchaseAmortizationSchedule = new cdoServicePurchaseAmortizationSchedule();
                LoadCurrentSchedule();
            }
            _idtLastPaymentDueDate = ibusCurrentSchedule.icdoServicePurchaseAmortizationSchedule.payment_due_date.AddMonths(
                                            icdoServicePurchaseHeader.number_of_payments - ibusCurrentSchedule.icdoServicePurchaseAmortizationSchedule.payment_number);
            */
            if (iclbServicePurchaseAmortizationSchedule == null)
                LoadAmortizationSchedule();
            if (iclbServicePurchaseAmortizationSchedule.Count > 0)
                idtLastPaymentDueDate = iclbServicePurchaseAmortizationSchedule.Last().icdoServicePurchaseAmortizationSchedule.payment_due_date;
        }

        //Method to load collection of system constants, code id 52
        public void LoadVSCThresholdMonths()
        {
            if (iclbVSCThresholdMonths == null)
            {
                DataTable ldtbList = new DataTable();
                ldtbList = iobjPassInfo.isrvDBCache.GetCodeValues(busConstant.SystemConstantCodeID);
                iclbVSCThresholdMonths = cdoCodeValue.GetCollection<cdoCodeValue>(ldtbList);
            }
        }

        //Method to load Plan retirement rate
        public void LoadPlanRetirementRate()
        {
            if (icdoPlanRetirementRate == null && icdoServicePurchaseHeader.member_type_value != null)
            {
                icdoPlanRetirementRate = busGlobalFunctions.GetRetirementRateForPlanDateCombination(icdoServicePurchaseHeader.plan_id,
                                                    icdoServicePurchaseHeader.date_of_purchase,
                                                    icdoServicePurchaseHeader.member_type_value);
            }
            else
                icdoPlanRetirementRate = new cdoPlanRetirementRate();
        }

        //Method to load orgainization details for a date
        public void LoadOrganizationForDate()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            ibusPersonAccount.icdoPersonAccount.person_employment_dtl_id = ibusPersonAccount.GetEmploymentDetailID(ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.userra_active_duty_start_date);
            ibusPersonAccount.LoadPersonEmploymentDetail();
            ibusPersonAccount.ibusPersonEmploymentDetail.LoadPersonEmployment();
            ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
        }

        //Method to get the first payment row where payment amt < interest amt
        public busServicePurchaseAmortizationSchedule GetFirstPaymentRowLessThanInterest()
        {
            if (iclbServicePurchaseAmortizationSchedule == null)
                LoadAmortizationSchedule();
            foreach (busServicePurchaseAmortizationSchedule lobjPayment in iclbServicePurchaseAmortizationSchedule)
            {
                if (lobjPayment.icdoServicePurchaseAmortizationSchedule.payment_date != DateTime.MinValue &&
                    lobjPayment.icdoServicePurchaseAmortizationSchedule.principle_in_payment_amount <= 0)
                    return lobjPayment;
            }
            return null;
        }

        #endregion


        //PIR 1962
        //set visibility 
        //if the purchase type is Consolidate and the consolidate type - PERS Previous Employment then
        //set Due date as invisible
        public bool VisibleRuleForDateOfPurchase()
        {
            if (icdoServicePurchaseHeader.action_status_value == busConstant.Service_Purchase_Action_Status_In_Payment)
            {
                return false;
            }

            if (icdoServicePurchaseHeader.service_purchase_type_value == busConstant.Service_Purchase_Type_Consolidated_Purchase)
            {
                if (ibusPrimaryServicePurchaseDetail.iclbServicePurchaseDetailConsolidated.IsNull())
                    ibusPrimaryServicePurchaseDetail.LoadServicePurchaseDetailConsolidated();

                var lintPreviousPERSCount = ibusPrimaryServicePurchaseDetail.iclbServicePurchaseDetailConsolidated.Where(lobjConsDtl => lobjConsDtl.icdoServicePurchaseDetailConsolidated.service_credit_type_value == busConstant.Service_Purchase_Type_Previous_Pers_Employment).Count();
                if (lintPreviousPERSCount > 0)
                    return false;
            }
            return true;
        }

        public ArrayList btnRefreshFAS_Click()
        {
            ArrayList larrList = new ArrayList();
            // UAT PIR ID 726
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPersonAccountRetirement == null)
                ibusPersonAccount.LoadPersonAccountRetirement();
            ibusPersonAccount.ibusPersonAccountRetirement.LoadNewRetirementBenefitCalculation();
            //Prod pir 6858
            if (icdoServicePurchaseHeader.is_created_from_portal == busConstant.Flag_Yes &&
                icdoServicePurchaseHeader.service_purchase_type_value == busConstant.Service_Purchase_Type_Unused_Sick_Leave)
            {
                ibusPersonAccount.ibusPersonAccountRetirement.AssignRetirementBenefitCalculationFields(busConstant.CalculationTypeEstimate, retirement_date_from_portal, termination_date_from_portal);
            }
            else
            {     //sp pir-1054 FAS for plan DC
                ibusPersonAccount.ibusPersonAccountRetirement.AssignRetirementBenefitCalculationFields(busConstant.CalculationTypeFinal);
                if (icdoServicePurchaseHeader.plan_id == busConstant.PlanIdDC ||
                    icdoServicePurchaseHeader.plan_id == busConstant.PlanIdDC2020 || //PIR 20232
                    icdoServicePurchaseHeader.plan_id == busConstant.PlanIdDC2025) //PIR 25920
                {
                    ibusPersonAccount.ibusPersonAccountRetirement.ibusRetirementBenefitCalculation.icdoBenefitCalculation.iblnUseMainPlanFasFactors = true;
                }
            }
            ibusPersonAccount.ibusPersonAccountRetirement.CalculateFAS();
            icdoServicePurchaseHeader.final_average_salary = ibusPersonAccount.ibusPersonAccountRetirement.ibusRetirementBenefitCalculation.icdoBenefitCalculation.computed_final_average_salary;
            larrList.Add(this);
            return larrList;
        }

        // Visible Rule for Refresh FAS Button
        public bool IsSuspendedAccountExists()
        {
            if (ibusPerson.IsNull())
                LoadPerson();
            if (ibusPerson.icolPersonAccount.IsNull())
                ibusPerson.LoadPersonAccount();
            bool ls = ibusPerson.icolPersonAccount.Where(lobj => lobj.icdoPersonAccount.plan_id == icdoServicePurchaseHeader.plan_id &&
                                        lobj.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetimentSuspended).Any();
            return ls;
        }

        public bool IsCurrentUserSameAsCreatedBy()
        {
            if (iobjPassInfo.istrUserID == icdoServicePurchaseHeader.created_by)
                return true;

            return false;
        }

        // Systest PIR ID 2200 - Just for the sake of Correspondence PIR.
        public string prorated_psc_formatted_lc
        {
            get
            {
                if (ibusPersonAccount.IsNull())
                    LoadPersonAccount();
                if (ibusPersonAccount.icdoPersonAccount.Total_PSC < 0)
                    return String.Format("{0} Years {1} Months", Math.Ceiling(ibusPersonAccount.icdoPersonAccount.Total_PSC / 12).ToString(),
                                     Math.Ceiling(ibusPersonAccount.icdoPersonAccount.Total_PSC % 12).ToString().ToLower());
                return String.Format("{0} Years {1} Months", Math.Floor(ibusPersonAccount.icdoPersonAccount.Total_PSC / 12).ToString(),
                                     Math.Floor(ibusPersonAccount.icdoPersonAccount.Total_PSC % 12).ToString().ToLower());
            }
        }

        public string istrPaymentDueDateLongFormat
        {
            get
            {
                if (ibusCurrentSchedule.IsNotNull())
                {
                    if (ibusCurrentSchedule.icdoServicePurchaseAmortizationSchedule.payment_due_date != DateTime.MinValue)
                        return ibusCurrentSchedule.icdoServicePurchaseAmortizationSchedule.payment_due_date.ToString(busConstant.DateFormatLongDate);
                    else return string.Empty;
                }
                else return string.Empty;
            }
        }

        public string istrLastPaymentDueDateLongFormat
        {
            get
            {
                return idtLastPaymentDueDate == DateTime.MinValue? string.Empty : idtLastPaymentDueDate.ToString(busConstant.DateFormatLongDate);
            }
        }

        // PROD PIR ID 4007
        public decimal idecContributionPercentage
        {
            get
            {
                if (ibusPersonEmploymentDetail.IsNull()) LoadLatestEmploymentDetail();
                return ibusPersonEmploymentDetail.LoadMonthlyEEPercentage(icdoServicePurchaseHeader.member_type_value, icdoServicePurchaseHeader.plan_id, false, true);                
            }
        }

        public bool iblnLoadMemberTypeByPlan { get; set; } //PIR 18363 - Dual Member changes

        public bool IsNewCalcPanelVisible()
        {
            return (icdoServicePurchaseHeader.date_of_purchase.Date < new DateTime(2018, 01, 01).Date) ? false : true;
        }
        //PIR 20233 - added a property For check plan is main2020 or dc2020
        public int iintIsMain2020orDC2020
        {
            get
            {
                return (icdoServicePurchaseHeader.plan_id == busConstant.PlanIdMain2020 || icdoServicePurchaseHeader.plan_id == busConstant.PlanIdDC2020) ? 1 : 0;
            }
        }

        public bool iblnFromMssCheck180 { get; set; } = false; 
		//PIR 19046 New soft error(suppressible) 
        public bool IsUUSLGreaterThanZeroWithAnyEmployment()
        {
            if (ibusPerson.IsNull()) LoadPerson();
            if (ibusPerson.icolPersonEmployment.IsNull()) ibusPerson.LoadPersonEmployment(false);
            return icdoServicePurchaseHeader.service_purchase_header_id == 0 &&
                     icdoServicePurchaseHeader.is_created_from_portal != busConstant.Flag_Yes &&
                     icdoServicePurchaseHeader.service_purchase_type_value == busConstant.Service_Purchase_Type_Unused_Sick_Leave &&
                     icdoServicePurchaseHeader.suppress_warnings_flag != busConstant.Flag_Yes &&
                     ibusPerson.icolPersonEmployment.Any(emp => emp.icdoPersonEmployment.unused_sick_leave > 0);
        }
        public DateTime first_payment_date
        {
            get
            {
                return GetFirstPaymentDate();
            }
        }
        //PIR 20063
        public decimal idecTotalERCost
        {
            get
            {
                if (icdoServicePurchaseHeader.service_purchase_type_value == busConstant.Service_Purchase_Type_USERRA_Military_Service)
                {
                    if (_ibusPrimaryServicePurchaseDetail.IsNull()) LoadServicePurchaseDetail();
                    if (_ibusPrimaryServicePurchaseDetail.iclbServicePurchaseDetailUserra.IsNull()) _ibusPrimaryServicePurchaseDetail.LoadServicePurchaseDetailUSERRA();
                    return _ibusPrimaryServicePurchaseDetail.iclbServicePurchaseDetailUserra.Sum(i => i.icdoServicePurchaseDetailUserra.employee_pickup + i.icdoServicePurchaseDetailUserra.employer_contribution + i.icdoServicePurchaseDetailUserra.rhic_contribution);
                }
                return 0.00M;
            }                
        }
        public string istrFormattedExpirationDate => icdoServicePurchaseHeader.expiration_date.ToString(busConstant.DateFormatLongDate);
        public bool IsPersonReached401aFlag()
        {
            if (ibusPerson.IsNull())
                LoadPerson();
            return (ibusPerson?.icdoPerson?.limit_401a == busConstant.Flag_Yes);                
        }

        public bool IsPersonEligibleForPurchaseAsTemporary()
        {
            DataTable ldtPersonEmployementDetail = busBase.Select("entPersonEmploymentDetail.LoadAllEmploymentDetailsForPerson", new object[1] { icdoServicePurchaseHeader.person_id });
            Collection<busPersonEmploymentDetail> lclbPersonEmployementDetails = GetCollection<busPersonEmploymentDetail>(ldtPersonEmployementDetail, "icdoPersonEmploymentDetail");
            if (((string.IsNullOrEmpty(icdoServicePurchaseHeader.suppress_warnings_flag)) ||
                (icdoServicePurchaseHeader.suppress_warnings_flag.ToUpper() == busConstant.Flag_No)) &&
                lclbPersonEmployementDetails.Count > 0 &&
                !lclbPersonEmployementDetails.All(emp => emp.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary))
            {
                System.Collections.Generic.IEnumerable<busPersonEmploymentDetail> lInuPersonEmployementDetails = lclbPersonEmployementDetails.Where(emp => emp.icdoPersonEmploymentDetail.end_date == DateTime.MinValue || emp.icdoPersonEmploymentDetail.end_date.Date >= DateTime.Today.Date);
                if (lInuPersonEmployementDetails.Count() > 0 && lInuPersonEmployementDetails.All(emp => emp.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary))
                    return false;
            }
            return true;
        }
        public bool IsUserCheckingOwnOrSpouseServicePurchase()
        {
            if (icdoServicePurchaseHeader.person_id > 0)
            {
                return busBase.Select("entServicePurchaseHeader.IsLoggedInUserWorkingOnOwnOrSpousePurchase", new object[2] {
                   iobjPassInfo.istrUserID, icdoServicePurchaseHeader.person_id }).Rows.Count > 0;
            }
            return false;
        }
        //PIR 18212 : Show User FAS if available or only FAS on corr
        public decimal idecTotalFAS {
            get
            {
                if (icdoServicePurchaseHeader.overridden_final_average_salary == 0)
                    return icdoServicePurchaseHeader.final_average_salary;
                else
                    return icdoServicePurchaseHeader.overridden_final_average_salary;
            }
        }

        //PIR 23183
        public bool IsFASCalculatedBy3Consecutive12MonthsPeriod()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();

            if (ibusPersonAccount.ibusPersonAccountRetirement == null)
                ibusPersonAccount.LoadPersonAccountRetirement();

            if (ibusPersonAccount.ibusPersonAccountRetirement.ibusRetirementBenefitCalculation == null)
            {
                ibusPersonAccount.ibusPersonAccountRetirement.LoadNewRetirementBenefitCalculation();
                if (icdoServicePurchaseHeader.is_created_from_portal == busConstant.Flag_Yes &&
                icdoServicePurchaseHeader.service_purchase_type_value == busConstant.Service_Purchase_Type_Unused_Sick_Leave)
                {
                    ibusPersonAccount.ibusPersonAccountRetirement.AssignRetirementBenefitCalculationFields(busConstant.CalculationTypeEstimate, retirement_date_from_portal, termination_date_from_portal);
                }
                else
                {
                    ibusPersonAccount.ibusPersonAccountRetirement.AssignRetirementBenefitCalculationFields(busConstant.CalculationTypeFinal);
                }
            }

            return ibusPersonAccount.ibusPersonAccountRetirement.ibusRetirementBenefitCalculation.IsFASCalculatedBy3Consecutive12MonthsPeriod();
        }
    }
}
