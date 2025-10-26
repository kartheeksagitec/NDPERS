using System;
using Sagitec.BusinessObjects;
using System.Collections;
using Sagitec.Common;
using System.Data;
using NeoSpin.CustomDataObjects;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busTFFRSSNInboundFile : busSoFileBase
    {
        public busTFFRSSNInboundFile()
        { }

        //property to store the data read from file
        public busPerson iobjPerson { get; set; }

        //property to store all person details who come in death match criteria
        public DataTable idtPerson { get; set; }
        public override busBase NewDetail()
        {
            iobjPerson = new busPerson { icdoPerson = new cdoPerson() };
            return iobjPerson;
        }
        public override void InitializeFile()
        {
            base.InitializeFile();
            
        }
        
    }
}