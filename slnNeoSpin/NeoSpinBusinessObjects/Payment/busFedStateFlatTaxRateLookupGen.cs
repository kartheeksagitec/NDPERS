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
	public class busFedStateFlatTaxRateLookupGen : busMainBase
	{

		private Collection<busFedStateFlatTaxRate> _iclbFedStateFlatTaxRate;
		public Collection<busFedStateFlatTaxRate> iclbFedStateFlatTaxRate
		{
			get
			{
				return _iclbFedStateFlatTaxRate;
			}
			set
			{
				_iclbFedStateFlatTaxRate = value;
			}
		}

		public void LoadFedStateFlatTaxRates(DataTable adtbSearchResult)
		{
			_iclbFedStateFlatTaxRate = GetCollection<busFedStateFlatTaxRate>(adtbSearchResult, "icdoFedStateFlatTaxRate");
		}
	}
}
