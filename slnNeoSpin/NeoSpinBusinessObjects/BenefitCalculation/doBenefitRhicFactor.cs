#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sagitec.Common;
using Sagitec.DataObjects;
#endregion

namespace NeoSpin.DataObjects
{
    [Serializable]
    public class doBenefitRhicFactor : doBase
    {
         
         public doBenefitRhicFactor() : base()
         {
         }
		private int _benefit_rhic_factor_id;
		public int benefit_rhic_factor_id
		{
			get
			{
				return _benefit_rhic_factor_id;
			}

			set
			{
				_benefit_rhic_factor_id = value;
			}
		}

		private decimal _rhic_factor;
		public decimal rhic_factor
		{
			get
			{
				return _rhic_factor;
			}

			set
			{
				_rhic_factor = value;
			}
		}

		private DateTime _effective_date;
		public DateTime effective_date
		{
			get
			{
				return _effective_date;
			}

			set
			{
				_effective_date = value;
			}
		}

    }
}

