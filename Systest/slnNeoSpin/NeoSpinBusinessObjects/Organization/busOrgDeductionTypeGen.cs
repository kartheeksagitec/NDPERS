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
	public class busOrgDeductionTypeGen : busOrganization
	{
		public busOrgDeductionTypeGen()
		{

		}

		private cdoOrgDeductionType _icdoOrgDeductionType;
		public cdoOrgDeductionType icdoOrgDeductionType
		{
			get
			{
				return _icdoOrgDeductionType;
			}
			set
			{
				_icdoOrgDeductionType = value;
			}
		}

		public bool FindOrgDeductionType(int Aintorgdeductiontypeid)
		{
			bool lblnResult = false;
			if (_icdoOrgDeductionType == null)
			{
				_icdoOrgDeductionType = new cdoOrgDeductionType();
			}
			if (_icdoOrgDeductionType.SelectRow(new object[1] { Aintorgdeductiontypeid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        private busPaymentItemType _ibusPaymentItemType;
        public busPaymentItemType ibusPaymentItemType
        {
            get { return _ibusPaymentItemType; }
            set { _ibusPaymentItemType = value; }
        }

        // To Display the Deduction Item in Org maintenanc - Other Details - Deduction Type grid.
        public void LoadPaymentItemType()
        {
            if (_ibusPaymentItemType == null)
                _ibusPaymentItemType = new busPaymentItemType();
            _ibusPaymentItemType.FindPaymentItemType(_icdoOrgDeductionType.payment_item_type_id);
        }
	}
}
