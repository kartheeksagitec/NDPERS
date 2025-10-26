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
	public class cdoPersonAccountRetirementDbDbTransfer : doPersonAccountRetirementDbDbTransfer
	{
		public cdoPersonAccountRetirementDbDbTransfer() : base()
		{
		}

        public decimal _idecPostTaxERAmount;
        public decimal idecPostTaxERAmount
        {
            get
            {
                return _idecPostTaxERAmount;
            }
            set
            {
                _idecPostTaxERAmount = value;
            }
        }

        public decimal _idecPostTaxEEAmount;
        public decimal idecPostTaxEEAmount
        {
            get
            {
                return _idecPostTaxEEAmount;
            }
            set
            {
                _idecPostTaxEEAmount = value;
            }
        }

        public decimal _idecPreTaxERAmount;
        public decimal idecPreTaxERAmount
        {
            get
            {
                return _idecPreTaxERAmount;
            }
            set
            {
                _idecPreTaxERAmount = value;
            }
        }

        public decimal _idecPreTaxEEAmount;
        public decimal idecPreTaxEEAmount
        {
            get
            {
                return _idecPreTaxEEAmount;
            }
            set
            {
                _idecPreTaxEEAmount = value;
            }
        }
        public decimal _idecEERHICAmount;
        public decimal idecEERHICAmount
        {
            get
            {
                return _idecEERHICAmount;
            }
            set
            {
                _idecEERHICAmount = value;
            }
        }

        public decimal _idecERRHICAmount;
        public decimal idecERRHICAmount
        {
            get
            {
                return _idecERRHICAmount;
            }
            set
            {
                _idecERRHICAmount = value;
            }
        }

        public decimal _idecEEERPickupAmount;
        public decimal idecEEERPickupAmount
        {
            get
            {
                return _idecEEERPickupAmount;
            }
            set
            {
                _idecEEERPickupAmount = value;
            }
        }

        public decimal _idecERVestedAmount;
        public decimal idecERVestedAmount
        {
            get
            {
                return _idecERVestedAmount;
            }
            set
            {
                _idecERVestedAmount = value;
            }
        }

        public decimal _idecInterestAmount;
        public decimal idecInterestAmount
        {
            get
            {
                return _idecInterestAmount;
            }
            set
            {
                _idecInterestAmount = value;
            }
        }

        public decimal _idecVestedServiceCredit;
        public decimal idecVestedServiceCredit
        {
            get
            {
                return _idecVestedServiceCredit;
            }
            set
            {
                _idecVestedServiceCredit = value;
            }
        }

        public decimal _idecPensionServiceCredit;
        public decimal idecPensionServiceCredit
        {
            get
            {
                return _idecPensionServiceCredit;
            }
            set
            {
                _idecPensionServiceCredit = value;
            }
        }

        public int iintTransferToPlanPersonAccountID { get; set; }

        public int iintTransferToMemberPersonID { get; set; }

        /// <summary>
        /// *** BR - 01 *** Transfers between NDPERS DB plans
        /// </summary>
        /// <param name="astrOperation"></param>
        /// <param name="astrColumnName"></param>
        /// <returns></returns>
        public override bool AuditColumn(string astrOperation, string astrColumnName)
        {
            if (astrColumnName == "status_value")
            {
                if (ihstOldValues.Count > 0)
                {
                    if ((status_value == BusinessObjects.busConstant.TransferStatusPosted) &&
                       (Convert.ToString(ihstOldValues["status_value"]) == BusinessObjects.busConstant.TransferStatusValid))
                        return true;
                }
                return false;
            }
            return base.AuditColumn(astrOperation, astrColumnName);
        }

        // for display purpose only
        public string istrFromPersonName { get; set; }
        public string istrToPersonName { get; set; }
        public string istrToPlanName { get; set; }
        public string istrFromPlanName { get; set; }
    } 
} 
