#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using NeoSpin.DataObjects;

#endregion

namespace NeoSpin.CustomDataObjects
{
    [Serializable]
	public class cdoOrgPlanGroupHealthMedicarePartDCoverageRef : doOrgPlanGroupHealthMedicarePartDCoverageRef
	{
		public cdoOrgPlanGroupHealthMedicarePartDCoverageRef() : base()
		{
		}
    }
 
    public class cdoOrgPlanGroupHealthEquality : IEqualityComparer<cdoOrgPlanGroupHealthMedicarePartDCoverageRef>
    {

        public bool Equals(cdoOrgPlanGroupHealthMedicarePartDCoverageRef x, cdoOrgPlanGroupHealthMedicarePartDCoverageRef y)
        {
            return x.coverage_code == y.coverage_code;
        }

        public int GetHashCode(cdoOrgPlanGroupHealthMedicarePartDCoverageRef obj)
        {
            return obj.coverage_code.GetHashCode();
        }
    }
} 
