namespace LiveResults.Client
{
    partial class PrintTotalResults
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
            this.separateClasses = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.nrStages = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.totalname = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // separateClasses
            // 
            this.separateClasses.AutoSize = true;
            this.separateClasses.Location = new System.Drawing.Point(15, 97);
            this.separateClasses.Name = "separateClasses";
            this.separateClasses.Size = new System.Drawing.Size(149, 17);
            this.separateClasses.TabIndex = 0;
            this.separateClasses.Text = "Each class in separate file";
            this.separateClasses.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 120);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(113, 75);
            this.button1.TabIndex = 1;
            this.button1.Text = "Export to html";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // nrStages
            // 
            this.nrStages.Location = new System.Drawing.Point(15, 71);
            this.nrStages.Name = "nrStages";
            this.nrStages.Size = new System.Drawing.Size(175, 20);
            this.nrStages.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 55);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(148, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Skriv ut totalresultat för etapp:";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // totalname
            // 
            this.totalname.Location = new System.Drawing.Point(15, 25);
            this.totalname.Name = "totalname";
            this.totalname.Size = new System.Drawing.Size(175, 20);
            this.totalname.TabIndex = 10;
            this.totalname.TextChanged += new System.EventHandler(this.totalname_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Namn:";
            // 
            // PrintTotalResults
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.totalname);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.nrStages);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.separateClasses);
            this.Name = "PrintTotalResults";
            this.Text = "Print / export total results";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox separateClasses;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox nrStages;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox totalname;
        private System.Windows.Forms.Label label2;
    }
}