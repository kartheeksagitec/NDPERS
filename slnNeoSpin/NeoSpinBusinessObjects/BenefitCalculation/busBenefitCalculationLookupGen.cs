#region Using directives

using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using NeoSpin.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;

#endregion

namespace NeoSpin.BusinessObjects
{
	[Serializable]
	public class busBenefitCalculationLookupGen : busMainBase
	{
        private Collection<busBenefitCalculation> _iclbBenefitCalculation;
        public Collection<busBenefitCalculation> iclbBenefitCalculation
        {
            get { return _iclbBenefitCalculation; }
            set { _iclbBenefitCalculation = value; }
        }

        public void LoadBenefitCalculations(DataTable adtbSearchResult)
        {
            if (_iclbBenefitCalculation == null)
                _iclbBenefitCalculation = new Collection<busBenefitCalculation>();
            foreach (DataRow ldtrCalculation in adtbSearchResult.Rows)
            {
                busBenefitCalculation lobjBenefitCalculation = new busBenefitCalculation();
                lobjBenefitCalculation.icdoBenefitCalculation = new cdoBenefitCalculation();
                lobjBenefitCalculation.ibusMember = new busPerson();
                lobjBenefitCalculation.ibusMember.icdoPerson = new cdoPerson();
                lobjBenefitCalculation.ibusPlan = new busPlan();
                lobjBenefitCalculation.ibusPlan.icdoPlan = new cdoPlan();

                lobjBenefitCalculation.icdoBenefitCalculation.LoadData(ldtrCalculation);
                lobjBenefitCalculation.ibusMember.icdoPerson.LoadData(ldtrCalculation);
                lobjBenefitCalculation.ibusPlan.icdoPlan.LoadData(ldtrCalculation);

                _iclbBenefitCalculation.Add(lobjBenefitCalculation);
            }
        }
	}
}
