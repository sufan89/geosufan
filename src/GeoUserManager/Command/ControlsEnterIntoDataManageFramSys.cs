﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace GeoUserManager
{
    // *=======================================================================
    // *类功能：子系统跳转。实现从配置管理子系统页面跳转到数据中心管理子系统页面
    // *开发者：陈亚飞
    // *时  间：20110518
    // *=========================================================================
   public class ControlsEnterIntoDataManageFramSys: Fan.Plugin.Interface.CommandRefBase
    {
        private Fan.Plugin.Application.IAppDBIntegraRef m_Hook;
       //构造函数
        public ControlsEnterIntoDataManageFramSys()
        {
            base._Name = "GeoUserManager.ControlsEnterIntoDataManageFramSys";
            base._Caption = "中心管理";
            base._Tooltip = "中心管理";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "进入数据管理子系统";
        }

       //设置按钮的Enable属性
        public override bool Enabled
        {
            get 
            {
                //===========================================================
                //cyf add  20110520
                 //根据用户信息判断能否进行子系统
                  if (!File.Exists(ModFrameData.v_AppDBConectXml)) return false;
                  XmlDocument xmlDoc = new XmlDocument();
                  xmlDoc.Load(ModFrameData.v_AppDBConectXml);
                  XmlNode pNode = xmlDoc.DocumentElement.SelectSingleNode(".//用户信息");
                  if (pNode == null) return false;
                  XmlElement pElem = pNode as XmlElement;
                  if (pElem == null) return false;
                  string pType = pElem.GetAttribute("type").Trim();//用户类型
                //若不是集成管理的用户，则不能进入子系统
                  if (pType != "1") return false;
                return true;
            }
        }

       //设置按钮的提示信息
        public override string Message
        {
            get
            {
                Fan.Plugin.Application.IAppFormRef pAppFormRef = m_Hook as Fan.Plugin.Application.IAppFormRef;
                if (pAppFormRef != null)
                {
                    pAppFormRef.OperatorTips = base._Message;
                }
                return base._Message;
            }
        }
       //清楚按钮的提示信息
        public override void ClearMessage()
        {
            Fan.Plugin.Application.IAppFormRef pAppFormRef = m_Hook as Fan.Plugin.Application.IAppFormRef;
            if (pAppFormRef != null)
            {
                pAppFormRef.OperatorTips = string.Empty;
            }
        }

       //按钮的click事件
        public override void OnClick()
        {
            
            string pSysName = "";     //数据中心管理子系统Name
            string pSysCaption = "";  //数据中心管理子系统Caption

            pSysName = "GeoSysUpdate.ControlSysUpdate";
            //根据Name获得子系统的caption
            XmlDocument sysXml = new XmlDocument();
            sysXml.Load(ModFrameData.m_SysXmlPath);
            XmlNode sysNode = sysXml.SelectSingleNode("//Main//System[@Name='" + pSysName + "']");
            if (sysNode == null)
            {
                Fan.Common.Error.ErrorHandle.ShowFrmErrorHandle("提示", "不存在Name为" + pSysName + "的系统");
                return;
            }
            pSysCaption = (sysNode as XmlElement).GetAttribute("Caption").Trim();  //caption

            //进入数据中心管理子系统界面
            ModDBOperate.InitialForm(pSysName, pSysCaption);

            //功能日志 enter feature Db Log
            if (ModFrameData.v_SysLog != null)
            {
                List<string> Pra = new List<string>();
                ModFrameData.v_SysLog.Write("进入数据库管理子系统", Pra, DateTime.Now);
            } 
        }

       //创建按钮
        public override void OnCreate(Fan.Plugin.Application.IApplicationRef hook)
        {
            if (hook == null) return;
            m_Hook = hook as Fan.Plugin.Application.IAppDBIntegraRef;
        }

    }
}