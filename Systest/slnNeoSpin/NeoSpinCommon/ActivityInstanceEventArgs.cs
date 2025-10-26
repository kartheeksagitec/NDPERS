using System;
using NeoSpin.CustomDataObjects;

namespace NeoSpin.Common
{
    [Serializable]
    public class ActivityInstanceEventArgs
    {
        public ActivityInstanceEventArgs()
        {
        }
        public cdoActivityInstance icdoActivityInstance { get; set; }
        public enmNextAction ienmNextAction { get; set; }
    }

    [Serializable]
    public enum enmNextAction
    {
        Next,
        Previous,
        First,
        Return,
        Correspondance,
        Cancel,
        ReturnBack
    }
}
