using GraslandenBL.Domain;
using GraslandenBL.DTOs;
using GraslandenBL.Enums;
using GraslandenBL.Interfaces;
using Microsoft.Data.SqlClient;
using System.Collections;
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
            string queryCampus = "SELECT DISTINCT campus FROM grass_plot";

            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmdCampus = new SqlCommand(queryCampus, con))
            {

                //Open connection
                con.Open();

                using (SqlDataReader reader = cmdCampus.ExecuteReader())
                {
                    cmdCampus.CommandText = queryCampus;
                    while (reader.Read())
                    {
                        campuses.Add(reader.GetString("campus"));
                    }
                    return campuses;
                }
            }
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

                        cmd.CommandText = query;
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string name = reader.GetString(1);
                            string rating = reader.GetString(2);
                            int moisture = reader.GetInt32(3);
                            int ph = reader.GetInt32(4);
                            int nitrogen = reader.GetInt32(5);
                            //check if null
                            int? nectarValue;
                            if (reader.IsDBNull(6))
                            {
                                nectarValue = null;
                            }
                            else
                            {
                                nectarValue = reader.GetInt32(6);
                            }
                            //check if nulll
                            int? biodiversity;
                            if (reader.IsDBNull(7))
                            {
                                biodiversity = null;
                            }
                            else
                            {
                                biodiversity = reader.GetInt32(7);
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


        public int ImportInventory(Inventory inventory)
        {
            string queryInventory = "INSERT INTO inventory(date,name) output INSERTED.ID VALUES(@date,@name)";
            const string querySpecies = "select id from species where name = @name";
            const string insertSpecies = "insert into species (name, rating, moisture, ph, nitrogen, nectar_production, biodiversity_relevance)" +
                                         "output inserted.id VALUES(@name, @rating, @moisture, @ph, @nitrogen, @nectar_production, @biodiversity)";
            const string queryPlot = "select code from grass_plot where code = @code";
            const string insertPlot = "insert into grass_plot (code, campus, area_sq_meter) VALUES(@code, @campus, @area_sq_meter)";
            const string queryManagementType = "select id,type from management_type";
            const string queryInventoriedPlot = "INSERT INTO inventoried_plot (inventory_id,plot_code,management_type, plot_type) " +
                                                "OUTPUT inserted.id " +
                                                "VALUES (@inventory_id, @plot_code, @management_type, @plot_type)";
            string queryMeasurement = "INSERT INTO measurement (inventoried_plot_id, species_id, coverage) VALUES (@inventoried_plot_id,@species_id,@coverage)";

            Dictionary<Species, int> speciesList = new();
            Dictionary<string, int> managementTypeList = new();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmdInventory = conn.CreateCommand())
            using (SqlCommand cmdSpecies = conn.CreateCommand())
            using (SqlCommand cmdInsertSpecies = conn.CreateCommand())
            using (SqlCommand cmdPlot = conn.CreateCommand())
            using (SqlCommand cmdInsertPlot = conn.CreateCommand())
            using (SqlCommand cmdManagementType = conn.CreateCommand())
            using (SqlCommand cmdInventoriedPlot = conn.CreateCommand())
            using (SqlCommand cmdMeasurement = conn.CreateCommand())
            {
                cmdInventory.CommandText = queryInventory;
                cmdInventory.Parameters.AddWithValue("@date", inventory.Date);
                cmdInventory.Parameters.AddWithValue("@name", inventory.Name);

                cmdSpecies.CommandText = querySpecies;
                cmdSpecies.Parameters.Add(new SqlParameter("@name", SqlDbType.NVarChar));
                cmdInsertSpecies.CommandText = insertSpecies;
                cmdInsertSpecies.Parameters.Add(new SqlParameter("@name", SqlDbType.NVarChar));
                cmdInsertSpecies.Parameters.Add(new SqlParameter("@rating", SqlDbType.NVarChar));
                cmdInsertSpecies.Parameters.Add(new SqlParameter("@moisture", SqlDbType.Int));
                cmdInsertSpecies.Parameters.Add(new SqlParameter("@ph", SqlDbType.Int));
                cmdInsertSpecies.Parameters.Add(new SqlParameter("@nitrogen", SqlDbType.Int));
                cmdInsertSpecies.Parameters.Add(new SqlParameter("@nectar_production", SqlDbType.Int));
                cmdInsertSpecies.Parameters.Add(new SqlParameter("@biodiversity", SqlDbType.Int));

                cmdPlot.CommandText = queryPlot;
                cmdPlot.Parameters.Add(new SqlParameter("@code", SqlDbType.NVarChar));
                cmdInsertPlot.CommandText = insertPlot;
                cmdInsertPlot.Parameters.Add(new SqlParameter("@code", SqlDbType.NVarChar));
                cmdInsertPlot.Parameters.Add(new SqlParameter("@campus", SqlDbType.NVarChar));
                cmdInsertPlot.Parameters.Add(new SqlParameter("@area_sq_meter", SqlDbType.Float));

                cmdManagementType.CommandText = queryManagementType;

                cmdInventoriedPlot.CommandText = queryInventoriedPlot;
                cmdInventoriedPlot.Parameters.Add(new SqlParameter("@plot_code", SqlDbType.NVarChar));
                cmdInventoriedPlot.Parameters.Add(new SqlParameter("@management_type", SqlDbType.Int));
                cmdInventoriedPlot.Parameters.Add(new SqlParameter("@plot_type", SqlDbType.NVarChar));
                cmdInventoriedPlot.Parameters.Add(new SqlParameter("@inventory_id", SqlDbType.Int));

                cmdMeasurement.CommandText = queryMeasurement;
                cmdMeasurement.Parameters.Add(new SqlParameter("@inventoried_plot_id", SqlDbType.Int));
                cmdMeasurement.Parameters.Add(new SqlParameter("@species_id", SqlDbType.Int));
                cmdMeasurement.Parameters.Add(new SqlParameter("@coverage", SqlDbType.NVarChar));

                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                cmdInventory.Transaction = transaction;
                cmdSpecies.Transaction = transaction;
                cmdInsertSpecies.Transaction = transaction;
                cmdPlot.Transaction = transaction;
                cmdInsertPlot.Transaction = transaction;
                cmdManagementType.Transaction = transaction;
                cmdInventoriedPlot.Transaction = transaction;
                cmdMeasurement.Transaction = transaction;

                try
                {
                    int inventoryId = (int)cmdInventory.ExecuteScalar();
                    using (SqlDataReader reader = cmdManagementType.ExecuteReader()) 
                    {
                        while (reader.Read())
                        {
                            managementTypeList.Add(reader.GetString(1), reader.GetInt32(0));
                        }
                    }
                    foreach (Species species in inventory.GetSpecies())
                    {
                        cmdSpecies.Parameters["@name"].Value = species.Name;
                        int? speciesId = (int?)cmdSpecies.ExecuteScalar();
                        if (speciesId == null)
                        {
                            cmdInsertSpecies.Parameters["@name"].Value = species.Name;
                            cmdInsertSpecies.Parameters["@rating"].Value = species.Rating;
                            cmdInsertSpecies.Parameters["@moisture"].Value = species.Moisture;
                            cmdInsertSpecies.Parameters["@ph"].Value = species.Ph;
                            cmdInsertSpecies.Parameters["@nitrogen"].Value = species.Nitrogen;
                            cmdInsertSpecies.Parameters["@nectar_production"].Value = species.Nectarvalue is not null ? species.Nectarvalue : DBNull.Value;
                            cmdInsertSpecies.Parameters["@biodiversity"].Value = species.Biodiversity is not null ? species.Biodiversity : DBNull.Value;
                            speciesId = (int?)cmdInsertSpecies.ExecuteScalar();
                        }
                        speciesList.Add(species, (int)speciesId);
                    }
                    int inventoriedPlotId = 0;
                    foreach (Plot plot in inventory.GetPlots())
                    {
                        cmdPlot.Parameters["@code"].Value = plot.Code;
                        if (cmdPlot.ExecuteScalar() == null)
                        {
                            cmdInsertPlot.Parameters["@code"].Value = plot.Code;
                            cmdInsertPlot.Parameters["@campus"].Value = plot.Campus;
                            cmdInsertPlot.Parameters["@area_sq_meter"].Value = plot.AreaSqMeters;
                            cmdInsertPlot.ExecuteNonQuery();
                        }
                    }
                    foreach (Measurement measurement in inventory.Measurements)
                    {
                        cmdInventoriedPlot.Parameters["@inventory_id"].Value = inventoryId;
                        cmdInventoriedPlot.Parameters["@plot_code"].Value = measurement.Plot.Code;
                        cmdInventoriedPlot.Parameters["@management_type"].Value = managementTypeList[measurement.Plot.ManagementType.ToString()];
                        cmdInventoriedPlot.Parameters["@plot_type"].Value = measurement.Plot.PlotType;
                        inventoriedPlotId = (int)cmdInventoriedPlot.ExecuteScalar();

                        cmdMeasurement.Parameters["@inventoried_plot_id"].Value = inventoriedPlotId;
                        cmdMeasurement.Parameters["@species_id"].Value = speciesList[measurement.Species];
                        cmdMeasurement.Parameters["@coverage"].Value = measurement.Coverage;
                        cmdMeasurement.ExecuteNonQuery();
                    }
                    transaction.Commit();
                    return inventoryId;
                }
                catch (Exception ex)
                {
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
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
                using (SqlDataReader reader = cmd.ExecuteReader()) 
                { 

                    while (reader.Read())
                    {
                        inventories.Add(new InventoryDTO(reader.GetDateTime(0), reader.GetString(1)));
                    }
                }
            }
            return inventories;
        }

        public int ImportEmptyInventory(InventoryDTO inventoryDTO)
        {
            const string query = "INSERT INTO inventory (date, name) output inserted.ID VALUES (@date, @name)";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@date", inventoryDTO.Date);
                cmd.Parameters.AddWithValue("@name", inventoryDTO.Name);
                conn.Open();
                // return id 
                return (int)cmd.ExecuteScalar();
            }
        }
        public Dictionary<Plot, string> GetAllGrassPlots(int inventoryID)
        {
            Dictionary<Plot, string> grassPlots = new Dictionary<Plot, string>();

            //Join grass_plot, inventoried_plot
            string queryGrassPlot = "SELECT ip.plot_code, ip.management_type, ip.plot_type, gp.campus,gp.area_sq_meter FROM inventoried_plot ip JOIN grass_plot gp ON ip.plot_code = gp.code WHERE ip.inventory_id = @inventoryID";

            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmdGrassPlot = new SqlCommand(queryGrassPlot, con))
            {
                cmdGrassPlot.CommandText = queryGrassPlot;
                //Parameterrs
                cmdGrassPlot.Parameters.AddWithValue("@inventoryID", inventoryID);

                //Open connection
                con.Open();
                SqlDataReader reader = cmdGrassPlot.ExecuteReader();
                while (reader.Read())
                {
                    string code = reader.GetString(reader.GetOrdinal("plot_code"));
                    //Netheidsboord, Schapenweide, Intensief, Extensief
                    string managementType = reader.GetString(reader.GetOrdinal("management_type"));
                    ManagementType managementTypeEnum = managementType switch
                    {
                        "Netheidsboord" => ManagementType.Netheidsboord,
                        "Schapenweide" => ManagementType.Schapenweide,
                        "Intensief" => ManagementType.Intensief,
                        "Extensief" => ManagementType.Extensief,
                    };

                    string plotTypeCode = reader.GetString(reader.GetOrdinal("plot_type"));
                    string campus = reader.GetString(reader.GetOrdinal("campus"));
                    double areaSqMeterString = reader.GetDouble(reader.GetOrdinal("area_sq_meter"));

                    Plot plot = new Plot(code, areaSqMeterString, campus, managementTypeEnum, plotTypeCode);
                    grassPlots.Add(plot, campus);
                }

            }

            return grassPlots;
        }
        public void InsertMeasurement(Measurement measurement)
        {

        }
    }
}
