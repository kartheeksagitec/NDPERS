using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sagitec.BusinessObjects;
using System.Data;
using System.Collections.ObjectModel;
using NeoSpin.CustomDataObjects;
using NeoSpin.DataObjects;
using Sagitec.CustomDataObjects;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busMSSPersonAccountDefCompProviderLookup : busMainBase
    {
        private Collection<busOrgContact> _icolMSSProviderContact;
        public Collection<busOrgContact> icolMSSProviderContact
        {
            get
            {
                return _icolMSSProviderContact;
            }

            set
            {
                _icolMSSProviderContact = value;
            }
        }

        public void LoadMSSProviderContact(DataTable adtbSearchResult)
        {
            _icolMSSProviderContact = GetCollection<busOrgContact>(adtbSearchResult, "icdoOrgContact");
        }

        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            busOrgContact lobjOrgContact = (busOrgContact)aobjBus;
            lobjOrgContact.ibusContact = new busContact { icdoContact = new cdoContact() };
            lobjOrgContact.ibusContact.icdoContact.LoadData(adtrRow);
            base.LoadOtherObjects(adtrRow, aobjBus);
        }
    }
}
