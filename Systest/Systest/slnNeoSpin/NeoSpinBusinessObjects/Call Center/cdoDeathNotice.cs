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
    public class cdoDeathNotice : doDeathNotice
    {
        public cdoDeathNotice()
            : base()
        {
        }
		//PIR-17314
        public bool iblnIsPersonEnrolledInRetirementPlan { get; set; }
        public string reporting_Month { get; set; }
        public string istrFullNameWithPersonId { get; set; }
    }
}
