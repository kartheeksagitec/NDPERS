#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sagitec.Common;
using Sagitec.DataObjects;
#endregion

namespace NeoSpin.DataObjects
{
    [Serializable]
    public class doEmployerPayrollHeaderError : doBase
    {
         
         public doEmployerPayrollHeaderError() : base()
         {
         }
		private int _employer_payroll_header_error_id;
		public int employer_payroll_header_error_id
		{
			get
			{
				return _employer_payroll_header_error_id;
			}

			set
			{
				_employer_payroll_header_error_id = value;
			}
		}

		private int _employer_payroll_header_id;
		public int employer_payroll_header_id
		{
			get
			{
				return _employer_payroll_header_id;
			}

			set
			{
				_employer_payroll_header_id = value;
			}
		}

		private int _message_id;
		public int message_id
		{
			get
			{
				return _message_id;
			}

			set
			{
				_message_id = value;
			}
		}

    }
}

