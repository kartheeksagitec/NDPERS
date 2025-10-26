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
    /// Class NeoSpin.BusinessObjects.busBenefitRhicCombineHealthSplitGen:
    /// Inherited from busBase, used to create new business object for main table cdoBenefitRhicCombineHealthSplit and its children table. 
    /// </summary>
	[Serializable]
	public class busBenefitRhicCombineHealthSplitGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busBenefitRhicCombineHealthSplitGen
        /// </summary>
		public busBenefitRhicCombineHealthSplitGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busBenefitRhicCombineHealthSplitGen.
        /// </summary>
		public cdoBenefitRhicCombineHealthSplit icdoBenefitRhicCombineHealthSplit { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busBenefitRhicCombine.
        /// </summary>
		public busBenefitRhicCombine ibusBenefitRhicCombine { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busPersonAccount.
        /// </summary>
		public busPersonAccount ibusPersonAccount { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busPayeeAccountRetroPayment.
        /// </summary>
		public busPayeeAccountRetroPayment ibusPayeeAccountRetroPayment { get; set; }




        /// <summary>
        /// NeoSpin.busBenefitRhicCombineHealthSplitGen.FindBenefitRhicCombineHealthSplit():
        /// Finds a particular record from cdoBenefitRhicCombineHealthSplit with its primary key. 
        /// </summary>
        /// <param name="aintbenefitrhiccombinehealthsplitid">A primary key value of type int of cdoBenefitRhicCombineHealthSplit on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindBenefitRhicCombineHealthSplit(int aintbenefitrhiccombinehealthsplitid)
		{
			bool lblnResult = false;
			if (icdoBenefitRhicCombineHealthSplit == null)
			{
				icdoBenefitRhicCombineHealthSplit = new cdoBenefitRhicCombineHealthSplit();
			}
			if (icdoBenefitRhicCombineHealthSplit.SelectRow(new object[1] { aintbenefitrhiccombinehealthsplitid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        /// NeoSpin.busBenefitRhicCombineHealthSplitGen.LoadBenefitRhicCombine():
        /// Loads non-collection object ibusBenefitRhicCombine of type busBenefitRhicCombine.
        /// </summary>
		public virtual void LoadBenefitRhicCombine()
		{
			if (ibusBenefitRhicCombine == null)
			{
				ibusBenefitRhicCombine = new busBenefitRhicCombine();
			}
			ibusBenefitRhicCombine.FindBenefitRhicCombine(icdoBenefitRhicCombineHealthSplit.benefit_rhic_combine_id);
		}

        /// <summary>
        /// NeoSpin.busBenefitRhicCombineHealthSplitGen.LoadPersonAccount():
        /// Loads non-collection object ibusPersonAccount of type busPersonAccount.
        /// </summary>
		public virtual void LoadPersonAccount()
		{
			if (ibusPersonAccount == null)
			{
				ibusPersonAccount = new busPersonAccount();
			}
			ibusPersonAccount.FindPersonAccount(icdoBenefitRhicCombineHealthSplit.person_account_id);
		}

        /// <summary>
        /// NeoSpin.busBenefitRhicCombineHealthSplitGen.LoadPayeeAccountRetroPayment():
        /// Loads non-collection object ibusPayeeAccountRetroPayment of type busPayeeAccountRetroPayment.
        /// </summary>
		public virtual void LoadPayeeAccountRetroPayment()
		{
			if (ibusPayeeAccountRetroPayment == null)
			{
				ibusPayeeAccountRetroPayment = new busPayeeAccountRetroPayment();
			}
			ibusPayeeAccountRetroPayment.FindPayeeAccountRetroPayment(icdoBenefitRhicCombineHealthSplit.payee_account_retro_payment_id);
		}

	}
}
