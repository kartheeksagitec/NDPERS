#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using  NeoSpin.CustomDataObjects;

#endregion

namespace  NeoSpin.BusinessObjects
{
    /// <summary>
    /// Class  NeoSpin.BusinessObjects.busBenefitRhicEstimateCombineDetailGen:
    /// Inherited from busBase, used to create new business object for main table cdoBenefitRhicEstimateCombineDetail and its children table. 
    /// </summary>
	[Serializable]
	public class busBenefitRhicEstimateCombineDetailGen : busExtendBase
    {
        /// <summary>
        /// Constructor for  NeoSpin.BusinessObjects.busBenefitRhicEstimateCombineDetailGen
        /// </summary>
		public busBenefitRhicEstimateCombineDetailGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busBenefitRhicEstimateCombineDetailGen.
        /// </summary>
		public cdoBenefitRhicEstimateCombineDetail icdoBenefitRhicEstimateCombineDetail { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busPersonAccount.
        /// </summary>
		public busPersonAccount ibusPersonAccount { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busPayeeAccount.
        /// </summary>
		public busPayeeAccount ibusPayeeAccount { get; set; }




        /// <summary>
        ///  NeoSpin.busBenefitRhicEstimateCombineDetailGen.FindBenefitRhicEstimateCombineDetail():
        /// Finds a particular record from cdoBenefitRhicEstimateCombineDetail with its primary key. 
        /// </summary>
        /// <param name="aintbenefitrhicestimatecombinedetailid">A primary key value of type int of cdoBenefitRhicEstimateCombineDetail on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindBenefitRhicEstimateCombineDetail(int aintbenefitrhicestimatecombinedetailid)
		{
			bool lblnResult = false;
			if (icdoBenefitRhicEstimateCombineDetail == null)
			{
				icdoBenefitRhicEstimateCombineDetail = new cdoBenefitRhicEstimateCombineDetail();
			}
			if (icdoBenefitRhicEstimateCombineDetail.SelectRow(new object[1] { aintbenefitrhicestimatecombinedetailid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        ///  NeoSpin.busBenefitRhicEstimateCombineDetailGen.LoadPersonAccount():
        /// Loads non-collection object ibusPersonAccount of type busPersonAccount.
        /// </summary>
		public virtual void LoadPersonAccount()
		{
			if (ibusPersonAccount == null)
			{
				ibusPersonAccount = new busPersonAccount();
			}
			ibusPersonAccount.FindPersonAccount(icdoBenefitRhicEstimateCombineDetail.donar_person_account_id);
		}

        /// <summary>
        ///  NeoSpin.busBenefitRhicEstimateCombineDetailGen.LoadPayeeAccount():
        /// Loads non-collection object ibusPayeeAccount of type busPayeeAccount.
        /// </summary>
		public virtual void LoadPayeeAccount()
		{
			if (ibusPayeeAccount == null)
			{
				ibusPayeeAccount = new busPayeeAccount();
			}
			ibusPayeeAccount.FindPayeeAccount(icdoBenefitRhicEstimateCombineDetail.donar_payee_account_id);
		}

	}
}
