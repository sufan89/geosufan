﻿
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ESRI.ArcGIS.Carto;

namespace GeoSysUpdate
{
    public class ControlsMapSheetShowMapContxt : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppArcGISRef _AppHk;
        private Plugin.Application.AppGidUpdate _hook;
        public ControlsMapSheetShowMapContxt()
        {
            base._Name = "GeoSysUpdate.ControlsMapSheetShowMapContxt";
            base._Caption = "图幅号定位";
            base._Tooltip = "图幅号定位";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "图幅号定位";
        }

        public override bool Enabled
        {
            get
            {
                if (_AppHk.MapControl == null || _AppHk.TOCControl == null) return false;
                return true;
            }
        }

        public override string Message
        {
            get
            {
                Plugin.Application.IAppFormRef pAppFormRef = _AppHk as Plugin.Application.IAppFormRef;
                if (pAppFormRef != null)
                {
                    pAppFormRef.OperatorTips = base._Message;
                }
                return base._Message;
            }
        }

        public override void ClearMessage()
        {
            Plugin.Application.IAppFormRef pAppFormRef = _AppHk as Plugin.Application.IAppFormRef;
            if (pAppFormRef != null)
            {
                pAppFormRef.OperatorTips = string.Empty;
            }
        }

        public override void OnClick()
        {//XZQLocation
            if (_hook == null) return;
            if (_hook.MainUserControl == null) return;
            UserControlSMPD pUserControl = _hook.MainUserControl as UserControlSMPD;
            pUserControl.ShowMapByMapSheetNode(pUserControl.GettreView());
        }

        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null) return;
            _AppHk = hook as Plugin.Application.IAppArcGISRef;
            _hook = hook as Plugin.Application.AppGidUpdate;
        }
    }
}
