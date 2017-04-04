﻿namespace RelhaxModpack
{
    partial class MainWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.statusLabel = new System.Windows.Forms.Label();
            this.childProgressBar = new System.Windows.Forms.ProgressBar();
            this.findWotExe = new System.Windows.Forms.OpenFileDialog();
            this.forceManuel = new System.Windows.Forms.CheckBox();
            this.formPageLink = new System.Windows.Forms.LinkLabel();
            this.parrentProgressBar = new System.Windows.Forms.ProgressBar();
            this.speedLabel = new System.Windows.Forms.Label();
            this.installRelhaxMod = new System.Windows.Forms.Button();
            this.uninstallRelhaxMod = new System.Windows.Forms.Button();
            this.cleanInstallCB = new System.Windows.Forms.CheckBox();
            this.cancerFontCB = new System.Windows.Forms.CheckBox();
            this.backupModsCheckBox = new System.Windows.Forms.CheckBox();
            this.settingsGroupBox = new System.Windows.Forms.GroupBox();
            this.darkUICB = new System.Windows.Forms.CheckBox();
            this.cleanUninstallCB = new System.Windows.Forms.CheckBox();
            this.saveUserDataCB = new System.Windows.Forms.CheckBox();
            this.saveLastInstallCB = new System.Windows.Forms.CheckBox();
            this.largerFontButton = new System.Windows.Forms.CheckBox();
            this.loadingImageGroupBox = new System.Windows.Forms.GroupBox();
            this.thirdGuardsLoadingImageRB = new System.Windows.Forms.RadioButton();
            this.standardImageRB = new System.Windows.Forms.RadioButton();
            this.findBugAddModLabel = new System.Windows.Forms.LinkLabel();
            this.cancelDownloadButton = new System.Windows.Forms.Button();
            this.downloadTimer = new System.Windows.Forms.Timer(this.components);
            this.downloadProgress = new System.Windows.Forms.RichTextBox();
            this.settingsGroupBox.SuspendLayout();
            this.loadingImageGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusLabel
            // 
            this.statusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.statusLabel.AutoSize = true;
            this.statusLabel.Location = new System.Drawing.Point(9, 308);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(53, 13);
            this.statusLabel.TabIndex = 10;
            this.statusLabel.Text = "STATUS:";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // childProgressBar
            // 
            this.childProgressBar.Location = new System.Drawing.Point(12, 411);
            this.childProgressBar.Name = "childProgressBar";
            this.childProgressBar.Size = new System.Drawing.Size(265, 23);
            this.childProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.childProgressBar.TabIndex = 11;
            // 
            // findWotExe
            // 
            this.findWotExe.Filter = "WorldOfTanks.exe|WorldOfTanks.exe";
            this.findWotExe.Title = "Find WorldOfTanks.exe";
            // 
            // forceManuel
            // 
            this.forceManuel.AutoSize = true;
            this.forceManuel.Location = new System.Drawing.Point(6, 15);
            this.forceManuel.Name = "forceManuel";
            this.forceManuel.Size = new System.Drawing.Size(166, 17);
            this.forceManuel.TabIndex = 13;
            this.forceManuel.Text = "Force manual game detection";
            this.forceManuel.UseVisualStyleBackColor = true;
            this.forceManuel.CheckedChanged += new System.EventHandler(this.forceManuel_CheckedChanged);
            this.forceManuel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.forceManuel_MouseDown);
            this.forceManuel.MouseEnter += new System.EventHandler(this.forceManuel_MouseEnter);
            this.forceManuel.MouseLeave += new System.EventHandler(this.forceManuel_MouseLeave);
            // 
            // formPageLink
            // 
            this.formPageLink.AutoSize = true;
            this.formPageLink.Location = new System.Drawing.Point(9, 474);
            this.formPageLink.Name = "formPageLink";
            this.formPageLink.Size = new System.Drawing.Size(132, 13);
            this.formPageLink.TabIndex = 16;
            this.formPageLink.TabStop = true;
            this.formPageLink.Text = "View Modpack Form Page";
            this.formPageLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.formPageLink_LinkClicked);
            // 
            // parrentProgressBar
            // 
            this.parrentProgressBar.Location = new System.Drawing.Point(12, 382);
            this.parrentProgressBar.Name = "parrentProgressBar";
            this.parrentProgressBar.Size = new System.Drawing.Size(265, 23);
            this.parrentProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.parrentProgressBar.TabIndex = 17;
            // 
            // speedLabel
            // 
            this.speedLabel.AutoSize = true;
            this.speedLabel.Location = new System.Drawing.Point(12, 437);
            this.speedLabel.Name = "speedLabel";
            this.speedLabel.Size = new System.Drawing.Size(24, 13);
            this.speedLabel.TabIndex = 18;
            this.speedLabel.Text = "Idle";
            // 
            // installRelhaxMod
            // 
            this.installRelhaxMod.Location = new System.Drawing.Point(12, 12);
            this.installRelhaxMod.Name = "installRelhaxMod";
            this.installRelhaxMod.Size = new System.Drawing.Size(265, 34);
            this.installRelhaxMod.TabIndex = 19;
            this.installRelhaxMod.Text = "Install Relhax ModPack";
            this.installRelhaxMod.UseVisualStyleBackColor = true;
            this.installRelhaxMod.Click += new System.EventHandler(this.installRelhaxMod_Click);
            // 
            // uninstallRelhaxMod
            // 
            this.uninstallRelhaxMod.Location = new System.Drawing.Point(12, 52);
            this.uninstallRelhaxMod.Name = "uninstallRelhaxMod";
            this.uninstallRelhaxMod.Size = new System.Drawing.Size(265, 34);
            this.uninstallRelhaxMod.TabIndex = 20;
            this.uninstallRelhaxMod.Text = "Uninstall Relhax Modpack";
            this.uninstallRelhaxMod.UseVisualStyleBackColor = true;
            this.uninstallRelhaxMod.Click += new System.EventHandler(this.uninstallRelhaxMod_Click);
            // 
            // cleanInstallCB
            // 
            this.cleanInstallCB.AutoSize = true;
            this.cleanInstallCB.Checked = true;
            this.cleanInstallCB.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cleanInstallCB.Location = new System.Drawing.Point(6, 30);
            this.cleanInstallCB.Name = "cleanInstallCB";
            this.cleanInstallCB.Size = new System.Drawing.Size(187, 17);
            this.cleanInstallCB.TabIndex = 21;
            this.cleanInstallCB.Text = "Clean Installation (Recommended)";
            this.cleanInstallCB.UseVisualStyleBackColor = true;
            this.cleanInstallCB.CheckedChanged += new System.EventHandler(this.cleanInstallCB_CheckedChanged);
            this.cleanInstallCB.MouseDown += new System.Windows.Forms.MouseEventHandler(this.cleanInstallCB_MouseDown);
            this.cleanInstallCB.MouseEnter += new System.EventHandler(this.cleanInstallCB_MouseEnter);
            this.cleanInstallCB.MouseLeave += new System.EventHandler(this.cleanInstallCB_MouseLeave);
            // 
            // cancerFontCB
            // 
            this.cancerFontCB.AutoSize = true;
            this.cancerFontCB.Location = new System.Drawing.Point(6, 60);
            this.cancerFontCB.Name = "cancerFontCB";
            this.cancerFontCB.Size = new System.Drawing.Size(81, 17);
            this.cancerFontCB.TabIndex = 23;
            this.cancerFontCB.Text = "Cancer font";
            this.cancerFontCB.UseVisualStyleBackColor = true;
            this.cancerFontCB.CheckedChanged += new System.EventHandler(this.cancerFontCB_CheckedChanged);
            this.cancerFontCB.MouseDown += new System.Windows.Forms.MouseEventHandler(this.cancerFontCB_MouseDown);
            this.cancerFontCB.MouseEnter += new System.EventHandler(this.cancerFontCB_MouseEnter);
            this.cancerFontCB.MouseLeave += new System.EventHandler(this.cancerFontCB_MouseLeave);
            // 
            // backupModsCheckBox
            // 
            this.backupModsCheckBox.AutoSize = true;
            this.backupModsCheckBox.Location = new System.Drawing.Point(6, 45);
            this.backupModsCheckBox.Name = "backupModsCheckBox";
            this.backupModsCheckBox.Size = new System.Drawing.Size(156, 17);
            this.backupModsCheckBox.TabIndex = 24;
            this.backupModsCheckBox.Text = "Backup current mods folder";
            this.backupModsCheckBox.UseVisualStyleBackColor = true;
            this.backupModsCheckBox.CheckedChanged += new System.EventHandler(this.backupModsCheckBox_CheckedChanged);
            this.backupModsCheckBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.backupModsCheckBox_MouseDown);
            this.backupModsCheckBox.MouseEnter += new System.EventHandler(this.backupModsCheckBox_MouseEnter);
            this.backupModsCheckBox.MouseLeave += new System.EventHandler(this.backupModsCheckBox_MouseLeave);
            // 
            // settingsGroupBox
            // 
            this.settingsGroupBox.Controls.Add(this.darkUICB);
            this.settingsGroupBox.Controls.Add(this.cleanUninstallCB);
            this.settingsGroupBox.Controls.Add(this.saveUserDataCB);
            this.settingsGroupBox.Controls.Add(this.saveLastInstallCB);
            this.settingsGroupBox.Controls.Add(this.largerFontButton);
            this.settingsGroupBox.Controls.Add(this.forceManuel);
            this.settingsGroupBox.Controls.Add(this.cancerFontCB);
            this.settingsGroupBox.Controls.Add(this.backupModsCheckBox);
            this.settingsGroupBox.Controls.Add(this.cleanInstallCB);
            this.settingsGroupBox.Location = new System.Drawing.Point(12, 92);
            this.settingsGroupBox.Name = "settingsGroupBox";
            this.settingsGroupBox.Size = new System.Drawing.Size(265, 155);
            this.settingsGroupBox.TabIndex = 25;
            this.settingsGroupBox.TabStop = false;
            this.settingsGroupBox.Text = "RelHax ModPack Settings";
            // 
            // darkUICB
            // 
            this.darkUICB.AutoSize = true;
            this.darkUICB.Location = new System.Drawing.Point(6, 135);
            this.darkUICB.Name = "darkUICB";
            this.darkUICB.Size = new System.Drawing.Size(63, 17);
            this.darkUICB.TabIndex = 30;
            this.darkUICB.Text = "Dark UI";
            this.darkUICB.UseVisualStyleBackColor = true;
            this.darkUICB.CheckedChanged += new System.EventHandler(this.darkUICB_CheckedChanged);
            // 
            // cleanUninstallCB
            // 
            this.cleanUninstallCB.AutoSize = true;
            this.cleanUninstallCB.Location = new System.Drawing.Point(6, 120);
            this.cleanUninstallCB.Name = "cleanUninstallCB";
            this.cleanUninstallCB.Size = new System.Drawing.Size(117, 17);
            this.cleanUninstallCB.TabIndex = 29;
            this.cleanUninstallCB.Text = "Clean uninstallation";
            this.cleanUninstallCB.UseVisualStyleBackColor = true;
            this.cleanUninstallCB.CheckedChanged += new System.EventHandler(this.cleanUninstallCB_CheckedChanged);
            this.cleanUninstallCB.MouseDown += new System.Windows.Forms.MouseEventHandler(this.cleanUninstallCB_MouseDown);
            this.cleanUninstallCB.MouseEnter += new System.EventHandler(this.cleanUninstallCB_MouseEnter);
            this.cleanUninstallCB.MouseLeave += new System.EventHandler(this.cleanUninstallCB_MouseLeave);
            // 
            // saveUserDataCB
            // 
            this.saveUserDataCB.AutoSize = true;
            this.saveUserDataCB.Location = new System.Drawing.Point(6, 105);
            this.saveUserDataCB.Name = "saveUserDataCB";
            this.saveUserDataCB.Size = new System.Drawing.Size(139, 17);
            this.saveUserDataCB.TabIndex = 27;
            this.saveUserDataCB.Text = "Save User created data";
            this.saveUserDataCB.UseVisualStyleBackColor = true;
            this.saveUserDataCB.CheckedChanged += new System.EventHandler(this.saveUserDataCB_CheckedChanged);
            this.saveUserDataCB.MouseDown += new System.Windows.Forms.MouseEventHandler(this.saveUserDataCB_MouseDown);
            this.saveUserDataCB.MouseEnter += new System.EventHandler(this.saveUserDataCB_MouseEnter);
            this.saveUserDataCB.MouseLeave += new System.EventHandler(this.saveUserDataCB_MouseLeave);
            // 
            // saveLastInstallCB
            // 
            this.saveLastInstallCB.AutoSize = true;
            this.saveLastInstallCB.Location = new System.Drawing.Point(6, 90);
            this.saveLastInstallCB.Name = "saveLastInstallCB";
            this.saveLastInstallCB.Size = new System.Drawing.Size(138, 17);
            this.saveLastInstallCB.TabIndex = 26;
            this.saveLastInstallCB.Text = "Save last install\'s config";
            this.saveLastInstallCB.UseVisualStyleBackColor = true;
            this.saveLastInstallCB.CheckedChanged += new System.EventHandler(this.saveLastInstallCB_CheckedChanged);
            this.saveLastInstallCB.MouseDown += new System.Windows.Forms.MouseEventHandler(this.saveLastInstallCB_MouseDown);
            this.saveLastInstallCB.MouseEnter += new System.EventHandler(this.saveLastInstallCB_MouseEnter);
            this.saveLastInstallCB.MouseLeave += new System.EventHandler(this.saveLastInstallCB_MouseLeave);
            // 
            // largerFontButton
            // 
            this.largerFontButton.AutoSize = true;
            this.largerFontButton.Location = new System.Drawing.Point(6, 75);
            this.largerFontButton.Name = "largerFontButton";
            this.largerFontButton.Size = new System.Drawing.Size(80, 17);
            this.largerFontButton.TabIndex = 25;
            this.largerFontButton.Text = "Larger Font";
            this.largerFontButton.UseVisualStyleBackColor = true;
            this.largerFontButton.CheckedChanged += new System.EventHandler(this.largerFontButton_CheckedChanged);
            this.largerFontButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.largerFontButton_MouseDown);
            this.largerFontButton.MouseEnter += new System.EventHandler(this.largerFontButton_MouseEnter);
            this.largerFontButton.MouseLeave += new System.EventHandler(this.largerFontButton_MouseLeave);
            // 
            // loadingImageGroupBox
            // 
            this.loadingImageGroupBox.Controls.Add(this.thirdGuardsLoadingImageRB);
            this.loadingImageGroupBox.Controls.Add(this.standardImageRB);
            this.loadingImageGroupBox.Location = new System.Drawing.Point(12, 251);
            this.loadingImageGroupBox.Name = "loadingImageGroupBox";
            this.loadingImageGroupBox.Size = new System.Drawing.Size(96, 54);
            this.loadingImageGroupBox.TabIndex = 26;
            this.loadingImageGroupBox.TabStop = false;
            this.loadingImageGroupBox.Text = "Loading Image";
            // 
            // thirdGuardsLoadingImageRB
            // 
            this.thirdGuardsLoadingImageRB.AutoSize = true;
            this.thirdGuardsLoadingImageRB.Location = new System.Drawing.Point(6, 30);
            this.thirdGuardsLoadingImageRB.Name = "thirdGuardsLoadingImageRB";
            this.thirdGuardsLoadingImageRB.Size = new System.Drawing.Size(72, 17);
            this.thirdGuardsLoadingImageRB.TabIndex = 1;
            this.thirdGuardsLoadingImageRB.TabStop = true;
            this.thirdGuardsLoadingImageRB.Text = "3rdguards";
            this.thirdGuardsLoadingImageRB.UseVisualStyleBackColor = true;
            this.thirdGuardsLoadingImageRB.CheckedChanged += new System.EventHandler(this.standardImageRB_CheckedChanged);
            this.thirdGuardsLoadingImageRB.MouseEnter += new System.EventHandler(this.thirdGuardsLoadingImageRB_MouseEnter);
            this.thirdGuardsLoadingImageRB.MouseLeave += new System.EventHandler(this.thirdGuardsLoadingImageRB_MouseLeave);
            // 
            // standardImageRB
            // 
            this.standardImageRB.AutoSize = true;
            this.standardImageRB.Location = new System.Drawing.Point(6, 15);
            this.standardImageRB.Name = "standardImageRB";
            this.standardImageRB.Size = new System.Drawing.Size(68, 17);
            this.standardImageRB.TabIndex = 0;
            this.standardImageRB.TabStop = true;
            this.standardImageRB.Text = "Standard";
            this.standardImageRB.UseVisualStyleBackColor = true;
            this.standardImageRB.CheckedChanged += new System.EventHandler(this.thirdGuardsLoadingImageRB_CheckedChanged);
            this.standardImageRB.MouseEnter += new System.EventHandler(this.standardImageRB_MouseEnter);
            this.standardImageRB.MouseLeave += new System.EventHandler(this.standardImageRB_MouseLeave);
            // 
            // findBugAddModLabel
            // 
            this.findBugAddModLabel.AutoSize = true;
            this.findBugAddModLabel.Location = new System.Drawing.Point(9, 454);
            this.findBugAddModLabel.Name = "findBugAddModLabel";
            this.findBugAddModLabel.Size = new System.Drawing.Size(163, 13);
            this.findBugAddModLabel.TabIndex = 27;
            this.findBugAddModLabel.TabStop = true;
            this.findBugAddModLabel.Text = "Find a bug? Want a mod added?";
            this.findBugAddModLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.findBugAddModLabel_LinkClicked);
            // 
            // cancelDownloadButton
            // 
            this.cancelDownloadButton.Enabled = false;
            this.cancelDownloadButton.Location = new System.Drawing.Point(178, 454);
            this.cancelDownloadButton.Name = "cancelDownloadButton";
            this.cancelDownloadButton.Size = new System.Drawing.Size(99, 23);
            this.cancelDownloadButton.TabIndex = 28;
            this.cancelDownloadButton.Text = "Cancel Download";
            this.cancelDownloadButton.UseVisualStyleBackColor = true;
            this.cancelDownloadButton.Visible = false;
            this.cancelDownloadButton.Click += new System.EventHandler(this.cancelDownloadButton_Click);
            // 
            // downloadTimer
            // 
            this.downloadTimer.Interval = 1000;
            this.downloadTimer.Tick += new System.EventHandler(this.downloadTimer_Tick);
            // 
            // downloadProgress
            // 
            this.downloadProgress.Location = new System.Drawing.Point(12, 324);
            this.downloadProgress.Name = "downloadProgress";
            this.downloadProgress.ReadOnly = true;
            this.downloadProgress.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.downloadProgress.Size = new System.Drawing.Size(265, 52);
            this.downloadProgress.TabIndex = 29;
            this.downloadProgress.Text = "";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(289, 497);
            this.Controls.Add(this.downloadProgress);
            this.Controls.Add(this.cancelDownloadButton);
            this.Controls.Add(this.findBugAddModLabel);
            this.Controls.Add(this.loadingImageGroupBox);
            this.Controls.Add(this.settingsGroupBox);
            this.Controls.Add(this.uninstallRelhaxMod);
            this.Controls.Add(this.installRelhaxMod);
            this.Controls.Add(this.speedLabel);
            this.Controls.Add(this.parrentProgressBar);
            this.Controls.Add(this.formPageLink);
            this.Controls.Add(this.childProgressBar);
            this.Controls.Add(this.statusLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainWindow";
            this.Text = "RelHax ";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing);
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.settingsGroupBox.ResumeLayout(false);
            this.settingsGroupBox.PerformLayout();
            this.loadingImageGroupBox.ResumeLayout(false);
            this.loadingImageGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.ProgressBar childProgressBar;
        private System.Windows.Forms.OpenFileDialog findWotExe;
        private System.Windows.Forms.CheckBox forceManuel;
        private System.Windows.Forms.LinkLabel formPageLink;
        private System.Windows.Forms.ProgressBar parrentProgressBar;
        private System.Windows.Forms.Label speedLabel;
        private System.Windows.Forms.Button installRelhaxMod;
        private System.Windows.Forms.Button uninstallRelhaxMod;
        private System.Windows.Forms.CheckBox cleanInstallCB;
        private System.Windows.Forms.CheckBox cancerFontCB;
        private System.Windows.Forms.CheckBox backupModsCheckBox;
        private System.Windows.Forms.GroupBox settingsGroupBox;
        private System.Windows.Forms.CheckBox largerFontButton;
        private System.Windows.Forms.GroupBox loadingImageGroupBox;
        private System.Windows.Forms.RadioButton thirdGuardsLoadingImageRB;
        private System.Windows.Forms.RadioButton standardImageRB;
        private System.Windows.Forms.LinkLabel findBugAddModLabel;
        private System.Windows.Forms.CheckBox saveLastInstallCB;
        private System.Windows.Forms.Button cancelDownloadButton;
        private System.Windows.Forms.CheckBox saveUserDataCB;
        private System.Windows.Forms.Timer downloadTimer;
        private System.Windows.Forms.CheckBox cleanUninstallCB;
        private System.Windows.Forms.RichTextBox downloadProgress;
        private System.Windows.Forms.CheckBox darkUICB;
    }
}

