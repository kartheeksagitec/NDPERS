using System;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class RequestAdditionalInfo
    {
        public string FILENET_DOCUMENT_TYPE;
        public string IMAGE_DOC_CATEGORY;
    }

    [Serializable]
    public class ProcessInstanceAttachmentsAdditionalInfo
    {
        public int FILENET_DOCUMENT_TYPE_ID;
        public string FILENET_DOCUMENT_TYPE_VALUE;
        public int IMAGE_DOC_CATEGORY_ID;
        public string IMAGE_DOC_CATEGORY_VALUE;
    }
}