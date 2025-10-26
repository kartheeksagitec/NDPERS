#region Using directives

using System;
using System.Collections;
using System.Data;
using Sagitec.Common;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busProcessInitiation : busExtendBase
    {
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
        public ArrayList InitializeProcess()
        {
            ArrayList larrResult = new ArrayList();
            utlError lobjError = new utlError();

            //Validation           
            if (iintProcessID == 0)
            {
                lobjError = AddError(4129, String.Empty);
                larrResult.Add(lobjError);
                return larrResult;
            }
            //PIR 2207
            //if ((_iintPersonId == 0) && (String.IsNullOrEmpty(_istrOrgCodeId)))
            //{
            //    lobjError = AddError(4130, String.Empty);
            //    larrResult.Add(lobjError);
            //    return larrResult;
            //}

            if ((_iintPersonId != 0) && (!String.IsNullOrEmpty(_istrOrgCodeId)))
            {
                lobjError = AddError(4130, String.Empty);
                larrResult.Add(lobjError);
                return larrResult;
            }

            if (_iintPersonId != 0)
            {
                LoadPerson();
                if (ibusPerson.icdoPerson.person_id == 0)
                {
                    lobjError = AddError(4131, String.Empty);
                    larrResult.Add(lobjError);
                    return larrResult;
                }
            }

            if (!String.IsNullOrEmpty(_istrOrgCodeId))
            {
                LoadOrganization();
                if (ibusOrganization.icdoOrganization.org_id == 0)
                {
                    lobjError = AddError(4132, String.Empty);
                    larrResult.Add(lobjError);
                    return larrResult;
                }
            }

            busProcess lbusProcess = new busProcess();
            lbusProcess.FindProcess(iintProcessID);

            if (lbusProcess.icdoProcess.process_id > 0)
            {
                if (!String.IsNullOrEmpty(_istrOrgCodeId))
                {
                    if (lbusProcess.icdoProcess.type_value == busConstant.ProcessType_Person)
                    {
                        lobjError = AddError(4134, String.Empty);
                        larrResult.Add(lobjError);
                        return larrResult;

                    }
                }
                else if ((_iintPersonId != 0))
                {
                    if (lbusProcess.icdoProcess.type_value == busConstant.ProcessType_Org)
                    {
                        lobjError = AddError(4135, String.Empty);
                        larrResult.Add(lobjError);
                        return larrResult;
                    }
                }

                if (lbusProcess.icdoProcess.type_value == busConstant.ProcessType_Person)
                {
                    if (_iintPersonId == 0)
                    {
                        lobjError = AddError(176, String.Empty);
                        larrResult.Add(lobjError);
                        return larrResult;
                    }
                }
                else if (lbusProcess.icdoProcess.type_value == busConstant.ProcessType_Org)
                {
                    if (_istrOrgCodeId.IsNullOrEmpty())
                    {
                        lobjError = AddError(1032, String.Empty);
                        larrResult.Add(lobjError);
                        return larrResult;
                    }
                }
            }

            if (_iintPersonId != 0)
            {
                DataTable ldtblistPerson = Select("cdoActivityInstance.LoadRunningInstancesByPersonAndProcess", new object[2] { _iintPersonId, iintProcessID });
                if (ldtblistPerson.Rows.Count > 0)
                {
                    lobjError = AddError(4133, String.Empty);
                    larrResult.Add(lobjError);
                    return larrResult;
                }
            }

            //Initialize the Workflow
            cdoWorkflowRequest lcdoWorkflowRequest = new cdoWorkflowRequest();
            lcdoWorkflowRequest.process_id = iintProcessID;
            lcdoWorkflowRequest.person_id = _iintPersonId;
            lcdoWorkflowRequest.org_code = _istrOrgCodeId;
            lcdoWorkflowRequest.status_value = busConstant.WorkflowProcessStatus_UnProcessed;
            lcdoWorkflowRequest.source_value = busConstant.WorkflowProcessSource_Online;
            lcdoWorkflowRequest.Insert();
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
