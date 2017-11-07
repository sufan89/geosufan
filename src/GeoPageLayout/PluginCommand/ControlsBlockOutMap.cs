using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace GeoPageLayout
{
   /// <summary>
   /// ZQ 20111008  add  ������Χר��ͼ   
   /// </summary>
    public class ControlsBlockOutMap:Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppGisUpdateRef m_Hook;
        private Plugin.Application.IAppFormRef m_frmhook;
        public ControlsBlockOutMap()
        {
            base._Name = "GeoPageLayout.ControlsBlockOutMap";
            base._Caption = "ʸ��������Χר��ͼ";
            base._Tooltip = "ʸ��������Χר��ͼ";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "ʸ��������Χר��ͼ";
        }

        public override void OnClick()
        {
            if (m_Hook == null)
                return;
            if (m_Hook.ArcGisMapControl.Map.LayerCount == 0)
            {
                MessageBox.Show("��ǰû�е������ݣ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (this.WriteLog)
                {
                    Plugin.LogTable.Writelog("��׼�ַ���ͼ ��ʾ����ǰû�е������ݣ���", m_Hook.tipRichBox);
                }
                return;
            }
           
            FrmBlockOutMap pFrmBlockOutMap = new FrmBlockOutMap();
            if (pFrmBlockOutMap.ShowDialog() != DialogResult.OK)
                return;
            //�������������ָ��Ϊ�ȴ�
            Cursor cur = m_frmhook.MainForm.Cursor;
            m_frmhook.MainForm.Cursor = Cursors.WaitCursor;
            try
            {
                GeoPageLayout geoPageLayout = new GeoPageLayout();
                geoPageLayout.WriteLog = WriteLog;//ygc 2012-9-12 �Ƿ�д��־
                geoPageLayout.pageLayoutExtentBat(m_Hook.ArcGisMapControl.Map, pFrmBlockOutMap.m_QueryResult, pFrmBlockOutMap.ExtentFC, pFrmBlockOutMap.OutputPath);
                
            }
            catch (Exception ex)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", ex.Message);
            }
            m_frmhook.MainForm.Cursor = cur;
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