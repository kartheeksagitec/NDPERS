#region Using directives

using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.Bpm;

#endregion

namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// Class NeoSpin.busBpmEscalationLookup:
    /// Inherited from busBpmEscalationLookupGen, this class is used to customize the lookup business object busBpmEscalationLookupGen. 
    /// </summary>
    [Serializable]
    public class busBpmEscalationLookup : busBpmEscalationLookupGen
    {
        #region Overriden Methods

        /// <summary>
        /// This method is used to Load Bpm Escalations on lookup.
        /// </summary>
        /// <param name="adtbSearchResult">object of DataTable containing search result</param>
        public override void LoadBpmEscalations(DataTable adtbSearchResult)
        {
            base.LoadBpmEscalations(adtbSearchResult);
            foreach (busBpmEscalation lbusBpmEscalation in iclbBpmEscalation)
            {
                lbusBpmEscalation.LoadBpmActivity();
            }
        }

        #endregion
    }
}
