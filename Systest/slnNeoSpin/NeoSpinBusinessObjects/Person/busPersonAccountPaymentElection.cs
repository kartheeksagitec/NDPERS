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

#endregion

namespace NeoSpin.BusinessObjects
{
	[Serializable]
	public class busPersonAccountPaymentElection : busPersonAccount
	{
        private cdoPersonAccountPaymentElection _icdoPersonAccountPaymentElection;
        public cdoPersonAccountPaymentElection icdoPersonAccountPaymentElection
        {
            get
            {
                return _icdoPersonAccountPaymentElection;
            }
            set
            {
                _icdoPersonAccountPaymentElection = value;
            }
        }

        public bool FindPersonAccountPaymentElection(int Aintaccountpaymentelectionid)
        {
            bool lblnResult = false;
            if (_icdoPersonAccountPaymentElection == null)
            {
                _icdoPersonAccountPaymentElection = new cdoPersonAccountPaymentElection();
            }
            if (_icdoPersonAccountPaymentElection.SelectRow(new object[1] { Aintaccountpaymentelectionid }))
            {
                lblnResult = true;
            }
            return lblnResult;
        }

        public bool FindPersonAccountPaymentElectionByPersonAccountID(int AintPersonAccountID)
        {
            bool lblnResult = false;
            DataTable ldtbList = Select<cdoPersonAccountPaymentElection>(
                new string[1] { "person_account_id" },
                new object[1] { AintPersonAccountID }, null, null);
            if (_icdoPersonAccountPaymentElection == null)
                _icdoPersonAccountPaymentElection = new cdoPersonAccountPaymentElection();
            if (ldtbList.Rows.Count == 1)
            {
                _icdoPersonAccountPaymentElection.LoadData(ldtbList.Rows[0]);
                lblnResult = true;
            }
            else if (ldtbList.Rows.Count > 1)
            {
                throw new Exception("FindPersonAccountPaymentElectionByPersonAccountID method : Multiple records returned for given Person Account ID : " +
                    AintPersonAccountID);
            }
            return lblnResult;
        }

        public busPayeeAccount ibusPayeeAccount { get; set; }

        public void LoadPayeeAccount()
        {
            if (ibusPayeeAccount.IsNull())
                ibusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
            ibusPayeeAccount.FindPayeeAccount(icdoPersonAccountPaymentElection.payee_account_id);
        }

