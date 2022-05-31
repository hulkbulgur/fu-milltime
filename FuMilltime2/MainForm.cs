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
            LoadRecordsFromFile();

            DateManager.DateChanged += (_, _) => LoadRecordsFromFile();

            this.projectInputPanel.DataChanged +=
                (sender, args) => this.resultGrid.SetData(this.projectInputPanel.GetData());
            
            this.projectInputPanel.DataChanged +=
                (sender, args) => this.datasource.SaveRecords(this.projectInputPanel.GetRecords());
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
