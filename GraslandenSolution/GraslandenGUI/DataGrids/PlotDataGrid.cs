using GraslandenBL.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace GraslandenGUI.DataGrids
{
    public class PlotDataGrid : DataGrid
    {
        public PlotDataGrid(ObservableCollection<Plot> itemsSource)
        {
            AutoGenerateColumns = false;
            Columns.Add(new DataGridTextColumn { Header = "Code", Binding = new Binding("Code") });
            Columns.Add(new DataGridTextColumn { Header = "Oppervlakte (m²)", Binding = new Binding("AreaSqMeters") });
            Columns.Add(new DataGridTextColumn { Header = "Beheertype", Binding = new Binding("ManagementType") });
            Columns.Add(new DataGridTextColumn { Header = "Graslandtype", Binding = new Binding("PlotType") });
            ItemsSource = itemsSource;
        }
    }
}
