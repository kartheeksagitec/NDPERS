using System;
using System.Collections.ObjectModel;
using System.Data;
using Sagitec.BusinessObjects;



namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busHumunaPaymentFileOut : busFileBaseOut
    {
        public Collection<busHumunaPaymentFile> iclbHumanaPaymentFile { get; set; }

        public override void InitializeFile()
        {
            istrFileName = "NDPERS_" + DateTime.Now.ToString(busConstant.DateFormat) + "_700100" +".csv";
        }

        public void LoadHumanaProviderData(DataTable adtbHumanaPaymentMedicare)
        {
            DataTable ldtbHumanaPaymentMedicare = new DataTable();
            ldtbHumanaPaymentMedicare = (DataTable)iarrParameters[0];
            iclbHumanaPaymentFile = new Collection<busHumunaPaymentFile>();

            DateTime ldteSystemBatchDate = busGlobalFunctions.GetSysManagementBatchDate();

            foreach (DataRow dr in ldtbHumanaPaymentMedicare.Rows)
            {
                busHumunaPaymentFile lobjHumanaPaymentFile = new busHumunaPaymentFile();
                
                if (!Convert.IsDBNull(dr["first_name"]))
                    lobjHumanaPaymentFile.first_name = Convert.ToString(dr["first_name"]);

                if (!Convert.IsDBNull(dr["last_name"]))
                    lobjHumanaPaymentFile.last_name = Convert.ToString(dr["last_name"]);

                if (!Convert.IsDBNull(dr["billing_period"]))
                    lobjHumanaPaymentFile.billing_period = Convert.ToDateTime(dr["billing_period"]);
                
                if (!Convert.IsDBNull(dr["medicare_id_number"]))
                    lobjHumanaPaymentFile.medicare_id_number = Convert.ToString(dr["medicare_id_number"]);

                if (!Convert.IsDBNull(dr["ssn"]))
                    lobjHumanaPaymentFile.ssn = Convert.ToString(dr["ssn"]);

                if (!Convert.IsDBNull(dr["amount"]))
                    lobjHumanaPaymentFile.amount = Convert.ToDecimal(dr["amount"]);

                iclbHumanaPaymentFile.Add(lobjHumanaPaymentFile);
            }
        }
    }
}
