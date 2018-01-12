namespace Arcas.Controls
{
    partial class WsdlXsdCsGen
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btFromFile = new System.Windows.Forms.Button();
            this.tbWsdlUri = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chbCreateAsuncMethod = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.chbCreateAsuncMethod);
            this.groupBox1.Controls.Add(this.btFromFile);
            this.groupBox1.Controls.Add(this.tbWsdlUri);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(672, 237);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Генерация из WSDL";
            // 
            // btFromFile
            // 
            this.btFromFile.Location = new System.Drawing.Point(87, 23);
            this.btFromFile.Name = "btFromFile";
            this.btFromFile.Size = new System.Drawing.Size(67, 23);
            this.btFromFile.TabIndex = 2;
            this.btFromFile.Text = "Из файла";
            this.btFromFile.UseVisualStyleBackColor = true;
            this.btFromFile.Click += new System.EventHandler(this.btFromFile_Click);
            // 
            // tbWsdlUri
            // 
            this.tbWsdlUri.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbWsdlUri.Location = new System.Drawing.Point(160, 25);
            this.tbWsdlUri.MaxLength = 250;
            this.tbWsdlUri.Name = "tbWsdlUri";
            this.tbWsdlUri.Size = new System.Drawing.Size(481, 20);
            this.tbWsdlUri.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Путь к WSDL";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Location = new System.Drawing.Point(0, 243);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(672, 267);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Генерация из XSD";
            // 
            // chbCreateAsuncMethod
            // 
            this.chbCreateAsuncMethod.AutoSize = true;
            this.chbCreateAsuncMethod.Location = new System.Drawing.Point(9, 62);
            this.chbCreateAsuncMethod.Name = "chbCreateAsuncMethod";
            this.chbCreateAsuncMethod.Size = new System.Drawing.Size(192, 17);
            this.chbCreateAsuncMethod.TabIndex = 3;
            this.chbCreateAsuncMethod.Text = "Создавать асинхронные методы";
            this.chbCreateAsuncMethod.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(310, 147);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // WsdlXsdCsGen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Name = "WsdlXsdCsGen";
            this.Size = new System.Drawing.Size(672, 521);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btFromFile;
        private System.Windows.Forms.TextBox tbWsdlUri;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox chbCreateAsuncMethod;
    }
}
