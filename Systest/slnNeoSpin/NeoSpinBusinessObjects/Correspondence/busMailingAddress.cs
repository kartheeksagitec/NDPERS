#region Using directives

using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
    public class busMailingAddress : busExtendBase
    {
        public busMailingAddress()
        {
        }

        private cdoMailingAddress _icdoMailingAddress;
        public cdoMailingAddress icdoMailingAddress
        {
            get { return _icdoMailingAddress; }
            set { _icdoMailingAddress = value; }
        }
    }
}
