#region Using directives
using System;
using System.Data;
using NeoSpin.CustomDataObjects;
using NeoSpin.BusinessObjects;
using Sagitec.DBUtility;
using System.Collections;

#endregion

namespace NeoSpinBatch
{
    class busMedicareAge65Batch : busNeoSpinBatch
    {
        private DateTime ldtDateToCheck
        {
            get
            {
                int lintYear = iobjSystemManagement.icdoSystemManagement.batch_date.AddMonths(2).Year;
                int lintMonth = iobjSystemManagement.icdoSystemManagement.batch_date.AddMonths(2).Month;
                int lintDay=DateTime.DaysInMonth(lintYear,lintMonth);
                DateTime _ldtDateToCheck = new DateTime(lintYear, lintMonth, lintDay);
                return _ldtDateToCheck;
            }
        }

        private DateTime ldtLastDayofCurrentMonth
        {
            get
            {
                int lintyear = iobjSystemManagement.icdoSystemManagement.batch_date.Year;
                int lintMonth = iobjSystemManagement.icdoSystemManagement.batch_date.Month;
                int lintday = DateTime.DaysInMonth(lintyear, lintMonth);
                DateTime _ldtLastDayofCurrentMonth = new DateTime(lintyear, lintMonth, lintday);
                return _ldtLastDayofCurrentMonth;
            }
        }

        private DateTime ldtFirstDayofCurrentMonth
        {
            get
            {
                int lintyear = iobjSystemManagement.icdoSystemManagement.batch_date.Year;
                int lintMonth = iobjSystemManagement.icdoSystemManagement.batch_date.Month;
                int lintday = 1;
                DateTime _ldtFirstDayofCurrentMonth = new DateTime(lintyear, lintMonth, lintday);
                return _ldtFirstDayofCurrentMonth;
            }
        }

