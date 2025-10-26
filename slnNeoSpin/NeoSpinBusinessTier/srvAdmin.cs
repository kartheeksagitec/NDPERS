#region Using directives

using System;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using Sagitec.BusinessObjects;
using Sagitec.CustomDataObjects;
using NeoSpin.CustomDataObjects;
using NeoSpin.BusinessObjects;
using Sagitec.Common;
using System.IO;

#endregion

namespace NeoSpin.BusinessTier
{
    public class srvAdmin : srvNeoSpin
    {
        public srvAdmin()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public busCode FindCode(int aintCodeId)
        {
            busCode lobjCode = new busCode();
            if (lobjCode.FindCode(aintCodeId))
            {
                lobjCode.LoadCodeValues();
            }
            return lobjCode;
        }

        public busCode NewCode()
        {
            busCode lobjCode = new busCode();
            lobjCode.icdoCode = new cdoCode();
            return lobjCode;
        }

        public busCodeLookup LoadCodes(DataTable adtbSearchResult)
        {
            busCodeLookup lobjCodeLookup = new busCodeLookup();
            lobjCodeLookup.LoadCodes(adtbSearchResult);
            return lobjCodeLookup;
        }

        public busCodeValue FindCodeValue(int aintCodeValueId)
        {
            busCodeValue lobjCodeValue = new busCodeValue();
            if (lobjCodeValue.FindCodeValue(aintCodeValueId))
            {
                lobjCodeValue.LoadCode();
            }
            return lobjCodeValue;
        }

        public busCodeValue NewCodeValue(int aintCodeId)
        {
            busCodeValue lobjCodeValue = new busCodeValue();
            lobjCodeValue.icdoCodeValue = new cdoCodeValue();
            lobjCodeValue.icdoCodeValue.code_id = aintCodeId;
            lobjCodeValue.LoadCode();
            return lobjCodeValue;
        }

        public busMessages FindMessage(int aintMessageId)
        {
            busMessages lobjMessage = new busMessages();
            if (lobjMessage.FindMessage(aintMessageId))
            {
            }
            return lobjMessage;
        }

        public busMessages NewMessage()
        {
            busMessages lobjMessages = new busMessages();
            lobjMessages.icdoMessages = new cdoMessages();
            return lobjMessages;
        }

        public busMessagesLookup LoadMessages(DataTable adtbSearchResult)
        {
            busMessagesLookup lobjMessageLookup = new busMessagesLookup();
            lobjMessageLookup.LoadMessages(adtbSearchResult);
            return lobjMessageLookup;
        }

        public busUser NewUser(int aintUserSerialId)
        {
            busUser lobjUser = new busUser();
            lobjUser.icdoUser = new cdoUser();
            lobjUser.icdoUser.user_serial_id = aintUserSerialId;
            return lobjUser;
        }

        public busUser FindUser(int aintUserSerialId)
        {
            busUser lobjUser = new busUser();
            if (lobjUser.FindUser(aintUserSerialId))
            {
                lobjUser.LoadUserRoles();
                lobjUser.LoadSecurity();
                lobjUser.LoadSupervisor();
            }
            return lobjUser;
        }

        public busUserLookup LoadUser(DataTable adtbSearchResult)
        {
            busUserLookup lobjUserLookup = new busUserLookup();
            lobjUserLookup.LoadUsers(adtbSearchResult);
            return lobjUserLookup;
        }

        public busUserRoles FindUserRoles(int aintUserSerialId, int aintRoleId)
        {
            busUserRoles lobjUserRoles = new busUserRoles();
            if (lobjUserRoles.FindUserRoles(aintUserSerialId, aintRoleId))
            {
                lobjUserRoles.LoadUser();
                lobjUserRoles.LoadRoles();
            }
            return lobjUserRoles;
        }

        public busUserRoles NewUserRoles(int aintUserSerialId)
        {
            busUserRoles lobjUserRoles = new busUserRoles();
            lobjUserRoles.icdoUserRoles = new cdoUserRoles();
            lobjUserRoles.icdoUserRoles.user_serial_id = aintUserSerialId;
            lobjUserRoles.LoadUser();
            return lobjUserRoles;
        }

        public busResources FindResource(int aintResourceId)
        {
            busResources lobjResource = new busResources();
            if (lobjResource.FindResource(aintResourceId))
            {
                lobjResource.LoadSecurity();
            }
            return lobjResource;
        }

