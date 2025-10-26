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
    public class doBenefitCalculationError : doBase
    {
         public doBenefitCalculationError() : base()
         {
         }
		private int _benefit_calculation_error_id;
        public int benefit_calculation_error_id
		{
			get
			{
                return _benefit_calculation_error_id;
			}

			set
			{
                _benefit_calculation_error_id = value;
			}
		}

		private int _benefit_calculation_id;
		public int benefit_calculation_id
		{
			get
			{
				return _benefit_calculation_id;
			}

			set
			{
				_benefit_calculation_id = value;
			}
		}

		private int _message_id;
        public int message_id
		{
			get
			{
                return _message_id;
			}

			set
			{
                _message_id = value;
			}
		}
    }
}

