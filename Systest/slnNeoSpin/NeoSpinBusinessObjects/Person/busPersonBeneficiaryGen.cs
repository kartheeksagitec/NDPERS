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
	public class busPersonBeneficiaryGen : busExtendBase
	{
        public busPersonBeneficiaryGen()
		{

		} 

		private cdoPersonBeneficiary _icdoPersonBeneficiary;
		public cdoPersonBeneficiary icdoPersonBeneficiary
		{
			get
			{
				return _icdoPersonBeneficiary;
			}

			set
			{
				_icdoPersonBeneficiary = value;
			}
		}

		public bool FindPersonBeneficiary(int Aintbeneficiaryid)
		{
			bool lblnResult = false;
			if (_icdoPersonBeneficiary == null)
			{
				_icdoPersonBeneficiary = new cdoPersonBeneficiary();
			}
			if (_icdoPersonBeneficiary.SelectRow(new object[1] { Aintbeneficiaryid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
