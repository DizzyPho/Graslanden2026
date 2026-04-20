using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace GraslandenGUI.Models
{
    public class MeasurementUI : INotifyPropertyChanged
    {
        private string _coverage;
        public string Coverage
        {
            get { return _coverage; }
            set { _coverage = value; OnPropertyChanged("Coverage"); }
        }


        public int SpeciesId { get; set; }
        public MeasurementUI(int speciesid, string coverage)
        {
            SpeciesId = speciesid;
            Coverage = coverage;
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}