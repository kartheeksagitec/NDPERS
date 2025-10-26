using Sagitec.BusinessObjects;
using System;
using System.Collections.ObjectModel;
using NeoSpin.CustomDataObjects;
using NeoSpin.DataObjects;
using Sagitec.DataObjects;
using System.Data;
using System.Collections;
using Sagitec.Common;
using Sagitec.DBUtility;
using System.Linq;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busMSSRetBenAppWeb : busExtendBase
    {
        public busMSSRetBenAppWeb()
        {
            iblnNoMainCDO = true;
        }
        public string istrBenRetOption { get; set; }
        public string istrBenTypeValue { get; set; }
        public int iintPlanId { get; set; }
        public Collection<cdoPlan> iclcSupendedRetPlanAccounts { get; set; }
        public Collection<busBenefitApplication> iclbBenApps { get; set; }
        public Collection<busWssBenApp> iclbWSSBenApps { get; set; }
        public Collection<busWssBenApp> iclbWSSBenAppsSACL { get; set; }
        
        busWssBenApp ibusWssBenApp = new busWssBenApp();
        public int iintPersonId { get; set; }

        public int iintSACLWSSApplicationId { get; set; }

        public string istrDisplayErrorMsgText { get; set; }

        public bool iblnDisplayUpdateContactInfoLink { get; set; }

        public busPerson ibusPerson { get; set; }

        public bool iblnIsShowSavedApplications { get; set; }
        public DateTime idtTerminationDate { get; set; }
        public string istrNewEmailAddress { get; set; }
        public void LoadPerson()
        {
            if (ibusPerson.IsNull()) ibusPerson = new busPerson();
            ibusPerson.FindPerson(iintPersonId);
        }
        public void LoadBenAppsExcBenTypesPSTDAndDETH(int aintPersonId)
        {
            iclbWSSBenApps = GetCollection<busWssBenApp>(Select("cdoWssPersonAccountEnrollmentRequest.LoadBenefitAppsExceptPSTDAndDETH", new object[1] { aintPersonId }), "icdoWssBenApp");
            //iclbWSSBenApps.ForEach(i => i.LoadPlan());
            LoadWssBenAppDetails(aintPersonId);
        }
        public void LoadBenAppDeatils(int aintPersonId)
        {
           
            iclbWSSBenApps = GetCollection<busWssBenApp>(Select("cdoMSSRetBenAppWeb.LoadBenAppDetails", new object[1] { aintPersonId }), "icdoWssBenApp");
            LoadWssBenAppDetails(aintPersonId);

        }

        public void LoadWssBenAppDetails(int aintPersonId)
        {
            foreach (busWssBenApp lbusWssBenApp in iclbWSSBenApps)
            {
                //this load method is only for displying document list those are depend upon initial load rule. Hence calling some load methods.
                //if(lbusWssBenApp.icdoWssBenApp.wss_ben_app_id > 0) 
                //{
                    lbusWssBenApp.icdoWssBenApp.person_id = aintPersonId;
                    lbusWssBenApp.FindWssBenApp(lbusWssBenApp.icdoWssBenApp.wss_ben_app_id);
                    iintSACLWSSApplicationId = lbusWssBenApp.icdoWssBenApp.wss_ben_app_id;
                    lbusWssBenApp.icdoWssBenApp.plan_id = iintPlanId;
                    lbusWssBenApp.LoadPlan();
                    lbusWssBenApp.LoadMemberPerson();
                    lbusWssBenApp.LoadPersonAccount();
                    lbusWssBenApp.ibusPersonAccount.ibusPerson = lbusWssBenApp.ibusMemberPerson;
                    lbusWssBenApp.LoadPersonEmploymentDetail();
                    lbusWssBenApp.InitializeDisabilityAndOtherObjects();
                    lbusWssBenApp.LoadWssOtherDisabilityBenefits();
                    lbusWssBenApp.LoadInsPremACHDetailsAcknowledgement();
                    lbusWssBenApp.LoadDependents();
                    lbusWssBenApp.FindUploadedDocuements();
                    //lbusWssBenApp.LoadLifePersonAccount();
                    //lbusWssBenApp.SetInsStepsAndPanelsVisibilityAndLoadEnrollmentObjects(lbusWssBenApp.icdoWssBenApp.modified_date);                    
                    lbusWssBenApp.EvaluateInitialLoadRules();
                //}
            }
        }
        //public override void AddToResponse(utlResponseData aobjResponseData)
        //{
        //    if(aobjResponseData.iarrErrors.IsNull() && aobjResponseData.istrFormName == "wfmMSSSelectRetPlanMaintenance" && iobjPassInfo.idictParams.ContainsKey("PostBackControl") && Convert.ToString(iobjPassInfo.idictParams["PostBackControl"]) == "btnExecuteRefreshFromObject1") 
        //        aobjResponseData.ConcurrentOtherData["IsRefreshFromButton"] = "CallingWizard";

        //    base.AddToResponse(aobjResponseData);
        //}
        

        public void LoadSuspendedRetPlanAccounts()
        {
            iclcSupendedRetPlanAccounts = doBase.GetCollection<cdoPlan>(Select("cdoWssPersonAccountEnrollmentRequest.LoadSuspendedRetPlanAccounts", new object[1] { iintPersonId }));
        }
        public Collection<cdoPlan> LoadSuspendedRetPlanAccountsScreen()
        {
            return iclcSupendedRetPlanAccounts.IsNotNull() && iclcSupendedRetPlanAccounts.Count > 0 ? iclcSupendedRetPlanAccounts :
                doBase.GetCollection<cdoPlan>(Select("cdoWssPersonAccountEnrollmentRequest.LoadSuspendedRetPlanAccounts", new object[1] { iintPersonId }));
        }
        public Collection<busWssBenApp> LoadSuspendedRetPlanForModify()
        {
            iclbWSSBenAppsSACL = GetCollection<busWssBenApp>(Select("cdoWssBenApp.LookUp", new object[0] { })).Where<busWssBenApp>(iclbFilterWssBenApps => iclbFilterWssBenApps.icdoWssBenApp.person_id == iintPersonId
                    && iclbFilterWssBenApps.icdoWssBenApp.ben_action_status_value == busConstant.BenefitApplicationActionStatusSaveAndContinueLater).ToList().ToCollection();

            iclbWSSBenAppsSACL.ForEach(i => i.LoadPlan());
            if (iclbWSSBenAppsSACL.Count > 0)
                iblnIsShowSavedApplications = true;
            return iclbWSSBenAppsSACL;
        }
                
        public ArrayList ValidateSelectedPlan()
        {
            ArrayList larlstResult = new ArrayList();
            utlError lobjError = new utlError();
            if (iintPlanId == 0)
            {
                lobjError = new utlError();
                if(iblnDisplayUpdateContactInfoLink)
                    lobjError = AddError(0, "Please update your Spouse information.");
                else
                    lobjError = AddError(0, "Please Select Plan.");
                larlstResult.Add(lobjError);
                return larlstResult;
            }

            //if (ibusPerson.icdoPerson.email_address.IsNull())
            //{
            //    lobjError = new utlError();
            //    lobjError = AddError(0, "Email address does not exists.");
            //    larlstResult.Add(lobjError);
            //    return larlstResult;
            //}
            //if member employment has end date set this end date as termination date. 
            if (idtTerminationDate == DateTime.MinValue && ibusPerson.IsNotNull() && ibusPerson.ibusLastEmployment.IsNotNull() && ibusPerson.ibusLastEmployment.icdoPersonEmployment.IsNotNull())
                idtTerminationDate = ibusPerson.ibusLastEmployment.icdoPersonEmployment.end_date;

            Collection<cdoPlan> iclcPreviousApplicationExists = doBase.GetCollection<cdoPlan>(Select("cdoWssPersonAccountEnrollmentRequest.ValidatePreviousApplications", new object[2] { iintPlanId,iintPersonId}));

            
			ibusWssBenApp?.FindWssBenAppByPersonId(iintPersonId, iintPlanId, busConstant.BenefitApplicationActionStatusSaveAndContinueLater); //For Always Load
            if (ibusWssBenApp.IsNotNull() && ibusWssBenApp.icdoWssBenApp.IsNotNull())
            {
                iintSACLWSSApplicationId = ibusWssBenApp.icdoWssBenApp.wss_ben_app_id;
                //if member has set termination date previously set this date as termination date. This could be orride employment_end_date and cause a defect
                //also if member have two plans in this case termination date will vary based upon plan, may cause lead a contradictory termination date.
                //shown two plans on screen with termination date field termination date will not able to update.
                if (idtTerminationDate == DateTime.MinValue)
                    idtTerminationDate = ibusWssBenApp.icdoWssBenApp.retirement_date;
            }
            if (istrBenRetOption != busConstant.BenAppsValue)
            {
                if (idtTerminationDate == DateTime.MinValue)
                {
                    lobjError = new utlError();
                    lobjError = AddError(1905, string.Empty);
                    larlstResult.Add(lobjError);
                    lobjError.istrFocusControl = "txtIdtTerminationDate";
                    return larlstResult;
                }
                if (idtTerminationDate > DateTime.Today.AddMonths(6))
                {
                    lobjError = new utlError();
                    lobjError = AddError(10429, string.Empty);
                    larlstResult.Add(lobjError);
                    lobjError.istrFocusControl = "txtIdtTerminationDate";
                    return larlstResult;
                }
                if (idtTerminationDate <= ibusPerson.ibusLastEmployment.icdoPersonEmployment.start_date)
                {
                    lobjError = new utlError();
                    lobjError = AddError(10392, string.Empty);
                    larlstResult.Add(lobjError);
                    lobjError.istrFocusControl = "txtIdtTerminationDate";
                    return larlstResult;
                }
                larlstResult.Add(this);
            }
            
            return larlstResult;
        }

        //public bool VisibilityTerminationDate()
        //{
        //    if (!AreApplicationPlanOptionsBeShown())
        //    {
        //        return false;
        //    }
        //    //Remove code and place in seperate function so that same code will be called from various places
        //    if(idtTerminationDate == DateTime.MinValue)
        //        idtTerminationDate = GetEmpTerminationDate();
        //    if (idtTerminationDate == DateTime.MinValue)
        //    { return true; }
        //    else
        //    {
        //        return false;
        //    }            
        //}

        public bool VisibilityTerminationDate()
        {
            if (!AreApplicationPlanOptionsBeShown())
            {
                return false;
            }
            if (ibusPerson.IsNull()) LoadPerson();
            if (ibusPerson.icolPersonEmployment == null)
                ibusPerson.LoadPersonEmployment();
            if (ibusPerson.icolPersonEmployment.IsNotNull() && ibusPerson.icolPersonEmployment.Count > 0)
            {
                ibusPerson.ibusLastEmployment = ibusPerson.icolPersonEmployment.Where(o => o.icdoPersonEmployment.end_date == DateTime.MinValue)
                    .OrderByDescending(i => i.icdoPersonEmployment.end_date_no_null).FirstOrDefault();
            }
            if (ibusPerson.ibusLastEmployment.IsNotNull() && ibusPerson.ibusLastEmployment.icdoPersonEmployment.person_employment_id > 0)
            {
                return true;
            }
            else
            {
                ibusPerson.LoadLastEmployment();
                if (ibusPerson.ibusLastEmployment.icdoPersonEmployment.person_employment_id > 0)
                {
                    //here assign employment end date to variable and use this employment_end_date further in application wizard.
                    idtTerminationDate = ibusPerson.ibusLastEmployment.icdoPersonEmployment.end_date_no_null_today;
                    if (idtTerminationDate == DateTime.MinValue)
                    { return true; }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public bool AreApplicationPlanOptionsBeShown()
        {
            if (ibusPerson.IsNull()) LoadPerson();
            ibusPerson.LoadPayeeAccount(true);
            if (ibusPerson.iclbPayeeAccount.Count > 0 && ibusPerson.iclbPayeeAccount.Any(payeeaccount => payeeaccount.ibusPayeeAccountActiveStatus.istrPayeeAccountStatusData2 == busConstant.PayeeAccountStatusSuspended))
            {
                istrDisplayErrorMsgText = busGlobalFunctions.GetMessageTextByMessageID(10356, iobjPassInfo);
                return false;
            }
            if (iclbWSSBenAppsSACL.IsNull()) { LoadSuspendedRetPlanForModify(); }
            if (iclbWSSBenApps.IsNull()) { LoadBenAppDeatils(ibusPerson.icdoPerson.person_id); }
            if (iclcSupendedRetPlanAccounts.IsNull()) { LoadSuspendedRetPlanAccounts();}
            if (iclcSupendedRetPlanAccounts.Count > 0 && ibusPerson.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusMarried)
            {
                if (!checkIfSpouseInformationExists())
                {
                    iblnDisplayUpdateContactInfoLink = true;
                    return false;
                }
                
            }
            if (iclcSupendedRetPlanAccounts.Count == 0)
            {
                istrDisplayErrorMsgText = busGlobalFunctions.GetMessageTextByMessageID(10357, iobjPassInfo);
                return false;
            }
            else if (iclcSupendedRetPlanAccounts.Count == 1)
            {
                iintPlanId = Convert.ToInt32(iclcSupendedRetPlanAccounts[0].plan_id);
                if (ibusWssBenApp.FindWssBenAppByPersonId(iintPersonId, iintPlanId, busConstant.BenefitApplicationActionStatusSaveAndContinueLater))
                {
                    idtTerminationDate = ibusWssBenApp.icdoWssBenApp.termination_date;
                }
                //return false;
            }
            if (iclcSupendedRetPlanAccounts.Count > 0) return true;
            return false;
        }
        public DateTime GetEmpTerminationDate()
        {
            //DateTime ldtTerDate = DateTime.MinValue;
            if (ibusPerson.IsNull()) LoadPerson();
            if (ibusPerson.icolPersonEmployment == null)
                ibusPerson.LoadPersonEmployment();
            if (ibusPerson.icolPersonEmployment.IsNotNull() && ibusPerson.icolPersonEmployment.Count > 0)
            {
                ibusPerson.ibusLastEmployment = ibusPerson.icolPersonEmployment.Where(o => o.icdoPersonEmployment.end_date == DateTime.MinValue)
                    .OrderByDescending(i => i.icdoPersonEmployment.end_date_no_null).FirstOrDefault();
            }
            if (ibusPerson.ibusLastEmployment.IsNotNull() && ibusPerson.ibusLastEmployment.icdoPersonEmployment.person_employment_id > 0)
            {
                //return true;
            }
            else
            {
                ibusPerson.LoadLastEmployment();
                if (ibusPerson.ibusLastEmployment.icdoPersonEmployment.person_employment_id > 0)
                {
                    //here assign employment end date to variable and use this employment_end_date further in application wizard.
                    idtTerminationDate = ibusPerson.ibusLastEmployment.icdoPersonEmployment.end_date_no_null_today;
                    //ldtTerDate = idtTerminationDate;                    
                }
            }
            return idtTerminationDate;
        }
        public string VerifyPlansAndRedirectToWizard(int aintPersonID)
        {
            string astrModeAction = string.Empty;
            iintPersonId = aintPersonID;
            LoadSuspendedRetPlanAccounts();
            if (iclcSupendedRetPlanAccounts.Count == 1)
            {
                ibusWssBenApp = new busWssBenApp() { icdoWssBenApp = new cdoWssBenApp() };
                iintPlanId = Convert.ToInt32(iclcSupendedRetPlanAccounts[0].plan_id);
                idtTerminationDate = GetEmpTerminationDate();
                ibusWssBenApp.FindWssBenAppByPersonId(iintPersonId, iintPlanId, busConstant.BenefitApplicationActionStatusSaveAndContinueLater);
                if (ibusWssBenApp.IsNotNull() && ibusWssBenApp.icdoWssBenApp.IsNotNull())
                {
                    if (idtTerminationDate == DateTime.MinValue)
                        idtTerminationDate = ibusWssBenApp.icdoWssBenApp.termination_date;
                }
                if (idtTerminationDate != DateTime.MinValue)
                {
                    astrModeAction = ValidateDataIfSinglePlan();
                    if (!string.IsNullOrEmpty(astrModeAction))
                    {
                        return astrModeAction;
                    }
                    if (ibusWssBenApp.IsNotNull() && ibusWssBenApp.icdoWssBenApp.IsNotNull())
                    {
                        iintSACLWSSApplicationId = ibusWssBenApp.icdoWssBenApp.wss_ben_app_id;
                        if (iintSACLWSSApplicationId == 0)
                            return "new";
                        iintPlanId = ibusWssBenApp.icdoWssBenApp.plan_id;
                        //if member has set termination date previously set this date as termination date. This could be orride employment_end_date and cause a defect
                        //also if member have two plans in this case termination date will vary based upon plan, may cause lead a contradictory termination date.
                        //shown two plans on screen with termination date field termination date will not able to update.
                        if (idtTerminationDate == DateTime.MinValue)
                            idtTerminationDate = ibusWssBenApp.icdoWssBenApp.retirement_date;
                        astrModeAction = "update";
                    }
                    else
                    {
                        iintPlanId = iintPlanId;
                        astrModeAction = "new";
                    }
                }
                else
                {
                    astrModeAction = "stay";
                }
            }
            return astrModeAction;
        }
        public string ValidateDataIfSinglePlan()
        {
            string lstrValidationMsg = string.Empty;

            if (ibusPerson.IsNull()) LoadPerson();
            ibusPerson.LoadPayeeAccount(true);
            if (ibusPerson.iclbPayeeAccount.Count > 0 && ibusPerson.iclbPayeeAccount.Any(payeeaccount => payeeaccount.ibusPayeeAccountActiveStatus.istrPayeeAccountStatusData2 == busConstant.PayeeAccountStatusSuspended))
            {
                lstrValidationMsg = busGlobalFunctions.GetMessageTextByMessageID(10356, iobjPassInfo);                
            }
            if (iclbWSSBenAppsSACL.IsNull()) { LoadSuspendedRetPlanForModify(); }
            if (iclcSupendedRetPlanAccounts.IsNull()) { LoadSuspendedRetPlanAccounts(); }
            if (iclcSupendedRetPlanAccounts.Count > 0 && ibusPerson.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusMarried)
            {
                if(!checkIfSpouseInformationExists())
                    lstrValidationMsg = "Please update your Spouse information.";                    
            }

            Collection<cdoPlan> iclcPreviousApplicationExists = doBase.GetCollection<cdoPlan>(Select("cdoWssPersonAccountEnrollmentRequest.ValidatePreviousApplications", new object[2] { iintPlanId, iintPersonId }));

            if (iclcPreviousApplicationExists.IsNotNull() && iclcPreviousApplicationExists.Count > 0)
            {
                lstrValidationMsg = busGlobalFunctions.GetMessageTextByMessageID(10431, iobjPassInfo);
            }
            if (iclcSupendedRetPlanAccounts.Count == 0)
            {
                lstrValidationMsg = busGlobalFunctions.GetMessageTextByMessageID(10357, iobjPassInfo);
            }
            
            return lstrValidationMsg;
        }
        public bool checkIfSpouseInformationExists()
        {
            if (ibusPerson.ibusSpouse.IsNull()) ibusPerson.LoadSpouse();
            busPersonContact lobjPersonContact = ibusPerson.icolPersonContact.FirstOrDefault(o => o.icdoPersonContact.relationship_value == busConstant.PersonContactTypeSpouse
                                                                        && o.icdoPersonContact.status_value == busConstant.PersonContactStatusActive);
            if (lobjPersonContact.IsNull())
            {
                iblnDisplayUpdateContactInfoLink = true;
                return false;
            }
            else if (ibusPerson.ibusSpouse.icdoPerson.date_of_death != DateTime.MinValue)
            {
                iblnDisplayUpdateContactInfoLink = true;
                return false;
            }
            else
                return true;
        }
        public ArrayList SaveEmailAddress()
        {
            if (ibusPerson.IsNull())
                LoadPerson();
            ibusWssBenApp.ibusMemberPerson = ibusPerson;
            ibusWssBenApp.istrNewEmailAddress = istrNewEmailAddress;
            return ibusWssBenApp.SaveEmailAddress();
        }
        public bool iblnBenAppIdIsNotNull()
        {
            if (iclbWSSBenApps.Count() > 0)
            {
                return true;
            }
            return false;
        }
        public ArrayList btnDeleteBenApp(int aintWssBenAppId)
        {
            ArrayList larlstResult = new ArrayList();
            utlError lobjError = new utlError();
            int ldtDelete = DBFunction.DBNonQuery("cdoMSSRetBenAppWeb.DeleteBenAppRecord",
                new object[1] { aintWssBenAppId }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return larlstResult;
        }
        public void LoadBenTypeRecords()
        {
            if (ibusPerson.IsNull()) LoadPerson();
            string idecBenType = Convert.ToString(DBFunction.DBExecuteScalar("cdoMSSRetBenAppWeb.LoadBenAppRecords",
                        new object[1] { ibusPerson.icdoPerson.person_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
            istrBenTypeValue = idecBenType;
        }
        public bool iblnPlanVisible()
        {
            if(iclcSupendedRetPlanAccounts.Count() >1)
            {
                return true;
            }
            return false;
        }
    }
}
