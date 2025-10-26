using Sagitec.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using NeoBase.Common;

namespace NeoSpin.Common
{
    [Serializable]
    public class ApplicationSettings : NeoBaseApplicationSettings
    {
        /// <summary>
        /// Project application settings
        /// </summary>
        public new static ApplicationSettings Instance
        {
            get
            {
                return SystemSettings.Instance as ApplicationSettings;
            }
        }

        static ApplicationSettings()
        {
            SystemSettings.InitializeSettingsObject = delegate () { return new ApplicationSettings(); };
        }

        public static void MapSettingsObject()
        {
            SystemSettings.InitializeSettingsObject = delegate () { return new ApplicationSettings(); };
        }

        protected ApplicationSettings() : base()
        {

        }

public ApplicationSettings(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt)
{
}

public override void GetObjectData(SerializationInfo info, StreamingContext context)
{
    base.GetObjectData(info, context);
}


        public string ServiceType { get; protected set;}

        public string NEOFLOW_SERVICE_TIER_URL { get; protected set;}

        public string WSSRetrieveMailFrom { get; protected set;}

        public string WSSEmailNotificationMsg { get; protected set;}

        public string WSSEmailNotificationSubject { get; protected set;}

        public string WSSMailBodySignature { get; protected set;}

        public string RetireeChangeNotice { get; protected set;}

        public string OBJECT_STORE { get; protected set;}

        public string FILENET_CONTENT_ENGINE_WS_API_URL { get; protected set;}

        public string NDPERS_USERNAME { get; protected set;}

        public string NDPERS_PASSWORD { get; protected set;}

        public string USPS_WebService_PATH { get; protected set;}

        public string USPS_WebService_USERID { get; protected set;}

        public string WSSActivationCodeEmailSubject { get; protected set;}

        public string WSSActivationCodeEmailMsg { get; protected set;}

        public string WSSMailFrom { get; protected set;}

        public string NeoFlowMapPath { get; protected set;}

        public string WSSEmailChangeEmailSubject { get; protected set;}

        public string WSSEmailChangeEmailMsg { get; protected set;}

        public string WSSMailSubject { get; protected set;}

        public string WSSMailBody { get; protected set;}

        public bool IsLoggingEnabled { get; protected set; }
        public string SmtpServers { get; set; }
        public string MSSEmailServerNotReachableMsg { get; set; }
        public int SmtpTimeOut { get; set; }
    }
}
