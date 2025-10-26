#region Using directives

using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using NeoSpin.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;

#endregion

namespace NeoSpin.BusinessObjects
{
	public partial class busOrganizationLookup
	{
        public override ArrayList ValidateNew(Hashtable ahstParam)
        {
            // PROD PIR 4087
            ArrayList larrErrors = new ArrayList();
            if (string.IsNullOrEmpty(ahstParam["astr_org_type_value"].ToString()))
            {
                utlError lobjError = null;
                lobjError = AddError(4011, "");
                larrErrors.Add(lobjError);
            }
            else if (ahstParam["astr_org_type_value"].ToString() == busConstant.OrganizationTypeEmployer)
            {
                if (string.IsNullOrEmpty(ahstParam["astr_employer_type_value"].ToString()))
                {
                    utlError lobjError = null;
                    lobjError = AddError(4006, "");
                    larrErrors.Add(lobjError);
                }
            }
            return larrErrors;
        }

        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            busOrganization lbusOrganization = (busOrganization)aobjBus;
            
            lbusOrganization.ibusOrgPrimaryAddress = new busOrgContactAddress();
            lbusOrganization.ibusOrgPrimaryAddress.icdoOrgContactAddress = new cdoOrgContactAddress();
            lbusOrganization.ibusOrgPrimaryAddress.icdoOrgContactAddress.LoadData(adtrRow);
            //lbusOrganization.LoadOrgPrimaryAddress(); //PIR 207
        }
	}
}
