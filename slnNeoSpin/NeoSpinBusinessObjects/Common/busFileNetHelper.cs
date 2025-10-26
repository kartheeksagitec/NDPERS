using System;
using System.Collections;
using System.Collections.ObjectModel;
using NeoSpin.CustomDataObjects;
using Sagitec.Common;
using Sagitec.ExceptionPub;
using System.Linq;
using Microsoft.Web.Services3;
using Microsoft.Web.Services3.Security.Tokens;
using Microsoft.Web.Services3.Design;
using Sagitec.Bpm;
using Sagitec.BusinessObjects;

namespace NeoSpin.BusinessObjects
{
    public static class busFileNetHelper
    {
        public static Collection<busSolBpmProcessInstanceAttachments> LoadFileNetImages(string astrAdditionalInfo,
                                                                   string astrDocumentCode,
                                                                   DateTime adtInitiatedDate,
                                                                   int aintPersonID,
                                                                   string astrOrgCodeID)
        {
            string lstrImageDocCategory = string.Empty, lstrFileNetDocumentType=string.Empty;
            ProcessInstanceAttachmentsAdditionalInfo info = HelperFunction.JsonDeserialize(astrAdditionalInfo, typeof(ProcessInstanceAttachmentsAdditionalInfo)) as ProcessInstanceAttachmentsAdditionalInfo;
            if(info == null)
            {
                return new Collection<busSolBpmProcessInstanceAttachments>();
            }                
            else
            {
                lstrFileNetDocumentType = utlPassInfo.iobjPassInfo.isrvDBCache.GetCodeDescriptionString(info.FILENET_DOCUMENT_TYPE_ID, info.FILENET_DOCUMENT_TYPE_VALUE);
                lstrImageDocCategory = utlPassInfo.iobjPassInfo.isrvDBCache.GetCodeDescriptionString(info.IMAGE_DOC_CATEGORY_ID, info.IMAGE_DOC_CATEGORY_VALUE);
            }
                
            

            return LoadFileNetImages(lstrImageDocCategory, lstrFileNetDocumentType, astrDocumentCode, adtInitiatedDate, aintPersonID, astrOrgCodeID);
        }

            /// <summary>
            /// Loading the Filenet Images by given Criteria
            /// </summary>
            /// <param name="astrImageDocCategory"></param>
            /// <param name="astrFileNetDocumentType"></param>
            /// <param name="astrDocumentCode"></param>
            /// <param name="adtInitiatedDate"></param>
            /// <param name="aintPersonID"></param>
            /// <param name="astrOrgCodeID"></param>
            /// <returns></returns>
            public static Collection<busSolBpmProcessInstanceAttachments> LoadFileNetImages(string astrImageDocCategory,
                                                                   string astrFileNetDocumentType,
                                                                   string astrDocumentCode,
                                                                   DateTime adtInitiatedDate,
                                                                   int aintPersonID,
                                                                   string astrOrgCodeID)
        {
            Collection<busSolBpmProcessInstanceAttachments> _lclbWfImageData = new Collection<busSolBpmProcessInstanceAttachments>();
            try
            {
                // Create a wse-enabled web service object to provide access to SOAP header
                FNCEWS40ServiceWse wseService = InitializeFileNetWebService();

                // Specify the scope of the search
                ObjectStoreScope elemObjectStoreScope = new ObjectStoreScope();
                elemObjectStoreScope.objectStore = NeoSpin.Common.ApplicationSettings.Instance.OBJECT_STORE;
                // Create RepositorySearch
                RepositorySearch elemRepositorySearch = new RepositorySearch();
                elemRepositorySearch.repositorySearchMode = RepositorySearchModeType.Rows;
                elemRepositorySearch.repositorySearchModeSpecified = true;
                elemRepositorySearch.SearchScope = elemObjectStoreScope;
                string lstrSQL = String.Empty;
                if (aintPersonID != 0)
                {
                    lstrSQL =
                        String.Format(
                            "SELECT Id,VersionSeries,DocumentTitle,Date,NDShortName,SubjectTitle FROM [{0}] WHERE (DateCreated >={1}) AND NDShortName = '{2}' AND PERSLinkID = {3}",
                            astrImageDocCategory, adtInitiatedDate.ToString("yyyy-MM-dd"), astrDocumentCode, aintPersonID);
                }
                else if (!String.IsNullOrEmpty(astrOrgCodeID))
                {
                    lstrSQL =
                        String.Format(
                            "SELECT Id,VersionSeries,DocumentTitle,Date,NDShortName,SubjectTitle FROM [{0}] WHERE (DateCreated >={1}) AND NDShortName = '{2}' AND OrgCodeID = '{3}'",
                            astrImageDocCategory, adtInitiatedDate.ToString("yyyy-MM-dd"), astrDocumentCode, astrOrgCodeID);
                }
                elemRepositorySearch.SearchSQL = lstrSQL;

                // Invoke the ExecuteSearch operation
                ObjectSetType objObjectSet = wseService.ExecuteSearch(elemRepositorySearch);

                _lclbWfImageData = LoadFileNetDataByObjectSet(objObjectSet);

            }
            catch (Exception _exc)
            {
                ExceptionManager.Publish(_exc);
            }
            return _lclbWfImageData;
        }