        public busResourcesLookup LoadResources(DataTable adtbSearchResult)
        {
            busResourcesLookup lobjResourceLookup = new busResourcesLookup();
            lobjResourceLookup.LoadResources(adtbSearchResult);
            return lobjResourceLookup;
        }

        public busPERSLinkRoles FindRole(int aintRoleId)
        {
            busPERSLinkRoles lobjRole = new busPERSLinkRoles();
            if (lobjRole.FindRole(aintRoleId))
            {
                lobjRole.LoadSecurity();
                lobjRole.LoadUsers();
            }
            return lobjRole;
        }

        public busRolesLookup LoadRoles(DataTable adtbSearchResult)
        {
            busRolesLookup lobjRoleLookup = new busRolesLookup();
            lobjRoleLookup.LoadRoles(adtbSearchResult);
            return lobjRoleLookup;
        }

        public busSecurity FindSecurity(int aintResourceId, int aintRoleId)
        {
            busSecurity lobjSecurity = new busSecurity();
            if (lobjSecurity.FindSecurity(aintResourceId, aintRoleId))
            {
                lobjSecurity.LoadResource();
                lobjSecurity.LoadRole();
            }
            return lobjSecurity;
        }

        public busSystemManagement FindSystemManagement()
        {
            busSystemManagement lobjSystemManagement = new busSystemManagement();
            if (lobjSystemManagement.FindSystemManagement())
            {
            }
            return lobjSystemManagement;
        }

        public busSystemPaths FindPath(int aintPathId)
        {
            busSystemPaths lobjSystemPath = new busSystemPaths();
            if (lobjSystemPath.FindPath(aintPathId))
            {
            }
            return lobjSystemPath;
        }

        public busSystemPathsLookup LoadPaths(DataTable adtbSearchResult)
        {
            busSystemPathsLookup lobjSystemPathsLookup = new busSystemPathsLookup();
            lobjSystemPathsLookup.LoadSystemPaths(adtbSearchResult);
            return lobjSystemPathsLookup;
        }

        public busBatchSchedule FindBatchSchedule(int aintBatchScheduleId)
        {
            busBatchSchedule lobjBatchSchedule = new busBatchSchedule();
            if (lobjBatchSchedule.FindBatchSchedule(aintBatchScheduleId))
            {
            }
            return lobjBatchSchedule;
        }

        public busBatchScheduleLookup LoadBatchSchedules(DataTable adtbSearchResult)
        {
            busBatchScheduleLookup lobjBatchScheduleLookup = new busBatchScheduleLookup();
            lobjBatchScheduleLookup.LoadBatchSchedule(adtbSearchResult);
            return lobjBatchScheduleLookup;
        }

        public busProcessLogLookup LoadProcessLog(DataTable adtbSearchResult)
        {
            busProcessLogLookup lobjProcessLogLookup = new busProcessLogLookup();
            lobjProcessLogLookup.LoadProcessLog(adtbSearchResult);
            return lobjProcessLogLookup;
        }

        public busFile FindFile(int aintFileId)
        {
            busFile lobjFile = new busFile();
            if (lobjFile.FindFile(aintFileId))
            {
            }
            return lobjFile;
        }

        public busFileLookup LoadFiles(DataTable adtbSearchResult)
        {
            busFileLookup lobjFileLookup = new busFileLookup();
            lobjFileLookup.LoadFiles(adtbSearchResult);
            return lobjFileLookup;
        }

        public busFileLayout FinFileLayout(int aintFileId, string astrTransactionCode)
        {
            busFileLayout lobjFileLayout = new busFileLayout();
            lobjFileLayout.FindFileLayout(aintFileId, astrTransactionCode);
            return lobjFileLayout;
        }

        public busFileHdr FindFileHdr(int aintFileHdrId)
        {
            busFileHdr lobjFileHdr = new busFileHdr();
            if (lobjFileHdr.FindFileHdr(aintFileHdrId))
            {
                lobjFileHdr.LoadFile();
                lobjFileHdr.LoadStatusSummary();
            }
            return lobjFileHdr;
        }

        public busFileHdrLookup LoadFileHdrs(DataTable adtbSearchResult)
        {
            busFileHdrLookup lobjFileHdrLookup = new busFileHdrLookup();
            lobjFileHdrLookup.LoadFileHdrs(adtbSearchResult);
            return lobjFileHdrLookup;
        }

