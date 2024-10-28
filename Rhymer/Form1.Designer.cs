namespace Rhymer
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
            buttonRhyme = new Button();
            textBoxIn = new TextBox();
            textBoxOut = new TextBox();
            SuspendLayout();
            // 
            // buttonRhyme
            // 
            buttonRhyme.Location = new Point(376, 175);
            buttonRhyme.Name = "buttonRhyme";
            buttonRhyme.Size = new Size(152, 48);
            buttonRhyme.TabIndex = 0;
            buttonRhyme.Text = "Перерифмовать>>";
            buttonRhyme.UseVisualStyleBackColor = true;
            buttonRhyme.Click += buttonRhyme_Click;
            // 
            // textBoxIn
            // 
            textBoxIn.Location = new Point(28, 38);
            textBoxIn.Multiline = true;
            textBoxIn.Name = "textBoxIn";
            textBoxIn.Size = new Size(329, 323);
            textBoxIn.TabIndex = 1;
            // 
            // textBoxOut
            // 
            textBoxOut.Location = new Point(553, 38);
            textBoxOut.Multiline = true;
            textBoxOut.Name = "textBoxOut";
            textBoxOut.Size = new Size(329, 323);
            textBoxOut.TabIndex = 2;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(923, 407);
            Controls.Add(textBoxOut);
            Controls.Add(textBoxIn);
            Controls.Add(buttonRhyme);
            Name = "Form1";
            Text = "Рифмоплет";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button buttonRhyme;
        private TextBox textBoxIn;
        private TextBox textBoxOut;
    }
}
