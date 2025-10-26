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

#endregion

namespace NeoSpin.BusinessObjects
{
	[Serializable]
	public class busMergeEmployerHeader : busMergeEmployerHeaderGen
	{
        private bool _iblnDetailHasError;

        public bool iblnDetailHasError
        {
            get { return _iblnDetailHasError; }
            set { _iblnDetailHasError = value; }
        }

        private string _istrFromOrgCodeID;
        public string istrFromOrgCodeID
        {
            get { return _istrFromOrgCodeID; }
            set { _istrFromOrgCodeID = value; }
        }
        private string _istrToOrgCodeID;
        public string istrToOrgCodeID
        {
            get { return _istrToOrgCodeID; }
            set { _istrToOrgCodeID = value; }
        }
        private Collection<busOrgPlan> _iclbFromOrgPlans;

        public Collection<busOrgPlan> iclbFromOrgPlans
        {
            get { return _iclbFromOrgPlans; }
            set { _iclbFromOrgPlans = value; }
        }
        private Collection<busOrgPlan> _iclbToOrgPlans;

        public Collection<busOrgPlan> iclbToOrgPlans
        {
            get { return _iclbToOrgPlans; }
            set { _iclbToOrgPlans = value; }
        }

        private Collection<busMergeEmployerDetail> _iclbMergeEmployerDetail;
        public Collection<busMergeEmployerDetail> iclbMergeEmployerDetail
        {
            get { return _iclbMergeEmployerDetail; }
            set { _iclbMergeEmployerDetail = value; }
        } 
        public void LoadMergeEmployerDetails()
        {
            DataTable ldtbList = Select<cdoMergeEmployerDetail>(
                 new string[1] { "merge_employer_header_id" },
                 new object[1] {icdoMergeEmployerHeader.merge_employer_header_id }, null, null);
            _iclbMergeEmployerDetail = GetCollection<busMergeEmployerDetail>(ldtbList, "icdoMergeEmployerDetail");
            foreach(busMergeEmployerDetail lobjMergeEmployerDetail in _iclbMergeEmployerDetail)
            {
                lobjMergeEmployerDetail.LoadPerson();
                lobjMergeEmployerDetail.LoadMergeEmployerHeader();
            }
        }
        public void LoadFromOrgPlans()
        {
            DataTable ldtbListFromOrgPlans = Select<cdoOrgPlan>(new string[1] { "org_id" }, 
                                                new object[1] { icdoMergeEmployerHeader.from_employer_id }, null, "plan_id");
            _iclbFromOrgPlans=GetCollection<busOrgPlan>(ldtbListFromOrgPlans,"icdoOrgPlan");
        }
        public void LoadToOrgPlans()
        {
            DataTable ldtbListToOrgPlans = Select<cdoOrgPlan>(new string[1] { "org_id" }, 
                                            new object[1] { icdoMergeEmployerHeader.to_employer_id }, null, "plan_id");
            _iclbToOrgPlans = GetCollection<busOrgPlan>(ldtbListToOrgPlans, "icdoOrgPlan");
        }
        public bool ValidateMergeOrgPlansMemberTypesAreValid()
        {
            foreach (busOrgPlan lobjFromOrgPlan in iclbFromOrgPlans)
            {
                lobjFromOrgPlan.LoadPlanInfo();
                if (lobjFromOrgPlan.ibusPlan.IsRetirementPlan())
                {
                    lobjFromOrgPlan.LoadMemberType();
                    foreach (busOrgPlan lobjToOrgPlan in iclbToOrgPlans)
                    {
                        lobjToOrgPlan.LoadPlanInfo();
                        if (lobjToOrgPlan.ibusPlan.IsRetirementPlan())
                        {
                            lobjToOrgPlan.LoadMemberType();
                            if (lobjFromOrgPlan.icdoOrgPlan.plan_id == lobjToOrgPlan.icdoOrgPlan.plan_id)
                            {
                                if (((lobjFromOrgPlan.iclbOrgPlanMemberType.Count == 0) && (lobjToOrgPlan.iclbOrgPlanMemberType.Count > 0))
                                    || ((lobjFromOrgPlan.iclbOrgPlanMemberType.Count > 0) && (lobjToOrgPlan.iclbOrgPlanMemberType.Count == 0)))
                                    return false;
                                else
                                {
                                    // If both employers Member type matches then no validations should be fired.
                                    foreach (busOrgPlanMemberType lobjFromOrgPlanMemberType in lobjFromOrgPlan.iclbOrgPlanMemberType)
                                    {
                                        if (!lobjToOrgPlan.iclbOrgPlanMemberType.Where(i => i.icdoOrgPlanMemberType.member_type_value == 
                                                                    lobjFromOrgPlanMemberType.icdoOrgPlanMemberType.member_type_value).Any())
                                            return false;
                                    }
                                    foreach (busOrgPlanMemberType lobjToOrgPlanMemberType in lobjToOrgPlan.iclbOrgPlanMemberType)
                                    {
                                        if (!lobjFromOrgPlan.iclbOrgPlanMemberType.Where(i => i.icdoOrgPlanMemberType.member_type_value ==
                                                                    lobjToOrgPlanMemberType.icdoOrgPlanMemberType.member_type_value).Any())
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
        // Validation will throw an error if the org plans associated with
        //From Employer are not matching with the org plans associated with To Employer.
        public bool ValidateMergeOrgPlansAreValid()
        {
            bool lblnvalid = false;            
            foreach (busOrgPlan lobjFromOrgPlan in iclbFromOrgPlans)
            {
                if (lobjFromOrgPlan.icdoOrgPlan.end_date_no_null > DateTime.Today)
                {
                    foreach (busOrgPlan lobjToOrgPlan in iclbToOrgPlans)
                    {
                        if (lobjToOrgPlan.icdoOrgPlan.end_date_no_null > DateTime.Today)
                        {
                            if (lobjFromOrgPlan.icdoOrgPlan.plan_id == lobjToOrgPlan.icdoOrgPlan.plan_id)
                            {
                                lblnvalid = true;
                                break;
                            }
                            else
                            {
                                lblnvalid = false; ;
                                continue;
                            }
                        }
                    }
                }
            }
            return lblnvalid;
        }
        // Validation will throw an error if the org plan Provider associated with
        // From Employer is not same as Provider associated with To Employer.
        public bool ValidateMergeEmployersProvidersAreSame()
        {           
            foreach (busOrgPlan lobjFromOrgPlan in iclbFromOrgPlans)
            {
                lobjFromOrgPlan.LoadOrgPlanProviders();
                lobjFromOrgPlan.LoadPlanInfo();
                if (!lobjFromOrgPlan.ibusPlan.IsRetirementPlan())
                 {
                     foreach (busOrgPlan lobjToOrgPlan in iclbToOrgPlans)
                     {
                         lobjToOrgPlan.LoadOrgPlanProviders();
                         lobjToOrgPlan.LoadPlanInfo();
                         if (!lobjToOrgPlan.ibusPlan.IsRetirementPlan())
                         {
                             if (lobjFromOrgPlan.icdoOrgPlan.plan_id == lobjToOrgPlan.icdoOrgPlan.plan_id)
                             {
                                 if (((lobjFromOrgPlan.iclbOrgPlanProvider.Count == 0) && (lobjToOrgPlan.iclbOrgPlanProvider.Count > 0))
                                     || ((lobjFromOrgPlan.iclbOrgPlanProvider.Count > 0) && (lobjToOrgPlan.iclbOrgPlanProvider.Count == 0)))
                                     return false;
                                 else
                                     foreach (busOrgPlanProvider lobjFromOrgPlanProvider in lobjFromOrgPlan.iclbOrgPlanProvider)
                                     {
                                         foreach (busOrgPlanProvider lobjToOrgPlanProvider in lobjToOrgPlan.iclbOrgPlanProvider)
                                         {
                                             if (lobjFromOrgPlanProvider.icdoOrgPlanProvider.provider_org_id
                                                                     != lobjToOrgPlanProvider.icdoOrgPlanProvider.provider_org_id)
                                             {
                                                 return false;
                                             }
                                         }
                                     }
                             }
                         }
                     }
                 }
            }
            return true;
        }
        // Validation will throw an error if the report frequency for deferred comp plan and 
        // plan option and wellness flag associated with From Employer are not matching with To Employer.
        public bool IsReportFrequencyMatchForDeferredComp()
        {            
            foreach (busOrgPlan lobjFromOrgPlan in iclbFromOrgPlans)
            {
                if (lobjFromOrgPlan.icdoOrgPlan.end_date_no_null > DateTime.Today)
                {
                    if (lobjFromOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdDeferredCompensation)
                    {
                        string lstrReportFrequency = lobjFromOrgPlan.icdoOrgPlan.report_frequency_value;

                        foreach (busOrgPlan lobjToOrgPlan in iclbToOrgPlans)
                        {
                            if (lobjToOrgPlan.icdoOrgPlan.end_date_no_null > DateTime.Today)
                            {
                                if (lobjToOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdDeferredCompensation)
                                {
                                    if (lstrReportFrequency != lobjToOrgPlan.icdoOrgPlan.report_frequency_value)
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                    else if(lobjFromOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdGroupHealth)
                    {
                        string lstrPlanOption = lobjFromOrgPlan.icdoOrgPlan.plan_option_value;
                        string lstrWellnessFlag= lobjFromOrgPlan.icdoOrgPlan.wellness_flag;
                         foreach (busOrgPlan lobjToOrgPlan in iclbToOrgPlans)
                        {
                            if (lobjToOrgPlan.icdoOrgPlan.end_date_no_null > DateTime.Today)
                            {
                                if (lobjToOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdGroupHealth)
                                {
                                    if ((lstrPlanOption != lobjToOrgPlan.icdoOrgPlan.plan_option_value)
                                        ||(lstrWellnessFlag != lobjToOrgPlan.icdoOrgPlan.wellness_flag))
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }       
        public bool IsEmployersHavingPlans()
        {
            if((iclbFromOrgPlans.Count==0)||(iclbToOrgPlans.Count==0))
            {
                return true;
            }
            return false;
        }
        public bool IsHeaderExist()
        {
            DataTable ldtbMergeEmployerHeader = busNeoSpinBase.Select("cdoMergeEmployerHeader.CheckRecordExist",
                                            new object[4] { icdoMergeEmployerHeader.merge_status_value,
                                                            icdoMergeEmployerHeader.to_employer_id,
                                                            icdoMergeEmployerHeader.from_employer_id,
                                                            icdoMergeEmployerHeader.merge_employer_header_id
                                                        });
            if (ldtbMergeEmployerHeader.Rows.Count > 0)
            {
                return true;
            }
            return false;
        }
        public override void  UpdateValidateStatus()
        {
            base.UpdateValidateStatus();
            OverrideHeaderStatus();
        }

        private void OverrideHeaderStatus()
        {
            if (iclbMergeEmployerDetail == null)
                LoadMergeEmployerDetails();
            foreach (busMergeEmployerDetail lobjMergeEmployerDetail in iclbMergeEmployerDetail)
            {
                lobjMergeEmployerDetail.ValidateSoftErrors();
                if (lobjMergeEmployerDetail.ibusSoftErrors.iclbError.Count > 0)
                {
                    iblnDetailHasError = true;
                }
            }
            if (iblnDetailHasError)
            {
                icdoMergeEmployerHeader.status_value = busConstant.StatusReview;
                icdoMergeEmployerHeader.Update();
            }
        }                          
        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            icdoMergeEmployerHeader.from_employer_id = busGlobalFunctions.GetOrgIdFromOrgCode(istrFromOrgCodeID);
            icdoMergeEmployerHeader.to_employer_id = busGlobalFunctions.GetOrgIdFromOrgCode(istrToOrgCodeID);
            if (ibusFromEmployer == null)
                LoadFromEmployer();
            if (ibusToEmployer == null)
                LoadToEmployer();
            if (_iclbFromOrgPlans == null)
                LoadFromOrgPlans();
            if (_iclbToOrgPlans == null)
                LoadToOrgPlans();
            base.BeforeValidate(aenmPageMode);
        }
	}
}
