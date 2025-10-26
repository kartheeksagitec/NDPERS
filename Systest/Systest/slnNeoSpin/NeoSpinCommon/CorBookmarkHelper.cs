#region [Using Directives]
using NeoBase.Common;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.CorBuilder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
#endregion [Using Directives]

namespace NeoBase.Common
{
    public static class CorBookmarkHelper
    {
        public static Dictionary<string, List<string>> idictCachedStdBookmarks
        {
            private get;
            set;
        }

        /// <summary>
        /// Cache standard bookmarks while running application.
        /// </summary>
        public static void CacheTemplateStdBookmarks()
        {
            CorBookmarkHelper.idictCachedStdBookmarks = new Dictionary<string, List<string>>();
            var lvarCorTemplates = utlCache.icolMetaDataCache.Where(itm => itm.Key.StartsWith("cor"));
            string lstrTemplateFolderPath = utlPassInfo.iobjPassInfo.isrvDBCache.GetPathInfo("CorrTmpl");
            foreach (KeyValuePair<string, XmlMainObject> CurrCorTemplate in lvarCorTemplates)
            {
                string lstrTemplateName = CurrCorTemplate.Key.Substring(3);
                try
                {
                    utlCorresPondenceInfo lobjutlCorresPondenceInfo = utlPassInfo.iobjPassInfo.isrvMetaDataCache.GetCorresPondenceInfo(lstrTemplateName);
                    if (null != lobjutlCorresPondenceInfo)
                    {
                        CorBookmarkHelper.idictCachedStdBookmarks[lobjutlCorresPondenceInfo.istrTemplateName] =
                            FetchBookmarksFromDocument(lobjutlCorresPondenceInfo, lstrTemplateFolderPath);
                    }
                }
                catch (Exception exception)
                {
                    File.AppendAllText("templateLoadingErrors.txt", $"{CurrCorTemplate.Key} {exception.Message}{Environment.NewLine}");
                }
            }
        }

        private static List<string> FetchBookmarksFromDocument(utlCorresPondenceInfo autlCorresPondenceInfo, string astrTemplateFolderPath)
        {
            List<string> llstBookmark = new List<string>();
            string lstrTemplateName = $"{astrTemplateFolderPath}{autlCorresPondenceInfo.istrTemplateName}.docx";
            List<CorBasicBookmark> llstAllCorBookmarks = new CorBuilderXML().RetrieveAllBookmarks(lstrTemplateName);

            //if (!llstAllCorBookmarks.IsNullOrEmpty())
            //{
            //    foreach (CorBasicBookmark lobjCorBasicBookmark in llstAllCorBookmarks)
            //    {
            //        if (!llstBookmark.Contains(lobjCorBasicBookmark.Name))
            //        {
            //            llstBookmark.Add(lobjCorBasicBookmark.Name);
            //        }
            //        if (lobjCorBasicBookmark.Type == BookmarkType.Template)
            //        {
            //            utlCorresPondenceInfo lchidutlCorresPondenceInfo = autlCorresPondenceInfo.icolBookmarkChldTemplateInfo.FirstOrDefault(ele => ele.istrChildTemplateName == lobjCorBasicBookmark.Name);
            //            llstBookmark.AddRange(FetchBookmarksFromDocument(lchidutlCorresPondenceInfo, astrTemplateFolderPath));
            //        }
            //    }
            //}
            return llstBookmark;
        }

        /// <summary>
        /// Fetch correspondence template standard bookmarks.
        /// </summary>
        /// <param name="astrTemplateName"></param>
        /// <returns></returns>
        public static List<string> GetStdBooksMarksForTemplate(string astrTemplateName)
        {
            List<string> llstTemplateStdBookmarks = new List<string>();
            CorBookmarkHelper.idictCachedStdBookmarks.TryGetValue(astrTemplateName, out llstTemplateStdBookmarks);
            return llstTemplateStdBookmarks;
        }
    }
}
