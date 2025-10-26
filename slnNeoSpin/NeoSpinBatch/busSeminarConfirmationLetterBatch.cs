using NeoSpin.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using NeoSpin.CustomDataObjects;


namespace NeoSpinBatch
{
    class busSeminarConfirmationLetterBatch : busNeoSpinBatch
    {
        public busSeminarConfirmationLetterBatch() {}
        public void GenerateCorrepondenceForSeminarConfirmationLetter()
        {
            idlgUpdateProcessLog("Loading Seminars within 21 days from the batch run date.", "INFO", iobjBatchSchedule.step_name);
            DataTable ldtSeminarSchedules = busNeoSpinBase.Select("cdoSeminarSchedule.LoadSeminarsForConfirmationLetter", new object[0] { });
            if (ldtSeminarSchedules.Rows.Count > 0)
            {
                foreach (DataRow drSeminarSchedule in ldtSeminarSchedules.Rows)
                {
                    int lintSeminarScheduleId = Convert.ToInt32(drSeminarSchedule["SEMINAR_SCHEDULE_ID"]);
                    busSeminarSchedule lbusSeminarScheduleHeader = new busSeminarSchedule();
                    lbusSeminarScheduleHeader.FindSeminarSchedule(lintSeminarScheduleId);
                    DataTable ldtSeminarDetails = busNeoSpinBase.Select("cdoSeminarSchedule.LoadSeminarDetailsForConfirmationLetter", new object[1] { (Convert.ToInt32(drSeminarSchedule["SEMINAR_SCHEDULE_ID"])) });
                    if (ldtSeminarDetails.Rows.Count > 0)
                    {
                        foreach (DataRow dr in ldtSeminarDetails.Rows)
                        {
                            busSeminarSchedule lobjbusSeminarSchedule = new busSeminarSchedule();
                            lobjbusSeminarSchedule.icdoSeminarSchedule = new cdoSeminarSchedule();
                            lobjbusSeminarSchedule.icdoSeminarSchedule.LoadData(dr);
                            lobjbusSeminarSchedule.icdoSeminarSchedule.seminar_type_description = Convert.ToString(dr["SEMINAR_TYPE_DESCRIPTION"]);
                            try
                            {
                                CreateCorrespondence(lobjbusSeminarSchedule);
                            }
                            catch (Exception ex)
                            {
                                idlgUpdateProcessLog("Exception occurred while processing seminar schedule id " + Convert.ToString(drSeminarSchedule["SEMINAR_SCHEDULE_ID"]) + ", Exception : " + ex.Message, "ERR", iobjBatchSchedule.step_name);
                            }
                        }
                        lbusSeminarScheduleHeader.icdoSeminarSchedule.confirmation_sent_flag = busConstant.Flag_Yes;
                        lbusSeminarScheduleHeader.icdoSeminarSchedule.Update();
                        idlgUpdateProcessLog("Confirmation letters generated for seminar schedule id " + Convert.ToString(drSeminarSchedule["SEMINAR_SCHEDULE_ID"]), "INFO", iobjBatchSchedule.step_name);
                    }
                    else
                    {
                        idlgUpdateProcessLog("No Attendees for seminar schedule id " + Convert.ToString(drSeminarSchedule["SEMINAR_SCHEDULE_ID"]), "INFO", iobjBatchSchedule.step_name);
                    }
                }
            }
            else
            {
                idlgUpdateProcessLog("No Seminar Schedules To Generate Confirmation Letter.", "INFO", iobjBatchSchedule.step_name);
            }
        }

        private void CreateCorrespondence(busSeminarSchedule aobjbusSeminarSchedule)
        {
            // Generate Correspondence
            //ArrayList larrlist = new ArrayList();
            //larrlist.Add(aobjbusSeminarSchedule);
            Hashtable lhstDummyTable = new Hashtable();
            lhstDummyTable.Add("sfwCallingForm", "Batch");
            CreateCorrespondence("EVT-3055", aobjbusSeminarSchedule, lhstDummyTable);
        }
    }
}