        public bool IsValidPayeeAccountSelected()
        {
            if (icdoPersonAccountPaymentElection.payment_method_value == busConstant.IBSModeOfPaymentPensionCheck)
            {
                if ((icdoPersonAccountPaymentElection.person_account_id != 0) &&
                    icdoPersonAccountPaymentElection.payee_account_id != 0)
                {
                    int lintCount = Convert.ToInt32(DBFunction.DBExecuteScalar("cdoPersonAccountGhdv.CheckValidPayeeAccountSelected", new object[2]{
                                 icdoPersonAccountPaymentElection.payee_account_id,icdoPersonAccountPaymentElection.person_account_id},
                                            iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
                    if (lintCount < 1)
                        return false;
                }
            }
            return true;
        }

        // *** BR-074-10 *** Returns true if the selected Payee Account Status is any one of the following 1,Approved 2,Receiving, 3,DC Receiving, 4,Review. 
        public bool IsValidPayeeAccountStatusExists()
        {
            if (icdoPersonAccountPaymentElection.payment_method_value == busConstant.IBSModeOfPaymentPensionCheck)
            {
                if (ibusPayeeAccount.IsNull())
                    LoadPayeeAccount();
                if (ibusPayeeAccount.iclbPayeeAccountStatus.IsNull())
                    ibusPayeeAccount.LoadPayeeAccountStatus();

                foreach (busPayeeAccountStatus lobjStatus in ibusPayeeAccount.iclbPayeeAccountStatus)
                {
                    string lstrStatusValue = busGlobalFunctions.GetData2ByCodeValue(2203, lobjStatus.icdoPayeeAccountStatus.status_value,iobjPassInfo);
                    // Sorted the Collection in Load method by Effective Date
                    if ((lstrStatusValue == busConstant.PayeeAccountStatusApproved) ||
                        (lstrStatusValue == busConstant.PayeeAccountStatusReceiving) ||
                        (lstrStatusValue == busConstant.PayeeAccountStatusDCReceiving) ||
                        (lstrStatusValue == busConstant.PayeeAccountStatusReview))
                        return true;
                    else
                        return false;
                }
            }
            return true;
        }

        //NOTE :- to be set only if called from PERSLink BATCH
        //property to post the batch schedule ID into PAPIT if called from Update RHIC Rate Change batch
        public int iintBatchScheduleId { get; set; }

        // *** BR-074-12 *** System must transfer insurance premium information to the Payee Account's deduction list,
        // when insurance premium information is updated by the enrollment
        public void ManagePayeeAccountPaymentItemType(int aintPayeeAccountID, int aintOldPayeeAccountID, decimal adecMonthlyPremiumAmount, int aintPlanID,
            DateTime adteStartDate, string astrPaymentMethodValue, int lintProviderOrgPlanId, int aintPersonAccountID = 0, bool IsMedicare = false, bool ablnIsPayeeAccountChanged = false)
        {
            string lstrItemCode = string.Empty;
            switch (aintPlanID)
            {
                case busConstant.PlanIdGroupHealth:
                    lstrItemCode = busConstant.PAPITHealthInsurance;
                    break;
                case busConstant.PlanIdGroupLife:
                    lstrItemCode = busConstant.PAPITLifeInsurance;
                    break;
                case busConstant.PlanIdDental:
                    lstrItemCode = busConstant.PAPITDentalInsurance;
                    break;
                case busConstant.PlanIdVision:
                    lstrItemCode = busConstant.PAPITVisionInsurance;
                    break;
                case busConstant.PlanIdLTC:
                    lstrItemCode = busConstant.PAPITLTCInsurance;
                    break;
                case busConstant.PlanIdMedicarePartD:
                    lstrItemCode = busConstant.PAPITMedicarePartD;
                    break;
                default:
                    break;
            }
            busPayeeAccount lobjPayeeAccount = null;
            if (!ablnIsPayeeAccountChanged)
            {
                lobjPayeeAccount = new busPayeeAccount();
                lobjPayeeAccount.FindPayeeAccount(aintPayeeAccountID);
                lobjPayeeAccount.LoadPayeeAccountPaymentItemType();
                bool lblnIsAmountChanged = false; bool lblnIsItemExists = false;
                if (astrPaymentMethodValue == busConstant.IBSModeOfPaymentPensionCheck)
                {
                    foreach (busPayeeAccountPaymentItemType lobjPAPIT in lobjPayeeAccount.iclbPayeeAccountPaymentItemType)
                    {
                        if (lobjPAPIT.ibusPaymentItemType == null)
                            lobjPAPIT.LoadPaymentItemType();
                        if ((lobjPAPIT.ibusPaymentItemType.icdoPaymentItemType.item_type_code == lstrItemCode && !IsMedicare) ||
                            (lobjPAPIT.ibusPaymentItemType.icdoPaymentItemType.item_type_code == lstrItemCode && IsMedicare && lobjPAPIT.icdoPayeeAccountPaymentItemType.person_account_id
                            == aintPersonAccountID)) //PIR 15575
                        {
                            if (adteStartDate != DateTime.MinValue && lobjPAPIT.icdoPayeeAccountPaymentItemType.end_date == DateTime.MinValue)
                                lblnIsItemExists = true;
                            if (lobjPAPIT.icdoPayeeAccountPaymentItemType.amount != adecMonthlyPremiumAmount)
                                lblnIsAmountChanged = true;
                            if (lblnIsAmountChanged)
                            {
                                UpdatePayeeAccountPaymentItemTypeEndDate(lobjPAPIT, lstrItemCode, adteStartDate);
                            }
                        }
                    }
                    if ((lblnIsAmountChanged) || (!lblnIsItemExists) || lobjPayeeAccount.iclbPayeeAccountPaymentItemType.Count == 0)
                    {
                        lobjPayeeAccount.iintBatchScheudleID = iintBatchScheduleId;
                        lobjPayeeAccount.CreatePayeeAccountPaymentItemType(lstrItemCode, adecMonthlyPremiumAmount, string.Empty, lintProviderOrgPlanId, adteStartDate, DateTime.MinValue, aintPersonAccountID);
                    }
                }
            }
            if (ablnIsPayeeAccountChanged)
            {
                if (aintOldPayeeAccountID != 0 && aintOldPayeeAccountID != aintPayeeAccountID)
                {
                    lobjPayeeAccount = new busPayeeAccount();
                    lobjPayeeAccount.FindPayeeAccount(aintOldPayeeAccountID);
                    lobjPayeeAccount.LoadPayeeAccountPaymentItemType();
                    if (!IsMedicare)
                    {
                        UpdatePayeeAccountPaymentItemTypeEndDate(lobjPayeeAccount, lstrItemCode, adteStartDate);
                    }
                    else
                    {
                        UpdatePayeeAccountPaymentItemTypeEndDate(lobjPayeeAccount, lstrItemCode, adteStartDate, aintPersonAccountID);
                    }
                }
            }
        }

        public void UpdatePayeeAccountPaymentItemTypeEndDate(busPayeeAccount aobjPayeeAccount, string astrItemCode, DateTime adtStartDate, int aintPersonAccountID)
        {
            foreach (busPayeeAccountPaymentItemType lobjPAPIT in aobjPayeeAccount.iclbPayeeAccountPaymentItemType)
            {
                if (lobjPAPIT.ibusPaymentItemType == null)
                    lobjPAPIT.LoadPaymentItemType();
                if (lobjPAPIT.ibusPaymentItemType.icdoPaymentItemType.item_type_code == astrItemCode && lobjPAPIT.icdoPayeeAccountPaymentItemType.person_account_id == aintPersonAccountID)
                {
                    UpdatePayeeAccountPaymentItemTypeEndDate(lobjPAPIT, astrItemCode, adtStartDate);
                }
            }
        }

        //Function to loop through payee account payment item type and update the record
        public void UpdatePayeeAccountPaymentItemTypeEndDate(busPayeeAccount aobjPayeeAccount, string astrItemCode, DateTime adtStartDate)
        {
            foreach (busPayeeAccountPaymentItemType lobjPAPIT in aobjPayeeAccount.iclbPayeeAccountPaymentItemType)
            {
                if (lobjPAPIT.ibusPaymentItemType == null)
                    lobjPAPIT.LoadPaymentItemType();
                if (lobjPAPIT.ibusPaymentItemType.icdoPaymentItemType.item_type_code == astrItemCode)
                {
                    UpdatePayeeAccountPaymentItemTypeEndDate(lobjPAPIT, astrItemCode, adtStartDate);
                }
            }
        }

        //Function to update the end date for the payee account payment item type
        public void UpdatePayeeAccountPaymentItemTypeEndDate(busPayeeAccountPaymentItemType aobjPAPIT, string astrItemCode, DateTime adtStartDate)
        {
            if (adtStartDate != DateTime.MinValue && aobjPAPIT.icdoPayeeAccountPaymentItemType.end_date == DateTime.MinValue)
            {
                if (aobjPAPIT.icdoPayeeAccountPaymentItemType.start_date < adtStartDate)
                {
                    aobjPAPIT.icdoPayeeAccountPaymentItemType.end_date = adtStartDate.AddDays(-1);
                    aobjPAPIT.icdoPayeeAccountPaymentItemType.batch_schedule_id = iintBatchScheduleId;
                    aobjPAPIT.icdoPayeeAccountPaymentItemType.Update();
                }
                else
                {
                    aobjPAPIT.icdoPayeeAccountPaymentItemType.Delete();
                }
            }
        }

        // PROD PIR ID 4735
        public void EndPAPITEntriesForSuspendedAccount(int aintPlanID, DateTime adteEndDate)
        {
            if (icdoPersonAccountPaymentElection.payment_method_value == busConstant.IBSModeOfPaymentPensionCheck &&
                icdoPersonAccountPaymentElection.payee_account_id != 0)
            {
                if (ibusPayeeAccount.IsNull())
                    LoadPayeeAccount();
                if (ibusPayeeAccount.iclbPayeeAccountPaymentItemType.IsNull())
                    ibusPayeeAccount.LoadPayeeAccountPaymentItemType();

                string lstrItemCode = string.Empty;
                switch (aintPlanID)
                {
                    case busConstant.PlanIdGroupHealth:
                        lstrItemCode = busConstant.PAPITHealthInsurance;
                        break;
                    case busConstant.PlanIdGroupLife:
                        lstrItemCode = busConstant.PAPITLifeInsurance;
                        break;
                    case busConstant.PlanIdDental:
                        lstrItemCode = busConstant.PAPITDentalInsurance;
                        break;
                    case busConstant.PlanIdVision:
                        lstrItemCode = busConstant.PAPITVisionInsurance;
                        break;
                    case busConstant.PlanIdLTC:
                        lstrItemCode = busConstant.PAPITLTCInsurance;
                        break;
                    case busConstant.PlanIdMedicarePartD:
                        lstrItemCode = busConstant.PAPITMedicarePartD;
                        break;
                    default:
                        break;
                }

                DateTime ldteLastDayofNextPaymentDate = busPayeeAccountHelper.GetLastBenefitPaymentDate().GetLastDayofMonth(); //Next Benefit Date - 1 day
                DateTime ldtePreviousDayofSuspensionDate = adteEndDate != DateTime.MinValue ? adteEndDate.AddDays(-1) : adteEndDate; //History change date - 1 day
                DateTime ldteEndDate = busGlobalFunctions.GetMax(ldteLastDayofNextPaymentDate, ldtePreviousDayofSuspensionDate);

                if (aintPlanID != busConstant.PlanIdMedicarePartD)
                {
                    foreach (busPayeeAccountPaymentItemType lobjPAPIT in ibusPayeeAccount.iclbPayeeAccountPaymentItemType)
                    {
                        if (lobjPAPIT.ibusPaymentItemType == null)
                            lobjPAPIT.LoadPaymentItemType();
                        if (lobjPAPIT.ibusPaymentItemType.icdoPaymentItemType.item_type_code == lstrItemCode)
                        {
                            if (ldteEndDate != DateTime.MinValue && lobjPAPIT.icdoPayeeAccountPaymentItemType.end_date == DateTime.MinValue)
                            {
                                if (lobjPAPIT.icdoPayeeAccountPaymentItemType.start_date < ldteEndDate)
                                {
                                    lobjPAPIT.icdoPayeeAccountPaymentItemType.end_date = ldteEndDate;
                                    lobjPAPIT.icdoPayeeAccountPaymentItemType.Update();
                                }
                                else
                                {
                                    lobjPAPIT.icdoPayeeAccountPaymentItemType.Delete();
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (busPayeeAccountPaymentItemType lobjPAPIT in ibusPayeeAccount.iclbPayeeAccountPaymentItemType)
                    {
                        if (lobjPAPIT.ibusPaymentItemType == null)
                            lobjPAPIT.LoadPaymentItemType();
                        if (lobjPAPIT.ibusPaymentItemType.icdoPaymentItemType.item_type_code == lstrItemCode && lobjPAPIT.icdoPayeeAccountPaymentItemType.person_account_id == icdoPersonAccountPaymentElection.person_account_id)
                        {
                            if (ldteEndDate != DateTime.MinValue && lobjPAPIT.icdoPayeeAccountPaymentItemType.end_date == DateTime.MinValue)
                            {
                                if (lobjPAPIT.icdoPayeeAccountPaymentItemType.start_date < ldteEndDate)
                                {
                                    lobjPAPIT.icdoPayeeAccountPaymentItemType.end_date = ldteEndDate;
                                    lobjPAPIT.icdoPayeeAccountPaymentItemType.Update();
                                }
                                else
                                {
                                    lobjPAPIT.icdoPayeeAccountPaymentItemType.Delete();
                                }
                            }
                        }
                    }
                }
            }
        }
	}
}
