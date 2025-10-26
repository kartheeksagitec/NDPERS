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
	public class busFedStateTaxRateLookupGen : busMainBase
	{

		private Collection<busFedStateTaxRate> _iclbFedStateTaxRate;
        public Collection<busFedStateTaxRate> iclbFedStateTaxRate
		{
			get
			{
				return _iclbFedStateTaxRate;
			}
			set
			{
				_iclbFedStateTaxRate = value;
			}
		}

		public void LoadFedStateTaxRates(DataTable adtbSearchResult)
		{
			_iclbFedStateTaxRate = GetCollection<busFedStateTaxRate>(adtbSearchResult, "icdoFedStateTaxRate");
		}
	}
}
