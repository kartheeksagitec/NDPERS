#region Using directives
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoSpin.DataObjects;
using Sagitec.CustomDataObjects;
using NeoSpin.CustomDataObjects;
using System.Linq;
#endregion
namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// partial Class NeoSpin.busWssEmploymentAcaCert:
    /// </summary>
	[Serializable]
    public partial class busWssEmploymentAcaCert : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.busWssEmploymentAcaCert
        /// </summary>
        public busWssEmploymentAcaCert()
        {
        }
        public cdoWssEmploymentAcaCert icdoWssEmploymentAcaCert { get; set; }

        public DateTime idtAnnualFromDate { get; set; }
        public DateTime idtAnnualToDate { get; set; }
        public bool iblnIsCertifyClick { get; set; }
        public string istrFullTimeEmpCriteria { get; set; }
        public string istrIsNewHire { get; set; }
        public int iintOrgId { get; set; }
        public busPerson ibusPerson { get; set; }
        public string istrMethodDescription { get; set; }
        public string istrLBMethodType { get; set; }
        public bool FindWssEmploymentAcaCert(int AintWssEmploymentAcaCertId)
        {
            bool lblnResult = false;
            if (icdoWssEmploymentAcaCert == null)
            {
                icdoWssEmploymentAcaCert = new cdoWssEmploymentAcaCert();
            }
            if (icdoWssEmploymentAcaCert.SelectRow(new object[1] { AintWssEmploymentAcaCertId }))
            {
                lblnResult = true;
            }
            return lblnResult;
        }
        public void CertifyButtonClick()
        {
            this.iblnIsCertifyClick = true;
            this.istrFullTimeEmpCriteria = busConstant.Flag_Yes;
           
        }

        public Collection<cdoCodeValue> LoadMthodValueForAcaCertification()
        {
            Collection<cdoCodeValue> lclbResult = GetCodeValue(7031);
            if (istrIsNewHire == busConstant.Flag_No)
            {
                lclbResult = lclbResult
                            .Where(option => option.code_value == busConstant.ACACertificationMethodChangePosition || option.code_value == busConstant.ACACertificationMethodLookBack)
                            .ToList()
                            .ToCollection();
            }
            return lclbResult;
        }
        public Collection<cdoCodeValue> LoadLookBackOptions()
        {
            Collection<cdoCodeValue> lclbResult = GetCodeValue(7032);
            DateTime ldteAnnualLookBackStartDate = new DateTime(DateTime.Today.Year, 11, 1);
            DateTime ldteAnnualLookBackEndDate = new DateTime(DateTime.Today.Year, 12, 31);
            if (!busGlobalFunctions.CheckDateOverlapping(DateTime.Today.Date, ldteAnnualLookBackStartDate.Date, ldteAnnualLookBackEndDate.Date))
            {
                lclbResult = lclbResult.Where(option => option.code_value != busConstant.ACACertificationLookBackTypeAnnual).ToList().ToCollection();
            }
            return lclbResult;
        }

        public override void BeforePersistChanges()
        {
            if (this.icdoWssEmploymentAcaCert.met_req == busConstant.Flag_No)
            {
                icdoWssEmploymentAcaCert.method = null;
                icdoWssEmploymentAcaCert.to_date = DateTime.MinValue;
                icdoWssEmploymentAcaCert.from_date = DateTime.MinValue;
                icdoWssEmploymentAcaCert.lb_measure = null;
                iblnIsCertifyClick = false;
            }
            if (this.icdoWssEmploymentAcaCert.method == busConstant.ACACertificationMethodLookBack) 
                
            {
                if (this.icdoWssEmploymentAcaCert.lb_measure == busConstant.ACACertificationLookBackTypeAnnual)
                {
                    icdoWssEmploymentAcaCert.to_date = idtAnnualToDate;
                    icdoWssEmploymentAcaCert.from_date = idtAnnualFromDate;
                }
                else if(this.icdoWssEmploymentAcaCert.lb_measure == busConstant.ACACertificationLookBackTypeNew && this.icdoWssEmploymentAcaCert.from_date != DateTime.MinValue)
                {
                    icdoWssEmploymentAcaCert.to_date = this.icdoWssEmploymentAcaCert.from_date.AddYears(1).AddDays(-1);
                }
            }
            base.BeforePersistChanges();
        }
        public bool IsNewlyEligibleLookBackNotValid()
        {
            if (this.icdoWssEmploymentAcaCert.method == busConstant.ACACertificationMethodLookBack && this.icdoWssEmploymentAcaCert.lb_measure == busConstant.ACACertificationLookBackTypeNew && this.icdoWssEmploymentAcaCert.from_date != DateTime.MinValue)
            {
                icdoWssEmploymentAcaCert.to_date = this.icdoWssEmploymentAcaCert.from_date.AddYears(1).AddDays(-1);
                return (icdoWssEmploymentAcaCert.to_date.Date > DateTime.Today.Date);
            }
            return false;
        }

    }
}
