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
using NeoSpin.DataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// Class NeoSpin.BusinessObjects.busPaymentBenefitOverpaymentHeaderGen:
    /// Inherited from busBase, used to create new business object for main table cdoPaymentBenefitOverpaymentHeader and its children table. 
    /// </summary>
	[Serializable]
	public class busPaymentBenefitOverpaymentHeaderGen : busExtendBase
	{
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busPaymentBenefitOverpaymentHeaderGen
        /// </summary>
		public busPaymentBenefitOverpaymentHeaderGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPaymentBenefitOverpaymentHeaderGen.
        /// </summary>
		public cdoPaymentBenefitOverpaymentHeader icdoPaymentBenefitOverpaymentHeader { get; set; }


        /// <summary>
        /// Gets or sets the collection object of type busPaymentBenefitOverpaymentDetail. 
        /// </summary>
		public Collection<busPaymentBenefitOverpaymentDetail> iclbPaymentBenefitOverpaymentDetail { get; set; }



        /// <summary>
        /// NeoSpin.busPaymentBenefitOverpaymentHeaderGen.FindPaymentBenefitOverpaymentHeader():
        /// Finds a particular record from cdoPaymentBenefitOverpaymentHeader with its primary key. 
        /// </summary>
        /// <param name="aintbenefitoverpaymentid">A primary key value of type int of cdoPaymentBenefitOverpaymentHeader on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPaymentBenefitOverpaymentHeader(int aintbenefitoverpaymentid)
		{
			bool lblnResult = false;
			if (icdoPaymentBenefitOverpaymentHeader == null)
			{
				icdoPaymentBenefitOverpaymentHeader = new cdoPaymentBenefitOverpaymentHeader();
			}
			if (icdoPaymentBenefitOverpaymentHeader.SelectRow(new object[1] { aintbenefitoverpaymentid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        /// NeoSpin.busPaymentBenefitOverpaymentHeaderGen.LoadPaymentBenefitOverpaymentDetails():
        /// Loads Collection object iclbPaymentBenefitOverpaymentDetail of type busPaymentBenefitOverpaymentDetail.
        /// </summary>
		public virtual void LoadPaymentBenefitOverpaymentDetails()
		{
			DataTable ldtbList = Select<cdoPaymentBenefitOverpaymentDetail>(
				new string[1] { enmPaymentBenefitOverpaymentDetail.benefit_overpayment_id.ToString() },
				new object[1] { icdoPaymentBenefitOverpaymentHeader.benefit_overpayment_id }, null, null);
			iclbPaymentBenefitOverpaymentDetail = GetCollection<busPaymentBenefitOverpaymentDetail>(ldtbList, "icdoPaymentBenefitOverpaymentDetail");
            foreach (busPaymentBenefitOverpaymentDetail lobjOverpaymentDetail in iclbPaymentBenefitOverpaymentDetail)
                lobjOverpaymentDetail.LoadPaymentItemType();
		}

	}
}
