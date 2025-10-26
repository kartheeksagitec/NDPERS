//using NeoBase.Common;
using NeoSpinMVVM.BPMExecution.Controller;
using Newtonsoft.Json;
using Sagitec.Bpm;
using Sagitec.Common;
using Sagitec.MVVMClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Xml.Linq;
using System.Xml.XPath;
namespace NeoSpinWebClient.Controllers
{
    public class MapController : ApiControllerBase
    {       
        #region[Constructor]
        public MapController()
        {
            isrvServers = new srvServers();
        }
        #endregion

        #region [Protected]
        protected new Dictionary<string, object> SetParams(string astrFormID)
        {
            Dictionary<string, object> ldictParams = new Dictionary<string, object>();
            ldictParams["SessionID"] = iobjSessionData.istrSessionId;
            ldictParams[utlConstants.istrConstFormName] = astrFormID;
            int lintUserSerialID = int.Parse(iobjSessionData["UserSerialID"].ToString());
            ldictParams[utlConstants.istrConstUserSerialID] = lintUserSerialID;
            ldictParams[utlConstants.istrConstUserID] = iobjSessionData["UserID"].ToString();
            return ldictParams;
        }
        #endregion

        #region [Public]
        // GET api/Map
        /// <summary>
        /// Get Data
        /// </summary>
        /// <param name="aintCaseInstanceID"></param>
        /// <returns>HttpResponseMessage</returns>
        [HttpGet]
        public HttpResponseMessage GetData([System.Web.Http.FromUri]int aintCaseInstanceID)
        {
            HttpResponseMessage response = null;
            var idictParams = SetParams("wfmBpmCaseInstanceMaintenance");
            isrvServers.ConnectToBT("wfmBpmCaseInstanceMaintenance");
            
            Hashtable lhstParams = new Hashtable();
            lhstParams.Add("aintCaseInstanceId", aintCaseInstanceID);
            busBpmCaseInstance objCaseInstance = (busBpmCaseInstance)isrvServers.isrvBusinessTier.ExecuteMethod("FindBpmCaseInstanceForExecution", lhstParams, false, idictParams);
            if (!string.IsNullOrWhiteSpace(objCaseInstance.ibusBpmCase.icdoBpmCase.bpmmap))
            {
                MapExecutionDetails objData = new MapExecutionDetails();
                objData.lstShapes = PrePareData(objCaseInstance.ibusBpmCase.icdoBpmCase.bpmmap);
                UpdateCanvasBounds(objData);
                objData.lstExecutedSteps = this.LoadExecutedSteps(objCaseInstance);
                
                response = Request.CreateResponse(HttpStatusCode.OK, objData);
            }
            return response;
        }
        /// <summary>
        /// Render BPM
        /// </summary>
        /// <param name="design_specification_bpm_map_id"></param>
        /// <returns>HttpResponseMessage</returns>
        [HttpGet]
        public HttpResponseMessage RenderBPM([System.Web.Http.FromUri]int design_specification_bpm_map_id)
        {
            HttpResponseMessage response = null;
          
            return response;
        }
        // GET api/ReadOnlyMap
        /// <summary>
        /// Render Read Only BPM
        /// </summary>
        /// <param name="aintProcessId"></param>
        /// <param name="aintCaseId"></param>
        /// <returns>HttpResponseMessage</returns>
        [HttpGet]
        public HttpResponseMessage RenderReadOnlyBPM([System.Web.Http.FromUri]int aintProcessId, [System.Web.Http.FromUri]int aintCaseId)
        {
            HttpResponseMessage response = null;
            var idictParams = SetParams("wfmBpmCaseInstanceMaintenance");
            isrvServers.ConnectToBT("wfmBpmCaseInstanceMaintenance");

            busBpmCase objCaseInstance = null;
            Hashtable lhstParams = new Hashtable();
            if (aintProcessId > 0)
            {
                lhstParams.Add("aintProcessId", aintProcessId);
                objCaseInstance = (busBpmCase)isrvServers.isrvBusinessTier.ExecuteMethod("FindBpmCaseForMap", lhstParams, false, idictParams);
            }
            else if (aintCaseId > 0)
            {
                lhstParams.Add("aintCaseId", aintCaseId);
                objCaseInstance = (busBpmCase)isrvServers.isrvBusinessTier.ExecuteMethod("FindBpmCaseToRenderMap", lhstParams, false, idictParams);
            }
            if (objCaseInstance !=null && objCaseInstance.icdoBpmCase != null && !string.IsNullOrWhiteSpace(objCaseInstance.icdoBpmCase.bpmmap))
            {
                MapExecutionDetails objData = new MapExecutionDetails();
                objData.lstShapes = PrePareData(objCaseInstance.icdoBpmCase.bpmmap);
                UpdateCanvasBounds(objData);
                response = Request.CreateResponse(HttpStatusCode.OK, objData);
            }
            return response;
        }
        // POST api/Map
        /// <summary>
        /// Get Activity Data
        /// </summary>
        /// <param name="astrXMLFile"></param>
        /// <returns>HttpResponseMessage</returns>
        [HttpPost]
        public HttpResponseMessage GetCallActivityData([System.Web.Http.FromBody]object astrXMLFile)
        {
            HttpResponseMessage response = null;
            CallActivityPostBackData objData = JsonConvert.DeserializeObject<CallActivityPostBackData>(Convert.ToString(astrXMLFile));
            if (!string.IsNullOrWhiteSpace(objData.astrXMLFile))
            {
                MapExecutionDetails objResponse = new MapExecutionDetails();
                objResponse.Title = objData.Title;
                objResponse.WindowID =string.Format("wndw_{0}", Guid.NewGuid().ToString());
                objResponse.lstShapes = PrePareData(objData.astrXMLFile);
                UpdateCanvasBounds(objResponse);
                objResponse.lstExecutedSteps = objData.lstExecutedSteps;
                if (objData.IsExecuted)
                {
                    this.UpdateExecutionStatus(objResponse);
                }
                response = Request.CreateResponse(HttpStatusCode.OK, objResponse);
            }
            return response;
        }
        public string GetXmlAttributeValue(XElement xEle, string attrName)
        {
            return null != xEle.Attribute(attrName) ? xEle.Attribute(attrName).Value : null;
        }
        #endregion

