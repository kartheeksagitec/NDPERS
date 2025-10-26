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
    public class busPersonAccountGhdvHistory : busPersonAccountGhdvHistoryGen
    {
        /// <summary>
        /// This Method Gets the GHDV objects as param and returns the same with old value. 
        /// It needs the object in param because of the start date.
        /// </summary>
        /// <param name="abusPersonAccountGhdv"></param>
        /// <returns></returns>
        public busPersonAccountGhdv LoadGHDVObject(busPersonAccountGhdv abusPersonAccountGhdv)
        {
            abusPersonAccountGhdv.icdoPersonAccountGhdv.person_account_ghdv_id = icdoPersonAccountGhdvHistory.person_account_ghdv_id;
            if (abusPersonAccountGhdv.icdoPersonAccount.start_date == DateTime.MinValue)
                abusPersonAccountGhdv.icdoPersonAccount.start_date = icdoPersonAccountGhdvHistory.start_date;
            abusPersonAccountGhdv.icdoPersonAccount.history_change_date = icdoPersonAccountGhdvHistory.start_date;
            abusPersonAccountGhdv.icdoPersonAccount.end_date = icdoPersonAccountGhdvHistory.end_date;
            abusPersonAccountGhdv.icdoPersonAccount.plan_participation_status_id = icdoPersonAccountGhdvHistory.plan_participation_status_id;
            abusPersonAccountGhdv.icdoPersonAccount.plan_participation_status_value = icdoPersonAccountGhdvHistory.plan_participation_status_value;
            abusPersonAccountGhdv.icdoPersonAccount.status_id = icdoPersonAccountGhdvHistory.status_id;
            abusPersonAccountGhdv.icdoPersonAccount.status_value = icdoPersonAccountGhdvHistory.status_value;
            abusPersonAccountGhdv.icdoPersonAccount.from_person_account_id = icdoPersonAccountGhdvHistory.from_person_account_id;
            abusPersonAccountGhdv.icdoPersonAccount.to_person_account_id = icdoPersonAccountGhdvHistory.to_person_account_id;
            abusPersonAccountGhdv.icdoPersonAccount.suppress_warnings_flag = icdoPersonAccountGhdvHistory.suppress_warnings_flag;
            abusPersonAccountGhdv.icdoPersonAccount.suppress_warnings_by = icdoPersonAccountGhdvHistory.suppress_warnings_by;
            abusPersonAccountGhdv.icdoPersonAccount.suppress_warnings_date = icdoPersonAccountGhdvHistory.suppress_warnings_date;
            abusPersonAccountGhdv.icdoPersonAccountGhdv.health_insurance_type_id = icdoPersonAccountGhdvHistory.health_insurance_type_id;
            abusPersonAccountGhdv.icdoPersonAccountGhdv.health_insurance_type_value = icdoPersonAccountGhdvHistory.health_insurance_type_value;
            abusPersonAccountGhdv.icdoPersonAccountGhdv.dental_insurance_type_id = icdoPersonAccountGhdvHistory.dental_insurance_type_id;
            abusPersonAccountGhdv.icdoPersonAccountGhdv.dental_insurance_type_value = icdoPersonAccountGhdvHistory.dental_insurance_type_value;
            abusPersonAccountGhdv.icdoPersonAccountGhdv.vision_insurance_type_id = icdoPersonAccountGhdvHistory.vision_insurance_type_id;
            abusPersonAccountGhdv.icdoPersonAccountGhdv.vision_insurance_type_value = icdoPersonAccountGhdvHistory.vision_insurance_type_value;
            abusPersonAccountGhdv.icdoPersonAccountGhdv.hmo_insurance_type_id = icdoPersonAccountGhdvHistory.hmo_insurance_type_id;
            abusPersonAccountGhdv.icdoPersonAccountGhdv.hmo_insurance_type_value = icdoPersonAccountGhdvHistory.hmo_insurance_type_value;
            abusPersonAccountGhdv.icdoPersonAccountGhdv.medicare_insurance_type_id = icdoPersonAccountGhdvHistory.medicare_insurance_type_id;
            abusPersonAccountGhdv.icdoPersonAccountGhdv.medicare_insurance_type_value = icdoPersonAccountGhdvHistory.medicare_insurance_type_value;
            abusPersonAccountGhdv.icdoPersonAccountGhdv.level_of_coverage_id = icdoPersonAccountGhdvHistory.level_of_coverage_id;
            abusPersonAccountGhdv.icdoPersonAccountGhdv.level_of_coverage_value = icdoPersonAccountGhdvHistory.level_of_coverage_value;
            abusPersonAccountGhdv.icdoPersonAccountGhdv.coverage_code = icdoPersonAccountGhdvHistory.coverage_code;
            abusPersonAccountGhdv.icdoPersonAccountGhdv.plan_option_id = icdoPersonAccountGhdvHistory.plan_option_id;
            abusPersonAccountGhdv.icdoPersonAccountGhdv.plan_option_value = icdoPersonAccountGhdvHistory.plan_option_value;
            abusPersonAccountGhdv.icdoPersonAccountGhdv.rate_structure_id = icdoPersonAccountGhdvHistory.rate_structure_id;
            abusPersonAccountGhdv.icdoPersonAccountGhdv.rate_structure_value = icdoPersonAccountGhdvHistory.rate_structure_value;
            abusPersonAccountGhdv.icdoPersonAccountGhdv.epo_org_id = icdoPersonAccountGhdvHistory.epo_org_id;
            abusPersonAccountGhdv.icdoPersonAccountGhdv.cobra_type_id = icdoPersonAccountGhdvHistory.cobra_type_id;
            abusPersonAccountGhdv.icdoPersonAccountGhdv.cobra_type_value = icdoPersonAccountGhdvHistory.cobra_type_value;
            abusPersonAccountGhdv.icdoPersonAccountGhdv.medicare_claim_no = icdoPersonAccountGhdvHistory.medicare_claim_no;
            abusPersonAccountGhdv.icdoPersonAccountGhdv.keeping_other_coverage_flag = icdoPersonAccountGhdvHistory.keeping_other_coverage_flag;
            abusPersonAccountGhdv.icdoPersonAccountGhdv.medicare_part_a_effective_date = icdoPersonAccountGhdvHistory.medicare_part_a_effective_date;
            abusPersonAccountGhdv.icdoPersonAccountGhdv.medicare_part_b_effective_date = icdoPersonAccountGhdvHistory.medicare_part_b_effective_date;
            abusPersonAccountGhdv.icdoPersonAccountGhdv.low_income_credit = icdoPersonAccountGhdvHistory.low_income_credit;            
            abusPersonAccountGhdv.icdoPersonAccountGhdv.reason_id = icdoPersonAccountGhdvHistory.reason_id;
            abusPersonAccountGhdv.icdoPersonAccountGhdv.reason_value = icdoPersonAccountGhdvHistory.reason_value;
            abusPersonAccountGhdv.icdoPersonAccount.provider_org_id = icdoPersonAccountGhdvHistory.provider_org_id;
            abusPersonAccountGhdv.icdoPersonAccountGhdv.employment_type_id = icdoPersonAccountGhdvHistory.employer_type_id;
            abusPersonAccountGhdv.icdoPersonAccountGhdv.employment_type_value = icdoPersonAccountGhdvHistory.employer_type_value;
            //as per discussion with Satya, 17Jun2011 added the below fields to be loaded from History
            abusPersonAccountGhdv.icdoPersonAccountGhdv.overridden_structure_code = icdoPersonAccountGhdvHistory.overridden_structure_code;
            abusPersonAccountGhdv.icdoPersonAccountGhdv.alternate_structure_code_id = icdoPersonAccountGhdvHistory.alternate_structure_code_id;
            abusPersonAccountGhdv.icdoPersonAccountGhdv.alternate_structure_code_value = icdoPersonAccountGhdvHistory.alternate_structure_code_value;
            return abusPersonAccountGhdv;
        }

        public string istrCoverageCode { get; set; }
    }
}
