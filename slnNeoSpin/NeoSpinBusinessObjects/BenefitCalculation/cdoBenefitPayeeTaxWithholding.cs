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
	public class cdoBenefitPayeeTaxWithholding : doBenefitPayeeTaxWithholding
	{
		public cdoBenefitPayeeTaxWithholding() : base()
		{
		}

        private bool _iblnValueEntered;
        public bool iblnValueEntered
        {
            get
            {
                return _iblnValueEntered;
            }
            set
            {
                _iblnValueEntered = value;
            }
        }
    } 
} 
