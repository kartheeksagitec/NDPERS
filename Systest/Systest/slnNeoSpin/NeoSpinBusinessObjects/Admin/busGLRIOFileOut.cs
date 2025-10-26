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
    public class busGLRIOFileOut:busFileBaseOut
    {
        public busGLRIOFileOut()
        {
        }

        private Collection<busGLTransaction> _iclbGLTransaction;
        public Collection<busGLTransaction> iclbGLTransaction
        {
            get { return _iclbGLTransaction; }
            set { _iclbGLTransaction = value; }
        }

        public override bool ValidateFile()
        {
            bool lblnFlag = true;
            if (_iclbGLTransaction.Count == 0)
            {
                this.istrError = "No Records Found.";
                lblnFlag = false;
            }
            return lblnFlag;
        }

        public void LoadGLTransaction(DataTable ldtbGLTransaction)
        {
            DateTime ldtTempDate= DateTime.Now.AddMonths(-1);
            DateTime ldtStartDate = new DateTime(ldtTempDate.Year,ldtTempDate.Month,01);
            DateTime ldtEndDate = new DateTime(ldtTempDate.Year,ldtTempDate.Month,DateTime.DaysInMonth(ldtTempDate.Year, ldtTempDate.Month));
            istrFileName = busConstant.GLRIOFileName+ldtStartDate.ToString(busConstant.GLRIODateFormat) + busConstant.FileFormattxt;
            ldtbGLTransaction = DBFunction.DBSelect("cdoGLTransaction.GetTransactionForRIO", new object[2] { ldtStartDate, ldtEndDate }
                , iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);          
            _iclbGLTransaction = new Collection<busGLTransaction>();
            foreach (DataRow dr in ldtbGLTransaction.Rows)
            {
                busGLTransaction lobjTransaction = new busGLTransaction();
                lobjTransaction.icdoGlTransaction = new cdoGlTransaction();
                lobjTransaction.icdoGlTransaction.LoadData(dr);
                lobjTransaction.ldclTotalAmount = Convert.ToDecimal(dr["TOTAL_AMOUNT"]);
                lobjTransaction.LoadAccountNumber();
                lobjTransaction.LoadPlanCode();
                _iclbGLTransaction.Add(lobjTransaction);
            }
        }
    }
}
