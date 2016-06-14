namespace FuMilltime2.Controls.ProjectInput
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    [ToolboxItem(false)]
    public class ProjectInputControl : TableLayoutPanel
    {
        private readonly TextBox namebox;

        private readonly Label sumlabel;

        private readonly TimeStampInputTextBox timebox;

        public ProjectInputControl(AutoCompleteStringCollection autoCompleteCollection)
        {
            this.namebox = new TextBox
                               {
                                   AutoCompleteCustomSource = autoCompleteCollection,
                                   AutoCompleteMode = AutoCompleteMode.SuggestAppend,
                                   AutoCompleteSource = AutoCompleteSource.CustomSource
                               };

            this.namebox.LostFocus += this.UpdateAutoCompleteCollection;

            this.timebox = new TimeStampInputTextBox();
            this.sumlabel = new Label { Text = @"00:00" };
            this.Size = new Size(0, 0);
            this.AutoSize = true;
            this.SuspendLayout();

            this.ColumnCount = 3;
            this.RowCount = 1;
            this.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            this.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            this.timebox.MinutesChanged += this.ValidateProjectName;
            this.namebox.TextChanged += this.ValidateProjectName;

            this.timebox.MinutesChanged += (sender, args) => this.OnContentChanged();
            this.namebox.TextChanged += (sender, args) =>
                {
                    if (this.TotalMinutes > 0)
                    {
                        // Only fire content changed event if the number of minutes in this control is relevant.
                        this.OnContentChanged();
                    }
                };

            this.timebox.MinutesChanged += this.SetSumLabel;

            this.namebox.KeyDown += (sender, args) =>
                {
                    if (args.KeyCode == Keys.Enter)
                    {
                        this.timebox.Focus();
                    }
                };

            this.timebox.KeyDown += (sender, args) =>
                {
                    if (args.KeyCode == Keys.Enter)
                    {
                        this.OnInputConfirmed();
                    }
                };

            this.Controls.Add(this.namebox);
            this.Controls.Add(this.timebox);
            this.Controls.Add(this.sumlabel);

            this.ResumeLayout();
        }

        private void UpdateAutoCompleteCollection(object sender, EventArgs e)
        {
            var text = (this.namebox.Text ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            foreach (string existing in this.namebox.AutoCompleteCustomSource)
            {
                if (existing.Equals(text, StringComparison.OrdinalIgnoreCase))
                {
                    // Already in the list. Don't add.
                    return;
                }
            }

            this.namebox.AutoCompleteCustomSource.Add(text);
        }

        public event ContentChangedHandler ContentChanged;

        public event InputConfirmedHandler InputConfirmed;

        public bool IsValid
        {
            get
            {
                return false == string.IsNullOrWhiteSpace(this.namebox.Text) && this.timebox.TotalMinutes > 0;
            }
        }

        public string ProjectName
        {
            get
            {
                return (this.namebox.Text ?? string.Empty).Trim();
            }
            set
            {
                this.namebox.Text = value;
            }
        }

        public int TotalMinutes
        {
            get
            {
                return this.timebox.TotalMinutes;
            }
        }

        public string TimeStamp
        {
            get
            {
                return this.timebox.Text;
            }
            set
            {
                this.timebox.Text = value;
            }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            this.namebox.Focus();
        }

        private void OnContentChanged()
        {
            if (this.ContentChanged != null)
            {
                this.ContentChanged(this, EventArgs.Empty);
            }
        }

        private void OnInputConfirmed()
        {
            if (this.InputConfirmed != null)
            {
                this.InputConfirmed(this, EventArgs.Empty);
            }
        }

        private void SetSumLabel(object sender, EventArgs args)
        {
            var ts = TimeSpan.FromMinutes(this.timebox.TotalMinutes);
            this.sumlabel.Text = string.Format("{0:D2}:{1:D2}", ts.Hours, ts.Minutes);
        }

        private void ValidateProjectName(object sender, EventArgs args)
        {
            if (this.timebox.TotalMinutes == 0)
            {
                this.namebox.BackColor = Color.White;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(this.namebox.Text))
                {
                    this.namebox.BackColor = Color.LightPink;
                }
                else
                {
                    this.namebox.BackColor = Color.LightGreen;
                }
            }
        }
    }

    public delegate void ContentChangedHandler(object sender, EventArgs args);

    public delegate void InputConfirmedHandler(object sender, EventArgs args);
}