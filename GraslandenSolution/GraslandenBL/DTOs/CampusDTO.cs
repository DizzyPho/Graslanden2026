using GraslandenBL.Domain;

namespace GraslandenBL.DTOs
{
    public class CampusDTO
    {
        public string Name { get; set; }
        public List<Plot> Plots { get; set; }

        public Dictionary<string, PlotValue> PlotTypes { get; set; }

        public CampusDTO()
        {
            Plots = new List<Plot>();
            PlotTypes = new Dictionary<string, PlotValue>();
        }

        public CampusDTO(List<Plot> plots, Dictionary<string, PlotValue> plotTypes, string name)
        {
            Name = name;
            Plots = plots;
            PlotTypes = plotTypes;
        }
    }

    public record struct PlotValue
    {
        public PlotValue(int count, double totalAreaSqMeters)
        {
            Count = count;
            TotalAreaSqMeters = totalAreaSqMeters;
        }

        public int Count { get; set; }

        public double TotalAreaSqMeters { get; set; }
    }
}