#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using Sagitec.Common;
using Sagitec.DataObjects;

using System.Collections.ObjectModel;
using System.Collections;
using System.Data;
using System.Reflection;
#endregion

namespace NeoSpin.DataObjects
{
    [Serializable]
    public class doImages:doBase
	{
        public doImages(): base()
		{
		}
        private string _e_name;
        public string e_name{get{return  _e_name;}set{ _e_name = value;}}
        private string _e_owner;
        public string e_owner{get{return  _e_owner;}set{ _e_owner = value;}}
        private string _e_group;
        public string e_group{get{return  _e_group;}set{ _e_group = value;}}
        private DateTime _e_create_date;
        public DateTime e_create_date{get{return  _e_create_date;}set{ _e_create_date = value;}}
        private string _e_comment;
        public string e_comment{get{return  _e_comment;}set{ _e_comment = value;}}
        private string _e_org_filename;
        public string e_org_filename{get{return  _e_org_filename;}set{ _e_org_filename = value;}}
        private string _e_alloc_type;
        public string e_alloc_type{get{return  _e_alloc_type;}set{ _e_alloc_type = value;}}
        private string _e_chkout_file;
        public string e_chkout_file{get{return  _e_chkout_file;}set{ _e_chkout_file = value;}}
        private int _e_online_lmt;
        public int e_online_lmt{get{return  _e_online_lmt;}set{ _e_online_lmt = value;}}
        private string _e_disp_mode;
        public string e_disp_mode{get{return  _e_disp_mode;}set{ _e_disp_mode = value;}}
        private string _e_curver_num;
        public string e_curver_num{get{return  _e_curver_num;}set{ _e_curver_num = value;}}
        private DateTime _e_curver_date;
        public DateTime e_curver_date{get{return  _e_curver_date;}set{ _e_curver_date = value;}}
        private string _e_curver_author;
        public string e_curver_author{get{return  _e_curver_author;}set{ _e_curver_author = value;}}
        private int _e_online_cnt;
        public int e_online_cnt{get{return  _e_online_cnt;}set{ _e_online_cnt = value;}}
        private int _e_online_size;
        public int e_online_size{get{return  _e_online_size;}set{ _e_online_size = value;}}
        private DateTime _e_last_date;
        public DateTime e_last_date{get{return  _e_last_date;}set{ _e_last_date = value;}}
        private string _e_last_ver;
        public string e_last_ver{get{return  _e_last_ver;}set{ _e_last_ver = value;}}
        private string _e_icon_title;
        public string e_icon_title{get{return  _e_icon_title;}set{ _e_icon_title = value;}}
        private int _e_csindex_count;
        public int e_csindex_count{get{return  _e_csindex_count;}set{ _e_csindex_count = value;}}
        private int _e_csindex_limit;
        public int e_csindex_limit{get{return  _e_csindex_limit;}set{ _e_csindex_limit = value;}}
        private int _e_del_access;
        public int e_del_access{get{return  _e_del_access;}set{ _e_del_access = value;}}

    }
}

