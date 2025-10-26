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
    public partial class busNewGroup : busExtendBase
    {

        public bool FindNewGroupByContactTicket(int Aintcontactticketid)
        {
            if (_icdoNewGroup == null)
            {
                _icdoNewGroup = new cdoNewGroup();
            }
            DataTable ldtb = Select<cdoNewGroup>(new string[1] { "contact_ticket_id" },
                  new object[1] { Aintcontactticketid }, null, null);
            if (ldtb.Rows.Count > 0)
            {
                _icdoNewGroup.LoadData(ldtb.Rows[0]);
                return true;
            }
            return false;
        }
        private utlCollection<cdoNewGroupPlanType> _iclcNewGroupPlanType;

        public utlCollection<cdoNewGroupPlanType> iclcNewGroupPlanType
        {
            get { return _iclcNewGroupPlanType; }
            set { _iclcNewGroupPlanType = value; }
        }
        public override void SetParentKey(Sagitec.DataObjects.doBase aobjBase)
        {
            cdoNewGroupPlanType lobjcdoNewGroupPlanType = (cdoNewGroupPlanType)aobjBase;
            lobjcdoNewGroupPlanType.new_group_id = icdoNewGroup.new_group_id;
        }
        public void LoadPlanTypes()
        {
            _iclcNewGroupPlanType = GetCollection<cdoNewGroupPlanType>(
                new string[1] { "new_group_id" }, new object[1] { this.icdoNewGroup.new_group_id }, null, null);
        }
    }
}
