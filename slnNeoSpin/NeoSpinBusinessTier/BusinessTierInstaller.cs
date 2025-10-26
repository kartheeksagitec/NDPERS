using System;
using System.Collections;
using System.ComponentModel;
using System.ServiceProcess;
using System.Configuration.Install;
using System.Configuration;

namespace NeoSpin.BusinessTier
{
    [RunInstaller(true)]
    public partial class BusinessTierInstaller : System.Configuration.Install.Installer
    {
        public BusinessTierInstaller()
        {
            InitializeComponent();

            ServiceProcessInstaller serviceProcessInstaller = new ServiceProcessInstaller();
            ServiceInstaller serviceInstaller = new ServiceInstaller();

            // Service Account Information
            serviceProcessInstaller.Account = ServiceAccount.LocalSystem;
            serviceProcessInstaller.Username = null;
            serviceProcessInstaller.Password = null;

            // Service Information
            serviceInstaller.ServiceName = ConfigurationManager.AppSettings["Mutex"];// "PERSLink NeospinBusinessTier";
            serviceInstaller.DisplayName = ConfigurationManager.AppSettings["Mutex"]; ;// "NeospinBusinessTier";
            serviceInstaller.Description = "Provides access to 'NeoSpinMetaDataCache', 'NeoSpinDBCache' " +
                "and 'NeoSpinBusinessTier' services.";

            serviceInstaller.StartType = ServiceStartMode.Automatic;

            this.Installers.Add(serviceProcessInstaller);
            this.Installers.Add(serviceInstaller);
        }

        protected override void OnBeforeInstall(IDictionary savedState)
        {
            base.OnBeforeInstall(savedState);

            // If you want to customize the name/description/accounttype etc
            // you can do it here.
        }

        /// <summary>
        /// Uninstall based on the service name
        /// </summary>
        /// <param name="savedState"></param>
        protected override void OnBeforeUninstall(IDictionary savedState)
        {
            base.OnBeforeUninstall(savedState);

            // Set the service name based on the input custom name as in
            // OnBeforeInstall
        }
    }
}
