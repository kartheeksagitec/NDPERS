using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.ObjectModel;
using System.Data;
using NeoSpin.BusinessObjects;
using NeoSpin.CustomDataObjects;
using Sagitec.Common;
using Sagitec.BusinessObjects;
using Sagitec.DBUtility;
using Sagitec.ExceptionPub;
using NeoSpin.DataObjects;
using System.Globalization;
using Sagitec.CustomDataObjects;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busESSHome : busWSSHome
    {
        public busWssMessageHeader ibusWSSMessageHeader { get; set; }

        public busOrganization ibusOrganization { get; set; }

        public busContact ibusContact { get; set; }

        public Collection<busWssMessageDetail> iclbWSSMessageDetails { get; set; }

        public Collection<busEmployerPayrollHeader> iclbEmployerPayrollHeader { get; set; }

        public Collection<busWssMemberRecordRequest> iclbWssMemberRecordRequest { get; set; }

        public Collection<busWssEmploymentChangeRequest> iclbWssEmploymentChangeRequest { get; set; }
        public busEmployerPayrollMonthlyStatement ibusEmployerPayrollMonthlyStatement { get; set; }
        public Collection<busOrgPlan> iclbProviderOrgPlan { get; set; }
        public busDBCacheData ibusDBCacheData { get; set; }
        public string istrOrgCode_OrgName { get; set; }

        public bool iblnExternalLogin = false;
        public string istrProfileEmailID;
        public string istrUpdateAccountURL { get; set; }
        public string istrEmployeeOptions { get; set; }
        //prod pir 4796 : search by message text
        public string istrMessageText { get; set; }
        public string istrProviderOrgNameForPlan { get; set; }
        public void LoadWssMemberRecordRequest(int aintOrgID)
        {
            iclbWssMemberRecordRequest = new Collection<busWssMemberRecordRequest>();
            DataTable ldtWssMemberRecordRequest = Select("cdoWssMemberRecordRequest.LoadMemberRecordRequest", new object[1] { aintOrgID });
            foreach (DataRow dr in ldtWssMemberRecordRequest.Rows)
            {
                busWssMemberRecordRequest lobjWssMemberRecordRequest = new busWssMemberRecordRequest { icdoWssMemberRecordRequest = new cdoWssMemberRecordRequest() };
                lobjWssMemberRecordRequest.ibusWssPersonEmployment = new busWssPersonEmployment { icdoWssPersonEmployment = new cdoWssPersonEmployment() };
                lobjWssMemberRecordRequest.icdoWssMemberRecordRequest.LoadData(dr);
                lobjWssMemberRecordRequest.ibusWssPersonEmployment.icdoWssPersonEmployment.LoadData(dr);
                if (lobjWssMemberRecordRequest.icdoWssMemberRecordRequest.status_value == busConstant.EmploymentChangeRequestStatusRejected)
                {
                    DataTable ldtbUserInfo = null;
                    if (!string.IsNullOrEmpty(Convert.ToString(dr["istrRejectedByName"])))
                    {
                        ldtbUserInfo = iobjPassInfo.isrvDBCache.GetUserInfo(Convert.ToString(dr["istrRejectedByName"]));
                        if (ldtbUserInfo?.Rows?.Count > 0)
                            lobjWssMemberRecordRequest.istrRejectedByName = ldtbUserInfo.Rows[0]["FIRST_NAME"] + " " + ldtbUserInfo.Rows[0]["LAST_NAME"];
                    }
                    lobjWssMemberRecordRequest.idtmRejectedDate = lobjWssMemberRecordRequest.icdoWssMemberRecordRequest.modified_date;
                }
                iclbWssMemberRecordRequest.Add(lobjWssMemberRecordRequest);
            }
        }

        public void LoadWSSEmploymentChangeRequests(int aintOrgID)
        {
            iclbWssEmploymentChangeRequest = new Collection<busWssEmploymentChangeRequest>();
            DataTable ldtWssEmploymentChangeRequest = Select("cdoWssEmploymentChangeRequest.LoadEmploymentChangeRequest", new object[1] { aintOrgID });
            foreach (DataRow dr in ldtWssEmploymentChangeRequest.Rows)
            {
                busWssEmploymentChangeRequest lobDetail = new busWssEmploymentChangeRequest { icdoWssEmploymentChangeRequest = new cdoWssEmploymentChangeRequest() };
                lobDetail.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                lobDetail.icdoWssEmploymentChangeRequest.LoadData(dr);
                //PIR 13214 - Type of Change in the Employee Change Request section on dashboard did not display
                if (string.IsNullOrEmpty(lobDetail.icdoWssEmploymentChangeRequest.change_type_value))
                {
                    lobDetail.icdoWssEmploymentChangeRequest.change_type_value = "TEEM";
                    lobDetail.icdoWssEmploymentChangeRequest.change_type_description = busGlobalFunctions.GetDescriptionByCodeValue(3506, lobDetail.icdoWssEmploymentChangeRequest.change_type_value, iobjPassInfo);
                }
                lobDetail.icdoWssEmploymentChangeRequest.change_type_value = lobDetail.icdoWssEmploymentChangeRequest.change_type_value == "ETCG" ? "CLSC" : lobDetail.icdoWssEmploymentChangeRequest.change_type_value;
                lobDetail.ibusPerson.icdoPerson.LoadData(dr);

                if (lobDetail.icdoWssEmploymentChangeRequest.status_value == busConstant.EmploymentChangeRequestStatusRejected)
                {
                    DataTable ldtbUserInfo = null;
                    if (!string.IsNullOrEmpty(Convert.ToString(dr["istrRejectedByName"])))
                    {
                        ldtbUserInfo = iobjPassInfo.isrvDBCache.GetUserInfo(Convert.ToString(dr["istrRejectedByName"]));
                        if (ldtbUserInfo?.Rows?.Count > 0)
                            lobDetail.istrRejectedByName = ldtbUserInfo.Rows[0]["FIRST_NAME"] + " " + ldtbUserInfo.Rows[0]["LAST_NAME"];
                    }
                    lobDetail.idtmRejectedDate = lobDetail.icdoWssEmploymentChangeRequest.modified_date;
                }
                iclbWssEmploymentChangeRequest.Add(lobDetail);
            }
        }

        public void LoadMessageBoard()
        {
            iclbWSSMessageDetails = new Collection<busWssMessageDetail>();
            if (string.IsNullOrEmpty(istrMessageText))
                istrMessageText = string.Empty;
            DataTable ldtMessageDetail = Select("cdoWssMessageHeader.LoadESSMessageBoard",
                new object[3] { ibusOrganization.icdoOrganization.org_id, ibusContact.icdoContact.contact_id, istrMessageText });
            foreach (DataRow dr in ldtMessageDetail.Rows)
            {
                busWssMessageDetail lobDetail = new busWssMessageDetail { icdoWssMessageDetail = new cdoWssMessageDetail() };
                lobDetail.ibusWSSMessageHeader = new busWssMessageHeader { icdoWssMessageHeader = new cdoWssMessageHeader() };

                lobDetail.icdoWssMessageDetail.LoadData(dr);
                lobDetail.ibusWSSMessageHeader.icdoWssMessageHeader.LoadData(dr);

                if (lobDetail.ibusWSSMessageHeader.icdoWssMessageHeader.message_id > 0)
                {
                    lobDetail.ibusWSSMessageHeader.icdoWssMessageHeader.message_text =
                        busGlobalFunctions.GetMessageTextByMessageID(lobDetail.ibusWSSMessageHeader.icdoWssMessageHeader.message_id, iobjPassInfo);
                }
                iclbWSSMessageDetails.Add(lobDetail);
            }
        }

        public void LoadLastThreeMonthUnReadMessages()
        {
            iclbWSSMessageDetails = new Collection<busWssMessageDetail>();
            if (string.IsNullOrEmpty(istrMessageText))
                istrMessageText = string.Empty;
            DataTable ldtMessageDetail = Select("cdoWssMessageHeader.LoadESSUnReadMessageForLast3Months",
                new object[3] { ibusOrganization.icdoOrganization.org_id, ibusContact.icdoContact.contact_id, istrMessageText });
            foreach (DataRow dr in ldtMessageDetail.Rows)
            {
                busWssMessageDetail lobDetail = new busWssMessageDetail { icdoWssMessageDetail = new cdoWssMessageDetail() };
                lobDetail.ibusWSSMessageHeader = new busWssMessageHeader { icdoWssMessageHeader = new cdoWssMessageHeader() };

                lobDetail.icdoWssMessageDetail.LoadData(dr);
                lobDetail.ibusWSSMessageHeader.icdoWssMessageHeader.LoadData(dr);

                if (lobDetail.ibusWSSMessageHeader.icdoWssMessageHeader.message_id > 0)
                {
                    lobDetail.ibusWSSMessageHeader.icdoWssMessageHeader.message_text =
                        busGlobalFunctions.GetMessageTextByMessageID(lobDetail.ibusWSSMessageHeader.icdoWssMessageHeader.message_id, iobjPassInfo);
                }
                //PIR-19351
                if (lobDetail.ibusWSSMessageHeader.icdoWssMessageHeader.message_text.IsNotNullOrEmpty())
                {
                    if (lobDetail.ibusWSSMessageHeader.icdoWssMessageHeader.message_text.Contains("#"))
                    {
                        string lstrMsgText = lobDetail.ibusWSSMessageHeader.icdoWssMessageHeader.message_text;
                        int lintIndex = lstrMsgText.IndexOf('#');
                        if (lintIndex != -1)
                        {
                            string lstrEndText = lstrMsgText.Substring(lintIndex + 1);
                            string lstrContactTicketId = !string.IsNullOrEmpty(lstrMsgText.Substring(lintIndex + 1, lstrEndText.IndexOf(' ')))
                                                            ? lstrMsgText.Substring(lintIndex + 1, lstrEndText.IndexOf(' ')).Trim()
                                                            : lstrMsgText.Substring(lintIndex + 1, lstrEndText.IndexOf(' '));
                            int lintContactTicketId = 0;
                            if (int.TryParse(lstrContactTicketId, out lintContactTicketId))
                            {
                                lobDetail.ibusWSSMessageHeader.icdoWssMessageHeader.message_text = !string.IsNullOrEmpty(lstrMsgText.Substring(0, lintIndex + 1))
                                                                                                        ? lstrMsgText.Substring(0, lintIndex + 1).Trim()
                                                                                                        : lstrMsgText.Substring(0, lintIndex + 1);
                                lobDetail.ibusWSSMessageHeader.icdoWssMessageHeader.istrContactTicketId = Convert.ToString(lintContactTicketId);
                                if (!string.IsNullOrEmpty(lstrEndText) && lstrEndText.Length > Convert.ToString(lintContactTicketId).Length)
                                    lobDetail.ibusWSSMessageHeader.icdoWssMessageHeader.istrEndText = "&nbsp;" + lstrEndText.Substring(Convert.ToString(lintContactTicketId).Length);
                            }
                        }
                    }
                    else if (lobDetail.ibusWSSMessageHeader.icdoWssMessageHeader.message_text.Contains("@"))
                    {
                        string lstrMsgText = lobDetail.ibusWSSMessageHeader.icdoWssMessageHeader.message_text;
                        int lintIndex = lstrMsgText.IndexOf('@');
                        if (lintIndex != -1)
                        {
                            string lstrEndText = lstrMsgText.Substring(lintIndex + 1);
                            Int32 lintMemberRequestRecordId = 0;
                            if (int.TryParse(lstrEndText, out lintMemberRequestRecordId))
                            {
                                lobDetail.ibusWSSMessageHeader.icdoWssMessageHeader.message_text = !string.IsNullOrEmpty(lstrMsgText.Substring(0, lintIndex + 1))
                                                                                                            ? lstrMsgText.Substring(0, lintIndex).Trim()
                                                                                                            : lstrMsgText.Substring(0, lintIndex);
                                lobDetail.ibusWSSMessageHeader.icdoWssMessageHeader.iintRequestId = lintMemberRequestRecordId;
                                busWssMemberRecordRequest lbusMemberRecordRequest = new busWssMemberRecordRequest() { icdoWssMemberRecordRequest = new cdoWssMemberRecordRequest() };
                                busWssEmploymentChangeRequest lbusEmploymentChange = new busWssEmploymentChangeRequest { icdoWssEmploymentChangeRequest = new cdoWssEmploymentChangeRequest() };
                                if (lbusMemberRecordRequest.FindWssMemberRecordRequest(lintMemberRequestRecordId) && lbusMemberRecordRequest.icdoWssMemberRecordRequest.member_record_request_id == lintMemberRequestRecordId)
                                {
                                    lobDetail.ibusWSSMessageHeader.icdoWssMessageHeader.istrRequestType = busConstant.MemberRecordRequestWizard;
                                }
                                else if (lbusEmploymentChange.FindWssEmploymentChangeRequest(lintMemberRequestRecordId) && lbusEmploymentChange.icdoWssEmploymentChangeRequest.org_id == lobDetail.ibusWSSMessageHeader.icdoWssMessageHeader.org_id)
                                {
                                    lobDetail.ibusWSSMessageHeader.icdoWssMessageHeader.istrRequestType = lbusEmploymentChange.icdoWssEmploymentChangeRequest.change_type_value;
                                    lobDetail.ibusWSSMessageHeader.icdoWssMessageHeader.iintPersonEmploymentId = lbusEmploymentChange.icdoWssEmploymentChangeRequest.person_employment_id;
                                    lobDetail.ibusWSSMessageHeader.icdoWssMessageHeader.iintPersonEmploymentDetailId = lbusEmploymentChange.icdoWssEmploymentChangeRequest.person_employment_detail_id;
                                }
                            }
                        }
                    }
                }
                iclbWSSMessageDetails.Add(lobDetail);
            }
        }

        public void LoadOrganization(int aintOrgID)
        {
            ibusOrganization = new busOrganization();
            ibusOrganization.FindOrganization(aintOrgID);
        }

        public bool LoadContact(int aintContactID)
        {
            ibusContact = new busContact();
            return ibusContact.FindContact(aintContactID);
        }

        public void LoadOrgCodeOrgName()
        {
            istrOrgCode_OrgName = ibusOrganization.icdoOrganization.org_code + " " + ibusOrganization.icdoOrganization.org_name;
        }

        public DataTable GetESSQuestionsForOnlineAccess()
        {
            return iobjPassInfo.isrvDBCache.GetCodeValues(3401);
        }

        public ArrayList btnClearMessage_Click(ArrayList aarrSelectedObjects)
        {
            ArrayList larrlist = new ArrayList();
            if (aarrSelectedObjects != null)
            {
                foreach (busWssMessageDetail lobjDetail in aarrSelectedObjects)
                {
                    iclbWSSMessageDetails.Remove(lobjDetail);
                    //lobjDetail.icdoWssMessageDetail.correspondence_link = "Please contact NDPERS for correspondence";
                    lobjDetail.icdoWssMessageDetail.clear_message_flag = busConstant.Flag_Yes;
                    lobjDetail.icdoWssMessageDetail.Update();
                }
            }

            larrlist.Add(this);
            return larrlist;
        }

        public bool IsProfileEmailMatchContactEmail()
        {
            bool lblnResult = false;
            if (iblnExternalLogin)
            {
                if (ibusContact.icdoContact.email_address.IsNotNull() && istrProfileEmailID.IsNotNull())
                {
                    if (ibusContact.icdoContact.email_address.ToLower() == istrProfileEmailID.ToLower())
                    {
                        lblnResult = true;
                    }
                }
            }
            else
            {
                lblnResult = true;
            }
            return lblnResult;
        }

        public void LoadContactTickets()
        {
            if (iclbContactTicket == null)
                iclbContactTicket = new Collection<busContactTicket>();
            DataTable ldtbContactTicket = busNeoSpinBase.Select("cdoContactTicket.LoadESSContactTickets", new object[1] { ibusOrganization.icdoOrganization.org_id });
            iclbContactTicket = GetCollection<busContactTicket>(ldtbContactTicket, "icdoContactTicket");
        }

        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            if (aobjBus is busContactTicket)
            {
                busContactTicket lbusContactTicket = (busContactTicket)aobjBus;
                lbusContactTicket.ibusAppointmentSchedule = new busAppointmentSchedule { icdoAppointmentSchedule = new cdoAppointmentSchedule() };
                lbusContactTicket.ibusAppointmentSchedule.icdoAppointmentSchedule.LoadData(adtrRow);
                lbusContactTicket.ibusCounselor = new busUser { icdoUser = new cdoUser() };
                if (lbusContactTicket.ibusAppointmentSchedule.icdoAppointmentSchedule.appointment_schedule_id > 0)
                {
                    lbusContactTicket.LoadAppointmentCounselorName();
                }
            }
            base.LoadOtherObjects(adtrRow, aobjBus);
        }

        //BR - 11-112  Load only specfic seminar types.
        public void LoadSeminars()
        {
            if (iclbSeminars == null)
                iclbSeminars = new Collection<busSeminarSchedule>();
            DataTable ldtbSeminars = busNeoSpinBase.Select("cdoSeminarSchedule.LoadSeminarsByOrgForESS", new object[1] { ibusOrganization.icdoOrganization.org_id });
            iclbSeminars = GetCollection<busSeminarSchedule>(ldtbSeminars, "icdoSeminarSchedule");
        }

        public void LoadEmployerPayrollHeader()
        {
            if (iclbEmployerPayrollHeader == null)
                iclbEmployerPayrollHeader = new Collection<busEmployerPayrollHeader>();
            DataTable ldtEmployerPayrollHeader = busNeoSpinBase.Select("cdoEmployerPayrollHeader.ESSLoadEmployerPayroll",
                                                 new object[2] { ibusContact.icdoContact.contact_id, ibusOrganization.icdoOrganization.org_id });
            iclbEmployerPayrollHeader = GetCollection<busEmployerPayrollHeader>(ldtEmployerPayrollHeader, "icdoEmployerPayrollHeader");
            foreach (busEmployerPayrollHeader lbusEmployerPayrollHeader in iclbEmployerPayrollHeader)
            {
                if (lbusEmployerPayrollHeader.ibusOrganization == null)
                    lbusEmployerPayrollHeader.LoadOrganization();

                if (lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.payroll_paid_date != DateTime.MinValue)
                {
                    lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.pay_period = lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.payroll_paid_date.ToString("MM/yyyy", CultureInfo.CreateSpecificCulture("en-US"));
                }
                else
                {
                    lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.pay_period = String.Empty;
                }
            }
        }

        public void LoadLastThreeMonthMessages()
        {
            DataTable ldtMessage = Select("cdoWssMessageHeader.LoadESSMessageForLast3Months",
                                            new object[2] { ibusOrganization.icdoOrganization.org_id, ibusContact.icdoContact.contact_id });
            iclbWSSMessageDetails = new Collection<busWssMessageDetail>();
            foreach (DataRow dr in ldtMessage.Rows)
            {
                busWssMessageDetail lobDetail = new busWssMessageDetail { icdoWssMessageDetail = new cdoWssMessageDetail() };
                lobDetail.ibusWSSMessageHeader = new busWssMessageHeader { icdoWssMessageHeader = new cdoWssMessageHeader() };

                lobDetail.icdoWssMessageDetail.LoadData(dr);
                lobDetail.ibusWSSMessageHeader.icdoWssMessageHeader.LoadData(dr);

                iclbWSSMessageDetails.Add(lobDetail);
            }
        }

        public ArrayList btnSearchbyMessageText()
        {
            ArrayList larrList = new ArrayList();
            LoadMessageBoard();
            larrList.Add(this);
            return larrList;
        }

        public ArrayList btnSearchbyUnReadMessageText()
        {
            ArrayList larrList = new ArrayList();
            LoadLastThreeMonthUnReadMessages();
            larrList.Add(this);
            return larrList;
        }

        public int LoadDefaultOrgPlanIdByPlanId(int aintPlanid, DateTime? adteGivenDate)
        {
            int lintOrgPlanID = 0;
            Collection<busOrgPlan> lclbProviderOrgPlanForCobaRetiree = new Collection<busOrgPlan>();
            DataTable ldtbProviderOrgPlan = Select("cdoPersonAccount.LoadProviderOrgPlanIfEmploymentNotExists", new object[1] { aintPlanid });
            lclbProviderOrgPlanForCobaRetiree = GetCollection<busOrgPlan>(ldtbProviderOrgPlan, "icdoOrgPlan");
            foreach (busOrgPlan lobjOrgPlan in lclbProviderOrgPlanForCobaRetiree)
            {
                if (busGlobalFunctions.CheckDateOverlapping(adteGivenDate ?? DateTime.Now, lobjOrgPlan.icdoOrgPlan.participation_start_date,
                    lobjOrgPlan.icdoOrgPlan.participation_end_date))
                {
                    lintOrgPlanID = lobjOrgPlan.icdoOrgPlan.org_plan_id;
                    break;
                }
            }
            return lintOrgPlanID;
        }

        public void LoadProviderOrgNameForPlanReport(int aintOrgID, int aintPlanID)
        {
            DataTable ldtbAllProviderOrgPlans = busNeoSpinBase.Select("entRateChangeLetterRequest.LoadProviderOrgPlan", new object[] { aintOrgID, busGlobalFunctions.GetSysManagementBatchDate(), aintPlanID });
            iclbProviderOrgPlan = GetCollection<busOrgPlan>(ldtbAllProviderOrgPlans, "icdoOrgPlan");

            foreach (var lbusOrgPlan in iclbProviderOrgPlan)
            {
                DataTable ldtProvider = Select<cdoOrgPlanProvider>(new string[1] { "org_plan_id" },
                                            new object[1] { lbusOrgPlan.icdoOrgPlan.org_plan_id }, null, null);
                if (ldtProvider.Rows.Count > 0)
                {
                    busOrgPlanProvider lobjProvider = new busOrgPlanProvider { icdoOrgPlanProvider = new cdoOrgPlanProvider() };
                    lobjProvider.icdoOrgPlanProvider.LoadData(ldtProvider.Rows[0]);
                    lobjProvider.LoadProviderOrg();
                    istrProviderOrgNameForPlan = lobjProvider.ibusProviderOrg.icdoOrganization.org_name;
                }
            }
        }


        public DataSet GetDataSetToCreateReport(int aintOrgId, string astrReportName)
        {
            DataSet lds = new DataSet("MLC");
            DataTable ldtresult = new DataTable();
            switch (astrReportName)
            {
                case "Leave of Absence":
                    ldtresult = Select("cdoWssPersonEmployment.LeaveOfAbsence",
                               new object[1] { aintOrgId });
                    break;
              
                case "Missing Retirement Contributions":
                    ldtresult = Select("cdoWssPersonEmployment.LoadMissingRetirementContributionsESS",
                               new object[1] { aintOrgId });
                    break;
                case "Missing Retirement Enrollment":
                    ldtresult = Select("cdoWssPersonEmployment.LoadMissingRetirementEnrollment",
                              new object[1] { aintOrgId });
                    break;
                case "Life Insurance Level of Coverage":
                    ldtresult = Select("cdoWssPersonEmployment.LoadLifeInsuranceLevelofCoverage",
                              new object[1] { aintOrgId });
                    DataColumn premiumAmtCol = ldtresult.Columns.Add("PREMIUM_AMOUNT", typeof(decimal));
                    DataColumn EEpremiumAmtCol = ldtresult.Columns.Add("EE_PREMIUM_AMOUNT", typeof(decimal));
                    DataColumn ERpremiumAmtCol = ldtresult.Columns.Add("ER_PREMIUM_AMOUNT", typeof(decimal));
                    DateTime adtEffectiveDate = DateTime.Now;
                    ibusOrganization = new busOrganization();
                    ibusOrganization.FindOrganization(aintOrgId);
                     if (ibusDBCacheData == null)
                        ibusDBCacheData = new busDBCacheData();               
                        ibusDBCacheData.idtbCachedLifeRate = busGlobalFunctions.LoadLifeRateCacheData(iobjPassInfo);
                    int aintOrgPlanId = LoadDefaultOrgPlanIdByPlanId(busConstant.PlanIdGroupLife, adtEffectiveDate);
                    foreach (DataRow row in ldtresult.Rows)
                    {
                        decimal ldecLifePremiumAmount = 0M;
                        decimal ldecADAndDBasicRate = 0.0000M;
                        decimal ldecADAndDSupplementalRate = 0.0000M;
                        decimal ldecEmployerPremiumAmount = 0.0M;
                        busPersonAccountLifeHistory lobjPALifeHistory = new busPersonAccountLifeHistory{icdoPersonAccountLifeHistory=new cdoPersonAccountLifeHistory()};
                        lobjPALifeHistory.icdoPersonAccountLifeHistory.LoadData(row);
                        busPersonAccountLife lbusPersonAccountLife=new busPersonAccountLife();
                        lbusPersonAccountLife.ibusPerson=new busPerson {icdoPerson=new cdoPerson()};
                        lbusPersonAccountLife.ibusPerson.icdoPerson.LoadData(row);
                        lbusPersonAccountLife.icdoPersonAccountLife = new cdoPersonAccountLife();
                        lbusPersonAccountLife.LoadMemberAge(adtEffectiveDate);
                        ldecLifePremiumAmount = busRateHelper.GetLifePremiumAmount(
                            lobjPALifeHistory.icdoPersonAccountLifeHistory.life_insurance_type_value,
                            lobjPALifeHistory.icdoPersonAccountLifeHistory.level_of_coverage_value,
                            lobjPALifeHistory.icdoPersonAccountLifeHistory.coverage_amount,
                            lbusPersonAccountLife.icdoPersonAccountLife.Life_Insurance_Age, aintOrgPlanId,
                            adtEffectiveDate, ref ldecEmployerPremiumAmount, ibusDBCacheData.idtbCachedLifeRate, iobjPassInfo, ref ldecADAndDBasicRate, ref ldecADAndDSupplementalRate);
                        //Basic only has an ER and everything else is EE premium
                        row["EE_PREMIUM_AMOUNT"] = ldecLifePremiumAmount;
                        row["ER_PREMIUM_AMOUNT"] = ldecEmployerPremiumAmount;
                        row["PREMIUM_AMOUNT"] = ldecEmployerPremiumAmount + ldecLifePremiumAmount;
                        row.EndEdit();
                        ldtresult.AcceptChanges();
                    }
                    break;
                //PIR 23729
                case "Annual Enrollment Summary":
                    ldtresult = busBase.Select("cdoWssPersonAccountEnrollmentRequest.rptAnnualEnrollmentSummary", new object[1] { aintOrgId });
                    break;
                //PIR 24994
                case "Retirement Contributions - Audit Confirmation":
                    ibusOrganization = new busOrganization();
                    ibusOrganization.FindOrganization(aintOrgId);
                    ldtresult = busBase.Select("cdoEmployerPayrollDetail.rptRetirementContributionConfirmation", new object[3] { idtmStartDate, idtmEndDate, ibusOrganization.icdoOrganization.org_code });
                    break;
                case "Dental Enrollment Report":
                case "Vision Enrollment Report":
                case "Health Enrollment Report":
                    if (astrReportName == "Dental Enrollment Report")
                    {
                        ldtresult = Select("cdoWssPersonEmployment.LoadDentalEnrollmentPlanRecord",
                                  new object[1] { aintOrgId });
                    }
                    else if (astrReportName == "Vision Enrollment Report")
                    {
                        ldtresult = Select("cdoWssPersonEmployment.LoadVisionEnrollmentPlanRecord",
                              new object[1] { aintOrgId });
                    }
                    else
                    {
                        ldtresult = Select("cdoWssPersonEmployment.LoadHealthEnrollmentPlanRecord",
                              new object[1] { aintOrgId });
                    }

                    DataColumn premiumAmtCollection = ldtresult.Columns.Add("PREMIUM_AMOUNT", typeof(decimal));
                    DataColumn ProviderOrgCollection = ldtresult.Columns.Add("PROVIDER_NAME", typeof(string));
                    //DateTime adtGHDVEffectiveDate = DateTime.Now;
                    DateTime adtGHDVEffectiveDate = busGlobalFunctions.GetSysManagementBatchDate();
                    ibusOrganization = new busOrganization();
                    ibusOrganization.FindOrganization(aintOrgId);
                    //if (ibusDBCacheData == null)
                    ibusDBCacheData = new busDBCacheData();

                    int PlanGHDVID = 0;
                    PlanGHDVID = astrReportName == "Dental Enrollment Report" ? busConstant.PlanIdDental : astrReportName == "Vision Enrollment Report" ? busConstant.PlanIdVision
                                : busConstant.PlanIdGroupHealth;

                    int aintOrgPlanId1 = LoadDefaultOrgPlanIdByPlanId(PlanGHDVID, adtGHDVEffectiveDate);
                    //LoadProviderOrgNameForPlanReport(aintOrgId, PlanGHDVID);
                    busOrganization lobjGHDVPlanProvider = busGlobalFunctions.GetProviderOrgByPlan(PlanGHDVID, busGlobalFunctions.GetSysManagementBatchDate());
                    istrProviderOrgNameForPlan = lobjGHDVPlanProvider.icdoOrganization.org_name;

                    decimal ldecPremiumAmount = 0;
                    foreach (DataRow row in ldtresult.Rows)
                    {

                        busPersonAccountGhdvHistory lobjPADentalEnrollHistory = new busPersonAccountGhdvHistory { icdoPersonAccountGhdvHistory = new cdoPersonAccountGhdvHistory() };
                        lobjPADentalEnrollHistory.icdoPersonAccountGhdvHistory.LoadData(row);
                        if (astrReportName == "Dental Enrollment Report")
                        {
                            ibusDBCacheData.idtbCachedDentalRate = busGlobalFunctions.LoadDentalRateCacheData(iobjPassInfo);
                            ldecPremiumAmount = busRateHelper.GetDentalPremiumAmount(aintOrgPlanId1,
                           lobjPADentalEnrollHistory.icdoPersonAccountGhdvHistory.dental_insurance_type_value,
                           lobjPADentalEnrollHistory.icdoPersonAccountGhdvHistory.level_of_coverage_value,
                           adtGHDVEffectiveDate, ibusDBCacheData.idtbCachedDentalRate, iobjPassInfo);
                        }
                        else if (astrReportName == "Vision Enrollment Report")
                        {
                            ibusDBCacheData.idtbCachedDentalRate = busGlobalFunctions.LoadVisionRateCacheData(iobjPassInfo);
                            ldecPremiumAmount = busRateHelper.GetVisionPremiumAmount(aintOrgPlanId1,
                           lobjPADentalEnrollHistory.icdoPersonAccountGhdvHistory.vision_insurance_type_value,
                           lobjPADentalEnrollHistory.icdoPersonAccountGhdvHistory.level_of_coverage_value,
                           adtGHDVEffectiveDate, ibusDBCacheData.idtbCachedDentalRate, iobjPassInfo);
                        }
                        else
                        {

                            busPersonAccountGhdv ibusESSPersonAccountHealthGhdv = new busPersonAccountGhdv { icdoPersonAccount = new cdoPersonAccount(), icdoPersonAccountGhdv = new cdoPersonAccountGhdv() };
                            // busPersonAccount lbusPersonAccount = new busPersonAccount();
                            ibusESSPersonAccountHealthGhdv.icdoPersonAccount.LoadData(row);
                            if (ibusESSPersonAccountHealthGhdv.FindGHDVByPersonAccountID(ibusESSPersonAccountHealthGhdv.icdoPersonAccount.person_account_id))
                            {
                                ibusESSPersonAccountHealthGhdv.icdoPersonAccount = ibusESSPersonAccountHealthGhdv.icdoPersonAccount;
                                ibusESSPersonAccountHealthGhdv.LoadPlan();
                                //ibusESSPersonAccountHealthGhdv.ESSLoadProviderOrgName(lbusPersonAccount.icdoPersonAccount);
                                ibusESSPersonAccountHealthGhdv.LoadPlanEffectiveDate();
                                ibusESSPersonAccountHealthGhdv.DetermineEnrollmentAndLoadObjects(ibusESSPersonAccountHealthGhdv.idtPlanEffectiveDate, false);
                                ibusESSPersonAccountHealthGhdv.LoadPersonAccountGHDVHsaFutureDated();
                                if (ibusESSPersonAccountHealthGhdv.icdoPersonAccountGhdv.overridden_structure_code.IsNotNullOrEmpty())
                                {
                                    ibusESSPersonAccountHealthGhdv.LoadRateStructureForUserStructureCode();
                                }
                                else
                                {
                                    //Load the Health Plan Participation Date (based on effective Date)
                                    ibusESSPersonAccountHealthGhdv.LoadHealthParticipationDate();
                                    //To Get the Rate Structure Code (Derived Field)
                                    ibusESSPersonAccountHealthGhdv.LoadRateStructure();
                                }
                                //Get the Coverage Ref ID
                                ibusESSPersonAccountHealthGhdv.LoadCoverageRefID();
                                ibusESSPersonAccountHealthGhdv.GetMonthlyPremiumAmountByRefID();
                                //iblnHealth = true;
                                //ibusESSPersonAccountHealthGhdv.LoadCoverageCodeByFilter();
                                //ibusESSPersonAccountHealthGhdv.ESSLoadCoverageCodeDescription();
                                ldecPremiumAmount = ibusESSPersonAccountHealthGhdv.icdoPersonAccountGhdv.MonthlyPremiumAmount;
                            }
                        }

                        row["PREMIUM_AMOUNT"] = ldecPremiumAmount;
                        row["PROVIDER_NAME"] = istrProviderOrgNameForPlan;
                        row.EndEdit();
                        ldtresult.AcceptChanges();


                    }
                    break;
                case "EAP Enrollment Report":
                    if (astrReportName == "EAP Enrollment Report")
                    {
                        ldtresult = Select("cdoWssPersonEmployment.LoadEAPEnrollmentPlanRecord",
                                  new object[1] { aintOrgId });
                    }
                    DataColumn premiumAmtEAPCollection = ldtresult.Columns.Add("PREMIUM_AMOUNT", typeof(decimal));
                    DataColumn ProviderOrgEAPCollection = ldtresult.Columns.Add("PROVIDER_NAME", typeof(string));
                    LoadProviderOrgNameForPlanReport(aintOrgId, busConstant.PlanIdEAP);
                    //busOrganization lobjEAPPlanProvider = busGlobalFunctions.GetProviderOrgByPlan(busConstant.PlanIdEAP, busGlobalFunctions.GetSysManagementBatchDate());
                    //istrProviderOrgNameForPlan = lobjEAPPlanProvider.icdoOrganization.org_name;
                    decimal ldecEAPPremiumAmount = 0;
                    foreach (DataRow row in ldtresult.Rows)
                    {
                        busPersonAccountEAP ibusESSPersonAccountEAP = new busPersonAccountEAP { icdoPersonAccount = new cdoPersonAccount() };
                        ibusESSPersonAccountEAP.icdoPersonAccount.LoadData(row);
                        if (ibusESSPersonAccountEAP.FindPersonAccount(ibusESSPersonAccountEAP.icdoPersonAccount.person_account_id))
                        {
                            ibusESSPersonAccountEAP.LoadPlanEffectiveDate();
                            ibusESSPersonAccountEAP.LoadOrgPlan(ibusESSPersonAccountEAP.idtPlanEffectiveDate);
                            ibusESSPersonAccountEAP.LoadProviderOrgPlanByProviderOrgID(ibusESSPersonAccountEAP.icdoPersonAccount.provider_org_id, ibusESSPersonAccountEAP.idtPlanEffectiveDate);
                            ibusESSPersonAccountEAP.GetMonthlyPremium();
                            ibusESSPersonAccountEAP.LoadPlan();
                            ibusESSPersonAccountEAP.LoadProvider();
                            ldecEAPPremiumAmount = ibusESSPersonAccountEAP.idecMonthlyPremium;
                        }

                        row["PREMIUM_AMOUNT"] = ldecEAPPremiumAmount;
                        row["PROVIDER_NAME"] = istrProviderOrgNameForPlan;
                        row.EndEdit();
                        ldtresult.AcceptChanges();
                    }
                    break;
                case "Deferred Compensation Enrollment Report":
                    ldtresult = Select("cdoWssPersonEmployment.LoadDeffComEnrollmentPlanRecord",
                              new object[1] { aintOrgId });
                    break;
                case "Flex Enrollment Report":
                    ldtresult = Select("cdoWssPersonEmployment.LoadFlexCompEnrollmentPlanRecord",
                              new object[1] { aintOrgId });
                    DataColumn ProviderOrgFLEXCollection = ldtresult.Columns.Add("PROVIDER_NAME", typeof(string));
                    //LoadProviderOrgNameForPlanReport(aintOrgId, busConstant.PlanIdFlex);
                    busOrganization lobjFlexPlanProvider = busGlobalFunctions.GetProviderOrgByPlan(busConstant.PlanIdFlex, busGlobalFunctions.GetSysManagementBatchDate());
                    istrProviderOrgNameForPlan = lobjFlexPlanProvider.icdoOrganization.org_name;

                    foreach (DataRow row in ldtresult.Rows)
                    {
                        row["PROVIDER_NAME"] = istrProviderOrgNameForPlan;
                        row.EndEdit();
                        ldtresult.AcceptChanges();
                    }
                    break;
                case "Retirement Enrollment Report":
                    ldtresult = Select("cdoWssPersonEmployment.LoadRetirementEnrollmentPlanRecord",
                              new object[1] { aintOrgId });
                    break;
                case "DC 2025 Retirement Enrollment Report":
                    ldtresult = Select("cdoWssPersonEmployment.LoadDC25RetrEnrollmentPlanRecord",
                              new object[1] { aintOrgId });
                    break;
            }
            //DataTable dtCopy = ldtresult.Copy();
            //lds.Tables.Add(dtCopy);
            //lds.AcceptChanges();
            //lds.Tables.Add(ldtresult);
            ldtresult.TableName = busConstant.ReportTableName;
            return ldtresult.DataSet;
        }

     
        public void LoadESSReports()
        {
            iclbWSSMessageDetails = new Collection<busWssMessageDetail>();

            busWssMessageDetail lbusWssMessageDetailLeaveofAbsence = new busWssMessageDetail { icdoWssMessageDetail = new cdoWssMessageDetail() };
            lbusWssMessageDetailLeaveofAbsence.icdoWssMessageDetail.org_id = this.ibusOrganization.icdoOrganization.org_id;
            lbusWssMessageDetailLeaveofAbsence.icdoWssMessageDetail.istrReportNameText = "Leave of Absence";
            lbusWssMessageDetailLeaveofAbsence.icdoWssMessageDetail.istrReportNameHidden = "Leave of Absence";
            iclbWSSMessageDetails.Add(lbusWssMessageDetailLeaveofAbsence);

            busWssMessageDetail lbusWssMessageDetailMissingContributionsReport = new busWssMessageDetail { icdoWssMessageDetail = new cdoWssMessageDetail() };
            lbusWssMessageDetailMissingContributionsReport.icdoWssMessageDetail.org_id = this.ibusOrganization.icdoOrganization.org_id;
            lbusWssMessageDetailMissingContributionsReport.icdoWssMessageDetail.istrReportNameText = "Missing Retirement Contributions";
            lbusWssMessageDetailMissingContributionsReport.icdoWssMessageDetail.istrReportNameHidden = "Missing Retirement Contributions";
            iclbWSSMessageDetails.Add(lbusWssMessageDetailMissingContributionsReport);

            busWssMessageDetail lbusWssMessageDetailMissingRetirementEnrollmentReport = new busWssMessageDetail { icdoWssMessageDetail = new cdoWssMessageDetail() };
            lbusWssMessageDetailMissingRetirementEnrollmentReport.icdoWssMessageDetail.org_id = this.ibusOrganization.icdoOrganization.org_id;
            lbusWssMessageDetailMissingRetirementEnrollmentReport.icdoWssMessageDetail.istrReportNameText = "Missing Retirement Enrollment";
            lbusWssMessageDetailMissingRetirementEnrollmentReport.icdoWssMessageDetail.istrReportNameHidden = "Missing Retirement Enrollment";
            iclbWSSMessageDetails.Add(lbusWssMessageDetailMissingRetirementEnrollmentReport);

            busWssMessageDetail lbusWssMessageDetailBenefitEnrollmentReport= new busWssMessageDetail { icdoWssMessageDetail = new cdoWssMessageDetail() };
            lbusWssMessageDetailBenefitEnrollmentReport.icdoWssMessageDetail.org_id = this.ibusOrganization.icdoOrganization.org_id;
            lbusWssMessageDetailBenefitEnrollmentReport.icdoWssMessageDetail.istrReportName = "Benefit Enrollment Report";
            lbusWssMessageDetailBenefitEnrollmentReport.icdoWssMessageDetail.istrReportNameHidden = "Benefit Enrollment Report";
            iclbWSSMessageDetails.Add(lbusWssMessageDetailBenefitEnrollmentReport);

            //PIR 20463
            //busWssMessageDetail lbusWssMessageDetailTransactionHistory = new busWssMessageDetail { icdoWssMessageDetail = new cdoWssMessageDetail() };
            //lbusWssMessageDetailTransactionHistory.icdoWssMessageDetail.org_id = this.ibusOrganization.icdoOrganization.org_id;
            //lbusWssMessageDetailTransactionHistory.icdoWssMessageDetail.istrReportName = "Transaction History";
            //iclbWSSMessageDetails.Add(lbusWssMessageDetailTransactionHistory);

            busWssMessageDetail lbusWssMessageDetailPayrollReporting  = new busWssMessageDetail { icdoWssMessageDetail = new cdoWssMessageDetail() };
            lbusWssMessageDetailPayrollReporting.icdoWssMessageDetail.org_id = this.ibusOrganization.icdoOrganization.org_id;
            lbusWssMessageDetailPayrollReporting.icdoWssMessageDetail.istrReportName = "Payroll Reporting";
            lbusWssMessageDetailPayrollReporting.icdoWssMessageDetail.istrReportNameHidden = "Payroll Reporting";
            iclbWSSMessageDetails.Add(lbusWssMessageDetailPayrollReporting);

            //PIR 23729
            DateTime ldtStartDate = Convert.ToDateTime(busGlobalFunctions.GetData1ByCodeValue(52, "ANNE", iobjPassInfo));
            DateTime ldtEndDate = Convert.ToDateTime(busGlobalFunctions.GetData3ByCodeValue(52, "ANNE", iobjPassInfo));
            string lstrANNEMonths = busGlobalFunctions.GetData1ByCodeValue(7024, "ANNM", iobjPassInfo);
            ldtEndDate = ldtEndDate.AddMonths(Convert.ToInt32(lstrANNEMonths));
            if (busGlobalFunctions.CheckDateOverlapping(DateTime.Now, ldtStartDate, ldtEndDate))
            {
                busWssMessageDetail lbusWssAnnualEnrollmentSummaryReport = new busWssMessageDetail { icdoWssMessageDetail = new cdoWssMessageDetail() };
                lbusWssAnnualEnrollmentSummaryReport.icdoWssMessageDetail.org_id = this.ibusOrganization.icdoOrganization.org_id;
                lbusWssAnnualEnrollmentSummaryReport.icdoWssMessageDetail.istrReportNameText = "Annual Enrollment Summary";
                lbusWssAnnualEnrollmentSummaryReport.icdoWssMessageDetail.istrReportNameHidden = "Annual Enrollment Summary";
                iclbWSSMessageDetails.Add(lbusWssAnnualEnrollmentSummaryReport);
            }

            if (ibusOrganization.iclbOrgPlan.IsNull())
                ibusOrganization.LoadOrgPlan();
            if (ibusOrganization.iclbOrgPlan.Where(i => i.icdoOrgPlan.plan_id == busConstant.PlanIdGroupLife
                && (i.icdoOrgPlan.participation_end_date == DateTime.MinValue || i.icdoOrgPlan.participation_end_date > DateTime.Now)).Count() > 0)
            {
                busWssMessageDetail lbusWssMessageDetailLifeInsuranceLevelofCoverageReport = new busWssMessageDetail { icdoWssMessageDetail = new cdoWssMessageDetail() };
                lbusWssMessageDetailLifeInsuranceLevelofCoverageReport.icdoWssMessageDetail.org_id = this.ibusOrganization.icdoOrganization.org_id;
                lbusWssMessageDetailLifeInsuranceLevelofCoverageReport.icdoWssMessageDetail.istrReportNameText = "Life Insurance Level of Coverage";
                lbusWssMessageDetailLifeInsuranceLevelofCoverageReport.icdoWssMessageDetail.istrReportNameHidden = "Life Insurance Level of Coverage";
                iclbWSSMessageDetails.Add(lbusWssMessageDetailLifeInsuranceLevelofCoverageReport);
            }
            if (ibusOrganization.iclbOrgPlan.Where(i => i.icdoOrgPlan.plan_id == busConstant.PlanIdDental
                && (i.icdoOrgPlan.participation_end_date == DateTime.MinValue || i.icdoOrgPlan.participation_end_date > DateTime.Now)).Count() > 0)
            {
                busWssMessageDetail lbusWssMessageDetailDentalEnrollmentReport = new busWssMessageDetail { icdoWssMessageDetail = new cdoWssMessageDetail() };
                lbusWssMessageDetailDentalEnrollmentReport.icdoWssMessageDetail.org_id = this.ibusOrganization.icdoOrganization.org_id;
                lbusWssMessageDetailDentalEnrollmentReport.icdoWssMessageDetail.istrReportNameText = "Dental Enrollment Report";
                lbusWssMessageDetailDentalEnrollmentReport.icdoWssMessageDetail.istrReportNameHidden = "Dental Enrollment Report";
                iclbWSSMessageDetails.Add(lbusWssMessageDetailDentalEnrollmentReport);
            }
            if (ibusOrganization.iclbOrgPlan.Where(i => (i.icdoOrgPlan.plan_id == busConstant.PlanIdDeferredCompensation || i.icdoOrgPlan.plan_id == busConstant.PlanIdOther457)
                && (i.icdoOrgPlan.participation_end_date == DateTime.MinValue || i.icdoOrgPlan.participation_end_date > DateTime.Now)).Count() > 0)
            {
                busWssMessageDetail lbusWssMessageDetailDeffComEnrollmentReport = new busWssMessageDetail { icdoWssMessageDetail = new cdoWssMessageDetail() };
                lbusWssMessageDetailDeffComEnrollmentReport.icdoWssMessageDetail.org_id = this.ibusOrganization.icdoOrganization.org_id;
                lbusWssMessageDetailDeffComEnrollmentReport.icdoWssMessageDetail.istrReportNameText = "Deferred Compensation Enrollment Report";
                lbusWssMessageDetailDeffComEnrollmentReport.icdoWssMessageDetail.istrReportNameHidden = "Deferred Compensation Enrollment Report";
                iclbWSSMessageDetails.Add(lbusWssMessageDetailDeffComEnrollmentReport);
            }
            if (ibusOrganization.iclbOrgPlan.Where(i => i.icdoOrgPlan.plan_id == busConstant.PlanIdFlex
                && (i.icdoOrgPlan.participation_end_date == DateTime.MinValue || i.icdoOrgPlan.participation_end_date > DateTime.Now)).Count() > 0)
            {
                busWssMessageDetail lbusWssMessageDetailFlexEnrollmentReport = new busWssMessageDetail { icdoWssMessageDetail = new cdoWssMessageDetail() };
                lbusWssMessageDetailFlexEnrollmentReport.icdoWssMessageDetail.org_id = this.ibusOrganization.icdoOrganization.org_id;
                lbusWssMessageDetailFlexEnrollmentReport.icdoWssMessageDetail.istrReportNameText = "Flex Enrollment Report";
                lbusWssMessageDetailFlexEnrollmentReport.icdoWssMessageDetail.istrReportNameHidden = "Flex Enrollment Report";
                iclbWSSMessageDetails.Add(lbusWssMessageDetailFlexEnrollmentReport);
            }
            if (ibusOrganization.iclbOrgPlan.Where(i => i.icdoOrgPlan.plan_id == busConstant.PlanIdVision
                && (i.icdoOrgPlan.participation_end_date == DateTime.MinValue || i.icdoOrgPlan.participation_end_date > DateTime.Now)).Count() > 0)
            {
                busWssMessageDetail lbusWssMessageDetailVisionEnrollmentReport = new busWssMessageDetail { icdoWssMessageDetail = new cdoWssMessageDetail() };
                lbusWssMessageDetailVisionEnrollmentReport.icdoWssMessageDetail.org_id = this.ibusOrganization.icdoOrganization.org_id;
                lbusWssMessageDetailVisionEnrollmentReport.icdoWssMessageDetail.istrReportNameText = "Vision Enrollment Report";
                lbusWssMessageDetailVisionEnrollmentReport.icdoWssMessageDetail.istrReportNameHidden = "Vision Enrollment Report";
                iclbWSSMessageDetails.Add(lbusWssMessageDetailVisionEnrollmentReport);
            }
            if (ibusOrganization.iclbOrgPlan.Where(i => i.icdoOrgPlan.plan_id == busConstant.PlanIdGroupHealth
                && (i.icdoOrgPlan.participation_end_date == DateTime.MinValue || i.icdoOrgPlan.participation_end_date > DateTime.Now)).Count() > 0)
            {
                busWssMessageDetail lbusWssMessageDetailHealthEnrollmentReport = new busWssMessageDetail { icdoWssMessageDetail = new cdoWssMessageDetail() };
                lbusWssMessageDetailHealthEnrollmentReport.icdoWssMessageDetail.org_id = this.ibusOrganization.icdoOrganization.org_id;
                lbusWssMessageDetailHealthEnrollmentReport.icdoWssMessageDetail.istrReportNameText = "Health Enrollment Report";
                lbusWssMessageDetailHealthEnrollmentReport.icdoWssMessageDetail.istrReportNameHidden = "Health Enrollment Report";
                iclbWSSMessageDetails.Add(lbusWssMessageDetailHealthEnrollmentReport);
            }
            //if (ibusOrganization.iclbOrgPlan.Where(i => i.icdoOrgPlan.plan_id == busConstant.PlanIdMain || i.icdoOrgPlan.plan_id == busConstant.PlanIdLE
            //|| i.icdoOrgPlan.plan_id == busConstant.PlanIdNG || i.icdoOrgPlan.plan_id == busConstant.PlanIdHP || i.icdoOrgPlan.plan_id == busConstant.PlanIdJudges
            //|| i.icdoOrgPlan.plan_id == busConstant.PlanIdJobService || i.icdoOrgPlan.plan_id == busConstant.PlanIdDC || i.icdoOrgPlan.plan_id == busConstant.PlanIdLEWithoutPS
            //|| i.icdoOrgPlan.plan_id == busConstant.PlanIdBCILawEnf || i.icdoOrgPlan.plan_id == busConstant.PlanIdMain2020 || i.icdoOrgPlan.plan_id == busConstant.PlanIdDC2020).Count() > 0)
            if (ibusOrganization.iclbOrgPlan.Where(i => i.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeRetirement &&
                (i.icdoOrgPlan.participation_end_date == DateTime.MinValue || i.icdoOrgPlan.participation_end_date > DateTime.Now)).Any() )
            {
                busWssMessageDetail lbusWssMessageDetailRetirementEnrollmentReport = new busWssMessageDetail { icdoWssMessageDetail = new cdoWssMessageDetail() };
                lbusWssMessageDetailRetirementEnrollmentReport.icdoWssMessageDetail.org_id = this.ibusOrganization.icdoOrganization.org_id;
                lbusWssMessageDetailRetirementEnrollmentReport.icdoWssMessageDetail.istrReportNameText = "Retirement Enrollment Report";
                lbusWssMessageDetailRetirementEnrollmentReport.icdoWssMessageDetail.istrReportNameHidden = "Retirement Enrollment Report";
                iclbWSSMessageDetails.Add(lbusWssMessageDetailRetirementEnrollmentReport);
            }
            if (ibusOrganization.iclbOrgPlan.Where(i => i.icdoOrgPlan.plan_id == busConstant.PlanIdEAP &&
                (i.icdoOrgPlan.participation_end_date == DateTime.MinValue || i.icdoOrgPlan.participation_end_date > DateTime.Now)).Count() > 0)
            {
                busWssMessageDetail lbusWssMessageDetailEAPEnrollmentReport = new busWssMessageDetail { icdoWssMessageDetail = new cdoWssMessageDetail() };
                lbusWssMessageDetailEAPEnrollmentReport.icdoWssMessageDetail.org_id = this.ibusOrganization.icdoOrganization.org_id;
                lbusWssMessageDetailEAPEnrollmentReport.icdoWssMessageDetail.istrReportNameText = "EAP Enrollment Report";
                lbusWssMessageDetailEAPEnrollmentReport.icdoWssMessageDetail.istrReportNameHidden = "EAP Enrollment Report";
                iclbWSSMessageDetails.Add(lbusWssMessageDetailEAPEnrollmentReport);
            }
            if (ibusOrganization.iclbOrgPlan.Where(i => i.icdoOrgPlan.plan_id == busConstant.PlanIdDC2025 &&
                (i.icdoOrgPlan.participation_end_date == DateTime.MinValue || i.icdoOrgPlan.participation_end_date > DateTime.Now)).Count() > 0)//PIR 25920
            {
                busWssMessageDetail lbusWssMessageDetailDC25RetrEnrollmentReport = new busWssMessageDetail { icdoWssMessageDetail = new cdoWssMessageDetail() };
                lbusWssMessageDetailDC25RetrEnrollmentReport.icdoWssMessageDetail.org_id = this.ibusOrganization.icdoOrganization.org_id;
                lbusWssMessageDetailDC25RetrEnrollmentReport.icdoWssMessageDetail.istrReportNameText = "DC 2025 Retirement Enrollment Report";
                lbusWssMessageDetailDC25RetrEnrollmentReport.icdoWssMessageDetail.istrReportNameHidden = "DC 2025 Retirement Enrollment Report";
                iclbWSSMessageDetails.Add(lbusWssMessageDetailDC25RetrEnrollmentReport);
            }
            //PIR 24994
            busWssMessageDetail lbusRetirementContributionAuditoConfirmationReport = new busWssMessageDetail { icdoWssMessageDetail = new cdoWssMessageDetail() };
            lbusRetirementContributionAuditoConfirmationReport.icdoWssMessageDetail.org_id = this.ibusOrganization.icdoOrganization.org_id;
            lbusRetirementContributionAuditoConfirmationReport.icdoWssMessageDetail.istrReportName = "Retirement Contributions - Audit Confirmation";
            lbusRetirementContributionAuditoConfirmationReport.icdoWssMessageDetail.istrReportNameHidden = "Retirement Contributions - Audit Confirmation";
            iclbWSSMessageDetails.Add(lbusRetirementContributionAuditoConfirmationReport);
        }

        //FW Upgrade : Code conversion (View ESS Report)
        public int iintOrgId { get; set; }
        public int iintContactId { get; set; }
        public string istrReportName { get; set; }
        public string istrDownloadFileName { get; set; }
        public DateTime idtmStartDate { get; set; }
        public DateTime idtmEndDate { get; set; }
        public ArrayList ViewEssReport_Click(int aintOrgId, string astrReportName)
        {
            ArrayList larlstResult = new ArrayList();
            try
            {
                busNeoSpinBase lobjNeospinBase = new busNeoSpinBase();
                //busESSHome lobj1099R = new busESSHome();
                DataSet ldt = new DataSet();
                Hashtable lhstParam = new Hashtable();
                ldt = this.GetDataSetToCreateReport(aintOrgId, astrReportName);
                busOrganization lobjOrganization = new busOrganization();
                lobjOrganization.FindOrganization(aintOrgId);
                if (astrReportName == "Missing Retirement Contributions")
                {
                    lhstParam.Add("txtReportRunDate", DateTime.Now.ToString(NeoSpin.BusinessObjects.busConstant.ReportDateFormat));
                }
                else if(astrReportName == "Retirement Contributions - Audit Confirmation") //PIR 24994
                {
                    lhstParam.Add("START_DATE", idtmStartDate.ToString(NeoSpin.BusinessObjects.busConstant.ReportDateFormat));
                    lhstParam.Add("END_DATE", idtmEndDate.ToString(NeoSpin.BusinessObjects.busConstant.ReportDateFormat));
                    lhstParam.Add("REPORT_RUN_DATE", DateTime.Now.ToString(NeoSpin.BusinessObjects.busConstant.ReportDateFormat));
                }
                else
                {
                    if (astrReportName == "Annual Enrollment Summary") //PIR 23729
                    {
                        DateTime ldtStartDate = Convert.ToDateTime(busGlobalFunctions.GetData1ByCodeValue(52, "ANNE", iobjPassInfo));
                        DateTime ldtEndDate = Convert.ToDateTime(busGlobalFunctions.GetData3ByCodeValue(52, "ANNE", iobjPassInfo));
                        string lstrANNEMonths = busGlobalFunctions.GetData1ByCodeValue(7024, "ANNM", iobjPassInfo);
                        ldtEndDate = ldtEndDate.AddMonths(Convert.ToInt32(lstrANNEMonths));
                        lhstParam.Add("FromDate", ldtStartDate);
                        lhstParam.Add("ToDate", ldtEndDate);
                        lhstParam.Add("OrgCodeAndName", lobjOrganization.icdoOrganization.org_code + " " + lobjOrganization.icdoOrganization.org_name);
                    }
                    else
                    {
                        lhstParam.Add("OrgName", lobjOrganization.icdoOrganization.org_name);
                        lhstParam.Add("OrgCode", lobjOrganization.icdoOrganization.org_code);
                    }
                }

                if(astrReportName == "Retirement Contributions - Audit Confirmation")
                {
                    larlstResult.Add("rptRetirementContributionsAuditConfirmation.rpt" + ".pdf");
                    larlstResult.Add(lobjNeospinBase.CreateDynamicReport("rptRetirementContributionsAuditConfirmation.rpt", ldt, string.Empty, hstParam: lhstParam));
                    larlstResult.Add("application/pdf");
                }
                else if (iobjPassInfo.istrPostBackControlID == "btnESSReportsDownload")
                {
                    switch (astrReportName)
                    {
                        case "Leave of Absence":
                            larlstResult.Add("rptLeaveOfAbsence.rpt" + ".pdf");
                            larlstResult.Add(lobjNeospinBase.CreateDynamicReport("rptLeaveOfAbsence.rpt", ldt, string.Empty, hstParam: lhstParam));
                            break;
                        case "Missing Retirement Contributions":
                            larlstResult.Add("rptMissingContributions.rpt" + ".pdf");
                            larlstResult.Add(lobjNeospinBase.CreateDynamicReport("rptMissingContributions.rpt", ldt, string.Empty, hstParam: lhstParam));
                            break;
                        case "Missing Retirement Enrollment":
                            larlstResult.Add("rptMissingRetirementEnrollment.rpt" + ".pdf");
                            larlstResult.Add(lobjNeospinBase.CreateDynamicReport("rptMissingRetirementEnrollment.rpt", ldt, string.Empty, hstParam: lhstParam));
                            break;
                        case "Life Insurance Level of Coverage":
                            larlstResult.Add("rptLifeLevelofCoverage.rpt" + ".pdf");
                            larlstResult.Add(lobjNeospinBase.CreateDynamicReport("rptLifeLevelofCoverage.rpt", ldt, string.Empty, hstParam: lhstParam));
                            break;
                        case "Dental Enrollment Report":
                            larlstResult.Add("rptDentalEnrollment.rpt" + ".pdf");
                            larlstResult.Add(lobjNeospinBase.CreateDynamicReport("rptDentalEnrollment.rpt", ldt, string.Empty, hstParam: lhstParam));
                            break;
                        case "Deferred Compensation Enrollment Report":
                            larlstResult.Add("rptDeferredCompensationEnrollment.rpt" + ".pdf");
                            larlstResult.Add(lobjNeospinBase.CreateDynamicReport("rptDeferredCompensationEnrollment.rpt", ldt, string.Empty, hstParam: lhstParam));
                            break;
                        case "Flex Enrollment Report":
                            larlstResult.Add("rptFlexEnrollment.rpt" + ".pdf");
                            larlstResult.Add(lobjNeospinBase.CreateDynamicReport("rptFlexEnrollment.rpt", ldt, string.Empty, hstParam: lhstParam));
                            break;
                        case "Vision Enrollment Report":
                            larlstResult.Add("rptVisionEnrollment.rpt" + ".pdf");
                            larlstResult.Add(lobjNeospinBase.CreateDynamicReport("rptVisionEnrollment.rpt", ldt, string.Empty, hstParam: lhstParam));
                            break;
                        case "Health Enrollment Report":
                            larlstResult.Add("rptHealthEnrollment.rpt" + ".pdf");
                            larlstResult.Add(lobjNeospinBase.CreateDynamicReport("rptHealthEnrollment.rpt", ldt, string.Empty, hstParam: lhstParam));
                            break;
                        case "Retirement Enrollment Report":
                            larlstResult.Add("rptRetirementEnrollment.rpt" + ".pdf");
                            larlstResult.Add(lobjNeospinBase.CreateDynamicReport("rptRetirementEnrollment.rpt", ldt, string.Empty, hstParam: lhstParam));
                            break;
                        case "EAP Enrollment Report":
                            larlstResult.Add("rptEAPEnrollment.rpt" + ".pdf");
                            larlstResult.Add(lobjNeospinBase.CreateDynamicReport("rptEAPEnrollment.rpt", ldt, string.Empty, hstParam: lhstParam));
                            break;
                        case "Annual Enrollment Summary": //PIR 23729
                            larlstResult.Add("rptBenefitEnrollment.rpt" + ".pdf");
                            larlstResult.Add(lobjNeospinBase.CreateDynamicReport("rptBenefitEnrollment.rpt", ldt, string.Empty, hstParam: lhstParam));
                            break;
                        case "DC 2025 Retirement Enrollment Report":
                            larlstResult.Add("rptDC25RetirementEnrollment.rpt" + ".pdf");
                            larlstResult.Add(lobjNeospinBase.CreateDynamicReport("rptDC25RetirementEnrollment.rpt", ldt, string.Empty, hstParam: lhstParam));
                            break;
                            //case "Retirement Contributions - Audit Confirmation": // PIR 24994
                            //    larlstResult.Add("rptRetirementContributionsAuditConfirmation.rpt" + ".pdf");
                            //    larlstResult.Add(lobjNeospinBase.CreateDynamicReport("rptRetirementContributionsAuditConfirmation.rpt", ldt, string.Empty, hstParam: lhstParam));
                            //    break;
                    }
                    larlstResult.Add("application/pdf");
                }
                else if(iobjPassInfo.istrPostBackControlID == "btnESSCSVReportsDownload")
                {
                    switch (astrReportName)
                    {
                        case "Leave of Absence":
                            larlstResult.Add("rptLeaveOfAbsence.rpt" + ".xls");
                            larlstResult.Add(lobjNeospinBase.CreateDynamicExcelReport("rptLeaveOfAbsence.rpt", ldt, string.Empty, hstParam: lhstParam));
                            break;
                        case "Missing Retirement Contributions":
                            larlstResult.Add("rptMissingContributions.rpt" + ".xls");
                            larlstResult.Add(lobjNeospinBase.CreateDynamicExcelReport("rptMissingContributions.rpt", ldt, string.Empty, hstParam: lhstParam));
                            break;
                        case "Missing Retirement Enrollment":
                            larlstResult.Add("rptMissingRetirementEnrollment.rpt" + ".xls");
                            larlstResult.Add(lobjNeospinBase.CreateDynamicExcelReport("rptMissingRetirementEnrollment.rpt", ldt, string.Empty, hstParam: lhstParam));
                            break;
                        case "Life Insurance Level of Coverage":
                            larlstResult.Add("rptLifeLevelofCoverage.rpt" + ".xls");
                            larlstResult.Add(lobjNeospinBase.CreateDynamicExcelReport("rptLifeLevelofCoverage.rpt", ldt, string.Empty, hstParam: lhstParam));
                            break;
                        case "Dental Enrollment Report":
                            larlstResult.Add("rptDentalEnrollment.rpt" + ".xls");
                            larlstResult.Add(lobjNeospinBase.CreateDynamicExcelReport("rptDentalEnrollment.rpt", ldt, string.Empty, hstParam: lhstParam));
                            break;
                        case "Deferred Compensation Enrollment Report":
                            larlstResult.Add("rptDeferredCompensationEnrollment.rpt" + ".xls");
                            larlstResult.Add(lobjNeospinBase.CreateDynamicExcelReport("rptDeferredCompensationEnrollment.rpt", ldt, string.Empty, hstParam: lhstParam));
                            break;
                        case "Flex Enrollment Report":
                            larlstResult.Add("rptFlexEnrollment.rpt" + ".xls");
                            larlstResult.Add(lobjNeospinBase.CreateDynamicExcelReport("rptFlexEnrollment.rpt", ldt, string.Empty, hstParam: lhstParam));
                            break;
                        case "Vision Enrollment Report":
                            larlstResult.Add("rptVisionEnrollment.rpt" + ".xls");
                            larlstResult.Add(lobjNeospinBase.CreateDynamicExcelReport("rptVisionEnrollment.rpt", ldt, string.Empty, hstParam: lhstParam));
                            break;
                        case "Health Enrollment Report":
                            larlstResult.Add("rptHealthEnrollment.rpt" + ".xls");
                            larlstResult.Add(lobjNeospinBase.CreateDynamicExcelReport("rptHealthEnrollment.rpt", ldt, string.Empty, hstParam: lhstParam));
                            break;
                        case "Retirement Enrollment Report":
                            larlstResult.Add("rptRetirementEnrollment.rpt" + ".xls");
                            larlstResult.Add(lobjNeospinBase.CreateDynamicExcelReport("rptRetirementEnrollment.rpt", ldt, string.Empty, hstParam: lhstParam));
                            break;
                        case "EAP Enrollment Report":
                            larlstResult.Add("rptEAPEnrollment.rpt" + ".xls");
                            larlstResult.Add(lobjNeospinBase.CreateDynamicExcelReport("rptEAPEnrollment.rpt", ldt, string.Empty, hstParam: lhstParam));
                            break;
                        case "Annual Enrollment Summary":
                            larlstResult.Add("rptBenefitEnrollment.rpt" + ".xls");
                            larlstResult.Add(lobjNeospinBase.CreateDynamicExcelReport("rptBenefitEnrollment.rpt", ldt, string.Empty, hstParam: lhstParam));
                            break;
                        case "DC 2025 Retirement Enrollment Report"://PIR 25920
                            larlstResult.Add("rptDC25RetirementEnrollment.rpt" + ".xls");
                            larlstResult.Add(lobjNeospinBase.CreateDynamicExcelReport("rptDC25RetirementEnrollment.rpt", ldt, string.Empty, hstParam: lhstParam));
                            break;
                    }
                    larlstResult.Add("application/pdf");
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.Publish(ex);
                utlError lutlError = AddError(0, ex.Message);
                larlstResult.Add(lutlError);
            }
            return larlstResult;
        }
        public bool LoadBalancingStatusValue()
        {
            DataTable ldtEmployerPayrollHeader = Select("cdoEmployerPayrollHeader.LoadBalancingStatus", new object[1] { this.ibusOrganization?.icdoOrganization?.org_id });
            int ldetailCount = 0;
            if (ldtEmployerPayrollHeader.Rows.Count > 0 && ldtEmployerPayrollHeader.Rows[0]["COUNT"] != DBNull.Value)
                ldetailCount = Convert.ToInt32(ldtEmployerPayrollHeader.Rows[0]["COUNT"]);
            return ldetailCount > 0;
        }
        public override void ValidateHardErrors(utlPageMode aenmPageMode)
        {
            base.ValidateHardErrors(aenmPageMode);
            foreach (utlError lobjErr in iarrErrors)
                lobjErr.istrErrorID = string.Empty;
        }
        public ArrayList ValidateAndDownloadReport(int aintOrgId, string astrReportName, DateTime admStartDate, DateTime admEndDate)
        {
            busESSHome lbusESSHome = new busESSHome();
            lbusESSHome.idtmStartDate = admStartDate;
            lbusESSHome.idtmEndDate = admEndDate;
            lbusESSHome.istrReportName = astrReportName;

            lbusESSHome.ValidateHardErrors(utlPageMode.All);
            
            return lbusESSHome.iarrErrors;
        }
    }
}
