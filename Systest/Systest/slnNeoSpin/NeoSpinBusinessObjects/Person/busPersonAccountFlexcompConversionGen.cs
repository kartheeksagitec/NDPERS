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
	public class busPersonAccountFlexcompConversionGen : busExtendBase
    {
		public busPersonAccountFlexcompConversionGen()
		{

		}
        
		private cdoPersonAccountFlexcompConversion _icdoPersonAccountFlexcompConversion;
		public cdoPersonAccountFlexcompConversion icdoPersonAccountFlexcompConversion
		{
			get
			{
				return _icdoPersonAccountFlexcompConversion;
			}
			set
			{
				_icdoPersonAccountFlexcompConversion = value;
			}
		}

		public bool FindPersonAccountFlexcompConversion(int Aintpersonaccountflexcompconversionid)
		{
			bool lblnResult = false;
			if (_icdoPersonAccountFlexcompConversion == null)
			{
				_icdoPersonAccountFlexcompConversion = new cdoPersonAccountFlexcompConversion();
			}
			if (_icdoPersonAccountFlexcompConversion.SelectRow(new object[1] { Aintpersonaccountflexcompconversionid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        private busOrganization _ibusProvider;

        public busPerson ibusPerson {get;set;}
        public busPersonEmploymentDetail ibusPersonEmploymentDetail { get; set; }
        public busPersonEmployment ibusPersonEmployment { get; set; }
        public busPersonAccountFlexComp ibusPersonAccountFlexComp { get; set; }

        public busOrganization ibusProvider
        {
            get { return _ibusProvider; }
            set { _ibusProvider = value; }
        }              

        private busPersonAccount _ibusPersonAccount;

        public busPersonAccount ibusPersonAccount
        {
            get { return _ibusPersonAccount; }
            set { _ibusPersonAccount = value; }
        }
        public void LoadPersonAccount()
        {
            if(_ibusPersonAccount==null)
            {
                _ibusPersonAccount = new busPersonAccount();
            }
            _ibusPersonAccount.FindPersonAccount(icdoPersonAccountFlexcompConversion.person_account_id);
        }
        public void LoadProvider()
        {
            if (_ibusProvider == null)
            {
                _ibusProvider = new busOrganization();
            }
            _ibusProvider.FindOrganization(icdoPersonAccountFlexcompConversion.org_id);
        }

        public void LoadPerson()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();

            if (ibusPerson == null)
                ibusPerson = new busPerson();
            ibusPerson.FindPerson(ibusPersonAccount.icdoPersonAccount.person_id);
        }

        public void LoadPersonEmploymentDetail()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();

            if (ibusPersonAccount.icdoPersonAccount.person_employment_dtl_id == 0)
                ibusPersonAccount.icdoPersonAccount.person_employment_dtl_id = ibusPersonAccount.GetEmploymentDetailID();

            if (ibusPersonEmploymentDetail == null)
            {
                ibusPersonEmploymentDetail = new busPersonEmploymentDetail();
            }
            ibusPersonEmploymentDetail.FindPersonEmploymentDetail(ibusPersonAccount.icdoPersonAccount.person_employment_dtl_id);
            
        }

        public void LoadFlexComp()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccountFlexComp == null)
                ibusPersonAccountFlexComp = new busPersonAccountFlexComp();

            ibusPersonAccountFlexComp.FindPersonAccountFlexComp(ibusPersonAccount.icdoPersonAccount.person_account_id);
        }

        }
}
