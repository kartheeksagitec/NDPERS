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

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busFileNetImages : busExtendBase
    {       
        private busPerson _ibusPerson;
        public busPerson ibusPerson
        {
            get { return _ibusPerson; }
            set { _ibusPerson = value; }
        }
      
        private busOrganization _ibusOrganization;
        public busOrganization ibusOrganization
        {
            get { return _ibusOrganization; }
            set { _ibusOrganization = value; }
        }       
    }
}
