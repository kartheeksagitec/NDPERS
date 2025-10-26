using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;
using Sagitec.CustomDataObjects;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using NeoSpin.DataObjects;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busASIEligibilityFileOut : busFileBaseOut
    {
        private Collection<busASIEligibilityFile> _iclbASIEligibilityFile;
        public Collection<busASIEligibilityFile> iclbASIEligibilityFile
        {
            get { return _iclbASIEligibilityFile; }
            set { _iclbASIEligibilityFile = value; }
        }

        public override void InitializeFile()
        {
            istrFileName = "NDPERS_ASI_Eligibility_" + DateTime.Now.ToString(busConstant.DateTimeFormatYYMMDD) + ".txt";
        }

        public void LoadASIEligibility(DataTable adtbASIEligibility)
        {
            adtbASIEligibility = new DataTable();
            _iclbASIEligibilityFile = new Collection<busASIEligibilityFile>();
            adtbASIEligibility = busBase.Select("cdoPerson.CreateASIEligibilityFile", new object[] {});
            foreach (DataRow dr in adtbASIEligibility.Rows)
            {
                busASIEligibilityFile lobjASIEligibility = new busASIEligibilityFile();
                if (!Convert.IsDBNull(dr["person_id"]))
                    lobjASIEligibility.person_id = Convert.ToInt32(dr["person_id"]);

                if (!Convert.IsDBNull(dr["division_code"]))
                    lobjASIEligibility.division_code = Convert.ToInt32(dr["division_code"]);

                if (!Convert.IsDBNull(dr["last_name"]))
                    lobjASIEligibility.last_name = dr["last_name"].ToString();

                if (!Convert.IsDBNull(dr["first_name"]))
                    lobjASIEligibility.first_name = dr["first_name"].ToString();

                if (!Convert.IsDBNull(dr["middle_name"]))
                    lobjASIEligibility.middle_name = dr["middle_name"].ToString();

                if (!Convert.IsDBNull(dr["addr_line_1"]))
                    lobjASIEligibility.address_line_1 = dr["addr_line_1"].ToString();

                if (!Convert.IsDBNull(dr["addr_line_2"]))
                    lobjASIEligibility.address_line_2 = dr["addr_line_2"].ToString();

                if (!Convert.IsDBNull(dr["addr_city"]))
                    lobjASIEligibility.city = dr["addr_city"].ToString();

                if (!Convert.IsDBNull(dr["addr_state_value"]))
                    lobjASIEligibility.addr_state = dr["addr_state_value"].ToString();

                if (!Convert.IsDBNull(dr["addr_zip_code"]))
                    lobjASIEligibility.zip_code = dr["addr_zip_code"].ToString() + dr["addr_zip_4_code"].ToString();

                if (!Convert.IsDBNull(dr["country"]))
                    lobjASIEligibility.addr_country = dr["country"].ToString();

                if (!Convert.IsDBNull(dr["foreign_province"]))
                    lobjASIEligibility.foreign_province = dr["foreign_province"].ToString();

                if (!Convert.IsDBNull(dr["foreign_postal_code"]))
                    lobjASIEligibility.foreign_postal_code = dr["foreign_postal_code"].ToString();

                if (!Convert.IsDBNull(dr["rhic_amount"]))
                    lobjASIEligibility.rhic_amount = Convert.ToDecimal(dr["rhic_amount"]);

                if (!Convert.IsDBNull(dr["start_date"]))
                    lobjASIEligibility.rhic_start_date = Convert.ToDateTime(dr["start_date"]);

                //PIR 18271 
                if (!Convert.IsDBNull(dr["routing_no"]))
                    lobjASIEligibility.routing_no = (dr["routing_no"]).ToString().PadLeft(9, '0');

                if (!Convert.IsDBNull(dr["account_number"]))
                    lobjASIEligibility.account_number = dr["account_number"].ToString();

                if (!Convert.IsDBNull(dr["bank_account_type_value"]))
                    lobjASIEligibility.bank_account_type_value = dr["bank_account_type_value"].ToString();
					
				if (!Convert.IsDBNull(dr["DATE_OF_DEATH"]))
                    lobjASIEligibility.date_of_death = Convert.ToDateTime(dr["DATE_OF_DEATH"]); //PIR 19290

                _iclbASIEligibilityFile.Add(lobjASIEligibility);
            }
        }

        public override void FinalizeFile()
        {
            DBFunction.DBNonQuery("cdoPerson.UpdateASISentFlag", new object[] { }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            base.FinalizeFile();
        }
    }
}
