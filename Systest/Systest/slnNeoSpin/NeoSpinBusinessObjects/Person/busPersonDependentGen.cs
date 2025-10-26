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
	public class busPersonDependentGen : busExtendBase
	{
		public busPersonDependentGen()
		{

		} 

		private cdoPersonDependent _icdoPersonDependent;
		public cdoPersonDependent icdoPersonDependent
		{
			get
			{
				return _icdoPersonDependent;
			}

			set
			{
				_icdoPersonDependent = value;
			}
		}

		public bool FindPersonDependent(int Aintdependentid)
		{
			bool lblnResult = false;
			if (_icdoPersonDependent == null)
			{
				_icdoPersonDependent = new cdoPersonDependent();
			}
			if (_icdoPersonDependent.SelectRow(new object[1] { Aintdependentid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
