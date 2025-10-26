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
	public partial class busBenefitEstimate : busExtendBase
    {
        public bool FindBenefitEstimateByContactTicket(int Aintcontactticketid)
        {
            if (_icdoBenefitEstimate == null)
            {
                _icdoBenefitEstimate = new cdoBenefitEstimate();
            }
            DataTable ldtb = Select<cdoBenefitEstimate>(new string[1] { "contact_ticket_id" },
                  new object[1] { Aintcontactticketid }, null, null);
            if (ldtb.Rows.Count > 0)
            {
                _icdoBenefitEstimate.LoadData(ldtb.Rows[0]);
                return true;
            }
            return false;

        }

        public utlCollection<cdoBenefitEstimateRetirementType> iclcRetirementType { get; set; }

        public override void SetParentKey(Sagitec.DataObjects.doBase aobjBase)
        {
            if (aobjBase is cdoBenefitEstimateRetirementType)
            {
                cdoBenefitEstimateRetirementType lcdoRetirementType = (cdoBenefitEstimateRetirementType)aobjBase;
                lcdoRetirementType.benefit_estimate_id = icdoBenefitEstimate.benefit_estimate_id;
            }
        }

        public void LoadRetirementType()
        {
            iclcRetirementType = new utlCollection<cdoBenefitEstimateRetirementType>();
            iclcRetirementType = GetCollection<cdoBenefitEstimateRetirementType>(
                new string[1] { "BENEFIT_ESTIMATE_ID" }, new object[1] { icdoBenefitEstimate.benefit_estimate_id }, null, null);
        }
        public busContactTicket ibusContactTicket { get; set; }
	}
}
