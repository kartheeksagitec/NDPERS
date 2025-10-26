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
	/// <summary>
	/// Class NeoSpin.CustomDataObjects.cdoPersonAccountMedicarePartDHistory:
	/// Inherited from doPersonAccountMedicarePartDHistory, the class is used to customize the database object doPersonAccountMedicarePartDHistory.
	/// </summary>
    [Serializable]
	public class cdoPersonAccountMedicarePartDHistory : doPersonAccountMedicarePartDHistory
	{
		public cdoPersonAccountMedicarePartDHistory() : base()
		{
		}
        public string istrName { get; set; }
        public string istrRelationship { get; set; }
        public string istrStatus { get; set; }
        public string istrMedicareClaim { get; set; }

        public DateTime idtPartADate { get; set; }
        public DateTime idtPartBDate { get; set; }
        public decimal iintLISCredit { get; set; }
        public decimal iintPenalty { get; set; }
        public int iintPersonID { get; set; }
        public int iintHistoryID { get; set; }
        public int iintPersonAcctID { get; set; }

        public int iintCoveredMember { get; set; }

        private decimal _MonthlyPremiumAmount;
        public decimal MonthlyPremiumAmount
        {
            get { return _MonthlyPremiumAmount; }
            set { _MonthlyPremiumAmount = value; }
        }

        private decimal _MonthlyPremiumAmountFromRef;
        public decimal MonthlyPremiumAmountFromRef
        {
            get { return _MonthlyPremiumAmountFromRef; }
            set { _MonthlyPremiumAmountFromRef = value; }
        }

        public string istrMemberName { get; set; }

        public decimal idecLISAmount { get; set; }
    } 
} 
