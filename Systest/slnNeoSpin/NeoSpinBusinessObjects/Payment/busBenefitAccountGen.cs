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
	public class busBenefitAccountGen : busExtendBase
    {
		public busBenefitAccountGen()
		{

		}

		private cdoBenefitAccount _icdoBenefitAccount;
		public cdoBenefitAccount icdoBenefitAccount
		{
			get
			{
				return _icdoBenefitAccount;
			}
			set
			{
				_icdoBenefitAccount = value;
			}
		}

		public bool FindBenefitAccount(int Aintbenefitaccountid)
		{
			bool lblnResult = false;
			if (_icdoBenefitAccount == null)
			{
				_icdoBenefitAccount = new cdoBenefitAccount();
			}
			if (_icdoBenefitAccount.SelectRow(new object[1] { Aintbenefitaccountid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
        private busOrganization _ibusRetirementOrg;

        public busOrganization ibusRetirementOrg
        {
            get { return _ibusRetirementOrg; }
            set { _ibusRetirementOrg = value; }
        }
        public void LoadRetirementOrg()
        {
            if (ibusRetirementOrg == null)
            {
                ibusRetirementOrg = new busOrganization();
            }
            ibusRetirementOrg.FindOrganization(icdoBenefitAccount.retirement_org_id);
        }
     

        //properties used in correspondence

        public string istrPayee1 { get; set; }
        public string istrPayee2 { get; set; }
        public string istrPayee3 { get; set; }
        public string istrPayee4 { get; set; }
        public string istrPayee5 { get; set; }

        public string istrPayeeAddress1 { get; set; }
        public string istrPayeeAddress2 { get; set; }
        public string istrPayeeAddress3{ get; set; }
        public string istrPayeeAddress4 { get; set; }
        public string istrPayeeAddress5 { get; set; }

        public string istrPayee2exists { get; set; }
        public string istrPayee3exists { get; set; }
        public string istrPayee4exists { get; set; }
        public string istrPayee5exists { get; set; }

        public string istrBeneficiaryPercentage1 { get; set; }
        public string istrBeneficiaryPercentage2 { get; set; }
        public string istrBeneficiaryPercentage3 { get; set; }
        public string istrBeneficiaryPercentage4 { get; set; }
        public string istrBeneficiaryPercentage5 { get; set; }

        public decimal idecBeneficiaryPercentage1 { get; set; }
        public decimal idecBeneficiaryPercentage2 { get; set; }
        public decimal idecBeneficiaryPercentage3 { get; set; }
        public decimal idecBeneficiaryPercentage4 { get; set; }
        public decimal idecBeneficiaryPercentage5 { get; set; }

        public decimal idecValueAtDateOfDeath { get; set; }

        public string istrPaymentDate { get; set; }
	}
}
