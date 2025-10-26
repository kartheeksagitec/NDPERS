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
	public partial class busRemittanceLookup  : busMainBase
	{

		private Collection<busRemittance> _icolRemittance;
		public Collection<busRemittance> icolRemittance
		{
			get
			{
				return _icolRemittance;
			}

			set
			{
				_icolRemittance = value;
			}
		}

		public void LoadRemittance(DataTable adtbSearchResult)
		{
			_icolRemittance = GetCollection<busRemittance>(adtbSearchResult, "icdoRemittance");
		}
	}
}
