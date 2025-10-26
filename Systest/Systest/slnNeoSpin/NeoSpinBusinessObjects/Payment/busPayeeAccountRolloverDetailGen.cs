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
    public class busPayeeAccountRolloverDetailGen : busExtendBase
    {
        public busPayeeAccountRolloverDetailGen()
        {

        }
        private bool _iblnIsRolloverToBeAdjusted;

        public bool iblnIsRolloverToBeAdjusted
        {
            get { return _iblnIsRolloverToBeAdjusted; }
            set { _iblnIsRolloverToBeAdjusted = value; }
        }
        private busPayeeAccount _ibusPayeeAccount;

        public busPayeeAccount ibusPayeeAccount
        {
            get { return _ibusPayeeAccount; }
            set { _ibusPayeeAccount = value; }
        }
        private busOrganization _ibusRolloverOrg;

        public busOrganization ibusRolloverOrg
        {
            get { return _ibusRolloverOrg; }
            set { _ibusRolloverOrg = value; }
        }
        private cdoPayeeAccountRolloverDetail _icdoPayeeAccountRolloverDetail;
        public cdoPayeeAccountRolloverDetail icdoPayeeAccountRolloverDetail
        {
            get
            {
                return _icdoPayeeAccountRolloverDetail;
            }
            set
            {
                _icdoPayeeAccountRolloverDetail = value;
            }
        }
        private Collection<busPayeeAccountRolloverItemDetail> _iclbRolloverItemDetail;
        public Collection<busPayeeAccountRolloverItemDetail> iclbRolloverItemDetail
        {
            get { return _iclbRolloverItemDetail; }
            set { _iclbRolloverItemDetail = value; }
        }
        private Collection<busPayeeAccountPaymentItemType> _iclbPayeeAcountRolloverItems;

        public Collection<busPayeeAccountPaymentItemType> iclbPayeeAcountRolloverItems
        {
            get { return _iclbPayeeAcountRolloverItems; }
            set { _iclbPayeeAcountRolloverItems = value; }
        }

        public bool FindPayeeAccountRolloverDetail(int Aintpayeeaccountrolloverinfoid)
        {
            bool lblnResult = false;
            if (_icdoPayeeAccountRolloverDetail == null)
            {
                _icdoPayeeAccountRolloverDetail = new cdoPayeeAccountRolloverDetail();
            }
            if (_icdoPayeeAccountRolloverDetail.SelectRow(new object[1] { Aintpayeeaccountrolloverinfoid }))
            {
                lblnResult = true;
            }
            return lblnResult;
        }
        public void LoadPayeeAccount()
        {
            if (_ibusPayeeAccount == null)
            {
                _ibusPayeeAccount = new busPayeeAccount();
            }
            _ibusPayeeAccount.FindPayeeAccount(icdoPayeeAccountRolloverDetail.payee_account_id);
        }
        public void LoadRolloverOrgByOrgID()
        {
            if (ibusRolloverOrg == null)
            {
                ibusRolloverOrg = new busOrganization();
            }
            ibusRolloverOrg.FindOrganization(icdoPayeeAccountRolloverDetail.rollover_org_id);
        }
        public void LoadRolloverOrgByOrgcode()
        {
            if (ibusRolloverOrg == null)
            {
                ibusRolloverOrg = new busOrganization();
            }
            ibusRolloverOrg.FindOrganizationByOrgCode(icdoPayeeAccountRolloverDetail.org_code);
        }
        public void LoadRolloverItemDetail()
        {
            DataTable ldtbList = Select<cdoPayeeAccountRolloverItemDetail>(
                  new string[1] { "payee_account_rollover_detail_id" },
                  new object[1] { icdoPayeeAccountRolloverDetail.payee_account_rollover_detail_id }, null, null);
            _iclbRolloverItemDetail = GetCollection<busPayeeAccountRolloverItemDetail>(ldtbList, "icdoPayeeAccountRolloverItemDetail");
        }
        public void LoadPayeeAccountRolloverItems()
        {
            if (_iclbPayeeAcountRolloverItems == null)
                _iclbPayeeAcountRolloverItems = new Collection<busPayeeAccountPaymentItemType>();
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.iclbPayeeAccountPaymentItemType == null)
                ibusPayeeAccount.LoadPayeeAccountPaymentItemType();
            if (_iclbRolloverItemDetail == null)
                LoadRolloverItemDetail();
            foreach (busPayeeAccountPaymentItemType lobjRolloverItems in ibusPayeeAccount.iclbPayeeAccountPaymentItemType)
            {
                foreach (busPayeeAccountRolloverItemDetail lobjRolloverItemDetail in _iclbRolloverItemDetail)
                {
                    if (lobjRolloverItems.icdoPayeeAccountPaymentItemType.payee_account_payment_item_type_id
                        == lobjRolloverItemDetail.icdoPayeeAccountRolloverItemDetail.payee_account_payment_item_type_id)
                    {
                        _iclbPayeeAcountRolloverItems.Add(lobjRolloverItems);
                    }
                }
            }
        }
    }
}