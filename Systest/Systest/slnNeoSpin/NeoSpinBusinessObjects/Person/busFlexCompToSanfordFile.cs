
namespace NeoSpin.BusinessObjects
{
    public class busFlexCompToSanfordFile
    {
        private string _ssn;
        public string ssn
        {
            get
            {
                return _ssn;
            }
            set { _ssn = value; }
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

        private string _middle_name;
        public string middle_name
        {
            get
            {
                if (string.IsNullOrEmpty(_middle_name))
                    return _middle_name;
                if (_middle_name.Length > 50)
                    return _middle_name.Trim().ToUpper().Substring(0, 50);
                return _middle_name.Trim().ToUpper();
            }
            set { _middle_name = value; }
        }
    }
}
