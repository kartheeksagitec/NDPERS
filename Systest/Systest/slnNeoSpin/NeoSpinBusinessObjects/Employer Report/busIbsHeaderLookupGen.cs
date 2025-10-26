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
	public partial class busIbsHeaderLookup  : busMainBase
	{

		private Collection<busIbsHeader> _icolIbsHeader;
		public Collection<busIbsHeader> icolIbsHeader
		{
			get
			{
				return _icolIbsHeader;
			}

			set
			{
				_icolIbsHeader = value;
			}
		}

		public void LoadIbsHeader(DataTable adtbSearchResult)
		{
			_icolIbsHeader = GetCollection<busIbsHeader>(adtbSearchResult, "icdoIbsHeader");
		}
	}
}
