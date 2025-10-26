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
	/// Class NeoSpin.busWssBenAppDisaOtherBenefits:
	/// Inherited from busWssBenAppDisaOtherBenefitsGen, the class is used to customize the business object busWssBenAppDisaOtherBenefitsGen.
	/// </summary>
	[Serializable]
	public class busWssBenAppDisaOtherBenefits : busWssBenAppDisaOtherBenefitsGen
	{
        public Collection<busWssBenAppDisaOtherBenefits> iclcWssBenAppDisaOtherBenefits { get; set; }
    }
}
