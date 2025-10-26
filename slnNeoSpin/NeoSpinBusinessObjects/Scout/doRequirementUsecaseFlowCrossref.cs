#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sagitec.Common;
using Sagitec.DataObjects;
#endregion

namespace NeoSpin.DataObjects
{
    [Serializable]
    public class doRequirementUsecaseFlowCrossref : doBase
    {
         
         public doRequirementUsecaseFlowCrossref() : base()
         {
         }
		private int _requirement_flow_crossref_id;
		public int requirement_flow_crossref_id
		{
			get
			{
				return _requirement_flow_crossref_id;
			}

			set
			{
				_requirement_flow_crossref_id = value;
			}
		}

		private int _requirement_id;
		public int requirement_id
		{
			get
			{
				return _requirement_id;
			}

			set
			{
				_requirement_id = value;
			}
		}

		private int _flow_id;
		public int flow_id
		{
			get
			{
				return _flow_id;
			}

			set
			{
				_flow_id = value;
			}
		}

		private int _update_sequence;
		public int update_sequence
		{
			get
			{
				return _update_sequence;
			}

			set
			{
				_update_sequence = value;
			}
		}

    }
}

