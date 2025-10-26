#region Using directives

using System;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using NeoSpin.BusinessObjects;
using NeoSpin.CustomDataObjects;
using Sagitec.Common;
using Sagitec.BusinessObjects;
using Sagitec.DBUtility;
using Sagitec.ExceptionPub;
using NeoSpin.DataObjects;
using System.Globalization;
using Sagitec.CustomDataObjects;
using System.Linq;
using System.Diagnostics;
using Sagitec.DataObjects;
using System.Data.SqlClient;
using System.IO;
using System.Web.Script.Serialization;
#endregion

namespace NeoSpin.BusinessTier
{
    public class srvNeoSpinWSS : srvNeoSpin
    {
        public srvNeoSpinWSS()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        //FMUpgrade : Added Global Parameter for srvMethods

        private int iintOrgID;
        private int iintContactID;
        private int iintPersonID;
        private string istrNDPERSEmailID;
        private string istrNDPERSEmailIDESS;
        private string istrSystemRegion;

        private void SetWebParameters()
        {
            if (iobjPassInfo.idictParams.ContainsKey("ContactID"))
                iintContactID = (int)iobjPassInfo.idictParams["ContactID"];
            if (iobjPassInfo.idictParams.ContainsKey("OrgID"))
                iintOrgID = (int)iobjPassInfo.idictParams["OrgID"];
            if (iobjPassInfo.idictParams.ContainsKey("PersonID"))
                iintPersonID = (int)iobjPassInfo.idictParams["PersonID"];
            if (iobjPassInfo.idictParams.ContainsKey("NDPERSEmailID"))
                istrNDPERSEmailID = (string)iobjPassInfo.idictParams["NDPERSEmailID"];
            if (iobjPassInfo.idictParams.ContainsKey("NDPERSEmailIDESS"))
                istrNDPERSEmailIDESS = (string)iobjPassInfo.idictParams["NDPERSEmailIDESS"];
            if (iobjPassInfo.idictParams.ContainsKey("SystemRegion"))
                istrSystemRegion = (string)iobjPassInfo.idictParams["SystemRegion"]; 
        }

        // PIR 9115 functionality enable/disable property
        public string istrIsPIR9115Enabled
        {
            get
            {
                return busGlobalFunctions.GetData1ByCodeValue(52, "9115", iobjPassInfo);
            }
        }

        #region DashBoard
        protected override ArrayList ValidateNew(string astrFormName, Hashtable ahstParam)
        {
            ArrayList larrErrors = null;
            //iobjPassInfo.iconFramework.Open();
            try
            {
                if (astrFormName == "wfmESSEmployerPayrollHeaderLookup")
                {
                    busEmployerPayrollHeaderLookup lbusEmployerPayrollHeaderLookup = new busEmployerPayrollHeaderLookup();
                    larrErrors = lbusEmployerPayrollHeaderLookup.ValidateNew(ahstParam);
                }
                else if (astrFormName == "wfmESSEmployerPayrollDetailLookup")
                {
                    busEmployerPayrollDetailLookup lbusEmployerPayrollDetailLookup = new busEmployerPayrollDetailLookup();
                    larrErrors = lbusEmployerPayrollDetailLookup.ValidateNew(ahstParam);
                }
                else if ((astrFormName == "wfmESSSeminarMaintenance") || (astrFormName == "wfmMSSSeminarMaintenance"))
                {
                    busSeminarSchedule lbusSeminarSchedule = new busSeminarSchedule();
                    larrErrors = lbusSeminarSchedule.ValidateNew(ahstParam);
                }
                else if (astrFormName == "wfmWssMessageRequestLookup")   //WSS message request PIR-17066
                {
                    busWSSMessageHeaderLookup lbusWSSMessageHeaderLookup = new busWSSMessageHeaderLookup();
                    larrErrors = lbusWSSMessageHeaderLookup.ValidateNew(ahstParam);
                }
            }
            finally
            {
                //iobjPassInfo.iconFramework.Close();
            }
            return larrErrors;
        }
        public bool IsPersonExists(int aintPersonID)
        {
            DataTable ldtbResult = busNeoSpinBase.Select<cdoPerson>(new string[1] { "person_id" },
                                                                    new object[1] { aintPersonID }, null, null);

            if (ldtbResult.Rows.Count > 0)
                return true;
            return false;
        }

        public busMSSHome FindMSSHome(int aintPersonID, string astrProfileEmailID = null, bool ablnIsExternalUser = false)
        {
            //FMUpgrade: Changes for set Navigation parameter from menu item click
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmMSSActiveMemberHomeMaintenance" || iobjPassInfo.istrFormName == "wfmMSSHomeLimited")
            {
                aintPersonID = aintPersonID != 0 ? aintPersonID : iintPersonID;
            }
            astrProfileEmailID = istrNDPERSEmailID;
            if (iobjPassInfo.idictParams.ContainsKey("IsExternalLogin"))
                ablnIsExternalUser = Convert.ToBoolean(iobjPassInfo.idictParams["IsExternalLogin"]);
            busMSSHome lbusMSSHome = new busMSSHome();
            if (lbusMSSHome.LoadPerson(aintPersonID))
            {
                lbusMSSHome.istrProfileEmailID = astrProfileEmailID;
                ////Based on the Flag, we will be showing information bar if the Email does not match with profile data
                lbusMSSHome.iblnExternalLogin = ablnIsExternalUser;
                DataTable ldtbSystemManagement = iobjPassInfo.isrvDBCache.GetDBSystemManagement();
                lbusMSSHome.istrRegion_value = Convert.ToString(ldtbSystemManagement.Rows[0]["Region_value"]);

                lbusMSSHome.istrUpdateAccountURL = istrSystemRegion != busConstant.SystemRegionValueProd ? busGlobalFunctions.GetData2ByCodeValue(busConstant.SystemConstantsAndVariablesCodeID, busConstant.UpdateAccountUrlCodeValue, iobjPassInfo) :
                                                                                                            busGlobalFunctions.GetData1ByCodeValue(busConstant.SystemConstantsAndVariablesCodeID, busConstant.UpdateAccountUrlCodeValue, iobjPassInfo);
                //lbusMSSHome.ibusPerson.LoadPersonCurrentAddress();
                ////address
                //lbusMSSHome.ibusPerson.LoadMSSAddress();
                ////contacts
                //lbusMSSHome.ibusPerson.LoadMSSActiveCOntacts();
                ////current employment -- to control visibility rules
                lbusMSSHome.ibusPerson.LoadCurrentEmployer();
                lbusMSSHome.ibusPerson.LoadCurrentEmployerDetails();
                ////to display in the member detail maintenance
                lbusMSSHome.LoadAllPersonEmploymentDetails();
                ////load plans
                lbusMSSHome.LoadEnrolledAndEligiblePlans();
                ////Load contact tickets for the person (Do not show closed tickets which are older than 90 days)
                //lbusMSSHome.LoadContactTickets();
                //lbusMSSHome.LoadMessageBoard();
                //lbusMSSHome.LoadIsDCEligible();
                ////Load seminars for the person for pir 7122
                //lbusMSSHome.LoadSeminars();
                lbusMSSHome.iintMessageCount = GetMSSUnreadMessagesCount(aintPersonID);
                if (lbusMSSHome.iclbPersonEmploymentDetail.Count > 0)
                    lbusMSSHome.iintCurrentEmploymentDetailID = lbusMSSHome.iclbPersonEmploymentDetail[0].icdoPersonEmploymentDetail.person_employment_dtl_id;
                lbusMSSHome.LoadActiveRetirementPlan();
                if (lbusMSSHome.iclbEnrolledPlans.Count() > 0 &&
                    lbusMSSHome.iclbMSSRetirementPersonAccount.Count == 1)
                    lbusMSSHome.ibusRetirementPlan = lbusMSSHome.iclbEnrolledPlans[0];
                lbusMSSHome.GetNoOfDaysRemainingBeforeElection();   //PIR 25920 DC 2025 change
                lbusMSSHome.SetElectionID(busConstant.MemberTypeActive);
            }
            return lbusMSSHome;
        }

        public busMSSHome FindMSSActiveMemberBenefitPlans(int aintPersonID)
        {
            //FMUpgrade: Changes for set Navigation parameter from menu item click
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmActiveMemberBenefitPlansMaintenance")
            {
                aintPersonID = aintPersonID != 0 ? aintPersonID : iintPersonID;
            }
            busMSSHome lbusMSSHome = new busMSSHome();
            if (lbusMSSHome.LoadPerson(aintPersonID))
            {
                lbusMSSHome.iblnIsFromMSSActivePlan = true;//PIR 16010
                lbusMSSHome.LoadEnrolledAndEligiblePlans();
                lbusMSSHome.LoadAllPersonEmploymentDetails();
                if (lbusMSSHome.iclbPersonEmploymentDetail.Count > 0)
                {
                    lbusMSSHome.iintCurrentEmploymentDetailID = lbusMSSHome.iclbPersonEmploymentDetail[0].icdoPersonEmploymentDetail.person_employment_dtl_id;
                    lbusMSSHome.iblnIsTemporaryMember = IsTemporaryMember(lbusMSSHome.iintCurrentEmploymentDetailID);
                }
                //PIR 19997
                busBenefitPlanWeb lbusBenefitPlanWeb = lbusMSSHome.iclbEnrolledPlansUI.FirstOrDefault(ep => ep.iintPlanId == 12);
                if (lbusBenefitPlanWeb.IsNotNull())
                {
                    lbusMSSHome.iintHealthPersonAccountId = lbusBenefitPlanWeb.iintPersonAccountId;
                    lbusMSSHome.iintHDHPPersonEmploymentDetailid = lbusBenefitPlanWeb.iintPersonEmploymentDetailid;
                }
            }
            return lbusMSSHome;
        }

        public busMSSHome FindMSSAnnualEnrolmentActiveMemberBenefitPlans(int aintPersonID)
        {
            //FMUpgrade: Changes for set Navigation parameter from menu item click
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmAnnualEnrollmentBenefitPlansMaintenance")
            {
                aintPersonID = aintPersonID != 0 ? aintPersonID : iintPersonID;
            }
            busMSSHome lbusMSSHome = new busMSSHome();
            if (lbusMSSHome.LoadPerson(aintPersonID))
            {
                lbusMSSHome.iblnIsFromMSSActivePlan = true;//PIR 16010
                lbusMSSHome.LoadAnnualEnrollmentPlans();
                lbusMSSHome.LoadCurrentANNERequestStatus();  // PIR 15126
                lbusMSSHome.LoadAllPersonEmploymentDetails();
                if (lbusMSSHome.iclbPersonEmploymentDetail.Count > 0)
                {
                    lbusMSSHome.iintCurrentEmploymentDetailID = lbusMSSHome.iclbPersonEmploymentDetail[0].icdoPersonEmploymentDetail.person_employment_dtl_id;
                    lbusMSSHome.iblnIsTemporaryMember = IsTemporaryMember(lbusMSSHome.iintCurrentEmploymentDetailID);
                }
            }
            return lbusMSSHome;
        }

        public busMSSHome FindMSSActiveMemberEmployment(int aintPersonID)
        {
            //FMUpgrade: Changes for set Navigation parameter from menu item click
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmActiveMemberEmploymentMaintenance")
            {
                aintPersonID = aintPersonID != 0 ? aintPersonID : iintPersonID;
            }
            busMSSHome lbusMSSHome = new busMSSHome();
            if (lbusMSSHome.LoadPerson(aintPersonID))
            {
                lbusMSSHome.LoadEnrolledAndEligiblePlans();
                lbusMSSHome.LoadActiveRetirementPlan();
                if (lbusMSSHome.iclbEnrolledPlans.Count() > 0 &&
                    lbusMSSHome.iclbMSSRetirementPersonAccount.Count == 1)
                    lbusMSSHome.ibusRetirementPlan = lbusMSSHome.iclbEnrolledPlans[0];
            }
            return lbusMSSHome;
        }

        //For MSS Layout change
        public int GetMSSUnreadMessagesCount(int aintPersonID)
        {
            busMSSHome lbusMSSHome = new busMSSHome();
            lbusMSSHome = LoadMSSMessageDetails(aintPersonID);
            return lbusMSSHome.iclbWSSMessageDetails != null ? lbusMSSHome.iclbWSSMessageDetails.Count : 0; //F/W Upgrade PIR 11934 - When nonexistent PersonId entered, message details collection remains null, handled this scenario
        }

        //Method for Get ESS Message Count
        public int? GetESSUnreadMessagesCount(int aintContactID, int aintOrgID)
        {
            busESSHome lbusESSHome = new busESSHome();
            lbusESSHome = LoadESSMessageDetails(aintContactID, aintOrgID);
            return lbusESSHome.iclbWSSMessageDetails?.Count;
        }
        //For MSS Layout change
        public busMSSHome LoadMSSMessageDetails(int aintPersonID)
        {
            //FMUpgrade: Changes for set Navigation parameter from menu item click
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmMSSInboxMaintenance")
            {
                aintPersonID = aintPersonID != 0 ? aintPersonID : iintPersonID;
            }
            busMSSHome lbusMSSHome = new busMSSHome();
            if (lbusMSSHome.LoadPerson(aintPersonID))
            {
                lbusMSSHome.LoadMessageBoard();
            }
            return lbusMSSHome;
        }

        //For ESS Layout Change
        public busESSHome LoadESSMessageDetails(int aintContactID, int aintOrgID)
        {
            busESSHome lbusESSHome = new busESSHome();
            lbusESSHome.ibusOrganization = new busOrganization() { icdoOrganization = new cdoOrganization() };
            if (lbusESSHome.LoadContact(aintContactID) && lbusESSHome.ibusOrganization.FindOrganization(aintOrgID))
            {
                lbusESSHome.LoadLastThreeMonthUnReadMessages();
            }
            return lbusESSHome;
        }

        //For MSS Layout change
        public busMSSHome FindMSSMemberProfile(int aintPersonID, bool ablnIsExternalUser = false)
        {
            //FMUpgrade: Changes for set Navigation parameter from menu item click
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmMSSProfileMaintenance" || iobjPassInfo.istrFormName == "wfmMSSActiveMemberProfileMaintenance")
            {
                aintPersonID = aintPersonID != 0 ? aintPersonID : iintPersonID;
            }
            busMSSHome lbusMSSHome = new busMSSHome();
            if (lbusMSSHome.LoadPerson(aintPersonID))
            {
                lbusMSSHome.ibusPerson.LoadPersonCurrentAddress();
                lbusMSSHome.ibusPerson.LoadFutureAddresses(); // PIR 8664
                lbusMSSHome.ibusPerson.icdoPerson.communication_preference_value = busConstant.PersonCommPrefRegMail; // PIR 9219
                lbusMSSHome.ibusPerson.LoadCurrentEmployer(); //PIR-11030
                lbusMSSHome.ibusPerson.ibusCurrentEmployment.LoadOrganization(); //PIR-11030                
                //PIR-18492: loading prev data befor changing: 
                if (iobjPassInfo.idictParams.ContainsKey("IsExternalLogin"))
                    ablnIsExternalUser = Convert.ToBoolean(iobjPassInfo.idictParams["IsExternalLogin"]);
                lbusMSSHome.iblnExternalLogin = ablnIsExternalUser;

                // PIR- 18492 Email Waiver is only valid for 9 months so user will have to waive again once expired.
                if (ablnIsExternalUser)
                {


                    if (lbusMSSHome.iblnIsActivationCodeNotVerified)
                    {
                        lbusMSSHome.ibusPerson.icdoPerson.certify_date = DateTime.MinValue;
                        lbusMSSHome.ibusPerson.icdoPerson.activation_code = string.Empty;
                        lbusMSSHome.ibusPerson.icdoPerson.activation_code_date = DateTime.MinValue;
                        lbusMSSHome.ibusPerson.icdoPerson.activation_code_flag = busConstant.Flag_No;
                        lbusMSSHome.ibusPerson.icdoPerson.email_address = string.Empty;
                        lbusMSSHome.ibusPerson.icdoPerson.email_waiver_date = DateTime.MinValue;
                    }
                    int lintMonths = Convert.ToInt32(busGlobalFunctions.GetData1ByCodeValue(7010, "MNTH", iobjPassInfo));
                    int lintMthDiff = busGlobalFunctions.DateDiffByMonth(lbusMSSHome.ibusPerson.icdoPerson.email_waiver_date, DateTime.Today);
                    if (lintMthDiff > lintMonths)
                    {
                        lbusMSSHome.ibusPerson.icdoPerson.email_waiver_flag = busConstant.Flag_No;
                    }
                }
                lbusMSSHome.idtEmilWaiver = lbusMSSHome.ibusPerson.icdoPerson.email_waiver_date;
                lbusMSSHome.istrEmailAddressPre = lbusMSSHome.ibusPerson.icdoPerson.email_address;
                lbusMSSHome.iblnIsCertifyDateNull = lbusMSSHome.IsPersonCertify();
                lbusMSSHome.istrPopUpMessage = LoadMessage(busConstant.PopUpMessageForCertify);
                lbusMSSHome.istrIsEmailWaiverFlagSelected = LoadMessage(busConstant.IsEmailWaiverFlagSelected);
            }
            //wfmDefault.aspx file code conversion - btn_OpenPDF method 
            if (iobjPassInfo.istrFormName == "wfmMSSProfileMaintenance")
            {
                DataTable ldtbPathData = utlPassInfo.iobjPassInfo.isrvDBCache.GetCacheData("sgs_system_paths", "path_code = 'CorrPdf'");
                lbusMSSHome.istrDownloadFileName = ldtbPathData.Rows[0]["path_value"].ToString() + "SFN-10766.pdf";
            }
            lbusMSSHome.ibusPerson.iblnIsFromMSS = true;

            return lbusMSSHome;
        }
        //For MSS Layout change
        public busMSSHome FindMSSMemberContact(int aintPersonID)
        {
            //FMUpgrade: Changes for set Navigation parameter from menu item click
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmMSSContactInfoMaintenance")
            {
                aintPersonID = aintPersonID != 0 ? aintPersonID : iintPersonID;
            }
            busMSSHome lbusMSSHome = new busMSSHome();
            if (lbusMSSHome.LoadPerson(aintPersonID))
            {
                lbusMSSHome.ibusPerson.LoadMSSActiveCOntacts();
            }
            return lbusMSSHome;
        }
        //For MSS Layout change
        public busMSSHome FindMSSMemberBeneficiary(int aintPersonID)
        {
            //FMUpgrade: Changes for set Navigation parameter from menu item click
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmMSSBeneficiaryMaintenance")
            {
                aintPersonID = aintPersonID != 0 ? aintPersonID : iintPersonID;
            }
            busMSSHome lbusMSSHome = new busMSSHome();
            if (lbusMSSHome.LoadPerson(aintPersonID))
            {
                lbusMSSHome.ibusPerson.LoadBeneficiaryForRetirementPlan(); //pir 8504
            }
            return lbusMSSHome;
        }

        public busMSSHome FindMSSMemberDetail(int aintPersonID)
        {
            busMSSHome lbusMSSHome = new busMSSHome();
            if (lbusMSSHome.LoadPerson(aintPersonID))
            {
                //to load dependents
                lbusMSSHome.ibusPerson.LoadDependent();
                //load death notification
                lbusMSSHome.ibusPerson.LoadDeathNotification();
                //set visbility
                lbusMSSHome.SetSFN53405Visibility();
            }
            return lbusMSSHome;
        }

        //load 1099 R for the last 3 years
        public busMSSHome FindMSS1099R(int aintPersonID)
        {
            //FMUpgrade: Changes for set Navigation parameter from menu item click
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmMSSOneZeroNineNineRMaintenance")
            {
                aintPersonID = aintPersonID != 0 ? aintPersonID : iintPersonID;
            }
            busMSSHome lbusMSSHome = new busMSSHome();
            if (lbusMSSHome.LoadPerson(aintPersonID))
            {
                //load 1099 R for the last 3 years
                lbusMSSHome.Load1099RForLast3Yrs();
            }
            return lbusMSSHome;
        }

        //annual Statements for last 3 years
        public busMSSHome FindMSSAnnualStatements(int aintPersonID)
        {
            //FMUpgrade: Changes for set Navigation parameter from menu item click
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmMSSAnnualStatementMaintenance")
            {
                aintPersonID = aintPersonID != 0 ? aintPersonID : iintPersonID;
            }
            busMSSHome lbusMSSHome = new busMSSHome();
            if (lbusMSSHome.LoadPerson(aintPersonID))
            {
                //annual Statements for last 3 years
                lbusMSSHome.LoadAnnualStatementsForLast3Yrs();
                lbusMSSHome.EvaluateInitialLoadRules();
            }
            return lbusMSSHome;
        }

        //load all Payee Accounts for the person
        public busMSSHome FindMSSAllPayeeAccounts(int aintPersonID)
        {
            //FMUpgrade: Changes for set Navigation parameter from menu item click
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmMSSPensionPaymentDetailsMaintenance" || iobjPassInfo.istrFormName == "wfmMSSRetireeHomeMaintenance")
            {
                aintPersonID = aintPersonID != 0 ? aintPersonID : iintPersonID;
            }
            busMSSHome lbusMSSHome = new busMSSHome();
            if (lbusMSSHome.LoadPerson(aintPersonID))
            {
                if (iobjPassInfo.idictParams.ContainsKey("IsExternalLogin"))
                    lbusMSSHome.iblnExternalLogin = Convert.ToBoolean(iobjPassInfo.idictParams["IsExternalLogin"]);
                DataTable ldtbSystemManagement = iobjPassInfo.isrvDBCache.GetDBSystemManagement();
                lbusMSSHome.istrRegion_value = Convert.ToString(ldtbSystemManagement.Rows[0]["Region_value"]);
                lbusMSSHome.LoadPayeeAccounts();
                lbusMSSHome.iintMessageCount = GetMSSUnreadMessagesCount(aintPersonID);
                lbusMSSHome.SetElectionID(busConstant.MemberTypeRetire);
            }
            return lbusMSSHome;
        }

        //For MSS Layout change
        public busMSSHome FindMSSAllBenefitPlans(int aintPersonID)
        {
            //FMUpgrade: Changes for set Navigation parameter from menu item click
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmMSSBenefitPlanInfoMaintenance")
            {
                aintPersonID = aintPersonID != 0 ? aintPersonID : iintPersonID;
            }
            busMSSHome lbusMSSHome = new busMSSHome();
            if (lbusMSSHome.LoadPerson(aintPersonID))
            {
                lbusMSSHome.LoadActiveRetirementPlan(); //pir 8345
                lbusMSSHome.LoadActivePlans();
            }
            return lbusMSSHome;
        }

        //load all Approved / In Payment Service Purchase
        public busMSSHome FindMSSAllServicePurchase(int aintPersonID)
        {
            //FMUpgrade: Changes for set Navigation parameter from menu item click
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmMSSServicePurchaseMaintenance")
            {
                aintPersonID = aintPersonID != 0 ? aintPersonID : iintPersonID;
            }
            busMSSHome lbusMSSHome = new busMSSHome();
            if (lbusMSSHome.LoadPerson(aintPersonID))
            {
                lbusMSSHome.LoadServicePurchaseContracts();
            }
            return lbusMSSHome;
        }

        public busMSSHome FindMSSAllBenefitEstimate(int aintPersonID)
        {
            //FMUpgrade: Changes for set Navigation parameter from menu item click
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmMSSViewBenefitEstimateMaintenance")
            {
                aintPersonID = aintPersonID != 0 ? aintPersonID : iintPersonID;
            }
            busMSSHome lbusMSSHome = new busMSSHome();
            if (lbusMSSHome.LoadPerson(aintPersonID))
            {
                lbusMSSHome.LoadBenefitEstimates(aintPersonID);
            }
            return lbusMSSHome;
        }

        //For MSS Layout change
        public busPayeeAccount FindPaymentHistory(int aintPayeeAccountID, DateTime ldatePaymentDateFrom, DateTime ldatePaymentDateTo)
        {
            busPayeeAccount lbusPayeeAccount = new busPayeeAccount();
            if (lbusPayeeAccount.FindPayeeAccount(aintPayeeAccountID))
            {
                lbusPayeeAccount.LoadPayee();
                lbusPayeeAccount.LoadPaymentDetailsThatYear(ldatePaymentDateFrom, ldatePaymentDateTo);
                lbusPayeeAccount.LoadPlan();
                lbusPayeeAccount.LoadActivePayeeStatusAsofNextBenefitPaymentDate();
            }

            return lbusPayeeAccount;

        }
        //PIR 25699
        public busPayeeAccountTaxWithholding FindTaxWithholdingInformation(int aintPATaxWithholdingId)
        {
            busPayeeAccountTaxWithholding lbusPATaxWithholding = new busPayeeAccountTaxWithholding { icdoPayeeAccountTaxWithholding = new cdoPayeeAccountTaxWithholding() };
            lbusPATaxWithholding.FindPayeeAccountTaxWithholding(aintPATaxWithholdingId);
            lbusPATaxWithholding.LoadPayeeAccount();
            return lbusPATaxWithholding;
        }

        public busPayeeAccount FindMSSTaxWithholdingCompletion(int aintPayeeAccountId)
        {
            if (aintPayeeAccountId == 0 && (iobjPassInfo.idictParams["SenderID"].ToString() == "FromMenu" || iobjPassInfo.idictParams["SenderID"].ToString() == "btnCancel")
                && (iobjPassInfo.istrSenderForm == "wfmMSSPayeeAccountsMaintenance" || iobjPassInfo.istrSenderForm == "wfmMSSTaxWithholdingInformationMaintenance"
                || iobjPassInfo.istrSenderForm == "wfmMSSDirectDepositMaintenance" || iobjPassInfo.istrSenderForm == "wfmMSSPaymentHistorySummaryMaintenance" ||
                iobjPassInfo.istrSenderForm == "wfmMSSTaxWithholdingInformationMaintenanceNew"))
            {
                aintPayeeAccountId = Convert.ToInt32(iobjPassInfo.idictParams["PayeeAccountIdMenu"].ToString());
            }

            busPayeeAccount lbusPayeeAccount = new busPayeeAccount();
            if (lbusPayeeAccount.FindPayeeAccount(aintPayeeAccountId))
            {
                lbusPayeeAccount.LoadApplication();
                lbusPayeeAccount.LoadPlan();
                lbusPayeeAccount.LoadMember();
                lbusPayeeAccount.LoadPayee();
                lbusPayeeAccount.LoadStateTaxWithHoldingInfo();
                lbusPayeeAccount.LoadFedTaxWithHoldingInfo();
                lbusPayeeAccount.LoadTaxWithHoldingHistory();
                lbusPayeeAccount.LoadACHDetail();
                lbusPayeeAccount.LoadActiveACHDetail(); //pir 8643
                lbusPayeeAccount.LoadBenefitAmount();
                lbusPayeeAccount.LoadLastBenefitPaymentDate();
                lbusPayeeAccount.LoadActivePayeeStatusAsofNextBenefitPaymentDate();
            }
            return lbusPayeeAccount;
        }

        //Payee Accounts and ACH detail related payee account
        public busPayeeAccount FindMSSPayeeAccounts(int aintPayeeAccountId)
        {
            if (aintPayeeAccountId == 0 && (iobjPassInfo.idictParams["SenderID"].ToString() == "FromMenu" || iobjPassInfo.idictParams["SenderID"].ToString() == "btnCancel")
                && (iobjPassInfo.istrSenderForm == "wfmMSSPayeeAccountsMaintenance" || iobjPassInfo.istrSenderForm == "wfmMSSTaxWithholdingInformationMaintenance"
                || iobjPassInfo.istrSenderForm == "wfmMSSDirectDepositMaintenance" || iobjPassInfo.istrSenderForm == "wfmMSSPaymentHistorySummaryMaintenance" ||
                iobjPassInfo.istrSenderForm == "wfmMSSTaxWithholdingInformationMaintenanceNew"))
            {
                aintPayeeAccountId = Convert.ToInt32(iobjPassInfo.idictParams["PayeeAccountIdMenu"].ToString());
            }

            busPayeeAccount lbusPayeeAccount = new busPayeeAccount();
            if (lbusPayeeAccount.FindPayeeAccount(aintPayeeAccountId))
            {
                lbusPayeeAccount.LoadApplication();
                lbusPayeeAccount.LoadPlan();
                lbusPayeeAccount.LoadMember();
                lbusPayeeAccount.LoadPayee();
                lbusPayeeAccount.LoadStateTaxWithHoldingInfo();
                lbusPayeeAccount.LoadFedTaxWithHoldingInfo();
                lbusPayeeAccount.LoadTaxWithHoldingHistory();
                lbusPayeeAccount.LoadACHDetail();
                lbusPayeeAccount.LoadActiveACHDetail(); //pir 8643
                lbusPayeeAccount.LoadBenefitAmount();
                lbusPayeeAccount.LoadLastBenefitPaymentDate();
                lbusPayeeAccount.LoadRetroTaxableAndNonTaxableAmount();


                lbusPayeeAccount.LoadGrossAmount();
                lbusPayeeAccount.LoadGrossBenefitAmount();
                lbusPayeeAccount.LoadTotalDeductionAmount();
                lbusPayeeAccount.LoadMontlyBenefits();//For MSS Layout change- order changed
                lbusPayeeAccount.LoadActiveDeductions();//For MSS Layout change
                lbusPayeeAccount.LoadTaxAmounts(); //pir 8618
                lbusPayeeAccount.LoadBenfitAccount();
                lbusPayeeAccount.ibusBenefitAccount.LoadRetirementOrg();
                lbusPayeeAccount.LoadExclusionAmount();
                lbusPayeeAccount.LoadBalanceNontaxableAmount();

                lbusPayeeAccount.LoadPaymentDetails();

                lbusPayeeAccount.LoadMinimumGuaranteeAmount();
                lbusPayeeAccount.LoadNontaxableBeginningBalnce();
                lbusPayeeAccount.LoadMSSYTDPaymentDetails();
                lbusPayeeAccount.LoadMSSPaymentDetailsFromGoLiveDate();

                lbusPayeeAccount.LoadActivePayeeStatusAsofNextBenefitPaymentDate();
                lbusPayeeAccount.iclbTaxWithholingHistory = lbusPayeeAccount.iclbTaxWithholingHistory.Where(history => (history.icdoPayeeAccountTaxWithholding.end_date == DateTime.MinValue || history.icdoPayeeAccountTaxWithholding.start_date < history.icdoPayeeAccountTaxWithholding.end_date)).ToList().ToCollection();
                //PIR 25699
                lbusPayeeAccount.GetTaxWitholdingEffectiveDates();
            }
            return lbusPayeeAccount;
        }

        public busMSSTaxWithholding NewMSSTaxWithholding(int Aintpayeeaccountid, string astrTaxIdentidifier, string astrDistributionType, bool ablnIsExternalUser = false, string astrTaxRef = null)
        {
            if (iobjPassInfo.idictParams.ContainsKey("IsExternalLogin"))
                ablnIsExternalUser = Convert.ToBoolean(iobjPassInfo.idictParams["IsExternalLogin"]);
            busMSSTaxWithholding lobjPayeeAccountTaxWithholding = new busMSSTaxWithholding();
            lobjPayeeAccountTaxWithholding.iblnExternalUser = ablnIsExternalUser; //18492
            lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding = new cdoPayeeAccountTaxWithholding();
            lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.payee_account_id = Aintpayeeaccountid;
            lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_ref = astrTaxRef;
            lobjPayeeAccountTaxWithholding.LoadPayeeAccount();
            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadApplication();
            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadPayee();
            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.ibusPayee.GetPersonCurrentAddressByType(busConstant.AddressTypePermanent, DateTime.Now);
            lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_identifier_value = astrTaxIdentidifier;
            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadStateTaxWithHoldingInfo();//code moved here // PIR 8618
            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadFedTaxWithHoldingInfo();//code moved here // PIR 8618
            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.iblnIsFromMSS = true; // PIR 8618
            lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.benefit_distribution_type_value = astrDistributionType;
            if (lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierFedTax &&
                lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionMonthlyBenefit)
            {
                lobjPayeeAccountTaxWithholding.LoadW4PUIElements();
            }
            if (astrTaxIdentidifier == busConstant.PayeeAccountTaxIdentifierFedTax)
            {
                if (astrDistributionType == busConstant.BenefitDistributionMonthlyBenefit)
                {
                    // PIR 8618
                    if (lobjPayeeAccountTaxWithholding.ibusPayeeAccount.ibusPayeeAccountFedTaxWithholding.IsNotNull()
                        && lobjPayeeAccountTaxWithholding.ibusPayeeAccount.ibusPayeeAccountFedTaxWithholding.icdoPayeeAccountTaxWithholding.payee_account_tax_withholding_id != 0)
                    {
                        lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.monthly_benefit_tax_option = lobjPayeeAccountTaxWithholding.ibusPayeeAccount.ibusPayeeAccountFedTaxWithholding.icdoPayeeAccountTaxWithholding.tax_option_value;
                        //lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.marital_status_value = lobjPayeeAccountTaxWithholding.ibusPayeeAccount.ibusPayeeAccountFedTaxWithholding.icdoPayeeAccountTaxWithholding.marital_status_value;
                        //lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.marital_status_description
                        //    = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(306, lobjPayeeAccountTaxWithholding.ibusPayeeAccount.ibusPayeeAccountFedTaxWithholding.icdoPayeeAccountTaxWithholding.marital_status_value);
                        //lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.no_of_tax_allowance = lobjPayeeAccountTaxWithholding.ibusPayeeAccount.ibusPayeeAccountFedTaxWithholding.icdoPayeeAccountTaxWithholding.tax_allowance.ToString();
                        //lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.additional_tax_amount = lobjPayeeAccountTaxWithholding.ibusPayeeAccount.ibusPayeeAccountFedTaxWithholding.icdoPayeeAccountTaxWithholding.additional_tax_amount;
                    }
                    else
                    {
                        lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.monthly_benefit_tax_option = busConstant.FedTaxOptionFedTaxBasedOnIRS;
                        //lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.marital_status_value = busConstant.PayeeAccountMaritalStatusMarried;
                        //lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.marital_status_description
                        //    = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(306, busConstant.PersonMaritalStatusMarried);
                        //lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.no_of_tax_allowance = busConstant.WithholdingTaxAllowance.ToString();
                    }
                }
                else if (astrDistributionType == busConstant.BenefitDistributionLumpSum)
                {
                    lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.refund_tax_option = busConstant.FedTaxOptionFederalTaxwithheld;
                }
                else if (astrDistributionType == busConstant.BenefitDistributionPSLO)
                {
                    //uat pir 1278 : need to default to Federal tax withheld option
                    lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.plso_tax_option = busConstant.TaxOptionFedTaxwithheld;
                }
            }
            else if (astrTaxIdentidifier == busConstant.PayeeAccountTaxIdentifierStateTax)
            {
                if (astrDistributionType == busConstant.BenefitDistributionMonthlyBenefit)
                {
                    // PIR 8618
                    if (lobjPayeeAccountTaxWithholding.ibusPayeeAccount.ibusPayeeAccountStateTaxWithholding.IsNotNull()
                        && lobjPayeeAccountTaxWithholding.ibusPayeeAccount.ibusPayeeAccountStateTaxWithholding.icdoPayeeAccountTaxWithholding.payee_account_tax_withholding_id != 0)
                    {
                        lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.monthly_benefit_tax_option = lobjPayeeAccountTaxWithholding.ibusPayeeAccount.ibusPayeeAccountStateTaxWithholding.icdoPayeeAccountTaxWithholding.tax_option_value;
                        lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.marital_status_value = lobjPayeeAccountTaxWithholding.ibusPayeeAccount.ibusPayeeAccountStateTaxWithholding.icdoPayeeAccountTaxWithholding.marital_status_value;
                        lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.marital_status_description
                            = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(306, lobjPayeeAccountTaxWithholding.ibusPayeeAccount.ibusPayeeAccountStateTaxWithholding.icdoPayeeAccountTaxWithholding.marital_status_value);
                        lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.no_of_tax_allowance = lobjPayeeAccountTaxWithholding.ibusPayeeAccount.ibusPayeeAccountStateTaxWithholding.icdoPayeeAccountTaxWithholding.tax_allowance.ToString();
                        lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.additional_tax_amount = lobjPayeeAccountTaxWithholding.ibusPayeeAccount.ibusPayeeAccountStateTaxWithholding.icdoPayeeAccountTaxWithholding.additional_tax_amount;
                    }
                    else
                    {
                        lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.monthly_benefit_tax_option = busConstant.StateTaxOptionNoMonthlyNDTax;
                        lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.marital_status_value = busConstant.PayeeAccountMaritalStatusMarried;
                        lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.marital_status_description
                            = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(306, busConstant.PersonMaritalStatusMarried);
                        lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.no_of_tax_allowance = busConstant.WithholdingTaxAllowance.ToString();
                    }
                }
                else if (astrDistributionType == busConstant.BenefitDistributionPSLO)
                {
                    //uat pir 1278 : need to default to State tax withheld option
                    lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.plso_tax_option = busConstant.TaxOptionStateTaxwithheld;
                }
                else if (astrDistributionType == busConstant.BenefitDistributionLumpSum)
                {
                    lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.refund_tax_option = busConstant.StateTaxOptionNoOnetimeNDTax;
                }
            }
            lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.benefit_distribution_type_description
                = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2224, astrDistributionType);
            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadTaxWithHoldingHistory();
            //pir 8618
            if (lobjPayeeAccountTaxWithholding.ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadNexBenefitPaymentDate();
            DateTime ldteSystBatchDate = busGlobalFunctions.GetSysManagementBatchDate();
            lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.start_date = ldteSystBatchDate.Day < 15 ? busGlobalFunctions.GetFirstDayofNextMonth(ldteSystBatchDate) : busGlobalFunctions.GetFirstDayofNextMonth(ldteSystBatchDate.AddMonths(1)); //PIR 21227
            lobjPayeeAccountTaxWithholding.EvaluateInitialLoadRules();
            return lobjPayeeAccountTaxWithholding;
        }
        public busMSSPaymentHistory FindMSSPaymentHistory(int aintPayeePaymentHistoryId)
        {
            busMSSPaymentHistory lbusPaymentHistory = new busMSSPaymentHistory();
            if (lbusPaymentHistory.FindPaymentHistoryHeader(aintPayeePaymentHistoryId))
            {
                lbusPaymentHistory.icdoPaymentHistoryHeader.istrStateTaxTaxExemption =
                     lbusPaymentHistory.icdoPaymentHistoryHeader.state_tax_allowance > 0 ? lbusPaymentHistory.icdoPaymentHistoryHeader.state_tax_allowance.ToString() : string.Empty;
                lbusPaymentHistory.icdoPaymentHistoryHeader.istrFedTaxExemtion =
                    lbusPaymentHistory.icdoPaymentHistoryHeader.fed_tax_allowance > 0 ? lbusPaymentHistory.icdoPaymentHistoryHeader.fed_tax_allowance.ToString() : string.Empty;
                if (lbusPaymentHistory.icdoPaymentHistoryHeader.org_id > 0)
                {
                    lbusPaymentHistory.LoadOrganization();
                }
                lbusPaymentHistory.CalculateAmounts();
                lbusPaymentHistory.LoadPlan();
                lbusPaymentHistory.LoadPaymentHistoryDetails();
                lbusPaymentHistory.LoadPaymentHistoryDistribution();
                lbusPaymentHistory.LoadTaxAmount();
                lbusPaymentHistory.LoadPaymentSchedule();
            }
            return lbusPaymentHistory;
        }

        public busMSSPaymentHistory FindMSSAllPaymentHistory(int aintPayeePaymentHistoryId)
        {
            busMSSPaymentHistory lbusPaymentHistory = new busMSSPaymentHistory();

            return lbusPaymentHistory;
        }

        public busMSSPaymentHistoryLookup LoadPaymentHistoryHeaders(DataTable adtbSearchResult)
        {
            busMSSPaymentHistoryLookup lobjPaymentHistoryHeaderLookup = new busMSSPaymentHistoryLookup();
            lobjPaymentHistoryHeaderLookup.LoadPaymentHistoryHeaders(adtbSearchResult);
            return lobjPaymentHistoryHeaderLookup;
        }

        //PIR 18567 - Previously, srv find method for both wfmMSSPensionPlanMaintenance and wfmMSSPensionPlanContributionSummaryMaintenance
        //are same, made separate method to load just what is required for form  wfmMSSPensionPlanContributionSummaryMaintenance for 
        //performance.
        # region Retirement enrollment
        public busMSSPersonAccountRetirementWeb FindRetirementPlanOverviewEnrolledSummary(int aintPersonID)
        {
            SetWebParameters();
            aintPersonID = aintPersonID != 0 ? aintPersonID : iintPersonID;
            busMSSHome lbusMSSHome = new busMSSHome();
            if (lbusMSSHome.LoadPerson(aintPersonID))
            {
                lbusMSSHome.LoadEnrolledAndEligiblePlans();
                lbusMSSHome.LoadActiveRetirementPlan();
                if (lbusMSSHome.iclbEnrolledPlans.Count() > 0 &&
                    lbusMSSHome.iclbMSSRetirementPersonAccount.Count == 1)
                    lbusMSSHome.ibusRetirementPlan = lbusMSSHome.iclbEnrolledPlans[0];
            }
            if (lbusMSSHome.ibusRetirementPlan.IsNull()) lbusMSSHome.ibusRetirementPlan = new busBenefitPlanWeb();
            busMSSPersonAccountRetirementWeb lbusPersonAccountWeb = new busMSSPersonAccountRetirementWeb() { icdoPersonAccountRetirement = new cdoPersonAccountRetirement() };
            if (lbusPersonAccountWeb.FindPersonAccount(lbusMSSHome.ibusRetirementPlan.iintPersonAccountId))
            {
                if (lbusPersonAccountWeb.FindPersonAccountRetirement(lbusMSSHome.ibusRetirementPlan.iintPersonAccountId))
                {
                    lbusPersonAccountWeb.istrMSSElectionStatus = lbusMSSHome.ibusRetirementPlan.istrScreenDifferentiator;
                    lbusPersonAccountWeb.LoadPerson();
                    lbusPersonAccountWeb.LoadPlan();
                    lbusPersonAccountWeb.icdoPersonAccount.person_employment_dtl_id = lbusMSSHome.ibusRetirementPlan.iintPersonEmploymentDetailid;
                    lbusPersonAccountWeb.LoadPersonEmploymentDetail();
                    //BR- 24 -13 -a - contribution summary
                    lbusPersonAccountWeb.LoadMSSContributionSummaryForRetirementPlans();
                    //BR- 24 -13 -b - benificiaries
                    lbusPersonAccountWeb.LoadMSSBeneficiariesForPlan();
                    //BR- 24 -13 -c - employer details
                    lbusPersonAccountWeb.LoadMSSContributingEmployers();
                    lbusPersonAccountWeb.iintEnrollmentRequestID = lbusMSSHome.ibusRetirementPlan.iintEnrollmentRequestId;
                    if (lbusMSSHome.ibusRetirementPlan.iintEnrollmentRequestId > 0)
                    {
                        lbusPersonAccountWeb.LoadWSSEnrollmentRequestUpdate(lbusMSSHome.ibusRetirementPlan.iintEnrollmentRequestId);
                    }
                    lbusPersonAccountWeb.SetVisibilityNewRequestButtons();
                    lbusPersonAccountWeb.istrIsDCEligible = lbusMSSHome.istrIsDCEligible;
                    lbusPersonAccountWeb.SetDCEnrollmentType();
                    lbusPersonAccountWeb.LoadPersonAccount();
                    lbusPersonAccountWeb.LoadPersonAccountForRetirementPlan();
                }
            }
            if (lbusMSSHome.iclbMSSRetirementPersonAccount.Count() > 0)
                lbusPersonAccountWeb.iblnIsRetirementAccountAvailable = true;
            return lbusPersonAccountWeb;
        }
        /// <summary>
        /// initial method for hyper link click set privious screen values for return back from page itself, even though not in use in current screen
        /// PIR 25920 New Plan DC 2025
        /// </summary>
        /// <param name="aintPersonAccountId"></param>
        /// <param name="aintPersonEmploymentDetailId"></param>
        /// <param name="aintRequestId"></param>
        /// <param name="astrScreenDifferentiator"></param>
        /// <param name="astrIsDcEligible"></param>
        /// <returns></returns>
        public busWssPersonAccountEnrollmentRequest FindRetirementPlanEnrolledInfo(int aintPersonAccountId,
            int aintPersonEmploymentDetailId, int aintRequestId, string astrScreenDifferentiator, string astrIsDcEligible)
        {
            busWssPersonAccountEnrollmentRequest lbusWssPersonAccountEnrollmentRequest = new busWssPersonAccountEnrollmentRequest {icdoWssPersonAccountEnrollmentRequest = new cdoWssPersonAccountEnrollmentRequest(),
                                            ibusPersonAccount = new busPersonAccount(),ibusMSSPersonAccountRetirement = new busPersonAccountRetirement() };
            if (lbusWssPersonAccountEnrollmentRequest.ibusPersonAccount.FindPersonAccount(aintPersonAccountId))
            {
                if (lbusWssPersonAccountEnrollmentRequest.ibusMSSPersonAccountRetirement.FindPersonAccountRetirement(aintPersonAccountId))
                {
                    lbusWssPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.target_person_account_id = aintPersonAccountId;
                    lbusWssPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.person_id = lbusWssPersonAccountEnrollmentRequest.ibusPersonAccount.icdoPersonAccount.person_id;
                    lbusWssPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id = aintPersonEmploymentDetailId;
                    lbusWssPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id = aintRequestId;
                    lbusWssPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.plan_id = lbusWssPersonAccountEnrollmentRequest.ibusPersonAccount.icdoPersonAccount.plan_id;
                    lbusWssPersonAccountEnrollmentRequest.LoadPlan();
                    lbusWssPersonAccountEnrollmentRequest.istrIsDCEligible = astrIsDcEligible;
                    lbusWssPersonAccountEnrollmentRequest.istrScreenDifferentiator = astrScreenDifferentiator;
                    lbusWssPersonAccountEnrollmentRequest.LoadADECAckGenDetails();
                    lbusWssPersonAccountEnrollmentRequest.iblnIsOnlyADECUpdate = true;
                    lbusWssPersonAccountEnrollmentRequest.istrADECAcknowledgementAgreementFlag = "N";
                }
            }
            lbusWssPersonAccountEnrollmentRequest.EvaluateInitialLoadRules();
            return lbusWssPersonAccountEnrollmentRequest;
        }

        public busMSSPersonAccountRetirementWeb FindRetirementPlanOverviewEnrolled(int aintPersonAccountId,
            int aintPersonEmploymentDetailId, int aintRequestId, string astrScreenDifferentiator, string astrIsDcEligible)
        {
            busMSSPersonAccountRetirementWeb lbusPersonAccountWeb = new busMSSPersonAccountRetirementWeb();
            if (lbusPersonAccountWeb.FindPersonAccount(aintPersonAccountId))
            {
                if (lbusPersonAccountWeb.FindPersonAccountRetirement(aintPersonAccountId))
                {
                    lbusPersonAccountWeb.istrMSSElectionStatus = astrScreenDifferentiator;
                    lbusPersonAccountWeb.LoadPerson();
                    lbusPersonAccountWeb.LoadPlan();
                    lbusPersonAccountWeb.LoadMSSHistory(); // PIR 10695
                    lbusPersonAccountWeb.icdoPersonAccount.person_employment_dtl_id = aintPersonEmploymentDetailId;
                    lbusPersonAccountWeb.LoadPersonEmploymentDetail();
                    //BR- 24 -13 -a - contribution summary
                    lbusPersonAccountWeb.LoadMSSContributionSummaryForRetirementPlans();
                    //BR- 24 -13 -b - benificiaries
                    lbusPersonAccountWeb.LoadMSSBeneficiariesForPlan();
                    //BR- 24 -13 -c - employer details
                    lbusPersonAccountWeb.LoadMSSContributingEmployers();
                    lbusPersonAccountWeb.iintEnrollmentRequestID = aintRequestId;
                    if (aintRequestId > 0)
                    {
                        lbusPersonAccountWeb.LoadWSSEnrollmentRequestUpdate(aintRequestId);
                    }
                    lbusPersonAccountWeb.SetVisibilityNewRequestButtons();
                    lbusPersonAccountWeb.istrIsDCEligible = astrIsDcEligible;
                    lbusPersonAccountWeb.SetDCEnrollmentType();
                }
            }
            lbusPersonAccountWeb.LoadPersonAccount();
            lbusPersonAccountWeb.LoadPersonAccountForRetirementPlan();
            lbusPersonAccountWeb.LoadAdditionalContributionPercentage();
            //PIR 18567 - When loading contributions, the system selects all the columns due to which 
            //the contributions collection is getting heavier and causing performance issues and sometimes 
            // out of memory exception, example perslinkid = 9993 (for whom the contributions are 
            // more than 21000), iblnLoadOnlyRequiredFieldsForVSCOrPSC flag is used to only load required columns
            lbusPersonAccountWeb.iblnLoadOnlyRequiredFieldsForVSCOrPSC = true;
            lbusPersonAccountWeb.ibusPersonAccount.iblnLoadOnlyRequiredFieldsForVSCOrPSC = true;
            lbusPersonAccountWeb.ibusPerson.iblnLoadOnlyRequiredFieldsForVSCOrPSC = true;
            //PIR 5500
            if (lbusPersonAccountWeb.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeRetirement)
            {
                if (lbusPersonAccountWeb.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetimentSuspended)
                    lbusPersonAccountWeb.ibusPersonAccount.icdoPersonAccount.plan_participation_status_description = busGlobalFunctions.GetCodeValueDetails(337, busConstant.PlanParticipationStatusRetirementEnrolled).description;
            }
            lbusPersonAccountWeb.LoadRetirementContributionAll();
            //PIR 18567 - All the calculation objects are made from global to local so that these global child objects are not included 
            //in serialization and deserialization processes thereby decreasing significant processing load on remote server (business tier)
            //as well as the webserver, none of these global child objects' properties are shown on the screen
            lbusPersonAccountWeb.CalculateRetirementBenefitAmountForMSS();
            lbusPersonAccountWeb.LoadTotalVSC();
            lbusPersonAccountWeb.LoadTotalPSC(); // PIR ID 2187 - Earlier, in Calculation method loads the PSC as of Retirement date
            lbusPersonAccountWeb.CalculateERVestedPercentage();
            return lbusPersonAccountWeb;
        }

        public busWssPersonAccountEnrollmentRequest FindPensionPlanRetirementEnrollment(int aintRequestId)
        {
            busWssPersonAccountEnrollmentRequest lbusEnrollmentRequest = new busWssPersonAccountEnrollmentRequest();
            if (lbusEnrollmentRequest.FindWssPersonAccountEnrollmentRequest(aintRequestId))
            {
                lbusEnrollmentRequest.LoadPerson();
                lbusEnrollmentRequest.LoadPlan();
                lbusEnrollmentRequest.LoadPersonEmploymentDetail();
                lbusEnrollmentRequest.LoadPersonAccountEmploymentDetail();
                lbusEnrollmentRequest.LoadPersonAccountForPosting();
                lbusEnrollmentRequest.SetDCEligibleFlag();
                lbusEnrollmentRequest.iclcEeAcknowledgement = new utlCollection<cdoWssPersonAccountEeAcknowledgement>();
                lbusEnrollmentRequest.SetJobClassValue();
                lbusEnrollmentRequest.LoadAckMemAuthDetails();
                lbusEnrollmentRequest.LoadAckNoticekDetails();
                lbusEnrollmentRequest.LoadPreviousAdditionalContributionPercent();
                //wfmDefault.aspx file code conversion - btn_OpenPDF method 
                if (iobjPassInfo.istrFormName == "wfmMSSPensionPlanRetirementEnrollmentMaintenance")
                {
                    DataTable ldtbPathData = utlPassInfo.iobjPassInfo.isrvDBCache.GetCacheData("sgs_system_paths", "path_code = 'CorrPdf'");
                    lbusEnrollmentRequest.istrDownloadFileName = ldtbPathData.Rows[0]["path_value"].ToString() + "SFN-02560.pdf";
                }
            }
            return lbusEnrollmentRequest;
        }
        public busWssPersonAccountEnrollmentRequest NewPensionPlanRetirementEnrollment(int aintPlanId, int aintPersonId, int aintPersonEmploymentDetailId)
        {
            //FMUpgrade: Changes for set Navigation parameter from menu item click
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmMSSPensionPlanRetirementEnrollmentMaintenance")
            {
                aintPersonId = aintPersonId != 0 ? aintPersonId : iintPersonID;
            }
            busWssPersonAccountEnrollmentRequest lbusEnrollmentRequest = new busWssPersonAccountEnrollmentRequest
            {
                icdoWssPersonAccountEnrollmentRequest = new cdoWssPersonAccountEnrollmentRequest()
            };
            lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.person_id = aintPersonId;
            lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.plan_id = aintPlanId;
            lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id = aintPersonEmploymentDetailId;
            lbusEnrollmentRequest.LoadPerson();
            lbusEnrollmentRequest.LoadPlan();
            lbusEnrollmentRequest.LoadPersonEmploymentDetail();
            lbusEnrollmentRequest.LoadPersonAccountEmploymentDetail();
            lbusEnrollmentRequest.LoadPersonAccountForPosting();
            lbusEnrollmentRequest.iclcEeAcknowledgement = new utlCollection<cdoWssPersonAccountEeAcknowledgement>();
            lbusEnrollmentRequest.SetJobClassValue();
            lbusEnrollmentRequest.SetDCEligibleFlag();
            lbusEnrollmentRequest.LoadPreviousAdditionalContributionPercent();
            lbusEnrollmentRequest.EvaluateInitialLoadRules();
            lbusEnrollmentRequest.LoadAckNoticekDetails();
            lbusEnrollmentRequest.LoadAckMemAuthDetails();
            //wfmDefault.aspx file code conversion - btn_OpenPDF method 
            if (iobjPassInfo.istrFormName == "wfmMSSPensionPlanRetirementEnrollmentMaintenance")
            {
                DataTable ldtbPathData = utlPassInfo.iobjPassInfo.isrvDBCache.GetCacheData("sgs_system_paths", "path_code = 'CorrPdf'");
                lbusEnrollmentRequest.istrDownloadFileName = ldtbPathData.Rows[0]["path_value"].ToString() + "SFN-02560.pdf";
            }
            return lbusEnrollmentRequest;
        }
        public busWssPersonAccountEnrollmentRequest FindViewRequestPensionPlanRetirementEnrollment(int aintRequestId)
        {
            busWssPersonAccountEnrollmentRequest lbusEnrollmentRequest = new busWssPersonAccountEnrollmentRequest();
            if (lbusEnrollmentRequest.FindWssPersonAccountEnrollmentRequest(aintRequestId))
            {
                lbusEnrollmentRequest.LoadPerson();
                lbusEnrollmentRequest.LoadPlan();
                lbusEnrollmentRequest.LoadPersonEmploymentDetail();
                lbusEnrollmentRequest.ibusPersonEmploymentDetail.LoadPersonEmployment();
                lbusEnrollmentRequest.ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
                lbusEnrollmentRequest.LoadPersonAccountEmploymentDetail();
                lbusEnrollmentRequest.LoadPersonAccount();
                lbusEnrollmentRequest.SetDCEligibleFlag();
                lbusEnrollmentRequest.iclcEeAcknowledgement = new utlCollection<cdoWssPersonAccountEeAcknowledgement>();
                lbusEnrollmentRequest.SetJobClassValue();
                lbusEnrollmentRequest.LoadAckDetailsForView();
                lbusEnrollmentRequest.LoadAdditionalEEContribution();       //PIR 25920 DC 2025 changes
            }
            return lbusEnrollmentRequest;
        }
        //public busWssPersonAccountEnrollmentRequest FindPensionPlanDCRetirementEnrollment(int aintRequestId)
        //{
        //    busWssPersonAccountEnrollmentRequest lbusEnrollmentRequest = new busWssPersonAccountEnrollmentRequest();
        //    if (lbusEnrollmentRequest.FindWssPersonAccountEnrollmentRequest(aintRequestId))
        //    {
        //        lbusEnrollmentRequest.LoadPerson();
        //        lbusEnrollmentRequest.LoadPlan();
        //        lbusEnrollmentRequest.LoadPersonEmploymentDetail();
        //        lbusEnrollmentRequest.LoadPersonAccountEmploymentDetail();
        //        lbusEnrollmentRequest.LoadPersonAccount();
        //        lbusEnrollmentRequest.SetJobClassCareerAndtechEdCertifiedTeacher();
        //        lbusEnrollmentRequest.SetJobClassDeptOfPublicInstructionCertifiedTeacher();
        //    }
        //    return lbusEnrollmentRequest;
        //}

        public busWssPersonAccountEnrollmentRequest NewPensionPlanDCRetirementEnrollment(int aintPlanId, int aintPersonId, int aintPersonEmploymentDetailId)
        {
            //FMUpgrade: Changes for set Navigation parameter from menu item click
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmMSSPensionPlanDCRetirementEnrollmentMaintenance")
            {
                aintPersonId = aintPersonId != 0 ? aintPersonId : iintPersonID;
            }
            busWssPersonAccountEnrollmentRequest lbusEnrollmentRequest = new busWssPersonAccountEnrollmentRequest
            {
                icdoWssPersonAccountEnrollmentRequest = new cdoWssPersonAccountEnrollmentRequest()
            };
            lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.person_id = aintPersonId;
            lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.plan_id = aintPlanId;
            lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id = aintPersonEmploymentDetailId;
            lbusEnrollmentRequest.LoadPerson();
            lbusEnrollmentRequest.LoadPlan();
            lbusEnrollmentRequest.LoadPersonEmploymentDetail();
            lbusEnrollmentRequest.LoadPersonAccountEmploymentDetail();
            lbusEnrollmentRequest.LoadPersonAccountForPosting();
            lbusEnrollmentRequest.iclcEeAcknowledgement = new utlCollection<cdoWssPersonAccountEeAcknowledgement>();
            lbusEnrollmentRequest.SetJobClassValue();
            lbusEnrollmentRequest.SetDCEligibleFlag();
            lbusEnrollmentRequest.EvaluateInitialLoadRules();
            lbusEnrollmentRequest.LoadAckNoticekDetails();
            lbusEnrollmentRequest.LoadAckMemAuthDetails();
            return lbusEnrollmentRequest;
        }

        public busWssPersonAccountEnrollmentRequest FindViewRequestPensionPlanDCRetirementEnrollment(int aintRequestId)
        {
            busWssPersonAccountEnrollmentRequest lbusEnrollmentRequest = new busWssPersonAccountEnrollmentRequest();
            if (lbusEnrollmentRequest.FindWssPersonAccountEnrollmentRequest(aintRequestId))
            {
                lbusEnrollmentRequest.LoadPerson();
                lbusEnrollmentRequest.LoadPlan();
                lbusEnrollmentRequest.LoadPersonEmploymentDetail();
                lbusEnrollmentRequest.ibusPersonEmploymentDetail.LoadPersonEmployment();
                lbusEnrollmentRequest.ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
                lbusEnrollmentRequest.LoadPersonAccountEmploymentDetail();
                lbusEnrollmentRequest.LoadPersonAccount();
                lbusEnrollmentRequest.SetJobClassValue();
                lbusEnrollmentRequest.LoadAckMemAuthDetails();
                lbusEnrollmentRequest.LoadAckMemAuthDetails();
            }
            return lbusEnrollmentRequest;
        }

        public busWssPersonAccountEnrollmentRequest FindPensionPlanMainOptionalEnrollment(int aintRequestId)
        {
            busWssPersonAccountEnrollmentRequest lbusEnrollmentRequest = new busWssPersonAccountEnrollmentRequest();
            if (lbusEnrollmentRequest.FindWssPersonAccountEnrollmentRequest(aintRequestId))
            {
                lbusEnrollmentRequest.LoadPlan();
                lbusEnrollmentRequest.LoadPerson();
                lbusEnrollmentRequest.LoadPersonAccountEmploymentDetail();
                lbusEnrollmentRequest.LoadPersonAccountForPosting();
                lbusEnrollmentRequest.LoadEEAcknowledgement();
                lbusEnrollmentRequest.LoadEEAcknowledgementCollection();
                lbusEnrollmentRequest.LoadAckMemAuthDetails();
                lbusEnrollmentRequest.LoadAckNoticekDetails();
                lbusEnrollmentRequest.LoadAckGenDetails();
                lbusEnrollmentRequest.LoadPreviousAdditionalContributionPercent();
            }
            return lbusEnrollmentRequest;
        }

        public busWssPersonAccountEnrollmentRequest NewPensionPlanMainOptionalEnrollment(int aintPlanId, int aintPersonId, int aintPersonEmploymentDetailId)
        {
            //FMUpgrade: Changes for set Navigation parameter from menu item click
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmMSSPensionPlanMainRetirementOptionalEnrollmentWizard")
            {
                aintPersonId = aintPersonId != 0 ? aintPersonId : iintPersonID;
            }
            busWssPersonAccountEnrollmentRequest lbusEnrollmentRequest = new busWssPersonAccountEnrollmentRequest
            {
                icdoWssPersonAccountEnrollmentRequest = new cdoWssPersonAccountEnrollmentRequest()
            };
            lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.person_id = aintPersonId;
            lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.plan_id = aintPlanId;
            lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id = aintPersonEmploymentDetailId;
            lbusEnrollmentRequest.LoadPerson();
            lbusEnrollmentRequest.LoadPlan();
            lbusEnrollmentRequest.LoadPersonEmploymentDetail();
            lbusEnrollmentRequest.LoadPersonAccountEmploymentDetail();
            lbusEnrollmentRequest.LoadPersonAccountForPosting();
            lbusEnrollmentRequest.iclcEeAcknowledgement = new utlCollection<cdoWssPersonAccountEeAcknowledgement>();
            lbusEnrollmentRequest.LoadEEAcknowledgementCollection();
            lbusEnrollmentRequest.LoadAckMemAuthDetails();
            lbusEnrollmentRequest.LoadAckNoticekDetails();
            lbusEnrollmentRequest.LoadAckGenDetails();
            lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.acknolwedgement_part_c_date = busGlobalFunctions.GetSysManagementBatchDate();
            lbusEnrollmentRequest.LoadPreviousAdditionalContributionPercent();
            lbusEnrollmentRequest.EvaluateInitialLoadRules();
            return lbusEnrollmentRequest;
        }
        public busWssPersonAccountEnrollmentRequest FindViewRequestPensionPlanMainOptionalEnrollment(int aintRequestId)
        {
            busWssPersonAccountEnrollmentRequest lbusEnrollmentRequest = new busWssPersonAccountEnrollmentRequest();
            if (lbusEnrollmentRequest.FindWssPersonAccountEnrollmentRequest(aintRequestId))
            {
                lbusEnrollmentRequest.LoadPlan();
                lbusEnrollmentRequest.LoadPerson();
                lbusEnrollmentRequest.LoadPersonAccountEmploymentDetail();
                lbusEnrollmentRequest.LoadPersonEmploymentDetail();
                lbusEnrollmentRequest.ibusPersonEmploymentDetail.LoadPersonEmployment();
                lbusEnrollmentRequest.ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
                lbusEnrollmentRequest.LoadPersonAccountForPosting();
                lbusEnrollmentRequest.LoadEEAcknowledgement();
                lbusEnrollmentRequest.LoadEEAcknowledgementCollection();
                lbusEnrollmentRequest.LoadAckDetailsForDBODCOView();
                lbusEnrollmentRequest.LoadAdditionalEEContribution();       //PIR 25920 DC 2025 changes
            }
            return lbusEnrollmentRequest;
        }
        public busWssPersonAccountEnrollmentRequest FindPensionPlanDCOptionalEnrollment(int aintRequestId)
        {
            busWssPersonAccountEnrollmentRequest lbusEnrollmentRequest = new busWssPersonAccountEnrollmentRequest();
            if (lbusEnrollmentRequest.FindWssPersonAccountEnrollmentRequest(aintRequestId))
            {
                lbusEnrollmentRequest.LoadPlan();
                lbusEnrollmentRequest.LoadPerson();
                lbusEnrollmentRequest.LoadPersonEmploymentDetail();
                lbusEnrollmentRequest.LoadPersonAccountEmploymentDetail();
                lbusEnrollmentRequest.LoadPersonAccountForPosting();
                lbusEnrollmentRequest.LoadEEAcknowledgement();
                lbusEnrollmentRequest.LoadEEAcknowledgementCollection();
                lbusEnrollmentRequest.LoadAckMemAuthDetails();
                lbusEnrollmentRequest.LoadAckNoticekDetails();
                lbusEnrollmentRequest.LoadAckGenDetails();
            }
            return lbusEnrollmentRequest;
        }
        public busWssPersonAccountEnrollmentRequest NewPensionPlanDCOptionalEnrollment(int aintPlanId, int aintPersonId, int aintPersonEmploymentDetailId)
        {
            busWssPersonAccountEnrollmentRequest lbusEnrollmentRequest = new busWssPersonAccountEnrollmentRequest
            {
                icdoWssPersonAccountEnrollmentRequest = new cdoWssPersonAccountEnrollmentRequest()
            };
            lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.person_id = aintPersonId;
            lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.plan_id = aintPlanId;
            lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id = aintPersonEmploymentDetailId;
            lbusEnrollmentRequest.LoadPerson();
            lbusEnrollmentRequest.LoadPlan();
            lbusEnrollmentRequest.LoadPersonEmploymentDetail();
            lbusEnrollmentRequest.LoadPersonAccountEmploymentDetail();
            lbusEnrollmentRequest.LoadPersonAccountForPosting();
            lbusEnrollmentRequest.iclcEeAcknowledgement = new utlCollection<cdoWssPersonAccountEeAcknowledgement>();
            lbusEnrollmentRequest.LoadEEAcknowledgementCollection();
            lbusEnrollmentRequest.LoadAckMemAuthDetails();
            lbusEnrollmentRequest.LoadAckNoticekDetails();
            lbusEnrollmentRequest.LoadAckGenDetails();
            lbusEnrollmentRequest.EvaluateInitialLoadRules();
            return lbusEnrollmentRequest;
        }
        public busWssPersonAccountEnrollmentRequest FindViewRequestPensionPlanDCOptionalEnrollment(int aintRequestId)
        {
            busWssPersonAccountEnrollmentRequest lbusEnrollmentRequest = new busWssPersonAccountEnrollmentRequest();
            if (lbusEnrollmentRequest.FindWssPersonAccountEnrollmentRequest(aintRequestId))
            {
                lbusEnrollmentRequest.LoadPlan();
                lbusEnrollmentRequest.LoadPerson();
                lbusEnrollmentRequest.LoadPersonEmploymentDetail();
                lbusEnrollmentRequest.ibusPersonEmploymentDetail.LoadPersonEmployment();
                lbusEnrollmentRequest.ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
                lbusEnrollmentRequest.LoadPersonAccountEmploymentDetail();
                lbusEnrollmentRequest.LoadPersonAccountForPosting();
                lbusEnrollmentRequest.LoadEEAcknowledgementCollection();
                lbusEnrollmentRequest.LoadAckDetailsForDBODCOView();
            }
            return lbusEnrollmentRequest;
        }
        public busWssPersonAccountEnrollmentRequest FindMSSBenefitPlanDBElectedOfficial(int aintRequestId)
        {
            busWssPersonAccountEnrollmentRequest lbusEnrollmentRequest = new busWssPersonAccountEnrollmentRequest();
            if (lbusEnrollmentRequest.FindWssPersonAccountEnrollmentRequest(aintRequestId))
            {
                lbusEnrollmentRequest.LoadPlan();
                lbusEnrollmentRequest.LoadPerson();
                lbusEnrollmentRequest.LoadPersonEmploymentDetail();
                lbusEnrollmentRequest.LoadPersonAccountEmploymentDetail();
                lbusEnrollmentRequest.LoadPersonAccountForPosting();
                lbusEnrollmentRequest.LoadAckMemAuthDetails();
                lbusEnrollmentRequest.LoadAckWaiverAuthDetails();
                lbusEnrollmentRequest.LoadSFN53405PartBWaiverAuthorization();
                lbusEnrollmentRequest.iclcEeAcknowledgement = new utlCollection<cdoWssPersonAccountEeAcknowledgement>();
            }
            return lbusEnrollmentRequest;
        }
        public busWssPersonAccountEnrollmentRequest NewMSSBenefitPlanDBElectedOfficial(int aintPlanId, int aintPersonId, int aintPersonEmploymentDetailId)
        {
            //FMUpgrade: Changes for set Navigation parameter from menu item click
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmMSSDBElectedOfficialMaintenance")
            {
                aintPersonId = aintPersonId != 0 ? aintPersonId : iintPersonID;
            }
            busWssPersonAccountEnrollmentRequest lbusEnrollmentRequest = new busWssPersonAccountEnrollmentRequest
            {
                icdoWssPersonAccountEnrollmentRequest = new cdoWssPersonAccountEnrollmentRequest()
            };
            lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.person_id = aintPersonId;
            lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.plan_id = aintPlanId;
            lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id = aintPersonEmploymentDetailId;
            lbusEnrollmentRequest.LoadPerson();
            lbusEnrollmentRequest.LoadPlan();
            lbusEnrollmentRequest.LoadPersonEmploymentDetail();
            lbusEnrollmentRequest.LoadPersonAccountEmploymentDetail();
            lbusEnrollmentRequest.LoadPersonAccountForPosting();
            lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.status_value = busConstant.EnrollRequestStatusPendingRequest;
            lbusEnrollmentRequest.LoadAckMemAuthDetails();
            lbusEnrollmentRequest.LoadAckWaiverAuthDetails();
            lbusEnrollmentRequest.LoadSFN53405PartBWaiverAuthorization();
            lbusEnrollmentRequest.iclcEeAcknowledgement = new utlCollection<cdoWssPersonAccountEeAcknowledgement>();
            lbusEnrollmentRequest.EvaluateInitialLoadRules();
            //F/W Upgrade PIR 11863 - Record Not Saved When No changes made on the screen, manually setting object state and adding to change log
            lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.ienuObjectState = ObjectState.Insert;
            lbusEnrollmentRequest.iarrChangeLog.Add(lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest);

            return lbusEnrollmentRequest;
        }

        public busWssPersonAccountEnrollmentRequest FindViewRequestMSSBenefitPlanDBElectedOfficial(int aintRequestId)
        {
            busWssPersonAccountEnrollmentRequest lbusEnrollmentRequest = new busWssPersonAccountEnrollmentRequest();
            if (lbusEnrollmentRequest.FindWssPersonAccountEnrollmentRequest(aintRequestId))
            {
                lbusEnrollmentRequest.LoadPlan();
                lbusEnrollmentRequest.LoadPerson();
                lbusEnrollmentRequest.LoadPersonEmploymentDetail();
                lbusEnrollmentRequest.ibusPersonEmploymentDetail.LoadPersonEmployment();
                lbusEnrollmentRequest.ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
                lbusEnrollmentRequest.LoadPersonAccountEmploymentDetail();
                lbusEnrollmentRequest.LoadPersonAccount();
                lbusEnrollmentRequest.LoadSFN53405PartBWaiverAuthorization();
                lbusEnrollmentRequest.LoadAckDetailsForView();
            }
            return lbusEnrollmentRequest;
        }
        #endregion

        #region Deferred Comp Enrollment
        public busMSSDefCompWeb FindDeferredCompEnrolled(int aintPersonAccountId, int aintPersonEmploymentDetailId)
        {
            busMSSDefCompWeb lobjDeferredComp = new busMSSDefCompWeb();
            if (lobjDeferredComp.FindPersonAccount(aintPersonAccountId))
            {
                if (lobjDeferredComp.FindPersonAccountDeferredComp(aintPersonAccountId))
                {
                    lobjDeferredComp.icdoPersonAccount.person_employment_dtl_id = aintPersonEmploymentDetailId;
                    lobjDeferredComp.LoadPerson();
                    lobjDeferredComp.LoadPlan();
                    lobjDeferredComp.LoadPersonAccountProviders();
                    lobjDeferredComp.LoadPersonAccountProviderInfo();
                    lobjDeferredComp.LoadMSSDefCompProviders();
                    lobjDeferredComp.Set457Limit();
                    lobjDeferredComp.idec457Limit = lobjDeferredComp.idecLimit457;
                }
            }
            return lobjDeferredComp;
        }

        public busMSSDefCompWeb FindDeferredCompWizard(int aintPersonAccountId, int aintPersonEmploymentDetailId)
        {
            busMSSDefCompWeb lobjDeferredComp = new busMSSDefCompWeb();
            if (lobjDeferredComp.FindPersonAccountDeferredComp(aintPersonAccountId))
            {
                if (lobjDeferredComp.FindPersonAccount(aintPersonAccountId))
                {
                    lobjDeferredComp.LoadPerson();
                    lobjDeferredComp.LoadPlan();

                    lobjDeferredComp.icdoPersonAccount.person_employment_dtl_id = aintPersonEmploymentDetailId;

                    lobjDeferredComp.LoadCalculatedStartDate();
                    lobjDeferredComp.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusDefCompEnrolled;
                    lobjDeferredComp.icdoPersonAccount.history_change_date = lobjDeferredComp.idtProviderStartDate;
                    lobjDeferredComp.LoadPersonAccountProviders();
                    lobjDeferredComp.LoadPersonAccountProviderInfo();
                    lobjDeferredComp.LoadExpAcknowledgement();
                    lobjDeferredComp.iblnIsFromPortal = true;
                    lobjDeferredComp.iclbMSSDefCompAcknowledgements = new utlCollection<cdoCodeValue>();
                    lobjDeferredComp.iclbMSSExpDefCompAcknowledgements = new utlCollection<cdoWssAcknowledgement>();
                    //pir 6372
                    lobjDeferredComp.iclbDefCompAcknowledgement = new utlCollection<busWssAcknowledgement>();
                    lobjDeferredComp.iclbDefCompExpAck = new utlCollection<busWssAcknowledgement>();
                    lobjDeferredComp.LoadDefCompAcknowledgementGen();
                    lobjDeferredComp.LoadDefCompAcknowledgementCheck();
                    lobjDeferredComp.ibusPersonEmploymentDetail.LoadAllPersonAccountEmploymentDetails();
                    lobjDeferredComp.ibusPersonEmploymentDetail.iclbPersonAccountEmpDtl = new Collection<busPersonAccountEmploymentDetail>();
                    foreach (busPersonAccountEmploymentDetail lobjPAED in lobjDeferredComp.ibusPersonEmploymentDetail.iclbAllPersonAccountEmpDtl)
                    {
                        if (lobjPAED.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdDeferredCompensation
                            && lobjPAED.icdoPersonAccountEmploymentDetail.person_employment_dtl_id == aintPersonEmploymentDetailId)
                        {
                            lobjPAED.icdoPersonAccountEmploymentDetail.election_value = busConstant.PersonAccountElectionValueEnrolled;
                            lobjDeferredComp.ibusPersonEmploymentDetail.iclbPersonAccountEmpDtl.Add(lobjPAED);
                        }
                    }
                    //pir 7758
                    lobjDeferredComp.icdoPersonAccount.ienuObjectState = ObjectState.Update;
                    lobjDeferredComp.icdoPersonAccountDeferredComp.ienuObjectState = ObjectState.Update;
                    lobjDeferredComp.IsHistoryEntryRequired = true;
                    lobjDeferredComp.iarrChangeLog.Add(lobjDeferredComp.icdoPersonAccountDeferredComp);
                    lobjDeferredComp.LoadPreviousHistory();// ibusHistory = new busPersonAccountDeferredCompHistory { icdoPersonAccountDeferredCompHistory = new cdoPersonAccountDeferredCompHistory() };

                }
            }
            return lobjDeferredComp;
        }

        //PIR 6961
        /// <summary>
        /// To display the acknowledgements in the View Request page
        /// </summary>
        /// <param name="aintRequestId">Enrollment Request ID</param>
        /// <returns></returns>
        public busMSSDefCompWeb FindViewRequestDeferredCompEnrollment(int aintRequestId)
        {
            busMSSDefCompWeb lobjDeferredComp = new busMSSDefCompWeb()
            {
                icdoPersonAccount = new cdoPersonAccount(),
                icdoPersonAccountDeferredComp = new cdoPersonAccountDeferredComp(),
                icdoWssPersonAccountEnrollmentRequest = new cdoWssPersonAccountEnrollmentRequest()
            };
            if (aintRequestId > 0)
            {
                lobjDeferredComp.FindPersonAccountDeferredCompByRequestID(aintRequestId);
                lobjDeferredComp.LoadWSSEnrollmentRequestUpdate(aintRequestId);
                lobjDeferredComp.icdoWssPersonAccountEnrollmentRequest = lobjDeferredComp.ibusWSSPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest;
                if (lobjDeferredComp.icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueWaive)
                {
                    lobjDeferredComp.icdoPersonAccount.person_id = lobjDeferredComp.icdoWssPersonAccountEnrollmentRequest.person_id;
                    lobjDeferredComp.icdoPersonAccount.plan_id = lobjDeferredComp.icdoWssPersonAccountEnrollmentRequest.plan_id;
                    lobjDeferredComp.icdoPersonAccountDeferredComp.person_account_id = lobjDeferredComp.icdoWssPersonAccountEnrollmentRequest.target_person_account_id;
                }
                lobjDeferredComp.icdoPersonAccount.person_employment_dtl_id =
                    lobjDeferredComp.ibusWSSPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id;
                lobjDeferredComp.LoadPerson();
                lobjDeferredComp.LoadPlan();
                lobjDeferredComp.LoadSpecificWssPersonAccountProvidersForAck(lobjDeferredComp.ibusWSSPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id); // PIR 9692
                //lobjDeferredComp.LoadPersonAccountProviderInfo();
                lobjDeferredComp.LoadPersonEmploymentDetail();
                lobjDeferredComp.ibusPersonEmploymentDetail.LoadPersonEmployment();
                lobjDeferredComp.ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
                lobjDeferredComp.LoadCalculatedStartDate();
                lobjDeferredComp.LoadDefCompAckDetailsForView(aintRequestId);
                lobjDeferredComp.idec457Limit = lobjDeferredComp.idecLimit457;
            }
            lobjDeferredComp.LoadDefCompAckDetailsForView(aintRequestId);
            return lobjDeferredComp;
        }

        public busMSSDefCompWeb NewDeferredCompWizard(int aintPlanId, int aintPersonId, int aintPersonEmploymentDetailId)
        {
            //FMUpgrade: Changes for set Navigation parameter from menu item click
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmMSSDeferredCompWizard")
            {
                aintPersonId = aintPersonId != 0 ? aintPersonId : iintPersonID;
            }
            busMSSDefCompWeb lobjDeferredComp = new busMSSDefCompWeb
            {
                icdoPersonAccount = new cdoPersonAccount(),
                icdoPersonAccountDeferredComp = new cdoPersonAccountDeferredComp()
            };
            lobjDeferredComp.iblnIsFromMSS = true;
            lobjDeferredComp.icdoPersonAccount.person_id = aintPersonId;
            lobjDeferredComp.icdoPersonAccount.plan_id = aintPlanId;
            lobjDeferredComp.icdoPersonAccount.person_employment_dtl_id = aintPersonEmploymentDetailId;

            lobjDeferredComp.LoadPerson();
            lobjDeferredComp.LoadPlan();
            lobjDeferredComp.LoadPersonEmploymentDetail();
            lobjDeferredComp.LoadCalculatedStartDate();
            lobjDeferredComp.IsHistoryEntryRequired = true;
            if (lobjDeferredComp.ibusPerson.IsMemberEnrolledInPlan(aintPlanId))
            {
                if (lobjDeferredComp.ibusPerson.icolPersonAccount.IsNull()) lobjDeferredComp.ibusPerson.LoadPersonAccount();
                var lobjDefCompAccount = lobjDeferredComp.ibusPerson.icolPersonAccount.Where(lobj => lobj.icdoPersonAccount.plan_id == aintPlanId).FirstOrDefault();
                if (lobjDefCompAccount.IsNotNull())
                    lobjDeferredComp.icdoPersonAccount = lobjDefCompAccount.icdoPersonAccount;
                lobjDeferredComp.LoadPersonAccount();
                lobjDeferredComp.icdoPersonAccount.person_employment_dtl_id = aintPersonEmploymentDetailId;
                lobjDeferredComp.FindPersonAccountDeferredComp(lobjDeferredComp.icdoPersonAccount.person_account_id);
                lobjDeferredComp.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusDefCompEnrolled;
                lobjDeferredComp.icdoPersonAccount.ienuObjectState = ObjectState.Select;
                lobjDeferredComp.icdoPersonAccountDeferredComp.ienuObjectState = ObjectState.Select;
            }
            else
            {
                lobjDeferredComp.icdoPersonAccount.ienuObjectState = ObjectState.Insert;
                lobjDeferredComp.icdoPersonAccountDeferredComp.ienuObjectState = ObjectState.Insert;
            }
            if (lobjDeferredComp.icdoPersonAccount.start_date == DateTime.MinValue) //PIR 26037
                lobjDeferredComp.icdoPersonAccount.start_date = lobjDeferredComp.idtProviderStartDate;
            lobjDeferredComp.icdoPersonAccount.history_change_date = lobjDeferredComp.idtProviderStartDate;
            lobjDeferredComp.icdoPersonAccount.status_value = busConstant.StatusValid;
            lobjDeferredComp.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusDefCompEnrolled;
            lobjDeferredComp.iarrChangeLog.Add(lobjDeferredComp.icdoPersonAccountDeferredComp);

            //Def Comp Provider
            lobjDeferredComp.ibusDefCompProvider = new busPersonAccountDeferredCompProvider
            {
                icdoPersonAccountDeferredCompProvider = new cdoPersonAccountDeferredCompProvider(),
                ibusPersonAccountDeferredComp = new busPersonAccountDeferredComp
                {
                    icdoPersonAccount = new cdoPersonAccount(),
                    icdoPersonAccountDeferredComp = new cdoPersonAccountDeferredComp(),
                    ibusPerson = new busPerson { icdoPerson = new cdoPerson() },
                    ibusPlan = new busPlan { icdoPlan = new cdoPlan() },
                    iclbEmploymentDetail = new Collection<busPersonEmploymentDetail>(),
                    ibusPersonEmploymentDetail = new busPersonEmploymentDetail { icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail() }
                }
            };
            lobjDeferredComp.ibusDefCompProvider.ibusPersonAccountDeferredComp.ibusPerson = lobjDeferredComp.ibusPerson;
            lobjDeferredComp.ibusDefCompProvider.ibusPersonAccountDeferredComp.ibusPlan = lobjDeferredComp.ibusPlan;
            lobjDeferredComp.ibusDefCompProvider.ibusPersonAccountDeferredComp.icdoPersonAccount.plan_id = lobjDeferredComp.ibusPlan.icdoPlan.plan_id;
            lobjDeferredComp.ibusDefCompProvider.ibusPersonAccountDeferredComp.icdoPersonAccount.start_date = lobjDeferredComp.icdoPersonAccount.start_date;  //PIR 21153 member can enroll or select the provider effective can be effective on or after Employment Start Date
            lobjDeferredComp.ibusDefCompProvider.ibusPersonAccountDeferredComp.ibusPersonEmploymentDetail = lobjDeferredComp.ibusPersonEmploymentDetail;
            lobjDeferredComp.ibusDefCompProvider.ibusPersonAccountDeferredComp.ibusPersonEmploymentDetail.LoadPersonEmployment();
            lobjDeferredComp.ibusDefCompProvider.ibusPersonAccountDeferredComp.ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
            lobjDeferredComp.ibusDefCompProvider.ibusPersonAccountDeferredComp.LoadOrgPlan();
            lobjDeferredComp.ibusDefCompProvider.SetStartDate();
            lobjDeferredComp.ibusDefCompProvider.ibusPersonAccountDeferredComp.iclbEmploymentDetail.Add(lobjDeferredComp.ibusDefCompProvider.ibusPersonAccountDeferredComp.ibusPersonEmploymentDetail);
            lobjDeferredComp.ibusDefCompProvider.ibusPersonEmployment = lobjDeferredComp.ibusDefCompProvider.ibusPersonAccountDeferredComp.ibusPersonEmploymentDetail.ibusPersonEmployment;
            lobjDeferredComp.ibusDefCompProvider.iblnIsFromPortal = true;
            lobjDeferredComp.LoadDeferredCompEmploymentsNew();
            lobjDeferredComp.LoadDeferredCompProvidersNew();

            // Acknowledgements
            lobjDeferredComp.iclbMSSDefCompAcknowledgements = new utlCollection<cdoCodeValue>();
            lobjDeferredComp.iclbDefCompAcknowledgement = new utlCollection<busWssAcknowledgement>();
            lobjDeferredComp.iclbMSSExpDefCompAcknowledgements = new utlCollection<cdoWssAcknowledgement>();
            lobjDeferredComp.iclbDefCompExpAck = new utlCollection<busWssAcknowledgement>();
            lobjDeferredComp.LoadDefCompAcknowledgementGen();
            lobjDeferredComp.LoadDefCompAcknowledgementCheck();
            lobjDeferredComp.LoadAcknowledgement();
            lobjDeferredComp.LoadExpAcknowledgement();

            lobjDeferredComp.icdoPersonAccountDeferredComp.hardship_withdrawal_flag = busConstant.Flag_No;
            lobjDeferredComp.ibusHistory = new busPersonAccountDeferredCompHistory { icdoPersonAccountDeferredCompHistory = new cdoPersonAccountDeferredCompHistory() };

            lobjDeferredComp.ibusPersonEmploymentDetail.LoadAllPersonAccountEmploymentDetails();
            lobjDeferredComp.ibusPersonEmploymentDetail.iclbPersonAccountEmpDtl = new Collection<busPersonAccountEmploymentDetail>();
            foreach (busPersonAccountEmploymentDetail lobjPAED in lobjDeferredComp.ibusPersonEmploymentDetail.iclbAllPersonAccountEmpDtl)
            {
                if (lobjPAED.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdDeferredCompensation
                    && lobjPAED.icdoPersonAccountEmploymentDetail.person_employment_dtl_id == aintPersonEmploymentDetailId)
                {
                    lobjPAED.icdoPersonAccountEmploymentDetail.election_value = busConstant.PersonAccountElectionValueEnrolled;
                    lobjDeferredComp.ibusPersonEmploymentDetail.iclbPersonAccountEmpDtl.Add(lobjPAED);
                }
            }
            lobjDeferredComp.istrIsExpDefCompWaivedChecked = busConstant.Flag_No;
            lobjDeferredComp.iblnIsFromPortal = true;
            lobjDeferredComp.AssignWSSEnrollmentRequest();
            return lobjDeferredComp;
        }

        public busPersonAccountDeferredCompProvider NewDeferredCompProvider(int aintPersonAccount, int aintPersonEmpDtlId)
        {
            busPersonAccountDeferredCompProvider lobjDeferredCompProvider = new busPersonAccountDeferredCompProvider
            {
                icdoPersonAccountDeferredCompProvider = new cdoPersonAccountDeferredCompProvider()
            };
            lobjDeferredCompProvider.istrIsPopupValidation = busConstant.Flag_No;
            lobjDeferredCompProvider.icdoPersonAccountDeferredCompProvider.person_account_id = aintPersonAccount;
            lobjDeferredCompProvider.LoadPersonAccountDeferredComp();
            lobjDeferredCompProvider.ibusPersonAccountDeferredComp.LoadPerson();
            lobjDeferredCompProvider.ibusPersonAccountDeferredComp.LoadPlan();
            lobjDeferredCompProvider.ibusPersonAccountDeferredComp.icdoPersonAccount.person_employment_dtl_id = aintPersonEmpDtlId;
            lobjDeferredCompProvider.ibusPersonAccountDeferredComp.LoadPersonEmploymentDetail();
            lobjDeferredCompProvider.ibusPersonAccountDeferredComp.ibusPersonEmploymentDetail.LoadPersonEmployment();
            lobjDeferredCompProvider.ibusPersonAccountDeferredComp.ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
            lobjDeferredCompProvider.ibusPersonAccountDeferredComp.LoadOrgPlan();
            //pir 6321
            lobjDeferredCompProvider.SetStartDate();
            lobjDeferredCompProvider.iblnIsAddNewProvider = true; // PIR 9692
            lobjDeferredCompProvider.ibusPersonAccountDeferredComp.iclbEmploymentDetail = new Collection<busPersonEmploymentDetail>();
            lobjDeferredCompProvider.ibusPersonAccountDeferredComp.iclbEmploymentDetail.Add(lobjDeferredCompProvider.ibusPersonAccountDeferredComp.ibusPersonEmploymentDetail);
            lobjDeferredCompProvider.ibusPersonEmployment = lobjDeferredCompProvider.ibusPersonAccountDeferredComp.ibusPersonEmploymentDetail.ibusPersonEmployment;
            //lobjDeferredCompProvider.ibusPersonAccountDeferredComp.LoadPersonAccountProviders();
            //lobjDeferredCompProvider.LoadMSSAllProviderDetials();
            lobjDeferredCompProvider.iblnIsFromPortal = true;
            lobjDeferredCompProvider.iclbDefCompAcknowledgement = lobjDeferredCompProvider.LoadAcknowledgement();
            lobjDeferredCompProvider.LoadDeferredCompEmploymentsNew();
            lobjDeferredCompProvider.LoadDeferredCompProvidersNew();
			//PIR 25920 New Plan DC 2025 check provider is available with employer matching check
            //lobjDeferredCompProvider.IsApplyEmployerMatchingContributionAvailbale();
            lobjDeferredCompProvider.EvaluateInitialLoadRules();
            return lobjDeferredCompProvider;
        }

        public busPersonAccountDeferredCompProvider FindDeferredCompProvider(int aintPersonAccountDeferredCompProviderId, int aintPersonEmplDtlId)
        {
            busPersonAccountDeferredCompProvider lobjDeferredCompProvider = new busPersonAccountDeferredCompProvider();
            if (lobjDeferredCompProvider.FindPersonAccountDeferredCompProvider(aintPersonAccountDeferredCompProviderId))
            {

                lobjDeferredCompProvider.LoadPersonAccountDeferredComp();
                lobjDeferredCompProvider.ibusPersonAccountDeferredComp.LoadPersonAccount();
                lobjDeferredCompProvider.ibusPersonAccountDeferredComp.ibusPersonAccount.icdoPersonAccount.person_employment_dtl_id = aintPersonEmplDtlId;
                lobjDeferredCompProvider.ibusPersonAccountDeferredComp.LoadPerson();
                lobjDeferredCompProvider.LoadPersonEmployment();
                lobjDeferredCompProvider.ibusPersonEmployment.LoadPersonEmploymentDetail(); // PIR 11395
                lobjDeferredCompProvider.ibusPersonEmployment.LoadOrganization();
                lobjDeferredCompProvider.LoadProviderAgentOrgContact();
                lobjDeferredCompProvider.ibusProviderAgentOrgContact.LoadContact();
                lobjDeferredCompProvider.ibusPersonAccountDeferredComp.LoadOrgPlan(lobjDeferredCompProvider.ibusPersonEmployment.icolPersonEmploymentDetail.FirstOrDefault().icdoPersonEmploymentDetail.start_date); // PIR 11395
                lobjDeferredCompProvider.SetStartDate();// PIR 9692
                lobjDeferredCompProvider.iintStartMonth = lobjDeferredCompProvider.icdoPersonAccountDeferredCompProvider.start_date.Month;// PIR 9692
                lobjDeferredCompProvider.istrStartYear = lobjDeferredCompProvider.icdoPersonAccountDeferredCompProvider.start_date.Year.ToString();// PIR 9692
                lobjDeferredCompProvider.icdoPersonAccountDeferredCompProvider.per_pay_period_contribution_amt = 0;// PIR 9692
                lobjDeferredCompProvider.ibusPersonAccountDeferredComp.iclbEmploymentDetail = new Collection<busPersonEmploymentDetail>();
                lobjDeferredCompProvider.ibusPersonAccountDeferredComp.LoadPersonAccountProviderInfo();
                //lobjDeferredCompProvider.LoadMSSAllProviderDetials();// PIR 9692
                lobjDeferredCompProvider.istrIsPopupValidation = busConstant.Flag_No;
                lobjDeferredCompProvider.iblnIsFromPortal = true;
                lobjDeferredCompProvider.iblnIsUpdate = true; // PIR 9692
                lobjDeferredCompProvider.iclbDefCompAcknowledgement = new utlCollection<cdoWssAcknowledgement>();
                lobjDeferredCompProvider.LoadAcknowledgement();
                lobjDeferredCompProvider.LoadProviderOrgPlan();
                lobjDeferredCompProvider.ibusProviderOrgPlan.LoadOrganization();
                lobjDeferredCompProvider.icdoPersonAccountDeferredCompProvider.istrProviderOrgCode =
                    lobjDeferredCompProvider.ibusProviderOrgPlan.ibusOrganization.icdoOrganization.org_code;
                lobjDeferredCompProvider.icdoPersonAccountDeferredCompProvider.provider_org_plan_id = lobjDeferredCompProvider.ibusProviderOrgPlan.icdoOrgPlan.org_plan_id;
                lobjDeferredCompProvider.LoadAllDeferredCompProviderByProvider();

            }
            return lobjDeferredCompProvider;
        }
        public busMSSPersonAccountDefCompProviderLookup LoadPersons(DataTable adtbSearchResult)
        {
            busMSSPersonAccountDefCompProviderLookup lobjProviderLookup = new busMSSPersonAccountDefCompProviderLookup();
            lobjProviderLookup.LoadMSSProviderContact(adtbSearchResult);
            return lobjProviderLookup;
        }
        #endregion

        # region UCS 24 Insurance
        public busMSSGHDVWeb FindInsurancePlanGHDVEnrolled(int aintPersonAccountId, int aintRequestID, int aintPersonEmploymentDetailId, bool ablnEnrollInHealthAsTemporary = false)
        {
            busMSSGHDVWeb lbusPersonAccountGHDV = new busMSSGHDVWeb();
            if (lbusPersonAccountGHDV.FindPersonAccount(aintPersonAccountId))
            {
                if (lbusPersonAccountGHDV.FindGHDVByPersonAccountID(aintPersonAccountId))
                {
                    lbusPersonAccountGHDV.LoadPerson();
                    lbusPersonAccountGHDV.LoadPlan();
                    lbusPersonAccountGHDV.GetCoverageCodeBasedOnPlans();
                    lbusPersonAccountGHDV.LoadPaymentElection();
                    lbusPersonAccountGHDV.LoadMSSInsuranceDetails();
                    lbusPersonAccountGHDV.LoadMSSContributingEmployers();
                    lbusPersonAccountGHDV.LoadMSSDependent();
                    lbusPersonAccountGHDV.LoadMSSHistoryGhdv(); // PIR 10695
                    lbusPersonAccountGHDV.LoadPersonAccountAchDetail();
                    lbusPersonAccountGHDV.iintEnrollmentRequestId = aintRequestID;
                    lbusPersonAccountGHDV.icdoPersonAccount.person_employment_dtl_id = aintPersonEmploymentDetailId;
                    lbusPersonAccountGHDV.LoadProvider();
                    lbusPersonAccountGHDV.LoadPretaxPayrollDeduction(); // PIR 9778
                    lbusPersonAccountGHDV.iblnEnrollInHealthAsTemporary = ablnEnrollInHealthAsTemporary;
                }
            }
            return lbusPersonAccountGHDV;
        }
        public busWssPersonAccountEnrollmentRequest FindViewRequestInsurancePlanGHDV(int aintRequestId)
        {
            busWssPersonAccountEnrollmentRequest lbusEnrollmentRequest = new busWssPersonAccountEnrollmentRequest();
            if (lbusEnrollmentRequest.FindWssPersonAccountEnrollmentRequest(aintRequestId))
            {
                lbusEnrollmentRequest.LoadPlan();
                lbusEnrollmentRequest.LoadPerson();
                lbusEnrollmentRequest.LoadPersonAccountForPosting();
                lbusEnrollmentRequest.LoadMSSWorkersCompensation();
                lbusEnrollmentRequest.LoadMSSOtherCoverageDetails();
                lbusEnrollmentRequest.LoadDependentsForViewRequest();
                lbusEnrollmentRequest.LoadMSSGHDV();
                lbusEnrollmentRequest.LoadPersonAccountGHDV();
                lbusEnrollmentRequest.ibusMSSPersonAccountGHDV.LoadPersonAccountGHDVHistory();
                //lbusEnrollmentRequest.iclbLatestGHDVHistory = lbusEnrollmentRequest.ibusMSSPersonAccountGHDV.iclbPersonAccountGHDVHistory.Where(i => i.icdoPersonAccountGhdvHistory.end_date.IsNull()).AsEnumerable();
                if (lbusEnrollmentRequest.ibusMSSPersonAccountGHDV.iclbPersonAccountGHDVHistory.IsNotNull())
                {
                    lbusEnrollmentRequest.iclbLatestGHDVHistory = new Collection<busPersonAccountGhdvHistory>();
                    foreach (busPersonAccountGhdvHistory lbusGHDVHistory in lbusEnrollmentRequest.ibusMSSPersonAccountGHDV.iclbPersonAccountGHDVHistory)
                    {
                        if (lbusGHDVHistory.icdoPersonAccountGhdvHistory.end_date == DateTime.MinValue)
                        {
                            lbusEnrollmentRequest.iclbLatestGHDVHistory.Add(lbusGHDVHistory);
                        }
                    }
                }
                lbusEnrollmentRequest.LoadPersonAccountEmploymentDetail();
                lbusEnrollmentRequest.LoadPersonEmploymentDetail();
                lbusEnrollmentRequest.ibusPersonEmploymentDetail.LoadPersonEmployment();
                lbusEnrollmentRequest.ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
                //lbusEnrollmentRequest.ibusMSSPersonAccountGHDV.FindPersonAccount(lbusEnrollmentRequest.ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.person_account_id);
                lbusEnrollmentRequest.ibusMSSPersonAccountGHDV.icdoPersonAccount = lbusEnrollmentRequest.ibusPersonAccount.icdoPersonAccount;
                //prod pir 6367
                lbusEnrollmentRequest.ibusMSSPersonAccountGHDV.icdoPersonAccount.person_employment_dtl_id = lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id;
                lbusEnrollmentRequest.ibusMSSPersonAccountGHDV.LoadPaymentElection();
                lbusEnrollmentRequest.ibusMSSPersonAccountGHDV.LoadRateStructure();
                //PIR 25432 / 25669 If member is currently enrolled in COBRA and enrolls via the wizard in MSS make the COBRA Type Value NULL.
                //For view request posted and process clear cobra values
                if (lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.status_value != busConstant.EnrollRequestStatusPosted &&
                    lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.status_value != busConstant.StatusProcessed)
                    lbusEnrollmentRequest.ClearCobraValues();
                lbusEnrollmentRequest.LoadCoverageCodeDescription();
                lbusEnrollmentRequest.LoadPersonAccountEmploymentDetail();
                lbusEnrollmentRequest.LoadPersonEmploymentDetail();
                lbusEnrollmentRequest.ibusPersonEmploymentDetail.LoadPersonEmployment();
                lbusEnrollmentRequest.ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
                //prod pir 6367
                lbusEnrollmentRequest.ibusMSSPersonAccountGHDV.icdoPersonAccount.person_employment_dtl_id = lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id;
                if (lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.acknolwedgement_part_d_flag != busConstant.Flag_Yes)
                    lbusEnrollmentRequest.LoadGHDVAcknowledgements();
                if (lbusEnrollmentRequest.icdoMSSGDHV.type_of_coverage_value == busConstant.AlternateStructureCodeHDHP)
                {
                    lbusEnrollmentRequest.LoadHDHPAcknowledgementView();
                    lbusEnrollmentRequest.LoadPersonAccountGHDVHsa(); //19997
                }
                if (!string.IsNullOrEmpty(lbusEnrollmentRequest.icdoMSSGDHV.waive_reason))
                {
                    lbusEnrollmentRequest.istrWaiveReason = lbusEnrollmentRequest.icdoMSSGDHV.waive_reason;
                    if (lbusEnrollmentRequest.istrWaiveReason == busConstant.WaiveReasonOther && !string.IsNullOrEmpty(lbusEnrollmentRequest.icdoMSSGDHV.waive_reason_text))
                        lbusEnrollmentRequest.istrWaiveReasonOTHRText = lbusEnrollmentRequest.icdoMSSGDHV.waive_reason_text;
                }
                lbusEnrollmentRequest.LoadAckDetailsForView();
                if ((lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ChangeReasonAnnualEnrollment) &&
                    (lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.status_value == busConstant.EnrollmentStatusPending))         //PIR-15125
                    lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.change_effective_date = lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.date_of_change;
                lbusEnrollmentRequest.istrAcknowledgementFlag = busConstant.Flag_Yes;
            }
            return lbusEnrollmentRequest;
        }

        public busWssPersonAccountEnrollmentRequest NewMSSANNEGHDVWizard(int aintPersonId, int aintPlanId, int aintPersonEmploymentDetailId, int aintRequestId)
        {
            //FMUpgrade: Changes for set Navigation parameter from menu item click
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmMSSGHDVAnnualEnrollmentWizard")
            {
                aintPersonId = aintPersonId != 0 ? aintPersonId : iintPersonID;
            }
            busWssPersonAccountEnrollmentRequest lobjEnrollmentRequest = new busWssPersonAccountEnrollmentRequest
            {
                icdoWssPersonAccountEnrollmentRequest = new cdoWssPersonAccountEnrollmentRequest(),
                ibusMSSGHDVWeb = new busMSSGHDVWeb()
            };
            if (aintRequestId > 0)
            {
                lobjEnrollmentRequest = FindMSSGHDVWizard(aintRequestId);
                lobjEnrollmentRequest.LoadDependents();
            }
            else
                lobjEnrollmentRequest = NewMSSGHDVWizard(aintPersonId, aintPlanId, aintPersonEmploymentDetailId);
            lobjEnrollmentRequest.istrCancelEnrollmentFlag = busConstant.Flag_No;
            if (lobjEnrollmentRequest.ibusPersonAccount.icdoPersonAccount.person_account_id > 0)
            {
                lobjEnrollmentRequest.ibusMSSGHDVWeb = FindInsurancePlanGHDVEnrolled(
                    lobjEnrollmentRequest.ibusPersonAccount.icdoPersonAccount.person_account_id, aintRequestId, aintPersonEmploymentDetailId);
                if (!(lobjEnrollmentRequest.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended) &&
                    !(lobjEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.status_value == busConstant.EnrollRequestStatusPendingRequest)) //PIR 10695 "Save & Quit" functionality // PIR 14889
                {
                    lobjEnrollmentRequest.icdoMSSGDHV.level_of_coverage_value = lobjEnrollmentRequest.ibusMSSGHDVWeb.icdoPersonAccountGhdv.level_of_coverage_value;
                    lobjEnrollmentRequest.icdoMSSGDHV.coverage_code = lobjEnrollmentRequest.ibusMSSGHDVWeb.icdoPersonAccountGhdv.coverage_code;
                    lobjEnrollmentRequest.icdoMSSGDHV.type_of_coverage_value = lobjEnrollmentRequest.ibusMSSGHDVWeb.icdoPersonAccountGhdv.alternate_structure_code_value;// PIR 12008
                }
                lobjEnrollmentRequest.ibusMSSGHDVWeb.LoadCoverageCodeDescription();
                lobjEnrollmentRequest.istrCoverageCodeDescription = lobjEnrollmentRequest.ibusMSSGHDVWeb.istrCoverageCode;
                lobjEnrollmentRequest.ibusMSSGHDVWeb.LoadMSSHistory();
                //Need To call this common method
                //lobjEnrollmentRequest.ClearCobraValues();
                if (lobjEnrollmentRequest.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusInsuranceEnrolled)
                {
                    // In Person Account GHDV clear COBRA Type, COBRA Expiration Date, change Insurance Type (Dental, Vision), remove User Structure Override (Health).
                    if (lobjEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupHealth)
                    {
                        if (lobjEnrollmentRequest.ibusMSSGHDVWeb.icdoPersonAccountGhdv.cobra_type_value.IsNotNullOrEmpty())
                            lobjEnrollmentRequest.ibusMSSGHDVWeb.icdoPersonAccountGhdv.cobra_type_value = string.Empty;
                        if (lobjEnrollmentRequest.ibusMSSGHDVWeb.icdoPersonAccountGhdv.overridden_structure_code.IsNotNullOrEmpty())
                            lobjEnrollmentRequest.ibusMSSGHDVWeb.icdoPersonAccountGhdv.overridden_structure_code = string.Empty;
                        if (lobjEnrollmentRequest.ibusMSSGHDVWeb.icdoPersonAccount.cobra_expiration_date != DateTime.MinValue)
                            lobjEnrollmentRequest.ibusMSSGHDVWeb.icdoPersonAccount.cobra_expiration_date = DateTime.MinValue;
                    }
                    else
                    {
                        if (lobjEnrollmentRequest.ibusMSSGHDVWeb.icdoPersonAccountGhdv.dental_insurance_type_value == busConstant.DentalInsuranceTypeCOBRA)
                            lobjEnrollmentRequest.ibusMSSGHDVWeb.icdoPersonAccountGhdv.dental_insurance_type_value = busConstant.DentalInsuranceTypeActive;
                        if (lobjEnrollmentRequest.ibusMSSGHDVWeb.icdoPersonAccountGhdv.vision_insurance_type_value == busConstant.VisionInsuranceTypeCOBRA)
                            lobjEnrollmentRequest.ibusMSSGHDVWeb.icdoPersonAccountGhdv.vision_insurance_type_value = busConstant.VisionInsuranceTypeActive;
                    }
                    if (lobjEnrollmentRequest.ibusMSSGHDVWeb.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag == busConstant.Flag_Yes)
                    {
                        // Reset IBS Flags
                        lobjEnrollmentRequest.ibusMSSGHDVWeb.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag = busConstant.Flag_No;
                        lobjEnrollmentRequest.ibusMSSGHDVWeb.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id = 0;
                        lobjEnrollmentRequest.ibusMSSGHDVWeb.ibusPaymentElection.icdoPersonAccountPaymentElection.payment_method_value = string.Empty;
                        lobjEnrollmentRequest.ibusMSSGHDVWeb.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date = DateTime.MinValue;
                        lobjEnrollmentRequest.ibusMSSGHDVWeb.ibusPaymentElection.icdoPersonAccountPaymentElection.payee_account_id = 0;
                    }
                }
            }
            // PIR 10087 -  
            else
            {
                lobjEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.is_changes_in_anne_flag = busConstant.Flag_Yes;
            }

            lobjEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.date_of_change = lobjEnrollmentRequest.AnnualEnrollmentEffectiveDate;
            lobjEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value = "ENRL";
            lobjEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.reason_value = busConstant.AnnualEnrollment;
            lobjEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_description = busGlobalFunctions.GetDescriptionByCodeValue(6003, "ENRL", iobjPassInfo);
            lobjEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.reason_description = busGlobalFunctions.GetDescriptionByCodeValue(332, busConstant.AnnualEnrollment, iobjPassInfo);
            lobjEnrollmentRequest.EvaluateInitialLoadRules();
            if (aintRequestId > 0)
            {
                lobjEnrollmentRequest.LoadCoverageCodeDescription();
                lobjEnrollmentRequest.istrCoverageCodeDescription = lobjEnrollmentRequest.ibusMSSPersonAccountGHDV.istrCoverageCode;
                lobjEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.ienuObjectState = ObjectState.Update;
                lobjEnrollmentRequest.icdoMSSGDHV.ienuObjectState = ObjectState.Update;
            }
            else
            {
                lobjEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.ienuObjectState = ObjectState.Insert;
                lobjEnrollmentRequest.icdoMSSGDHV.ienuObjectState = ObjectState.Insert;
            }
            lobjEnrollmentRequest.iarrChangeLog.Add(lobjEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest);
            lobjEnrollmentRequest.iarrChangeLog.Add(lobjEnrollmentRequest.icdoMSSGDHV);
            return lobjEnrollmentRequest;
        }

        public busWssPersonAccountEnrollmentRequest NewMSSGHDVWizard(int aintPersonId, int aintPlanId, int aintPersonEmploymentDetailId, bool ablnIsEnrollInHealthAsTemporary = false, bool ablnIsFromANNETempHealth = false)
        {
            //FMUpgrade: Changes for set Navigation parameter from menu item click
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmMSSGHDVInsuranceEnrollmentWizard")
            {
                aintPersonId = aintPersonId != 0 ? aintPersonId : iintPersonID;
            }
            busWssPersonAccountEnrollmentRequest lbusEnrollmentRequest = new busWssPersonAccountEnrollmentRequest
            {
                icdoWssPersonAccountEnrollmentRequest = new cdoWssPersonAccountEnrollmentRequest()
            };
            lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.plan_id = aintPlanId;
            lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.person_id = aintPersonId;
            lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id = aintPersonEmploymentDetailId;
            lbusEnrollmentRequest.LoadPerson();
            lbusEnrollmentRequest.LoadPlan();
            lbusEnrollmentRequest.LoadPersonEmploymentDetail();
            lbusEnrollmentRequest.ibusPersonEmploymentDetail.LoadPersonEmployment();
            lbusEnrollmentRequest.ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
            lbusEnrollmentRequest.LoadPersonAccountForPosting();
            lbusEnrollmentRequest.ibusPersonAccount.icdoPersonAccount.person_id = aintPersonId;
            lbusEnrollmentRequest.ibusPersonAccount.icdoPersonAccount.plan_id = aintPlanId;
            lbusEnrollmentRequest.ibusPersonAccount.icdoPersonAccount.person_employment_dtl_id = aintPersonEmploymentDetailId;
            lbusEnrollmentRequest.LoadPersonAccountGHDV();
            if (lbusEnrollmentRequest.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary)
                lbusEnrollmentRequest.ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.cobra_type_value = string.Empty;
            lbusEnrollmentRequest.icdoMSSGDHV = new cdoWssPersonAccountGhdv();
            
            if (ablnIsEnrollInHealthAsTemporary)
            {
                lbusEnrollmentRequest.iblnIsEnrollInHealthAsTemporary = ablnIsEnrollInHealthAsTemporary;
                //lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value = "ENRL";
                if (ablnIsFromANNETempHealth)
                {
                    //if (lbusEnrollmentRequest.ibusPersonAccount.icdoPersonAccount.person_account_id <= 0)
                    lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.is_changes_in_anne_flag = busConstant.Flag_Yes;
                    lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.date_of_change = lbusEnrollmentRequest.AnnualEnrollmentTempEffectiveDate;
                    lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.reason_value = busConstant.ChangeReasonACAAnnualEnrollment;
                    lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.reason_description = busGlobalFunctions.GetDescriptionByCodeValue(332, busConstant.ChangeReasonACAAnnualEnrollment, iobjPassInfo);
                }
                else
                {
                    lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.reason_value = busConstant.ChangeReasonACAEligibleTemporary;
                    lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.reason_description = busGlobalFunctions.GetDescriptionByCodeValue(332, busConstant.ChangeReasonACAEligibleTemporary, iobjPassInfo);
                }
               // lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_description = busGlobalFunctions.GetDescriptionByCodeValue(6003, "ENRL", iobjPassInfo);
            }
            // PIR 11840 - 3rd issue
            StackFrame frame = new StackFrame(1);
            if (frame.GetMethod().Name.Contains("ANNE"))
                lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.reason_value = busConstant.ChangeReasonAnnualEnrollment;

            if (lbusEnrollmentRequest.ibusPersonAccount.icdoPersonAccount.person_account_id == 0) //PROD PIR ID 6998
                lbusEnrollmentRequest.LoadChangeEffectiveDate();
            //lbusEnrollmentRequest.ibusMSSPersonAccountGHDV.ibusPaymentElection = new busPersonAccountPaymentElection { icdoPersonAccountPaymentElection = new cdoPersonAccountPaymentElection() };            
            lbusEnrollmentRequest.ibusMSSPersonAccountGHDV.FindPersonAccount(lbusEnrollmentRequest.ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.person_account_id);
            lbusEnrollmentRequest.ibusMSSPersonAccountGHDV.LoadPaymentElection();
            lbusEnrollmentRequest.ibusPersonAccount.ibusPlan = lbusEnrollmentRequest.ibusPlan;
            lbusEnrollmentRequest.LoadMSSWorkersCompensation();
            lbusEnrollmentRequest.LoadMSSOtherCoverageDetails();
            lbusEnrollmentRequest.iclbMSSPersonDependent = new Collection<busWssPersonDependent>();
            lbusEnrollmentRequest.LoadDependents();
            lbusEnrollmentRequest.iclbGHDVAcknowledgement = new Collection<busWssAcknowledgement>();
            lbusEnrollmentRequest.LoadAckCheckDetails();
            lbusEnrollmentRequest.LoadAckGenDetails();
            //lbusEnrollmentRequest.LoadAckMemAuthDetails();//PIR 16533 & 17842
            lbusEnrollmentRequest.LoadHDHPAcknowledgement();
            lbusEnrollmentRequest.LoadAutoPostingCrossRef();
            lbusEnrollmentRequest.LoadMedicareDetails();//pir 7790
            lbusEnrollmentRequest.EvaluateInitialLoadRules();
            //PIR-10856 Start COndition if Person earlier has Suspended Health plan with COBRA type and Insurance Type of Retiree and is enrolling into it again 
            //in that case clear the Cobra TYppe Value , Cobra Expiration Date, IBS Billing flag, IBS Org ID, IBS Effective Date
            lbusEnrollmentRequest.ClearCobraValues();
            lbusEnrollmentRequest.iarrChangeLog.Add(lbusEnrollmentRequest.ibusMSSPersonAccountGHDV.ibusPaymentElection.icdoPersonAccountPaymentElection);
            //PIR-10856 End
            //PIR 16533 & 17842 
            if ((lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.AnnualEnrollment) || (lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.reason_value != busConstant.ChangeReasonAnnualEnrollment &&
                !(lbusEnrollmentRequest.ibusPersonAccount.IsPersonLinkedNLatestEmpDtlGapGreaterThan31Days()))) //PIR 25221
                lbusEnrollmentRequest.LoadPretaxDeductionFlag();
            //19997
            lbusEnrollmentRequest.LoadPersonAccountGHDVHsa();
            DataTable ldtbListdtDTP = iobjPassInfo.isrvDBCache.GetCacheData("sgt_wss_acknowledgement", "ACKNOWLEDGEMENT_ID=148");
            if (ldtbListdtDTP != null && ldtbListdtDTP.Rows.Count > 0)
                lbusEnrollmentRequest.istrAcknowledgementTextForHDHP = ldtbListdtDTP.Rows[0]["acknowledgement_text"].ToString() + "&nbsp&nbsp";
            DataTable ldtbListdtDTP1 = iobjPassInfo.isrvDBCache.GetCacheData("sgt_wss_acknowledgement", "screen_step_value='" + busConstant.ScreenStepValueHSA + "' AND EFFECTIVE_DATE <= '" + DateTime.Now + "'"); //PIR 20986
            var lvarRow = ldtbListdtDTP1.AsEnumerable().OrderByDescending(dr => dr.Field<DateTime>("EFFECTIVE_DATE")).ThenByDescending(dr => dr.Field<int>("DISPLAY_SEQUENCE")).FirstOrDefault();
            if (lvarRow.IsNotNull())
                lbusEnrollmentRequest.istrAcknowledgementTextForHDHP2 = Convert.ToString(lvarRow["acknowledgement_text"]);
            lbusEnrollmentRequest.iblnIsFromPortal = true;
            return lbusEnrollmentRequest;
        }

        //Wizard Expects this
        public void AddNewChild()
        {

        }

        public busWssPersonAccountEnrollmentRequest FindMSSGHDVWizard(int aintRequestId, bool ablnIsEnrollInHealthAsTemporary = false, bool ablnIsFromANNETempHealth = false)
        {
            busWssPersonAccountEnrollmentRequest lbusEnrollmentRequest = new busWssPersonAccountEnrollmentRequest();
            if (lbusEnrollmentRequest.FindWssPersonAccountEnrollmentRequest(aintRequestId))
            {
                lbusEnrollmentRequest.LoadPerson();
                lbusEnrollmentRequest.LoadPlan();
                lbusEnrollmentRequest.LoadPersonAccountForPosting();
                lbusEnrollmentRequest.LoadPersonEmploymentDetail();
                lbusEnrollmentRequest.ibusPersonEmploymentDetail.LoadPersonEmployment();
                lbusEnrollmentRequest.ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
                lbusEnrollmentRequest.LoadMSSGHDV();
                lbusEnrollmentRequest.LoadPersonAccountGHDV();
                lbusEnrollmentRequest.LoadMSSOtherCoverageDetails();
                lbusEnrollmentRequest.LoadMSSWorkersCompensation();
                lbusEnrollmentRequest.iblnIsFromPortal = true;
                lbusEnrollmentRequest.LoadDependentsForViewRequest();
                lbusEnrollmentRequest.LoadDependents(); //Existing issue-Pending request was not posted correctly as it was not loading the iclbMSSDependent Collection in the update mode. iclbMSSDependent was remaining null .
                lbusEnrollmentRequest.ibusPersonAccount.ibusPlan = new busPlan();
                lbusEnrollmentRequest.ibusPersonAccount.ibusPlan = lbusEnrollmentRequest.ibusPlan;
                lbusEnrollmentRequest.ibusPersonAccount.icdoPersonAccount.person_employment_dtl_id = lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id;
                lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.date_of_change = DateTime.MinValue;
                
                if (ablnIsEnrollInHealthAsTemporary)
                {
                    lbusEnrollmentRequest.iblnIsEnrollInHealthAsTemporary = ablnIsEnrollInHealthAsTemporary;
                    //lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value = "ENRL";
                    if (ablnIsFromANNETempHealth)
                    {
                        //if (lbusEnrollmentRequest.ibusPersonAccount.icdoPersonAccount.person_account_id <= 0)
                        lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.is_changes_in_anne_flag = busConstant.Flag_Yes;
                        lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.date_of_change = lbusEnrollmentRequest.AnnualEnrollmentTempEffectiveDate;
                        lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.reason_value = busConstant.ChangeReasonACAAnnualEnrollment;
                        lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.reason_description = busGlobalFunctions.GetDescriptionByCodeValue(332, busConstant.ChangeReasonACAAnnualEnrollment, iobjPassInfo);
                    }
                    else
                    {
                        lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.reason_value = busConstant.ChangeReasonACAEligibleTemporary;
                        lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.reason_description = busGlobalFunctions.GetDescriptionByCodeValue(332, busConstant.ChangeReasonACAEligibleTemporary, iobjPassInfo);
                    }
                    //lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_description = busGlobalFunctions.GetDescriptionByCodeValue(6003, "ENRL", iobjPassInfo);
                }
                if ((lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupHealth) ||
                    (lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdMedicarePartD))
                {
                    lbusEnrollmentRequest.LoadCoverageCodeForMSS();
                }
                else
                {
                    lbusEnrollmentRequest.LoadMSSLevelOfCoverageByPlan();
                }
                lbusEnrollmentRequest.ibusMSSPersonAccountGHDV.icdoPersonAccount = lbusEnrollmentRequest.ibusPersonAccount.icdoPersonAccount;
                lbusEnrollmentRequest.ibusMSSPersonAccountGHDV.LoadPaymentElection();
                lbusEnrollmentRequest.iclbGHDVAcknowledgement = new Collection<busWssAcknowledgement>();
                lbusEnrollmentRequest.LoadHDHPAcknowledgement();
                lbusEnrollmentRequest.LoadAckGenDetails();
                //lbusEnrollmentRequest.LoadAckMemAuthDetails();//PIR 16533 & 17842
                lbusEnrollmentRequest.LoadAckCheckDetails();
                lbusEnrollmentRequest.LoadAutoPostingCrossRef();
                //PIR 16533 & 17842 
                if ((lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.AnnualEnrollment) || (lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.reason_value != busConstant.ChangeReasonAnnualEnrollment &&
                    !(lbusEnrollmentRequest.ibusPersonAccount.IsPersonLinkedNLatestEmpDtlGapGreaterThan31Days()))) //PIR 25221
                    lbusEnrollmentRequest.LoadPretaxDeductionFlag();
                //19997
                lbusEnrollmentRequest.LoadPersonAccountGHDVHsa();
                DataTable ldtbListdtDTP = iobjPassInfo.isrvDBCache.GetCacheData("sgt_wss_acknowledgement", "ACKNOWLEDGEMENT_ID=148");
                if (ldtbListdtDTP != null && ldtbListdtDTP.Rows.Count > 0)
                    lbusEnrollmentRequest.istrAcknowledgementTextForHDHP = ldtbListdtDTP.Rows[0]["acknowledgement_text"].ToString() + "&nbsp";
                DataTable ldtbListdtDTP1 = iobjPassInfo.isrvDBCache.GetCacheData("sgt_wss_acknowledgement", "screen_step_value ='" + busConstant.ScreenStepValueHSA + "' AND EFFECTIVE_DATE <= '" + DateTime.Now + "'"); //PIR 20986
                var lvarRow = ldtbListdtDTP1.AsEnumerable().OrderByDescending(dr => dr.Field<DateTime>("EFFECTIVE_DATE")).ThenByDescending(dr => dr.Field<int>("DISPLAY_SEQUENCE")).FirstOrDefault();
                if (lvarRow.IsNotNull())
                    lbusEnrollmentRequest.istrAcknowledgementTextForHDHP2 = Convert.ToString(lvarRow["acknowledgement_text"]);
                lbusEnrollmentRequest.iblnIsFromPortal = true;
            }
            return lbusEnrollmentRequest;
        }

        public busMSSLifeWeb FindInsurancePlanLifeEnrolled(int aintPersonAccountId, int aintRequestID, int aintPersonEmploymentDetailId)
        {
            busMSSLifeWeb lbusPersonAccountLife = new busMSSLifeWeb();
            if (lbusPersonAccountLife.FindPersonAccount(aintPersonAccountId))
            {
                if (lbusPersonAccountLife.FindPersonAccountLife(aintPersonAccountId))
                {
                    lbusPersonAccountLife.iblnIsFromMSS = true; //PIR 10422
                    lbusPersonAccountLife.LoadPerson();
                    lbusPersonAccountLife.LoadPlan();
                    lbusPersonAccountLife.LoadPaymentElection();
                    lbusPersonAccountLife.LoadMSSInsuranceDetails();
                    //lbusPersonAccountLife.LoadAllPersonEmploymentDetails();
                    lbusPersonAccountLife.LoadPreviousHistoryMSS(lbusPersonAccountLife.icdoPersonAccount.history_change_date); // PIR 10695
                    lbusPersonAccountLife.LoadMSSBeneficiariesForPlan();
                    lbusPersonAccountLife.icdoEnrollmentRequest = new cdoWssPersonAccountEnrollmentRequest();
                    lbusPersonAccountLife.icdoPersonAccount.person_employment_dtl_id = aintPersonEmploymentDetailId;
                    lbusPersonAccountLife.iintEnrollmentRequestID = aintRequestID;
                    lbusPersonAccountLife.LoadLifeOptionData();
                    lbusPersonAccountLife.LoadPersonAccountAchDetail();
                    lbusPersonAccountLife.LoadProvider();
                    if(!lbusPersonAccountLife.IsPersonLinkedNLatestEmpDtlGapGreaterThan31Days()) //PIR 25221
                        lbusPersonAccountLife.LoadPretaxPayrollDeduction(); // PIR 9778

                    // Supplemental amount should be displayed with the addition of Basic coverage amount
                    decimal ldecBasicAmount = 0M;

                    foreach (busPersonAccountLifeOption lobjOption in lbusPersonAccountLife.iclbLifeOption)
                    {
                        lobjOption.icdoPersonAccountLifeOption.effective_start_date = lbusPersonAccountLife.icdoPersonAccount.history_change_date; //PIR 10422
                        if (lobjOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_Basic)
                        {
                            lobjOption.icdoPersonAccountLifeOption.ActualCoverageAmount = lobjOption.icdoPersonAccountLifeOption.coverage_amount; //PIR 10422
                            ldecBasicAmount = lobjOption.icdoPersonAccountLifeOption.coverage_amount;
                        }
                        if (lobjOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_Supplemental &&
                            lobjOption.icdoPersonAccountLifeOption.coverage_amount > 0M)
                        {
                            lobjOption.icdoPersonAccountLifeOption.ActualCoverageAmount = lobjOption.icdoPersonAccountLifeOption.coverage_amount; //PIR 10422
                            lobjOption.icdoPersonAccountLifeOption.coverage_amount += ldecBasicAmount;
                        }
                        //PIR 10422
                        if (lobjOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_DependentSupplemental)
                        {
                            lobjOption.icdoPersonAccountLifeOption.ActualCoverageAmount = lobjOption.icdoPersonAccountLifeOption.coverage_amount;
                        }
                        if (lobjOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_SpouseSupplemental)
                        {
                            lobjOption.icdoPersonAccountLifeOption.ActualCoverageAmount = lobjOption.icdoPersonAccountLifeOption.coverage_amount;
                        }
                    }
                }
            }
            return lbusPersonAccountLife;
        }

        public busWssPersonAccountEnrollmentRequest NewAnnualEnrollmentPlanLifeEnrolledRequest(int aintPlanId, int aintPersonId, int aintPersonEmploymentDetailId, int aintRequestID)
        {
            //FMUpgrade: Changes for set Navigation parameter from menu item click
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmLifeAnnualEnrollmentWizard")
            {
                aintPersonId = aintPersonId != 0 ? aintPersonId : iintPersonID;
            }
            busWssPersonAccountEnrollmentRequest lbusLifeRequest = new busWssPersonAccountEnrollmentRequest
            {
                icdoWssPersonAccountEnrollmentRequest = new cdoWssPersonAccountEnrollmentRequest(),
                ibusMSSLifeWeb = new busMSSLifeWeb()
            };
            if (aintRequestID > 0)
                lbusLifeRequest = FindInsurancePlanLifeEnrolledRequest(aintRequestID);
            else
                lbusLifeRequest = NewInsurancePlanLifeEnrolledRequest(aintPlanId, aintPersonId, aintPersonEmploymentDetailId);
            if (lbusLifeRequest.ibusPersonAccount.icdoPersonAccount.person_account_id > 0)
            {
                lbusLifeRequest.ibusMSSLifeWeb = FindInsurancePlanLifeEnrolled(lbusLifeRequest.ibusPersonAccount.icdoPersonAccount.person_account_id, aintRequestID, aintPersonEmploymentDetailId);
                lbusLifeRequest.ibusMSSLifeWeb.LoadPreviousHistoryForAnnualEnrollment(lbusLifeRequest.AnnualEnrollmentEffectiveDate);
                foreach (busPersonAccountLifeOption lobjOption in lbusLifeRequest.ibusMSSLifeWeb.iclbLifeOption)
                {
                    if (lobjOption.icdoPersonAccountLifeOption.coverage_amount > 0)
                    {
                        lobjOption.icdoPersonAccountLifeOption.effective_start_date = lbusLifeRequest.ibusMSSLifeWeb.icdoPersonAccount.history_change_date; // PIR 10257
                        if (lobjOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_Supplemental)
                        {
                            lbusLifeRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_amount = lobjOption.icdoPersonAccountLifeOption.coverage_amount;
                            lbusLifeRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_waiver_flag = busConstant.Flag_No;
                        }
                        if (lobjOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_DependentSupplemental)
                        {
                            lbusLifeRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_coverage_option_value =
                                Convert.ToString(Convert.ToInt32(lobjOption.icdoPersonAccountLifeOption.coverage_amount));
                            lbusLifeRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_waiver_flag = busConstant.Flag_No;
                        }
                        if (lobjOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_SpouseSupplemental)
                        {
                            lbusLifeRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_amount = lobjOption.icdoPersonAccountLifeOption.coverage_amount;
                            lbusLifeRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_waiver_flag = busConstant.Flag_No;
                        }
                        lbusLifeRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.pre_tax_payroll_deduction = lbusLifeRequest.ibusMSSLifeWeb.icdoPersonAccountLife.premium_conversion_indicator_flag;
                    }
                }
            }
            else
                lbusLifeRequest.icdoWssPersonAccountEnrollmentRequest.is_changes_in_anne_flag = busConstant.Flag_Yes;
            lbusLifeRequest.icdoWssPersonAccountEnrollmentRequest.date_of_change = lbusLifeRequest.AnnualEnrollmentEffectiveDate;
            lbusLifeRequest.icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value = "ENRL";
            lbusLifeRequest.icdoWssPersonAccountEnrollmentRequest.reason_value = busConstant.AnnualEnrollment;
            lbusLifeRequest.icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_description = busGlobalFunctions.GetDescriptionByCodeValue(6003, "ENRL", iobjPassInfo);
            lbusLifeRequest.icdoWssPersonAccountEnrollmentRequest.reason_description = busGlobalFunctions.GetDescriptionByCodeValue(332, busConstant.AnnualEnrollment, iobjPassInfo);
            lbusLifeRequest.istrCancelEnrollmentFlag = busConstant.Flag_No; //Make no changes should take to Review step
            if (lbusLifeRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.wss_person_account_life_option_id > 0)
                lbusLifeRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.ienuObjectState = ObjectState.Update;
            else
                lbusLifeRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.ienuObjectState = ObjectState.Insert;
            lbusLifeRequest.iarrChangeLog.Add(lbusLifeRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption);
            lbusLifeRequest.GetCoverageAmountDetailsForDisplay();
            return lbusLifeRequest;
        }

        public busWssPersonAccountEnrollmentRequest NewInsurancePlanLifeEnrolledRequest(int aintPlanId, int aintPersonId, int aintPersonEmploymentDetailId)
        {
            //FMUpgrade: Changes for set Navigation parameter from menu item click
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmMSSLifeEnrollmentMaintenance" || iobjPassInfo.istrFormName == "wfmLifeEnrollmentWizard")
            {
                aintPersonId = aintPersonId != 0 ? aintPersonId : iintPersonID;
            }
            busWssPersonAccountEnrollmentRequest lbusLifeRequest = new busWssPersonAccountEnrollmentRequest
            {
                icdoWssPersonAccountEnrollmentRequest = new cdoWssPersonAccountEnrollmentRequest(),
                ibusMSSLifeWeb = new busMSSLifeWeb() //PIR-20537
            };
            lbusLifeRequest.icdoWssPersonAccountEnrollmentRequest.person_id = aintPersonId;
            lbusLifeRequest.icdoWssPersonAccountEnrollmentRequest.plan_id = aintPlanId;
            lbusLifeRequest.icdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id = aintPersonEmploymentDetailId;
            lbusLifeRequest.LoadPerson();
            lbusLifeRequest.LoadPlan();
            lbusLifeRequest.LoadPersonAccountForPosting();
            lbusLifeRequest.LoadMSSPersonAccountLife();
            lbusLifeRequest.LoadPersonEmploymentDetail();
            lbusLifeRequest.ibusPersonEmploymentDetail.LoadPersonEmployment();
            lbusLifeRequest.ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
            lbusLifeRequest.LoadPersonAccountEmploymentDetail();
            if (lbusLifeRequest.ibusPersonAccount.icdoPersonAccount.person_account_id == 0)
            {
                //PROD PIR ID 6998
                lbusLifeRequest.LoadChangeEffectiveDate();
                lbusLifeRequest.idecBasicCoverageAmount = lbusLifeRequest.GetCoverageAmountDetails(busConstant.LevelofCoverage_Basic);
            }
            else if (lbusLifeRequest.ibusPersonAccount.icdoPersonAccount.person_account_id > 0)  //PIR-20537
            {
                lbusLifeRequest.ibusMSSLifeWeb = FindInsurancePlanLifeEnrolled(lbusLifeRequest.ibusPersonAccount.icdoPersonAccount.person_account_id, 0, aintPersonEmploymentDetailId);
            }
            lbusLifeRequest.ibusMSSLifeOption = new busWssPersonAccountLifeOption { icdoWssPersonAccountLifeOption = new cdoWssPersonAccountLifeOption() };
            lbusLifeRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.coverage_amount = lbusLifeRequest.idecBasicCoverageAmount;
            lbusLifeRequest.SetVisibilityofLink();
            lbusLifeRequest.LoadExistingLifeInsuranceValues();
            lbusLifeRequest.GetSpouseDetails();
            lbusLifeRequest.LoadAckCheckDetails();
            lbusLifeRequest.SetEnrollmentValues();
            lbusLifeRequest.iclbLifeAcknowledgement = new Collection<busWssAcknowledgement>();
            lbusLifeRequest.EvaluateInitialLoadRules();
            lbusLifeRequest.GetCoverageAmountDetailsForDisplay();
            return lbusLifeRequest;
        }

        public busWssPersonAccountEnrollmentRequest FindInsurancePlanLifeEnrolledRequest(int aintRequestId)
        {
            busWssPersonAccountEnrollmentRequest lbusLifeRequest = new busWssPersonAccountEnrollmentRequest();
            if (lbusLifeRequest.FindWssPersonAccountEnrollmentRequest(aintRequestId))
            {
                lbusLifeRequest.LoadPerson();
                lbusLifeRequest.LoadPlan();
                lbusLifeRequest.LoadPersonAccountForPosting();
                lbusLifeRequest.LoadPersonEmploymentDetail();
                lbusLifeRequest.LoadPersonAccountEmploymentDetail();
                lbusLifeRequest.LoadMSSLifeOptions();
                lbusLifeRequest.SetVisibilityofLink();
                lbusLifeRequest.iclbLifeAcknowledgement = new Collection<busWssAcknowledgement>();
                lbusLifeRequest.LoadAckCheckDetails();
                lbusLifeRequest.LoadExistingLifeInsuranceValues();
                lbusLifeRequest.GetCoverageAmountDetailsForDisplay();
                //lbusLifeRequest.ConfirmationText();               
            }
            return lbusLifeRequest;
        }

        public busWssPersonAccountEnrollmentRequest FindViewRequestInsurancePlanLifeEnrolled(int aintRequestId)
        {
            busWssPersonAccountEnrollmentRequest lbusLifeRequest = new busWssPersonAccountEnrollmentRequest();
            if (lbusLifeRequest.FindWssPersonAccountEnrollmentRequest(aintRequestId))
            {
                lbusLifeRequest.LoadPerson();
                lbusLifeRequest.LoadPlan();
                lbusLifeRequest.LoadPersonAccountForPosting();
                lbusLifeRequest.LoadPersonEmploymentDetail();
                lbusLifeRequest.ibusPersonEmploymentDetail.LoadPersonEmployment();
                lbusLifeRequest.ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
                lbusLifeRequest.LoadPersonAccountEmploymentDetail();
                lbusLifeRequest.LoadMSSPersonAccountLife();
                lbusLifeRequest.ibusMSSPersonAccountLife.LoadHistory();
                if (lbusLifeRequest.ibusMSSPersonAccountLife.iclbPersonAccountLifeHistory.IsNotNull())
                {
                    lbusLifeRequest.iclbLatestLifeHistory = new Collection<busPersonAccountLifeHistory>();
                    if (lbusLifeRequest.ibusMSSPersonAccountLife.iclbPersonAccountLifeHistory.IsNotNull())
                    {
                        lbusLifeRequest.iclbLatestLifeHistory = lbusLifeRequest.ibusMSSPersonAccountLife.iclbPersonAccountLifeHistory.Where(o => busGlobalFunctions.CheckDateOverlapping(DateTime.Now,
                                                                o.icdoPersonAccountLifeHistory.effective_start_date, o.icdoPersonAccountLifeHistory.effective_end_date)).ToList().ToCollection();
                    }
                }
                lbusLifeRequest.LoadMSSLifeOptions();
                lbusLifeRequest.SetVisibilityofLink();
                lbusLifeRequest.iclbLifeAcknowledgement = new Collection<busWssAcknowledgement>();
                lbusLifeRequest.LoadAckDetailsForView();
                //lbusLifeRequest.ConfirmationText();
                if ((lbusLifeRequest.icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ChangeReasonAnnualEnrollment) &&
                    (lbusLifeRequest.icdoWssPersonAccountEnrollmentRequest.status_value == busConstant.EnrollmentStatusPending))         //PIR-15125
                    lbusLifeRequest.icdoWssPersonAccountEnrollmentRequest.change_effective_date = lbusLifeRequest.icdoWssPersonAccountEnrollmentRequest.date_of_change;
                // PIR 18493 App wizard, if request comes from app wizard, the basic amount always calculate as Retiree.
                if(lbusLifeRequest.icdoWssPersonAccountEnrollmentRequest.wss_ben_app_id > 0)
                    lbusLifeRequest.idecBasicCoverageAmount = lbusLifeRequest.GetCoverageAmountDetails(busConstant.LevelofCoverage_Basic,busConstant.LifeInsuranceTypeRetireeMember);
                else
                    lbusLifeRequest.idecBasicCoverageAmount = lbusLifeRequest.GetCoverageAmountDetails(busConstant.LevelofCoverage_Basic);

                decimal adecMinAmount = 0.00M; decimal adecMaxAmount = 0.00M;
                lbusLifeRequest.GetCoverageAmountDetailsSupplemental(ref adecMinAmount, ref adecMaxAmount);
                lbusLifeRequest.idecMinSupplementalLimit = adecMinAmount + lbusLifeRequest.idecBasicCoverageAmount;
                lbusLifeRequest.idecSupplementalLimit = adecMaxAmount + lbusLifeRequest.idecBasicCoverageAmount;
                lbusLifeRequest.idecSpouseSupplementalLimit = lbusLifeRequest.GetCoverageAmountDetails(busConstant.LevelofCoverage_SpouseSupplemental);

                lbusLifeRequest.GetCoverageAmountDetailsForDisplay();
            }
            return lbusLifeRequest;
        }

        public busMssLifeInsurancePremiumReference FindMSSViewLifeInsurancePremium(int aintRequestId)
        {
            busMssLifeInsurancePremiumReference lbusLife = new busMssLifeInsurancePremiumReference();
            lbusLife.LoadLifeInsuranceRate();
            return lbusLife;
        }

        public busWssPersonAccountEnrollmentRequest FindInsurancePlanLTCEnrolledRequest()
        {
            busWssPersonAccountEnrollmentRequest lbusLTCRequest = new busWssPersonAccountEnrollmentRequest();
            //wfmDefault.aspx file code conversion - btn_OpenPDF method 
            if (iobjPassInfo.istrFormName == "wfmMSSLTCEnrollmentMaintenance")
            {
                DataTable ldtbPathData = utlPassInfo.iobjPassInfo.isrvDBCache.GetCacheData("sgs_system_paths", "path_code = 'CorrPdf'");
                lbusLTCRequest.istrDownloadFileName = ldtbPathData.Rows[0]["path_value"].ToString() + "SFN-58925.pdf";
            }
            return lbusLTCRequest;
        }

        public busWssPersonAccountEnrollmentRequest NewInsurancePlanLTCEnrolledRequest(int aintPersonId, int aintPersonEmploymentDetailId)
        {
            busWssPersonAccountEnrollmentRequest lbusPersonAccountRequest = new busWssPersonAccountEnrollmentRequest
            {
                icdoWssPersonAccountEnrollmentRequest = new cdoWssPersonAccountEnrollmentRequest()
            };
            lbusPersonAccountRequest.icdoWssPersonAccountEnrollmentRequest.person_id = aintPersonId;
            lbusPersonAccountRequest.icdoWssPersonAccountEnrollmentRequest.plan_id = busConstant.PlanIdLTC;
            lbusPersonAccountRequest.icdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id = aintPersonEmploymentDetailId;
            lbusPersonAccountRequest.LoadPerson();
            lbusPersonAccountRequest.LoadPlan();
            lbusPersonAccountRequest.LoadPersonEmploymentDetail();
            //wfmDefault.aspx file code conversion - btn_OpenPDF method 
            if (iobjPassInfo.istrFormName == "wfmMSSLTCEnrollmentMaintenance")
            {
                DataTable ldtbPathData = utlPassInfo.iobjPassInfo.isrvDBCache.GetCacheData("sgs_system_paths", "path_code = 'CorrPdf'");
                lbusPersonAccountRequest.istrDownloadFileName = ldtbPathData.Rows[0]["path_value"].ToString() + "SFN-58925.pdf";
            }
            return lbusPersonAccountRequest;
        }

        public busMSSLTCWeb FindInsurancePlanLTC(int aintPersonAccountId, int aintRequestId)
        {
            busMSSLTCWeb lbusPersonAccountLTC = new busMSSLTCWeb();
            if (lbusPersonAccountLTC.FindPersonAccount(aintPersonAccountId))
            {
                lbusPersonAccountLTC.LoadPerson();
                lbusPersonAccountLTC.ibusPerson.LoadSpouse();
                lbusPersonAccountLTC.iintSpousePERSLinkID = lbusPersonAccountLTC.ibusPerson.ibusSpouse.icdoPerson.person_id;
                lbusPersonAccountLTC.LoadPlan();
                lbusPersonAccountLTC.LoadPreviousHistoryMSS(); // PIR 10695
                //lbusPersonAccountLTC.LoadMSSDependent();
                lbusPersonAccountLTC.LoadPaymentElection();
                lbusPersonAccountLTC.LoadMSSInsuranceDetails();
                //lbusPersonAccountLTC.LoadMSSContributingEmployers();
                lbusPersonAccountLTC.LoadPersonAccountAchDetail();
                lbusPersonAccountLTC.LoadEnrolledCoverage();
                lbusPersonAccountLTC.LoadProvider();
                if (lbusPersonAccountLTC.icdoPersonAccount.person_employment_dtl_id != 0)
                {
                    lbusPersonAccountLTC.LoadPersonEmploymentDetail();
                    lbusPersonAccountLTC.ibusPersonEmploymentDetail.LoadPersonEmployment();
                    lbusPersonAccountLTC.LoadOrgPlan();
                    lbusPersonAccountLTC.LoadProviderOrgPlan();
                }
                else
                {
                    lbusPersonAccountLTC.LoadActiveProviderOrgPlan(lbusPersonAccountLTC.idtPlanEffectiveDate);
                }
                lbusPersonAccountLTC.GetMonthlyPremiumAmount();
            }
            return lbusPersonAccountLTC;
        }

        public busPersonAccountEAP FindPersonAccountEAP(int aintPersonAccountId, int aintPersonEmploymentDetailId)
        {
            busPersonAccountEAP lbusPersonAccountEAP = new busPersonAccountEAP();
            if (lbusPersonAccountEAP.FindPersonAccount(aintPersonAccountId))
            {
                lbusPersonAccountEAP.LoadPlanEffectiveDate();
                lbusPersonAccountEAP.LoadPerson();
                lbusPersonAccountEAP.LoadPlan();
                lbusPersonAccountEAP.icdoPersonAccount.person_employment_dtl_id = aintPersonEmploymentDetailId;
                lbusPersonAccountEAP.LoadPersonEmploymentDetail();
                lbusPersonAccountEAP.ibusPersonEmploymentDetail.LoadPersonEmployment();
                lbusPersonAccountEAP.ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();

                lbusPersonAccountEAP.LoadOrgPlan(lbusPersonAccountEAP.idtPlanEffectiveDate);
                lbusPersonAccountEAP.LoadProviderOrgPlanByProviderOrgID(lbusPersonAccountEAP.icdoPersonAccount.provider_org_id, lbusPersonAccountEAP.idtPlanEffectiveDate);

                lbusPersonAccountEAP.iclbEmploymentDetail = new Collection<busPersonEmploymentDetail>();
                lbusPersonAccountEAP.iclbEmploymentDetail.Add(lbusPersonAccountEAP.ibusPersonEmploymentDetail);

                lbusPersonAccountEAP.ibusPersonEmploymentDetail.LoadAllPersonAccountEmploymentDetails();
                lbusPersonAccountEAP.ibusPersonEmploymentDetail.iclbPersonAccountEmpDtl = new Collection<busPersonAccountEmploymentDetail>();
                foreach (busPersonAccountEmploymentDetail lobjPAED in lbusPersonAccountEAP.ibusPersonEmploymentDetail.iclbAllPersonAccountEmpDtl)
                {
                    if (lobjPAED.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdEAP
                        && lobjPAED.icdoPersonAccountEmploymentDetail.person_employment_dtl_id == aintPersonEmploymentDetailId)
                    {
                        lobjPAED.icdoPersonAccountEmploymentDetail.election_value = busConstant.PersonAccountElectionValueEnrolled;
                        lbusPersonAccountEAP.ibusPersonEmploymentDetail.iclbPersonAccountEmpDtl.Add(lobjPAED);
                    }
                }
                lbusPersonAccountEAP.icdoPersonAccount.history_change_date = lbusPersonAccountEAP.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date.GetFirstDayofNextMonth();//PIR 12867
                if (lbusPersonAccountEAP.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusInsuranceEnrolled)
                    lbusPersonAccountEAP.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusInsuranceEnrolled;

                lbusPersonAccountEAP.icdoPersonAccount.ienuObjectState = ObjectState.Update;
                lbusPersonAccountEAP.iarrChangeLog.Add(lbusPersonAccountEAP.icdoPersonAccount);

                lbusPersonAccountEAP.iblnIsFromPortal = true;
                lbusPersonAccountEAP.LoadEapProviders(); //PIR 10269
            }
            return lbusPersonAccountEAP;
        }
        public busPersonAccountEAP NewPersonAccountEAP(int aintPlanId, int aintPersonId, int aintPersonEmploymentDetailId)
        {
            //FMUpgrade: Changes for set Navigation parameter from menu item click
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmMSSEAPEnrollmentMaintenance")
            {
                aintPersonId = aintPersonId != 0 ? aintPersonId : iintPersonID;
            }
            busPersonAccountEAP lbusPersonAccountEAP = new busPersonAccountEAP { icdoPersonAccount = new cdoPersonAccount() };
            lbusPersonAccountEAP.icdoPersonAccount.person_id = aintPersonId;
            lbusPersonAccountEAP.icdoPersonAccount.plan_id = aintPlanId;
            lbusPersonAccountEAP.icdoPersonAccount.person_employment_dtl_id = aintPersonEmploymentDetailId;
            lbusPersonAccountEAP.LoadPerson();
            lbusPersonAccountEAP.LoadPlan();
            //load existing person account
            if (lbusPersonAccountEAP.ibusPerson.icolPersonAccountByBenefitType.IsNull())
                lbusPersonAccountEAP.ibusPerson.LoadPersonAccountByBenefitType(busConstant.PlanBenefitTypeInsurance);

            lbusPersonAccountEAP.LoadPersonEmploymentDetail();
            lbusPersonAccountEAP.ibusPersonEmploymentDetail.LoadPersonEmployment();
            lbusPersonAccountEAP.ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
            lbusPersonAccountEAP.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusInsuranceEnrolled;
            lbusPersonAccountEAP.LoadOrgPlan();
            lbusPersonAccountEAP.icdoPersonAccount.start_date = lbusPersonAccountEAP.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date.GetFirstDayofNextMonth();
            //We are initializing this collection here because we dont have enrolled PA Emp Detail here. (Different logic from Internal Screen)

            lbusPersonAccountEAP.iclbEmploymentDetail = new Collection<busPersonEmploymentDetail>();
            lbusPersonAccountEAP.iclbEmploymentDetail.Add(lbusPersonAccountEAP.ibusPersonEmploymentDetail);

            lbusPersonAccountEAP.ibusPersonEmploymentDetail.LoadAllPersonAccountEmploymentDetails();
            lbusPersonAccountEAP.ibusPersonEmploymentDetail.iclbPersonAccountEmpDtl = new Collection<busPersonAccountEmploymentDetail>();
            foreach (busPersonAccountEmploymentDetail lobjPAED in lbusPersonAccountEAP.ibusPersonEmploymentDetail.iclbAllPersonAccountEmpDtl)
            {
                if (lobjPAED.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdEAP
                    && lobjPAED.icdoPersonAccountEmploymentDetail.person_employment_dtl_id == aintPersonEmploymentDetailId)
                {
                    lobjPAED.icdoPersonAccountEmploymentDetail.election_value = busConstant.PersonAccountElectionValueEnrolled;
                    lbusPersonAccountEAP.ibusPersonEmploymentDetail.iclbPersonAccountEmpDtl.Add(lobjPAED);
                }
            }
            //DONT DO DEFAULTING PROVIDER ORG ID
            //lbusPersonAccountEAP.icdoPersonAccount.provider_org_id = lbusPersonAccountEAP.GetDefaultEAPProvider();
            lbusPersonAccountEAP.icdoPersonAccount.ienuObjectState = ObjectState.Insert;
            lbusPersonAccountEAP.iarrChangeLog.Add(lbusPersonAccountEAP.icdoPersonAccount); //PIR 19373
            lbusPersonAccountEAP.iblnIsFromPortal = true;
            lbusPersonAccountEAP.LoadEapProviders();
            return lbusPersonAccountEAP;
        }

        public busMSSEAPWeb FindInsurancePlanEAPEnrolled(int aintPersonAccountId, int aintRequestId)
        {
            busMSSEAPWeb lbusPersonAccountEAP = new busMSSEAPWeb();
            if (lbusPersonAccountEAP.FindPersonAccount(aintPersonAccountId))
            {
                lbusPersonAccountEAP.LoadPerson();
                lbusPersonAccountEAP.LoadPlan();
                lbusPersonAccountEAP.LoadPaymentElection();
                lbusPersonAccountEAP.LoadMSSDependent();
                lbusPersonAccountEAP.LoadMSSHistory(); // PIR 10695
                lbusPersonAccountEAP.LoadMSSContributingEmployers();
                lbusPersonAccountEAP.LoadMSSInsuranceDetails();
                lbusPersonAccountEAP.iintEnrollmentRequestID = aintRequestId;
                lbusPersonAccountEAP.LoadProvider();
            }
            return lbusPersonAccountEAP;
        }

        //PIR 6961
        /// <summary>
        /// To display the acknowledgemnts in the View Request page
        /// </summary>
        /// <param name="aintRequestId">Enrollment Request ID</param>
        /// <returns></returns>
        public busMSSEAPWeb FindViewRequestEAP(int aintRequestId)
        {
            busMSSEAPWeb lobjEAP = new busMSSEAPWeb();
            lobjEAP.LoadPersonAccountId(aintRequestId);
            lobjEAP.FindPersonAccount(lobjEAP.icdoPersonAccount.person_account_id);
            lobjEAP.LoadPerson();
            lobjEAP.LoadPlan();
            lobjEAP.LoadPersonEmploymentDetail();
            lobjEAP.ibusPersonEmploymentDetail.LoadPersonEmployment();
            lobjEAP.ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
            lobjEAP.LoadPersonAccountForEAP();
            lobjEAP.LoadEAPAckDetailsForView(aintRequestId);
            return lobjEAP;
        }

        public busMSSFlexCompWeb FindInsurancePlanFlexEnrolled(int aintPersonAccountId, int aintRequestId)
        {
            busMSSFlexCompWeb lbusPersonAccountFlexComp = new busMSSFlexCompWeb();
            if (lbusPersonAccountFlexComp.FindPersonAccount(aintPersonAccountId))
            {
                if (lbusPersonAccountFlexComp.FindPersonAccountFlexComp(aintPersonAccountId))
                {
                    lbusPersonAccountFlexComp.LoadPerson();
                    lbusPersonAccountFlexComp.LoadPlan();
                    lbusPersonAccountFlexComp.LoadPreviousHistoryMSS(); // PIR 10695
                    lbusPersonAccountFlexComp.LoadPaymentElection();
                    lbusPersonAccountFlexComp.LoadMSSInsuranceDetails();
                    lbusPersonAccountFlexComp.LoadAllPersonEmploymentDetails();
                    lbusPersonAccountFlexComp.icdoPersonAccount.person_employment_dtl_id = lbusPersonAccountFlexComp.GetEmploymentDetailID(
                                lbusPersonAccountFlexComp.icdoPersonAccount.plan_id, lbusPersonAccountFlexComp.icdoPersonAccount.person_account_id, DateTime.Now, false, true); // PIR 11156
                    lbusPersonAccountFlexComp.LoadMSSAnnualPledgeAmount();
                    lbusPersonAccountFlexComp.LoadFlexCompConversion();
                    lbusPersonAccountFlexComp.iintEnrollmentRequestID = aintRequestId;
                    lbusPersonAccountFlexComp.LoadFlexCompIndividualOptions();
                    lbusPersonAccountFlexComp.LoadPremiumConversionForDentalVisionLife();
                    lbusPersonAccountFlexComp.LoadPersonAccountFlexCompConversion();
                    lbusPersonAccountFlexComp.LoadPretaxPremiumElection();
                }
            }
            return lbusPersonAccountFlexComp;
        }



        public busWssPersonAccountEnrollmentRequest NewInsurancePlanFlexRequest(int aintPlanId, int aintPersonId, int aintPersonEmploymentDetailId)
        {
            //FMUpgrade: Changes for set Navigation parameter from menu item click
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmFlexEnrollmentWizard")
            {
                aintPersonId = aintPersonId != 0 ? aintPersonId : iintPersonID;
            }
            busWssPersonAccountEnrollmentRequest lobjRequest = new busWssPersonAccountEnrollmentRequest
            {
                icdoWssPersonAccountEnrollmentRequest = new cdoWssPersonAccountEnrollmentRequest()
            };
            lobjRequest.icdoWssPersonAccountEnrollmentRequest.person_id = aintPersonId;
            lobjRequest.icdoWssPersonAccountEnrollmentRequest.plan_id = aintPlanId;
            lobjRequest.icdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id = aintPersonEmploymentDetailId;
            lobjRequest.icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value = "ENRL";
            lobjRequest.icdoMSSFlexComp = new cdoWssPersonAccountFlexComp();
            lobjRequest.iclbMSSFlexCompConversion = new utlCollection<busWssPersonAccountFlexCompConversion>(); //PIR-2373
            lobjRequest.icdoMSSFlexCompOption = new cdoWssPersonAccountFlexCompOption();
            lobjRequest.LoadPerson();
            lobjRequest.LoadPlan();
            lobjRequest.LoadPersonAccountForPosting();
            lobjRequest.LoadMSSPersonAccountFlexComp();
            lobjRequest.LoadMSSAnnualPledgeAmount();   //PIR -17035
            lobjRequest.LoadPersonEmploymentDetail();
            lobjRequest.ibusPersonEmploymentDetail.LoadPersonEmployment();
            lobjRequest.ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
            lobjRequest.LoadPersonAccountEmploymentDetail();
            lobjRequest.ibusMSSPersonAccountFlexComp.ibusPerson = lobjRequest.ibusPerson;
            lobjRequest.ibusMSSPersonAccountFlexComp.LoadPremiumConversionForDentalVisionLife();
            lobjRequest.LoadMSSActiveProviders();
            lobjRequest.SetEffectiveStartDate();
            lobjRequest.SetVisbilityToDirectDeposit();
            lobjRequest.SetVisbilityToInsideMail();
            lobjRequest.SetVisibilityToFlexIntroMessage();
            lobjRequest.EvaluateInitialLoadRules();
            lobjRequest.LoadAckMemAuthDetails();
            lobjRequest.LoadNDPERSPlanProviderInfo(); // PIR 10699
            //wfmDefault.aspx file code conversion - btn_OpenPDF method 
            if (iobjPassInfo.istrFormName == "wfmMSSFlexCompEnrollmentMaintenance")
            {
                DataTable ldtbPathData = utlPassInfo.iobjPassInfo.isrvDBCache.GetCacheData("sgs_system_paths", "path_code = 'CorrPdf'");
                lobjRequest.istrDownloadFileName = ldtbPathData.Rows[0]["path_value"].ToString() + "SFN-53852.pdf";
            }
            return lobjRequest;
        }

        public busWssPersonAccountEnrollmentRequest FindInsurancePlanFlexRequest(int aintRequestId)
        {
            busWssPersonAccountEnrollmentRequest lobjRequest = new busWssPersonAccountEnrollmentRequest();
            if (lobjRequest.FindWssPersonAccountEnrollmentRequest(aintRequestId))
            {
                lobjRequest.LoadPerson();
                lobjRequest.LoadPlan();
                lobjRequest.LoadPersonAccountForPosting();
                lobjRequest.LoadMSSPersonAccountFlexComp();
                lobjRequest.LoadMSSPersonAccountFlexComp();
                lobjRequest.LoadPersonEmploymentDetail();
                lobjRequest.LoadPersonAccountEmploymentDetail();
                lobjRequest.LoadMSSFlexComp();
                lobjRequest.LoadMSSFlexCompConversion();
                lobjRequest.LoadMSSActiveProviders();
                lobjRequest.LoadMssFlexCompOption();
                lobjRequest.SetVisbilityToDirectDeposit();
                lobjRequest.SetVisbilityToInsideMail();
                lobjRequest.SetVisibilityToFlexIntroMessage();
                lobjRequest.SetEffectiveStartDate();
                lobjRequest.LoadAckMemAuthDetails();
                lobjRequest.ibusMSSPersonAccountFlexComp.LoadPremiumConversionForDentalVisionLife();
                lobjRequest.LoadNDPERSPlanProviderInfo(); // PIR 10699
                                                          //wfmDefault.aspx file code conversion - btn_OpenPDF method 
                if (iobjPassInfo.istrFormName == "wfmMSSFlexCompEnrollmentMaintenance")
                {
                    DataTable ldtbPathData = utlPassInfo.iobjPassInfo.isrvDBCache.GetCacheData("sgs_system_paths", "path_code = 'CorrPdf'");
                    lobjRequest.istrDownloadFileName = ldtbPathData.Rows[0]["path_value"].ToString() + "SFN-53852.pdf";
                }
            }
            return lobjRequest;
        }

        public busWssPersonAccountEnrollmentRequest FindAnnualEnrollmentFlexRequest(int aintPlanId, int aintPersonId, int aintPersonEmploymentDetailId)
        {
            //FMUpgrade: Changes for set Navigation parameter from menu item click
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmFlexAnnualEnrollmentWizard")
            {
                aintPersonId = aintPersonId != 0 ? aintPersonId : iintPersonID;
            }
            busWssPersonAccountEnrollmentRequest lobjRequest = new busWssPersonAccountEnrollmentRequest();
            lobjRequest.FindWssPersonAccountEnrollmentRequest(aintPlanId, aintPersonId, aintPersonEmploymentDetailId);
            if (lobjRequest.icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.AnnualEnrollment)
            {
                lobjRequest = FindInsurancePlanFlexRequest(lobjRequest.icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id);
                lobjRequest.icdoWssPersonAccountEnrollmentRequest.ienuObjectState = ObjectState.Update;
                lobjRequest.icdoMSSFlexComp.ienuObjectState = ObjectState.Insert;
            }
            else
            {
                lobjRequest = NewInsurancePlanFlexRequest(aintPlanId, aintPersonId, aintPersonEmploymentDetailId);
                lobjRequest.icdoWssPersonAccountEnrollmentRequest.date_of_change = lobjRequest.AnnualEnrollmentEffectiveDate;
                lobjRequest.icdoWssPersonAccountEnrollmentRequest.reason_value = busConstant.AnnualEnrollment;
                lobjRequest.SetEffectiveStartDate();
                lobjRequest.icdoWssPersonAccountEnrollmentRequest.ienuObjectState = ObjectState.Insert;
                lobjRequest.icdoMSSFlexComp.ienuObjectState = ObjectState.Insert;
            }
            lobjRequest.iarrChangeLog.Add(lobjRequest.icdoWssPersonAccountEnrollmentRequest);
            lobjRequest.iarrChangeLog.Add(lobjRequest.icdoMSSFlexComp);
            return lobjRequest;
        }

        public busWssPersonAccountEnrollmentRequest FindViewRequestInsurancePlanFlexRequest(int aintRequestId)
        {
            busWssPersonAccountEnrollmentRequest lobjRequest = new busWssPersonAccountEnrollmentRequest();
            if (lobjRequest.FindWssPersonAccountEnrollmentRequest(aintRequestId))
            {
                lobjRequest.LoadPerson();
                lobjRequest.LoadPlan();
                lobjRequest.LoadPersonAccount();
                lobjRequest.LoadPersonEmploymentDetail();
                lobjRequest.ibusPersonEmploymentDetail.LoadPersonEmployment();
                lobjRequest.ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
                lobjRequest.LoadPersonAccountEmploymentDetail();
                lobjRequest.LoadMSSFlexComp();
                lobjRequest.LoadMSSFlexCompConversion();
                lobjRequest.LoadMssFlexCompOption();
                lobjRequest.LoadAckDetailsForView();
                lobjRequest.LoadMSSPersonAccountFlexComp();
                lobjRequest.ibusMSSPersonAccountFlexComp.LoadModifiedFlexCompHistory();
                if (lobjRequest.ibusMSSPersonAccountFlexComp.iclbModifiedHistory.IsNotNull())
                {
                    lobjRequest.iclbLatestFlexCompHistory = new Collection<busPersonAccountFlexCompHistory>();
                    foreach (busPersonAccountFlexCompHistory lbusFlexCompHistory in lobjRequest.ibusMSSPersonAccountFlexComp.iclbModifiedHistory)
                    {
                        if (lbusFlexCompHistory.icdoPersonAccountFlexCompHistory.effective_end_date == DateTime.MinValue)
                        {
                            lobjRequest.iclbLatestFlexCompHistory.Add(lbusFlexCompHistory);
                        }
                    }
                }
                lobjRequest.ibusMSSPersonAccountFlexComp.ibusPerson = lobjRequest.ibusPerson;
                lobjRequest.ibusMSSPersonAccountFlexComp.LoadPremiumConversionForDentalVisionLife();
                if ((lobjRequest.icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ChangeReasonAnnualEnrollment) &&
                    (lobjRequest.icdoWssPersonAccountEnrollmentRequest.status_value == busConstant.EnrollmentStatusPending))         //PIR-15125
                    lobjRequest.icdoWssPersonAccountEnrollmentRequest.change_effective_date = lobjRequest.icdoWssPersonAccountEnrollmentRequest.date_of_change;
            }
            return lobjRequest;
        }

        //For MSS Layout change
        public busMSSBenefitPlan FindMSSBenefitPlanDetails(int aintPersonAccountId)
        {

            busMSSBenefitPlan lbusMSSBenefitPlan = new busMSSBenefitPlan();
            lbusMSSBenefitPlan.ibusMSSLifeWeb = new busMSSLifeWeb();
            lbusMSSBenefitPlan.ibusMSSGHDVWeb = new busMSSGHDVWeb();
            lbusMSSBenefitPlan.ibusMSSLTCWeb = new busMSSLTCWeb();
            lbusMSSBenefitPlan.ibusMSSMedicarePartDWeb = new busMSSMedicarePartDWeb();

            if (lbusMSSBenefitPlan.FindPersonAccount(aintPersonAccountId))
            {
                lbusMSSBenefitPlan.LoadPerson();
                lbusMSSBenefitPlan.LoadPlan();
                lbusMSSBenefitPlan.LoadPaymentElection();
                lbusMSSBenefitPlan.LoadProvider();
                if (lbusMSSBenefitPlan.icdoPersonAccount.plan_id == busConstant.PlanIdGroupLife)
                {
                    lbusMSSBenefitPlan.ibusMSSLifeWeb.icdoPersonAccount = lbusMSSBenefitPlan.icdoPersonAccount;
                    if (lbusMSSBenefitPlan.ibusMSSLifeWeb.FindPersonAccountLife(aintPersonAccountId))
                    {
                        lbusMSSBenefitPlan.ibusMSSLifeWeb.ibusPaymentElection = lbusMSSBenefitPlan.ibusPaymentElection;
                        lbusMSSBenefitPlan.ibusMSSLifeWeb.LoadMSSInsuranceDetails();
                        lbusMSSBenefitPlan.ibusMSSLifeWeb.LoadMSSBeneficiariesForPlan();
                        lbusMSSBenefitPlan.ibusMSSLifeWeb.LoadMSSLifeOptionData(); //pir 8507
                        lbusMSSBenefitPlan.LoadPersonAccountAchDetail();
                    }
                }
                else if (lbusMSSBenefitPlan.icdoPersonAccount.plan_id == busConstant.PlanIdLTC)
                {
                    // PIR 10143
                    lbusMSSBenefitPlan.ibusMSSLTCWeb.icdoPersonAccount = lbusMSSBenefitPlan.icdoPersonAccount;
                    lbusMSSBenefitPlan.ibusMSSLTCWeb.FindPersonAccount(aintPersonAccountId);
                    lbusMSSBenefitPlan.ibusMSSLTCWeb.ibusPerson = lbusMSSBenefitPlan.ibusPerson;
                    lbusMSSBenefitPlan.ibusMSSLTCWeb.ibusPerson.LoadSpouse();
                    lbusMSSBenefitPlan.ibusMSSLTCWeb.iintSpousePERSLinkID = lbusMSSBenefitPlan.ibusMSSLTCWeb.ibusPerson.ibusSpouse.icdoPerson.person_id;
                    lbusMSSBenefitPlan.ibusMSSLTCWeb.LoadEnrolledCoverage();
                }
                //PIR 15870
                else if (lbusMSSBenefitPlan.icdoPersonAccount.plan_id == busConstant.PlanIdMedicarePartD)
                {
                    lbusMSSBenefitPlan.ibusMSSMedicarePartDWeb.icdoPersonAccount = lbusMSSBenefitPlan.icdoPersonAccount;
                    if (lbusMSSBenefitPlan.ibusMSSMedicarePartDWeb.FindMedicareByPersonAccountID(aintPersonAccountId))
                    {
                        lbusMSSBenefitPlan.ibusMSSMedicarePartDWeb.ibusPaymentElection = lbusMSSBenefitPlan.ibusPaymentElection;
                        lbusMSSBenefitPlan.ibusMSSMedicarePartDWeb.LoadMSSInsuranceDetailsMedicare(lbusMSSBenefitPlan.ibusMSSMedicarePartDWeb.icdoPersonAccountMedicarePartDHistory.member_person_id);
                        lbusMSSBenefitPlan.LoadPersonAccountAchDetail();
                    }
                }
                else
                {
                    lbusMSSBenefitPlan.ibusMSSGHDVWeb.icdoPersonAccount = lbusMSSBenefitPlan.icdoPersonAccount;
                    if (lbusMSSBenefitPlan.ibusMSSGHDVWeb.FindGHDVByPersonAccountID(aintPersonAccountId))
                    {
                        lbusMSSBenefitPlan.GetCoverageCodeBasedOnPlans();
                        lbusMSSBenefitPlan.ibusMSSGHDVWeb.ibusPaymentElection = lbusMSSBenefitPlan.ibusPaymentElection;
                        lbusMSSBenefitPlan.ibusMSSGHDVWeb.LoadMSSInsuranceDetails();
                        lbusMSSBenefitPlan.ibusMSSGHDVWeb.LoadMSSDependent();
                        lbusMSSBenefitPlan.LoadPersonAccountAchDetail();
                    }
                }
            }

            return lbusMSSBenefitPlan;
        }
        # endregion

        //load address details
        public busPersonAddress FindMSSPersonAddress(int aintPersonAddressID)
        {
            busPersonAddress lbusPersonAddress = new busPersonAddress();
            if (lbusPersonAddress.FindPersonAddress(aintPersonAddressID))
            {
                lbusPersonAddress.LoadOtherAddressses();
            }
            return lbusPersonAddress;
        }

        public busPersonAddress NewMSSPersonAddress(int aintPersonID, string astrCurrentCountryCode, bool ablnIsExternalUser = false)
        {
            if (iobjPassInfo.idictParams.ContainsKey("IsExternalLogin"))
                ablnIsExternalUser = Convert.ToBoolean(iobjPassInfo.idictParams["IsExternalLogin"]);
            busPersonAddress lbusPersonAddress = new busPersonAddress { icdoPersonAddress = new cdoPersonAddress() };
            lbusPersonAddress.iblnExternalUser = ablnIsExternalUser;//18492
            lbusPersonAddress.icdoPersonAddress.person_id = aintPersonID;
            lbusPersonAddress.iblnIsFromMSS = true;//For MSS Layout change
            lbusPersonAddress.LoadOtherAddressses();
            lbusPersonAddress.EvaluateInitialLoadRules(); //pir 8622
            lbusPersonAddress.icdoPersonAddress.start_date = DateTime.Now;
            lbusPersonAddress.icdoPersonAddress.addr_country_value = astrCurrentCountryCode;
            lbusPersonAddress.icdoPersonAddress.addr_change_letter_status_flag =  busConstant.AddrChangeLetterNotSent; //PIR 20868
            return lbusPersonAddress;
        }

        public busMSSPersonContact FindMSSPersonContact(int aintPersonContactID, int iintIsReadOnly = 0, bool ablnIsExternalUser = false)
        {
            if (iobjPassInfo.idictParams.ContainsKey("IsExternalLogin"))
                ablnIsExternalUser = Convert.ToBoolean(iobjPassInfo.idictParams["IsExternalLogin"]);
            busMSSPersonContact lbusPersonContact = new busMSSPersonContact();
            if (lbusPersonContact.FindPersonContact(aintPersonContactID))
            {
                lbusPersonContact.LoadPerson();
                lbusPersonContact.LoadContactAddress();
                if (iintIsReadOnly == 1) // PIR 8623
                    lbusPersonContact.iblnIsReadOnly = true;
                lbusPersonContact.LoadMSSContactName();
                lbusPersonContact.iblnExternalUser = ablnIsExternalUser; //18492
            }
            return lbusPersonContact;
        }

        public busMSSPersonContact NewMSSPersonContact(int aintPersonID, bool ablnIsExternalUser = false)
        {
            if (iobjPassInfo.idictParams.ContainsKey("IsExternalLogin"))
                ablnIsExternalUser = Convert.ToBoolean(iobjPassInfo.idictParams["IsExternalLogin"]);
            busMSSPersonContact lobjPersonContact = new busMSSPersonContact
            {
                icdoPersonContact = new cdoPersonContact()
            };
            lobjPersonContact.icdoPersonContact.person_id = aintPersonID;
            //lobjPersonContact.icdoPersonContact.same_as_member_address = busConstant.Flag_No;
            lobjPersonContact.istrSuppressWarning = busConstant.Flag_No;
            lobjPersonContact.LoadPerson();//For MSS Layout change
            //lobjPersonContact.ibusPerson.LoadPersonCurrentAddress();//For MSS Layout change
            lobjPersonContact.iblnExternalUser = ablnIsExternalUser; //18492
            return lobjPersonContact;
        }

        public busPersonDependent FindMSSPersonDependent(int aintPersonDependentID)
        {
            busPersonDependent lbusPersonDependent = new busPersonDependent();
            if (lbusPersonDependent.FindPersonDependent(aintPersonDependentID))
            {
            }
            return lbusPersonDependent;
        }

        public busMaritalStatusChangeWeb NewMSSPersonMaritalChangeWizard(int aintPersonID)
        {
            //FMUpgrade: Changes for set Navigation parameter from menu item click
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmMaritalChangeWizard")
            {
                aintPersonID = aintPersonID != 0 ? aintPersonID : iintPersonID;
            }
            busMaritalStatusChangeWeb lbusMaritalChange = new busMaritalStatusChangeWeb();
            lbusMaritalChange.LoadPerson(aintPersonID);
            lbusMaritalChange.LoadMSSPersonContacts();
            if (lbusMaritalChange.ibusPerson.icdoPerson.ihstOldValues.Count > 0)
            {
                if (!String.IsNullOrEmpty(lbusMaritalChange.ibusPerson.icdoPerson.ihstOldValues["marital_status_value"].ToString()))
                {
                    lbusMaritalChange.istrOldMaritalStatus = lbusMaritalChange.ibusPerson.icdoPerson.ihstOldValues["marital_status_value"].ToString();
                }
                lbusMaritalChange.ibusPerson.icdoPerson.ms_change_date = DateTime.MinValue;
            }
            return lbusMaritalChange;
        }

        public busMSSInsurancePlansWeb FindMSSInsurancePlanEnrollment(int aintPersonID)
        {
            busMSSInsurancePlansWeb lbusInsurancePlans = new busMSSInsurancePlansWeb();
            lbusInsurancePlans.LoadPerson(aintPersonID);
            lbusInsurancePlans.LoadEnrolledEligibleInsurancePlans();
            return lbusInsurancePlans;
        }

        public busServicePurchaseWeb NewServicePurchaseEstimateWeb(int aintPersonID)
        {
            //FMUpgrade: Changes for set Navigation parameter from menu item click
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmServicePurchaseWebMaintenance")
            {
                aintPersonID = aintPersonID != 0 ? aintPersonID : iintPersonID;
            }
            busServicePurchaseWeb lobjServicePurchaseWeb = new busServicePurchaseWeb { icdoServicePurchaseWeb = new cdoServicePurchaseHeader() };
            lobjServicePurchaseWeb.ibusServicePurchaseHeader = new busServicePurchaseHeader
            {
                icdoServicePurchaseHeader = new cdoServicePurchaseHeader(),
                ibusPlan = new busPlan { icdoPlan = new cdoPlan() },
                ibusPrimaryServicePurchaseDetail = new busServicePurchaseDetail
                {
                    icdoServicePurchaseDetail = new cdoServicePurchaseDetail(),
                    ibusServicePurchaseHeader = new busServicePurchaseHeader
                    {
                        icdoServicePurchaseHeader = new cdoServicePurchaseHeader(),
                        ibusPlan = new busPlan { icdoPlan = new cdoPlan() }
                    }
                }
            };
            lobjServicePurchaseWeb.icdoServicePurchaseWeb.person_id = aintPersonID;
            lobjServicePurchaseWeb.icdoServicePurchaseWeb.date_of_purchase = DateTime.Now;
            lobjServicePurchaseWeb.icdoServicePurchaseWeb.action_status_value = busConstant.Service_Purchase_Action_Status_Pending;
            lobjServicePurchaseWeb.icdoServicePurchaseWeb.service_purchase_status_value = busConstant.Service_Purchase_Status_Review;
            lobjServicePurchaseWeb.icdoServicePurchaseWeb.is_created_from_portal = busConstant.Flag_Yes;
            lobjServicePurchaseWeb.ibusServicePurchaseHeader.icdoServicePurchaseHeader = lobjServicePurchaseWeb.icdoServicePurchaseWeb;
            lobjServicePurchaseWeb.ibusServicePurchaseHeader.LoadPerson();

            lobjServicePurchaseWeb.LoadEligiblePlans();
            if (lobjServicePurchaseWeb.iclbEligiblePlan.Count == 1)
            {
                lobjServicePurchaseWeb.icdoServicePurchaseWeb.plan_id = lobjServicePurchaseWeb.iclbEligiblePlan[0].plan_id;
                lobjServicePurchaseWeb.ibusServicePurchaseHeader.icdoServicePurchaseHeader.plan_id = lobjServicePurchaseWeb.icdoServicePurchaseWeb.plan_id;
                lobjServicePurchaseWeb.ibusServicePurchaseHeader.LoadPlan();
            }
            lobjServicePurchaseWeb.LoadConsolidatePurchaseInNewMode();
            //lobjServicePurchaseWeb.ibusServicePurchaseHeader.iintWhatIfNoOfPayments = 180; //F/W Upgrade PIR 11238
            return lobjServicePurchaseWeb;
        }

        public busServicePurchaseAmortizationScheduleWeb FindServicePurchaseAmortizationScheduleWeb(int aintBenefitCalculatorId, string astrIsSickLeave, string astrIsConsolidated)
        {
            busServicePurchaseAmortizationScheduleWeb lbusMSSServicePurchase = new busServicePurchaseAmortizationScheduleWeb { icdoServicePurchaseHeader = new cdoServicePurchaseHeader() };
            lbusMSSServicePurchase.iintBenefitCalculatorId = aintBenefitCalculatorId;
            lbusMSSServicePurchase.LoadBenefitCalculator();
            if (astrIsConsolidated == busConstant.Flag_Yes)
            {
                lbusMSSServicePurchase.istrConsolidatedPurchase = astrIsConsolidated;
                lbusMSSServicePurchase.LoadConsolidatedPurchaseHeader(lbusMSSServicePurchase.ibusBenefitCalculator.icdoWssBenefitCalculator.consolidated_service_purchase_header_id);
                lbusMSSServicePurchase.ibusConsolidatedPurchaseHeader.LoadServicePurchaseDetail();
                lbusMSSServicePurchase.ibusConsolidatedPurchaseHeader.ibusPrimaryServicePurchaseDetail.LoadServicePurchaseDetailConsolidated();
                lbusMSSServicePurchase.ibusConsolidatedPurchaseHeader.LoadAmortizationSchedule();
            }
            if (astrIsSickLeave == busConstant.Flag_Yes)
            {
                lbusMSSServicePurchase.istrUnusedSickLeavePurchase = astrIsSickLeave;
                lbusMSSServicePurchase.LoadSickLeavePurchaseHeader(lbusMSSServicePurchase.ibusBenefitCalculator.icdoWssBenefitCalculator.unused_sick_leave_service_purchase_header_id);
                lbusMSSServicePurchase.ibusSickLeavePurchaseHeader.LoadServicePurchaseDetail();
                lbusMSSServicePurchase.ibusSickLeavePurchaseHeader.LoadAmortizationSchedule();
            }
            return lbusMSSServicePurchase;
        }

        public busServicePurchaseAmortizationScheduleWeb FindServicePurchaseScheduleWeb(int aintHeaderID)
        {
            busServicePurchaseAmortizationScheduleWeb lbusMSSServicePurchase = new busServicePurchaseAmortizationScheduleWeb { icdoServicePurchaseHeader = new cdoServicePurchaseHeader() };
            lbusMSSServicePurchase.LoadPurchaseHeader(aintHeaderID);
            lbusMSSServicePurchase.ibusServicePurchaseHeader.LoadServicePurchaseDetail();
            lbusMSSServicePurchase.ibusServicePurchaseHeader.LoadAmortizationSchedule();
            //lbusMSSServicePurchase.GenerateWhatifSchedule();
            return lbusMSSServicePurchase;
        }

        public busServicePurchaseAmortizationScheduleWeb FindServicePurchaseWhatIfSchedule(int aintHeaderID, int aintNumberofPayments, decimal adecOneTimePaymentAmount,
                                                                            decimal adecExpectedInstallmentAmount, string astrPaymentFrequency)
        {
            busServicePurchaseAmortizationScheduleWeb lbusMSSServicePurchaseWeb = new busServicePurchaseAmortizationScheduleWeb
            {
                ibusServicePurchaseHeader = new busServicePurchaseHeader { icdoServicePurchaseHeader = new cdoServicePurchaseHeader() }
            };
            lbusMSSServicePurchaseWeb.LoadPurchaseHeader(aintHeaderID);
            lbusMSSServicePurchaseWeb.ibusServicePurchaseHeader.LoadServicePurchaseDetail();
            lbusMSSServicePurchaseWeb.ibusServicePurchaseHeader.idecWhatIfPaymentAmount = adecOneTimePaymentAmount;
            lbusMSSServicePurchaseWeb.ibusServicePurchaseHeader.istrWhatIfPaymentFrequency = astrPaymentFrequency;
            lbusMSSServicePurchaseWeb.ibusServicePurchaseHeader.iintWhatIfNoOfPayments = aintNumberofPayments;
            lbusMSSServicePurchaseWeb.ibusServicePurchaseHeader.idecWhatIfExpextedInstallmentAmount = adecExpectedInstallmentAmount;
            lbusMSSServicePurchaseWeb.GenerateWhatifSchedule();
            if (astrPaymentFrequency.IsNotNullOrEmpty())
                lbusMSSServicePurchaseWeb.ibusServicePurchaseHeader.istrWhatIfPaymentFrequency = busGlobalFunctions.GetDescriptionByCodeValue(330, astrPaymentFrequency, iobjPassInfo);
            return lbusMSSServicePurchaseWeb;
        }

        public busBenefitCalculatorWeb NewBenefitCalculationWeb(int aintPersonId)
        {
            //FMUpgrade: Changes for set Navigation parameter from menu item click
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmBenefitCalculationWebWizard")
            {
                aintPersonId = aintPersonId != 0 ? aintPersonId : iintPersonID;
            }
            busBenefitCalculatorWeb lbusMSSBenefitCalculation = new busBenefitCalculatorWeb { icdoWssBenefitcalculator = new cdoWssBenefitCalculator() };
            lbusMSSBenefitCalculation.LoadMember(aintPersonId);
            lbusMSSBenefitCalculation.ibusMember.LoadSpouse();
            lbusMSSBenefitCalculation.LoadTffrTiaaService(aintPersonId);
            lbusMSSBenefitCalculation.ibusBenefitCalculation = new busRetirementBenefitCalculation { icdoBenefitCalculation = new cdoBenefitCalculation() };
            lbusMSSBenefitCalculation.ibusPreRetirementDeathCalculation = new busPreRetirementDeathBenefitCalculation { icdoBenefitCalculation = new cdoBenefitCalculation() };
            lbusMSSBenefitCalculation.ibusBenefitCalculation.icdoBenefitCalculation.person_id = aintPersonId;
            if (lbusMSSBenefitCalculation.ibusMember.ibusSpouse.icdoPerson.person_id > 0)
            {
                lbusMSSBenefitCalculation.ibusBenefitCalculation.icdoBenefitCalculation.spouse_date_of_birth
                    = lbusMSSBenefitCalculation.ibusMember.ibusSpouse.icdoPerson.date_of_birth;
            }
            lbusMSSBenefitCalculation.ibusBenefitCalculation.icdoBenefitCalculation.calculation_type_value = busConstant.CalculationTypeEstimate;
            //load plans
            lbusMSSBenefitCalculation.LoadEligiblePlans();
            if (lbusMSSBenefitCalculation.iclbEligiblePlan.Count == 1)
            {
                lbusMSSBenefitCalculation.icdoWssBenefitcalculator.plan_id = lbusMSSBenefitCalculation.iclbEligiblePlan[0].plan_id;
                lbusMSSBenefitCalculation.ibusBenefitCalculation.icdoBenefitCalculation.plan_id = lbusMSSBenefitCalculation.icdoWssBenefitcalculator.plan_id;
                lbusMSSBenefitCalculation.LoadPlan();
            }
            bool lblnResult = false;
            DateTime ldteActualEmployeeTerminationDate = DateTime.MinValue;

            //PIR 17073 - Check if plan is suspended
            if (lbusMSSBenefitCalculation.ibusMember.icolPersonAccountByBenefitType.Count() > 0)
            {

                var lenumEnrolledPlans = lbusMSSBenefitCalculation.ibusMember.icolPersonAccountByBenefitType.Where(i =>
                                            i.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled);

                var lenumSuspendedPlans = lbusMSSBenefitCalculation.ibusMember.icolPersonAccountByBenefitType.Where(i =>
                                            i.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetimentSuspended);
                if (lenumSuspendedPlans.Count() > 0 && lenumEnrolledPlans.Count() == 0)
                    lblnResult = true;
            }

            lbusMSSBenefitCalculation.ibusBenefitCalculation.icdoBenefitCalculation.is_created_from_portal = busConstant.Flag_Yes;
            lbusMSSBenefitCalculation.ibusBenefitCalculation.LoadBenefitServicePurchase();


            if (lbusMSSBenefitCalculation.ibusDeductionCalculationWeb.IsNull())
                lbusMSSBenefitCalculation.ibusDeductionCalculationWeb = new busDeductionCalculation { icdoBenefitCalculation = new cdoBenefitCalculation() };

            lbusMSSBenefitCalculation.ibusDeductionCalculationWeb.ibusPerson = lbusMSSBenefitCalculation.ibusMember;

            lbusMSSBenefitCalculation.ibusDeductionCalculationWeb.ibusBenefitDentalDeduction = new busBenefitGhdvDeduction { icdoBenefitGhdvDeduction = new cdoBenefitGhdvDeduction() };
            lbusMSSBenefitCalculation.ibusDeductionCalculationWeb.ibusBenefitDentalDeduction.ibusPersonAccountGHDV = new busPersonAccountGhdv
            {
                icdoPersonAccountGhdv = new cdoPersonAccountGhdv(),
                icdoPersonAccount = new cdoPersonAccount()
            };
            lbusMSSBenefitCalculation.ibusDeductionCalculationWeb.ibusBenefitHealthDeduction = new busBenefitGhdvDeduction { icdoBenefitGhdvDeduction = new cdoBenefitGhdvDeduction() };
            lbusMSSBenefitCalculation.ibusDeductionCalculationWeb.ibusBenefitHealthDeduction.ibusPersonAccountGHDV = new busPersonAccountGhdv
            {
                icdoPersonAccountGhdv = new cdoPersonAccountGhdv(),
                icdoPersonAccount = new cdoPersonAccount()
            };
            lbusMSSBenefitCalculation.ibusDeductionCalculationWeb.ibusBenefitVisionDeduction = new busBenefitGhdvDeduction { icdoBenefitGhdvDeduction = new cdoBenefitGhdvDeduction() };
            lbusMSSBenefitCalculation.ibusDeductionCalculationWeb.ibusBenefitVisionDeduction.ibusPersonAccountGHDV = new busPersonAccountGhdv
            {
                icdoPersonAccountGhdv = new cdoPersonAccountGhdv(),
                icdoPersonAccount = new cdoPersonAccount()
            };
            lbusMSSBenefitCalculation.ibusDeductionCalculationWeb.ibusBenefitPayeeFedTaxWithholding = new busBenefitPayeeTaxWithholding { icdoBenefitPayeeTaxWithholding = new cdoBenefitPayeeTaxWithholding() };
            lbusMSSBenefitCalculation.ibusDeductionCalculationWeb.ibusBenefitPayeeStateTaxWithholding = new busBenefitPayeeTaxWithholding { icdoBenefitPayeeTaxWithholding = new cdoBenefitPayeeTaxWithholding() };
            lbusMSSBenefitCalculation.ibusDeductionCalculationWeb.iclbBenefitLtcMemberDeduction = new Collection<busBenefitLtcDeduction>();
            lbusMSSBenefitCalculation.ibusDeductionCalculationWeb.iclbBenefitLtcSpouseDeduction = new Collection<busBenefitLtcDeduction>();
            lbusMSSBenefitCalculation.ibusDeductionCalculationWeb.iclbLTCBenefitDeductions = new Collection<busBenefitLtcDeduction>();

            lbusMSSBenefitCalculation.ibusDeductionCalculationWeb.LoadBenefitDentalDeduction();
            lbusMSSBenefitCalculation.ibusDeductionCalculationWeb.LoadBenefitHealthDeduction();
            lbusMSSBenefitCalculation.ibusDeductionCalculationWeb.LoadBenefitVisionDeduction();

            lbusMSSBenefitCalculation.ibusDeductionCalculationWeb.LoadBenefitLTCMemberNew();
            lbusMSSBenefitCalculation.ibusDeductionCalculationWeb.LoadBenefitLTCSpouseNew();
            lbusMSSBenefitCalculation.ibusDeductionCalculationWeb.LoadBenefitLTCNew();
            // lbusMSSBenefitCalculation.ibusDeductionCalculationWeb.LoadBenefitLifeNew();
            lbusMSSBenefitCalculation.idecLifeBasicPremiumAmt = 1300.00M;

            lbusMSSBenefitCalculation.ibusDeductionCalculationWeb.LoadBenefitPayeeFedTaxWithholding();
            lbusMSSBenefitCalculation.ibusDeductionCalculationWeb.LoadBenefitPayeeStateTaxWithholding();

            //defaulting insurance type value           
            //rule - BR-24-57 - default setting
            lbusMSSBenefitCalculation.ibusDeductionCalculationWeb.ibusBenefitDentalDeduction.icdoBenefitGhdvDeduction.dental_insurance_type_value = busConstant.DentalInsuranceTypeRetiree;
            lbusMSSBenefitCalculation.ibusDeductionCalculationWeb.ibusBenefitVisionDeduction.icdoBenefitGhdvDeduction.vision_insurance_type_value = busConstant.VisionInsuranceTypeRetiree;
            if (lbusMSSBenefitCalculation.ibusMember.IsMemberEnrolledInPlan(busConstant.PlanIdDental))
            {
                lbusMSSBenefitCalculation.ibusDeductionCalculationWeb.ibusBenefitDentalDeduction.icdoBenefitGhdvDeduction.dental_insurance_type_value = busConstant.DentalInsuranceTypeCOBRA;
                lbusMSSBenefitCalculation.ibusDeductionCalculationWeb.ibusBenefitDentalDeduction.icdoBenefitGhdvDeduction.cobra_type_value = busConstant.COBRATypeRetiree18Month;
            }
            if (lbusMSSBenefitCalculation.ibusMember.IsMemberEnrolledInPlan(busConstant.PlanIdVision))
            {
                lbusMSSBenefitCalculation.ibusDeductionCalculationWeb.ibusBenefitVisionDeduction.icdoBenefitGhdvDeduction.vision_insurance_type_value = busConstant.VisionInsuranceTypeCOBRA;
                lbusMSSBenefitCalculation.ibusDeductionCalculationWeb.ibusBenefitVisionDeduction.icdoBenefitGhdvDeduction.cobra_type_value = busConstant.COBRATypeRetiree18Month;
            }
            //service purchase
            lbusMSSBenefitCalculation.ibusConsolidatedPurchase = new busServicePurchaseHeader
            {
                ibusPrimaryServicePurchaseDetail = new busServicePurchaseDetail
                {
                    icdoServicePurchaseDetail = new cdoServicePurchaseDetail()
                },
                icdoServicePurchaseHeader = new cdoServicePurchaseHeader()
            };
            lbusMSSBenefitCalculation.ibusSickLeavePurchase = new busServicePurchaseHeader
            {
                ibusPrimaryServicePurchaseDetail = new busServicePurchaseDetail
                {
                    icdoServicePurchaseDetail = new cdoServicePurchaseDetail()
                },
                icdoServicePurchaseHeader = new cdoServicePurchaseHeader()
            };
            lbusMSSBenefitCalculation.LoadConsolidatePurchaseInNewMode();
            lbusMSSBenefitCalculation.ibusBenefitCalculation.LoadBenefitServicePurchaseForMSS();
            lbusMSSBenefitCalculation.ibusBenefitCalculation.LoadBenefitCalculationPayeeForNewMode();
            lbusMSSBenefitCalculation.GetTentativeOrApprovedTFFRTIAAAmount();
            lbusMSSBenefitCalculation.icdoWssBenefitcalculator.number_of_payments = 180;
            lbusMSSBenefitCalculation.icdoWssBenefitcalculator.payment_frequency_value = busConstant.ServicePurchasePaymentFrequencyValueMonthly;
            lbusMSSBenefitCalculation.ibusBenefitCalculation.iclcBenCalcOtherDisBenefit = new Collection<busBenefitCalculationOtherDisBenefit>();
            lbusMSSBenefitCalculation.ibusBenefitCalculation.CalculateQDROAmount(true);
            lbusMSSBenefitCalculation.EvaluateInitialLoadRules();

            // PIR 9711
            lbusMSSBenefitCalculation.ibusPreRetirementDeathCalculation.icdoBenefitCalculation = lbusMSSBenefitCalculation.ibusBenefitCalculation.icdoBenefitCalculation;
            lbusMSSBenefitCalculation.ibusPreRetirementDeathCalculation.LoadBenefitServicePurchase();
            lbusMSSBenefitCalculation.ibusPreRetirementDeathCalculation.LoadBenefitServicePurchaseForMSS();
            lbusMSSBenefitCalculation.ibusPreRetirementDeathCalculation.LoadBenefitCalculationPayeeForNewMode();
            lbusMSSBenefitCalculation.ibusPreRetirementDeathCalculation.iclcBenCalcOtherDisBenefit = new Collection<busBenefitCalculationOtherDisBenefit>();
            lbusMSSBenefitCalculation.ibusPreRetirementDeathCalculation.CalculateQDROAmount(true);

            //PIR 17073
            if (lblnResult)
            {
                ldteActualEmployeeTerminationDate = lbusMSSBenefitCalculation.LoadLastEndDateOfPersonEmployment(aintPersonId);
                lbusMSSBenefitCalculation.ibusBenefitCalculation.icdoBenefitCalculation.termination_date = ldteActualEmployeeTerminationDate;
            }


            return lbusMSSBenefitCalculation;
        }

        public busWssBenefitCalculator FindBenefitCalculatorSummary(int aintBenefitCalculatorId, int aintPersonId)
        {
            //FMUpgrade: Changes for set Navigation parameter from menu item click
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmBenefitCalculatorSummaryMaintenance")
            {
                aintPersonId = aintPersonId != 0 ? aintPersonId : iintPersonID;
            }
            busWssBenefitCalculator lbusBenefitCalculatorSummary = new busWssBenefitCalculator();
            if (lbusBenefitCalculatorSummary.FindWssBenefitCalculator(aintBenefitCalculatorId))
            {
                lbusBenefitCalculatorSummary.LoadMember(aintPersonId);
                lbusBenefitCalculatorSummary.ibusMember.LoadSpouse();
                lbusBenefitCalculatorSummary.LoadPlan();
                lbusBenefitCalculatorSummary.LoadbusUnusedServicePurchaseHeader();
                lbusBenefitCalculatorSummary.ibusUnusedServicePurchaseHeader.LoadServicePurchaseDetail();
                lbusBenefitCalculatorSummary.ibusUnusedServicePurchaseHeader.ibusPrimaryServicePurchaseDetail.CalculateTimeToPurchaseForUnUsedSickLeave();
                lbusBenefitCalculatorSummary.LoadbusConsolidatedServicePurchaseHeader();
                lbusBenefitCalculatorSummary.ibusConsolidatedServicePurchaseHeader.LoadServicePurchaseDetail();
                lbusBenefitCalculatorSummary.LoadRetirementBenefitCalculation();
                lbusBenefitCalculatorSummary.ibusRetirementBenefitCalculation.LoadPersonAccount();
                lbusBenefitCalculatorSummary.ibusRetirementBenefitCalculation.ibusPersonAccount.ibusPlan = new busPlan { icdoPlan = new cdoPlan() };
                lbusBenefitCalculatorSummary.ibusRetirementBenefitCalculation.ibusPersonAccount.ibusPlan = lbusBenefitCalculatorSummary.ibusPlan;
                lbusBenefitCalculatorSummary.LoadLatestEmployer();
                lbusBenefitCalculatorSummary.LoadLastSalary();
                lbusBenefitCalculatorSummary.ibusRetirementBenefitCalculation.CalculateMemberAge();
                lbusBenefitCalculatorSummary.SetEstimatedServiceCredit();
                lbusBenefitCalculatorSummary.ibusRetirementBenefitCalculation.LoadDeduction();
                lbusBenefitCalculatorSummary.LoadDeductionWeb();
                // PIR 11382
                //lbusBenefitCalculatorSummary.ibusRetirementBenefitCalculation.idecRemainingServiceCredit =
                //    lbusBenefitCalculatorSummary.ibusConsolidatedServicePurchaseHeader.ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.RoundedTotalTimeOfPurchaseExcludeFreeService
                //                        + lbusBenefitCalculatorSummary.ibusUnusedServicePurchaseHeader.ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.total_time_to_purchase;

                //PIR 14652
                if (lbusBenefitCalculatorSummary.ibusMember.ibusSpouse.icdoPerson.person_id > 0 &&
                    lbusBenefitCalculatorSummary.icdoWssBenefitCalculator.spouse_date_of_birth == DateTime.MinValue)
                    lbusBenefitCalculatorSummary.ldtSpouseDateOfBirth = lbusBenefitCalculatorSummary.ibusMember.ibusSpouse.icdoPerson.date_of_birth;
                else
                    lbusBenefitCalculatorSummary.ldtSpouseDateOfBirth = lbusBenefitCalculatorSummary.icdoWssBenefitCalculator.spouse_date_of_birth;

                lbusBenefitCalculatorSummary.ibusRetirementBenefitCalculation.SetBenefitSubType();
                lbusBenefitCalculatorSummary.iblnIsPersonEligibleForEarly = lbusBenefitCalculatorSummary.ibusRetirementBenefitCalculation.icdoBenefitCalculation.benefit_account_sub_type_value
                    == busConstant.ApplicationBenefitSubTypeEarly ? true : false;
                lbusBenefitCalculatorSummary.ibusRetirementBenefitCalculation.LoadBenefitRHICOption();
                lbusBenefitCalculatorSummary.ibusRetirementBenefitCalculation.CalculateQDROAmount(true);
                lbusBenefitCalculatorSummary.ibusRetirementBenefitCalculation.CalculateFAS();
                lbusBenefitCalculatorSummary.LoadBenefitMulitiplier();
                //lbusBenefitCalculatorSummary.LoadMonthlyNonTaxableAmount();
                lbusBenefitCalculatorSummary.ibusRetirementBenefitCalculation.iclcBenCalcOtherDisBenefit = new Collection<busBenefitCalculationOtherDisBenefit>();
                if (!lbusBenefitCalculatorSummary.ibusRetirementBenefitCalculation.iblnConsolidatedPSCLoaded) // PIR 11382
                    lbusBenefitCalculatorSummary.ibusRetirementBenefitCalculation.CalculateConsolidatedPSC();
                if (lbusBenefitCalculatorSummary.ibusRetirementBenefitCalculation.icdoBenefitCalculation.uniform_income_or_ssli_flag == busConstant.Flag_Yes)
                    lbusBenefitCalculatorSummary.SetDateLevelIncomeEffective();
                lbusBenefitCalculatorSummary.idecTotalVestingServiceCredit = lbusBenefitCalculatorSummary.icdoWssBenefitCalculator.tffr_tiaa_service_credit +
                                                                              lbusBenefitCalculatorSummary.ibusRetirementBenefitCalculation.icdoBenefitCalculation.idecCredited_Vsc_From_File +
                                                                              lbusBenefitCalculatorSummary.ibusRetirementBenefitCalculation.icdoBenefitCalculation.projected_vsc;
            }
            return lbusBenefitCalculatorSummary;
        }

        public busFormListing FindFormListing()
        {
            busFormListing lbusFormListing = new busFormListing();
            lbusFormListing.LoadFormListingItems();
            return lbusFormListing;
        }

        // PIR 9884
        public busMssForms FindMssForms()
        {
            busMssForms lbusMssForms = new busMssForms();
            return lbusMssForms;
        }

        public bool IsContactExists(int aintContactID)
        {
            DataTable ldtResult = busNeoSpinBase.Select<cdoContact>(new string[1] { "contact_id" },
                                                                    new object[1] { aintContactID }, null, null);
            if (ldtResult.Rows.Count > 0)
                return true;
            return false;
        }

        public int CheckContactAccessAndReturnContactID(string astrNDLoginID)
        {
            int lintContactID = 0;
            DataTable ldtContact = new DataTable();
            ldtContact = busNeoSpinBase.Select<cdoContact>(new string[1] { enmContact.ndpers_login_id.ToString() },
                                                                        new object[1] { astrNDLoginID }, null, null);
            if (ldtContact.Rows.Count > 0)
            {
                lintContactID = Convert.ToInt32(ldtContact.Rows[0]["contact_id"]);
            }
            return lintContactID;
        }

        public DataTable GetEmployersForContact(int aintContactID)
        {
            busContact lobjContact = new busContact();
            lobjContact.FindContact(aintContactID);
            return lobjContact.LoadOrgContactDetails();
        }

        /// <summary>
        /// PIR 18493 - A deceased member should not be able to login
        /// Added lobjPerson.icdoPerson.date_of_death == DateTime.MinValue condition
        /// </summary>
        /// <param name="aintPersonID"></param>
        /// <returns></returns>
        public busPerson LoadPerson(int aintPersonID)
        {
            busPerson lobjPerson = new busPerson();
            if (!(lobjPerson.FindPerson(aintPersonID) && lobjPerson.icdoPerson.date_of_death == DateTime.MinValue))
                lobjPerson.icdoPerson.person_id = 0;
            return lobjPerson;
        }

        public busPerson LoadPersonByNDLoginID(string astrNDLoginID)
        {
            busPerson lobjPerson = new busPerson { icdoPerson = new cdoPerson() };
            DataTable ldtPerson = new DataTable();
            ldtPerson = busNeoSpinBase.Select<cdoPerson>(new string[1] { "ndpers_login_id" }, new object[1] { astrNDLoginID }, null, null);
            if (ldtPerson.Rows.Count > 0)
                lobjPerson.icdoPerson.LoadData(ldtPerson.Rows[0]);
            return lobjPerson;
        }

        public busContact LoadContact(int aintContactID)
        {
            busContact lobjContact = new busContact();
            lobjContact.FindContact(aintContactID);
            return lobjContact;
        }

        public busOrganization LoadOrganization(int aintOrgID)
        {
            busOrganization lobjOrganization = new busOrganization();
            lobjOrganization.FindOrganization(aintOrgID);
            return lobjOrganization;
        }

        public busOrganization LoadOrganizationByOrgCode(string astrOrgCodeID)
        {
            busOrganization lobjOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
            lobjOrganization.FindOrganizationByOrgCode(astrOrgCodeID);
            return lobjOrganization;
        }

        public busContact LoadContactByNDLoginID(string astrNDLoginID)
        {
            busContact lobjContact = new busContact { icdoContact = new cdoContact() };
            DataTable ldtContact = new DataTable();
            ldtContact = busNeoSpinBase.Select<cdoContact>(new string[1] { enmContact.ndpers_login_id.ToString() },
                                                                    new object[1] { astrNDLoginID }, null, null);
            if (ldtContact.Rows.Count > 0)
                lobjContact.icdoContact.LoadData(ldtContact.Rows[0]);
            return lobjContact;
        }

        public busWssDebitAchRequest FindWssDebitAchRequest(int aintdebitachrequestid)
        {
            busWssDebitAchRequest lobjWssDebitAchRequest = new busWssDebitAchRequest();
            if (lobjWssDebitAchRequest.FindWssDebitAchRequest(aintdebitachrequestid))
            {
            }

            return lobjWssDebitAchRequest;
        }

        public busWssMessageHeader NewWssMessageHeader()
        {
            busWssMessageHeader lobjWssMessageHeader = new busWssMessageHeader { icdoWssMessageHeader = new cdoWssMessageHeader() };
            return lobjWssMessageHeader;
        }

        public busWssMessageHeader FindWssMessageHeader(int aintwssmessageid)
        {
            busWssMessageHeader lobjWssMessageHeader = new busWssMessageHeader();
            if (lobjWssMessageHeader.FindWssMessageHeader(aintwssmessageid))
            {
                lobjWssMessageHeader.LoadTop100MessageDetails();
            }

            return lobjWssMessageHeader;
        }

        public busWssMessageDetail FindWssMessageDetail(int aintwssmessageid)
        {
            busWssMessageDetail lobjWssMessageDetail = new busWssMessageDetail();
            if (lobjWssMessageDetail.FindWssMessageDetail(aintwssmessageid))
            {
            }

            return lobjWssMessageDetail;
        }

        public busESSHome FindESSHome(int aintOrgID, int aintContactID, string astrProfileEmailID = null, bool ablnIsExternalUser = false)
        {
            SetWebParameters();
            aintContactID = aintContactID != 0 ? aintContactID : iintContactID;
            aintOrgID = aintOrgID != 0 ? aintOrgID : iintOrgID;
            astrProfileEmailID = istrNDPERSEmailIDESS;
            if (iobjPassInfo.idictParams.ContainsKey("IsExternalLogin"))
                ablnIsExternalUser = Convert.ToBoolean(iobjPassInfo.idictParams["IsExternalLogin"]);
            busESSHome lobjESSHome = new busESSHome();
            busWssMemberRecordRequest lobjWssMemberRecordRequest = new busWssMemberRecordRequest();
            if (lobjESSHome.LoadContact(aintContactID))
            {
                lobjESSHome.LoadOrganization(aintOrgID);
                //set astrProfileEmailID=string.Empty for Trace log functionality issue.
                lobjESSHome.istrProfileEmailID = astrProfileEmailID;
                lobjESSHome.iblnExternalLogin = ablnIsExternalUser;
                lobjESSHome.LoadOrgCodeOrgName();
                lobjESSHome.LoadLastThreeMonthUnReadMessages();
                //lobjESSHome.LoadContactTickets();
                //lobjESSHome.LoadEmployerPayrollHeader();
                lobjESSHome.istrUpdateAccountURL = istrSystemRegion != busConstant.SystemRegionValueProd ? busGlobalFunctions.GetData2ByCodeValue(busConstant.SystemConstantsAndVariablesCodeID, busConstant.UpdateAccountUrlCodeValue,  iobjPassInfo) :
                                                                                                            busGlobalFunctions.GetData1ByCodeValue(busConstant.SystemConstantsAndVariablesCodeID, busConstant.UpdateAccountUrlCodeValue, iobjPassInfo);
                lobjESSHome.LoadWssMemberRecordRequest(aintOrgID);
                lobjESSHome.LoadWSSEmploymentChangeRequests(aintOrgID);
                //Uncommented for PIR 14474
                foreach (busWssEmploymentChangeRequest lobjEmploymentChangeRequest in lobjESSHome.iclbWssEmploymentChangeRequest)
                {
                    lobjEmploymentChangeRequest.LoadPerson();
                    //PIR 13214 - Employment Detail Not Showing On Termination Wizard When navigated From Dashboard
                    lobjEmploymentChangeRequest.ibusPerson.iintOrgID = aintOrgID;
                    lobjEmploymentChangeRequest.ibusPerson.ESSLoadPersonEmployment();
                    lobjEmploymentChangeRequest.icdoWssEmploymentChangeRequest.person_employment_id = lobjEmploymentChangeRequest.ibusPerson.ibusESSPersonEmployment.icdoPersonEmployment.person_employment_id;
                    lobjEmploymentChangeRequest.icdoWssEmploymentChangeRequest.person_employment_detail_id = lobjEmploymentChangeRequest.ibusPerson.ibusESSPersonEmployment.ibusESSLatestPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id;
                }
                lobjESSHome.ibusEmployerPayrollMonthlyStatement = new busEmployerPayrollMonthlyStatement();
                lobjESSHome.ibusEmployerPayrollMonthlyStatement.iblnIsFromESS = true;
                // PIR 13852 - Commented for performance issue
                //lobjESSHome.ibusEmployerPayrollMonthlyStatement.LoadAgencyStatementInfo(aintOrgID);
            }
            return lobjESSHome;
        }


        public busESSHome FindESSContactTickets(int aintOrgID, int aintContactID)
        {
            /*
            * FMUpgrade: Changes for set Navigation parameter from menu item click
            */
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmESSContactTicketMaintenance")
            {
                aintOrgID = aintOrgID != 0 ? aintOrgID : iintOrgID;
                aintContactID = aintContactID != 0 ? aintContactID : iintContactID;
            }
            busESSHome lobjESSHome = new busESSHome();
            lobjESSHome.LoadOrganization(aintOrgID);
            lobjESSHome.LoadContactTickets();
            return lobjESSHome;
        }

        // PIR 9115
        public busBenefitEnrollmentReport LoadGeneratedReports(int aintOrgID, int aintContactID)
        {
            /*
             * FMUpgrade: Changes for set Navigation parameter from menu item click
             */
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmBenefitEnrollmentReportMaintenance")
            {
                aintOrgID = aintOrgID != 0 ? aintOrgID : iintOrgID;
                aintContactID = aintContactID != 0 ? aintContactID : iintContactID;
            }
            busBenefitEnrollmentReport lobjBenefitEnrollmentReport = new busBenefitEnrollmentReport();
            if (lobjBenefitEnrollmentReport.LoadContact(aintContactID))
            {
                lobjBenefitEnrollmentReport.LoadOrganization(aintOrgID);
                lobjBenefitEnrollmentReport.LoadContact(aintContactID);
                lobjBenefitEnrollmentReport.LoadGeneratedReportsFromInbox(aintOrgID, aintContactID);
            }
            return lobjBenefitEnrollmentReport;
        }

        public busESSHome LoadLastThreeMonthMessages(int aintOrgID, int aintContactID)
        {
            busESSHome lobjESSHome = new busESSHome();
            lobjESSHome.LoadContact(aintContactID);
            lobjESSHome.LoadOrganization(aintOrgID);
            lobjESSHome.LoadLastThreeMonthMessages();
            return lobjESSHome;
        }

        public DataTable GetESSQuestionsForOnlineAccess()
        {
            busESSHome lobjESSHome = new busESSHome();
            return lobjESSHome.GetESSQuestionsForOnlineAccess();
        }

        public DataTable GetMSSQuestionsForOnlineAccess()
        {
            busMSSHome lobjMSSHome = new busMSSHome();
            return lobjMSSHome.GetMSSQuestionsForOnlineAccess();
        }

        public DataTable GetSecurityForContact(int aintContactID, int aintOrgID)
        {
            return busNeoSpinBase.Select("cdoContact.GetSecurityBasedOnContactIDOrgID", new object[2] { aintContactID, aintOrgID });
        }
        public DataTable GetOrgContactRoles(int aintContactID, int aintOrgID)
        {
            return busNeoSpinBase.Select("cdoContact.GetRolesByOrgIDandContactID", new object[2] { aintContactID, aintOrgID });
        }
        public DataTable GetSecurityForMember(int aintPersonID)
        {
            return busNeoSpinBase.Select("cdoPerson.GetSecurityBasedOnPersonID", new object[1] { aintPersonID });
        }

        public int GrantOnlineAccessForEmployer(string astrOrgCodeID, int aintContactID, string astrQuestionCode, string astrAnswer,
                                                string astrNDPERSLoginID, string astrFirstName, string astrLastName)
        {
            busOrganization lobjOrganization = new busOrganization();
            lobjOrganization.FindOrganizationByOrgCode(astrOrgCodeID);
            if (lobjOrganization.icdoOrganization.org_id > 0)
                return lobjOrganization.GrantOnlineAccessForEmployer(aintContactID, astrQuestionCode, astrAnswer, astrNDPERSLoginID, astrFirstName, astrLastName, lobjOrganization.icdoOrganization.org_id);
            else
                return 4;
        }

        // PIR 10266
        public int GrantOnlineAccessForMember(int aintPersonID, string astrQuestion1Code, string astrAnswer1, string astrQuestion2Code,
                                                    string astrAnswer2, string astrNDPERSLoginID, string astrFirstName, string astrLastName)
        {
            busPerson lbusPerson = new busPerson();
            lbusPerson.FindPerson(aintPersonID);
            if (lbusPerson.icdoPerson.person_id > 0)
                return lbusPerson.GrantOnlineAccessForMember(astrQuestion1Code, astrAnswer1, astrQuestion2Code, astrAnswer2, astrNDPERSLoginID, astrFirstName, astrLastName);
            else
                return 5;
        }

        public busPerson GrantPersLinkAccess(string astrLastName, string astrSSN, string astrDOB)
        {
            busPerson lbusPerson = new busPerson { icdoPerson = new cdoPerson() };
            busMSSHome lbusMSSHome = new busMSSHome();
            lbusPerson = lbusMSSHome.GetPerson(astrLastName, astrSSN, astrDOB);
            return lbusPerson;
        }
        public bool IsPrimaryAuthAgent(int aintOrgID, int aintContactID)
        {
            DataTable ldtbList = busNeoSpinBase.Select("cdoOrgContact.IsPrimaryAuthAgent", new object[2] { aintOrgID, aintContactID });
            if (ldtbList.Rows.Count > 0)
                return true;
            return false;
        }
        #endregion

        #region UCS - 011 Addendum

        //load view seminars
        public busMSSHome FindMSSViewSeminars(int aintPersonID)
        {
            //FMUpgrade: Changes for set Navigation parameter from menu item click
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmMSSViewSeminarsMaintenance")
            {
                aintPersonID = aintPersonID != 0 ? aintPersonID : iintPersonID;
            }
            busMSSHome lbusMSSHome = new busMSSHome();
            if (lbusMSSHome.LoadPerson(aintPersonID))
            {
                lbusMSSHome.LoadSeminars();
            }
            return lbusMSSHome;
        }

        public busESSHome FindESSViewSeminars(int aintOrgID, int aintContactID)
        {
            /*
            * FMUpgrade: Changes for set Navigation parameter from menu item click
            */
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmESSViewSeminarsMaintenance")
            {
                aintOrgID = aintOrgID != 0 ? aintOrgID : iintOrgID;
                aintContactID = aintContactID != 0 ? aintContactID : iintContactID;
            }
            busESSHome lbusESSHome = new busESSHome();
            if (lbusESSHome.LoadContact(aintContactID))
            {
                lbusESSHome.LoadOrganization(aintOrgID);
                lbusESSHome.LoadSeminars();
            }
            return lbusESSHome;
        }

        public busContactTicket NewMSSContactNDPERS(int aintPersonId)
        {
            busContactTicket lbusContactTicket = new busContactTicket { icdoContactTicket = new cdoContactTicket() };
            lbusContactTicket.icdoContactTicket.person_id = aintPersonId;
            lbusContactTicket.LoadPerson();
            lbusContactTicket.icdoContactTicket.contact_method_value = busConstant.ResponseMethodWeb;
            lbusContactTicket.icdoContactTicket.status_value = busConstant.ContactTicketStatusOpen;
            lbusContactTicket.iblnIsFromPortal = true;
            lbusContactTicket.icdoContactTicket.is_ticket_created_from_portal_flag = busConstant.Flag_Yes;
            return lbusContactTicket;
        }

        public busContactTicket NewESSContactNDPERS(int aintOrgId, int aintContactID)
        {
            /*
            * FMUpgrade: Changes for set Navigation parameter from menu item click
            */
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmESSContactNDPERSMaintenance")
            {
                aintOrgId = aintOrgId != 0 ? aintOrgId : iintOrgID;
                aintContactID = aintContactID != 0 ? aintContactID : iintContactID;
            }
            busContactTicket lbusContactTicket = new busContactTicket { icdoContactTicket = new cdoContactTicket() };
            lbusContactTicket.icdoContactTicket.org_id = aintOrgId;
            lbusContactTicket.LoadOrganization();
            lbusContactTicket.icdoContactTicket.istrOrgCodeID = lbusContactTicket.ibusOrganization.icdoOrganization.org_code;
            lbusContactTicket.icdoContactTicket.web_contact_id = aintContactID;
            lbusContactTicket.LoadWebContact();
            lbusContactTicket.icdoContactTicket.caller_name = lbusContactTicket.ibusWebContact.icdoContact.ContactName;
            lbusContactTicket.icdoContactTicket.contact_method_value = busConstant.ResponseMethodWeb;
            lbusContactTicket.icdoContactTicket.status_value = busConstant.ContactTicketStatusOpen;
            lbusContactTicket.iblnIsFromPortal = true;
            lbusContactTicket.icdoContactTicket.is_ticket_created_from_portal_flag = busConstant.Flag_Yes;
            return lbusContactTicket;
        }

        public busContactTicket NewESSReportAProblem(int aintOrgId, int aintContactID)
        {
            /*
            * FMUpgrade: Changes for set Navigation parameter from menu item click
            */
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmESSNewReportAProblemMaintenance")
            {
                aintOrgId = aintOrgId != 0 ? aintOrgId : iintOrgID;
                aintContactID = aintContactID != 0 ? aintContactID : iintContactID;
            }
            busContactTicket lbusContactTicket = new busContactTicket { icdoContactTicket = new cdoContactTicket() };
            lbusContactTicket.icdoContactTicket.org_id = aintOrgId;
            lbusContactTicket.LoadOrganization();
            lbusContactTicket.icdoContactTicket.istrOrgCodeID = lbusContactTicket.ibusOrganization.icdoOrganization.org_code;
            lbusContactTicket.icdoContactTicket.web_contact_id = aintContactID;
            lbusContactTicket.LoadWebContact();
            lbusContactTicket.icdoContactTicket.caller_name = lbusContactTicket.ibusWebContact.icdoContact.ContactName;
            lbusContactTicket.icdoContactTicket.contact_method_value = busConstant.ResponseMethodWeb;
            lbusContactTicket.icdoContactTicket.status_value = busConstant.ContactTicketStatusOpen;
            lbusContactTicket.iblnIsFromPortal = true;
            lbusContactTicket.iblnIsFromESS = true;
            lbusContactTicket.icdoContactTicket.is_ticket_created_from_portal_flag = busConstant.Flag_Yes;
            lbusContactTicket.iblnIsFromReportAProblem = true;
            lbusContactTicket.LoadESSReportedProblems();
            lbusContactTicket.icdoContactTicket.istrPublishToWss = busConstant.Flag_Yes; //PIR-19351
            lbusContactTicket.EvaluateInitialLoadRules();
            return lbusContactTicket;
        }

        public busContactTicket NewMSSDeathNotice(int aintPersonId)
        {
            //FMUpgrade: Changes for set Navigation parameter from menu item click
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmMSSDeathNoticeMaintenance")
            {
                aintPersonId = aintPersonId != 0 ? aintPersonId : iintPersonID;
            }
            busContactTicket lbusContactTicket = new busContactTicket { icdoContactTicket = new cdoContactTicket() };
            lbusContactTicket.ibusDeathNotice = new busDeathNotice { icdoDeathNotice = new cdoDeathNotice() };
            lbusContactTicket.icdoContactTicket.person_id = aintPersonId;
            lbusContactTicket.LoadPerson();
            lbusContactTicket.icdoContactTicket.contact_method_value = busConstant.ResponseMethodWeb;
            lbusContactTicket.icdoContactTicket.status_value = busConstant.ContactTicketStatusOpen;
            lbusContactTicket.iblnIsFromPortal = true;
            lbusContactTicket.iblnIsFromMSS = true;//For MSS Layout change
            lbusContactTicket.icdoContactTicket.is_ticket_created_from_portal_flag = busConstant.Flag_Yes;
            lbusContactTicket.icdoContactTicket.contact_type_value = busConstant.ContactTicketTypeDeath;
            lbusContactTicket.EvaluateInitialLoadRules();
            //We must set the object state here because there is no data change in Contact Ticket. But, logic involved based on this object state.
            lbusContactTicket.icdoContactTicket.ienuObjectState = ObjectState.Insert;
            return lbusContactTicket;
        }

        public busContactTicket NewESSDeathNotice(int aintOrgId, int aintContactID)
        {
            /*
           * FMUpgrade: Changes for set Navigation parameter from menu item click
           */
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmESSDeathNoticeMaintenance")
            {
                aintOrgId = aintOrgId != 0 ? aintOrgId : iintOrgID;
                aintContactID = aintContactID != 0 ? aintContactID : iintContactID;
            }
            busContactTicket lbusContactTicket = new busContactTicket { icdoContactTicket = new cdoContactTicket() };
            lbusContactTicket.ibusDeathNotice = new busDeathNotice { icdoDeathNotice = new cdoDeathNotice() };
            lbusContactTicket.icdoContactTicket.org_id = aintOrgId;
            lbusContactTicket.LoadOrganization();
            lbusContactTicket.icdoContactTicket.istrOrgCodeID = lbusContactTicket.ibusOrganization.icdoOrganization.org_code;
            lbusContactTicket.icdoContactTicket.web_contact_id = aintContactID;
            lbusContactTicket.LoadWebContact();
            lbusContactTicket.icdoContactTicket.caller_name = lbusContactTicket.ibusWebContact.icdoContact.ContactName;
            lbusContactTicket.icdoContactTicket.contact_method_value = busConstant.ResponseMethodWeb;
            lbusContactTicket.icdoContactTicket.status_value = busConstant.ContactTicketStatusOpen;
            lbusContactTicket.iblnIsFromPortal = true;
            lbusContactTicket.iblnIsFromESS = true;
            lbusContactTicket.icdoContactTicket.is_ticket_created_from_portal_flag = busConstant.Flag_Yes;
            lbusContactTicket.icdoContactTicket.contact_type_value = busConstant.ContactTicketTypeDeath;
            lbusContactTicket.ibusDeathNotice.icdoDeathNotice.deceased_member_flag = busConstant.Flag_Yes;
            lbusContactTicket.EvaluateInitialLoadRules();
            //We must set the object state here because there is no data change in Contact Ticket. But, logic involved based on this object state.
            lbusContactTicket.icdoContactTicket.ienuObjectState = ObjectState.Insert;
            lbusContactTicket.LoadESSDeathNotices();
            return lbusContactTicket;
        }

        public busContactTicket NewMSSBenefitEstimate(int aintPersonId)
        {
            //FMUpgrade: Changes for set Navigation parameter from menu item click
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmMSSBenefitEstimateMaintenance")
            {
                aintPersonId = aintPersonId != 0 ? aintPersonId : iintPersonID;
            }
            busContactTicket lbusContactTicket = new busContactTicket { icdoContactTicket = new cdoContactTicket() };
            lbusContactTicket.ibusBenefitEstimate = new busBenefitEstimate { icdoBenefitEstimate = new cdoBenefitEstimate() };
            lbusContactTicket.ibusBenefitEstimate.iclcRetirementType = new utlCollection<cdoBenefitEstimateRetirementType>();
            lbusContactTicket.icdoContactTicket.person_id = aintPersonId;
            lbusContactTicket.LoadPerson();
            lbusContactTicket.icdoContactTicket.contact_method_value = busConstant.ResponseMethodWeb;
            lbusContactTicket.icdoContactTicket.status_value = busConstant.ContactTicketStatusOpen;
            lbusContactTicket.iblnIsFromPortal = true;
            lbusContactTicket.icdoContactTicket.is_ticket_created_from_portal_flag = busConstant.Flag_Yes;
            lbusContactTicket.icdoContactTicket.contact_type_value = busConstant.ContactTicketTypeRetBenefitEstimate;
            //We must set the object state here because there is no data change in Contact Ticket. But, logic involved based on this object state.
            lbusContactTicket.icdoContactTicket.ienuObjectState = ObjectState.Insert;
            return lbusContactTicket;
        }

        public busContactTicket NewESSBenefitEstimate(int aintOrgId, int aintContactID)
        {
            busContactTicket lbusContactTicket = new busContactTicket { icdoContactTicket = new cdoContactTicket() };
            lbusContactTicket.ibusBenefitEstimate = new busBenefitEstimate { icdoBenefitEstimate = new cdoBenefitEstimate() };
            lbusContactTicket.ibusBenefitEstimate.iclcRetirementType = new utlCollection<cdoBenefitEstimateRetirementType>();
            lbusContactTicket.icdoContactTicket.org_id = aintOrgId;
            lbusContactTicket.LoadOrganization();
            lbusContactTicket.icdoContactTicket.istrOrgCodeID = lbusContactTicket.ibusOrganization.icdoOrganization.org_code;
            lbusContactTicket.icdoContactTicket.web_contact_id = aintContactID;
            lbusContactTicket.LoadWebContact();
            lbusContactTicket.icdoContactTicket.caller_name = lbusContactTicket.ibusWebContact.icdoContact.ContactName;
            lbusContactTicket.icdoContactTicket.contact_method_value = busConstant.ResponseMethodWeb;
            lbusContactTicket.icdoContactTicket.status_value = busConstant.ContactTicketStatusOpen;
            lbusContactTicket.iblnIsFromPortal = true;
            lbusContactTicket.icdoContactTicket.is_ticket_created_from_portal_flag = busConstant.Flag_Yes;
            lbusContactTicket.icdoContactTicket.contact_type_value = busConstant.ContactTicketTypeRetBenefitEstimate;
            //We must set the object state here because there is no data change in Contact Ticket. But, logic involved based on this object state.
            lbusContactTicket.icdoContactTicket.ienuObjectState = ObjectState.Insert;
            return lbusContactTicket;
        }

        public busContactTicket NewMSSAppointmentSchedule(int aintPersonId)
        {
            //FMUpgrade: Changes for set Navigation parameter from menu item click
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmMSSAppointmentScheduleMaintenance")
            {
                aintPersonId = aintPersonId != 0 ? aintPersonId : iintPersonID;
            }
            busContactTicket lbusContactTicket = new busContactTicket { icdoContactTicket = new cdoContactTicket() };
            lbusContactTicket.ibusAppointmentSchedule = new busAppointmentSchedule { icdoAppointmentSchedule = new cdoAppointmentSchedule() };
            lbusContactTicket.icdoContactTicket.person_id = aintPersonId;
            lbusContactTicket.LoadPerson();
            lbusContactTicket.icdoContactTicket.contact_method_value = busConstant.ResponseMethodWeb;
            lbusContactTicket.icdoContactTicket.status_value = busConstant.ContactTicketStatusOpen;
            lbusContactTicket.iblnIsFromPortal = true;
            lbusContactTicket.iblnIsFromMSS = true;//For MSS Layout change
            lbusContactTicket.icdoContactTicket.is_ticket_created_from_portal_flag = busConstant.Flag_Yes;
            lbusContactTicket.icdoContactTicket.contact_type_value = busConstant.ContactTicketTypeAppointment;
            //Backlog PIR 12832          
            lbusContactTicket.iblnIsFromSeminarSchedule = true;
            //We must set the object state here because there is no data change in Contact Ticket. But, logic involved based on this object state.
            lbusContactTicket.icdoContactTicket.ienuObjectState = ObjectState.Insert;
            return lbusContactTicket;
        }

        public busContactTicket NewESSAppointmentSchedule(int aintOrgId, int aintContactID)
        {
            /*
            * FMUpgrade: Changes for set Navigation parameter from menu item click
            */
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmESSAppointmentScheduleMaintenance")
            {
                aintOrgId = aintOrgId != 0 ? aintOrgId : iintOrgID;
                aintContactID = aintContactID != 0 ? aintContactID : iintContactID;
            }
            busContactTicket lbusContactTicket = new busContactTicket { icdoContactTicket = new cdoContactTicket() };
            lbusContactTicket.ibusAppointmentSchedule = new busAppointmentSchedule { icdoAppointmentSchedule = new cdoAppointmentSchedule() };
            lbusContactTicket.icdoContactTicket.org_id = aintOrgId;
            lbusContactTicket.LoadOrganization();
            lbusContactTicket.icdoContactTicket.istrOrgCodeID = lbusContactTicket.ibusOrganization.icdoOrganization.org_code;
            lbusContactTicket.icdoContactTicket.web_contact_id = aintContactID;
            lbusContactTicket.LoadWebContact();
            lbusContactTicket.icdoContactTicket.caller_name = lbusContactTicket.ibusWebContact.icdoContact.ContactName;
            lbusContactTicket.icdoContactTicket.contact_method_value = busConstant.ResponseMethodWeb;
            lbusContactTicket.icdoContactTicket.status_value = busConstant.ContactTicketStatusOpen;
            lbusContactTicket.iblnIsFromPortal = true;
            lbusContactTicket.iblnIsFromESS = true;
            lbusContactTicket.iblnIsFromSeminarSchedule = true;
            lbusContactTicket.icdoContactTicket.is_ticket_created_from_portal_flag = busConstant.Flag_Yes;
            lbusContactTicket.icdoContactTicket.contact_type_value = busConstant.ContactTicketTypeAppointment;
            //We must set the object state here because there is no data change in Contact Ticket. But, logic involved based on this object state.
            lbusContactTicket.icdoContactTicket.ienuObjectState = ObjectState.Insert;
            lbusContactTicket.LoadESSAppointments();
            return lbusContactTicket;
        }

        public busContactTicket NewMSSContactMGMTServicePurchase(int aintPersonId)
        {
            //FMUpgrade: Changes for set Navigation parameter from menu item click
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmMSSServicePurchaseEstimateMaintenance")
            {
                aintPersonId = aintPersonId != 0 ? aintPersonId : iintPersonID;
            }
            busContactTicket lbusContactTicket = new busContactTicket { icdoContactTicket = new cdoContactTicket() };
            lbusContactTicket.ibusContactMgmtServicePurchase = new busContactMgmtServicePurchase { icdoContactMgmtServicePurchase = new cdoContactMgmtServicePurchase() };
            lbusContactTicket.icdoContactTicket.person_id = aintPersonId;
            lbusContactTicket.LoadPerson();
            lbusContactTicket.icdoContactTicket.contact_method_value = busConstant.ResponseMethodWeb;
            lbusContactTicket.icdoContactTicket.status_value = busConstant.ContactTicketStatusOpen;
            lbusContactTicket.iblnIsFromPortal = true;
            lbusContactTicket.icdoContactTicket.is_ticket_created_from_portal_flag = busConstant.Flag_Yes;
            lbusContactTicket.icdoContactTicket.contact_type_value = busConstant.ContactTicketTypeRetPurchases;
            lbusContactTicket.ibusContactMgmtServicePurchase.iclcSerPurRolloverInfo = new utlCollection<cdoSerPurRolloverInfo>();
            lbusContactTicket.ibusContactMgmtServicePurchase.LoadSerPurType();
            lbusContactTicket.icdoContactTicket.PopulateDescriptions();
            //We must set the object state here because there is no data change in Contact Ticket. But, logic involved based on this object state.
            lbusContactTicket.icdoContactTicket.ienuObjectState = ObjectState.Insert;
            return lbusContactTicket;
        }

        public busContactTicket NewESSContactMGMTServicePurchase(int aintOrgId, int aintContactID)
        {
            busContactTicket lbusContactTicket = new busContactTicket { icdoContactTicket = new cdoContactTicket() };
            lbusContactTicket.ibusContactMgmtServicePurchase = new busContactMgmtServicePurchase { icdoContactMgmtServicePurchase = new cdoContactMgmtServicePurchase() };
            lbusContactTicket.icdoContactTicket.org_id = aintOrgId;
            lbusContactTicket.icdoContactTicket.web_contact_id = aintContactID;
            lbusContactTicket.LoadOrganization();
            lbusContactTicket.icdoContactTicket.istrOrgCodeID = lbusContactTicket.ibusOrganization.icdoOrganization.org_code;
            lbusContactTicket.LoadWebContact();
            lbusContactTicket.icdoContactTicket.caller_name = lbusContactTicket.ibusWebContact.icdoContact.ContactName;
            lbusContactTicket.icdoContactTicket.contact_method_value = busConstant.ResponseMethodWeb;
            lbusContactTicket.icdoContactTicket.status_value = busConstant.ContactTicketStatusOpen;
            lbusContactTicket.iblnIsFromPortal = true;
            lbusContactTicket.icdoContactTicket.is_ticket_created_from_portal_flag = busConstant.Flag_Yes;
            lbusContactTicket.icdoContactTicket.contact_type_value = busConstant.ContactTicketTypeRetPurchases;
            lbusContactTicket.ibusContactMgmtServicePurchase.iclcSerPurRolloverInfo = new utlCollection<cdoSerPurRolloverInfo>();
            lbusContactTicket.ibusContactMgmtServicePurchase.LoadSerPurType();
            lbusContactTicket.icdoContactTicket.PopulateDescriptions();
            //We must set the object state here because there is no data change in Contact Ticket. But, logic involved based on this object state.
            lbusContactTicket.icdoContactTicket.ienuObjectState = ObjectState.Insert;
            return lbusContactTicket;
        }

        public busSeminarAttendeeDetail NewMSSSeminarAttendeeDetail(int aintSeminarId, int aintLoggedInPersonID)
        {
            //FMUpgrade: Changes for set Navigation parameter from menu item click
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmMSSSeminarAttendeeDetailsMaintenance")
            {
                aintLoggedInPersonID = aintLoggedInPersonID != 0 ? aintLoggedInPersonID : iintPersonID;
            }
            busSeminarAttendeeDetail lbusSeminarAttendeeDetail = new busSeminarAttendeeDetail { icdoSeminarAttendeeDetail = new cdoSeminarAttendeeDetail() };
            lbusSeminarAttendeeDetail.icdoSeminarAttendeeDetail.seminar_schedule_id = aintSeminarId;
            lbusSeminarAttendeeDetail.iblnIsFromPortal = true;
            lbusSeminarAttendeeDetail.icdoSeminarAttendeeDetail.person_id = aintLoggedInPersonID;
            lbusSeminarAttendeeDetail.LoadPerson();
            lbusSeminarAttendeeDetail.LoadSeminarSchedule();
            lbusSeminarAttendeeDetail.ibusSeminarSchedule.LoadContactTicket();
            lbusSeminarAttendeeDetail.LoadSponsorName();
            lbusSeminarAttendeeDetail.LoadOrganization();
            lbusSeminarAttendeeDetail.iclcRetirementType = new utlCollection<cdoSeminarAttendeeDetailRetirementType>();
            return lbusSeminarAttendeeDetail;
        }

        public busSeminarAttendeeDetail NewESSSeminarAttendeeDetail(int aintSeminarId, int aintLoggedInContactID, int aintLogedOrgId)  //PIR-14667, repurposed under PIR-11258
        {
            /*
             * FMUpgrade: Changes for set Navigation parameter from menu item click
             */
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmESSSeminarAttendeeDetailsMaintenance")
            {
                aintLogedOrgId = aintLogedOrgId != 0 ? aintLogedOrgId : iintOrgID;
                aintLoggedInContactID = aintLoggedInContactID != 0 ? aintLoggedInContactID : iintContactID;
            }
            busSeminarAttendeeDetail lbusSeminarAttendeeDetail = new busSeminarAttendeeDetail { icdoSeminarAttendeeDetail = new cdoSeminarAttendeeDetail() };
            lbusSeminarAttendeeDetail.icdoSeminarAttendeeDetail.seminar_schedule_id = aintSeminarId;
            lbusSeminarAttendeeDetail.LoadSeminarAttendeeDetailForESS();

            lbusSeminarAttendeeDetail.iblnIsFromPortal = true;
            var lvarSeminarAttendeeDetail = lbusSeminarAttendeeDetail.iclbSeminarAttendeeDetail.Where(i => i.icdoSeminarAttendeeDetail.contact_id == aintLoggedInContactID);
            if (lvarSeminarAttendeeDetail.Count() > 0)
            {
                foreach (busSeminarAttendeeDetail lbusSeminarAttendeeDetailNew in lvarSeminarAttendeeDetail.AsEnumerable<busSeminarAttendeeDetail>())
                {
                    lbusSeminarAttendeeDetailNew.LoadSeminarSchedule();
                    lbusSeminarAttendeeDetailNew.ibusSeminarSchedule.LoadContactTicket();
                    if (lbusSeminarAttendeeDetailNew.ibusSeminarSchedule.icdoSeminarSchedule.seminar_type_value == busConstant.SeminarTypePayrollConference ||
                     lbusSeminarAttendeeDetailNew.ibusSeminarSchedule.icdoSeminarSchedule.seminar_type_value == busConstant.SeminarTypeWellnessForum)
                    {
                        lbusSeminarAttendeeDetailNew.LoadContact();
                    }
                    lbusSeminarAttendeeDetailNew.LoadSponsorName();
                    if (lbusSeminarAttendeeDetailNew.ibusSeminarSchedule.icdoSeminarSchedule.seminar_type_value == busConstant.SeminarTypePrepEmployer)
                    {
                        lbusSeminarAttendeeDetailNew.icdoSeminarAttendeeDetail.org_to_bill_id = lbusSeminarAttendeeDetail.ibusSeminarSchedule.ibusContactTicket.icdoContactTicket.org_id;
                    }
                    lbusSeminarAttendeeDetailNew.LoadOrganization();
                    lbusSeminarAttendeeDetailNew.iclcRetirementType = new utlCollection<cdoSeminarAttendeeDetailRetirementType>();
                    lbusSeminarAttendeeDetail = lbusSeminarAttendeeDetailNew;
                    lbusSeminarAttendeeDetail.iblnIsFromESS = true;
                    lbusSeminarAttendeeDetail.EvaluateInitialLoadRules(utlPageMode.Update);
                }
            }
            else
            {
                lbusSeminarAttendeeDetail.LoadSeminarSchedule();
                lbusSeminarAttendeeDetail.iintLoginOrgId = aintLogedOrgId;	//PIR-14667, repurposed under PIR-11258
                lbusSeminarAttendeeDetail.ibusSeminarSchedule.LoadContactTicket();
                if (lbusSeminarAttendeeDetail.ibusSeminarSchedule.icdoSeminarSchedule.seminar_type_value == busConstant.SeminarTypePayrollConference ||
                    lbusSeminarAttendeeDetail.ibusSeminarSchedule.icdoSeminarSchedule.seminar_type_value == busConstant.SeminarTypeWellnessForum)
                {
                    lbusSeminarAttendeeDetail.icdoSeminarAttendeeDetail.contact_id = aintLoggedInContactID;
                    lbusSeminarAttendeeDetail.LoadContact();
                }
                lbusSeminarAttendeeDetail.LoadSponsorName();
                if (lbusSeminarAttendeeDetail.ibusSeminarSchedule.icdoSeminarSchedule.seminar_type_value == busConstant.SeminarTypePrepEmployer)
                {
                    lbusSeminarAttendeeDetail.icdoSeminarAttendeeDetail.org_to_bill_id = lbusSeminarAttendeeDetail.ibusSeminarSchedule.ibusContactTicket.icdoContactTicket.org_id;
                }
                lbusSeminarAttendeeDetail.LoadOrganization();
                lbusSeminarAttendeeDetail.iclcRetirementType = new utlCollection<cdoSeminarAttendeeDetailRetirementType>();
                lbusSeminarAttendeeDetail.EvaluateInitialLoadRules(utlPageMode.New);
            }
            return lbusSeminarAttendeeDetail;
        }

        public busContactTicket FindMSSContactNDPERS(int aintPersonId)
        {
            //FMUpgrade: Changes for set Navigation parameter from menu item click
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmMSSContactNDPERSMaintenance")
            {
                aintPersonId = aintPersonId != 0 ? aintPersonId : iintPersonID;
            }
            //PIR-9849
            busContactTicket lbusContactTicket = new busContactTicket { icdoContactTicket = new cdoContactTicket() };
            lbusContactTicket.icdoContactTicket.person_id = aintPersonId;
            lbusContactTicket.LoadPerson();
            lbusContactTicket.icdoContactTicket.contact_method_value = busConstant.ResponseMethodWeb;
            lbusContactTicket.icdoContactTicket.status_value = busConstant.ContactTicketStatusOpen;
            lbusContactTicket.icdoContactTicket.contact_type_value = busConstant.ContactTicketTypePerslinkPortal;
            lbusContactTicket.iblnIsFromPortal = true;
            lbusContactTicket.icdoContactTicket.is_ticket_created_from_portal_flag = busConstant.Flag_Yes;
            //Backlog PIR 12832          
            lbusContactTicket.iblnIsFromMssContactNDPERS = true;
            lbusContactTicket.icdoContactTicket.istrPublishToWss = busConstant.Flag_Yes; //PIR-19351
            lbusContactTicket.LoadContactTicketByPerson();
            lbusContactTicket.EvaluateInitialLoadRules();
            return lbusContactTicket;
        }

        public busContactTicket FindESSContactNDPERS(int aintContactTicketId)
        {
            busContactTicket lbusContactTicket = new busContactTicket { icdoContactTicket = new cdoContactTicket() };
            if (lbusContactTicket.FindContactTicket(aintContactTicketId))
            {
                lbusContactTicket.LoadOrganization();
                lbusContactTicket.icdoContactTicket.istrOrgCodeID = lbusContactTicket.ibusOrganization.icdoOrganization.org_code;
                lbusContactTicket.iblnIsFromPortal = true;
                lbusContactTicket.LoadContactTicketHistory();
            }
            return lbusContactTicket;
        }

        public busContactTicket FindESSReportAProblem(int aintContactTicketId)
        {
            busContactTicket lbusContactTicket = new busContactTicket { icdoContactTicket = new cdoContactTicket() };
            if (lbusContactTicket.FindContactTicket(aintContactTicketId))
            {
                if (iobjPassInfo.istrFormName == "wfmESSNewReportAProblemMaintenance")
                {
                    lbusContactTicket.iblnWSSIsHistoryInserted = true;
                }
                lbusContactTicket.LoadOrganization();
                lbusContactTicket.icdoContactTicket.istrOrgCodeID = lbusContactTicket.ibusOrganization.icdoOrganization.org_code;
                lbusContactTicket.iblnIsFromPortal = true;
                lbusContactTicket.iblnIsFromESS = true;
                lbusContactTicket.iblnIsFromReportAProblem = true;
                lbusContactTicket.LoadContactTicketHistory();
                lbusContactTicket.icdoContactTicket.notes = string.Empty;
                lbusContactTicket.LoadContactTicketDetailHistory(aintContactTicketId);
                lbusContactTicket.icdoContactTicket.istrPublishToWss = busConstant.Flag_Yes;
                lbusContactTicket.LoadESSReportedProblems(); //F/W Upgrade PIR 11035 - History is not getting loaded upon breadcrum closed
            }
            return lbusContactTicket;
        }

        public busContactTicket FindMSSDeathNotice(int aintContactTicketId)
        {
            busContactTicket lobjContactTicket = new busContactTicket { icdoContactTicket = new cdoContactTicket() };
            lobjContactTicket.ibusDeathNotice = new busDeathNotice();
            if (lobjContactTicket.FindContactTicket(aintContactTicketId))
            {
                lobjContactTicket.iblnIsFromPortal = true;
                lobjContactTicket.LoadPerson();
                lobjContactTicket.ibusDeathNotice.FindDeathNoticeByContactTicket(aintContactTicketId);
            }
            return lobjContactTicket;
        }

        public busContactTicket FindESSDeathNotice(int aintContactTicketId)
        {
            busContactTicket lobjContactTicket = new busContactTicket { icdoContactTicket = new cdoContactTicket() };
            lobjContactTicket.ibusDeathNotice = new busDeathNotice();
            if (lobjContactTicket.FindContactTicket(aintContactTicketId))
            {
                lobjContactTicket.iblnIsFromPortal = true;
                lobjContactTicket.iblnIsFromESS = true;
                lobjContactTicket.LoadOrganization();
                lobjContactTicket.icdoContactTicket.istrOrgCodeID = lobjContactTicket.ibusOrganization.icdoOrganization.org_code;
                lobjContactTicket.ibusDeathNotice.FindDeathNoticeByContactTicket(aintContactTicketId);
                busPerson lbusperson = lobjContactTicket.GetPersonNameByPersonID(lobjContactTicket.ibusDeathNotice.icdoDeathNotice.perslink_id);
                lobjContactTicket.ibusDeathNotice.icdoDeathNotice.iblnIsPersonEnrolledInRetirementPlan = lbusperson.icdoPerson.iblnIsPersonEnrolledInRetirementPlan;
                if (lobjContactTicket.ibusDeathNotice.icdoDeathNotice.last_reporting_month_for_retirement_contributions != DateTime.MinValue)
                    lobjContactTicket.ibusDeathNotice.icdoDeathNotice.reporting_Month = lobjContactTicket.ibusDeathNotice.icdoDeathNotice.last_reporting_month_for_retirement_contributions.ToString("MM/yyyy", System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                lobjContactTicket.LoadESSDeathNotices();
            }
            return lobjContactTicket;
        }

        public busContactTicket FindMSSBenefitEstimate(int aintContactTicketId)
        {
            busContactTicket lobjContactTicket = new busContactTicket { icdoContactTicket = new cdoContactTicket() };
            lobjContactTicket.ibusBenefitEstimate = new busBenefitEstimate();
            lobjContactTicket.ibusBenefitEstimate.iclcRetirementType = new utlCollection<cdoBenefitEstimateRetirementType>();

            if (lobjContactTicket.FindContactTicket(aintContactTicketId))
            {
                lobjContactTicket.iblnIsFromPortal = true;
                lobjContactTicket.LoadPerson();
                lobjContactTicket.ibusBenefitEstimate.FindBenefitEstimateByContactTicket(aintContactTicketId);
                lobjContactTicket.ibusBenefitEstimate.LoadRetirementType();
            }
            return lobjContactTicket;
        }

        public busContactTicket FindESSBenefitEstimate(int aintContactTicketId)
        {
            busContactTicket lobjContactTicket = new busContactTicket { icdoContactTicket = new cdoContactTicket() };
            lobjContactTicket.ibusBenefitEstimate = new busBenefitEstimate();
            lobjContactTicket.ibusBenefitEstimate.iclcRetirementType = new utlCollection<cdoBenefitEstimateRetirementType>();

            if (lobjContactTicket.FindContactTicket(aintContactTicketId))
            {
                lobjContactTicket.iblnIsFromPortal = true;
                lobjContactTicket.LoadOrganization();
                lobjContactTicket.icdoContactTicket.istrOrgCodeID = lobjContactTicket.ibusOrganization.icdoOrganization.org_code;
                lobjContactTicket.ibusBenefitEstimate.FindBenefitEstimateByContactTicket(aintContactTicketId);
                lobjContactTicket.ibusBenefitEstimate.LoadRetirementType();
            }
            return lobjContactTicket;
        }

        public busContactTicket FindMSSAppointmentSchedule(int aintContactTicketId)
        {
            busContactTicket lobjContactTicket = new busContactTicket { icdoContactTicket = new cdoContactTicket() };
            lobjContactTicket.ibusAppointmentSchedule = new busAppointmentSchedule();

            if (lobjContactTicket.FindContactTicket(aintContactTicketId))
            {
                lobjContactTicket.iblnIsFromPortal = true;
                //Backlog PIR 12832          
                lobjContactTicket.iblnIsFromSeminarSchedule = true;
                lobjContactTicket.iblnIsFromMSS = true;
                lobjContactTicket.LoadPerson();
                lobjContactTicket.ibusAppointmentSchedule.FindAppointmentScheduleByContactTicket(aintContactTicketId);
            }
            return lobjContactTicket;
        }

        public busContactTicket FindESSAppointmentSchedule(int aintContactTicketId)
        {
            busContactTicket lobjContactTicket = new busContactTicket { icdoContactTicket = new cdoContactTicket() };
            lobjContactTicket.ibusAppointmentSchedule = new busAppointmentSchedule();

            if (lobjContactTicket.FindContactTicket(aintContactTicketId))
            {
                lobjContactTicket.iblnIsFromPortal = true;
                lobjContactTicket.iblnIsFromSeminarSchedule = true;
                lobjContactTicket.iblnIsFromESS = true;
                lobjContactTicket.LoadOrganization();
                lobjContactTicket.icdoContactTicket.istrOrgCodeID = lobjContactTicket.ibusOrganization.icdoOrganization.org_code;
                lobjContactTicket.ibusAppointmentSchedule.FindAppointmentScheduleByContactTicket(aintContactTicketId);
            }
            return lobjContactTicket;
        }

        public busContactTicket FindMSSContactMGMTServicePurchase(int aintContactTicketId)
        {
            busContactTicket lobjContactTicket = new busContactTicket { icdoContactTicket = new cdoContactTicket() };
            lobjContactTicket.ibusContactMgmtServicePurchase = new busContactMgmtServicePurchase();
            lobjContactTicket.ibusContactMgmtServicePurchase.iclcSerPurRolloverInfo = new utlCollection<cdoSerPurRolloverInfo>();
            if (lobjContactTicket.FindContactTicket(aintContactTicketId))
            {
                lobjContactTicket.iblnIsFromPortal = true;
                lobjContactTicket.LoadPerson();
                lobjContactTicket.ibusContactMgmtServicePurchase.FindServicePurchaseByContactTicket(aintContactTicketId);
                lobjContactTicket.ibusContactMgmtServicePurchase.LoadServiceRollInfo();
                lobjContactTicket.ibusContactMgmtServicePurchase.LoadSerPurServiceTypeAfterInserting();
            }
            return lobjContactTicket;
        }

        public busContactTicket FindESSContactMGMTServicePurchase(int aintContactTicketId)
        {
            busContactTicket lobjContactTicket = new busContactTicket { icdoContactTicket = new cdoContactTicket() };
            lobjContactTicket.ibusContactMgmtServicePurchase = new busContactMgmtServicePurchase();
            lobjContactTicket.ibusContactMgmtServicePurchase.iclcSerPurRolloverInfo = new utlCollection<cdoSerPurRolloverInfo>();
            if (lobjContactTicket.FindContactTicket(aintContactTicketId))
            {
                lobjContactTicket.iblnIsFromPortal = true;
                lobjContactTicket.LoadOrganization();
                lobjContactTicket.ibusContactMgmtServicePurchase.FindServicePurchaseByContactTicket(aintContactTicketId);
                lobjContactTicket.ibusContactMgmtServicePurchase.LoadServiceRollInfo();
                lobjContactTicket.ibusContactMgmtServicePurchase.LoadSerPurServiceTypeAfterInserting();
            }
            return lobjContactTicket;
        }

        public busContactTicket FindMSSSeminarSchedule(int aintContactTicketId, int aintPersonID)
        {
            busContactTicket lbusContactTicket = new busContactTicket { icdoContactTicket = new cdoContactTicket() };
            lbusContactTicket.ibusSeminarSchedule = new busSeminarSchedule();
            if (lbusContactTicket.FindContactTicket(aintContactTicketId))
            {
                lbusContactTicket.ibusSeminarSchedule.FindSeminarScheduleByContactTicket(aintContactTicketId);
                lbusContactTicket.iblnIsFromPortal = true;
                lbusContactTicket.ibusSeminarSchedule.LoadMSSSeminarAttendeeDetail(aintPersonID);
                lbusContactTicket.LoadSeminarTicketStatus();
                lbusContactTicket.LoadSeminarFacilitatorName();
            }
            return lbusContactTicket;
        }

        public busContactTicket FindESSSeminarSchedule(int aintContactTicketId, int aintOrgId)
        {
            busContactTicket lbusContactTicket = new busContactTicket { icdoContactTicket = new cdoContactTicket() };
            lbusContactTicket.ibusSeminarSchedule = new busSeminarSchedule();
            if (lbusContactTicket.FindContactTicket(aintContactTicketId))
            {
                lbusContactTicket.ibusSeminarSchedule.FindSeminarScheduleByContactTicket(aintContactTicketId);
                lbusContactTicket.iblnIsFromPortal = true;
                //prod pir 7844
                lbusContactTicket.ibusSeminarSchedule.icdoSeminarSchedule.org_id = aintOrgId;
                //PIR 13381 Removed the grid.
                //lbusContactTicket.ibusSeminarSchedule.LoadESSSeminarAttendeeDetail();
                lbusContactTicket.LoadOrganization();
                lbusContactTicket.LoadSeminarTicketStatus();
                lbusContactTicket.LoadSeminarFacilitatorName();
            }
            return lbusContactTicket;
        }

        public busSeminarAttendeeDetail FindMSSSeminarAttendeeDetail(int aintSeminarAttendeeDetailId)
        {
            busSeminarAttendeeDetail lbusSeminarAttendeeDetail = new busSeminarAttendeeDetail { icdoSeminarAttendeeDetail = new cdoSeminarAttendeeDetail() };
            if (lbusSeminarAttendeeDetail.FindSeminarAttendeeDetail(aintSeminarAttendeeDetailId))
            {
                lbusSeminarAttendeeDetail.iblnIsFromPortal = true;
                lbusSeminarAttendeeDetail.LoadSeminarSchedule();
                lbusSeminarAttendeeDetail.ibusSeminarSchedule.LoadContactTicket();
                lbusSeminarAttendeeDetail.ibusSeminarSchedule.ibusContactTicket.ibusSeminarSchedule = lbusSeminarAttendeeDetail.ibusSeminarSchedule;
                lbusSeminarAttendeeDetail.ibusSeminarSchedule.ibusContactTicket.iblnIsFromPortal = true;
                lbusSeminarAttendeeDetail.LoadSponsorName();
                lbusSeminarAttendeeDetail.LoadPerson();
                lbusSeminarAttendeeDetail.LoadRetirementType();
                lbusSeminarAttendeeDetail.LoadOrganization();
                lbusSeminarAttendeeDetail.LoadOrgToBillOrganization();
            }
            return lbusSeminarAttendeeDetail;
        }

        public busSeminarAttendeeDetail FindESSSeminarAttendeeDetail(int aintSeminarAttendeeDetailId)
        {
            busSeminarAttendeeDetail lbusSeminarAttendeeDetail = new busSeminarAttendeeDetail { icdoSeminarAttendeeDetail = new cdoSeminarAttendeeDetail() };
            if (lbusSeminarAttendeeDetail.FindSeminarAttendeeDetail(aintSeminarAttendeeDetailId))
            {
                lbusSeminarAttendeeDetail.iblnIsFromPortal = true;
                lbusSeminarAttendeeDetail.LoadSeminarSchedule();
                lbusSeminarAttendeeDetail.ibusSeminarSchedule.LoadContactTicket();
                lbusSeminarAttendeeDetail.ibusSeminarSchedule.ibusContactTicket.iblnIsFromPortal = true;
                lbusSeminarAttendeeDetail.LoadSponsorName();
                lbusSeminarAttendeeDetail.LoadPerson();
                lbusSeminarAttendeeDetail.LoadContact();
                lbusSeminarAttendeeDetail.LoadRetirementType();
                lbusSeminarAttendeeDetail.LoadOrganization();
                lbusSeminarAttendeeDetail.LoadOrgToBillOrganization();
            }
            return lbusSeminarAttendeeDetail;
        }
        #endregion

        #region UCS 32
        public busEmployerPayrollHeader NewEmployerPayrollHeaderESS(int aintOrgID, int aintContactID, string astrBenefitType, DateTime adtPayPeriodDate, string astrRecordType
            , DateTime adtPayPeriodStartDate, DateTime adtPayPeriodEndDate, DateTime adtPayCheckDate, string astrRecordTypeBonus, string astrIsDetailPopulated, string astrIsDetailPopulatedRet)
        {
            //If BenefitType  = retirement then set original property i.e astrIsDetailPopulated = astrIsDetailPopulatedRet
            if (astrBenefitType == busConstant.PayrollHeaderBenefitTypeRtmt && !string.IsNullOrEmpty(astrIsDetailPopulatedRet))
                astrIsDetailPopulated = astrIsDetailPopulatedRet;

            busEmployerPayrollHeader lobjEmployerPayrollHeader = new busEmployerPayrollHeader();
            lobjEmployerPayrollHeader.icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader();
            lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.org_id = aintOrgID;
            lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.contact_id = aintContactID;
            lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.status_value = busConstant.PayrollHeaderStatusPending;
            lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1209, lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.status_value);
            lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.reporting_source_value = busConstant.PayrollHeaderReportingSourcePaperRpt;
            lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.balancing_status_value = busConstant.PayrollHeaderBalancingStatusNoRemittance;
            lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value = astrBenefitType;
            lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.astrRecordTypeBonus = astrRecordTypeBonus;
            //if (string.IsNullOrEmpty(astrRecordType) && astrRecordTypeBonus == busConstant.PayrollDetailRecordTypeBonus)
            //{
            //    lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.report_type_value = busConstant.PayrollHeaderReportTypeAdjustment;
            //}
            //if (string.IsNullOrEmpty(astrRecordType) && astrRecordTypeBonus != busConstant.PayrollDetailRecordTypeBonus)
            //{
            //    lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.report_type_value = astrRecordTypeBonus;
            //}
            // setting the report type for retirement and deferred compensation
            if (!string.IsNullOrEmpty(astrRecordType) && astrBenefitType == busConstant.PayrollHeaderBenefitTypeDefComp)
            {
                lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.report_type_value = astrRecordType;
            }
            if (!string.IsNullOrEmpty(astrRecordTypeBonus) && astrBenefitType == busConstant.PayrollHeaderBenefitTypeRtmt)
            {
                if (astrRecordTypeBonus == busConstant.PayrollDetailRecordTypeBonus)
                {
                    lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.report_type_value = busConstant.PayrollHeaderReportTypeAdjustment;
                }
                else
                {
                    lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.report_type_value = astrRecordTypeBonus;
                }
            }
            //internal finding - Service purchase getting error on processing if report type is not set
            if (astrBenefitType == busConstant.PayrollHeaderBenefitTypePurchases)
            {
                lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.report_type_value = busConstant.PayrollHeaderReportTypeRegular;
            }
            if (lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.report_type_value == busConstant.PayrollHeaderReportTypeAdjustment)
            {
                lobjEmployerPayrollHeader.astrIsDetailPopulated = astrIsDetailPopulated;
            }
            else
            {
                lobjEmployerPayrollHeader.astrIsDetailPopulated = busConstant.Flag_Yes_CAPS;
            }
            lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.report_type_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1206, lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.report_type_value);
            lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1212, lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value);
            lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.received_date = DateTime.Now;
            lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.submitted_date = DateTime.Now;
            if (lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
            {
                lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.pay_period_start_date = adtPayPeriodStartDate;
                lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.pay_period_end_date = adtPayPeriodEndDate;
                lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.pay_check_date = adtPayCheckDate;
            }
            else
            {
                lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.payroll_paid_date = adtPayPeriodDate;
            }
            if (adtPayPeriodDate != DateTime.MinValue)
            {
                lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.pay_period = lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.payroll_paid_date.ToString("MM/yyyy", System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
            }
            lobjEmployerPayrollHeader.LoadOrganization();
            lobjEmployerPayrollHeader.istrOrgCodeId = lobjEmployerPayrollHeader.ibusOrganization.icdoOrganization.org_code;
            lobjEmployerPayrollHeader.LoadOrgContact();
            lobjEmployerPayrollHeader.iblnIsFromESS = true;
            if (lobjEmployerPayrollHeader.astrIsDetailPopulated == busConstant.Flag_Yes_CAPS)
            {
                lobjEmployerPayrollHeader.LoadActiveMemberForHeader();
                lobjEmployerPayrollHeader.AssignEmployerPayrollDetailID(); //PIR 15448
                lobjEmployerPayrollHeader.iclbEmployerPayrollDetail.ForEach(o => o.aintOrgId = aintOrgID);
            }

            if (astrRecordTypeBonus == busConstant.PayrollDetailRecordTypeBonus && astrBenefitType == busConstant.PayrollHeaderBenefitTypeRtmt)
            {
                if (lobjEmployerPayrollHeader.astrIsDetailPopulated == busConstant.Flag_Yes_CAPS)
                {
                    lobjEmployerPayrollHeader.iclbEmployerPayrollDetail.ForEach(o => o.icdoEmployerPayrollDetail.record_type_value = astrRecordTypeBonus);
                    lobjEmployerPayrollHeader.iclbEmployerPayrollDetail.ForEach(o => o.icdoEmployerPayrollDetail.PopulateDescriptions());
                }
                lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.report_type_description = "Bonus/Retro Pay";
                // Bonus/Retro Pay
            }
            //PIR 13275 - The message should only be shown when employer offers Only Other457 and employer selects no in is this information correct drop down.
            if (lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp
                && lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.report_type_value == busConstant.PayrollHeaderReportTypeRegular &&
                lobjEmployerPayrollHeader.iclbEmployerPayrollDetail.Count == 0)
            {
                if (lobjEmployerPayrollHeader.ibusOrganization.iclbOrgPlan.IsNull())
                    lobjEmployerPayrollHeader.ibusOrganization.LoadOrgPlan();
                var lenuOfferedPlans = lobjEmployerPayrollHeader.ibusOrganization.iclbOrgPlan.Where(i => i.icdoOrgPlan.plan_id == busConstant.PlanIdDeferredCompensation || i.icdoOrgPlan.plan_id == busConstant.PlanIdOther457)
                                                .Where(i => i.icdoOrgPlan.participation_end_date == DateTime.MinValue || i.icdoOrgPlan.participation_end_date > DateTime.Now);
                if (lenuOfferedPlans.Count() == 1 && lenuOfferedPlans.Where(i => i.icdoOrgPlan.plan_id == busConstant.PlanIdOther457).Count() == 1)
                {
                    lobjEmployerPayrollHeader.istrOnlyOther457Text = busConstant.EmployerOfferingOnly457text;
                }

            }
            //PIR 13852 - Do not prepopulate Reporting Month for Adjustments or Bonus - start
            if (lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt
                && lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.report_type_value != busConstant.PayrollHeaderReportTypeRegular
                && lobjEmployerPayrollHeader.iclbEmployerPayrollDetail.IsNotNull() && lobjEmployerPayrollHeader.iclbEmployerPayrollDetail.Count > 0)
            {
                lobjEmployerPayrollHeader.iclbEmployerPayrollDetail.ForEach(i => i.icdoEmployerPayrollDetail.pay_period = string.Empty);
            }
            lobjEmployerPayrollHeader.LoadPlanForOrganization();
            //PIR 13852 - Do not prepopulate Reporting Month for Adjustments or Bonus - end
            lobjEmployerPayrollHeader.EvaluateInitialLoadRules();
            lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.ienuObjectState = ObjectState.Insert;
            lobjEmployerPayrollHeader.iarrChangeLog.Add(lobjEmployerPayrollHeader.icdoEmployerPayrollHeader);
            return lobjEmployerPayrollHeader;
        }
        public busEmployerPayrollHeader FindEmployerPayrollHeaderESS(int Aintemployerpayrollheaderid, int aintIsReload)
        {
            busEmployerPayrollHeader lobjEmployerPayrollHeader = new busEmployerPayrollHeader();
            if (lobjEmployerPayrollHeader.FindEmployerPayrollHeader(Aintemployerpayrollheaderid))
            {
                if (lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value.Equals("INSR") &&
                    lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.report_type_value.Equals("REG") &&
                    !lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.status_value.Equals("IGNR") &&
                    !lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.status_value.Equals("PSTD") &&
                    //PIR - 13981 - Insurance headers - if flowservice does not pick ready to post headers, reload method makes header status valid again
                    !lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.status_value.Equals("RPST") &&
                    aintIsReload.Equals(1))
                {
                    aintIsReload = 0;
                    lobjEmployerPayrollHeader.ReloadInsurance();
                }
                lobjEmployerPayrollHeader.LoadEmployerPayrollDetail();
                lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.total_detail_record_count = lobjEmployerPayrollHeader.iclbEmployerPayrollDetail.Count;
                lobjEmployerPayrollHeader.LoadOrganization();
                lobjEmployerPayrollHeader.istrOrgCodeId = lobjEmployerPayrollHeader.ibusOrganization.icdoOrganization.org_code;
                //lobjEmployerPayrollHeader.LoadEmployerRemittanceAllocation(); //PIr-18736
                //lobjEmployerPayrollHeader.LoadAvailableRemittanace();

                if (lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
                {
                    lobjEmployerPayrollHeader.LoadRetirementContributionByPlan();
                    //lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.total_wages_from_details = lobjEmployerPayrollHeader.iclbEmployerPayrollDetail.Sum(i => i.icdoEmployerPayrollDetail.eligible_wages);
                    //lobjEmployerPayrollHeader.LoadOriginalWagesAndContribution(); //18736
                }
                else if (lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
                {
                    lobjEmployerPayrollHeader.LoadESSDeferredCompContributionByPlan();
                }
                else if (lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeInsr)
                {
                    lobjEmployerPayrollHeader.LoadESSInsurancePremiumByPlan();
                    //prod pir 4194
                    //lobjEmployerPayrollHeader.LoadOtherInsuranceObjectsForCorr();// System out of Memory Exception- code optmized
                    lobjEmployerPayrollHeader.LoadLifeSplitUpPremiumAmounts();// System out of Memory Exception- code optmized
                }
                else if (lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypePurchases)
                {
                    lobjEmployerPayrollHeader.LoadPurchaseByPlan();
                }

                lobjEmployerPayrollHeader.LoadTotalAppliedRemittance();
                if (lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.payroll_paid_date != DateTime.MinValue)
                {
                    lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.pay_period = lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.payroll_paid_date.ToString("MM/yyyy", CultureInfo.CreateSpecificCulture("en-US"));
                }
                else
                {
                    lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.pay_period = String.Empty;
                }
                //PROD PIR 4373
                //--Start --//
                //lobjEmployerPayrollHeader.LoadStatusSummaryESS();

                lobjEmployerPayrollHeader.LoadStatusSummary();
                lobjEmployerPayrollHeader.LoadDetailErrorFromESS();

                //lobjEmployerPayrollHeader.LoadEmployerPayrollHeaderError();
                lobjEmployerPayrollHeader.CalculateContributionWagesInterestFromDtl();
                if (lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
                    lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.total_contributions_from_details = lobjEmployerPayrollHeader.idecTotalContributionFromDetails;
                if (lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
                    lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.total_contributions_from_details = lobjEmployerPayrollHeader.idecTotalContributionCalculatedForDef;
                lobjEmployerPayrollHeader.iblnIsFromESS = true;
                // ESS Backlog PIR - 13416
                lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.comments = String.Empty;
                lobjEmployerPayrollHeader.LoadEmployerPayrollHeaderComments();
                //lobjEmployerPayrollHeader.LoadDetailComments();
            }
            return lobjEmployerPayrollHeader;
        }
        
        public busEmployerPayrollDetail NewEmployerPayrollDetail(int aintEmployerPayrollHeaderId)
        {
            busEmployerPayrollDetail lobjPayrollDetail = new busEmployerPayrollDetail();
            lobjPayrollDetail.icdoEmployerPayrollDetail = new cdoEmployerPayrollDetail();
            lobjPayrollDetail.icdoEmployerPayrollDetail.employer_payroll_header_id = aintEmployerPayrollHeaderId;
            lobjPayrollDetail.iblnIsFromESS = true;
            lobjPayrollDetail.LoadPayrollHeader();
            lobjPayrollDetail.EvaluateInitialLoadRules();
            return lobjPayrollDetail;
        }

        public busEmployerPayrollDetail FindEmployerPayrollDetail(int Aintemployerpayrolldetailid)
        {
            busEmployerPayrollDetail lobjEmployerPayrollDetail = new busEmployerPayrollDetail();
            //To Avoid Null Issues in Smart Navigation
            lobjEmployerPayrollDetail.ibusEmployerPayrollHeader = new busEmployerPayrollHeader();
            lobjEmployerPayrollDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader();
            if (lobjEmployerPayrollDetail.FindEmployerPayrollDetail(Aintemployerpayrolldetailid))
            {
                lobjEmployerPayrollDetail.iblnIsFromESS = true;
                lobjEmployerPayrollDetail.LoadPayrollHeader();
                lobjEmployerPayrollDetail.LoadOrgCodeID();
                lobjEmployerPayrollDetail.LoadObjectsForValidation();
                lobjEmployerPayrollDetail.LoadErrors();
                lobjEmployerPayrollDetail.LoadEmployerPurchaseAllocation();
                lobjEmployerPayrollDetail.LoadEmployerPayrollDetailError();
                if (lobjEmployerPayrollDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.payroll_paid_date != DateTime.MinValue)
                {
                    lobjEmployerPayrollDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.pay_period = lobjEmployerPayrollDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.payroll_paid_date.ToString("MM/yyyy", CultureInfo.CreateSpecificCulture("en-US"));
                }
                else
                {
                    lobjEmployerPayrollDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.pay_period = String.Empty;
                }
                if (lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.pay_period_date != DateTime.MinValue)
                {
                    lobjEmployerPayrollDetail.pay_period = lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.pay_period_date.ToString("MM/yyyy", CultureInfo.CreateSpecificCulture("en-US"));
                }
                else
                {
                    lobjEmployerPayrollDetail.pay_period = String.Empty;
                }
                if (lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.pay_period_end_month_for_bonus != DateTime.MinValue)
                {
                    lobjEmployerPayrollDetail.pay_end_month = lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.pay_period_end_month_for_bonus.ToString("MM/yyyy", CultureInfo.CreateSpecificCulture("en-US"));
                }
                else
                {
                    lobjEmployerPayrollDetail.pay_end_month = String.Empty;
                }
                //for ESS load the provider names in case of def comp
                if (lobjEmployerPayrollDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
                {
                    lobjEmployerPayrollDetail.LoadOrgIdForProviders();
                }
                //PROD PIR 4373
                //lobjEmployerPayrollDetail.LoadEmployerErrorsForESS();
                lobjEmployerPayrollDetail.LoadPeoplesoftID();
                //if (lobjEmployerPayrollDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
                //{
                //    lobjEmployerPayrollDetail.LoadOriginalWages(string.Empty);
                //}
                lobjEmployerPayrollDetail.LoadNegativeComponents();
                // ESS Backlog PIR - 13416
                lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.comments = String.Empty;
                lobjEmployerPayrollDetail.LoadEmployerPayrollDetailComments();
                lobjEmployerPayrollDetail.LoadRetiremntPlanRate();
            }
            return lobjEmployerPayrollDetail;
        }

        public busEmployerPayrollHeaderLookup LoadEmployerPayrollHeaders(DataTable adtbSearchResult)
        {
            busEmployerPayrollHeaderLookup lobjEmployerPayrollHeaderLookup = new busEmployerPayrollHeaderLookup();
            //PIR 24525
            if (iobjPassInfo.istrFormName == "wfmESSEmployerPayrollHeaderHomeLookup" || iobjPassInfo.istrFormName == "wfmESSRetirementEmployerPayrollHeaderLookup" || iobjPassInfo.istrFormName == "wfmESSInsuranceEmployerPayrollHeaderLookup")
            {
                DataTable ldtSearchResult = adtbSearchResult.AsEnumerable().Where(i => !(i.Field<string>(enmEmployerPayrollHeader.report_type_value.ToString()) == busConstant.PayrollHeaderReportTypePenalty && i.Field<string>(enmEmployerPayrollHeader.status_value.ToString()) != busConstant.PayrollHeaderStatusPosted)).AsDataTable();
                lobjEmployerPayrollHeaderLookup.LoadEmployerPayrollHeader(ldtSearchResult);
            }
            else
            {
                lobjEmployerPayrollHeaderLookup.LoadEmployerPayrollHeader(adtbSearchResult);
            }
            lobjEmployerPayrollHeaderLookup.LoadCommentsOfPayrollHeaders(); // ESS Backlog PIR - 13416
            return lobjEmployerPayrollHeaderLookup;
        }
        public busEmployerPayrollDetailLookup LoadPayrollDetail(DataTable adtbSearchResult)
        {
            busEmployerPayrollDetailLookup lobjPayrollDetailLookup = new busEmployerPayrollDetailLookup();
            lobjPayrollDetailLookup.LoadPayrollDetail(adtbSearchResult);
            lobjPayrollDetailLookup.LoadCommentsOfPayrollDetails(); // ESS Backlog PIR - 13416
            return lobjPayrollDetailLookup;
        }
        #endregion
        public busEmployerPayrollMonthlyStatement ESSFindEmployerPayrollMonthlyStatement(int aintOrgID)
        {
            /*
             * FMUpgrade: Changes for set Navigation parameter from menu item click
             */
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmESSAgencyStatementMaintenance")
            {
                aintOrgID = aintOrgID != 0 ? aintOrgID : iintOrgID;
            }
            busEmployerPayrollMonthlyStatement lobjEmployerPayrollMonthlyStatement = new busEmployerPayrollMonthlyStatement();
            lobjEmployerPayrollMonthlyStatement.iblnIsFromESS = true;
            lobjEmployerPayrollMonthlyStatement.OrgID = aintOrgID;
            lobjEmployerPayrollMonthlyStatement.LoadAgencyStatementInfo(aintOrgID);
            return lobjEmployerPayrollMonthlyStatement;
        }

        public busPersonAccountMissedDeposit ESSFindMissedDeposit(int aintPersonAccountID)
        {
            busPersonAccountMissedDeposit lobjPersonAccountMissedDeposit = new busPersonAccountMissedDeposit { icdoPersonAccountMissedDeposit = new cdoPersonAccountMissedDeposit() };
            lobjPersonAccountMissedDeposit.icdoPersonAccountMissedDeposit.person_account_id = aintPersonAccountID;
            lobjPersonAccountMissedDeposit.LoadPersonAccount();
            lobjPersonAccountMissedDeposit.ibusPersonAccountRetirement.LoadPerson();
            lobjPersonAccountMissedDeposit.ibusPersonAccountRetirement.LoadLatestEmployment();
            lobjPersonAccountMissedDeposit.ibusPersonAccountRetirement.LoadMissedDeposits();
            return lobjPersonAccountMissedDeposit;
        }
        #region Upload files

        //  FMUpgrade:Added for Default page conversion of FrameworkInit method
        public Collection<cdoCodeValue> FilterFileTypesByContactRole()
        {
            SetWebParameters();
            Collection<cdoCodeValue> lclbCodeValue = new Collection<cdoCodeValue>();
            DataTable ldtblResult;
            if (iobjPassInfo.istrFormName == "wfmCreatePayrollMaintenance")
            {
                ldtblResult = busNeoSpinBase.Select("cdoFile.LookupESSInboundFiles", new object[2] { iintOrgID, iintContactID });
            }
            else if (iobjPassInfo.istrFormName == "wfmESSFileHdrLookup" || iobjPassInfo.istrFormName == "wfmUploadFileMaintenance")
            {
                ldtblResult = busNeoSpinBase.Select("cdoFile.LookupESSInboundFilesUploadd", new object[2] { iintOrgID, iintContactID });
            }
            else
            {
                ldtblResult = busNeoSpinBase.Select("cdoFile.LookupESSInboundFilesUpload", new object[2] { iintOrgID, iintContactID });
            }

            lclbCodeValue = Sagitec.DataObjects.doBase.GetCollection<cdoCodeValue>(ldtblResult);
            return lclbCodeValue;
            //    return DBFunction.DBSelect("cdoFile.LookupESSInboundFiles", new object[2] { aintOrgID, aintContactID },
            //                                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
        }



        //public DataTable FilterHeaderTypesByContactRole(int aintOrgID, int aintContactID)
        //{
        //    return DBFunction.DBSelect("cdoFile.LoadHeaderTypeForESS", new object[2] { aintOrgID, aintContactID },
        //                                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
        //}
        public Collection<cdoCodeValue> FilterHeaderTypesByContactRole()
        {
            SetWebParameters();
            Collection<cdoCodeValue> lclbCodeValue = new Collection<cdoCodeValue>();
            DataTable ldtblResult = DBFunction.DBSelect("cdoFile.LoadHeaderTypeForESS", new object[2] { iintOrgID, iintContactID },
                                        iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            //busBase lbusBase = new busBase();
            //lclbCodeValue = lbusBase.GetCollection<busCodeValue>(ldtblResult, "icdoCodeValue");
            lclbCodeValue = Sagitec.DataObjects.doBase.GetCollection<cdoCodeValue>(ldtblResult);
            return lclbCodeValue;
        }




        #endregion

        public busWssDebitAchRequestDetail FindWssDebitAchRequestDetail(int aintdebitachrequestdtlid)
        {
            busWssDebitAchRequestDetail lobjWssDebitAchRequestDetail = new busWssDebitAchRequestDetail();
            if (lobjWssDebitAchRequestDetail.FindWssDebitAchRequestDetail(aintdebitachrequestdtlid))
            {
                lobjWssDebitAchRequestDetail.LoadWssDebitAchRequest();
            }

            return lobjWssDebitAchRequestDetail;
        }

        public busWSSMessageHeaderLookup LoadWSSMessageHeaders(DataTable adtbSearchResult)
        {
            busWSSMessageHeaderLookup lobjWSSMessageHeaderLookup = new busWSSMessageHeaderLookup();
            lobjWSSMessageHeaderLookup.LoadWssMessageHeaders(adtbSearchResult);
            return lobjWSSMessageHeaderLookup;
        }
        public busFileHdrLookup LoadESSFileHdrs(DataTable adtbSearchResult)
        {
            busFileHdrLookup lobjFileHdrLookup = new busFileHdrLookup();
            lobjFileHdrLookup.LoadFileHdrs(adtbSearchResult);
            return lobjFileHdrLookup;
        }

        public busWssPersonAccountEnrollmentRequest FindWssPersonAccountEnrollmentRequest(int aintwsspersonaccountenrollmentrequestid)
        {
            busWssPersonAccountEnrollmentRequest lobjWssPersonAccountEnrollmentRequest = new busWssPersonAccountEnrollmentRequest();
            if (lobjWssPersonAccountEnrollmentRequest.FindWssPersonAccountEnrollmentRequest(aintwsspersonaccountenrollmentrequestid))
            {
                lobjWssPersonAccountEnrollmentRequest.LoadPerson();
            }

            return lobjWssPersonAccountEnrollmentRequest;
        }

        public busWSSPersonAccountEnrollmentRequestLookup LoadWSSPersonAccountEnrollmentRequests(DataTable adtbSearchResult)
        {
            busWSSPersonAccountEnrollmentRequestLookup lobjWSSPersonAccountEnrollmentRequestLookup = new busWSSPersonAccountEnrollmentRequestLookup();
            lobjWSSPersonAccountEnrollmentRequestLookup.LoadWssPersonAccountEnrollmentRequests(adtbSearchResult);
            return lobjWSSPersonAccountEnrollmentRequestLookup;
        }


        public busWssMemberRecordRequest FindWssMemberRecordRequest(int aintmemberrecordrequestid)
        {
            busWssMemberRecordRequest lobjWssMemberRecordRequest = new busWssMemberRecordRequest();
            if (lobjWssMemberRecordRequest.FindWssMemberRecordRequest(aintmemberrecordrequestid))
            {
            }

            return lobjWssMemberRecordRequest;
        }


        public busWssPersonAddress FindWssPersonAddress(int aintwsspersonaddressid)
        {
            busWssPersonAddress lobjWssPersonAddress = new busWssPersonAddress();
            if (lobjWssPersonAddress.FindWssPersonAddress(aintwsspersonaddressid))
            {
            }

            return lobjWssPersonAddress;
        }

        public busWssPersonContact FindWssPersonContact(int aintwsspersoncontactid)
        {
            busWssPersonContact lobjWssPersonContact = new busWssPersonContact();
            if (lobjWssPersonContact.FindWssPersonContact(aintwsspersoncontactid))
            {
            }

            return lobjWssPersonContact;
        }

        public busWssPersonEmployment FindWssPersonEmployment(int aintwsspersonemploymentid)
        {
            busWssPersonEmployment lobjWssPersonEmployment = new busWssPersonEmployment();
            if (lobjWssPersonEmployment.FindWssPersonEmployment(aintwsspersonemploymentid))
            {
            }

            return lobjWssPersonEmployment;
        }

        public busWssPersonEmploymentDetail FindWssPersonEmploymentDetail(int aintwsspersonemploymentdtlid)
        {
            busWssPersonEmploymentDetail lobjWssPersonEmploymentDetail = new busWssPersonEmploymentDetail();
            if (lobjWssPersonEmploymentDetail.FindWssPersonEmploymentDetail(aintwsspersonemploymentdtlid))
            {
            }

            return lobjWssPersonEmploymentDetail;
        }

        //RA 
        protected override void InitializeNewChildObject(object aobjParentObject, busBase aobjChildObject)
        {
            if (aobjChildObject is busPersonAccountDeferredCompProvider)
            {
                if (aobjParentObject != null)
                {
                    busPersonAccountDeferredCompProvider lbusMssDefCompProvider = (busPersonAccountDeferredCompProvider)aobjChildObject;
                    busMSSDefCompWeb lbusMssDefCompWeb = (busMSSDefCompWeb)aobjParentObject;
                    lbusMssDefCompProvider.ibusPersonAccountDeferredComp = lbusMssDefCompWeb;
                }
            }
            if (aobjChildObject is busPersonContact)
            {
                if (aobjParentObject != null)
                {
                    busPersonContact lbusMssPersonContact = (busPersonContact)aobjChildObject;
                    busMaritalStatusChangeWeb lbusMssMaritalChange = (busMaritalStatusChangeWeb)aobjParentObject;
                    lbusMssPersonContact.ibusPerson = lbusMssMaritalChange.ibusPerson;
                }
            }
            if (aobjParentObject is busEmployerPayrollHeader && aobjChildObject is busEmployerPayrollDetail)
            {
                busEmployerPayrollHeader lbusEmployerPayrollHeader = (busEmployerPayrollHeader)aobjParentObject;
                busEmployerPayrollDetail lbusEmployerPayrollDetail = (busEmployerPayrollDetail)aobjChildObject;
                // Code Added & Commented for performance optimization
                //lbusEmployerPayrollDetail.ibusEmployerPayrollHeader = lbusEmployerPayrollHeader;
                lbusEmployerPayrollDetail.ldtbPlan = lbusEmployerPayrollHeader.iclbPlan;
                lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.employer_payroll_header_id = lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.employer_payroll_header_id;
                lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.employer_payroll_detail_id = lbusEmployerPayrollHeader.aintIndexEmployer_payroll_detail_id;
                lbusEmployerPayrollHeader.aintIndexEmployer_payroll_detail_id = lbusEmployerPayrollHeader.aintIndexEmployer_payroll_detail_id + 1;
                lbusEmployerPayrollDetail.aintOrgId = lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.org_id;
                lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.record_type_value = lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.report_type_value;
                lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.status_value = busConstant.PayrollDetailStatusReview;
                if (lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt ||
                    lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypePurchases)
                {
                    lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.pay_period_date = lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.payroll_paid_date;
                    //This line is commented as part of PIR 13852 fix - Do not prepopulate Reporting Month for Adjustments or Bonus
                    //lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.pay_period = lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.pay_period;
                }
                if (lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
                {
                    lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.pay_period_start_date = lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.pay_period_start_date;
                    lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.pay_period_end_date = lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.pay_period_end_date;
                    lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.pay_check_date = lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.pay_check_date;
                }

            }
            if (aobjParentObject is busWssPersonAccountEnrollmentRequest && aobjChildObject is busWssPersonDependent)
            {
                if (aobjParentObject != null && aobjChildObject != null)
                {
                    busWssPersonAccountEnrollmentRequest lbusWssPersonAccountEnrollmentRequest = (busWssPersonAccountEnrollmentRequest)aobjParentObject;
                    busWssPersonDependent lbusWssPersonDependent = (busWssPersonDependent)aobjChildObject;
                    lbusWssPersonDependent.icdoWssPersonDependent.effective_start_date = lbusWssPersonAccountEnrollmentRequest.GetDateofChange();
                    lbusWssPersonDependent.icdoWssPersonDependent.level_of_coverage_value = lbusWssPersonAccountEnrollmentRequest.icdoMSSGDHV.level_of_coverage_value;
                    //F/W Upgrade PIR 11798 - System throwing hard errors because of personid not being passed
                    lbusWssPersonDependent.icdoWssPersonDependent.iintPersonID = lbusWssPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.person_id;
                    lbusWssPersonDependent.icdoWssPersonDependent.iintPlanId = lbusWssPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.plan_id;
                }
            }
            if (aobjParentObject is busWssBenApp && aobjChildObject is busWssPersonDependent)
            {
                busWssBenApp lbusWssBenApp = (busWssBenApp)aobjParentObject;
                busWssPersonDependent lbusWssPersonDependent = (busWssPersonDependent)aobjChildObject;
                lbusWssPersonDependent.icdoWssPersonDependent.iblnIsHealthEnrltAsCobra = lbusWssBenApp.iblnIsHealthEnrltAsCobra;
                lbusWssPersonDependent.icdoWssPersonDependent.iblnIsDentalEnrltAsCobra = lbusWssBenApp.iblnIsDentalEnrltAsCobra;
                lbusWssPersonDependent.icdoWssPersonDependent.iblnIsVisionEnrltAsCobra = lbusWssBenApp.iblnIsVisionEnrltAsCobra;
                lbusWssPersonDependent.icdoWssPersonDependent.iblnIsHealthEnrltAsRetiree = lbusWssBenApp.iblnIsHealthEnrltAsRetiree;
                lbusWssPersonDependent.icdoWssPersonDependent.iblnIsDentalEnrltAsRetiree = lbusWssBenApp.iblnIsDentalEnrltAsRetiree;
                lbusWssPersonDependent.icdoWssPersonDependent.iblnIsVisionEnrltAsRetiree = lbusWssBenApp.iblnIsVisionEnrltAsRetiree;

                lbusWssPersonDependent.ibusPerson = lbusWssBenApp.ibusMemberPerson;
                //lbusWssPersonDependent.icdoWssPersonDependent.relationship_value = busConstant.FamilyRelationshipSpouse;

                //Health
                if (lbusWssBenApp.iblnIsHealthStepVisible && lbusWssBenApp.ibusPAEnrollReqHealth.icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueEnroll)
                {
                    if (lbusWssBenApp.ibusPAEnrollReqHealth.icdoMSSGDHV.level_of_coverage_value == busConstant.VisionLevelofCoverageFamily)
                        lbusWssPersonDependent.icdoWssPersonDependent.iblnHealthCovEnrollFamily = true;
                }
                //Dental
                if (lbusWssBenApp.iblnIsDentalStepVisible && lbusWssBenApp.ibusPAEnrollReqDental.icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueEnroll)
                {
                    switch (lbusWssBenApp.ibusPAEnrollReqDental.icdoMSSGDHV.level_of_coverage_value)
                    {
                        case busConstant.DentalLevelofCoverageFamily:
                            lbusWssPersonDependent.icdoWssPersonDependent.iblnDentalCovEnrollFamily = true;
                            break;
                        case busConstant.DentalLevelofCoverageIndividualSpouse:
                            lbusWssPersonDependent.icdoWssPersonDependent.iblnDentalCovEnrollIndSpouse = true;
                            break;
                        case busConstant.DentalLevelofCoverageIndividualChild:
                            lbusWssPersonDependent.icdoWssPersonDependent.iblnDentalCovEnrollIndChild = true;
                            break;
                    }
                }
                //Vision
                if (lbusWssBenApp.iblnIsVisionStepVisible && lbusWssBenApp.ibusPAEnrollReqVision.icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueEnroll)
                {
                    switch (lbusWssBenApp.ibusPAEnrollReqVision.icdoMSSGDHV.level_of_coverage_value)
                    {
                        case busConstant.VisionLevelofCoverageFamily:
                            lbusWssPersonDependent.icdoWssPersonDependent.iblnVisionCovEnrollFamily = true;
                            break;
                        case busConstant.VisionLevelofCoverageIndividualSpouse:
                            lbusWssPersonDependent.icdoWssPersonDependent.iblnVisionCovEnrollIndSpouse = true;
                            break;
                        case busConstant.VisionLevelofCoverageIndividualChild:
                            lbusWssPersonDependent.icdoWssPersonDependent.iblnVisionCovEnrollIndChild = true;
                            break;
                    }
                }
                lbusWssPersonDependent.EvaluateInitialLoadRules();
            }
            base.InitializeNewChildObject(aobjParentObject, aobjChildObject);
        }

        public busWssMemberRecordRequest NewMemberRecordRequest(int aintOrgId)
        {
            /*
            * FMUpgrade: Changes for set Navigation parameter from menu item click - BeforeGetInitialData
            */
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmESSMemberRecordDataWizard" || iobjPassInfo.istrFormName == "wfmMemberRecordDataWizard")
            {
                aintOrgId = aintOrgId != 0 ? aintOrgId : iintOrgID;
            }
            busWssMemberRecordRequest lobjMemberRecordRequest = new busWssMemberRecordRequest
            {
                icdoWssMemberRecordRequest = new cdoWssMemberRecordRequest(),
                icdoWssPersonAddress = new cdoWssPersonAddress(),
                icdoWssPersonContact = new cdoWssPersonContact(),
                icdoWssPersonEmployment = new cdoWssPersonEmployment(),
                icdoWssPersonEmploymentDetail = new cdoWssPersonEmploymentDetail()
            };
            lobjMemberRecordRequest.icdoWssMemberRecordRequest.status_value = busConstant.EmploymentChangeRequestStatusReview;
            lobjMemberRecordRequest.icdoWssMemberRecordRequest.org_id = aintOrgId;
            lobjMemberRecordRequest.icdoWssMemberRecordRequest.contact_id = iintContactID;
            lobjMemberRecordRequest.LoadOrganization();
            lobjMemberRecordRequest.iblnIsFromESS = true;

            //PIR 24663
            if (!(lobjMemberRecordRequest.ibusOrganization.iblnIsOrgOfferingAnyRetPlan))
            {
                lobjMemberRecordRequest.icdoWssPersonEmploymentDetail.employment_status_value = busConstant.EmploymentStatusNonContributing;
            }
            return lobjMemberRecordRequest;
        }
        public busWssMemberRecordRequest FindMemberRecordRequest(int aintMemberRecordRequestID)
        {
            busWssMemberRecordRequest lobjMemberRecordRequest = new busWssMemberRecordRequest()
            {
                icdoWssMemberRecordRequest = new cdoWssMemberRecordRequest()
            };
            lobjMemberRecordRequest.ibusContact = new busContact() { icdoContact = new cdoContact() };
            lobjMemberRecordRequest.FindWssMemberRecordRequest(aintMemberRecordRequestID);
            lobjMemberRecordRequest.SetPerson();
            lobjMemberRecordRequest.SetContactPerson();
            lobjMemberRecordRequest.icdoWssMemberRecordRequest.org_id = lobjMemberRecordRequest.icdoWssPersonEmployment.org_id;
            lobjMemberRecordRequest.LoadOrganization();
            lobjMemberRecordRequest.LoadRejectionDetails();
            lobjMemberRecordRequest.DisplayWssMemberPlanEnrollmentStatus();
            //For PIR 7952
            lobjMemberRecordRequest.SetRequestedByDetails();
            lobjMemberRecordRequest.iblnIsFromESS = true;
            return lobjMemberRecordRequest;
        }
        public busWssEmploymentChangeRequest NewWssEmploymentRequest(int aintemploymentId, int aintemploymentdetailid, int aintplanid)
        {
            busWssEmploymentChangeRequest lbusWssEmploymentChangeRequest = new busWssEmploymentChangeRequest
            {
                icdoWssEmploymentChangeRequest = new cdoWssEmploymentChangeRequest()
            };
            lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.person_employment_id = aintemploymentId;
            lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.person_employment_detail_id = aintemploymentdetailid;
            lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.plan_id = aintplanid;
            lbusWssEmploymentChangeRequest.LoadEmployment();
            lbusWssEmploymentChangeRequest.LoadEmploymentDetail();
            lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.person_id = lbusWssEmploymentChangeRequest.ibusPersonEmployment.icdoPersonEmployment.person_id;
            lbusWssEmploymentChangeRequest.LoadPerson();
            lbusWssEmploymentChangeRequest.ibusPerson.iintOrgID = lbusWssEmploymentChangeRequest.ibusPersonEmployment.icdoPersonEmployment.org_id;
            lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.org_id = lbusWssEmploymentChangeRequest.ibusPersonEmployment.icdoPersonEmployment.org_id;
            lbusWssEmploymentChangeRequest.LoadOrganization();
            lbusWssEmploymentChangeRequest.ibusPerson.LoadESSEmploymentChangeRequestDetails();
            return lbusWssEmploymentChangeRequest;
        }
        public int GetEmploymentChangeRequestID(int aintemploymentId, int aintemploymentdetailid, int aintplanid)
        {
            busWssEmploymentChangeRequest lbusWssEmploymentChangeRequest = new busWssEmploymentChangeRequest
            {
                icdoWssEmploymentChangeRequest = new cdoWssEmploymentChangeRequest()
            };
            lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.person_employment_id = aintemploymentId;
            lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.person_employment_detail_id = aintemploymentdetailid;
            lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.plan_id = aintplanid;
            lbusWssEmploymentChangeRequest.LoadEmployment();
            lbusWssEmploymentChangeRequest.LoadEmploymentDetail();
            lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.person_id = lbusWssEmploymentChangeRequest.ibusPersonEmployment.icdoPersonEmployment.person_id;
            lbusWssEmploymentChangeRequest.LoadPerson();
            lbusWssEmploymentChangeRequest.ibusPerson.iintOrgID = lbusWssEmploymentChangeRequest.ibusPersonEmployment.icdoPersonEmployment.org_id;
            lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.org_id = lbusWssEmploymentChangeRequest.ibusPersonEmployment.icdoPersonEmployment.org_id;
            //lbusWssEmploymentChangeRequest.LoadOrganization();
            lbusWssEmploymentChangeRequest.ibusPerson.LoadESSEmploymentChangeRequestDetails();
            return lbusWssEmploymentChangeRequest.ibusPerson.iintTermEmploymentChangeRequestID;
        }
        public busWssEmploymentChangeRequest NewWssEmploymentChangeRequest(int aintemploymentId, int aintemploymentdetailid, int aintplanid, string astrchangetypevalue)
        {
            busWssEmploymentChangeRequest lbusWssEmploymentChangeRequest = new busWssEmploymentChangeRequest
            {
                icdoWssEmploymentChangeRequest = new cdoWssEmploymentChangeRequest()
            };
            if (astrchangetypevalue == busConstant.EmploymentChangeRequestChangeTypeLOA)
                lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.change_type_value = string.Empty;
            else
                lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.change_type_value = astrchangetypevalue;
            lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.person_employment_id = aintemploymentId;
            lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.person_employment_detail_id = aintemploymentdetailid;
            lbusWssEmploymentChangeRequest.LoadEmployment();
            lbusWssEmploymentChangeRequest.LoadEmploymentDetail();
            lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.person_id = lbusWssEmploymentChangeRequest.ibusPersonEmployment.icdoPersonEmployment.person_id;
            lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.org_id = lbusWssEmploymentChangeRequest.ibusPersonEmployment.icdoPersonEmployment.org_id;
            lbusWssEmploymentChangeRequest.LoadPerson();
            lbusWssEmploymentChangeRequest.ibusPerson.LoadPlanEnrollmentVisibility();
            lbusWssEmploymentChangeRequest.ibusPerson.IsPlanEnrolled(); //PIR 22732
            lbusWssEmploymentChangeRequest.LoadOrganization();
            lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.status_value = busConstant.EmploymentChangeRequestStatusReview;
            lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.plan_id = aintplanid;
            lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.job_class_value = lbusWssEmploymentChangeRequest.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.job_class_value;
            lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.type_value = lbusWssEmploymentChangeRequest.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value;
            lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.seasonal_id = lbusWssEmploymentChangeRequest.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.seasonal_id;
            lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.seasonal_value = lbusWssEmploymentChangeRequest.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.seasonal_value;
            lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.hourly_value = lbusWssEmploymentChangeRequest.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.hourly_value;

            //PIR 13852
            lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.employment_type_change_effective_date = DateTime.MinValue;
            //lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.employment_type_change_effective_date = lbusWssEmploymentChangeRequest.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date;
            lbusWssEmploymentChangeRequest.istrLess12hourWorkIndicator = !string.IsNullOrEmpty(lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.seasonal_value) ? "Yes" : "No";
            //PIR 22877 termination wizard if the members current employment detail status is LOA
            if (lbusWssEmploymentChangeRequest.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.status_value.IsNotNull()
                && (lbusWssEmploymentChangeRequest.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.status_value.Equals("LOA")
                || lbusWssEmploymentChangeRequest.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.status_value.Equals("LOAM")
                || lbusWssEmploymentChangeRequest.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.status_value.Equals("FMLA"))                
                 && (astrchangetypevalue != "CLSC" && astrchangetypevalue != busConstant.EmploymentChangeRequestChangeTypeTEEM))
            {
                lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.istrEmpDetailStartDate = Convert.ToString(lbusWssEmploymentChangeRequest.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date.Date.ToString("MM/dd/yyyy"));
                string astrLOAType = lbusWssEmploymentChangeRequest.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.status_value.IsNullOrEmpty() ? string.Empty : lbusWssEmploymentChangeRequest.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.status_value;
                lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.change_type_value = astrLOAType.Equals("LOA") ? "LOAL" : astrLOAType;
                lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.istrEmpDetailStatusDescription = astrLOAType.Equals("LOA") ? "LOA/Leave without pay" :
                                                                                                               astrLOAType.Equals("LOAM") ? @"Leave Of Absence - Military" : "Family and Medical Leave Act";
            }
            else if (!String.IsNullOrEmpty(astrchangetypevalue) && astrchangetypevalue.Equals("CLSC"))
            {
                lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.change_type_value = astrchangetypevalue;
                lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.official_list_value = lbusWssEmploymentChangeRequest.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.official_list_value;
                if (lbusWssEmploymentChangeRequest.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.status_value.IsNotNull() && lbusWssEmploymentChangeRequest.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.status_value.Contains("CON"))
                    lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.employment_status_value = lbusWssEmploymentChangeRequest.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.status_value;
            }
            lbusWssEmploymentChangeRequest.iblnIsFromESS = true; //PIR 14073
            lbusWssEmploymentChangeRequest.iblnIsESSTerminateEmployment = true; // PIR 23963
            lbusWssEmploymentChangeRequest.EvaluateInitialLoadRules();
            LoadContactForEmpChangeRequest(lbusWssEmploymentChangeRequest);

            //PIR 24663
            if (!(lbusWssEmploymentChangeRequest.ibusOrganization.iblnIsOrgOfferingAnyRetPlan))
            {
                lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.employment_status_value = busConstant.EmploymentStatusNonContributing;
            }
            return lbusWssEmploymentChangeRequest;
        }
        /// <summary>
        /// F/W Upgrade PIR 12037 - Loaded contact ID, need this on post of the request.
        /// </summary>
        /// <param name="abusWssEmploymentChangeRequest"></param>
        private void LoadContactForEmpChangeRequest(busWssEmploymentChangeRequest abusWssEmploymentChangeRequest)
        {
            if ((iobjPassInfo.istrFormName == "wfmTerminateEmploymentWizard" || iobjPassInfo.istrFormName == "wfmUpdateTermEmploymentWizard" ||
                iobjPassInfo.istrFormName == "wfmUpdateTermEmploymentLOARecertificationWizard" || iobjPassInfo.istrFormName == "wfmUpdateTermEmploymentLOAMilitaryWizard" ||
                iobjPassInfo.istrFormName == "wfmUpdateTermEmploymentLOAWithoutPayWizard" || iobjPassInfo.istrFormName == "wfmESSEmploymentStatusChangeRequestWizard"))
            {
                SetWebParameters();
                if (iintContactID > 0)
                    abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.contact_id = iintContactID;
            }
        }
        public busWssEmploymentChangeRequest FindWssEmploymentChangeRequest(int aintEmploymentChangeRequestID, int aintemploymentId, int aintemploymentdetailid)
        {
            busWssEmploymentChangeRequest lbusWssEmploymentChangeRequest = new busWssEmploymentChangeRequest
            {
                icdoWssEmploymentChangeRequest = new cdoWssEmploymentChangeRequest()
            };
            lbusWssEmploymentChangeRequest.FindWssEmploymentChangeRequest(aintEmploymentChangeRequestID);
            lbusWssEmploymentChangeRequest.LoadPerson();
            lbusWssEmploymentChangeRequest.ibusPerson.LoadPlanEnrollmentVisibility();
            lbusWssEmploymentChangeRequest.ibusPerson.IsPlanEnrolled(); //PIR 22732
            lbusWssEmploymentChangeRequest.LoadOrganization();
            lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.person_employment_id = aintemploymentId;
            lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.person_employment_detail_id = aintemploymentdetailid;
            lbusWssEmploymentChangeRequest.LoadEmployment();
            lbusWssEmploymentChangeRequest.LoadEmploymentDetail();
            lbusWssEmploymentChangeRequest.ibusPerson.iintOrgID = lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.org_id;
            if (lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.last_month_on_employer_billing != DateTime.MinValue)
            {
                lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.istrLastMonthOnEmployerBilling =
                    lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.last_month_on_employer_billing.ToString("MM/yyyy", CultureInfo.CreateSpecificCulture("en-US"));
            }
            if (lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.last_retirement_transmittal_of_deduction != DateTime.MinValue)
            {
                lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.istrLastRetirementTransmittalOfDeduction =
                                   lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.last_retirement_transmittal_of_deduction.ToString("MM/yyyy", CultureInfo.CreateSpecificCulture("en-US"));
            }
            if (lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.date_of_return != DateTime.MinValue)
            {
                lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.istrEmpDetailStartDate = Convert.ToString(lbusWssEmploymentChangeRequest.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date.Date.ToString("MM/dd/yyyy"));
                string astrLOAType = lbusWssEmploymentChangeRequest.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.status_value.IsNullOrEmpty() ? string.Empty : lbusWssEmploymentChangeRequest.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.status_value;
                lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.change_type_value = astrLOAType.Equals("LOA") ? "LOAL" : astrLOAType;
                lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.istrEmpDetailStatusDescription = astrLOAType.Equals("LOA") ? "LOA/Leave without pay" :
                                                                                                               astrLOAType.Equals("LOAM") ? @"Leave Of Absence - Military" : "Family and Medical Leave Act";
            }
            lbusWssEmploymentChangeRequest.LoadRejectionDetails();
            lbusWssEmploymentChangeRequest.istrLess12hourWorkIndicator = !string.IsNullOrEmpty(lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.seasonal_value) ? "Yes" : "No";
            lbusWssEmploymentChangeRequest.istrIsTermsAndConditionsAgreed = busConstant.Flag_Yes;
            lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.ienuObjectState = ObjectState.Update;
            lbusWssEmploymentChangeRequest.iblnIsFromESS = true; //PIR 14073
            LoadContactForEmpChangeRequest(lbusWssEmploymentChangeRequest);
            return lbusWssEmploymentChangeRequest;
        }
        public busEnrollOther457Web Enrollin457(int aintemploymentId, int aintemploymentdetailid, int aintplanid)
        {
            busEnrollOther457Web lbusEnrollOther457Web = new busEnrollOther457Web();
            lbusEnrollOther457Web.person_employment_id = aintemploymentId;
            lbusEnrollOther457Web.person_employment_detail_id = aintemploymentdetailid;
            lbusEnrollOther457Web.LoadEmployment();
            lbusEnrollOther457Web.LoadEmploymentDetail();
            lbusEnrollOther457Web.person_id = lbusEnrollOther457Web.ibusPersonEmployment.icdoPersonEmployment.person_id;
            lbusEnrollOther457Web.org_id = lbusEnrollOther457Web.ibusPersonEmployment.icdoPersonEmployment.org_id;
            lbusEnrollOther457Web.LoadPerson();
            lbusEnrollOther457Web.LoadOrganization();
            lbusEnrollOther457Web.plan_id = aintplanid;
            lbusEnrollOther457Web.iblnIsFromESS = true;
            lbusEnrollOther457Web.ibusPerson.iintOrgID = lbusEnrollOther457Web.ibusPersonEmployment.icdoPersonEmployment.org_id;  //PIR-18010
            //PIR 13380
            lbusEnrollOther457Web.ibusPerson.ibusESSPersonAccountDeffCompOther457 = new busPersonAccountDeferredComp { icdoPersonAccount = new cdoPersonAccount(), icdoPersonAccountDeferredComp = new cdoPersonAccountDeferredComp() };
            lbusEnrollOther457Web.ibusPerson.ESSLoadPersonAccountForEnrolledPlans();


            return lbusEnrollOther457Web;
        }

        public busMemberDataRequestLookup LoadMemberDataRequests(DataTable adtbSearchResult)
        {
            busMemberDataRequestLookup lobjMemberDataRequestLookup = new busMemberDataRequestLookup();
            lobjMemberDataRequestLookup.LoadWssMemberRecordRequests(adtbSearchResult);
            return lobjMemberDataRequestLookup;
        }

        public busWssMessageDetailLookup LoadWssMessageDetails(DataTable adtbSearchResult)
        {
            busWssMessageDetailLookup lobjWssMessageDetailLookup = new busWssMessageDetailLookup();
            lobjWssMessageDetailLookup.LoadWssMessageDetails(adtbSearchResult);
            return lobjWssMessageDetailLookup;
        }

        public busWSSHome FindWSSHome()
        {
            busWSSHome lobjWSSHome = new busWSSHome();
            lobjWSSHome.LoadFormsForESS();
            return lobjWSSHome;
        }

        public busWSSEmploymentChangeRequestLookup LoadWSSEmploymentChangeRequests(DataTable adtbSearchResult)
        {
            busWSSEmploymentChangeRequestLookup lobjWSSEmploymentChangeRequestLookup = new busWSSEmploymentChangeRequestLookup();
            lobjWSSEmploymentChangeRequestLookup.LoadWssEmploymentChangeRequests(adtbSearchResult);
            return lobjWSSEmploymentChangeRequestLookup;
        }

        public busPersonLookup LoadEmployees(DataTable adtbSearchResult)
        {
            busPersonLookup lobjPersonLookup = new busPersonLookup();
            lobjPersonLookup.LoadPersonFromESS(adtbSearchResult);
            return lobjPersonLookup;
        }
        public void GenerateCorrespondenceForSendingPersLinkID(busPerson lobjPerson)
        {
            lobjPerson.GenerateCorrespondenceForSendingPersLinkID();
        }
        public busPerson FindESSEmployee(int aintPersonID, int aintOrgID)
        {
            busPerson lobjPerson = new busPerson();
            if (lobjPerson.FindPerson(aintPersonID))
            {
                lobjPerson.iintOrgID = aintOrgID;
                lobjPerson.GetPersonCurrentAddressByType(busConstant.AddressTypePermanent, DateTime.Now);
                lobjPerson.ESSLoadPersonEmployment();
                lobjPerson.LoadEmploymentChangeRequestDetails();
                lobjPerson.EvaluateInitialLoadRules();
            }
            return lobjPerson;
        }


        public busDeductionCalculation FindDeductionCalculation(int aintBenefitCalculationID, int aintDeductionSummaryID, int aintBenefitOptionID)
        {
            busDeductionCalculation lobjDeductionCalculation = new busDeductionCalculation();
            lobjDeductionCalculation.FindDeductionCalculation(aintBenefitCalculationID);
            lobjDeductionCalculation.ibusBenefitDeductionSummary = new busBenefitDeductionSummary();
            lobjDeductionCalculation.ibusBenefitDeductionSummary.FindBenefitDeductionSummary(aintDeductionSummaryID);
            lobjDeductionCalculation.ibusBenefitCalculationOptions = new busBenefitCalculationOptions();
            lobjDeductionCalculation.ibusBenefitCalculationOptions.FindBenefitCalculationOptions(aintBenefitOptionID);
            lobjDeductionCalculation.ibusBenefitCalculationOptions.LoadBenefitProvisionOption();
            lobjDeductionCalculation.icdoBenefitCalculation.benefit_calculation_id = aintBenefitCalculationID;
            lobjDeductionCalculation.LoadBenefitCalculation();
            lobjDeductionCalculation.LoadPerson();
            lobjDeductionCalculation.ibusPerson.LoadPersonEmployment();
            lobjDeductionCalculation.LoadPlan();
            lobjDeductionCalculation.LoadBenefitApplication();
            lobjDeductionCalculation.LoadBenefitHealthDeduction();
            lobjDeductionCalculation.LoadBenefitDentalDeduction();
            lobjDeductionCalculation.LoadBenefitVisionDeduction();
            lobjDeductionCalculation.LoadBenefitLifeDeductions();
            lobjDeductionCalculation.LoadBenefitLtcMemberDeductions();
            lobjDeductionCalculation.LoadBenefitLtcSpouseDeductions();
            lobjDeductionCalculation.LoadBenefitPayeeFedTaxWithholding();
            lobjDeductionCalculation.LoadBenefitPayeeStateTaxWithholding();
            if (lobjDeductionCalculation.ibusBenefitCalculationOptions.icdoBenefitCalculationOptions.taxable_amount == 0.0M)
                lobjDeductionCalculation.ibusBenefitCalculationOptions.icdoBenefitCalculationOptions.taxable_amount =
                lobjDeductionCalculation.ibusBenefitCalculationOptions.icdoBenefitCalculationOptions.benefit_option_amount;
            if (lobjDeductionCalculation.ibusBenefitCalculationOptions.ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.benefit_option_value == busConstant.BenefitOptionRefund ||
                    lobjDeductionCalculation.ibusBenefitPayeeFedTaxWithholding.icdoBenefitPayeeTaxWithholding.tax_option_value == busConstant.TaxOptionFedTaxwithheld)
                lobjDeductionCalculation.ProcessFedTax();
            if (lobjDeductionCalculation.ibusBenefitCalculationOptions.ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.benefit_option_value == busConstant.BenefitOptionRefund ||
                lobjDeductionCalculation.ibusBenefitPayeeStateTaxWithholding.icdoBenefitPayeeTaxWithholding.tax_option_value == busConstant.TaxOptionStateTaxwithheld)
                lobjDeductionCalculation.ProcessStateTax();
            lobjDeductionCalculation.LoadBenefitDeductionSummary();

            lobjDeductionCalculation.ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.gross_monthly_benefit_amount =
                lobjDeductionCalculation.ibusBenefitCalculationOptions.icdoBenefitCalculationOptions.benefit_option_amount;
            if (lobjDeductionCalculation.ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.rhic_overridden_amount == 0M)
                lobjDeductionCalculation.ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.rhic_overridden_amount =
                     Math.Round(((1 - lobjDeductionCalculation.icdoBenefitCalculation.rhic_early_reduction_factor) *
                     lobjDeductionCalculation.icdoBenefitCalculation.unreduced_rhic_amount), 2,
                     MidpointRounding.AwayFromZero);
            lobjDeductionCalculation.ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.net_health_insurance_premium_amount =
                 lobjDeductionCalculation.ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.health_overridden_amount;
            // PIR 15271 and 20269 : Removing rhic_overridden_amount     
            //- lobjDeductionCalculation.ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.rhic_overridden_amount;

            lobjDeductionCalculation.ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.fit_overridden_amount =
                lobjDeductionCalculation.ibusBenefitPayeeFedTaxWithholding.icdoBenefitPayeeTaxWithholding.computed_tax_amount;
            lobjDeductionCalculation.ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.ndit_overridden_amount =
                lobjDeductionCalculation.ibusBenefitPayeeStateTaxWithholding.icdoBenefitPayeeTaxWithholding.computed_tax_amount;

            lobjDeductionCalculation.ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.net_monthly_pension_benefit_amount =
                lobjDeductionCalculation.ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.gross_monthly_benefit_amount -
                               (lobjDeductionCalculation.ibusBenefitPayeeFedTaxWithholding.icdoBenefitPayeeTaxWithholding.computed_tax_amount +
                               lobjDeductionCalculation.ibusBenefitPayeeStateTaxWithholding.icdoBenefitPayeeTaxWithholding.computed_tax_amount +
                               ((lobjDeductionCalculation.ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.net_health_insurance_premium_amount > 0) ?
                                lobjDeductionCalculation.ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.net_health_insurance_premium_amount : 0) +
                               lobjDeductionCalculation.ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.vision_overridden_amount +
                               lobjDeductionCalculation.ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.dental_overridden_amount +
                               lobjDeductionCalculation.ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.life_overridden_amount +
                               lobjDeductionCalculation.ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.ltc_overridden_amount +
                               lobjDeductionCalculation.ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.miscellaneous_deduction_amount);
            lobjDeductionCalculation.CalculateNetMonthlyPensionBenefit(); // Net benefit calculation
            return lobjDeductionCalculation;
        }

        /// <summary>
        /// Added for pir 6939
        /// Loads Employer Payroll Header 
        /// </summary>
        /// <param name="aintEmployerPayrollHeaderID"></param>
        /// <returns>busEmployerPayrollHeader object</returns>
        public busEmployerPayrollHeader LoadEmployerPayrollHeaderESS(int aintEmployerPayrollHeaderID)
        {
            busEmployerPayrollHeader lobjEmployerPayrollHeader = new busEmployerPayrollHeader();
            lobjEmployerPayrollHeader = FindEmployerPayrollHeaderESS(aintEmployerPayrollHeaderID, 0);
            if (lobjEmployerPayrollHeader.IsNotNull())
                lobjEmployerPayrollHeader.btnAcceptReport_Click();
            return lobjEmployerPayrollHeader;
        }

        public busWssPersonAccountEnrollmentRequestAck FindWssPersonAccountEnrollmentRequestAck(int aintpersonaccountenrollmentrequestackid)
        {
            busWssPersonAccountEnrollmentRequestAck lobjWssPersonAccountEnrollmentRequestAck = new busWssPersonAccountEnrollmentRequestAck();
            if (lobjWssPersonAccountEnrollmentRequestAck.FindWssPersonAccountEnrollmentRequestAck(aintpersonaccountenrollmentrequestackid))
            {
            }

            return lobjWssPersonAccountEnrollmentRequestAck;
        }

        public busWssAcknowledgement FindWssAcknowledgement(int aintacknowledgementid)
        {
            busWssAcknowledgement lobjWssAcknowledgement = new busWssAcknowledgement();
            if (lobjWssAcknowledgement.FindWssAcknowledgement(aintacknowledgementid))
            {
            }
            return lobjWssAcknowledgement;
        }
        public busWssAcknowledgement NewWssAcknowledgement(int aintacknowledgementid, DateTime adateeffectivedate, int aintdisplaysequence, string astrstepvalue, string astrack)
        {
            busWssAcknowledgement lobjWssAcknowledgement = new busWssAcknowledgement { icdoWssAcknowledgement = new cdoWssAcknowledgement() };
            if (aintacknowledgementid != 0)
            {
                lobjWssAcknowledgement.FindWssAcknowledgement(aintacknowledgementid);
                lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_id = 0;
                lobjWssAcknowledgement.icdoWssAcknowledgement.effective_date = adateeffectivedate;
                lobjWssAcknowledgement.icdoWssAcknowledgement.display_sequence = aintdisplaysequence;
                lobjWssAcknowledgement.icdoWssAcknowledgement.screen_step_id = 6000;
                lobjWssAcknowledgement.icdoWssAcknowledgement.screen_step_value = astrstepvalue;
                lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text = astrack;

                lobjWssAcknowledgement.icdoWssAcknowledgement.ienuObjectState = ObjectState.Insert;
                lobjWssAcknowledgement.iarrChangeLog.Add(lobjWssAcknowledgement.icdoWssAcknowledgement);
            }
            lobjWssAcknowledgement.icdoWssAcknowledgement.screen_step_id = 6000;
            //lobjWssAcknowledgement.icdoWssAcknowledgement.ienuObjectState = ObjectState.Insert;
            lobjWssAcknowledgement.EvaluateInitialLoadRules();
            return lobjWssAcknowledgement;
        }

        public busWssAcknowledgementLookup LoadWssAcknowledgements(DataTable adtbSearchResult)
        {
            busWssAcknowledgementLookup lobjWssAcknowledgementLookup = new busWssAcknowledgementLookup();
            lobjWssAcknowledgementLookup.LoadWssAcknowledgement(adtbSearchResult);
            return lobjWssAcknowledgementLookup;
        }

        public busEnrollOther457Web FindEnroll457(int aintemploymentId, int aintemploymentdetailid, int aintplanid)
        {

            busEnrollOther457Web lbusEnrollOther457Web = new busEnrollOther457Web();
            lbusEnrollOther457Web.person_employment_id = aintemploymentId;
            lbusEnrollOther457Web.person_employment_detail_id = aintemploymentdetailid;
            lbusEnrollOther457Web.LoadEmployment();
            lbusEnrollOther457Web.LoadEmploymentDetail();
            lbusEnrollOther457Web.person_id = lbusEnrollOther457Web.ibusPersonEmployment.icdoPersonEmployment.person_id;
            lbusEnrollOther457Web.org_id = lbusEnrollOther457Web.ibusPersonEmployment.icdoPersonEmployment.org_id;
            lbusEnrollOther457Web.LoadPerson();
            lbusEnrollOther457Web.LoadOrganization();
            lbusEnrollOther457Web.plan_id = aintplanid;
            lbusEnrollOther457Web.LoadProviderOrgCode();

            if (lbusEnrollOther457Web.IsEnrolledIn457())
            {
            }
            return lbusEnrollOther457Web;
        }

        public busWssPersonAccountEnrollmentRequest FindWizardCompletion(int aintRequestID)
        {
            busWssPersonAccountEnrollmentRequest lobjPARequest = new busWssPersonAccountEnrollmentRequest
            {
                icdoWssPersonAccountEnrollmentRequest = new cdoWssPersonAccountEnrollmentRequest(),
                icdoMSSGDHV = new cdoWssPersonAccountGhdv()
            };
            lobjPARequest.FindWssPersonAccountEnrollmentRequest(aintRequestID);
            if (lobjPARequest.icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdFlex)
            {
                lobjPARequest.LoadPlan();
                lobjPARequest.LoadAckMemAuthDetails();
                lobjPARequest.LoadMSSFlexCompConversion();
                lobjPARequest.LoadPersonAccount();
                lobjPARequest.LoadMSSPersonAccountFlexComp();
                lobjPARequest.LoadMssFlexCompOption();
                lobjPARequest.LoadMSSAnnualPledgeAmount();
            }
            else if (lobjPARequest.icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupHealth ||
                     lobjPARequest.icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdDental ||
                     lobjPARequest.icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdVision)
            {
                lobjPARequest.LoadPerson();
                lobjPARequest.LoadPlan();
                lobjPARequest.LoadPersonAccount();
                lobjPARequest.LoadPersonEmploymentDetail();
                lobjPARequest.ibusPersonEmploymentDetail.LoadPersonEmployment();
                lobjPARequest.ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
                lobjPARequest.LoadMSSGHDV();
                lobjPARequest.LoadPersonAccountGHDV();
                lobjPARequest.LoadMSSOtherCoverageDetails();
                lobjPARequest.LoadMSSWorkersCompensation();
                lobjPARequest.iblnIsFromPortal = true;
                lobjPARequest.LoadDependentsForViewRequest();
                //lobjPARequest.LoadDependents(); //Existing issue-Pending request was not posted correctly as it was not loading the iclbMSSDependent Collection in the update mode. iclbMSSDependent was remaining null .
                lobjPARequest.ibusPersonAccount.ibusPlan = new busPlan();
                lobjPARequest.ibusPersonAccount.ibusPlan = lobjPARequest.ibusPlan;
                lobjPARequest.ibusPersonAccount.icdoPersonAccount.person_employment_dtl_id = lobjPARequest.icdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id;
                if ((lobjPARequest.icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupHealth) ||
                    (lobjPARequest.icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdMedicarePartD))
                {
                    lobjPARequest.LoadCoverageCodeForMSS();
                }
                else
                {
                    lobjPARequest.LoadMSSLevelOfCoverageByPlan();
                }
                lobjPARequest.ibusMSSPersonAccountGHDV.icdoPersonAccount = lobjPARequest.ibusPersonAccount.icdoPersonAccount;
                lobjPARequest.ibusMSSPersonAccountGHDV.LoadPaymentElection();
                lobjPARequest.iclbGHDVAcknowledgement = new Collection<busWssAcknowledgement>();
                lobjPARequest.LoadHDHPAcknowledgement();
                lobjPARequest.LoadAckGenDetails();
                lobjPARequest.LoadAckMemAuthDetails();
                lobjPARequest.LoadAckCheckDetails();
                lobjPARequest.LoadAutoPostingCrossRef();
                lobjPARequest.LoadPretaxDeductionFlag();
                lobjPARequest.LoadPersonAccountGHDVHsa();
                DataTable ldtbListdtDTP = iobjPassInfo.isrvDBCache.GetCacheData("sgt_wss_acknowledgement", "ACKNOWLEDGEMENT_ID=148");
                if (ldtbListdtDTP != null && ldtbListdtDTP.Rows.Count > 0)
                    lobjPARequest.istrAcknowledgementTextForHDHP = ldtbListdtDTP.Rows[0]["acknowledgement_text"].ToString() + "&nbsp";
                DataTable ldtbListdtDTP1 = iobjPassInfo.isrvDBCache.GetCacheData("sgt_wss_acknowledgement", "ACKNOWLEDGEMENT_ID=149");
                if (ldtbListdtDTP1 != null && ldtbListdtDTP.Rows.Count > 0)
                    lobjPARequest.istrAcknowledgementTextForHDHP2 = ldtbListdtDTP1.Rows[0]["acknowledgement_text"].ToString();
                lobjPARequest.LoadCoverageCodeDescription();
                lobjPARequest.istrCoverageCodeDescription = lobjPARequest.ibusMSSPersonAccountGHDV.istrCoverageCode;
                lobjPARequest.istrPreTaxHSADisplay = lobjPARequest.istrPreTaxHSA == busConstant.Flag_Yes ? busConstant.Flag_Yes_Value : busConstant.Flag_No_Value;
                lobjPARequest.istrAcknowledgementFlag = busConstant.Flag_Yes;
            }
            else if (lobjPARequest.icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupLife)
            {
                lobjPARequest.LoadPerson();
                lobjPARequest.LoadPlan();
                lobjPARequest.LoadPersonAccount();
                lobjPARequest.LoadPersonEmploymentDetail();
                lobjPARequest.ibusPersonEmploymentDetail.LoadPersonEmployment();
                lobjPARequest.ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
                lobjPARequest.LoadPersonAccountEmploymentDetail();
                lobjPARequest.LoadMSSPersonAccountLife();
                lobjPARequest.LoadMSSLifeOptions();
                lobjPARequest.LoadSupplementalAmount();
                lobjPARequest.SetVisibilityofLink();
                lobjPARequest.iclbLifeAcknowledgement = new Collection<busWssAcknowledgement>();
                lobjPARequest.LoadAckCheckDetails();
                //lbusLifeRequest.ConfirmationText();
                if ((lobjPARequest.icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ChangeReasonAnnualEnrollment) &&
                    (lobjPARequest.icdoWssPersonAccountEnrollmentRequest.status_value == busConstant.EnrollmentStatusPending))         //PIR-15125
                    lobjPARequest.icdoWssPersonAccountEnrollmentRequest.change_effective_date = lobjPARequest.icdoWssPersonAccountEnrollmentRequest.date_of_change;

                lobjPARequest.idecBasicCoverageAmount = lobjPARequest.GetCoverageAmountDetails(busConstant.LevelofCoverage_Basic);
                decimal adecMinAmount = 0.00M; decimal adecMaxAmount = 0.00M;
                lobjPARequest.GetCoverageAmountDetailsSupplemental(ref adecMinAmount, ref adecMaxAmount);
                lobjPARequest.idecMinSupplementalLimit = adecMinAmount + lobjPARequest.idecBasicCoverageAmount;
                lobjPARequest.idecSupplementalLimit = adecMaxAmount + lobjPARequest.idecBasicCoverageAmount;
                lobjPARequest.idecSpouseSupplementalLimit = lobjPARequest.GetCoverageAmountDetails(busConstant.LevelofCoverage_SpouseSupplemental);
                if (lobjPARequest.lstrFilePath == string.Empty)
                {
                    DataTable ldtbPathData = utlPassInfo.iobjPassInfo.isrvDBCache.GetCacheData("sgs_system_paths", "path_code = 'CorrPdf'");
                    lobjPARequest.lstrFilePath = ldtbPathData.Rows[0]["path_value"].ToString();
                }
                lobjPARequest.istrDownloadFileName = lobjPARequest.lstrFilePath + "SFN-53855.pdf";
            }
            return lobjPARequest;
        }
        public busWssEmploymentChangeRequest FindESSWizardCompletion(int aintRequestID)
        {
            busWssEmploymentChangeRequest lobjPARequest = new busWssEmploymentChangeRequest
            {
                icdoWssEmploymentChangeRequest = new cdoWssEmploymentChangeRequest()

            };
            lobjPARequest.FindWssEmploymentChangeRequest(aintRequestID);
            return lobjPARequest;
        }

        public busServicePurchaseWeb NewServicePurchaseContracts(int aintPersonID)
        {
            //FMUpgrade: Changes for set Navigation parameter from menu item click
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmServicePurchaseMaintenance")
            {
                aintPersonID = aintPersonID != 0 ? aintPersonID : iintPersonID;
            }
            busServicePurchaseWeb lobjServicePurchaseWeb = new busServicePurchaseWeb { icdoServicePurchaseWeb = new cdoServicePurchaseHeader { person_id = aintPersonID } };
            lobjServicePurchaseWeb.iclbEligiblePlan = new Collection<cdoPlan>();    
			//PIR 26099        
            lobjServicePurchaseWeb.ibusServicePurchaseHeader = new busServicePurchaseHeader
            {
                icdoServicePurchaseHeader = new cdoServicePurchaseHeader() { person_id = aintPersonID }
            };
            lobjServicePurchaseWeb.ibusServicePurchaseHeader.LoadPerson();
            lobjServicePurchaseWeb.LoadPlansForServicePurchase();
            return lobjServicePurchaseWeb;
        }

        public busBenefitCalculatorWeb NewBenefitEstimate()
        {
            busBenefitCalculatorWeb lobjBenefitEstimate = new busBenefitCalculatorWeb();
            lobjBenefitEstimate.icdoWssBenefitcalculator = new cdoWssBenefitCalculator();
            lobjBenefitEstimate.iclbEligiblePlan = new Collection<cdoPlan>();
            lobjBenefitEstimate.ibusMember = new busPerson { icdoPerson = new cdoPerson() };
            lobjBenefitEstimate.iclbTffrTiaaService = new Collection<busPersonTffrTiaaService>();
            return lobjBenefitEstimate;
        }

        public busESSHome NewESSEmployeeHome()
        {
            busESSHome lobjESSHome = new busESSHome();
            return lobjESSHome;
        }

        public busESSHome FindESSRequests(int aintOrgID)
        {
            /*
             * FMUpgrade: Changes for set Navigation parameter from menu item click - BeforeGetInitialData
             */
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmESSViewRequestsMaintenance")
            {
                aintOrgID = aintOrgID != 0 ? aintOrgID : iintOrgID;
            }
            busESSHome lobjESSHome = new busESSHome();
            lobjESSHome.ibusContact = new busContact { icdoContact = new cdoContact() };
            lobjESSHome.LoadWSSEmploymentChangeRequests(aintOrgID);
            lobjESSHome.LoadWssMemberRecordRequest(aintOrgID);
            foreach (busWssEmploymentChangeRequest lobjEmploymentChangeRequest in lobjESSHome.iclbWssEmploymentChangeRequest)
            {
                lobjEmploymentChangeRequest.LoadPerson();
                lobjEmploymentChangeRequest.ibusPerson.iintOrgID = aintOrgID;
                lobjEmploymentChangeRequest.ibusPerson.ESSLoadPersonEmployment();
                lobjEmploymentChangeRequest.icdoWssEmploymentChangeRequest.person_employment_id = lobjEmploymentChangeRequest.ibusPerson.ibusESSPersonEmployment.icdoPersonEmployment.person_employment_id;
                lobjEmploymentChangeRequest.icdoWssEmploymentChangeRequest.person_employment_detail_id = lobjEmploymentChangeRequest.ibusPerson.ibusESSPersonEmployment.ibusESSLatestPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id;
            }
            return lobjESSHome;
        }

        // PIR 9493
        public bool IsMSSServicePurchaseTypeValid(int aintPersonID)
        {
            busPerson lobjPerson = new busPerson { icdoPerson = new cdoPerson { person_id = aintPersonID } };
            lobjPerson.LoadRetirementAccount();
            foreach (busPersonAccount lobjPA in lobjPerson.iclbRetirementAccount)
            {
                if ((lobjPA.icdoPersonAccount.plan_id != busConstant.PlanIdDC && lobjPA.icdoPersonAccount.plan_id != busConstant.PlanIdDC2020 && lobjPA.icdoPersonAccount.plan_id != busConstant.PlanIdDC2025) && //PIR 20232 //PIR 25920
                    lobjPA.icdoPersonAccount.plan_id != busConstant.PlanIdJobService)
                {
                    if (lobjPA.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled ||
                        lobjPA.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetimentSuspended)
                        return true;
                }
            }
            return false;
        }

        public bool IsMSSBenefitEstimateTypeValid(int aintPersonID)
        {
            busPerson lobjPerson = new busPerson { icdoPerson = new cdoPerson { person_id = aintPersonID } };
            if (lobjPerson.iclbRetirementAccount.IsNull()) lobjPerson.LoadRetirementAccount();
            foreach (busPersonAccount lobjPA in lobjPerson.iclbRetirementAccount)
            {
                if (lobjPA.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled ||
                    lobjPA.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetimentSuspended)
                    return true;
            }
            return false;
        }

        public bool IsPayPeriodValid(int OrgId, string ReportType, string HeaderType, DateTime PayPeriod, DateTime PayPeriodBeginDate, DateTime PayPeriodEndDate)
        {
            busEmployerPayrollHeader lbusEmployerPayrollHeader = new busEmployerPayrollHeader { icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader { org_id = OrgId, header_type_value = HeaderType, report_type_value = ReportType, pay_period = PayPeriod.ToString(), pay_period_start_date = PayPeriodBeginDate, pay_period_end_date = PayPeriodEndDate } };
            if (HeaderType != busConstant.PlanBenefitTypeDeferredComp)
                return lbusEmployerPayrollHeader.IsPayPeriodValid();
            else
                return lbusEmployerPayrollHeader.IsPayPeriodValidForDefComp();
        }
        public bool CheckHeaderDateContinuity(int OrgId, string ReportType, string HeaderType, DateTime PayPeriod, DateTime PayPeriodBeginDate, DateTime PayPeriodEndDate)
        {
            busEmployerPayrollHeader lbusEmployerPayrollHeader = new busEmployerPayrollHeader { icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader { org_id = OrgId, header_type_value = HeaderType, report_type_value = ReportType, pay_period = PayPeriod.ToString(), payroll_paid_date = PayPeriod, pay_period_start_date = PayPeriodBeginDate, pay_period_end_date = PayPeriodEndDate } };
            return lbusEmployerPayrollHeader.CheckHeaderDateContinuity();
        }
        // PIR 9393
        public bool IsTemporaryMember(int aintPerEmpDtlID)
        {
            busWssPersonAccountEnrollmentRequest lobjRequest = new busWssPersonAccountEnrollmentRequest
            {
                icdoWssPersonAccountEnrollmentRequest = new cdoWssPersonAccountEnrollmentRequest { person_employment_dtl_id = aintPerEmpDtlID }
            };
            lobjRequest.LoadPersonEmploymentDetail();
            return lobjRequest.IsTemporaryEmployee;
        }

        //PIR 12946 - Modified logic behind visibility of Service Purchase Function
        public bool IsPermanentMember(int aintPersonID)
        {
            busPerson lbusPerson = new busPerson
            {
                icdoPerson = new cdoPerson { person_id = aintPersonID }
            };

            return lbusPerson.IsPermanentMemberService();  //Service Purchase PIR-15531 and PIR-9403
        }

        // PIR 9405
        public bool IsDBPlan(int aintPlanID)
        {
            busPlan lobjPlan = new busPlan { icdoPlan = new cdoPlan() };
            lobjPlan.FindPlan(aintPlanID);
            if (lobjPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueDB || lobjPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueHB)   //PIR 25920  New DC plan
                return true;
            return false;
        }
        public busPayrollReportWeb NewPayrollReport(int aintOrgID, int aintContactID, string astrPayrollRepSelection = null)
        {
            /*
            * FMUpgrade: Changes for set Navigation parameter from menu item click
            */
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmCreatePayrollMaintenance" || iobjPassInfo.istrFormName == "wfmESSEmployeeHomeMaintenance" || iobjPassInfo.istrFormName == "wfmCreateReportCompletionMaintenance")
            {
                aintOrgID = aintOrgID != 0 ? aintOrgID : iintOrgID;
                aintContactID = aintContactID != 0 ? aintContactID : iintContactID;
            }
            busPayrollReportWeb lobjPayrollReportWeb = new busPayrollReportWeb();
            lobjPayrollReportWeb.aintOrgID = aintOrgID;
            lobjPayrollReportWeb.aintContactID = aintContactID;
            if (!string.IsNullOrEmpty(astrPayrollRepSelection))
                lobjPayrollReportWeb.astrReportSelection = astrPayrollRepSelection;
            return lobjPayrollReportWeb;
        }
        public busDepositLookup LoadDeposit(DataTable adtbSearchResult)
        {
            busDepositLookup lobjDeposit = new busDepositLookup();
            lobjDeposit.LoadDeposit(adtbSearchResult);
            return lobjDeposit;
        }

        public busWSSResourceLibrary LoadResourceLibrary(int aintOrgID, int aintContactID)
        {
            /*
             * FMUpgrade: Changes for set Navigation parameter from menu item click
             */
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmESSResourceLibraryMaintenance")
            {
                aintOrgID = aintOrgID != 0 ? aintOrgID : iintOrgID;
                aintContactID = aintContactID != 0 ? aintContactID : iintContactID;
            }
            busWSSResourceLibrary lobjWSSResourceLibrary = new busWSSResourceLibrary { icdoOrganization = new cdoOrganization() };
            lobjWSSResourceLibrary.LoadResourceLibrary(aintOrgID, aintContactID);
            return lobjWSSResourceLibrary;

        }
        public busRemittanceLookup LoadRemittances(DataTable adtbSearchResult)
        {
            busRemittanceLookup lobjRemittanceLookup = new busRemittanceLookup();
            lobjRemittanceLookup.LoadRemittance(adtbSearchResult);
            return lobjRemittanceLookup;
        }

        public busESSHome FindReportListing(int aintOrgID, int aintContactID)
        {
            /*
            * FMUpgrade: Changes for set Navigation parameter from menu item click
            */
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmESSReportsMaintenance")
            {
                aintOrgID = aintOrgID != 0 ? aintOrgID : iintOrgID;
                aintContactID = aintContactID != 0 ? aintContactID : iintContactID;
            }
            busESSHome lobjESSHome = new busESSHome();
            lobjESSHome.ibusContact = new busContact { icdoContact = new cdoContact() };
            lobjESSHome.LoadOrganization(aintOrgID);

            lobjESSHome.LoadESSReports();

            return lobjESSHome;
        }
        //public override void InitializeNewChildObject(object aobjParentObject, busBase aobjChildObject)
        //{


        //}

        //  FMUpgrade:Added for Default page conversion of FrameworkInit method
        public Collection<cdoPlan> GetPlanByOrganization()
        {
            SetWebParameters();
            Collection<cdoPlan> lclbCodeValue = new Collection<cdoPlan>();
            DataTable ldtbPlan = busNeoSpinBase.Select("cdoPlan.OrgSpecificPlans",
                                                          new object[2] { iintOrgID, DateTime.Now });
            lclbCodeValue = Sagitec.DataObjects.doBase.GetCollection<cdoPlan>(ldtbPlan);
            return lclbCodeValue;
        }
        //PIR 13773 - Second Point - Do not display �Create new Payroll Report� button - this method used on default page. 
        public bool IsOrganizationOnCentralPayroll(int aintOrgID)
        {
            DataTable ldtbOrganization = busNeoSpinBase.Select<cdoOrganization>(new string[1] { enmOrganization.org_id.ToString() },
                                                                     new object[1] { aintOrgID }, null, null);
            if (ldtbOrganization.Rows.Count > 0)
            {
                string lstrCentralPayrollFlag = Convert.ToString(ldtbOrganization.Rows[0]["CENTRAL_PAYROLL_FLAG"]);
                return string.IsNullOrEmpty(lstrCentralPayrollFlag) ? false : lstrCentralPayrollFlag.ToUpper() == busConstant.Flag_Yes ? true : false;
            }
            return false;
        }

        /// <summary>
        /// Get Organization specific Benefit Type 
        /// </summary>
        /// <param name="aintOrgId"></param>
        /// <returns></returns>
		//  FMUpgrade:Added for Default page conversion of FrameworkInit method
        public Collection<cdoCodeValue> GetBenefitByOrganization()
        {
            SetWebParameters();
            Collection<cdoCodeValue> lclbCodeValue = new Collection<cdoCodeValue>();
            DataTable ldtbBenefitType = busNeoSpinBase.Select("cdoDeposit.GetBenefitTypeByOrganization",
                                                          new object[1] { iintOrgID });
            lclbCodeValue = Sagitec.DataObjects.doBase.GetCollection<cdoCodeValue>(ldtbBenefitType);
            return lclbCodeValue;

        }
        //PIR 14042 - Org and benefit specific plans have to be displayed on detail lookup
        /// <summary>
        /// Returns Organization participating plans based on benefit type
        /// </summary>
        /// <param name="aintOrgId"></param>
        /// <param name="astrBenefitTypeValue"></param>
        /// <returns></returns>
        //public DataTable GetOrgAndBenefitTypePlans(int aintOrgId, string astrBenefitTypeValue)
        //{
        //    DataTable ldtbPlan = busNeoSpinBase.Select("cdoPlan.GetOrgSpecificPlansDetailLookup",
        //                                                  new object[2] { aintOrgId, astrBenefitTypeValue });
        //    return ldtbPlan;
        //}
        public Collection<cdoPlan> GetOrgAndBenefitTypePlans(string astrBenefitTypeValue)
        {
            Collection<cdoPlan> lclbCodeValue = new Collection<cdoPlan>();
            DataTable ldtbPlan = busNeoSpinBase.Select("cdoPlan.GetOrgSpecificPlansDetailLookup",
                                                          new object[2] { iobjPassInfo.idictParams["OrgID"], astrBenefitTypeValue });
            lclbCodeValue = Sagitec.DataObjects.doBase.GetCollection<cdoPlan>(ldtbPlan);
            return lclbCodeValue;
        }

        public Collection<cdoCodeValue> GetBenefitTypes()
        {
            Collection<cdoCodeValue> lclbCodeValue = new Collection<cdoCodeValue>();
            DataTable ldtbPlan = busNeoSpinBase.Select("entEmployerPayrollHeader.LoadBenefitTypeSearchDetail",
                                                          new object[0] { });
            lclbCodeValue = Sagitec.DataObjects.doBase.GetCollection<cdoCodeValue>(ldtbPlan);
            return lclbCodeValue;
        }

        //PIR 14042 - For displaying headers based on benefit and status (RETR and INSR with posted status and DEFF and PRCH valid status) 
        //while navigating from initiate payment and unpaid invoices
        protected override void AddWhereClause(string astrFormName, Collection<utlWhereClause> acolWhereClause)
        {
            SetWebParameters();
            if (astrFormName == "wfmESSEmployerPayrollHeaderHomeLookup")
            {
                

                if (!acolWhereClause.Any(i => i.istrFieldName == "Temp.org_id"))
                {
                    utlWhereClause lobjWhereClause1 = new utlWhereClause();
                    lobjWhereClause1.istrQueryId = "cdoEmployerPayrollHeader";
                    lobjWhereClause1.istrFieldName = "Temp.org_id";
                    if (acolWhereClause.Count > 0)
                        lobjWhereClause1.istrCondition = "and";
                    lobjWhereClause1.istrOperator = "=";
                    lobjWhereClause1.iobjValue1 = iintOrgID;
                    acolWhereClause.Add(lobjWhereClause1);
                }
                
            }
            else if (astrFormName == "wfmESSEmployeeLookup")
            {
                utlWhereClause lobjWhereClause = new utlWhereClause();
                lobjWhereClause.istrQueryId = "cdoWssMemberRecordRequest";
                lobjWhereClause.istrFieldName = "org_id";
                if (acolWhereClause.Count > 0)
                {
                    lobjWhereClause.istrCondition = "and";
                }
                lobjWhereClause.istrOperator = "=";
                lobjWhereClause.iobjValue1 = iintOrgID;
                acolWhereClause.Add(lobjWhereClause);
            }
            else if (astrFormName == "wfmESSDepositLookup")
            {
                utlWhereClause lobjWhereClause = new utlWhereClause();
                lobjWhereClause.istrQueryId = "cdoDeposit";
                lobjWhereClause.istrFieldName = "org_id";
                if (acolWhereClause.Count > 0)
                {
                    lobjWhereClause.istrCondition = "and";
                }
                lobjWhereClause.istrOperator = "=";
                lobjWhereClause.iobjValue1 = iintOrgID;
                acolWhereClause.Add(lobjWhereClause);
            }
            else if (astrFormName == "wfmESSFileHdrLookup")
            {
                utlWhereClause lobjWhereClause = new utlWhereClause();
                utlWhereClause lobjWhereClause1 = new utlWhereClause();
                utlWhereClause lobjWhereClause2 = new utlWhereClause();
                lobjWhereClause.istrQueryId = "cdoFileHdr";
                lobjWhereClause1.istrQueryId = "cdoFileHdr";
                lobjWhereClause2.istrQueryId = "cdoFileHdr";
                lobjWhereClause.istrFieldName = "ocr.org_id";
                lobjWhereClause1.istrFieldName = "ocr.contact_id";
                lobjWhereClause2.istrFieldName = "reference_id";
                if (acolWhereClause.Count > 0)
                {
                    lobjWhereClause.istrCondition = "and";
                }
                lobjWhereClause1.istrCondition = "and";
                lobjWhereClause2.istrCondition = "and";

                lobjWhereClause.istrOperator = "=";
                lobjWhereClause1.istrOperator = "=";
                lobjWhereClause2.istrOperator = "=";

                lobjWhereClause.iobjValue1 = iintOrgID;
                lobjWhereClause1.iobjValue1 = iintContactID;
                lobjWhereClause2.iobjValue1 = iintOrgID;

                acolWhereClause.Add(lobjWhereClause);
                acolWhereClause.Add(lobjWhereClause1);
                acolWhereClause.Add(lobjWhereClause2);

            }
            else if (astrFormName == "wfmESSRemittanceLookup")
            {
                utlWhereClause lobjWhereClause = new utlWhereClause();
                lobjWhereClause.istrQueryId = "cdoRemittance";
                lobjWhereClause.istrFieldName = "r.org_id";
                if (acolWhereClause.Count > 0)
                {
                    lobjWhereClause.istrCondition = "and";
                }
                lobjWhereClause.istrOperator = "=";
                lobjWhereClause.iobjValue1 = iintOrgID;
                acolWhereClause.Add(lobjWhereClause);
            }
            else if (astrFormName == "wfmESSEmployerPayrollHeaderLookup" || astrFormName == "wfmESSServicePurchaseEmployerPayrollHeaderLookup" || astrFormName == "wfmESSRetirementEmployerPayrollHeaderLookup" || astrFormName == "wfmESSDefferedCompEmployerPayrollHeaderLookup" || astrFormName == "wfmESSInsuranceEmployerPayrollHeaderLookup")
            {
                utlWhereClause lobjWhereClause = new utlWhereClause();
                utlWhereClause lobjWhereClause1 = new utlWhereClause();

                lobjWhereClause.istrQueryId = "cdoEmployerPayrollHeader";
                lobjWhereClause1.istrQueryId = "cdoEmployerPayrollHeader";

                lobjWhereClause.istrFieldName = "Temp.org_id";
                lobjWhereClause1.istrFieldName = "contact_id";

                lobjWhereClause1.istrCondition = "and";
                if (acolWhereClause.Count > 0)
                {
                    lobjWhereClause.istrCondition = "and";
                }
                lobjWhereClause.istrOperator = "=";
                lobjWhereClause1.istrOperator = "=";

                lobjWhereClause.iobjValue1 = iintOrgID;
                lobjWhereClause1.iobjValue1 = iintContactID;
                acolWhereClause.Add(lobjWhereClause);
                acolWhereClause.Add(lobjWhereClause1);
            }
            else if (astrFormName == "wfmESSServicePurchaseCombinedPayrollDetailLookup" || astrFormName == "wfmESSDeferredCompCombinedPayrollDetailLookup" || astrFormName == "wfmESSRetirementCombinedPayrollDetailLookup" || astrFormName == "wfmESSInsuranceCombinedPayrollDetailLookup" || astrFormName == "wfmESSCombinedPayrollDetailLookup")
            {
                utlWhereClause lobjWhereClause = new utlWhereClause();
                utlWhereClause lobjWhereClause1 = new utlWhereClause();

                lobjWhereClause.istrQueryId = "ESSLookup";
                lobjWhereClause1.istrQueryId = "sqEssLookupContactFilter";

                lobjWhereClause.istrFieldName = "EPH.org_id";
                lobjWhereClause1.istrFieldName = "OC.contact_id";

                lobjWhereClause1.istrCondition = "and";
                if (acolWhereClause.Count > 0)
                {
                    lobjWhereClause.istrCondition = "and";
                }
                lobjWhereClause.istrOperator = "=";
                lobjWhereClause1.istrOperator = "=";

                lobjWhereClause.iobjValue1 = iintOrgID;
                lobjWhereClause1.iobjValue1 = iintContactID;
                acolWhereClause.Add(lobjWhereClause);
                acolWhereClause.Add(lobjWhereClause1);
            }
            //PIR-13278 when selects 'ACH' from dropdown list, It return result for both 'ACH' &'ACHP'
            if ((astrFormName == "wfmESSDepositLookup" || astrFormName == "wfmESSRemittanceLookup") && acolWhereClause.IsNotNull() && acolWhereClause.Count > 0)
            {
                foreach (utlWhereClause iobjutlWhereClause in acolWhereClause)
                {
                    if (iobjutlWhereClause.istrFieldName == "deposit_method_value" && Convert.ToString(iobjutlWhereClause.iobjValue1) == "ACH")
                    {
                        iobjutlWhereClause.iobjValue1 = "'ACH','ACHP'";
                    }
                }
            }
            base.AddWhereClause(astrFormName, acolWhereClause);
        }

        /// <summary>
        /// PIR 14514
        /// </summary>
        /// <param name="OrgId"></param>
        /// <param name="ReportType"></param>
        /// <param name="HeaderType"></param>
        /// <param name="PayPeriod"></param>
        /// <param name="PayPeriodBeginDate"></param>
        /// <param name="PayPeriodEndDate"></param>
        /// <returns></returns>
        public String GetDeferredCompFrequencyPeriodByOrg(int OrgId, string ReportType, string HeaderType, DateTime PayPeriod, DateTime PayPeriodBeginDate, DateTime PayPeriodEndDate)
        {
            busEmployerPayrollHeader lbusEmployerPayrollHeader = new busEmployerPayrollHeader { icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader { org_id = OrgId, header_type_value = HeaderType, report_type_value = ReportType, pay_period = PayPeriod.ToString(), payroll_paid_date = PayPeriod, pay_period_start_date = PayPeriodBeginDate, pay_period_end_date = PayPeriodEndDate } };
            return lbusEmployerPayrollHeader.GetDeferredCompFrequencyPeriodByOrg(OrgId);
        }

        public string IsDateOfChangeText(string astrChangeReasonValue, string astrPlanEnrlOptVal)
        {
            DataTable ldtblist = busNeoSpinBase.Select("cdoWssAutoPostingCrossRef.GetPromptUserText",
                 new object[2] { astrChangeReasonValue, astrPlanEnrlOptVal });
            if (ldtblist.Rows.Count > 0)
            {
                if (Convert.ToString(ldtblist.Rows[0]["PROMPT_USER_TEXT"]) == "NULL")
                {
                    return busConstant.ChangeEffectiveDefaultText;
                }
                else
                {
                    return Convert.ToString(ldtblist.Rows[0]["PROMPT_USER_TEXT"]);
                }
            }
            return string.Empty;
        }
        //PIR-17314
        public busPerson GetPersonNameByPersonID(int aintPersonID)
        {
            busContactTicket lbusContactTicket = new busContactTicket();
            lbusContactTicket.iblnIsFromESS = true;
            return lbusContactTicket.GetPersonNameByPersonID(aintPersonID);
        }
        /// <summary>
        /// PIR 18492 - Insert login details in UserLoginHistory table
        /// </summary>
        /// <param name="lobjPerson"></param>
        /// <param name="lboolIsLoginSuccess"></param>
        /// <param name="ProfileUserID"></param>
        /// <returns></returns>
        //public bool SetUserLoginHistory(busPerson lobjPerson, bool lboolIsLoginSuccess, string astrProfileUserID, string astrUserCountry, string astrUserRegionName, string astrUserCity, string astrIPAddress)
        //{
        //    busUserLoginHistory lobjbusUserLoginHistory = new busUserLoginHistory { icdoUserLoginHistory = new cdoUserLoginHistory() };
        //    //if (!string.IsNullOrEmpty(astrIPAddress)) //Call IP Loaction function if java script function doesnot return location details.
        //    //{ 
        //    //   busGlobalFunctions.GetIPLocation(astrIPAddress, ref astrUserCountry, ref astrUserRegionName, ref astrUserCity);
        //    //}
        //    lobjbusUserLoginHistory.InsertUserLoginHistory(lobjPerson.icdoPerson.person_id, astrProfileUserID, lboolIsLoginSuccess, astrUserCountry, astrUserRegionName, astrUserCity);
        //    lobjPerson.icdoPerson.ienuObjectState = ObjectState.Update;
        //    bool lboolIsUserLocked = lobjPerson.SetUserLock(lboolIsLoginSuccess, astrProfileUserID);
        //    return lboolIsUserLocked;
        //}

        #region //WSS message request PIR-17066
        public busWssMessageHeader NewWssMessageRequestHeader(string astrMemberType)
        {
            busWssMessageHeader lobjWssMessageHeader = new busWssMessageHeader { icdoWssMessageHeader = new cdoWssMessageHeader() };
            lobjWssMessageHeader.icdoWssMessageHeader.member_type_value = astrMemberType;
            lobjWssMessageHeader.icdoWssMessageHeader.member_type_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(7008, astrMemberType);
            lobjWssMessageHeader.icdoWssMessageHeader.is_message_sent = busConstant.Flag_No;
            lobjWssMessageHeader.EvaluateInitialLoadRules();
            return lobjWssMessageHeader;
        }
        public busWssMessageHeader FindWssMessageRequestHeader(int aintwssmessageid)
        {
            busWssMessageHeader lobjWssMessageHeader = new busWssMessageHeader();
            if (lobjWssMessageHeader.FindWssMessageHeader(aintwssmessageid))
            {
                lobjWssMessageHeader.icdoWssMessageHeader.member_type_description = busGlobalFunctions.GetDescriptionByCodeValue(7008, lobjWssMessageHeader.icdoWssMessageHeader.member_type_value, utlPassInfo.iobjPassInfo);
            }
            return lobjWssMessageHeader;
        }
        #endregion //WSS message request PIR-17066     

        public busMSSRetBenAppWeb NewRetBenApp(int aintPersonId)
        {
            //FMUpgrade: Changes for set Navigation parameter from menu item click
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmMSSRetBenAppsLandingScreenMaintenance")
            {
                aintPersonId = aintPersonId != 0 ? aintPersonId : iintPersonID;
            }
            busMSSRetBenAppWeb lbusMSSRetBenAppWeb = new busMSSRetBenAppWeb();
            lbusMSSRetBenAppWeb.iintPersonId = aintPersonId;
            lbusMSSRetBenAppWeb.VerifyPlansAndRedirectToWizard(aintPersonId);
            lbusMSSRetBenAppWeb.LoadBenTypeRecords();
            return lbusMSSRetBenAppWeb;
        }
        public busMSSRetBenAppWeb NewViewRetBenApps(int aintPersonId)
        {
            //FMUpgrade: Changes for set Navigation parameter from menu item click
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmMSSViewRetAppsMaintenance")
            {
                aintPersonId = aintPersonId != 0 ? aintPersonId : iintPersonID;
            }
            busMSSRetBenAppWeb lbusMSSRetBenAppWeb = new busMSSRetBenAppWeb();
            lbusMSSRetBenAppWeb.LoadBenAppsExcBenTypesPSTDAndDETH(aintPersonId);
            return lbusMSSRetBenAppWeb;
        }
        public busMSSRetBenAppWeb NewSelectRetPlan(int aintPersonId, string astrBenefitAppsOption)
        {
            //FMUpgrade: Changes for set Navigation parameter from menu item click
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmMSSSelectRetPlanMaintenance")
            {
                aintPersonId = aintPersonId != 0 ? aintPersonId : iintPersonID;
            }
            busMSSRetBenAppWeb lbusMSSRetBenAppWeb = new busMSSRetBenAppWeb();
            lbusMSSRetBenAppWeb.iintPersonId = aintPersonId;
            lbusMSSRetBenAppWeb.istrBenRetOption = astrBenefitAppsOption;
            lbusMSSRetBenAppWeb.LoadSuspendedRetPlanAccounts();
            lbusMSSRetBenAppWeb.EvaluateInitialLoadRules();
            return lbusMSSRetBenAppWeb;
        }
        public busWssBenApp NewBenAppWizard(int aintPersonId, int aintPlanId, DateTime adtTerminationDate, string astrBenefitTypeValue)
        {
            //FMUpgrade: Changes for set Navigation parameter from menu item click
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmMSSBenefitApplicationWizard")
            {
                aintPersonId = aintPersonId != 0 ? aintPersonId : iintPersonID;
            }
            busWssBenApp lbusWssBenApp = new busWssBenApp { icdoWssBenApp = new cdoWssBenApp() };
            lbusWssBenApp.icdoWssBenApp.person_id = aintPersonId;
            lbusWssBenApp.icdoWssBenApp.plan_id = aintPlanId;
            lbusWssBenApp.icdoWssBenApp.termination_date = adtTerminationDate;
            lbusWssBenApp.icdoWssBenApp.ben_action_status_value = busConstant.BenefitApplicationActionStatusSaveAndContinueLater;
            lbusWssBenApp.LoadPlan();
            lbusWssBenApp.LoadMemberPerson();
            lbusWssBenApp.LoadPersonAccount();
            lbusWssBenApp.ibusPersonAccount.ibusPerson = lbusWssBenApp.ibusMemberPerson;
            lbusWssBenApp.icdoWssBenApp.ben_type_value = astrBenefitTypeValue;
            lbusWssBenApp.LoadPersonEmploymentDetail();
            lbusWssBenApp.InitializeDisabilityAndOtherObjects();
            lbusWssBenApp.LoadWssOtherDisabilityBenefits();
            lbusWssBenApp.LoadInsPremACHDetailsAcknowledgement();
            lbusWssBenApp.LoadDependents();
            lbusWssBenApp.LoadLifePersonAccount();
            lbusWssBenApp.LoadRetAppForNRDAndSubTypeAndEligibility();
            lbusWssBenApp.icdoWssBenApp.istrRetirementDate = lbusWssBenApp.icdoWssBenApp.normal_retr_date.ToString("MM/yyyy");
            lbusWssBenApp.LoadActiveRetirementPlan();
            lbusWssBenApp.IsRetireeAttainedAge65();
            if (iobjPassInfo.idictParams.ContainsKey("IsExternalLogin"))
                lbusWssBenApp.iblnExternalLogin = Convert.ToBoolean(iobjPassInfo.idictParams["IsExternalLogin"]);
            lbusWssBenApp.EvaluateInitialLoadRules();
            return lbusWssBenApp;
        }
        public busWssBenApp FindBenAppWizard(int aintBenAppId)
        {
            busWssBenApp lbusWssBenApp = new busWssBenApp { icdoWssBenApp = new cdoWssBenApp() };
            if (lbusWssBenApp.FindWssBenApp(aintBenAppId, true, true))
            {
                lbusWssBenApp.PopulateObjectsWithDefaults();
                if (lbusWssBenApp.icdoWssBenApp.ben_type_value != busConstant.ApplicationBenefitTypeRefund && lbusWssBenApp.icdoWssBenApp.retirement_date != DateTime.MinValue)
                {
                    lbusWssBenApp.icdoWssBenApp.istrRetirementDate = lbusWssBenApp.icdoWssBenApp
                                                            .retirement_date.ToString("MM/yyyy", System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                }
                lbusWssBenApp.LoadPlan();
                lbusWssBenApp.LoadMemberPerson();
                lbusWssBenApp.LoadPersonAccount();
                lbusWssBenApp.ibusPersonAccount.ibusPerson = lbusWssBenApp.ibusMemberPerson;
                lbusWssBenApp.LoadPersonEmploymentDetail();
                lbusWssBenApp.LoadWssOtherDisabilityBenefits();
                lbusWssBenApp.LoadInsEnrlRequests();
                lbusWssBenApp.LoadInsPremACHDetailsAcknowledgement();
                lbusWssBenApp.LoadDependents();
                lbusWssBenApp.LoadLifePersonAccount();
                if (!string.IsNullOrEmpty(lbusWssBenApp.icdoWssBenApp.last_step_where_user_left))
                {
                    lbusWssBenApp.istrLastStepToLoad = lbusWssBenApp.icdoWssBenApp.last_step_where_user_left.Trim();
                    lbusWssBenApp.LoadPreviousStepObjects(lbusWssBenApp.icdoWssBenApp.last_step_where_user_left);
                }
                lbusWssBenApp.LoadActiveRetirementPlan();
                lbusWssBenApp.IsRetireeAttainedAge65();
                if (iobjPassInfo.idictParams.ContainsKey("IsExternalLogin"))
                    lbusWssBenApp.iblnExternalLogin = Convert.ToBoolean(iobjPassInfo.idictParams["IsExternalLogin"]);
                lbusWssBenApp.EvaluateInitialLoadRules();
            }
            return lbusWssBenApp;
        }
        public object GetBenOptionsByEffectiveDate(string astrBenType, string astrRetDate, int aintBenProvisionId, string astrMaritalStatusValue, bool ablnReturnCollection)
        {
            DateTime ldteResult = DateTime.MinValue;
            Collection<busCustomCodeValue> lclcBenOptions = new Collection<busCustomCodeValue>();
            if (!string.IsNullOrEmpty(astrBenType) && !string.IsNullOrEmpty(astrRetDate) &&
                DateTime.TryParse(astrRetDate, out ldteResult) && (aintBenProvisionId != busConstant.PlanIdDC && aintBenProvisionId != busConstant.BenProvisionIdDC20) && astrBenType != busConstant.ApplicationBenefitTypeRefund) //PIR 20232
            {
                DataTable ldtbBenOptions = busBase.Select("cdoWssBenApp.LoadWssBenOptions", new object[4] { aintBenProvisionId, astrBenType, ldteResult, (astrMaritalStatusValue == busConstant.PersonMaritalStatusMarried) ? "1" : "-999" });
                if (ldtbBenOptions.IsNotNull() && ldtbBenOptions.Rows.Count > 0)
                {
                    foreach (DataRow dr in ldtbBenOptions.Rows)
                    {
                        if (Convert.ToString(dr["CODE_VALUE"]) != busConstant.BenefitOption5YearTermLife)
                            lclcBenOptions.Add(new busCustomCodeValue() { code_value = Convert.ToString(dr["CODE_VALUE"]), description = Convert.ToString(dr["DESCRIPTION"]) });
                    }
                }
            }
            else if (astrBenType == busConstant.ApplicationBenefitTypeRefund)
            {
                lclcBenOptions.Add(new busCustomCodeValue() { code_value = busConstant.BenefitOptionRegularRefund, description = "Regular Refund" });
            }
            else if ((aintBenProvisionId == busConstant.PlanIdDC || aintBenProvisionId == busConstant.BenProvisionIdDC20)
                && astrBenType != busConstant.ApplicationBenefitTypeRefund) //PIR 20232
            {
                lclcBenOptions.Add(new busCustomCodeValue() { code_value = busConstant.BenefitOptionPeriodicPayment, description = "Periodic Payment" });
            }
            if (!ablnReturnCollection)
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Serialize(lclcBenOptions);
            }
            else
            {
                return lclcBenOptions;
            }
        }
        public busWssBenefitApplicationLookup LoadWSSBenefitApplicationRequests(DataTable adtbSearchResult)
        {
            busWssBenefitApplicationLookup lobjWssBenefitApplicationLookup = new busWssBenefitApplicationLookup();
            lobjWssBenefitApplicationLookup.LoadWssBenApps(adtbSearchResult);
            return lobjWssBenefitApplicationLookup;
        }
        public busWssBenApp FindWssBenAppRequest(int aintWssBenAppId)
        {
            busWssBenApp lbusWssBenApp = new busWssBenApp();
            if (lbusWssBenApp.FindWssBenApp(aintWssBenAppId, true, true))
            {
                lbusWssBenApp.LoadMemberPerson();
                lbusWssBenApp.LoadPlan();
                lbusWssBenApp.LoadPersonAccount();
                lbusWssBenApp.LoadPersonEmploymentDetail();
                lbusWssBenApp.LoadWssOtherDisabilityBenefits();
                lbusWssBenApp.LoadInsEnrlRequests();
                lbusWssBenApp.LoadInsPremACHDetailsAcknowledgement();
                lbusWssBenApp.LoadDependents();
                lbusWssBenApp.FindUploadedDocuements();
                lbusWssBenApp.LoadLifePersonAccount();
                lbusWssBenApp.LoadAllAcknowledgement();
                lbusWssBenApp.LoadActiveRetirementPlan();
                lbusWssBenApp.icdoWssBenAppTaxWithholdingFederal.refund_fed_percent = busConstant.RefundFedPercent;
                lbusWssBenApp.SetInsStepsAndPanelsVisibilityAndLoadEnrollmentObjects(lbusWssBenApp.icdoWssBenApp.modified_date);
            }
            return lbusWssBenApp;
        }
        public busDocUpload FindWssDocUpload(int aintPersonID)
        {
            //FMUpgrade: Changes for set Navigation parameter from menu item click
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmWssDocsUploadMaintenance")
            {
                aintPersonID = aintPersonID != 0 ? aintPersonID : iintPersonID;
            }
            busDocUpload lbusDocUpload = new busDocUpload { icdoDocUpload = new cdoDocUpload() };
            lbusDocUpload.icdoDocUpload.person_id = aintPersonID;
            return lbusDocUpload;
        }
        public ArrayList UploadWssDocs(byte[] uploadedFileContent, string astrDocumentId, string astrFileName)
        {
          ArrayList larlstResult = new ArrayList();
            try
            {
                if(!string.IsNullOrEmpty(astrDocumentId))
                {
                    //Find Document ID by name
                    object lobjDocumentId = DBFunction.DBExecuteScalar("entDocUpload.FindDocumentIdByDocName", new object[1] { astrDocumentId }, utlPassInfo.iobjPassInfo.iconFramework, utlPassInfo.iobjPassInfo.itrnFramework);
                    if(lobjDocumentId != null && lobjDocumentId != DBNull.Value)
                    {
                        astrDocumentId = Convert.ToString(lobjDocumentId);
                    }
                    else
                    {
                        larlstResult.Add(new utlError() { istrErrorMessage = "We Cannot Find The Entered Document In The System." });
                    }
                }
                else
                {
                    larlstResult.Add(new utlError() { istrErrorMessage = "Document Name Is Required." });
                }
                string lstrFullPathName = utlPassInfo.iobjPassInfo.isrvDBCache.GetPathInfo("DocsUpld");
                if (astrDocumentId == busConstant.DocumentIdMSSOtherDocuments)
                {
                    lstrFullPathName = utlPassInfo.iobjPassInfo.isrvDBCache.GetPathInfo("MSSDocs");
                }

                if (System.IO.Directory.Exists(lstrFullPathName))
                {
                    string lstrFileExtension = System.IO.Path.GetExtension(astrFileName);
                    string lstrFileName = System.IO.Path.GetFileNameWithoutExtension(astrFileName);
                    if (astrDocumentId == busConstant.DocumentIdMSSOtherDocuments)
                    {
                        SetWebParameters();
                        lstrFileName = lstrFileName + "_" + iintPersonID + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss.fff");                        
                    }
                    else
                    {
                        busDocUpload lbusDocUpload = new busDocUpload
                        {
                            icdoDocUpload = new cdoDocUpload
                            {
                                person_id = (int)iobjPassInfo.idictParams["PersonID"],
                                document_id = Convert.ToInt32(astrDocumentId),
                                doc_status_value = busConstant.CorrespondenceStatus_Ready_For_Imaging,
                                converted_to_image_flag = busConstant.Flag_No,
                                uploaded_date = DateTime.Now
                            }
                        };
                        lbusDocUpload.icdoDocUpload.Insert();
                        if (string.IsNullOrEmpty(lbusDocUpload.istrUploadedFileName)) return larlstResult;
                        lstrFileName = lbusDocUpload.istrUploadedFileName;
                    }
                    File.WriteAllBytes(lstrFullPathName + "\\" + lstrFileName + lstrFileExtension, uploadedFileContent);
                }
                return larlstResult;
            }
            catch (Exception _exc)
            {
                larlstResult.Add(new utlError() { istrErrorMessage = _exc.Message });
                return larlstResult;
            }
        }      
        //PIR 21231
        public ArrayList UploadEssDocs(byte[] uploadedFileContent, string astrFileName)
        {
            SetWebParameters();
            ArrayList larlstResult = new ArrayList();
            try
            {
                string lstrFullPathName = utlPassInfo.iobjPassInfo.isrvDBCache.GetPathInfo("ESSDocUpld");
                if (System.IO.Directory.Exists(lstrFullPathName))
                {
                    string lstrFileExtension = System.IO.Path.GetExtension(astrFileName);
                    string lstrFileName = System.IO.Path.GetFileNameWithoutExtension(astrFileName);
                    busOrganization lobjOrganization = new busOrganization();
                    lobjOrganization.FindOrganization(iintOrgID);
                    string lstrOrgCode = lobjOrganization.icdoOrganization.org_code;
                    lstrFileName = lstrFileName + "_" + lstrOrgCode + "_" + iintContactID + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss.fff");
                    File.WriteAllBytes(lstrFullPathName + "\\" + lstrFileName + lstrFileExtension, uploadedFileContent);
                }
                return larlstResult;
            }
            catch (Exception _exc)
            {
                larlstResult.Add(new utlError() { istrErrorMessage = _exc.Message });
                return larlstResult;
            }
        }
        public ArrayList ValidateSelectedPlan(busMSSRetBenAppWeb abusMSSRetBenAppWeb)
        {
            ArrayList larlstResult = new ArrayList();
            larlstResult = abusMSSRetBenAppWeb.ValidateSelectedPlan();
            return larlstResult;
        }
        //PIR 18503
        public busPayeeAccountAchDetail NewMSSPayeeAccountAchDetail(int Aintpayeeaccountid)
        {
            busPayeeAccountAchDetail lobjPayeeAccountAchDetail = new busPayeeAccountAchDetail();

            lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail = new cdoPayeeAccountAchDetail();
            lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.payee_account_id = Aintpayeeaccountid;
            lobjPayeeAccountAchDetail.LoadPayeeAccount();
            lobjPayeeAccountAchDetail.ibusPayeeAccount.LoadAllACHDetailsWithEndDateNullMSS();
            lobjPayeeAccountAchDetail.ibusPayeeAccount.LoadBenefitAmount();
            lobjPayeeAccountAchDetail.ibusPayeeAccount.LoadApplication();
            lobjPayeeAccountAchDetail.ibusPayeeAccount.ibusApplication.LoadPersonAccount();
            lobjPayeeAccountAchDetail.ibusPayeeAccount.LoadMember();
            lobjPayeeAccountAchDetail.ibusPayeeAccount.LoadPayee();
            lobjPayeeAccountAchDetail.LoadBankOrgByOrgID();
            lobjPayeeAccountAchDetail.LoadACHDetailsAcknowledgement();
            lobjPayeeAccountAchDetail.iblnIsFromMSS = true;
            lobjPayeeAccountAchDetail.InitializeObjects();
            lobjPayeeAccountAchDetail.SetPrimaryACHDetail();
            lobjPayeeAccountAchDetail.ibusPayeeAccount.LoadPayeePerson();
            lobjPayeeAccountAchDetail.EvaluateInitialLoadRules();

            return lobjPayeeAccountAchDetail;
        }

        public busOrganization GetBankNameByRoutingNumber1(string AintRoutingNumber1)
        {
            busPayeeAccountAchDetail lobjACHDetail = new busPayeeAccountAchDetail();
            return lobjACHDetail.GetBankNameByRoutingNumber1(AintRoutingNumber1);
        }

        public busOrganization GetBankNameByRoutingNumber2(string AintRoutingNumber2)
        {
            busPayeeAccountAchDetail lobjACHDetail = new busPayeeAccountAchDetail();
            return lobjACHDetail.GetBankNameByRoutingNumber2(AintRoutingNumber2);
        }

        public busWssPersonAccountACHDetail NewWssPersonAccountACHDetail(int AintPersonID)
        {
            busWssPersonAccountACHDetail lobjWssPersonAccountACHDetail = new busWssPersonAccountACHDetail() { icdoPersonAccountAchDetail = new cdoPersonAccountAchDetail() };
            if (lobjWssPersonAccountACHDetail.LoadPerson(AintPersonID))
            {
                lobjWssPersonAccountACHDetail.iintPersonID = lobjWssPersonAccountACHDetail.ibusPerson.icdoPerson.person_id;
                lobjWssPersonAccountACHDetail.LoadCurrentlyEnrolledPlans();
                lobjWssPersonAccountACHDetail.InitializeObjects();
            }

            return lobjWssPersonAccountACHDetail;
        }

        public busPayeeAccountAchDetail GetPartialAmountSel1(string AidecPartialAmount1)
        {
            busPayeeAccountAchDetail lobjACHDetail = new busPayeeAccountAchDetail();
            return lobjACHDetail.GetPartialAmountSel1(AidecPartialAmount1);
        }

        public busOrganization GetBankNameForWithdrawal(string AintRoutingNumber)
        {
            busWssPersonAccountACHDetail lobjWssPersonAccountACHDetail = new busWssPersonAccountACHDetail();
            return lobjWssPersonAccountACHDetail.GetBankNameForWithdrawal(AintRoutingNumber);
        }

        public bool IsGeneratedOTPExpired(int AintPersonID)
        {
            DataTable ldtCorrectOTP = busNeoSpinBase.Select("cdoWssOtpActivation.IsGeneratedOTPExpired",
                                        new object[2] { AintPersonID, busConstant.PayeeAccountACHDetailOTPSource });

            if (ldtCorrectOTP.Rows.Count == 0)
                return true;
            return false;
        }

        public bool IsEmailAddressNotWaived(int Aintpayeeaccountid)
        {
            DataTable ldtEmailAddressWaived = busNeoSpinBase.Select("cdoPayeeAccountAchDetail.IsEmailAddressWaived",
                                                new object[1] { Aintpayeeaccountid });
            if (ldtEmailAddressWaived.Rows.Count > 0)
                return true;
            return false;
        }
        //18492
        public string LoadMessage(int aintMsgId) => iobjPassInfo.isrvDBCache.GetMessageText(aintMsgId);
        //19351
        public busContactTicket FindMSSContactTicketDetailHistory(int aintContactTicketId)
        {
            busContactTicket lbusContactTicket = new busContactTicket { icdoContactTicket = new cdoContactTicket() };
            if (lbusContactTicket.FindContactTicket(aintContactTicketId))
            {
                lbusContactTicket.LoadContactTicketDetailHistory(aintContactTicketId);
            }
            return lbusContactTicket;
        }

        #region PIR-19997
        public busPersonAccountGhdvHsa FindMSSGHDVHSAHistory(int aintPersonAccountId)
        {
            busPersonAccountGhdvHsa lbusPersonAccountGhdvHsa = new busPersonAccountGhdvHsa() { icdoPersonAccountGhdvHsa = new doPersonAccountGhdvHsa() };
            lbusPersonAccountGhdvHsa.iintPersonAccountId = aintPersonAccountId;
            lbusPersonAccountGhdvHsa.iblnIsFromMSS = true;
            lbusPersonAccountGhdvHsa.iblnIsFromMSSForACK = true;//PIR 26812
            if (lbusPersonAccountGhdvHsa.FindPersonAccount(aintPersonAccountId))
            {
                if (lbusPersonAccountGhdvHsa.FindGHDVByPersonAccountID(aintPersonAccountId))
                {
                    lbusPersonAccountGhdvHsa.LoadGHDVHsaByPersonAccountGhdvID();
                    lbusPersonAccountGhdvHsa.LoadPerson();
                    lbusPersonAccountGhdvHsa.LoadPersonAccountGHDV();
                    lbusPersonAccountGhdvHsa.LoadPersonAccountGHDVHistory();
                }
            }
            return lbusPersonAccountGhdvHsa;
        }
        public busPersonAccountGhdvHsa FindMSSGHDVHSA(int aintPersonAccountId, int aintPersonAccountGhdvHsaId)
        {
            busPersonAccountGhdvHsa lbusPersonAccountGhdvHsa = new busPersonAccountGhdvHsa() { icdoPersonAccountGhdvHsa = new doPersonAccountGhdvHsa() };
            lbusPersonAccountGhdvHsa.iintPersonAccountId = aintPersonAccountId;
            lbusPersonAccountGhdvHsa.iblnIsFromMSS = true;
            lbusPersonAccountGhdvHsa.iblnIsSaveClick = false;
            lbusPersonAccountGhdvHsa.iblnIsFromMSSForACK = true;//PIR 26812

            if (lbusPersonAccountGhdvHsa.FindPersonAccount(aintPersonAccountId))
            {
                if (lbusPersonAccountGhdvHsa.FindGHDVByPersonAccountID(aintPersonAccountId))
                {
                    if (aintPersonAccountGhdvHsaId > 0)
                    {
                        lbusPersonAccountGhdvHsa.iintPersonAccountGhdvHSAId = aintPersonAccountGhdvHsaId;
                    }
                    lbusPersonAccountGhdvHsa.LoadGHDVHsaByPersonAccountGhdvID();
                    lbusPersonAccountGhdvHsa.LoadPersonAccount();
                    lbusPersonAccountGhdvHsa.LoadPlanEffectiveDate();
                    lbusPersonAccountGhdvHsa.DetermineEnrollmentAndLoadObjects(lbusPersonAccountGhdvHsa.idtPlanEffectiveDate, false);
                    lbusPersonAccountGhdvHsa.LoadPerson();
                    lbusPersonAccountGhdvHsa.LoadPersonAccountGHDV();
                    lbusPersonAccountGhdvHsa.LoadPersonAccountGHDVHistory();
                    lbusPersonAccountGhdvHsa.EvaluateInitialLoadRules();

                    DataTable ldtbListdtDTP1 = iobjPassInfo.isrvDBCache.GetCacheData("sgt_wss_acknowledgement", "screen_step_value = '" + busConstant.ScreenStepValueHSA + "' AND EFFECTIVE_DATE <= '" + DateTime.Now + "'"); //PIR 20986
                    var lvarRow = ldtbListdtDTP1.AsEnumerable().OrderByDescending(dr => dr.Field<DateTime>("EFFECTIVE_DATE")).ThenByDescending(dr => dr.Field<int>("DISPLAY_SEQUENCE")).FirstOrDefault();
                    if (lvarRow.IsNotNull())
                        lbusPersonAccountGhdvHsa.istrAcknowledgementText = Convert.ToString(lvarRow["acknowledgement_text"]);

                    busWssPersonAccountEnrollmentRequest lbusEnrollmentRequest = new busWssPersonAccountEnrollmentRequest
                    {
                        icdoWssPersonAccountEnrollmentRequest = new cdoWssPersonAccountEnrollmentRequest()
                    };
                    lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.person_id = lbusPersonAccountGhdvHsa.icdoPersonAccount.person_id;
                    lbusEnrollmentRequest.LoadPerson();
                    lbusPersonAccountGhdvHsa.istrConfirmationTextForHSA = lbusEnrollmentRequest.istrConfirmationText;
                }
            }
            return lbusPersonAccountGhdvHsa;
        }
        #endregion PIR-19997

        public busUploadFile NewUpload()
        {
            busUploadFile lbusUploadFile = new busUploadFile();
            return lbusUploadFile;
        }

        public busPayrollReportWeb FindMSSVideoPlayer(int aintPlanID)
        {
            busPayrollReportWeb lobjPayrollReportWeb = new busPayrollReportWeb();
            //PIR PIR 26955 - Learn More Link Goes to the Wrong Document
            DataTable ldtMSSVideoPath = busNeoSpinBase.Select("cdoCodeValue.LoadMSSVideoPath",
                                        new object[1] { aintPlanID });
            if (ldtMSSVideoPath.Rows.Count > 0)
                lobjPayrollReportWeb.astrVideoPath = Convert.ToString(ldtMSSVideoPath.Rows[0].Field<string>("DATA1")) + Convert.ToString(ldtMSSVideoPath.Rows[0].Field<string>("DATA2"));

            return lobjPayrollReportWeb;
        }
        
        /// <summary>
        /// FW Upgrade : Download Benefit Enrollment Report(Code Conversion).
        /// </summary>
        /// <returns></returns>
        public ArrayList DownloadBenefitEnrollmentReport(string astrFileName)
        {
            string lstrPDFDownloadFileName = string.Empty;
            string lstrFileExtention = string.Empty;
            ArrayList larrPlanProvisionForm = new ArrayList();
            lstrFileExtention = Path.GetExtension(astrFileName);
            lstrPDFDownloadFileName = Path.GetFileNameWithoutExtension(astrFileName) + lstrFileExtention;
            byte[] larrFileContent = null;
            if (astrFileName.IsNotNullOrEmpty())
            {
                FileInfo lfioFileInfo = new FileInfo(astrFileName);
                if (lfioFileInfo.IsNotNull() && lfioFileInfo.Exists)
                {
                    using (FileStream lobjFileStream = lfioFileInfo.OpenRead())
                    {
                        larrFileContent = new byte[lobjFileStream.Length];
                        lobjFileStream.Read(larrFileContent, 0, (int)lobjFileStream.Length);
                    }

                    larrPlanProvisionForm.Add(lstrPDFDownloadFileName);
                    larrPlanProvisionForm.Add(larrFileContent);
                    larrPlanProvisionForm.Add(busNeoSpinBase.DeriveMimeTypeFromFileName(astrFileName));
                    return larrPlanProvisionForm;
                }
                else
                {
                    larrPlanProvisionForm.Add(new utlError { istrErrorMessage = "File not found." });
                }
            }

            return larrPlanProvisionForm;
        }

        protected string checkSinglePlan()
        {
            SetWebParameters();
            busMSSRetBenAppWeb lbusMSSRetBenAppWeb = new busMSSRetBenAppWeb();
            string astrActionValue = lbusMSSRetBenAppWeb.VerifyPlansAndRedirectToWizard(iintPersonID);
            return astrActionValue;
        }

        public ArrayList btnVertifyOTP_Click(string AstrActivationCode)
        {
            SetWebParameters();
            ArrayList larrList = new ArrayList();
            object lbusPayeeAccountAchDetail = busMainBase.GetObjectFromDB("wfmMSSDirectDepositInformationMaintenance", 0);
            if (lbusPayeeAccountAchDetail is busPayeeAccountAchDetail)
            {
                larrList = ((busPayeeAccountAchDetail)lbusPayeeAccountAchDetail).btnVertifyOTP_Click(AstrActivationCode);
            }
            return larrList;
        }
        public ArrayList CreateMemberAnnualStatementDownload(int aintSelectionID, string astrReportName)
        {
            busMSSHome lbusMSSHome = new busMSSHome();
            ArrayList larlstResult = lbusMSSHome.CreateMemberAnnualStatementDownload(aintSelectionID, astrReportName);
            return larlstResult;
        }
        public ArrayList View1099RReport(int aint1099rID)
        {
            busMSSHome lbusMSSHome = new busMSSHome();
            ArrayList larlstResult = lbusMSSHome.View1099RReport(aint1099rID);
            return larlstResult;
        }

        public ArrayList ViewEssReport_Click(int aintOrgId, string astrReportName)
        {
            busESSHome lbusESSHome = new busESSHome();
            ArrayList larlstResult = lbusESSHome.ViewEssReport_Click(aintOrgId, astrReportName);
            return larlstResult;
        }

        public object LoadBenefitAccountTypeByPlan(int aintPlanId)
        {
            DateTime ldteResult = DateTime.MinValue;
            Collection<cdoCodeValue> lclcBenOptions = new Collection<cdoCodeValue>();
            Collection<busCustomCodeValue> lclcBenefitAccountType = new Collection<busCustomCodeValue>();
            object lbusBenefitCalculationWeb = busMainBase.GetObjectFromDB("wfmBenefitCalculationWebWizard", 0);
            if (lbusBenefitCalculationWeb is busBenefitCalculatorWeb)
            {
                ((busBenefitCalculatorWeb)lbusBenefitCalculationWeb).ibusBenefitCalculation.icdoBenefitCalculation.plan_id = aintPlanId;
                lclcBenOptions = ((busBenefitCalculatorWeb)lbusBenefitCalculationWeb).LoadBenefitAccountType();
            }
            foreach (cdoCodeValue lcdocodeValue in lclcBenOptions)
            {
                lclcBenefitAccountType.Add(new busCustomCodeValue() { code_value = Convert.ToString(lcdocodeValue.code_value), description = Convert.ToString(lcdocodeValue.description) });
            }
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(lclcBenefitAccountType);
        }
        public busBoardMemberVote NewBoardMemberVote(int aintBoardMemVoteId, int aintElectionID)
        {
            SetWebParameters();
            busBoardMemberVote lbusBoardMemberVote = new busBoardMemberVote() { icdoBoardMemberVote = new doBoardMemberVote() };
            lbusBoardMemberVote.icdoBoardMemberVote.election_id = aintElectionID;
            lbusBoardMemberVote.icdoBoardMemberVote.person_id = iintPersonID;
            lbusBoardMemberVote.LoadBoardMemberElectionCandidatesInRandomOrder();
            lbusBoardMemberVote.LoadBoardMemberElection();
            lbusBoardMemberVote.iclcBoardMemberElectedCandidates = new utlCollection<doBoardMemberElectionCandidate>(); 
            lbusBoardMemberVote.LoadStaticTexts();
            return lbusBoardMemberVote;
        }
        
		public ArrayList OIDCMSSUserValidate(string astrAuthCode, string astrTokenEndpoint, string astrClientId, string astrClientSecret, string astrRedirectUrl, string astrJwksUri, string astrIssuer, bool ablnIsMember)
        {
            ArrayList larrResult = new ArrayList();
            try
            {
                OIDCHelper lobjOIDCHelper = new OIDCHelper();
                string astrIdToken = null;
                Dictionary<string, string> ldictUserInfo = lobjOIDCHelper.ValidateJwtAndGetUniqueId(astrAuthCode, astrTokenEndpoint, astrClientId, astrClientSecret, astrRedirectUrl, astrJwksUri, astrIssuer, out astrIdToken);
                if (ldictUserInfo.IsNotNull() && ldictUserInfo.ContainsKey("username") && !string.IsNullOrEmpty(ldictUserInfo["username"]))
                {
                    if (ablnIsMember)
                    {
                        busPerson lbusPerson = LoadPersonByNDLoginID(ldictUserInfo["username"]);
                        if (lbusPerson.icdoPerson.person_id > 0 && lbusPerson.icdoPerson.date_of_death != DateTime.MinValue)
                        {
                            larrResult.Add(new utlError() { istrErrorMessage = "OIDCUserValidate : User Is Deceased" });
                        }
                        else
                        {
                            Dictionary<string, object> ldictUserProfileInfo = new Dictionary<string, object>();
                            ldictUserProfileInfo.Add("id_token", astrIdToken);
                            ldictUserProfileInfo.Add("ndpers_login_id", ldictUserInfo["username"]);
                            ldictUserProfileInfo.Add("email", ldictUserInfo["email"]);
                            ldictUserProfileInfo.Add("given_name", ldictUserInfo["given_name"]);
                            ldictUserProfileInfo.Add("family_name", ldictUserInfo["family_name"]);
                            ldictUserProfileInfo.Add("busPerson", lbusPerson);
                            larrResult.Add(ldictUserProfileInfo);
                        }
                    }
                    else
                    {
                        busContact lbusContact = LoadContactByNDLoginID(ldictUserInfo["username"]);
                        Dictionary<string, object> ldictUserProfileInfo = new Dictionary<string, object>();
                        ldictUserProfileInfo.Add("id_token", astrIdToken);
                        //PIR 26076 external user login issue, Remove the case sensitivity on the ESS and possibly the MSS login.
                        if (lbusContact.IsNotNull() && lbusContact.icdoContact.IsNotNull() && lbusContact.icdoContact.ndpers_login_id.IsNotNullOrEmpty())
                            ldictUserProfileInfo.Add("ndpers_login_id", lbusContact?.icdoContact?.ndpers_login_id);
                        else
                            ldictUserProfileInfo.Add("ndpers_login_id", ldictUserInfo["username"]);
                        ldictUserProfileInfo.Add("email", ldictUserInfo["email"]);
                        ldictUserProfileInfo.Add("given_name", ldictUserInfo["given_name"]);
                        ldictUserProfileInfo.Add("family_name", ldictUserInfo["family_name"]);
                        ldictUserProfileInfo.Add("busContact", lbusContact);
                        larrResult.Add(ldictUserProfileInfo);
                    }
                }
                else
                {
                    larrResult.Add(new utlError() { istrErrorMessage = "OIDCUserValidate : User Not Found" });
                }
            }
            catch (Exception ex)
            {
                larrResult.Add(new utlError() { istrErrorMessage = ex.Message });
            }
            return larrResult;
        }
        public bool IsBankInfoAlreadyExists(string astrRoutingNumber, string astrAccountNumber, string astrAccountType, string astrPersonAccountIds)
        {
            string[] laryPersonAccountIds = astrPersonAccountIds.TrimEnd(',').Split(',');
            bool lblnIsExists = false;
            foreach (string lstrPersonAccountId in laryPersonAccountIds)
            {
                if (lstrPersonAccountId.IsNotNullOrEmpty())
                {
                    DataTable ldtPAACHBankInfo = busNeoSpinBase.Select("entPersonAccountAchDetail.IsBankInfoAlreadyExists",
                                                    new object[4] { astrRoutingNumber, astrAccountNumber, astrAccountType, Convert.ToInt32(lstrPersonAccountId) });
                    if (ldtPAACHBankInfo.Rows.Count > 0)
                    {
                        lblnIsExists = true;
                        break;
                    }
                }
            }
            return lblnIsExists;
        }
        //PIR 24994
        public busESSHome FindESSDownloadReports(int aintOrgID, int aintContactID, string astrReportName)
        {
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmESSDownloadReportsMaintenance")
            {
                aintOrgID = aintOrgID != 0 ? aintOrgID : iintOrgID;
                aintContactID = aintContactID != 0 ? aintContactID : iintContactID;
            }
            busESSHome lobjESSHome = new busESSHome();
            lobjESSHome.ibusContact = new busContact { icdoContact = new cdoContact() };
            lobjESSHome.LoadOrganization(aintOrgID);

            lobjESSHome.iintContactId = aintContactID;
            lobjESSHome.iintOrgId = aintOrgID;
            lobjESSHome.istrReportName = astrReportName;

            return lobjESSHome;
        }
        //PIR 24994
        public ArrayList ViewRetirementContribution_Click(int aintOrgId, string astrReportName, DateTime admStartDate, DateTime admEndDate)
        {
            busESSHome lbusESSHome = new busESSHome();
            lbusESSHome.idtmStartDate = admStartDate;
            lbusESSHome.idtmEndDate = admEndDate;
            lbusESSHome.istrReportName = astrReportName;

            ArrayList larlstResult = lbusESSHome.ViewEssReport_Click(aintOrgId, astrReportName);
            return larlstResult;
        }
        
		public busPayeeAccountTaxWithholding NewMSSWFourRTaxWithholding(int Aintpayeeaccountid, string astrTaxIdentidifier)
        {
            busPayeeAccountTaxWithholding lobjPayeeAccountTaxWithholding = new busPayeeAccountTaxWithholding();
            lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding = new cdoPayeeAccountTaxWithholding();
            lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.payee_account_id = Aintpayeeaccountid;
            lobjPayeeAccountTaxWithholding.LoadPayeeAccount();
            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadApplication();
            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadPayee();
            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.ibusPayee.GetPersonCurrentAddressByType(busConstant.AddressTypePermanent, DateTime.Now);
            lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_identifier_value = astrTaxIdentidifier;
            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.iblnIsFromMSS = true; // PIR 8618
            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadTaxWithHoldingHistory();
            //pir 8618
            if (lobjPayeeAccountTaxWithholding.ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadNexBenefitPaymentDate();
            DateTime ldteSystBatchDate = busGlobalFunctions.GetSysManagementBatchDate();
            lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.start_date = ldteSystBatchDate.Day < 15 ? busGlobalFunctions.GetFirstDayofNextMonth(ldteSystBatchDate) : busGlobalFunctions.GetFirstDayofNextMonth(ldteSystBatchDate.AddMonths(1)); //PIR 21227
            lobjPayeeAccountTaxWithholding.EvaluateInitialLoadRules();
            return lobjPayeeAccountTaxWithholding;
        }
        public busPayeeAccountTaxWithholding NewMwpPaymentTaxWithholding(int Aintpayeeaccountid, string astrTaxIdentidifier)
        {
            busPayeeAccountTaxWithholding lobjPayeeAccountTaxWithholding = new busPayeeAccountTaxWithholding();
            lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding = new cdoPayeeAccountTaxWithholding();
            lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.payee_account_id = Aintpayeeaccountid;
            lobjPayeeAccountTaxWithholding.LoadPayeeAccount();
            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadApplication();
            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadMember();
            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadTotalTaxableAmount();
            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadBenefitAmount();
            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadPayee();
            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.ibusPayee.GetPersonCurrentAddressByType(busConstant.AddressTypePermanent, DateTime.Now);
            lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_identifier_value = astrTaxIdentidifier;


            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadStateTaxWithHoldingInfo();
            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadFedTaxWithHoldingInfo();
            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadTaxWithHoldingHistory();
            lobjPayeeAccountTaxWithholding.EvaluateInitialLoadRules();
            return lobjPayeeAccountTaxWithholding;
        }
    }
}
