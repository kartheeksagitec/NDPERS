#region Using directives

using System;
using NeoSpin.DataObjects;
using NeoSpin.BusinessObjects;

#endregion

namespace NeoSpin.CustomDataObjects
{
	/// <summary>
	/// Class NeoSpin.CustomDataObjects.cdoWssBenAppDisaSicknessOrInjury:
	/// Inherited from doWssBenAppDisaSicknessOrInjury, the class is used to customize the database object doWssBenAppDisaSicknessOrInjury.
	/// </summary>
    [Serializable]
	public class cdoWssBenAppDisaSicknessOrInjury : doWssBenAppDisaSicknessOrInjury
	{
		public cdoWssBenAppDisaSicknessOrInjury() : base()
		{
		}
        public string is_bed_confined_desc
        {
            get
            {
                if (is_bed_confined == busConstant.Flag_Yes)
                    return busConstant.Flag_Yes_Value;
                return busConstant.Flag_No_Value;
            }
        }
        public string is_house_confined_desc
        {
            get
            {
                if (is_house_confined == busConstant.Flag_Yes)
                    return busConstant.Flag_Yes_Value;
                return busConstant.Flag_No_Value;
            }
        }
        public string did_you_have_same_kind_of_sick_or_inj_desc
        {
            get
            {
                if (did_you_have_same_kind_of_sickness_or_injury == busConstant.Flag_Yes)
                    return busConstant.Flag_Yes_Value;
                return busConstant.Flag_No_Value;
            }
        }
        public string was_accident_work_related_desc
        {
            get
            {
                if (was_accident_work_related == busConstant.Flag_Yes)
                    return busConstant.Flag_Yes_Value;
                return busConstant.Flag_No_Value;
            }
        }
    } 
} 
