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
	public class busBenefitCalculationFasMonths : busBenefitCalculationFasMonthsGen
	{
        private busPersonAccount _ibusPersonAccount;
        public busPersonAccount ibusPersonAccount
        {
            get { return _ibusPersonAccount; }
            set { _ibusPersonAccount = value; }
        }

        public decimal normal_salary_amount
        {
            get 
            {
                if (icdoBenefitCalculationFasMonths.projected_flag == busConstant.Flag_No)
                    return icdoBenefitCalculationFasMonths.salary_amount;
                else
                    return 0M;
            }
        }

        public decimal projected_salary_amount
        {
            get
            {
                if (icdoBenefitCalculationFasMonths.projected_flag == busConstant.Flag_Yes)
                    return icdoBenefitCalculationFasMonths.salary_amount;
                else
                    return 0M;
            }
        }

        //PIR 23571, 24269
        public string istrPlanName { get; set; }
	}
}
