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
	/// Class NeoSpin.CustomDataObjects.cdoWssPersonAddress:
	/// Inherited from doWssPersonAddress, the class is used to customize the database object doWssPersonAddress.
	/// </summary>
    [Serializable]
	public class cdoWssPersonAddress : doWssPersonAddress
	{
		public cdoWssPersonAddress() : base()
		{
		}

        private string _addr_description;
        public string addr_description
        {
            get
            {
                _addr_description = string.Empty;
                if (addr_line_1 != null)
                {
                    _addr_description += addr_line_1 + ", ";
                }
                if (addr_line_2 != null)
                {
                    _addr_description += addr_line_2 + ", ";
                }
                if (addr_country_value == busConstant.US_Code_ID)
                {
                    if (addr_city != null)
                    {
                        _addr_description += addr_city + ", ";
                    }
                    if (addr_state_value != null)
                    {
                        _addr_description += addr_state_value + " ";
                    }
                    if (addr_zip_code != null)
                    {
                        _addr_description += addr_zip_code;
                    }
                    if (addr_zip_4_code != null)
                    {
                        _addr_description += "-" + addr_zip_4_code;
                    }
                }
                else
                {
                    if (addr_state_description != null)
                    {
                        _addr_description += addr_state_description + ", ";
                    }

                    if (!String.IsNullOrEmpty(foreign_province))
                    {
                        _addr_description += foreign_province + ", ";
                    }

                    if (!String.IsNullOrEmpty(foreign_postal_code))
                    {
                        _addr_description += foreign_postal_code + " ";
                    }

                    if (addr_country_description != null)
                    {
                        _addr_description += addr_country_description;
                    }
                }
                return _addr_description;
            }
        }
    } 
} 
