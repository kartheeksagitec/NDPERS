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
    public partial class busJsRhicBill : busExtendBase
    {
        #region QueryProperties
        private int _iintJSRHOrgID;
        public int iintJSRHOrgID
        {
            get
            {
                return _iintJSRHOrgID;
            }
            set
            {
                _iintJSRHOrgID = value;
            }
        }
        private int _iintIBSHeaderID;
        public int iintIBSHeaderID
        {
            get
            {
                return _iintIBSHeaderID;
            }
            set
            {
                _iintIBSHeaderID = value;
            }
        }
        private int _iintOrgID;
        public int iintOrgID
        {
            get
            {
                return _iintOrgID;
            }
            set
            {
                _iintOrgID = value;
            }
        }
        private int _iintRemittanceID;
        public int iintRemittanceID
        {
            get
            {
                return _iintRemittanceID;
            }
            set
            {
                _iintRemittanceID = value;
            }
        }
        private string _istrOrgCode;
        public string istrOrgCode
        {
            get
            {
                return _istrOrgCode;
            }
            set
            {
                _istrOrgCode = value;
            }
        }
        private decimal _idecRemittaceAmount;
        public decimal idecRemittaceAmount
        {
            get
            {
                return _idecRemittaceAmount;
            }
            set
            {
                _idecRemittaceAmount = value;
            }
        }
        private decimal _idecAllocatedAmount;
        public decimal idecAllocatedAmount
        {
            get
            {
                return _idecAllocatedAmount;
            }
            set
            {
                _idecAllocatedAmount = value;
            }
        }
        private decimal _idecAvailableAmount;
        public decimal idecAvailableAmount
        {
            get
            {
                return _idecAvailableAmount;
            }
            set
            {
                _idecAvailableAmount = value;
            }
        }
        private decimal _idecRemainingBillAmount;
        public decimal idecRemainingBillAmount
        {
            get
            {
                return _idecRemainingBillAmount;
            }
            set
            {
                _idecRemainingBillAmount = value;
            }
        }
        #endregion
        #region BusProperties
        private Collection<busJsRhicBill> _iclbJsRhicBillAvlRemittance;
        public Collection<busJsRhicBill> iclbJsRhicBillAvlRemittance
        {
            get
            {
                return _iclbJsRhicBillAvlRemittance;
            }
            set
            {
                _iclbJsRhicBillAvlRemittance = value;
            }
        }
        private Collection<busJsRhicBill> _iclbJsRhicBillAllocationHistory;
        public Collection<busJsRhicBill> iclbJsRhicBillAllocationHistory
        {
            get
            {
                return _iclbJsRhicBillAllocationHistory;
            }
            set
            {
                _iclbJsRhicBillAllocationHistory = value;
            }
        }
        private busRemittance _ibusAvlRemittance;
        public busRemittance ibusAvlRemittance
        {
            get
            {
                return _ibusAvlRemittance;
            }
            set
            {
                _ibusAvlRemittance = value;
            }
        }
        private busRemittance _ibusRemittance;
        public busRemittance ibusRemittance
        {
            get
            {
                return _ibusRemittance;
            }
            set
            {
                _ibusRemittance = value;
            }
        }
        private busJsRhicRemittanceAllocation _ibusJsRhicRemittanceAllocation;
        public busJsRhicRemittanceAllocation ibusJsRhicRemittanceAllocation
        {
            get
            {
                return _ibusJsRhicRemittanceAllocation;
            }
            set
            {
                _ibusJsRhicRemittanceAllocation = value;
            }
        }
        private busDeposit _ibusDeposit;
        public busDeposit ibusDeposit
        {
            get
            {
                return _ibusDeposit;
            }
            set
            {
                _ibusDeposit = value;
            }
        }
        private busIbsDetail _ibusIbsDetail;
        public busIbsDetail ibusIbsDetail
        {
            get
            {
                return _ibusIbsDetail;
            }
            set
            {
                _ibusIbsDetail = value;
            }
        }
#endregion
        public void LoadDeposit()
        {
            if (_ibusDeposit == null)
            {
                _ibusDeposit = new busDeposit();
            }
            _ibusDeposit.FindDeposit(_ibusAvlRemittance.icdoRemittance.deposit_id);
        }

        public void LoadIBSDetail()
        {
            if (_ibusIbsDetail == null)
            {
                _ibusIbsDetail = new busIbsDetail();
            }
            _ibusIbsDetail.FindIbsDetail(_icdoJsRhicBill.js_rhic_bill_id);
        }
        public void LoadRemittanceForAvlRemittance()
        {
            if (_ibusAvlRemittance == null)
            {
                _ibusAvlRemittance = new busRemittance();
            }
            _ibusAvlRemittance.FindRemittance(_iintRemittanceID);
        }
        public void LoadRemittanceForAllocation()
        {
            if (_ibusRemittance == null)
            {
                _ibusRemittance = new busRemittance();
            }
            _ibusRemittance.FindRemittance(_ibusJsRhicRemittanceAllocation.icdoJsRhicRemittanceAllocation.remittance_id);
        }
        public void LoadAvalilableRemittance()
        {
            DataTable ldtbList = Select("cdoJsRhicBill.LoadAvailableRemittance",
                        new object[1] { _icdoJsRhicBill.org_id });
            _iclbJsRhicBillAvlRemittance = new Collection<busJsRhicBill>();
            foreach (DataRow ldtr in ldtbList.Rows)
            {
                busJsRhicBill lobjJsRhicBill = new busJsRhicBill();
                sqlFunction.LoadQueryResult(lobjJsRhicBill, ldtr);
                _iclbJsRhicBillAvlRemittance.Add(lobjJsRhicBill);
            }
            foreach (busJsRhicBill lobjJsRhicBill in _iclbJsRhicBillAvlRemittance)
            {
                lobjJsRhicBill.LoadRemittanceForAvlRemittance();
                lobjJsRhicBill.LoadDeposit();
            }
        }
        public void LoadAllocationHistory()
        {
            DataTable ldtbList = Select("cdoJsRhicBill.LoadAllocationHistory",
                        new object[1] { _icdoJsRhicBill.org_id });
            _iclbJsRhicBillAllocationHistory = new Collection<busJsRhicBill>();
            foreach (DataRow ldtr in ldtbList.Rows)
            {
                busJsRhicBill lobjJsRhicBill = new busJsRhicBill();
                sqlFunction.LoadQueryResult(lobjJsRhicBill, ldtr);
                _iclbJsRhicBillAllocationHistory.Add(lobjJsRhicBill);
            }
            foreach (busJsRhicBill lobjJsRhicBill in _iclbJsRhicBillAllocationHistory)
            {
                lobjJsRhicBill.LoadRemittanceForAvlRemittance();
                lobjJsRhicBill.LoadDeposit();
            }
        }
        public int GetJSRHOrgID()
        {
            _iintJSRHOrgID = Convert.ToInt32(DBFunction.DBExecuteScalar("cdoJsRhicBill.GetJSRHOrgID",  iobjPassInfo.iconFramework,  iobjPassInfo.itrnFramework));
            return _iintJSRHOrgID;
        }
        public ArrayList btnAllocate_Click()
        {
            ArrayList larrErros = new ArrayList();
            utlError lobjError = null;
            int lintIBSHeaderID=0;
            DataTable ldtbListHdr = Select<cdoIbsHeader>(new string[1] { "js_rhic_bill_id" },new object[1] { _icdoJsRhicBill.js_rhic_bill_id }, null, null);
            if (ldtbListHdr.Rows.Count > 0)
            {
                lintIBSHeaderID = Convert.ToInt32(ldtbListHdr.Rows[0]["IBS_HEADER_ID"]);
            }            
           
            if ((_ibusJsRhicRemittanceAllocation.icdoJsRhicRemittanceAllocation.remittance_id == 0) || (_ibusJsRhicRemittanceAllocation.icdoJsRhicRemittanceAllocation.allocated_amount == 0.00M))
            {
                lobjError = AddError(4505, "");
                larrErros.Add(lobjError);
                return larrErros;
            }
            else if (_icdoJsRhicBill.bill_amount == _icdoJsRhicBill.allocated_amount)
            {
                lobjError = AddError(4509, "");
                larrErros.Add(lobjError);
                return larrErros;
            }
            else if (_ibusJsRhicRemittanceAllocation.icdoJsRhicRemittanceAllocation.remittance_id != 0)
            {
                LoadRemittanceForAllocation();
                DataTable ldtbListRem = Select("cdoJsRhicBill.FindJsRhicRemittance",
                        new object[3] { lintIBSHeaderID, _icdoJsRhicBill.js_rhic_bill_id, _ibusJsRhicRemittanceAllocation.icdoJsRhicRemittanceAllocation.remittance_id });
                if (ldtbListRem.Rows.Count > 0)
                {
                    lobjError = AddError(4510, "");
                    larrErros.Add(lobjError);
                    return larrErros;
                }
            }
            if ((_ibusJsRhicRemittanceAllocation.icdoJsRhicRemittanceAllocation.remittance_id != 0) || (_ibusJsRhicRemittanceAllocation.icdoJsRhicRemittanceAllocation.allocated_amount != 0.00M))
            {
                LoadRemittanceForAllocation();
                if ((_ibusRemittance.icdoRemittance.org_id == GetJSRHOrgID()) && (_ibusRemittance.icdoRemittance.remittance_type_value == "JSRD"))
                {
                    if (_ibusJsRhicRemittanceAllocation.icdoJsRhicRemittanceAllocation.allocated_amount <= _ibusRemittance.icdoRemittance.remittance_amount)
                    {
                        if (_ibusJsRhicRemittanceAllocation.icdoJsRhicRemittanceAllocation.allocated_amount <=_icdoJsRhicBill.bill_amount)
                        {
                            LoadIBSDetail();
                            _ibusJsRhicRemittanceAllocation.icdoJsRhicRemittanceAllocation.rhic_allocation_status_value = "ALTD";
                            _ibusJsRhicRemittanceAllocation.icdoJsRhicRemittanceAllocation.js_rhic_bill_id = _icdoJsRhicBill.js_rhic_bill_id;
                            _ibusJsRhicRemittanceAllocation.icdoJsRhicRemittanceAllocation.ibs_header_id = lintIBSHeaderID;
                            _ibusJsRhicRemittanceAllocation.icdoJsRhicRemittanceAllocation.Insert();
                            _icdoJsRhicBill.allocated_amount+= _ibusJsRhicRemittanceAllocation.icdoJsRhicRemittanceAllocation.allocated_amount;
                            _icdoJsRhicBill.Update();
                            idecRemainingBillAmount = _icdoJsRhicBill.bill_amount - _icdoJsRhicBill.allocated_amount;
                            LoadAvalilableRemittance();
                            LoadAllocationHistory();
                        }
                        else
                        {
                            lobjError = AddError(4508, "");
                            larrErros.Add(lobjError);
                            return larrErros;
                        }
                    }
                    else
                    {
                        lobjError = AddError(4507, "");
                        larrErros.Add(lobjError);
                        return larrErros;
                    }
                }
                else
                {
                    lobjError = AddError(4506, "");
                    larrErros.Add(lobjError);
                    return larrErros;
                }
            }
            larrErros.Add(this);
            return larrErros;
        }
    }
}
