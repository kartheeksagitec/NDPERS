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
	/// Class NeoSpin.DataObjects.doWssEmploymentAcaCert:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doWssEmploymentAcaCert : doBase
    {
        public doWssEmploymentAcaCert() : base()
        {
        }
        public int wss_employment_aca_cert_id { get; set; }
        public int person_id { get; set; }
        public int person_employment_id { get; set; }
        public int contact_id { get; set; }
        public string met_req { get; set; }
        public string method { get; set; }
        public string lb_measure { get; set; }
        public DateTime from_date { get; set; }
        public DateTime to_date { get; set; }
        public int per_emp_dtl_id { get; set; }
    }
    [Serializable]
    public enum enmWssEmploymentAcaCert
    {
        wss_employment_aca_cert_id,
        person_id,
        person_employment_id,
        contact_id,
        met_req,
        method,
        lb_measure,
        from_date,
        to_date,
        per_emp_dtl_id
    }
}
