namespace PCSSoftware
{
    partial class fMain
    {
        /// <summary>
        /// Обязательная переменная конструктора.
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
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.DrawPanel = new System.Windows.Forms.Panel();
            this.bPrint = new System.Windows.Forms.Button();
            this.rbEcho = new System.Windows.Forms.RadioButton();
            this.rbFinn = new System.Windows.Forms.RadioButton();
            this.rbPhase = new System.Windows.Forms.RadioButton();
            this.rbLi = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // DrawPanel
            // 
            this.DrawPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DrawPanel.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.DrawPanel.Location = new System.Drawing.Point(12, 12);
            this.DrawPanel.Name = "DrawPanel";
            this.DrawPanel.Size = new System.Drawing.Size(760, 507);
            this.DrawPanel.TabIndex = 0;
            // 
            // bPrint
            // 
            this.bPrint.Location = new System.Drawing.Point(649, 525);
            this.bPrint.Name = "bPrint";
            this.bPrint.Size = new System.Drawing.Size(123, 24);
            this.bPrint.TabIndex = 1;
            this.bPrint.Text = "Show Algorithm";
            this.bPrint.UseVisualStyleBackColor = true;
            this.bPrint.Click += new System.EventHandler(this.bPrint_Click);
            // 
            // rbEcho
            // 
            this.rbEcho.AutoSize = true;
            this.rbEcho.Location = new System.Drawing.Point(12, 529);
            this.rbEcho.Name = "rbEcho";
            this.rbEcho.Size = new System.Drawing.Size(50, 17);
            this.rbEcho.TabIndex = 2;
            this.rbEcho.TabStop = true;
            this.rbEcho.Text = "Echo";
            this.rbEcho.UseVisualStyleBackColor = true;
            this.rbEcho.CheckedChanged += new System.EventHandler(this.rbEcho_CheckedChanged);
            // 
            // rbFinn
            // 
            this.rbFinn.AutoSize = true;
            this.rbFinn.Location = new System.Drawing.Point(68, 529);
            this.rbFinn.Name = "rbFinn";
            this.rbFinn.Size = new System.Drawing.Size(45, 17);
            this.rbFinn.TabIndex = 3;
            this.rbFinn.TabStop = true;
            this.rbFinn.Text = "Finn";
            this.rbFinn.UseVisualStyleBackColor = true;
            this.rbFinn.CheckedChanged += new System.EventHandler(this.rbFinn_CheckedChanged);
            // 
            // rbPhase
            // 
            this.rbPhase.AutoSize = true;
            this.rbPhase.Location = new System.Drawing.Point(119, 529);
            this.rbPhase.Name = "rbPhase";
            this.rbPhase.Size = new System.Drawing.Size(55, 17);
            this.rbPhase.TabIndex = 4;
            this.rbPhase.TabStop = true;
            this.rbPhase.Text = "Phase";
            this.rbPhase.UseVisualStyleBackColor = true;
            this.rbPhase.CheckedChanged += new System.EventHandler(this.rbPhase_CheckedChanged);
            // 
            // rbLi
            // 
            this.rbLi.AutoSize = true;
            this.rbLi.Location = new System.Drawing.Point(180, 529);
            this.rbLi.Name = "rbLi";
            this.rbLi.Size = new System.Drawing.Size(33, 17);
            this.rbLi.TabIndex = 5;
            this.rbLi.TabStop = true;
            this.rbLi.Text = "Li";
            this.rbLi.UseVisualStyleBackColor = true;
            this.rbLi.CheckedChanged += new System.EventHandler(this.rbLi_CheckedChanged);
            // 
            // fMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.rbLi);
            this.Controls.Add(this.rbPhase);
            this.Controls.Add(this.rbFinn);
            this.Controls.Add(this.rbEcho);
            this.Controls.Add(this.bPrint);
            this.Controls.Add(this.DrawPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "fMain";
            this.Text = "Parallel Compute System Software";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel DrawPanel;
        private System.Windows.Forms.Button bPrint;
        private System.Windows.Forms.RadioButton rbEcho;
        private System.Windows.Forms.RadioButton rbFinn;
        private System.Windows.Forms.RadioButton rbPhase;
        private System.Windows.Forms.RadioButton rbLi;
    }
}

