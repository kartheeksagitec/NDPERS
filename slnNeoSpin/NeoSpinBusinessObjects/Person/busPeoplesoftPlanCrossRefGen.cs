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
	public class busPeoplesoftPlanCrossRefGen : busExtendBase
    {
		public busPeoplesoftPlanCrossRefGen()
		{

		}

		private cdoPeoplesoftPlanCrossRef _icdoPeoplesoftPlanCrossRef;
		public cdoPeoplesoftPlanCrossRef icdoPeoplesoftPlanCrossRef
		{
			get
			{
				return _icdoPeoplesoftPlanCrossRef;
			}
			set
			{
				_icdoPeoplesoftPlanCrossRef = value;
			}
		}        

		public bool FindPeoplesoftPlanCrossRef(int Aintsgtpeoplesoftplancrossrefid)
		{
			bool lblnResult = false;
			if (_icdoPeoplesoftPlanCrossRef == null)
			{
				_icdoPeoplesoftPlanCrossRef = new cdoPeoplesoftPlanCrossRef();
			}
			if (_icdoPeoplesoftPlanCrossRef.SelectRow(new object[1] { Aintsgtpeoplesoftplancrossrefid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
