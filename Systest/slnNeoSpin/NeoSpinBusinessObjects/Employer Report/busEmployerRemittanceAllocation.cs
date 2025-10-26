#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;
using System.Linq;
using System.Linq.Expressions;

#endregion

namespace NeoSpin.BusinessObjects
{
    public partial class busEmployerRemittanceAllocation : busExtendBase
    {
        private busEmployerPayrollHeader _ibusEmployerPayrollHeader;
        public busEmployerPayrollHeader ibusEmployerPayrollHeader
        {
            get
            {
                return _ibusEmployerPayrollHeader;
            }

            set
            {
                _ibusEmployerPayrollHeader = value;
            }
        }

        private Collection<busEmployerRemittanceAllocation> _iclbEmployerRemittanceAllocation;
        public Collection<busEmployerRemittanceAllocation> iclbEmployerRemittanceAllocation
        {
            get
            {
                return _iclbEmployerRemittanceAllocation;
            }

            set
            {
                _iclbEmployerRemittanceAllocation = value;
            }
        }

        /// <summary>
        /// Remittance Avaialable Amount
        /// </summary>
        private decimal _idecRemittanceAvailableAmount;
        public decimal idecRemittanceAvailableAmount
        {
            get
            {
                return _idecRemittanceAvailableAmount;
            }
            set
            {
                _idecRemittanceAvailableAmount = value;
            }
        }

        public void LoadAvailableAmount()
        {
            _idecRemittanceAvailableAmount = busEmployerReportHelper.GetRemittanceAvailableAmount(_icdoEmployerRemittanceAllocation.remittance_id);
        }

        public void LoadEmployerRemittanceAllocation()
        {
            if (_iclbEmployerRemittanceAllocation == null)
                _iclbEmployerRemittanceAllocation = new Collection<busEmployerRemittanceAllocation>();

            DataTable ldtbList = Select<cdoEmployerRemittanceAllocation>(
                            new string[1] { "employer_payroll_header_id" },
                            new object[1] { _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.employer_payroll_header_id }, null, null);
            _iclbEmployerRemittanceAllocation = GetCollection<busEmployerRemittanceAllocation>(ldtbList, "icdoEmployerRemittanceAllocation");
        }

        //If you change this method, make sure you also modify the code in CreateRemittanceAllocationAndAllocate Methid in EmployerPayrollHeader
        public void LoadEmployerPayrollHeader()
        {
            if (_ibusEmployerPayrollHeader == null)
            {
                _ibusEmployerPayrollHeader = new busEmployerPayrollHeader();
            }
            _ibusEmployerPayrollHeader.FindEmployerPayrollHeader(_icdoEmployerRemittanceAllocation.employer_payroll_header_id);
            if (_ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.payroll_paid_date != DateTime.MinValue)
            {
                _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.pay_period = _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.payroll_paid_date.ToString("MM/yyyy", CultureInfo.CreateSpecificCulture("en-US"));
            }
            else
            {
                _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.pay_period = String.Empty;
            }
            _ibusEmployerPayrollHeader.LoadOrganization();
            _ibusEmployerPayrollHeader.LoadContributionByPlan();
            _ibusEmployerPayrollHeader.CalculateContributionWagesInterestFromDtl();
        }


