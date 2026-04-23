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
        public TabItemInventoryMessages(string inventoryName, Dictionary<string, MessageType> messages) 
        {
            Grid contentGrid = CreateGrid(inventoryName);
            AddTitles(contentGrid);
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
    }
}
