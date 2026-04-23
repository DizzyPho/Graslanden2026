using GraslandenBL.Domain;

namespace GraslandenBL.DTOs
{
    public class CampusDTO
    {
        public string Name { get; set; }
        public List<Plot> Plots { get; set; }

        public Dictionary<string, PlotTypeValue> PlotTypes { get; set; }

        public CampusDTO()
        {
            Plots = new List<Plot>();
            PlotTypes = new Dictionary<string, PlotTypeValue>();
        }

        public CampusDTO(List<Plot> plots, Dictionary<string, PlotTypeValue> plotTypes, string name)
        {
            Name = name;
            Plots = plots;
            PlotTypes = plotTypes;
        }
    }

    public record struct PlotTypeValue
    {
        public PlotTypeValue(int count, double totalAreaSqMeters)
        {
            Count = count;
            TotalAreaSqMeters = totalAreaSqMeters;
        }

        public int Count { get; set; }

        public double TotalAreaSqMeters { get; set; }
    }
}