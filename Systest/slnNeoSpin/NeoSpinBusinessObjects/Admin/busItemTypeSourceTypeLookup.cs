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
	public partial class busItemTypeSourceTypeLookup 
	{
        private Collection<busItemTypeSourceTypeCrossref> _iclbItemTypeSourceType;
        public Collection<busItemTypeSourceTypeCrossref> iclbItemTypeSourceType
        {
            get { return _iclbItemTypeSourceType; }
            set { _iclbItemTypeSourceType = value; }
        }

        public void LoadItemTypeSourceType(DataTable adtbSearchResult)
        {
            _iclbItemTypeSourceType = GetCollection<busItemTypeSourceTypeCrossref>(adtbSearchResult, "icdoItemTypeSourceTypeCrossref");
        }

	}
}
