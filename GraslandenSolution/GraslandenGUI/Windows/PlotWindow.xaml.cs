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
    /// Interaction logic for PlotWindow.xaml
    /// </summary>
    public partial class PlotWindow : Window
    {

        private ObservableCollection<MeasurementDTO> Measurements { get; init; }
        private int CurrentInventoryId { get; init; }
        Manager _manager;
        public PlotWindow(Manager manager, Plot currentPlot, int currentInventoryId)
        {
            CurrentInventoryId = currentInventoryId;
            _manager = manager;
            InitializeComponent();
            Measurements = new ObservableCollection<MeasurementDTO>(_manager.GetSpeciesOfPlot(currentPlot, currentInventoryId));
            DataGridMeasurements.ItemsSource = Measurements;
        }

        private void ButtonBack(object sender, RoutedEventArgs e)
        {

        }
    }
}
