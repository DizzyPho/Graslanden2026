using GraslandenBL.Domain;
using GraslandenBL.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace GraslandenGUI.Models
{
    public class PlotUI : INotifyPropertyChanged
    {
        public PlotUI(string code, double areaSqMeters, string campus, ManagementType managementType, PlotType plotType)
        {
            Code = code;
            AreaSqMeters = areaSqMeters;
            Campus = campus;
            ManagementType = managementType;
            PlotType = plotType;
        }

        private string _code;
        private double _areaSqMeters;
        private string _campus;
        private ManagementType _managementType;
        private PlotType _plotType;

        public string Code { 
            get { return _code; }
            set { _code = value; OnPropertyChanged("Code"); } 
        }

        public double AreaSqMeters
        {
            get { return _areaSqMeters; }
            set { _areaSqMeters = value; OnPropertyChanged("AreaSqMeters"); }
        }

        public string Campus
        {
            get { return _campus; }
            set { _campus = value; OnPropertyChanged("Campus"); }
        }

        public ManagementType ManagementType
        {
            get { return _managementType; }
            set { _managementType = value; OnPropertyChanged("ManagementType"); }
        }

        public PlotType PlotType
        {
            get { return _plotType; }
            set { _plotType = value; OnPropertyChanged("PlotType"); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
