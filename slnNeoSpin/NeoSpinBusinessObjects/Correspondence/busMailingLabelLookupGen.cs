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
	public partial class busMailingLabelLookup  : busMainBase
	{

		private Collection<busMailingLabel> _icolMailingLabel;
		public Collection<busMailingLabel> icolMailingLabel
		{
			get
			{
				return _icolMailingLabel;
			}

			set
			{
				_icolMailingLabel = value;
			}
		}

		public void LoadMailingLabel(DataTable adtbSearchResult)
		{
			_icolMailingLabel = GetCollection<busMailingLabel>(adtbSearchResult, "icdoMailingLabel");
		}
	}
}
