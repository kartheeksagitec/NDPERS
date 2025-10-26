#region Using directives

using System;
using System.Collections.ObjectModel;
using System.Data;
using Sagitec.BusinessObjects;

#endregion
namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busRHICFileOut : busFileBaseOut
    {
        public Collection<busPensionFile> iclbPensionFile { get; set; }
        public void LoadPensionFile(DataTable adtbRhicFile)
        {
            iclbPensionFile = (Collection<busPensionFile>)iarrParameters[0];

            foreach (busPensionFile lobjPensionFile in iclbPensionFile)
            {
                try
                {
                lobjPensionFile.icdoActuaryFileRhicDetail.Insert();               
                }
                catch
                {
                    new Exception("Error in Creating RHIC File");
                }
            }
        }
    }
}