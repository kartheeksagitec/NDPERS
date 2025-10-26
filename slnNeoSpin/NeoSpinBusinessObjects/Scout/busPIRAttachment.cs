using Sagitec.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busPIRAttachment : busExtendBase
    {

        public busPIRAttachment()
        {
        }

        private string _istrAttchmentName;
        public string istrAttchmentName
        {
            get
            {
                return _istrAttchmentName;
            }
            set
            {
                _istrAttchmentName = value;
            }
        }

        private DateTime _idtmAttchmentLastModifiedDate;
        public DateTime idtmAttchmentLastModifiedDate
        {
            get
            {
                return _idtmAttchmentLastModifiedDate;
            }
            set
            {
                _idtmAttchmentLastModifiedDate = value;
            }
        }
        private Double _idblAttchmentSize;
        public Double idblAttchmentSize
        {
            get
            {
                return _idblAttchmentSize;
            }
            set
            {
                _idblAttchmentSize = value;
            }
        }
        private string _istrAttchmentType;
        public string istrAttachmentType
        {
            get
            {
                return _istrAttchmentType;
            }
            set
            {
                _istrAttchmentType = value;
            }
        }
    }
}
