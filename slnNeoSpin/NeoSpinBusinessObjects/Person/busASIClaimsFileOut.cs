using NeoSpin.CustomDataObjects;
using NeoSpin.DataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.CustomDataObjects;
using Sagitec.DBUtility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busASIClaimsFileOut : busFileBaseOut
    {
        DateTime ldtCurrentDateTime;
        private Collection<busASIClaimsFile> _iclbASIClaimsFile;
        public Collection<busASIClaimsFile> iclbASIClaimsFile
        {
            get { return _iclbASIClaimsFile; }
            set { _iclbASIClaimsFile = value; }
        }

        public override void InitializeFile()
        {
            istrFileName = "NDPERS_ASI_Claims_" + DateTime.Now.ToString(busConstant.DateTimeFormatYYMMDD) + ".txt";
        }

        public void LoadASIClaims(DataTable adtbASIClaims)
        {
            adtbASIClaims = new DataTable();
            _iclbASIClaimsFile = new Collection<busASIClaimsFile>();
            //Used same datetime for selection & update query.
            ldtCurrentDateTime = DateTime.Now;

            adtbASIClaims = busBase.Select("cdoPerson.CreateASIClaimsFile", new object[1] { ldtCurrentDateTime });
            foreach (DataRow dr in adtbASIClaims.Rows)
            {
                busASIClaimsFile lobjASIClaims = new busASIClaimsFile();
                if (!Convert.IsDBNull(dr["person_id"]))
                    lobjASIClaims.person_id = Convert.ToInt32(dr["person_id"]);

                if (!Convert.IsDBNull(dr["last_name"]))
                    lobjASIClaims.last_name = dr["last_name"].ToString();

                if (!Convert.IsDBNull(dr["first_name"]))
                    lobjASIClaims.first_name = dr["first_name"].ToString();

                if (!Convert.IsDBNull(dr["middle_name"]))
                    lobjASIClaims.middle_name = dr["middle_name"].ToString();

                if (!Convert.IsDBNull(dr["total_premium"]))
                    lobjASIClaims.total_premium = Convert.ToDecimal(dr["total_premium"]);

                if (!Convert.IsDBNull(dr["billing_month_and_year"]))
                    lobjASIClaims.billing_month_and_year = Convert.ToDateTime(dr["billing_month_and_year"]);

                _iclbASIClaimsFile.Add(lobjASIClaims);
            }
        }

        //Update the flag so that the file wont pick these records next time.
        public override void FinalizeFile()
        {
            DBFunction.DBNonQuery("cdoPerson.Update_RHIC_SENT_Flag", new object[1] { ldtCurrentDateTime }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            base.FinalizeFile();
        }
    }
}
