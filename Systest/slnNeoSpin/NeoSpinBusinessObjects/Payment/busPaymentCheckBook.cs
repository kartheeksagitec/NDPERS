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
using System.Linq;
using System.Linq.Expressions;
#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busPaymentCheckBook : busPaymentCheckBookGen
    {
        //Last Check number cannot be less than first check number,else throw an error
        public bool IsLastCheckNumberLessThanFirstCheckNumber()
        {
            if (!string.IsNullOrEmpty(icdoPaymentCheckBook.first_check_number) && !string.IsNullOrEmpty(icdoPaymentCheckBook.last_check_number))
            {
                if (Convert.ToInt32(icdoPaymentCheckBook.last_check_number) < Convert.ToInt32(icdoPaymentCheckBook.first_check_number))
                {
                    return true;
                }
            }
            return false;
        }

        //Max Check number cannot be less than Last check number,else throw an error
        public bool IsMaxCheckNumberLessThanLastCheckNumber()
        {
            if (!string.IsNullOrEmpty(icdoPaymentCheckBook.max_check_number) && !string.IsNullOrEmpty(icdoPaymentCheckBook.last_check_number))
            {
                if (Convert.ToInt32(icdoPaymentCheckBook.max_check_number) <= Convert.ToInt32(icdoPaymentCheckBook.last_check_number))
                {
                    return true;
                }
            }
            return false;
        }
        //Effective date should be equal to or greater than next benefit payment date 
        //else throw an error
        public bool IsEffectivedateNotValid()
        {
            if (icdoPaymentCheckBook.effective_date != DateTime.MinValue)
            {
                DateTime ldtNextBenefitPaymentDate = busPayeeAccountHelper.GetLastBenefitPaymentDate().AddMonths(1);
                DateTime ldtStartDate = new DateTime(ldtNextBenefitPaymentDate.Year, ldtNextBenefitPaymentDate.Month, 1);
                if (icdoPaymentCheckBook.effective_date < ldtStartDate)
                {
                    return true;
                }
            }
            return false;
        }
        //there should be only one record for the effective date entered in the screen,else throw an error
        public bool IsCheckBookExistsforGivenDate()
        {       
            DataTable ldtbResult = busBase.SelectWithOperator<cdoPaymentCheckBook>
                                  (  new string[2] { "effective_date", "check_book_id" },
                                     new string[2] { "=", "<>" },
                                     new object[2] {icdoPaymentCheckBook.effective_date,icdoPaymentCheckBook.check_book_id}, null);

            if (ldtbResult.Rows.Count > 0)
            {
                return true;
            }
            return false;
        }
    }
}