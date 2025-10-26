using Sagitec.BusinessObjects;
using Sagitec.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoSpin.BusinessObjects
{  
    // PIR 16736 - This class is added to add Corrospondence Header image path property,
    //this property is accessible to all corrospondence business objects, which are inherited from this class.
    [Serializable]
    public class busExtendBase : busBase
    {
        public string istrImage 
        {
            get
            {
                return busGlobalFunctions.GetData1ByCodeValue(52, "CRIM", iobjPassInfo);
            }
        }
        public string istrImageSFNLogo
        {
            get
            {
                return busGlobalFunctions.GetData1ByCodeValue(52, "SFNL", iobjPassInfo);
            }
        }

        public string istrImageSFNAddress
        {
            get
            {
                return busGlobalFunctions.GetData1ByCodeValue(52, "SFNA", iobjPassInfo);
            }
        }
        public bool iblnExternalUser { get; set; } //PIr-18492

        //No correspondance button should be shown when no template is associated to the form.
        public override void AddToResponse(utlResponseData aobjResponseData)
        {
            base.AddToResponse(aobjResponseData);
            utlObjectData HiddenControlList = (utlObjectData)aobjResponseData.HeaderData["ControlList"]["HiddenControls"];
            utlThreadStatic lutlThreadStatic = (utlThreadStatic)Sagitec.Common.utlThreadStatic.iutlThreadStatic.Value;
            XmlFormObject lFormObject = lutlThreadStatic?.iobjFormXml;
            utlDataControl ludcCorrButton = lFormObject?.icolutlDataControl?.FirstOrDefault(c => c.istrMethodName == "btnCorrespondence_Click");
            if(HiddenControlList.IsNotNull() && ludcCorrButton.IsNotNull() && !HiddenControlList.ContainsKey(ludcCorrButton.istrControlID))
            {
                DataTable ldtbCorrespondence = Select("entCorTemplates.Templates", new object[] { "%" + lFormObject.istrFileName + ";%" });
                if(ldtbCorrespondence.Rows.Count == 0)
                {
                    HiddenControlList.Add(ludcCorrButton.istrControlID, null);
                }
                //else
                //{
                //    int lintFormResource = Convert.ToInt32(lFormObject.idictAttributes["sfwResource"]);
                //    int lintFormSecurityLevel = 0;
                //    if (lintFormResource > 0)
                //    {
                //        srvHelper.HasAccess(lintFormResource, out lintFormSecurityLevel);
                //        if (lintFormSecurityLevel <= 1)
                //        {
                //            HiddenControlList.Add(ludcCorrButton.istrControlID, null);
                //        }
                //    }
                //}
            }
            //F/W PIR 22282 - Displaying Step Name in MVVM application for Workflow 
            object lstrActivityType = string.Empty;
            if (aobjResponseData.ConcurrentOtherData.TryGetValue("ActivityInstanceType", out lstrActivityType))
            {
                if ("BPM".Equals(Convert.ToString(lstrActivityType)))
                {
                    aobjResponseData.ConcurrentOtherData["ShowActivityInstanceDetails"] = true;
                    busSolBpmActivityInstance lbusBaseActivityInstance = ibusBaseActivityInstance as busSolBpmActivityInstance;
                    if (lbusBaseActivityInstance != null)
                    {
                        aobjResponseData.ConcurrentOtherData["ProcessName"] = lbusBaseActivityInstance?.ibusBpmProcessInstance?.ibusBpmProcess?.icdoBpmProcess?.description;
                        aobjResponseData.ConcurrentOtherData["ActivityName"] = lbusBaseActivityInstance?.ibusBpmActivity?.icdoBpmActivity?.name;
                        aobjResponseData.ConcurrentOtherData["ProcessInstanceId"] = lbusBaseActivityInstance?.ibusBpmProcessInstance?.icdoBpmProcessInstance?.process_instance_id;
                        aobjResponseData.ConcurrentOtherData["ActivityDetailsNavParams"] = HelperFunction.EncryptString($"aintactivityinstanceid=#{lbusBaseActivityInstance.icdoBpmActivityInstance.activity_instance_id}", utlConstants.istrMenuNavParamKey, utlConstants.istrFormNameNavParamKey);
                    }
                }
            }
        }
    }
}
