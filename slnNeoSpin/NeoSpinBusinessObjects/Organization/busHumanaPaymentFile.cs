using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busHumunaPaymentFile
    {
        private string _first_name;
        public string first_name
        {
            get
            {
                if (string.IsNullOrEmpty(_first_name))
                    return _first_name;
                if (_first_name.Length > 50)
                    return _first_name.Trim().ToUpper().Substring(0, 50);
                return _first_name.Trim().ToUpper();
            }
            set { _first_name = value; }
        }

        private string _last_name;
        public string last_name
        {
            get
            {
                if (string.IsNullOrEmpty(_last_name))
                    return _last_name;
                if (_last_name.Length > 50)
                    return _last_name.Trim().ToUpper().Substring(0, 50);
                return _last_name.Trim().ToUpper();
            }
            set { _last_name = value; }
        }

        public decimal amount { get; set; }
        public DateTime billing_period { get; set; }
        public string medicare_id_number { get; set; }

        public string ssn { get; set; }
    }
}
