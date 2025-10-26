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
    class busTFFRGroupPaymentInboundFile : busFileBase
    {
        private bool lblnRemittanceCreated = false;
        bool lblnErrorFound;
        public busTFFRGroupPaymentInboundFile()
        {
            iintDepositID = Convert.ToInt32(DBFunction.DBExecuteScalar("cdoRemittance.GetTFFRDepositID",  iobjPassInfo.iconFramework,  iobjPassInfo.itrnFramework));
        }
        #region Business Properties
        private Collection<busRemittance> _iclbRemittance;
        public Collection<busRemittance> iclbRemittance
        {
            get
            {
                return _iclbRemittance;
            }

            set
            {
                _iclbRemittance = value;
            }
        }
        private int _iintDepositID;
        public int iintDepositID
        {
            get
            {
                return _iintDepositID;
            }

            set
            {
                _iintDepositID = value;
            }
        }
        private int _iintPersonID;
        public int iintPersonID
        {
            get
            {
                return _iintPersonID;
            }

            set
            {
                _iintPersonID = value;
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
        private decimal _idecTotalDepositAmount;
        public decimal idecTotalDepositAmount
        {
            get
            {
                return _idecTotalDepositAmount;
            }

            set
            {
                _idecTotalDepositAmount = value;
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
        #endregion
        public override busBase NewDetail()
        {
            _ibusRemittance = new busRemittance();
            _ibusRemittance.icdoRemittance = new cdoRemittance();
            return _ibusRemittance;
        }
        public void LoadDeposit()
        {
            if (_ibusDeposit == null)
            {
                _ibusDeposit = new busDeposit();
            }
            _ibusDeposit.FindDeposit(iintDepositID);
        }
        public override void ProcessDetail()
        {
            lblnErrorFound = false;

            ArrayList larrErrors = new ArrayList();
            utlError lobjError = new utlError();

            DataTable ldtbMbr = busNeoSpinBase.Select<cdoPerson>(
                                           new string[1] { "ssn" }, new object[1] { _ibusRemittance.istrSSN }, null, null);
            if (ldtbMbr.Rows.Count > 0)
            {
                iintPersonID = Convert.ToInt32(ldtbMbr.Rows[0]["person_id"]);
            }
            else
            {
                lobjError = new utlError();
                lobjError.istrErrorID = "4143";
                lobjError.istrErrorMessage = "Invalid SSN :" + _ibusRemittance.istrSSN;
                larrErrors.Add(lobjError);
                lblnErrorFound = true;
            }
            if (_iclbRemittance == null)
            {
                _iclbRemittance = new Collection<busRemittance>();
            }
            _ibusRemittance.icdoRemittance.deposit_id = iintDepositID;
            _ibusRemittance.icdoRemittance.person_id = iintPersonID;
            _ibusRemittance.icdoRemittance.plan_id = busConstant.PlanIdGroupHealth;
            _ibusRemittance.icdoRemittance.remittance_amount = _ibusRemittance.idecDepositAmount;
            _idecTotalDepositAmount += _ibusRemittance.idecDepositAmount;
            _ibusRemittance.icdoRemittance.remittance_type_value = busConstant.RemittanceTypeIBSDeposit;
            _iclbRemittance.Add(_ibusRemittance);

            if (lblnErrorFound)
                _ibusRemittance.iarrErrors = larrErrors;
        }
        public override busFileBase.sfwOnFileError ContinueOnValueError(string astrObjectField, out string astrValue)
        {
            astrValue = "";
            string lstrObjectField;
            if (astrObjectField.IndexOf(".") > -1)
                lstrObjectField = astrObjectField.Substring(astrObjectField.LastIndexOf(".") + 1);
            else
                lstrObjectField = astrObjectField;

            switch (lstrObjectField.ToLower())
            {
                default: return base.ContinueOnValueError(astrObjectField, out astrValue);
            }
        }
        public override void InitializeFile()
        {
            base.InitializeFile();
        }
        public override bool ValidateFile()
        {
            //If any detail records with errors, let the header completes the process with warnings.
            foreach (busRemittance lobjRemittance in _iclbRemittance)
            {
                if (lobjRemittance.iarrErrors.Count > 0) return true;
            }

            if (iintDepositID == 0)
            {
                this.istrError = "No Deposit Found!";
                return false;
            }
            ArrayList larrList = new ArrayList();
            LoadDeposit();
            if (_ibusDeposit.icdoDeposit.deposit_amount == _idecTotalDepositAmount)
            {
                foreach (busRemittance lobjRemittance in _iclbRemittance)
                {
                    lobjRemittance.icdoRemittance.Insert();
                }
                ibusDeposit.LoadDepositTape();
                ibusDeposit.ibusDepositTape.LoadDeposits();
                ibusDeposit.ibusDepositTape.LoadDepositsCountAndTotalAmount();
                //Change Deposit Status To Valid if deposit and sum of remittance amount match
                larrList = ibusDeposit.ibusDepositTape.ValidateDepositTape();
                if ((larrList.Count > 0) && (larrList[0] is utlError))
                {
                    utlError lobjutlError = (utlError)larrList[0];
                    this.istrError = lobjutlError.istrErrorMessage;
                    return false;
                }
                ibusDeposit.icdoDeposit.Select();
                ibusDeposit.LoadRemittances();
                // Applying the Deposit Tape.
                larrList = ibusDeposit.btnApply_Click();
                if (larrList.Count > 0)
                {
                    if ((larrList.Count > 0) && (larrList[0] is utlError))
                    {
                        utlError lobjError = (utlError)larrList[0];
                        this.istrError = lobjError.istrErrorMessage;
                        return false;
                    }
                }
                return true;
            }
            else
            {
                this.istrError = "Sum of Total Remittance amount does not match with Deposit Amount.";
                return false;
            }
        }  
    }
}
