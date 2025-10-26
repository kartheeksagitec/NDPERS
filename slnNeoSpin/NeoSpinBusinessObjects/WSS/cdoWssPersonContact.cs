#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using NeoSpin.DataObjects;
using NeoSpin.BusinessObjects;

#endregion

namespace NeoSpin.CustomDataObjects
{
	/// <summary>
	/// Class NeoSpin.CustomDataObjects.cdoWssPersonContact:
	/// Inherited from doWssPersonContact, the class is used to customize the database object doWssPersonContact.
	/// </summary>
    [Serializable]
	public class cdoWssPersonContact : doWssPersonContact
	{
		public cdoWssPersonContact() : base()
		{
		}
        public string contact_org_code { get; set; }

        public string contact_first_name { get; set; } // PIR 10071

        public string contact_middle_name { get; set; } // PIR 10071

        public string contact_last_name { get; set; } // PIR 10071

        public string ReenterContactSSN { get; set; }

        // PIR 10071
        public String FullName
        {
            get
            {
                string lstrName = String.Empty;
                if (!String.IsNullOrEmpty(contact_first_name))
                {
                    lstrName = contact_first_name.Trim();
                }
                if (!String.IsNullOrEmpty(contact_middle_name))
                {
                    lstrName += " " + contact_middle_name.Trim();
                }
                if (!String.IsNullOrEmpty(contact_last_name))
                    lstrName += " " + contact_last_name.Trim();

                return lstrName;
            }
        }

        private string _addr_description;
        public string addr_description
        {
            get
            {
                _addr_description = string.Empty;
                if (address_line_1 != null)
                {
                    _addr_description += address_line_1 + ", ";
                }
                if (address_line_2 != null)
                {
                    _addr_description += address_line_2 + ", ";
                }
                if (address_country_value == busConstant.US_Code_ID)
                {
                    if (address_city != null)
                    {
                        _addr_description += address_city + ", ";
                    }
                    if (address_state_value != null)
                    {
                        _addr_description += address_state_value + " ";
                    }
                    if (address_zip_code != null)
                    {
                        _addr_description += address_zip_code;
                    }
                    if (address_zip_4_code != null)
                    {
                        _addr_description += "-" + address_zip_4_code;
                    }
                }
                else
                {
                    if (address_state_description != null)
                    {
                        _addr_description += address_state_description + ", ";
                    }

                    if (!String.IsNullOrEmpty(foreign_province))
                    {
                        _addr_description += foreign_province + ", ";
                    }

                    if (!String.IsNullOrEmpty(foreign_postal_code))
                    {
                        _addr_description += foreign_postal_code + " ";
                    }

                    if (address_country_description != null)
                    {
                        _addr_description += address_country_description;
                    }
                }
                return _addr_description;
            }
        }
    } 
} 
