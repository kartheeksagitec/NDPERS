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
	public class busBenefitCalculationOptions : busBenefitCalculationOptionsGen
	{
        private busBenefitCalculationPayee _ibusBenefitCalculationPayee;
        public busBenefitCalculationPayee ibusBenefitCalculationPayee
        {
            get { return _ibusBenefitCalculationPayee; }
            set { _ibusBenefitCalculationPayee = value; }
        }

        private busBenefitProvisionBenefitOption _ibusBenefitProvisionBenefitOption;
        public busBenefitProvisionBenefitOption ibusBenefitProvisionBenefitOption
        {
            get { return _ibusBenefitProvisionBenefitOption; }
            set { _ibusBenefitProvisionBenefitOption = value; }
        }       

        public void LoadBenefitProvisionOption()
        {
            if (ibusBenefitProvisionBenefitOption == null)
                ibusBenefitProvisionBenefitOption = new busBenefitProvisionBenefitOption();
            ibusBenefitProvisionBenefitOption.FindBenefitProvisionBenefitOption(icdoBenefitCalculationOptions.benefit_provision_benefit_option_id);
        }

        public void LoadBenefitCalculationPayee()
        {
            if (ibusBenefitCalculationPayee == null)
                ibusBenefitCalculationPayee = new  busBenefitCalculationPayee();
            ibusBenefitCalculationPayee.FindBenefitCalculationPayee(icdoBenefitCalculationOptions.benefit_calculation_payee_id);
        }

        private busBenefitCalculation _ibusBenefitCalculation;
        public busBenefitCalculation ibusBenefitCalculation
        {
            get { return _ibusBenefitCalculation; }
            set { _ibusBenefitCalculation = value; }
        }

        public void LoadBenefitCalculation()
        {
            if (ibusBenefitCalculation == null)
                ibusBenefitCalculation = new busBenefitCalculation();
            ibusBenefitCalculation.FindBenefitCalculation(icdoBenefitCalculationOptions.benefit_calculation_id);
        }

        # region UCS 24
        public decimal idecBenefitAmountAfterDeductions { get; set; }
        public bool iblnIsPersonEligibleForEarly { get; set; }
        #endregion

        public decimal idecTotalDeductions { get; set; }
    }
}
