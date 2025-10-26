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
using System.Linq;

#endregion

namespace NeoSpin.BusinessObjects
{
	[Serializable]
	public class busServicePurchaseHeaderLookup : busServicePurchaseHeaderLookupGen
	{
        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            base.LoadOtherObjects(adtrRow, aobjBus);
            busServicePurchaseHeader lobjServicePurchaseHeader = (busServicePurchaseHeader) aobjBus;
            lobjServicePurchaseHeader.ibusPerson = new busPerson();
            lobjServicePurchaseHeader.ibusPerson.icdoPerson = new cdoPerson();
            lobjServicePurchaseHeader.ibusPerson.icdoPerson.LoadData(adtrRow);

            // Load the service purchase detail records for this header, since we need to show "Total time to Purchase" in the output 
            lobjServicePurchaseHeader.LoadServicePurchaseDetail();
        }

        public override ArrayList ValidateNew(Hashtable ahstParam)
        {
            ArrayList larrErrors = new ArrayList();
            // Person Account ID has to be entered for creating  anew record
            if (( (ahstParam["service_credit_header_type"].ToString()) == "") || (ahstParam["service_credit_header_type"].ToString()) == "ALL")
            {
                utlError lobjError = null;
                lobjError = AddError(1108, "");
                larrErrors.Add(lobjError);
            }
            if (ahstParam["person_id"].ToString() == "")
            {
                utlError lobjError = null;
                lobjError = AddError(1112, "");
                larrErrors.Add(lobjError);
            }
            #region service purchase PIR-15531 and PIR-9403 
            int lintPersonId = 0;
            if (int.TryParse(ahstParam["person_id"].ToString(), out lintPersonId))
            {
                busPerson lbusPerson = new busPerson();
                if(lbusPerson.FindPerson(lintPersonId))
                {
                    DataTable ldtPersonEmployementDetail = busBase.Select("entPersonEmploymentDetail.LoadAllEmploymentDetailsForPerson", new object[1] { lintPersonId });
                    if (ldtPersonEmployementDetail.Rows.Count > 0 && ldtPersonEmployementDetail.AsEnumerable().All(emp => emp.Field<string>("TYPE_VALUE")== busConstant.PersonJobTypeTemporary))
                    {
                        utlError lobjError = null;
                        lobjError = AddError(0, "You are not eligible to Purchase Credit as a Temporary Employee.");
                        larrErrors.Add(lobjError);                                                
                    }
                }
                else
                {
                    utlError lobjError = null;
                    lobjError = AddError(2009, "");
                    larrErrors.Add(lobjError);
                }
            }
			#endregion service purchase PIR-15531 and PIR-9403
            return larrErrors;
        }
	}
}
