using GraslandenBL.Domain;
using GraslandenBL.Enums;
using GraslandenBL.FactoryResults;

namespace GraslandenDL.Factories
{
    public static class SpeciesFactory
    {

        //public Species(int? id, string name, int moisture, int ph, int biodiversity, Rating? rating)
        //{
        //    Id = id;
        //    Name = name;
        //    Moisture = moisture;
        //    Ph = ph;
        //    Biodiversity = biodiversity;
        //    Rating = rating;
        //}
        public static FactoryResult<Species> CreateSpecies(string? idString, string name, string moistureString, string phString, string biodiversityString, string? ratingString)
        {
            List<string> errors = new List<string>();
            int? id = -1;
            int moisture = -1;
            int ph = -1;
            int biodiversity = -1;
            Rating? rating = null;
            try
            {
                if(idString != null) id = int.Parse(idString);
            }
            catch
            {
                errors.Add("ID moet een getal zijn.");
            }

            try
            {
                moisture = int.Parse(moistureString);
            }
            catch
            {
                errors.Add("Vochtgehalte moet een getal zijn.");
            }

            try
            {
                ph = int.Parse(phString);
            }
            catch
            {
                errors.Add("Zuurtegraad moet een getal zijn.");
            }

            // if (errors.Count == 0) return Species.Create(id, name, moisture, ph, biodiversity, rating);
            // else return new FactoryResult<Species>(errors);
        }
    }
}
