using GraslandenBL.DTOs;
using Microsoft.Extensions.Configuration;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using GraslandenBL.Managers;
using GraslandenBL.Interfaces;
using GraslandenUtil.Factories;
using Microsoft.Win32;
using GraslandenGUI.Windows;

namespace GraslandenGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<InventoryDTO> Inventories { get; init; }
        private ImportManager _importManager;
        private Manager _manager;
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

            IFileReader fileReader = FileReaderFactory.CreateFileReader(indicatorValuesPath: indicatorValuesPath,
                                                                        fileType: importFileType);

            _importManager = new ImportManager(repository: repository,
                                                            fileReader: fileReader);
            _manager = new Manager(repository);
            Inventories = new ObservableCollection<InventoryDTO>(_manager.GetInventoryDTOs());
            ListBoxInventories.ItemsSource = Inventories;
        }

        private void ImportNew_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "TXT Files (*.txt)|*.txt|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = openFileDialog.FileName;
                if (!fileName.EndsWith(".txt"))
                {
                    MessageBox.Show("Selecteer a.u.b. een TXT-bestand.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    ProgressBarWindow progressBarWindow = new ProgressBarWindow();
                    progressBarWindow.Show();
                    Inventories.Add(_importManager.ImportData(fileName, DateTime.Now, "test"));
                    progressBarWindow.Close();
                    MessageBox.Show("Inventarisatie geïmporteerd!", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }      
        }

        private void AddNew_Click(object sender, RoutedEventArgs e)
        {
            NewInventoryWindow niw = new NewInventoryWindow();
            niw.ShowDialog();
            if(niw.Success)
            {
                int newInventoryId = _manager.ImportEmptyInventory(niw.Inventory);
                niw.Inventory.Id = newInventoryId;
                Inventories.Add(niw.Inventory);
            }
        }

        private void InspectInventory_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxInventories.SelectedItem == null)
            {
                return;
            }
            InventoryWindow iw = new InventoryWindow((InventoryDTO)ListBoxInventories.SelectedItem, _manager);
            iw.ShowDialog();
        }
    }
}