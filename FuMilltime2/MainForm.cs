namespace FuMilltime2
{
    using System.Windows.Forms;

    using FuMilltime2.DataBase;

    public partial class MainForm : Form
    {
        private readonly IFuMilltimeDataSource datasource = new LogFileDataStorage();

        public MainForm()
        {
            this.InitializeComponent();
            this.AutoSize = true;
            SetWindowTitle();
            LoadRecordsFromFile();

            DateManager.DateChanged += (_, _) =>
            {
                SetWindowTitle();
                LoadRecordsFromFile();
            };

            this.projectInputPanel.DataChanged +=
                (_, _) => this.resultGrid.SetData(this.projectInputPanel.GetData());
            
            this.projectInputPanel.DataChanged +=
                (_, _) => this.datasource.SaveRecords(this.projectInputPanel.GetRecords());


        }

        private void SetWindowTitle()
        {
            var dayText = DateManager.CurrentDate == DateOnly.FromDateTime(DateTime.Today)
                   ? "I dag"
                   : DateManager.CurrentDate.ToString("d MMMM yyyy");

            this.Text = "FU Milltime 2 - " + dayText;
        }
        private void LoadRecordsFromFile()
        {
            var currentData = this.datasource.GetRecords();
            if (currentData != null)
            {
                this.projectInputPanel.SetData(currentData);
            }
        }
    }
}
