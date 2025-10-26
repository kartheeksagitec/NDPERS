#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using NeoSpin.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;

#endregion

namespace NeoSpin.BusinessObjects
{
	[Serializable]
	public class busResources : busExtendBase
    {
		public busResources()
		{
		}

		private cdoResources _icdoResources;
		public cdoResources icdoResources
		{
			get
			{
				return _icdoResources;
			}

			set
			{
				_icdoResources = value;
			}
		}

		private Collection<busSecurity> _iclbSecurity;
		public Collection<busSecurity> iclbSecurity
		{
			get
			{
				return _iclbSecurity;
			}

			set
			{
				_iclbSecurity = value;
			}
		}

        private Collection<busResourcesScreen> _iclbResourcesScreen;
        public Collection<busResourcesScreen> iclbResourcesScreen
        {
            get
            {
                return _iclbResourcesScreen;
            }

            set
            {
                _iclbResourcesScreen = value;
            }
        }

		public bool FindResource(int aintResourceId)
		{
			bool lblnResult = false;
			if (_icdoResources == null)
			{
				_icdoResources = new cdoResources();
			}
			if (_icdoResources.SelectRow(new object[1] { aintResourceId }))
			{
				lblnResult = true;
			}
            LoadResourcesScreen();
			return lblnResult;
		}

        public void LoadResourcesScreen()
        {
            ArrayList larrResourceScreens = 
                iobjPassInfo.isrvMetaDataCache.GetScreensHavingResource(_icdoResources.resource_id.ToString());
            Collection<busResourcesScreen> ltemp = new Collection<busResourcesScreen>();
            foreach (string lstrScreenElement in larrResourceScreens)
            {
                busResourcesScreen ltmpResourceScreen = new busResourcesScreen();
                string[] lstrSplit = lstrScreenElement.Split(new char[1] { '=' });
                if (lstrSplit[0] != null)
                {
                    ltmpResourceScreen.istrResourceFileName = lstrSplit[0].ToString();
                }
                if (lstrSplit[1] != null)
                {
                    ltmpResourceScreen.istrResourceElement = lstrSplit[1].ToString();
                }
                if (lstrSplit[2] != null)
                {
                    ltmpResourceScreen.istrResourceID = lstrSplit[2].ToString();
                }
                ltemp.Add(ltmpResourceScreen);
            }
             _iclbResourcesScreen = ltemp;
        }
		public void LoadSecurity()
		{
			DataTable ldtbList = 
				Select("cdoSecurity.ByResource", new object[1] { icdoResources.resource_id });
			_iclbSecurity = GetCollection<busSecurity>(ldtbList, "icdoSecurity");
		}

		protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
		{
			if (aobjBus is busSecurity)
			{
				busSecurity lobjSec = (busSecurity)aobjBus;
				lobjSec.ibusRoles = new busRoles();
				lobjSec.ibusRoles.icdoRoles = new cdoRoles();
				sqlFunction.LoadQueryResult(lobjSec.ibusRoles.icdoRoles, adtrRow);
			}
		}

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            LoadSecurity();
        }

    }
}
