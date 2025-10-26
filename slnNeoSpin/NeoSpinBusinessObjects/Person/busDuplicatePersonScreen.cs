#region Using directives

using System;
using System.Collections.ObjectModel;
using System.Data;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busDuplicatePersonScreen : busExtendBase
    {
        public int iintDuplicateRecordCount { get; set; }

        public busPerson ibusPerson { get; set; }

        public Collection<busPerson> iclbDuplicatePersons { get; set; }
		//PIR 25990 Possible Duplicate Person screen changes
        public void btnDuplicatePersonSearch_click()
        {
            if (ibusPerson.IsNull()) ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
            LoadDuplicatePersons(ibusPerson.icdoPerson.person_id, ibusPerson.icdoPerson.last_name, ibusPerson.icdoPerson.date_of_birth,
                                    ibusPerson.icdoPerson.gender_value, 1, 0, ibusPerson.icdoPerson.ssn);
        }
        public void LoadDuplicatePersons(int aintPersonID, string astrLastName, DateTime adteDateofBirth, string astrGenderValue,
                                                                                int aintScreenIdentifier, int aintOrgID, string astrSSN)
        {
            ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
            if (aintPersonID != 0)
                ibusPerson.FindPerson(aintPersonID);
            else
            {
                /// To Display in the Screen - Person Information
                ibusPerson.icdoPerson.last_name = astrLastName;
                ibusPerson.icdoPerson.date_of_birth = adteDateofBirth;
                ibusPerson.icdoPerson.gender_value = astrGenderValue;
                ibusPerson.icdoPerson.gender_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(300, astrGenderValue);
                ibusPerson.icdoPerson.ssn = astrSSN;
            }
            iclbDuplicatePersons = new Collection<busPerson>();
            if ((astrLastName.IsNotNullOrEmpty()) &&
                (astrSSN.IsNotNullOrEmpty()) &&
                (astrSSN.Length >= 9) &&
                (astrGenderValue.IsNotNullOrEmpty()) &&
                (adteDateofBirth != DateTime.MinValue))
            {

                /// aintScreenIdentifier = 1 --> Person Maintenance -- Equal to or greater than 3 similar record exists then raise error
                /// aintScreenIdentifier = 2 --> Person Employment Maintenance -- Equal to or greater than 4 similar record exists then raise error
                int lintRetCount = 0;
                if (aintScreenIdentifier == 1)
                    lintRetCount = 3;
                else if (aintScreenIdentifier == 2)
                    lintRetCount = 4;

                string lstrSSN = string.Empty;
                foreach (char lcr in astrSSN)
                {
                    if (lcr != '-')
                        lstrSSN += lcr;
                }
                //123456789
                string lstrSSN1 = lstrSSN.Substring(3, 4);
                string lstrSSN2 = lstrSSN.Substring(4, 4);
                string lstrSSN3 = lstrSSN.Substring(5, 4);
                //string lstrSSN4 = lstrSSN.Substring(3, 4);
                //string lstrSSN5 = lstrSSN.Substring(4, 4);

                string lstrEmployerName = string.Empty;
                if (aintScreenIdentifier == 2)
                    lstrEmployerName = busGlobalFunctions.GetOrgNameByOrgID(aintOrgID);

                DataTable ldtbResult = Select("cdoPerson.GetPossibleDuplicatePersons", new object[10]{
                                    ibusPerson.icdoPerson.person_id, astrLastName,adteDateofBirth ,astrGenderValue,aintScreenIdentifier,aintOrgID,
                                    lstrSSN1,lstrSSN2,lstrSSN3,lintRetCount});
                foreach (DataRow ldtr in ldtbResult.Rows)
                {
                    busPerson lobjDuplicatePerson = new busPerson { icdoPerson = new cdoPerson() };
                    lobjDuplicatePerson.icdoPerson.LoadData(ldtr);
                    lobjDuplicatePerson.LoadPersonCurrentAddress();
                    lobjDuplicatePerson.istrPossibleDuplicateEmployerName = lstrEmployerName;
                    iclbDuplicatePersons.Add(lobjDuplicatePerson);
                }
            }
            iintDuplicateRecordCount = iclbDuplicatePersons.Count;
        }
    }
}
