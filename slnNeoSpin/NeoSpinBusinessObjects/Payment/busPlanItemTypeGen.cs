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
	public class busPlanItemTypeGen : busExtendBase
    {
		public busPlanItemTypeGen()
		{

		}

		private cdoPlanItemType _icdoPlanItemType;
		public cdoPlanItemType icdoPlanItemType
		{
			get
			{
				return _icdoPlanItemType;
			}
			set
			{
				_icdoPlanItemType = value;
			}
		}

		public bool FindPlanItemType(int Aintplanitemtypeid)
		{
			bool lblnResult = false;
			if (_icdoPlanItemType == null)
			{
				_icdoPlanItemType = new cdoPlanItemType();
			}
			if (_icdoPlanItemType.SelectRow(new object[1] { Aintplanitemtypeid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
