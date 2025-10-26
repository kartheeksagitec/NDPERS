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
    public class busEmployerPurchaseAllocation : busEmployerPurchaseAllocationGen
    {
        private decimal _idecTotalExpectedPaymentAmount;
        public decimal idecTotalExpectedPaymentAmount
        {
            get { return _idecTotalExpectedPaymentAmount; }
            set { _idecTotalExpectedPaymentAmount = value; }
        }
       
        private Collection<busServicePurchaseHeader> _iclbServicePurchase;
        public Collection<busServicePurchaseHeader> iclbServicePurchase
        {
            get { return _iclbServicePurchase; }
            set { _iclbServicePurchase = value; }
        }

        private busEmployerPayrollDetail _ibusEmployerPayrollDetail;

        public busEmployerPayrollDetail ibusEmployerPayrollDetail
        {
            get { return _ibusEmployerPayrollDetail; }
            set { _ibusEmployerPayrollDetail = value; }
        }

        public void LoadEmployerPayrollDetail()
        {
            if (_ibusEmployerPayrollDetail == null)
            {
                _ibusEmployerPayrollDetail = new busEmployerPayrollDetail();
            }
            _ibusEmployerPayrollDetail.FindEmployerPayrollDetail(icdoEmployerPurchaseAllocation.employer_payroll_detail_id);
        }

        private busServicePurchaseHeader _ibusServicePurchaseHeader;

        public busServicePurchaseHeader ibusServicePurchaseHeader
        {
            get { return _ibusServicePurchaseHeader; }
            set { _ibusServicePurchaseHeader = value; }
        }

        public void LoadPurchaseHeader()
        {
            if (_ibusServicePurchaseHeader == null)
            {
                _ibusServicePurchaseHeader = new busServicePurchaseHeader();
            }
            _ibusServicePurchaseHeader.FindServicePurchaseHeader(icdoEmployerPurchaseAllocation.service_purchase_header_id);
        }

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            if (icdoEmployerPurchaseAllocation.service_purchase_header_id != 0)
            {
                LoadPurchaseHeader();               
            } 
            LoadAllServicePurchaseHeaderForAllocation(_ibusEmployerPayrollDetail);
            base.BeforeValidate(aenmPageMode);
        }

        /// <summary>
        /// Load All Purchase For employer detail Person
        /// which are in IN-Payment Status and payroll deduction checkbox is checked.
        /// </summary>
        /// <param name="aobjEmployerPayrollDetail"></param>
        public void LoadAllServicePurchaseHeaderForAllocation(busEmployerPayrollDetail aobjEmployerPayrollDetail)
        {
            LoadPurchaseForPerson(aobjEmployerPayrollDetail);
            foreach (busServicePurchaseHeader lobjServicePurchaseHeader in iclbServicePurchase)
            {
                if (lobjServicePurchaseHeader.icdoServicePurchaseHeader.expected_installment_amount != 0.00M)
                {
                    _idecTotalExpectedPaymentAmount += lobjServicePurchaseHeader.icdoServicePurchaseHeader.expected_installment_amount;
                }
            }         
        }

        public void LoadPurchaseForPerson(busEmployerPayrollDetail aobjEmployerPayrollDetail)
        {
            DataTable ldtbAppliedRemittance = busNeoSpinBase.Select("cdoEmployerPurchaseAllocation.LoadPurchaseForPerson", new object[1] {                         
                        aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.person_id });
            _iclbServicePurchase = GetCollection<busServicePurchaseHeader>(ldtbAppliedRemittance, "icdoServicePurchaseHeader");
        }
    }
}
