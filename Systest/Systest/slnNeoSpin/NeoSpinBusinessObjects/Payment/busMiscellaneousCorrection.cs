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
using System.Collections.Generic;
#endregion

namespace NeoSpin.BusinessObjects
{

    [Serializable]
    public class busMiscellaneousCorrection : busPayeeAccountPaymentItemType
    {
        public int iintVendorOrgID { get; set; }
        //propety to contain all PAPITs for a payee account id
        public Collection<busMiscellaneousCorrection> iclbPAPIT { get; set; }
        public int iintUserSerialID {get;set;}
        /// <summary>
        /// method to load history of PAPIT
        /// </summary>
        /// <param name="aintPayeeAccountID"></param>
        public bool LoadPAPITHistory(int aintPayeeAccountID)
        {
            bool lblnResult = false;
            DataTable ldtPAPIT = Select<cdoPayeeAccountPaymentItemType>(new string[1] { "payee_account_id" },
                new object[1] { aintPayeeAccountID }, null, null);
            if (ldtPAPIT.Rows.Count > 0)
            {
                lblnResult = true;
                iclbPAPIT = GetCollection<busMiscellaneousCorrection>(ldtPAPIT, "icdoPayeeAccountPaymentItemType");
                foreach (busMiscellaneousCorrection lobjPAPIT in iclbPAPIT)
                {
                    lobjPAPIT.LoadPaymentItemType();
                }
            }
            return lblnResult;
        }       

        //Method to load Next Benefit Payment Date
        public void LoadNextBenefitPaymentDate()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            ibusPayeeAccount.LoadNexBenefitPaymentDate();
        }

        /// <summary>
        /// Method to check whether end date is last day of the month
        /// </summary>
        /// <returns>boolean value</returns>
        public bool IsEndDateLastDayofMonth()
        {
            bool lblnResult = false;
            if (icdoPayeeAccountPaymentItemType.end_date == DateTime.MinValue)
                lblnResult = true;
            else if (icdoPayeeAccountPaymentItemType.end_date.GetFirstDayofNextMonth().AddDays(-1) == icdoPayeeAccountPaymentItemType.end_date)
                lblnResult = true;
            return lblnResult;
        }

        /// <summary>
        /// Method to check whether end date is one day less than next benefit payment date
        /// </summary>
        /// <returns>boolean value</returns>
        public bool IsEndDateValid()
        {
             bool lblnResult = true;
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                ibusPayeeAccount.LoadNexBenefitPaymentDate();
            if(icdoPayeeAccountPaymentItemType.end_date != DateTime.MinValue &&
                icdoPayeeAccountPaymentItemType.start_date < ibusPayeeAccount.idtNextBenefitPaymentDate &&
                icdoPayeeAccountPaymentItemType.end_date != ibusPayeeAccount.idtNextBenefitPaymentDate.AddDays(-1))                
                lblnResult = false;
            return lblnResult;
        }

        /// <summary>
        /// Method to check whether end date is greater than start date
        /// </summary>
        /// <returns>boolean value</returns>
        public bool IsEndDateGreaterThanStartDate()
        {
            bool lblnResult = true;
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                ibusPayeeAccount.LoadNexBenefitPaymentDate();
            if (icdoPayeeAccountPaymentItemType.end_date != DateTime.MinValue &&                
                icdoPayeeAccountPaymentItemType.end_date < icdoPayeeAccountPaymentItemType.start_date)
                lblnResult = false;
            return lblnResult;
        }

        /// <summary>
        /// Method to check whether new start date is greater than last end date
        /// </summary>
        /// <returns>boolean value</returns>
        public bool IsStartDateValid()
        {
            bool lblnResult = false;    
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                ibusPayeeAccount.LoadNexBenefitPaymentDate();
            if (icdoPayeeAccountPaymentItemType.payment_item_type_id == ibusPayeeAccount.GetPaymentItemTypeIDByItemCode(busConstant.PAPITTaxableAmount))
                lblnResult = true;
            else if (ibusPayeeAccount.idtNextBenefitPaymentDate <= icdoPayeeAccountPaymentItemType.start_date)
                lblnResult = true;
            return lblnResult;
        }

