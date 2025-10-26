using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Collections.ObjectModel;
using Sagitec.BusinessObjects;
using System.Data;
using NeoSpin.BusinessObjects;
using Sagitec.DataObjects;
using Sagitec.DBUtility;
using Sagitec.CustomDataObjects;
using NeoSpin.CustomDataObjects;
using Sagitec.Common;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busGLPeopleSoftFileOut:busFileBaseOut
    {
        public busGLPeopleSoftFileOut()
        {
        }

        private Collection<busGLTransaction> _iclbGLTransaction;
        public Collection<busGLTransaction> iclbGLTransaction
        {
            get { return _iclbGLTransaction; }
            set { _iclbGLTransaction = value; }
        }

        public override void InitializeFile()
        {
            istrFileName = busConstant.GLFileOutFileName + busConstant.FileFormattxt;
        }

        public override bool ValidateFile()
        {
            bool lblnFlag = true;
            if (_iclbGLTransaction.Count == 0)
            {
                this.istrError = "No Records Found.";
                lblnFlag = false;
            }
            if (!IsTotalAmountValid())
            {
                this.istrError = "Sum of Debit and Credit Amount doest not match.";
                lblnFlag = false;
            }
            return lblnFlag;
        }

        public DateTime idtPostingDate { get; set; }

        public void LoadGLTransaction(DataTable ldtbGLTransaction)
        {
            idtPostingDate = (DateTime)iarrParameters[0];
            ldtbGLTransaction = DBFunction.DBSelect("cdoGLTransaction.GetTransactionForPeopleSoft",
                new object[1] { idtPostingDate }
                , iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);       

           _iclbGLTransaction = new Collection<busGLTransaction>();
            foreach (DataRow dr in ldtbGLTransaction.Rows)
            {
                busGLTransaction lobjTransaction = new busGLTransaction();
                lobjTransaction.icdoGlTransaction = new cdoGlTransaction();
                lobjTransaction.icdoGlTransaction.LoadData(dr);
                lobjTransaction.ldclTotalAmount = Convert.ToDecimal(dr["TOTAL_AMOUNT"]);
                lobjTransaction.ldteBatchRunDate = idtPostingDate; //PROD PIR 6579
                lobjTransaction.LoadFiscalPeriod();
                lobjTransaction.LoadAccountNumber();
                lobjTransaction.LoadPlanCode();
                _iclbGLTransaction.Add(lobjTransaction);
            }
        }        

        public bool IsTotalAmountValid()
        {
            decimal ldclTotal=0;
            foreach (busGLTransaction lobjGL in _iclbGLTransaction)
            {
                ldclTotal += lobjGL.ldclTotalAmount;
            }
            if (ldclTotal.Equals(0))
                return true;
            else
                return false;
        }       

        public override void FinalizeFile()
        {
            try
            {
                DBFunction.DBNonQuery("cdoGLTransaction.UpdateGLTransactionExtractDate", new object[2] { istrFileName, idtPostingDate }, iobjPassInfo.iconFramework,
                    iobjPassInfo.itrnFramework);
                base.FinalizeFile();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
