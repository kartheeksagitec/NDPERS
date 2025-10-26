#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
	[Serializable]
	public partial class busOrgPlan : busExtendBase
	{
		public busOrgPlan()
		{

		} 

		private cdoOrgPlan _icdoOrgPlan;
		public cdoOrgPlan icdoOrgPlan
		{
			get
			{
				return _icdoOrgPlan;
			}

			set
			{
				_icdoOrgPlan = value;
			}
		}

		public bool FindOrgPlan(int Aintorgplanid)
		{
			bool lblnResult = false;
			if (_icdoOrgPlan == null)
			{
				_icdoOrgPlan = new cdoOrgPlan();
			}
			if (_icdoOrgPlan.SelectRow(new object[1] { Aintorgplanid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
