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
	public partial class busEmployerRemittanceAllocationLookup  : busMainBase
	{

		private Collection<busEmployerRemittanceAllocation> _icolEmployerRemittanceAllocation;
		public Collection<busEmployerRemittanceAllocation> icolEmployerRemittanceAllocation
		{
			get
			{
				return _icolEmployerRemittanceAllocation;
			}

			set
			{
				_icolEmployerRemittanceAllocation = value;
			}
		}

		public void LoadEmployerRemittanceAllocation(DataTable adtbSearchResult)
		{
			_icolEmployerRemittanceAllocation = GetCollection<busEmployerRemittanceAllocation>(adtbSearchResult, "icdoEmployerRemittanceAllocation");
		}
	}
}
