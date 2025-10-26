#region Using directives

using System;
using System.Collections;
using System.Data;
using NeoSpin.Common;
using Sagitec.Common;
using NeoSpin.CustomDataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
	/// <summary>
	/// Class NeoSpin.BusinessObjects.busDuesRateChangeRequest:
	/// Inherited from busDuesRateChangeRequestGen, the class is used to customize the business object busDuesRateChangeRequestGen.
	/// </summary>
	[Serializable]
	public class busDuesRateChangeRequest : busDuesRateChangeRequestGen
    {
        # region Business Rules

        //check if any duplicate record exist
        public bool IsRecordDuplicate()
        {
            DataTable ldtbRecordList = Select<cdoDuesRateChangeRequest>(new string[3] { "MONTHLY_AMOUNT", "EFFECTIVE_DATE", "VENDOR_ORG_ID" },
                                        new object[3]{icdoDuesRateChangeRequest.monthly_amount,icdoDuesRateChangeRequest.effective_date,
                                        icdoDuesRateChangeRequest.vendor_org_id}, null, null);
            if (ldtbRecordList.Rows.Count >= 1)
            {               
                    if (icdoDuesRateChangeRequest.dues_rate_change_request_id != Convert.ToInt32(ldtbRecordList.Rows[0]["DUES_RATE_CHANGE_REQUEST_ID"]))
                        return false;                      
            }
            return true;
        }

        //check is effective date day is first of month
        public bool IsEffectiveDateFirstOfMonth()
        {
            return icdoDuesRateChangeRequest.effective_date == DateTime.MinValue ? 
                                            true:(icdoDuesRateChangeRequest.effective_date.Day.Equals(1)) ? true:false;                
        }

        public bool IsLoginUserSameAsCreatedBy()
        {
            return (icdoDuesRateChangeRequest.created_by == null ? true :
                                    icdoDuesRateChangeRequest.created_by.Equals(iobjPassInfo.istrUserID));
        }

        public int IsEffectiveDateValid()
        {
            return icdoDuesRateChangeRequest.effective_date == DateTime.MinValue ? 1 :
                                    icdoDuesRateChangeRequest.effective_date.CompareTo(busPayeeAccountHelper.GetLastBenefitPaymentDate());
        }

        # endregion

        # region Button Logic

        public ArrayList btnUpdateDues()
        {
            ArrayList larrList = new ArrayList();
            ValidateHardErrors(utlPageMode.All);
            if (iarrErrors.Count == 0)
            {
                icdoDuesRateChangeRequest.status_value = busConstant.StatusApproved;
                icdoDuesRateChangeRequest.Update();
            }
            else
            {
                foreach (utlError lerrors in iarrErrors)
                {
                    larrList.Add(lerrors);
                }
            }
            return larrList;
        }

        # endregion

        # region overriden Methods

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            if (!String.IsNullOrEmpty(icdoDuesRateChangeRequest.istrOrgCodeId))
            {
                if (ibusVendor.FindOrganizationByOrgCode(icdoDuesRateChangeRequest.istrOrgCodeId))
                {
                    icdoDuesRateChangeRequest.vendor_org_id = ibusVendor.icdoOrganization.org_id;
                }
            }
            base.BeforeValidate(aenmPageMode);
        }        
        # endregion
    }
}