        /// <summary>
        /// Loading the FileNet Images by Person
        /// </summary>
        /// <param name="aintPersonId"></param>
        /// <returns></returns>
        public static Collection<busSolBpmProcessInstanceAttachments> LoadFileNetImagesByPerson(int aintPersonId)
        {
            Collection<busSolBpmProcessInstanceAttachments> _lclbWfImageData = new Collection<busSolBpmProcessInstanceAttachments>();
            try
            {
                // Create a wse-enabled web service object to provide access to SOAP header
                FNCEWS40ServiceWse wseService = InitializeFileNetWebService();

                string lstrSQL = String.Format("SELECT Id,VersionSeries,DocumentTitle,Date,NDShortName,SubjectTitle FROM [Document] WHERE PERSLinkID = {0}", aintPersonId.ToString());

                ObjectSetType objObjectSet = ExecuteFileNetQuery(wseService, lstrSQL);

                _lclbWfImageData = LoadFileNetDataByObjectSet(objObjectSet);
            }
            catch (Exception _exc)
            {
                ExceptionManager.Publish(_exc);
            }
            return _lclbWfImageData;
        }

        /// <summary>
        /// Loading the FileNet Images by Org Code
        /// </summary>
        /// <param name="astrOrgCode"></param>
        /// <returns></returns>
        public static Collection<busSolBpmProcessInstanceAttachments> LoadFileNetImagesByOrgCode(string astrOrgCode)
        {
            Collection<busSolBpmProcessInstanceAttachments> _lclbWfImageData = new Collection<busSolBpmProcessInstanceAttachments>();
            try
            {
                // Create a wse-enabled web service object to provide access to SOAP header
                FNCEWS40ServiceWse wseService = InitializeFileNetWebService();

                // PROD PIR ID 5425 - Load only Org associated images.
                // PROD PIR ID 8491 - The PERSLinkID = 0 condition added to load all the images associated to the Organization.
                string lstrSQL = String.Format("SELECT Id,VersionSeries,DocumentTitle,Date,NDShortName,SubjectTitle FROM [Document] WHERE OrgCodeID = '{0}' AND (PERSLinkID IS NULL OR PERSLinkID = 0)", astrOrgCode);

                ObjectSetType objObjectSet = ExecuteFileNetQuery(wseService, lstrSQL);

                _lclbWfImageData = LoadFileNetDataByObjectSet(objObjectSet);
            }
            catch (Exception _exc)
            {
                ExceptionManager.Publish(_exc);
            }
            return _lclbWfImageData;
        }

        private static ObjectSetType ExecuteFileNetQuery(FNCEWS40ServiceWse aWseService, string astrSQL)
        {
            // Specify the scope of the search
            ObjectStoreScope elemObjectStoreScope = new ObjectStoreScope();
            elemObjectStoreScope.objectStore = NeoSpin.Common.ApplicationSettings.Instance.OBJECT_STORE;
            // Create RepositorySearch
            RepositorySearch elemRepositorySearch = new RepositorySearch();
            elemRepositorySearch.repositorySearchMode = RepositorySearchModeType.Rows;
            elemRepositorySearch.repositorySearchModeSpecified = true;
            elemRepositorySearch.SearchScope = elemObjectStoreScope;
            elemRepositorySearch.SearchSQL = astrSQL;

            // Invoke the ExecuteSearch operation
            return aWseService.ExecuteSearch(elemRepositorySearch);
        }

