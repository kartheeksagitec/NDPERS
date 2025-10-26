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
using System.Collections.Generic;
using Sagitec.CustomDataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
    public partial class busOrgPlan : busExtendBase
    {
        private busOrganization _ibusOrganization;
        public busOrganization ibusOrganization
        {
            get { return _ibusOrganization; }
            set { _ibusOrganization = value; }
        }
        /// <summary>
        /// To load Organization's Information.
        /// </summary>
        public void LoadOrganization()
        {
            if (_ibusOrganization == null)
            {
                _ibusOrganization = new busOrganization();
            }
            _ibusOrganization.FindOrganization(_icdoOrgPlan.org_id);
        }
        public string HealthInsuranceType
        {
            get
            {
                if (ibusOrganization.IsNull())
                    LoadOrganization();
                if ((ibusOrganization.icdoOrganization.emp_category_value == busConstant.EmployerCategoryState) ||
                    (ibusOrganization.icdoOrganization.emp_category_value == busConstant.EmployerCategoryDistrictHealthUnits))
                {
                    return busConstant.HealthInsuranceTypeState;
                }
                else
                {
                    return busConstant.HealthInsuranceTypeNonState;
                }
            }
        }

        # region these fields are added for generating monthly Employer Statement Cor
        public decimal idecBeginningBalanceAsMonthlyDue { get; set; }
        private decimal _idecNewBeginingBalanceAmount;
        public decimal idecNewBeginingBalanceAmount
        {
            get { return _idecNewBeginingBalanceAmount; }
            set { _idecNewBeginingBalanceAmount = value; }
        }

        private decimal _idecNewRemittanceAmount;
        public decimal idecNewRemittanceAmount
        {
            get { return _idecNewRemittanceAmount; }
            set { _idecNewRemittanceAmount = value; }
        }

        private decimal _idecNewMonthDueAmount;
        public decimal idecNewMonthDueAmount
        {
            get { return _idecNewMonthDueAmount; }
            set { _idecNewMonthDueAmount = value; }
        }
        private DateTime _idtStartDate;
        public DateTime idtStartDate
        {
            get { return _idtStartDate; }
            set { _idtStartDate = value; }
        }

        private DateTime _idtEndDate;
        public DateTime idtEndDate
        {
            get { return _idtEndDate; }
            set { _idtEndDate = value; }
        }
        private decimal _idecInvoiceAmount;

        public decimal idecInvoiceAmount
        {
            get { return _idecInvoiceAmount; }
            set { _idecInvoiceAmount = value; }
        }


        # endregion

        private Collection<busOrgPlan> _iclbOrgPlan;
        public Collection<busOrgPlan> iclbOrgPlan
        {
            get { return _iclbOrgPlan; }
            set { _iclbOrgPlan = value; }
        }
        private Collection<busOrgPlanProvider> _iclbOrgPlanProvider;
        public Collection<busOrgPlanProvider> iclbOrgPlanProvider
        {
            get { return _iclbOrgPlanProvider; }
            set { _iclbOrgPlanProvider = value; }
        }
        private Collection<busOrgPlanDentalRate> _iclbOrgPlanDentalRate;
        public Collection<busOrgPlanDentalRate> iclbOrgPlanDentalRate
        {
            get { return _iclbOrgPlanDentalRate; }
            set { _iclbOrgPlanDentalRate = value; }
        }
        private Collection<busOrgPlanDentalRate> _iclbEmployerDentalRate;
        public Collection<busOrgPlanDentalRate> iclbEmployerDentalRate
        {
            get { return _iclbEmployerDentalRate; }
            set { _iclbEmployerDentalRate = value; }
        }
        private Collection<busOrgPlanEapRate> _iclbOrgPlanEapRate;
        public Collection<busOrgPlanEapRate> iclbOrgPlanEapRate
        {
            get { return _iclbOrgPlanEapRate; }
            set { _iclbOrgPlanEapRate = value; }
        }
        private Collection<busOrgPlanEapRate> _iclbOrgPlanEmplEapRate;
        public Collection<busOrgPlanEapRate> iclbOrgPlanEmplEapRate
        {
            get { return _iclbOrgPlanEmplEapRate; }
            set { _iclbOrgPlanEmplEapRate = value; }
        }
        private Collection<busOrgPlanVisionRate> _iclbOrgPlanVisionRate;
        public Collection<busOrgPlanVisionRate> iclbOrgPlanVisionRate
        {
            get { return _iclbOrgPlanVisionRate; }
            set { _iclbOrgPlanVisionRate = value; }
        }
        private Collection<busOrgPlanVisionRate> _iclbEmployerVisionRate;
        public Collection<busOrgPlanVisionRate> iclbEmployerVisionRate
        {
            get { return _iclbEmployerVisionRate; }
            set { _iclbEmployerVisionRate = value; }
        }
        private Collection<busOrgPlanHmoRate> _iclbOrgPlanHmoRate;
        public Collection<busOrgPlanHmoRate> iclbOrgPlanHmoRate
        {
            get { return _iclbOrgPlanHmoRate; }
            set { _iclbOrgPlanHmoRate = value; }
        }
        private Collection<busOrgPlanHmoRate> _iclbEmployerHmoRate;
        public Collection<busOrgPlanHmoRate> iclbEmployerHmoRate
        {
            get { return _iclbEmployerHmoRate; }
            set { _iclbEmployerHmoRate = value; }
        }
        private Collection<busOrgPlanMemberType> _iclbOrgPlanMemberType;
        public Collection<busOrgPlanMemberType> iclbOrgPlanMemberType
        {
            get { return _iclbOrgPlanMemberType; }
            set { _iclbOrgPlanMemberType = value; }
        }
        private Collection<busOrgPlanLtcRate> _iclbOrgPlanLtcRate;
        public Collection<busOrgPlanLtcRate> iclbOrgPlanLtcRate
        {
            get { return _iclbOrgPlanLtcRate; }
            set { _iclbOrgPlanLtcRate = value; }
        }
        private Collection<busOrgPlanLtcRate> _iclbEmployerLtcRate;
        public Collection<busOrgPlanLtcRate> iclbEmployerLtcRate
        {
            get { return _iclbEmployerLtcRate; }
            set { _iclbEmployerLtcRate = value; }
        }
        private Collection<busOrgPlanLifeRate> _iclbOrgPlanLifeRate;
        public Collection<busOrgPlanLifeRate> iclbOrgPlanLifeRate
        {
            get { return _iclbOrgPlanLifeRate; }
            set { _iclbOrgPlanLifeRate = value; }
        }
        private Collection<busOrgPlanLifeRate> _iclbEmployerLifeRate;
        public Collection<busOrgPlanLifeRate> iclbEmployerLifeRate
        {
            get { return _iclbEmployerLifeRate; }
            set { _iclbEmployerLifeRate = value; }
        }
        private Collection<busOrgPlanHealthMedicarePartDRate> _iclbOrgPlanHealthRate;
        public Collection<busOrgPlanHealthMedicarePartDRate> iclbOrgPlanHealthRate
        {
            get { return _iclbOrgPlanHealthRate; }
            set { _iclbOrgPlanHealthRate = value; }
        }
        private Collection<busOrgPlanHealthMedicarePartDRate> _iclbEmployerHealthRate;
        public Collection<busOrgPlanHealthMedicarePartDRate> iclbEmployerHealthRate
        {
            get { return _iclbEmployerHealthRate; }
            set { _iclbEmployerHealthRate = value; }
        }
        private Collection<busOrgPlanHealthMedicarePartDRate> _iclbOrgPlanMedicarePartDRate;
        public Collection<busOrgPlanHealthMedicarePartDRate> iclbOrgPlanMedicarePartDRate
        {
            get { return _iclbOrgPlanMedicarePartDRate; }
            set { _iclbOrgPlanMedicarePartDRate = value; }
        }
        private Collection<busOrgPlanHealthMedicarePartDRate> _iclbEmployerMedicarePartDRate;
        public Collection<busOrgPlanHealthMedicarePartDRate> iclbEmployerMedicarePartDRate
        {
            get { return _iclbEmployerMedicarePartDRate; }
            set { _iclbEmployerMedicarePartDRate = value; }
        }
        private Collection<busPlanRetirementRate> _iclbPlanRetirementRate;
        public Collection<busPlanRetirementRate> iclbPlanRetirementRate
        {
            get { return _iclbPlanRetirementRate; }
            set { _iclbPlanRetirementRate = value; }
        }
        private Collection<busPlanRetirementRate> _iclbPlanRetirementRateHistory;
        public Collection<busPlanRetirementRate> iclbPlanRetirementRateHistory
        {
            get { return _iclbPlanRetirementRateHistory; }
            set { _iclbPlanRetirementRateHistory = value; }
        }
        private Collection<busOrgPlanProvider> _iclbRateProviderForEmployer;
        public Collection<busOrgPlanProvider> iclbRateProviderForEmployer
        {
            get { return _iclbRateProviderForEmployer; }
            set { _iclbRateProviderForEmployer = value; }
        }
        public void LoadDentalRates()
        {
            DataTable ldtbList = Select<cdoOrgPlanDentalRate>(
                  new string[1] { "org_plan_id" },
                  new object[1] { _icdoOrgPlan.org_plan_id }, null, null);
            _iclbOrgPlanDentalRate = GetCollection<busOrgPlanDentalRate>(ldtbList, "icdoOrgPlanDentalRate");
        }
        public void LoadRateProvidersForEmployer()
        {
            DataTable ldtbListRateProviders = Select("cdoOrgPlan.LoadRateProviders",
                       new object[1] { _icdoOrgPlan.org_plan_id });
            _iclbRateProviderForEmployer = GetCollection<busOrgPlanProvider>(ldtbListRateProviders, "icdoOrgPlanProvider");
        }
        public void LoadEmployerDentalRates()
        {
            LoadRateProvidersForEmployer();
            if (_iclbRateProviderForEmployer.Count > 0)
            {
                _iclbEmployerDentalRate = new Collection<busOrgPlanDentalRate>();
                foreach (busOrgPlanProvider lobjRateProvider in _iclbRateProviderForEmployer)
                {
                    DataTable ldtbList = Select("cdoOrgPlan.GetOrgPlanAndEffectiveDateForDentalRates",
                        new object[2] { lobjRateProvider.icdoOrgPlanProvider.provider_org_id, _icdoOrgPlan.plan_id });
                    if (ldtbList.Rows.Count > 0)
                    {
                        int lintOrgPlanID = Convert.ToInt32(ldtbList.Rows[0]["org_plan_id"]);
                        DateTime ldtEffectiveDate = Convert.ToDateTime(ldtbList.Rows[0]["effective_date"]);
                        DataTable ldtbListEmpDentalRate = Select("cdoOrgPlan.LoadDentalRatesForEmployer", new object[2] { lintOrgPlanID, ldtEffectiveDate });
                        foreach (DataRow dr in ldtbListEmpDentalRate.Rows)
                        {
                            busOrgPlanDentalRate lobjOrgPlanDentalRate = new busOrgPlanDentalRate();
                            lobjOrgPlanDentalRate.icdoOrgPlanDentalRate = new cdoOrgPlanDentalRate();
                            lobjOrgPlanDentalRate.iintProviderID = lobjRateProvider.icdoOrgPlanProvider.provider_org_id;
                            sqlFunction.LoadQueryResult(lobjOrgPlanDentalRate.icdoOrgPlanDentalRate, dr);
                            _iclbEmployerDentalRate.Add(lobjOrgPlanDentalRate);
                        }
                    }
                    foreach (busOrgPlanDentalRate lobjOrgPlanDentalRate in _iclbEmployerDentalRate)
                    {
                        lobjOrgPlanDentalRate.LoadProvider(lobjOrgPlanDentalRate.iintProviderID);
                    }
                }
            }
        }

        public void LoadEapRates()
        {
            DataTable ldtbList = Select<cdoOrgPlanEapRate>(
                  new string[1] { "org_plan_id" },
                  new object[1] { _icdoOrgPlan.org_plan_id }, null, null);
            _iclbOrgPlanEapRate = GetCollection<busOrgPlanEapRate>(ldtbList, "icdoOrgPlanEapRate");
        }

        public void LoadEmployerEAPRates()
        {
            LoadRateProvidersForEmployer();
            if (_iclbRateProviderForEmployer.Count > 0)
            {
                _iclbOrgPlanEmplEapRate = new Collection<busOrgPlanEapRate>();
                foreach (busOrgPlanProvider lobjRateProvider in _iclbRateProviderForEmployer)
                {
                    DataTable ldtbList = Select("cdoOrgPlan.GetOrgPlanAndEffectiveDateForEaplRates",
                        new object[2] { lobjRateProvider.icdoOrgPlanProvider.provider_org_id, _icdoOrgPlan.plan_id });
                    if (ldtbList.Rows.Count > 0)
                    {
                        int lintOrgPlanID = Convert.ToInt32(ldtbList.Rows[0]["org_plan_id"]);
                        DateTime ldtEffectiveDate = Convert.ToDateTime(ldtbList.Rows[0]["effective_date"]);
                        DataTable ldtbListEmpEapRate = Select("cdoOrgPlan.LoadEapRatesForEmployer", new object[2] { lintOrgPlanID, ldtEffectiveDate });
                        foreach (DataRow dr in ldtbListEmpEapRate.Rows)
                        {
                            busOrgPlanEapRate lobjOrgPlanEapRate = new busOrgPlanEapRate();
                            lobjOrgPlanEapRate.icdoOrgPlanEapRate = new cdoOrgPlanEapRate();
                            lobjOrgPlanEapRate.iintProviderID = lobjRateProvider.icdoOrgPlanProvider.provider_org_id;
                            sqlFunction.LoadQueryResult(lobjOrgPlanEapRate.icdoOrgPlanEapRate, dr);
                            _iclbOrgPlanEmplEapRate.Add(lobjOrgPlanEapRate);
                        }
                    }
                    foreach (busOrgPlanEapRate lobjOrgPlanEapRate in _iclbOrgPlanEmplEapRate)
                    {
                        lobjOrgPlanEapRate.LoadProvider(lobjOrgPlanEapRate.iintProviderID);
                    }
                }
            }
        }
        public void LoadVisionRates()
        {
            DataTable ldtbList = Select<cdoOrgPlanVisionRate>(
                  new string[1] { "org_plan_id" },
                  new object[1] { _icdoOrgPlan.org_plan_id }, null, null);
            _iclbOrgPlanVisionRate = GetCollection<busOrgPlanVisionRate>(ldtbList, "icdoOrgPlanVisionRate");
        }
        public void LoadEmployerVisionRates()
        {
            LoadRateProvidersForEmployer();
            if (_iclbRateProviderForEmployer.Count > 0)
            {
                _iclbEmployerVisionRate = new Collection<busOrgPlanVisionRate>();
                foreach (busOrgPlanProvider lobjRateProvider in _iclbRateProviderForEmployer)
                {
                    DataTable ldtbList = Select("cdoOrgPlan.GetOrgPlanAndEffectiveDateForVisionRates",
                        new object[2] { lobjRateProvider.icdoOrgPlanProvider.provider_org_id, _icdoOrgPlan.plan_id });
                    if (ldtbList.Rows.Count > 0)
                    {
                        int lintOrgPlanID = Convert.ToInt32(ldtbList.Rows[0]["org_plan_id"]);
                        DateTime ldtEffectiveDate = Convert.ToDateTime(ldtbList.Rows[0]["effective_date"]);
                        DataTable ldtbListEmpVisionRate = Select("cdoOrgPlan.LoadVisionRatesForEmployer", new object[2] { lintOrgPlanID, ldtEffectiveDate });
                        foreach (DataRow dr in ldtbListEmpVisionRate.Rows)
                        {
                            busOrgPlanVisionRate lobjOrgPlanVisionRate = new busOrgPlanVisionRate();
                            lobjOrgPlanVisionRate.icdoOrgPlanVisionRate = new cdoOrgPlanVisionRate();
                            lobjOrgPlanVisionRate.iintProviderID = lobjRateProvider.icdoOrgPlanProvider.provider_org_id;
                            sqlFunction.LoadQueryResult(lobjOrgPlanVisionRate.icdoOrgPlanVisionRate, dr);
                            _iclbEmployerVisionRate.Add(lobjOrgPlanVisionRate);
                        }
                    }
                    foreach (busOrgPlanVisionRate lobjOrgPlanVisionRate in _iclbEmployerVisionRate)
                    {
                        lobjOrgPlanVisionRate.LoadProvider(lobjOrgPlanVisionRate.iintProviderID);
                    }
                }
            }
        }
        public void LoadHmoRates()
        {
            DataTable ldtbList = Select<cdoOrgPlanHmoRate>(
                  new string[1] { "org_plan_id" },
                  new object[1] { _icdoOrgPlan.org_plan_id }, null, null);
            _iclbOrgPlanHmoRate = GetCollection<busOrgPlanHmoRate>(ldtbList, "icdoOrgPlanHmoRate");
        }
        public void LoadEmployerHmoRates()
        {
            LoadRateProvidersForEmployer();
            if (_iclbRateProviderForEmployer.Count > 0)
            {
                _iclbEmployerHmoRate = new Collection<busOrgPlanHmoRate>();
                foreach (busOrgPlanProvider lobjRateProvider in _iclbRateProviderForEmployer)
                {
                    DataTable ldtbList = Select("cdoOrgPlan.GetOrgPlanAndEffectiveDateForHMORates",
                        new object[2] { lobjRateProvider.icdoOrgPlanProvider.provider_org_id, _icdoOrgPlan.plan_id });
                    if (ldtbList.Rows.Count > 0)
                    {
                        int lintOrgPlanID = Convert.ToInt32(ldtbList.Rows[0]["org_plan_id"]);
                        DateTime ldtEffectiveDate = Convert.ToDateTime(ldtbList.Rows[0]["effective_date"]);
                        DataTable ldtbListEmpHmoRate = Select("cdoOrgPlan.LoadHMORatesForEmployer", new object[3] { lintOrgPlanID, ldtEffectiveDate, ibusOrganization.icdoOrganization.emp_category_value });
                        foreach (DataRow dr in ldtbListEmpHmoRate.Rows)
                        {
                            busOrgPlanHmoRate lobjOrgPlanHmoRate = new busOrgPlanHmoRate();
                            lobjOrgPlanHmoRate.icdoOrgPlanHmoRate = new cdoOrgPlanHmoRate();
                            lobjOrgPlanHmoRate.iintProviderID = lobjRateProvider.icdoOrgPlanProvider.provider_org_id;
                            sqlFunction.LoadQueryResult(lobjOrgPlanHmoRate.icdoOrgPlanHmoRate, dr);
                            _iclbEmployerHmoRate.Add(lobjOrgPlanHmoRate);
                        }
                    }
                }
            }
        }
        public void LoadMemberType()
        {
            DataTable ldtbList = Select<cdoOrgPlanMemberType>(
                  new string[1] { "org_plan_id" },
                  new object[1] { _icdoOrgPlan.org_plan_id }, null, null);
            _iclbOrgPlanMemberType = GetCollection<busOrgPlanMemberType>(ldtbList, "icdoOrgPlanMemberType");
        }
        public void LoadLtcRates()
        {
            DataTable ldtbList = Select<cdoOrgPlanLtcRate>(
                  new string[1] { "org_plan_id" },
                  new object[1] { _icdoOrgPlan.org_plan_id }, null, null);
            _iclbOrgPlanLtcRate = GetCollection<busOrgPlanLtcRate>(ldtbList, "icdoOrgPlanLtcRate");
        }
        public void LoadEmployerLTCRates()
        {
            LoadRateProvidersForEmployer();
            if (_iclbRateProviderForEmployer.Count > 0)
            {
                _iclbEmployerLtcRate = new Collection<busOrgPlanLtcRate>();
                foreach (busOrgPlanProvider lobjRateProvider in _iclbRateProviderForEmployer)
                {
                    DataTable ldtbList = Select("cdoOrgPlan.GetOrgPlanAndEffectiveDateForLtcRates",
                        new object[2] { lobjRateProvider.icdoOrgPlanProvider.provider_org_id, _icdoOrgPlan.plan_id });
                    if (ldtbList.Rows.Count > 0)
                    {
                        int lintOrgPlanID = Convert.ToInt32(ldtbList.Rows[0]["org_plan_id"]);
                        DateTime ldtEffectiveDate = Convert.ToDateTime(ldtbList.Rows[0]["effective_date"]);
                        DataTable ldtbListEmpLtcRate = Select("cdoOrgPlan.LoadLTCRatesForEmployer", new object[2] { lintOrgPlanID, ldtEffectiveDate });
                        foreach (DataRow dr in ldtbListEmpLtcRate.Rows)
                        {
                            busOrgPlanLtcRate lobjOrgPlanLtcRate = new busOrgPlanLtcRate();
                            lobjOrgPlanLtcRate.icdoOrgPlanLtcRate = new cdoOrgPlanLtcRate();
                            lobjOrgPlanLtcRate.iintProviderID = lobjRateProvider.icdoOrgPlanProvider.provider_org_id;
                            sqlFunction.LoadQueryResult(lobjOrgPlanLtcRate.icdoOrgPlanLtcRate, dr);
                            _iclbEmployerLtcRate.Add(lobjOrgPlanLtcRate);
                        }
                    }
                    foreach (busOrgPlanLtcRate lobjOrgPlanLtcRate in _iclbEmployerLtcRate)
                    {
                        lobjOrgPlanLtcRate.LoadProvider(lobjOrgPlanLtcRate.iintProviderID);
                    }
                }
            }
        }
        public void LoadEmployerLifeRates()
        {
            LoadRateProvidersForEmployer();
            if (_iclbRateProviderForEmployer.Count > 0)
            {
                _iclbEmployerLifeRate = new Collection<busOrgPlanLifeRate>();
                foreach (busOrgPlanProvider lobjRateProvider in _iclbRateProviderForEmployer)
                {
                    DataTable ldtbList = Select("cdoOrgPlan.GetOrgPlanAndEffectiveDateForLifeRates",
                        new object[2] { lobjRateProvider.icdoOrgPlanProvider.provider_org_id, _icdoOrgPlan.plan_id });
                    if (ldtbList.Rows.Count > 0)
                    {
                        int lintOrgPlanID = Convert.ToInt32(ldtbList.Rows[0]["org_plan_id"]);
                        DateTime ldtEffectiveDate = Convert.ToDateTime(ldtbList.Rows[0]["effective_date"]);
                        DataTable ldtbListEmpLifeRate = Select("cdoOrgPlan.LoadLifeRatesForEmployer", new object[2] { lintOrgPlanID, ldtEffectiveDate });
                        foreach (DataRow dr in ldtbListEmpLifeRate.Rows)
                        {
                            busOrgPlanLifeRate lobjOrgPlanLifeRate = new busOrgPlanLifeRate();
                            lobjOrgPlanLifeRate.icdoOrgPlanLifeRate = new cdoOrgPlanLifeRate();
                            lobjOrgPlanLifeRate.iintProviderID = lobjRateProvider.icdoOrgPlanProvider.provider_org_id;
                            sqlFunction.LoadQueryResult(lobjOrgPlanLifeRate.icdoOrgPlanLifeRate, dr);
                            _iclbEmployerLifeRate.Add(lobjOrgPlanLifeRate);
                        }
                    }
                    foreach (busOrgPlanLifeRate lobjOrgPlanLifeRate in _iclbEmployerLifeRate)
                    {
                        lobjOrgPlanLifeRate.LoadProvider(lobjOrgPlanLifeRate.iintProviderID);
                    }
                }
            }
        }
        public void LoadLifeRates()
        {
            DataTable ldtbList = Select<cdoOrgPlanLifeRate>(
                  new string[1] { "org_plan_id" },
                  new object[1] { _icdoOrgPlan.org_plan_id }, null, null);
            _iclbOrgPlanLifeRate = GetCollection<busOrgPlanLifeRate>(ldtbList, "icdoOrgPlanLifeRate");
        }
        public void LoadHealthRates()
        {
            DataTable ldtbList = Select<cdoOrgPlanHealthMedicarePartDRate>(
                  new string[1] { "org_plan_id" },
                  new object[1] { _icdoOrgPlan.org_plan_id }, null, null);
            _iclbOrgPlanHealthRate = GetCollection<busOrgPlanHealthMedicarePartDRate>(ldtbList, "icdoOrgPlanHealthMedicarePartDRate");
            foreach (busOrgPlanHealthMedicarePartDRate lobjHealthRate in _iclbOrgPlanHealthRate)
            {
                lobjHealthRate.LoadMedicarePartDCoverageRef();
                lobjHealthRate.LoadMedicarePartDRateRef();
            }
        }
        public void LoadMedicarePartDRates()
        {
            DataTable ldtbList = Select<cdoOrgPlanHealthMedicarePartDRate>(
                  new string[1] { "org_plan_id" },
                  new object[1] { _icdoOrgPlan.org_plan_id }, null, null);
            _iclbOrgPlanMedicarePartDRate = GetCollection<busOrgPlanHealthMedicarePartDRate>(ldtbList, "icdoOrgPlanHealthMedicarePartDRate");
            foreach (busOrgPlanHealthMedicarePartDRate lobjMedicarePartDRate in _iclbOrgPlanMedicarePartDRate)
            {
                lobjMedicarePartDRate.LoadMedicarePartDCoverageRef();
                lobjMedicarePartDRate.LoadMedicarePartDRateRef();
            }
        }

        public void LoadEmployerRetirementRates()
        {
            //Loading the Top Most Effective Date as of today           

            _iclbPlanRetirementRate = new Collection<busPlanRetirementRate>();

            foreach (busOrgPlanMemberType lobjOrgPlanMemberType in _iclbOrgPlanMemberType)
            {
                if ((busGlobalFunctions.CheckDateOverlapping(DateTime.Today, lobjOrgPlanMemberType.icdoOrgPlanMemberType.start_date,
                        lobjOrgPlanMemberType.icdoOrgPlanMemberType.end_date)))
                {
                    DataTable ldtbRetirementRates = Select("cdoOrgPlan.LoadEmployerRetirementRates",
                              new object[2] { _icdoOrgPlan.plan_id, lobjOrgPlanMemberType.icdoOrgPlanMemberType.member_type_value });
                    foreach (DataRow dr in ldtbRetirementRates.Rows)
                    {
                        busPlanRetirementRate lobjPlanRtrRate = new busPlanRetirementRate();
                        lobjPlanRtrRate.icdoPlanRetirementRate = new cdoPlanRetirementRate();
                        sqlFunction.LoadQueryResult(lobjPlanRtrRate.icdoPlanRetirementRate, dr);
                        _iclbPlanRetirementRate.Add(lobjPlanRtrRate);
                    }
                }
            }
        }

        public void LoadEmployerRetirementRatesHistory()
        {
            //Loading the Top Most Effective Date as of today           

            _iclbPlanRetirementRateHistory = new Collection<busPlanRetirementRate>();

            foreach (busOrgPlanMemberType lobjOrgPlanMemberType in _iclbOrgPlanMemberType)
            {
                if ((busGlobalFunctions.CheckDateOverlapping(DateTime.Today, lobjOrgPlanMemberType.icdoOrgPlanMemberType.start_date,
                        lobjOrgPlanMemberType.icdoOrgPlanMemberType.end_date)))
                {
                    DataTable ldtbRetirementRates = Select("cdoOrgPlan.LoadEmployerRetirementRateHistory",
                              new object[2] { _icdoOrgPlan.plan_id, lobjOrgPlanMemberType.icdoOrgPlanMemberType.member_type_value });
                    foreach (DataRow dr in ldtbRetirementRates.Rows)
                    {
                        busPlanRetirementRate lobjPlanRtrRate = new busPlanRetirementRate();
                        lobjPlanRtrRate.icdoPlanRetirementRate = new cdoPlanRetirementRate();
                        sqlFunction.LoadQueryResult(lobjPlanRtrRate.icdoPlanRetirementRate, dr);
                        _iclbPlanRetirementRateHistory.Add(lobjPlanRtrRate);
                    }
                }
            }
        }

        public void LoadEmployerHealthRates()
        {
            LoadRateProvidersForEmployer();
            if (_iclbRateProviderForEmployer.Count > 0)
            {
                _iclbEmployerHealthRate = new Collection<busOrgPlanHealthMedicarePartDRate>();
                foreach (busOrgPlanProvider lobjRateProvider in _iclbRateProviderForEmployer)
                {
                    DataTable ldtbList = Select("cdoOrgPlan.GetOrgPlanAndEffectiveDateForHealthRates",
                        new object[3] { lobjRateProvider.icdoOrgPlanProvider.provider_org_id, _icdoOrgPlan.plan_id, HealthInsuranceType });
                    if (ldtbList.Rows.Count > 0)
                    {
                        DataTable ldtbListEmpHealthRate = new DataTable();
                        int lintOrgPlanID = Convert.ToInt32(ldtbList.Rows[0]["org_plan_id"]);
                        DateTime ldtEffectiveDate = Convert.ToDateTime(ldtbList.Rows[0]["effective_date"]);
                        ldtbListEmpHealthRate = Select("cdoOrgPlan.LoadHealthRatesForEmployers",
                                        new object[7] { icdoOrgPlan.wellness_flag, HealthInsuranceType,icdoOrgPlan.org_plan_id,
                                                ldtEffectiveDate,icdoOrgPlan.plan_option_value,(icdoOrgPlan.health_participation_start_date == DateTime.MinValue?
                                                 icdoOrgPlan.participation_start_date:icdoOrgPlan.health_participation_start_date),lintOrgPlanID });//prod pir 6846 : new field to store health participation start date
                        foreach (DataRow dr in ldtbListEmpHealthRate.Rows)
                        {
                            busOrgPlanHealthMedicarePartDRate lobjOrgPlanHealthRate = new busOrgPlanHealthMedicarePartDRate();
                            lobjOrgPlanHealthRate.icdoOrgPlanHealthMedicarePartDRate = new cdoOrgPlanHealthMedicarePartDRate();
                            lobjOrgPlanHealthRate.iintProviderID = lobjRateProvider.icdoOrgPlanProvider.provider_org_id;
                            sqlFunction.LoadQueryResult(lobjOrgPlanHealthRate.icdoOrgPlanHealthMedicarePartDRate, dr);
                            lobjOrgPlanHealthRate.ibusOrgPlan = this;
                            _iclbEmployerHealthRate.Add(lobjOrgPlanHealthRate);
                        }
                    }
                    foreach (busOrgPlanHealthMedicarePartDRate lobjOrgPlanHealthRate in _iclbEmployerHealthRate)
                    {
                        lobjOrgPlanHealthRate.LoadProvider(lobjOrgPlanHealthRate.iintProviderID);
                        lobjOrgPlanHealthRate.LoadMedicarePartDCoverageRef();
                        lobjOrgPlanHealthRate.LoadMedicarePartDRateRef();
                    }
                }
            }
        }
        public void LoadEmployerMedicarePartDRates()
        {
            LoadRateProvidersForEmployer();
            if (_iclbRateProviderForEmployer.Count > 0)
            {
                _iclbEmployerMedicarePartDRate = new Collection<busOrgPlanHealthMedicarePartDRate>();
                foreach (busOrgPlanProvider lobjRateProvider in _iclbRateProviderForEmployer)
                {
                    DataTable ldtbList = Select("cdoOrgPlan.GetOrgPlanAndEffectiveDateforMedicareRates",
                        new object[3] { lobjRateProvider.icdoOrgPlanProvider.provider_org_id, _icdoOrgPlan.plan_id, HealthInsuranceType });
                    if (ldtbList.Rows.Count > 0)
                    {
                        int lintOrgPlanID = Convert.ToInt32(ldtbList.Rows[0]["org_plan_id"]);
                        DateTime ldtEffectiveDate = Convert.ToDateTime(ldtbList.Rows[0]["effective_date"]);
                        DataTable ldtbListEmpHealthRate = Select("cdoOrgPlan.LoadMedicarePartDRates", new object[2] { lintOrgPlanID, ldtEffectiveDate });
                        foreach (DataRow dr in ldtbListEmpHealthRate.Rows)
                        {
                            busOrgPlanHealthMedicarePartDRate lobjOrgPlanHealthRate = new busOrgPlanHealthMedicarePartDRate();
                            lobjOrgPlanHealthRate.icdoOrgPlanHealthMedicarePartDRate = new cdoOrgPlanHealthMedicarePartDRate();
                            lobjOrgPlanHealthRate.iintProviderID = lobjRateProvider.icdoOrgPlanProvider.provider_org_id;
                            sqlFunction.LoadQueryResult(lobjOrgPlanHealthRate.icdoOrgPlanHealthMedicarePartDRate, dr);
                            _iclbEmployerMedicarePartDRate.Add(lobjOrgPlanHealthRate);
                        }
                    }
                    foreach (busOrgPlanHealthMedicarePartDRate lobjOrgPlanMedicarePartDrateRate in _iclbEmployerMedicarePartDRate)
                    {
                        lobjOrgPlanMedicarePartDrateRate.LoadProvider(lobjOrgPlanMedicarePartDrateRate.iintProviderID);
                        lobjOrgPlanMedicarePartDrateRate.LoadMedicarePartDCoverageRef();
                        lobjOrgPlanMedicarePartDrateRate.LoadMedicarePartDRateRef();
                    }
                }
            }
        }
        /// <summary>
        /// To List all active Provider linked to organization in the plan.
        /// </summary>
        public void LoadOrgPlanProviders()
        {
            DataTable ldtbList = Select("cdoOrgPlan.LOAD_ORG_PLAN_PROVIDERS", new object[1] { _icdoOrgPlan.org_plan_id });
            if (_ibusOrganization.IsNull())
                LoadOrganization();
            _ibusOrganization.iProviderName =ldtbList.Rows.Count > 0 ? Convert.ToString(ldtbList.Rows[0][0]) : string.Empty;
            _iclbOrgPlanProvider = GetCollection<busOrgPlanProvider>(ldtbList, "icdoOrgPlanProvider");
            foreach (busOrgPlanProvider lobjTemp in _iclbOrgPlanProvider)
            {
                lobjTemp.LoadOrganization(_icdoOrgPlan.org_id);
                lobjTemp.LoadOrgPlan(_icdoOrgPlan.org_plan_id);
            }
        }

        public void LoadActiveOrgPlanProvidersByEmployerOrgPlan()
        {
            DataTable ldtbList = Select<cdoOrgPlanProvider>(new string[2] { "org_plan_id", "status_value" },
                                                            new object[2] { _icdoOrgPlan.org_plan_id, busConstant.StatusActive },
                                                            null, null);
            _iclbOrgPlanProvider = GetCollection<busOrgPlanProvider>(ldtbList, "icdoOrgPlanProvider");
        }

        private Collection<busOrgPlan> _iclbOtherOrgPlans;
        public Collection<busOrgPlan> iclbOtherOrgPlans
        {
            get { return _iclbOtherOrgPlans; }
            set { _iclbOtherOrgPlans = value; }
        }

        /// <summary>
        /// To Load Other Organization Plans Information.
        /// </summary>
        public void LoadOtherOrgPlans()
        {
            DataTable ldtbList = Select("cdoOrgPlan.LOAD_OTHER_PLANS", new object[2] { _icdoOrgPlan.org_id, _icdoOrgPlan.org_plan_id });
            _iclbOtherOrgPlans = GetCollection<busOrgPlan>(ldtbList, "icdoOrgPlan");
            foreach (busOrgPlan lobjOrgPlan in _iclbOtherOrgPlans)
            {
                lobjOrgPlan.LoadPlanInfo(lobjOrgPlan.icdoOrgPlan.plan_id);
            }
        }

        private busPlan _ibusPlan;
        public busPlan ibusPlan
        {
            get { return _ibusPlan; }
            set { _ibusPlan = value; }
        }

        public void LoadPlanInfo(int lintPlanid)
        {
            if (_ibusPlan == null)
            {
                _ibusPlan = new busPlan();
            }
            _ibusPlan.FindPlan(lintPlanid);
        }

        public void LoadPlanInfo()
        {
            LoadPlanInfo(_icdoOrgPlan.plan_id);
        }
        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            LoadPlanInfo();
            base.BeforeValidate(aenmPageMode);
        }

        public override int PersistChanges()
        {
            int lintResult;
            ObjectState lobjObjectState = _icdoOrgPlan.ienuObjectState;

            //prod pir 6846 : new field to store health participation start date
            if (icdoOrgPlan.health_participation_start_date != DateTime.MinValue && icdoOrgPlan.plan_id != busConstant.PlanIdGroupHealth)
                icdoOrgPlan.health_participation_start_date = DateTime.MinValue;

            lintResult = base.PersistChanges();

            if (lobjObjectState == ObjectState.Insert)
            {
                AssociateAllProviders();
            }

            return lintResult;
        }


        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            //Reloading the Tabs
            //PIR 231            
            LoadOrganization();
            LoadOtherOrgPlans();
            LoadOrgPlanProviders();
            LoadPlanInfo();
            LoadRates();
        }

        public void LoadRates()
        {
            if (_ibusPlan.icdoPlan.plan_code == busConstant.PlanCodeDental)
            {
                if (_ibusOrganization.icdoOrganization.org_type_value == busConstant.OrgTypeEmployer)
                {
                    LoadEmployerDentalRates();
                }
                else if (_ibusOrganization.icdoOrganization.org_type_value == busConstant.OrgTypeProvider)
                {
                    LoadDentalRates();
                }
            }
            if (_ibusPlan.icdoPlan.plan_code == busConstant.PlanCodeEAP)
            {
                if (_ibusOrganization.icdoOrganization.org_type_value == busConstant.OrgTypeEmployer)
                {
                    LoadEmployerEAPRates();
                }
                else if (_ibusOrganization.icdoOrganization.org_type_value == busConstant.OrgTypeProvider)
                {
                    LoadEapRates();
                }
            }

            if (ibusPlan.icdoPlan.plan_code == busConstant.PlanCodeHMO)
            {

                if (_ibusOrganization.icdoOrganization.org_type_value == busConstant.OrgTypeEmployer)
                {
                    LoadEmployerHmoRates();
                }
                else if (_ibusOrganization.icdoOrganization.org_type_value == busConstant.OrgTypeProvider)
                {
                    LoadHmoRates();
                }
            }
            if (ibusPlan.icdoPlan.plan_code == busConstant.PlanCodeVision)
            {

                if (_ibusOrganization.icdoOrganization.org_type_value == busConstant.OrgTypeEmployer)
                {
                    LoadEmployerVisionRates();
                }
                else if (_ibusOrganization.icdoOrganization.org_type_value == busConstant.OrgTypeProvider)
                {
                    LoadVisionRates();
                }
            }
            if (ibusPlan.icdoPlan.plan_code == busConstant.PlanCodeLTC)
            {

                if (_ibusOrganization.icdoOrganization.org_type_value == busConstant.OrgTypeEmployer)
                {
                    LoadEmployerLTCRates();
                }
                else if (_ibusOrganization.icdoOrganization.org_type_value == busConstant.OrgTypeProvider)
                {
                    LoadLtcRates();
                }
            }
            if (ibusPlan.icdoPlan.plan_code == busConstant.PlanCodeGroupLife)
            {

                if (_ibusOrganization.icdoOrganization.org_type_value == busConstant.OrgTypeEmployer)
                {
                    LoadEmployerLifeRates();
                }
                else if (_ibusOrganization.icdoOrganization.org_type_value == busConstant.OrgTypeProvider)
                {
                    LoadLifeRates();
                }
            }
            if (ibusPlan.icdoPlan.plan_code == busConstant.PlanCodeGroupHealth)
            {

                if (_ibusOrganization.icdoOrganization.org_type_value == busConstant.OrgTypeEmployer)
                {
                    LoadEmployerHealthRates();
                }
                else if (_ibusOrganization.icdoOrganization.org_type_value == busConstant.OrgTypeProvider)
                {
                    LoadHealthRates();
                }
            }
            if (ibusPlan.icdoPlan.plan_code == busConstant.PlanCodeMedicarePartD)
            {

                if (_ibusOrganization.icdoOrganization.org_type_value == busConstant.OrgTypeEmployer)
                {
                    LoadEmployerMedicarePartDRates();
                }
                else if (_ibusOrganization.icdoOrganization.org_type_value == busConstant.OrgTypeProvider)
                {
                    LoadMedicarePartDRates();
                }
            }
            if ((_ibusOrganization.icdoOrganization.org_type_value == busConstant.OrgTypeEmployer) &&
                (ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeRetirement))
            {
                LoadMemberType();
                LoadEmployerRetirementRates();
                LoadEmployerRetirementRatesHistory();
            }
        }

        //Adding the List of Active Providers Automatically for the Newly Created Employer
        //Adding the Providers to All the Employer whenever new provides gets added.
        //This logic applicable for Deferred Comp and Flex plans . UAT-PIR 771
        public void AssociateAllProviders()
        {
            if ((_icdoOrgPlan.plan_id == busConstant.PlanIdDeferredCompensation) || (_icdoOrgPlan.plan_id == busConstant.PlanIdFlex))
            {
                if (_ibusOrganization.icdoOrganization.org_type_value == busConstant.OrganizationTypeEmployer)
                {
                    DataTable ldtbList = Select("cdoOrgPlan.LoadAssociatedProviders", new object[1] { _icdoOrgPlan.plan_id });
                    _iclbOrgPlan = GetCollection<busOrgPlan>(ldtbList, "icdoOrgPlan");
                    foreach (busOrgPlan lobjorgPlan in _iclbOrgPlan)
                    {
                        if (busGlobalFunctions.CheckDateOverlapping(_icdoOrgPlan.participation_start_date,
                                  lobjorgPlan.icdoOrgPlan.participation_start_date, lobjorgPlan.icdoOrgPlan.participation_end_date))
                        {
                            cdoOrgPlanProvider lobjcdoOrgProvider = new cdoOrgPlanProvider();
                            lobjcdoOrgProvider.provider_org_id = lobjorgPlan.icdoOrgPlan.org_id;
                            lobjcdoOrgProvider.org_plan_id = _icdoOrgPlan.org_plan_id;
                            lobjcdoOrgProvider.status_value = busConstant.StatusActive;
                            lobjcdoOrgProvider.Insert();
                        }
                    }
                }
                else if (_ibusOrganization.icdoOrganization.org_type_value == busConstant.OrganizationTypeProvider)
                {
                    DataTable ldtbList = Select("cdoOrgPlan.LoadAssociatedEmployers", new object[1] { _icdoOrgPlan.plan_id });
                    _iclbOrgPlanProvider = GetCollection<busOrgPlanProvider>(ldtbList, "icdoOrgPlanProvider");
                    foreach (busOrgPlanProvider lobjPlanProvider in _iclbOrgPlanProvider)
                    {
                        lobjPlanProvider.icdoOrgPlanProvider.provider_org_id = _icdoOrgPlan.org_id;
                        lobjPlanProvider.icdoOrgPlanProvider.Insert();
                    }
                }
            }
        }

        /// <summary>
        /// Check if the given Start and End date is overlapping with the existing records.
        /// </summary>
        /// <returns>bool</returns>
        public bool IsDatesOverlapping()
        {
            bool lblnRecordMatch = false;
            foreach (busOrgPlan lobjOrgPlan in _iclbOtherOrgPlans)
            {
                if (_icdoOrgPlan.plan_id == lobjOrgPlan._icdoOrgPlan.plan_id &&
                    (busGlobalFunctions.CheckDateOverlapping(
                    _icdoOrgPlan.participation_start_date,
                    lobjOrgPlan._icdoOrgPlan.participation_start_date,
                    lobjOrgPlan._icdoOrgPlan.participation_end_date) ||
                    (busGlobalFunctions.CheckDateOverlapping(
                    _icdoOrgPlan.participation_end_date,
                    lobjOrgPlan._icdoOrgPlan.participation_start_date,
                    lobjOrgPlan._icdoOrgPlan.participation_end_date)
                    )))
                {
                    lblnRecordMatch = true;
                    break;
                }
            }
            return lblnRecordMatch;
        }

        // to check in Person Employment whether plan is restricted
        public bool  IsPlanRestricted()
        {
            if (icdoOrgPlan.restriction == busConstant.Flag_Yes)
            { return true; }
            else { return false; }
        }

        // STC-031 PIR 38
        private string _istrSuppressWarning;
        public string istrSuppressWarning
        {
            get { return _istrSuppressWarning; }
            set { _istrSuppressWarning = value; }
        }

        public bool IsDateDiffLessThanFiveYears()
        {
            if (_icdoOrgPlan.participation_start_date != DateTime.MinValue && _icdoOrgPlan.participation_end_date != DateTime.MinValue)
            {
                DateTime ldtTempDate = _icdoOrgPlan.participation_start_date.AddYears(5);
                if (_icdoOrgPlan.participation_end_date <= ldtTempDate)
                    return true;
            }
            return false;
        }

        //PIR - 850
        //this  is special case where the org code 012500 is allowed to select LE with prior service and Emp Category as State
        public bool IsPlanValid()
        {
            if (!String.IsNullOrEmpty(ibusOrganization.icdoOrganization.emp_category_value))
            {
                DataTable ldtbGetPlanEmpCategory = iobjPassInfo.isrvDBCache.GetCacheData("sgt_plan_emp_category_crossref", null);
                DataTable lobject = (from s in ldtbGetPlanEmpCategory.AsEnumerable()
                                     where Convert.ToInt32(s["plan_id"]) == icdoOrgPlan.plan_id &&
                                     Convert.ToString(s["emp_category_value"]) == ibusOrganization.icdoOrganization.emp_category_value
                                     select s).AsDataTable();
                if (lobject.Rows.Count == 0)
                {
                    if ((icdoOrgPlan.plan_id == busConstant.PlanIdLE || (icdoOrgPlan.plan_id == busConstant.PlanIdBCILawEnf)//pir 7943
                        || (icdoOrgPlan.plan_id == busConstant.PlanIdStatePublicSafety) || (icdoOrgPlan.plan_id == busConstant.PlanIdNG)) && //PIR 25729
                        (ibusOrganization.icdoOrganization.org_code == busConstant.AttorneyGeneralOfficeOrgCode))
                    {
                        return true;
                    }
                    else
                        return false;
                }
            }
            return true;
        }

        # region UCS 40 correspondence

        public Collection<busInsurancePremium> iclbDentalPremium { get; set; }
        public Collection<busInsurancePremium> iclbVisionPremium { get; set; }
        public Collection<busInsurancePremium> iclbEAPPremium { get; set; }
        public Collection<busInsurancePremium> iclbHealthPremium { get; set; }
        public Collection<busInsurancePremium> iclbHealthCOBRAPremium { get; set; }
        public Collection<busInsurancePremium> iclbOrgToBillPremium { get; set; }
        public Collection<busInsurancePremium> iclbTFFRPensionCheckPremium { get; set; }

        public Collection<busInsurancePremium> iclbLifeAgeEmployerPremium { get; set; }

        public busRateChangeLetterRequest ibusRateChangeLetterRequest { get; set; }

        public bool iblnIsHealthCOBRA
        {
            get
            {
                if (iclbHealthCOBRAPremium.Count > 0)
                    return true;
                return false;
            }
        }

        public bool iblnIsOrgToBillExists
        {
            get
            {
                if (iclbOrgToBillPremium.Count > 0)
                    return true;
                return false;
            }
        }

        public bool iblnTFFRMembersExists
        {
            get
            {
                if (iclbTFFRPensionCheckPremium.Count > 0)
                    return true;
                return false;
            }
        }

        public bool iblnIsPeopleSoftOrgGroupExists
        {
            get
            {
                if (ibusOrganization.icdoOrganization.peoplesoft_org_group_value.IsNotNullOrEmpty())
                    return true;
                return false;
            }
        }

        # endregion

        //******** cor related properties       
        public override busBase GetCorOrganization()
        {
            if (_ibusOrganization == null)
                LoadOrganization();
            return _ibusOrganization;
        }

        //PIR - systest 941
        public DateTime idtBatchRunDate { get; set; }
        public string idtBatchRunDateAsLongDateFormat
        {
            get
            {
                return idtBatchRunDate.IsNull() ? string.Empty : idtBatchRunDate.ToString(busConstant.DateFormatLongDate);
            }
        }

        #region UCS - 032

        public Collection<busPerson> iclbEmployeeList { get; set; }
        public DataTable idtEmployee { get; set; }

        public int iintContactID { get; set; }

        public void LoadEmployeesEnrolledInPlan()
        {
            idtEmployee = Select("cdoPerson.ESSGetEmployeeList",
                                    new object[3] { icdoOrgPlan.plan_id, iintContactID, icdoOrgPlan.org_id });            
        }

        public void ESSFilterRates()
        {
            //filtering health rate
            if (iclbEmployerHealthRate != null && iclbEmployerHealthRate.Count > 0)
            {
                IEnumerable<busOrgPlanHealthMedicarePartDRate> lenmHealthRate = iclbEmployerHealthRate
                                        .Where(o => (o.icdoOrgPlanHealthMedicarePartDRate.premium_period_end_date == DateTime.MinValue ||
                                                    o.icdoOrgPlanHealthMedicarePartDRate.premium_period_end_date > DateTime.Today) &&
                                                    o.icdoOrgPlanHealthMedicarePartDRate.effective_date <= DateTime.Today);
                iclbEmployerHealthRate = new Collection<busOrgPlanHealthMedicarePartDRate>();
                foreach (busOrgPlanHealthMedicarePartDRate lobjRate in lenmHealthRate)
                {
                    iclbEmployerHealthRate.Add(lobjRate);
                }
            }
        }

        public Collection<cdoCodeValue> iclcCodeValue { get; set; }
        public void LoadESSLTCRatesLink()
        {
            iclcCodeValue = busBase.GetCollection<cdoCodeValue>(new string[2] { "code_id", "code_value" },
                                                                new object[2] { 52, busConstant.LTCRates }, null, null);
        }
        public void LoadESSLifeRatesLink()
        {
            iclcCodeValue = busBase.GetCollection<cdoCodeValue>(new string[2] { "code_id", "code_value" },
                                                                new object[2] { 52, busConstant.LifeRates }, null, null);
        }

        public string istrLastName { get; set; }
        public string istrFirstName { get; set; }
        public string istrLast4DigitsSSN { get; set; }
        public int iintPersonID { get; set; }

        public ArrayList btn_ShowEmployees()
        {
            ArrayList larrList = new ArrayList();
            LoadEmployeesEnrolledInPlan();
            DataTable ldtEmployee = new DataTable();
            bool lblnFiltered = false;
            if (iintPersonID > 0)
            {
                ldtEmployee = idtEmployee.AsEnumerable().Where(o => o.Field<int>("person_id") == iintPersonID).AsDataTable();
                lblnFiltered = true;
            }
            if (!string.IsNullOrEmpty(istrLastName))
            {
                ldtEmployee = idtEmployee.AsEnumerable().Where(o => o.Field<string>("last_name").ToString().ToLower().Contains(istrLastName.ToLower()) == true).AsDataTable();
                lblnFiltered = true;
            }
            if (!string.IsNullOrEmpty(istrFirstName))
            {
                ldtEmployee = idtEmployee.AsEnumerable().Where(o => o.Field<string>("first_name").ToString().ToLower().Contains(istrFirstName.ToLower()) == true).AsDataTable();
                lblnFiltered = true;
            }
            if (!string.IsNullOrEmpty(istrLast4DigitsSSN))
            {
                ldtEmployee = idtEmployee.AsEnumerable().Where(o => o.Field<string>("ssn").Substring(5,4) == istrLast4DigitsSSN).AsDataTable();
                lblnFiltered = true;
            }
            if (!lblnFiltered)
            {
                ldtEmployee = idtEmployee;
            }
            iclbEmployeeList = new Collection<busPerson>();
            iclbEmployeeList = GetCollection<busPerson>(ldtEmployee, "icdoPerson");
            foreach (busPerson lobjPerson in iclbEmployeeList)
                lobjPerson.GetPersonCurrentAddressByType(busConstant.AddressTypePermanent, DateTime.Now);
            larrList.Add(this);
            return larrList;
        }

        #endregion

        //UAT PIR 1898
        //used in SFN - 58437
        public string istrWellnessCoordinatorName { get; set; }
        private void LoadWellnessCoordinatorName()
        {
            istrWellnessCoordinatorName = string.Empty;
            if (ibusOrganization.IsNull())
                LoadOrganization();

            if (ibusOrganization.iclbOrgContact.IsNull())
                ibusOrganization.LoadOrgContact();

            bool lblnRecordFound = false;
            foreach (busOrgContact lobjOrgContact in ibusOrganization.iclbOrgContact)
            {
                if (lobjOrgContact.icdoOrgContact.status_value == busConstant.StatusActive)
                {
                    if (lobjOrgContact.iclbOrgContactRole == null)
                        lobjOrgContact.LoadContactTypes();
                    foreach (cdoOrgContactRole lobjOrgContactRole in lobjOrgContact.iclbOrgContactRole)
                    {
                        if (lobjOrgContactRole.contact_role_value == busConstant.OrgContactRoleWellnessCoordinator)
                        {
                            //if (lobjOrgContact.icdoOrgContact.plan_id == icdoOrgPlan.plan_id)
                           // {
                                lobjOrgContact.LoadContact();

                                istrWellnessCoordinatorName = lobjOrgContact.ibusContact.icdoContact.ContactName;
                                lblnRecordFound = true;
                                break;
                            //}
                        }
                    }
                    if (lblnRecordFound)
                        break;
                }
            }
        }

        public override void LoadCorresProperties(string astrTemplateName)
        {
            LoadWellnessCoordinatorName();
        }

        //UAT PIR 1962
        public Collection<busOrgPlan> iclbInvoiceCollection { get; set; }
        public Collection<busOrgPlan> iclbRemittanceCollection { get; set; }

        public DateTime idtBillingDate { get; set; }
        public DateTime idtPaymentDate { get; set; }
        public int iintInvoiceNumber { get; set; }
        public decimal idecIndividualInvoiceAmount { get; set; }
        public decimal idecTotalInvoiceAmount { get; set; }
        public int iintRemittanceNumber { get; set; }
        public decimal idecIndividualRemittanceAmount { get; set; }
        public decimal idecTotalRemittanceAmount { get; set; }
    }
}
