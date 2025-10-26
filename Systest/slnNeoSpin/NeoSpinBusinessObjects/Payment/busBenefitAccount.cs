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
using NeoSpin.DataObjects;
using System.Linq;
using System.Linq.Expressions;

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busBenefitAccount : busBenefitAccountGen
    {
        public Collection<busPayeeAccount> iclbPayeeAccount { get; set; }

        public void LoadPayeeAccounts()
        {
            DataTable ldtPayeeAccounts = Select<cdoPayeeAccount>
                (new string[1] { enmPayeeAccount.benefit_account_id.ToString() },
                new object[1] { icdoBenefitAccount.benefit_account_id }, null, null);
            iclbPayeeAccount = new Collection<busPayeeAccount>();
            iclbPayeeAccount = GetCollection<busPayeeAccount>(ldtPayeeAccounts, "icdoPayeeAccount");
        }

        /// <summary>
        /// method to load PAY-4020 corrs bookmarks
        /// </summary>
        public void LoadPayeesAddress(DateTime adtPaymentDate)
        {
            int lintIndex = 0;
            decimal ldecPercentage = 0.00M, ldecPaidGrossAmount = 000M;
            istrPaymentDate = adtPaymentDate.ToString(busConstant.DateFormatLongDate);
            foreach (busPayeeAccount lobjPayeeAccount in iclbPayeeAccount)
            {
                if (lobjPayeeAccount.icdoPayeeAccount.account_relation_value != busConstant.AccountRelationshipMember)
                {
                    if ((lobjPayeeAccount.icdoPayeeAccount.payee_perslink_id > 0 && lobjPayeeAccount.ibusPayee == null) ||
                        (lobjPayeeAccount.icdoPayeeAccount.payee_org_id > 0 && lobjPayeeAccount.ibusRecipientOrganization == null))
                    {
                        lobjPayeeAccount.LoadPayee();
                    }
                    if (lobjPayeeAccount.icdoPayeeAccount.payee_perslink_id > 0 && lobjPayeeAccount.ibusPayee.ibusPersonCurrentAddress == null)
                    {
                        lobjPayeeAccount.ibusPayee.LoadPersonCurrentAddress();
                    }
                    else if (lobjPayeeAccount.icdoPayeeAccount.payee_org_id > 0 && lobjPayeeAccount.ibusRecipientOrganization.ibusOrgPrimaryAddress == null)
                    {
                        lobjPayeeAccount.ibusRecipientOrganization.LoadOrgPrimaryAddress();
                    }
                    if (lobjPayeeAccount.ibusBenefitCalculaton == null)
                        lobjPayeeAccount.LoadBenefitCalculation();
                    if (lobjPayeeAccount.ibusBenefitCalculaton.iclbBenefitCalculationPayee == null)
                        lobjPayeeAccount.ibusBenefitCalculaton.LoadBenefitCalculationPayee();
                    ldecPercentage = 0.00M;
                    if (lobjPayeeAccount.icdoPayeeAccount.payee_perslink_id > 0)
                    {
                        ldecPercentage = lobjPayeeAccount.ibusBenefitCalculaton.iclbBenefitCalculationPayee
                                                .Where(o => o.icdoBenefitCalculationPayee.payee_person_id == lobjPayeeAccount.icdoPayeeAccount.payee_perslink_id)
                                                .Select(o => o.icdoBenefitCalculationPayee.benefit_percentage).FirstOrDefault();
                    }
                    else if (lobjPayeeAccount.icdoPayeeAccount.payee_org_id > 0)
                    {
                        ldecPercentage = lobjPayeeAccount.ibusBenefitCalculaton.iclbBenefitCalculationPayee
                                                .Where(o => o.icdoBenefitCalculationPayee.payee_org_id == lobjPayeeAccount.icdoPayeeAccount.payee_org_id)
                                                .Select(o => o.icdoBenefitCalculationPayee.benefit_percentage).FirstOrDefault();
                    }

                    if (lintIndex == 0)
                    {
                        if (lobjPayeeAccount.icdoPayeeAccount.payee_perslink_id > 0)
                        {
                            istrPayee1 = lobjPayeeAccount.ibusPayee.icdoPerson.PayeeName.ToUpper();
                            istrPayeeAddress1 = lobjPayeeAccount.ibusPayee.ibusPersonCurrentAddress.addr_description.ToUpper();
                        }
                        else if (lobjPayeeAccount.icdoPayeeAccount.payee_org_id > 0)
                        {
                            istrPayee1 = lobjPayeeAccount.ibusRecipientOrganization.icdoOrganization.org_name_caps;
                            istrPayeeAddress1 = lobjPayeeAccount.ibusRecipientOrganization.ibusOrgPrimaryAddress.addr_description.ToUpper();
                        }
                        idecBeneficiaryPercentage1 = ldecPercentage;
                    }
                    if (lintIndex == 1)
                    {
                        if (lobjPayeeAccount.icdoPayeeAccount.payee_perslink_id > 0)
                        {
                            istrPayee2 = lobjPayeeAccount.ibusPayee.icdoPerson.PayeeName.ToUpper();
                            istrPayeeAddress2 = lobjPayeeAccount.ibusPayee.ibusPersonCurrentAddress.addr_description.ToUpper();
                        }
                        else if (lobjPayeeAccount.icdoPayeeAccount.payee_org_id > 0)
                        {
                            istrPayee2 = lobjPayeeAccount.ibusRecipientOrganization.icdoOrganization.org_name_caps;
                            istrPayeeAddress2 = lobjPayeeAccount.ibusRecipientOrganization.ibusOrgPrimaryAddress.addr_description.ToUpper();
                        }
                        istrPayee2exists = busConstant.Flag_Yes;
                        idecBeneficiaryPercentage2 = ldecPercentage;
                    }
                    if (lintIndex == 2)
                    {
                        if (lobjPayeeAccount.icdoPayeeAccount.payee_perslink_id > 0)
                        {
                            istrPayee3 = lobjPayeeAccount.ibusPayee.icdoPerson.PayeeName.ToUpper();
                            istrPayeeAddress3 = lobjPayeeAccount.ibusPayee.ibusPersonCurrentAddress.addr_description.ToUpper();
                        }
                        else if (lobjPayeeAccount.icdoPayeeAccount.payee_org_id > 0)
                        {
                            istrPayee3 = lobjPayeeAccount.ibusRecipientOrganization.icdoOrganization.org_name_caps;
                            istrPayeeAddress3 = lobjPayeeAccount.ibusRecipientOrganization.ibusOrgPrimaryAddress.addr_description.ToUpper();
                        }
                        istrPayee3exists = busConstant.Flag_Yes;
                        idecBeneficiaryPercentage3 = ldecPercentage;
                    }
                    if (lintIndex == 3)
                    {
                        if (lobjPayeeAccount.icdoPayeeAccount.payee_perslink_id > 0)
                        {
                            istrPayee4 = lobjPayeeAccount.ibusPayee.icdoPerson.PayeeName.ToUpper();
                            istrPayeeAddress4 = lobjPayeeAccount.ibusPayee.ibusPersonCurrentAddress.addr_description.ToUpper();
                        }
                        else if (lobjPayeeAccount.icdoPayeeAccount.payee_org_id > 0)
                        {
                            istrPayee4 = lobjPayeeAccount.ibusRecipientOrganization.icdoOrganization.org_name_caps;
                            istrPayeeAddress4 = lobjPayeeAccount.ibusRecipientOrganization.ibusOrgPrimaryAddress.addr_description.ToUpper();
                        }
                        istrPayee4exists = busConstant.Flag_Yes;
                        idecBeneficiaryPercentage4 = ldecPercentage;
                    }
                    if (lintIndex == 4)
                    {
                        if (lobjPayeeAccount.icdoPayeeAccount.payee_perslink_id > 0)
                        {
                            istrPayee5 = lobjPayeeAccount.ibusPayee.icdoPerson.PayeeName.ToUpper();
                            istrPayeeAddress5 = lobjPayeeAccount.ibusPayee.ibusPersonCurrentAddress.addr_description.ToUpper();
                        }
                        else if (lobjPayeeAccount.icdoPayeeAccount.payee_org_id > 0)
                        {
                            istrPayee5 = lobjPayeeAccount.ibusRecipientOrganization.icdoOrganization.org_name_caps;
                            istrPayeeAddress5 = lobjPayeeAccount.ibusRecipientOrganization.ibusOrgPrimaryAddress.addr_description.ToUpper();
                        }
                        istrPayee5exists = busConstant.Flag_Yes;
                        idecBeneficiaryPercentage5 = ldecPercentage;
                    }
                    lintIndex++;                    
                }
                else if (lobjPayeeAccount.icdoPayeeAccount.account_relation_value == busConstant.AccountRelationshipMember)
                {
                    lobjPayeeAccount.LoadPaymentDetails();
                    ldecPaidGrossAmount += lobjPayeeAccount.idecpaidgrossamount;
                }
            }
            idecValueAtDateOfDeath = icdoBenefitAccount.TotalAccountBalance >= ldecPaidGrossAmount ?
                icdoBenefitAccount.TotalAccountBalance - ldecPaidGrossAmount : 0.00M;
        }
    }
}