        public busFileDtl FindFileDtl(int aintFileDtlId)
        {
            busFileDtl lobjFileDtl = new busFileDtl();
            if (lobjFileDtl.FindFileDtl(aintFileDtlId))
            {
                lobjFileDtl.LoadFileHdr();
                lobjFileDtl.LoadFileDtlErrors();
            }
            return lobjFileDtl;
        }

        public busFileDtlLookup LoadFileDtls(DataTable adtbSearchResult)
        {
            busFileDtlLookup lobjFileDtlLookup = new busFileDtlLookup();
            lobjFileDtlLookup.LoadFileDtls(adtbSearchResult);
            return lobjFileDtlLookup;
        }

        public busHelpIndexLookup LoadHelpIndexs(DataTable adtbSearchResult)
        {
            busHelpIndexLookup lobjHelpIndexLookup = new busHelpIndexLookup();
            lobjHelpIndexLookup.LoadHelpIndexs(adtbSearchResult);
            return lobjHelpIndexLookup;
        }

        public busGLTransactionLookup LoadGLTransaction(DataTable adtbSearchResult)
        {
            busGLTransactionLookup lobjGLTransaction = new busGLTransactionLookup();
            lobjGLTransaction.LoadGLTransaction(adtbSearchResult);
            return lobjGLTransaction;
        }

        public busGLReCreateFile FindGLReCreateFile(DateTime adtExtractDate)
        {
            busGLReCreateFile lobjGLReCreateFile = new busGLReCreateFile();
            lobjGLReCreateFile.icdoGLTransaction = new cdoGlTransaction();
            lobjGLReCreateFile.icdoGLTransaction.extract_date = adtExtractDate;
            lobjGLReCreateFile.LoadCollection();
            return lobjGLReCreateFile;
        }

        public busChartOfAccount FindChartOfAccount(int Aintchartofaccountid)
        {
            busChartOfAccount lobjChartOfAccount = new busChartOfAccount();
            if (lobjChartOfAccount.FindChartOfAccount(Aintchartofaccountid))
            {
            }
            return lobjChartOfAccount;
        }

        public busChartOfAccountLookup LoadChartOfAccount(DataTable adtbSearchResult)
        {
            busChartOfAccountLookup lobjChartOfAccount = new busChartOfAccountLookup();
            lobjChartOfAccount.LoadChartOfAccount(adtbSearchResult);
            return lobjChartOfAccount;
        }

        public busChartOfAccount NewChartOfAccount()
        {
            busChartOfAccount lobjChartOfAccount = new busChartOfAccount();
            lobjChartOfAccount.icdoChartOfAccount = new cdoChartOfAccount();
            return lobjChartOfAccount;
        }

        public busAccountReference FindAccountReference(int Aintaccountreferenceid)
        {
            busAccountReference lobjAccountReference = new busAccountReference();
            if (lobjAccountReference.FindAccountReference(Aintaccountreferenceid))
            {
                lobjAccountReference.LoadAccountNo();
            }

            return lobjAccountReference;
        }

        public busAccountReferenceLookup LoadAccountReference(DataTable adtbSearchResult)
        {
            busAccountReferenceLookup lobjAccountReference = new busAccountReferenceLookup();
            lobjAccountReference.LoadAccountReference(adtbSearchResult);
            return lobjAccountReference;
        }

        public busAccountReference NewAccountReference()
        {
            busAccountReference lobjAccountReference = new busAccountReference();
            lobjAccountReference.icdoAccountReference = new cdoAccountReference();
            lobjAccountReference.ibusDebitAccount = new busChartOfAccount();
            lobjAccountReference.ibusDebitAccount.icdoChartOfAccount = new cdoChartOfAccount();
            lobjAccountReference.ibusCreditAccount = new busChartOfAccount();
            lobjAccountReference.ibusCreditAccount.icdoChartOfAccount = new cdoChartOfAccount();
            return lobjAccountReference;
        }

        public busJournalHeaderLookup LoadJournalHeader(DataTable adtbSearchResult)
        {
            busJournalHeaderLookup lobjJournalHeader = new busJournalHeaderLookup();
            lobjJournalHeader.LoadJournalHeader(adtbSearchResult);
            return lobjJournalHeader;
        }

