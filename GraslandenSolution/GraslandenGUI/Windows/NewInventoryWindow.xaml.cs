using GraslandenBL.Domain;
using GraslandenBL.DTOs;
using GraslandenValidation;
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
    /// Interaction logic for NewInventoryWindow.xaml
    /// </summary>
    public partial class NewInventoryWindow : Window
    {
        public InventoryDTO Inventory { get; private set; }
        public bool Success { get => Inventory is not null; }
        public NewInventoryWindow()
        {
            InitializeComponent();
            DatePicker.SelectedDate = DateTime.Now;
        }

        private void NewInventory_Click(object sender, RoutedEventArgs e)
        {
            DateTime selectedDate = (DateTime)DatePicker.SelectedDate;
            if (InventoryValidation.Validate(selectedDate, TextBoxName.Text, out List<String> errors))
            {
                Inventory = new InventoryDTO(selectedDate, TextBoxName.Text);
                Close();
            }
            else
            {
                MessageBox.Show(string.Join('\n',errors), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
