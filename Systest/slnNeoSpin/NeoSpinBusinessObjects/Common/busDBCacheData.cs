using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using Sagitec.BusinessObjects;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busDBCacheData : busExtendBase
    {
        public DataTable idtbCachedPaymentItemType { get; set; }
        public DataTable idtbCachedRateRef { get; set; }
        public DataTable idtbCachedRateStructureRef { get; set; }
        public DataTable idtbCachedCoverageRef { get; set; }
        public DataTable idtbCachedHealthRate { get; set; }
        public DataTable idtbCachedMedicarePartDRate { get; set; }
        public DataTable idtbCachedHMORate { get; set; }
        public DataTable idtbCachedVisionRate { get; set; }
        public DataTable idtbCachedDentalRate { get; set; }
        public DataTable idtbCachedLifeRate { get; set; }
        public DataTable idtbCachedLtcRate { get; set; }
        public DataTable idtbCachedEapRate { get; set; }
        public DataTable idtbCachedBenefitProvisionEligibility { get; set; }
        public DataTable idtbCachedCaseManagementStepDetails { get; set; }
        public DataTable idtbCachedPostRetirementDeathBenefitOptions { get; set; }
    }
}
