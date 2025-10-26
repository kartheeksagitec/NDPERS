#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using  NeoSpin.CustomDataObjects;

#endregion

namespace  NeoSpin.BusinessObjects
{
    /// <summary>
    /// Class  NeoSpin.BusinessObjects.busPlanMemberTypeCrossrefGen:
    /// Inherited from busBase, used to create new business object for main table cdoPlanMemberTypeCrossref and its children table. 
    /// </summary>
	[Serializable]
	public class busPlanMemberTypeCrossrefGen : busExtendBase
    {
        /// <summary>
        /// Constructor for  NeoSpin.BusinessObjects.busPlanMemberTypeCrossrefGen
        /// </summary>
		public busPlanMemberTypeCrossrefGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPlanMemberTypeCrossrefGen.
        /// </summary>
		public cdoPlanMemberTypeCrossref icdoPlanMemberTypeCrossref { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busPlan.
        /// </summary>
		public busPlan ibusPlan { get; set; }




        /// <summary>
        ///  NeoSpin.busPlanMemberTypeCrossrefGen.FindPlanMemberTypeCrossref():
        /// Finds a particular record from cdoPlanMemberTypeCrossref with its primary key. 
        /// </summary>
        /// <param name="aintplanmembertypecrossrefid">A primary key value of type int of cdoPlanMemberTypeCrossref on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPlanMemberTypeCrossref(int aintplanmembertypecrossrefid)
		{
			bool lblnResult = false;
			if (icdoPlanMemberTypeCrossref == null)
			{
				icdoPlanMemberTypeCrossref = new cdoPlanMemberTypeCrossref();
			}
			if (icdoPlanMemberTypeCrossref.SelectRow(new object[1] { aintplanmembertypecrossrefid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        ///  NeoSpin.busPlanMemberTypeCrossrefGen.LoadPlan():
        /// Loads non-collection object ibusPlan of type busPlan.
        /// </summary>
		public virtual void LoadPlan()
		{
			if (ibusPlan == null)
			{
				ibusPlan = new busPlan();
			}
			ibusPlan.FindPlan(icdoPlanMemberTypeCrossref.plan_id);
		}

	}
}
