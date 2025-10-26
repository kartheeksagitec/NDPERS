#region Using directives
using System;
using System.Data;
using System.Collections.Generic;
using System.Diagnostics;
using NeoSpin.CustomDataObjects;
using NeoSpin.BusinessObjects;
using System.Collections.ObjectModel;
using Sagitec.DataObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.CustomDataObjects;
using Sagitec.BusinessObjects;
using System.Collections;
using Sagitec.CorBuilder;

#endregion

namespace NeoSpinBatch
{
    class busDCTransferReminderBatch : busNeoSpinBatch
    {
        public void CreateDCTransferReminderBatch()
        {
            istrProcessName = "DC Transfer Reminder Batch";
            idlgUpdateProcessLog("Creating Correspondence for Reminding DC Transfer", "INFO", istrProcessName);
            DataTable ldtbDCTransfer = DBFunction.DBSelect("cdoPersonAccountRetirement.DCTransferReminderBatch", new object[] { },
                                            iobjPassInfo.iconFramework,
                                            iobjPassInfo.itrnFramework);
            foreach (DataRow dr in ldtbDCTransfer.Rows)
            {

                busPersonAccountRetirement lobjRetirement = new busPersonAccountRetirement();
                lobjRetirement.icdoPersonAccountRetirement = new cdoPersonAccountRetirement();
                lobjRetirement.icdoPersonAccountRetirement.LoadData(dr);

                int lintPersonID = 0;
                if (!Convert.IsDBNull(dr["PERSON_ID"]))
                    lintPersonID = Convert.ToInt32(dr["PERSON_ID"]);

                if (!Convert.IsDBNull(dr["HISTORY_ELIGIBILITY_DATE"]))
                    lobjRetirement.icdoPersonAccountRetirement.dc_eligibility_long_date = Convert.ToDateTime(dr["HISTORY_ELIGIBILITY_DATE"]).ToString(busConstant.DateFormatLongDate);

                //Skip the Record if the Letter Already Sent (Three Phase)
                if ((lobjRetirement.icdoPersonAccountRetirement.LetterNo == 1) &&
                    (lobjRetirement.icdoPersonAccountRetirement.dc_trasnfer_reminder_letter1_flag == busConstant.Flag_Yes))
                    continue;

                if ((lobjRetirement.icdoPersonAccountRetirement.LetterNo == 2) &&
                    (lobjRetirement.icdoPersonAccountRetirement.dc_trasnfer_reminder_letter2_flag == busConstant.Flag_Yes))
                    continue;

                //ArrayList larrList = new ArrayList();
                //larrList.Add(lobjRetirement);
                Hashtable lshtTemp = new Hashtable();
                lshtTemp.Add("sfwCallingForm", "Batch");
                if (lobjRetirement.icdoPersonAccountRetirement.LetterNo > 0)
                {
                    string lstrFileName = CreateCorrespondence("ENR-5300", lobjRetirement, lshtTemp);
                    CreateContactTicket(lintPersonID);
                    idlgUpdateProcessLog("Creating Reminder Letter for PERSLink ID : " + lintPersonID, "INFO", istrProcessName);

                    //Update the Flag
                    if (lobjRetirement.icdoPersonAccountRetirement.LetterNo == 1)
                    {
                        lobjRetirement.icdoPersonAccountRetirement.dc_trasnfer_reminder_letter1_flag = busConstant.Flag_Yes;
                        lobjRetirement.icdoPersonAccountRetirement.dc_trasnfer_reminder_letter1_date = iobjSystemManagement.icdoSystemManagement.batch_date;
                    }
                    else if (lobjRetirement.icdoPersonAccountRetirement.LetterNo == 2)
                    {
                        lobjRetirement.icdoPersonAccountRetirement.dc_trasnfer_reminder_letter2_flag = busConstant.Flag_Yes;
                    }
                    
                    lobjRetirement.icdoPersonAccountRetirement.Update();
                }

            }
            idlgUpdateProcessLog("Correspondence created successfully", "INFO", istrProcessName);
        }


        // Create Contact Ticket
        private void CreateContactTicket(int aintPersonID)
        {
            cdoContactTicket lobjContactTicket = new cdoContactTicket();
            CreateContactTicket(aintPersonID, busConstant.ContactTicketTypeInsuranceRetiree, lobjContactTicket);
        }
    }
}
