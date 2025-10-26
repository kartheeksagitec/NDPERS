using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sagitec.MetaDataCache;
using System.Collections;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using System.Runtime.Remoting;
using Sagitec.DBCache;
using Sagitec.Rules;
using Sagitec.BusinessTier;
using Sagitec.ExceptionPub;
using Sagitec.Interface;
using System.ServiceModel;
using System.Collections.ObjectModel;
using System.Configuration;
using Sagitec.DBUtility;
using Sagitec.Bpm;

namespace NeoSpin.BusinessTier
{
    public partial class frmNeoSpinBusinessTier : Form
    {
        public frmNeoSpinBusinessTier()
        {
            InitializeComponent();
        }

        private busMQRequestHandler iobjRequestHandler = null;
        private busMQRequestHandler _ibusRequestHandler = null;

        private void frmNeoSpinBusinessTier_Load(object sender, EventArgs e)
        {
            srvMetaDataCache.idlgAfterRefresh = new srvMetaDataCache.AfterRefresh(AfterRefresh);
        }

        private void AfterRefresh()
        {
            lblMDC.Text = "MetaData cache successfully refreshed at " + DateTime.Now.ToString();
        }

        private void btnRefreshMetaData_Click(object sender, EventArgs e)
        {
            srvNeoSpinMetaDataCache.LoadXMLCache();
            lblMDC.Text = "MetaData cache successfully refreshed at " + DateTime.Now.ToString();
        }

        private void lsvMetaCacheInfo_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            lsvMetaCacheInfo.ListViewItemSorter = new ListViewItemComparer(e.Column);
            lsvMetaCacheInfo.Sort();
        }

        // Implements the manual sorting of items by columns.
        class ListViewItemComparer : IComparer
        {
            private int col;
            public ListViewItemComparer()
            {
                col = 0;
            }
            public ListViewItemComparer(int column)
            {
                col = column;
            }
            public int Compare(object x, object y)
            {
                return String.Compare(((ListViewItem)x).SubItems[col].Text, ((ListViewItem)y).SubItems[col].Text);
            }
        }

        private void btnRefreshDB_Click(object sender, EventArgs e)
        {
            //srvNeoSpinDBCache.idstDBCache.Clear();
            srvNeoSpinDBCache.idctDBCache.Clear();
            srvNeoSpinDBCache.LoadCacheInfo();

            lblDbCacheMessage.Text = "All queries successfully refreshed and cached at " + DateTime.Now.ToString();
        }

