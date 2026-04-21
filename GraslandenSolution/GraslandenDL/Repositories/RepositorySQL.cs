using GraslandenBL.Domain;
using GraslandenBL.DTOs;
using GraslandenBL.Enums;
using GraslandenBL.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection.Metadata;
using System.Xml.Linq;

namespace GraslandenDL.Repositories
{
    public class RepositorySQL : IRepository
    {
        private string _connectionString;

        public RepositorySQL(string connectionString)
        {
            _connectionString = connectionString;
        }

        public HashSet<string> GetAllCampuses()
        {
            HashSet<string> campuses = new HashSet<string>();
            string queryCampus = "SELECT campus FROM grass_plot";

            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmdCampus = new SqlCommand(queryCampus, con))
            {

                //Open connection
                con.Open();

                using (SqlDataReader reader = cmdCampus.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        campuses.Add(reader.GetString("campus"));
                    }
                    return campuses;
                }
            }
        }

        public Dictionary<Plot, string> GetAllGrassPlots()
        {
            Dictionary<Plot, string> grassPlots = new Dictionary<Plot, string>();
            return grassPlots;
        }

        public List<Species> GetAllSpecies()
        {

            List<Species> species = new List<Species>();

            string query = "SELECT id, name, rating, moisture, ph, nitrogen, nectar_production, biodiversity_relevance FROM species";

            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                try
                {
                    //Open connection
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string name = reader.GetString(1);
                            string rating = reader.GetString(2);
                            int moisture = reader.GetInt32(3);
                            int ph = reader.GetInt32(4);
                            int nitrogen = reader.GetInt32(5);
                            int biodiversity;
                            if (DBNull.Value.Equals(reader[7]))
                            {
                                biodiversity = 0;
                            }
                            else
                            {
                                biodiversity = reader.GetInt32(7);
                            }

                            int nectarValue;
                            if (DBNull.Value.Equals(reader[6]))
                            {
                                nectarValue = 0;
                            }
                            else
                            {
                                nectarValue = reader.GetInt32(6);
                            }


                            Species s = new Species(
                             id,
                             name,
                             moisture,
                             ph,
                             nitrogen,
                             nectarValue,
                             biodiversity,

                             //Sleutel, Begeleidend, Algemeen, Ruderaal, Invasief
                             rating switch
                             {
                                 "Sleutel" => Rating.Sleutel,
                                 "Begeleidend" => Rating.Begeleidend,
                                 "Algemeen" => Rating.Algemeen,
                                 "Ruderaal" => Rating.Ruderaal,
                                 "Invasief" => Rating.Invasief,
                                 _ => null
                             }
                             );
                            species.Add(s);
                        }

                        return species;
                    }
                }
                catch (Exception)
                {

                    throw;
                }


            }
        }


        public void ImportInventory(List<Inventory> data)
        {
            string queryInventory = "INSERT INTO inventory(id, date,name) output INSERTED.ID VALUES(@id,@date,@name)";
            string querySpecies = "INSERT INTO species(id, name, rating, moisture, ph, nitrogen, nectar_production, biodiversity) output INSERTED.ID VALUES(@id, @name, @rating, @moisture, @ph, @nitrogen, @nectar_production, @biodiversit)";
            string queryGrassPlot = "INSERT INTO plot(code, campus, area_sq_meter) output INSERTED.ID VALUES(@code, @campus, @area_sq_meter)";
            string queryManagementType = "INSERT INTO management_type(id, type) output INSERTED.ID VALUES(@id, @type)";
            string queryPlotType = "INSERT INTO plot_type(code, description) output INSERTED.ID VALUES(@code, @description)";
            string queryInventoried_plot = "INSERT INTO inventoried_plot(id,inventory_id,plot_code,management_type, plot_type) VALUES(@id, @inventory_id, @plot_code, @management_type, @plot_type)";
            string queryMeasurement = "INSERT INTO measurement(inventoried_plot_id, species_id, coverage) VALUES(@idInventory,@idSpecies,@coverage)";

            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmdInventory = con.CreateCommand())
            using (SqlCommand cmdSpecies = con.CreateCommand())
            using (SqlCommand cmdGrassPlot = con.CreateCommand())
            using (SqlCommand cmdManagementType = con.CreateCommand())
            using (SqlCommand cmdPlotType = con.CreateCommand())
            using (SqlCommand cmdInventoried_plot = con.CreateCommand())
            using (SqlCommand cmdMeasurement = con.CreateCommand())
            {
                //Open connection and transaction
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();
                cmdInventory.Transaction = transaction;


                //Parameters for inventory
                cmdInventory.CommandText = queryInventory;
                cmdInventory.Parameters.Add(new SqlParameter("@id", SqlDbType.NVarChar));
                cmdInventory.Parameters.Add(new SqlParameter("@date", SqlDbType.NVarChar));
                cmdInventory.Parameters.Add(new SqlParameter("@name", SqlDbType.NVarChar));

                //Parameters for Species
                cmdSpecies.CommandText = querySpecies;
                cmdSpecies.Parameters.Add(new SqlParameter("@id", SqlDbType.Int));
                cmdSpecies.Parameters.Add(new SqlParameter("@name", SqlDbType.NVarChar));
                cmdSpecies.Parameters.Add(new SqlParameter("@rating", SqlDbType.NVarChar));
                cmdSpecies.Parameters.Add(new SqlParameter("@moisture", SqlDbType.Int));
                cmdSpecies.Parameters.Add(new SqlParameter("@ph", SqlDbType.Int));
                cmdSpecies.Parameters.Add(new SqlParameter("@nitrogen", SqlDbType.Int));
                cmdSpecies.Parameters.Add(new SqlParameter("@nectar_production", SqlDbType.Int));
                cmdSpecies.Parameters.Add(new SqlParameter("@biodiversity_relevance", SqlDbType.Int));

                //Parameters for Grass Plot
                cmdGrassPlot.CommandText = queryGrassPlot;
                cmdGrassPlot.Parameters.Add(new SqlParameter("@code", SqlDbType.NVarChar));
                cmdGrassPlot.Parameters.Add(new SqlParameter("@campus", SqlDbType.NVarChar));
                cmdGrassPlot.Parameters.Add(new SqlParameter("area_sq_meter", SqlDbType.Float));

                //Parameters for Management Type
                cmdManagementType.CommandText = queryManagementType;
                cmdManagementType.Parameters.Add(new SqlParameter("@id", SqlDbType.Int));
                cmdManagementType.Parameters.Add(new SqlParameter("@type", SqlDbType.NVarChar));

                //Parameters for Plot Type
                cmdPlotType.CommandText = queryPlotType;
                cmdPlotType.Parameters.Add(new SqlParameter("@code", SqlDbType.NVarChar));
                cmdPlotType.Parameters.Add(new SqlParameter("@description", SqlDbType.NVarChar));

                //Parameters for Inventoried Plot
                cmdInventoried_plot.CommandText = queryInventoried_plot;
                cmdInventoried_plot.Parameters.Add(new SqlParameter("@id", SqlDbType.Int));
                cmdInventoried_plot.Parameters.Add(new SqlParameter("@inventory_id", SqlDbType.Int));
                cmdInventoried_plot.Parameters.Add(new SqlParameter("@plot_code", SqlDbType.NVarChar));
                cmdInventoried_plot.Parameters.Add(new SqlParameter("@management_type", SqlDbType.Int));
                cmdInventoried_plot.Parameters.Add(new SqlParameter("@plot_type", SqlDbType.NVarChar));


                //Parameters for Measurment
                cmdMeasurement.CommandText = queryMeasurement;
                cmdMeasurement.Parameters.Add(new SqlParameter("@idInventoriedPlotId", SqlDbType.Int));
                cmdMeasurement.Parameters.Add(new SqlParameter("@idSpecies", SqlDbType.Int));
                cmdMeasurement.Parameters.Add(new SqlParameter("@coverage", SqlDbType.NVarChar));

                try
                {
                    foreach (Inventory inventory in data)
                    {
                        //Insert inventory
                        cmdInventory.Parameters["@name"].Value = inventory.Name;
                        cmdInventory.Parameters["@date"].Value = inventory.Date;
                        //Get Inventory id
                        int inventoryId = (int)cmdInventory.ExecuteScalar();
                        cmdInventory.Parameters["@id"].Value = inventoryId;


                        foreach (var item in inventory.Measurements)
                        {

                            //Insert species
                            cmdSpecies.Parameters["@name"].Value = item.Species.Name;
                            cmdSpecies.Parameters["@rating"].Value = item.Species.Rating;
                            cmdSpecies.Parameters["@moisture"].Value = item.Species.Moisture;
                            cmdSpecies.Parameters["@ph"].Value = item.Species.Ph;
                            cmdSpecies.Parameters["@nitrogen"].Value = item.Species.Nitrogen;
                            cmdSpecies.Parameters["@nectar_production"].Value = item.Species.Nectarvalue;
                            cmdSpecies.Parameters["@biodiversity"].Value = item.Species.Biodiversity;


                            //Get Species id
                            int idSpecies = (int)cmdSpecies.ExecuteScalar();
                            cmdSpecies.Parameters["@id"].Value = idSpecies;

                            //Insert grass pLot
                            cmdGrassPlot.Parameters["@code"].Value = item.Plot.Code;
                            cmdGrassPlot.Parameters["@campus"].Value = item.Plot.Campus;
                            cmdGrassPlot.Parameters["@area_sq_meter"].Value = item.Plot.AreaSqMeters;

                            //Insert ManagementType
                            cmdManagementType.Parameters["@type"].Value = item.Plot.ManagementType;
                            //Get ManagementType id
                            int idManagementType = (int)cmdManagementType.ExecuteScalar();
                            cmdManagementType.Parameters["@id"].Value = item.Plot.Code;

                            //Insert Plot type 
                            cmdPlotType.Parameters["@code"].Value = item.Plot.PlotType;
                            cmdPlotType.Parameters["@description"].Value = item.Plot.PlotType;

                            //Insert Inventoried plot
                            cmdInventoried_plot.Parameters["@inventory_id"].Value = inventoryId;
                            cmdInventoried_plot.Parameters["@plot_code"].Value = item.Plot.Code;
                            cmdInventoried_plot.Parameters["@management_type"].Value = idManagementType;
                            cmdInventoried_plot.Parameters["@plot_type"].Value = item.Plot.PlotType;
                            //Get Inventoried_plot id
                            int idInventoried_plot = (int)cmdInventoried_plot.ExecuteScalar();
                            cmdInventoried_plot.Parameters["@id"].Value = idInventoried_plot;


                            //Now that you have species id and inventoried plot id ->
                            //Insert measurement
                            cmdMeasurement.Parameters["@idInventoriedPlotId"].Value = idInventoried_plot;
                            cmdMeasurement.Parameters["@idSpecies"].Value = idSpecies;
                            cmdMeasurement.Parameters["@coverage"].Value = item.Coverage;
                        }
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }

                transaction.Commit();
            }
        }


        public List<InventoryDTO> GetInventoryDTOs()
        {
            const string query = "SELECT date, name FROM inventory";
            List<InventoryDTO> inventories = new List<InventoryDTO>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = query;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    inventories.Add(new InventoryDTO(reader.GetDateTime(0), reader.GetString(1)));
                }
            }
            return inventories;
        }

        public void ImportEmptyInventory(InventoryDTO inventoryDTO)
        {
            const string query = "INSERT INTO inventory (date, name) VALUES (@date, @name)";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@date", inventoryDTO.Date);
                cmd.Parameters.AddWithValue("@name", inventoryDTO.Name);
                conn.Open();

                cmd.ExecuteNonQuery();
            }
        }
    }
}
