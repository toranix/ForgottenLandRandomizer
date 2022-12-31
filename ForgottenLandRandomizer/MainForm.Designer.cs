
namespace ForgottenLandRandomizer
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.romfsPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.openRomfs = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.randStoryStages = new System.Windows.Forms.CheckBox();
            this.randStoryStagesPanel = new System.Windows.Forms.Panel();
            this.scaleDees = new System.Windows.Forms.CheckBox();
            this.randomize = new System.Windows.Forms.Button();
            this.romfsHelp = new System.Windows.Forms.Button();
            this.seed = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.randCopyAbilities = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.randStoryStagesPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // romfsPath
            // 
            this.romfsPath.Location = new System.Drawing.Point(116, 6);
            this.romfsPath.Name = "romfsPath";
            this.romfsPath.Size = new System.Drawing.Size(313, 20);
            this.romfsPath.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Game RomFS Path";
            // 
            // openRomfs
            // 
            this.openRomfs.Location = new System.Drawing.Point(435, 5);
            this.openRomfs.Name = "openRomfs";
            this.openRomfs.Size = new System.Drawing.Size(29, 22);
            this.openRomfs.TabIndex = 2;
            this.openRomfs.Text = "...";
            this.openRomfs.UseVisualStyleBackColor = true;
            this.openRomfs.Click += new System.EventHandler(this.openRomfs_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.flowLayoutPanel1);
            this.groupBox1.Location = new System.Drawing.Point(12, 59);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(487, 423);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Randomizer Settings";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.randStoryStages);
            this.flowLayoutPanel1.Controls.Add(this.randStoryStagesPanel);
            this.flowLayoutPanel1.Controls.Add(this.randCopyAbilities);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 16);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(481, 404);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // randStoryStages
            // 
            this.randStoryStages.AutoSize = true;
            this.randStoryStages.Location = new System.Drawing.Point(3, 3);
            this.randStoryStages.Name = "randStoryStages";
            this.randStoryStages.Size = new System.Drawing.Size(122, 17);
            this.randStoryStages.TabIndex = 0;
            this.randStoryStages.Text = "Shuffle Story Stages";
            this.toolTip.SetToolTip(this.randStoryStages, "Shuffle the order of all main story stages (excludes: Point of Arrival, In the Pr" +
        "esence of the King, and Lab Discovera)");
            this.randStoryStages.UseVisualStyleBackColor = true;
            this.randStoryStages.CheckedChanged += new System.EventHandler(this.randStoryStages_CheckChanged);
            // 
            // randStoryStagesPanel
            // 
            this.randStoryStagesPanel.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.randStoryStagesPanel.Controls.Add(this.scaleDees);
            this.randStoryStagesPanel.Location = new System.Drawing.Point(3, 26);
            this.randStoryStagesPanel.Name = "randStoryStagesPanel";
            this.randStoryStagesPanel.Size = new System.Drawing.Size(475, 21);
            this.randStoryStagesPanel.TabIndex = 1;
            this.randStoryStagesPanel.Visible = false;
            // 
            // scaleDees
            // 
            this.scaleDees.AutoSize = true;
            this.scaleDees.Location = new System.Drawing.Point(3, 3);
            this.scaleDees.Name = "scaleDees";
            this.scaleDees.Size = new System.Drawing.Size(184, 17);
            this.scaleDees.TabIndex = 0;
            this.scaleDees.Text = "Scale Waddle Dee Requirements";
            this.toolTip.SetToolTip(this.scaleDees, "Scale Boss stages to their position in the unlock order, requiring an average of " +
        "5 Waddle Dees per stage globally. Leave unchecked to set everything except 6-6 t" +
        "o 0 Waddle Dees.");
            this.scaleDees.UseVisualStyleBackColor = true;
            // 
            // randomize
            // 
            this.randomize.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.randomize.Location = new System.Drawing.Point(12, 488);
            this.randomize.Name = "randomize";
            this.randomize.Size = new System.Drawing.Size(487, 23);
            this.randomize.TabIndex = 4;
            this.randomize.Text = "Randomize";
            this.randomize.UseVisualStyleBackColor = true;
            this.randomize.Click += new System.EventHandler(this.randomize_Click);
            // 
            // romfsHelp
            // 
            this.romfsHelp.Location = new System.Drawing.Point(470, 5);
            this.romfsHelp.Name = "romfsHelp";
            this.romfsHelp.Size = new System.Drawing.Size(29, 22);
            this.romfsHelp.TabIndex = 5;
            this.romfsHelp.Text = "?";
            this.romfsHelp.UseVisualStyleBackColor = true;
            this.romfsHelp.Click += new System.EventHandler(this.romfsHelp_Click);
            // 
            // seed
            // 
            this.seed.Location = new System.Drawing.Point(116, 33);
            this.seed.Name = "seed";
            this.seed.Size = new System.Drawing.Size(383, 20);
            this.seed.TabIndex = 6;
            this.toolTip.SetToolTip(this.seed, resources.GetString("seed.ToolTip"));
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Seed";
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 10000;
            this.toolTip.InitialDelay = 500;
            this.toolTip.ReshowDelay = 100;
            // 
            // randCopyAbilities
            // 
            this.randCopyAbilities.AutoSize = true;
            this.randCopyAbilities.Location = new System.Drawing.Point(3, 53);
            this.randCopyAbilities.Name = "randCopyAbilities";
            this.randCopyAbilities.Size = new System.Drawing.Size(144, 17);
            this.randCopyAbilities.TabIndex = 2;
            this.randCopyAbilities.Text = "Randomize Copy Abilities";
            this.randCopyAbilities.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(511, 523);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.seed);
            this.Controls.Add(this.romfsHelp);
            this.Controls.Add(this.randomize);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.openRomfs);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.romfsPath);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Kirby and the Forgotten Land Randomizer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.randStoryStagesPanel.ResumeLayout(false);
            this.randStoryStagesPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox romfsPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button openRomfs;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button randomize;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.CheckBox randStoryStages;
        private System.Windows.Forms.Button romfsHelp;
        private System.Windows.Forms.TextBox seed;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Panel randStoryStagesPanel;
        private System.Windows.Forms.CheckBox scaleDees;
        private System.Windows.Forms.CheckBox randCopyAbilities;
    }
}

