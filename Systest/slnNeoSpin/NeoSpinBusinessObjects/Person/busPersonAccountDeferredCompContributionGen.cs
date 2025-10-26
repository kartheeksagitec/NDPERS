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
	public class busPersonAccountDeferredCompContributionGen : busExtendBase
    {
        public busPersonAccountDeferredCompContributionGen()
		{

		}
        private busPersonAccountDeferredComp _ibusPADeferredComp;

        public busPersonAccountDeferredComp ibusPADeferredComp
        {
            get { return _ibusPADeferredComp; }
            set { _ibusPADeferredComp = value; }
        }

        public void LoadPersonAccountDeferredComp()
        {
            if (_ibusPADeferredComp == null)
            {
                _ibusPADeferredComp = new busPersonAccountDeferredComp();
            }
            _ibusPADeferredComp.FindPersonAccountDeferredComp(icdoPersonAccountDeferredCompContribution.person_account_id);
        }
      
        private busOrganization _ibusProvider;
        public busOrganization ibusProvider
        {
            get { return _ibusProvider; }
            set { _ibusProvider = value; }
        }
        public void LoadProvider()
        {
            if (_ibusProvider == null)
            {
                _ibusProvider = new busOrganization();
            }
            _ibusProvider.FindOrganization(icdoPersonAccountDeferredCompContribution.provider_org_id);
        }

		private cdoPersonAccountDeferredCompContribution _icdoPersonAccountDeferredCompContribution;
		public cdoPersonAccountDeferredCompContribution icdoPersonAccountDeferredCompContribution
		{
			get
			{
				return _icdoPersonAccountDeferredCompContribution;
			}
			set
			{
				_icdoPersonAccountDeferredCompContribution = value;
			}
		}
       
		public bool FindPersonAccountDeffCompContribution(int Aintdeferredcompcontributionid)
		{
			bool lblnResult = false;
			if (_icdoPersonAccountDeferredCompContribution == null)
			{
				_icdoPersonAccountDeferredCompContribution = new cdoPersonAccountDeferredCompContribution();
			}
			if (_icdoPersonAccountDeferredCompContribution.SelectRow(new object[1] { Aintdeferredcompcontributionid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        private busPersonEmploymentDetail _ibusPersonEmploymentDetail;
        public busPersonEmploymentDetail ibusPersonEmploymentDetail
        {
            get { return _ibusPersonEmploymentDetail; }
            set { _ibusPersonEmploymentDetail = value; }
        }

        public void LoadPersonEmploymentDetail()
        {
            if (_ibusPersonEmploymentDetail == null)
            {
                _ibusPersonEmploymentDetail = new busPersonEmploymentDetail();
            }
            _ibusPersonEmploymentDetail.FindPersonEmploymentDetail(icdoPersonAccountDeferredCompContribution.person_employment_dtl_id);
            _ibusPersonEmploymentDetail.LoadMemberType();
        }
	}
}
