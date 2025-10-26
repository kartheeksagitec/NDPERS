using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busEmpowerDCFile
    {

        private string _istrSSN;
        public string istrSSN
        {
            get { return _istrSSN; }
            set { _istrSSN = value; }
        }

        public string idecSUMEEPOSTAMOUNT { get; set; }
        public string idecSUMEEPREAMOUNT { get; set; }

        public string idecSUMERPREAMOUNT { get; set; }

        public DateTime idtNextRunDate { get; set; }

        public string istrFillerForFile
        {
            get
            {
                return " ";
            }
        }

    }
}
