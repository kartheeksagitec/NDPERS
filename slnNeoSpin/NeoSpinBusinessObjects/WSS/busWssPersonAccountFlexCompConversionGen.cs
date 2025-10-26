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
    /// Class NeoSpin.BusinessObjects.busWssPersonAccountFlexCompConversionGen:
    /// Inherited from busBase, used to create new business object for main table cdoWssPersonAccountFlexCompConversion and its children table. 
    /// </summary>
	[Serializable]
	public class busWssPersonAccountFlexCompConversionGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busWssPersonAccountFlexCompConversionGen
        /// </summary>
		public busWssPersonAccountFlexCompConversionGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busWssPersonAccountFlexCompConversionGen.
        /// </summary>
		public cdoWssPersonAccountFlexCompConversion icdoWssPersonAccountFlexCompConversion { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busOrganization.
        /// </summary>
		public busOrganization ibusOrganization { get; set; }




        /// <summary>
        /// NeoSpin.busWssPersonAccountFlexCompConversionGen.FindWssPersonAccountFlexCompConversion():
        /// Finds a particular record from cdoWssPersonAccountFlexCompConversion with its primary key. 
        /// </summary>
        /// <param name="aintwsspersonflexcompconversionid">A primary key value of type int of cdoWssPersonAccountFlexCompConversion on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindWssPersonAccountFlexCompConversion(int aintwsspersonflexcompconversionid)
		{
			bool lblnResult = false;
			if (icdoWssPersonAccountFlexCompConversion == null)
			{
				icdoWssPersonAccountFlexCompConversion = new cdoWssPersonAccountFlexCompConversion();
			}
			if (icdoWssPersonAccountFlexCompConversion.SelectRow(new object[1] { aintwsspersonflexcompconversionid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        /// NeoSpin.busWssPersonAccountFlexCompConversionGen.LoadOrganization():
        /// Loads non-collection object ibusOrganization of type busOrganization.
        /// </summary>
		public virtual void LoadOrganization()
		{
			if (ibusOrganization == null)
			{
				ibusOrganization = new busOrganization();
			}
			ibusOrganization.FindOrganization(icdoWssPersonAccountFlexCompConversion.org_id);
		}

	}
}
