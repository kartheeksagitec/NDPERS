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
	public partial class busOrgContactAddress : busExtendBase
    {
		public busOrgContactAddress()
		{

		}       
		private cdoOrgContactAddress _icdoOrgContactAddress;
		public cdoOrgContactAddress icdoOrgContactAddress
		{
			get
			{
				return _icdoOrgContactAddress;
			}

			set
			{
                _icdoOrgContactAddress = value;
			}
		}

        public bool FindOrgContactAddress(int Aintcontactorgaddressid)
        {
            bool lblnResult = false;
            if (_icdoOrgContactAddress == null)
            {
                _icdoOrgContactAddress = new cdoOrgContactAddress();
            }
            if (_icdoOrgContactAddress.SelectRow(new object[1] { Aintcontactorgaddressid }))
            {
                lblnResult = true;
            }
            return lblnResult;
        }

	}
}
