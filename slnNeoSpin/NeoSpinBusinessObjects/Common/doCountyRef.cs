#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sagitec.Common;
using Sagitec.DataObjects;
#endregion

namespace NeoSpin.DataObjects
{
    [Serializable]
    public class doCountyRef : doBase
    {
         
         public doCountyRef() : base()
         {
         }
		private int _county_ref_id;
		public int county_ref_id
		{
			get
			{
				return _county_ref_id;
			}

			set
			{
				_county_ref_id = value;
			}
		}

		private string _city;
		public string city
		{
			get
			{
				return _city;
			}

			set
			{
				_city = value;
			}
		}

		private string _county;
		public string county
		{
			get
			{
				return _county;
			}

			set
			{
				_county = value;
			}
		}

        private string _zip_code;
        public string zip_code
        {
            get
            {
                return _zip_code;
            }

            set
            {
                _zip_code = value;
            }
        }
    }
}

