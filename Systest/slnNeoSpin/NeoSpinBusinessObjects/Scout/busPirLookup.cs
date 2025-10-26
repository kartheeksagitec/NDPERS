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
	public class busPirLookup : busMainBase
	{
		private Collection<busPir> _iclbLookupResult;
		public Collection<busPir> iclbLookupResult
		{
			get
			{
				return _iclbLookupResult;
			}

			set
			{
				_iclbLookupResult = value;
			}
		}

		public void LoadPirs(DataTable adtbSearchResult)
		{
			_iclbLookupResult = GetCollection<busPir>(adtbSearchResult, "icdoPir");
		}

		protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
		//This is fired for every datarow found in the search result. 
		//Handle is returned to this method include the datarow and busObject being created
		//
		{
            ((busPir)aobjBus).LoadAssignedTo();
            ((busPir)aobjBus).LoadReportedBy();
            ((busPir)aobjBus).LoadTestCase();
            ((busPir)aobjBus).LoadReferentID();
        }

        public void btnReleaseToSystest(ArrayList aarrSelectedObjects)
        {
            UpdatePirs(aarrSelectedObjects, busConstant.PIRStatusReadyForSystest);
        }

        public void btnReleaseToUAT(ArrayList aarrSelectedObjects)
        {
            UpdatePirs(aarrSelectedObjects, busConstant.PIRStatusReadyForUAT);
        }

        public void btnReleaseToPROD(ArrayList aarrSelectedObjects)
        {
            UpdatePirs(aarrSelectedObjects, busConstant.PIRStatusReadyForPROD);
        }

        public void UpdatePirs(ArrayList aarrSelectedPirs, string astrStatusValue)
        {
            string lstrAdditionalNotes = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(40, astrStatusValue);
            lstrAdditionalNotes = lstrAdditionalNotes.Substring(lstrAdditionalNotes.IndexOf('.') + 1).Trim();
            if (aarrSelectedPirs.IsNotNull() && aarrSelectedPirs.Count > 0)
            {
                foreach (busPir lobjPIR in aarrSelectedPirs)
                {
                    if (lobjPIR.icdoPir.status_value != astrStatusValue)
                    {
                        // Update the Status
                        lobjPIR.icdoPir.status_value = astrStatusValue;
                        lobjPIR.icdoPir.additional_notes = lstrAdditionalNotes;
                        lobjPIR.icdoPir.release_info = DateTime.Today.ToString(busConstant.DateFormatD8);
                        if(astrStatusValue == busConstant.PIRStatusReadyForPROD)
                            lobjPIR.icdoPir.assigned_to_id = 0;
                        lobjPIR.icdoPir.Update();

                        // Send Mail
                        lobjPIR.SendMail();
                    }
                }
            }
        }
	}
}
