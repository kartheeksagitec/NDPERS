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
    /// Class NeoSpin.BusinessObjects.busWssPersonAccountLifeOptionGen:
    /// Inherited from busBase, used to create new business object for main table cdoWssPersonAccountLifeOption and its children table. 
    /// </summary>
	[Serializable]
	public class busWssPersonAccountLifeOptionGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busWssPersonAccountLifeOptionGen
        /// </summary>
		public busWssPersonAccountLifeOptionGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busWssPersonAccountLifeOptionGen.
        /// </summary>
		public cdoWssPersonAccountLifeOption icdoWssPersonAccountLifeOption { get; set; }




        /// <summary>
        /// NeoSpin.busWssPersonAccountLifeOptionGen.FindWssPersonAccountLifeOption():
        /// Finds a particular record from cdoWssPersonAccountLifeOption with its primary key. 
        /// </summary>
        /// <param name="aintwsspersonaccountlifeoptionid">A primary key value of type int of cdoWssPersonAccountLifeOption on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindWssPersonAccountLifeOption(int aintwsspersonaccountlifeoptionid)
		{
			bool lblnResult = false;
			if (icdoWssPersonAccountLifeOption == null)
			{
				icdoWssPersonAccountLifeOption = new cdoWssPersonAccountLifeOption();
			}
			if (icdoWssPersonAccountLifeOption.SelectRow(new object[1] { aintwsspersonaccountlifeoptionid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
