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
    public partial class busDeposit : busExtendBase
    {
        public busDeposit()
        {

        }

        private cdoDeposit _icdoDeposit;
        public cdoDeposit icdoDeposit
        {
            get
            {
                return _icdoDeposit;
            }

            set
            {
                _icdoDeposit = value;
            }
        }

        public bool FindDeposit(int Aintdepositid)
        {
            bool lblnResult = false;
            if (_icdoDeposit == null)
            {
                _icdoDeposit = new cdoDeposit();
            }
            if (_icdoDeposit.SelectRow(new object[1] { Aintdepositid }))
            {
                lblnResult = true;
            }
            return lblnResult;
        }
    }
}
