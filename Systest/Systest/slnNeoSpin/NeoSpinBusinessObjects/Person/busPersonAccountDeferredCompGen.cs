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
    public class busPersonAccountDeferredCompGen : busPersonAccount
    {
        public busPersonAccountDeferredCompGen()
        {

        }

        private cdoPersonAccountDeferredComp _icdoPersonAccountDeferredComp;
        public cdoPersonAccountDeferredComp icdoPersonAccountDeferredComp
        {
            get
            {
                return _icdoPersonAccountDeferredComp;
            }
            set
            {
                _icdoPersonAccountDeferredComp = value;
            }
        }

        public bool FindPersonAccountDeferredComp(int Aintpersonaccountid)
        {
            bool lblnResult = false;
            DataTable ldtbList = Select<cdoPersonAccountDeferredComp>(
                new string[1] { "person_account_id" },
                new object[1] { Aintpersonaccountid }, null, null);
            if (_icdoPersonAccountDeferredComp == null)
            {
                _icdoPersonAccountDeferredComp = new cdoPersonAccountDeferredComp();
            }
            if (ldtbList.Rows.Count == 1)
            {
                _icdoPersonAccountDeferredComp.LoadData(ldtbList.Rows[0]);
                lblnResult = true;
            }
            else if (ldtbList.Rows.Count > 1)
            {
                throw new Exception("FindPersonAccountDeferredComp method : Multiple records returned for given Person Account ID : " +
                    Aintpersonaccountid);
            };

            //Loading the Person Account too
            lblnResult = FindPersonAccount(Aintpersonaccountid);

            return lblnResult;
        }

        private Collection<busPersonAccountDeferredCompProvider> _icolPersonAccountDeferredCompProvider;
        public Collection<busPersonAccountDeferredCompProvider> icolPersonAccountDeferredCompProvider
        {
            get
            {
                return _icolPersonAccountDeferredCompProvider;
            }

            set
            {
                _icolPersonAccountDeferredCompProvider = value;
            }
        }

        public Collection<busPersonAccountDeferredCompProvider> iclbPADefCompProviders { get; set; }

        public void LoadPersonAccountProviders()
        {
            DataTable ldtbList = Select<cdoPersonAccountDeferredCompProvider>(
                new string[1] { "person_account_id" },
                new object[1] { icdoPersonAccountDeferredComp.person_account_id }, null, null);
            _icolPersonAccountDeferredCompProvider = GetCollection<busPersonAccountDeferredCompProvider>(ldtbList, "icdoPersonAccountDeferredCompProvider");
        }

        //PIR 9692
        public string istrProviderName { get; set; }
        public string istrEmployerName { get; set; }
        public DateTime idateStartDate { get; set; }
        public DateTime idateEndDate { get; set; }
        public decimal idecAmtPerPayPeriod { get; set; }
        public string istrReportFrequency { get; set; }
        public string istrIsUpdateExistingProvider { get; set; }
        public void LoadSpecificWssPersonAccountProvidersForAck(int aintWssRequestId)
        {
            DataTable ldtbDefCompProviderForAck = Select("cdoWssAcknowledgement.DefCompProviderForAck", new object[1] { aintWssRequestId });
            if (ldtbDefCompProviderForAck.Rows.Count > 0)
            {
                istrProviderName = ldtbDefCompProviderForAck.Rows[0]["ProviderName"].ToString();
                istrEmployerName = ldtbDefCompProviderForAck.Rows[0]["EmployerName"].ToString();
                if (ldtbDefCompProviderForAck.Rows[0]["StartDate"] != DBNull.Value)
                    idateStartDate = Convert.ToDateTime(ldtbDefCompProviderForAck.Rows[0]["StartDate"]);
                if (ldtbDefCompProviderForAck.Rows[0]["EndDate"] != DBNull.Value)
                    idateEndDate = Convert.ToDateTime(ldtbDefCompProviderForAck.Rows[0]["EndDate"]);
                if (ldtbDefCompProviderForAck.Rows[0]["AmtPerPayPeriod"] != DBNull.Value)
                    idecAmtPerPayPeriod = Convert.ToDecimal(ldtbDefCompProviderForAck.Rows[0]["AmtPerPayPeriod"]);
                string istrReportFrequencyValue = ldtbDefCompProviderForAck.Rows[0]["ReportFrequency"].ToString();
                switch (istrReportFrequencyValue)
                {
                    case ("BWLK"):
                        istrReportFrequency = "Bi-Weekly";
                        break;
                    case ("MONT"):
                        istrReportFrequency = "Monthly";
                        break;
                    case ("SEMI"):
                        istrReportFrequency = "Semi-Monthly";
                        break;
                    case ("WKLY"):
                        istrReportFrequency = "Weekly";
                        break;
                }
                string istrUpdateExistingProviderValue = ldtbDefCompProviderForAck.Rows[0]["IsUpdateExistingProvider"].ToString();
                if (istrUpdateExistingProviderValue == busConstant.Flag_Yes)
                    istrIsUpdateExistingProvider = busConstant.Flag_Yes_Value;
                else if (istrUpdateExistingProviderValue == busConstant.Flag_No)
                    istrIsUpdateExistingProvider = busConstant.Flag_No_Value;
                else
                    istrIsUpdateExistingProvider = string.Empty;

            }
        }

        public void LoadActivePersonAccountProviders()
        {
            _icolPersonAccountDeferredCompProvider = new Collection<busPersonAccountDeferredCompProvider>();
            DataTable ldtbList = Select<cdoPersonAccountDeferredCompProvider>(
                new string[1] { "person_account_id" },
                new object[1] { icdoPersonAccountDeferredComp.person_account_id }, null, null);
            Collection<busPersonAccountDeferredCompProvider> lclbPADCProvider = GetCollection<busPersonAccountDeferredCompProvider>(ldtbList, "icdoPersonAccountDeferredCompProvider");
            foreach (busPersonAccountDeferredCompProvider lbusPersonAccountDeferredCompProvider in lclbPADCProvider)
            {
                if (lbusPersonAccountDeferredCompProvider.icdoPersonAccountDeferredCompProvider.end_date_no_null > DateTime.Now
                    && lbusPersonAccountDeferredCompProvider.icdoPersonAccountDeferredCompProvider.start_date != lbusPersonAccountDeferredCompProvider.icdoPersonAccountDeferredCompProvider.end_date) //PIR 24581
                {
                    _icolPersonAccountDeferredCompProvider.Add(lbusPersonAccountDeferredCompProvider);
                }
            }
        }

        public void LoadPersonAccountProviderInfo()
        {
            if (_icolPersonAccountDeferredCompProvider == null)
            {
                LoadPersonAccountProviders();
            }
            foreach (busPersonAccountDeferredCompProvider lobjProvider in _icolPersonAccountDeferredCompProvider)
            {
                if (lobjProvider.ibusProviderOrgPlan == null)
                    lobjProvider.LoadProviderOrgPlan();
                if (lobjProvider.ibusProviderOrgPlan.ibusOrganization == null)
                    lobjProvider.ibusProviderOrgPlan.LoadOrganization();
                lobjProvider.icdoPersonAccountDeferredCompProvider.istrProviderOrgCode = lobjProvider.ibusProviderOrgPlan.ibusOrganization.icdoOrganization.org_code;
                lobjProvider.ibusProviderOrganization = lobjProvider.ibusProviderOrgPlan.ibusOrganization;

                if (lobjProvider.ibusPersonEmployment == null)
                    lobjProvider.LoadPersonEmployment();

                if (lobjProvider.ibusPersonEmployment.ibusOrganization == null)
                    lobjProvider.ibusPersonEmployment.LoadOrganization();

                if(lobjProvider.ibusPersonAccountDeferredComp == null)
                    lobjProvider.LoadPersonAccountDeferredComp();

                if (lobjProvider.ibusPersonAccountDeferredComp.ibusOrgPlan == null)
                    lobjProvider.ibusPersonAccountDeferredComp.LoadOrgPlan(
                        lobjProvider.icdoPersonAccountDeferredCompProvider.start_date, //PROD PIR ID 4737
                        lobjProvider.ibusPersonEmployment.icdoPersonEmployment.org_id);
            }
        }

        private Collection<busPersonAccountDeferredCompHistory> _icolPADeferredCompHistory;
        public Collection<busPersonAccountDeferredCompHistory> icolPADeferredCompHistory
        {
            get
            {
                return _icolPADeferredCompHistory;
            }

            set
            {
                _icolPADeferredCompHistory = value;
            }
        }
        private Collection<busPersonAccountDeferredCompHistory> _icolDeffCompHistoryToCompare;
        public Collection<busPersonAccountDeferredCompHistory> icolDeffCompHistoryToCompare
        {
            get
            {
                return _icolDeffCompHistoryToCompare;
            }

            set
            {
                _icolDeffCompHistoryToCompare = value;
            }
        }
        public void LoadPADeffCompHistory()
        {
            DataTable ldtbList = Select<cdoPersonAccountDeferredCompHistory>(
                new string[1] { "person_account_id" },
                new object[1] { icdoPersonAccountDeferredComp.person_account_id }, null, "PERSON_ACCOUNT_DEFERRED_COMP_HISTORY_ID DESC");
            _icolPADeferredCompHistory = GetCollection<busPersonAccountDeferredCompHistory>(ldtbList, "icdoPersonAccountDeferredCompHistory");
            foreach (busPersonAccountDeferredCompHistory lobjPADeffCompHistory in _icolPADeferredCompHistory)
            {
                lobjPADeffCompHistory.LoadPersonAccount();
                lobjPADeffCompHistory.LoadPlan();
            }
        }

        //UCS-041 
        public Collection<busPersonAccountDeferredCompTransfer> iclbPersonAccountDeferredCompTransfer { get; set; }

        public Collection<busPersonAccountDeferredCompProvider> iclbDefCompOpenProviders { get; set; }

        public Collection<busPersonAccountDeferredCompProvider> iclbDefCompClosedProviders { get; set; }

        public void LoadMSSDefCompProviders()
        {
            iclbDefCompOpenProviders = new Collection<busPersonAccountDeferredCompProvider>();
            iclbDefCompClosedProviders = new Collection<busPersonAccountDeferredCompProvider>();

            if (icolPersonAccountDeferredCompProvider.IsNull())
                LoadPersonAccountProviders();

            foreach (busPersonAccountDeferredCompProvider lobjDefCompProvider in icolPersonAccountDeferredCompProvider)
            {
                lobjDefCompProvider.iblnIsFromPortal = true;
                lobjDefCompProvider.icdoPersonAccountDeferredCompProvider.suppress_warning_flag = busConstant.Flag_No;
                if (lobjDefCompProvider.icdoPersonAccountDeferredCompProvider.end_date_no_null > DateTime.Today)
                {
                    lobjDefCompProvider.LoadProviderAgentOrgContact();
                    lobjDefCompProvider.ibusProviderAgentOrgContact.LoadContact();
                    lobjDefCompProvider.istrIsPopupValidation = busConstant.Flag_Yes;
                    iclbDefCompOpenProviders.Add(lobjDefCompProvider);
                }
				//PIR 25920 New Plan DC 2025 hide the def comp provider history 
                //else
                //    iclbDefCompClosedProviders.Add(lobjDefCompProvider);
            }
        }
    }
}
