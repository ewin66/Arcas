namespace DevTools
{
    partial class DevToolsMainMindow
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DevToolsMainMindow));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.файлToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.выходToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.настройкиToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TFSDBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tcTabs = new System.Windows.Forms.TabControl();
            this.tabPageDBVer = new System.Windows.Forms.TabPage();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tsStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.tpXsltTransform = new System.Windows.Forms.TabPage();
            this.updaterDB1 = new DevTools.Controls.UpdaterDB();
            this.xsltTransform1 = new DevTools.Controls.XsltTransform();
            this.menuStrip1.SuspendLayout();
            this.tcTabs.SuspendLayout();
            this.tabPageDBVer.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.tpXsltTransform.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.файлToolStripMenuItem,
            this.настройкиToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(940, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // файлToolStripMenuItem
            // 
            this.файлToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.выходToolStripMenuItem});
            this.файлToolStripMenuItem.Name = "файлToolStripMenuItem";
            this.файлToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.файлToolStripMenuItem.Text = "&Файл";
            // 
            // выходToolStripMenuItem
            // 
            this.выходToolStripMenuItem.Name = "выходToolStripMenuItem";
            this.выходToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
            this.выходToolStripMenuItem.Text = "Выход";
            this.выходToolStripMenuItem.Click += new System.EventHandler(this.выходToolStripMenuItem_Click);
            // 
            // настройкиToolStripMenuItem
            // 
            this.настройкиToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TFSDBToolStripMenuItem});
            this.настройкиToolStripMenuItem.Name = "настройкиToolStripMenuItem";
            this.настройкиToolStripMenuItem.Size = new System.Drawing.Size(79, 20);
            this.настройкиToolStripMenuItem.Text = "Настройки";
            // 
            // TFSDBToolStripMenuItem
            // 
            this.TFSDBToolStripMenuItem.Name = "TFSDBToolStripMenuItem";
            this.TFSDBToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.TFSDBToolStripMenuItem.Text = "Связка TFS-DB";
            this.TFSDBToolStripMenuItem.Click += new System.EventHandler(this.TFSDBToolStripMenuItem_Click);
            // 
            // tcTabs
            // 
            this.tcTabs.Controls.Add(this.tabPageDBVer);
            this.tcTabs.Controls.Add(this.tpXsltTransform);
            this.tcTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcTabs.Location = new System.Drawing.Point(0, 24);
            this.tcTabs.Name = "tcTabs";
            this.tcTabs.SelectedIndex = 0;
            this.tcTabs.Size = new System.Drawing.Size(940, 526);
            this.tcTabs.TabIndex = 3;
            // 
            // tabPageDBVer
            // 
            this.tabPageDBVer.Controls.Add(this.updaterDB1);
            this.tabPageDBVer.Location = new System.Drawing.Point(4, 22);
            this.tabPageDBVer.Name = "tabPageDBVer";
            this.tabPageDBVer.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageDBVer.Size = new System.Drawing.Size(932, 500);
            this.tabPageDBVer.TabIndex = 1;
            this.tabPageDBVer.Text = "Накатка БД";
            this.tabPageDBVer.UseVisualStyleBackColor = true;
            this.tabPageDBVer.Enter += new System.EventHandler(this.tabPageDBVer_Enter);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsStatusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 550);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(940, 22);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tsStatusLabel
            // 
            this.tsStatusLabel.Name = "tsStatusLabel";
            this.tsStatusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // tpXsltTransform
            // 
            this.tpXsltTransform.Controls.Add(this.xsltTransform1);
            this.tpXsltTransform.Location = new System.Drawing.Point(4, 22);
            this.tpXsltTransform.Name = "tpXsltTransform";
            this.tpXsltTransform.Padding = new System.Windows.Forms.Padding(3);
            this.tpXsltTransform.Size = new System.Drawing.Size(932, 500);
            this.tpXsltTransform.TabIndex = 2;
            this.tpXsltTransform.Text = "Xslt-преобразование";
            this.tpXsltTransform.UseVisualStyleBackColor = true;
            // 
            // updaterDB1
            // 
            this.updaterDB1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.updaterDB1.Location = new System.Drawing.Point(3, 3);
            this.updaterDB1.Name = "updaterDB1";
            this.updaterDB1.Size = new System.Drawing.Size(926, 494);
            this.updaterDB1.TabIndex = 0;
            // 
            // xsltTransform1
            // 
            this.xsltTransform1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xsltTransform1.Location = new System.Drawing.Point(3, 3);
            this.xsltTransform1.Name = "xsltTransform1";
            this.xsltTransform1.Size = new System.Drawing.Size(926, 494);
            this.xsltTransform1.TabIndex = 0;
            // 
            // DevToolsMainMindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(940, 572);
            this.Controls.Add(this.tcTabs);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(300, 300);
            this.Name = "DevToolsMainMindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Аркас";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DevToolsMainMindow_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tcTabs.ResumeLayout(false);
            this.tabPageDBVer.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.tpXsltTransform.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem файлToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem выходToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem настройкиToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem TFSDBToolStripMenuItem;
        private System.Windows.Forms.TabControl tcTabs;
        private System.Windows.Forms.TabPage tabPageDBVer;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tsStatusLabel;
        private Controls.UpdaterDB updaterDB1;
        private System.Windows.Forms.TabPage tpXsltTransform;
        private Controls.XsltTransform xsltTransform1;
    }
}

