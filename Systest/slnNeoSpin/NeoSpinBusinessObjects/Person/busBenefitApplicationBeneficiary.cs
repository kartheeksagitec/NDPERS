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
	/// <summary>
	/// Class NeoSpin.BusinessObjects.busBenefitApplicationBeneficiary:
	/// Inherited from busBenefitApplicationBeneficiaryGen, the class is used to customize the business object busBenefitApplicationBeneficiaryGen.
	/// </summary>
	[Serializable]
	public class busBenefitApplicationBeneficiary : busBenefitApplicationBeneficiaryGen
	{
        public busBenefitApplication ibusBenefitApplication { get; set; }

        public void LoadBenefitApplication()        
        {
            if (ibusBenefitApplication.IsNull())
                ibusBenefitApplication = new busBenefitApplication();
            ibusBenefitApplication.FindBenefitApplication(icdoBenefitApplicationBeneficiary.benefit_application_id);
        }

        public busBenefitDroApplication ibusBenefitDROApplication { get; set; }

        public int iintApplicationId { get; set; }

        public void LoadBenefitDROApplication()
        {
            if (ibusBenefitDROApplication.IsNull())
                ibusBenefitDROApplication = new busBenefitDroApplication();
            ibusBenefitDROApplication.FindBenefitDroApplication(icdoBenefitApplicationBeneficiary.dro_application_id);
        }
	}
}
