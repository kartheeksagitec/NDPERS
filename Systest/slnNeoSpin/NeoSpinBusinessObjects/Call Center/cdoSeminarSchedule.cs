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
    public class cdoSeminarSchedule : doSeminarSchedule
    {
        public cdoSeminarSchedule() : base()
        {
        }

        # region Custom properties for Batch Process

        private string _istrFirstChoice;
        public string istrFirstChoice
        {
            get { return _istrFirstChoice; }
            set { _istrFirstChoice = value; }
        }
        private string _istrSecondChoice;
        public string istrSecondChoice
        {
            get { return _istrSecondChoice; }
            set { _istrSecondChoice = value; }
        }

        # endregion        

        //prod pir 7844
        public int org_id { get; set; }
        //PIR 6927
        public int attendee_person_id { get; set; }

        // Property added for MSS
        public string full_address_to_search_in_google
        {
            get
            {
                string lstrTemp = string.Empty;
                if (!string.IsNullOrEmpty(location_name))
                    lstrTemp += location_name;
                if (!string.IsNullOrEmpty(location_address))
                {
                    if (!string.IsNullOrEmpty(location_name))
                        lstrTemp += ", ";
                    lstrTemp += location_address;
                }
                if (!string.IsNullOrEmpty(location_city))
                {
                    if (!string.IsNullOrEmpty(location_name) || !string.IsNullOrEmpty(location_address))
                        lstrTemp += ", ";
                    lstrTemp += location_city;
                }
                if (lstrTemp == string.Empty)
                    lstrTemp = "Bismarck";
                return "www.maps.google.com/maps?q=" + lstrTemp;
            }
        }

        //PIR 13660 - Property added for ESS
        public string SeminarStatus { get; set; }
    }
}
