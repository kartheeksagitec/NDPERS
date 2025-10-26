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
    public class busRetroItemTypeGen : busExtendBase
    {
        public busRetroItemTypeGen()
        {

        }

        private cdoRetroItemType _icdoRetroItemType;
        public cdoRetroItemType icdoRetroItemType
        {
            get
            {
                return _icdoRetroItemType;
            }
            set
            {
                _icdoRetroItemType = value;
            }
        }

        public bool FindRetroItemType(int Aintretroitemtypeid)
        {
            bool lblnResult = false;
            if (_icdoRetroItemType == null)
            {
                _icdoRetroItemType = new cdoRetroItemType();
            }
            if (_icdoRetroItemType.SelectRow(new object[1] { Aintretroitemtypeid }))
            {
                lblnResult = true;
            }
            return lblnResult;
        }
        //prop  to load from item code info
        public busPaymentItemType ibusFromPaymentItemType { get; set; }
        //load from item code info
        public void LoadFromItemCodeInfo()
        {
            if (ibusFromPaymentItemType == null)
                ibusFromPaymentItemType = new busPaymentItemType();
            ibusFromPaymentItemType.FindPaymentItemTypeByItemCode(icdoRetroItemType.from_item_type);
        }
        //prop  to load from item code info
        public busPaymentItemType ibusToPaymentItemType { get; set; }
        //load from item code info
        public void LoadToItemCodeInfo()
        {
            if (ibusToPaymentItemType == null)
                ibusToPaymentItemType = new busPaymentItemType();
            ibusToPaymentItemType.FindPaymentItemTypeByItemCode(icdoRetroItemType.to_item_type);
        }
    }
}