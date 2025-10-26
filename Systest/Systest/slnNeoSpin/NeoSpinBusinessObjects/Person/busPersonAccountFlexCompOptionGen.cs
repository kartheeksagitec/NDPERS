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
	public class busPersonAccountFlexCompOptionGen : busExtendBase
    {
		public busPersonAccountFlexCompOptionGen()
		{

		}

		private cdoPersonAccountFlexCompOption _icdoPersonAccountFlexCompOption;
		public cdoPersonAccountFlexCompOption icdoPersonAccountFlexCompOption
		{
			get
			{
				return _icdoPersonAccountFlexCompOption;
			}
			set
			{
				_icdoPersonAccountFlexCompOption = value;
			}
		}

		private busPersonAccountFlexComp _ibusPersonAccountFlexComp;
		public busPersonAccountFlexComp ibusPersonAccountFlexComp
		{
			get
			{
				return _ibusPersonAccountFlexComp;
			}
			set
			{
				_ibusPersonAccountFlexComp = value;
			}
		}

		public bool FindPersonAccountFlexCompOption(int Aintaccountflexcompoptionid)
		{
			bool lblnResult = false;
			if (_icdoPersonAccountFlexCompOption == null)
			{
				_icdoPersonAccountFlexCompOption = new cdoPersonAccountFlexCompOption();
			}
			if (_icdoPersonAccountFlexCompOption.SelectRow(new object[1] { Aintaccountflexcompoptionid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}		
	}
}
