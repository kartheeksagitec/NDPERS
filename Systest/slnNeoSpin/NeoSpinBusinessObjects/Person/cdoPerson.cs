#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using NeoSpin.DataObjects;
using System.Globalization;
using NeoSpin.BusinessObjects;

#endregion

namespace NeoSpin.CustomDataObjects
{
    [Serializable]
    public class cdoPerson : doPerson
    {
        public cdoPerson()
            : base()
        {

        }
        public string loggedin_user_id
        {
            get
            {
                return iobjPassInfo.istrUserID;
            }
        }
        public string LastFourDigitsOfSSN
        {
            get
            {
                if ((ssn != null) && (ssn.Length == 9))
                {
                    return ssn.Substring(5);
                }
                return string.Empty;
            }
        }
        //PIR 235
        public String FullName
        {
            get
            {
                string lstrName = String.Empty;
                if (!String.IsNullOrEmpty(first_name))
                {
                    lstrName = first_name.Trim();
                }
                if (!String.IsNullOrEmpty(middle_name))
                {
                    lstrName += " " + middle_name.Trim();
                }
                if (!String.IsNullOrEmpty(last_name))
                    lstrName += " " + last_name.Trim();

                return lstrName;
            }
        }
        //PIR 19016
 		public String istrBenefitCalculationFullName
        {
            get
            {
                string lstrName = String.Empty;
                if (!String.IsNullOrEmpty(last_name))
                    lstrName = last_name.Trim();
                if (!String.IsNullOrEmpty(first_name))
                {
                    lstrName += ", " + first_name.Trim();
                }
                if (!String.IsNullOrEmpty(middle_name))
                {
                    lstrName += " " + middle_name.Trim();
                }
                return lstrName;
            }
        }

        public string SalutationFullName
        {
            get
            {
                return busGlobalFunctions.ToTitleCase(FullName);
            }
        }

        //Prem :: Added for displaying custom format in the ServicePurchaseDetailConsolidated screen
        // Display as ‘PERSLink ID  Last Name, First Name, Middle Name’
        public String PersonIdWithName
        {
            get
            {
                const string lstrSeperator = ", ";
                const string lstrSpaceSeperator = " ";
                StringBuilder lsb = new StringBuilder();
                if (!String.IsNullOrEmpty(person_id.ToString()) && person_id > 0)
                {
                    lsb.Append(person_id);
                }
                if (!String.IsNullOrEmpty(last_name))
                {
                    lsb.Append(lstrSpaceSeperator + last_name);
                }
                if (!String.IsNullOrEmpty(first_name))
                {
                    lsb.Append(lstrSeperator + first_name);
                }
                if (!String.IsNullOrEmpty(middle_name))
                {
                    lsb.Append(lstrSeperator + middle_name);
                }

                return lsb.ToString();
            }
        }

        //will return empty if PerosnIDWithName is Empty
        public string istrRecipientPersonFullName
        {
            get
            {
                string lstrRecipientPersonFullName = String.Empty;
                if (person_id != 0)
                {
                    lstrRecipientPersonFullName = PersonIdWithName;
                }
                return lstrRecipientPersonFullName;
            }
        }

        //elayaraja :: Added for displaying custom format in the PayeeAccountMaitenance screen
        // Display as ‘PERSLink ID  Last Name, First Name'.
        public String PayeeName
        {
            get
            {
                const string lstrSeperator = ", ";
                const string lstrSpaceSeperator = " ";
                StringBuilder lsb = new StringBuilder();
                if (person_id > 0)
                {
                    if (!String.IsNullOrEmpty(person_id.ToString()))
                    {
                        lsb.Append(person_id);
                    }
                    if (!String.IsNullOrEmpty(last_name))
                    {
                        lsb.Append(lstrSpaceSeperator + last_name);
                    }
                    if (!String.IsNullOrEmpty(first_name))
                    {
                        lsb.Append(lstrSeperator + first_name);
                    }
                }
                return lsb.ToString();
            }
        }
        //elayaraja :: Added for displaying custom format in the PayeeAccountMaitenance screen
        // Display as ‘PERSLink ID  Last Name, First Name'.
        public String MemberName
        {
            get
            {
                const string lstrSeperator = ", ";
                const string lstrSpaceSeperator = " ";
                StringBuilder lsb = new StringBuilder();
                if (!String.IsNullOrEmpty(person_id.ToString()))
                {
                    lsb.Append(person_id);
                }
                if (!String.IsNullOrEmpty(last_name))
                {
                    lsb.Append(lstrSpaceSeperator + last_name);
                }
                if (!String.IsNullOrEmpty(first_name))
                {
                    lsb.Append(lstrSeperator + first_name);
                }
                return lsb.ToString();
            }
        }
        //this is used only in the seminar sign up sheet
        public String PersonName
        {
            get
            {
                string seperator = ", ";
                StringBuilder sb = new StringBuilder();
                sb.Append(this.last_name);
                if (this.first_name != null && this.first_name.Trim() != "")
                    sb.Append(seperator + this.first_name);
                if (this.middle_name != null && this.middle_name.Trim() != "")
                    sb.Append(" " + this.middle_name);

                return sb.ToString();
            }
        }

