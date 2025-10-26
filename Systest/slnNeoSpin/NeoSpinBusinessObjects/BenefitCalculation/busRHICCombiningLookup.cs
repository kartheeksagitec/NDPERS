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
    public class busRHICCombiningLookup : busRHICCombiningLookupGen
    {
        public override ArrayList ValidateNew(Hashtable ahstParam)
        {
            ArrayList larrErrors = new ArrayList();

            if (ahstParam["aint_person_id"].ToString() == "")
            {
                utlError lobjError = null;
                lobjError = AddError(2024, "");
                larrErrors.Add(lobjError);
            }
            else
            {
                DataTable ldtbPerson = busBase.Select<cdoPerson>(new string[1] { "person_id" }, new object[1] { Convert.ToInt32(ahstParam["aint_person_id"]) }, null, null);
                if (ldtbPerson.Rows.Count == 0)
                {
                    utlError lobjError = null;
                    lobjError = AddError(2009, "");
                    larrErrors.Add(lobjError);
                }
                //check date of death
                else
                {
                    if (!String.IsNullOrEmpty(ldtbPerson.Rows[0]["date_of_death"].ToString()))
                    {
                        utlError lobjError = null;
                        lobjError = AddError(1913, "");
                        larrErrors.Add(lobjError);
                    }                    
                }
            }
            return larrErrors;
        }

        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            busBenefitRhicCombine lobjRHICCombine = (busBenefitRhicCombine)aobjBus;
            lobjRHICCombine.ibusPerson = new busPerson();
            lobjRHICCombine.ibusPerson.icdoPerson = new cdoPerson();
            lobjRHICCombine.ibusPerson.icdoPerson.LoadData(adtrRow);
        }
    }
}