        private static Collection<busSolBpmProcessInstanceAttachments> LoadFileNetDataByObjectSet(ObjectSetType aobjObjectSet)
        {
            Collection<busSolBpmProcessInstanceAttachments> lclbWfImageData = new Collection<busSolBpmProcessInstanceAttachments>();
            int hitCount = (aobjObjectSet.Object == null) ? 0 : aobjObjectSet.Object.Length;
            SingletonObject singleObject;
            SingletonString singleString;
            SingletonId singleId;
            SingletonDateTime singletonDateTime;
            if (hitCount > 0)
            {
                foreach (ObjectValue ov in aobjObjectSet.Object)
                {
                    busSolBpmProcessInstanceAttachments lobjWfImageData = new busSolBpmProcessInstanceAttachments();
                    lobjWfImageData.icdoBpmProcessInstanceAttachments = new doBpmPrcsInstAttachments();
                    lobjWfImageData.object_store = NeoSpin.Common.ApplicationSettings.Instance.OBJECT_STORE;
                    foreach (PropertyType pt in ov.Property)
                    {
                        if (pt.GetType().Equals(typeof(SingletonId)))
                        {
                            if (pt.propertyId == busConstant.FileNetProperty_DocumentID)
                            {
                                singleId = (SingletonId)pt;
                                lobjWfImageData.document_id = singleId.Value;
                            }
                        }
                        else if (pt.GetType().Equals(typeof(SingletonObject)))
                        {
                            if (pt.propertyId == busConstant.FileNetProperty_VersionSeriesID)
                            {
                                singleObject = (SingletonObject)pt;
                                ObjectReference objReference = (ObjectReference)singleObject.Value;
                                lobjWfImageData.version_series_id = objReference.objectId;
                            }
                        }
                        else if (pt.GetType().Equals(typeof(SingletonString)))
                        {
                            if (pt.propertyId == busConstant.FileNetProperty_DocumentTitle)
                            {
                                singleString = (SingletonString)pt;
                                lobjWfImageData.document_title = singleString.Value;
                            }
                            else if (pt.propertyId == busConstant.FileNetProperty_ShortName)
                            {
                                singleString = (SingletonString)pt;
                                lobjWfImageData.short_name = singleString.Value;
                            }
                            else if (pt.propertyId == busConstant.FileNetProperty_SubjectTitle)
                            {
                                singleString = (SingletonString)pt;
                                lobjWfImageData.subject_title = singleString.Value;
                            }
                        }
                        else if (pt.GetType().Equals(typeof(SingletonDateTime)))
                        {
                            if (pt.propertyId == busConstant.FileNetProperty_InitiatedDate)
                            {
                                singletonDateTime = (SingletonDateTime)pt;
                                lobjWfImageData.initiated_date = singletonDateTime.Value;
                            }
                        }
                    }
                    if (!String.IsNullOrEmpty(lobjWfImageData.document_title))
                    {
                        //Avoid Duplicate Images.. Skip if already added.
                        if (!lclbWfImageData.Any(i => i.document_title == lobjWfImageData.document_title))
                            lclbWfImageData.Add(lobjWfImageData);
                    }
                }
            }
            return lclbWfImageData;
        }

        private static FNCEWS40ServiceWse InitializeFileNetWebService()
        {
            FNCEWS40ServiceWse wseService = new FNCEWS40ServiceWse();
            wseService.Url = NeoSpin.Common.ApplicationSettings.Instance.FILENET_CONTENT_ENGINE_WS_API_URL;
            UsernameToken token = new UsernameToken(NeoSpin.Common.ApplicationSettings.Instance.NDPERS_USERNAME,
                                                    HelperFunction.SagitecDecrypt(null, NeoSpin.Common.ApplicationSettings.Instance.NDPERS_PASSWORD),
                                                    PasswordOption.SendPlainText);
            wseService.SetClientCredential(token);
            Policy MyPolicy = new Policy();
            MyPolicy.Assertions.Add(new UsernameOverTransportAssertion());
            wseService.SetPolicy(MyPolicy);
            wseService.RequireMtom = true;

            Localization defaultLocale = new Localization();
            defaultLocale.Locale = "en-US";         // Build the Create action for a document
            return wseService;
        }

