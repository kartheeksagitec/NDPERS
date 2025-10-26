using System;
using System.Collections.Generic;
using System.Text;
using NeoSpin.BusinessObjects;
using Sagitec.BusinessObjects;
using System.Collections;
using System.Collections.ObjectModel;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.CorBuilder;
using Sagitec.DataObjects;
using System.Data;
using Sagitec.CustomDataObjects;
using NeoSpin.CustomDataObjects;
namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busPersonAccountDCFileOut : busFileBaseOut
    {
        private Collection<busPersonAccountRetirement> _iclbDCMembers;
        public Collection<busPersonAccountRetirement> iclbDCMembers
        {
            get { return _iclbDCMembers; }
            set { _iclbDCMembers = value; }
        }
        public DateTime ldteNextRunDate { get; set; }
        public string lstrTIAAFlag { get; set; }
        public void LoadDCMembers(DataTable ldtbDCMembers)
        {
            ldtbDCMembers = busBase.Select("cdoPersonAccountRetirement.fleFidelityDCEnrollmentOut",
                                                                    new object[0] { });
            iclbDCMembers = new Collection<busPersonAccountRetirement>();
            foreach (DataRow drDCMembers in ldtbDCMembers.Rows)
            {
                busPersonAccountRetirement lobjPersonAccountRetirement = new busPersonAccountRetirement();
                lobjPersonAccountRetirement.icdoPersonAccount = new cdoPersonAccount();
                lobjPersonAccountRetirement.icdoPersonAccountRetirement = new cdoPersonAccountRetirement();
                lobjPersonAccountRetirement.icdoPersonAccount.LoadData(drDCMembers);

                lobjPersonAccountRetirement.icdoPersonAccountRetirement.LoadData(drDCMembers);
                if (!Convert.IsDBNull(drDCMembers["update_seq_pa_retirement"]))
                    lobjPersonAccountRetirement.icdoPersonAccountRetirement.update_seq = Convert.ToInt32(drDCMembers["update_seq_pa_retirement"]);

                lobjPersonAccountRetirement.ibusPerson = new busPerson();
                lobjPersonAccountRetirement.ibusPerson.icdoPerson = new cdoPerson();
                lobjPersonAccountRetirement.ibusPerson.icdoPerson.LoadData(drDCMembers);

                lobjPersonAccountRetirement.ibusPerson.LoadPersonCurrentAddress();

                lobjPersonAccountRetirement.LoadTotalVSC();

                //Initializing the Object to Avoid Null Exception
                lobjPersonAccountRetirement.ibusPersonEmploymentDetail = new busPersonEmploymentDetail();
                lobjPersonAccountRetirement.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail();

                lobjPersonAccountRetirement.ibusPersonEmploymentDetail.ibusPersonEmployment = new busPersonEmployment();
                lobjPersonAccountRetirement.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment = new cdoPersonEmployment();

                lobjPersonAccountRetirement.LoadAllPersonEmploymentDetails(false);

                if (lobjPersonAccountRetirement.iclbEmploymentDetail.Count > 0)
                {
                    bool lblnActiveEmploymentFound = false;
                    int lintLastEntry = lobjPersonAccountRetirement.iclbEmploymentDetail.Count - 1;
                    if (lobjPersonAccountRetirement.iclbEmploymentDetail[0].ibusPersonEmployment == null)
                        lobjPersonAccountRetirement.iclbEmploymentDetail[0].LoadPersonEmployment();

                    if (lobjPersonAccountRetirement.iclbEmploymentDetail[lintLastEntry].ibusPersonEmployment == null)
                        lobjPersonAccountRetirement.iclbEmploymentDetail[lintLastEntry].LoadPersonEmployment();

                    //Set the Start Date as Least Employment Start Date
                    lobjPersonAccountRetirement.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.start_date =
                        lobjPersonAccountRetirement.iclbEmploymentDetail[lintLastEntry].ibusPersonEmployment.icdoPersonEmployment.start_date;


                    if (lobjPersonAccountRetirement.iclbEmploymentDetail[0].ibusPersonEmployment.icdoPersonEmployment.end_date_no_null <= DateTime.Now)
                    {
                        lobjPersonAccountRetirement.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.end_date =
                            lobjPersonAccountRetirement.iclbEmploymentDetail[0].ibusPersonEmployment.icdoPersonEmployment.end_date;
                    }
                }

                if (lobjPersonAccountRetirement.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementRetired)
                {
                    lobjPersonAccountRetirement.icdoPersonAccount.status_code = "R";
                }
                else if ((lobjPersonAccountRetirement.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled) &&
                    (lobjPersonAccountRetirement.icdoPersonAccountRetirement.mutual_fund_window_flag == busConstant.Flag_Yes))
                {
                    lobjPersonAccountRetirement.icdoPersonAccount.status_code = "M";
                }
                else if (lobjPersonAccountRetirement.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled)
                {
                    lobjPersonAccountRetirement.icdoPersonAccount.status_code = "A";
                }
                else
                {
                    lobjPersonAccountRetirement.icdoPersonAccount.status_code = "T";
                }

                iclbDCMembers.Add(lobjPersonAccountRetirement);
            }
        }
        public override void InitializeFile()
        {
            if (lstrTIAAFlag == busConstant.Flag_Yes) 
                istrFileName = "405545" + ldteNextRunDate.ToString(busConstant.DateFormatD8) + "A" + busConstant.FileFormattxt;
        }
        public override void AfterWriteRecord()
        {
            foreach (busPersonAccountRetirement lobjPersonAccountRetirement in iclbDCMembers)
            {
                //PROD PIR 4664 - Update this Flag only for Retired / Terminated People only
                if ((lobjPersonAccountRetirement.icdoPersonAccount.status_code == "R") ||
                    (lobjPersonAccountRetirement.icdoPersonAccount.status_code == "T"))
                {
                    lobjPersonAccountRetirement.icdoPersonAccountRetirement.dc_file_sent_flag = busConstant.Flag_Yes;
                    lobjPersonAccountRetirement.icdoPersonAccountRetirement.Update();
                }
            }
        }

        public override void BeforeWriteRecord()
        {
            if (lstrTIAAFlag == busConstant.Flag_Yes)
            {
                if (iobjDetail is busPersonAccountRetirement)
                {
                    char[] temporary = new char[630]; //PIR 9122
                    char[] temp1 = new char[490];
                    StringBuilder temp = new StringBuilder();
                    StringBuilder lsbrHIPAADetail = new StringBuilder();
                    busPersonAccountRetirement lobjPersonAccountRetirement = (busPersonAccountRetirement)iobjDetail;

                    //PIR 9122
                    if ((lobjPersonAccountRetirement.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.foreign_province.IsNullOrEmpty()))
                    {
                        istrRecord.CopyTo(0, temporary, 0, (istrRecord.Length));
                        int i = 0;
                        string lstrFileRecord = string.Empty;
                        foreach (char tempChar in temporary)
                        {
                          if ((i > 197 && i > 245) || (i < 197 && i < 245))
                              lstrFileRecord = lstrFileRecord + tempChar;
                           i++;
                        }
                        istrRecord = lstrFileRecord;
                    }
                    
                    //PIR 9122
                    if (!(lobjPersonAccountRetirement.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_line_2.IsNullOrEmpty()))
                    {
                        lsbrHIPAADetail.Append("405545  " + lobjPersonAccountRetirement.ibusPerson.icdoPerson.ssn + "062");
                        if (lobjPersonAccountRetirement.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_line_2.Length <= 40)
                        {
                                lsbrHIPAADetail.Append(lobjPersonAccountRetirement.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_line_2.ToUpper());
                                lsbrHIPAADetail.Append(' ', (40 - lobjPersonAccountRetirement.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_line_2.Length)); 
                        }
                        else
                        {
                            lsbrHIPAADetail.Append(lobjPersonAccountRetirement.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_line_2.Substring(0, 40));
                        }
                        
                        if (!(lobjPersonAccountRetirement.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.foreign_province.IsNullOrEmpty()))
                        {
                            
                            lsbrHIPAADetail.Append("\r\n");
                            lsbrHIPAADetail.Append("405545  " + lobjPersonAccountRetirement.ibusPerson.icdoPerson.ssn + "063");
                            lsbrHIPAADetail.Append(lobjPersonAccountRetirement.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.foreign_province.ToUpper() 
                                +", "
                                + lobjPersonAccountRetirement.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_country_description.ToUpper()
                                +" "
                                + lobjPersonAccountRetirement.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.foreign_postal_code.ToUpper());
                        }
                        lsbrHIPAADetail.Append(" \r\n");
                        istrRecord.CopyTo(135, temp1, 0, (istrRecord.Length - 135));
                        lsbrHIPAADetail.Append(temp1);
                        temp.Append(istrRecord, 0, 135);
                        temp.Append(lsbrHIPAADetail.ToString());
                        istrRecord = temp.ToString().TrimEnd('\0');
                    }
                    else if ((lobjPersonAccountRetirement.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_line_2.IsNullOrEmpty()))
                    {
                        
                        if (!(lobjPersonAccountRetirement.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.foreign_province.IsNullOrEmpty()))
                        {
                            lsbrHIPAADetail.Append("405545  " + lobjPersonAccountRetirement.ibusPerson.icdoPerson.ssn + "062");
                            lsbrHIPAADetail.Append(lobjPersonAccountRetirement.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.foreign_province.ToUpper()
                                + ", "
                                + lobjPersonAccountRetirement.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_country_description.ToUpper()
                                + " "
                                + lobjPersonAccountRetirement.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.foreign_postal_code.ToUpper());
                            lsbrHIPAADetail.Append(" \r\n");
                        }
                        
                        istrRecord.CopyTo(135, temp1, 0, (istrRecord.Length - 135));
                        lsbrHIPAADetail.Append(temp1);
                        temp.Append(istrRecord, 0, 135);
                        temp.Append(lsbrHIPAADetail.ToString());
                        istrRecord = temp.ToString().TrimEnd('\0');
                    }

                    
                }
            }
        }
        public void LoadDCMembersForTIAACREF(DataTable ldtbDCMembers)
        {
            ldtbDCMembers = busBase.Select("cdoPersonAccountRetirement.fleFidelityDCEnrollmentOut",
                                                                    new object[0] { });
            iclbDCMembers = new Collection<busPersonAccountRetirement>();
            ldteNextRunDate = Convert.ToDateTime(iarrParameters[0]);
            lstrTIAAFlag = iarrParameters[1].ToString();
            foreach (DataRow drDCMembers in ldtbDCMembers.Rows)
            {
                busPersonAccountRetirement lobjPersonAccountRetirement = new busPersonAccountRetirement();
                lobjPersonAccountRetirement.icdoPersonAccount = new cdoPersonAccount();
                lobjPersonAccountRetirement.icdoPersonAccountRetirement = new cdoPersonAccountRetirement();
                lobjPersonAccountRetirement.icdoPersonAccount.LoadData(drDCMembers);

                lobjPersonAccountRetirement.icdoPersonAccountRetirement.LoadData(drDCMembers);
                if (!Convert.IsDBNull(drDCMembers["update_seq_pa_retirement"]))
                    lobjPersonAccountRetirement.icdoPersonAccountRetirement.update_seq = Convert.ToInt32(drDCMembers["update_seq_pa_retirement"]);

                lobjPersonAccountRetirement.ibusPerson = new busPerson();
                lobjPersonAccountRetirement.ibusPerson.icdoPerson = new cdoPerson();
                lobjPersonAccountRetirement.ibusPerson.icdoPerson.LoadData(drDCMembers);
                if (lobjPersonAccountRetirement.ibusPerson.icdoPerson.gender_value == busConstant.GenderTypeMale)
                {
                    lobjPersonAccountRetirement.ibusPerson.istrGenderForFile = "M";
                }
                else if (lobjPersonAccountRetirement.ibusPerson.icdoPerson.gender_value == busConstant.GenderTypeFemale)
                {
                    lobjPersonAccountRetirement.ibusPerson.istrGenderForFile = "F";
                }
                else
                {
                    lobjPersonAccountRetirement.ibusPerson.istrGenderForFile = " ";
                }
                if (lobjPersonAccountRetirement.ibusPerson.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusMarried)
                {
                    lobjPersonAccountRetirement.ibusPerson.istrMaritalStatusForFile = "2";
                }
                else if (lobjPersonAccountRetirement.ibusPerson.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusSingle
                    || lobjPersonAccountRetirement.ibusPerson.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusWidow
                    || lobjPersonAccountRetirement.ibusPerson.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusDivorced)
                {
                    lobjPersonAccountRetirement.ibusPerson.istrMaritalStatusForFile = "1";
                }
                else
                {
                    lobjPersonAccountRetirement.ibusPerson.istrMaritalStatusForFile = "0";
                }
                lobjPersonAccountRetirement.ibusPerson.GetPersonLatestAddress();//PIR 11867

                //PIR 9122 Start
                if (lobjPersonAccountRetirement.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.foreign_province != null)
                {
                    string lstrAddrCountryValue = lobjPersonAccountRetirement.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_country_value;

                    lobjPersonAccountRetirement.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_country_value = busGlobalFunctions.GetData1ByCodeValue(151, lstrAddrCountryValue, iobjPassInfo);
                }
                if (lobjPersonAccountRetirement.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_city.IsNotNullOrEmpty())

                    lobjPersonAccountRetirement.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_city = lobjPersonAccountRetirement.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_city.ToUpper();

                else
                    lobjPersonAccountRetirement.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_city = string.Empty;

                //PIR 9122 End
           
                lobjPersonAccountRetirement.LoadTotalVSC();
                if (lobjPersonAccountRetirement.icdoPersonAccount.Rounded_Total_VSC_In_Years_Floor >= 0 && lobjPersonAccountRetirement.icdoPersonAccount.Rounded_Total_VSC_In_Years_Floor < 10)
                {
                    lobjPersonAccountRetirement.istrYearsOfService = "0" + lobjPersonAccountRetirement.icdoPersonAccount.Rounded_Total_VSC_In_Years_Floor;
                }
                else
                {
                    lobjPersonAccountRetirement.istrYearsOfService = "" + lobjPersonAccountRetirement.icdoPersonAccount.Rounded_Total_VSC_In_Years_Floor;
                }
                //Initializing the Object to Avoid Null Exception
                lobjPersonAccountRetirement.ibusPersonEmploymentDetail = new busPersonEmploymentDetail();
                lobjPersonAccountRetirement.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail();

                lobjPersonAccountRetirement.ibusPersonEmploymentDetail.ibusPersonEmployment = new busPersonEmployment();
                lobjPersonAccountRetirement.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment = new cdoPersonEmployment();

                lobjPersonAccountRetirement.LoadAllPersonEmploymentDetails(false);

                if (lobjPersonAccountRetirement.iclbEmploymentDetail.Count > 0)
                {
                    bool lblnActiveEmploymentFound = false;
                    int lintLastEntry = lobjPersonAccountRetirement.iclbEmploymentDetail.Count - 1;
                    if (lobjPersonAccountRetirement.iclbEmploymentDetail[0].ibusPersonEmployment == null)
                        lobjPersonAccountRetirement.iclbEmploymentDetail[0].LoadPersonEmployment();

                    if (lobjPersonAccountRetirement.iclbEmploymentDetail[lintLastEntry].ibusPersonEmployment == null)
                        lobjPersonAccountRetirement.iclbEmploymentDetail[lintLastEntry].LoadPersonEmployment();

                    //Set the Start Date as Least Employment Start Date
                    lobjPersonAccountRetirement.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.start_date =
                        lobjPersonAccountRetirement.iclbEmploymentDetail[lintLastEntry].ibusPersonEmployment.icdoPersonEmployment.start_date;


                    if (lobjPersonAccountRetirement.iclbEmploymentDetail[0].ibusPersonEmployment.icdoPersonEmployment.end_date_no_null <= DateTime.Now)
                    {
                        lobjPersonAccountRetirement.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.end_date =
                            lobjPersonAccountRetirement.iclbEmploymentDetail[0].ibusPersonEmployment.icdoPersonEmployment.end_date;
                    }
                }

                if (lobjPersonAccountRetirement.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.end_date == DateTime.MinValue
                    || lobjPersonAccountRetirement.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.end_date > DateTime.Now)
                {
                    lobjPersonAccountRetirement.icdoPersonAccount.status_code = "A";
                }
                
                else
                {
                    lobjPersonAccountRetirement.icdoPersonAccount.status_code = "T";
                }
                lobjPersonAccountRetirement.idtePayrollDate = ldteNextRunDate;
                if (lobjPersonAccountRetirement.icdoPersonAccount.Total_VSC_in_Years < 2)
                {
                    lobjPersonAccountRetirement.istrVestingPercent = "0000";
                }
                else if (lobjPersonAccountRetirement.icdoPersonAccount.Total_VSC_in_Years >= 2 && lobjPersonAccountRetirement.icdoPersonAccount.Total_VSC_in_Years < 3)
                {
                    lobjPersonAccountRetirement.istrVestingPercent = "0500";
                }
                else if (lobjPersonAccountRetirement.icdoPersonAccount.Total_VSC_in_Years >= 3 && lobjPersonAccountRetirement.icdoPersonAccount.Total_VSC_in_Years < 4)
                {
                    lobjPersonAccountRetirement.istrVestingPercent = "0750";
                }
                else
                {
                    lobjPersonAccountRetirement.istrVestingPercent = "1000";
                }
                iclbDCMembers.Add(lobjPersonAccountRetirement);
            }
        }
    }
}