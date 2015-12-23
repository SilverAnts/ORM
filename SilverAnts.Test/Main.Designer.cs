namespace SilverAnts.Test
{
    partial class Main
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.group_Data = new System.Windows.Forms.GroupBox();
            this.gd_View = new System.Windows.Forms.DataGridView();
            this.OperationName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OperationDes = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TimeSpan = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.query = new System.Windows.Forms.Button();
            this.combDataType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblPrividor = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtLimitCount = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtLimit = new System.Windows.Forms.TextBox();
            this.group_Data.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gd_View)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // group_Data
            // 
            this.group_Data.Controls.Add(this.gd_View);
            this.group_Data.Dock = System.Windows.Forms.DockStyle.Fill;
            this.group_Data.Location = new System.Drawing.Point(0, 0);
            this.group_Data.Name = "group_Data";
            this.group_Data.Size = new System.Drawing.Size(794, 551);
            this.group_Data.TabIndex = 1;
            this.group_Data.TabStop = false;
            this.group_Data.Text = "测试信息";
            // 
            // gd_View
            // 
            this.gd_View.AllowUserToAddRows = false;
            this.gd_View.AllowUserToDeleteRows = false;
            this.gd_View.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gd_View.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.OperationName,
            this.OperationDes,
            this.TimeSpan});
            this.gd_View.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.gd_View.Location = new System.Drawing.Point(3, 47);
            this.gd_View.Name = "gd_View";
            this.gd_View.ReadOnly = true;
            this.gd_View.RowTemplate.Height = 23;
            this.gd_View.Size = new System.Drawing.Size(788, 501);
            this.gd_View.TabIndex = 3;
            // 
            // OperationName
            // 
            this.OperationName.HeaderText = "操作类型";
            this.OperationName.Name = "OperationName";
            this.OperationName.ReadOnly = true;
            // 
            // OperationDes
            // 
            this.OperationDes.HeaderText = "操作内容";
            this.OperationDes.Name = "OperationDes";
            this.OperationDes.ReadOnly = true;
            this.OperationDes.Width = 400;
            // 
            // TimeSpan
            // 
            this.TimeSpan.HeaderText = "时间间隔";
            this.TimeSpan.Name = "TimeSpan";
            this.TimeSpan.ReadOnly = true;
            this.TimeSpan.Width = 200;
            // 
            // query
            // 
            this.query.Location = new System.Drawing.Point(411, 11);
            this.query.Name = "query";
            this.query.Size = new System.Drawing.Size(60, 23);
            this.query.TabIndex = 0;
            this.query.Text = "测试";
            this.query.UseVisualStyleBackColor = true;
            this.query.Click += new System.EventHandler(this.query_Click);
            // 
            // combDataType
            // 
            this.combDataType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combDataType.FormattingEnabled = true;
            this.combDataType.Items.AddRange(new object[] {
            "SQLite",
            "SQLServer",
            "MySql"});
            this.combDataType.Location = new System.Drawing.Point(54, 13);
            this.combDataType.Name = "combDataType";
            this.combDataType.Size = new System.Drawing.Size(123, 20);
            this.combDataType.TabIndex = 1;
            this.combDataType.SelectedIndexChanged += new System.EventHandler(this.combDataType_TextChanged);
            this.combDataType.TextChanged += new System.EventHandler(this.combDataType_TextChanged);
            this.combDataType.MouseLeave += new System.EventHandler(this.combDataType_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "数据源";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblPrividor);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtLimitCount);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtLimit);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.query);
            this.groupBox1.Controls.Add(this.combDataType);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(794, 41);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "操作";
            // 
            // lblPrividor
            // 
            this.lblPrividor.AutoSize = true;
            this.lblPrividor.Location = new System.Drawing.Point(528, 17);
            this.lblPrividor.Name = "lblPrividor";
            this.lblPrividor.Size = new System.Drawing.Size(0, 12);
            this.lblPrividor.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(301, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "插入次数";
            // 
            // txtLimitCount
            // 
            this.txtLimitCount.Location = new System.Drawing.Point(360, 11);
            this.txtLimitCount.Name = "txtLimitCount";
            this.txtLimitCount.Size = new System.Drawing.Size(42, 21);
            this.txtLimitCount.TabIndex = 3;
            this.txtLimitCount.Text = "1000";
            this.txtLimitCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(194, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "查询次数";
            // 
            // txtLimit
            // 
            this.txtLimit.Location = new System.Drawing.Point(253, 12);
            this.txtLimit.Name = "txtLimit";
            this.txtLimit.Size = new System.Drawing.Size(42, 21);
            this.txtLimit.TabIndex = 3;
            this.txtLimit.Text = "10";
            this.txtLimit.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(794, 551);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.group_Data);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "查询测试";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.group_Data.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gd_View)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox group_Data;
        private System.Windows.Forms.ComboBox combDataType;
        private System.Windows.Forms.Button query;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView gd_View;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtLimit;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtLimitCount;
        private System.Windows.Forms.Label lblPrividor;
        private System.Windows.Forms.DataGridViewTextBoxColumn OperationName;
        private System.Windows.Forms.DataGridViewTextBoxColumn OperationDes;
        private System.Windows.Forms.DataGridViewTextBoxColumn TimeSpan;
    }
}

