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
	public class busPeoplesoftPlanOrgCrossRefGen : busExtendBase
    {
		public busPeoplesoftPlanOrgCrossRefGen()
		{

		}

		private cdoPeoplesoftPlanOrgCrossRef _icdoPeoplesoftPlanOrgCrossRef;
		public cdoPeoplesoftPlanOrgCrossRef icdoPeoplesoftPlanOrgCrossRef
		{
			get
			{
				return _icdoPeoplesoftPlanOrgCrossRef;
			}
			set
			{
				_icdoPeoplesoftPlanOrgCrossRef = value;
			}
		}

		public bool FindPeoplesoftPlanOrgCrossRef(int Aintsgtpeoplesoftplanorgcrossrefid)
		{
			bool lblnResult = false;
			if (_icdoPeoplesoftPlanOrgCrossRef == null)
			{
				_icdoPeoplesoftPlanOrgCrossRef = new cdoPeoplesoftPlanOrgCrossRef();
			}
			if (_icdoPeoplesoftPlanOrgCrossRef.SelectRow(new object[1] { Aintsgtpeoplesoftplanorgcrossrefid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