        //BR-034-37, BR-034-39
        /// <summary>
        /// Allocate the Remittance Amount to Payroll Header.
        /// 1)Allocated Amount should not exceed Remittance Amount
        /// </summary>
        /// <returns></returns>
        public ArrayList btnAllocate_Click()
        {
            ArrayList larrErrors = new ArrayList();
            decimal ldecTotalAllocatedAmount = 0.00M;
            decimal ldecDifferenceAmount = 0.00M;
            DataTable ldtbList = Select<cdoRemittance>(
               new string[1] { "remittance_id" },
               new object[1] { icdoEmployerRemittanceAllocation.remittance_id }, null, null);
            if (ldtbList.Rows.Count > 0)
            {
                LoadRemittance();
                ldecTotalAllocatedAmount =
                    Convert.ToDecimal(DBFunction.DBExecuteScalar("cdoEmployerRemittanceAllocation.GetSumofAllocatedAmount",
                                                                 new object[2]
                                                                 {
                                                                     icdoEmployerRemittanceAllocation.
                                                                         employer_payroll_header_id,
                                                                     _icdoEmployerRemittanceAllocation.
                                                                         employer_remittance_allocation_id
                                                                 },
                                                                 iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
                //prod pir 4962
                //--start--//
                if (iclbEmployerRemittanceAllocation == null)
                    LoadEmployerRemittanceAllocation();
                ldecDifferenceAmount = iclbEmployerRemittanceAllocation.Where(o => o.icdoEmployerRemittanceAllocation.employer_remittance_allocation_id != icdoEmployerRemittanceAllocation.employer_remittance_allocation_id &&
                                                            !string.IsNullOrEmpty(o.icdoEmployerRemittanceAllocation.difference_type_value) &&
                                                            (o.icdoEmployerRemittanceAllocation.difference_type_value == busConstant.GLItemTypeOverContribution ||
                                                            o.icdoEmployerRemittanceAllocation.difference_type_value == busConstant.GLItemTypeOverRHIC))
                                                            .Sum(o => o.icdoEmployerRemittanceAllocation.difference_amount);
                if (!string.IsNullOrEmpty(icdoEmployerRemittanceAllocation.difference_type_value) &&
                    (icdoEmployerRemittanceAllocation.difference_type_value == busConstant.GLItemTypeOverContribution ||
                    icdoEmployerRemittanceAllocation.difference_type_value == busConstant.GLItemTypeOverRHIC))
                {
                    ldecDifferenceAmount += icdoEmployerRemittanceAllocation.difference_amount;
                }
                //--end--//
                if (_icdoEmployerRemittanceAllocation.allocated_amount <= 0)
                {
                    utlError lobjError = AddError(4503, string.Empty);
                    larrErrors.Add(lobjError);
                    return larrErrors;
                }

                if (IsAllocatedAmountExceedAvailableAmount())
                {
                    utlError lobjError = AddError(4501, string.Empty);
                    larrErrors.Add(lobjError);
                    return larrErrors;
                }
                //check this validation if header type is not Purchase
                if (ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value != busConstant.PayrollHeaderBenefitTypePurchases)
                {
                    //Total Allocated Amount is Greater than Remittance amount, throw an error
                    //CHANGE : Instad of comparing with Total contribution calculated, compare with Total Amount Reported By All Plans
                    if ((ldecTotalAllocatedAmount + _icdoEmployerRemittanceAllocation.allocated_amount) > _ibusEmployerPayrollHeader.idecTotalReportedByPlan)
                    {
                        utlError lobjError = AddError(4625, string.Empty);
                        larrErrors.Add(lobjError);
                        return larrErrors;
                    }
                    if (!CheckForValidRemittance())
                    {
                        utlError lobjError = AddError(4504, string.Empty);
                        larrErrors.Add(lobjError);
                        return larrErrors;
                    }
                }
                else
                {
                    //UAT PIR 1488
                    if (!CheckForValidRemittanceForPurchases())
                    {
                        utlError lobjError = AddError(4504, string.Empty);
                        larrErrors.Add(lobjError);
                        return larrErrors;
                    }
                }
                //check if the remittance id selected was associated with the header org id
                int lintCount = Convert.ToInt32(DBFunction.DBExecuteScalar("cdoEmployerRemittanceAllocation.GetRemittanceForOrgIDWithAppliedStatus",
                                 new object[3] { ibusEmployerPayrollHeader.PlanBenefitTypeForHeaderType, 
                                  ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.org_id,_icdoEmployerRemittanceAllocation.remittance_id},
                                  iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
                if (lintCount == 0)
                {
                    utlError lobjError = AddError(4714, string.Empty);
                    larrErrors.Add(lobjError);
                    return larrErrors;
                }

                // check for purchase header type 
                if (ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypePurchases)
                {
                    if (ibusEmployerPayrollHeader.idecTotalPurchaseAmount < (ldecTotalAllocatedAmount + _icdoEmployerRemittanceAllocation.allocated_amount))
                    {
                        utlError lobjError = AddError(4501, string.Empty);
                        larrErrors.Add(lobjError);
                        return larrErrors;
                    }
                    if (ibusEmployerPayrollHeader.idecTotalPurchaseAmount == ldecTotalAllocatedAmount + _icdoEmployerRemittanceAllocation.allocated_amount)
                    {
                        _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.balancing_status_value =
                                                   busConstant.PayrollHeaderBalancingStatusBalanced;
                    }
                    else
                    {
                        _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.balancing_status_value =
                           busConstant.PayrollHeaderBalancingStatusUnbalanced;
                    }

                }
                //check this validation if header type is not Purchase
                if (ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value != busConstant.PayrollHeaderBenefitTypePurchases)
                {
                    //Update the Payroll Hedaer Balancing Status, if the total allocated amount equals
                    //prod pir 4079 : allow a variance in remittane
                    //prod pir 5962 : need to add the difference amount and equate to total reported amount
                    if ((ldecTotalAllocatedAmount + _icdoEmployerRemittanceAllocation.allocated_amount + ldecDifferenceAmount) == _ibusEmployerPayrollHeader.idecTotalReportedByPlan)
                    {
                        _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.balancing_status_value =
                            busConstant.PayrollHeaderBalancingStatusBalanced;
                    }
                    else
                    {
                        _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.balancing_status_value =
                           busConstant.PayrollHeaderBalancingStatusUnbalanced;
                    }
                }
                _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.balancing_status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1209, ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.balancing_status_value);
                _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.Update();

                _icdoEmployerRemittanceAllocation.payroll_allocation_status_value = busConstant.Allocated;
                _icdoEmployerRemittanceAllocation.allocated_date = DateTime.Now;

                if (_icdoEmployerRemittanceAllocation.ienuObjectState == ObjectState.Insert)
                {
                    _icdoEmployerRemittanceAllocation.Insert();
                }
                else if ((_icdoEmployerRemittanceAllocation.ienuObjectState == ObjectState.Update)
                    || (_icdoEmployerRemittanceAllocation.ienuObjectState == ObjectState.Select))
                {
                    _icdoEmployerRemittanceAllocation.Update();
                }
            }
            else
            {
                utlError lobjError = AddError(4502, string.Empty);
                larrErrors.Add(lobjError);
                return larrErrors;
            }

            LoadRemittance();
            LoadDeposit();
            LoadAvailableAmount();

            //PIR 328 - to refresh available remittance after allocation
            ibusEmployerPayrollHeader.LoadAvailableRemittanace();

            EvaluateInitialLoadRules(utlPageMode.All);

            larrErrors.Add(this);
            return larrErrors;
        }

        //BR - 29,30,31,32,33
        public bool CheckForValidRemittance()
        {
            bool lblnAccountExists = false;
            DataTable ldtbListOfPlans = busNeoSpinBase.Select("cdoEmployerPayrollHeader.LoadDistinctPlanInPayrollDetailByHeader",
                     new object[1] { _icdoEmployerRemittanceAllocation.employer_payroll_header_id });
            if (ldtbListOfPlans.Rows.Count > 0)
            {
                foreach (DataRow dr in ldtbListOfPlans.Rows)
                {
                    string lintPlanID = Convert.ToString(dr["plan_id"]);
                    int lintcount = Convert.ToInt32(DBFunction.DBExecuteScalar("cdoEmployerRemittanceAllocation.CheckAccountExistsforPlanAndRemitType",
                                        new string[2] { lintPlanID, _ibusRemittance.icdoRemittance.remittance_type_value },
                                       iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
                    if (lintcount > 0)
                    {
                        lblnAccountExists = true;
                        break;
                    }
                }
            }
            return lblnAccountExists;
        }

        //BR - 29,30,31,32,33
        public bool CheckForValidRemittanceForPurchases()
        {
            bool lblnAccountExists = false;
            DataTable ldtbListOfPlans = busNeoSpinBase.Select("cdoEmployerPayrollHeader.LoadDistinctPlanByPurchaseAndPayrollHeader",
                     new object[1] { _icdoEmployerRemittanceAllocation.employer_payroll_header_id });
            if (ldtbListOfPlans.Rows.Count > 0)
            {
                foreach (DataRow dr in ldtbListOfPlans.Rows)
                {
                    string lintPlanID = Convert.ToString(dr["plan_id"]);
                    int lintcount = Convert.ToInt32(DBFunction.DBExecuteScalar("cdoEmployerRemittanceAllocation.CheckAccountExistsforPlanAndRemitType",
                                        new string[2] { lintPlanID, _ibusRemittance.icdoRemittance.remittance_type_value },
                                       iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
                    if (lintcount > 0)
                    {
                        lblnAccountExists = true;
                        break;
                    }
                }
            }
            return lblnAccountExists;
        }

        /// <summary>
        /// PIR 223 : This method validate the given amount should not exceed total amount by plan (header)
        /// </summary>
        /// <returns></returns>
        public bool IsAllocatedAmountExceedTotalAmountByPlan()
        {
            if (icdoEmployerRemittanceAllocation.remittance_id > 0)
            {
                if (_ibusRemittance == null)
                    LoadRemittance();

                decimal ldecTotalReportedAmountByPlan =
                    _ibusEmployerPayrollHeader.GetTotalReportedAmountByPlan(_ibusRemittance.icdoRemittance.plan_id, ibusRemittance.icdoRemittance.remittance_type_value);

                decimal ldecTotalAppliedAmountByPlan =
                    GetTotalAppliedAmountByPlan(_ibusRemittance.icdoRemittance.plan_id, ibusRemittance.icdoRemittance.remittance_type_value) + icdoEmployerRemittanceAllocation.allocated_amount;

                if (ldecTotalAppliedAmountByPlan > ldecTotalReportedAmountByPlan)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// This method calculates the Total Applied Amount for the current plan. It excluded the Current Entry 
        /// </summary>
        /// <returns></returns>
        public decimal GetTotalAppliedAmountByPlan(int aintPlanID, string astrRemittanceType)
        {
            decimal ldecAppliedAmount = 0;

            if (_iclbEmployerRemittanceAllocation == null)
                LoadEmployerRemittanceAllocation();

            //PIR 404
            bool lblnCheckRemittanceType = false;
            if (_ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
                lblnCheckRemittanceType = true;

            foreach (busEmployerRemittanceAllocation lobjEmpRemAllocation in _iclbEmployerRemittanceAllocation)
            {
                if (lobjEmpRemAllocation.icdoEmployerRemittanceAllocation.employer_remittance_allocation_id != icdoEmployerRemittanceAllocation.employer_remittance_allocation_id)
                {
                    lobjEmpRemAllocation.LoadRemittance();

                    if (lblnCheckRemittanceType)
                    {
                        if ((lobjEmpRemAllocation.ibusRemittance.icdoRemittance.plan_id == aintPlanID) &&
                            (lobjEmpRemAllocation.ibusRemittance.icdoRemittance.remittance_type_value == astrRemittanceType))
                        {
                            ldecAppliedAmount += lobjEmpRemAllocation.icdoEmployerRemittanceAllocation.allocated_amount;
                            //PROD PIR ID 6062
                            //PIR 15238 - Applied amount should be the sum of allocated remittances of type Retirement/RHIC Contributions (not considering Over/Under)
                            //if (lobjEmpRemAllocation.icdoEmployerRemittanceAllocation.difference_type_value == busConstant.GLItemTypeOverContribution ||
                            //    lobjEmpRemAllocation.icdoEmployerRemittanceAllocation.difference_type_value == busConstant.GLItemTypeOverRHIC)
                            //ldecAppliedAmount += lobjEmpRemAllocation.icdoEmployerRemittanceAllocation.difference_amount;
                        }
                    }
                    else
                    {
                        if (lobjEmpRemAllocation.ibusRemittance.icdoRemittance.plan_id == aintPlanID)
                        {
                            ldecAppliedAmount += lobjEmpRemAllocation.icdoEmployerRemittanceAllocation.allocated_amount;
                        }
                    }

                }
            }
            return ldecAppliedAmount;
        }

        //This method is called in Business Rule to check whether the entered "Allocated Amount" is greater than "Available Amount".
        public bool IsAllocatedAmountExceedAvailableAmount()
        {
            LoadAvailableAmount();
            if (_icdoEmployerRemittanceAllocation.allocated_amount > _idecRemittanceAvailableAmount)
            {
                return true;
            }
            return false;
        }

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            //Loading the Remittance Object for the Validation
            if (_icdoEmployerRemittanceAllocation.remittance_id != 0)
            {
                LoadRemittance();
                if (_ibusRemittance.icdoRemittance.deposit_id == 0)
                {
                    _icdoEmployerRemittanceAllocation.remittance_id = 0;
                }
                else
                {
                    LoadDeposit();
                    LoadAvailableAmount();
                }
            }

            base.BeforeValidate(aenmPageMode);
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            //prod pir 5394
            btnAllocate_Click();
            LoadAvailableAmount();
            LoadEmployerPayrollHeader();  // PIR-5715, Upadting an Allocation amount on click of Save button.
            LoadRemittance();
            LoadDeposit();
        }

        //UAT PIR 160
        public override int Delete()
        {
            int lintResult = base.Delete();
            if (lintResult == 1)
            {
                if (ibusEmployerPayrollHeader == null)
                    LoadEmployerPayrollHeader();

                ibusEmployerPayrollHeader.LoadEmployerRemittanceAllocation();

                if (ibusEmployerPayrollHeader.iclbEmployerRemittanceAllocation.Count > 0)
                    ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.balancing_status_value = busConstant.PayrollHeaderBalancingStatusUnbalanced;
                else
                    ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.balancing_status_value = busConstant.PayrollHeaderBalancingStatusNoRemittance;

                ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.Update();

                //PROD PIR 4079 : negate difference type GL
                if (icdoEmployerRemittanceAllocation.difference_amount > 0)
                {
                    if (ibusRemittance == null)
                        LoadRemittance();
                    if (ibusEmployerPayrollHeader == null)
                        LoadEmployerPayrollHeader();
                    CreateGLForDifferenceAmount(icdoEmployerRemittanceAllocation.difference_amount, ibusRemittance.icdoRemittance.plan_id);
                }
            }
            return lintResult;
        }

        /// <summary>
        /// prod pir 4079 : method to create gl
        /// </summary>
        /// <param name="astrRemittanceType"></param>
        /// <param name="adecDifferencAmount"></param>
        /// <param name="aintPlanId"></param>
        public void CreateGLForDifferenceAmount(decimal adecDifferenceAmount, int aintPlanId)
        {
            cdoAccountReference lcdoAcccountReference = new cdoAccountReference();
            lcdoAcccountReference.plan_id = aintPlanId;
            lcdoAcccountReference.source_type_value = busConstant.SourceTypeEmployerReporting;
            lcdoAcccountReference.transaction_type_value = busConstant.TransactionTypeAllocation;
            lcdoAcccountReference.from_item_type_value = icdoEmployerRemittanceAllocation.difference_type_value;
            
            busGLHelper.GenerateGL(lcdoAcccountReference, 0, ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.org_id,
                                              ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.employer_payroll_header_id,
                                              Math.Abs(adecDifferenceAmount),
                                              ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.payroll_paid_date,
                                              busGlobalFunctions.GetSysManagementBatchDate(), iobjPassInfo);
        }
    }
}
