using Sagitec.BusinessObjects;
using Sagitec.Common;
namespace NeoSpin.BusinessObjects
{
    public class busSoFileBase : busFileBase
    {
        public override void AddFrameworkErrors(bool ablnCertifcation, int aintErrorID, object[] aarrParams = null, string astrFieldValue = null)
        {
            if (this.iobjFile?.xml_layout_file == "fleTFFRSSNMatchInbound" && aintErrorID == 5002)
            {

            }
            else
            {
                if (ablnCertifcation)
                {
                    AddFieldErrors(0, aintErrorID.ToString(), string.Empty, astrFieldValue, aarrParams);
                }
                else
                {
                    string lstrErrorMessage = busMainBase.GetMessage(utlMessageType.Framework, aintErrorID, aarrParams);
                    AddErrors(aintErrorID.ToString(), lstrErrorMessage, astrFieldValue);
                }
            }
        }
    }
}