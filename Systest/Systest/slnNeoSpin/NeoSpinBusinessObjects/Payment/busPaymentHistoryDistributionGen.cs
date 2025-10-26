#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using NeoSpin.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busPaymentHistoryDistributionGen : busExtendBase
    {
        public busPaymentHistoryDistributionGen()
        {

        }
        public busPaymentHistoryDistributionStatusHistory ibusDistributionHistory { get; set; }
        public cdoPaymentHistoryDistribution icdoPaymentHistoryDistribution { get; set; }
        
        public virtual bool FindPaymentHistoryDistribution(int Aintpaymenthistorydistributionid)
        {
            bool lblnResult = false;
            if (icdoPaymentHistoryDistribution == null)
            {
                icdoPaymentHistoryDistribution = new cdoPaymentHistoryDistribution();
            }
            if (icdoPaymentHistoryDistribution.SelectRow(new object[1] { Aintpaymenthistorydistributionid }))
            {
                lblnResult = true;
            }
            return lblnResult;
        }
        //Property to load person
        public busPerson ibusPerson { get; set; }
        //property to load organization
        public busOrganization ibusOrganization { get; set; }

        //Load org details
        public void LoadOrganization()
        {
            if (ibusOrganization == null)
                ibusOrganization = new busOrganization();
            ibusOrganization.FindOrganization(icdoPaymentHistoryDistribution.org_id);
        }
        //load person details
        public void LoadPerson()
        {
            if (ibusPerson == null)
                ibusPerson = new busPerson();
            ibusPerson.FindPerson(icdoPaymentHistoryDistribution.person_id);
        }

        //Property to Load Payment History Header
        public busPaymentHistoryHeader ibusPaymentHistoryHeader { get; set; }
        //Load Payment History Details
        public void LoadPaymentHistoryHeader()
        {
            if (ibusPaymentHistoryHeader == null)
                ibusPaymentHistoryHeader = new busPaymentHistoryHeader();
            ibusPaymentHistoryHeader.FindPaymentHistoryHeader(icdoPaymentHistoryDistribution.payment_history_header_id);
        }
        //Load Payment History details
        public void LoadPaymentHistoryDetails()
        {
            if (ibusPaymentHistoryHeader == null)
                LoadPaymentHistoryHeader();
            if (ibusPaymentHistoryHeader.ibusPlan == null)
                ibusPaymentHistoryHeader.LoadPlan();
            if (ibusPaymentHistoryHeader.ibusPaymentSchedule == null)
                ibusPaymentHistoryHeader.LoadPaymentSchedule();
            if (ibusPaymentHistoryHeader.ibusPayeeAccount == null)
                ibusPaymentHistoryHeader.LoadPayeeAccount();
            ibusPaymentHistoryHeader.CalculateAmounts();
        }
        //this property to load all the status change history
        public Collection<busPaymentHistoryDistributionStatusHistory> iclbDistributionHistory { get; set; }
        //Load Distribution history
        public void LoadDistributionHistory()
        {
            DataTable ldtbHistory = Select<cdoPaymentHistoryDistributionStatusHistory>(new string[1] { "payment_history_distribution_id" },
                new object[1] { icdoPaymentHistoryDistribution.payment_history_distribution_id }, null, "distribution_status_history_id desc");
            iclbDistributionHistory = GetCollection<busPaymentHistoryDistributionStatusHistory>(ldtbHistory, "icdoPaymentHistoryDistributionStatusHistory");
        }
        //Load Latest payment distribution history
        public void LoadLatestDistibutionStatusHistory()
        {
            if(iclbDistributionHistory==null)
                LoadDistributionHistory();
            ibusDistributionHistory = new busPaymentHistoryDistributionStatusHistory { icdoPaymentHistoryDistributionStatusHistory = new cdoPaymentHistoryDistributionStatusHistory() };
            if (iclbDistributionHistory.Count > 0)
            {
                ibusDistributionHistory = iclbDistributionHistory[0];
            }
        }
        //Load Actual Status
        public void LoadStatus()
        {
            DataTable ldtbstatus = iobjPassInfo.isrvDBCache.GetCodeDescription(2505, icdoPaymentHistoryDistribution.distribution_status_value);
            icdoPaymentHistoryDistribution.status_value = ldtbstatus.Rows.Count > 0 ? ldtbstatus.Rows[0]["data2"].ToString() : string.Empty;
            icdoPaymentHistoryDistribution.status_description = ldtbstatus.Rows.Count > 0 ? ldtbstatus.Rows[0]["description"].ToString() : string.Empty;
        }
    }    
}