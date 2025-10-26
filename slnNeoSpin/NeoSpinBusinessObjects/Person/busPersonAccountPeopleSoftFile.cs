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
    public class busPersonAccountPeopleSoftFile
    {
        private busPersonAccount _ibusPersonAccount;

        public busPersonAccount ibusPersonAccount
        {
            get { return _ibusPersonAccount; }
            set { _ibusPersonAccount = value; }
        }
        public busPersonAccountGhdv ibusPersonAccountGhdv { get; set; }
        public busPersonAccountLife ibusPersonAccountLife { get; set; }
        public busPersonAccountDeferredComp ibusPersonAccountDeferredComp { get; set; }

        private busOrganization _ibusProvider;

        public busOrganization ibusProvider
        {
            get { return _ibusProvider; }
            set { _ibusProvider = value; }
        }
        public string premium_waiver_flag { get; set; }
        public string istrHistoryPSFileChangeEventValue { get; set; }

        private string _org_group_value;

        public string org_group_value
        {
            get { return _org_group_value; }
            set { _org_group_value = value; }
        }
        private DateTime _plan_option_start_date;

        public DateTime plan_option_start_date
        {
            get { return _plan_option_start_date; }
            set { _plan_option_start_date = value; }
        }
        private DateTime _def_deduction_start_date;

        public DateTime def_deduction_start_date
        {
            get { return _def_deduction_start_date; }
            set { _def_deduction_start_date = value; }
        }
        private string _def_provider_type;

        public string def_provider_type
        {
            get { return _def_provider_type; }
            set { _def_provider_type = value; }
        }
        private string _level_of_coverage;

        public string level_of_coverage
        {
            get { return _level_of_coverage; }
            set { _level_of_coverage = value; }
        }

        private string _plan_type;

        public string plan_type
        {
            get { return _plan_type; }
            set { _plan_type = value; }
        }
        private string _benefit_plan;

        public string benefit_plan
        {
            get { return _benefit_plan; }
            set { _benefit_plan = value; }
        }
        private string _employee_id;

        public string employee_id
        {
            get { return _employee_id; }
            set { _employee_id = value; }
        }
        private string _employee_record_number;

        public string employee_record_number
        {
            get { return _employee_record_number; }
            set { _employee_record_number = value; }
        }
        private DateTime _coverage_begin_date;

        public DateTime coverage_begin_date
        {
            get { return _coverage_begin_date; }
            set { _coverage_begin_date = value; }
        }
        private DateTime _deduction_begin_date;

        public DateTime deduction_begin_date
        {
            get { return _deduction_begin_date; }
            set { _deduction_begin_date = value; }
        }
        private string _coverage_election;

        public string coverage_election
        {
            get { return _coverage_election; }
            set { _coverage_election = value; }
        }
        private string _coverage_code;

        public string coverage_code
        {
            get { return _coverage_code; }
            set { _coverage_code = value; }
        }
        private DateTime _election_date;

        public DateTime election_date
        {
            get { return _election_date; }
            set { _election_date = value; }
        }
        private decimal _flat_amount;

        public decimal flat_amount
        {
            get { return _flat_amount; }
            set { _flat_amount = value; }
        }
        private string _direct_deposit;

        public string direct_deposit
        {
            get { return _direct_deposit; }
            set { _direct_deposit = value; }
        }
        private string _inside_mail;

        public string inside_mail
        {
            get { return _inside_mail; }
            set { _inside_mail = value; }
        }
        private string _company;

        public string company
        {
            get { return _company; }
            set { _company = value; }
        }

        //PIR 16823 Issue 7
        private int _person_id;

        public int person_id
        {
            get { return _person_id; }
            set { _person_id = value; }
        }

        private string _calculation_routine;

        public string calculation_routine
        {
            get { return _calculation_routine; }
            set { _calculation_routine = value; }
        }
        public decimal loc_life_supplemental_coverage_amount { get; set; }
        public decimal loc_life_supplemental_premium_amount { get; set; }
        public decimal flex_premium_conversion_annualfledge_amount { get; set; }
        public decimal loc_life_spouse_supplemental_coverage_amount { get; set; }
        public decimal loc_life_dependent_supplemental_coverage_amount { get; set; }

        public string business_unit { get; set; }
        public bool iblnTerminatedRecord { get; set; }

        public bool IsPlanOptionSuspended { get; set; }

        public busOrganization ibusOrganization { get; set; }

        public void LoadOrganization(int AintOrgID)
        {
            if (ibusOrganization == null)
            {
                ibusOrganization = new busOrganization();
            }
            ibusOrganization.FindOrganization(AintOrgID);
        }
    }
}

