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
    public class doBenefitServicePurchase : doBase
    {
         
         public doBenefitServicePurchase() : base()
         {
         }
		private int _benefit_service_purchase_id;
		public int benefit_service_purchase_id
		{
			get
			{
				return _benefit_service_purchase_id;
			}

			set
			{
				_benefit_service_purchase_id = value;
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

		private int _service_purchase_header_id;
		public int service_purchase_header_id
		{
			get
			{
				return _service_purchase_header_id;
			}

			set
			{
				_service_purchase_header_id = value;
			}
		}

		private decimal _remaining_psc;
		public decimal remaining_psc
		{
			get
			{
				return _remaining_psc;
			}

			set
			{
				_remaining_psc = value;
			}
		}

    }
}