        #region [Private]
        private void UpdateExecutionStatus(MapExecutionDetails objResponse)
        {
            foreach (ShapeDetails objShape in objResponse.lstShapes)
            {
                if (objResponse.lstExecutedSteps.Any(ele => ele.ElementId == objShape.Id))
                {
                    objShape.IsExecuted = true;
                }
            }
        }

        private List<ShapeDetails> PrePareData(string astrXml)
        {
            List<ShapeDetails> lst = new List<ShapeDetails>();
            XElement xEleMap = XElement.Parse(astrXml);
            foreach (XElement xEleDiagram in xEleMap.Elements())
            {
                if (xEleDiagram.Name.LocalName == "BPMNDiagram")
                {
                    foreach (XElement xElePlane in xEleDiagram.Elements())
                    {
                        foreach (XElement xele in xElePlane.Elements())
                        {
                            ShapeDetails objShape = new ShapeDetails();
                            objShape.ShapeName = xele.Name.LocalName;
                            SetShapeDetails(xele, objShape, xEleMap);
                            lst.Add(objShape);
                        }
                    }
                }
            }

            return lst;
        }

        private void UpdateCanvasBounds(MapExecutionDetails objData)
        {
            double maxHeight = 0;
            double maxWidth = 0;

            foreach (var objShape in objData.lstShapes)
            {
                if ((objShape.Top + objShape.Height) > maxHeight)
                {
                    maxHeight = (objShape.Top + objShape.Height);
                }

                if ((objShape.Left + objShape.Width) > maxWidth)
                {
                    maxWidth = (objShape.Left + objShape.Width);
                }

                if ((objShape.LabelTop + objShape.LabelHeight) > maxHeight)
                {
                    maxHeight = (objShape.LabelTop + objShape.LabelHeight);
                }

                if ((objShape.LabelLeft + objShape.LabelWidth) > maxWidth)
                {
                    maxWidth = (objShape.LabelLeft + objShape.LabelWidth);
                }
            }

            if (maxHeight > objData.Height)
            {
                objData.Height = maxHeight + 100;
            }
            if (maxWidth > objData.Width)
            {
                objData.Width = maxWidth + 100;
            }
        }

