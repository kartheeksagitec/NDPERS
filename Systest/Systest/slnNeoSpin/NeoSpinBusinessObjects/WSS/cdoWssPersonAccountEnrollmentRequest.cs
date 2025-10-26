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
	/// Class NeoSpin.CustomDataObjects.cdoWssPersonAccountEnrollmentRequest:
	/// Inherited from doWssPersonAccountEnrollmentRequest, the class is used to customize the database object doWssPersonAccountEnrollmentRequest.
	/// </summary>
    [Serializable]
	public class cdoWssPersonAccountEnrollmentRequest : doWssPersonAccountEnrollmentRequest
	{
		public cdoWssPersonAccountEnrollmentRequest() : base()
		{
		}

        public bool iblnIsGrandChildAdded { get; set; } //PIR 15820
        //PIR 23567
        public string is_applied_for_divorce_desc
        {
            get
            {
                if (is_applied_for_divorce == BusinessObjects.busConstant.Flag_Yes)
                    return BusinessObjects.busConstant.Flag_Yes_Value;
                return BusinessObjects.busConstant.Flag_No_Value;
            }
        }
    } 
} 
