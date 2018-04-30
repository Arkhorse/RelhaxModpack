﻿namespace RelhaxModpack
{
    partial class DatabaseUpdater
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DatabaseUpdater));
            this.UpdateDatabaseStep1 = new System.Windows.Forms.Button();
            this.DatabaseLocationTextBox = new System.Windows.Forms.RichTextBox();
            this.UpdateDatabaseStep3 = new System.Windows.Forms.Button();
            this.addZipsDialog = new System.Windows.Forms.OpenFileDialog();
            this.loadDatabaseDialog = new System.Windows.Forms.OpenFileDialog();
            this.UpdateDatabaseStep2 = new System.Windows.Forms.Button();
            this.UpdateDatabaseStep3Advanced = new System.Windows.Forms.Button();
            this.UpdateDatabaseStep5 = new System.Windows.Forms.Button();
            this.UpdateDatabaseStep6 = new System.Windows.Forms.Button();
            this.DatabaseUpdateTabControl = new System.Windows.Forms.TabControl();
            this.AuthStatus = new System.Windows.Forms.TabPage();
            this.AuthorizationTable = new System.Windows.Forms.TableLayoutPanel();
            this.CurrentAuthStatusLabel = new System.Windows.Forms.Label();
            this.AuthStatusLabel = new System.Windows.Forms.Label();
            this.RequestL1AuthLabel = new System.Windows.Forms.Label();
            this.RequestL2AuthLabel = new System.Windows.Forms.Label();
            this.RequestL3AuthLabel = new System.Windows.Forms.Label();
            this.RequestL1AuthButton = new System.Windows.Forms.Button();
            this.RequestL2AuthButton = new System.Windows.Forms.Button();
            this.RequestL3AuthButton = new System.Windows.Forms.Button();
            this.L1AuthPasswordAttempt = new System.Windows.Forms.TextBox();
            this.L2PasswordAttempt = new System.Windows.Forms.TextBox();
            this.L3PasswordAttempt = new System.Windows.Forms.TextBox();
            this.UpdateDatabaseTab = new System.Windows.Forms.TabPage();
            this.UpdateDatabaseStep4 = new System.Windows.Forms.Button();
            this.UpdateDatabaseStep0 = new System.Windows.Forms.RichTextBox();
            this.UpdateDatebaseStep8 = new System.Windows.Forms.RichTextBox();
            this.UpdateApplicationTab = new System.Windows.Forms.TabPage();
            this.UpdateApplicationStep9 = new System.Windows.Forms.RichTextBox();
            this.UpdateApplicationStep7 = new System.Windows.Forms.Button();
            this.UpdateApplicatonSteps0to4 = new System.Windows.Forms.RichTextBox();
            this.UpdateApplicationStep8 = new System.Windows.Forms.Button();
            this.CleanOnlineFolders = new System.Windows.Forms.TabPage();
            this.CleanFoldersStep2 = new System.Windows.Forms.Button();
            this.CleanFoldersStep1 = new System.Windows.Forms.Button();
            this.CreatePasswordTab = new System.Windows.Forms.TabPage();
            this.PasswordL3Panel = new System.Windows.Forms.Panel();
            this.ReadbackL3Password = new System.Windows.Forms.Button();
            this.L3GenerateOutput = new System.Windows.Forms.RichTextBox();
            this.CreatePasswordL3Label = new System.Windows.Forms.Label();
            this.L3TextPassword = new System.Windows.Forms.TextBox();
            this.GenerateL3Password = new System.Windows.Forms.Button();
            this.PasswordL2Panel = new System.Windows.Forms.Panel();
            this.ReadbackL2Password = new System.Windows.Forms.Button();
            this.L2GenerateOutput = new System.Windows.Forms.RichTextBox();
            this.CreatePasswordL2Label = new System.Windows.Forms.Label();
            this.L2TextPassword = new System.Windows.Forms.TextBox();
            this.GenerateL2Password = new System.Windows.Forms.Button();
            this.ScriptLogOutput = new System.Windows.Forms.RichTextBox();
            this.ScriptOutputLabel = new System.Windows.Forms.Label();
            this.ButtonInfo = new System.Windows.Forms.ToolTip(this.components);
            this.DatabaseUpdateTabControl.SuspendLayout();
            this.AuthStatus.SuspendLayout();
            this.AuthorizationTable.SuspendLayout();
            this.UpdateDatabaseTab.SuspendLayout();
            this.UpdateApplicationTab.SuspendLayout();
            this.CleanOnlineFolders.SuspendLayout();
            this.CreatePasswordTab.SuspendLayout();
            this.PasswordL3Panel.SuspendLayout();
            this.PasswordL2Panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // UpdateDatabaseStep1
            // 
            this.UpdateDatabaseStep1.Location = new System.Drawing.Point(6, 33);
            this.UpdateDatabaseStep1.Name = "UpdateDatabaseStep1";
            this.UpdateDatabaseStep1.Size = new System.Drawing.Size(430, 23);
            this.UpdateDatabaseStep1.TabIndex = 0;
            this.UpdateDatabaseStep1.Text = "Step 1: load modInfo.xml";
            this.UpdateDatabaseStep1.UseVisualStyleBackColor = true;
            this.UpdateDatabaseStep1.Click += new System.EventHandler(this.UpdateDatabaseStep1_Click);
            // 
            // DatabaseLocationTextBox
            // 
            this.DatabaseLocationTextBox.Location = new System.Drawing.Point(6, 62);
            this.DatabaseLocationTextBox.Name = "DatabaseLocationTextBox";
            this.DatabaseLocationTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.DatabaseLocationTextBox.Size = new System.Drawing.Size(430, 34);
            this.DatabaseLocationTextBox.TabIndex = 1;
            this.DatabaseLocationTextBox.Text = "";
            // 
            // UpdateDatabaseStep3
            // 
            this.UpdateDatabaseStep3.Location = new System.Drawing.Point(6, 131);
            this.UpdateDatabaseStep3.Name = "UpdateDatabaseStep3";
            this.UpdateDatabaseStep3.Size = new System.Drawing.Size(430, 23);
            this.UpdateDatabaseStep3.TabIndex = 3;
            this.UpdateDatabaseStep3.Text = "Step 3: Prepare database update (via online method)";
            this.UpdateDatabaseStep3.UseVisualStyleBackColor = true;
            this.UpdateDatabaseStep3.Click += new System.EventHandler(this.UpdateDatabaseStep3_Click);
            // 
            // addZipsDialog
            // 
            this.addZipsDialog.DefaultExt = "xml";
            this.addZipsDialog.FileName = "file.zip";
            this.addZipsDialog.Filter = "*.zip|*.zip";
            this.addZipsDialog.Multiselect = true;
            this.addZipsDialog.RestoreDirectory = true;
            this.addZipsDialog.Title = "select zip files to update";
            // 
            // loadDatabaseDialog
            // 
            this.loadDatabaseDialog.DefaultExt = "xml";
            this.loadDatabaseDialog.FileName = "*.xml";
            this.loadDatabaseDialog.Filter = "*.xml|*.xml";
            this.loadDatabaseDialog.RestoreDirectory = true;
            this.loadDatabaseDialog.Title = "load database";
            // 
            // UpdateDatabaseStep2
            // 
            this.UpdateDatabaseStep2.Location = new System.Drawing.Point(6, 102);
            this.UpdateDatabaseStep2.Name = "UpdateDatabaseStep2";
            this.UpdateDatabaseStep2.Size = new System.Drawing.Size(430, 23);
            this.UpdateDatabaseStep2.TabIndex = 5;
            this.UpdateDatabaseStep2.Text = "Step 2: Run script CreateDatabase.php";
            this.UpdateDatabaseStep2.UseVisualStyleBackColor = true;
            this.UpdateDatabaseStep2.Click += new System.EventHandler(this.UpdateDatabaseStep2_Click);
            // 
            // UpdateDatabaseStep3Advanced
            // 
            this.UpdateDatabaseStep3Advanced.Enabled = false;
            this.UpdateDatabaseStep3Advanced.Location = new System.Drawing.Point(6, 160);
            this.UpdateDatabaseStep3Advanced.Name = "UpdateDatabaseStep3Advanced";
            this.UpdateDatabaseStep3Advanced.Size = new System.Drawing.Size(430, 22);
            this.UpdateDatabaseStep3Advanced.TabIndex = 7;
            this.UpdateDatabaseStep3Advanced.Text = "(Advanced) (local method)";
            this.UpdateDatabaseStep3Advanced.UseVisualStyleBackColor = true;
            this.UpdateDatabaseStep3Advanced.Click += new System.EventHandler(this.UpdateDatabaseStep3Advanced_Click);
            // 
            // UpdateDatabaseStep5
            // 
            this.UpdateDatabaseStep5.Location = new System.Drawing.Point(6, 217);
            this.UpdateDatabaseStep5.Name = "UpdateDatabaseStep5";
            this.UpdateDatabaseStep5.Size = new System.Drawing.Size(430, 23);
            this.UpdateDatabaseStep5.TabIndex = 8;
            this.UpdateDatabaseStep5.Text = "Step 5: Run script CreateModInfo.php";
            this.UpdateDatabaseStep5.UseVisualStyleBackColor = true;
            this.UpdateDatabaseStep5.Click += new System.EventHandler(this.UpdateDatabaseStep5_Click);
            // 
            // UpdateDatabaseStep6
            // 
            this.UpdateDatabaseStep6.Location = new System.Drawing.Point(6, 246);
            this.UpdateDatabaseStep6.Name = "UpdateDatabaseStep6";
            this.UpdateDatabaseStep6.Size = new System.Drawing.Size(430, 23);
            this.UpdateDatabaseStep6.TabIndex = 9;
            this.UpdateDatabaseStep6.Text = "Step 6: Run script CreateManagerInfo.php";
            this.UpdateDatabaseStep6.UseVisualStyleBackColor = true;
            this.UpdateDatabaseStep6.Click += new System.EventHandler(this.UpdateDatabaseStep6_Click);
            // 
            // DatabaseUpdateTabControl
            // 
            this.DatabaseUpdateTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DatabaseUpdateTabControl.Controls.Add(this.AuthStatus);
            this.DatabaseUpdateTabControl.Controls.Add(this.UpdateDatabaseTab);
            this.DatabaseUpdateTabControl.Controls.Add(this.UpdateApplicationTab);
            this.DatabaseUpdateTabControl.Controls.Add(this.CleanOnlineFolders);
            this.DatabaseUpdateTabControl.Controls.Add(this.CreatePasswordTab);
            this.DatabaseUpdateTabControl.Location = new System.Drawing.Point(12, 34);
            this.DatabaseUpdateTabControl.Name = "DatabaseUpdateTabControl";
            this.DatabaseUpdateTabControl.SelectedIndex = 0;
            this.DatabaseUpdateTabControl.Size = new System.Drawing.Size(450, 395);
            this.DatabaseUpdateTabControl.TabIndex = 12;
            this.DatabaseUpdateTabControl.Selected += new System.Windows.Forms.TabControlEventHandler(this.DatabaseUpdateTabControl_Selected);
            // 
            // AuthStatus
            // 
            this.AuthStatus.Controls.Add(this.AuthorizationTable);
            this.AuthStatus.Location = new System.Drawing.Point(4, 22);
            this.AuthStatus.Name = "AuthStatus";
            this.AuthStatus.Padding = new System.Windows.Forms.Padding(3);
            this.AuthStatus.Size = new System.Drawing.Size(442, 369);
            this.AuthStatus.TabIndex = 4;
            this.AuthStatus.Text = "Auth Status";
            this.AuthStatus.UseVisualStyleBackColor = true;
            // 
            // AuthorizationTable
            // 
            this.AuthorizationTable.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.AuthorizationTable.ColumnCount = 3;
            this.AuthorizationTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 46.35569F));
            this.AuthorizationTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 53.64431F));
            this.AuthorizationTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 138F));
            this.AuthorizationTable.Controls.Add(this.CurrentAuthStatusLabel, 0, 0);
            this.AuthorizationTable.Controls.Add(this.AuthStatusLabel, 2, 0);
            this.AuthorizationTable.Controls.Add(this.RequestL1AuthLabel, 0, 1);
            this.AuthorizationTable.Controls.Add(this.RequestL2AuthLabel, 0, 2);
            this.AuthorizationTable.Controls.Add(this.RequestL3AuthLabel, 0, 3);
            this.AuthorizationTable.Controls.Add(this.RequestL1AuthButton, 2, 1);
            this.AuthorizationTable.Controls.Add(this.RequestL2AuthButton, 2, 2);
            this.AuthorizationTable.Controls.Add(this.RequestL3AuthButton, 2, 3);
            this.AuthorizationTable.Controls.Add(this.L1AuthPasswordAttempt, 1, 1);
            this.AuthorizationTable.Controls.Add(this.L2PasswordAttempt, 1, 2);
            this.AuthorizationTable.Controls.Add(this.L3PasswordAttempt, 1, 3);
            this.AuthorizationTable.Location = new System.Drawing.Point(6, 6);
            this.AuthorizationTable.Name = "AuthorizationTable";
            this.AuthorizationTable.RowCount = 4;
            this.AuthorizationTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.AuthorizationTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.AuthorizationTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.AuthorizationTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.AuthorizationTable.Size = new System.Drawing.Size(430, 130);
            this.AuthorizationTable.TabIndex = 1;
            // 
            // CurrentAuthStatusLabel
            // 
            this.CurrentAuthStatusLabel.AutoSize = true;
            this.CurrentAuthStatusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CurrentAuthStatusLabel.Location = new System.Drawing.Point(4, 1);
            this.CurrentAuthStatusLabel.Name = "CurrentAuthStatusLabel";
            this.CurrentAuthStatusLabel.Size = new System.Drawing.Size(127, 31);
            this.CurrentAuthStatusLabel.TabIndex = 0;
            this.CurrentAuthStatusLabel.Text = "Current Authorization status:";
            // 
            // AuthStatusLabel
            // 
            this.AuthStatusLabel.AutoSize = true;
            this.AuthStatusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AuthStatusLabel.Location = new System.Drawing.Point(293, 1);
            this.AuthStatusLabel.Name = "AuthStatusLabel";
            this.AuthStatusLabel.Size = new System.Drawing.Size(133, 31);
            this.AuthStatusLabel.TabIndex = 1;
            this.AuthStatusLabel.Text = "0";
            // 
            // RequestL1AuthLabel
            // 
            this.RequestL1AuthLabel.AutoSize = true;
            this.RequestL1AuthLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RequestL1AuthLabel.Location = new System.Drawing.Point(4, 33);
            this.RequestL1AuthLabel.Name = "RequestL1AuthLabel";
            this.RequestL1AuthLabel.Size = new System.Drawing.Size(127, 31);
            this.RequestL1AuthLabel.TabIndex = 2;
            this.RequestL1AuthLabel.Text = "Request Level 1 Authorization";
            // 
            // RequestL2AuthLabel
            // 
            this.RequestL2AuthLabel.AutoSize = true;
            this.RequestL2AuthLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RequestL2AuthLabel.Location = new System.Drawing.Point(4, 65);
            this.RequestL2AuthLabel.Name = "RequestL2AuthLabel";
            this.RequestL2AuthLabel.Size = new System.Drawing.Size(127, 31);
            this.RequestL2AuthLabel.TabIndex = 3;
            this.RequestL2AuthLabel.Text = "Request Level 2 Authorization";
            // 
            // RequestL3AuthLabel
            // 
            this.RequestL3AuthLabel.AutoSize = true;
            this.RequestL3AuthLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RequestL3AuthLabel.Location = new System.Drawing.Point(4, 97);
            this.RequestL3AuthLabel.Name = "RequestL3AuthLabel";
            this.RequestL3AuthLabel.Size = new System.Drawing.Size(127, 32);
            this.RequestL3AuthLabel.TabIndex = 4;
            this.RequestL3AuthLabel.Text = "Request Level 3 Authorization";
            // 
            // RequestL1AuthButton
            // 
            this.RequestL1AuthButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RequestL1AuthButton.Location = new System.Drawing.Point(293, 36);
            this.RequestL1AuthButton.Name = "RequestL1AuthButton";
            this.RequestL1AuthButton.Size = new System.Drawing.Size(133, 25);
            this.RequestL1AuthButton.TabIndex = 5;
            this.RequestL1AuthButton.Text = "Request";
            this.RequestL1AuthButton.UseVisualStyleBackColor = true;
            this.RequestL1AuthButton.Click += new System.EventHandler(this.RequestL1AuthButton_Click);
            // 
            // RequestL2AuthButton
            // 
            this.RequestL2AuthButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RequestL2AuthButton.Location = new System.Drawing.Point(293, 68);
            this.RequestL2AuthButton.Name = "RequestL2AuthButton";
            this.RequestL2AuthButton.Size = new System.Drawing.Size(133, 25);
            this.RequestL2AuthButton.TabIndex = 6;
            this.RequestL2AuthButton.Text = "Request";
            this.RequestL2AuthButton.UseVisualStyleBackColor = true;
            this.RequestL2AuthButton.Click += new System.EventHandler(this.RequestL2AuthButton_Click);
            // 
            // RequestL3AuthButton
            // 
            this.RequestL3AuthButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RequestL3AuthButton.Location = new System.Drawing.Point(293, 100);
            this.RequestL3AuthButton.Name = "RequestL3AuthButton";
            this.RequestL3AuthButton.Size = new System.Drawing.Size(133, 26);
            this.RequestL3AuthButton.TabIndex = 7;
            this.RequestL3AuthButton.Text = "Request";
            this.RequestL3AuthButton.UseVisualStyleBackColor = true;
            this.RequestL3AuthButton.Click += new System.EventHandler(this.RequestL3AuthButton_Click);
            // 
            // L1AuthPasswordAttempt
            // 
            this.L1AuthPasswordAttempt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.L1AuthPasswordAttempt.Location = new System.Drawing.Point(138, 36);
            this.L1AuthPasswordAttempt.Name = "L1AuthPasswordAttempt";
            this.L1AuthPasswordAttempt.Size = new System.Drawing.Size(148, 20);
            this.L1AuthPasswordAttempt.TabIndex = 8;
            // 
            // L2PasswordAttempt
            // 
            this.L2PasswordAttempt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.L2PasswordAttempt.Location = new System.Drawing.Point(138, 68);
            this.L2PasswordAttempt.Name = "L2PasswordAttempt";
            this.L2PasswordAttempt.Size = new System.Drawing.Size(148, 20);
            this.L2PasswordAttempt.TabIndex = 9;
            // 
            // L3PasswordAttempt
            // 
            this.L3PasswordAttempt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.L3PasswordAttempt.Location = new System.Drawing.Point(138, 100);
            this.L3PasswordAttempt.Name = "L3PasswordAttempt";
            this.L3PasswordAttempt.Size = new System.Drawing.Size(148, 20);
            this.L3PasswordAttempt.TabIndex = 10;
            // 
            // UpdateDatabaseTab
            // 
            this.UpdateDatabaseTab.Controls.Add(this.UpdateDatabaseStep4);
            this.UpdateDatabaseTab.Controls.Add(this.UpdateDatabaseStep0);
            this.UpdateDatabaseTab.Controls.Add(this.UpdateDatebaseStep8);
            this.UpdateDatabaseTab.Controls.Add(this.UpdateDatabaseStep1);
            this.UpdateDatabaseTab.Controls.Add(this.DatabaseLocationTextBox);
            this.UpdateDatabaseTab.Controls.Add(this.UpdateDatabaseStep3);
            this.UpdateDatabaseTab.Controls.Add(this.UpdateDatabaseStep6);
            this.UpdateDatabaseTab.Controls.Add(this.UpdateDatabaseStep2);
            this.UpdateDatabaseTab.Controls.Add(this.UpdateDatabaseStep5);
            this.UpdateDatabaseTab.Controls.Add(this.UpdateDatabaseStep3Advanced);
            this.UpdateDatabaseTab.Location = new System.Drawing.Point(4, 22);
            this.UpdateDatabaseTab.Name = "UpdateDatabaseTab";
            this.UpdateDatabaseTab.Padding = new System.Windows.Forms.Padding(3);
            this.UpdateDatabaseTab.Size = new System.Drawing.Size(442, 369);
            this.UpdateDatabaseTab.TabIndex = 0;
            this.UpdateDatabaseTab.Text = "Update Database";
            this.UpdateDatabaseTab.UseVisualStyleBackColor = true;
            // 
            // UpdateDatabaseStep4
            // 
            this.UpdateDatabaseStep4.Location = new System.Drawing.Point(6, 188);
            this.UpdateDatabaseStep4.Name = "UpdateDatabaseStep4";
            this.UpdateDatabaseStep4.Size = new System.Drawing.Size(430, 23);
            this.UpdateDatabaseStep4.TabIndex = 16;
            this.UpdateDatabaseStep4.Text = "Step 4: Upload changed files";
            this.UpdateDatabaseStep4.UseVisualStyleBackColor = true;
            this.UpdateDatabaseStep4.Click += new System.EventHandler(this.UpdateDatabaseStep4_Click);
            // 
            // UpdateDatabaseStep0
            // 
            this.UpdateDatabaseStep0.Location = new System.Drawing.Point(6, 6);
            this.UpdateDatabaseStep0.Name = "UpdateDatabaseStep0";
            this.UpdateDatabaseStep0.ReadOnly = true;
            this.UpdateDatabaseStep0.Size = new System.Drawing.Size(430, 21);
            this.UpdateDatabaseStep0.TabIndex = 15;
            this.UpdateDatabaseStep0.Text = "Step 0: Pull latest database!!!";
            // 
            // UpdateDatebaseStep8
            // 
            this.UpdateDatebaseStep8.Location = new System.Drawing.Point(6, 275);
            this.UpdateDatebaseStep8.Name = "UpdateDatebaseStep8";
            this.UpdateDatebaseStep8.ReadOnly = true;
            this.UpdateDatebaseStep8.Size = new System.Drawing.Size(430, 23);
            this.UpdateDatebaseStep8.TabIndex = 14;
            this.UpdateDatebaseStep8.Text = "Step 8: Post DatabaseUpdate.txt on discord, slack, forums";
            // 
            // UpdateApplicationTab
            // 
            this.UpdateApplicationTab.Controls.Add(this.UpdateApplicationStep9);
            this.UpdateApplicationTab.Controls.Add(this.UpdateApplicationStep7);
            this.UpdateApplicationTab.Controls.Add(this.UpdateApplicatonSteps0to4);
            this.UpdateApplicationTab.Controls.Add(this.UpdateApplicationStep8);
            this.UpdateApplicationTab.Location = new System.Drawing.Point(4, 22);
            this.UpdateApplicationTab.Name = "UpdateApplicationTab";
            this.UpdateApplicationTab.Padding = new System.Windows.Forms.Padding(3);
            this.UpdateApplicationTab.Size = new System.Drawing.Size(442, 369);
            this.UpdateApplicationTab.TabIndex = 1;
            this.UpdateApplicationTab.Text = "Update Application";
            this.UpdateApplicationTab.UseVisualStyleBackColor = true;
            // 
            // UpdateApplicationStep9
            // 
            this.UpdateApplicationStep9.Location = new System.Drawing.Point(6, 205);
            this.UpdateApplicationStep9.Name = "UpdateApplicationStep9";
            this.UpdateApplicationStep9.ReadOnly = true;
            this.UpdateApplicationStep9.Size = new System.Drawing.Size(430, 23);
            this.UpdateApplicationStep9.TabIndex = 15;
            this.UpdateApplicationStep9.Text = "Step 9: Post update notes on discord, slack, forums";
            // 
            // UpdateApplicationStep7
            // 
            this.UpdateApplicationStep7.Location = new System.Drawing.Point(6, 126);
            this.UpdateApplicationStep7.Name = "UpdateApplicationStep7";
            this.UpdateApplicationStep7.Size = new System.Drawing.Size(430, 32);
            this.UpdateApplicationStep7.TabIndex = 14;
            this.UpdateApplicationStep7.Text = "Step 7: Run script CreateUpdatePackages.php";
            this.UpdateApplicationStep7.UseVisualStyleBackColor = true;
            this.UpdateApplicationStep7.Click += new System.EventHandler(this.UpdateApplicationStep7_Click);
            // 
            // UpdateApplicatonSteps0to4
            // 
            this.UpdateApplicatonSteps0to4.Location = new System.Drawing.Point(6, 6);
            this.UpdateApplicatonSteps0to4.Name = "UpdateApplicatonSteps0to4";
            this.UpdateApplicatonSteps0to4.ReadOnly = true;
            this.UpdateApplicatonSteps0to4.Size = new System.Drawing.Size(430, 114);
            this.UpdateApplicatonSteps0to4.TabIndex = 11;
            this.UpdateApplicatonSteps0to4.Text = resources.GetString("UpdateApplicatonSteps0to4.Text");
            // 
            // UpdateApplicationStep8
            // 
            this.UpdateApplicationStep8.Location = new System.Drawing.Point(6, 164);
            this.UpdateApplicationStep8.Name = "UpdateApplicationStep8";
            this.UpdateApplicationStep8.Size = new System.Drawing.Size(430, 35);
            this.UpdateApplicationStep8.TabIndex = 10;
            this.UpdateApplicationStep8.Text = "Step 8: Run script CreateManagerInfo.php";
            this.UpdateApplicationStep8.UseVisualStyleBackColor = true;
            this.UpdateApplicationStep8.Click += new System.EventHandler(this.UpdateApplicationStep8_Click);
            // 
            // CleanOnlineFolders
            // 
            this.CleanOnlineFolders.Controls.Add(this.CleanFoldersStep2);
            this.CleanOnlineFolders.Controls.Add(this.CleanFoldersStep1);
            this.CleanOnlineFolders.Location = new System.Drawing.Point(4, 22);
            this.CleanOnlineFolders.Name = "CleanOnlineFolders";
            this.CleanOnlineFolders.Padding = new System.Windows.Forms.Padding(3);
            this.CleanOnlineFolders.Size = new System.Drawing.Size(442, 369);
            this.CleanOnlineFolders.TabIndex = 3;
            this.CleanOnlineFolders.Text = "Clean zip folders";
            this.CleanOnlineFolders.UseVisualStyleBackColor = true;
            // 
            // CleanFoldersStep2
            // 
            this.CleanFoldersStep2.Location = new System.Drawing.Point(6, 47);
            this.CleanFoldersStep2.Name = "CleanFoldersStep2";
            this.CleanFoldersStep2.Size = new System.Drawing.Size(430, 35);
            this.CleanFoldersStep2.TabIndex = 13;
            this.CleanFoldersStep2.Text = "Step 2: Start Process to delete old zip files";
            this.CleanFoldersStep2.UseVisualStyleBackColor = true;
            this.CleanFoldersStep2.Click += new System.EventHandler(this.CleanFoldersStep2_Click);
            // 
            // CleanFoldersStep1
            // 
            this.CleanFoldersStep1.Location = new System.Drawing.Point(6, 6);
            this.CleanFoldersStep1.Name = "CleanFoldersStep1";
            this.CleanFoldersStep1.Size = new System.Drawing.Size(430, 35);
            this.CleanFoldersStep1.TabIndex = 12;
            this.CleanFoldersStep1.Text = "Step 1: Run script CreateOutdatedFileList.php";
            this.CleanFoldersStep1.UseVisualStyleBackColor = true;
            this.CleanFoldersStep1.Click += new System.EventHandler(this.CleanFoldersStep1_Click);
            // 
            // CreatePasswordTab
            // 
            this.CreatePasswordTab.Controls.Add(this.PasswordL3Panel);
            this.CreatePasswordTab.Controls.Add(this.PasswordL2Panel);
            this.CreatePasswordTab.Location = new System.Drawing.Point(4, 22);
            this.CreatePasswordTab.Name = "CreatePasswordTab";
            this.CreatePasswordTab.Padding = new System.Windows.Forms.Padding(3);
            this.CreatePasswordTab.Size = new System.Drawing.Size(442, 369);
            this.CreatePasswordTab.TabIndex = 2;
            this.CreatePasswordTab.Text = "CreatePassword";
            this.CreatePasswordTab.UseVisualStyleBackColor = true;
            // 
            // PasswordL3Panel
            // 
            this.PasswordL3Panel.Controls.Add(this.ReadbackL3Password);
            this.PasswordL3Panel.Controls.Add(this.L3GenerateOutput);
            this.PasswordL3Panel.Controls.Add(this.CreatePasswordL3Label);
            this.PasswordL3Panel.Controls.Add(this.L3TextPassword);
            this.PasswordL3Panel.Controls.Add(this.GenerateL3Password);
            this.PasswordL3Panel.Location = new System.Drawing.Point(6, 107);
            this.PasswordL3Panel.Name = "PasswordL3Panel";
            this.PasswordL3Panel.Size = new System.Drawing.Size(430, 95);
            this.PasswordL3Panel.TabIndex = 5;
            // 
            // ReadbackL3Password
            // 
            this.ReadbackL3Password.Location = new System.Drawing.Point(213, 16);
            this.ReadbackL3Password.Name = "ReadbackL3Password";
            this.ReadbackL3Password.Size = new System.Drawing.Size(75, 20);
            this.ReadbackL3Password.TabIndex = 6;
            this.ReadbackL3Password.Text = "Read";
            this.ReadbackL3Password.UseVisualStyleBackColor = true;
            this.ReadbackL3Password.Click += new System.EventHandler(this.ReadbackL3Password_Click);
            // 
            // L3GenerateOutput
            // 
            this.L3GenerateOutput.Location = new System.Drawing.Point(6, 42);
            this.L3GenerateOutput.Name = "L3GenerateOutput";
            this.L3GenerateOutput.Size = new System.Drawing.Size(421, 48);
            this.L3GenerateOutput.TabIndex = 4;
            this.L3GenerateOutput.Text = "";
            // 
            // CreatePasswordL3Label
            // 
            this.CreatePasswordL3Label.AutoSize = true;
            this.CreatePasswordL3Label.Location = new System.Drawing.Point(3, 0);
            this.CreatePasswordL3Label.Name = "CreatePasswordL3Label";
            this.CreatePasswordL3Label.Size = new System.Drawing.Size(204, 13);
            this.CreatePasswordL3Label.TabIndex = 0;
            this.CreatePasswordL3Label.Text = "Create Password for Authorization Level 3";
            // 
            // L3TextPassword
            // 
            this.L3TextPassword.Location = new System.Drawing.Point(6, 16);
            this.L3TextPassword.Name = "L3TextPassword";
            this.L3TextPassword.Size = new System.Drawing.Size(120, 20);
            this.L3TextPassword.TabIndex = 1;
            // 
            // GenerateL3Password
            // 
            this.GenerateL3Password.Location = new System.Drawing.Point(132, 16);
            this.GenerateL3Password.Name = "GenerateL3Password";
            this.GenerateL3Password.Size = new System.Drawing.Size(75, 20);
            this.GenerateL3Password.TabIndex = 2;
            this.GenerateL3Password.Text = "Generate";
            this.GenerateL3Password.UseVisualStyleBackColor = true;
            this.GenerateL3Password.Click += new System.EventHandler(this.GenerateL3Password_Click);
            // 
            // PasswordL2Panel
            // 
            this.PasswordL2Panel.Controls.Add(this.ReadbackL2Password);
            this.PasswordL2Panel.Controls.Add(this.L2GenerateOutput);
            this.PasswordL2Panel.Controls.Add(this.CreatePasswordL2Label);
            this.PasswordL2Panel.Controls.Add(this.L2TextPassword);
            this.PasswordL2Panel.Controls.Add(this.GenerateL2Password);
            this.PasswordL2Panel.Location = new System.Drawing.Point(6, 6);
            this.PasswordL2Panel.Name = "PasswordL2Panel";
            this.PasswordL2Panel.Size = new System.Drawing.Size(430, 95);
            this.PasswordL2Panel.TabIndex = 3;
            // 
            // ReadbackL2Password
            // 
            this.ReadbackL2Password.Location = new System.Drawing.Point(213, 16);
            this.ReadbackL2Password.Name = "ReadbackL2Password";
            this.ReadbackL2Password.Size = new System.Drawing.Size(75, 20);
            this.ReadbackL2Password.TabIndex = 5;
            this.ReadbackL2Password.Text = "Read";
            this.ReadbackL2Password.UseVisualStyleBackColor = true;
            this.ReadbackL2Password.Click += new System.EventHandler(this.ReadbackL2Password_Click);
            // 
            // L2GenerateOutput
            // 
            this.L2GenerateOutput.Location = new System.Drawing.Point(6, 42);
            this.L2GenerateOutput.Name = "L2GenerateOutput";
            this.L2GenerateOutput.Size = new System.Drawing.Size(421, 48);
            this.L2GenerateOutput.TabIndex = 4;
            this.L2GenerateOutput.Text = "";
            // 
            // CreatePasswordL2Label
            // 
            this.CreatePasswordL2Label.AutoSize = true;
            this.CreatePasswordL2Label.Location = new System.Drawing.Point(3, 0);
            this.CreatePasswordL2Label.Name = "CreatePasswordL2Label";
            this.CreatePasswordL2Label.Size = new System.Drawing.Size(204, 13);
            this.CreatePasswordL2Label.TabIndex = 0;
            this.CreatePasswordL2Label.Text = "Create Password for Authorization Level 2";
            // 
            // L2TextPassword
            // 
            this.L2TextPassword.Location = new System.Drawing.Point(6, 16);
            this.L2TextPassword.Name = "L2TextPassword";
            this.L2TextPassword.Size = new System.Drawing.Size(120, 20);
            this.L2TextPassword.TabIndex = 1;
            // 
            // GenerateL2Password
            // 
            this.GenerateL2Password.Location = new System.Drawing.Point(132, 16);
            this.GenerateL2Password.Name = "GenerateL2Password";
            this.GenerateL2Password.Size = new System.Drawing.Size(75, 20);
            this.GenerateL2Password.TabIndex = 2;
            this.GenerateL2Password.Text = "Generate";
            this.GenerateL2Password.UseVisualStyleBackColor = true;
            this.GenerateL2Password.Click += new System.EventHandler(this.GenerateL2Password_Click);
            // 
            // ScriptLogOutput
            // 
            this.ScriptLogOutput.Location = new System.Drawing.Point(464, 34);
            this.ScriptLogOutput.Name = "ScriptLogOutput";
            this.ScriptLogOutput.ReadOnly = true;
            this.ScriptLogOutput.Size = new System.Drawing.Size(408, 395);
            this.ScriptLogOutput.TabIndex = 13;
            this.ScriptLogOutput.Text = "";
            // 
            // ScriptOutputLabel
            // 
            this.ScriptOutputLabel.Location = new System.Drawing.Point(464, 9);
            this.ScriptOutputLabel.Name = "ScriptOutputLabel";
            this.ScriptOutputLabel.Size = new System.Drawing.Size(408, 22);
            this.ScriptOutputLabel.TabIndex = 14;
            this.ScriptOutputLabel.Text = "Script Output";
            this.ScriptOutputLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // DatabaseUpdater
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 441);
            this.Controls.Add(this.ScriptOutputLabel);
            this.Controls.Add(this.ScriptLogOutput);
            this.Controls.Add(this.DatabaseUpdateTabControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(900, 480);
            this.Name = "DatabaseUpdater";
            this.Text = "DatabaseUpdateUtility";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CRCFileSizeUpdate_FormClosing);
            this.Load += new System.EventHandler(this.CRCFileSizeUpdate_Load);
            this.DatabaseUpdateTabControl.ResumeLayout(false);
            this.AuthStatus.ResumeLayout(false);
            this.AuthorizationTable.ResumeLayout(false);
            this.AuthorizationTable.PerformLayout();
            this.UpdateDatabaseTab.ResumeLayout(false);
            this.UpdateApplicationTab.ResumeLayout(false);
            this.CleanOnlineFolders.ResumeLayout(false);
            this.CreatePasswordTab.ResumeLayout(false);
            this.PasswordL3Panel.ResumeLayout(false);
            this.PasswordL3Panel.PerformLayout();
            this.PasswordL2Panel.ResumeLayout(false);
            this.PasswordL2Panel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button UpdateDatabaseStep1;
        private System.Windows.Forms.RichTextBox DatabaseLocationTextBox;
        private System.Windows.Forms.Button UpdateDatabaseStep3;
        private System.Windows.Forms.OpenFileDialog addZipsDialog;
        private System.Windows.Forms.OpenFileDialog loadDatabaseDialog;
        private System.Windows.Forms.Button UpdateDatabaseStep2;
        private System.Windows.Forms.Button UpdateDatabaseStep3Advanced;
        private System.Windows.Forms.Button UpdateDatabaseStep5;
        private System.Windows.Forms.Button UpdateDatabaseStep6;
        private System.Windows.Forms.TabControl DatabaseUpdateTabControl;
        private System.Windows.Forms.TabPage UpdateDatabaseTab;
        private System.Windows.Forms.TabPage UpdateApplicationTab;
        private System.Windows.Forms.TabPage CleanOnlineFolders;
        private System.Windows.Forms.Button CleanFoldersStep2;
        private System.Windows.Forms.Button CleanFoldersStep1;
        private System.Windows.Forms.TabPage CreatePasswordTab;
        private System.Windows.Forms.Panel PasswordL3Panel;
        private System.Windows.Forms.RichTextBox L3GenerateOutput;
        private System.Windows.Forms.Label CreatePasswordL3Label;
        private System.Windows.Forms.TextBox L3TextPassword;
        private System.Windows.Forms.Button GenerateL3Password;
        private System.Windows.Forms.Panel PasswordL2Panel;
        private System.Windows.Forms.RichTextBox L2GenerateOutput;
        private System.Windows.Forms.Label CreatePasswordL2Label;
        private System.Windows.Forms.TextBox L2TextPassword;
        private System.Windows.Forms.Button GenerateL2Password;
        private System.Windows.Forms.RichTextBox ScriptLogOutput;
        private System.Windows.Forms.Label ScriptOutputLabel;
        private System.Windows.Forms.TabPage AuthStatus;
        private System.Windows.Forms.RichTextBox UpdateApplicatonSteps0to4;
        private System.Windows.Forms.Button UpdateApplicationStep8;
        private System.Windows.Forms.TableLayoutPanel AuthorizationTable;
        private System.Windows.Forms.Label CurrentAuthStatusLabel;
        private System.Windows.Forms.Label AuthStatusLabel;
        private System.Windows.Forms.Label RequestL1AuthLabel;
        private System.Windows.Forms.Label RequestL2AuthLabel;
        private System.Windows.Forms.Label RequestL3AuthLabel;
        private System.Windows.Forms.Button RequestL1AuthButton;
        private System.Windows.Forms.Button RequestL2AuthButton;
        private System.Windows.Forms.Button RequestL3AuthButton;
        private System.Windows.Forms.RichTextBox UpdateDatebaseStep8;
        private System.Windows.Forms.RichTextBox UpdateApplicationStep9;
        private System.Windows.Forms.Button UpdateApplicationStep7;
        private System.Windows.Forms.RichTextBox UpdateDatabaseStep0;
        private System.Windows.Forms.TextBox L1AuthPasswordAttempt;
        private System.Windows.Forms.TextBox L2PasswordAttempt;
        private System.Windows.Forms.TextBox L3PasswordAttempt;
        private System.Windows.Forms.Button ReadbackL3Password;
        private System.Windows.Forms.Button ReadbackL2Password;
        private System.Windows.Forms.ToolTip ButtonInfo;
        private System.Windows.Forms.Button UpdateDatabaseStep4;
    }
}