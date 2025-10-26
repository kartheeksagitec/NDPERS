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
	public class cdoSeminarAttendeeDetail : doSeminarAttendeeDetail
	{
		public cdoSeminarAttendeeDetail() : base()
		{
		}

        private string _istrAttendeeDisplayName;

        public string istrAttendeeDisplayName
        {
            get { return _istrAttendeeDisplayName; }
            set { _istrAttendeeDisplayName = value; }
        }

        //this will be used in correspondence to display 
        //attended and guest flag as empty in Seminar sign up sheet     

        public string istrEmptyString
        {
            get { return String.Empty; }           
        }

        public string org_to_bill_org_code { get; set; }
        public int guest_speaker_contact_id { get; set; }
        public string org_name { get; set; } //PIR 9542        
    } 
} 
