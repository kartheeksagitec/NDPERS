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

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busGLReCreateFileOut : busFileBaseOut
    {
        public busGLReCreateFileOut()
        {
        }

        private string _lstrFileName;
        public string lstrFileName
        {
            get { return _lstrFileName; }
            set { _lstrFileName = value; }
        }

        public override void InitializeFile()
        {
            istrFileName = _lstrFileName;
        }

        private Collection<busGLTransaction> _iclbGLTransaction;
        public Collection<busGLTransaction> iclbGLTransaction
        {
            get { return _iclbGLTransaction; }
            set { _iclbGLTransaction = value; }
        }

        public DateTime idtExtractDate { get; set; }

        public void LoadGLTransaction(DataTable ldtbGLTransaction)
        {
            _lstrFileName = Convert.ToString(iarrParameters[0]);
            idtExtractDate = Convert.ToDateTime(iarrParameters[1]);
            ldtbGLTransaction = DBFunction.DBSelect("entGLTransaction.GetGLTransactionsByFileName", new object[2] { _lstrFileName, idtExtractDate }
                                 , iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            _iclbGLTransaction = new Collection<busGLTransaction>();
            foreach (DataRow dr in ldtbGLTransaction.Rows)
            {
                busGLTransaction lobjTransaction = new busGLTransaction();
                lobjTransaction.icdoGlTransaction = new cdoGlTransaction();
                lobjTransaction.icdoGlTransaction.LoadData(dr);
                lobjTransaction.ldclTotalAmount = Convert.ToDecimal(dr["TOTAL_AMOUNT"]);
                lobjTransaction.ldteBatchRunDate = idtExtractDate; //PROD PIR ID 6579
                lobjTransaction.LoadFiscalPeriod();
                lobjTransaction.LoadAccountNumber();
                lobjTransaction.LoadPlanCode();
                _iclbGLTransaction.Add(lobjTransaction);
            }
        }
    }
}
