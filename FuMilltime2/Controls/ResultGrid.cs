namespace FuMilltime2.Controls
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;

    [ToolboxItem(true)]
    public class ResultGrid : DataGridView
    {
        private Timer updateTimer;

        public ResultGrid()
        {
            this.SuspendLayout();
            this.AllowUserToAddRows = false;
            this.AllowUserToDeleteRows = false;
            this.AllowUserToResizeRows = false;
            this.ClipboardCopyMode = DataGridViewClipboardCopyMode.Disable;
            this.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ReadOnly = true;
            this.Size = new Size(0, 0);
            this.AutoSize = true;
            this.TabIndex = 5;

            this.Columns.Add("Projekt", "Projekt");
            this.Columns.Add("Äkta tid", "Äkta tid");
            this.Columns.Add("Att rapportera", "Att rapportera");

            foreach (DataGridViewColumn column in this.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            this.ResumeLayout();
        }

        public event RedrewHandler Redrew;

        public void SetData(IReadOnlyList<ProjectInfo> data)
        {
            this.SetBusy(true);
            if (this.updateTimer == null)
            {
                this.updateTimer = this.CreateTimer(data);
                return;
            }

            if (this.updateTimer.Enabled)
            {
                // Timer still running. Stop, dispose and reset.
                this.updateTimer.Stop();
                this.updateTimer.Dispose();
                this.updateTimer = this.CreateTimer(data);
            }
            else
            {
                // Timer is expired and should have been stopped and disposed. Replace with new.
                this.updateTimer = this.CreateTimer(data);
            }
        }

        private static string GetHours(int minutes)
        {
            var ts = TimeSpan.FromMinutes(minutes);
            return ts.TotalHours.ToString("0.00");
        }

        private Timer CreateTimer(IReadOnlyList<ProjectInfo> data)
        {
            var t = new Timer { Interval = 300 };
            t.Tick += (sender, args) =>
                {
                    t.Stop();
                    t.Dispose();
                    this.SetDataInternal(data);
                };
            t.Start();
            return t;
        }

        private void OnRedrew(IReadOnlyList<ProjectInfo> data)
        {
            if (this.Redrew != null)
            {
                this.Redrew(this, new ResultGridEventArgs(data));
            }
        }

        private void SetBusy(bool b)
        {
        }

        private void SetDataInternal(IReadOnlyList<ProjectInfo> data)
        {
            this.Rows.Clear();
            foreach (var i in data)
            {
                this.Rows.Add(new object[] { i.ProjectName, GetHours(i.RealMinutes), GetHours(i.MinutesWithCommon) });
            }

            if (this.Rows.Count > 1)
            {
                this.Rows.Add(
                    new object[]
                        {
                            "SUMMA", GetHours(data.Sum(d => d.RealMinutes)), GetHours(data.Sum(d => d.MinutesWithCommon))
                        });

                var sumRowStyle = new DataGridViewCellStyle { BackColor = Color.DarkGreen, ForeColor = Color.White };
                this.Rows[this.RowCount - 1].DefaultCellStyle = sumRowStyle;
            }

            this.OnRedrew(data);

            this.SetBusy(false);
        }
    }

    public delegate void RedrewHandler(object sender, ResultGridEventArgs args);

    public class ResultGridEventArgs
    {
        public ResultGridEventArgs(IReadOnlyList<ProjectInfo> data)
        {
            this.Data = data;
        }
        public IReadOnlyList<ProjectInfo> Data { get; private set; }
    }
}