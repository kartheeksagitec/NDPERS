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
	/// Class NeoSpin.BusinessObjects.busMASPerson:
	/// Inherited from busMASPersonGen, the class is used to customize the business object busMASPersonGen.
	/// </summary>
	[Serializable]
	public class busMASPerson : busMasPersonGen
	{
        public busMASSelection ibusMASSelection { get; set; }

        public void LoadMASSelection()
        {
            if (ibusMASSelection.IsNull())
                ibusMASSelection = new busMASSelection { icdoMasSelection = new cdoMasSelection() };
            ibusMASSelection.FindMASSelection(icdoMasPerson.mas_selection_id);
        }
	}
}