        public void CreateMedicareAge65BatchLetters()
        {
            istrProcessName = "Medicare Age 65 Batch Letter";
            // Query will fetch all Member's record whose age is greater than 64.
            // Batch will filter from those and generate records only if their current year birthdate falls between batch run date and the 2 months after the batch run date.
            idlgUpdateProcessLog("Create Correspondence for Medicare Age 65 Notice Letters", "INFO", istrProcessName);
            DataTable ldtbMember = DBFunction.DBSelect("cdoPersonAccountGhdv.MedicareAge65Letter", new object[] { },
                                            iobjPassInfo.iconFramework,
                                            iobjPassInfo.itrnFramework);
            foreach (DataRow dr in ldtbMember.Rows)
            {
                busPersonDependent lobjDependent = new busPersonDependent();
                lobjDependent.icdoPersonDependent = new cdoPersonDependent();
                lobjDependent.ibusPerson = new busPerson();
                lobjDependent.ibusPerson.icdoPerson = new cdoPerson();
                lobjDependent.ibusPeronAccountDependent = new busPersonAccountDependent();
                lobjDependent.ibusPeronAccountDependent.icdoPersonAccountDependent = new cdoPersonAccountDependent();
                // Check for Dependent
                if (dr["PERSON_DEPENDENT_ID"] != DBNull.Value)
                {
                    lobjDependent.icdoPersonDependent.LoadData(dr);
                    lobjDependent.ibusPeronAccountDependent.icdoPersonAccountDependent.LoadData(dr);
                    lobjDependent.LoadDependentInfo();
                    lobjDependent.LoadPerson();
                    bool iblnDependentBPMCorrespondence = false;
                    /// If Dependent's Day of Birth is First of month

                    int lintMonthsDep = 12 * (DateTime.Now.Year - lobjDependent.icdoPersonDependent.dependent_DOB.Year) + DateTime.Now.Month - lobjDependent.icdoPersonDependent.dependent_DOB.Month;

                    if (lobjDependent.icdoPersonDependent.dependent_DOB.Day == 1)
                    {
                        if (lintMonthsDep >= 776)
                        {
                            iblnDependentBPMCorrespondence = true;
                        }
                    }
                    else
                    {
                        if (lintMonthsDep >= 777)
                        {
                            iblnDependentBPMCorrespondence = true;
                        }
                    }
                    if ((iblnDependentBPMCorrespondence) &&
                        lobjDependent.icdoPersonDependent.istrMedicareAge65LetterSentFlagDependent != busConstant.Flag_Yes)

                    {
                        lobjDependent.icdoPersonDependent.full_name = lobjDependent.icdoPersonDependent.dependent_name;
                        lobjDependent.icdoPersonDependent.CurrentYearBirthDate = lobjDependent.icdoPersonDependent.dependent_DOB.AddYears(65);
                        idlgUpdateProcessLog("Creating Correspondence for dependent " + Convert.ToString(lobjDependent.icdoPersonDependent.full_name), "INFO", istrProcessName);
                        CreateMedicareAge65Letter(lobjDependent);
                        busPersonDependent lobjPersonDependent = new busPersonDependent();
                        lobjPersonDependent.FindPersonDependent(lobjDependent.icdoPersonDependent.person_dependent_id);
                        lobjPersonDependent.icdoPersonDependent.medicare_age_65_letter_sent_flag = busConstant.Flag_Yes;
                        lobjPersonDependent.icdoPersonDependent.Update();
                    }
                    //if (iblnDependentWorkflow)
                    //{
                    //    idlgUpdateProcessLog("Initiating workflow for member " + Convert.ToString(lobjDependent.ibusPerson.icdoPerson.FullName), "INFO", istrProcessName);
                    //    InitializeWorkFlow(lobjDependent.ibusPerson.icdoPerson.person_id);
                    //}
                }
                // Check for Member
                busPersonAccountGhdv lobjGHDV = new busPersonAccountGhdv();
                lobjGHDV.icdoPersonAccountGhdv = new cdoPersonAccountGhdv();
                lobjGHDV.icdoPersonAccount = new cdoPersonAccount();
                lobjGHDV.icdoPersonAccountGhdv.LoadData(dr);
                lobjGHDV.icdoPersonAccount.LoadData(dr);
                lobjGHDV.LoadPerson();
                lobjDependent.ibusPerson = lobjGHDV.ibusPerson;
                /// Is Member's Day of Birth is First of month
                bool iblnMemberBPMCorrespondence = false;

                int lintMonthMember = 12 * (DateTime.Now.Year - lobjDependent.ibusPerson.icdoPerson.date_of_birth.Year) + DateTime.Now.Month - lobjDependent.ibusPerson.icdoPerson.date_of_birth.Month;

                if (lobjDependent.ibusPerson.icdoPerson.date_of_birth.Day == 1)
                {                   
                    if (lintMonthMember >= 776)
                    {
                        iblnMemberBPMCorrespondence = true;
                    }
                }
                else
                {                   
                    if (lintMonthMember >= 777)
                    {
                        iblnMemberBPMCorrespondence = true;
                    }
                }
                /// Generates Correspondence only if Dependent or Member is nearing age 65
                if ((iblnMemberBPMCorrespondence) &&
                    (lobjGHDV.icdoPersonAccountGhdv.medicare_age_65_letter_sent_flag != busConstant.Flag_Yes))
                {
                    lobjDependent.icdoPersonDependent.dependent_first_name = lobjDependent.ibusPerson.icdoPerson.first_name;
                    lobjDependent.icdoPersonDependent.CurrentYearBirthDate = lobjDependent.ibusPerson.icdoPerson.date_of_birth.AddYears(65);
                    idlgUpdateProcessLog("Creating Correspondence for member " + Convert.ToString(lobjGHDV.ibusPerson.icdoPerson.FullName), "INFO", istrProcessName);
                    CreateMedicareAge65Letter(lobjDependent);
                    busPersonAccountGhdv lobjMember = new busPersonAccountGhdv();
                    lobjMember.FindGHDVByPersonAccountID(lobjGHDV.icdoPersonAccount.person_account_id);
                    lobjMember.icdoPersonAccountGhdv.medicare_age_65_letter_sent_flag = busConstant.Flag_Yes;
                    lobjMember.icdoPersonAccountGhdv.Update();
                }
                //if (iblnMemberWorkflow)
                //{
                //    idlgUpdateProcessLog("Initiating workflow for member " + Convert.ToString(lobjGHDV.ibusPerson.icdoPerson.FullName), "INFO", istrProcessName);
                //    InitializeWorkFlow(lobjDependent.ibusPerson.icdoPerson.person_id);
                //}
            }
            idlgUpdateProcessLog("Correspondence created successfully", "INFO", istrProcessName);
        }

        //Create Correspondence Letter
        public void CreateMedicareAge65Letter(busPersonDependent aobjPersonDependent)
        {
            //ArrayList larrList = new ArrayList();          
            //    larrList.Add(aobjPersonDependent);          

            Hashtable lshtTemp = new Hashtable();
            lshtTemp.Add("FormTable", "Batch");
            string lstrFileName = CreateCorrespondence("ENR-5150", aobjPersonDependent, lshtTemp);
            // WorkFlow will initialize a Contact Ticket.
            InitializeWorkFlow(aobjPersonDependent.ibusPerson.icdoPerson.person_id);            
        }

        private void InitializeWorkFlow(int aintPersonID)
        {     
            busWorkflowHelper.InitiateBpmRequest(busConstant.Map_Manage_Medicare_Age65_Letter, aintPersonID, 0, 0, iobjPassInfo, busConstant.WorkflowProcessSource_Batch);

        }
    }
}
