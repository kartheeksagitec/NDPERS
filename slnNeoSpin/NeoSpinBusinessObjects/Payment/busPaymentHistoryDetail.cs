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
    public class busPaymentHistoryDetail : busPaymentHistoryDetailGen
    {
        public Collection<busPaymentHistoryDetail> iclbPaymentHistoryDetail { get; set; }

        public bool LoadPaymentHistoryDetailByHistoryID(int aintPaymentHistoryID)
        {
            if (iclbPaymentHistoryDetail == null)
                iclbPaymentHistoryDetail = new Collection<busPaymentHistoryDetail>();
            bool lblnResult = false;
            DataTable ldtPaymentHistoryDetail = Select<cdoPaymentHistoryDetail>(new string[1] { "payment_history_header_id" },
                                                                    new object[1] { aintPaymentHistoryID },
                                                                    null, null);
            if (ldtPaymentHistoryDetail.Rows.Count > 0)
                lblnResult = true;
            iclbPaymentHistoryDetail = GetCollection<busPaymentHistoryDetail>(ldtPaymentHistoryDetail, "icdoPaymentHistoryDetail");
            return lblnResult;
        }
        public busPaymentHistoryHeader ibusPaymentHistoryHeader { get; set; }
        //Load Payment History Header
        public void LoadPaymentHistoryHeader()
        {
            if (ibusPaymentHistoryHeader == null)
                ibusPaymentHistoryHeader = new busPaymentHistoryHeader();
            ibusPaymentHistoryHeader.FindPaymentHistoryHeader(icdoPaymentHistoryDetail.payment_history_header_id);
        }

        private busOrganization _ibusVendor;
        public busOrganization ibusVendor
        {
            get { return _ibusVendor; }
            set { _ibusVendor = value; }
        }
        // Load the Vendor Org object for the selected Vendor.
        public void LoadVendor()
        {
            if (ibusVendor == null)
                ibusVendor = new busOrganization();
            ibusVendor.FindOrganization(icdoPaymentHistoryDetail.vendor_org_id);
        }
    }
}