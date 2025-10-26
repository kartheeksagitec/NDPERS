#region Using directives

using Sagitec.Bpm;
using System;
using System.Collections.ObjectModel;
using System.Data;

#endregion

namespace NeoBase.BPM
{
    [Serializable]
    public class busNeobaseBpmActivity : busBpmActivity
    {

        /// <summary>
        /// Collection for Activity Checklist.
        /// </summary>

        public busNeobaseBpmActivity()
            : base()
        {
            iclbBpmActivityChecklist = new Collection<busBpmActivityChecklist>();
        }
      

        /// <summary>
        /// Changes for 6.0.7.1 framework
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public busBpmActvChklGroup GetGroupData(string id)
        {
            int groupid = 0;
            var lbusBpmActivityChecklistGroup = new busBpmActvChklGroup();
            int.TryParse(id, out groupid);
            lbusBpmActivityChecklistGroup.FindByPrimaryKey(groupid);
            if (!int.TryParse(id, out groupid))

                lbusBpmActivityChecklistGroup.icdoBpmActvChklGroup.group_name = id;
            return lbusBpmActivityChecklistGroup;
        }
        /// <summary>
        /// LoadUserRoles
        /// </summary>
        //public new void LoadBpmActivityChecklist()
        //{
        //    DataTable ldtbList = Select("entBpmActivityChecklist.ActiveActivityChecklist", new object[] { icdoBpmActivity.activity_id });
        //    iclbBpmActivityChecklist = GetCollection<busBpmActivityChecklist>(ldtbList, "icdoBpmActivityChecklist");
        //}
    }
}
