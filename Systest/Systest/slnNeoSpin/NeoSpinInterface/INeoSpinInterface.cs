#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using Sagitec.Interface;

#endregion

namespace NeoSpin.Interface
{
	public interface INeoSpinDBCache: IDBCache
	{
	}

	public interface INeoSpinMetaDataCache: IMetaDataCache
	{
	}

	public interface INeoSpinBusinessTier: IBusinessTier
	{
        bool DoesFileExists(string astrFileName);
	}
}