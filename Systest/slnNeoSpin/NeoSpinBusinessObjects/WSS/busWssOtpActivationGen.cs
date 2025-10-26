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
using NeoSpin.BusinessObjects;

#endregion

namespace NeoSpin
{
    /// <summary>
    /// Class NeoSpin.busWssOtpActivationGen:
    /// Inherited from busBase, used to create new business object for main table cdoWssOtpActivation and its children table. 
    /// </summary>
	[Serializable]
	public class busWssOtpActivationGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.busWssOtpActivationGen
        /// </summary>
		public busWssOtpActivationGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busWssOtpActivationGen.
        /// </summary>
		public cdoWssOtpActivation icdoWssOtpActivation { get; set; }




        /// <summary>
        /// NeoSpin.busWssOtpActivationGen.FindWssOtpActivation():
        /// Finds a particular record from cdoWssOtpActivation with its primary key. 
        /// </summary>
        /// <param name="aintOtpActivationId">A primary key value of type int of cdoWssOtpActivation on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindWssOtpActivation(int aintOtpActivationId)
		{
			bool lblnResult = false;
			if (icdoWssOtpActivation == null)
			{
				icdoWssOtpActivation = new cdoWssOtpActivation();
			}
			if (icdoWssOtpActivation.SelectRow(new object[1] { aintOtpActivationId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        public void InsertActivationCodeInTable(int aintPersonID, string astrSourceValue, string astrActivationCode, DateTime adtActivationCodeDate)
        {
            icdoWssOtpActivation.person_id = aintPersonID;
            icdoWssOtpActivation.source_value = astrSourceValue;
            icdoWssOtpActivation.activation_code = astrActivationCode;
            icdoWssOtpActivation.activation_code_date = adtActivationCodeDate;

            icdoWssOtpActivation.activation_code_validate = busConstant.Flag_No;
            icdoWssOtpActivation.Insert();
        }
        public Collection<busWssOtpActivation> LoadWssOtpActivation(int aintPersonID,string astrSourceValue)
        {
            DataTable ldtCorrectOTP = busNeoSpinBase.Select<cdoWssOtpActivation>(new string[2] { "PERSON_ID", "SOURCE_VALUE" },
                                              new object[2] { aintPersonID, astrSourceValue }, null, null);
            return GetCollection<busWssOtpActivation>(ldtCorrectOTP, "icdoWssOtpActivation");
        }

    }
}