        private void SetShapeDetails(XElement xlBPMNShape, ShapeDetails objShape, XElement xEleMap)
        {
            double Left, width, Top, height;
            double LabelLeft, Labelwidth, LabelTop, Labelheight;
            Left = width = Top = height = 0;
            LabelLeft = Labelwidth = LabelTop = Labelheight = 0;

            if (objShape.ShapeName == "BPMNShape")
            {
                XAttribute xAttr = xlBPMNShape.Attributes().FirstOrDefault(itm => itm.Name == "bpmnElement");
                if (xAttr != null)
                {
                    string strId = xAttr.Value;
                    objShape.Id = strId;
                }

                foreach (XElement xlBPMNChildShapes in xlBPMNShape.Elements())
                {
                    if (xlBPMNChildShapes.Name.LocalName == "Bounds")
                    {
                        string strTemp = this.GetXmlAttributeValue(xlBPMNChildShapes, "x");
                        double.TryParse(strTemp, out Left);

                        strTemp = this.GetXmlAttributeValue(xlBPMNChildShapes, "y");
                        double.TryParse(strTemp, out Top);

                        strTemp = this.GetXmlAttributeValue(xlBPMNChildShapes, "height");
                        double.TryParse(strTemp, out height);

                        strTemp = this.GetXmlAttributeValue(xlBPMNChildShapes, "width");
                        double.TryParse(strTemp, out width);
                    }
                    else if (xlBPMNChildShapes.Name.LocalName == "BPMNLabel")
                    {
                        foreach (XElement xlBPMNChildChildShapes in xlBPMNChildShapes.Elements())
                        {
                            if (xlBPMNChildChildShapes.Name.LocalName == "Bounds")
                            {
                                string strTemp = this.GetXmlAttributeValue(xlBPMNChildChildShapes, "x");
                                double.TryParse(strTemp, out LabelLeft);

                                strTemp = this.GetXmlAttributeValue(xlBPMNChildChildShapes, "y");
                                double.TryParse(strTemp, out LabelTop);

                                strTemp = this.GetXmlAttributeValue(xlBPMNChildChildShapes, "height");
                                double.TryParse(strTemp, out Labelheight);

                                strTemp = this.GetXmlAttributeValue(xlBPMNChildChildShapes, "width");
                                double.TryParse(strTemp, out Labelwidth);
                            }
                        }
                    }
                }
                objShape.Left = Left;
                objShape.Top = Top;
                objShape.Width = width;
                objShape.Height = height;
                objShape.LabelLeft = LabelLeft;
                objShape.LabelTop = LabelTop;
                objShape.LabelWidth = Labelwidth;
                objShape.LabelHeight = Labelheight;

            }
            else if (objShape.ShapeName == "BPMNEdge")
            {
                XAttribute xAttr = xlBPMNShape.Attributes().FirstOrDefault(itm => itm.Name == "bpmnElement");
                if (xAttr != null)
                {
                    string strId = xAttr.Value;
                    objShape.Id = strId;
                }

                foreach (XElement xlwaypoint in xlBPMNShape.Elements())
                {
                    if (xlwaypoint.Name.LocalName == "waypoint")
                    {
                        string strTemp = this.GetXmlAttributeValue(xlwaypoint, "x");
                        double.TryParse(strTemp, out Left);
                        strTemp = this.GetXmlAttributeValue(xlwaypoint, "y");
                        double.TryParse(strTemp, out Top);

                        objShape.lstWayPoints.Add(new ShapeDetails() { Left = Left, Top = Top });
                    }
                    else if (xlwaypoint.Name.LocalName == "BPMNLabel")
                    {
                        foreach (XElement xlBPMNChildChildShapes in xlwaypoint.Elements())
                        {
                            if (xlBPMNChildChildShapes.Name.LocalName == "Bounds")
                            {
                                string strTemp = this.GetXmlAttributeValue(xlBPMNChildChildShapes, "x");
                                double.TryParse(strTemp, out LabelLeft);

                                strTemp = this.GetXmlAttributeValue(xlBPMNChildChildShapes, "y");
                                double.TryParse(strTemp, out LabelTop);

                                strTemp = this.GetXmlAttributeValue(xlBPMNChildChildShapes, "height");
                                double.TryParse(strTemp, out Labelheight);

                                strTemp = this.GetXmlAttributeValue(xlBPMNChildChildShapes, "width");
                                double.TryParse(strTemp, out Labelwidth);
                            }
                        }
                    }
                }
                objShape.LabelLeft = LabelLeft;
                objShape.LabelTop = LabelTop;
                objShape.LabelWidth = Labelwidth;
                objShape.LabelHeight = Labelheight;

            }
            this.SetShapeType(objShape, xEleMap);
        }
      
