#region Using directives

using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using NeoSpin.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public partial class busContactLookup : busMainBase
    {

        private Collection<busContact> _icolContact;
        public Collection<busContact> icolContact
        {
            get
            {
                return _icolContact;
            }

            set
            {
                _icolContact = value;
            }
        }

        public void LoadContact(DataTable adtbSearchResult)
        {
            _icolContact = GetCollection<busContact>(adtbSearchResult, "icdoContact");
        }
        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        //This is fired for every datarow found in the search result. 
        //Handle is returned to this method include the datarow and busObject being created
        //
        {
            busContact lobjbusContact = (busContact)aobjBus;
            lobjbusContact.LoadContactPrimaryAddress();
        }
    }
}
