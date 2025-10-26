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
    public class busMissingContribution : busExtendBase
    {
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

        public void AddMissingContributionEntry(busPersonEmploymentDetail aobjPersonEmploymentDetail)
        {

            busPersonEmploymentDetail lobjPersonEmploymentDetail = new busPersonEmploymentDetail();
            lobjPersonEmploymentDetail.icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail();
            lobjPersonEmploymentDetail.istrMissingMonthYear = aobjPersonEmploymentDetail.istrMissingMonthYear;
            lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.job_class_description = aobjPersonEmploymentDetail.icdoPersonEmploymentDetail.job_class_description;
            lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.seasonal_description = aobjPersonEmploymentDetail.icdoPersonEmploymentDetail.seasonal_description;
            if (aobjPersonEmploymentDetail.ibusPersonEmployment == null)
                aobjPersonEmploymentDetail.LoadPersonEmployment();           
            if (aobjPersonEmploymentDetail.ibusPersonEmployment.ibusPerson == null)
                aobjPersonEmploymentDetail.ibusPersonEmployment.LoadPerson();

            if (aobjPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization == null)
                aobjPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();

            lobjPersonEmploymentDetail.ibusPersonEmployment = aobjPersonEmploymentDetail.ibusPersonEmployment;
            lobjPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization = aobjPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization;
            
            _icolPersonEmploymentDetail.Add(lobjPersonEmploymentDetail);
        }

        public int iintOrgID { get; set; }

        public override busBase GetCorOrganization()
        {
            busOrganization lbusOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
            if (iintOrgID > 0)
            {
                lbusOrganization.FindOrganization(iintOrgID);
            }
            else
            {
                if ((_icolPersonEmploymentDetail != null) && (_icolPersonEmploymentDetail.Count > 0))
                {
                    if (_icolPersonEmploymentDetail[0].ibusPersonEmployment.IsNull()) _icolPersonEmploymentDetail[0].LoadPersonEmployment();
                    if (_icolPersonEmploymentDetail[0].ibusPersonEmployment.ibusOrganization.IsNull()) _icolPersonEmploymentDetail[0].ibusPersonEmployment.LoadOrganization();
                    lbusOrganization = _icolPersonEmploymentDetail[0].ibusPersonEmployment.ibusOrganization;
                }
            }
            return lbusOrganization;
        }
    }
}
