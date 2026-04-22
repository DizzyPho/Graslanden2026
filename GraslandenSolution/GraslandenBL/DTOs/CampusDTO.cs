using GraslandenBL.Domain;

namespace GraslandenBL.DTOs
{
    public class CampusDTO
    {
        public List<Plot> Plots { get; set; }

        public Dictionary<string, PlotValue> PlotTypes { get; set; }

        public CampusDTO()
        {
            Plots = new List<Plot>();
            PlotTypes = new Dictionary<string, PlotValue>();
        }

        public CampusDTO(List<Plot> plots, Dictionary<string, PlotValue> plotTypes)
        {
            Plots = plots;
            PlotTypes = plotTypes;
        }
    }

    public record struct PlotValue
    {
        public int SpeciesCount { get; set; }

        public double TotalAreaSqMeters { get; set; }
    }
}