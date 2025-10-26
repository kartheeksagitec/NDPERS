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
	/// Class NeoSpin.BusinessObjects.busUserActivityLog:
	/// Inherited from busUserActivityLogGen, the class is used to customize the business object busUserActivityLogGen.
	/// </summary>
	[Serializable]
	public class busUserActivityLog : busUserActivityLogGen
    {
        #region Public Methods

        /// <summary>
        /// Author                  : Framwork Team
        /// Modified By             : Gaurav Patadiya
        /// Applies To Use cases    : Security
        /// Usage                   : Update Logoff Time
        /// </summary>
        /// <param name="astrSessionId"></param>
        /// <returns></returns>
        public bool updateLogoffTime(string astrSessionId)
        {
            busUserActivityLog lobjbusUserActivityLog = new busUserActivityLog();

            DataTable ldtbList = Select<cdoUserActivityLog>(
               new string[1] { enmUserActivityLog.session_id.ToString() },
               new object[1] { astrSessionId }, null, null);

            if (ldtbList != null && ldtbList.Rows != null && ldtbList.Rows.Count > 0)
            {
                lobjbusUserActivityLog.icdoUserActivityLog = new cdoUserActivityLog();
                lobjbusUserActivityLog.icdoUserActivityLog.LoadData(ldtbList.Rows[0]);

                lobjbusUserActivityLog.icdoUserActivityLog.logoff_time = DateTime.Now;
                lobjbusUserActivityLog.icdoUserActivityLog.Update();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Author                  : Framwork Team
        /// Modified By             : NA
        /// Applies To Use cases    : Security
        /// Usage                   : Find User Activity Log
        /// </summary>
        /// <param name="aintUserActivityLogId">User Activity Log Id</param>
        /// <returns>True or False</returns>
        public override bool FindUserActivityLog(int aintUserActivityLogId)
        {
            bool lblnReult= base.FindUserActivityLog(aintUserActivityLogId);

            icdoUserActivityLog.activity_count = SelectCount<cdoUserActivityLogDetail>(new string[1] { enmUserActivityLogDetail.user_activity_log_id.ToString() },
                new object[1] { icdoUserActivityLog.user_activity_log_id }, null);

            return lblnReult;
        }

        #endregion
    }
}
