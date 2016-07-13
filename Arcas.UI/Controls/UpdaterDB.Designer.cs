namespace DevTools.Controls
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
            this.tbComment.Size = new System.Drawing.Size(631, 50);
            this.tbComment.TabIndex = 12;
            // 
            // btSaveScript
            // 
            this.btSaveScript.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btSaveScript.Location = new System.Drawing.Point(525, 390);
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
            this.tbScriptBody.Size = new System.Drawing.Size(631, 251);
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
            this.btClear.Location = new System.Drawing.Point(505, 107);
            this.btClear.Name = "btClear";
            this.btClear.Size = new System.Drawing.Size(129, 23);
            this.btClear.TabIndex = 16;
            this.btClear.Text = "Очистить поля ввода";
            this.btClear.UseVisualStyleBackColor = true;
            this.btClear.Click += new System.EventHandler(this.btClear_Click);
            // 
            // UpdaterDB
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btClear);
            this.Controls.Add(this.chbTransaction);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbComment);
            this.Controls.Add(this.btSaveScript);
            this.Controls.Add(this.tbScriptBody);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbxTfsDbLinc);
            this.Name = "UpdaterDB";
            this.Size = new System.Drawing.Size(637, 418);
            this.ResumeLayout(false);
            this.PerformLayout();

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
    }
}
