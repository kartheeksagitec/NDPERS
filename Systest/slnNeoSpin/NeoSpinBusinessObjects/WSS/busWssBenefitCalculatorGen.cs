#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// Class NeoSpin.BusinessObjects.busWssBenefitCalculatorGen:
    /// Inherited from busBase, used to create new business object for main table cdoWssBenefitCalculator and its children table. 
    /// </summary>
	[Serializable]
	public class busWssBenefitCalculatorGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busWssBenefitCalculatorGen
        /// </summary>
		public busWssBenefitCalculatorGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busWssBenefitCalculatorGen.
        /// </summary>
		public cdoWssBenefitCalculator icdoWssBenefitCalculator { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busServicePurchaseHeader.
        /// </summary>
		public busServicePurchaseHeader ibusUnusedServicePurchaseHeader { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busServicePurchaseHeader.
        /// </summary>
		public busServicePurchaseHeader ibusConsolidatedServicePurchaseHeader { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busRetirementBenefitCalculation.
        /// </summary>
		public busRetirementBenefitCalculation ibusRetirementBenefitCalculation { get; set; }

        public busPerson ibusMember { get; set; }
        public busPlan ibusPlan { get; set; }


        /// <summary>
        /// NeoSpin.busWssBenefitCalculatorGen.FindWssBenefitCalculator():
        /// Finds a particular record from cdoWssBenefitCalculator with its primary key. 
        /// </summary>
        /// <param name="aintwssbenefitcalculatorid">A primary key value of type int of cdoWssBenefitCalculator on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindWssBenefitCalculator(int aintwssbenefitcalculatorid)
		{
			bool lblnResult = false;
			if (icdoWssBenefitCalculator == null)
			{
				icdoWssBenefitCalculator = new cdoWssBenefitCalculator();
			}
			if (icdoWssBenefitCalculator.SelectRow(new object[1] { aintwssbenefitcalculatorid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        /// NeoSpin.busWssBenefitCalculatorGen.LoadbusUnusedServicePurchaseHeader():
        /// Loads non-collection object ibusbusUnusedServicePurchaseHeader of type busServicePurchaseHeader.
        /// </summary>
		public virtual void LoadbusUnusedServicePurchaseHeader()
		{
			if (ibusUnusedServicePurchaseHeader == null)
			{
				ibusUnusedServicePurchaseHeader = new busServicePurchaseHeader();
			}
			ibusUnusedServicePurchaseHeader.FindServicePurchaseHeader(icdoWssBenefitCalculator.unused_sick_leave_service_purchase_header_id);
		}

        /// <summary>
        /// NeoSpin.busWssBenefitCalculatorGen.LoadbusConsolidatedServicePurchaseHeader():
        /// Loads non-collection object ibusbusConsolidatedServicePurchaseHeader of type busServicePurchaseHeader.
        /// </summary>
		public virtual void LoadbusConsolidatedServicePurchaseHeader()
		{
            if (ibusConsolidatedServicePurchaseHeader == null)
			{
                ibusConsolidatedServicePurchaseHeader = new busServicePurchaseHeader();
			}
            ibusConsolidatedServicePurchaseHeader.FindServicePurchaseHeader(icdoWssBenefitCalculator.consolidated_service_purchase_header_id);
		}

        /// <summary>
        /// NeoSpin.busWssBenefitCalculatorGen.LoadRetirementBenefitCalculation():
        /// Loads non-collection object ibusRetirementBenefitCalculation of type busRetirementBenefitCalculation.
        /// </summary>
		public virtual void LoadRetirementBenefitCalculation()
		{
			if (ibusRetirementBenefitCalculation == null)
			{
				ibusRetirementBenefitCalculation = new busRetirementBenefitCalculation();
			}
			ibusRetirementBenefitCalculation.FindBenefitCalculation(icdoWssBenefitCalculator.benefit_calculation_id);
		}

       
	}
}
