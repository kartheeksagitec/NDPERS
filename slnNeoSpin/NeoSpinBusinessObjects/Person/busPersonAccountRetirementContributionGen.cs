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
    public class busPersonAccountRetirementContributionGen : busExtendBase
    {
        public busPersonAccountRetirementContributionGen()
        {

        }
        private busPersonAccountRetirement _ibusPARetirement;
        public busPersonAccountRetirement ibusPARetirement
        {
            get { return _ibusPARetirement; }
            set { _ibusPARetirement = value; }
        }
        private bool _ibnlIsProjectedSalaryRecord;

        public bool ibnlIsProjectedSalaryRecord
        {
            get { return _ibnlIsProjectedSalaryRecord; }
            set { _ibnlIsProjectedSalaryRecord = value; }
        }

        private string _istrMonthName;
        public string istrMonthName
        {
            get { return _istrMonthName; }
            set { _istrMonthName = value; }
        }

        private busPersonEmploymentDetail _ibusPersonEmploymentDetail;
        public busPersonEmploymentDetail ibusPersonEmploymentDetail
        {
            get { return _ibusPersonEmploymentDetail; }
            set { _ibusPersonEmploymentDetail = value; }
        }
     
        private cdoPersonAccountRetirementContribution _icdoPersonAccountRetirementContribution;
        public cdoPersonAccountRetirementContribution icdoPersonAccountRetirementContribution
        {
            get
            {
                return _icdoPersonAccountRetirementContribution;
            }
            set
            {
                _icdoPersonAccountRetirementContribution = value;
            }
        }

        public bool FindPersonAccountRetirementContribution(int Aintretirementcontributionid)
        {
            bool lblnResult = false;
            if (_icdoPersonAccountRetirementContribution == null)
            {
                _icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution();
            }
            if (_icdoPersonAccountRetirementContribution.SelectRow(new object[1] { Aintretirementcontributionid }))
            {
                lblnResult = true;
            }
            return lblnResult;
        }

        public void LoadPersonAccountRetirement()
        {
            if (_ibusPARetirement == null)
            {
                _ibusPARetirement = new busPersonAccountRetirement();
            }
            _ibusPARetirement.FindPersonAccountRetirement(icdoPersonAccountRetirementContribution.person_account_id);
        }

        public void LoadPersonEmploymentDetail()
        {
            if (_ibusPersonEmploymentDetail == null)
            {
                _ibusPersonEmploymentDetail = new busPersonEmploymentDetail();
            }            
            _ibusPersonEmploymentDetail.FindPersonEmploymentDetail(icdoPersonAccountRetirementContribution.person_employment_dtl_id);
            //Since Member Type Field is removed, now Member type will be loaded from this method. this needs to loaded all the time when emp. detail object loaded.
            _ibusPersonEmploymentDetail.LoadMemberType();
        }

        private decimal _idecTotalSalaryAmount;
        public decimal idecTotalSalaryAmount
        {
            get { return _idecTotalSalaryAmount; }
            set { _idecTotalSalaryAmount = value; }
        }

        private Collection<busPersonAccountRetirementContribution> _iclbJobServiceContributions;
        public Collection<busPersonAccountRetirementContribution> iclbJobServiceContributions
        {
            get { return _iclbJobServiceContributions; }
            set { _iclbJobServiceContributions = value; }
        }        
    }
}