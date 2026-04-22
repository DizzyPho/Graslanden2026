using GraslandenBL.Domain;
using GraslandenBL.DTOs;
using GraslandenBL.Managers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
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
        private Dictionary<String, CampusDTO> CampusInfo { get; set; }
        private InventoryDTO CurrentInventory { get; init; }
        public InventoryWindow(InventoryDTO inventoryDTO, Manager manager)
        {
            InitializeComponent();
            CurrentInventory = inventoryDTO;
            CampusInfo = new Dictionary<String, CampusDTO>();
            TextBlockTitle.Text = $"Inventarisatie '{inventoryDTO.ToString()}'";
            _manager = manager;
            HashSet<String> campuses = _manager.GetAllCampuses();
            foreach (String campus in campuses)
            {
                CampusDTO selectedCampus = _manager.GetCampus(CurrentInventory.Id, campus);
                CampusInfo[campus] = selectedCampus;
                DataGrid dataGridPlots = new DataGrid { ItemsSource = selectedCampus.Plots };
                TabItem tabItem = new TabItem 
                {
                    Header = campus, Name = campus, Content = new DataGrid 
                    { 
                        ItemsSource = selectedCampus.Plots
                    } 
                };
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
            if(e.Source == TabControlCampus)
            {
                FillCampusInfo(CampusInfo[((TabItem)TabControlCampus.SelectedItem).Name]);
            }
        }

        private void FillCampusInfo(CampusDTO campusDTO)
        {
            GridCampusInfo.Children.Clear();
            TextBlock txtTypeTitle = new TextBlock { Text = "Graslandsoort", Margin = new Thickness(10, 0, 10, 0) };
            TextBlock txtCountTitle = new TextBlock { Text = "Aantal Graslanden", Margin = new Thickness(10, 0, 10, 0) };
            TextBlock txtAreaTitle = new TextBlock { Text = "Oppervlakte (m²)", Margin = new Thickness(10, 0, 10, 0) };
            Grid.SetColumn(txtTypeTitle, 0);
            Grid.SetRow(txtTypeTitle, 0);
            Grid.SetColumn(txtCountTitle, 1);
            Grid.SetRow(txtCountTitle, 0);
            Grid.SetColumn(txtAreaTitle, 3);
            Grid.SetRow(txtAreaTitle, 0);
            GridCampusInfo.Children.Add(txtTypeTitle);
            GridCampusInfo.Children.Add(txtCountTitle);
            GridCampusInfo.Children.Add(txtAreaTitle);
            int currentRow = 1;
            foreach(KeyValuePair<string, PlotValue> kvp in campusDTO.PlotTypes)
            {
                GridCampusInfo.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                TextBlock txtType = new TextBlock { Text = kvp.Key, Margin = new Thickness(10,0,10,0) };
                Grid.SetColumn(txtType, 0);
                Grid.SetRow(txtType, currentRow);
                TextBlock txtCount = new TextBlock { Text = kvp.Value.Count.ToString(), Margin = new Thickness(10, 0, 10, 0) };
                Grid.SetColumn(txtCount, 1);
                Grid.SetRow(txtCount, currentRow);
                TextBlock txtArea = new TextBlock { Text = kvp.Value.TotalAreaSqMeters.ToString(), Margin = new Thickness(10, 0, 10, 0) };
                Grid.SetColumn(txtArea, 2);
                Grid.SetRow(txtArea, currentRow);
                currentRow++;
                GridCampusInfo.Children.Add(txtType);
                GridCampusInfo.Children.Add(txtCount);
                GridCampusInfo.Children.Add(txtArea);
            }
        }
    }
}
