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
    public class busOrgPlanHealthMedicarePartDRate : busOrgPlanHealthMedicarePartDRateGen
    {
        private busOrgPlanGroupHealthMedicarePartDCoverageRef _ibusMedicarePartDCoverageRef;
        public busOrgPlanGroupHealthMedicarePartDCoverageRef ibusMedicarePartDCoverageRef
        {
            get
            {
                return _ibusMedicarePartDCoverageRef;
            }
            set
            {
                _ibusMedicarePartDCoverageRef = value;
            }
        }
        private busOrgPlanGroupHealthMedicarePartDRateRef _ibusMedicarePartDRateRef ;

        public busOrgPlanGroupHealthMedicarePartDRateRef ibusMedicarePartDRateRef
        {
            get { return _ibusMedicarePartDRateRef ; }
            set { _ibusMedicarePartDRateRef  = value; }
        }

        private int _iintOrgPlanGroupHealthMedicarePartDRateRefID;
        public int iintOrgPlanGroupHealthMedicarePartDRateRefID
        {
            get
            {
                return _iintOrgPlanGroupHealthMedicarePartDRateRefID;
            }
            set
            {
                _iintOrgPlanGroupHealthMedicarePartDRateRefID = value;
            }
        }

        private int _iintOrgPlanGroupHealthMedicarePartDCoverageRefID;
        public int iintOrgPlanGroupHealthMedicarePartDCoverageRefID
        {
            get
            {
                return _iintOrgPlanGroupHealthMedicarePartDCoverageRefID;
            }
            set
            {
                _iintOrgPlanGroupHealthMedicarePartDCoverageRefID = value;
            }
        }
        private string _istrRateStructure;
        public string istrRateStructure
        {
            get
            {
                return _istrRateStructure;
            }
            set
            {
                _istrRateStructure = value;
            }
        }
        private string _istrCoverageCode;
        public string istrCoverageCode
        {
            get
            {
                return _istrCoverageCode;
            }
            set
            {
                _istrCoverageCode = value;
            }
        }
       
        private string _istrPlanOption;
        public string istrPlanOption
        {
            get
            {
                return _istrPlanOption;
            }
            set
            {
                _istrPlanOption = value;
            }
        }
        private string _istrMemberType;
        public string istrMemberType
        {
            get
            {
                return _istrMemberType;
            }
            set
            {
                _istrMemberType = value;
            }
        }
        private string _istrEmploymentType;
        public string istrEmploymentType
        {
            get
            {
                return _istrEmploymentType;
            }
            set
            {
                _istrEmploymentType = value;
            }
        }
        private string _istrWellnessFlag;
        public string istrWellnessFlag
        {
            get
            {
                return _istrWellnessFlag;
            }
            set
            {
                _istrWellnessFlag = value;
            }
        }
        private string _istrMedicareInFlag;
        public string istrMedicareInFlag
        {
            get
            {
                return _istrMedicareInFlag;
            }
            set
            {
                _istrMedicareInFlag = value;
            }
        }
        private string _istrLowIncomeFlag;
        public string istrLowIncomeFlag
        {
            get
            {
                return _istrLowIncomeFlag;
            }
            set
            {
                _istrLowIncomeFlag = value;
            }
        }
        private string _istrCobraFlag;
        public string istrCobraFlag
        {
            get
            {
                return _istrCobraFlag;
            }
            set
            {
                _istrCobraFlag = value;
            }
        }
        private string _istrMemberDependentEligible;
        public string istrMemberDependentEligible
        {
            get
            {
                return _istrMemberDependentEligible;
            }
            set
            {
                _istrMemberDependentEligible = value;
            }
        }
        private busOrganization _ibusProvider;
        public busOrganization ibusProvider
        {
            get { return _ibusProvider; }
            set { _ibusProvider = value; }
        }
        private int _iintProviderID;
        public int iintProviderID
        {
            get { return _iintProviderID; }
            set { _iintProviderID = value; }
        }
        public void LoadProvider(int AintProviderID)
        {
            if (_ibusProvider == null)
            {
                _ibusProvider = new busOrganization();
            }
            _ibusProvider.FindOrganization(AintProviderID);
        }
        //This property is used to validate certain mandatory fields only for Group Health Maintenance. (Not in Medicare)        
        public bool IsGroupHealthScreen
        {
            get
            {
                if (ibusPlan.icdoPlan.plan_code == busConstant.PlanCodeGroupHealth)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public void LoadOrganization()
        {
            if (ibusOrganization == null)
            {
                ibusOrganization = new busOrganization();
            }
            ibusOrganization.FindOrganization(ibusOrgPlan.icdoOrgPlan.org_id);
        }
        public void LoadMedicarePartDCoverageRef()
        {
            if(_ibusMedicarePartDCoverageRef==null)
            {
                _ibusMedicarePartDCoverageRef = new busOrgPlanGroupHealthMedicarePartDCoverageRef();
            }
            _ibusMedicarePartDCoverageRef.FindOrgPlanGroupHealthMedicarePartDCoverageRef(this.icdoOrgPlanHealthMedicarePartDRate.org_plan_group_health_medicare_part_d_coverage_ref_id);
        }
        public void LoadMedicarePartDRateRef()
        {
            if (_ibusMedicarePartDRateRef == null)
            {
                _ibusMedicarePartDRateRef = new busOrgPlanGroupHealthMedicarePartDRateRef();
            }
            _ibusMedicarePartDRateRef.FindOrgPlanGroupHealthMedicarePartDRateRef(this.ibusMedicarePartDCoverageRef.icdoOrgPlanGroupHealthMedicarePartDCoverageRef.org_plan_group_health_medicare_part_d_rate_ref_id);
        }
        public bool CheckForRatesOvelapping()
        {
            if (this.icdoOrgPlanHealthMedicarePartDRate.org_plan_group_health_medicare_part_d_coverage_ref_id != 0)
            {
                DataTable ldtbListOverlap = busNeoSpinBase.Select("cdoOrgPlanHealthMedicarePartDRate.CheckForOverlapping",
                               new object[2] { this.icdoOrgPlanHealthMedicarePartDRate.org_plan_id, this.icdoOrgPlanHealthMedicarePartDRate.org_plan_group_health_medicare_part_d_coverage_ref_id });
                if (ldtbListOverlap.Rows.Count > 0)
                {
                    foreach(DataRow dr in ldtbListOverlap.Rows)
                    {
                        if ((busGlobalFunctions.CheckDateOverlapping(icdoOrgPlanHealthMedicarePartDRate.premium_period_start_date, Convert.ToDateTime(dr["premium_period_start_date"]), Convert.ToDateTime(dr["premium_period_end_date"])))
                                       ||
                              (busGlobalFunctions.CheckDateOverlapping(icdoOrgPlanHealthMedicarePartDRate.premium_period_end_date, Convert.ToDateTime(dr["premium_period_start_date"]), Convert.ToDateTime(dr["premium_period_end_date"]))))
                        {
                            return true;
                        }                        
                     }
                }
            }
            return false;
        }
        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            _iintOrgPlanGroupHealthMedicarePartDRateRefID = 0;

            if (!IsGroupHealthScreen)
            {
                //Get the Member Type
                DataTable ldtbListMemberType = busNeoSpinBase.Select("cdoOrgPlanHealthMedicarePartDRate.GetMemberTypeForMedicare", new object[0] { });
                if (ldtbListMemberType.Rows.Count > 0)
                {
                    _istrMemberType = ldtbListMemberType.Rows[0]["code_value"].ToString();
                }

                //Set the Wellness
                _istrWellnessFlag = "0";
                _istrMedicareInFlag = "1";                
                _istrEmploymentType = "PERM";
                _istrPlanOption = string.Empty;               
            }
            if ((_istrMemberType == null) || (_istrWellnessFlag == null) || (_istrRateStructure == null) || (_istrPlanOption == null))
            {
                base.BeforeValidate(aenmPageMode);
                return;
            }
             DataTable ldtbList = busNeoSpinBase.Select("cdoOrgPlanHealthMedicarePartDRate.GetOrgPlanGroupHealthMedicarePartDRateRefID",
               new object[5] { this.istrPlanOption, this.istrWellnessFlag, this.istrLowIncomeFlag, this.istrRateStructure, this.istrMemberType });

            if (ldtbList.Rows.Count > 0)
            {
                _iintOrgPlanGroupHealthMedicarePartDRateRefID = Convert.ToInt32(ldtbList.Rows[0]["ORG_PLAN_GROUP_HEALTH_MEDICARE_PART_D_RATE_REF_ID"]);
            }

            if (_iintOrgPlanGroupHealthMedicarePartDRateRefID > 0)
            {
                this.icdoOrgPlanHealthMedicarePartDRate.org_plan_group_health_medicare_part_d_coverage_ref_id = Convert.ToInt32(DBFunction.DBExecuteScalar("cdoOrgPlanHealthMedicarePartDRate.GetOrgPlanGroupHealthMedicarePartDCoverageRefID",
                   new object[6] { _iintOrgPlanGroupHealthMedicarePartDRateRefID, this.istrEmploymentType, this.istrMemberDependentEligible, this.istrMedicareInFlag, this.istrCobraFlag, this.istrCoverageCode },
                                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
            }
            base.BeforeValidate(aenmPageMode);
        }
        public override void BeforePersistChanges()
        {
            base.BeforePersistChanges();
        }
    }
}
