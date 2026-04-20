using Microsoft.Extensions.Configuration;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GraslandenBL.Managers;
using GraslandenBL.Interfaces;
using GraslandenUtil.Factories;

namespace GraslandenGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("./Config/appsettings.json", optional: false, reloadOnChange: true);

            var config = builder.Build();

            string dbConnectionString = config.GetConnectionString("SQLServerConnection");
            string indicatorValuesPath = config.GetSection("AppSettings")["IndicatorValuesPath"];
            string importFileType = config.GetSection("AppSettings")["ImportFileType"];
            string DBType = config.GetSection("AppSettings")["DataBaseType"];

            IRepository repository = RepositoryFactory.CreateRepository(connectionString: dbConnectionString,
                                                                        databaseType: DBType);

            IFileReader fileReader = FileReaderFactory.CreateFileReader(inventoryFilePath: "",
                                                                        indicatorValuesPath: indicatorValuesPath,
                                                                        fileType: importFileType);

            ImportManager importManager = new ImportManager(repository: repository,
                                                            fileReader: fileReader);

            importManager.ReadFile();
        }
    }
}