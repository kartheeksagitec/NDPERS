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
	/// Class NeoSpin.BusinessObjects.busUserActivityLogQueries:
	/// Inherited from busUserActivityLogQueriesGen, the class is used to customize the business object busUserActivityLogQueriesGen.
	/// </summary>
	[Serializable]
	public class busUserActivityLogQueries : busUserActivityLogQueriesGen
	{
        #region Properties

        /// <summary>
        /// Author                  : Framwork Team
        /// Modified By             : NA
        /// Applies To Use cases    : Security
        /// Usage                   : Store User Activity Log Detail
        /// </summary>
        public busUserActivityLogDetail ibusUserActivityLogDetail { get; set; }

        #endregion
    }
}
