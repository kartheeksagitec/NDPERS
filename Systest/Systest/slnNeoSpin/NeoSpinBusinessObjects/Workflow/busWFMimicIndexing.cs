#region Using directives

using System;
using System.Collections;
using Sagitec.Common;
using Sagitec.BusinessObjects;
using Sagitec.CustomDataObjects;

using NeoSpin.CustomDataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
	public partial class busWFMimicIndexing : busExtendBase
    {
        #region Properties
        private int _iintPersonId;
        public int iintPersonId
        {
            get { return _iintPersonId; }
            set { _iintPersonId = value; }
        }

        private string _istrOrgCodeId;
        public string istrOrgCodeId
        {
            get { return _istrOrgCodeId; }
            set { _istrOrgCodeId = value; }
        }

        private int _iintProcessID;
        public int iintProcessID
        {
            get { return _iintProcessID; }
            set { _iintProcessID = value; }
        }

        private string _istrDocumentCode;
        public string istrDocumentCode
        {
            get { return _istrDocumentCode; }
            set { _istrDocumentCode = value; }
        }

        private string _istrFilenetDocType;
        public string istrFilenetDocType
        {
            get { return _istrFilenetDocType; }
            set { _istrFilenetDocType = value; }
        }

        private string _istrImageDocCategory;
        public string istrImageDocCategory
        {
            get { return _istrImageDocCategory; }
            set { _istrImageDocCategory = value; }
        }

        private string _istrImageFileName;
        public string istrImageFileName
        {
            get { return _istrImageFileName; }
            set { _istrImageFileName = value; }
        }

        //property is used to load Organization Details
        private busOrganization _ibusOrganization;
        public busOrganization ibusOrganization
        {
            get
            {
                return _ibusOrganization;
            }

            set
            {
                _ibusOrganization = value;
            }
        }

        //property is used to load Person Details
        private busPerson _ibusPerson;
        public busPerson ibusPerson
        {
            get
            {
                return _ibusPerson;
            }

            set
            {
                _ibusPerson = value;
            }
        }

        #endregion

        #region public functions
        public ArrayList ProcessMimicIndexing()
        {
            ArrayList larrResult = new ArrayList();
            utlError lobjError = new utlError();
            string lstrDocCategory = String.Empty;
            string lstrDocType = String.Empty;

            ValidateData(larrResult, lobjError);           

            if (larrResult.Count > 0)
                return larrResult;

            cdoCodeValue lcdoFolderName = busGlobalFunctions.GetCodeValueByDescription(603, _istrImageDocCategory);
            cdoCodeValue lcdoFileNetDocumentType = GetCodeValue(604, _istrFilenetDocType);
            lstrDocType = lcdoFileNetDocumentType.description;

            lstrDocCategory = lcdoFolderName.description;
            
            // Push the image to FileNet            
            PushImageToFileNet(lcdoFolderName, lcdoFileNetDocumentType, larrResult, lobjError);
        
            if (larrResult.Count > 0)
                return larrResult;

            // Add the data to Image data from FileNet to intiate mimic indexing
            AddImageDataToDatabase(lstrDocCategory, larrResult, lobjError, lstrDocType);
            
            larrResult.Add(this);
            return larrResult;
        }


        public void LoadPerson()
        {
            if (_ibusPerson == null)
            {
                _ibusPerson = new busPerson();
            }
            _ibusPerson.FindPerson(_iintPersonId);
        }

        public void LoadOrganization()
        {
            if (_ibusOrganization == null)
            {
                _ibusOrganization = new busOrganization();
            }
            _ibusOrganization.FindOrganizationByOrgCode(_istrOrgCodeId);
        }
    #endregion

        #region Private Functions
        private void ValidateData(ArrayList larrResult, utlError lobjError)
        {
            busPerson lobjPerson = new busPerson();
            busOrganization lobjOrganization = new busOrganization();

            //Validation           
            if ((_iintPersonId == 0) && (String.IsNullOrEmpty(_istrOrgCodeId)))
            {
                lobjError = AddError(4130, String.Empty);
                larrResult.Add(lobjError);
            }
            else if ((_iintPersonId != 0) && (!String.IsNullOrEmpty(_istrOrgCodeId)))
            {
                lobjError = AddError(4130, String.Empty);
                larrResult.Add(lobjError);
            }

            if ((_iintPersonId != 0) && (!(lobjPerson.FindPerson(_iintPersonId))))
            {
                lobjError = AddError(4131, String.Empty);
                larrResult.Add(lobjError);
            }
            if ((!String.IsNullOrEmpty(_istrOrgCodeId)) && (!(lobjOrganization.FindOrganizationByOrgCode(_istrOrgCodeId))))
            {
                lobjError = AddError(4132, String.Empty);
                larrResult.Add(lobjError);
            }

            if (_iintPersonId != 0)
            {
                LoadPerson();
            }

            if (!String.IsNullOrEmpty(_istrOrgCodeId))
            {
                LoadOrganization();
            }
            if (_iintPersonId != 0)
            {
                LoadPerson();
            }
            if (String.IsNullOrEmpty(_istrDocumentCode)) 
            {
                lobjError = AddError(611, String.Empty);
                larrResult.Add(lobjError);
            }           

            if (String.IsNullOrEmpty(_istrFilenetDocType))
            {
                lobjError = AddError(612, String.Empty);
                larrResult.Add(lobjError);
            }

            if (String.IsNullOrEmpty(_istrImageDocCategory))
            {
                lobjError = AddError(613, String.Empty);
                larrResult.Add(lobjError);
            }

            if (String.IsNullOrEmpty(_istrImageFileName))
            {
                lobjError = AddError(610, String.Empty);
                larrResult.Add(lobjError);
            }
        }

        private void PushImageToFileNet(cdoCodeValue lcdoFolderName, cdoCodeValue lcdoFileNetDocumentType, ArrayList larrResult, utlError lobjError)
        {
            string lstrMimicImagePath = utlPassInfo.iobjPassInfo.isrvDBCache.GetPathInfo("MimicImage");
            string lstrImageFileName = lstrMimicImagePath + _istrImageFileName;
            
            try
            {
                ArrayList larrError = busFileNetHelper.UploadFileNetDocument(lstrImageFileName, _iintPersonId, _istrOrgCodeId, _istrImageFileName, _istrImageFileName,
                    lcdoFolderName.description, lcdoFileNetDocumentType.description, _istrDocumentCode, _istrImageFileName);
                if (larrError.Count > 0)
                {
                    throw new Exception(((utlError)larrError[0]).istrErrorMessage);
                }
                larrError = busFileNetHelper.ProcessFileNetDocument(lstrImageFileName, lcdoFolderName.description, _istrImageFileName);
                if (larrError.Count > 0)
                {
                    throw new Exception(((utlError)larrError[0]).istrErrorMessage);
                }
            }
            catch (Exception lexc)
            {
                lobjError = AddError(0, lexc.Message);
                larrResult.Add(lobjError);
            }
        }

        private void AddImageDataToDatabase(string lstrDocCategory, ArrayList larrResult, utlError lobjError, string lstrFilenetDocType)
        {
            try
            {
                if (icdoWorkflowRequest == null)
                    icdoWorkflowRequest = new cdoWorkflowRequest();
                else
                    icdoWorkflowRequest.Reset();
                icdoWorkflowRequest.process_id = iintProcessID;
                icdoWorkflowRequest.person_id = _iintPersonId;
                icdoWorkflowRequest.org_code = _istrOrgCodeId;
                icdoWorkflowRequest.document_code = _istrDocumentCode;
                icdoWorkflowRequest.filenet_document_type = lstrFilenetDocType;
                icdoWorkflowRequest.image_doc_category = lstrDocCategory; //_istrImageDocCategory;
                icdoWorkflowRequest.initiated_date = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                icdoWorkflowRequest.status_value = busConstant.WorkflowProcessStatus_UnProcessed;
                icdoWorkflowRequest.source_value = busConstant.WorkflowProcessSource_Indexing;
                icdoWorkflowRequest.Insert();
            }
            catch (Exception lexc)
            {
                lobjError = AddError(0, lexc.Message);
                larrResult.Add(lobjError);
            }
        }
        #endregion

        //This method called when we click Hand Icon. That will reload the object and display the details
        public ArrayList ReloadHandButtonData()
        {
            ArrayList larrList = new ArrayList();
            if (_iintPersonId != 0)
            {
                LoadPerson();
            }
            if (!String.IsNullOrEmpty(_istrOrgCodeId))
            {
                LoadOrganization();
            }
            larrList.Add(this);
            return larrList;
        }
    }
}
