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
	public class busCaseLookupGen : busMainBase
	{
		public Collection<busCase> iclbCase { get; set; }



		public virtual void LoadCases(DataTable adtbSearchResult)
		{
			iclbCase = GetCollection<busCase>(adtbSearchResult, "icdoCase");
		}
	}
}
