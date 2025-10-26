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
	public partial class busChartOfAccount : busExtendBase
	{

        public void LoadByAccountNo(string aintAccountNo)
        {
            if (_icdoChartOfAccount == null)
            {
                _icdoChartOfAccount = new cdoChartOfAccount();
            }
            DataTable ldtChartOfAcct= Select("cdoChartOfAccount.GetByAccountNo", new object[1] { aintAccountNo});
            if (ldtChartOfAcct.Rows.Count > 0)
            {
                _icdoChartOfAccount.LoadData(ldtChartOfAcct.Rows[0]);
            }
        }

        public int GetAcctIDByAcctNo(string aintAccountNo)
        {
            int lintAccountID=0;
            DataTable ldtChartOfAcct = Select("cdoChartOfAccount.GetByAccountNo", new object[1] { aintAccountNo });
            if (ldtChartOfAcct.Rows.Count > 0)
            {
                lintAccountID= Convert.ToInt32(ldtChartOfAcct.Rows[0]["chart_of_account_id"]);
            }
            return lintAccountID;
        }

	}
}
