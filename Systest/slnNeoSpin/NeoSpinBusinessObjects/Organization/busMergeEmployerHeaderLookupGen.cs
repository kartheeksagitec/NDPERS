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
	public class busMergeEmployerHeaderLookupGen : busMainBase
	{

		private Collection<busMergeEmployerHeader> _iclbMergeEmployerHeader;
		public Collection<busMergeEmployerHeader> iclbMergeEmployerHeader
		{
			get
			{
				return _iclbMergeEmployerHeader;
			}
			set
			{
				_iclbMergeEmployerHeader = value;
			}
		}

		public void LoadMergeEmployerHeaders(DataTable adtbSearchResult)
		{
			_iclbMergeEmployerHeader = GetCollection<busMergeEmployerHeader>(adtbSearchResult, "icdoMergeEmployerHeader");
            foreach(busMergeEmployerHeader lobjMergeEmployerHeader in _iclbMergeEmployerHeader)
            {
              lobjMergeEmployerHeader.LoadFromEmployer();
              lobjMergeEmployerHeader.LoadToEmployer();
            }
		}
	}
}
