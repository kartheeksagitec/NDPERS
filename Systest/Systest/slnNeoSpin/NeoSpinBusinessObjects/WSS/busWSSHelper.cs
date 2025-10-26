using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sagitec.BusinessObjects;
using System.Collections.ObjectModel;
using NeoSpin.CustomDataObjects;
using System.Data;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public static class busWSSHelper
    {
        /// <summary>
        /// Method to publish message board for member
        /// </summary>
        /// <param name="aintMessageID"></param>
        /// <param name="astrMessageText"></param>
        /// <param name="astrPriorityValue"></param>
        /// <param name="astrAudienceValue"></param>
        /// <param name="aintPersonID"></param>
        /// <param name="aintPlanID"></param>
        /// <param name="astrPersonTypeValue"></param>
        /// <param name="astrWebLink"></param>
        /// <param name="astrCorrespondenceLink"></param>
        /// <param name="aintTrackingID"></param>
        public static void PublishMSSMessage(int aintWssMessageId, int aintMessageID, string astrMessageText, string astrPriorityValue,
                                            int aintPersonID = 0, int aintPlanID = 0, string astrPersonTypeValue = null,
                                            string astrWebLink = null, string astrCorrespondenceLink = null, int aintTrackingID = 0, string astrTemplateName = null)
        {

            busWssMessageHeader lobjMessageHeader = new busWssMessageHeader { icdoWssMessageHeader = new cdoWssMessageHeader() };
            lobjMessageHeader.icdoWssMessageHeader.message_id = aintMessageID;
            lobjMessageHeader.icdoWssMessageHeader.message_text = astrMessageText;
            lobjMessageHeader.icdoWssMessageHeader.priority_value = astrPriorityValue;
            lobjMessageHeader.icdoWssMessageHeader.audience_value = busConstant.WSS_MessageBoard_Audience_Member;
            lobjMessageHeader.icdoWssMessageHeader.plan_id = aintPlanID;
            lobjMessageHeader.icdoWssMessageHeader.person_id = aintPersonID;
            if (aintWssMessageId > 0)
                lobjMessageHeader.icdoWssMessageHeader.wss_message_id = aintWssMessageId;
            else
                lobjMessageHeader.icdoWssMessageHeader.Insert();

            Collection<busPersonAccount> lclbPersonAccount = new Collection<busPersonAccount>();
            lclbPersonAccount = IntialLoad(aintPersonID, aintPlanID, astrPersonTypeValue);
            IEnumerable<busPersonAccount> lenmPersonAccountFilterByPlanId = FilterPersonAccountByPlanID(lclbPersonAccount, aintPlanID);
            IEnumerable<busPersonAccount> lenmPersonAccountFilteredByPersonType = FilterPersonAccountByPersonType(lenmPersonAccountFilterByPlanId, astrPersonTypeValue, aintPlanID,
                aintPersonID);

            if (lenmPersonAccountFilteredByPersonType.Count() > 0)
            {
                PublishMSSMessageDetail(lenmPersonAccountFilteredByPersonType, lobjMessageHeader.icdoWssMessageHeader.wss_message_id, astrWebLink,
                    astrCorrespondenceLink, aintTrackingID, astrTemplateName);
            }
            else if(aintPersonID > 0)
            {
                PublishMSSMessageDetail(aintPersonID, lobjMessageHeader.icdoWssMessageHeader.wss_message_id, astrWebLink,
                                   astrCorrespondenceLink, aintTrackingID, astrTemplateName);
            }
        }

        /// <summary>
        /// Method to publish message board details for member
        /// </summary>
        /// <param name="aenmPersonAccountFilteredByPersonType"></param>
        /// <param name="aintWSSMessageID"></param>
        /// <param name="astrWebLink"></param>
        /// <param name="astrCorrespondenceLink"></param>
        /// <param name="aintTrackingID"></param>
        private static void PublishMSSMessageDetail(IEnumerable<busPersonAccount> aenmPersonAccountFilteredByPersonType, int aintWSSMessageID, string astrWebLink,
                                                    string astrCorrespondenceLink, int aintTrackingID, string astrTemplateName)
        {
            int lintPrevPersonID = 0;
            foreach (busPersonAccount lobjPersonAccount in aenmPersonAccountFilteredByPersonType)
            {
                if (lintPrevPersonID != 0 && lintPrevPersonID == lobjPersonAccount.icdoPersonAccount.person_id)
                    continue;

                busWssMessageDetail lobjMessageDetail = new busWssMessageDetail { icdoWssMessageDetail = new cdoWssMessageDetail() };
                lobjMessageDetail.icdoWssMessageDetail.wss_message_id = aintWSSMessageID;
                lobjMessageDetail.icdoWssMessageDetail.person_id = lobjPersonAccount.icdoPersonAccount.person_id;
                lobjMessageDetail.icdoWssMessageDetail.web_link = astrWebLink;
                lobjMessageDetail.icdoWssMessageDetail.correspondence_link = astrCorrespondenceLink;
                lobjMessageDetail.icdoWssMessageDetail.tracking_id = aintTrackingID;
                lobjMessageDetail.icdoWssMessageDetail.template_name = astrTemplateName;
                lobjMessageDetail.icdoWssMessageDetail.clear_message_flag = busConstant.Flag_No;
                lobjMessageDetail.icdoWssMessageDetail.Insert();

                lintPrevPersonID = lobjPersonAccount.icdoPersonAccount.person_id;
            }
        }

        /// <summary>
        /// Method to publish message board details for member
        /// </summary>
        /// <param name="aintPersonID"></param>
        /// <param name="aintWSSMessageID"></param>
        /// <param name="astrWebLink"></param>
        /// <param name="astrCorrespondenceLink"></param>
        /// <param name="aintTrackingID"></param>
        private static void PublishMSSMessageDetail(int aintPersonID, int aintWSSMessageID, string astrWebLink,
                                                    string astrCorrespondenceLink, int aintTrackingID, string astrTemplateName)
        {
            busWssMessageDetail lobjMessageDetail = new busWssMessageDetail { icdoWssMessageDetail = new cdoWssMessageDetail() };
            lobjMessageDetail.icdoWssMessageDetail.wss_message_id = aintWSSMessageID;
            lobjMessageDetail.icdoWssMessageDetail.person_id = aintPersonID;
            lobjMessageDetail.icdoWssMessageDetail.web_link = astrWebLink;
            lobjMessageDetail.icdoWssMessageDetail.correspondence_link = astrCorrespondenceLink;
            lobjMessageDetail.icdoWssMessageDetail.tracking_id = aintTrackingID;
            lobjMessageDetail.icdoWssMessageDetail.template_name = astrTemplateName;
            lobjMessageDetail.icdoWssMessageDetail.clear_message_flag = busConstant.Flag_No;
            lobjMessageDetail.icdoWssMessageDetail.Insert();
        }

        /// <summary>
        /// Method to filter person based on any of the parameter(person id, plan id or person type)
        /// </summary>
        /// <param name="aintPersonID"></param>
        /// <param name="aintPlanID"></param>
        /// <param name="astrPersonTypeValue"></param>
        /// <returns></returns>
        private static Collection<busPersonAccount> IntialLoad(int aintPersonID, int aintPlanID, string astrPersonTypeValue)
        {
            DataTable ldtPersonAccount = new DataTable();
            if (aintPersonID > 0)
            {
                ldtPersonAccount = busBase.Select<cdoPersonAccount>(new string[1] { "person_id" },
                                                                    new object[1] { aintPersonID }, null, null);
            }
            else if (aintPlanID > 0)
            {
                ldtPersonAccount = busBase.Select("cdoPersonAccount.ESSLoadByPlanId", new object[1] { aintPlanID });
            }
            else if (!string.IsNullOrEmpty(astrPersonTypeValue))
            {
                ldtPersonAccount = busBase.Select("cdoPersonAccount.ESSLoadPayeeNonPayee", new object[1] { astrPersonTypeValue });
            }
            busBase lobjBase = new busBase();
            return lobjBase.GetCollection<busPersonAccount>(ldtPersonAccount, "icdoPersonAccount");
        }

        /// <summary>
        /// Method to filter person based on plan id 
        /// </summary>
        /// <param name="aclbPersonAccount"></param>
        /// <param name="aintPlanID"></param>
        /// <returns></returns>
        private static IEnumerable<busPersonAccount> FilterPersonAccountByPlanID(Collection<busPersonAccount> aclbPersonAccount, int aintPlanID)
        {
            if (aintPlanID > 0)
                return aclbPersonAccount.Where(o => o.icdoPersonAccount.plan_id == aintPlanID);
            else
                return aclbPersonAccount.AsEnumerable();
        }

        /// <summary>
        /// Method to filter person based on person type
        /// </summary>
        /// <param name="aenmPersonAccountFilterByPlanId"></param>
        /// <param name="astrPersonTypeValue"></param>
        /// <returns></returns>
        private static IEnumerable<busPersonAccount> FilterPersonAccountByPersonType(IEnumerable<busPersonAccount> aenmPersonAccountFilterByPlanId, string astrPersonTypeValue,
            int aintPlanID, int aintPersonID)
        {
            if (!string.IsNullOrEmpty(astrPersonTypeValue))
            {
                DataTable ldtPersonAccount = new DataTable();
                ldtPersonAccount = busBase.Select("cdoPersonAccount.ESSLoadPayeeNonPayee", new object[3] { astrPersonTypeValue, aintPlanID == 0 ? -999 : aintPlanID, 
                    aintPersonID == 0 ? -999 : aintPersonID });
                busBase lobjBase = new busBase();
                Collection<busPersonAccount> lclbPersonAccount = new Collection<busPersonAccount>();
                lclbPersonAccount = lobjBase.GetCollection<busPersonAccount>(ldtPersonAccount, "icdoPersonAccount");
                return aenmPersonAccountFilterByPlanId.Join(lclbPersonAccount.AsEnumerable(),
                                                            o => o.icdoPersonAccount.person_account_id,
                                                            l => l.icdoPersonAccount.person_account_id,
                                                            (o, l) => l);
            }
            else
                return aenmPersonAccountFilterByPlanId;
        }

        /// <summary>
        /// Method to publish message board header for employer
        /// </summary>
        /// <param name="aintMessageID"></param>
        /// <param name="astrMessageText"></param>
        /// <param name="astrPriorityValue"></param>
        /// <param name="astrAudienceValue"></param>
        /// <param name="aintPlanID"></param>
        /// <param name="aintOrgID"></param>
        /// <param name="astrContactRoleValue"></param>
        /// <param name="aintContactID"></param>
        /// <param name="astrEmpCategoryValue"></param>
        /// <param name="astrWebLink"></param>
        /// <param name="astrCorrespondenceLink"></param>
        /// <param name="aintTrackingID"></param>
        public static void PublishESSMessage(int aintWssMessageId, int aintMessageID, string astrMessageText, string astrPriorityValue,
                                            int aintPlanID = 0, int aintOrgID = 0, string astrContactRoleValue = null, int aintContactID = 0, string astrEmpCategoryValue = null,
                                            string astrWebLink = null, string astrCorrespondenceLink = null, int aintTrackingID = 0, string astrTemplateName = null, string astrWssMessageRequestFlag = null, string astrJobClass = null,
                                            string astrType = null, string astrMemberType = null, string astrBenefitType = null, string astrBenefitOption = null, DateTime adtBenefitBeginDateFrom = default(DateTime), DateTime adtBenefitBeginDateTo = default(DateTime),
                                            string astrEmpCategory = null, string astrCentralPayroll = null, string astrPeopleSoftGroup=null, string astrContactRole = null, Collection<busOrgContact> aclbOrgContacts = null)
        {
            if (astrWssMessageRequestFlag.IsNullOrEmpty()) //WSS message request PIR-17066
            {
                DataTable ldtOrgContact = busBase.Select("cdoOrgContact.LoadOrgContactForESSMessage",
                    new object[5] { aintOrgID == 0? -9:aintOrgID, string.IsNullOrEmpty(astrEmpCategoryValue) ? "0":astrEmpCategoryValue, aintContactID==0?-9:aintContactID, 
                    aintPlanID==0?-9:aintPlanID, string.IsNullOrEmpty(astrContactRoleValue)?"0":astrContactRoleValue});

            if (ldtOrgContact.Rows.Count > 0)
            {
                busWssMessageHeader lobjMessageHeader = new busWssMessageHeader { icdoWssMessageHeader = new cdoWssMessageHeader() };
                lobjMessageHeader.icdoWssMessageHeader.message_id = aintMessageID;
                lobjMessageHeader.icdoWssMessageHeader.message_text = astrMessageText;
                lobjMessageHeader.icdoWssMessageHeader.priority_value = astrPriorityValue;
                lobjMessageHeader.icdoWssMessageHeader.audience_value = busConstant.WSS_MessageBoard_Audience_Employer;
                lobjMessageHeader.icdoWssMessageHeader.plan_id = aintPlanID;
                lobjMessageHeader.icdoWssMessageHeader.org_id = aintOrgID;
                lobjMessageHeader.icdoWssMessageHeader.contact_id = aintContactID;
                lobjMessageHeader.icdoWssMessageHeader.contact_role_value = astrContactRoleValue;
                lobjMessageHeader.icdoWssMessageHeader.emp_category_value = astrEmpCategoryValue;
                if (aintWssMessageId > 0)
                    lobjMessageHeader.icdoWssMessageHeader.wss_message_id = aintWssMessageId;
                else
                    lobjMessageHeader.icdoWssMessageHeader.Insert();


                busBase lobjBase = new busBase();
                Collection<busOrgContact> lclbOrgContact = new Collection<busOrgContact>();
                lclbOrgContact = lobjBase.GetCollection<busOrgContact>(ldtOrgContact, "icdoOrgContact");

                    PublishESSMessageDetail(aclbOrgContacts.IsNotNull() ? aclbOrgContacts : lclbOrgContact, lobjMessageHeader.icdoWssMessageHeader.wss_message_id, astrWebLink, astrCorrespondenceLink, aintTrackingID, astrTemplateName);
                }
            }
            else
            {   //WSS message request PIR-17066
                if (astrWssMessageRequestFlag == busConstant.Flag_No)
                {
                    busWssMessageHeader lobjMessageHeader = new busWssMessageHeader { icdoWssMessageHeader = new cdoWssMessageHeader() };
                    lobjMessageHeader.icdoWssMessageHeader.message_id = aintMessageID;
                    lobjMessageHeader.icdoWssMessageHeader.message_text = astrMessageText;
                    lobjMessageHeader.icdoWssMessageHeader.priority_value = astrPriorityValue;
                    lobjMessageHeader.icdoWssMessageHeader.audience_value = busConstant.WSS_MessageBoard_Audience_Employer;
                    lobjMessageHeader.icdoWssMessageHeader.plan_id = aintPlanID;
                    lobjMessageHeader.icdoWssMessageHeader.org_id = aintOrgID;
                    lobjMessageHeader.icdoWssMessageHeader.contact_id = aintContactID;
                    lobjMessageHeader.icdoWssMessageHeader.contact_role_value = astrContactRoleValue;
                    lobjMessageHeader.icdoWssMessageHeader.emp_category_value = astrEmpCategoryValue;

                    //active
                    lobjMessageHeader.icdoWssMessageHeader.job_class_value = astrJobClass;
                    lobjMessageHeader.icdoWssMessageHeader.type_value = astrType;
                    lobjMessageHeader.icdoWssMessageHeader.member_type_value = astrMemberType;
                    //retire
                    lobjMessageHeader.icdoWssMessageHeader.benefit_type_value = astrBenefitType;
                    lobjMessageHeader.icdoWssMessageHeader.benefit_option_value = astrBenefitOption;
                    lobjMessageHeader.icdoWssMessageHeader.benefit_begin_date_from = adtBenefitBeginDateFrom;
                    lobjMessageHeader.icdoWssMessageHeader.benefit_begin_date_to = adtBenefitBeginDateTo;
                    //employer
                    lobjMessageHeader.icdoWssMessageHeader.emp_category_value = astrEmpCategory;
                    lobjMessageHeader.icdoWssMessageHeader.central_payroll_flag = astrCentralPayroll;
                    lobjMessageHeader.icdoWssMessageHeader.peoplesoft_org_group_value = astrPeopleSoftGroup;
                    lobjMessageHeader.icdoWssMessageHeader.contact_role_value = astrContactRole;
                    lobjMessageHeader.icdoWssMessageHeader.is_message_sent = busConstant.Flag_Yes;

                    if (aintWssMessageId > 0)
                        lobjMessageHeader.icdoWssMessageHeader.wss_message_id = aintWssMessageId;
                    else
                        lobjMessageHeader.icdoWssMessageHeader.Insert();
                }
            }
            
        }

        /// <summary>
        /// Method to publish message board details for employer
        /// </summary>
        /// <param name="aenmFilteredByContactId"></param>
        /// <param name="aintWSSMessageID"></param>
        /// <param name="astrWebLink"></param>
        /// <param name="astrCorrespondenceLink"></param>
        /// <param name="aintTrackingID"></param>
        private static void PublishESSMessageDetail(Collection<busOrgContact> aclbOrgContact, int aintWSSMessageID, string astrWebLink = null, string astrCorrespondenceLink = null,
                                                    int aintTrackingID = 0, string astrTemplateName = null)
        {
            int lintPreviousOrgID = 0, lintPreviousContactID = 0;
            foreach (busOrgContact lobjOrgContact in aclbOrgContact)
            {
                if (lintPreviousContactID != 0 && lintPreviousOrgID != 0 &&
                    lintPreviousOrgID == lobjOrgContact.icdoOrgContact.org_id && lintPreviousContactID == lobjOrgContact.icdoOrgContact.contact_id)
                    continue;

                busWssMessageDetail lobjMessageDetail = new busWssMessageDetail { icdoWssMessageDetail = new cdoWssMessageDetail() };
                lobjMessageDetail.icdoWssMessageDetail.wss_message_id = aintWSSMessageID;
                lobjMessageDetail.icdoWssMessageDetail.org_id = lobjOrgContact.icdoOrgContact.org_id;
                lobjMessageDetail.icdoWssMessageDetail.contact_id = lobjOrgContact.icdoOrgContact.contact_id;
                lobjMessageDetail.icdoWssMessageDetail.web_link = astrWebLink;
                lobjMessageDetail.icdoWssMessageDetail.correspondence_link = astrCorrespondenceLink;
                lobjMessageDetail.icdoWssMessageDetail.tracking_id = aintTrackingID;
                lobjMessageDetail.icdoWssMessageDetail.template_name = astrTemplateName;
                lobjMessageDetail.icdoWssMessageDetail.clear_message_flag = busConstant.Flag_No;
                lobjMessageDetail.icdoWssMessageDetail.Insert();

                lintPreviousContactID = lobjOrgContact.icdoOrgContact.contact_id;
                lintPreviousOrgID = lobjOrgContact.icdoOrgContact.org_id;
            }
        }

        /// <summary>
        /// Method to filter employer based on org type and category
        /// </summary>
        /// <param name="aintOrgID"></param>
        /// <param name="astrEmpCategoryValue"></param>
        /// <returns></returns>
        private static Collection<busOrganization> FilterEmployersByOrgIdAndCategory(int aintOrgID, string astrEmpCategoryValue)
        {
            DataTable ldtEmployers;
            Collection<busOrganization> lclbEmployers = new Collection<busOrganization>();
            busBase lobjBase = new busBase();
            if (aintOrgID == 0)
            {
                ldtEmployers = busBase.Select<cdoOrganization>(new string[1] { "org_type_value" },
                                                                                   new object[1] { busConstant.OrganizationTypeEmployer }, null, null);
                if (!String.IsNullOrEmpty(astrEmpCategoryValue))
                {
                    DataTable ldtFilteredbyCategory = ldtEmployers.AsEnumerable()
                                                                    .Where(o => o.Field<string>("emp_category_value") == astrEmpCategoryValue)
                                                                    .AsDataTable();
                    lclbEmployers = lobjBase.GetCollection<busOrganization>(ldtFilteredbyCategory, "icdoOrganization");
                }
                else
                    lclbEmployers = lobjBase.GetCollection<busOrganization>(ldtEmployers, "icdoOrganization");
            }
            else
            {
                busOrganization lobjOrganization = new busOrganization();
                lobjOrganization.FindOrganization(aintOrgID);
                lclbEmployers.Add(lobjOrganization);
            }
            return lclbEmployers;
        }

        /// <summary>
        /// Method to filter employers based on contact role
        /// </summary>
        /// <param name="aclbEmployers"></param>
        /// <param name="astrContactRoleValue"></param>
        /// <returns></returns>
        private static IEnumerable<busOrgContact> FilterEmployersByContactRole(Collection<busOrganization> aclbEmployers, string astrContactRoleValue)
        {
            IEnumerable<busOrgContact> lenmEmployersOrgContact = null;
            foreach (busOrganization lobjOrg in aclbEmployers)
            {
                lobjOrg.LoadOrgContact();

                if (!String.IsNullOrEmpty(astrContactRoleValue))
                {
                    if (lenmEmployersOrgContact.IsNullOrEmpty())
                        lenmEmployersOrgContact = lobjOrg.iclbOrgContact.Where(o => o.icdoContactRole.contact_role_value == astrContactRoleValue);
                    else
                        lenmEmployersOrgContact.Concat(lobjOrg.iclbOrgContact.Where(o => o.icdoContactRole.contact_role_value == astrContactRoleValue));
                }
                else
                {
                    if (lenmEmployersOrgContact.IsNullOrEmpty())
                        lenmEmployersOrgContact = lobjOrg.iclbOrgContact.AsEnumerable();
                    else
                        lenmEmployersOrgContact.Concat(lobjOrg.iclbOrgContact.AsEnumerable());
                }
            }
            return lenmEmployersOrgContact;
        }

        /// <summary>
        /// Method to filter employers based on plan id
        /// </summary>
        /// <param name="aenmEmployersOrgContact"></param>
        /// <param name="aintPlanID"></param>
        /// <returns></returns>
        private static IEnumerable<busOrgContact> FilterEmployersByPlanID(IEnumerable<busOrgContact> aenmEmployersOrgContact, int aintPlanID)
        {
            if (aintPlanID > 0)
                return aenmEmployersOrgContact.Where(o => o.icdoOrgContact.plan_id == aintPlanID);
            else
                return aenmEmployersOrgContact;
        }

        /// <summary>
        /// Method to filter employers based on contact id
        /// </summary>
        /// <param name="aenmFilteredByPlanId"></param>
        /// <param name="aintContactID"></param>
        /// <returns></returns>
        private static IEnumerable<busOrgContact> FilterEmployersByContactID(IEnumerable<busOrgContact> aenmFilteredByPlanId, int aintContactID)
        {
            if (aintContactID > 0)
                return aenmFilteredByPlanId.Where(o => o.icdoOrgContact.contact_id == aintContactID);
            else
                return aenmFilteredByPlanId;
        }
        //db optional
        public static bool IsDBRetirementOptional(int aintPlanId, int aintEmploymentDetailId)
        {
            busPlan lbusPlan = new busPlan();
            lbusPlan.FindPlan(aintPlanId);
            busPersonEmploymentDetail lobjPersonEmploymentDetail = new busPersonEmploymentDetail();
            lobjPersonEmploymentDetail.FindPersonEmploymentDetail(aintEmploymentDetailId);

            //check for retirement
            if (lbusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeRetirement)
            { //check for DB
                if (lbusPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueDB || lbusPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueHB)       //PIR 25920 New Plan DC 2025
                {//if main is optional
                    if (aintPlanId == busConstant.PlanIdMain || aintPlanId == busConstant.PlanIdMain2020 || aintPlanId == busConstant.PlanIdDC2025)//PIR 20232//PIR 25920 New Plan DC 2025
                    {
                        lobjPersonEmploymentDetail.SetMSSIsTempEmploymentDtlWithinFirstSixMonths(DateTime.Now);

                        if (lobjPersonEmploymentDetail.iblnIsMSSTempEmploymentDtlWithinFirstSixMonths)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        //if DB is retirement enrollment 
        public static bool IsDBElectedRetirementEnrollment(int aintPlanId, int aintPersonAccountId, int aintEmploymentDetailId)
        {
            busPlan lbusPlan = new busPlan();
            lbusPlan.FindPlan(aintPlanId);
            busPersonEmploymentDetail lobjPersonEmploymentDetail = new busPersonEmploymentDetail();
            lobjPersonEmploymentDetail.FindPersonEmploymentDetail(aintEmploymentDetailId);
            busPersonAccount lobjPersonAccount = new busPersonAccount();
            lobjPersonAccount.FindPersonAccount(aintPersonAccountId);

            if ((lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.job_class_value == busConstant.PersonJobClassNonStateElectedOfficial)
                                 || (lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.job_class_value == busConstant.PersonJobClassStateElectedOfficial))
            {
                DateTime ldtDateToBeCompared = DateTime.MinValue;
                if (aintPersonAccountId > 0)
                {
                    ldtDateToBeCompared = lobjPersonAccount.icdoPersonAccount.start_date;
                }
                else
                {
                    ldtDateToBeCompared = DateTime.Now;
                }
                if (busGlobalFunctions.CheckDateOverlapping(ldtDateToBeCompared,
                                                                                               lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date,
                                                                                                lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date.AddMonths(6)))
                {
                    return true;
                }
            }
            return false;
        }


        //if DC is optional enrollment - 
        public static bool IsDCOptionalRetirementEnrollment(int aintPlanId, int aintPersonAccountId, int aintEmploymentDetailId)
        {
            busPlan lbusPlan = new busPlan();
            lbusPlan.FindPlan(aintPlanId);
            busPersonEmploymentDetail lobjPersonEmploymentDetail = new busPersonEmploymentDetail();
            lobjPersonEmploymentDetail.FindPersonEmploymentDetail(aintEmploymentDetailId);
            busPersonAccount lobjPersonAccount = new busPersonAccount();
            lobjPersonAccount.FindPersonAccount(aintPersonAccountId);
            DateTime ldtDateToBeCompared = DateTime.MinValue;
            if (aintPersonAccountId > 0)
            {
                ldtDateToBeCompared = lobjPersonAccount.icdoPersonAccount.start_date;
            }
            else
            {
                ldtDateToBeCompared = DateTime.Now;
            }

            lobjPersonEmploymentDetail.SetMSSIsTempEmploymentDtlWithinFirstSixMonths(ldtDateToBeCompared);

            if (lobjPersonEmploymentDetail.iblnIsMSSTempEmploymentDtlWithinFirstSixMonths)
            {

                return true;
            }
            return false;
        }


    }
}
