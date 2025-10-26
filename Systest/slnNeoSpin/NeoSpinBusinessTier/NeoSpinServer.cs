using System;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Runtime.Remoting;
using Sagitec.MetaDataCache;
using Sagitec.Common;
using System.Runtime.Remoting.Channels;
using System.Diagnostics;
using Sagitec.BusinessObjects;
using Sagitec.Rules;
using System.Text;
using Sagitec.BusinessTier;
using System.Collections.Generic;
using System.ServiceModel;
using Sagitec.Interface;
using System.Collections.ObjectModel;
using NeoSpin.Interface;
using Sagitec.Bpm;

namespace NeoSpin.BusinessTier
{
    /// <summary>
    /// Main server application for my service.
    /// </summary>
    /// <remarks>
    /// <para>$Id$</para>
    /// <author>Authors: Sagitec</author>
    /// </remarks>
    public class NeoSpinServer
    {
        private busMQRequestHandler iobjRequestHandler = null;
        private ICollection<ServiceHost> icolServiceHosts;
        /// <summary>
        /// Definitions of the state codes for the server process.
        /// </summary>
        private enum NeoSpinServerState
        {
            /// <summary>The server is not running.</summary>
            Stopped,
            /// <summary>The server is starting.</summary>
            Starting,
            /// <summary>The server is running.</summary>
            Running,
            /// <summary>The server is stopping.</summary>
            Stopping,
        };

        /// <summary>
        /// Time in milliseconds to respond to a <see cref="Start"/> or
        /// <see cref="Stop"/>.
        /// </summary>
        const int SYSRESPONSE_INTERVAL = 10000;

        /// <summary>
        /// The current process state.
        /// </summary>
        private volatile NeoSpinServerState iServerState;

        /// <summary>
        /// Lock object reserved for <see cref="Start"/> and <see cref="Stop"/>.
        /// </summary>
        private readonly object iStartStopLock = new object();

        /// <summary>
        /// Constructor.
        /// </summary>
        public NeoSpinServer()
        {
            this.iServerState = NeoSpinServerState.Stopped;

            // Create event handler to log an unhandled exception before the
            // server application is terminated by the CLR.
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(AppUnhandledExceptionEventHandler);
        }

        /// <summary>
        /// Event handler to log the unhandled exception event and perform an
        /// orderly shutdown before the application is terminated by the CLR.
        /// </summary>
        /// <remarks>
        /// This event handler is called by the CLR on the thread that
        /// encountered the unhandled exception. The code in this handler, as
        /// well as any single-threaded code the handler calls, will be executed
        /// without being suddenly terminated. When done executing this handler,
        /// the CLR immediately and silently terminates the application and all
        /// application threads. However, while executing this handler, the other
        /// threads in the application are still alive and the CLR keeps scheduling
        /// and running them. Plus, the CLR allows the thread executing the handler
        /// to be programmatically controlled. It does not automatically abort the
        /// thread if the handler goes into a tight loop or suspends its thread
        /// (which of course could be abused and allow an application to get into
        /// an unknown state). If code in this handler or code called by this
        /// handler encounters another unhandled exception, this handler is NOT
        /// called again. Instead, the CLR proceeds to terminate the application.
        /// </remarks>
        /// <param name="sender">Reference to object that initiated event.</param>
        /// <param name="ea">Object that holds the event data.</param>
        private void AppUnhandledExceptionEventHandler(object sender, UnhandledExceptionEventArgs ea)
        {
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
            Exception ex = ea.ExceptionObject as Exception;

            // Note: In a Debug build, the system logger writes the log message
            // to all debug listeners before placing the message in the log
            // message queue for logging.
            string strMessage = "Unhandled Exception! Server state: " + iServerState;
            if (ex != null)
            {
                // MySystemLogger.Log(strMessage, ex, "NeoSpinServer.LogUnhandledExceptionEventHandler");
            }
            else
            {
                // MySystemLogger.Log(strMessage, "NeoSpinServer.LogUnhandledExceptionEventHandler");
            }

            // Write to the Windows event log
            EventLog.WriteEntry("NeospinBusinessTier", strMessage, EventLogEntryType.Error);

            // Termination has begun. Resistance is futile! Let's do an
            // orderly shutdown before the CLR terminates the application.
            Stop();

            // TODO: Exit on AppUnhandledExceptionEventHandler is a only a test.
            Environment.Exit(5); // 5 was arbitrarily chosen for this 'Test'.
        }

