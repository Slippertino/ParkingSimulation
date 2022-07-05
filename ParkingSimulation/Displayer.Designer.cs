
namespace ParkingSimulation
{
    partial class Displayer
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
            this.console = new System.Windows.Forms.RichTextBox();
            this.extraInfoLabel = new System.Windows.Forms.Label();
            this.parking = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.parking)).BeginInit();
            this.SuspendLayout();
            // 
            // console
            // 
            this.console.BackColor = System.Drawing.Color.Black;
            this.console.BulletIndent = 10;
            this.console.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.console.Dock = System.Windows.Forms.DockStyle.Right;
            this.console.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.console.ForeColor = System.Drawing.Color.White;
            this.console.Location = new System.Drawing.Point(1614, 0);
            this.console.Name = "console";
            this.console.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.console.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.console.Size = new System.Drawing.Size(290, 1041);
            this.console.TabIndex = 0;
            this.console.Text = "";
            // 
            // extraInfoLabel
            // 
            this.extraInfoLabel.AutoSize = true;
            this.extraInfoLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 100F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.extraInfoLabel.Location = new System.Drawing.Point(1, 0);
            this.extraInfoLabel.Name = "extraInfoLabel";
            this.extraInfoLabel.Size = new System.Drawing.Size(0, 153);
            this.extraInfoLabel.TabIndex = 1;
            // 
            // parking
            // 
            this.parking.Location = new System.Drawing.Point(235, 180);
            this.parking.Name = "parking";
            this.parking.Size = new System.Drawing.Size(949, 393);
            this.parking.TabIndex = 2;
            this.parking.TabStop = false;
            // 
            // Displayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1904, 1041);
            this.Controls.Add(this.extraInfoLabel);
            this.Controls.Add(this.console);
            this.Controls.Add(this.parking);
            this.DoubleBuffered = true;
            this.ImeMode = System.Windows.Forms.ImeMode.On;
            this.Name = "Displayer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ParkingView";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.parking)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox console;
        private System.Windows.Forms.Label       extraInfoLabel;
        private System.Windows.Forms.PictureBox  parking;
    }
}

