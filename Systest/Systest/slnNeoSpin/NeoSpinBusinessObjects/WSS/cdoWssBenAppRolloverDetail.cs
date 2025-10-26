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
    /// Class NeoSpin.CustomDataObjects.cdoWssBenAppRolloverDetail:
    /// Inherited from doWssBenAppRolloverDetail, the class is used to customize the database object doWssBenAppRolloverDetail.
    /// </summary>
    [Serializable]
    public class cdoWssBenAppRolloverDetail : doWssBenAppRolloverDetail
    {
        public cdoWssBenAppRolloverDetail() : base()
        {
        }
        private string _addr_description;
        public string addr_description
        {
            get
            {
                _addr_description = "";
                if (addr_line_1 != null)
                {
                    _addr_description += addr_line_1 + ", ";
                }
                if (addr_line_2 != null)
                {
                    _addr_description += addr_line_2 + ", ";
                }
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
                return _addr_description;
            }
        }
    }
}
