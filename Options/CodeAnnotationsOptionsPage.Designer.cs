namespace CodeAnnotationPack.Options {
  partial class CodeAnnotationsOptionsPage
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
      if (disposing && (this.components != null))
      {
        this.components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.label3 = new System.Windows.Forms.Label();
      this.PublicAssertion = new System.Windows.Forms.TextBox();
      this.label4 = new System.Windows.Forms.Label();
      this.label7 = new System.Windows.Forms.Label();
      this.NonPublicAssertion = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.AlternativeAssertions = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label3.Location = new System.Drawing.Point(24, 33);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(135, 13);
      this.label3.TabIndex = 20;
      this.label3.Text = "Public member assertion:";
      // 
      // PublicAssertion
      // 
      this.PublicAssertion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.PublicAssertion.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.PublicAssertion.Location = new System.Drawing.Point(27, 49);
      this.PublicAssertion.Name = "PublicAssertion";
      this.PublicAssertion.Size = new System.Drawing.Size(344, 22);
      this.PublicAssertion.TabIndex = 21;
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label4.Location = new System.Drawing.Point(9, 11);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(61, 13);
      this.label4.TabIndex = 0;
      this.label4.Text = "Assertions";
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label7.Location = new System.Drawing.Point(24, 109);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(162, 13);
      this.label7.TabIndex = 25;
      this.label7.Text = "Non-public member assertion:";
      // 
      // NonPublicAssertion
      // 
      this.NonPublicAssertion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.NonPublicAssertion.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.NonPublicAssertion.Location = new System.Drawing.Point(27, 125);
      this.NonPublicAssertion.Name = "NonPublicAssertion";
      this.NonPublicAssertion.Size = new System.Drawing.Size(344, 22);
      this.NonPublicAssertion.TabIndex = 26;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.ForeColor = System.Drawing.SystemColors.GrayText;
      this.label1.Location = new System.Drawing.Point(24, 74);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(227, 13);
      this.label1.TabIndex = 25;
      this.label1.Text = "Example: Assert.ArgumentNotNull({0}, \"{0}\");";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.ForeColor = System.Drawing.SystemColors.GrayText;
      this.label2.Location = new System.Drawing.Point(24, 150);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(231, 13);
      this.label2.TabIndex = 27;
      this.label2.Text = "Example: Debug.ArgumentNotNull({0}, \"{0}\");";
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label5.Location = new System.Drawing.Point(24, 187);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(218, 13);
      this.label5.TabIndex = 28;
      this.label5.Text = "Alternative valid assertions (one per line):";
      // 
      // AlternativeAssertions
      // 
      this.AlternativeAssertions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.AlternativeAssertions.Location = new System.Drawing.Point(27, 204);
      this.AlternativeAssertions.Multiline = true;
      this.AlternativeAssertions.Name = "AlternativeAssertions";
      this.AlternativeAssertions.Size = new System.Drawing.Size(344, 174);
      this.AlternativeAssertions.TabIndex = 29;
      // 
      // CodeAnnotationsOptionsPage
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.AlternativeAssertions);
      this.Controls.Add(this.label5);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.NonPublicAssertion);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.label7);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.PublicAssertion);
      this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.Name = "CodeAnnotationsOptionsPage";
      this.Size = new System.Drawing.Size(385, 456);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox PublicAssertion;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.TextBox NonPublicAssertion;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.TextBox AlternativeAssertions;





  }
}