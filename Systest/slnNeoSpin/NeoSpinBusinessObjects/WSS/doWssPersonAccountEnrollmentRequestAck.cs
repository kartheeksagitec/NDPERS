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
	/// Class NeoSpin.DataObjects.doWssPersonAccountEnrollmentRequestAck:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doWssPersonAccountEnrollmentRequestAck : doBase
    {
         
         public doWssPersonAccountEnrollmentRequestAck() : base()
         {
         }
         public int person_account_enrollment_request_ack_id { get; set; }
         public int wss_person_account_enrollment_request_id { get; set; }
         public int acknowledgement_id { get; set; }
         public string acknowledgement_text { get; set; }
    }
    [Serializable]
    public enum enmWssPersonAccountEnrollmentRequestAck
    {
         person_account_enrollment_request_ack_id ,
         wss_person_account_enrollment_request_id ,
         acknowledgement_id ,
         acknowledgement_text ,
    }
}

