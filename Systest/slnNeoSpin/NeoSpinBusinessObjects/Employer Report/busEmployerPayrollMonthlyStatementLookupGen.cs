#region Using directives

using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using NeoSpin.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;

#endregion

namespace NeoSpin.BusinessObjects
{
	[Serializable]
	public class busEmployerPayrollMonthlyStatementLookupGen : busMainBase
	{

		private Collection<busEmployerPayrollMonthlyStatement> _iclbEmployerPayrollMonthlyStatment;
		public Collection<busEmployerPayrollMonthlyStatement> iclbEmployerPayrollMonthlyStatment
		{
			get
			{
				return _iclbEmployerPayrollMonthlyStatment;
			}
			set
			{
				_iclbEmployerPayrollMonthlyStatment = value;
			}
		}

		public void LoadEmployerPayrollMonthlyStatments(DataTable adtbSearchResult)
		{
			_iclbEmployerPayrollMonthlyStatment = GetCollection<busEmployerPayrollMonthlyStatement>(adtbSearchResult, "icdoEmployerPayrollMonthlyStatement");
		}
	}
}
