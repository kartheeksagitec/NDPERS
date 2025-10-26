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
    /// <summary>
    /// Class NeoSpin.BusinessObjects.busMASStatementFile:
    /// Inherited from busMASStatementFileGen, the class is used to customize the business object busMASStatementFileGen.
    /// </summary>
    [Serializable]
    public class busMASStatementFile : busMasStatementFileGen
    {
        public busMASSelection ibusMASSelection { get; set; }
        public void LoadMASSelection()
        {
            if (ibusMASSelection == null)
                ibusMASSelection = new busMASSelection();
            ibusMASSelection.FindMASSelection(icdoMasStatementFile.mas_selection_id);
        }
        public string report_name
        {
            get
            {
                if (ibusMASSelection == null)
                    LoadMASSelection();
                if (ibusMASSelection.ibusBatchRequest == null)
                    ibusMASSelection.LoadBatchRequest();
                if (ibusMASSelection.ibusBatchRequest.icdoMasBatchRequest.group_type_value == busConstant.GroupTypeNonRetired && icdoMasStatementFile.statement_type_value == "ONLN")
                    return "rptOnlineMemberStatement.rpt";
                else if (ibusMASSelection.ibusBatchRequest.icdoMasBatchRequest.group_type_value == busConstant.GroupTypeNonRetired && icdoMasStatementFile.statement_type_value == "SUMM")
                    return "rptSummaryMemberStatement.rpt";
                else if (ibusMASSelection.ibusBatchRequest.icdoMasBatchRequest.group_type_value == busConstant.GroupTypeRetired && icdoMasStatementFile.statement_type_value == "ONLN")
                    return "rptOnlineRetireeStatement.rpt";
                else if (ibusMASSelection.ibusBatchRequest.icdoMasBatchRequest.group_type_value == busConstant.GroupTypeRetired && icdoMasStatementFile.statement_type_value == "SUMM")
                    return "rptSummaryRetireeStatement.rpt";
                else
                    return string.Empty;
            }
        }
    }
}