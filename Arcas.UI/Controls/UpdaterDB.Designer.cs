namespace Arcas.Controls
{
    partial class UpdaterDB
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

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Обязательный метод для поддержки конструктора - не изменяйте 
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.chbTransaction = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbComment = new System.Windows.Forms.TextBox();
            this.btSaveScript = new System.Windows.Forms.Button();
            this.tbScriptBody = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbxTfsDbLinc = new System.Windows.Forms.ComboBox();
            this.btClear = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // chbTransaction
            // 
            this.chbTransaction.AutoSize = true;
            this.chbTransaction.Location = new System.Drawing.Point(260, 7);
            this.chbTransaction.Name = "chbTransaction";
            this.chbTransaction.Size = new System.Drawing.Size(159, 17);
            this.chbTransaction.TabIndex = 15;
            this.chbTransaction.Text = "Выполнять в транзанкции";
            this.chbTransaction.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 117);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Тело скрипта";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(130, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Комментарий к накатке";
            // 
            // tbComment
            // 
            this.tbComment.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbComment.Location = new System.Drawing.Point(3, 52);
            this.tbComment.Multiline = true;
            this.tbComment.Name = "tbComment";
            this.tbComment.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbComment.Size = new System.Drawing.Size(669, 50);
            this.tbComment.TabIndex = 12;
            // 
            // btSaveScript
            // 
            this.btSaveScript.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btSaveScript.Location = new System.Drawing.Point(563, 560);
            this.btSaveScript.Name = "btSaveScript";
            this.btSaveScript.Size = new System.Drawing.Size(109, 23);
            this.btSaveScript.TabIndex = 11;
            this.btSaveScript.Text = "Сохранить скрипт";
            this.btSaveScript.UseVisualStyleBackColor = true;
            this.btSaveScript.Click += new System.EventHandler(this.btSaveScript_Click);
            // 
            // tbScriptBody
            // 
            this.tbScriptBody.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbScriptBody.Location = new System.Drawing.Point(3, 133);
            this.tbScriptBody.Multiline = true;
            this.tbScriptBody.Name = "tbScriptBody";
            this.tbScriptBody.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbScriptBody.Size = new System.Drawing.Size(669, 421);
            this.tbScriptBody.TabIndex = 10;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Связка TFS-DB";
            // 
            // cbxTfsDbLinc
            // 
            this.cbxTfsDbLinc.DisplayMember = "Name";
            this.cbxTfsDbLinc.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxTfsDbLinc.FormattingEnabled = true;
            this.cbxTfsDbLinc.Location = new System.Drawing.Point(94, 3);
            this.cbxTfsDbLinc.Name = "cbxTfsDbLinc";
            this.cbxTfsDbLinc.Size = new System.Drawing.Size(146, 21);
            this.cbxTfsDbLinc.TabIndex = 8;
            // 
            // btClear
            // 
            this.btClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btClear.Location = new System.Drawing.Point(532, 104);
            this.btClear.Name = "btClear";
            this.btClear.Size = new System.Drawing.Size(129, 23);
            this.btClear.TabIndex = 16;
            this.btClear.Text = "Очистить поля ввода";
            this.btClear.UseVisualStyleBackColor = true;
            this.btClear.Click += new System.EventHandler(this.btClear_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btClear);
            this.panel1.Controls.Add(this.chbTransaction);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.tbComment);
            this.panel1.Controls.Add(this.btSaveScript);
            this.panel1.Controls.Add(this.tbScriptBody);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.cbxTfsDbLinc);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.MinimumSize = new System.Drawing.Size(450, 300);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(675, 591);
            this.panel1.TabIndex = 17;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 250F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(931, 597);
            this.tableLayoutPanel1.TabIndex = 17;
            // 
            // panel2
            // 
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(684, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(244, 591);
            this.panel2.TabIndex = 18;
            // 
            // UpdaterDB
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "UpdaterDB";
            this.Size = new System.Drawing.Size(931, 597);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox chbTransaction;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbComment;
        private System.Windows.Forms.Button btSaveScript;
        private System.Windows.Forms.TextBox tbScriptBody;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbxTfsDbLinc;
        private System.Windows.Forms.Button btClear;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel2;
    }
}
