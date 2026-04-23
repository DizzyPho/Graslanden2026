using GraslandenBL.Domain;
using GraslandenBL.Enums;
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
    /// Interaction logic for AddPlotWindow.xaml
    /// </summary>
    public partial class AddPlotWindow : Window
    {
        public string Code { get; private set; }
        public ManagementType ManagementType { get; private set;  }
        public string PlotType { get; private set; }
        public AddPlotWindow()
        {
            InitializeComponent();
            ComboBoxManagementType.ItemsSource = new List<String> { "Intensief", "Extensief", "Schapenweide", "Netheidsboord" };
            ComboBoxManagementType.SelectedIndex = 0;
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            Code = TextBoxCode.Text;
            ManagementType = Plot.StringToManagementType(ComboBoxManagementType.Text);
            PlotType = TextBoxPlotType.Text;
            Close();
        }
    }
}
