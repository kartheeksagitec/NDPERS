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
    public class busMergeEmployerDetailLookup : busMergeEmployerDetailLookupGen
    {
        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            if (aobjBus is busMergeEmployerDetail)
            {
                busMergeEmployerDetail lobjMergeEmployerDetail = (busMergeEmployerDetail)aobjBus;
                lobjMergeEmployerDetail.ibusMergeEmployerHeader = new busMergeEmployerHeader();
                lobjMergeEmployerDetail.ibusMergeEmployerHeader.icdoMergeEmployerHeader = new cdoMergeEmployerHeader();
                lobjMergeEmployerDetail.ibusMergeEmployerHeader.icdoMergeEmployerHeader.LoadData(adtrRow);
                lobjMergeEmployerDetail.ibusMergeEmployerHeader.LoadFromEmployer();
                lobjMergeEmployerDetail.ibusMergeEmployerHeader.LoadToEmployer();
                lobjMergeEmployerDetail.LoadPerson();
            }
        }
    }
}