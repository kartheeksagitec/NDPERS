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
	public class busEmployerPayrollMonthlyStatementGen : busExtendBase
    {
		public busEmployerPayrollMonthlyStatementGen()
		{

		}

		private cdoEmployerPayrollMonthlyStatement _icdoEmployerPayrollMonthlyStatement;
		public cdoEmployerPayrollMonthlyStatement icdoEmployerPayrollMonthlyStatement
		{
			get
			{
                return _icdoEmployerPayrollMonthlyStatement;
			}
			set
			{
                _icdoEmployerPayrollMonthlyStatement = value;
			}
		}

		public bool FindEmployerPayrollMonthlyStatment(int Aintemployerpayrollmonthlystatementid)
		{
			bool lblnResult = false;
            if (_icdoEmployerPayrollMonthlyStatement == null)
			{
                _icdoEmployerPayrollMonthlyStatement = new cdoEmployerPayrollMonthlyStatement();
			}
            if (_icdoEmployerPayrollMonthlyStatement.SelectRow(new object[1] { Aintemployerpayrollmonthlystatementid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
