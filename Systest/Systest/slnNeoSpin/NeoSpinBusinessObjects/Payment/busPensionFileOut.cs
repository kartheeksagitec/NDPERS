#region Using directives

using System;
using System.Collections.ObjectModel;
using System.Data;
using Sagitec.BusinessObjects;
using Sagitec.DBUtility;
using NeoSpin.CustomDataObjects;

#endregion
namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busPensionFileOut : busFileBaseOut
    {
        public Collection<busPensionFile> iclbPensionFile { get; set; }
        public void LoadPensionFile(DataTable adtbPensionFile)
        {
            iclbPensionFile = new Collection<busPensionFile>();
            adtbPensionFile = (DataTable)iarrParameters[0];
            istrPlanName = (string)iarrParameters[1];
            foreach (DataRow ldtrPensionFileDate in adtbPensionFile.Rows)
            {
                busPensionFile lobjPensionFile = new busPensionFile { icdoActuaryFilePensionDetail = new cdoActuaryFilePensionDetail() };
                sqlFunction.LoadQueryResult(lobjPensionFile.icdoActuaryFilePensionDetail, ldtrPensionFileDate);
                lobjPensionFile.icdoActuaryFilePensionDetail.employment_type_description = lobjPensionFile.icdoActuaryFilePensionDetail.employment_type_value != null ?
                                iobjPassInfo.isrvDBCache.GetCodeDescriptionString(313, lobjPensionFile.icdoActuaryFilePensionDetail.employment_type_value) : string.Empty;
                lobjPensionFile.icdoActuaryFilePensionDetail.hourly_description = lobjPensionFile.icdoActuaryFilePensionDetail.hourly_value != null ?
                                 iobjPassInfo.isrvDBCache.GetCodeDescriptionString(311, lobjPensionFile.icdoActuaryFilePensionDetail.hourly_value) : string.Empty;
                lobjPensionFile.icdoActuaryFilePensionDetail.seasonal_description = lobjPensionFile.icdoActuaryFilePensionDetail.seasonal_value != null ?
                                iobjPassInfo.isrvDBCache.GetCodeDescriptionString(312, lobjPensionFile.icdoActuaryFilePensionDetail.seasonal_value) : string.Empty;
                if (!string.IsNullOrEmpty(lobjPensionFile.icdoActuaryFilePensionDetail.plan_participation_status_value))
                {
                    DataTable ldtbDataTable = iobjPassInfo.isrvDBCache.GetCodeDescription(337,
                      lobjPensionFile.icdoActuaryFilePensionDetail.plan_participation_status_value);
                    lobjPensionFile.icdoActuaryFilePensionDetail.plan_participation_value_formatted = ldtbDataTable.Rows.Count > 0 ? ldtbDataTable.Rows[0]["data2"].ToString() : string.Empty;
                }              
                if (lobjPensionFile.icdoActuaryFilePensionDetail.MemberGender == busConstant.GenderTypeMale)
                {
                    lobjPensionFile.icdoActuaryFilePensionDetail.MemberGenderCode = "M";
                }
                else if (lobjPensionFile.icdoActuaryFilePensionDetail.MemberGender == busConstant.GenderTypeFemale)
                {
                    lobjPensionFile.icdoActuaryFilePensionDetail.MemberGenderCode = "F";
                }
                if (lobjPensionFile.icdoActuaryFilePensionDetail.SpouseGender == busConstant.GenderTypeMale)
                {
                    lobjPensionFile.icdoActuaryFilePensionDetail.SpouseGenderCode = "M";
                }
                else if (lobjPensionFile.icdoActuaryFilePensionDetail.SpouseGender == busConstant.GenderTypeFemale)
                {
                    lobjPensionFile.icdoActuaryFilePensionDetail.SpouseGenderCode = "F";
                }
                iclbPensionFile.Add(lobjPensionFile);
            }
        }

        public string istrPlanName { get; set; }

        public override void InitializeFile()
        {
            if (!string.IsNullOrEmpty(istrPlanName))
                istrFileName = "Actuarial_Pension_File_" + istrPlanName + "_" + DateTime.Now.ToString(busConstant.DateFormat) + busConstant.FileFormattxt;
            else
                istrFileName = "Actuarial_Pension_File_" + DateTime.Now.ToString(busConstant.DateFormat) + busConstant.FileFormattxt;
        }
    }
}
