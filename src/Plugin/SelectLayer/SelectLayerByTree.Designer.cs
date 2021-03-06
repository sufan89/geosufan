﻿namespace Fan.Plugin
{
    partial class SelectLayerByTree
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectLayerByTree));
            this.advTreeLayerList = new DevExpress.XtraTreeList.TreeList();
            this.node1 = new DevExpress.XtraTreeList.Nodes.TreeListNode();
            this.buttonCancel = new DevExpress.XtraEditors.SimpleButton();
            this.buttonOK = new DevExpress.XtraEditors.SimpleButton();
            this.ImageList = new System.Windows.Forms.ImageList(this.components);
            this.labelErr = new DevExpress.XtraEditors.LabelControl();
            this.SuspendLayout();
            // 
            // advTreeLayerList
            // 
            this.advTreeLayerList.AccessibleRole = System.Windows.Forms.AccessibleRole.Outline;
            this.advTreeLayerList.AllowDrop = true;
            this.advTreeLayerList.BackColor = System.Drawing.SystemColors.Window;
            // 
            // 
            // 
            this.advTreeLayerList.Dock = System.Windows.Forms.DockStyle.Top;
            this.advTreeLayerList.Location = new System.Drawing.Point(0, 0);
            this.advTreeLayerList.Name = "advTreeLayerList";
            this.advTreeLayerList.Size = new System.Drawing.Size(322, 295);
            this.advTreeLayerList.TabIndex = 1;
            this.advTreeLayerList.Text = "advTree1";
            // 
            // node1
            // 
            this.node1.Expanded = true;
            // 
            // nodeConnector1
            // 
            // 
            // elementStyle1
            // 
            // 
            // buttonCancel
            // 
            this.buttonCancel.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.buttonCancel.Location = new System.Drawing.Point(240, 308);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(70, 23);
            this.buttonCancel.TabIndex = 14;
            this.buttonCancel.Text = "取消";
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.buttonOK.Location = new System.Drawing.Point(154, 308);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(70, 23);
            this.buttonOK.TabIndex = 13;
            this.buttonOK.Text = "确定";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // ImageList
            // 
            this.ImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ImageList.ImageStream")));
            this.ImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.ImageList.Images.SetKeyName(0, "earth");
            this.ImageList.Images.SetKeyName(1, "Root");
            this.ImageList.Images.SetKeyName(2, "DIR");
            this.ImageList.Images.SetKeyName(3, "DataDIRHalfOpen");
            this.ImageList.Images.SetKeyName(4, "DataDIRClosed");
            this.ImageList.Images.SetKeyName(5, "DataDIROpen");
            this.ImageList.Images.SetKeyName(6, "Layer");
            this.ImageList.Images.SetKeyName(7, "_annotation");
            this.ImageList.Images.SetKeyName(8, "_Dimension");
            this.ImageList.Images.SetKeyName(9, "_line");
            this.ImageList.Images.SetKeyName(10, "_MultiPatch");
            this.ImageList.Images.SetKeyName(11, "_point");
            this.ImageList.Images.SetKeyName(12, "_polygon");
            this.ImageList.Images.SetKeyName(13, "annotation");
            this.ImageList.Images.SetKeyName(14, "Dimension");
            this.ImageList.Images.SetKeyName(15, "line");
            this.ImageList.Images.SetKeyName(16, "MultiPatch");
            this.ImageList.Images.SetKeyName(17, "point");
            this.ImageList.Images.SetKeyName(18, "polygon");
            this.ImageList.Images.SetKeyName(19, "PublicVersion");
            this.ImageList.Images.SetKeyName(20, "PersonalVersion");
            this.ImageList.Images.SetKeyName(21, "INVISIBLE");
            this.ImageList.Images.SetKeyName(22, "VISIBLE");
            // 
            // labelErr
            // 
            this.labelErr.AutoSize = true;
            this.labelErr.BackColor = System.Drawing.Color.Transparent;
            this.labelErr.ForeColor = System.Drawing.Color.Red;
            this.labelErr.Location = new System.Drawing.Point(0, 311);
            this.labelErr.Name = "labelErr";
            this.labelErr.Size = new System.Drawing.Size(0, 0);
            this.labelErr.TabIndex = 15;
            // 
            // SelectLayerByTree
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(322, 343);
            this.Controls.Add(this.labelErr);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.advTreeLayerList);
            this.DoubleBuffered = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectLayerByTree";
            this.ShowIcon = false;
            this.Text = "选择图层";
            this.Load += new System.EventHandler(this.SelectLayerByTree_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraTreeList.TreeList advTreeLayerList;
        private DevExpress.XtraTreeList.Nodes.TreeListNode node1;
        private DevExpress.XtraEditors.SimpleButton buttonCancel;
        private DevExpress.XtraEditors.SimpleButton buttonOK;
        public System.Windows.Forms.ImageList ImageList;
        private DevExpress.XtraEditors.LabelControl labelErr;
    }
}