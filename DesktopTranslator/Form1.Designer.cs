namespace DesktopTranslator
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            buttonTranslate = new Button();
            textBoxInput = new TextBox();
            label1 = new Label();
            comboBoxSourceLang = new ComboBox();
            textBoxOutput = new TextBox();
            comboBoxTargetLang = new ComboBox();
            SuspendLayout();
            // 
            // buttonTranslate
            // 
            buttonTranslate.Location = new Point(33, 370);
            buttonTranslate.Name = "buttonTranslate";
            buttonTranslate.Size = new Size(117, 45);
            buttonTranslate.TabIndex = 0;
            buttonTranslate.Text = "Перевести";
            buttonTranslate.UseVisualStyleBackColor = true;
            buttonTranslate.Click += buttonTranslate_Click;
            // 
            // textBoxInput
            // 
            textBoxInput.Location = new Point(33, 125);
            textBoxInput.Multiline = true;
            textBoxInput.Name = "textBoxInput";
            textBoxInput.Size = new Size(291, 219);
            textBoxInput.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(33, 27);
            label1.Name = "label1";
            label1.Size = new Size(414, 20);
            label1.TabIndex = 2;
            label1.Text = "Введите текст и выберите исходный язык и язык превода:";
            // 
            // comboBoxSourceLang
            // 
            comboBoxSourceLang.FormattingEnabled = true;
            comboBoxSourceLang.Location = new Point(33, 71);
            comboBoxSourceLang.Name = "comboBoxSourceLang";
            comboBoxSourceLang.Size = new Size(151, 28);
            comboBoxSourceLang.TabIndex = 3;
            // 
            // textBoxOutput
            // 
            textBoxOutput.Location = new Point(380, 125);
            textBoxOutput.Multiline = true;
            textBoxOutput.Name = "textBoxOutput";
            textBoxOutput.Size = new Size(291, 219);
            textBoxOutput.TabIndex = 4;
            // 
            // comboBoxTargetLang
            // 
            comboBoxTargetLang.FormattingEnabled = true;
            comboBoxTargetLang.Location = new Point(380, 71);
            comboBoxTargetLang.Name = "comboBoxTargetLang";
            comboBoxTargetLang.Size = new Size(151, 28);
            comboBoxTargetLang.TabIndex = 5;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(comboBoxTargetLang);
            Controls.Add(textBoxOutput);
            Controls.Add(comboBoxSourceLang);
            Controls.Add(label1);
            Controls.Add(textBoxInput);
            Controls.Add(buttonTranslate);
            Name = "Form1";
            Text = "Переводчик";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button buttonTranslate;
        private TextBox textBoxInput;
        private Label label1;
        private ComboBox comboBoxSourceLang;
        private TextBox textBoxOutput;
        private ComboBox comboBoxTargetLang;
    }
}
