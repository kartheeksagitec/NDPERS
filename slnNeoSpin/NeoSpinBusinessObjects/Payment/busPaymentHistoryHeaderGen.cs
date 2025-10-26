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
#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busPaymentHistoryHeaderGen : busExtendBase
    {
        public busPaymentHistoryHeaderGen()
        {

        }
        public cdoPaymentHistoryHeader icdoPaymentHistoryHeader { get; set; }
       
        public bool IsDistributionStatusChangedUserAndCurrentUserSame()
        {

            string lstrStatusChangeBy = string.Empty;            
            if (iclbPaymentHistoryDistribution == null)
                LoadPaymentHistoryDistribution();
            foreach (busPaymentHistoryDistribution lobjDistribution in iclbPaymentHistoryDistribution)
            {
                lobjDistribution.LoadLatestDistibutionStatusHistory();
                lobjDistribution.LoadStatus();
                if(lobjDistribution.ibusDistributionHistory.icdoPaymentHistoryDistributionStatusHistory.status_changed_by==iobjPassInfo.istrUserID)
                {
                    return true;
                }
            }
            return false;
        }
        public virtual bool FindPaymentHistoryHeader(int Aintpaymenthistoryid)
        {
            bool lblnResult = false;
            if (icdoPaymentHistoryHeader == null)
            {
                icdoPaymentHistoryHeader = new cdoPaymentHistoryHeader();
            }
            if (icdoPaymentHistoryHeader.SelectRow(new object[1] { Aintpaymenthistoryid }))
            {
                lblnResult = true;
            }
            return lblnResult;
        }
        //property to load the payee account  details
        public busPayeeAccount ibusPayeeAccount { get; set; }

        //property to load the person details
        public busPerson ibusPerson { get; set; }

        //property to load the organization details
        public busOrganization ibusOrganization { get; set; }

        //property to load the plan  details
        public busPlan ibusPlan { get; set; }
        //property to load payment schedule
        public busPaymentSchedule ibusPaymentSchedule { get; set; }

        //Load payment schedule
        public void LoadPaymentSchedule()
        {
            if (ibusPaymentSchedule == null)
                ibusPaymentSchedule = new busPaymentSchedule();
            ibusPaymentSchedule.FindPaymentSchedule(icdoPaymentHistoryHeader.payment_schedule_id);
        }

        //Load Payee account details
        public void LoadPayeeAccount()
        {
            if (ibusPayeeAccount == null)
                ibusPayeeAccount = new busPayeeAccount();
            ibusPayeeAccount.FindPayeeAccount(icdoPaymentHistoryHeader.payee_account_id);
        }
        //Load Person details
        public void LoadPerson()
        {
            if (ibusPerson == null)
                ibusPerson = new busPerson();
            ibusPerson.FindPerson(icdoPaymentHistoryHeader.person_id);
        }
        //Load plan details
        public void LoadPlan()
        {
            if (ibusPlan == null)
                ibusPlan = new busPlan();
            ibusPlan.FindPlan(icdoPaymentHistoryHeader.plan_id);
        }
        //Load organization details
        public void LoadOrganization()
        {
            if (ibusOrganization == null)
                ibusOrganization = new busOrganization();
            ibusOrganization.FindOrganization(icdoPaymentHistoryHeader.org_id);
        }
        //property to load the Payment History  details
        public Collection<busPaymentHistoryDetail> iclbPaymentHistoryDetail { get; set; }

        //this method loads the Payment History details for payment history header
        public void LoadPaymentHistoryDetails()
        {
            iclbPaymentHistoryDetail = new Collection<busPaymentHistoryDetail>();
            DataTable ldtbHistoryDetail = Select("cdoPaymentHistoryHeader.LoadPaymentHistoryDetails",
                new object[1] { icdoPaymentHistoryHeader.payment_history_header_id });
            foreach (DataRow drStep in ldtbHistoryDetail.Rows)
            {
                busPaymentHistoryDetail lobjPaymentHistoryDetail = new busPaymentHistoryDetail { icdoPaymentHistoryDetail = new cdoPaymentHistoryDetail() };
                lobjPaymentHistoryDetail.ibusPaymentItemType = new busPaymentItemType { icdoPaymentItemType = new cdoPaymentItemType() };
                lobjPaymentHistoryDetail.icdoPaymentHistoryDetail.LoadData(drStep);
                lobjPaymentHistoryDetail.ibusPaymentItemType.icdoPaymentItemType.LoadData(drStep);
                lobjPaymentHistoryDetail.icdoPaymentHistoryDetail.amount = lobjPaymentHistoryDetail.icdoPaymentHistoryDetail.amount *
                    lobjPaymentHistoryDetail.ibusPaymentItemType.icdoPaymentItemType.item_type_direction;
                iclbPaymentHistoryDetail.Add(lobjPaymentHistoryDetail);
            }
        }

        //this method loads the Payment History details for payment history header
        public void LoadPaymentHistoryDetailsUnsigned()
        {
            iclbPaymentHistoryDetail = new Collection<busPaymentHistoryDetail>();
            DataTable ldtbHistoryDetail = Select("cdoPaymentHistoryHeader.LoadPaymentHistoryDetails",
                new object[1] { icdoPaymentHistoryHeader.payment_history_header_id });
            foreach (DataRow drStep in ldtbHistoryDetail.Rows)
            {
                busPaymentHistoryDetail lobjPaymentHistoryDetail = new busPaymentHistoryDetail { icdoPaymentHistoryDetail = new cdoPaymentHistoryDetail() };
                lobjPaymentHistoryDetail.ibusPaymentItemType = new busPaymentItemType { icdoPaymentItemType = new cdoPaymentItemType() };
                lobjPaymentHistoryDetail.icdoPaymentHistoryDetail.LoadData(drStep);
                lobjPaymentHistoryDetail.ibusPaymentItemType.icdoPaymentItemType.LoadData(drStep);                
                iclbPaymentHistoryDetail.Add(lobjPaymentHistoryDetail);
            }
        }

        //property to load the Payment History Distribution details
        public Collection<busPaymentHistoryDistribution> iclbPaymentHistoryDistribution { get; set; }

        //this method loads the Payment History Distribution details for payment history header
        public void LoadPaymentHistoryDistribution()
        {
            DataTable ldtbList = Select<cdoPaymentHistoryDistribution>(
               new string[1] { "payment_history_header_id" },
               new object[1] { icdoPaymentHistoryHeader.payment_history_header_id }, null, null);
            iclbPaymentHistoryDistribution = GetCollection<busPaymentHistoryDistribution>(ldtbList, "icdoPaymentHistoryDistribution");
            foreach (busPaymentHistoryDistribution lbusPaymentHistoryDistribution in iclbPaymentHistoryDistribution)
            {
                lbusPaymentHistoryDistribution.LoadOrganization();
                lbusPaymentHistoryDistribution.LoadStatus();
            }
        }

        // Calculte Gross amount ,deduction amount,net amount ,taxable amount and non taxable amount from payment history details
        public void CalculateAmounts()
        {
            if (iclbPaymentHistoryDetail == null)
                LoadPaymentHistoryDetails();
            //to get the grosss amount and deduction amount, group payment history details by chect group code in the payment item type table
            var AmountsByGroupCode = from lobjPaymentHistoryDetail in iclbPaymentHistoryDetail
                                     group lobjPaymentHistoryDetail by lobjPaymentHistoryDetail.ibusPaymentItemType.icdoPaymentItemType.check_group_code_value into HistoryDetailByGroupCode
                                     select new
                                     {
                                         lstrGroupCode = HistoryDetailByGroupCode.Key,
                                         ldecAmount = HistoryDetailByGroupCode.Sum(lobjPaymentHistoryDetail => lobjPaymentHistoryDetail.icdoPaymentHistoryDetail.amount)
                                     };
            //Loop through the gouped components and get the amounts
            Array.ForEach(AmountsByGroupCode.ToArray(), o =>
            {
                if (o.lstrGroupCode == busConstant.CheckComponentGroupDeductions)
                {
                    icdoPaymentHistoryHeader.deduction_amount = o.ldecAmount;
                } 
            });
            if (icdoPaymentHistoryHeader.person_id > 0)
            {
                icdoPaymentHistoryHeader.gross_amount = iclbPaymentHistoryDetail.Where(o =>
                   o.ibusPaymentItemType.icdoPaymentItemType.item_type_direction == 1 &&
                   o.ibusPaymentItemType.icdoPaymentItemType.allow_rollover_code_value != busConstant.RollItemForCheck).Select(o => o.icdoPaymentHistoryDetail.amount).Sum();
            }
            else
            {
                icdoPaymentHistoryHeader.gross_amount = iclbPaymentHistoryDetail.Where(o =>
                  o.ibusPaymentItemType.icdoPaymentItemType.item_type_direction == 1 &&
                  o.ibusPaymentItemType.icdoPaymentItemType.allow_rollover_code_value == busConstant.RollItemForCheck).Select(o => o.icdoPaymentHistoryDetail.amount).Sum();
            }

            //To get the taxable amount , sum the amount from payment hisory detail if the taxable flag is yes.
            icdoPaymentHistoryHeader.taxable_amount = iclbPaymentHistoryDetail.Where(o =>
                o.ibusPaymentItemType.icdoPaymentItemType.taxable_item_flag == busConstant.Flag_Yes &&
                o.ibusPaymentItemType.icdoPaymentItemType.allow_rollover_code_value != busConstant.RollItemForCheck
                ).Select(o => o.icdoPaymentHistoryDetail.amount).Sum();

            //To get the non taxable amount , sum the amount from payment hisory detail if the taxable flag is no and chect group code is not deductions.

            icdoPaymentHistoryHeader.NonTaxable_Amount = iclbPaymentHistoryDetail.Where(o =>
                o.ibusPaymentItemType.icdoPaymentItemType.taxable_item_flag == busConstant.Flag_No
                && o.ibusPaymentItemType.icdoPaymentItemType.check_group_code_value != busConstant.CheckComponentGroupDeductions).Select(o => o.icdoPaymentHistoryDetail.amount).Sum();

            //To get the net amount , sum the amount from payment hisory detail if the taxable flag is no and chect group code is not deductions.
            icdoPaymentHistoryHeader.net_amount = iclbPaymentHistoryDetail.Sum(o =>
                o.icdoPaymentHistoryDetail.amount);
        }
       
    }
}