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
using NeoSpin.CustomDataObjects;
#endregion
namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// partial Class NeoSpin.busScreenNotes:
    /// </summary>
	[Serializable]
    public partial class busScreenNotes : busExtendBase
    {
        public busContact ibusContact { get; set; }

        /// <summary>
        /// Constructor for NeoSpin.busScreenNotes
        /// </summary>
        public busScreenNotes()
        {
        }

        #region public Methods
        /// <summary>
        /// Method returns the collection of Notes filter by screen.
        /// </summary>
        /// <param name="astrScreenValue">Screen name from where notes functionality implement</param>
        /// <param name="aintScreenPrimaryId">Screen Primary Key use as forign key</param>
        /// <returns></returns>
        public Collection<busScreenNotes> LoadScreenNotes(string astrScreenValue, int aintScreenPrimaryId)
        {
            DataTable ldtbScreenNotesList = Select<doScreenNotes>(
                new string[2] { enmScreenNotes.screen_value.ToString(), enmScreenNotes.screen_primary_id.ToString() },
                new object[2] { astrScreenValue, aintScreenPrimaryId }, null, "screen_notes_id desc");
            return GetCollection<busScreenNotes>(ldtbScreenNotesList);
        }
        #endregion
    }
}
