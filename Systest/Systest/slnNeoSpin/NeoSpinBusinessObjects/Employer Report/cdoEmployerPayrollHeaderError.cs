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
	public class cdoEmployerPayrollHeaderError : doEmployerPayrollHeaderError
	{
		public cdoEmployerPayrollHeaderError() : base()
		{
		}

        public string istrDisplayMessage { get; set; }
        public string istrSeverity { get; set; }
        public int iintMessageCount { get; set; }
        public string istrEmployerInstructions { get; set; }
    } 
} 
