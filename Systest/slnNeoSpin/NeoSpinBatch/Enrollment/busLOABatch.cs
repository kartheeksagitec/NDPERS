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
    class busLOABatch : busNeoSpinBatch
    {
        private Collection<busPersonEmploymentDetail> _iclbPersonEmploymentDetail;
        public Collection<busPersonEmploymentDetail> iclbPersonEmploymentDetail
        {
            get { return _iclbPersonEmploymentDetail; }
            set { _iclbPersonEmploymentDetail = value; }
        }

        public void ProcessLOABatch()
        {
            istrProcessName = "LOA Batch";
            idlgUpdateProcessLog("LOA Batch Started ", "INFO", istrProcessName);
            _iclbPersonEmploymentDetail = new Collection<busPersonEmploymentDetail>();
            busBase lobjBase = new busBase();
            /// Load Employment Details of Status LOA and not End-dated.
            DataTable ldtbListOfLOAs = busNeoSpinBase.Select("cdoPersonEmployment.LoadLOAEmployees", new object[0] { });
            Collection<busPersonEmploymentDetail> lclbEmplomentDetail = new Collection<busPersonEmploymentDetail>();
            lclbEmplomentDetail = lobjBase.GetCollection<busPersonEmploymentDetail>(ldtbListOfLOAs, "icdoPersonEmploymentDetail");
            DateTime ldtBatchRunDate = iobjSystemManagement.icdoSystemManagement.batch_date;
            foreach (busPersonEmploymentDetail lobjEmploymentDetail in lclbEmplomentDetail)
            {
                //PROD FIX : Oct-11-2010 Based on Maik Comments, we have added 15 days window instead of taking it from the first day of LOA.
                int lintLOADaysPlus305 = busGlobalFunctions.DateDiffInDays(lobjEmploymentDetail.icdoPersonEmploymentDetail.start_date.AddDays(305), ldtBatchRunDate);

                int lintLOADaysPlus320 = busGlobalFunctions.DateDiffInDays(lobjEmploymentDetail.icdoPersonEmploymentDetail.start_date.AddDays(320), ldtBatchRunDate);

                int lintLOADaysPlus350 = busGlobalFunctions.DateDiffInDays(lobjEmploymentDetail.icdoPersonEmploymentDetail.start_date.AddDays(350), ldtBatchRunDate);

                int lintLOADaysPlus365 = busGlobalFunctions.DateDiffInDays(lobjEmploymentDetail.icdoPersonEmploymentDetail.start_date.AddDays(365), ldtBatchRunDate);

                int lintLOADaysPlus715 = busGlobalFunctions.DateDiffInDays(lobjEmploymentDetail.icdoPersonEmploymentDetail.start_date.AddDays(715), ldtBatchRunDate);

                int lintLOADaysPlus730 = busGlobalFunctions.DateDiffInDays(lobjEmploymentDetail.icdoPersonEmploymentDetail.start_date.AddDays(730), ldtBatchRunDate);

                //Recertified = YES
                if (lobjEmploymentDetail.icdoPersonEmploymentDetail.is_recertified)
                {
                    if ((lintLOADaysPlus715 >= 0) && (lintLOADaysPlus730 < 0))
                    {
                        if (lobjEmploymentDetail.icdoPersonEmploymentDetail.loa_730_letter_sent_flag != busConstant.Flag_Yes)
                        {
                            lobjEmploymentDetail.Is24MonthLOA = "1";
                            GenerateLetter("PER-0101", lobjEmploymentDetail);
                            GenerateLetter("PER-0102", lobjEmploymentDetail);
                            lobjEmploymentDetail.icdoPersonEmploymentDetail.loa_730_letter_sent_flag = busConstant.Flag_Yes;
                            UpdateAndCreateNewEmploymentDetail(lobjEmploymentDetail);
                        }
                    }
                }
                else //Recertified = NO
                {
                    //320 Days Scenario
                    if ((lintLOADaysPlus305 >= 0) && (lintLOADaysPlus320 < 0))
                    {
                        if (lobjEmploymentDetail.icdoPersonEmploymentDetail.loa_320_letter_sent_flag != busConstant.Flag_Yes)
                        {
                            lobjEmploymentDetail.RecertifiedDateNullIDentifier = "1";
                            //Generate PER-0103 Correspondence
                            GenerateLetter("PER-0103", lobjEmploymentDetail);
                            //BR-192 , Generate Multiple Copies for Member and Employer
                            GenerateLetter("PER-0103", lobjEmploymentDetail);
                            lobjEmploymentDetail.icdoPersonEmploymentDetail.loa_320_letter_sent_flag = busConstant.Flag_Yes;
                            lobjEmploymentDetail.icdoPersonEmploymentDetail.Update();

                        }
                    }
                    else if ((lintLOADaysPlus350 >= 0) && (lintLOADaysPlus365 < 0))
                    {
                        if (lobjEmploymentDetail.icdoPersonEmploymentDetail.loa_365_letter_sent_flag != busConstant.Flag_Yes)
                        {
                            lobjEmploymentDetail.RecertificationNullAndEndaDateNotNullIdentifier = "1";
                            //365 Days Scenario
                            GenerateLetter("PER-0102", lobjEmploymentDetail);
                            GenerateLetter("PER-0103", lobjEmploymentDetail);
                            lobjEmploymentDetail.icdoPersonEmploymentDetail.loa_365_letter_sent_flag = busConstant.Flag_Yes;
                            UpdateAndCreateNewEmploymentDetail(lobjEmploymentDetail);
                        }
                    }
                    else if ((lintLOADaysPlus715 >= 0) && (lintLOADaysPlus730 < 0))
                    {
                        if (lobjEmploymentDetail.icdoPersonEmploymentDetail.loa_730_letter_sent_flag != busConstant.Flag_Yes)
                        {
                            lobjEmploymentDetail.Is24MonthLOA = "1";
                            GenerateLetter("PER-0101", lobjEmploymentDetail);
                            GenerateLetter("PER-0102", lobjEmploymentDetail);
                            lobjEmploymentDetail.icdoPersonEmploymentDetail.loa_730_letter_sent_flag = busConstant.Flag_Yes;
                            UpdateAndCreateNewEmploymentDetail(lobjEmploymentDetail);
                        }
                    }
                }
            }
            idlgUpdateProcessLog("LOA Batch Ended", "INFO", istrProcessName);
        }

        private void UpdateAndCreateNewEmploymentDetail(busPersonEmploymentDetail aobjEmpDetail)
        {
            aobjEmpDetail.icdoPersonEmploymentDetail.end_date = iobjSystemManagement.icdoSystemManagement.batch_date.AddDays(-1);
            aobjEmpDetail.icdoPersonEmploymentDetail.Update();
            CreateNewEmploymentDetail(aobjEmpDetail);
        }

        private void GenerateLetter(string astrTemplateName, busPersonEmploymentDetail aobjPersonEmploymentDetail)
        {
            try
            {
                //Load the Person Employment Object
                if (aobjPersonEmploymentDetail.ibusPersonEmployment == null)
                    aobjPersonEmploymentDetail.LoadPersonEmployment();

                //Loading the Object Before Generating
                if (aobjPersonEmploymentDetail.iclbAllPersonAccountEmpDtl == null)
                    aobjPersonEmploymentDetail.LoadAllPersonAccountEmploymentDetails();

                //Loading the Retirment Account LTD Summary and Other Details.
                foreach (busPersonAccountEmploymentDetail lbusPAEDetail in aobjPersonEmploymentDetail.iclbAllPersonAccountEmpDtl)
                {
                    if ((lbusPAEDetail.icdoPersonAccountEmploymentDetail.person_account_id > 0) &&
                        (lbusPAEDetail.icdoPersonAccountEmploymentDetail.election_value == busConstant.PersonAccountElectionValueEnrolled))
                    {
                        if (lbusPAEDetail.ibusPlan == null)
                            lbusPAEDetail.LoadPlan();

                        if (lbusPAEDetail.ibusPlan.IsRetirementPlan())
                        {
                            if (lbusPAEDetail.ibusPersonAccount == null)
                                lbusPAEDetail.LoadPersonAccount();

                            //Loading the Retirment Object
                            aobjPersonEmploymentDetail.ibusRetirement = new busPersonAccountRetirement();
                            aobjPersonEmploymentDetail.ibusRetirement.FindPersonAccountRetirement(
                                lbusPAEDetail.icdoPersonAccountEmploymentDetail.person_account_id);

                            aobjPersonEmploymentDetail.ibusRetirement.LoadTotalVSC();
                            if (aobjPersonEmploymentDetail.ibusRetirement.icdoPersonAccount.Total_VSC >= 6)
                                aobjPersonEmploymentDetail.IsVSC6orMore = true;
                            if (aobjPersonEmploymentDetail.ibusRetirement.icdoPersonAccount.Total_VSC >= 36)
                                aobjPersonEmploymentDetail.IsVSC36orMore = true;
                            aobjPersonEmploymentDetail.ibusRetirement.LoadLTDSummary();
                            break;
                        }
                    }
                }

                //ArrayList larrlist = new ArrayList();
                //larrlist.Add(aobjPersonEmploymentDetail);
                idlgUpdateProcessLog("Creating Letter for PERSLinkID " +
                    Convert.ToString(aobjPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.person_id), "INFO", istrProcessName);
                Hashtable lhstDummyTable = new Hashtable();
                lhstDummyTable.Add("sfwCallingForm", "Batch");
                string lstrFileName = CreateCorrespondence(astrTemplateName, aobjPersonEmploymentDetail, lhstDummyTable);
                CreateContactTicket(aobjPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.person_id);
                idlgUpdateProcessLog("Created", "INFO", istrProcessName);
            }
            catch (Exception _exc)
            {
                idlgUpdateProcessLog("ERROR:" + _exc.Message, "INFO", istrProcessName);
            }
        }

        private void CreateNewEmploymentDetail(busPersonEmploymentDetail AobjOldEmploymmentDetail)
        {
            busPersonEmploymentDetail lobjNewEmploymentDetail = new busPersonEmploymentDetail();
            lobjNewEmploymentDetail.icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail();
            lobjNewEmploymentDetail.icdoPersonEmploymentDetail.person_employment_id = AobjOldEmploymmentDetail.icdoPersonEmploymentDetail.person_employment_id;
            lobjNewEmploymentDetail.icdoPersonEmploymentDetail.hourly_id = AobjOldEmploymmentDetail.icdoPersonEmploymentDetail.hourly_id;
            lobjNewEmploymentDetail.icdoPersonEmploymentDetail.hourly_value = AobjOldEmploymmentDetail.icdoPersonEmploymentDetail.hourly_value;
            lobjNewEmploymentDetail.icdoPersonEmploymentDetail.hourly_value = AobjOldEmploymmentDetail.icdoPersonEmploymentDetail.hourly_value;
            lobjNewEmploymentDetail.icdoPersonEmploymentDetail.seasonal_id = AobjOldEmploymmentDetail.icdoPersonEmploymentDetail.seasonal_id;
            lobjNewEmploymentDetail.icdoPersonEmploymentDetail.seasonal_value = AobjOldEmploymmentDetail.icdoPersonEmploymentDetail.seasonal_value;
            lobjNewEmploymentDetail.icdoPersonEmploymentDetail.type_id = AobjOldEmploymmentDetail.icdoPersonEmploymentDetail.type_id;
            lobjNewEmploymentDetail.icdoPersonEmploymentDetail.type_value = AobjOldEmploymmentDetail.icdoPersonEmploymentDetail.type_value;
            lobjNewEmploymentDetail.icdoPersonEmploymentDetail.job_class_id = AobjOldEmploymmentDetail.icdoPersonEmploymentDetail.job_class_id;
            lobjNewEmploymentDetail.icdoPersonEmploymentDetail.job_class_value = AobjOldEmploymmentDetail.icdoPersonEmploymentDetail.job_class_value;
            lobjNewEmploymentDetail.icdoPersonEmploymentDetail.term_begin_date = AobjOldEmploymmentDetail.icdoPersonEmploymentDetail.term_begin_date;
            lobjNewEmploymentDetail.icdoPersonEmploymentDetail.official_list_id = AobjOldEmploymmentDetail.icdoPersonEmploymentDetail.official_list_id;
            lobjNewEmploymentDetail.icdoPersonEmploymentDetail.official_list_value = AobjOldEmploymmentDetail.icdoPersonEmploymentDetail.official_list_value;
            lobjNewEmploymentDetail.icdoPersonEmploymentDetail.status_id = AobjOldEmploymmentDetail.icdoPersonEmploymentDetail.status_id;
            lobjNewEmploymentDetail.icdoPersonEmploymentDetail.status_value = busConstant.EmploymentStatusNonContributing;
            lobjNewEmploymentDetail.icdoPersonEmploymentDetail.start_date = iobjSystemManagement.icdoSystemManagement.batch_date;
            lobjNewEmploymentDetail.icdoPersonEmploymentDetail.Insert();
        }

        // Create Contact Ticket
        private void CreateContactTicket(int aintPersonID)
        {
            cdoContactTicket lobjContactTicket = new cdoContactTicket();
            CreateContactTicket(aintPersonID, busConstant.ContactTicketTypeRetAccount, busConstant.ResponseMethodCorrespondence, lobjContactTicket);
        }
    }
}
