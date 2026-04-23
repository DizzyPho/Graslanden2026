using GraslandenBL.Domain;
using GraslandenBL.DTOs;
using GraslandenBL.Interfaces;
using GraslandenBL.Managers;
using GraslandenUtil.Factories;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for NewMeasurementWindow.xaml
    /// </summary>
    public partial class NewMeasurementWindow : Window
    {
        private Manager _manager;
        private string _plotCode;
        private int _inventoryId;
        public MeasurementDTO NewMeasurement { get; init; }

        public NewMeasurementWindow(Manager manager, string plotCode, int inventoryId)
        {
            InitializeComponent();

            _manager = manager;
            _plotCode = plotCode;
            _inventoryId = inventoryId;
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            MeasurementDTO measurement = _manager.InsertMeasurement(plotCode: _plotCode, species: TextBoxSpecies.Text, coverage: TextBoxCoverage.Text, inventoryId: _inventoryId);
            if(measurement != null)
            {
                Close();
            }
            else
            {
                MessageBox.Show("Plantensoort werd niet gevonden.", "Toevoeging gefaald", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
