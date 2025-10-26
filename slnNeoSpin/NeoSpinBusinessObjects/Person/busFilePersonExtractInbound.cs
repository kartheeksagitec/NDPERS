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
    public class busFilePersonExtractInbound:busFileBase
    {
        public busFilePersonExtractInbound()
        {
        }
        bool lblnErrorFound;

        private busPerson _ibusPerson;
	    public busPerson ibusPerson
	    {
		    get { return _ibusPerson;}
		    set { _ibusPerson = value;}
	    }

        public override busBase NewDetail()
        {
            _ibusPerson = new busPerson();
            _ibusPerson.icdoPerson = new cdoPerson();
            return _ibusPerson;
        }

        public override string BeforeFieldAssigned(string astrFieldName, string astrFieldValue)
        {
            string lstrObjectField;
            string lstrReturnValue = astrFieldValue;            
            if (astrFieldName.IndexOf(".") > -1)
                lstrObjectField = astrFieldName.Substring(astrFieldName.LastIndexOf(".") + 1);
            else
                lstrObjectField = astrFieldName;
            
            if (lstrObjectField == "date_of_birth")
            {
                if(lstrReturnValue.Trim()!=string.Empty)
                    lstrReturnValue = DateTime.ParseExact(lstrReturnValue, "MMddyyyy", null).ToString("MM/dd/yyyy");
            }
            return lstrReturnValue;
        }

        public override void ProcessDetail()
        {
            lblnErrorFound = false;
            _ibusPerson.iobjPassInfo.istrUserID = _ibusPerson.icdoPerson.modified_by;
                   
            ArrayList larrList = new ArrayList();
            utlError lobjError = new utlError();

            if (_ibusPerson.istrPrevSSN != "000000000")
            {
                if (_ibusPerson.istrPrevSSN != null)
                {
                    if (!IsSSNExists(_ibusPerson.istrPrevSSN))
                    {
                        lobjError = new utlError();
                        lobjError.istrErrorID = "1074";         // Previous SSN doest not Exists.
                        lobjError.istrErrorMessage = "SSN doest not Exists.";
                        larrList.Add(lobjError);
                        lblnErrorFound = true;
                    }
                    if (IsSSNExists(_ibusPerson.icdoPerson.ssn))
                    {
                        lobjError = new utlError();
                        lobjError.istrErrorID = "1076";         // New SSN already Exists.
                        lobjError.istrErrorMessage = "SSN Already Exists.";
                        larrList.Add(lobjError);
                        lblnErrorFound = true;
                    }
                }
                else
                {
                    if (!IsSSNExists(_ibusPerson.icdoPerson.ssn))
                    {
                        lobjError = new utlError();
                        lobjError.istrErrorID = "1074";         // SSN doest not Exists.
                        lobjError.istrErrorMessage = "SSN doest not Exists.";
                        larrList.Add(lobjError);
                        lblnErrorFound = true;
                    }
                }
            }
            else
            {
                lobjError = new utlError();
                lblnErrorFound = true;
                lobjError.istrErrorID = "1075";                 // New Member - Ignore Record.
                lobjError.istrErrorMessage = "New member cannot be updated.";
                larrList.Add(lobjError);
            }

            if (lblnErrorFound)
            {
                _ibusPerson.iarrErrors = larrList;
            }
            else
            {
                _ibusPerson = UpdateMember(_ibusPerson);
                _ibusPerson.icdoPerson.Update();
            }            
        }

        public busPerson UpdateMember(busPerson _ibusPerson)
        {
            busPerson ibusMember = new busPerson();
            ibusMember.icdoPerson = new cdoPerson();

            if (_ibusPerson.istrPrevSSN != null && _ibusPerson.istrPrevSSN != "000000000")
            {
                DataTable ldtbMember = GetPersonBySSN(_ibusPerson.istrPrevSSN);
                ibusMember.icdoPerson.LoadData(ldtbMember.Rows[0]);
                ibusMember.icdoPerson.ssn = _ibusPerson.icdoPerson.ssn;
            }
            else
            {
                DataTable ldtbMember = GetPersonBySSN(_ibusPerson.icdoPerson.ssn);
                ibusMember.icdoPerson.LoadData(ldtbMember.Rows[0]);
            }

            if (_ibusPerson.istrFirstandMiddleName != null)
            {
                string lstr=_ibusPerson.istrFirstandMiddleName.Trim();
                string[] strarr = lstr.Split(new char[] { ' ' });
                ibusMember.icdoPerson.first_name = strarr[0];
                if(strarr.Length>1)
                    ibusMember.icdoPerson.middle_name =strarr[1];
            }

            if (_ibusPerson.icdoPerson.last_name != null)
            {
                ibusMember.icdoPerson.last_name = _ibusPerson.icdoPerson.last_name.Trim();
            }

            if (_ibusPerson.icdoPerson.date_of_birth != DateTime.MinValue)
            {
                ibusMember.icdoPerson.date_of_birth = _ibusPerson.icdoPerson.date_of_birth;
            }
            return ibusMember;            
        }

        public DataTable GetPersonBySSN(string lstrSSN)
        {
            DataTable ldtbPerson = DBFunction.DBSelect("cdoPerson.GetPersonBySSN", new string[1] { lstrSSN },
                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return ldtbPerson;
        }

        public bool IsSSNExists(string _lstrSSN)
        {
            int lintCount=Convert.ToInt32(DBFunction.DBExecuteScalar("cdoPerson.VALIDATE_DUPLICATE_SSN",new string[1]{_lstrSSN},
                iobjPassInfo.iconFramework,iobjPassInfo.itrnFramework));
            if(lintCount>0)
                return true;
            else
                return false;
        }


    }
}
