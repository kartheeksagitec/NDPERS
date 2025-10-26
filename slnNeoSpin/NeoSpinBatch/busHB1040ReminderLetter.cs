using NeoSpin.BusinessObjects;
using NeoSpin.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.ExceptionPub;
using System;
using System.Collections;
using System.Data;
using Sagitec.CustomDataObjects;
using System.Collections.ObjectModel;
using System.Linq;
using NeoSpin.DataObjects;
using Microsoft.VisualBasic;
using Sagitec.DBUtility;


namespace NeoSpinBatch
{
    class busHB1040ReminderLetter : busNeoSpinBatch
    {
        public void ProcessHB1040ReminderLetter()
        {
            istrProcessName = "HB1040 Reminder Letter";
            idlgUpdateProcessLog("Loading All Eligible Members HB1040 Reminder Letter Batch.", "INFO", istrProcessName);
            DataTable ldtbHB1040ReminderMembers = busBase.Select("entHB1040Communication.GetPersonToSendReminderCorrespondence", new object[0] { });
            foreach (DataRow ldrRow in ldtbHB1040ReminderMembers.Rows)
            {
                try
                {
                    busHb1040Communication lbusHb1040Communication = new busHb1040Communication { icdoHb1040Communication = new doHb1040Communication() };
                    lbusHb1040Communication.icdoHb1040Communication.LoadData(ldrRow);
                    busPersonAccountRetirement lbusPersonAccountRetirement = new busPersonAccountRetirement { icdoPersonAccount = new cdoPersonAccount() };
                    lbusPersonAccountRetirement.FindPersonAccount(lbusHb1040Communication.icdoHb1040Communication.person_account_id);
                    lbusPersonAccountRetirement.FindPersonAccountRetirement(lbusHb1040Communication.icdoHb1040Communication.person_account_id);
                    lbusPersonAccountRetirement.LoadPerson();
                    lbusPersonAccountRetirement.LoadHb1040Communication();
                    CreateCorrespondence(lbusPersonAccountRetirement);

                    lbusHb1040Communication.icdoHb1040Communication.letter_generated_2 = busConstant.Flag_Yes;
                    lbusHb1040Communication.icdoHb1040Communication.letter_generated_date_2 = DateTime.Now;
                    lbusHb1040Communication.icdoHb1040Communication.Update();
                }
                catch (Exception _exc)
                {
                    idlgUpdateProcessLog("ERROR:" + _exc.Message, "INFO", istrProcessName);
                }
            }
            //updating the cor status to Ready for Imaging 
            DBFunction.DBNonQuery("entCorTracking.UpdateCorStatusToImagingFromCommunicationBatch", new object[0] { },
                    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
        }
        
        private void CreateCorrespondence(busPersonAccountRetirement abusPersonAccountRetirement)
        {
            Hashtable lhstDummyTable = new Hashtable();
            lhstDummyTable.Add("sfwCallingForm", "Batch");
            CreateCorrespondence("ENR-5306", abusPersonAccountRetirement, lhstDummyTable);
            idlgUpdateProcessLog("Correspondence Created successfully for the PERSLink ID : " + abusPersonAccountRetirement.icdoPersonAccount.person_id, "INFO", istrProcessName);
        }

    }
}
