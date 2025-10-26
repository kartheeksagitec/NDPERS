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
	public class busBenefitRHICOption : busBenefitRHICOptionGen
	{
        private busBenefitProvisionBenefitOption _ibusBenefitProvisionOption;
        public busBenefitProvisionBenefitOption ibusBenefitProvisionOption
        {
            get { return _ibusBenefitProvisionOption; }
            set { _ibusBenefitProvisionOption = value; }
        }

        public void LoadBenefitProvisionOption()
        {
            if (ibusBenefitProvisionOption == null)
                ibusBenefitProvisionOption = new busBenefitProvisionBenefitOption();
            ibusBenefitProvisionOption.FindBenefitProvisionBenefitOption(icdoBenefitRhicOption.benefit_provision_benefit_option_id);
        }

        private busBenefitCalculation _ibusBenefitCalculation;
        public busBenefitCalculation ibusBenefitCalculation
        {
            get { return _ibusBenefitCalculation; }
            set { _ibusBenefitCalculation = value; }
        }

        public void LoadBenefitCalculation()
        {
            if (_ibusBenefitCalculation == null)
                _ibusBenefitCalculation = new busBenefitCalculation();
            _ibusBenefitCalculation.FindBenefitCalculation(icdoBenefitRhicOption.benefit_calculation_id);
        }
	
	}
}
