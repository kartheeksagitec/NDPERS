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
	public class busBenefitCalculationPersonAccount : busBenefitCalculationPersonAccountGen
	{

        private busPersonAccountRetirement _ibusPersonAccountRetirement;
        public busPersonAccountRetirement ibusPersonAccountRetirement
        {
            get { return _ibusPersonAccountRetirement; }
            set { _ibusPersonAccountRetirement = value; }
        }

        public void LoadPersonAccountRetirement()
        {
            if (_ibusPersonAccountRetirement == null)
                _ibusPersonAccountRetirement = new  busPersonAccountRetirement();
            _ibusPersonAccountRetirement.FindPersonAccountRetirement(icdoBenefitCalculationPersonAccount.person_account_id);
        }

        public busBenefitCalculation ibusBenefitCalculation { get; set; }

        public void LoadBenefitCalculation()
        {
            if (ibusBenefitCalculation.IsNull())
                ibusBenefitCalculation = new busBenefitCalculation { icdoBenefitCalculation = new cdoBenefitCalculation() };
            ibusBenefitCalculation.FindBenefitCalculation(icdoBenefitCalculationPersonAccount.benefit_calculation_id);
        }
	}
}
