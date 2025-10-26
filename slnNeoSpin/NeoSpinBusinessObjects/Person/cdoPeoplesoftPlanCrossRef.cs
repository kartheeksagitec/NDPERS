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
using Sagitec.DataObjects;
using NeoSpin.DataObjects;


#endregion

namespace NeoSpin.CustomDataObjects
{
    [Serializable]
	public class cdoPeoplesoftPlanCrossRef : doPeoplesoftPlanCrossRef
	{
		public cdoPeoplesoftPlanCrossRef() : base()
		{
		}
        public bool IsCheckFlexCompPlanTrue
        {
            get
            {
                if (check_flex_comp_plan == "Y")
                    return true;
                else
                    return false;
            }
        }
    } 
} 