        public busJournalHeader NewJournalHeader()
        {
            busJournalHeader lobjJournalHeader = new busJournalHeader();
            lobjJournalHeader.icdoJournalHeader = new cdoJournalHeader();
            return lobjJournalHeader;
        }

        public busJournalHeader FindJournalHeader(int Aintjournalheaderid)
        {
            busJournalHeader lobjJournalHeader = new busJournalHeader();
            if (lobjJournalHeader.FindJournalHeader(Aintjournalheaderid))
            {
                lobjJournalHeader.LoadJournalDetails();
                lobjJournalHeader.LoadTotalDebitandCredit();
            }
            return lobjJournalHeader;
        }

        public busJournalDetail NewJournalDetail(int AintJournalHeaderID)
        {
            busJournalDetail lobjJournalDetail = new busJournalDetail();
            lobjJournalDetail.icdoJournalDetail = new cdoJournalDetail();
            lobjJournalDetail.icdoJournalDetail.journal_header_id = AintJournalHeaderID;
            lobjJournalDetail.LoadJournalHeader();
            lobjJournalDetail.LoadOtherJournalEntryDetails();
            lobjJournalDetail.LoadHeaderDebitsAndCredits();
            return lobjJournalDetail;
        }

        public busJournalDetail FindJournalDetail(int Aintjournaldetailid)
        {
            busJournalDetail lobjJournalDetail = new busJournalDetail();
            if (lobjJournalDetail.FindJournalDetail(Aintjournaldetailid))
            {
                lobjJournalDetail.icdoJournalDetail.istrOrgCodeID = busGlobalFunctions.GetOrgCodeFromOrgId(lobjJournalDetail.icdoJournalDetail.org_id);
                lobjJournalDetail.icdoJournalDetail.istrAccountNo = lobjJournalDetail.LoadAccountNo(lobjJournalDetail.icdoJournalDetail.account_id);
                lobjJournalDetail.LoadJournalHeader();
                lobjJournalDetail.LoadOtherJournalEntryDetails();
                lobjJournalDetail.LoadHeaderDebitsAndCredits();
            }
            return lobjJournalDetail;
        }

        public busNotes FindNotes(int Aintnoteid)
        {
            busNotes lobjNotes = new busNotes();
            if (lobjNotes.FindNotes(Aintnoteid))
            {
                if (lobjNotes.icdoNotes.person_id > 0)
                    lobjNotes.icdoNotes.person_name = busGlobalFunctions.GetPersonNameByPersonID(lobjNotes.icdoNotes.person_id);
                if (lobjNotes.icdoNotes.org_id > 0)
                    lobjNotes.icdoNotes.org_name = busGlobalFunctions.GetOrgNameByOrgID(lobjNotes.icdoNotes.org_id);
            }
            return lobjNotes;
        }

        public busNotes NewNotes(int aintPersonId, int aintOrgId, string astrSubsystemValue, int aintRefId, string astrSpecificFlag)
        {
            busNotes lobjNotes = new busNotes();
            lobjNotes.icdoNotes = new cdoNotes();
            lobjNotes.icdoNotes.subsystem_ref_id = aintRefId;
            lobjNotes.icdoNotes.subsystem_value = astrSubsystemValue;
            lobjNotes.icdoNotes.person_id = aintPersonId;
            if (aintPersonId > 0)
                lobjNotes.icdoNotes.person_name = busGlobalFunctions.GetPersonNameByPersonID(aintPersonId); // PROD PIR ID 5917
            lobjNotes.icdoNotes.org_id = aintOrgId;
            if (aintOrgId > 0)
                lobjNotes.icdoNotes.org_name = busGlobalFunctions.GetOrgNameByOrgID(aintOrgId); // PROD PIR ID 5917
            lobjNotes.icdoNotes.specific_flag = astrSpecificFlag;
            return lobjNotes;
        }

        public busItemTypeSourceTypeCrossref NewItemTypeSourceTypeCrossref()
        {
            busItemTypeSourceTypeCrossref lobjItemTypeSourceType = new busItemTypeSourceTypeCrossref();
            lobjItemTypeSourceType.icdoItemTypeSourceTypeCrossref = new cdoItemTypeSourceTypeCrossref();
            return lobjItemTypeSourceType;
        }

