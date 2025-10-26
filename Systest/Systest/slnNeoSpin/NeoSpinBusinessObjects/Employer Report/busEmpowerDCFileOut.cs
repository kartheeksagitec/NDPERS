using System;
using System.Collections.Generic;
using System.Text;
using NeoSpin.BusinessObjects;
using Sagitec.BusinessObjects;
using System.Collections;
using System.Collections.ObjectModel;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.CorBuilder;
using Sagitec.DataObjects;
using System.Data;
using Sagitec.CustomDataObjects;
using NeoSpin.CustomDataObjects;
using System.Linq;
using System.Linq.Expressions;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busEmpowerDCFileOut : busFileBaseOut
    {
        public busEmpowerDCFileOut()
        {
        }
        private Collection<busEmpowerDCFile> _iclbDCProvider;

        public Collection<busEmpowerDCFile> iclbDCProvider
        {
            get { return _iclbDCProvider; }
            set { _iclbDCProvider = value; }
        }
        public string istrTotalCount { get; set; }
        public string istrTotalContribution { get; set; }

        public decimal idecTotalContribution { get; set; }

        public DateTime ldtBatchDate { get; set; }

        public void LoadEmpowerContributions(DataTable adtbEmpowerContributions)
        {
            adtbEmpowerContributions = (DataTable)iarrParameters[0];

            iclbDCProvider = new Collection<busEmpowerDCFile>();

            if (adtbEmpowerContributions.Rows.Count > 0)
            {
                foreach (DataRow dr in adtbEmpowerContributions.Rows)
                {
                    busEmpowerDCFile lobjDCRecords = new busEmpowerDCFile();

                    if (!Convert.IsDBNull(dr["ssn"]))
                        lobjDCRecords.istrSSN = dr["ssn"].ToString();

                    if (!Convert.IsDBNull(dr["SUMEEPOSTAMOUNT"]))
                        lobjDCRecords.idecSUMEEPOSTAMOUNT = GetAmount(Convert.ToDecimal(dr["SUMEEPOSTAMOUNT"]));

                    if (!Convert.IsDBNull(dr["SUMEEPREAMOUNT"]))
                        lobjDCRecords.idecSUMEEPREAMOUNT = GetAmount(Convert.ToDecimal(dr["SUMEEPREAMOUNT"]));

                    if (!Convert.IsDBNull(dr["SUMERPREAMOUNT"]))
                        lobjDCRecords.idecSUMERPREAMOUNT = GetAmount(Convert.ToDecimal(dr["SUMERPREAMOUNT"]));

                    lobjDCRecords.idtNextRunDate = (DateTime)iarrParameters[1];

                    if (!Convert.IsDBNull(dr["TOTALSUM"]))
                    {
                        //istrTotalContribution += GetAmount(Convert.ToDecimal(dr["TOTALSUM"]));

                        idecTotalContribution += Convert.ToDecimal(dr["TOTALSUM"]);
                        istrTotalContribution = GetAmount(Convert.ToDecimal(idecTotalContribution));

                    }
                    iclbDCProvider.Add(lobjDCRecords);

                }
                istrTotalCount = ((iclbDCProvider.Count()).ToString().PadLeft(9, '0'));            
            }

        }

        public string GetAmount(decimal adecAmount)
        {
            decimal ldclTemp = 0.0M;
            string lstrDisplayAmount;
            if (adecAmount > 0)
            {
                ldclTemp = Convert.ToDecimal(adecAmount) * (100);
                lstrDisplayAmount = ldclTemp.ToString("#");
                lstrDisplayAmount = lstrDisplayAmount.PadLeft(9, '0');
            }
            else
            {
                ldclTemp = adecAmount * (-100);
                lstrDisplayAmount = ldclTemp.ToString("#");
                lstrDisplayAmount = lstrDisplayAmount.PadLeft(9, '0');

            }

            return lstrDisplayAmount;
        }

        public override void InitializeFile()
        {
            ldtBatchDate = (DateTime)iarrParameters[1];

            if (iclbDCProvider != null && iclbDCProvider.Count > 0 && idecTotalContribution > 0)
                istrFileName = "100456-01cash." + ldtBatchDate.ToString(busConstant.DateFormatD8) + "_Positive" + busConstant.FileFormattxt;
            else if (iclbDCProvider != null && iclbDCProvider.Count > 0 && idecTotalContribution < 0)
                istrFileName = "100456-01cash." + ldtBatchDate.ToString(busConstant.DateFormatD8) + "_Negative" + busConstant.FileFormattxt;
            else
                istrFileName = "100456-01cash." + ldtBatchDate.ToString(busConstant.DateFormatD8) + busConstant.FileFormattxt;
        }

        public string istrFillerForFile
        {
            get
            {
                return " ";
            }
        }
    }
}