        private void SetShapeType(ShapeDetails objShape, XElement xEleMap)
        {
            XElement xelmBPMNElement = xEleMap.Descendants().Where(itm => this.GetXmlAttributeValue(itm, "id") == objShape.Id).FirstOrDefault();
            if (xelmBPMNElement != null)
            {
                objShape.ShapeType = xelmBPMNElement.Name.LocalName;
                if (xelmBPMNElement.Name == "textAnnotation")
                {
                    objShape.Text = GetTextAnnotationText(xelmBPMNElement);
                }
                else
                {
                    objShape.Text = this.GetXmlAttributeValue(xelmBPMNElement, "name");
                }
            }
        }

        /// <summary>
        /// Framework PIR 19502 : Text annotation is showing as empty box at the time of BPM.
        /// </summary>
        /// <param name="aXEle"></param>
        /// <returns></returns>
        private static string GetTextAnnotationText(XElement aXEle)
        {
            string retVal = string.Empty;
            XElement xEleText=aXEle.Elements().FirstOrDefault(ele => ele.Name == "text");
            if (null != xEleText)
            {
                try
                {
                    System.Windows.Forms.RichTextBox rtBox = new System.Windows.Forms.RichTextBox();
                    // rtBox.Rtf = xEleText.Value;
                    //retVal = rtBox.Text;
                    if (!string.IsNullOrEmpty(xEleText.Value) && xEleText.Value.Substring(0, 5) == "{\\rtf")
                    {
                        rtBox.Rtf = xEleText.Value;
                        retVal = rtBox.Text;
                        xEleText.Value = rtBox.Text;
                    }
                    else
                    {
                        retVal = xEleText.Value;
                    }

                }
                catch
                {
                  
                }
            }

            return retVal;
        }

