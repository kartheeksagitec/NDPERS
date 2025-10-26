#region Using directives

using System;
using System.Collections.ObjectModel;
using System.Data;
using Sagitec.BusinessObjects;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;
#endregion
namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busDecrementFileOut : busFileBaseOut
    {
        public Collection<busPensionFile> iclbDecrementFile { get; set; }
        public void LoadDecrementFile(DataTable adtbDecrementFile)
        {
            iclbDecrementFile = new Collection<busPensionFile>();
            adtbDecrementFile = (DataTable)iarrParameters[0];
            istrPlanName = (string)iarrParameters[1];
            foreach (DataRow ldtrDecrementFile in adtbDecrementFile.Rows)
            {
                busPensionFile lobjDecrementFile = new busPensionFile { icdoActuaryFilePensionDetail = new  CustomDataObjects.cdoActuaryFilePensionDetail() };
                sqlFunction.LoadQueryResult(lobjDecrementFile.icdoActuaryFilePensionDetail, ldtrDecrementFile);
                if (!string.IsNullOrEmpty(lobjDecrementFile.icdoActuaryFilePensionDetail.previous_plan_participation_code_value))
                {
                    DataTable ldtbDataTable = iobjPassInfo.isrvDBCache.GetCodeDescription(337,
                      lobjDecrementFile.icdoActuaryFilePensionDetail.previous_plan_participation_code_value);
                    lobjDecrementFile.icdoActuaryFilePensionDetail.previous_plan_participation_value_formatted = ldtbDataTable.Rows.Count > 0 ? ldtbDataTable.Rows[0]["data2"].ToString() : string.Empty;
                }

                if (!string.IsNullOrEmpty(lobjDecrementFile.icdoActuaryFilePensionDetail.current_plan_participation_code_value))
                {
                    DataTable ldtbDataTable = iobjPassInfo.isrvDBCache.GetCodeDescription(337,
                      lobjDecrementFile.icdoActuaryFilePensionDetail.current_plan_participation_code_value);
                    lobjDecrementFile.icdoActuaryFilePensionDetail.current_plan_participation_value_formatted = ldtbDataTable.Rows.Count > 0 ? ldtbDataTable.Rows[0]["data2"].ToString() : string.Empty;
                }
                iclbDecrementFile.Add(lobjDecrementFile);
            }
        }

        public string istrPlanName { get; set; }

        public override void InitializeFile()
        {
            if (!string.IsNullOrEmpty(istrPlanName))
                istrFileName = "Actuarial_Decrement_File_" + istrPlanName + "_" + DateTime.Now.ToString(busConstant.DateFormat) + busConstant.FileFormattxt;
            else
                istrFileName = "Actuarial_Decrement_File_" + DateTime.Now.ToString(busConstant.DateFormat) + busConstant.FileFormattxt;
        }
    }
}
