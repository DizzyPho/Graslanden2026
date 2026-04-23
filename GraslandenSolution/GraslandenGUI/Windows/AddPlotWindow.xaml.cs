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
        public AddPlotWindow()
        {
            InitializeComponent();
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
