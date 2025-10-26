using System;
using System.Collections.ObjectModel;
using System.Data;
using Sagitec.BusinessObjects;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busFlexCompToSanfordFileOut : busFileBaseOut
    {
        private Collection<busFlexCompToSanfordFile> _iclbFlexCompToSanfordFile;
        public Collection<busFlexCompToSanfordFile> iclbFlexCompToSanfordFile
        {
            get { return _iclbFlexCompToSanfordFile; }
            set { _iclbFlexCompToSanfordFile = value; }
        }

        public override void InitializeFile()
        {
            istrFileName = "FlexComp_" + DateTime.Now.ToString(busConstant.DateTimeFormatYYYY_MM_DD) + ".csv";
        }

        public void LoadFlexCompToSanford(DataTable adtbFlexCompToSanford)
        {
            adtbFlexCompToSanford = new DataTable();
            _iclbFlexCompToSanfordFile = new Collection<busFlexCompToSanfordFile>();
            adtbFlexCompToSanford = busBase.Select("entPersonAccountFlexComp.fleFlexCompFileToSanford", new object[] { });
            
            foreach (DataRow dr in adtbFlexCompToSanford.Rows)
            {
                busFlexCompToSanfordFile lobjFlexCompToSanford = new busFlexCompToSanfordFile();
                if (!Convert.IsDBNull(dr["SSN"]))
                    lobjFlexCompToSanford.ssn = Convert.ToString(dr["SSN"]);

                if (!Convert.IsDBNull(dr["FIRST_NAME"]))
                    lobjFlexCompToSanford.first_name = Convert.ToString(dr["FIRST_NAME"]);

                if (!Convert.IsDBNull(dr["MIDDLE_NAME"]))
                    lobjFlexCompToSanford.middle_name = Convert.ToString(dr["MIDDLE_NAME"]);


                if (!Convert.IsDBNull(dr["LAST_NAME"]))
                    lobjFlexCompToSanford.last_name = Convert.ToString(dr["LAST_NAME"]);

                _iclbFlexCompToSanfordFile.Add(lobjFlexCompToSanford);
            }
        }
        
    }
}
