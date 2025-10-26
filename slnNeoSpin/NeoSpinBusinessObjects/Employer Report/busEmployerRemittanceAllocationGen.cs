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
    public partial class busEmployerRemittanceAllocation : busExtendBase
    {
        public busEmployerRemittanceAllocation()
        {

        }

        private cdoEmployerRemittanceAllocation _icdoEmployerRemittanceAllocation;
        public cdoEmployerRemittanceAllocation icdoEmployerRemittanceAllocation
        {
            get
            {
                return _icdoEmployerRemittanceAllocation;
            }

            set
            {
                _icdoEmployerRemittanceAllocation = value;
            }
        }

        private busRemittance _ibusRemittance;
        public busRemittance ibusRemittance
        {
            get
            {
                return _ibusRemittance;
            }

            set
            {
                _ibusRemittance = value;
            }
        }
        private busDeposit _ibusDeposit;
        public busDeposit ibusDeposit
        {
            get
            {
                return _ibusDeposit;
            }

            set
            {
                _ibusDeposit = value;
            }
        }

        public bool FindEmployerRemittanceAllocation(int Aintemployerremittanceallocationid)
        {
            bool lblnResult = false;
            if (_icdoEmployerRemittanceAllocation == null)
            {
                _icdoEmployerRemittanceAllocation = new cdoEmployerRemittanceAllocation();
            }
            if (_icdoEmployerRemittanceAllocation.SelectRow(new object[1] { Aintemployerremittanceallocationid }))
            {
                lblnResult = true;
            }
            return lblnResult;
        }

        public void LoadRemittance()
        {
            if (_ibusRemittance == null)
            {
                _ibusRemittance = new busRemittance();
            }
            _ibusRemittance.FindRemittance(_icdoEmployerRemittanceAllocation.remittance_id);
        }

        public void LoadDeposit()
        {
            if(_ibusDeposit == null)
            {
                _ibusDeposit = new busDeposit();
            }
            _ibusDeposit.FindDeposit(_ibusRemittance.icdoRemittance.deposit_id);
        }
    }
}