        /// <summary>
        /// Method to check whether new start date is valid for Payment item type Taxable amount
        /// </summary>
        /// <returns>boolean value</returns>
        public bool IsStartDateValidForTaxableAmount()
        {
            bool lblnResult = false;
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                ibusPayeeAccount.LoadNexBenefitPaymentDate();
            if (iclbPAPIT == null)
                LoadPAPITHistory(ibusPayeeAccount.icdoPayeeAccount.payee_account_id);
            busPayeeAccountPaymentItemType lobjPAPIT = iclbPAPIT.Where(o => o.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.PAPITTaxableAmount)
                                                                .OrderByDescending(o => o.icdoPayeeAccountPaymentItemType.start_date)
                                                                .FirstOrDefault();
            if (icdoPayeeAccountPaymentItemType.payment_item_type_id != ibusPayeeAccount.GetPaymentItemTypeIDByItemCode(busConstant.PAPITTaxableAmount))
                lblnResult = true;
            else if (lobjPAPIT == null && icdoPayeeAccountPaymentItemType.start_date >= ibusPayeeAccount.idtNextBenefitPaymentDate)
                lblnResult = true;
            else if (lobjPAPIT != null && icdoPayeeAccountPaymentItemType.start_date == lobjPAPIT.icdoPayeeAccountPaymentItemType.end_date.AddDays(1))
                lblnResult = true;            
            return lblnResult;
        }

        /// <summary>
        /// method to check whether start date is overlapping with same item which is closed
        /// </summary>
        /// <returns></returns>
        public bool IsStartDateOverlapping()
        {
            bool lblnResult = false;
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                ibusPayeeAccount.LoadNexBenefitPaymentDate();
            if (iclbPAPIT == null)
                LoadPAPITHistory(ibusPayeeAccount.icdoPayeeAccount.payee_account_id);
            busPayeeAccountPaymentItemType lobjPAPIT = null;
            if (icdoPayeeAccountPaymentItemType.payee_account_payment_item_type_id > 0)
            {
                lobjPAPIT = iclbPAPIT.Where(o =>o.icdoPayeeAccountPaymentItemType.payee_account_payment_item_type_id != icdoPayeeAccountPaymentItemType.payee_account_payment_item_type_id && 
                                                o.icdoPayeeAccountPaymentItemType.payment_item_type_id == icdoPayeeAccountPaymentItemType.payment_item_type_id &&
                                                ((o.icdoPayeeAccountPaymentItemType.start_date <= icdoPayeeAccountPaymentItemType.start_date &&
                                                o.icdoPayeeAccountPaymentItemType.end_date >= icdoPayeeAccountPaymentItemType.start_date) ||
                                                o.icdoPayeeAccountPaymentItemType.start_date > icdoPayeeAccountPaymentItemType.start_date)).FirstOrDefault();
            }
            else
            {
                lobjPAPIT = iclbPAPIT.Where(o => o.icdoPayeeAccountPaymentItemType.payment_item_type_id == icdoPayeeAccountPaymentItemType.payment_item_type_id &&
                                                ((o.icdoPayeeAccountPaymentItemType.start_date <= icdoPayeeAccountPaymentItemType.start_date &&
                                                o.icdoPayeeAccountPaymentItemType.end_date >= icdoPayeeAccountPaymentItemType.start_date) ||
                                                o.icdoPayeeAccountPaymentItemType.start_date > icdoPayeeAccountPaymentItemType.start_date)).FirstOrDefault();
            }
            if (lobjPAPIT != null)
                lblnResult = true;
            return lblnResult;
        }

        /// <summary>
        /// Method to check whether entered item is Active
        /// </summary>
        /// <returns></returns>
        public bool IsOpenItemAvailable()
        {
            bool lblnResult = false;
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                ibusPayeeAccount.LoadNexBenefitPaymentDate();
            if (iclbPAPIT == null)
                LoadPAPITHistory(ibusPayeeAccount.icdoPayeeAccount.payee_account_id);
            busPayeeAccountPaymentItemType lobjPAPIT = iclbPAPIT.Where(o => o.icdoPayeeAccountPaymentItemType.payment_item_type_id == icdoPayeeAccountPaymentItemType.payment_item_type_id &&
                                                                            o.icdoPayeeAccountPaymentItemType.end_date == DateTime.MinValue).FirstOrDefault();
            if (lobjPAPIT != null)
                lblnResult = true;
            return lblnResult;
        }

