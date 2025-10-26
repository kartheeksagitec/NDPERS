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
	public class busBenefitApplicationPersonAccount : busBenefitApplicationPersonAccountGen
	{
        private busPersonAccountRetirement _ibusPersonAccountRetirement;
        public busPersonAccountRetirement ibusPersonAccountRetirement
        {
            get { return _ibusPersonAccountRetirement; }
            set { _ibusPersonAccountRetirement = value; }
        }
        public void LoadPersonAccountRetirement()
        {
            if (_ibusPersonAccountRetirement == null)
                _ibusPersonAccountRetirement = new busPersonAccountRetirement();
            _ibusPersonAccountRetirement.FindPersonAccountRetirement(icdoBenefitApplicationPersonAccount.person_account_id);
        }
	}
}
