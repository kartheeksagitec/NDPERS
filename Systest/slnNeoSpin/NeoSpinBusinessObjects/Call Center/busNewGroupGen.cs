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
	public partial class busNewGroup : busExtendBase
    {
		public busNewGroup()
		{

		} 

		private cdoNewGroup _icdoNewGroup;
		public cdoNewGroup icdoNewGroup
		{
			get
			{
				return _icdoNewGroup;
			}

			set
			{
				_icdoNewGroup = value;
			}
		}

		public bool FindNewGroup(int Aintnewgroupid)
		{
			bool lblnResult = false;
			if (_icdoNewGroup == null)
			{
				_icdoNewGroup = new cdoNewGroup();
			}
			if (_icdoNewGroup.SelectRow(new object[1] { Aintnewgroupid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
