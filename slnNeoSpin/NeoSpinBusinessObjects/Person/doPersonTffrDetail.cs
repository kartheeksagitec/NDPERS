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
    /// Class NeoSpin.DataObjects.doPersonTffrDetail:
    /// Inherited from doBase, the class is used to create a wrapper of database table object.
    /// Each property of an instance of this class represents a column of database table object.  
    /// </summary>
    [Serializable]
    public class doPersonTffrDetail : doBase
    {
         
         public doPersonTffrDetail() : base()
         {
         }
         public int person_tffr_detail_id { get; set; }
         public int person_tffr_header_id { get; set; }
         public int upload_id { get; set; }
         public int transaction_type_value { get; set; }
         public int line_no { get; set; }
         public int month { get; set; }
         public int year { get; set; }
         public decimal tffr_wage { get; set; }
         public string service_credit { get; set; }

    }
    [Serializable]
    public enum enmPersonTffrDetail
    {
        person_tffr_detail_id,
        person_tffr_header_id,
        upload_id,
        line_no,
        transaction_type_value,
        month,
        year,
        tffr_wage,
        service_credit,
    }
}

