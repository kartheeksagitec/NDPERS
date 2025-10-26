#region [Using Directives]
using Sagitec.Common;
using System;
using System.Runtime.Serialization;
#endregion [Using Directives]

namespace NeoBase.Common
{
    [Serializable]
    public class NeoBaseApplicationSettings : SystemSettings
    {
        public NeoBaseApplicationSettings() : base()
        {

        }

        public new static NeoBaseApplicationSettings Instance
        {
            get
            {
                return SystemSettings.Instance as NeoBaseApplicationSettings;
            }
        }

        static NeoBaseApplicationSettings()
        {
            InitializeSettingsObject = delegate () { return new NeoBaseApplicationSettings(); };
        }

        public NeoBaseApplicationSettings(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt)
        {

        }

        public new static void MapSettingsObject()
        {
            InitializeSettingsObject = delegate () { return new NeoBaseApplicationSettings(); };
        }

    }
}
