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
	public partial class busSerPurServiceType : busExtendBase
    {
        private string _istrSelectedPlans;

        public string istrSelectedPlans
        {
            get { return _istrSelectedPlans; }
            set { _istrSelectedPlans = value; }
        }
	

	}
}
