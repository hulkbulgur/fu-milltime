namespace FuMilltime2.Controls.ProjectInput
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    [ToolboxItem(false)]
    public class ProjectInputPanel : FlowLayoutPanel
    {
        private readonly AutoCompleteStringCollection autoCompleteCollection;

        public ProjectInputPanel()
        {
            this.autoCompleteCollection = new AutoCompleteStringCollection();

            this.SuspendLayout();
            this.FlowDirection = FlowDirection.TopDown;
            this.WrapContents = false;
            this.AddNewInput();

            this.BorderStyle = BorderStyle.FixedSingle;
            this.Text = "Kundprojekt";

            this.Size = new Size(0, 0);
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            this.ResumeLayout(true);
        }

        public event DataChangedHandler DataChanged;

        public IReadOnlyDictionary<string, int> GetData()
        {
            var result = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            foreach (var control in this.Controls)
            {
                var c = (ProjectInputControl)control;
                if (c.IsValid)
                {
                    if (result.ContainsKey(c.ProjectName))
                    {
                        result[c.ProjectName] += c.TotalMinutes;
                    }
                    else
                    {
                        result.Add(c.ProjectName, c.TotalMinutes);
                    }
                }
            }

            return result;
        }

        public IReadOnlyList<TimeRecord> GetRecords(bool setCommon)
        {
            var result = new List<TimeRecord>();
            foreach (var control in this.Controls)
            {
                var c = (ProjectInputControl)control;
                result.Add(new TimeRecord { Common = setCommon, Name = c.ProjectName, TimeStamp = c.TimeStamp });
            }

            return result;
        }

        public void SetRecords(IEnumerable<TimeRecord> records)
        {
            this.Controls.Clear();
            foreach (var record in records)
            {
                var input = this.AddNewInput();
                input.ProjectName = record.Name;
                input.TimeStamp = record.TimeStamp;
            }
        }

        private ProjectInputControl AddNewInput()
        {
            var newInput = new ProjectInputControl(this.autoCompleteCollection);

            newInput.InputConfirmed += this.FocusNextInputAndAddNewIfNecessary;
            newInput.ContentChanged += (sender, args) => this.OnDataChanged();

            this.Controls.Add(newInput);

            this.InitLayout();

            return newInput;
        }

        private void FocusNextInputAndAddNewIfNecessary(object sender, EventArgs args)
        {
            var index = this.Controls.IndexOf((ProjectInputControl)sender);
            if (index == this.Controls.Count - 1)
            {
                // This is the last input. Add a new one and focus that.
                this.AddNewInput().Focus();
            }
            else
            {
                // This is not the last input. Focus the next.
                this.Controls[index + 1].Focus();
            }
        }

        private void OnDataChanged()
        {
            if (this.DataChanged != null)
            {
                this.DataChanged(this, EventArgs.Empty);
            }
        }
    }

    public delegate void DataChangedHandler(object sender, EventArgs args);
}