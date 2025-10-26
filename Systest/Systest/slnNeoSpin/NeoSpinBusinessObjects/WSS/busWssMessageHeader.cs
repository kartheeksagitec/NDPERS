#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;
using NeoSpin.DataObjects;
using Sagitec.CustomDataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
	/// <summary>
	/// Class NeoSpin.BusinessObjects.busWssMessageHeader:
	/// Inherited from busWssMessageHeaderGen, the class is used to customize the business object busWssMessageHeaderGen.
	/// </summary>
	[Serializable]
	public class busWssMessageHeader : busWssMessageHeaderGen
	{
        public Collection<busWssMessageDetail> iclbWssMessageDetail { get; set; }

        public void LoadMessageDetails()
        {
            DataTable ldtMessageDetails = Select<cdoWssMessageDetail>(new string[1] { enmWssMessageDetail.wss_message_id.ToString() },
                                                                        new object[1] { icdoWssMessageHeader.wss_message_id }, null, null);
            iclbWssMessageDetail = new Collection<busWssMessageDetail>();
            iclbWssMessageDetail = GetCollection<busWssMessageDetail>(ldtMessageDetails, "icdoWssMessageDetail");

            foreach (busWssMessageDetail lobjDetail in iclbWssMessageDetail)
            {
                lobjDetail.LoadPerson();
                lobjDetail.LoadOrganization();
            }
        }

        public void LoadTop100MessageDetails()
        {
            DataTable ldtMessageDetails = Select("cdoWssMessageDetail.LoadTop100", new object[1] { icdoWssMessageHeader.wss_message_id });
            iclbWssMessageDetail = new Collection<busWssMessageDetail>();
            iclbWssMessageDetail = GetCollection<busWssMessageDetail>(ldtMessageDetails, "icdoWssMessageDetail");

            foreach (busWssMessageDetail lobjDetail in iclbWssMessageDetail)
            {
                lobjDetail.LoadPerson();
                lobjDetail.LoadOrganization();
            }
        }

        public override void  BeforeValidate(utlPageMode aenmPageMode)
        {
            if (icdoWssMessageHeader.message_id > 0)
            {
                DataTable ldtMessage = iobjPassInfo.isrvDBCache.GetCacheData("sgs_messages", "message_id=" + icdoWssMessageHeader.message_id.ToString());
                if (ldtMessage.Rows.Count > 0)
                {
                    icdoWssMessageHeader.message_text = ldtMessage.Rows[0]["display_message"].ToString();
                }                
            }
         	base.BeforeValidate(aenmPageMode);
        }

        public override void BeforePersistChanges()
        {
            if (icdoWssMessageHeader.is_message_sent.IsNullOrEmpty()) //WSS message request PIR-17066
            {
                if (icdoWssMessageHeader.audience_value == busConstant.WSSMessageAudienceEmployer)
                {
                    icdoWssMessageHeader.person_id = 0;
                    icdoWssMessageHeader.person_type_value = null;

                    icdoWssMessageHeader.org_id = busGlobalFunctions.GetOrgIdFromOrgCode(icdoWssMessageHeader.istrOrgCode);
                }
                else if (icdoWssMessageHeader.audience_value == busConstant.WSSMessageAudienceMember)
                {
                    icdoWssMessageHeader.org_id = 0;
                    icdoWssMessageHeader.emp_category_value = null;
                    icdoWssMessageHeader.contact_role_value = null;
                    icdoWssMessageHeader.contact_id = 0;
                }
            }
            else
            {   //WSS message request PIR-17066
                if (icdoWssMessageHeader.is_message_sent == busConstant.Flag_No)
                {
                    icdoWssMessageHeader.person_id = 0;
                    icdoWssMessageHeader.person_type_value = null;                    
                    icdoWssMessageHeader.contact_id = 0;

                    if (icdoWssMessageHeader.member_type_value == busConstant.MemberTypeEmployer) //WSS message request PIR-17066
                    {
                        if (!String.IsNullOrEmpty(icdoWssMessageHeader.istrOrgCode))
                        {
                            icdoWssMessageHeader.org_id = busGlobalFunctions.GetOrgIdFromOrgCode(icdoWssMessageHeader.istrOrgCode);
                        }
                        else
                            icdoWssMessageHeader.org_id = 0;
                    }
                    else
                        icdoWssMessageHeader.org_id = 0;                    
                }
            }
            base.BeforePersistChanges();
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            if (icdoWssMessageHeader.is_message_sent.IsNullOrEmpty()) //WSS message request PIR-17066
            {
                if (icdoWssMessageHeader.audience_value == busConstant.WSSMessageAudienceEmployer)
                {
                    busWSSHelper.PublishESSMessage(icdoWssMessageHeader.wss_message_id, icdoWssMessageHeader.message_id, icdoWssMessageHeader.message_text, icdoWssMessageHeader.priority_value,
                        icdoWssMessageHeader.plan_id, icdoWssMessageHeader.org_id, icdoWssMessageHeader.contact_role_value, icdoWssMessageHeader.contact_id,
                        icdoWssMessageHeader.emp_category_value, icdoWssMessageHeader.istrWebLink, icdoWssMessageHeader.istrCorrespondenceLink, 0);
                }
                else if (icdoWssMessageHeader.audience_value == busConstant.WSSMessageAudienceMember)
                {
                    busWSSHelper.PublishMSSMessage(icdoWssMessageHeader.wss_message_id, icdoWssMessageHeader.message_id, icdoWssMessageHeader.message_text, icdoWssMessageHeader.priority_value,
                        icdoWssMessageHeader.person_id, icdoWssMessageHeader.plan_id, icdoWssMessageHeader.person_type_value,
                        icdoWssMessageHeader.istrWebLink, icdoWssMessageHeader.istrCorrespondenceLink, 0);
                }
                LoadMessageDetails();
            }
            else
            {
                //WSS message request PIR-17066
                if (icdoWssMessageHeader.is_message_sent == busConstant.Flag_No)
                {
                    busWSSHelper.PublishESSMessage(icdoWssMessageHeader.wss_message_id, icdoWssMessageHeader.message_id, icdoWssMessageHeader.message_text, icdoWssMessageHeader.priority_value,
                         icdoWssMessageHeader.plan_id, icdoWssMessageHeader.org_id, icdoWssMessageHeader.contact_role_value, icdoWssMessageHeader.contact_id,
                         icdoWssMessageHeader.emp_category_value, icdoWssMessageHeader.istrWebLink, icdoWssMessageHeader.istrCorrespondenceLink, 0, null, icdoWssMessageHeader.is_message_sent, icdoWssMessageHeader.job_class_value, icdoWssMessageHeader.type_value, icdoWssMessageHeader.member_type_value,
                         icdoWssMessageHeader.benefit_type_value, icdoWssMessageHeader.benefit_option_value, icdoWssMessageHeader.benefit_begin_date_from, icdoWssMessageHeader.benefit_begin_date_to, icdoWssMessageHeader.emp_category_value,
                         icdoWssMessageHeader.central_payroll_flag, icdoWssMessageHeader.peoplesoft_org_group_value, icdoWssMessageHeader.contact_role_value);
                }
            }
        }

        public bool IsMessageIdNotValid()
        {
            bool lblnResult = false;
            if (icdoWssMessageHeader.message_id > 0 && string.IsNullOrEmpty(icdoWssMessageHeader.message_text))
            {
                lblnResult = true;                
            }
            return lblnResult;
        }

        public bool IsNoContactExists()
        {
            bool lblnResult = false;
            if (icdoWssMessageHeader.audience_value == busConstant.WSSMessageAudienceEmployer && !string.IsNullOrEmpty(icdoWssMessageHeader.istrOrgCode))
            {
                busOrganization lobjOrg = new busOrganization();
                lobjOrg.FindOrganization(busGlobalFunctions.GetOrgIdFromOrgCode(icdoWssMessageHeader.istrOrgCode));
                lobjOrg.LoadOrgContact();
                if (lobjOrg.iclbOrgContact.Count == 0)
                    lblnResult = true;
            }
            return lblnResult;
        }

        public busOrganization ibusOrganization { get; set; }

        public void LoadOrganization()
        {
            ibusOrganization = new busOrganization();
            ibusOrganization.FindOrganization(icdoWssMessageHeader.org_id);
        }

        public busPerson ibusPerson { get; set; }

        public void LoadPerson()
        {
            ibusPerson = new busPerson();
            ibusPerson.FindPerson(icdoWssMessageHeader.person_id);
        }

        public bool IsMessageLengthGreaterThan2000()
        {
            bool lblnResult = false;

            if (!string.IsNullOrEmpty(icdoWssMessageHeader.message_text) && icdoWssMessageHeader.message_text.Length > 2000)
                lblnResult = true;

            return lblnResult;
        }

        public bool IsWSSMessageDetailExists()
        {
            bool lblnResult = false;
            if (icdoWssMessageHeader.audience_value == busConstant.WSS_MessageBoard_Audience_Employer)
            {
                DataTable ldtWSSMessage = busBase.Select("cdoOrgContact.LoadOrgContactForESSMessage",
                    new object[5] { icdoWssMessageHeader.org_id == 0? -9:icdoWssMessageHeader.org_id, 
                                string.IsNullOrEmpty(icdoWssMessageHeader.emp_category_value) ? "0":icdoWssMessageHeader.emp_category_value, 
                                icdoWssMessageHeader.contact_id==0?-9:icdoWssMessageHeader.contact_id, 
                                icdoWssMessageHeader.plan_id==0?-9:icdoWssMessageHeader.plan_id, 
                                string.IsNullOrEmpty(icdoWssMessageHeader.contact_role_value)?"0":icdoWssMessageHeader.contact_role_value});
                if (ldtWSSMessage.Rows.Count > 0)
                    lblnResult = true;
            }
            else
                lblnResult = true;
            return lblnResult;
        }

        public override int Delete()
        {
            if (iclbWssMessageDetail == null)
                LoadMessageDetails();
            foreach (busWssMessageDetail lobjDetail in iclbWssMessageDetail)
                lobjDetail.icdoWssMessageDetail.Delete();
            return base.Delete();
        }

        public bool IsMemberMessageParametersInvalid()
        {
            bool lblnResult = false;
            if (icdoWssMessageHeader.audience_value == busConstant.WSS_MessageBoard_Audience_Member &&
                (!string.IsNullOrEmpty(icdoWssMessageHeader.istrOrgCode) || icdoWssMessageHeader.contact_id > 0 ||
                !string.IsNullOrEmpty(icdoWssMessageHeader.contact_role_value) || !string.IsNullOrEmpty(icdoWssMessageHeader.emp_category_value)))
            {
                lblnResult = true;
            }
            return lblnResult;
        }

        public bool IsEmployerMessageParametersInvalid()
        {
            bool lblnResult = false;
            if (icdoWssMessageHeader.audience_value == busConstant.WSS_MessageBoard_Audience_Employer &&
                (!string.IsNullOrEmpty(icdoWssMessageHeader.person_type_value) || icdoWssMessageHeader.person_id > 0))
            {
                lblnResult = true;
            }
            return lblnResult;
        }

        #region //WSS message request PIR-17066
        public Collection<cdoCodeValue> LoadBenefitOptionsBasedOnBenefit(int aintPlanId, string astrBenefitAccountType)
        {
            Collection<cdoCodeValue> lclcBenefitOption = new Collection<cdoCodeValue>();
            Collection<busCodeValue> lclbBenefitOption = busGlobalFunctions.LoadCodeValueByData1(1903, aintPlanId.ToString());
            if (lclbBenefitOption.Count > 0)
            {
                foreach (busCodeValue lobjCodeValue in lclbBenefitOption)
                {
                    if (lobjCodeValue.icdoCodeValue.data2 == astrBenefitAccountType)
                    {
                        busCodeValue lobjCodeValueNew = new busCodeValue();
                        lobjCodeValueNew.icdoCodeValue = new cdoCodeValue();
                        lobjCodeValueNew.icdoCodeValue = busGlobalFunctions.GetCodeValueDetails(2216, lobjCodeValue.icdoCodeValue.data3);
                        lclcBenefitOption.Add(lobjCodeValueNew.icdoCodeValue);
                    }
                }
            }
            lclcBenefitOption = busGlobalFunctions.Sort<cdoCodeValue>("code_value_order", lclcBenefitOption);
            return lclcBenefitOption;
        }

        public utlCollection<cdoCodeValue> iclbRefundBenefitOption { get; set; }
        public Collection<cdoCodeValue> LoadBenefitOptionsBasedOnPlans(int aintPlanId, string astrBenefitAccountType)
        {
            if (astrBenefitAccountType == busConstant.ApplicationBenefitTypeRetirement || astrBenefitAccountType == busConstant.ApplicationBenefitTypeDisability ||
                astrBenefitAccountType == busConstant.ApplicationBenefitTypePreRetirementDeath)
                return LoadBenefitOptionsBasedOnBenefit(aintPlanId, astrBenefitAccountType);
            if (astrBenefitAccountType == busConstant.ApplicationBenefitTypeRefund)
            {
                DataTable ldtResult = busBase.Select("cdoBenefitApplication.LoadBenefitOptionForRefund", new object[0]{});
                if(ldtResult.Rows.Count> 0)
                    iclbRefundBenefitOption =cdoCodeValue.GetCollection<cdoCodeValue>(ldtResult);
                return iclbRefundBenefitOption;
            }
            if (astrBenefitAccountType == busConstant.ApplicationBenefitTypePostRetirementDeath)
            {
                DataTable ldtResult = busBase.Select("cdoBenefitApplication.LoadDistinctBenefitOption", new object[0] { });
                if (ldtResult.Rows.Count > 0)
                    iclbRefundBenefitOption = cdoCodeValue.GetCollection<cdoCodeValue>(ldtResult);
                return iclbRefundBenefitOption; ;
            }
            return iclbRefundBenefitOption;
        }	
        #endregion //WSS message request PIR-17066
    }
}
