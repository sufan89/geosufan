using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using GeoDataCenterFunLib;

namespace GeoDBConfigFrame
{
    class BindRasterSource : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppPrivilegesRef m_Hook;
    
     public BindRasterSource()
        {
            base._Name = "GeoDBConfigFrame.BindRasterSource";
            base._Caption = "栅格数据挂接";
            base._Tooltip = "栅格数据挂接";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "栅格数据挂接";
        }

        public override void OnClick()
        {

            if (m_Hook != null)
            {
                LogFile log = new LogFile(m_Hook.tipRichBox, m_Hook.strLogFilePath);
                if (log != null)
                {
                    log.Writelog("影像库挂接");

                }
            }
            frmBindSource frm = new frmBindSource(0);
            frm.Show();
        }

        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null)
                return;
            m_Hook = hook as Plugin.Application.IAppPrivilegesRef;
        }
    }
}