        private ICollection<ServiceHost> icolServiceHosts = new Collection<ServiceHost>();
        private void frmNeoSpinBusinessTier_Shown(object sender, EventArgs e)
        {
            System.ServiceModel.Channels.Binding lntbBinding = ServiceHelper.GetNetTcpBinding(true);

            //ReceiveTimeout property has default value is 00,10,00 which gives exception for reports,
            //Use "WCFRECEIVETIMEOUT" key from app.config to set custom timespan for "ReceiveTimeout" property of WCF (FW Version 6.0.2.3)
            lntbBinding.ReceiveTimeout = TimeSpan.Parse(ConfigurationManager.AppSettings["WCFRECEIVETIMEOUT"]);
            
            string lstrBaseUrl = NeoSpin.Common.ApplicationSettings.Instance.BusinessTierUrl;
            icolServiceHosts.Add(ServiceHelper.GetServiceHost<IBusinessTier>(typeof(srvCommon), string.Format(lstrBaseUrl, "srvCommon"), lntbBinding));
            lblStatusBar.Text = "Loading MetaData Cache....";
            Application.DoEvents();
            srvMetaDataCache.LoadXMLCache();
            foreach (stcCacheInfo lstcCacheInfo in srvNeoSpinMetaDataCache.icolCacheInfo)
            {
                ListViewItem lvi = new ListViewItem(lstcCacheInfo.istrName);
                lvi.SubItems.Add(lstcCacheInfo.istrReplaced);
                lvi.SubItems.Add(lstcCacheInfo.istrType);
                lvi.SubItems.Add(lstcCacheInfo.istrDirectory);
                lsvMetaCacheInfo.Items.Add(lvi);
            }
            lblMDC.Text = "All XML files successfully loaded and cached at " + DateTime.Now.ToString();
            Application.DoEvents();
            System.Threading.Thread.Sleep(2000);

            lblStatusBar.Text = "Loading Database Cache....";
            tbcSagitecServer.SelectedTab = tbpDbCache;
            Application.DoEvents();

            // Raj: Nice to have feature. So proceeding on exceptions
            try
            {
                utlConnection lobjConnection = HelperFunction.GetDBConnectionProperties("core");
                List<string> llstConnectionItems = HelperFunction.SplitQuoted(lobjConnection.istrConnectionString, ";");
                string lstrDataSource = llstConnectionItems.Find(item => item.StartsWith("Data Source"));
                string lstrDatabase = llstConnectionItems.Find(item => item.StartsWith("Initial Catalog"));
                lblDbCacheMessage.Text = string.Format("Server={0} and Database={1}", (lstrDataSource.Split('='))[1], (lstrDatabase).Split('=')[1]);
            }
            catch
            {
                // Raj: Nice to have feature. So proceeding on exceptions
            }

            bool lblnSuccess = srvNeoSpinDBCache.LoadCacheInfo();
            if (lblnSuccess)
            {
                int i = 0;
                foreach (KeyValuePair<string, string> query in srvNeoSpinDBCache.iarrDBCacheInfo)
                {
                    ListViewItem lvi = new ListViewItem(query.Key);
                    lvi.SubItems.Add((srvNeoSpinDBCache.iarrRowCount[i]).ToString().PadLeft(12));
                    lvi.SubItems.Add(query.Value);
                    lsvDbCache.Items.Add(lvi);
                    i++;
                }

                lblDbCacheMessage.Text = "All queries successfully executed and cached at " + DateTime.Now.ToString();
                Application.DoEvents();
                System.Threading.Thread.Sleep(2000);
            }
            else
            {
                MessageBox.Show(srvNeoSpinDBCache.istrResult);
                Close();
            }

            utlPassInfo lobjPassInfo = new utlPassInfo(); // Added for ESS issue. The call to this constructor initializes.
            CompileRules();
            InitializeExtendedObjects();
            tbcSagitecServer.SelectedTab = tbpBusinessTier;
            lblStatusBar.Text = "Completed Intialization";
            Application.DoEvents();

            try
            {
                icolServiceHosts.Add(GetServiceHost<IMetaDataCache>(typeof(srvNeoSpinMetaDataCache), String.Format(lstrBaseUrl, utlConstants.istrMetaDataCache), lntbBinding));
                icolServiceHosts.Add(GetServiceHost<IDBCache>(typeof(srvNeoSpinDBCache), String.Format(lstrBaseUrl, utlConstants.istrDBCache), lntbBinding));
                icolServiceHosts.Add(GetServiceHost<IBusinessTier>(typeof(srvAdmin), string.Format(lstrBaseUrl, "srvAdmin"), lntbBinding));
                icolServiceHosts.Add(GetServiceHost<IBusinessTier>(typeof(srvApplication), String.Format(lstrBaseUrl, "srvApplication"), lntbBinding));
                icolServiceHosts.Add(GetServiceHost<IBusinessTier>(typeof(srvAudit), string.Format(lstrBaseUrl, "srvAudit"), lntbBinding));
                icolServiceHosts.Add(GetServiceHost<IBusinessTier>(typeof(srvBenefitCalculation), string.Format(lstrBaseUrl, "srvBenefitCalculation"), lntbBinding));
                icolServiceHosts.Add(GetServiceHost<IBusinessTier>(typeof(srvCallCenter), string.Format(lstrBaseUrl, "srvCallCenter"), lntbBinding));
                icolServiceHosts.Add(GetServiceHost<IBusinessTier>(typeof(srvCase), string.Format(lstrBaseUrl, "srvCase"), lntbBinding));
                //icolServiceHosts.Add(GetServiceHost<IBusinessTier>(typeof(srvCommon), string.Format(lstrBaseUrl, "srvCommon"), lntbBinding));
                icolServiceHosts.Add(GetServiceHost<IBusinessTier>(typeof(srvCorrespondence), string.Format(lstrBaseUrl, "srvCorrespondence"), lntbBinding));
                icolServiceHosts.Add(GetServiceHost<IBusinessTier>(typeof(srvEmployerReport), string.Format(lstrBaseUrl, "srvEmployerReport"), lntbBinding));
                icolServiceHosts.Add(GetServiceHost<IBusinessTier>(typeof(srvNeoSpinWSS), string.Format(lstrBaseUrl, "srvNeoSpinWSS"), lntbBinding));
                icolServiceHosts.Add(GetServiceHost<IBusinessTier>(typeof(srvOrganization), string.Format(lstrBaseUrl, "srvOrganization"), lntbBinding));
                icolServiceHosts.Add(GetServiceHost<IBusinessTier>(typeof(srvPayment), string.Format(lstrBaseUrl, "srvPayment"), lntbBinding));
                icolServiceHosts.Add(GetServiceHost<IBusinessTier>(typeof(srvPerson), string.Format(lstrBaseUrl, "srvPerson"), lntbBinding));
                icolServiceHosts.Add(GetServiceHost<IBusinessTier>(typeof(srvReports), string.Format(lstrBaseUrl, "srvReports"), lntbBinding));
                icolServiceHosts.Add(GetServiceHost<IBusinessTier>(typeof(srvScout), string.Format(lstrBaseUrl, "srvScout"), lntbBinding));
                icolServiceHosts.Add(GetServiceHost<IBusinessTier>(typeof(srvUserActivity), string.Format(lstrBaseUrl, "srvUserActivity"), lntbBinding));
                //icolServiceHosts.Add(GetServiceHost<IBusinessTier>(typeof(srvWorkflow), string.Format(lstrBaseUrl, "srvWorkflow"), lntbBinding));
                icolServiceHosts.Add(GetServiceHost<IBusinessTier>(typeof(srvBPMN), string.Format(lstrBaseUrl, "srvBPMN"), lntbBinding));

                icolServiceHosts.Add(ServiceHelper.GetServiceHost<IBusinessTier>(Type.GetType("MobiasNeospinExtensions.srvMobiasService,MobiasNeospinExtensions"), string.Format(lstrBaseUrl, "srvMobiasService"), lntbBinding));
            }
            catch (Exception E)
            {
                lblBTMessage.Text = "Business Tier might already be running";
                txbError.Text = E.Message;
                return;
            }

            try
            {
                // Used for System Queues
                _ibusRequestHandler = new busMQRequestHandler(utlConstants.SystemQueue);
                _ibusRequestHandler.idlgFileProgress = UpdateProcessLog;
                _ibusRequestHandler.StartProcessing();
            }
            catch (Exception ex)
            {
                ExceptionManager.Publish(ex);
                MessageBox.Show("Following Error occured while instantiating MQHandler processer : " + ex.Message);
            }

            lblStatusBar.Text = "Ready";
            srvMainDB.iblnBTReady = true;
            // This is used by the Start/Stop service of Controller
            this.Text += " - STARTED";

            #region 5.0 Commented Code
            //lblStatusBar.Text = "Loading MetaData Cache....";
            //Application.DoEvents();
            //srvMetaDataCache.LoadXMLCache();
            //foreach (stcCacheInfo lstcCacheInfo in srvNeoSpinMetaDataCache.icolCacheInfo)
            //{
            //    ListViewItem lvi = new ListViewItem(lstcCacheInfo.istrName);
            //    lvi.SubItems.Add(lstcCacheInfo.istrReplaced);
            //    lvi.SubItems.Add(lstcCacheInfo.istrType);
            //    lvi.SubItems.Add(lstcCacheInfo.istrDirectory);
            //    lsvMetaCacheInfo.Items.Add(lvi);
            //}
            //lblMDC.Text = "All XML files successfully loaded and cached at " + DateTime.Now.ToString();
            //Application.DoEvents();
            //System.Threading.Thread.Sleep(2000);

            //lblStatusBar.Text = "Loading Database Cache....";
            //tbcSagitecServer.SelectedTab = tbpDbCache;
            //Application.DoEvents();
            //bool lblnSuccess = srvNeoSpinDBCache.LoadCacheInfo();
            //if (lblnSuccess)
            //{
            //    //Looping through iarrDBCacheInfo values
            //    int lintCounter = 0;
            //    foreach (KeyValuePair<string, string> query in srvNeoSpinDBCache.iarrDBCacheInfo)
            //    {
            //        ListViewItem lvi = new ListViewItem(query.Key);
            //        lvi.SubItems.Add((srvNeoSpinDBCache.iarrRowCount[lintCounter]).ToString().PadLeft(12));
            //        lvi.SubItems.Add(query.Value);
            //        lsvDbCache.Items.Add(lvi);
            //        lintCounter++;
            //    }
            //    lblDbCacheMessage.Text = "All queries successfully executed and cached at " + DateTime.Now.ToString();
            //    Application.DoEvents();
            //    System.Threading.Thread.Sleep(2000);
            //}
            //else
            //{
            //    MessageBox.Show(srvNeoSpinDBCache.istrResult);
            //    Close();
            //}

            //tbcSagitecServer.SelectedTab = tbpBusinessTier;
            //lblStatusBar.Text = "Completed Intialization";
            //Application.DoEvents();
            //try
            //{
            //    RemotingConfiguration.Configure("NeoSpinBusinessTier.exe.config", false);
            //}
            //catch (Exception E)
            //{
            //    lblBTMessage.Text = "Business Tier might already be running";
            //    txbError.Text = E.Message;
            //    return;
            //}
            //utlPassInfo lobjPassInfo = new utlPassInfo(); // Added for ESS issue. The call to this constructor initializes.
            //CompileRules();
            //lblStatusBar.Text = "Ready";
            //bgwAuditLog.RunWorkerAsync();
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
        private ServiceHost GetServiceHost<T>(Type atypObject, string astrUrl, System.ServiceModel.Channels.Binding antBinding)
        {
            ServiceHost lsrvHost = ServiceHelper.GetServiceHost<T>(atypObject, astrUrl, antBinding);
            return lsrvHost;
        }

        private void bgwAuditLog_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                iobjRequestHandler = new busMQRequestHandler(utlConstants.SystemQueue);
                iobjRequestHandler.StartProcessing();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Following Error occured while instantiating AuditLogQueue processer : " + ex.Message);
            }
        }

        private void frmVNAVBusinessTier_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (iobjRequestHandler != null)
                iobjRequestHandler.StopProcessing();
        }

        /// <summary>
        /// Refresh Rules
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="aEventArgs"></param>
        private void btnRefreshRules_Click(object sender, EventArgs e)
        {
            try
            {
                btnRefreshMetaData_Click(sender, e);
                CompileRules();
            }
            catch (Exception er)
            {
                MessageBox.Show("error occured in loading business rules " + er.Message + "\n" + er.StackTrace.ToString());
            }
        }

        /// <summary>
        /// Compile Rules
        /// </summary>
        private static void CompileRules()
        {
            try
            {
                ParsingResult lobjLoadResult = srvMainDB.LoadRulesAndExpressions();
                BuildResult lBuildResult = RulesEngine.CompileRules(true);
                BuildResult lBuildResult2 = RulesEngine.CompileRules(false);
                StringBuilder lstrbRulesError = new StringBuilder();
                foreach (utlRuleMessage lobjMessage in lobjLoadResult.ilstErrors)
                {
                    string lstrError = string.Empty;
                    lstrbRulesError.AppendLine(String.Format("Object : {0} Rule : {1} : Message : {2}", lobjMessage.istrObjectID, lobjMessage.istrRuleID, lobjMessage.istrMessage));
                }

                if (lstrbRulesError.ToString().Length > 0)
                    MessageBox.Show(lstrbRulesError.ToString());
            }
            catch (Exception er)
            {
                MessageBox.Show("error occured in loading business rules " + er.Message + "\n" + er.StackTrace.ToString());
            }
        }

        public void UpdateProcessLog(string astrMessage, string astrMessageType, string astrStepName)
        {
            utlPassInfo.iobjPassInfo.BeginTransaction();
            try
            {
                DBFunction.StoreProcessLog(50000, astrStepName, astrMessageType, astrMessage, utlPassInfo.iobjPassInfo.istrUserID,
                    utlPassInfo.iobjPassInfo.iconFramework, utlPassInfo.iobjPassInfo.itrnFramework);
                utlPassInfo.iobjPassInfo.Commit();
            }
            catch (Exception ex)
            {
                utlPassInfo.iobjPassInfo.Rollback();
                MessageBox.Show("UpdateProcessLog failed with following error: " + ex.Message);
            }
        }

    }
}
