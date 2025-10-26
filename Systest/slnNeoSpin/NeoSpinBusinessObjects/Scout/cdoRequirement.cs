#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using NeoSpin.DataObjects;

#endregion

namespace NeoSpin.CustomDataObjects
{
    [Serializable]
	public class cdoRequirement : doRequirement
	{
		public cdoRequirement() : base()
		{
		}
        private String _Parent_Requirement_Key;
        public String Parent_Requirement_Key
        {
            get { return _Parent_Requirement_Key; }
            set { _Parent_Requirement_Key = value; }
        }
        
        public override int Update()
        {
            InsertToRequirementHistory();            
            return base.Update();
        }

        public override int Insert()
        {
            int lintResult = base.Insert();
            InsertToRequirementHistory();            
            return lintResult;
        }

        public void InsertToRequirementHistory()
        {
            //Create a cdoRequirementHistory and keep it ready 
            //Insert into history
            cdoRequirementHistory lobjRequirementHistory = new cdoRequirementHistory();
            lobjRequirementHistory.requirement_id = requirement_id;
            lobjRequirementHistory.requirement_key = requirement_key;
            lobjRequirementHistory.parent_requirement_id = parent_requirement_id;
            lobjRequirementHistory.requirement_desc = requirement_desc;
            lobjRequirementHistory.requirement_short_desc = requirement_short_desc;
            lobjRequirementHistory.status_value = status_value;
            lobjRequirementHistory.category_value = category_value;
            lobjRequirementHistory.requirement_type_value = requirement_type_value;
            lobjRequirementHistory.priority = priority;
            lobjRequirementHistory.owner = owner;            
            lobjRequirementHistory.file_location = file_location;
            lobjRequirementHistory.notes = notes;
            lobjRequirementHistory.created_by = created_by;
            lobjRequirementHistory.modified_by = iobjPassInfo.istrUserID;
            lobjRequirementHistory.created_date = created_date;
            lobjRequirementHistory.modified_date = DateTime.Now;
            lobjRequirementHistory.Insert();
        }

    } 
} 
