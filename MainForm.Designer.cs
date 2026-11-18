// Copyright (c) 2026 Süleyman BEYHAN
// Licensed under the MIT License. See the LICENSE file in the project root.

namespace SBNotepad.UI
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            buttonSuccess = new System.Windows.Forms.Button();
            buttonError = new System.Windows.Forms.Button();
            buttonInfo = new System.Windows.Forms.Button();
            buttonWarning = new System.Windows.Forms.Button();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            //
            // tableLayoutPanel1
            //
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(buttonSuccess, 0, 0);
            tableLayoutPanel1.Controls.Add(buttonError, 1, 0);
            tableLayoutPanel1.Controls.Add(buttonInfo, 0, 1);
            tableLayoutPanel1.Controls.Add(buttonWarning, 1, 1);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(20);
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel1.Size = new System.Drawing.Size(520, 200);
            tableLayoutPanel1.TabIndex = 0;
            //
            // buttonSuccess
            //
            buttonSuccess.Dock = System.Windows.Forms.DockStyle.Fill;
            buttonSuccess.Margin = new System.Windows.Forms.Padding(8);
            buttonSuccess.Name = "buttonSuccess";
            buttonSuccess.TabIndex = 0;
            buttonSuccess.Text = "Success";
            buttonSuccess.UseVisualStyleBackColor = true;
            buttonSuccess.Click += buttonSuccess_Click;
            //
            // buttonError
            //
            buttonError.Dock = System.Windows.Forms.DockStyle.Fill;
            buttonError.Margin = new System.Windows.Forms.Padding(8);
            buttonError.Name = "buttonError";
            buttonError.TabIndex = 1;
            buttonError.Text = "Error";
            buttonError.UseVisualStyleBackColor = true;
            buttonError.Click += buttonError_Click;
            //
            // buttonInfo
            //
            buttonInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            buttonInfo.Margin = new System.Windows.Forms.Padding(8);
            buttonInfo.Name = "buttonInfo";
            buttonInfo.TabIndex = 2;
            buttonInfo.Text = "Info";
            buttonInfo.UseVisualStyleBackColor = true;
            buttonInfo.Click += buttonInfo_Click;
            //
            // buttonWarning
            //
            buttonWarning.Dock = System.Windows.Forms.DockStyle.Fill;
            buttonWarning.Margin = new System.Windows.Forms.Padding(8);
            buttonWarning.Name = "buttonWarning";
            buttonWarning.TabIndex = 3;
            buttonWarning.Text = "Warning";
            buttonWarning.UseVisualStyleBackColor = true;
            buttonWarning.Click += buttonWarning_Click;
            //
            // MainForm
            //
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            ClientSize = new System.Drawing.Size(520, 320);
            Controls.Add(tableLayoutPanel1);
            MinimumSize = new System.Drawing.Size(400, 300);
            Name = "MainForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Snackbar Demo";
            tableLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button buttonSuccess;
        private System.Windows.Forms.Button buttonError;
        private System.Windows.Forms.Button buttonInfo;
        private System.Windows.Forms.Button buttonWarning;
    }
}
