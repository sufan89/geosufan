﻿using System;
using System.Collections.Generic;
using System.Text;

namespace GeoDataManagerFrame
{
   public class YwOutMap : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppGisUpdateRef m_Hook;
        private Plugin.Application.IAppFormRef m_frmhook;
        public YwOutMap()
        {
            base._Name = "GeoDataManagerFrame.YwOutMap";
            base._Caption = "按照业务制图";
            base._Tooltip = "按照业务制图";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "按照业务制图";
        }

        public override void OnClick()
        {
          
        }

        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null)
                return;
            m_Hook = hook as Plugin.Application.IAppGisUpdateRef;
            m_frmhook = hook as Plugin.Application.IAppFormRef;
        }
    }
}