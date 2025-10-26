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
	/// Class NeoSpin.BusinessObjects.busWssPersonAccountLifeOption:
	/// Inherited from busWssPersonAccountLifeOptionGen, the class is used to customize the business object busWssPersonAccountLifeOptionGen.
	/// </summary>
	[Serializable]
	public class busWssPersonAccountLifeOption : busWssPersonAccountLifeOptionGen
	{
        public string IsSupplementalAmountMultipleof5000
        {
            get
            {
                if (icdoWssPersonAccountLifeOption.supplemental_amount % 5000M == 0)
                    return busConstant.Flag_Yes;
                return busConstant.Flag_No;
            }
        }

        public string IsSpouseSupplementalAmountMultipleof5000
        {
            get
            {
                if (icdoWssPersonAccountLifeOption.spouse_supplemental_amount % 5000M == 0)
                    return busConstant.Flag_Yes;
                return busConstant.Flag_No;
            }
        }
	}
}
