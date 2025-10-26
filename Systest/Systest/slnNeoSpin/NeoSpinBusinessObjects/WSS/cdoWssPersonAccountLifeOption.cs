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
	/// <summary>
	/// Class NeoSpin.CustomDataObjects.cdoWssPersonAccountLifeOption:
	/// Inherited from doWssPersonAccountLifeOption, the class is used to customize the database object doWssPersonAccountLifeOption.
	/// </summary>
    [Serializable]
	public class cdoWssPersonAccountLifeOption : doWssPersonAccountLifeOption
	{
		public cdoWssPersonAccountLifeOption() : base()
		{
		}

        public string pre_tax_payroll_deduction_desc
        {
            get
            {
                if (pre_tax_payroll_deduction == BusinessObjects.busConstant.Flag_Yes)
                    return BusinessObjects.busConstant.Flag_Yes_Value;
                return BusinessObjects.busConstant.Flag_No_Value;
            }
        }
    }
} 