        /// <summary>
        /// Configures NeoSpinMetaDataCache, NeoSpinDBCache and
        /// NeoSpinBusinessTier services, and loads the cache.
        /// </summary>
        private void StartServers()
        {

            // Get the configuration file path
            string strAssemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            HelperFunction.istrAppSettingsLocation = Path.Combine(strAssemblyLocation, "AppSettings.xml");
            // Load the MetaDataCache and DBCache
            srvNeoSpinMetaDataCache.LoadXMLCache();
            srvNeoSpinDBCache.LoadCacheInfo();
            InitializeExtendedObjects();
            CompileRules();
            // Configure the remoting servers /* FM 6.0.0.30 change */
            //RemotingConfiguration.Configure(Path.Combine(strAssemblyLocation, "NeoSpinBusinessTier.exe.config"), false);
            System.ServiceModel.Channels.Binding lntbBinding = ServiceHelper.GetNetTcpBinding(true);
            lntbBinding.ReceiveTimeout = TimeSpan.Parse("00:59:00");
            string lstrBaseUrl = NeoSpin.Common.ApplicationSettings.Instance.BusinessTierUrl;
            icolServiceHosts = new Collection<ServiceHost>();
            icolServiceHosts.Add(ServiceHelper.GetServiceHost<IMetaDataCache>(typeof(srvNeoSpinMetaDataCache), String.Format(lstrBaseUrl, utlConstants.istrMetaDataCache), lntbBinding));
            icolServiceHosts.Add(ServiceHelper.GetServiceHost<IBusinessTier>(typeof(srvCommon), string.Format(lstrBaseUrl, "srvCommon"), lntbBinding));
            icolServiceHosts.Add(ServiceHelper.GetServiceHost<IDBCache>(typeof(srvNeoSpinDBCache), String.Format(lstrBaseUrl, utlConstants.istrDBCache), lntbBinding));
            icolServiceHosts.Add(ServiceHelper.GetServiceHost<IBusinessTier>(typeof(srvAdmin), string.Format(lstrBaseUrl, "srvAdmin"), lntbBinding));
            icolServiceHosts.Add(ServiceHelper.GetServiceHost<IBusinessTier>(typeof(srvApplication), String.Format(lstrBaseUrl, "srvApplication"), lntbBinding));
            icolServiceHosts.Add(ServiceHelper.GetServiceHost<IBusinessTier>(typeof(srvAudit), string.Format(lstrBaseUrl, "srvAudit"), lntbBinding));
            icolServiceHosts.Add(ServiceHelper.GetServiceHost<IBusinessTier>(typeof(srvBenefitCalculation), string.Format(lstrBaseUrl, "srvBenefitCalculation"), lntbBinding));
            icolServiceHosts.Add(ServiceHelper.GetServiceHost<IBusinessTier>(typeof(srvCallCenter), string.Format(lstrBaseUrl, "srvCallCenter"), lntbBinding));
            icolServiceHosts.Add(ServiceHelper.GetServiceHost<IBusinessTier>(typeof(srvCase), string.Format(lstrBaseUrl, "srvCase"), lntbBinding));
            //icolServiceHosts.AdServiceHelper.d(GetServiceHost<IBusinessTier>(typeof(srvCommon), string.Format(lstrBaseUrl, "srvCommon"), lntbBinding));
            icolServiceHosts.Add(ServiceHelper.GetServiceHost<IBusinessTier>(typeof(srvCorrespondence), string.Format(lstrBaseUrl, "srvCorrespondence"), lntbBinding));
            icolServiceHosts.Add(ServiceHelper.GetServiceHost<IBusinessTier>(typeof(srvEmployerReport), string.Format(lstrBaseUrl, "srvEmployerReport"), lntbBinding));
            icolServiceHosts.Add(ServiceHelper.GetServiceHost<IBusinessTier>(typeof(srvNeoSpinWSS), string.Format(lstrBaseUrl, "srvNeoSpinWSS"), lntbBinding));
            icolServiceHosts.Add(ServiceHelper.GetServiceHost<IBusinessTier>(typeof(srvOrganization), string.Format(lstrBaseUrl, "srvOrganization"), lntbBinding));
            icolServiceHosts.Add(ServiceHelper.GetServiceHost<IBusinessTier>(typeof(srvPayment), string.Format(lstrBaseUrl, "srvPayment"), lntbBinding));
            icolServiceHosts.Add(ServiceHelper.GetServiceHost<IBusinessTier>(typeof(srvPerson), string.Format(lstrBaseUrl, "srvPerson"), lntbBinding));
            icolServiceHosts.Add(ServiceHelper.GetServiceHost<IBusinessTier>(typeof(srvReports), string.Format(lstrBaseUrl, "srvReports"), lntbBinding));
            icolServiceHosts.Add(ServiceHelper.GetServiceHost<IBusinessTier>(typeof(srvScout), string.Format(lstrBaseUrl, "srvScout"), lntbBinding));
            icolServiceHosts.Add(ServiceHelper.GetServiceHost<IBusinessTier>(typeof(srvUserActivity), string.Format(lstrBaseUrl, "srvUserActivity"), lntbBinding));
            //icolServiceHosts.Add(ServiceHelper.GetServiceHost<IBusinessTier>(typeof(srvWorkflow), string.Format(lstrBaseUrl, "srvWorkflow"), lntbBinding));
            icolServiceHosts.Add(ServiceHelper.GetServiceHost<IBusinessTier>(typeof(srvBPMN), string.Format(lstrBaseUrl, "srvBPMN"), lntbBinding));

            icolServiceHosts.Add(ServiceHelper.GetServiceHost<IBusinessTier>(Type.GetType("MobiasNeospinExtensions.srvMobiasService,MobiasNeospinExtensions"), string.Format(lstrBaseUrl, "srvMobiasService"), lntbBinding));

            
            srvMainDB.iblnBTReady = true;   

            iobjRequestHandler = new busMQRequestHandler(utlConstants.SystemQueue);
            iobjRequestHandler.StartProcessing();

            #region 5.0 Commented Code
            // Get the configuration file path
            //string strAssemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            //HelperFunction.istrAppSettingsLocation = Path.Combine(strAssemblyLocation, "AppSettings.xml");

            //string lstrServiceType = NeoSpin.Common.ApplicationSettings.Instance.ServiceType;
            //if (lstrServiceType != null && lstrServiceType.ToUpper() == "WCF")
            //{
            //    ServiceHelper.Initialize(utlServiceType.WCF);
            //}
            //else
            //{
            //    ServiceHelper.Initialize(utlServiceType.Remoting);
            //}

            ////WF 4.0 IMP:We must load this to make sure the workflow loading to work properly.
            ////Assembly designAssembly = Assembly.LoadFrom(@"C:\Program Files\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\System.Activities.Design.dll");

            //RemotingConfiguration.Configure(Path.Combine(strAssemblyLocation, "NeoSpinBusinessTier.exe.config"), false);

            //srvMetaDataCache.LoadXMLCache();
            //Sagitec.DBCache.srvDBCache.LoadCacheInfo();
            //CompileRules();

            //iobjRequestHandler = new busMQRequestHandler(utlConstants.SystemQueue);
            //iobjRequestHandler.StartProcessing();
            #endregion
        }