        private List<clsExecutedStep> LoadExecutedSteps(busBpmCaseInstance objCaseInstance)
        {
            List<clsExecutedStep> lstExecutedShapes = new List<clsExecutedStep>();
            string lstrXML = objCaseInstance.ibusBpmCase.icdoBpmCase.bpmmap;
            List<busBpmCaseInstanceExecutionPath> result = objCaseInstance.iclbBpmCaseInstanceExecutionPath.OrderBy(executionPath => executionPath.icdoBpmCaseInstanceExecutionPath.execution_path_id).ToList();
            foreach (busBpmCaseInstanceExecutionPath ele in result)
            {
                string elementId = ele.icdoBpmCaseInstanceExecutionPath.element_id;
                //code to get containerelementid
                string containerElementId = "";
                int lintActivityInstanceId = 0;
                if (ele.icdoBpmCaseInstanceExecutionPath.activity_instance_id != 0)
                    lintActivityInstanceId = ele.icdoBpmCaseInstanceExecutionPath.activity_instance_id;
                else if (ele.icdoBpmCaseInstanceExecutionPath.parent_activity_instance_id != 0)
                    lintActivityInstanceId = ele.icdoBpmCaseInstanceExecutionPath.parent_activity_instance_id;
                busBpmActivityInstance lobjContainerActivityInstance = null;
                foreach (busBpmProcessInstance lbusBpmProcessInstance in objCaseInstance.iclbBpmProcessInstance)
                {
                    lobjContainerActivityInstance = lbusBpmProcessInstance.iclbBpmActivityInstance.Where(activityInstance => activityInstance.icdoBpmActivityInstance.activity_instance_id == lintActivityInstanceId).FirstOrDefault();
                    if (lobjContainerActivityInstance != null)
                        break;
                }
                busBpmActivity lobjContainerActivity = null;
                if (lobjContainerActivityInstance != null)
                {
                    foreach (busBpmProcess lbusBpmProcess in objCaseInstance.ibusBpmCase.iclbBpmProcess)
                    {
                        lobjContainerActivity = lbusBpmProcess.iclbBpmActivity.Where(activity => activity.icdoBpmActivity.activity_id == lobjContainerActivityInstance.icdoBpmActivityInstance.activity_id && activity.icdoBpmActivity.parent_activity_id != 0).FirstOrDefault();
                        if (lobjContainerActivity != null)
                        {
                            lobjContainerActivity = lbusBpmProcess.iclbBpmActivity.Where(activity => activity.icdoBpmActivity.activity_id == lobjContainerActivity.icdoBpmActivity.parent_activity_id).FirstOrDefault();
                            break;
                        }
                    }
                }
                if (lobjContainerActivity != null)
                    containerElementId = lobjContainerActivity.icdoBpmActivity.bpm_activity_id;
                //code to get containerelementid ends
                clsExecutedStep obj = new clsExecutedStep();
                obj.ContainerElementId = containerElementId;
                obj.ElementId = elementId;
                obj.ParametersSnapShot = ele.icdoBpmCaseInstanceExecutionPath.parameters_snapshot;//dr["PARAMETERS_SNAPSHOT"] as byte[];
                obj.ActivityInstanceId = ele.icdoBpmCaseInstanceExecutionPath.activity_instance_id;
                obj.Parameters = HelperFunction.DeSerializeToObject(obj.ParametersSnapShot);
                if (obj.Parameters != null && obj.Parameters is System.Collections.ObjectModel.Collection<busBpmCaseInstanceExecutionPathParameterValueSnapshot>)
                {
                    obj.Parameters = obj.Parameters as System.Collections.ObjectModel.Collection<busBpmCaseInstanceExecutionPathParameterValueSnapshot>;
                }
                else if (obj.Parameters != null && obj.Parameters is Dictionary<string, System.Collections.ObjectModel.Collection<busBpmCaseInstanceExecutionPathParameterValueSnapshot>>)
                {
                    obj.Parameters = (obj.Parameters as Dictionary<string, System.Collections.ObjectModel.Collection<busBpmCaseInstanceExecutionPathParameterValueSnapshot>>)["Initial"];
                }
                clsExecutedStep parentStep = GetParentStep(lstExecutedShapes, containerElementId);
                obj.EleType = ele.icdoBpmCaseInstanceExecutionPath.element_type_value;
                if (ele.icdoBpmCaseInstanceExecutionPath.element_type_value == "USTK")
                {
                    //UpdateUserTaskExecutionDetails(obj);
                }
                if (parentStep != null)
                {
                    obj.ParentEleId = parentStep.ElementId;
                    obj.XMLFile = GetCallActivityXML(elementId, parentStep.XMLFile);
                    parentStep.lstExecutedSteps.Add(obj);
                }
                else
                {
                    obj.XMLFile = GetCallActivityXML(elementId, lstrXML);
                    lstExecutedShapes.Add(obj);
                }
            }
            return lstExecutedShapes;
        }

        private clsExecutedStep GetParentStep(List<clsExecutedStep> alst, string containerElementId)
        {
            clsExecutedStep retVal = null;
            if (null != alst)
            {
                retVal = alst.FirstOrDefault(itm => itm.ElementId == containerElementId);
                if (null == retVal)
                {
                    foreach (clsExecutedStep itm in alst)
                    {
                        retVal = this.GetParentStep(itm.lstExecutedSteps, containerElementId);
                        if (null != retVal)
                        {
                            break;
                        }
                    }
                }
            }

            return retVal;
        }

        private string GetCallActivityXML(string astrCallActivityID, string astrXML)
        {
            string strReturn = null;
            try
            {
                if (!string.IsNullOrEmpty(astrXML))
                {
                    XElement xelem = XElement.Parse(astrXML);
                    XElement xelemCallActivity = xelem.Descendants().FirstOrDefault(itm => itm.Name.LocalName == "callActivity" && this.GetXmlAttributeValue(itm, "id") == astrCallActivityID);
                    if (xelemCallActivity != null)
                    {
                        XElement xelemCalledCaseBpm = xelemCallActivity.XPathSelectElement(string.Format("{0}/calledCaseBpmMap", "extensionElements"));
                        if (xelemCalledCaseBpm != null)
                        {
                            strReturn = xelemCalledCaseBpm.Value;
                        }
                    }
                }
            }
            catch
            {
            }
            return strReturn;
        }
        #endregion
    } 
}