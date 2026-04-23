using GraslandenBL.Enums;
using GraslandenBL.Managers;
using GraslandenGUI.TabItems;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for LogWindow.xaml
    /// </summary>
    public partial class LogWindow : Window
    {
        private Manager _manager;
        public LogWindow(Manager manager)
        {
            InitializeComponent();
            _manager = manager;
            _manager.GetAllMessages();
            foreach (KeyValuePair<String, Dictionary<String, MessageType>> inventoryMessages in _manager.GetAllMessages())
            {
                TabControlErrors.Items.Add(new TabItemInventoryMessages(inventoryMessages.Key, inventoryMessages.Value, this));
            }
        }
    }
}
