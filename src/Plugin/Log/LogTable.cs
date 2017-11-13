﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

using ESRI.ArcGIS.Geodatabase;
using SysCommon.Gis;
using SysCommon.Error;
using System.Net;

using System.Data.OleDb;
namespace Plugin
{
    /// <summary>
    /// 作者：cll
    /// 修改：yjl
    /// 日期：2011.06.4
    /// 说明：系统日志管理类，提供arcgis操作表的方法
    /// </summary> 
    public class LogTable
    {

        public static string m_LogNAME = "DataManager_LOG";
        public static SysGisDB m_gisDb = null;
        public static SysGisTable m_sysTable = null;
        public static string user = "";
        public static RichTextBox _richbox;
        public static string userIP = "";
        private static StreamWriter _StreamWriter = null;
        //静态构造函数
        static LogTable()
        {
            if (m_gisDb == null)
            {
                user = (Plugin.ModuleCommon.AppUser == null) ? "" : Plugin.ModuleCommon.AppUser.Name;
                string strHostName = Dns.GetHostName();  //得到本机的主机名
                IPHostEntry ipEntry = Dns.GetHostByName(strHostName); //取得本机IP
                userIP = ipEntry.AddressList[0].ToString();
                SysCommon.Gis.SysGisDB vgisDb = new SysGisDB();
                SysCommon.Authorize.AuthorizeClass.GetConnectInfo(Mod.v_ConfigPath, out Mod.Server, out Mod.Instance, out Mod.Database, out Mod.User, out Mod.Password, out Mod.Version, out Mod.dbType);
                bool blnCanConnect = CanOpenConnect(vgisDb, Mod.dbType, Mod.Server, Mod.Instance, Mod.Database, Mod.User, Mod.Password, Mod.Version);

                if (blnCanConnect == false)
                {
                    MessageBox.Show("数据库连接失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                m_gisDb = vgisDb;
            }
            if (m_sysTable == null)
            {
                SysGisTable sysTable = new SysGisTable(m_gisDb);
                m_sysTable = sysTable;
            }
        }



        public static bool CreateLogTable()
        {
            if (m_sysTable == null)
                return false;
            //InitStaticFields();
            Exception err;
            //SetWorkSpace();
            //m_sysTable = new SysGisTable(m_gisDb);
            m_sysTable.WorkSpace = Plugin.ModuleCommon.TmpWorkSpace;//获得业务库工作空间
            ITable pTable = m_sysTable.OpenTable(m_LogNAME, out err); //OpenTable(m_LogNAME, out err);
            if (pTable != null)
            {
                return true;//若日志表已存在，返回true
            }
            IFields pFields = new FieldsClass();
            IFieldsEdit pFieldsEdit = pFields as IFieldsEdit;
            IField pField = new FieldClass();
            IFieldEdit pEdit = pField as IFieldEdit;
            pEdit.Name_2 = "logTime";
            pEdit.Type_2 = esriFieldType.esriFieldTypeString;
            pEdit.Length_2 = 30;
            pFieldsEdit.AddField(pField);
            pField = new FieldClass();
            pEdit = pField as IFieldEdit;
            pEdit.Name_2 = "logUser";
            pEdit.Type_2 = esriFieldType.esriFieldTypeString;
            pEdit.Length_2 = 50;
            pFieldsEdit.AddField(pField);
            pField = new FieldClass();
            pEdit = pField as IFieldEdit;
            pEdit.Name_2 = "logIP";
            pEdit.Type_2 = esriFieldType.esriFieldTypeString;
            pEdit.Length_2 = 30;
            pFieldsEdit.AddField(pField);
            pField = new FieldClass();
            pEdit = pField as IFieldEdit;
            pEdit.Name_2 = "logEVENT";
            pEdit.Type_2 = esriFieldType.esriFieldTypeString;
            pEdit.Length_2 = 255;
            pFieldsEdit.AddField(pField);
            IFieldChecker pFieldChecker = new FieldCheckerClass();//检查字段有效性
            pFieldChecker.ValidateWorkspace = Plugin.ModuleCommon.TmpWorkSpace;//xisheng 20111221 
            IFields pValidFields = null;
            IEnumFieldError pEFE = null;
            pFieldChecker.Validate(pFields, out pEFE, out pValidFields);
            return m_sysTable.CreateTable(m_LogNAME, pValidFields, out err);
        }
        private static void SetWorkSpace()
        {
            SysCommon.Authorize.AuthorizeClass.GetConnectInfo(Mod.v_ConfigPath, out Mod.Server, out Mod.Instance, out Mod.Database, out Mod.User, out Mod.Password, out Mod.Version, out Mod.dbType);
            bool blnCanConnect = CanOpenConnect(m_gisDb, Mod.dbType, Mod.Server, Mod.Instance, Mod.Database, Mod.User, Mod.Password, Mod.Version);

            if (blnCanConnect == false)
            {
                MessageBox.Show("数据库连接失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }
        public static void WriteLocalLog(string logstr)
        {
            if (_StreamWriter == null)
            {
                string strLogFilePath = System.Windows.Forms.Application.StartupPath + "\\..\\Log";
                string strLogFileName = "SystemLog.txt";
                if (!Directory.Exists(strLogFilePath))
                {
                    Directory.CreateDirectory(strLogFilePath);
                }
                if (!File.Exists(strLogFilePath + "\\" + strLogFileName))
                {
                    FileStream pFileStream = File.Create(strLogFilePath + "\\" + strLogFileName);
                    pFileStream.Close();
                }
                _StreamWriter = new StreamWriter(strLogFilePath + "\\" + strLogFileName, true);
            }
            _StreamWriter.Write(_StreamWriter.NewLine);
            _StreamWriter.Write(logstr+"   "+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            _StreamWriter.Write(_StreamWriter.NewLine);
            _StreamWriter.Flush();
        }
        public static void LocalLogClose()
        {
            if (_StreamWriter != null)
            {
                _StreamWriter.Close();
                _StreamWriter = null;
            }
        }
        //写日志
        public static void Writelog(string logstr)
        {
            if (!CreateLogTable())
                return;

            Dictionary<string, object> dicData = new Dictionary<string, object>();
            string strHostName = Dns.GetHostName();  //得到本机的主机名
            IPHostEntry ipEntry = Dns.GetHostByName(strHostName); //取得本机IP
            string timestr = "";

            switch (Mod.dbType)
            {
                case "SDE":
                    string connstr = "Provider=OraOLEDB.Oracle;Data Source=" + Mod.Database + ";User Id=" + Mod.User + ";Password=" + Mod.Password + ";OLEDB.NET=True;";
                    OleDbConnection pConn = new OleDbConnection(connstr);
                    pConn.Open();
                    OleDbCommand pCommand = pConn.CreateCommand();
                    OleDbDataReader pReader = GetReader(pConn, "SELECT TO_CHAR(SYSDATE, 'YYYY-MM-DD HH24:Mi:SS') FROM dual");
                    if (pReader.Read())
                    {
                        timestr = pReader.GetValue(0).ToString();
                    }
                    pReader.Close();
                    pConn.Close();
                    break;
                default:
                    timestr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    break;
            }
            //SELECT TO_CHAR(SYSDATE, 'YYYY-MM-DD HH24:Mi:SS') FROM dual
            dicData.Add("logTime", timestr);
            dicData.Add("logUser", user);
            dicData.Add("logIP", ipEntry.AddressList[0].ToString());
            dicData.Add("logEVENT", logstr);
            if (_richbox != null)
            {
                _richbox.AppendText(timestr + "/当前用户:" + user + "/在进行-->" + logstr + "\r\n");
            }
            IWorkspace pWorkspace = m_gisDb.WorkSpace;
            ITransactions pTransactions = (ITransactions)pWorkspace;
            try
            {

                if (!pTransactions.InTransaction) pTransactions.StartTransaction();
            }
            catch (Exception eX)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("提示", eX.Message);
                return;
            }
            Exception exError;
            if (!m_sysTable.NewRow(m_LogNAME, dicData, out exError))
            {
                ErrorHandle.ShowFrmErrorHandle("提示", "添加失败！" + exError.Message);
                return;
            }
            try
            {
                if (pTransactions.InTransaction) pTransactions.CommitTransaction();
            }
            catch (Exception eX)
            {
            }

        }
        //写日志 传提示框重载 added by xisheng 2011.07.08
        public static void Writelog(string logstr, RichTextBox richtextbox)
        {
            if (!CreateLogTable())
                return;

            Dictionary<string, object> dicData = new Dictionary<string, object>();
            string strHostName = Dns.GetHostName();  //得到本机的主机名
            IPHostEntry ipEntry = Dns.GetHostByName(strHostName); //取得本机IP
            string timestr = "";

            switch (Mod.dbType)
            {
                case "SDE":
                    string connstr = "Provider=OraOLEDB.Oracle;Data Source=" + Mod.Database + ";User Id=" + Mod.User + ";Password=" + Mod.Password + ";OLEDB.NET=True;";
                    OleDbConnection pConn = new OleDbConnection(connstr);
                    pConn.Open();
                    OleDbCommand pCommand = pConn.CreateCommand();
                    OleDbDataReader pReader = GetReader(pConn, "SELECT TO_CHAR(SYSDATE, 'YYYY-MM-DD HH24:Mi:SS') FROM dual");
                    if (pReader.Read())
                    {
                        timestr = pReader.GetValue(0).ToString();
                    }
                    pReader.Close();
                    pConn.Close();
                    break;
                default:
                    timestr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    break;
            }
            //SELECT TO_CHAR(SYSDATE, 'YYYY-MM-DD HH24:Mi:SS') FROM dual
            dicData.Add("logTime", timestr);
            dicData.Add("logUser", user);
            dicData.Add("logIP", ipEntry.AddressList[0].ToString());
            dicData.Add("logEVENT", logstr);
            if (richtextbox != null)
            {
                richtextbox.AppendText(timestr + "/当前用户:" + user + "/在进行-->" + logstr + "\r\n");
            }
            IWorkspace pWorkspace = m_gisDb.WorkSpace;
            ITransactions pTransactions = (ITransactions)pWorkspace;
            try
            {

                if (!pTransactions.InTransaction) pTransactions.StartTransaction();
            }
            catch (Exception eX)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("提示", eX.Message);
                return;
            }
            Exception exError;
            if (!m_sysTable.NewRow(m_LogNAME, dicData, out exError))
            {
                ErrorHandle.ShowFrmErrorHandle("提示", "添加失败！" + exError.Message);
                return;
            }
            try
            {
                if (pTransactions.InTransaction) pTransactions.CommitTransaction();
            }
            catch (Exception eX)
            {
            }
        }
        //测试链接信息是否可用
        public static bool CanOpenConnect(SysCommon.Gis.SysGisDB vgisDb, string strType, string strServer, string strService, string strDatabase, string strUser, string strPassword, string strVersion)
        {
            bool blnOpen = false;

            Exception Err;

            if (strType.ToUpper() == "ORACLE")
            {
                blnOpen = vgisDb.SetWorkspace(strServer, strService, strDatabase, strUser, strPassword, strVersion, out Err);
            }
            else if (strType.ToUpper() == "ACCESS")
            {
                blnOpen = vgisDb.SetWorkspace(strServer, SysCommon.enumWSType.PDB, out Err);
            }
            else if (strType.ToUpper() == "FILE")
            {
                blnOpen = vgisDb.SetWorkspace(strServer, SysCommon.enumWSType.GDB, out Err);
            }

            return blnOpen;

        }
        //函数功能：获取游标
        //输入参数：数据库连接  sql语句 //输出参数：根据sql语句打开的表的游标
        public static OleDbDataReader GetReader(OleDbConnection conn, string sqlstr)
        {
            OleDbCommand comm = conn.CreateCommand();
            comm.CommandText = sqlstr;
            OleDbDataReader myreader;
            try
            {
                myreader = comm.ExecuteReader();
                return myreader;
            }
            catch (System.Exception e)
            {
                e.Data.Clear();
                return null;
            }
        }
        //查询日志
        public static List<string[]> SeachLog(string inWhere)
        {
            if (m_sysTable == null)
                return null;
            //InitStaticFields();
            Exception err;
            ITable pTable = m_sysTable.OpenTable(m_LogNAME, out err);
            if (pTable == null)
            {
                MessageBox.Show(err.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;//若日志表不存在，返回null
            }
            List<string[]> resList = new List<string[]>();
            string sOrder = null;
            if (inWhere.Contains("order by"))//yjl20120808 add 修改过滤对象支持带order by查询
            {
                int idx=inWhere.IndexOf("order by");
                sOrder = inWhere.Substring(idx);
                inWhere = inWhere.Substring(0, idx);
            }
            IQueryFilter pQF = new QueryFilterClass();
            pQF.WhereClause = inWhere;
            ICursor pCursor = pTable.Search(pQF, false);
            if (sOrder != null)
            {
                IQueryFilterDefinition queryFilterDefinition = (IQueryFilterDefinition)pQF;
                queryFilterDefinition.PostfixClause = sOrder;
            }
            IRow pRow = pCursor.NextRow();
            int i1 = pTable.FindField("logTime");
            int i3 = pTable.FindField("logUser");
            int i2 = pTable.FindField("logIP");
            int i4 = pTable.FindField("logEVENT");
            while (pRow != null)
            {
                string[] logRow = new string[4];
                logRow[0] = pRow.get_Value(i1).ToString();
                logRow[1] = pRow.get_Value(i2).ToString();
                logRow[2] = pRow.get_Value(i3).ToString();
                logRow[3] = pRow.get_Value(i4).ToString();
                resList.Add(logRow);
                pRow = pCursor.NextRow();
            }



            return resList;


        }
        //重载一个无参的查询日志
        public List<string[]> SeachLog()
        {
            if (m_sysTable == null)
                return null;
            Exception err;
            ITable pTable = m_sysTable.OpenTable(m_LogNAME, out err);
            if (pTable == null)
            {
                MessageBox.Show(err.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;//若日志表不存在，返回null
            }
            List<string[]> resList = new List<string[]>();
            IQueryFilter pQF = new QueryFilterClass();
            pQF.WhereClause = "";
            ICursor pCursor = pTable.Search(pQF, false);
            IRow pRow = pCursor.NextRow();
            int i1 = pTable.FindField("logTime");
            int i2 = pTable.FindField("logUser");
            int i3 = pTable.FindField("logIP");
            int i4 = pTable.FindField("logEVENT");
            while (pRow != null)
            {
                string[] logRow = new string[4];
                logRow[0] = pRow.get_Value(i1).ToString();
                logRow[1] = pRow.get_Value(i2).ToString();
                logRow[2] = pRow.get_Value(i3).ToString();
                logRow[3] = pRow.get_Value(i4).ToString();
                resList.Add(logRow);
                pRow = pCursor.NextRow();
            }



            return resList;

        }
        //重载一个去重复枚举字段值的查询日志
        public static List<string> SeachLog2(string inFieldName)
        {
            if (m_sysTable == null)
                return null;
            Exception err;
            ITable pTable = m_sysTable.OpenTable(m_LogNAME, out err);
            if (pTable == null)
            {
                MessageBox.Show(err.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;//若日志表不存在，返回null
            }
            List<string> resList = new List<string>();
            IQueryDef pQueryDef = (m_sysTable.WorkSpace as IFeatureWorkspace).CreateQueryDef();
            pQueryDef.Tables = m_LogNAME;
            pQueryDef.SubFields = "distinct(" + inFieldName + ")";
            //IQueryFilter pQF = new QueryFilterClass();
            //pQF.WhereClause = "";
            //ICursor pCursor = pTable.Search(pQF, false);

            //int i1 = pTable.FindField(inFieldName);
            ICursor pCursor = pQueryDef.Evaluate();
            IRow pRow = pCursor.NextRow();
            while (pRow != null)
            {
                string logRow = string.Empty;
                logRow = pRow.get_Value(0).ToString();
                resList.Add(logRow);
                pRow = pCursor.NextRow();
            }
            return resList;

        }
        //清空日志
        public static void ClearLog(ListView lv)
        {
            if (m_sysTable == null)
                return;
            Exception err;
            ITable pTable = m_sysTable.OpenTable(m_LogNAME, out err);
            if (pTable == null)
            {
                MessageBox.Show(err.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;//若日志表不存在，返回null
            }
            //for (int i = 0; i < lv.Items.Count;i++ )
            //{
            //    ListViewItem  lvi = lv.Items[i];
            //    IQueryFilter pQF = new QueryFilterClass();
            //    pQF.WhereClause = "logTime = '" + lvi.SubItems[0].Text
            //        + "' AND logUser = '" + lvi.SubItems[2].Text
            //        + "' AND logIP = '" + lvi.SubItems[1].Text
            //        + "' AND logEVENT = '" + lvi.SubItems[3].Text+"'";
            IWorkspace pWorkspace = m_gisDb.WorkSpace;
            ITransactions pTransactions = (ITransactions)pWorkspace;
            try
            {

                if (!pTransactions.InTransaction) pTransactions.StartTransaction();
            }
            catch (Exception eX)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("提示", eX.Message);
                return;
            }
            Exception exError;
            if (!m_sysTable.DeleteRows(m_LogNAME, "", out exError))
            {
                ErrorHandle.ShowFrmErrorHandle("提示", "清空日志失败！" + exError.Message);
                return;
            }
            try
            {
                if (pTransactions.InTransaction) pTransactions.CommitTransaction();
            }
            catch (Exception eX)
            {
            }


            //}

        }
        //删除选择的日志
        public static void DeleteSelectedLog(ListView lv)
        {
            if (m_sysTable == null)
                return;
            Exception err;
            ITable pTable = m_sysTable.OpenTable(m_LogNAME, out err);
            if (pTable == null)
            {
                MessageBox.Show(err.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;//若日志表不存在，返回null
            }
            for (int i = 0; i < lv.SelectedItems.Count; i++)
            {
                ListViewItem lvi = lv.SelectedItems[i];
                IQueryFilter pQF = new QueryFilterClass();
                pQF.WhereClause = "logTime = '" + lvi.SubItems[0].Text
                    + "' AND logUser = '" + lvi.SubItems[2].Text
                    + "' AND logIP = '" + lvi.SubItems[1].Text
                    + "' AND logEVENT = '" + lvi.SubItems[3].Text + "'";
                IWorkspace pWorkspace = m_gisDb.WorkSpace;
                ITransactions pTransactions = (ITransactions)pWorkspace;
                try
                {

                    if (!pTransactions.InTransaction) pTransactions.StartTransaction();
                }
                catch (Exception eX)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("提示", eX.Message);
                    return;
                }
                Exception exError;
                if (!m_sysTable.DeleteRows(m_LogNAME, pQF.WhereClause, out exError))
                {
                    ErrorHandle.ShowFrmErrorHandle("提示", "删除日志失败！" + exError.Message);
                    return;
                }
                try
                {
                    if (pTransactions.InTransaction) pTransactions.CommitTransaction();
                }
                catch (Exception eX)
                {
                }


            }//end for

        }
        //导出日志
        public static void ExportLog(string fileName, ListView inListView)
        {
            string saveFileName = "";
            //bool fileSaved = false;
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.DefaultExt = "xls";
            saveDialog.Filter = "Excel文件(*.xls)|*.xls";
            saveDialog.FileName = fileName;
            saveDialog.ShowDialog();
            System.Windows.Forms.Application.DoEvents();
            saveFileName = saveDialog.FileName;
            if (saveFileName.IndexOf(":") < 0) return; //被点了取消 
            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            if (xlApp == null)
            {
                MessageBox.Show("无法创建Excel对象，可能您的机子未安装Excel", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            SysCommon.CProgress cProgress = new SysCommon.CProgress("开始导出日志");
            cProgress.ShowProgress();
            cProgress.TopMost = true;
            cProgress.ShowDescription = true;
            cProgress.MaxValue = inListView.Items.Count + 2;
            cProgress.SetProgress("正在创建EXCEL文件结构...");
            Microsoft.Office.Interop.Excel.Workbooks workbooks = xlApp.Workbooks;
            Microsoft.Office.Interop.Excel.Workbook workbook = workbooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet);
            Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets[1];//取得sheet1

            //写入标题
            for (int i = 0; i < inListView.Columns.Count; i++)
            {
                worksheet.Cells[1, i + 1] = inListView.Columns[i].Text;//注意exel起始索引1
            }
            cProgress.SetProgress(1, "正在写入日志...");
            //写入数值
            for (int r = 0; r < inListView.Items.Count; r++)
            {
                //worksheet.Cells[r + 2, 1] = inListView.Items[r].Text;
                for (int i = 0; i < inListView.Items[r].SubItems.Count; i++)
                {
                    worksheet.Cells[r + 2, i + 1] = inListView.Items[r].SubItems[i].Text;
                }
                cProgress.ProgresssValue++;

            }
            worksheet.Columns.EntireColumn.AutoFit();//列宽自适应
            //if (Microsoft.Office.Interop.cmbxType.Text != "Notification")
            //{
            //    Excel.Range rg = worksheet.get_Range(worksheet.Cells[2, 2], worksheet.Cells[ds.Tables[0].Rows.Count + 1, 2]);
            //    rg.NumberFormat = "00000000";
            //}
            cProgress.SetProgress("正在保存日志文件");
            if (saveFileName != "")
            {
                try
                {
                    workbook.Saved = true;
                    workbook.SaveAs(saveFileName, 56, null, null, null, null, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, null, null, null, null, null);
                    //workbook.SaveCopyAs(saveFileName);
                    //fileSaved = true;
                    cProgress.ProgresssValue = inListView.Items.Count + 2;
                    cProgress.Close();
                }
                catch (Exception ex)
                {
                    //fileSaved = false;
                    MessageBox.Show("导出文件时出错,文件可能正被打开！\n" + ex.Message, "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    cProgress.Close();
                    return;
                }

            }
            //else
            //{
            //    fileSaved = false;
            //}
            xlApp.Quit();
            GC.Collect();//强行销毁 
            // if (fileSaved && System.IO.File.Exists(saveFileName)) System.Diagnostics.Process.Start(saveFileName); //打开EXCEL
            MessageBox.Show("导出成功！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        //导出日志 导出当前用户和IP的日志 20110810
        public static string ExportLog()
        {

            string err = "";
            string saveFileName = "";
            //bool fileSaved = false;
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.DefaultExt = "xls";
            saveDialog.Filter = "Excel文件(*.xls)|*.xls";
            saveDialog.FileName = "日志文件";
            saveDialog.ShowDialog();
            System.Windows.Forms.Application.DoEvents();
            saveFileName = saveDialog.FileName;
            if (saveFileName.IndexOf(":") < 0) return ""; //被点了取消 
            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            if (xlApp == null)
            {
                err = "无法创建Excel对象，可能您的机子未安装Excel";
                return err;
            }

            Microsoft.Office.Interop.Excel.Workbooks workbooks = xlApp.Workbooks;
            Microsoft.Office.Interop.Excel.Workbook workbook = workbooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet);
            Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets[1];//取得sheet1

            string[] strHeader = { "访问时间", "访问IP", "用户名", "操作时间" };
            //写入标题
            for (int i = 0; i < 4; i++)
            {
                worksheet.Cells[1, i + 1] = strHeader[i];//注意exel起始索引1
            }
            string whereclause = "";
            whereclause += (user == "") ? "" : "logUser = '" + user + "' AND ";
            whereclause += (userIP == "") ? "" : "logIP = '" + userIP + "'";
            if (whereclause.EndsWith(" AND "))
                whereclause = whereclause.Substring(0, whereclause.Length - 5);

            List<string[]> list = SeachLog(whereclause);

            if (list.Count == 0)
            {
                err = "没有符合条件的日志";
                return err;
            }

            //写入数值
            for (int r = 0; r < list.Count; r++)
            {
                //worksheet.Cells[r + 2, 1] = inListView.Items[r].Text;
                string[] strRow = list[r];
                for (int i = 0; i < 4; i++)
                {
                    worksheet.Cells[r + 2, i + 1] = strRow[i];
                }

            }
            worksheet.Columns.EntireColumn.AutoFit();//列宽自适应
            //if (Microsoft.Office.Interop.cmbxType.Text != "Notification")
            //{
            //    Excel.Range rg = worksheet.get_Range(worksheet.Cells[2, 2], worksheet.Cells[ds.Tables[0].Rows.Count + 1, 2]);
            //    rg.NumberFormat = "00000000";
            //}

            if (saveFileName != "")
            {
                try
                {
                    workbook.Saved = true;
                    workbook.SaveCopyAs(saveFileName);
                    //fileSaved = true;
                }
                catch (Exception ex)
                {
                    //fileSaved = false;
                    err = "导出文件时出错,文件可能正被打开！\n" + ex.Message;
                    return err;
                }

            }
            //else
            //{
            //    fileSaved = false;
            //}
            xlApp.Quit();
            GC.Collect();//强行销毁 
            // if (fileSaved && System.IO.File.Exists(saveFileName)) System.Diagnostics.Process.Start(saveFileName); //打开EXCEL
            err = "导出成功！";
            return err;
        }
        //新增获取用户组唯一值 ygc 2012-9-3
        public static List<string> GetGroupUser()
        {
            List<string> newList = new List<string>();
            if (m_sysTable == null)
                return null;
            Exception err;
            ITable pTable = m_sysTable.OpenTable("user_info ", out err);
            if (pTable == null)
            {
                MessageBox.Show(err.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;
            }
            IQueryDef pQueryDef = (m_sysTable.WorkSpace as IFeatureWorkspace).CreateQueryDef();
            pQueryDef.Tables = "user_info ";
            pQueryDef.SubFields = "distinct(uposition)";
            ICursor pCursor = pQueryDef.Evaluate();
            try
            {
                IRow pRow = pCursor.NextRow();
                while (pRow != null)
                {
                    string logRow = string.Empty;
                    logRow = pRow.get_Value(0).ToString();
                    newList.Add(logRow);
                    pRow = pCursor.NextRow();
                }
            }
            catch { }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pCursor);
            }
            return newList;
        }
        //通过用户组名称获取该用户组中的用户名 ygc 2012-9-3
        public static List<string> GetUsersByGroupUsers(string GroupUser)
        {
            List<string> newList = new List<string>();
            if (m_sysTable == null)
                return null;
            Exception err;
            ITable pTable = m_sysTable.OpenTable("user_info ", out err);
            if (pTable == null)
            {
                MessageBox.Show(err.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;
            }
            IQueryDef pQueryDef = (m_sysTable.WorkSpace as IFeatureWorkspace).CreateQueryDef();
            pQueryDef.Tables = "user_info ";
            pQueryDef.SubFields = "distinct(name)";
            pQueryDef.WhereClause = "uposition='" + GroupUser + "'";
            ICursor pCursor = pQueryDef.Evaluate();
            try
            {
                IRow pRow = pCursor.NextRow();
                while (pRow != null)
                {
                    string logRow = string.Empty;
                    logRow = pRow.get_Value(0).ToString();
                    newList.Add(logRow);
                    pRow = pCursor.NextRow();
                }
            }
            catch { }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pCursor);
            }
            return newList;
        }
    }
}