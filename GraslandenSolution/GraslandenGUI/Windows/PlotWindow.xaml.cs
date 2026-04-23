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
        private TextBlock TextBlockCount { get; set; }
        private TextBlock TextBlockMoisture { get; set; }
        private TextBlock TextBlockPh { get; set; }
        private TextBlock TextBlockNitrogen { get; set; }
        private TextBlock TextBlockNectar { get; set; }
        private TextBlock TextBlockBiodiversity { get; set; }
        private TextBlock TextBlockMoistureComment { get; set; }
        private TextBlock TextBlockPhComment{ get; set; }
        private TextBlock TextBlockNitrogenComment { get; set; }
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
            Thickness marginNumberBlocks = new Thickness(10, 2, 10, 2);
            
            TextBlockCount = new TextBlock { Margin = marginNumberBlocks };
            TextBlockMoisture = new TextBlock { Margin = marginNumberBlocks };
            TextBlockPh = new TextBlock { Margin = marginNumberBlocks };
            TextBlockNitrogen = new TextBlock { Margin = marginNumberBlocks };
            TextBlockNectar = new TextBlock { Margin = marginNumberBlocks };
            TextBlockBiodiversity = new TextBlock { Margin = marginNumberBlocks };

            Grid.SetColumn(TextBlockCount, 1);
            Grid.SetColumn(TextBlockMoisture, 1);
            Grid.SetColumn(TextBlockPh, 1);
            Grid.SetColumn(TextBlockNitrogen, 3);
            Grid.SetColumn(TextBlockNectar, 3);
            Grid.SetColumn(TextBlockBiodiversity, 3);
            Grid.SetRow(TextBlockCount, 0);
            Grid.SetRow(TextBlockMoisture, 1);
            Grid.SetRow(TextBlockPh, 2);
            Grid.SetRow(TextBlockNitrogen, 0);
            Grid.SetRow(TextBlockNectar, 1);
            Grid.SetRow(TextBlockBiodiversity, 2);
            GridInfo.Children.Add(TextBlockMoisture);
            GridInfo.Children.Add(TextBlockPh);
            GridInfo.Children.Add(TextBlockNitrogen);
            GridInfo.Children.Add(TextBlockNectar);
            GridInfo.Children.Add(TextBlockBiodiversity);
            GridInfo.Children.Add(TextBlockCount);

            Thickness marginCommentBlock = new Thickness(10, 15, 10, 2);
            TextBlockMoistureComment = new TextBlock { Margin = marginCommentBlock };
            TextBlockPhComment = new TextBlock { Margin = marginCommentBlock };
            TextBlockNitrogenComment = new TextBlock { Margin = marginCommentBlock };
            Grid.SetColumn(TextBlockMoistureComment, 0);
            Grid.SetColumn(TextBlockPhComment, 0);
            Grid.SetColumn(TextBlockNitrogenComment, 0);
            Grid.SetRow(TextBlockMoistureComment, 3);
            Grid.SetRow(TextBlockPhComment, 4);
            Grid.SetRow(TextBlockNitrogenComment, 5);
            GridInfo.Children.Add(TextBlockMoistureComment);
            GridInfo.Children.Add(TextBlockPhComment);
            GridInfo.Children.Add(TextBlockNitrogenComment);

            RefreshPlotInfo(measurements);
        }

        private void RefreshPlotInfo(IEnumerable<MeasurementDTO> measurements)
        {
            int count = measurements.Count();
            double? avgMoisture = measurements.Average(m => m.Species.Moisture);
            double? avgPh = measurements.Average(m => m.Species.Ph);
            double? avgNitrogen = measurements.Average(m => m.Species.Nitrogen);
            double? avgNectar = measurements.Average(m => m.Species.Nectarvalue);
            double? avgBiodiversity = measurements.Average(m => m.Species.Biodiversity);

            TextBlockCount.Text = count.ToString();
            TextBlockMoisture.Text = string.Format("{0:0.0}", avgMoisture);
            TextBlockPh.Text = string.Format("{0:0.0}", avgPh);
            TextBlockNitrogen.Text = string.Format("{0:0.0}", avgNitrogen);
            TextBlockNectar.Text = string.Format("{0:0.0}", avgNectar);
            TextBlockBiodiversity.Text = string.Format("{0:0.0}", avgBiodiversity);

            TextBlockMoistureComment.Text = Plot.CalculateMoistureString(avgMoisture);
            TextBlockPhComment.Text = Plot.CalculatePhString(avgPh);
            TextBlockNitrogenComment.Text = Plot.CalculateNitrogenString(avgNitrogen);

        }

        private void NewMeasurement_Click(object sender, RoutedEventArgs e)
        {
            NewMeasurementWindow nmw = new NewMeasurementWindow(_manager, CurrentPlotCode, CurrentInventoryId);
            nmw.ShowDialog();
            if (nmw.NewMeasurement != null) 
            { 
                Measurements.Add(nmw.NewMeasurement);
                RefreshPlotInfo(Measurements);
            }
        }

        private void DeleteMeasurement_Click(object sender, RoutedEventArgs e)
        {
            if(DataGridMeasurements.SelectedItem == null)
            {
                return;
            }
            MeasurementDTO selectedMeasurement = (MeasurementDTO)DataGridMeasurements.SelectedItem;
            _manager.DeleteMeasurement(selectedMeasurement.Id);
            Measurements.Remove(selectedMeasurement);
            RefreshPlotInfo(Measurements);
        }
    }
}
