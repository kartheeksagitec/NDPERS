#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using Sagitec.CustomDataObjects;
using NeoSpin.CustomDataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busPlanBase : busExtendBase
    {
        public busPlanBase()
        {
        }
        private busPlan _ibusPlan;
        public busPlan ibusPlan
        {
            get
            {
                return _ibusPlan;
            }

            set
            {
                _ibusPlan = value;
            }
        }
        private busOrganization _ibusOrganization;
        public busOrganization ibusOrganization
        {
            get
            {
                return _ibusOrganization;
            }

            set
            {
                _ibusOrganization = value;
            }
        }       

        /// <summary>
        /// This method is used only for Group Health / Medicare Plan. It uses the Code ID 345
        /// </summary>
        /// <returns></returns>
        public Collection<cdoCodeValue> LoadMemberTypeByPlan()
        {
            DataTable ldtbList = iobjPassInfo.isrvDBCache.GetCodeValues(345, _ibusPlan.icdoPlan.plan_id.ToString(), null, null);
            Collection<cdoCodeValue> lclcCodeValue = Sagitec.DataObjects.doBase.GetCollection<cdoCodeValue>(ldtbList);
            return lclcCodeValue;
        }
        // PIR - 590 - Level of Coverage Drop down List values
        public Collection<cdoCodeValue> LoadLevelOfCoverageByPlan()
        {
            DataTable ldtbList = iobjPassInfo.isrvDBCache.GetCodeValues(408, _ibusPlan.icdoPlan.plan_id.ToString(), null, null);
            Collection<cdoCodeValue> lclcCodeValue = Sagitec.DataObjects.doBase.GetCollection<cdoCodeValue>(ldtbList);
            return lclcCodeValue;
        }  
    }
}
