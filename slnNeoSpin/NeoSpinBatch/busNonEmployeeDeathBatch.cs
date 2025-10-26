using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Data;
using NeoSpin.BusinessObjects;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;
using System.Linq;

namespace NeoSpinBatch
{
    class busNonEmployeeDeathBatch : busNeoSpinBatch
    {
        public void GenerateCorrepondenceForNonEmployeeDeath()
        {
            istrProcessName = "Non Employee Death Batch ";
            busBase lobjBase = new busBase();
            idlgUpdateProcessLog("Load all deceased Person not enrolled in any Retirement Or Insurance Plan", "INFO", istrProcessName);

            DataTable ldtbGetNonEmployeeDeathRecords = busBase.Select("cdoBenefitApplication.NonEmployeeDeathBatch", new object[0] { });
            if (ldtbGetNonEmployeeDeathRecords.Rows.Count > 0)
            {
                foreach (DataRow dr in ldtbGetNonEmployeeDeathRecords.Rows)
                {
                    busDeathNotification lobjDeathNotification = new busDeathNotification { icdoDeathNotification = new cdoDeathNotification() };
                    lobjDeathNotification.icdoDeathNotification.LoadData(dr);

                    idlgUpdateProcessLog("Processing Non Employee Death Notification for the Person "
                        + lobjDeathNotification.icdoDeathNotification.person_id, "INFO", istrProcessName);

                    lobjDeathNotification.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                    lobjDeathNotification.ibusPerson.icdoPerson.LoadData(dr);

                    if (lobjDeathNotification.ibusPerson.iclbPersonAccountBeneficiary == null)
                        lobjDeathNotification.LoadPersonAccountBeneficiary();

                    //declared a dummy object to check duplicate generation of correspondence
                    Collection<busPerson> lobjDummyPerson = new Collection<busPerson>();
                    foreach (busPersonAccountBeneficiary lobjPersonAccountBeneficiary in lobjDeathNotification.ibusPerson.iclbPersonAccountBeneficiary)
                    {
                        lobjDeathNotification.SetPABeneficiaryCorrespondenceProperties(lobjDummyPerson, lobjPersonAccountBeneficiary);
                    }

                    lobjDeathNotification.LoadPersonAccountDependent();
                    foreach (busPersonAccountDependent lobjPersonAccountDependent in lobjDeathNotification.ibusPerson.iclbPersonAccountDependent)
                    {
                        lobjDeathNotification.SetPADependentCorrespondenceProperties(lobjDummyPerson, lobjPersonAccountDependent);
                    }

                    //update flag that will check no correspondence for the same person send again
                    lobjDeathNotification.icdoDeathNotification.non_employee_death_batch_flag = busConstant.Flag_Yes;
                    lobjDeathNotification.icdoDeathNotification.Update();
                }
            }
            else
            {
                idlgUpdateProcessLog("No Records found", "INFO", istrProcessName);
            }

        }


        private void CreateCorrespondence(busDeathNotification aobjDeathNotification)
        {
            // Generate Correspondence
            //ArrayList larrlist = new ArrayList();
            //larrlist.Add(aobjDeathNotification);
            Hashtable lhstDummyTable = new Hashtable();
            lhstDummyTable.Add("sfwCallingForm", "Batch");
            CreateCorrespondence("APP-7500", aobjDeathNotification, lhstDummyTable);
        }
    }
}
