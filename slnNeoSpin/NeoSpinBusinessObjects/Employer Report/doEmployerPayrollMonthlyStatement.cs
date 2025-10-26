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
	/// <summary>
	/// Class NeoSpin.DataObjects.doEmployerPayrollMonthlyStatement:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doEmployerPayrollMonthlyStatement : doBase
    {
         
         public doEmployerPayrollMonthlyStatement() : base()
         {
         }
         public int employer_payroll_monthly_statement_id { get; set; }
         public int org_id { get; set; }
         public int plan_id { get; set; }
         public decimal beginning_balance_amt { get; set; }
         public decimal month_due_amt { get; set; }
         public decimal remittance_amount { get; set; }
         public decimal invoice_amount { get; set; }
         public DateTime run_date { get; set; }
    }
    [Serializable]
    public enum enmEmployerPayrollMonthlyStatement
    {
         employer_payroll_monthly_statement_id ,
         org_id ,
         plan_id ,
         beginning_balance_amt ,
         month_due_amt ,
         remittance_amount ,
         invoice_amount ,
         run_date ,
    }
}

