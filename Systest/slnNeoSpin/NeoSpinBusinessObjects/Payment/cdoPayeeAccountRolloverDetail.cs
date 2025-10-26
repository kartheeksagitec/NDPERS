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
    public class cdoPayeeAccountRolloverDetail : doPayeeAccountRolloverDetail
    {
        public cdoPayeeAccountRolloverDetail()
            : base()
        {
        }
        private string _org_code;

        public string org_code
        {
            get { return _org_code; }
            set { _org_code = value; }
        }
      
        public string suppress_warnings_flag { get; set; }
    }
}