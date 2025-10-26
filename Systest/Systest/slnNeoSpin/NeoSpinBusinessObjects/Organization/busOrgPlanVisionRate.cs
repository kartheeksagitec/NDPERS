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
	public class busOrgPlanVisionRate : busOrgPlanVisionRateGen
	{
        private busOrganization _ibusProvider;
        public busOrganization ibusProvider
        {
            get { return _ibusProvider; }
            set { _ibusProvider = value; }
        }
        private int _iintProviderID;
        public int iintProviderID
        {
            get { return _iintProviderID; }
            set { _iintProviderID = value; }
        }
        public void LoadProvider(int AintProviderID)
        {
            if (_ibusProvider == null)
            {
                _ibusProvider = new busOrganization();
            }
            _ibusProvider.FindOrganization(AintProviderID);
        }
        public void LoadOrganization()
        {
            if (ibusOrganization == null)
            {
                ibusOrganization = new busOrganization();
            }
            ibusOrganization.FindOrganization(ibusOrgPlan.icdoOrgPlan.org_id);
        }
	}
}
