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
    public class cdoPersonContact : doPersonContact
    {
        public cdoPersonContact()
            : base()
        {
        }
        private string _istrOrgCodeID;

        public string istrOrgCodeID
        {
            get { return _istrOrgCodeID; }
            set { _istrOrgCodeID = value; }
        }
        private int _lintSameAsPersonAddress;
        public int lintSameAsPersonAddress
        {
            get { return _lintSameAsPersonAddress; }
            set { _lintSameAsPersonAddress = value; }
        }

        private String _lstrContactName;
        public String lstrContactName
        {
            get { return _lstrContactName; }
            set { _lstrContactName = value; }
        }       

        # region UCS 24
        //marital status update
        public string istrMSSFirstName { get; set; }
        public string istrMSSLastName { get; set; }
        public string istrMSSMiddleName { get; set; }
        public string istrMSSSSN { get; set; }
        public string istrMSSNamePrefix { get; set; }
        public string istrMSSNameSuffix { get; set; }
        public string istrMSSGender { get; set; }
        public DateTime idtMSSContactDOB { get; set; }
        public string istrMSSContactName { get; set; }
        public string istrMSSSuppressWarning { get; set; }
        public int iintMSSPersonContactId { get; set; }
        public string istrMSSLastFourDigitsOfSSN
        {
            get
            {
                if ((istrMSSSSN != null) && (istrMSSSSN.Length == 9))
                {
                    return istrMSSSSN.Substring(5);
                }
                return string.Empty;
            }
        }
        #endregion

        public string istrAddressLine1_CAPS
        { get { return string.IsNullOrEmpty(address_line_1) ? string.Empty : address_line_1.ToUpper(); } }

        public string istrAddressLine2_CAPS
        { get { return string.IsNullOrEmpty(address_line_2) ? string.Empty : address_line_2.ToUpper(); } }

        public string istrAddressCity_CAPS
        { get { return string.IsNullOrEmpty(address_city) ? string.Empty : address_city.ToUpper(); } }

        //pir 8623
        public string istrAddressState_CAPS
        { get { return string.IsNullOrEmpty(address_state_description) ? string.Empty : address_state_description.ToUpper(); } }

        public string istrCountryDescription_CAPS
        { get { return string.IsNullOrEmpty(address_country_description) ? string.Empty : address_country_description.ToUpper(); } }

        //Pir 8540
        public string istrMSSContactName_CAPS
        {
            get
            {
                return string.IsNullOrEmpty(istrMSSContactName) ? string.Empty : istrMSSContactName.ToUpper();
            }
        }

        public String istrFullName
        {
            get
            {
                string lstrName = String.Empty;
                if (!String.IsNullOrEmpty(istrMSSFirstName))
                {
                    lstrName = istrMSSFirstName.Trim();
                }
                if (!String.IsNullOrEmpty(istrMSSMiddleName))
                {
                    lstrName += " " + istrMSSMiddleName.Trim();
                }
                if (!String.IsNullOrEmpty(istrMSSLastName))
                    lstrName += " " + istrMSSLastName.Trim();

                return lstrName;
            }
        }
    }
}
