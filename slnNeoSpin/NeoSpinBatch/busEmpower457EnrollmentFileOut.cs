using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using NeoSpin.BusinessObjects;
using System.Collections.ObjectModel;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;
using System.Collections;
using Sagitec.DBUtility;

namespace NeoSpinBatch
{
    class busEmpower457EnrollmentFileOut : busNeoSpinBatch
    {
        public busEmpower457EnrollmentFileOut()
        { }
        public Collection<busPersonAccountDeferredComp> iclb457Members;
        public void Generate457EnrollmentFile()
        {
            DataTable ldtb457Members = busBase.Select("cdoPersonAccountDeferredComp.fleFidelity457EnrollmentOut",
                                                                                    new object[0] { });
            iclb457Members = new Collection<busPersonAccountDeferredComp>();

            foreach (DataRow dr in ldtb457Members.Rows)
            {
                busPersonAccountDeferredComp lobjPersonAccountDeferredComp = new busPersonAccountDeferredComp
                {
                    icdoPersonAccount = new cdoPersonAccount(),
                    icdoPersonAccountDeferredComp = new cdoPersonAccountDeferredComp(),
                    ibusPerson = new busPerson { icdoPerson = new cdoPerson() },
                    ibusPersonEmploymentDetail = new busPersonEmploymentDetail
                    {
                        icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail(),
                        ibusPersonEmployment = new busPersonEmployment { icdoPersonEmployment = new cdoPersonEmployment() }
                    }
                };

                lobjPersonAccountDeferredComp.icdoPersonAccount.LoadData(dr);
                lobjPersonAccountDeferredComp.icdoPersonAccountDeferredComp.LoadData(dr);
                if (!Convert.IsDBNull(dr["UPDATE_SEQ_FOR_DEFE_COMP"]))
                    lobjPersonAccountDeferredComp.icdoPersonAccountDeferredComp.update_seq = Convert.ToInt32(dr["UPDATE_SEQ_FOR_DEFE_COMP"]);
                lobjPersonAccountDeferredComp.ibusPerson.icdoPerson.LoadData(dr);
                if (lobjPersonAccountDeferredComp.ibusPerson.icdoPerson.gender_value.IsNotNullOrEmpty() && lobjPersonAccountDeferredComp.ibusPerson.icdoPerson.gender_value == busConstant.GenderTypeMale)
                {
                    lobjPersonAccountDeferredComp.ibusPerson.istrGenderForFile = "M";
                }
                else if (lobjPersonAccountDeferredComp.ibusPerson.icdoPerson.gender_value.IsNotNullOrEmpty() && lobjPersonAccountDeferredComp.ibusPerson.icdoPerson.gender_value == busConstant.GenderTypeFemale)
                {
                    lobjPersonAccountDeferredComp.ibusPerson.istrGenderForFile = "F";
                }
                else
                {
                    lobjPersonAccountDeferredComp.ibusPerson.istrGenderForFile = "U";
                }
                if (lobjPersonAccountDeferredComp.ibusPerson.icdoPerson.marital_status_value.IsNotNullOrEmpty() && lobjPersonAccountDeferredComp.ibusPerson.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusMarried)
                {
                    lobjPersonAccountDeferredComp.ibusPerson.istrMaritalStatusForFile = "M";
                }
                else if (lobjPersonAccountDeferredComp.ibusPerson.icdoPerson.marital_status_value.IsNotNullOrEmpty() && lobjPersonAccountDeferredComp.ibusPerson.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusSingle)   
                {
                    lobjPersonAccountDeferredComp.ibusPerson.istrMaritalStatusForFile = "S";
                }
                else if (lobjPersonAccountDeferredComp.ibusPerson.icdoPerson.marital_status_value.IsNotNullOrEmpty() && lobjPersonAccountDeferredComp.ibusPerson.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusDivorced)
                {
                    lobjPersonAccountDeferredComp.ibusPerson.istrMaritalStatusForFile = "D";
                }
                else
                {
                    lobjPersonAccountDeferredComp.ibusPerson.istrMaritalStatusForFile = "W";
                }
                lobjPersonAccountDeferredComp.ibusPerson.GetPersonLatestAddress();// PIR 11867

                //PIR 9122 Start
                // PIR 26456 country code for non US address
                if (lobjPersonAccountDeferredComp.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_country_value != "0001") 
                {
                    string lstrAddrCountryValue = lobjPersonAccountDeferredComp.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_country_value;

                    lobjPersonAccountDeferredComp.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_country_value = busGlobalFunctions.GetData1ByCodeValue(151, lstrAddrCountryValue, iobjPassInfo);

                    if (lobjPersonAccountDeferredComp.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.foreign_province.IsNotNullOrEmpty())
                    {
                        string lstrAddrStateValue = lobjPersonAccountDeferredComp.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.foreign_province;
                        Sagitec.CustomDataObjects.cdoCodeValue lcdoCodeValue = new Sagitec.CustomDataObjects.cdoCodeValue();
                        lcdoCodeValue =  busGlobalFunctions.GetCodeValueByDescription(150, lstrAddrStateValue);

                        lobjPersonAccountDeferredComp.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_state_value = lcdoCodeValue.code_value;

                        if (lobjPersonAccountDeferredComp.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.foreign_province != lcdoCodeValue.description)
                        {
                            lobjPersonAccountDeferredComp.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_state_value = lobjPersonAccountDeferredComp.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.foreign_province;
                        }
                    }
                    else if ((lobjPersonAccountDeferredComp.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.foreign_province.IsNullOrEmpty()) && (lobjPersonAccountDeferredComp.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_state_value.IsNotNullOrEmpty()))
                    {
                        lobjPersonAccountDeferredComp.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.foreign_province = lobjPersonAccountDeferredComp.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_state_value;
                    }
                    else if ((lobjPersonAccountDeferredComp.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.foreign_province.IsNullOrEmpty()) && (lobjPersonAccountDeferredComp.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_state_value.IsNullOrEmpty()))
                    {
                        lobjPersonAccountDeferredComp.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_state_value = string.Empty;
                    }
                    if (lobjPersonAccountDeferredComp.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.foreign_postal_code.IsNotNullOrEmpty())
                    {
                        lobjPersonAccountDeferredComp.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_zip_code = lobjPersonAccountDeferredComp.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.foreign_postal_code;
                    }
                    else if ((lobjPersonAccountDeferredComp.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.foreign_postal_code.IsNullOrEmpty()) && (lobjPersonAccountDeferredComp.ibusPerson.ibusPersonCurrentAddress.istrZipCodeForFile.IsNotNullOrEmpty()))
                    {
                        lobjPersonAccountDeferredComp.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.foreign_postal_code = lobjPersonAccountDeferredComp.ibusPerson.ibusPersonCurrentAddress.istrZipCodeForFile;
                    }
                    else if ((lobjPersonAccountDeferredComp.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.foreign_postal_code.IsNullOrEmpty()) && (lobjPersonAccountDeferredComp.ibusPerson.ibusPersonCurrentAddress.istrZipCodeForFile.IsNullOrEmpty()))
                    {
                        lobjPersonAccountDeferredComp.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_zip_code = string.Empty;
                    }
                }
                else if (lobjPersonAccountDeferredComp.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_country_value == "0001") // PIR 26456 country code for US address
                {
                    lobjPersonAccountDeferredComp.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_country_value = string.Empty;

                    if (lobjPersonAccountDeferredComp.ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_state_value.IsNotNullOrEmpty())
                    {
                        lobjPersonAccountDeferredComp.ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_state_value = lobjPersonAccountDeferredComp.ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_state_value.ToUpper();
                    }
                }
                if (lobjPersonAccountDeferredComp.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_city.IsNotNullOrEmpty())
                {
                    lobjPersonAccountDeferredComp.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_city = lobjPersonAccountDeferredComp.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_city.ToUpper();
                }
                else
                    lobjPersonAccountDeferredComp.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_city = string.Empty;   
                //PIR 9122 End

                lobjPersonAccountDeferredComp.LoadAllPersonEmploymentDetails(false);
                lobjPersonAccountDeferredComp.idtePayrollDate = iobjSystemManagement.icdoSystemManagement.batch_date.AddDays(1); 
                foreach (busPersonEmploymentDetail lobjEmpDtl in lobjPersonAccountDeferredComp.iclbEmploymentDetail)
                    lobjEmpDtl.LoadPersonEmployment();

                if (lobjPersonAccountDeferredComp.iclbEmploymentDetail.Count > 0)
                {
                    //Set the Start Date as Least Employment Start Date
                    int lintLastEntry = lobjPersonAccountDeferredComp.iclbEmploymentDetail.Count - 1;
                    lobjPersonAccountDeferredComp.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.start_date =
                            lobjPersonAccountDeferredComp.iclbEmploymentDetail[lintLastEntry].ibusPersonEmployment.icdoPersonEmployment.start_date;

                    //Set the End Date as Top Employment End Date
                    //PIR 9903
                    if (lobjPersonAccountDeferredComp.iclbEmploymentDetail[0].ibusPersonEmployment.icdoPersonEmployment.end_date < DateTime.Now)
                    {
                        lobjPersonAccountDeferredComp.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.end_date =
                                lobjPersonAccountDeferredComp.iclbEmploymentDetail[0].ibusPersonEmployment.icdoPersonEmployment.end_date;
                    }
                }

                // PIR ID 1603
                // Status Code is T when all employment records associated with the Def comp plan are end dated.
                if (lobjPersonAccountDeferredComp.iclbEmploymentDetail.Where(row => row.ibusPersonEmployment.icdoPersonEmployment.end_date == DateTime.MinValue
                    || row.ibusPersonEmployment.icdoPersonEmployment.end_date > DateTime.Now).Any())
                {
                    lobjPersonAccountDeferredComp.icdoPersonAccount.status_code = "A";
                }
                else
                {
                    lobjPersonAccountDeferredComp.icdoPersonAccount.status_code = "T";

                    lobjPersonAccountDeferredComp.icdoPersonAccountDeferredComp.file_457_sent_flag = busConstant.Flag_Yes;
                    lobjPersonAccountDeferredComp.icdoPersonAccountDeferredComp.Update();
                }
                if (lobjPersonAccountDeferredComp.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.person_address_id > 0)
                {
                    iclb457Members.Add(lobjPersonAccountDeferredComp);
                }
                else
                {
                    istrProcessName = "Empower 457 Enrollment - Outbound File";
                    idlgUpdateProcessLog("Person " + lobjPersonAccountDeferredComp.ibusPerson.icdoPerson.person_id + " does not have address as of today", "ERR", istrProcessName);
                }
                
            }
            busProcessOutboundFile lobjProcessFiles = new busProcessOutboundFile();
            lobjProcessFiles.iarrParameters = new object[3];
            lobjProcessFiles.iarrParameters[0] = iclb457Members;
            lobjProcessFiles.iarrParameters[1] = busConstant.Flag_Yes;
            lobjProcessFiles.iarrParameters[2] = iobjSystemManagement.icdoSystemManagement.batch_date.AddDays(1); 
            lobjProcessFiles.CreateOutboundFile(106);
        }
    }
}
