#region Using directives

using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using NeoSpin.CustomDataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
	/// <summary>
	/// Class NeoSpin.BusinessObjects.busUserActivityLogLookup:
	/// Inherited from busUserActivityLogLookupGen, this class is used to customize the lookup business object busUserActivityLogLookupGen. 
	/// </summary>
	[Serializable]
	public class busUserActivityLogLookup : busUserActivityLogLookupGen
    {
        #region Properties

        /// <summary>
        /// Author                  : Framwork Team
        /// Modified By             : NA
        /// Applies To Use cases    : Security
        /// Usage                   : Store User Id
        /// </summary>
        public string user_id { get; set; }

        /// <summary>
        /// Author                  : Framwork Team
        /// Modified By             : NA
        /// Applies To Use cases    : Security
        /// Usage                   : Store User Name
        /// </summary>
        public string user_name { get; set; }

        #endregion
    }
}
