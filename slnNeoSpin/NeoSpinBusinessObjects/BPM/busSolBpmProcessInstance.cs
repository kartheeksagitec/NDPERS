using NeoBase.BPM;
//using NeoSpin.Communication;
using NeoSpin.DataObjects;
using Sagitec.Bpm;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;

namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// Class NeoSpin.BusinessObjects.busBpmProcessInstance:
    /// Inherited from busBpmProcessInstanceGen, the class is used to customize the business object busBpmProcessInstanceGen.
    /// </summary>
    [Serializable]
    public class busSolBpmProcessInstance : busNeobaseBpmProcessInstance //busBpmProcessInstance
    {
        public int iintUserSerialid
        {
            get
            {
                return iobjPassInfo.iintUserSerialID;
            }
        }

        public string istrPersonOrOrganizationName
        {
            get
            {
                if (ibusBpmCaseInstance.icdoBpmCaseInstance.person_id > 0)
                {
                    if (ibusBpmCaseInstance.ibusPerson == null)
                    {
                        ibusPerson = new busPerson();
                        ((busPerson)ibusPerson).FindByPrimaryKey(ibusBpmCaseInstance.icdoBpmCaseInstance.person_id);
                    }
                    return ((busPerson)ibusPerson).icdoPerson.FullName;
                }
                else if (ibusBpmCaseInstance.icdoBpmCaseInstance.org_id > 0)
                {
                    if (ibusBpmCaseInstance.ibusOrganization == null)
                    {
                        ibusOrganization = new busOrganization();
                        ((busOrganization)ibusOrganization).FindByPrimaryKey(ibusBpmCaseInstance.icdoBpmCaseInstance.org_id);
                    }
                    return ((busOrganization)ibusOrganization).icdoOrganization.org_name;
                }
                return "";
            }
        }
        public string istrOrgCode { get; set; }
    }
}
