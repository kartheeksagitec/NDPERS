#region Using directives

using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using NeoSpin.CustomDataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
	[Serializable]
	public class busProcessLogLookup : busMainBase
	{
        private Collection<cdoProcessLog> _icolProcessLog;
        public Collection<cdoProcessLog> icolProcessLog
        {
            get
            {
                return _icolProcessLog;
            }

            set
            {
                _icolProcessLog = value;
            }
        }

		public void LoadProcessLog(DataTable adtbSearchResult)
		{
            _icolProcessLog = new Collection<cdoProcessLog>();
            cdoProcessLog lobjProcessLog;
            foreach (DataRow ldtrData in adtbSearchResult.Rows)
            {
                lobjProcessLog = new cdoProcessLog();
                lobjProcessLog.LoadData(ldtrData);
                _icolProcessLog.Add(lobjProcessLog);
            } 
		}

	}
}
