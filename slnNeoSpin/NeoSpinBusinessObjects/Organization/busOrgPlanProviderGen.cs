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
	public partial class busOrgPlanProvider : busExtendBase
    {
		public busOrgPlanProvider()
		{

		} 

		private cdoOrgPlanProvider _icdoOrgPlanProvider;
		public cdoOrgPlanProvider icdoOrgPlanProvider
		{
			get
			{
				return _icdoOrgPlanProvider;
			}

			set
			{
				_icdoOrgPlanProvider = value;
			}
		}

		public bool FindOrgPlanProvider( int lintOrgPlanProviderID)
		{
			bool lblnResult = false;
			if (_icdoOrgPlanProvider == null)
			{
				_icdoOrgPlanProvider = new cdoOrgPlanProvider();
			}
            if (_icdoOrgPlanProvider.SelectRow(new object[1] { lintOrgPlanProviderID }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
