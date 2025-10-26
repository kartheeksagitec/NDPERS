#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using Sagitec.CustomDataObjects;
using NeoSpin.CustomDataObjects;
using System.IO;

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busCorTracking : busExtendBase
    {
        public busCorTracking()
        {

        }

        public string short_description
        {
            get
            {
                string lstrDescription = "";
                if (_ibusCorTemplates != null)
                {
                    lstrDescription = _ibusCorTemplates.icdoCorTemplates.template_desc;
                }
                else
                {
                    lstrDescription = "Invalid Correspondence";
                }
                return lstrDescription;
            }
        }

        public string long_description
        {
            get
            {
                string lstrDescription = "";
                if (_ibusCorTemplates != null)
                {
                    lstrDescription = _ibusCorTemplates.icdoCorTemplates.template_desc +
                        " is in " + icdoCorTracking.cor_status_description + " status and generated " +
                        " on " + HelperFunction.FormatData(icdoCorTracking.generated_date.ToString(), "{0:d}");
                }
                else
                {
                    lstrDescription = "Invalid Correspondence";
                }
                return lstrDescription;
            }
        }
        private cdoCorTracking _icdoCorTracking;
        public cdoCorTracking icdoCorTracking
        {
            get
            {
                return _icdoCorTracking;
            }

            set
            {
                _icdoCorTracking = value;
            }
        }

        private busOrgContact _ibusOrgContact;
        public busOrgContact ibusOrgContact
        {
            get
            {
                return _ibusOrgContact;
            }

            set
            {
                _ibusOrgContact = value;
            }
        }

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

        private busCorTemplates _ibusCorTemplates;
        public busCorTemplates ibusCorTemplates
        {
            get
            {
                return _ibusCorTemplates;
            }

            set
            {
                _ibusCorTemplates = value;
            }
        }
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

        public busPlan ibusPlan { get; set; }

        public void LoadCorTemplates()
        {
            if (_ibusCorTemplates == null)
            {
                _ibusCorTemplates = new busCorTemplates();
            }
            _ibusCorTemplates.FindCorTemplates(_icdoCorTracking.template_id);
        }


        public void LoadOtherCorTemplates(int lintid)
        {
            if (_ibusCorTemplates == null)
            {
                _ibusCorTemplates = new busCorTemplates();
            }
            _ibusCorTemplates.FindCorTemplates(lintid);
        }

        public void LoadOrganization()
        {
            if (_ibusOrganization == null)
            {
                _ibusOrganization = new busOrganization();
            }
            _ibusOrganization.FindOrganization(_icdoCorTracking.org_id);
        }

        public void LoadOrganizationContact()
        {
            if (_ibusOrgContact == null)
            {
                _ibusOrgContact = new busOrgContact();
            }
            _ibusOrgContact.FindOrgContact(_icdoCorTracking.org_contact_id);
        }

        public void LoadPerson()
        {
            if (_ibusPerson == null)
            {
                _ibusPerson = new busPerson();
            }
            _ibusPerson.FindPerson(_icdoCorTracking.person_id);
        }

        public bool FindCorTracking(int ainttrackingid)
        {

            bool lblnResult = false;
            if (_icdoCorTracking == null)
            {
                _icdoCorTracking = new cdoCorTracking();
            }
            if (_icdoCorTracking.SelectRow(new object[1] { ainttrackingid }))
            {
                lblnResult = true;
            }
            return lblnResult;

        }

        public void LoadPlan()
        {
            if (ibusPlan == null)
            {
                ibusPlan = new busPlan();
            }
            ibusPlan.FindPlan(_icdoCorTracking.plan_id);
        }

        public string istrFileName
        {
            get
            {
                return icdoCorTracking.tracking_id.ToString().PadLeft(10, '0');
            }
        }

        public string istrWordFileName
        {
            get
            {
               
                return ibusCorTemplates.icdoCorTemplates.template_name + "-" + istrFileName + ".docx";
            }
        }
        //OpenXML Conversion Code
        public string istrWordFileNameDoc
        {
            get
            {

                return ibusCorTemplates.icdoCorTemplates.template_name + "-" + istrFileName + ".doc";
            }
        }

        public string istrTifFileName
        {
            get
            {
                return ibusCorTemplates.icdoCorTemplates.template_name + "-" + istrFileName + ".tif";
            }
        }

        public void RemoveFile()
        {
            //OpenXML COnversion Code
            string lstrFileName = iobjPassInfo.isrvDBCache.GetPathInfo("CorrGenr") + istrWordFileName;

            if (File.Exists(lstrFileName))
            {
                lstrFileName = iobjPassInfo.isrvDBCache.GetPathInfo("CorrGenr") +  istrWordFileNameDoc;
                
            }
           
            File.Delete(lstrFileName);
        }
        public bool FileExists(string Path)
        {
         
                if (File.Exists(@Path))
                    return true;
                else
                    return false;
        }

    }
}
