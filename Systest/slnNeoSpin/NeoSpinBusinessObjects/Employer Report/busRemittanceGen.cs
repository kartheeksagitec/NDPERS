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
	public partial class busRemittance : busExtendBase
    {
		public busRemittance()
		{

		} 

		private cdoRemittance _icdoRemittance;
		public cdoRemittance icdoRemittance
		{
			get
			{
				return _icdoRemittance;
			}

			set
			{
				_icdoRemittance = value;
			}
		}

		public bool FindRemittance(int Aintremittanceid)
		{
			bool lblnResult = false;
			if (_icdoRemittance == null)
			{
				_icdoRemittance = new cdoRemittance();
			}
			if (_icdoRemittance.SelectRow(new object[1] { Aintremittanceid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
