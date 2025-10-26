using System;
using System.Collections.Generic;
using System.Text;
using NeoSpin.BusinessObjects;
using Sagitec.BusinessObjects;
using System.Collections;
using System.Collections.ObjectModel;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.CorBuilder;
using Sagitec.DataObjects;
using System.Data;
using Sagitec.CustomDataObjects;
using NeoSpin.CustomDataObjects;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busExecuteDeathMatchFileOut : busFileBaseOut
    {
        public busExecuteDeathMatchFileOut()
        { }

        //Property to contain all person records
        public Collection<busPerson> iclbPerson { get; set; }

        public override void InitializeFile()
        {
            istrFileName = "NDPERS" + DateTime.Now.ToString(busConstant.DateFormat) + busConstant.FileFormattxt;
        }

        /// <summary>
        /// Method which takes the person details from batch.
        /// </summary>
        public void LoadPersonDetails(DataTable ldtPerson)
        {
            if (iclbPerson == null)
                iclbPerson = new Collection<busPerson>();
            iclbPerson = (Collection<busPerson>)iarrParameters[0];            
        }        
    }
}
