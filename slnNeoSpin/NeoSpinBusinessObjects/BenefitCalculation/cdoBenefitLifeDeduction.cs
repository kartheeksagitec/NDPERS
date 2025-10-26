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
	public class cdoBenefitLifeDeduction : doBenefitLifeDeduction
	{
		public cdoBenefitLifeDeduction() : base()
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

        private DateTime _idtDateofBirth;

        public DateTime idtDateofBirth
        {
            get { return _idtDateofBirth; }
            set { _idtDateofBirth = value; }
        }

        private DateTime _idtCalculationDate;

        public DateTime idtCalculationDate
        {
            get { return _idtCalculationDate; }
            set { _idtCalculationDate = value; }
        }	
    } 
} 
