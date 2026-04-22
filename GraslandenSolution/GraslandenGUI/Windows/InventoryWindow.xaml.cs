using GraslandenBL.Domain;
using GraslandenBL.DTOs;
using GraslandenBL.Managers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GraslandenGUI.Windows
{
    /// <summary>
    /// Interaction logic for InventoryWindow.xaml
    /// </summary>
    public partial class InventoryWindow : Window
    {
        Manager _manager;
        ObservableCollection<Plot> Plots { get; set; }
        private InventoryDTO CurrentInventory { get; init; }
        public InventoryWindow(InventoryDTO inventoryDTO, Manager manager)
        {
            InitializeComponent();
            CurrentInventory = inventoryDTO;
            TextBlockTitle.Text = $"Inventarisatie '{inventoryDTO.ToString()}'";
            _manager = manager;
            HashSet<String> campuses = _manager.GetAllCampuses();
            foreach (String campus in campuses)
            {
                TabItem tabItem = new TabItem { Header = campus , Name = campus };
                TabControlCampus.Items.Add(tabItem);
            }
        }

        private void ButtonInspectPlot(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonBack(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CampusDTO selectedCampus = _manager.GetCampus(CurrentInventory.Id, ((TabItem)TabControlCampus.SelectedItem).Name);
            Plots = new ObservableCollection<Plot>(selectedCampus.Plots);
        }
    }
}
