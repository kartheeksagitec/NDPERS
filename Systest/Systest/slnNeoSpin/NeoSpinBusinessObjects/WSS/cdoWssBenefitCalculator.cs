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
	/// Class NeoSpin.CustomDataObjects.cdoWssBenefitCalculator:
	/// Inherited from doWssBenefitCalculator, the class is used to customize the database object doWssBenefitCalculator.
	/// </summary>
    [Serializable]
	public class cdoWssBenefitCalculator : doWssBenefitCalculator
	{
		public cdoWssBenefitCalculator() : base()
		{
		}

        public string additional_serivce_purchase_selected
        {
            get
            {
                if (estimated_service_purchase_type_value == BusinessObjects.busConstant.EstimatedAdditionalServicePurchaseType ||
                    estimated_service_purchase_type_value == BusinessObjects.busConstant.EstimatedBothServicePurchaseType)
                    return BusinessObjects.busConstant.Flag_Yes;
                return BusinessObjects.busConstant.Flag_No;
            }
        }

        public string unused_service_purchase_selected
        {
            get
            {
                if (estimated_service_purchase_type_value == BusinessObjects.busConstant.EstimatedUnusedServicePurchaseType ||
                    estimated_service_purchase_type_value == BusinessObjects.busConstant.EstimatedBothServicePurchaseType)
                    return BusinessObjects.busConstant.Flag_Yes;
                return BusinessObjects.busConstant.Flag_No;
            }
        }
    } 
} 
