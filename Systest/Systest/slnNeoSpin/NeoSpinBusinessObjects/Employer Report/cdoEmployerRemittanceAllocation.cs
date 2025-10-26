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
	public class cdoEmployerRemittanceAllocation : doEmployerRemittanceAllocation
	{
		public cdoEmployerRemittanceAllocation() : base()
		{
        }      
        
        private decimal _idecAllocatedAmount;
        public decimal idecAllocatedAmount
        {
            get { return _idecAllocatedAmount; }
            set { _idecAllocatedAmount = value; }
        }
		
    } 
} 