        public busItemTypeSourceTypeCrossref FindItemTypeSourceTypeCrossref(int Aintitemtypesourcetypecrossrefid)
        {
            busItemTypeSourceTypeCrossref lobjItemTypeSourceTypeCrossref = new busItemTypeSourceTypeCrossref();
            if (lobjItemTypeSourceTypeCrossref.FindItemTypeSourceTypeCrossref(Aintitemtypesourcetypecrossrefid))
            {
            }

            return lobjItemTypeSourceTypeCrossref;
        }

        public busItemTypeSourceTypeLookup LoadItemTypeSourceType(DataTable adtbSearchResult)
        {
            busItemTypeSourceTypeLookup lobjItemTypeSourceType = new busItemTypeSourceTypeLookup();
            lobjItemTypeSourceType.LoadItemTypeSourceType(adtbSearchResult);
            return lobjItemTypeSourceType;
        }

        public busCountyRef FindCityCountyRef(int Aintcountyrefid)
        {
            busCountyRef lobjCountyRef = new busCountyRef();
            if (lobjCountyRef.FindCountyRef(Aintcountyrefid))
            {
            }
            return lobjCountyRef;
        }
        public busCountyRef NewCityCountyCrossref()
        {
            busCountyRef lobjCountyRef = new busCountyRef();
            lobjCountyRef.icdoCountyRef = new cdoCountyRef();
            return lobjCountyRef;
        }

        public busCityCountyLookup LoadCityCountys(DataTable adtbSearchResult)
        {
            busCityCountyLookup lobjCityCountyLookup = new busCityCountyLookup();
            lobjCityCountyLookup.LoadCountyRef(adtbSearchResult);
            return lobjCityCountyLookup;
        }

        public busFileNetImages FindFileNetImages(int AintPersonid, int AintOrgId)
        {
            busFileNetImages lobjFileNetImages = new busFileNetImages();
            if (AintPersonid != 0)
            {
                lobjFileNetImages.ibusPerson = new busPerson();
                if (lobjFileNetImages.ibusPerson.FindPerson(AintPersonid))
                    lobjFileNetImages.ibusPerson.LoadWorkflowImageData();
            }
            else if (AintOrgId != 0)
            {
                lobjFileNetImages.ibusOrganization = new busOrganization();
                if (lobjFileNetImages.ibusOrganization.FindOrganization(AintOrgId))
                    lobjFileNetImages.ibusOrganization.LoadWorkflowImageData();
            }
            lobjFileNetImages.EvaluateInitialLoadRules();
            return lobjFileNetImages;
        }
        public busPaymentCheckBook NewPaymentCheckBook()
        {
            busPaymentCheckBook lobjPaymentCheckBook = new busPaymentCheckBook { icdoPaymentCheckBook = new cdoPaymentCheckBook() };
            return lobjPaymentCheckBook;
        }
        public busPaymentCheckBook FindPaymentCheckBook(int Aintcheckbookid)
        {
            busPaymentCheckBook lobjPaymentCheckBook = new busPaymentCheckBook();
            if (lobjPaymentCheckBook.FindPaymentCheckBook(Aintcheckbookid))
            {
            }
            return lobjPaymentCheckBook;
        }
        public busPaymentCheckBookLookup LoadPaymentCheckBooks(DataTable adtbSearchResult)
        {
            busPaymentCheckBookLookup lobjPaymentCheckBookLookup = new busPaymentCheckBookLookup();
            lobjPaymentCheckBookLookup.LoadPaymentCheckBooks(adtbSearchResult);
            return lobjPaymentCheckBookLookup;
        }

        # region UCS 40
        public busRateChangeLetterRequest NewRateChangeLetterRequest()
        {
            busRateChangeLetterRequest lobjRateChangeLetterRequest = new busRateChangeLetterRequest { icdoRateChangeLetterRequest = new cdoRateChangeLetterRequest() };
            lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.status_value = busConstant.LetterStatuValuePending;
            lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(4001, lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.status_value);
            lobjRateChangeLetterRequest.ibusProviderOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
            return lobjRateChangeLetterRequest;
        }

