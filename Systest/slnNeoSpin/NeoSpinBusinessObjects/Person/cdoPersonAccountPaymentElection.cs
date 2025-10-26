#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using NeoSpin.DataObjects;

#endregion

namespace NeoSpin.CustomDataObjects
{
    [Serializable]
    public class cdoPersonAccountPaymentElection : doPersonAccountPaymentElection
    {
        public cdoPersonAccountPaymentElection()
            : base()
        {

        }

        private string _Billing_Organization;
        public string Billing_Organization
        {
            get { return _Billing_Organization; }
            set { _Billing_Organization = value; }
        }

        private string _Supplemental_Billing_Organization;
        public string Supplemental_Billing_Organization
        {
            get { return _Supplemental_Billing_Organization; }
            set { _Supplemental_Billing_Organization = value; }
        }

        /// <summary>
        /// *** BR - 01 *** IBS selection changes from ‘True’ to ‘False’  
        /// </summary>
        /// <param name="astrOperation"></param>
        /// <param name="astrColumnName"></param>
        /// <returns></returns>
        public override bool AuditColumn(string astrOperation, string astrColumnName)
        {
            if ((astrColumnName == "ibs_flag") ||
               (astrColumnName == "ibs_supplemental_flag"))
            {
                if (ihstOldValues.Count > 0)
                {
                    if (((Convert.ToString(ihstOldValues["ibs_flag"]) == BusinessObjects.busConstant.Flag_Yes) &&
                        (ibs_flag == BusinessObjects.busConstant.Flag_No)) ||
                        ((Convert.ToString(ihstOldValues["ibs_supplemental_flag"]) == BusinessObjects.busConstant.Flag_Yes) &&
                        (ibs_supplemental_flag == BusinessObjects.busConstant.Flag_No)))
                        return true;
                }
                return false;
            }
            return base.AuditColumn(astrOperation, astrColumnName);
        }
    }
}