        internal static void InitializeExtendedObjects()
        {
            bool iblnTemp = Sagitec.Bpm.BpmHelper.iblnDebugMode;
            if (utlPassInfo.iobjPassInfo == null)
            {
                utlPassInfo.iobjPassInfo = Sagitec.Bpm.BPMDBHelper.CreatePassInfo("Bpm Service");
            }
            busBpmCaseInstance lbusBpmCaseInstance = ClassMapper.GetObject<busBpmCaseInstance>();
            busBpmProcessInstance lbusBpmProcessInstance = ClassMapper.GetObject<busBpmProcessInstance>();
            busBpmActivityInstance lbusBpmActivityInstance = ClassMapper.GetObject<busBpmActivityInstance>();
            busBpmRequest ExtendedEmptyObject = ClassMapper.GetObject<busBpmRequest>();
            busBpmTimerActivityInstanceDetails lbusBpmTimerActivityInstanceDetails = ClassMapper.GetObject<busBpmTimerActivityInstanceDetails>();
            busBpmProcessEscalationInstance lbusBpmProcessEscalationInstance = ClassMapper.GetObject<busBpmProcessEscalationInstance>();
            busBpmEscalationInstance lbusBpmEscalationInstance = ClassMapper.GetObject<busBpmEscalationInstance>();

        }
        /// <summary>
        /// Clears the cache, and stops the remoting services.
        /// </summary>
        private void StopServers()
        {
            if (iobjRequestHandler != null)
                iobjRequestHandler.StopProcessing();

            //// Clear all the cache
            //if (srvNeoSpinDBCache.idstDBCache != null)
            //    srvNeoSpinDBCache.idstDBCache.Clear();

            // Stop all remoting services
            foreach (IChannel objChannel in ChannelServices.RegisteredChannels)
                ChannelServices.UnregisterChannel(objChannel);
        }

