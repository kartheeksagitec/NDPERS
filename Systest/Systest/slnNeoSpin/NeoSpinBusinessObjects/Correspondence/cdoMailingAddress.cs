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
    public class cdoMailingAddress
    {
        public cdoMailingAddress() : base()
        {
        }

        #region Mailing Address

        private int _lintID;
        public int lintID
        {
            get { return _lintID; }
            set { _lintID = value; }
        }

        private string _lstrFullName;
        public string lstrFullName
        {
            get { return _lstrFullName; }
            set { _lstrFullName = value; }
        }

        private string _lstrContactName;
        public string lstrContactName
        {
            get { return _lstrContactName; }
            set { _lstrContactName = value; }
        }

        private string _lstrAddressLine1;
        public string lstrAddressLine1
        {
            get { return _lstrAddressLine1; }
            set { _lstrAddressLine1 = value; }
        }

        private string _lstrAddressLine2;
        public string lstrAddressLine2
        {
            get { return _lstrAddressLine2; }
            set { _lstrAddressLine2 = value; }
        }

        private string _lstrCity;
        public string lstrCity
        {
            get { return _lstrCity; }
            set { _lstrCity = value; }
        }

        private string _lstrState;
        public string lstrState
        {
            get { return _lstrState; }
            set { _lstrState = value; }
        }

        private string _lstrZipCode;
        public string lstrZipCode
        {
            get { return _lstrZipCode; }
            set { _lstrZipCode = value; }
        }

        private string _lstrCountry;
        public string lstrCountry
        {
            get { return _lstrCountry; }
            set { _lstrCountry = value; }
        }

        // PIR 488
        private string _lstrFirstName;
        public string lstrFirstName
        {
            get { return _lstrFirstName; }
            set { _lstrFirstName = value; }
        }

        private string _lstrLastName;
        public string lstrLastName
        {
            get { return _lstrLastName; }
            set { _lstrLastName = value; }
        }



        #endregion
    }
}
