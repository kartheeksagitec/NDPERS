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
	public class busBenefitCalculationPayeeGen : busExtendBase
    {
		public busBenefitCalculationPayeeGen()
		{

		}

		private cdoBenefitCalculationPayee _icdoBenefitCalculationPayee;
		public cdoBenefitCalculationPayee icdoBenefitCalculationPayee
		{
			get
			{
				return _icdoBenefitCalculationPayee;
			}
			set
			{
				_icdoBenefitCalculationPayee = value;
			}
		}

		private busPerson _ibusPayee;
        public busPerson ibusPayee
		{
			get
			{
                return _ibusPayee;
			}
			set
			{
                _ibusPayee = value;
			}
		}

		private busBenefitCalculation _ibusBenefitCalculation;
		public busBenefitCalculation ibusBenefitCalculation
		{
			get
			{
				return _ibusBenefitCalculation;
			}
			set
			{
				_ibusBenefitCalculation = value;
			}
		}

        private busBenefitApplication _ibusBenefitApplication;
        public busBenefitApplication ibusBenefitApplication
        {
            get { return _ibusBenefitApplication; }
            set { _ibusBenefitApplication = value; }
        }

		public bool FindBenefitCalculationPayee(int Aintbenefitcalculationpayeeid)
		{
			bool lblnResult = false;
			if (_icdoBenefitCalculationPayee == null)
			{
				_icdoBenefitCalculationPayee = new cdoBenefitCalculationPayee();
			}
			if (_icdoBenefitCalculationPayee.SelectRow(new object[1] { Aintbenefitcalculationpayeeid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

		public void LoadPayee()
		{
            if (_ibusBenefitCalculation == null)
                LoadBenefitCalculation();
			if (_ibusPayee == null)
                _ibusPayee = new busPerson();
            _ibusPayee.FindPerson(icdoBenefitCalculationPayee.payee_person_id);
		}

		public void LoadBenefitCalculation()
		{
			if (_ibusBenefitCalculation == null)
			{
				_ibusBenefitCalculation = new busBenefitCalculation();
			}
            _ibusBenefitCalculation.FindBenefitCalculation(_icdoBenefitCalculationPayee.benefit_calculation_id);
		}

        public void LoadMember()
        {
            if (_ibusBenefitCalculation == null)
                LoadBenefitCalculation();
            if (_ibusBenefitCalculation.ibusMember == null)
                _ibusBenefitCalculation.LoadMember();
        }

        private bool _iblnIsDeceased;
        public bool iblnIsDeceased
        {
            get { return _iblnIsDeceased; }
            set { _iblnIsDeceased = value; }
        }

        private string  _istrPayeeOrgCode;
        public string istrPayeeOrgCode
        {
            get { return _istrPayeeOrgCode; }
            set { _istrPayeeOrgCode = value; }
        }
        
        private busOrganization _ibusPayeeOrg;
        public busOrganization ibusPayeeOrg
        {
            get { return _ibusPayeeOrg; }
            set { _ibusPayeeOrg = value; }
        }

        public void LoadPayeeOrg()
        {
            if (ibusPayeeOrg == null)
                ibusPayeeOrg = new busOrganization();
            ibusPayeeOrg.FindOrganization(icdoBenefitCalculationPayee.payee_org_id);
        }

        public busBenefitProvisionBenefitOption ibusBenefitProvisionBenefitOption { get; set; }

        public void LoadBenefitProvisionBenefitOption()
        {
            if (ibusBenefitProvisionBenefitOption == null)
                ibusBenefitProvisionBenefitOption = new busBenefitProvisionBenefitOption();
            ibusBenefitProvisionBenefitOption.FindBenefitProvisionBenefitOption(icdoBenefitCalculationPayee.payee_benefit_provision_benefit_option_id);
        }
	}
}