        /// <summary>
        /// Upload the Generated Coresspondence (TIFF) to FileNet 
        /// along with indexing data
        /// </summary>
        /// <param name="astrFileName"></param>
        /// <param name="aintPersonID"></param>
        /// <param name="astrOrgCode"></param>
        /// <returns></returns>
        public static ArrayList UploadFileNetDocument(string astrFileName, int aintPersonID, string astrOrgCode)
        {
            string lstrDocumentTitle = astrFileName.Substring(astrFileName.LastIndexOf("\\") + 1);

            string lstrTemplateName = lstrDocumentTitle.Substring(0, lstrDocumentTitle.LastIndexOf("-"));
            string lstrFolderName = String.Empty;
            string lstrFileNetDocumentType = String.Empty;
            string lstrDocumentCode = String.Empty;
            string lstrSubjectTitle = String.Empty;
            busCorTemplates lobjCorTemplates = new busCorTemplates();
            if (lobjCorTemplates.FindCorTemplatesByTemplateName(lstrTemplateName))
            {
                lstrFolderName = lobjCorTemplates.icdoCorTemplates.image_doc_category_description;
                lstrFileNetDocumentType = lobjCorTemplates.icdoCorTemplates.filenet_document_type_description;
                lstrDocumentCode = lobjCorTemplates.icdoCorTemplates.document_code;
                lstrSubjectTitle = lobjCorTemplates.icdoCorTemplates.template_desc;
            }
            return UploadFileNetDocument(astrFileName, aintPersonID, astrOrgCode, lstrDocumentTitle, lstrTemplateName, lstrFolderName, lstrFileNetDocumentType, lstrDocumentCode, lstrSubjectTitle);
        }
        public static ArrayList UploadFileNetDocumentWSS(string astrFileName, int aintPersonID, string astrOrgCode)
        {
            string lstrDocumentTitle = astrFileName.Substring(astrFileName.LastIndexOf("\\") + 1);
            string lstrDocumentId = lstrDocumentTitle.Substring(0, lstrDocumentTitle.LastIndexOf("-"));
            string lstrFolderName = String.Empty;
            string lstrFileNetDocumentType = String.Empty;
            string lstrDocumentCode = String.Empty;
            string lstrSubjectTitle = String.Empty;
            busBpmEvent lbusDocument = new busBpmEvent();
            if(lbusDocument.FindByPrimaryKey(Convert.ToInt32(lstrDocumentId)))
            {
                lstrFolderName = lbusDocument.icdoBpmEvent.screen_id == busConstant.MSSPortal ? busConstant.ImageDocCategoryMember : busConstant.ImageDocCategoryOrganization;
                lstrFileNetDocumentType = lbusDocument.icdoBpmEvent.document_category;
                lstrDocumentCode = lbusDocument.icdoBpmEvent.doc_type;
                lstrSubjectTitle = lbusDocument.icdoBpmEvent.event_desc;
            }
            return UploadFileNetDocument(astrFileName, aintPersonID, astrOrgCode, lstrDocumentTitle, lstrDocumentId, lstrFolderName, lstrFileNetDocumentType, lstrDocumentCode, lstrSubjectTitle);
        }
        public static ArrayList UploadFileNetDocument(string astrFileName, int aintPersonID, string astrOrgCode,
            string astrDocumentTitle, string astrTemplateName, string astrFolderName, string astrFileNetDocumentType,
            string astrDocumentCode, string astrSubjectTitle)
        {
            ArrayList larrErrorList = new ArrayList();
            try
            {
                string lstrPersonName = String.Empty;
                string lstrOrgName = String.Empty;

                //Getting the Person Name
                if (aintPersonID != 0)
                {
                    busPerson lobjPerson = new busPerson();
                    if (lobjPerson.FindPerson(aintPersonID))
                    {
                        lstrPersonName = lobjPerson.icdoPerson.PersonName;
                    }
                }

                if (!String.IsNullOrEmpty(astrOrgCode))
                {
                    busOrganization lobjOrganization = new busOrganization();
                    if (lobjOrganization.FindOrganizationByOrgCode(astrOrgCode))
                    {
                        lstrOrgName = lobjOrganization.icdoOrganization.org_name;
                    }
                }

                //System.DateTime dateCreated = new System.DateTime();         
                // Create a wse-enabled web service object to provide access to SOAP header
                FNCEWS40ServiceWse wseService = InitializeFileNetWebService();

                CreateAction verbCreate = new CreateAction();
                verbCreate.classId = astrFolderName;
                // Build the Checkin action
                CheckinAction verbCheckin = new CheckinAction();
                verbCheckin.checkinMinorVersion = false;
                verbCheckin.checkinMinorVersionSpecified = false;

                // Assign the actions to the ChangeRequestType element
                ChangeRequestType elemChangeRequestType = new ChangeRequestType();
                elemChangeRequestType.Action = new ActionType[2];
                elemChangeRequestType.Action[0] = (ActionType)verbCreate; // Assign Create action
                elemChangeRequestType.Action[1] = (ActionType)verbCheckin; // Assign Checkin action

                // Specify the target object (an object store) for the actions
                elemChangeRequestType.TargetSpecification = new ObjectReference();
                elemChangeRequestType.TargetSpecification.classId = "ObjectStore";
                elemChangeRequestType.TargetSpecification.objectId = NeoSpin.Common.ApplicationSettings.Instance.OBJECT_STORE;
                elemChangeRequestType.id = "1";

                // Build a list of properties to set in the new doc
                ModifiablePropertyType[] elemInputProps = new ModifiablePropertyType[10];

                // DocumentTitle property
                SingletonString propDocumentTitle = new SingletonString();
                propDocumentTitle.Value = astrDocumentTitle;
                propDocumentTitle.propertyId = "DocumentTitle";
                elemInputProps[0] = propDocumentTitle;

                //Document Type
                SingletonString propDocumentType = new SingletonString();
                propDocumentType.Value = astrFileNetDocumentType;
                propDocumentType.propertyId = "DocumentType";
                elemInputProps[1] = propDocumentType;

                //PERSLink ID                
                SingletonInteger32 propPERSLinkID = new SingletonInteger32();
                propPERSLinkID.Value = aintPersonID;
                propPERSLinkID.propertyId = "PERSLinkID";
                propPERSLinkID.ValueSpecified = true;
                elemInputProps[2] = propPERSLinkID;

                //Org Code ID
                SingletonString propOrgCode = new SingletonString();
                propOrgCode.Value = astrOrgCode;
                propOrgCode.propertyId = "OrgCodeID";
                elemInputProps[3] = propOrgCode;

                //Document Code (Short Name)
                SingletonString propDocumentCode = new SingletonString();
                propDocumentCode.Value = astrDocumentCode;
                propDocumentCode.propertyId = "NDShortName";
                elemInputProps[4] = propDocumentCode;

                //Person Name
                SingletonString propPersonName = new SingletonString();
                propPersonName.Value = lstrPersonName;
                propPersonName.propertyId = "Names";
                elemInputProps[5] = propPersonName;

                //Org Name
                SingletonString propOrgName = new SingletonString();
                propOrgName.Value = lstrOrgName;
                propOrgName.propertyId = "OrganizationName";
                elemInputProps[6] = propOrgName;

                //Subject Title
                SingletonString propSubjectTitle = new SingletonString();
                propSubjectTitle.Value = astrSubjectTitle;
                propSubjectTitle.propertyId = "SubjectTitle";
                elemInputProps[7] = propSubjectTitle;

                SingletonDateTime propInitiatedDate = new SingletonDateTime();
                propInitiatedDate.Value = DateTime.Now;
                propInitiatedDate.propertyId = "Date";
                propInitiatedDate.ValueSpecified = true;
                elemInputProps[8] = propInitiatedDate;

                // Create an object reference to dependently persistable ContentTransfer object
                DependentObjectType objContentTransfer = new DependentObjectType();
                objContentTransfer.classId = "ContentTransfer";
                objContentTransfer.dependentAction = DependentObjectTypeDependentAction.Insert;
                objContentTransfer.dependentActionSpecified = true;
                objContentTransfer.Property = new PropertyType[2];

                // Create reference to the object set of ContentTransfer objects returned by the Document.ContentElements property
                ListOfObject propContentElement = new ListOfObject();
                propContentElement.propertyId = "ContentElements";
                propContentElement.Value = new DependentObjectType[1];
                propContentElement.Value[0] = objContentTransfer;

                // Read data stream from file containing the document content
                InlineContent objInlineContent = new InlineContent();
                System.IO.Stream inputStream = System.IO.File.OpenRead(astrFileName);
                objInlineContent.Binary = new byte[inputStream.Length];                
                inputStream.Read(objInlineContent.Binary, 0, (int)inputStream.Length);
                inputStream.Close();

                // Create reference to Content pseudo-property
                ContentData prpContent = new ContentData();
                prpContent.Value = (ContentType)objInlineContent;
                prpContent.propertyId = "Content";

                // Assign Content property to ContentTransfer object 
                objContentTransfer.Property[0] = prpContent;

                SingletonString propContentType = new SingletonString();
                propContentType.propertyId = "ContentType";
                propContentType.Value = "image/tiff"; // Set MIME-type to Tiff
                objContentTransfer.Property[1] = propContentType;

                elemInputProps[9] = propContentElement;         // Assign list of document properties to set in ChangeRequestType element
                elemChangeRequestType.ActionProperties = elemInputProps;

                // Build a list of properties to exclude on the new doc object that will be returned
                string[] excludeProps = new string[2];
                excludeProps[0] = "Owner";
                excludeProps[1] = "DateLastModified";

                // Assign the list of excluded properties to the ChangeRequestType element
                elemChangeRequestType.RefreshFilter = new PropertyFilterType();
                elemChangeRequestType.RefreshFilter.ExcludeProperties = excludeProps;

                // Create array of ChangeRequestType elements and assign ChangeRequestType element to it 
                ChangeRequestType[] elemChangeRequestTypeArray = new ChangeRequestType[1];
                elemChangeRequestTypeArray[0] = elemChangeRequestType;         // Create ChangeResponseType element array 
                ChangeResponseType[] elemChangeResponseTypeArray = null;

                // Build ExecuteChangesRequest element and assign ChangeRequestType element array to it
                ExecuteChangesRequest elemExecuteChangesRequest = new ExecuteChangesRequest();
                elemExecuteChangesRequest.ChangeRequest = elemChangeRequestTypeArray;
                elemExecuteChangesRequest.refresh = true; // return a refreshed object
                elemExecuteChangesRequest.refreshSpecified = true;

                try
                {
                    // Call ExecuteChanges operation to implement the doc creation and checkin
                    elemChangeResponseTypeArray = wseService.ExecuteChanges(elemExecuteChangesRequest);
                }
                catch (System.Net.WebException ex)
                {
                    utlError lobjError = new utlError();
                    lobjError.istrErrorMessage = "ERROR : " + "An exception occurred while creating a document: [" + ex.Message + "]";
                    larrErrorList.Add(lobjError);
                    return larrErrorList;
                }
                catch (Exception _exc)
                {
                    utlError lobjError = new utlError();
                    lobjError.istrErrorMessage = "ERROR : " + _exc.Message;
                    larrErrorList.Add(lobjError);
                    return larrErrorList;
                }
            }
            catch (Exception _exc)
            {
                utlError lobjError = new utlError();
                lobjError.istrErrorMessage = "ERROR : " + _exc.Message;
                larrErrorList.Add(lobjError);
            }
            return larrErrorList;
        }

