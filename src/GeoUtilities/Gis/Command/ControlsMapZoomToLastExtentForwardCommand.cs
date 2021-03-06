using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;

namespace GeoUtilities
{
    public class ControlsMapZoomToLastExtentForwardCommand : Plugin.Interface.CommandRefBase
    {

        private Plugin.Application.IAppArcGISRef _AppHk;

        private ICommand _cmd = null;

        public ControlsMapZoomToLastExtentForwardCommand()
        {
            base._Name = "GeoUtilities.ControlsMapZoomToLastExtentForwardCommand";
            base._Caption = "后一视图";
            base._Tooltip = "后一视图";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "后一视图";
            //base._Image = "";
            //base._Category = "";
        }

        public override bool Enabled
        {
            get
            {
                try
                {
                    if (_AppHk.CurrentControl is ISceneControl) return false;  //为了只有效于2维控件
                    if (_AppHk.MapControl.LayerCount == 0)
                    {
                        base._Enabled = false;
                        return false;
                    }

                    base._Enabled = true;
                    return true;
                }
                catch
                {
                    base._Enabled = false;
                    return false;
                }
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
        {
            if (_cmd == null || _AppHk == null) return;
            if (_AppHk.MapControl == null) return;
            _cmd.OnClick();
            if (this.WriteLog)
            {
                Plugin.LogTable.Writelog("使用二维工具:" + Message);//xisheng 2011.07.08 增加日志
            }
        }

        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null) return;
            _AppHk = hook as Plugin.Application.IAppArcGISRef;
            if (_AppHk.MapControl == null) return;

            _cmd = new ControlsMapZoomToLastExtentForwardCommandClass();
            _cmd.OnCreate(_AppHk.MapControl);
        }
    }
}
