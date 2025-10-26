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
    public class doBenefitProvision : doBase
    {
         
         public doBenefitProvision() : base()
         {
         }
		private int _benefit_provision_id;
		public int benefit_provision_id
		{
			get
			{
				return _benefit_provision_id;
			}

			set
			{
				_benefit_provision_id = value;
			}
		}

		private string _short_decription;
		public string short_decription
		{
			get
			{
				return _short_decription;
			}

			set
			{
				_short_decription = value;
			}
		}

		private string _description;
		public string description
		{
			get
			{
				return _description;
			}

			set
			{
				_description = value;
			}
		}

    }
}

