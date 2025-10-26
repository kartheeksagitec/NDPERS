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
    public class busPayeeAccountRetroPaymentDetailGen : busExtendBase
    {
        public busPayeeAccountRetroPaymentDetailGen()
        {

        }

        private cdoPayeeAccountRetroPaymentDetail _icdoPayeeAccountRetroPaymentDetail;
        public cdoPayeeAccountRetroPaymentDetail icdoPayeeAccountRetroPaymentDetail
        {
            get
            {
                return _icdoPayeeAccountRetroPaymentDetail;
            }
            set
            {
                _icdoPayeeAccountRetroPaymentDetail = value;
            }
        }

        public bool FindPayeeAccountRetroPaymentDetail(int AintRetroPaymentDetailId)
        {
            bool lblnResult = false;
            if (_icdoPayeeAccountRetroPaymentDetail == null)
            {
                _icdoPayeeAccountRetroPaymentDetail = new cdoPayeeAccountRetroPaymentDetail();
            }
            if (_icdoPayeeAccountRetroPaymentDetail.SelectRow(new object[1] { AintRetroPaymentDetailId }))
            {
                lblnResult = true;
            }
            return lblnResult;
        }
        private busPayeeAccountRetroPayment _ibusRetroPayment;

        public busPayeeAccountRetroPayment ibusRetroPayment
        {
            get { return _ibusRetroPayment; }
            set { _ibusRetroPayment = value; }
        }
	        
        private busPaymentItemType _ibusPaymentItemType;

        public busPaymentItemType ibusPaymentItemType
        {
            get { return _ibusPaymentItemType; }
            set { _ibusPaymentItemType = value; }
        }
        public void LoadPaymentItemType()
        {
            if (_ibusPaymentItemType == null)
                _ibusPaymentItemType = new busPaymentItemType();
            _ibusPaymentItemType.FindPaymentItemType(icdoPayeeAccountRetroPaymentDetail.payment_item_type_id);
        }
        public busPaymentItemType ibusOriginalPaymentItemType { get; set; }
        public void LoadOriginalPaymentItemType()
        {
            if (ibusOriginalPaymentItemType == null)
                ibusOriginalPaymentItemType = new busPaymentItemType();
            ibusOriginalPaymentItemType.FindPaymentItemType(icdoPayeeAccountRetroPaymentDetail.original_payment_item_type_id);
        }
        public void LoadRetroPayment()
        {
            if (ibusRetroPayment == null)
            {
                ibusRetroPayment = new busPayeeAccountRetroPayment();
            }
            ibusRetroPayment.FindPayeeAccountRetroPayment(icdoPayeeAccountRetroPaymentDetail.payee_account_retro_payment_id);
        }	
    }
}