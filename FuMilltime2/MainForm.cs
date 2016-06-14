namespace FuMilltime2
{
    using System;
    using System.Windows.Forms;

    using FuMilltime2.DataBase;

    public partial class MainForm : Form
    {
        private readonly DateTime currentDate = DateTime.Now.Date;

        private readonly IFuMilltimeDataSource datasource = new LogFileDataStorage();

        public MainForm()
        {
            this.InitializeComponent();

            this.projectInputPanel.DataChanged +=
                (sender, args) => this.resultGrid.SetData(this.projectInputPanel.GetData());

            var currentData = this.datasource.GetRecords(this.currentDate);
            if (currentData != null)
            {
                this.projectInputPanel.SetData(currentData);
            }

            this.AutoSize = true;

            this.projectInputPanel.DataChanged +=
                (sender, args) => this.datasource.SaveRecords(this.currentDate, this.projectInputPanel.GetRecords());
        }
    }
}
