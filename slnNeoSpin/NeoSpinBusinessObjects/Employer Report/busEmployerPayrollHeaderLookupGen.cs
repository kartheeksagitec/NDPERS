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
	public partial class busEmployerPayrollHeaderLookup  : busMainBase
	{

		private Collection<busEmployerPayrollHeader> _icolEmployerPayrollHeader;
		public Collection<busEmployerPayrollHeader> icolEmployerPayrollHeader
		{
			get
			{
				return _icolEmployerPayrollHeader;
			}

			set
			{
				_icolEmployerPayrollHeader = value;
			}
		}

		public void LoadEmployerPayrollHeader(DataTable adtbSearchResult)
		{
			_icolEmployerPayrollHeader = GetCollection<busEmployerPayrollHeader>(adtbSearchResult, "icdoEmployerPayrollHeader");
		}
	}
}
