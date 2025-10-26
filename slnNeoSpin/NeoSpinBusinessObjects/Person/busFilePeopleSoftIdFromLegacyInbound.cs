#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busFilePeopleSoftIdFromLegacyInbound : busFileBase
    {
        public busFilePeopleSoftIdFromLegacyInbound()
        {
        }
        bool lblnErrorFound;

        string lstrSSN =string.Empty;


        private busPerson _ibusPerson;
        public busPerson ibusPerson
        {
            get { return _ibusPerson; }
            set { _ibusPerson = value; }
        }

        public override busBase NewDetail()
        {
            _ibusPerson = new busPerson();
            _ibusPerson.icdoPerson = new cdoPerson();
            return _ibusPerson;
        }
       
        public override void ProcessDetail()
        {
            lblnErrorFound = false;

            ArrayList larrList = new ArrayList();
            utlError lobjError = new utlError();

            lstrSSN = _ibusPerson.icdoPerson.ssn;
             
            if (!IsSSNExists(lstrSSN))
            {
                lobjError = new utlError();
                lobjError.istrErrorID = "1074";         // SSN doest not Exists.
                lobjError.istrErrorMessage = "SSN doest not Exists.";
                larrList.Add(lobjError);
                lblnErrorFound = true;
            }

            if (lblnErrorFound)
            {
                _ibusPerson.iarrErrors = larrList;
            }
            else
            {
                UpdatePersonPeopleSoftId();                
            }
        }
        public bool IsSSNExists(string _lstrSSN)
        {
            int lintCount = Convert.ToInt32(DBFunction.DBExecuteScalar("cdoPerson.VALIDATE_DUPLICATE_SSN", new string[1] { _lstrSSN },
                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
            if (lintCount > 0)
                return true;
            else
                return false;
        }

        public DataTable GetPersonBySSN(string lstrSSN)
        {
            DataTable ldtbPerson = DBFunction.DBSelect("cdoPerson.GetPersonBySSN", new string[1] { lstrSSN },
                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return ldtbPerson;
        }

        public void UpdatePersonPeopleSoftId()
        {
            busPerson lobjPerson = new busPerson();
            lobjPerson.icdoPerson = new cdoPerson();
            lobjPerson.icdoPerson.LoadData(GetPersonBySSN(lstrSSN).Rows[0]);
            lobjPerson.icdoPerson.peoplesoft_id = _ibusPerson.icdoPerson.peoplesoft_id;
            lobjPerson.icdoPerson.Update();
        }
    }
}
