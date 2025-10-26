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
using System.Linq;
using System.Linq.Expressions;

#endregion

namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// Class NeoSpin.BusinessObjects.busMASSelection:
    /// Inherited from busMASSelectionGen, the class is used to customize the business object busMASSelectionGen.
    /// </summary>
    [Serializable]
    public class busMASSelection : busMASSelectionGen
    {
        public busMASBatchRequest ibusBatchRequest { get; set; }

        public void LoadBatchRequest()
        {
            if (ibusBatchRequest.IsNull())
                ibusBatchRequest = new busMASBatchRequest { icdoMasBatchRequest = new cdoMasBatchRequest() };
            ibusBatchRequest.FindMASBatchRequest(icdoMasSelection.mas_batch_request_id);
        }

        public byte[] CreateMASReport(string astrReportName, string astrReportGNPath = busConstant.ReportPath)
        {
            byte[] lbyteFile = null;
            FetchAllReportData(icdoMasSelection.mas_batch_request_id, icdoMasSelection.mas_selection_id);
            lbyteFile = CreateDynamicReport(astrReportName, FilterReportData(icdoMasSelection.mas_selection_id), string.Empty, astrReportGNPath);
            return lbyteFile;
        }

        //FW Upgrade :: wfmDefault.aspx.cs file btn_OpenPDF conversion for the file htxMSSAnnualStatementMaintenance.xml only 
        public string CreateMASReportPath(string astrReportName, string astrReportGNPath = busConstant.ReportPath)
        {
            FetchAllReportData(icdoMasSelection.mas_batch_request_id, icdoMasSelection.mas_selection_id);
            return CreateDynamicReportPath(astrReportName, FilterReportData(icdoMasSelection.mas_selection_id), string.Empty, astrReportGNPath);
        }
        public byte[] CreateAnnualRetireeStatement(string astrReportName, string astrReportGNPath = busConstant.ReportPath)
        {
            byte[] lbyteFile = null;
            busRetireeAnnualStatement lobjRetireeAnnualStatement = new busRetireeAnnualStatement();
            lobjRetireeAnnualStatement.ibusCurrentRequest = ibusBatchRequest;
            lobjRetireeAnnualStatement.CreateRetireeAnnualStatements();
            lobjRetireeAnnualStatement.FetchAllRetireeDataBySelection(icdoMasSelection.mas_selection_id);
            lbyteFile = CreateDynamicReport(astrReportName, lobjRetireeAnnualStatement.CreateMASReport(icdoMasSelection.person_id,lobjRetireeAnnualStatement.idtEffectiveDate), string.Empty, astrReportGNPath);
            return lbyteFile;
        }

        //FW Upgrade :: wfmDefault.aspx.cs file btn_OpenPDF conversion for the file htxMSSAnnualStatementMaintenance.xml only 
        public string CreateAnnualRetireeStatementPath(string astrReportName, string astrReportGNPath = busConstant.ReportPath)
        {
            busRetireeAnnualStatement lobjRetireeAnnualStatement = new busRetireeAnnualStatement();
            lobjRetireeAnnualStatement.ibusCurrentRequest = ibusBatchRequest;
            lobjRetireeAnnualStatement.CreateRetireeAnnualStatements();
            lobjRetireeAnnualStatement.FetchAllRetireeDataBySelection(icdoMasSelection.mas_selection_id);
            return CreateDynamicReportPath(astrReportName, lobjRetireeAnnualStatement.CreateMASReport(icdoMasSelection.person_id, lobjRetireeAnnualStatement.idtEffectiveDate), string.Empty, astrReportGNPath);
        }

        public Collection<busMASPerson> iclbPerson { get; set; }

        public void LoadMasPerson()
        {
            if (iclbPerson.IsNull()) iclbPerson = new Collection<busMASPerson>();
            DataTable ldtbList = Select<cdoMasPerson>(
                                new string[1] { "MAS_SELECTION_ID" },
                                new object[1] { icdoMasSelection.mas_selection_id }, null, null);
            iclbPerson = GetCollection<busMASPerson>(ldtbList, "icdoMasPerson");
        }

        public Collection<busMASStatementFile> iclbStatementFile { get; set; }

        public void LoadMasStatementFile()
        {
            if (iclbStatementFile.IsNull()) iclbStatementFile = new Collection<busMASStatementFile>();
            DataTable ldtbList = Select<cdoMasStatementFile>(
                                new string[1] { "MAS_SELECTION_ID" },
                                new object[1] { icdoMasSelection.mas_selection_id }, null, null);
            iclbStatementFile = GetCollection<busMASStatementFile>(ldtbList, "icdoMasStatementFile");
        }
    }
}