        /// <summary>
        /// A sample routine where you'll start your server work
        /// </summary>
        private void DoSomeWork()
        {
            // Do some work here. For example start listening to some event on
            // a serial port or start a tcp listener or start watching your
            // directories and files on your machine etc
            // Or wire up event listeners

            StartServers();
        }

        // *********************************************************************
        /// <summary>
        /// A sample routine where you'll start your server work
        /// </summary>
        private void StopDoingWork()
        {
            // Stop doing your sample work here. For example stop listening to the
            // event on your serial port or stop the tcp listener or stop watching your
            // directories and files on your machine etc. that you started in
            // DoSomeWork or un-wire your event listeners

            StopServers();
        }

        /// <summary>
        /// Starts the logging subsystem for the server, the server's system
        /// logger, and the audit logger.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Startup of the logging subsystem creates a static work queue that
        /// is used by all loggers.
        /// </para>
        /// <para>
        /// Important server process information is written to the system log
        /// at startup.
        /// </para>
        /// </remarks>
        private void StartLogging()
        {
            // Starup your custom logging here in some text file etc.
        }

        /// <summary>
        /// Startup the server subsystems.
        /// </summary>
        /// <exception cref="Exception">Exceptions are captured in <c>Startup</c>
        /// only to update state info or log additional details.
        /// The <see cref="Exception"/> must be rethrown so that the OS service
        /// control routines can handle it.</exception>
        private void Startup()
        {
            // Exit if not in "Starting" state
            if (this.iServerState != NeoSpinServerState.Starting)
                return;

            try
            {
                // Startup our logging.
                StartLogging();

                this.iServerState = NeoSpinServerState.Running;

                // Now start doing some work
                DoSomeWork();
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("NeoSpin Service", "Error starting NeoSpin servers: " + ex.Message, EventLogEntryType.Error);

                throw;
            }

            //Debug.Print("NeoSpinServer.Startup has finished.");
        }

        /// <summary>
        /// Shutdown the server subsystems.
        /// </summary>
        private void Shutdown()
        {
            // Exit if not in "Stopping" state
            if (this.iServerState != NeoSpinServerState.Stopping)
                return;

            try
            {
                StopDoingWork();
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("NeoSpin Service", "Error stopping NeoSpin servers: " + ex.Message, EventLogEntryType.Error);
            }
            finally
            {
                this.iServerState = NeoSpinServerState.Stopped;
            }

            //Debug.Print("NeoSpinServer.Shutdown has finished.");
        }

        /// <summary>
        /// Start the Server.
        /// </summary>
        /// <remarks>
        /// Start the subsystems needed to run the server.
        /// </remarks>
        public void Start()
        {
            if (this.iServerState == NeoSpinServerState.Stopped)
            {
                this.iServerState = NeoSpinServerState.Starting;

                lock (this.iStartStopLock)
                {
                    // DEVNOTE: We do not want any exceptions to be caught at
                    // this level because they are considered fatal and we want
                    // the service to shutdown so that it can be restarted.
                    Thread startup = new Thread(Startup);
                    startup.Priority = ThreadPriority.AboveNormal;
                    startup.Name = "NeoSpin Server Startup";
                    startup.Start();
                    startup.IsBackground = true;
                    startup.Join(SYSRESPONSE_INTERVAL);
                }
            }
        }

        /// <summary>
        /// Stops the Server.
        /// </summary>
        /// <remarks>
        /// Stops the server and its subsystems.
        /// </remarks>
        public void Stop()
        {
            if (this.iServerState == NeoSpinServerState.Starting || this.iServerState == NeoSpinServerState.Running)
            {
                this.iServerState = NeoSpinServerState.Stopping;

                lock (this.iStartStopLock)
                {
                    Thread shutdown = new Thread(Shutdown);
                    shutdown.Priority = ThreadPriority.Highest;
                    shutdown.Name = "NeoSpin Server Shutdown";
                    shutdown.IsBackground = true;
                    shutdown.Start();
                    shutdown.Join(SYSRESPONSE_INTERVAL);
                }
            }
        }

        private static void CompileRules()
        {
            try
            {
                ParsingResult lobjLoadResult = srvMainDB.LoadRulesAndExpressions();
                StringBuilder lstrRulesError = new StringBuilder();
                foreach (utlRuleMessage lobjMessage in lobjLoadResult.ilstErrors)
                {
                    string lstrerror = string.Empty;
                    lstrRulesError.AppendLine(String.Format("Object : {0} Rule : {1} : Message : {2}", lobjMessage.istrObjectID, lobjMessage.istrRuleID, lobjMessage.istrMessage));
                }
            }
            catch (Exception er)
            {
            }
        }
    }
}
