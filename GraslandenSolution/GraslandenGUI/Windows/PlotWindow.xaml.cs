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
        private string CurrentPlotCode { get; init; }
        Manager _manager;
        public PlotWindow(Manager manager, Plot currentPlot, int currentInventoryId)
        {
            CurrentPlotCode = currentPlot.Code;
            CurrentInventoryId = currentInventoryId;
            _manager = manager;
            InitializeComponent();
            List<MeasurementDTO> measurements = manager.GetSpeciesOfPlot(currentPlot, currentInventoryId);
            Measurements = new ObservableCollection<MeasurementDTO>(measurements);
            ShowPlotInfo(measurements);
            TextBlockTitle.Text = $"Grasland {currentPlot.Code}";
            DataGridMeasurements.ItemsSource = Measurements;
        }

        private void ButtonBack(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ShowPlotInfo(List<MeasurementDTO> measurements)
        {
            int count = measurements.Count;
            double? avgMoisture = measurements.Average(m => m.Species.Moisture);
            double? avgPh = measurements.Average(m => m.Species.Ph);
            double? avgNitrogen = measurements.Average(m => m.Species.Nitrogen);
            double? avgNectar = measurements.Average(m => m.Species.Nectarvalue);
            double? avgBiodiversity = measurements.Average(m => m.Species.Biodiversity);

            TextBlock txtCount = new TextBlock { Text = count.ToString(), Margin = new Thickness(10, 2, 10, 2) };
            TextBlock txtMoisture = new TextBlock { Text = string.Format("{0:0.0}", avgMoisture), Margin = new Thickness(10, 2, 10, 2) };
            TextBlock txtPh = new TextBlock { Text = string.Format("{0:0.0}", avgPh), Margin = new Thickness(10, 2, 10, 2) };
            TextBlock txtNitrogen = new TextBlock { Text = string.Format("{0:0.0}", avgNitrogen), Margin = new Thickness(10, 2, 10, 2) };
            TextBlock txtNectar = new TextBlock { Text = string.Format("{0:0.0}", avgNectar), Margin = new Thickness(10, 2, 10, 2) };
            TextBlock txtBiodiversity = new TextBlock { Text = string.Format("{0:0.0}", avgBiodiversity), Margin = new Thickness(10, 2, 10, 2) };
            Grid.SetColumn(txtCount, 1);
            Grid.SetColumn(txtMoisture, 1);
            Grid.SetColumn(txtPh, 1);
            Grid.SetColumn(txtNitrogen, 3);
            Grid.SetColumn(txtNectar, 3);
            Grid.SetColumn(txtBiodiversity, 3);
            Grid.SetRow(txtCount, 0);
            Grid.SetRow(txtMoisture, 1);
            Grid.SetRow(txtPh, 2);
            Grid.SetRow(txtNitrogen, 0);
            Grid.SetRow(txtNectar, 1);
            Grid.SetRow(txtBiodiversity, 2);
            GridInfo.Children.Add(txtMoisture);
            GridInfo.Children.Add(txtPh);
            GridInfo.Children.Add(txtNitrogen);
            GridInfo.Children.Add(txtNectar);
            GridInfo.Children.Add(txtBiodiversity);
            GridInfo.Children.Add(txtCount);

            TextBlock moistureText = new TextBlock { Text = Plot.CalculateMoistureString(avgMoisture), Margin = new Thickness(10, 15, 10, 2) };
            TextBlock phText = new TextBlock { Text = Plot.CalculatePhString(avgPh), Margin = new Thickness(10, 2, 10, 2) };
            TextBlock nitrogenText = new TextBlock { Text = Plot.CalculateNitrogenString(avgNitrogen), Margin = new Thickness(10, 2, 10, 2) };
            Grid.SetColumn(moistureText, 0);
            Grid.SetColumn(phText, 0);
            Grid.SetColumn(nitrogenText, 0);
            Grid.SetRow(moistureText, 3);
            Grid.SetRow(phText, 4);
            Grid.SetRow(nitrogenText, 5);
            GridInfo.Children.Add(moistureText);
            GridInfo.Children.Add(phText);
            GridInfo.Children.Add(nitrogenText);
        }

        private void NewMeasurement_Click(object sender, RoutedEventArgs e)
        {
            NewMeasurementWindow nmw = new NewMeasurementWindow(_manager, CurrentPlotCode, CurrentInventoryId);
            nmw.ShowDialog();
            Measurements.Add(nmw.NewMeasurement);
        }
    }
}
