using GraslandenBL.Domain;
using GraslandenBL.DTOs;
using GraslandenBL.Enums;
using GraslandenBL.Managers;
using GraslandenGUI.DataGrids;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        TabItem _selectedTabItem;
        private Dictionary<String, CampusDTO> CampusInfo { get; set; }
        private InventoryDTO CurrentInventory { get; init; }
        public InventoryWindow(InventoryDTO inventoryDTO, Manager manager)
        {
            InitializeComponent();
            CurrentInventory = inventoryDTO;
            CampusInfo = new Dictionary<String, CampusDTO>();
            TextBlockTitle.Text = $"Inventarisatie '{inventoryDTO.ToString()}'";
            _manager = manager;
            List<CampusDTO> campuses = _manager.GetAllCampusesDTO(inventoryDTO.Id);
            foreach (CampusDTO campus in campuses)
            {
                CampusInfo[campus.Name] = campus;
                TabItem tabItem = new TabItem
                {
                   Header = campus.Name, Name = campus.Name, Content = new PlotDataGrid(campus.Plots)
                };
                TabControlCampus.Items.Add(tabItem);
            }
            FillCampusInfo(campuses[0]);
        }

        private void ButtonInspectPlot(object sender, RoutedEventArgs e)
        { 
            PlotWindow pw = new PlotWindow(_manager, (Plot)((DataGrid)_selectedTabItem.Content).SelectedItem, CurrentInventory.Id);
            pw.Show();
        }

        private void ButtonBack(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedTabItem = ((TabItem)TabControlCampus.SelectedItem);
            if (e.Source == TabControlCampus)
            {
                FillCampusInfo(CampusInfo[_selectedTabItem.Name]);
            }
        }

        private void FillCampusInfo(CampusDTO campusDTO)
        {

            FillTitles();
            int currentRow = 1;
            foreach(KeyValuePair<string, PlotTypeValue> kvp in campusDTO.PlotTypes)
            {
                GridCampusInfoPlotTypes.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
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
                GridCampusInfoPlotTypes.Children.Add(txtType);
                GridCampusInfoPlotTypes.Children.Add(txtCount);
                GridCampusInfoPlotTypes.Children.Add(txtArea);
            }
            currentRow = 1;
            foreach(KeyValuePair<ManagementType, ManagementTypeValue> kvp in campusDTO.ManagementTypes)
            {
                GridCampusInfoManagementTypes.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                TextBlock txtType = new TextBlock { Text = kvp.Key.ToString(), Margin = new Thickness(10, 0, 10, 0) };
                Grid.SetColumn(txtType, 0);
                Grid.SetRow(txtType, currentRow);
                TextBlock txtCount = new TextBlock { Text = kvp.Value.Count.ToString(), Margin = new Thickness(10, 0, 10, 0) };
                Grid.SetColumn(txtCount, 1);
                Grid.SetRow(txtCount, currentRow);
                TextBlock txtArea = new TextBlock { Text = kvp.Value.TotalAreaSqMeters.ToString(), Margin = new Thickness(10, 0, 10, 0) };
                Grid.SetColumn(txtArea, 2);
                Grid.SetRow(txtArea, currentRow);
                currentRow++;
                GridCampusInfoManagementTypes.Children.Add(txtType);
                GridCampusInfoManagementTypes.Children.Add(txtCount);
                GridCampusInfoManagementTypes.Children.Add(txtArea);
            }
        }

        private void FillTitles()
        {
            {
                GridCampusInfoPlotTypes.Children.Clear();
                TextBlock txtTypeTitle = new TextBlock { Text = "Graslandsoort", Margin = new Thickness(10, 0, 10, 0) };
                TextBlock txtCountTitle = new TextBlock { Text = "Aantal Graslanden", Margin = new Thickness(10, 0, 10, 0) };
                TextBlock txtAreaTitle = new TextBlock { Text = "Oppervlakte (m²)", Margin = new Thickness(10, 0, 10, 0) };
                Grid.SetColumn(txtTypeTitle, 0);
                Grid.SetRow(txtTypeTitle, 0);
                Grid.SetColumn(txtCountTitle, 1);
                Grid.SetRow(txtCountTitle, 0);
                Grid.SetColumn(txtAreaTitle, 3);
                Grid.SetRow(txtAreaTitle, 0);
                GridCampusInfoPlotTypes.Children.Add(txtTypeTitle);
                GridCampusInfoPlotTypes.Children.Add(txtCountTitle);
                GridCampusInfoPlotTypes.Children.Add(txtAreaTitle);
            }
            {
                GridCampusInfoManagementTypes.Children.Clear();
                TextBlock txtTypeTitle = new TextBlock { Text = "Graslandsoort", Margin = new Thickness(10, 0, 10, 0) };
                TextBlock txtCountTitle = new TextBlock { Text = "Aantal Graslanden", Margin = new Thickness(10, 0, 10, 0) };
                TextBlock txtAreaTitle = new TextBlock { Text = "Oppervlakte (m²)", Margin = new Thickness(10, 0, 10, 0) };
                Grid.SetColumn(txtTypeTitle, 0);
                Grid.SetRow(txtTypeTitle, 0);
                Grid.SetColumn(txtCountTitle, 1);
                Grid.SetRow(txtCountTitle, 0);
                Grid.SetColumn(txtAreaTitle, 3);
                Grid.SetRow(txtAreaTitle, 0);
                GridCampusInfoManagementTypes.Children.Add(txtTypeTitle);
                GridCampusInfoManagementTypes.Children.Add(txtCountTitle);
                GridCampusInfoManagementTypes.Children.Add(txtAreaTitle);
            }

        }

        private void AddPlot_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
