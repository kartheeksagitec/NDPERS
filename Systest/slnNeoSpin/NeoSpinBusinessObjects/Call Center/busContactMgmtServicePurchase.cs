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
using Sagitec.CustomDataObjects;
#endregion

namespace NeoSpin.BusinessObjects
{
    public partial class busContactMgmtServicePurchase : busExtendBase
    {
        public bool FindServicePurchaseByContactTicket(int Aintcontactticketid)
        {
            if (_icdoContactMgmtServicePurchase == null)
            {
                _icdoContactMgmtServicePurchase = new cdoContactMgmtServicePurchase();
            }
            DataTable ldtb = Select<cdoContactMgmtServicePurchase>(new string[1] { "contact_ticket_id" },
                  new object[1] { Aintcontactticketid }, null, null);
            if (ldtb.Rows.Count > 0)
            {
                _icdoContactMgmtServicePurchase.LoadData(ldtb.Rows[0]);
                return true;
            }
            return false;

        }

        private utlCollection<cdoSerPurRolloverInfo> _iclcSerPurRolloverInfo;
        public utlCollection<cdoSerPurRolloverInfo> iclcSerPurRolloverInfo
        {
            get { return _iclcSerPurRolloverInfo; }
            set { _iclcSerPurRolloverInfo = value; }
        }

        // load Roll Info
        public void LoadServiceRollInfo()
        {
            _iclcSerPurRolloverInfo = GetCollection<cdoSerPurRolloverInfo>(
                   new string[1] { "service_purchase_id" }, new object[1] { this.icdoContactMgmtServicePurchase.service_purchase_id }, null, null);
        }

        private busSerPurServiceType _ibusSerPurServiceType;

        public busSerPurServiceType ibusSerPurServiceType
        {
            get { return _ibusSerPurServiceType; }
            set { _ibusSerPurServiceType = value; }
        }
        private Collection<busSerPurServiceType> _iclbSerPurServiceType;

        public Collection<busSerPurServiceType> iclbSerPurServiceType
        {
            get { return _iclbSerPurServiceType; }
            set { _iclbSerPurServiceType = value; }
        }

        public void LoadSerPurType()
        {

            DataTable ldtbList1 = busBase.Select<cdoCodeValue>(new string[1] { "code_id" }, new object[1] { 1020 }, null, "code_value asc");
            if (ldtbList1.Rows.Count != 0)
            {
                int lintIndex = 0;
                if (_iclbSerPurServiceType == null)
                {
                    _iclbSerPurServiceType = new Collection<busSerPurServiceType>();
                }
                foreach (DataRow dr in ldtbList1.Rows)
                {
                    busSerPurServiceType lobjSerPurServiceType = new busSerPurServiceType();
                    lobjSerPurServiceType.icdoSerPurServiceType = new cdoSerPurServiceType();
                    lobjSerPurServiceType.icdoSerPurServiceType.ser_pur_service_type_id = lintIndex + 1;
                    lobjSerPurServiceType.icdoSerPurServiceType.service_type_description = (ldtbList1.Rows[lintIndex]["description"]).ToString();
                    lobjSerPurServiceType.icdoSerPurServiceType.service_type_value = (ldtbList1.Rows[lintIndex]["code_value"]).ToString();
                    _iclbSerPurServiceType.Add(lobjSerPurServiceType);
                    lintIndex++;
                }
            }
        }
        public void LoadSerPurServiceTypeAfterInserting()
        {
            DataTable ldtbList1 = busBase.Select<cdoSerPurServiceType>(new string[1] { "service_purchase_id" }, new object[1] { this.icdoContactMgmtServicePurchase.service_purchase_id }, null, "ser_pur_service_type_id asc");
            _iclbSerPurServiceType = GetCollection<busSerPurServiceType>(ldtbList1, "icdoSerPurServiceType");
        }
        public busContactTicket ibusContactTicket { get; set; }
    }
}
