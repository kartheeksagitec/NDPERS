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
	public class busBenefitDroCalculationGen : busExtendBase
    {
		public busBenefitDroCalculationGen()
		{

		}

        private cdoBenefitDroCalculation _icdoBenefitDroCalculation;
        public cdoBenefitDroCalculation icdoBenefitDroCalculation
        {
            get
            {
                return _icdoBenefitDroCalculation;
            }
            set
            {
                _icdoBenefitDroCalculation = value;
            }
        }

        public bool FindBenefitDroCalculation(int Aintdrocalculationid)
        {
            bool lblnResult = false;
            if (_icdoBenefitDroCalculation == null)
            {
                _icdoBenefitDroCalculation = new cdoBenefitDroCalculation();
            }
            if (_icdoBenefitDroCalculation.SelectRow(new object[1] { Aintdrocalculationid }))
            {
                lblnResult = true;
            }
            return lblnResult;
        }
        public bool FindBenefitDroCalculationByApplicationID(int Aintdroapplicationid)
        {
            bool lblnResult = false;
            if (_icdoBenefitDroCalculation == null)
            {
                _icdoBenefitDroCalculation = new cdoBenefitDroCalculation();
            }
            DataTable ldtbList = Select<cdoBenefitDroCalculation>(
                                  new string[1] { "dro_application_id" },  new object[1] { Aintdroapplicationid }, null, null);
            if (ldtbList.Rows.Count == 1)
            {
                _icdoBenefitDroCalculation.LoadData(ldtbList.Rows[0]);
                lblnResult = true;
            }
            else
            {
                lblnResult = false;
            }
            return lblnResult;
        }
	}
}
