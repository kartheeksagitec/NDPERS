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
	public class busPayeeAccountTaxWithholdingItemDetail : busPayeeAccountTaxWithholdingItemDetailGen
	{
        public override int Delete()
        {
            return base.Delete();
        }
	}
}
