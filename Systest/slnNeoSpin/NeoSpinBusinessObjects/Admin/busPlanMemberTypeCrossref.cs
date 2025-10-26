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

#endregion

namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// Class NeoSpin.BusinessObjects.busPlanMemberTypeCrossref:
    /// Inherited from busPlanMemberTypeCrossrefGen, the class is used to customize the business object busPlanMemberTypeCrossrefGen.
    /// </summary>
    [Serializable]
    public class busPlanMemberTypeCrossref : busPlanMemberTypeCrossrefGen
    {
        public bool IsCombinationAlreadyExists()
        {
            DataTable ldtbList = Select<cdoPlanMemberTypeCrossref>(new string[4] {enmPlanMemberTypeCrossref.employment_type_value.ToString(),
                                                                                  enmPlanMemberTypeCrossref.job_class_value.ToString(),
                                                                                  enmPlanMemberTypeCrossref.member_type_value.ToString(),
                                                                                  enmPlanMemberTypeCrossref.plan_id.ToString()},
                                                                   new object[4] {icdoPlanMemberTypeCrossref.employment_type_value,
                                                                                  icdoPlanMemberTypeCrossref.job_class_value,
                                                                                  icdoPlanMemberTypeCrossref.member_type_value,
                                                                                  icdoPlanMemberTypeCrossref.plan_id}, null, null);
            Collection<busPlanMemberTypeCrossref> lclbPlanMemberTypeCrossRef = GetCollection<busPlanMemberTypeCrossref>(ldtbList, "icdoPlanMemberTypeCrossref");
            if (lclbPlanMemberTypeCrossRef.Any(i => i.icdoPlanMemberTypeCrossref.plan_member_type_crossref_id != icdoPlanMemberTypeCrossref.plan_member_type_crossref_id))
            {
                return true;
            }
            return false;
        }
    }
}
