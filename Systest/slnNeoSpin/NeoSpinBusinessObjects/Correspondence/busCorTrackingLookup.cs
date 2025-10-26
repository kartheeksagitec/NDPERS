#region Using directives

using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using Sagitec.CustomDataObjects;
using NeoSpin.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busCorTrackingLookup : busMainBase
    {

        private Collection<busCorTracking> _iclbLookupResult;
        public Collection<busCorTracking> iclbLookupResult
        {
            get
            {
                return _iclbLookupResult;
            }

            set
            {
                _iclbLookupResult = value;
            }
        }

        public void LoadCorTracking(DataTable adtbSearchResult)
        {
            _iclbLookupResult = GetCollection<busCorTracking>(adtbSearchResult, "icdoCorTracking");
        }

        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        //This is fired for every datarow found in the search result. 
        //Handle is returned to this method include the datarow and busObject being created
        //
        {
            busCorTracking lobjBusCorTracking = (busCorTracking)aobjBus;
            lobjBusCorTracking.ibusCorTemplates = new busCorTemplates { icdoCorTemplates = new cdoCorTemplates() };
            lobjBusCorTracking.ibusCorTemplates.icdoCorTemplates.LoadData(adtrRow);

            lobjBusCorTracking.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
            lobjBusCorTracking.ibusPerson.icdoPerson.LoadData(adtrRow);

            lobjBusCorTracking.ibusOrgContact = new busOrgContact { icdoOrgContact = new cdoOrgContact() };
            lobjBusCorTracking.ibusOrgContact.icdoOrgContact.LoadData(adtrRow);

            lobjBusCorTracking.ibusOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
            lobjBusCorTracking.ibusOrganization.icdoOrganization.LoadData(adtrRow);

            lobjBusCorTracking.ibusPlan = new busPlan { icdoPlan = new cdoPlan() };
            lobjBusCorTracking.ibusPlan.icdoPlan.LoadData(adtrRow);
        }
    }
}
