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
	public class busPersonAccountOtherCoverageDetail : busPersonAccountOtherCoverageDetailGen
	{
        public override void BeforePersistChanges()
        {
            if (icdoPersonAccountOtherCoverageDetail.provider_org_code != string.Empty)
            {
                icdoPersonAccountOtherCoverageDetail.provider_org_id = busGlobalFunctions.GetOrgIdFromOrgCode(icdoPersonAccountOtherCoverageDetail.provider_org_code);
                LoadProviderName();
            }
            base.BeforePersistChanges();
        }

        public void LoadProviderName()
        {
            if (icdoPersonAccountOtherCoverageDetail.provider_org_id != 0)
                icdoPersonAccountOtherCoverageDetail.provider_org_name = busGlobalFunctions.GetOrgNameByOrgID(icdoPersonAccountOtherCoverageDetail.provider_org_id);
        }

        public void LoadProviderOrgCode()
        {
            if (icdoPersonAccountOtherCoverageDetail.provider_org_id != 0)
                icdoPersonAccountOtherCoverageDetail.provider_org_code = busGlobalFunctions.GetOrgCodeFromOrgId(icdoPersonAccountOtherCoverageDetail.provider_org_id);
        }

        //This method called when we click Hand Icon. That will reload the object and display the details
        public ArrayList ReloadHandButtonData()
        {
            ArrayList larrList = new ArrayList();
            icdoPersonAccountOtherCoverageDetail.provider_org_id = busGlobalFunctions.GetOrgIdFromOrgCode(icdoPersonAccountOtherCoverageDetail.provider_org_code);
            LoadProviderName();
            larrList.Add(this);
            return larrList;
        }
	}
}
