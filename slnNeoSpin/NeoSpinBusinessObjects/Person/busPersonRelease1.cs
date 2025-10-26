#region Using directives

using System;

#endregion


namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busPersonRelease1 : busPerson
    {
        public override long iintPrimaryKey
        {
            get
            {
                if (iobjPassInfo?.istrSenderID == "btnSave" && iobjPassInfo?.istrFormName == "wfmPersonIndexingMaintenance")
                {
                    return 0;
                }
                return base.iintPrimaryKey;
            }
        }

    }
}
