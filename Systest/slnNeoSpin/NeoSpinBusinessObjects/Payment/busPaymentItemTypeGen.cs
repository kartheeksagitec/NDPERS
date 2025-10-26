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
	public class busPaymentItemTypeGen : busExtendBase
    {
		public busPaymentItemTypeGen()
		{

		}

		private cdoPaymentItemType _icdoPaymentItemType;
		public cdoPaymentItemType icdoPaymentItemType
		{
			get
			{
				return _icdoPaymentItemType;
			}
			set
			{
				_icdoPaymentItemType = value;
			}
		}

		public bool FindPaymentItemType(int Aintpaymentitemtypeid)
		{
			bool lblnResult = false;
			if (_icdoPaymentItemType == null)
			{
				_icdoPaymentItemType = new cdoPaymentItemType();
			}
			if (_icdoPaymentItemType.SelectRow(new object[1] { Aintpaymentitemtypeid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
        public bool FindPaymentItemTypeByItemCode(string AintPaymentItemTypeCode)
        {
            bool lblnResult = false;
            DataTable ldtbList = Select<cdoPaymentItemType>(
                new string[1] { "item_type_code" },
                new object[1] { AintPaymentItemTypeCode }, null, null);
            if (_icdoPaymentItemType == null)
                _icdoPaymentItemType = new  cdoPaymentItemType();
            if (ldtbList.Rows.Count == 1)
            {
                _icdoPaymentItemType.LoadData(ldtbList.Rows[0]);
                lblnResult = true;
            }
            return lblnResult;
        }
        private Collection<busOrgDeductionType> _iclbOrgDeductionType;
        public Collection<busOrgDeductionType> iclbOrgDeductionType
        {
            get { return _iclbOrgDeductionType; }
            set { _iclbOrgDeductionType = value; }
        }
	}
}
