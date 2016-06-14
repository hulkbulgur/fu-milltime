namespace FuMilltime2.Controls
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;

    using FuMilltime2.Controls.ProjectInput;

    [ToolboxItem(true)]
    public class CustomerAndCommonProjectsPanel : FlowLayoutPanel
    {
        private readonly ProjectInputPanel commonProjectPanel;

        private readonly ProjectInputPanel customerProjectPanel;

        public CustomerAndCommonProjectsPanel()
        {
            this.SuspendLayout();

            this.Size = new Size(0, 0);
            this.AutoSize = true;
            this.WrapContents = false;
            this.FlowDirection = FlowDirection.LeftToRight;

            this.customerProjectPanel = new ProjectInputPanel();
            this.commonProjectPanel = new ProjectInputPanel();

            this.customerProjectPanel.DataChanged += (sender, args) => this.OnDataChanged();
            this.commonProjectPanel.DataChanged += (sender, args) => this.OnDataChanged();

            this.Controls.Add(this.customerProjectPanel);
            this.Controls.Add(this.commonProjectPanel);

            this.ResumeLayout();
        }

        public IReadOnlyList<TimeRecord> GetRecords()
        {
            var result = new List<TimeRecord>();
            result.AddRange(this.customerProjectPanel.GetRecords(false));
            result.AddRange(this.commonProjectPanel.GetRecords(true));
            return result;
        }

        public event DataChangedHandler DataChanged;

        public IReadOnlyList<ProjectInfo> GetData()
        {
            var commonMinutes = this.commonProjectPanel.GetData().Values.Sum();

            var customerData = this.customerProjectPanel.GetData();

            var customerMinutes = customerData.Values.Sum();

            var extraPerCustomerMinute = (double)commonMinutes / customerMinutes;

            var result = new List<ProjectInfo>();
            foreach (var originalCustomerPair in customerData)
            {
                var newMinutes = originalCustomerPair.Value + (originalCustomerPair.Value * extraPerCustomerMinute);

                var item = new ProjectInfo
                               {
                                   ProjectName = originalCustomerPair.Key,
                                   RealMinutes = originalCustomerPair.Value,
                                   MinutesWithCommon = (int)Math.Round(newMinutes)
                               };

                result.Add(item);
            }

            return result;
        }

        private void OnDataChanged()
        {
            if (false == this.supressDataChanged && this.DataChanged != null)
            {
                this.DataChanged(this, EventArgs.Empty);
            }
        }


        private bool supressDataChanged;

        public void SetData(IReadOnlyList<TimeRecord> data)
        {
            this.supressDataChanged = true;
            this.commonProjectPanel.SetRecords(data.Where(d => d.Common));
            this.customerProjectPanel.SetRecords(data.Where(d => false == d.Common));
            this.supressDataChanged = false;
            this.OnDataChanged();
        }
    }
}