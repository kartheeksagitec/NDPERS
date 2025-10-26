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
    public class busBenefitApplicationLookupGen : busMainBase
	{

		private Collection<busBenefitApplication> _iclbApplication;
		public Collection<busBenefitApplication> iclbApplication
		{
			get
			{
				return _iclbApplication;
			}
			set
			{
				_iclbApplication = value;
			}
		}

		public void LoadApplications(DataTable adtbSearchResult)
		{
			_iclbApplication = GetCollection<busBenefitApplication>(adtbSearchResult, "icdoBenefitApplication");
		}
	}
}
