#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using NeoSpin.Interface;
using Sagitec.Interface;
using Sagitec.DBCache;
using Sagitec.Common;
using Sagitec.DBUtility;

#endregion

namespace NeoSpin.BusinessTier
{
	public class srvNeoSpinDBCache: srvDBCache, INeoSpinDBCache
	{
		public srvNeoSpinDBCache()
		{

		}        
	}
}