        // this field is only used for the file - PeopleSoftId from Legacy
        //do not use for ny other method purpose
        private string _istrDummy;
        public string istrDummy
        {
            get
            {
                return _istrDummy;
            }

            set
            {
                _istrDummy = value;
            }
        }

        //Full Name in caps
        public string FullNameCaps
        {
            get
            {
                return FullName.ToUpper();
            }
        }

        //UCS - 081 : Property to store Confirmation tracking number
        public string istrConfirmationTrackingNumber { get; set; }

        //UCS - 081 : Property to store Verification Code
        public int iintVerificationCode { get; set; }

        //UCS - 081 : Property to store Death Indicator
        public string istrDeathIndicator { get; set; }

        //UCS - 081 : Property to store DOB
        public string istrDOB { get; set; }

        // UCS-041 - Deleting a Person is just a logical delete
        public override int Delete()
        {
            deleted_ssn = ssn;
            ssn = Convert.ToString(person_id).PadRight(9, 'X');
            is_person_deleted_flag = BusinessObjects.busConstant.Flag_Yes;
            Update();
            return 1;
        }

        public string temp_ssn { get; set; }

        public string istrPersonIdFullNameSSN
        {
            get
            {
                string lstrReturnValue = String.Empty;
                lstrReturnValue = PersonIdWithName + " " + LastFourDigitsOfSSN;

                return lstrReturnValue;
            }
        }

        public string istrPersonFullNameWithSSN
        {
            get
            {
                return FullName + " " + LastFourDigitsOfSSN;
            }
        }

        //Used in UCS 041 workflow
        public bool iblnIs1099RExists { get; set; }

        //UAT PIR 1185    
        // Display as ‘Last Name, First Name, Middle Name’
        public String PersonNameForSorting
        {
            get
            {
                const string lstrSpaceSeperator = " ";
                StringBuilder lsb = new StringBuilder();
                if (!String.IsNullOrEmpty(last_name))
                {
                    lsb.Append(last_name);
                }
                if (!String.IsNullOrEmpty(middle_name))
                {
                    lsb.Append(lstrSpaceSeperator + middle_name);
                }
                if (!String.IsNullOrEmpty(first_name))
                {
                    lsb.Append(lstrSpaceSeperator + first_name);
                }

                return lsb.ToString();
            }
        }

        public String PersonNameWithPrefixAndSuffix
        {
            get
            {
                const string lstrSpaceSeperator = " ";
                StringBuilder lsb = new StringBuilder();
                if (!String.IsNullOrEmpty(name_prefix_description))
                {
                    lsb.Append(name_prefix_description);
                }
                if (!String.IsNullOrEmpty(last_name))
                {
                    lsb.Append(" " + last_name);
                }
                if (!String.IsNullOrEmpty(first_name))
                {
                    lsb.Append(lstrSpaceSeperator + first_name);
                }
                if (!String.IsNullOrEmpty(middle_name))
                {
                    lsb.Append(lstrSpaceSeperator + middle_name);
                }
                if (!String.IsNullOrEmpty(name_suffix_description))
                {
                    lsb.Append(" " + name_suffix_description);
                }
                return lsb.ToString();
            }
        }

        // PROD PIR ID 5018 --> To Add First Name and Middle Name
        // PROD PIR ID 5550 --> If both combination is greater than 15 return only first name
        public string FirstNameMiddleName
        {
            get
            {
                string lstrValue = string.Empty;
                if (!string.IsNullOrEmpty(first_name))
                    lstrValue = first_name;
                if (!string.IsNullOrEmpty(middle_name))
                    lstrValue += " " + middle_name;
                if (lstrValue.Length > 15)
                    return first_name;
                return lstrValue;
            }
        }

        //this is used to display the date of birth in PDF correspondence
        public string istrDateOfBirth
        {
            get
            {
                return date_of_birth.ToString("d", CultureInfo.CreateSpecificCulture("en-US"));
            }
        }

        //used in death notification correspondence
        public string istrFirstNameInTitleCase
        {
            get
            {
                return busGlobalFunctions.ToTitleCase(first_name);
            }
        }

		public string istrFormattedDateOfBirth
        {
            get
            {
                if (date_of_birth == DateTime.MinValue)
                    return string.Empty;
                else
                    return date_of_birth.ToString(busConstant.DateFormatLongDate);
            }
        }

