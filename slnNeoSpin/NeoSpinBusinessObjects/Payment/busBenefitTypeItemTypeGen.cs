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
	public class busBenefitTypeItemTypeGen : busExtendBase
    {
		public busBenefitTypeItemTypeGen()
		{

		}

		private cdoBenefitTypeItemType _icdoBenefitTypeItemType;
		public cdoBenefitTypeItemType icdoBenefitTypeItemType
		{
			get
			{
				return _icdoBenefitTypeItemType;
			}
			set
			{
				_icdoBenefitTypeItemType = value;
			}
		}

		public bool FindBenefitTypeItemType(int Aintbenefittypeitemtypeid)
		{
			bool lblnResult = false;
			if (_icdoBenefitTypeItemType == null)
			{
				_icdoBenefitTypeItemType = new cdoBenefitTypeItemType();
			}
			if (_icdoBenefitTypeItemType.SelectRow(new object[1] { Aintbenefittypeitemtypeid }))
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
        public void LoadPaymentItemType()
        {
            if (ibusPaymentItemType == null)
            {
                ibusPaymentItemType = new busPaymentItemType();
            }
            ibusPaymentItemType.FindPaymentItemType(icdoBenefitTypeItemType.payment_item_type_id);
        }
	}
}
