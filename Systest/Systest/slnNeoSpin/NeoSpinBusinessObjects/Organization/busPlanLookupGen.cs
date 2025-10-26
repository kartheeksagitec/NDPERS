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
	public partial class busPlanLookup  : busMainBase
	{

		private Collection<busPlan> _icolPlan;
		public Collection<busPlan> icolPlan
		{
			get
			{
				return _icolPlan;
			}

			set
			{
				_icolPlan = value;
			}
		}

		public void LoadPlan(DataTable adtbSearchResult)
		{
			_icolPlan = GetCollection<busPlan>(adtbSearchResult, "icdoPlan");
		}
	}
}
