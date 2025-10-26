#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using NeoSpin.DataObjects;

#endregion

namespace NeoSpin.CustomDataObjects
{
    [Serializable]
	public class cdoEmployerPayrollMonthlyStatement : doEmployerPayrollMonthlyStatement
	{
		public cdoEmployerPayrollMonthlyStatement() : base()
		{
		}

        private DateTime _idtStartDate;

        public DateTime idtStartDate
        {
            get { return _idtStartDate; }
            set { _idtStartDate = value; }
        }

        private DateTime _idtEndDate;

        public DateTime idtEndDate
        {
            get { return _idtEndDate; }
            set { _idtEndDate = value; }
        }
	
	
    } 
} 