        public busDuesRateChangeRequest NewDuesRateChangeRequest(string astrVendorOrgCode)
        {
            busDuesRateChangeRequest lobjDuesRateChangeRequest = new busDuesRateChangeRequest { icdoDuesRateChangeRequest = new cdoDuesRateChangeRequest() };
            lobjDuesRateChangeRequest.icdoDuesRateChangeRequest.status_value = busConstant.StatusValid;
            lobjDuesRateChangeRequest.ibusVendor = new busOrganization { icdoOrganization = new cdoOrganization() };
            if (astrVendorOrgCode.IsNotNullOrEmpty())
            {
                lobjDuesRateChangeRequest.ibusVendor.FindOrganizationByOrgCode(astrVendorOrgCode);
                lobjDuesRateChangeRequest.icdoDuesRateChangeRequest.vendor_org_id = lobjDuesRateChangeRequest.ibusVendor.icdoOrganization.org_id;
                lobjDuesRateChangeRequest.icdoDuesRateChangeRequest.istrOrgCodeId = astrVendorOrgCode;
            }
            return lobjDuesRateChangeRequest;
        }

        public busRateChangeLetterRequest FindRateChangeLetterRequest(int aintratechangeletterrequestid)
        {
            busRateChangeLetterRequest lobjRateChangeLetterRequest = new busRateChangeLetterRequest();
            if (lobjRateChangeLetterRequest.FindRateChangeLetterRequest(aintratechangeletterrequestid))
            {
                lobjRateChangeLetterRequest.LoadProviderOrganization();
                lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.istrOrgCodeId = lobjRateChangeLetterRequest.ibusProviderOrganization.icdoOrganization.org_code;
            }

            return lobjRateChangeLetterRequest;
        }

        public busDuesRateChangeRequest FindDuesRateChangeRequest(int aintduesratechangerequestid)
        {
            busDuesRateChangeRequest lobjDuesRateChangeRequest = new busDuesRateChangeRequest();
            if (lobjDuesRateChangeRequest.FindDuesRateChangeRequest(aintduesratechangerequestid))
            {
                lobjDuesRateChangeRequest.LoadVendor();
                lobjDuesRateChangeRequest.icdoDuesRateChangeRequest.istrOrgCodeId = lobjDuesRateChangeRequest.ibusVendor.icdoOrganization.org_code;
            }

            return lobjDuesRateChangeRequest;
        }

        public busDuesRateChangeRequestLookup LoadDuesRateChangeRequests(DataTable adtbSearchResult)
        {
            busDuesRateChangeRequestLookup lobjDuesRateChangeRequestLookup = new busDuesRateChangeRequestLookup();
            lobjDuesRateChangeRequestLookup.LoadDuesRateChangeRequests(adtbSearchResult);
            return lobjDuesRateChangeRequestLookup;
        }

        public busRateChangeLetterRequestLookup LoadRateChangeLetterRequests(DataTable adtbSearchResult)
        {
            busRateChangeLetterRequestLookup lobjRateChangeLetterRequestLookup = new busRateChangeLetterRequestLookup();
            lobjRateChangeLetterRequestLookup.LoadRateChangeLetterRequests(adtbSearchResult);
            return lobjRateChangeLetterRequestLookup;
        }

        # endregion

        public busPlanMemberTypeCrossref NewPlanMemberTypeCrossref()
        {
            busPlanMemberTypeCrossref lobjPlanMemberTypeCrossref = new busPlanMemberTypeCrossref();
            lobjPlanMemberTypeCrossref.icdoPlanMemberTypeCrossref = new cdoPlanMemberTypeCrossref();
            return lobjPlanMemberTypeCrossref;
        }

		public busPlanMemberTypeCrossref FindPlanMemberTypeCrossref(int aintplanmembertypecrossrefid)
		{
			busPlanMemberTypeCrossref lobjPlanMemberTypeCrossref = new busPlanMemberTypeCrossref();
			if (lobjPlanMemberTypeCrossref.FindPlanMemberTypeCrossref(aintplanmembertypecrossrefid))
			{
			}

			return lobjPlanMemberTypeCrossref;
		}

		public busPlanMemberTypeLookup LoadPlanMemberTypes(DataTable adtbSearchResult)
		{
			busPlanMemberTypeLookup lobjPlanMemberTypeLookup = new busPlanMemberTypeLookup();
			lobjPlanMemberTypeLookup.LoadPlanMemberTypeCrossrefs(adtbSearchResult);
			return lobjPlanMemberTypeLookup;
		}
       
    }
}
