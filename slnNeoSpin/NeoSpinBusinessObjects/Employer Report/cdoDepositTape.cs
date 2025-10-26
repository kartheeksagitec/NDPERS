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
	public class cdoDepositTape : doDepositTape
	{
		public cdoDepositTape() : base()
		{
		}

        private int _DepositsCount;
        public int DepositsCount
        {
            get { return _DepositsCount; }
            set { _DepositsCount = value; }
        }

        private decimal _TotalDepositAmount;
        public decimal TotalDepositAmount
        {
            get { return _TotalDepositAmount; }
            set { _TotalDepositAmount = value; }
        }


    } 
} 
