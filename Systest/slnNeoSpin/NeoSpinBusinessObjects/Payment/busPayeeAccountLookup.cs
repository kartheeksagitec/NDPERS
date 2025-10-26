#region Using directives

using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using NeoSpin.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busPayeeAccountLookup : busPayeeAccountLookupGen
    {
        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            if (aobjBus is busPayeeAccount)
            {
                busPayeeAccount lobjPayeeAccount = (busPayeeAccount)aobjBus;

                lobjPayeeAccount.ibusPayee = new busPerson();
                lobjPayeeAccount.ibusPayee.icdoPerson = new cdoPerson();
                if (!Convert.IsDBNull(adtrRow["payeeperslinkid"]))
                {
                    lobjPayeeAccount.ibusPayee.icdoPerson.person_id = Convert.ToInt32(adtrRow["payeeperslinkid"]);
                    lobjPayeeAccount.ibusPayee.icdoPerson.first_name = adtrRow["payeefirstname"].ToString();
                    lobjPayeeAccount.ibusPayee.icdoPerson.last_name = adtrRow["payeelastname"].ToString();

                }
                lobjPayeeAccount.ibusMember = new busPerson();
                lobjPayeeAccount.ibusMember.icdoPerson = new cdoPerson();
                if (!Convert.IsDBNull(adtrRow["memberperslinkid"]))
                {
                    lobjPayeeAccount.ibusMember.icdoPerson.person_id = Convert.ToInt32(adtrRow["memberperslinkid"]);
                    lobjPayeeAccount.ibusMember.icdoPerson.first_name = adtrRow["memberfirstname"].ToString();
                    lobjPayeeAccount.ibusMember.icdoPerson.last_name = adtrRow["memberlastname"].ToString();
                }
                lobjPayeeAccount.ibusPlan = new busPlan();
                lobjPayeeAccount.ibusPlan.icdoPlan = new cdoPlan();
                lobjPayeeAccount.ibusPlan.icdoPlan.plan_name = adtrRow["planname"].ToString();

                lobjPayeeAccount.ibusApplication = new busBenefitApplication();
                lobjPayeeAccount.ibusApplication.icdoBenefitApplication = new cdoBenefitApplication();
                lobjPayeeAccount.ibusApplication.icdoBenefitApplication.LoadData(adtrRow);

                lobjPayeeAccount.ibusPayeeAccountActiveStatus = new busPayeeAccountStatus();
                lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus = new cdoPayeeAccountStatus();
                lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.LoadData(adtrRow);
                lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_id = Convert.ToInt16(adtrRow["payeestatusid"]);
                lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value = adtrRow["Payeestatus"].ToString();
                lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_description =
                   iobjPassInfo.isrvDBCache.GetCodeDescriptionString(lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_id,
                                                                    lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value);
                lobjPayeeAccount.LoadPayeeName();
            }
        }
    }
}