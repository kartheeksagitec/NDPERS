using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public partial class busOrgContactPlanAndRole : busExtendBase
    {
        public int iintCodeId { get; set; }
        public string istrCodeValueRole { get; set; }
        public string istrDescriptionRole { get; set; }
        public string istrCheckDefComp { get; set; }
        public string istrCheckInsurance { get; set; }
        public string istrCheckRetirement { get; set; }
        public string istrCheckFlex { get; set; }
        public string istrCheckNoPlan { get; set; }
    }
}
