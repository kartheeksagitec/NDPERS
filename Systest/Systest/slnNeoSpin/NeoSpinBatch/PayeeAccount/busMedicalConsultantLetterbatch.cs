#region Using directives
using System;
using System.Data;
using NeoSpin.CustomDataObjects;
using NeoSpin.BusinessObjects;
using System.Collections.ObjectModel;
using Sagitec.BusinessObjects;
using System.Collections;

#endregion

namespace NeoSpinBatch
{
    public class busMedicalConsultantLetterbatch : busNeoSpinBatch
    {
        public void MedicalConsultantLetter()
        {
            istrProcessName = "Medical Consultant Record Purge";
            int lintNoofMonths = Convert.ToInt32(busGlobalFunctions.GetData1ByCodeValue(busConstant.SystemConstantCodeID,"MONT",iobjPassInfo));
            //DateTime ldteEndDate = iobjSystemManagement.icdoSystemManagement.batch_date.AddMonths(lintNoofMonths);
            DataTable ldtbResult = busBase.Select("cdoPayeeAccount.MedicalConsultantLetterBatch",
                                            new object[2] { iobjSystemManagement.icdoSystemManagement.batch_date, lintNoofMonths });
            if (ldtbResult.Rows.Count > 0)
            {
                idlgUpdateProcessLog("Processing Fetched Member Records", "INFO", istrProcessName);
                busCase lobjCase = new busCase { icdoCase = new cdoCase(), iclbMedicalConsultantRecordsPurge = new Collection<busPerson>() };
                foreach (DataRow dr in ldtbResult.Rows)
                {
                    busPayeeAccount lobjPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                    lobjPayeeAccount.ibusMember = new busPerson { icdoPerson=new cdoPerson() };

                    lobjPayeeAccount.icdoPayeeAccount.LoadData(dr);
                    lobjPayeeAccount.ibusMember.icdoPerson.LoadData(dr);

                    lobjCase.iclbMedicalConsultantRecordsPurge.Add(lobjPayeeAccount.ibusMember);

                    // Create a Contact Ticket
                    busContactTicket lobjContactTicket=new busContactTicket();
                    lobjContactTicket.icdoContactTicket=new cdoContactTicket();
                    CreateContactTicket(lobjPayeeAccount.ibusMember.icdoPerson.person_id,
                                        busConstant.ContactTicketTypeRetAccount,
                                        busConstant.ResponseMethodCorrespondence,
                                        string.Empty,
                                        lobjContactTicket.icdoContactTicket);

                    // Updates the Flag
                    lobjPayeeAccount.icdoPayeeAccount.is_medical_batch_letter_sent_flag = busConstant.Flag_Yes;
                    lobjPayeeAccount.icdoPayeeAccount.Update();
                }

                // Create a Single Letter contains  all records needs to be purged by Medical Consultant
                idlgUpdateProcessLog("Generating Letter to Medical Consultant", "INFO", istrProcessName);
                //ArrayList larrList = new ArrayList();
                //larrList.Add(lobjCase);
                Hashtable lshtTemp = new Hashtable();
                lshtTemp.Add("FormTable", "Batch");
                CreateCorrespondence("PAY-4257", lobjCase, lshtTemp);
            }
            else
            {
                idlgUpdateProcessLog("No Records Fetched", "INFO", istrProcessName);
            }
        }
    }
}