        public string istrDateOfBirthFSA
        {
            get
            {
                if (date_of_birth == DateTime.MinValue)
                    return string.Empty;
                else
                    return date_of_birth.ToString(busConstant.DateFormatYearMonthDay);
            }
        }

        //PIR 23950
        public string istrDateOfDeathFSA
        {
            get
            {
                return date_of_death == DateTime.MinValue ? string.Empty : date_of_death.ToString(busConstant.DateFormatYearMonthDay);
            }
        }

        //pir 8607
        public string istrMSSDisplayName
        {
            get
            {
                string lstrMSSDisplayName = String.Empty;
                if (first_name.IsNotNullOrEmpty())
                    lstrMSSDisplayName = first_name;
                if (middle_name.IsNotNullOrEmpty())
                    lstrMSSDisplayName += " " + middle_name;
                if(last_name.IsNotNullOrEmpty())
                    lstrMSSDisplayName += " " + last_name;
                if(name_suffix_description.IsNotNullOrEmpty())
                    lstrMSSDisplayName += " " + name_suffix_description;
                return lstrMSSDisplayName;
            }
        }


        public string first_name_from_person { get; set; } //PIR 14166

        public string last_name_from_person { get; set; } //PIR 14166

        public string MemberPersonName { get; set; }
        public string PersonMemberName { get; set; }
        public int retr_bene_count { get; set; } //PIR 13054 Corr properties
        public int life_bene_count { get; set; }//PIR 13054
 
        public int Retr_Bene_Required
        {
            get
            {
                return retr_bene_count > 0 && life_bene_count == 0 ? 1 : 0;
            }
        }
        public int Life_Bene_Required
        {
            get
            {
                return retr_bene_count == 0 && life_bene_count > 0 ? 1 : 0;
            }
        }
        public int Retr_Life_Bene_Required
        {
            get
            {
                return retr_bene_count > 0 && life_bene_count > 0 ? 1 : 0;
            }
        }
        public bool iblnIsPersonEnrolledInRetirementPlan { get; set; } //PIR-17314
        public int iintTermEmpID { get; set; } //F/W Upgrade PIR 11095
        //PIR 20807
        public string istrRestrictionReasonDesc
        {
            get
            {
                if (!string.IsNullOrEmpty(restriction_reason_value))
                    return busGlobalFunctions.GetDescriptionByCodeValue(busConstant.RestrictionReasonValueCodeId, restriction_reason_value, iobjPassInfo);
                return string.Empty;
            }
        }
        public string istrFirstNameMiddleInitial
        {
            get
            {
                string lstrValue = string.Empty;
                if (!string.IsNullOrEmpty(first_name))
                    lstrValue = first_name;
                if (!string.IsNullOrEmpty(middle_name))
                    lstrValue += " " + middle_name.Substring(0, 1);                
                return lstrValue;
            }
        }
        public string istrScrambledSSN
        {
            get
            {
                string SSN_part = String.Empty;
                string scrambled = String.Empty;
                if (!String.IsNullOrEmpty(ssn))
                {
                    SSN_part = ssn.Trim().Right(4);
                    //scrambled_part = ssn.Trim().Left(ssn.Trim().Length - 4);
                    scrambled = SSN_part.PadLeft(9, 'X');
                }
                return scrambled;
            }
        }
        public String PayeeFullName
        {
            get
            {
                const string lstrSpaceSeperator = " ";
                StringBuilder lsb = new StringBuilder();
                if (person_id > 0)
                {
                    if (!String.IsNullOrEmpty(first_name))
                    {
                        lsb.Append(first_name);
                    }
                    if (!String.IsNullOrEmpty(middle_name))
                    {
                        lsb.Append(lstrSpaceSeperator + middle_name);
                    }
                    if (!String.IsNullOrEmpty(last_name))
                    {
                        lsb.Append(lstrSpaceSeperator + last_name);
                    }

                }
                return lsb.ToString();
            }
        }
        // Display as ‘Last Name, First Name, Middle Name PERSLink ID’
        public String NameWithPersonID
        {
            get
            {
                const string lstrSeperator = ", ";
                const string lstrSpaceSeperator = " ";
                StringBuilder lsb = new StringBuilder();

                if (!String.IsNullOrEmpty(last_name))
                {
                    lsb.Append(last_name);
                }
                if (!String.IsNullOrEmpty(first_name))
                {
                    lsb.Append(lstrSpaceSeperator + first_name);
                }
                if (!String.IsNullOrEmpty(middle_name))
                {
                    lsb.Append(lstrSpaceSeperator + middle_name);
                }
                if (!String.IsNullOrEmpty(person_id.ToString()) && person_id > 0)
                {
                    lsb.Append(lstrSeperator + person_id);
                }

                return lsb.ToString();
            }
        }
    }
}