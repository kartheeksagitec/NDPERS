using System;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Forms;

namespace NeoSpin.BusinessTier
{
	static class EntryPoint
	{
        //static Mutex mutex = new Mutex(false, "PerslinkBusinessTier");
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
		static void Main(string[] args)
		{
            //if (!mutex.WaitOne(TimeSpan.FromSeconds(5), false))
            //{
            //    MessageBox.Show("Another instance of BusinessTier is already running..");
            //    return;
            //}

            try
            {
                bool blnConsoleMode = false;//System.Diagnostics.Debugger.IsAttached;
#if DEBUG
                blnConsoleMode = true;
#endif
                if (args.Length > 0)
                {
                    string strParam = args[0].Trim();
                    blnConsoleMode = strParam == "/c" || strParam == "/console" ||
                                     strParam == "-c" || strParam == "-console";
                }

                //Framework Upgrade - 6.0.7.0 - Code will ensure framework will create an object of Solution side class while its loading SystemSettings.
                NeoSpin.Common.ApplicationSettings.MapSettingsObject();

                if (blnConsoleMode)  // Run as Windows application
                {
                    Application.Run(new frmNeoSpinBusinessTier());
                }
                else  // Run as Windows service
                {
                    ServiceBase[] ServicesToRun;
                    ServicesToRun = new ServiceBase[]
                      {
                         new NeoSpinService()
                      };
                    ServiceBase.Run(ServicesToRun);
                }
            }
            finally
            {
              //  mutex.ReleaseMutex();
            }
		}
	}
}
