namespace SCS.Web.UI.WebControls.Design
{
	partial class TabImageBuilderForm
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
			this.CreateCommand = new System.Windows.Forms.Button();
			this.TabLeftImage = new System.Windows.Forms.PictureBox();
			this.TabRightImage = new System.Windows.Forms.PictureBox();
			this.SaveCommand = new System.Windows.Forms.Button();
			this.TabCornerPropertyGroup = new System.Windows.Forms.GroupBox();
			this.TabTypeGroup = new System.Windows.Forms.Panel();
			this.SymetricalOption = new System.Windows.Forms.RadioButton();
			this.LeanRightOption = new System.Windows.Forms.RadioButton();
			this.LeanLeftOption = new System.Windows.Forms.RadioButton();
			this.TabCornerTypeGroup = new System.Windows.Forms.Panel();
			this.ChamferedOption = new System.Windows.Forms.RadioButton();
			this.RoundedOption = new System.Windows.Forms.RadioButton();
			this.CornerWidthEntry = new System.Windows.Forms.TextBox();
			this.CornerHeightLabel = new System.Windows.Forms.Label();
			this.CornerHeightEntry = new System.Windows.Forms.TextBox();
			this.CornerWidthLabel = new System.Windows.Forms.Label();
			this.BorderColorLabel = new System.Windows.Forms.Label();
			this.BorderColorEntry = new System.Windows.Forms.TextBox();
			this.BorderWidthLabel = new System.Windows.Forms.Label();
			this.TabHeightLabel = new System.Windows.Forms.Label();
			this.TabHeightEntry = new System.Windows.Forms.TextBox();
			this.BorderWidthEntry = new System.Windows.Forms.TextBox();
			this.TabWidthEntry = new System.Windows.Forms.TextBox();
			this.TabColorEntry = new System.Windows.Forms.TextBox();
			this.TabColorLabel = new System.Windows.Forms.Label();
			this.TabColorSelectCommand = new System.Windows.Forms.Button();
			this.BorderColorSelectCommand = new System.Windows.Forms.Button();
			this.TabWidthLabel = new System.Windows.Forms.Label();
			this.TabPropertyGroup = new System.Windows.Forms.GroupBox();
			this.SelectedColorSelectCommand = new System.Windows.Forms.Button();
			this.SelectedColorEntry = new System.Windows.Forms.TextBox();
			this.SelectedColorLabel = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.PreviewSelectedSelect = new System.Windows.Forms.RadioButton();
			this.PreviewNormalSelect = new System.Windows.Forms.RadioButton();
			this.label1 = new System.Windows.Forms.Label();
			this.SaveDirectoryLabel = new System.Windows.Forms.Label();
			this.OutputPathEntry = new System.Windows.Forms.TextBox();
			this.PathSelectCommand = new System.Windows.Forms.Button();
			this.FolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
			this.CloseCommand = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.TabLeftImage)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.TabRightImage)).BeginInit();
			this.TabCornerPropertyGroup.SuspendLayout();
			this.TabTypeGroup.SuspendLayout();
			this.TabCornerTypeGroup.SuspendLayout();
			this.TabPropertyGroup.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// CreateCommand
			// 
			this.CreateCommand.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.CreateCommand.Location = new System.Drawing.Point(184, 449);
			this.CreateCommand.Name = "CreateCommand";
			this.CreateCommand.Size = new System.Drawing.Size(90, 23);
			this.CreateCommand.TabIndex = 6;
			this.CreateCommand.Text = "&Refresh";
			this.CreateCommand.UseVisualStyleBackColor = true;
			this.CreateCommand.Click += new System.EventHandler(this.CreateCommand_Click);
			// 
			// TabLeftImage
			// 
			this.TabLeftImage.Location = new System.Drawing.Point(24, 44);
			this.TabLeftImage.Name = "TabLeftImage";
			this.TabLeftImage.Size = new System.Drawing.Size(76, 97);
			this.TabLeftImage.TabIndex = 9;
			this.TabLeftImage.TabStop = false;
			// 
			// TabRightImage
			// 
			this.TabRightImage.Location = new System.Drawing.Point(108, 44);
			this.TabRightImage.Name = "TabRightImage";
			this.TabRightImage.Size = new System.Drawing.Size(196, 97);
			this.TabRightImage.TabIndex = 10;
			this.TabRightImage.TabStop = false;
			// 
			// SaveCommand
			// 
			this.SaveCommand.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.SaveCommand.Enabled = false;
			this.SaveCommand.Location = new System.Drawing.Point(280, 449);
			this.SaveCommand.Name = "SaveCommand";
			this.SaveCommand.Size = new System.Drawing.Size(75, 23);
			this.SaveCommand.TabIndex = 7;
			this.SaveCommand.Text = "&Save Images";
			this.SaveCommand.UseVisualStyleBackColor = true;
			this.SaveCommand.Click += new System.EventHandler(this.SaveCommand_Click);
			// 
			// TabCornerPropertyGroup
			// 
			this.TabCornerPropertyGroup.Controls.Add(this.TabTypeGroup);
			this.TabCornerPropertyGroup.Controls.Add(this.TabCornerTypeGroup);
			this.TabCornerPropertyGroup.Controls.Add(this.CornerWidthEntry);
			this.TabCornerPropertyGroup.Controls.Add(this.CornerHeightLabel);
			this.TabCornerPropertyGroup.Controls.Add(this.CornerHeightEntry);
			this.TabCornerPropertyGroup.Controls.Add(this.CornerWidthLabel);
			this.TabCornerPropertyGroup.Location = new System.Drawing.Point(221, 13);
			this.TabCornerPropertyGroup.Name = "TabCornerPropertyGroup";
			this.TabCornerPropertyGroup.Size = new System.Drawing.Size(214, 188);
			this.TabCornerPropertyGroup.TabIndex = 1;
			this.TabCornerPropertyGroup.TabStop = false;
			this.TabCornerPropertyGroup.Text = "Tab Corner Properties";
			// 
			// TabTypeGroup
			// 
			this.TabTypeGroup.Controls.Add(this.SymetricalOption);
			this.TabTypeGroup.Controls.Add(this.LeanRightOption);
			this.TabTypeGroup.Controls.Add(this.LeanLeftOption);
			this.TabTypeGroup.Location = new System.Drawing.Point(9, 27);
			this.TabTypeGroup.Name = "TabTypeGroup";
			this.TabTypeGroup.Size = new System.Drawing.Size(89, 68);
			this.TabTypeGroup.TabIndex = 0;
			// 
			// SymetricalOption
			// 
			this.SymetricalOption.AutoSize = true;
			this.SymetricalOption.Checked = true;
			this.SymetricalOption.Location = new System.Drawing.Point(3, 7);
			this.SymetricalOption.Name = "SymetricalOption";
			this.SymetricalOption.Size = new System.Drawing.Size(73, 17);
			this.SymetricalOption.TabIndex = 0;
			this.SymetricalOption.TabStop = true;
			this.SymetricalOption.Tag = "1";
			this.SymetricalOption.Text = "Symetrical";
			this.SymetricalOption.UseVisualStyleBackColor = true;
			this.SymetricalOption.CheckedChanged += new System.EventHandler(this.TabType_CheckedChanged);
			// 
			// LeanRightOption
			// 
			this.LeanRightOption.AutoSize = true;
			this.LeanRightOption.Location = new System.Drawing.Point(3, 27);
			this.LeanRightOption.Name = "LeanRightOption";
			this.LeanRightOption.Size = new System.Drawing.Size(77, 17);
			this.LeanRightOption.TabIndex = 1;
			this.LeanRightOption.Tag = "2";
			this.LeanRightOption.Text = "Lean Right";
			this.LeanRightOption.UseVisualStyleBackColor = true;
			this.LeanRightOption.CheckedChanged += new System.EventHandler(this.TabType_CheckedChanged);
			// 
			// LeanLeftOption
			// 
			this.LeanLeftOption.AutoSize = true;
			this.LeanLeftOption.Location = new System.Drawing.Point(3, 46);
			this.LeanLeftOption.Name = "LeanLeftOption";
			this.LeanLeftOption.Size = new System.Drawing.Size(70, 17);
			this.LeanLeftOption.TabIndex = 2;
			this.LeanLeftOption.Tag = "3";
			this.LeanLeftOption.Text = "Lean Left";
			this.LeanLeftOption.UseVisualStyleBackColor = true;
			this.LeanLeftOption.CheckedChanged += new System.EventHandler(this.TabType_CheckedChanged);
			// 
			// TabCornerTypeGroup
			// 
			this.TabCornerTypeGroup.Controls.Add(this.ChamferedOption);
			this.TabCornerTypeGroup.Controls.Add(this.RoundedOption);
			this.TabCornerTypeGroup.Location = new System.Drawing.Point(119, 27);
			this.TabCornerTypeGroup.Name = "TabCornerTypeGroup";
			this.TabCornerTypeGroup.Size = new System.Drawing.Size(89, 57);
			this.TabCornerTypeGroup.TabIndex = 1;
			// 
			// ChamferedOption
			// 
			this.ChamferedOption.AutoSize = true;
			this.ChamferedOption.Location = new System.Drawing.Point(3, 26);
			this.ChamferedOption.Name = "ChamferedOption";
			this.ChamferedOption.Size = new System.Drawing.Size(76, 17);
			this.ChamferedOption.TabIndex = 1;
			this.ChamferedOption.Tag = "2";
			this.ChamferedOption.Text = "Chamfered";
			this.ChamferedOption.UseVisualStyleBackColor = true;
			this.ChamferedOption.CheckedChanged += new System.EventHandler(this.TabCornerType_CheckedChanged);
			// 
			// RoundedOption
			// 
			this.RoundedOption.AutoSize = true;
			this.RoundedOption.Checked = true;
			this.RoundedOption.Location = new System.Drawing.Point(3, 6);
			this.RoundedOption.Name = "RoundedOption";
			this.RoundedOption.Size = new System.Drawing.Size(69, 17);
			this.RoundedOption.TabIndex = 0;
			this.RoundedOption.TabStop = true;
			this.RoundedOption.Tag = "1";
			this.RoundedOption.Text = "Rounded";
			this.RoundedOption.UseVisualStyleBackColor = true;
			this.RoundedOption.CheckedChanged += new System.EventHandler(this.TabCornerType_CheckedChanged);
			// 
			// CornerWidthEntry
			// 
			this.CornerWidthEntry.Location = new System.Drawing.Point(102, 101);
			this.CornerWidthEntry.Name = "CornerWidthEntry";
			this.CornerWidthEntry.Size = new System.Drawing.Size(62, 20);
			this.CornerWidthEntry.TabIndex = 3;
			this.CornerWidthEntry.Text = "7";
			this.CornerWidthEntry.Leave += new System.EventHandler(this.CornerWidthEntry_Leave);
			// 
			// CornerHeightLabel
			// 
			this.CornerHeightLabel.AutoSize = true;
			this.CornerHeightLabel.Location = new System.Drawing.Point(23, 128);
			this.CornerHeightLabel.Name = "CornerHeightLabel";
			this.CornerHeightLabel.Size = new System.Drawing.Size(75, 13);
			this.CornerHeightLabel.TabIndex = 4;
			this.CornerHeightLabel.Text = "Corner Height:";
			// 
			// CornerHeightEntry
			// 
			this.CornerHeightEntry.Location = new System.Drawing.Point(102, 121);
			this.CornerHeightEntry.Name = "CornerHeightEntry";
			this.CornerHeightEntry.Size = new System.Drawing.Size(62, 20);
			this.CornerHeightEntry.TabIndex = 5;
			this.CornerHeightEntry.Text = "7";
			this.CornerHeightEntry.Leave += new System.EventHandler(this.CornerHeightEntry_Leave);
			// 
			// CornerWidthLabel
			// 
			this.CornerWidthLabel.AutoSize = true;
			this.CornerWidthLabel.Location = new System.Drawing.Point(23, 108);
			this.CornerWidthLabel.Name = "CornerWidthLabel";
			this.CornerWidthLabel.Size = new System.Drawing.Size(72, 13);
			this.CornerWidthLabel.TabIndex = 2;
			this.CornerWidthLabel.Text = "Corner Width:";
			// 
			// BorderColorLabel
			// 
			this.BorderColorLabel.AutoSize = true;
			this.BorderColorLabel.Location = new System.Drawing.Point(16, 154);
			this.BorderColorLabel.Name = "BorderColorLabel";
			this.BorderColorLabel.Size = new System.Drawing.Size(68, 13);
			this.BorderColorLabel.TabIndex = 9;
			this.BorderColorLabel.Text = "Border Color:";
			// 
			// BorderColorEntry
			// 
			this.BorderColorEntry.Location = new System.Drawing.Point(90, 147);
			this.BorderColorEntry.Name = "BorderColorEntry";
			this.BorderColorEntry.Size = new System.Drawing.Size(62, 20);
			this.BorderColorEntry.TabIndex = 10;
			this.BorderColorEntry.Text = "Black";
			this.BorderColorEntry.Leave += new System.EventHandler(this.BorderColorEntry_Leave);
			// 
			// BorderWidthLabel
			// 
			this.BorderWidthLabel.AutoSize = true;
			this.BorderWidthLabel.Location = new System.Drawing.Point(12, 75);
			this.BorderWidthLabel.Name = "BorderWidthLabel";
			this.BorderWidthLabel.Size = new System.Drawing.Size(72, 13);
			this.BorderWidthLabel.TabIndex = 4;
			this.BorderWidthLabel.Text = "Border Width:";
			// 
			// TabHeightLabel
			// 
			this.TabHeightLabel.AutoSize = true;
			this.TabHeightLabel.Location = new System.Drawing.Point(21, 54);
			this.TabHeightLabel.Name = "TabHeightLabel";
			this.TabHeightLabel.Size = new System.Drawing.Size(63, 13);
			this.TabHeightLabel.TabIndex = 2;
			this.TabHeightLabel.Text = "Tab Height:";
			// 
			// TabHeightEntry
			// 
			this.TabHeightEntry.Location = new System.Drawing.Point(90, 47);
			this.TabHeightEntry.Name = "TabHeightEntry";
			this.TabHeightEntry.Size = new System.Drawing.Size(62, 20);
			this.TabHeightEntry.TabIndex = 3;
			this.TabHeightEntry.Text = "100";
			this.TabHeightEntry.Leave += new System.EventHandler(this.TabHeightEntry_Leave);
			// 
			// BorderWidthEntry
			// 
			this.BorderWidthEntry.Location = new System.Drawing.Point(90, 68);
			this.BorderWidthEntry.Name = "BorderWidthEntry";
			this.BorderWidthEntry.Size = new System.Drawing.Size(62, 20);
			this.BorderWidthEntry.TabIndex = 5;
			this.BorderWidthEntry.Text = "1";
			this.BorderWidthEntry.Leave += new System.EventHandler(this.BorderWidthEntry_Leave);
			// 
			// TabWidthEntry
			// 
			this.TabWidthEntry.Location = new System.Drawing.Point(90, 27);
			this.TabWidthEntry.Name = "TabWidthEntry";
			this.TabWidthEntry.Size = new System.Drawing.Size(62, 20);
			this.TabWidthEntry.TabIndex = 1;
			this.TabWidthEntry.Text = "300";
			this.TabWidthEntry.Leave += new System.EventHandler(this.TabWidthEntry_Leave);
			// 
			// TabColorEntry
			// 
			this.TabColorEntry.Location = new System.Drawing.Point(90, 102);
			this.TabColorEntry.Name = "TabColorEntry";
			this.TabColorEntry.Size = new System.Drawing.Size(62, 20);
			this.TabColorEntry.TabIndex = 7;
			this.TabColorEntry.Text = "#EBEBEB";
			this.TabColorEntry.Leave += new System.EventHandler(this.TabColorEntry_Leave);
			// 
			// TabColorLabel
			// 
			this.TabColorLabel.AutoSize = true;
			this.TabColorLabel.Location = new System.Drawing.Point(28, 109);
			this.TabColorLabel.Name = "TabColorLabel";
			this.TabColorLabel.Size = new System.Drawing.Size(56, 13);
			this.TabColorLabel.TabIndex = 6;
			this.TabColorLabel.Text = "Tab Color:";
			// 
			// TabColorSelectCommand
			// 
			this.TabColorSelectCommand.Location = new System.Drawing.Point(158, 102);
			this.TabColorSelectCommand.Name = "TabColorSelectCommand";
			this.TabColorSelectCommand.Size = new System.Drawing.Size(28, 20);
			this.TabColorSelectCommand.TabIndex = 8;
			this.TabColorSelectCommand.Text = "...";
			this.TabColorSelectCommand.UseVisualStyleBackColor = true;
			this.TabColorSelectCommand.Click += new System.EventHandler(this.TabColorSelectCommand_Click);
			// 
			// BorderColorSelectCommand
			// 
			this.BorderColorSelectCommand.Location = new System.Drawing.Point(158, 146);
			this.BorderColorSelectCommand.Name = "BorderColorSelectCommand";
			this.BorderColorSelectCommand.Size = new System.Drawing.Size(28, 20);
			this.BorderColorSelectCommand.TabIndex = 11;
			this.BorderColorSelectCommand.Text = "...";
			this.BorderColorSelectCommand.UseVisualStyleBackColor = true;
			this.BorderColorSelectCommand.Click += new System.EventHandler(this.BorderColorSelectCommand_Click);
			// 
			// TabWidthLabel
			// 
			this.TabWidthLabel.AutoSize = true;
			this.TabWidthLabel.Location = new System.Drawing.Point(24, 34);
			this.TabWidthLabel.Name = "TabWidthLabel";
			this.TabWidthLabel.Size = new System.Drawing.Size(60, 13);
			this.TabWidthLabel.TabIndex = 0;
			this.TabWidthLabel.Text = "Tab Width:";
			// 
			// TabPropertyGroup
			// 
			this.TabPropertyGroup.Controls.Add(this.SelectedColorSelectCommand);
			this.TabPropertyGroup.Controls.Add(this.TabWidthLabel);
			this.TabPropertyGroup.Controls.Add(this.BorderColorSelectCommand);
			this.TabPropertyGroup.Controls.Add(this.SelectedColorEntry);
			this.TabPropertyGroup.Controls.Add(this.SelectedColorLabel);
			this.TabPropertyGroup.Controls.Add(this.TabColorSelectCommand);
			this.TabPropertyGroup.Controls.Add(this.TabColorLabel);
			this.TabPropertyGroup.Controls.Add(this.TabColorEntry);
			this.TabPropertyGroup.Controls.Add(this.TabWidthEntry);
			this.TabPropertyGroup.Controls.Add(this.BorderWidthEntry);
			this.TabPropertyGroup.Controls.Add(this.TabHeightEntry);
			this.TabPropertyGroup.Controls.Add(this.TabHeightLabel);
			this.TabPropertyGroup.Controls.Add(this.BorderWidthLabel);
			this.TabPropertyGroup.Controls.Add(this.BorderColorEntry);
			this.TabPropertyGroup.Controls.Add(this.BorderColorLabel);
			this.TabPropertyGroup.Location = new System.Drawing.Point(13, 13);
			this.TabPropertyGroup.Name = "TabPropertyGroup";
			this.TabPropertyGroup.Size = new System.Drawing.Size(202, 188);
			this.TabPropertyGroup.TabIndex = 0;
			this.TabPropertyGroup.TabStop = false;
			this.TabPropertyGroup.Text = "Tab Properties";
			// 
			// SelectedColorSelectCommand
			// 
			this.SelectedColorSelectCommand.Location = new System.Drawing.Point(158, 125);
			this.SelectedColorSelectCommand.Name = "SelectedColorSelectCommand";
			this.SelectedColorSelectCommand.Size = new System.Drawing.Size(28, 19);
			this.SelectedColorSelectCommand.TabIndex = 14;
			this.SelectedColorSelectCommand.Text = "...";
			this.SelectedColorSelectCommand.UseVisualStyleBackColor = true;
			this.SelectedColorSelectCommand.Click += new System.EventHandler(this.SelectedColorSelectCommand_Click);
			// 
			// SelectedColorEntry
			// 
			this.SelectedColorEntry.Location = new System.Drawing.Point(90, 124);
			this.SelectedColorEntry.Name = "SelectedColorEntry";
			this.SelectedColorEntry.Size = new System.Drawing.Size(62, 20);
			this.SelectedColorEntry.TabIndex = 13;
			this.SelectedColorEntry.Text = "White";
			this.SelectedColorEntry.Leave += new System.EventHandler(this.SelectedColorEntry_Leave);
			// 
			// SelectedColorLabel
			// 
			this.SelectedColorLabel.AutoSize = true;
			this.SelectedColorLabel.Location = new System.Drawing.Point(5, 131);
			this.SelectedColorLabel.Name = "SelectedColorLabel";
			this.SelectedColorLabel.Size = new System.Drawing.Size(79, 13);
			this.SelectedColorLabel.TabIndex = 12;
			this.SelectedColorLabel.Text = "Selected Color:";
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.panel1);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.TabRightImage);
			this.groupBox1.Controls.Add(this.TabLeftImage);
			this.groupBox1.Location = new System.Drawing.Point(13, 255);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(422, 188);
			this.groupBox1.TabIndex = 5;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Preview";
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.PreviewSelectedSelect);
			this.panel1.Controls.Add(this.PreviewNormalSelect);
			this.panel1.Location = new System.Drawing.Point(58, 11);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(357, 27);
			this.panel1.TabIndex = 11;
			// 
			// PreviewSelectedSelect
			// 
			this.PreviewSelectedSelect.AutoSize = true;
			this.PreviewSelectedSelect.Location = new System.Drawing.Point(290, 7);
			this.PreviewSelectedSelect.Name = "PreviewSelectedSelect";
			this.PreviewSelectedSelect.Size = new System.Drawing.Size(67, 17);
			this.PreviewSelectedSelect.TabIndex = 1;
			this.PreviewSelectedSelect.Text = "Selected";
			this.PreviewSelectedSelect.UseVisualStyleBackColor = true;
			this.PreviewSelectedSelect.CheckedChanged += new System.EventHandler(this.TabPreviewTypeSelect_CheckedChanged);
			// 
			// PreviewNormalSelect
			// 
			this.PreviewNormalSelect.AutoSize = true;
			this.PreviewNormalSelect.Checked = true;
			this.PreviewNormalSelect.Location = new System.Drawing.Point(226, 7);
			this.PreviewNormalSelect.Name = "PreviewNormalSelect";
			this.PreviewNormalSelect.Size = new System.Drawing.Size(58, 17);
			this.PreviewNormalSelect.TabIndex = 0;
			this.PreviewNormalSelect.TabStop = true;
			this.PreviewNormalSelect.Text = "Normal";
			this.PreviewNormalSelect.UseVisualStyleBackColor = true;
			this.PreviewNormalSelect.CheckedChanged += new System.EventHandler(this.TabPreviewTypeSelect_CheckedChanged);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 165);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(394, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Note: Tab should be large enough to account for the browser\'s largest text settin" +
				"g.";
			// 
			// SaveDirectoryLabel
			// 
			this.SaveDirectoryLabel.AutoSize = true;
			this.SaveDirectoryLabel.Location = new System.Drawing.Point(10, 204);
			this.SaveDirectoryLabel.Name = "SaveDirectoryLabel";
			this.SaveDirectoryLabel.Size = new System.Drawing.Size(87, 13);
			this.SaveDirectoryLabel.TabIndex = 2;
			this.SaveDirectoryLabel.Text = "Output Directory:";
			// 
			// OutputPathEntry
			// 
			this.OutputPathEntry.Location = new System.Drawing.Point(13, 220);
			this.OutputPathEntry.Name = "OutputPathEntry";
			this.OutputPathEntry.Size = new System.Drawing.Size(382, 20);
			this.OutputPathEntry.TabIndex = 3;
			this.OutputPathEntry.Leave += new System.EventHandler(this.OutputPathEntry_Leave);
			this.OutputPathEntry.TextChanged += new System.EventHandler(this.OutputPathEntry_TextChanged);
			// 
			// PathSelectCommand
			// 
			this.PathSelectCommand.Location = new System.Drawing.Point(401, 219);
			this.PathSelectCommand.Name = "PathSelectCommand";
			this.PathSelectCommand.Size = new System.Drawing.Size(28, 20);
			this.PathSelectCommand.TabIndex = 4;
			this.PathSelectCommand.Text = "...";
			this.PathSelectCommand.UseVisualStyleBackColor = true;
			this.PathSelectCommand.Click += new System.EventHandler(this.PathSelectCommand_Click);
			// 
			// CloseCommand
			// 
			this.CloseCommand.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.CloseCommand.Location = new System.Drawing.Point(360, 449);
			this.CloseCommand.Name = "CloseCommand";
			this.CloseCommand.Size = new System.Drawing.Size(75, 23);
			this.CloseCommand.TabIndex = 8;
			this.CloseCommand.Text = "&Close";
			this.CloseCommand.UseVisualStyleBackColor = true;
			this.CloseCommand.Click += new System.EventHandler(this.CloseCommand_Click);
			// 
			// TabImageBuilderForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(447, 484);
			this.Controls.Add(this.CloseCommand);
			this.Controls.Add(this.PathSelectCommand);
			this.Controls.Add(this.OutputPathEntry);
			this.Controls.Add(this.SaveDirectoryLabel);
			this.Controls.Add(this.SaveCommand);
			this.Controls.Add(this.CreateCommand);
			this.Controls.Add(this.TabPropertyGroup);
			this.Controls.Add(this.TabCornerPropertyGroup);
			this.Controls.Add(this.groupBox1);
			this.MinimumSize = new System.Drawing.Size(455, 511);
			this.Name = "TabImageBuilderForm";
			this.Text = "TabStrip Image Builder";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TabImageBuilderForm_FormClosing);
			this.Load += new System.EventHandler(this.TabImageBuilderForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.TabLeftImage)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.TabRightImage)).EndInit();
			this.TabCornerPropertyGroup.ResumeLayout(false);
			this.TabCornerPropertyGroup.PerformLayout();
			this.TabTypeGroup.ResumeLayout(false);
			this.TabTypeGroup.PerformLayout();
			this.TabCornerTypeGroup.ResumeLayout(false);
			this.TabCornerTypeGroup.PerformLayout();
			this.TabPropertyGroup.ResumeLayout(false);
			this.TabPropertyGroup.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button CreateCommand;
		private System.Windows.Forms.PictureBox TabLeftImage;
		private System.Windows.Forms.PictureBox TabRightImage;
		private System.Windows.Forms.Button SaveCommand;
		private System.Windows.Forms.GroupBox TabCornerPropertyGroup;
		private System.Windows.Forms.RadioButton SymetricalOption;
		private System.Windows.Forms.TextBox CornerWidthEntry;
		private System.Windows.Forms.RadioButton LeanLeftOption;
		private System.Windows.Forms.Label CornerHeightLabel;
		private System.Windows.Forms.RadioButton LeanRightOption;
		private System.Windows.Forms.TextBox CornerHeightEntry;
		private System.Windows.Forms.RadioButton ChamferedOption;
		private System.Windows.Forms.RadioButton RoundedOption;
		private System.Windows.Forms.Label CornerWidthLabel;
		private System.Windows.Forms.Label BorderColorLabel;
		private System.Windows.Forms.TextBox BorderColorEntry;
		private System.Windows.Forms.Label BorderWidthLabel;
		private System.Windows.Forms.Label TabHeightLabel;
		private System.Windows.Forms.TextBox TabHeightEntry;
		private System.Windows.Forms.TextBox BorderWidthEntry;
		private System.Windows.Forms.TextBox TabWidthEntry;
		private System.Windows.Forms.TextBox TabColorEntry;
		private System.Windows.Forms.Label TabColorLabel;
		private System.Windows.Forms.Button TabColorSelectCommand;
		private System.Windows.Forms.Button BorderColorSelectCommand;
		private System.Windows.Forms.Label TabWidthLabel;
		private System.Windows.Forms.GroupBox TabPropertyGroup;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Panel TabTypeGroup;
		private System.Windows.Forms.Panel TabCornerTypeGroup;
		private System.Windows.Forms.Label SaveDirectoryLabel;
		private System.Windows.Forms.TextBox OutputPathEntry;
		private System.Windows.Forms.Button PathSelectCommand;
		private System.Windows.Forms.FolderBrowserDialog FolderBrowserDialog;
		private System.Windows.Forms.Button SelectedColorSelectCommand;
		private System.Windows.Forms.TextBox SelectedColorEntry;
		private System.Windows.Forms.Label SelectedColorLabel;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.RadioButton PreviewSelectedSelect;
		private System.Windows.Forms.RadioButton PreviewNormalSelect;
		private System.Windows.Forms.Button CloseCommand;

	}
}