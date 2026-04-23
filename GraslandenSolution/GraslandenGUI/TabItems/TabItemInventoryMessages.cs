using GraslandenBL.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace GraslandenGUI.TabItems
{
    public class TabItemInventoryMessages : TabItem
    {
        private Window _window;
        public TabItemInventoryMessages(string inventoryName, Dictionary<string, MessageType> messages, Window window) 
        {
            _window = window;
            Grid contentGrid = CreateGrid(inventoryName);
            AddTitles(contentGrid);
            AddMessageLists(contentGrid, messages);
            Header = inventoryName;
            Content = contentGrid;
        }

        private Grid CreateGrid(string inventoryName)
        {
            Grid grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star)});
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star)});
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star)});
            return grid;
        }

        private void AddTitles(Grid grid)
        {
            TextBlock txtRemark = new TextBlock { Text  = "Meldingen", Margin = new Thickness(10), HorizontalAlignment = HorizontalAlignment.Center };
            TextBlock txtError = new TextBlock { Text  = "Fouten", Margin = new Thickness(10), HorizontalAlignment = HorizontalAlignment.Center };
            Grid.SetColumn(txtError, 0);
            Grid.SetColumn(txtRemark, 1);
            Grid.SetRow(txtRemark, 0);
            Grid.SetRow(txtError, 0);
            grid.Children.Add(txtRemark);
            grid.Children.Add(txtError);
        }

        private void AddMessageLists(Grid grid, Dictionary<string, MessageType> messages)
        {
            Thickness margin = new Thickness(10);
            Style ListItemStyle = (Style)_window.FindResource("ListBoxItemStyle");
            ListBox listBoxRemarks = new ListView { Margin = margin, ItemContainerStyle = ListItemStyle };
            ListBox listBoxErrors = new ListView { Margin = margin, };
            Grid.SetColumn(listBoxRemarks, 0);
            Grid.SetColumn(listBoxErrors, 1);
            Grid.SetRow(listBoxRemarks, 1);
            Grid.SetRow(listBoxErrors, 1);

            List<string> remarks = new List<string>();
            List<string> errors = new List<string>();

            foreach (KeyValuePair<string, MessageType> message in messages)
            {
                switch (message.Value)
                {
                    case MessageType.Remark: remarks.Add(message.Key); break;
                    case MessageType.Error: errors.Add(message.Key); break;
                }
            }
            listBoxRemarks.ItemsSource = remarks;
            listBoxErrors.ItemsSource = errors;
            grid.Children.Add(listBoxRemarks);
            grid.Children.Add(listBoxErrors);
        }
    }
}