        /// <summary>
        /// Process the Uploaded Document. (CheckIn the Document)
        /// </summary>
        /// <param name="astrFileName"></param>
        /// <returns></returns>
        public static ArrayList ProcessFileNetDocument(string astrFileName)
        {
            string lstrDocumentTitle = astrFileName.Substring(astrFileName.LastIndexOf("\\") + 1);

            string lstrTemplateName = lstrDocumentTitle.Substring(0, lstrDocumentTitle.LastIndexOf("-"));
            string lstrFolderName = String.Empty;
            busCorTemplates lobjCorTemplates = new busCorTemplates();
            if (lobjCorTemplates.FindCorTemplatesByTemplateName(lstrTemplateName))
            {
                lstrFolderName = lobjCorTemplates.icdoCorTemplates.image_doc_category_description;
            }
            return ProcessFileNetDocument(astrFileName, lstrFolderName, lstrDocumentTitle);

        }

        public static ArrayList ProcessFileNetDocumentWSS(string astrFileName, string astrWssIdentifier)
        {
            string lstrDocumentTitle = astrFileName.Substring(astrFileName.LastIndexOf("\\") + 1);
            string lstrFolderName = (astrWssIdentifier == busConstant.MSSPortal) ? busConstant.ImageDocCategoryMember : busConstant.ImageDocCategoryOrganization;
            return ProcessFileNetDocument(astrFileName, lstrFolderName, lstrDocumentTitle);
        }
        public static ArrayList ProcessFileNetDocument(string astrFileName, string astrFolderName, string astrDocumentTitle)
        {
            ArrayList larrErrorList = new ArrayList();
            try
            {
                string containmentName = String.Empty;
                //System.DateTime dateCreated = new System.DateTime();         
                FNCEWS40ServiceWse wseService = InitializeFileNetWebService();

                // Build the Create action for a DynamicReferentialContainmentRelationship object
                CreateAction verbCreate = new CreateAction();
                verbCreate.classId = "DynamicReferentialContainmentRelationship";          // Assign the action to the ChangeRequestType element
                ChangeRequestType elemChangeRequestType = new ChangeRequestType();
                elemChangeRequestType.Action = new ActionType[1];
                elemChangeRequestType.Action[0] = (ActionType)verbCreate; // Assign Create action

                // Specify the target object (an object store) for the actions
                elemChangeRequestType.TargetSpecification = new ObjectReference();
                elemChangeRequestType.TargetSpecification.classId = "ObjectStore";
                elemChangeRequestType.TargetSpecification.objectId = NeoSpin.Common.ApplicationSettings.Instance.OBJECT_STORE;
                elemChangeRequestType.id = "1";         // Build a list of properties to set in the new doc
                ModifiablePropertyType[] elemInputProps = new ModifiablePropertyType[3];         // Specify and set a string-valued property for the ContainmentName property
                SingletonString propContainmentName = new SingletonString();
                propContainmentName.propertyId = "ContainmentName";
                propContainmentName.Value = astrDocumentTitle;
                elemInputProps[0] = propContainmentName; // Add to property list         // Specify the scope of the search
                ObjectStoreScope elemObjectStoreScope = new ObjectStoreScope();
                elemObjectStoreScope.objectStore = NeoSpin.Common.ApplicationSettings.Instance.OBJECT_STORE;         // Create the search for unfiled doc
                RepositorySearch elemRepositorySearch = new RepositorySearch();
                elemRepositorySearch.repositorySearchMode = RepositorySearchModeType.Rows;
                elemRepositorySearch.repositorySearchModeSpecified = true;
                elemRepositorySearch.SearchScope = elemObjectStoreScope;
                elemRepositorySearch.SearchSQL = "SELECT [Id] FROM [Document] WHERE ([DocumentTitle] = '" + astrDocumentTitle + "')";

                // Invoke the ExecuteSearch operation
                ObjectSetType objObjectSet = wseService.ExecuteSearch(elemRepositorySearch);         // Get ID of the first doc returned
                SingletonId propId = (SingletonId)objObjectSet.Object[0].Property[0];

                // Create an object reference to the document to file
                ObjectReference objDocument = new ObjectReference();
                objDocument.classId = astrFolderName;
                objDocument.objectStore = NeoSpin.Common.ApplicationSettings.Instance.OBJECT_STORE;
                objDocument.objectId = propId.Value;         // Create an object reference to the folder in which to file the document
                ObjectSpecification objFolder = new ObjectSpecification();
                objFolder.classId = "Folder";
                objFolder.objectStore = NeoSpin.Common.ApplicationSettings.Instance.OBJECT_STORE;
                objFolder.path = "/" + astrFolderName;         // Specify and set an object-valued property for the Head property
                SingletonObject propHead = new SingletonObject();
                propHead.propertyId = "Head";
                propHead.Value = (ObjectEntryType)objDocument; // Set its value to the Document object
                elemInputProps[1] = propHead; // Add to property list         // Specify and set an object-valued property for the Tail property
                SingletonObject propTail = new SingletonObject();
                propTail.propertyId = "Tail";
                propTail.Value = (ObjectEntryType)objFolder; // Set its value to the folder object
                elemInputProps[2] = propTail; // Add to property list

                // Assign list of DRCR properties to set in ChangeRequestType element
                elemChangeRequestType.ActionProperties = elemInputProps;         // Build a list of properties to exclude on the new DRCR object that will be returned
                string[] excludeProps = new string[2];
                excludeProps[0] = "Owner";
                excludeProps[1] = "DateLastModified";         // Assign the list of excluded properties to the ChangeRequestType element
                elemChangeRequestType.RefreshFilter = new PropertyFilterType();
                elemChangeRequestType.RefreshFilter.ExcludeProperties = excludeProps;         // Create array of ChangeRequestType elements and assign ChangeRequestType element to it 
                ChangeRequestType[] elemChangeRequestTypeArray = new ChangeRequestType[1];
                elemChangeRequestTypeArray[0] = elemChangeRequestType;         // Create ChangeResponseType element array 
                ChangeResponseType[] elemChangeResponseTypeArray = null;

                // Build ExecuteChangesRequest element and assign ChangeRequestType element array to it
                ExecuteChangesRequest elemExecuteChangesRequest = new ExecuteChangesRequest();
                elemExecuteChangesRequest.ChangeRequest = elemChangeRequestTypeArray;
                elemExecuteChangesRequest.refresh = true; // return a refreshed object
                elemExecuteChangesRequest.refreshSpecified = true; try
                {
                    // Call ExecuteChanges operation to implement the DRCR creation
                    elemChangeResponseTypeArray = wseService.ExecuteChanges(elemExecuteChangesRequest);
                }
                catch (System.Net.WebException ex)
                {
                    utlError lobjError = new utlError();
                    lobjError.istrErrorMessage = "ERROR : " + "An exception occurred while creating a folder: [" + ex.Message + "]";
                    larrErrorList.Add(lobjError);
                    return larrErrorList;
                }         // The new DRCR object should be returned, unless there is an error
                if (elemChangeResponseTypeArray == null || elemChangeResponseTypeArray.Length < 1)
                {
                    utlError lobjError = new utlError();
                    lobjError.istrErrorMessage = "ERROR : " + "A valid object was not returned from the ExecuteChanges operation";
                    larrErrorList.Add(lobjError);
                    return larrErrorList;
                }
            }
            catch (Exception _exc)
            {
                utlError lobjError = new utlError();
                lobjError.istrErrorMessage = "ERROR : " + _exc.Message;
                larrErrorList.Add(lobjError);
            }
            return larrErrorList;
        }

