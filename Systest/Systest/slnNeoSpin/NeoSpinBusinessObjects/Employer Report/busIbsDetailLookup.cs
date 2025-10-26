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
    public partial class busIbsDetailLookup
    {
        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            busIbsDetail lbusIbsDetail = (busIbsDetail)aobjBus;
            lbusIbsDetail.ibusIbsHeader = new busIbsHeader { icdoIbsHeader = new cdoIbsHeader() };
            lbusIbsDetail.ibusIbsHeader.icdoIbsHeader.LoadData(adtrRow);

            lbusIbsDetail.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
            lbusIbsDetail.ibusPerson.icdoPerson.LoadData(adtrRow);

            lbusIbsDetail.ibusPlan = new busPlan { icdoPlan = new cdoPlan() };
            lbusIbsDetail.ibusPlan.icdoPlan.LoadData(adtrRow);

            lbusIbsDetail.ibusIbsPersonSummary = new busIbsPersonSummary { icdoIbsPersonSummary = new cdoIbsPersonSummary() };
            lbusIbsDetail.ibusIbsPersonSummary.icdoIbsPersonSummary.LoadData(adtrRow);

        }

        

        public void UpdateStatusToIgnore(ArrayList arrSelectedIBSRecords)
        {
            if (arrSelectedIBSRecords.IsNotNull() && arrSelectedIBSRecords.Count > 0)
            {
                foreach (busIbsDetail lobjIBSDetail in arrSelectedIBSRecords)
                {
                    if (lobjIBSDetail.icdoIbsDetail.detail_status_value != busConstant.PayrollDetailStatusPosted)
                    {
                        lobjIBSDetail.icdoIbsDetail.detail_status_value = busConstant.PayrollDetailStatusIgnored;
                        lobjIBSDetail.icdoIbsDetail.Update();
                    }
                }
            }
        }
    }
}
