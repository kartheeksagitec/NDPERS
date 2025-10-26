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
	public class busBenefitDeductionSummary : busBenefitDeductionSummaryGen
	{

        public bool FindBenefitDeductionSummaryByCalId(int aintBenefitCalculationid)
        {
            bool lblFindFlag = false;
            if(icdoBenefitDeductionSummary==null)
                icdoBenefitDeductionSummary = new cdoBenefitDeductionSummary();
            DataTable ldtbList = Select<cdoBenefitDeductionSummary>(
                           new string[1] { "benefit_calculation_id" },
                           new object[1] { aintBenefitCalculationid }, null, null);
            if (ldtbList.Rows.Count > 0)
            {
                icdoBenefitDeductionSummary.LoadData(ldtbList.Rows[0]);
                lblFindFlag = true;
            }
            return lblFindFlag;
        }
    }
}
