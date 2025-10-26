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
using System.Collections;
using Sagitec.Common;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busFormListing : busExtendBase
    {

        public Collection<cdoCodeValue> iclbFormListingItems { get; set; }
        public void LoadFormListingItems()
        {
            if (iclbFormListingItems.IsNull())
                iclbFormListingItems = new Collection<cdoCodeValue>();

            DataTable ldtbList = iobjPassInfo.isrvDBCache.GetCodeValues(1028);

            DataTable ldtbMSSForms = ldtbList.AsEnumerable()
                                            .Where(o => o.Field<string>("data2") == busConstant.MSSPortal || o.Field<string>("data2") == busConstant.BothPortal)
                                            .AsDataTable();

            iclbFormListingItems = cdoCodeValue.GetCollection<cdoCodeValue>(ldtbMSSForms);
        }
    }
}