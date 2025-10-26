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

#endregion

namespace NeoSpin.BusinessObjects
{
	[Serializable]
	public class busPlanRetirementRate : busPlanRetirementRateGen
	{
        //PIR 25920 New Plan DC 2025
        public string istrEmploymentTypeValue { get; set; }
        private utlCollection<cdoPlanMemberTypeCrossref> _iclbPlanMemberTypeCrossref;
        public utlCollection<cdoPlanMemberTypeCrossref> iclbPlanMemberTypeCrossref
        {
            get { return _iclbPlanMemberTypeCrossref; }
            set { _iclbPlanMemberTypeCrossref = value; }
        }
        
        public utlCollection<cdoPlanMemberTypeCrossref> iclbPlanMemberTypeCrossrefForDelete{get;set;}
        public void LoadPlanMemberTypeCrossref()
        {
            _iclbPlanMemberTypeCrossref = GetCollection<cdoPlanMemberTypeCrossref>(
                new string[2] { "plan_id", "member_type_value" }, new object[2] { icdoPlanRetirementRate.plan_id, icdoPlanRetirementRate.member_type_value }, null, null);
            if(_iclbPlanMemberTypeCrossref.IsNotNull() && _iclbPlanMemberTypeCrossref.Count > 0)
                istrEmploymentTypeValue = _iclbPlanMemberTypeCrossref[0].employment_type_value;
            //get another collection to delete previous records only
            iclbPlanMemberTypeCrossrefForDelete = GetCollection<cdoPlanMemberTypeCrossref>(
                new string[2] { "plan_id", "member_type_value" }, new object[2] { icdoPlanRetirementRate.plan_id, icdoPlanRetirementRate.member_type_value }, null, null);
        }
        
        public override int PersistChanges()
        {
            //iclbPlanMemberTypeCrossref.user_id = busGlobalFunctions.ToTitleCase(iobjPassInfo.istrUserID);
            if (icdoPlanRetirementRate.ienuObjectState == ObjectState.Insert)
                icdoPlanRetirementRate.Insert();
            if (icdoPlanRetirementRate.ienuObjectState == ObjectState.Update)
                icdoPlanRetirementRate.Update();
            //delete previous all records from collection and from DB
            if(iclbPlanMemberTypeCrossrefForDelete.IsNotNull())
                iclbPlanMemberTypeCrossrefForDelete.ForEach(lobjPlanMemberTypeCrossref => lobjPlanMemberTypeCrossref.Delete());
            //this collection will only get the checked values. hence we delete previous saved all records and insert all again
            foreach (cdoPlanMemberTypeCrossref lobjPlanMemberTypeCrossref in iclbPlanMemberTypeCrossref)
            {
                if (lobjPlanMemberTypeCrossref.ienuObjectState != ObjectState.CheckListDelete)
                {
                    lobjPlanMemberTypeCrossref.plan_id = icdoPlanRetirementRate.plan_id;
                    lobjPlanMemberTypeCrossref.member_type_value = icdoPlanRetirementRate.member_type_value;
                    lobjPlanMemberTypeCrossref.employment_type_value = istrEmploymentTypeValue;

                    lobjPlanMemberTypeCrossref.Insert();
                }
                
            }

            return 1;
        }
        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            LoadPlanMemberTypeCrossref();
        }
    }
}
