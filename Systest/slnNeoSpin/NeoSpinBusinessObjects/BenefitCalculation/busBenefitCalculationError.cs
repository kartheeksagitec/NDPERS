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
	public class busBenefitCalculationError : busExtendBase
    {

        private cdoBenefitCalculationError _icdoBenefitCalculationError;
        public cdoBenefitCalculationError icdoBenefitCalculationError
        {
            get
            {
                return _icdoBenefitCalculationError;
            }
            set
            {
                _icdoBenefitCalculationError = value;
            }
        }

        public bool FindBenefitCalculationError(int Aintbenefitcalculationerrorid)
        {
            bool lblnResult = false;
            if (_icdoBenefitCalculationError == null)
            {
                _icdoBenefitCalculationError = new cdoBenefitCalculationError();
            }
            if (_icdoBenefitCalculationError.SelectRow(new object[1] { Aintbenefitcalculationerrorid }))
            {
                lblnResult = true;
            }
            return lblnResult;
        }
	}
}
