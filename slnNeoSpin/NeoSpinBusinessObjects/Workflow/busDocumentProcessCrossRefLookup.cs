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
	public class busDocumentProcessCrossRefLookup : busDocumentProcessCrossRefLookupGen
	{
        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            if (aobjBus is busDocumentProcessCrossref)
            {
                busDocumentProcessCrossref lbusDocumentProcessCrossref = (busDocumentProcessCrossref) aobjBus;

                lbusDocumentProcessCrossref.ibusDocument = new busDocument();
                lbusDocumentProcessCrossref.ibusDocument.icdoDocument = new cdoDocument();
                lbusDocumentProcessCrossref.ibusDocument.icdoDocument.LoadData(adtrRow);
                
                lbusDocumentProcessCrossref.ibusProcess = new busProcess();
                lbusDocumentProcessCrossref.ibusProcess.icdoProcess = new cdoProcess();
                lbusDocumentProcessCrossref.ibusProcess.icdoProcess.LoadData(adtrRow);
            }
        }
	}
}