        public override void BeforePersistChanges()
        {
            if(ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (string.IsNullOrEmpty(icdoPayeeAccountPaymentItemType.miscellaneous_correction_flag))
                icdoPayeeAccountPaymentItemType.miscellaneous_correction_flag = busConstant.Flag_Yes;

            icdoPayeeAccountPaymentItemType.vendor_org_id = iintVendorOrgID;
        
            base.BeforePersistChanges();
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            LoadPAPITHistory(ibusPayeeAccount.icdoPayeeAccount.payee_account_id);
        }

        //ucs - 54 -- BR - 054- 53
        /// <summary>
        /// method to check whether rhic benefit reimb. item can be created for the payee account
        /// </summary>
        /// <returns>bool</returns>
        public bool IsRHICItemAllowable()
        {
            bool lblnResult = true;
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.icdoPayeeAccount.account_relation_value == busConstant.AccountRelationshipBeneficiary &&
                icdoPayeeAccountPaymentItemType.payment_item_type_id ==
                ibusPayeeAccount.GetPaymentItemTypeIDByItemCode(busConstant.PaymentItemTypeIDRHICBenefitReimbursement))
            {
                lblnResult = false;
            }
            return lblnResult;
        }

        //uat pir 1420
        public bool IsPayeeAccountStatusCancelPendingOrCancelled()
        {
            bool lblnResult = false;
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.ibusPayeeAccountActiveStatus == null)
                ibusPayeeAccount.LoadActivePayeeStatusAsofNextBenefitPaymentDate();

            if (ibusPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusCancelPending() || ibusPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusCancelled())
                lblnResult = true;

            return lblnResult;
        }

        //PROD Pir - 4488
        //visible rule for amount field
        public bool IsAmountFieldEditable()
        {
            bool lblnResult = false;

            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                ibusPayeeAccount.LoadNexBenefitPaymentDate();

            if (icdoPayeeAccountPaymentItemType.start_date == DateTime.MinValue || icdoPayeeAccountPaymentItemType.start_date >= ibusPayeeAccount.idtNextBenefitPaymentDate)
                lblnResult = true;

            return lblnResult;
        }
		
        //PIR 17321
		public bool IsResourceEligibleForSave
        {
            get
            {
                bool lblnResult = false;

                DataTable ldtbCount = busNeoSpinBase.Select("cdoPayeeAccountPaymentItemType.CheckResourceForLoggedInUserForSaveButton", new object[1] { iobjPassInfo.iintUserSerialID });
                if (ldtbCount.Rows.Count > 0)
                {
                    lblnResult = true;
                }
                return lblnResult;
            }

        }

        public bool IsPaymentTypeCodeReceivables
        {
            get
            {
                bool lblnResult = false;
                if (ibusPaymentItemType.IsNotNull())
                {
                    if (ibusPaymentItemType.icdoPaymentItemType.item_type_code == "ITEM88" ||
                        ibusPaymentItemType.icdoPaymentItemType.item_type_code == "ITEM89" ||
                        ibusPaymentItemType.icdoPaymentItemType.item_type_code == "ITEM90" ||
                        ibusPaymentItemType.icdoPaymentItemType.item_type_code == "ITEM91" ||
                        ibusPaymentItemType.icdoPaymentItemType.item_type_code == "ITEM92" ||
                        ibusPaymentItemType.icdoPaymentItemType.item_type_code == "ITEM121")
                    {
                        lblnResult = true;
                    }
                }
                return lblnResult;
            }

        }

		//PIR 23167, 23340, 23408
        public bool IsPaymentTypeCodeInsurancePremium
        {
            get
            {
                bool lblnResult = false;
                if (ibusPaymentItemType.IsNotNull())
                {
                    if (ibusPaymentItemType.icdoPaymentItemType.item_type_code == "ITEM76" ||
                        ibusPaymentItemType.icdoPaymentItemType.item_type_code == "ITEM77" ||
                        ibusPaymentItemType.icdoPaymentItemType.item_type_code == "ITEM78" ||
                        ibusPaymentItemType.icdoPaymentItemType.item_type_code == "ITEM79" ||
                        ibusPaymentItemType.icdoPaymentItemType.item_type_code == "ITEM120")
                        lblnResult = true;
                }
                return lblnResult;
            }
        }
    }
}
