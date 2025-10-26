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

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busPersonEmploymentGen : busPersonBase
    {
        public busPersonEmploymentGen()
        {

        }

        private cdoPersonEmployment _icdoPersonEmployment;
        public cdoPersonEmployment icdoPersonEmployment
        {
            get
            {
                return _icdoPersonEmployment;
            }
            set
            {
                _icdoPersonEmployment = value;
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
        private Collection<busPersonEmploymentDetail> _icolPersonEmploymentDetail;
        public Collection<busPersonEmploymentDetail> icolPersonEmploymentDetail
        {
            get
            {
                return _icolPersonEmploymentDetail;
            }

            set
            {
                _icolPersonEmploymentDetail = value;
            }
        }
        public bool FindPersonEmployment(int AintPersonEmploymentID)
        {
            bool lblnResult = false;
            if (_icdoPersonEmployment == null)
            {
                _icdoPersonEmployment = new cdoPersonEmployment();
            }
            if (_icdoPersonEmployment.SelectRow(new object[1] { AintPersonEmploymentID }))
            {
                lblnResult = true;
            }
            return lblnResult;
        }

        public void LoadPerson()
        {
            if (_ibusPerson == null)
            {
                _ibusPerson = new busPerson();
            }
            //	_ibusPerson.ibusPersonEmployment = this;
            _ibusPerson.FindPerson(_icdoPersonEmployment.person_id);
        }


        public void LoadOrganization()
        {
            if (_ibusOrganization == null)
            {
                _ibusOrganization = new busOrganization();
            }
            _ibusOrganization.FindOrganization(_icdoPersonEmployment.org_id);
        }
        public void LoadPersonEmploymentDetail()
        {
            LoadPersonEmploymentDetail(true);
        }

        public void LoadPersonEmploymentDetail(bool ablnLoadOtherObjects)
        {
            //DO NOT CHANGE THIS SORTING ORDER.. BUSINESS LOGIC INVOLVED BASED ON THIS SORTING..
            DataTable ldtbList = Select<cdoPersonEmploymentDetail>(
                new string[1] { "person_employment_id" },
                new object[1] { _icdoPersonEmployment.person_employment_id }, null, "case when end_date is null then 0 else 1 end, start_date DESC, end_date DESC");
            _icolPersonEmploymentDetail = GetCollection<busPersonEmploymentDetail>(ldtbList, "icdoPersonEmploymentDetail");
            if (ablnLoadOtherObjects)
            {
                foreach (busPersonEmploymentDetail lobjPersonEmploymentDetail in _icolPersonEmploymentDetail)
                {
                    lobjPersonEmploymentDetail.LoadMemberType();
                    lobjPersonEmploymentDetail.LoadPersonEmployment();
                    lobjPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
                    lobjPersonEmploymentDetail.LoadEnrollmentRequest();
                }
            }
        }
        //default date for def comp and flex
        public DateTime idtmSuspendEffectiveDateDCandFlex { get; set; }
        //default date for HDV Life and EAP
        public DateTime idtmSuspendEffectiveDateDefaultInsPlans { get; set; }
        public DateTime idtmDefCompProviderEndDateWhenTEEM { get; set; }
        public DateTime idtmProviderEndDateFromDeathNotif { get; set; }
    }
}
