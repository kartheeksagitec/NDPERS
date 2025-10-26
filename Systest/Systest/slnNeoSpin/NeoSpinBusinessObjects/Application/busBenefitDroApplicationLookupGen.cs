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
    public class busBenefitDroApplicationLookupGen : busMainBase
	{

		private Collection<busBenefitDroApplication> _iclbBenefitDroApplication;
		public Collection<busBenefitDroApplication> iclbBenefitDroApplication
		{
			get
			{
				return _iclbBenefitDroApplication;
			}
			set
			{
				_iclbBenefitDroApplication = value;
			}
		}

		public void LoadBenefitDroApplications(DataTable adtbSearchResult)
		{
			_iclbBenefitDroApplication = GetCollection<busBenefitDroApplication>(adtbSearchResult, "icdoBenefitDroApplication");
            foreach (busBenefitDroApplication lobjDroApplication in _iclbBenefitDroApplication)
            {
                lobjDroApplication.LoadAlternatePayee();
            }
		}
        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            if (aobjBus is busBenefitDroApplication)
            {
                busBenefitDroApplication lobjBenefitDroApplication = (busBenefitDroApplication)aobjBus;
                lobjBenefitDroApplication.ibusMember = new busPerson();
                lobjBenefitDroApplication.ibusMember.icdoPerson = new cdoPerson();
                lobjBenefitDroApplication.ibusMember.icdoPerson.LoadData(adtrRow);
                lobjBenefitDroApplication.ibusPlan = new busPlan();
                lobjBenefitDroApplication.ibusPlan.icdoPlan = new cdoPlan();
                lobjBenefitDroApplication.ibusPlan.icdoPlan.LoadData(adtrRow);
            }
        }
	}
}
