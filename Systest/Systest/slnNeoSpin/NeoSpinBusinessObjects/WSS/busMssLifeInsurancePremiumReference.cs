using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sagitec.BusinessObjects;
using Sagitec.CustomDataObjects;
using System.Data;
using System.Collections.ObjectModel;
using NeoSpin.CustomDataObjects;
using NeoSpin.DataObjects;
using Sagitec.Common;
using System.Collections;


namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busMssLifeInsurancePremiumReference : busExtendBase
    {
        public Collection<busOrgPlanLifeRate> iclbLifeRate { get; set; }

        public void LoadLifeInsuranceRate()
        {
            DataTable ldtbList = busGlobalFunctions.LoadLifeRateCacheData(iobjPassInfo);

            if (ldtbList.Rows.Count > 0)
            {
                DateTime ldtLatestEffectiveDate = ldtbList.AsEnumerable().OrderByDescending(lrow => lrow.Field<DateTime>("effective_date")).FirstOrDefault().Field<DateTime>("effective_date");

                DataTable ldtbResult = ldtbList.AsEnumerable().Where(lrow => lrow.Field<DateTime>("effective_date") >= ldtLatestEffectiveDate).AsDataTable();

                iclbLifeRate = GetCollection<busOrgPlanLifeRate>(ldtbResult, "icdoOrgPlanLifeRate");
            }
        }
    }
}

