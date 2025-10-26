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
	public class cdoEmployerPayrollDetailError : doEmployerPayrollDetailError
	{
		public cdoEmployerPayrollDetailError() : base()
		{
           
 
		}
        public string severity_description { get; set; }
        public int count { get; set; }
        public int employer_payroll_header_id { get; set; }
        public string istrDisplayMessage { get; set; }
        public string istrEmployerInstructions { get; set; }
    } 
} 