        /// <summary>
        /// Loading the FileNet Images by Person
        /// </summary>
        /// <param name="aintPersonId"></param>
        /// <returns></returns>
        public static Collection<busSolBpmProcessInstanceAttachments> LoadFileNetImagesByPersonDate(int aintPersonId, DateTime adtFromDate, DateTime adtToDate)
        {
            Collection<busSolBpmProcessInstanceAttachments> _lclbWfImageData = new Collection<busSolBpmProcessInstanceAttachments>();
            try
            {
                // Create a wse-enabled web service object to provide access to SOAP header
                FNCEWS40ServiceWse wseService = InitializeFileNetWebService();

                string lstrSQL = String.Format("SELECT Id,VersionSeries,DocumentTitle,Date,NDShortName,SubjectTitle FROM [Document] WHERE PERSLinkID = {0} AND (DateCreated >={1} and DateCreated <={2})", aintPersonId.ToString(), adtFromDate.ToString("yyyy-MM-dd"), adtToDate.ToString("yyyy-MM-dd"));

                ObjectSetType objObjectSet = ExecuteFileNetQuery(wseService, lstrSQL);

                _lclbWfImageData = LoadFileNetDataByObjectSet(objObjectSet);
            }
            catch (Exception _exc)
            {
                ExceptionManager.Publish(_exc);
            }
            return _lclbWfImageData;
        }
    }
}
