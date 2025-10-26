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
    public class cdoPayeeAccountStatus : doPayeeAccountStatus
    {
        public cdoPayeeAccountStatus()
            : base()
        {
        }
        //Actual status value of the status selected in the screen,bcoz data1 column of the code_id 2203 is having the actual status value for all benefit account types
        public string status_value_data1 { get; set; }
        //Used to put a information in the screen
        public string suppress_warnings_flag { get; set; }

    }
}
