using GraslandenBL.Domain;
using GraslandenBL.DTOs;
using GraslandenBL.Enums;
using GraslandenBL.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics.Metrics;
using System.Transactions;

namespace GraslandenDL.Repositories
{
    public class RepositorySQL : IRepository
    {
        private string _connectionString;

        public RepositorySQL(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<CampusDTO> GetAllCampusesDTO(int inventoryID)
        {
            //public CampusDTO(List<Plot> plots, Dictionary<string, PlotValue> plotTypes)
            List<string> campusesList = new List<string>();
            List<CampusDTO> campusDTOs = new List<CampusDTO>();

            //Get all campuses  
            //dont filter on inventoryID because we want all campuses even those WITHOUT plots in this inventory
            string queryGetAllCampuses = "SELECT DISTINCT gp.campus FROM inventoried_plot ip " +
                "JOIN grass_plot gp ON ip.plot_code = gp.code";

            //Join grass_plot, inventoried_plot
            const string queryGetPlots = "SELECT i.plot_code, gp.area_sq_meter, gp.campus, mt.type, i.plot_type FROM inventoried_plot i " +
                "JOIN grass_plot gp ON i.plot_code = gp.code " +
                "JOIN management_type mt on mt.id = i.management_type " +
                "WHERE i.inventory_id = @inventoryID AND gp.campus = @campus";
            const string queryPlotTypeValues = "select plot_type, count(plot_type) as count, sum(g.area_sq_meter) as area_sum from inventoried_plot i " +
                "join grass_plot g on i.plot_code = g.code group by plot_type,inventory_id,campus " +
                "having campus = @campus and inventory_id = @inventoryID";
            const string queryManagementTypeValues = "select mt.type, count(type), sum(g.area_sq_meter) from inventoried_plot ip " +
                "join grass_plot g on ip.plot_code = g.code join management_type mt on mt.id = ip.management_type " +
                "group by type,inventory_id,campus having campus = @campus and inventory_id = @inventoryID";

            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmdGetAllCampuses = con.CreateCommand())
            using (SqlCommand cmdGetAllPlots = con.CreateCommand())
            using (SqlCommand cmdPlotTypeValues = con.CreateCommand())
            using (SqlCommand cmdManagementTypeValues = con.CreateCommand())
            {
                //Parameters
                cmdGetAllCampuses.Parameters.AddWithValue("@inventoryID", inventoryID);
                cmdGetAllPlots.Parameters.AddWithValue("@inventoryID", inventoryID);
                cmdGetAllPlots.Parameters.Add(new SqlParameter("@campus", SqlDbType.NVarChar));
                cmdPlotTypeValues.Parameters.AddWithValue("@inventoryID", inventoryID);
                cmdPlotTypeValues.Parameters.Add(new SqlParameter("@campus", SqlDbType.NVarChar));
                cmdManagementTypeValues.Parameters.AddWithValue("@inventoryID", inventoryID);
                cmdManagementTypeValues.Parameters.Add(new SqlParameter("@campus", SqlDbType.NVarChar));
                //Open connection
                con.Open();
                cmdGetAllCampuses.CommandText = queryGetAllCampuses;
                cmdGetAllPlots.CommandText = queryGetPlots;
                cmdPlotTypeValues.CommandText = queryPlotTypeValues;
                cmdManagementTypeValues.CommandText = queryManagementTypeValues;
                //Get all campuses
                using (SqlDataReader reader = cmdGetAllCampuses.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        campusesList.Add(reader.GetString(0));
                    }
                }
                //Now you have all campuses
                //then
                //For each campus make the list of plots and the plotvalue dict
                foreach (string campus in campusesList)
                {
                    cmdGetAllPlots.Parameters["@campus"].Value = campus;
                    cmdPlotTypeValues.Parameters["@campus"].Value = campus;
                    cmdManagementTypeValues.Parameters["@campus"].Value = campus;
                    Dictionary<string, PlotTypeValue> plotTypes = new Dictionary<string, PlotTypeValue>();
                    Dictionary<ManagementType, ManagementTypeValue> managementTypes = new Dictionary<ManagementType, ManagementTypeValue>();
                    List<Plot> plots = new List<Plot>();
                    using (SqlDataReader reader = cmdGetAllPlots.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            //public Plot(string code, double areaSqMeters, string campus, ManagementType managementType, string plotType)
                            string code = reader.GetString(reader.GetOrdinal("plot_code"));
                            double areaSqMeter = reader.GetDouble(reader.GetOrdinal("area_sq_meter"));
                            string campusValue = reader.GetString(reader.GetOrdinal("campus"));
                            string managementType = reader.GetString(reader.GetOrdinal("type"));
                            ManagementType managementTypeEnum = Plot.StringToManagementType(managementType);

                            string plotTypeCode = reader.GetString(reader.GetOrdinal("plot_type"));
                            Plot plot = new Plot(code, areaSqMeter, campusValue, managementTypeEnum, plotTypeCode);
                            plots.Add(plot);
                        }
                    }

                    using (SqlDataReader reader = cmdPlotTypeValues.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string plotType = reader.GetString(0);
                            int count = reader.GetInt32(1);
                            double areaSum = reader.GetDouble(2);
                            plotTypes.Add(plotType, new PlotTypeValue(count, areaSum));
                        }
                    }

                    using(SqlDataReader reader = cmdManagementTypeValues.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ManagementType managementType = Plot.StringToManagementType(reader.GetString(0));
                            int count = reader.GetInt32(1);
                            double areaSum = reader.GetDouble(2);
                            managementTypes.Add(managementType, new ManagementTypeValue(count, areaSum));
                        }
                    }

                    CampusDTO campusDTO = new CampusDTO(plots, plotTypes, managementTypes, campus);
                    campusDTOs.Add(campusDTO);
                }
            }

            return campusDTOs;
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
                             Species.ParseRating(rating)
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
            const string queryInventory = "INSERT INTO inventory(date,name) output INSERTED.ID VALUES(@date,@name)";
            const string querySpecies = "select id from species where name = @name";
            const string insertSpecies = "insert into species (name, rating, moisture, ph, nitrogen, nectar_production, biodiversity_relevance)" +
                                         "output inserted.id VALUES(@name, @rating, @moisture, @ph, @nitrogen, @nectar_production, @biodiversity)";
            const string queryPlot = "select code from grass_plot where code = @code";
            const string insertPlot = "insert into grass_plot (code, campus, area_sq_meter) VALUES(@code, @campus, @area_sq_meter)";
            const string queryManagementType = "select id,type from management_type";
            const string queryInventoriedPlot = "INSERT INTO inventoried_plot (inventory_id,plot_code,management_type, plot_type) " +
                                                "output inserted.id " +
                                                "VALUES (@inventory_id, @plot_code, @management_type, @plot_type)";
            const string queryMeasurement = "INSERT INTO measurement (inventoried_plot_id, species_id, coverage)" +
                                    "OUTPUT Inserted.id " +
                                      "VALUES (@inventoried_plot_id,@species_id,@coverage)";
            const string insertMessage = "INSERT INTO message (object_id, object_type, inventory_id, description, message_type) " +
                                    "VALUES (@objectId, @objectType, @inventoryId, @description, @messageType)";
            const string queryMessage = "SELECT id FROM message WHERE inventory_id is null AND description = @description";

            Dictionary<Species, int> speciesList = new();
            Dictionary<string, int> inventoriedPlotIds = new();
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
            using (SqlCommand cmdMessageInsert = conn.CreateCommand())
            using (SqlCommand cmdMessageQuery = conn.CreateCommand())
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

                cmdMessageInsert.CommandText = insertMessage;
                cmdMessageInsert.Parameters.Add("@objectId", SqlDbType.Int);
                cmdMessageInsert.Parameters.Add("@objectType", SqlDbType.NVarChar);
                cmdMessageInsert.Parameters.Add("@inventoryId", SqlDbType.Int);
                cmdMessageInsert.Parameters.Add("@description", SqlDbType.NVarChar);
                cmdMessageInsert.Parameters.Add("@messageType", SqlDbType.NVarChar);

                cmdMessageQuery.CommandText = queryMessage;
                cmdMessageQuery.Parameters.Add("@description", SqlDbType.NVarChar);

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
                cmdMessageInsert.Transaction = transaction;
                cmdMessageQuery.Transaction = transaction;

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

                        foreach (KeyValuePair<string, MessageType> message in species.Errors)
                        {
                            cmdMessageInsert.Parameters["@objectId"].Value = (int)speciesId;
                            cmdMessageInsert.Parameters["@objectType"].Value = nameof(species);
                            cmdMessageInsert.Parameters["@description"].Value = message.Key;
                            cmdMessageInsert.Parameters["@messageType"].Value = message.Value.ToString();
                            cmdMessageInsert.Parameters["@inventoryId"].Value = DBNull.Value;

                            cmdMessageQuery.Parameters["@description"].Value = message.Key;
                            if(cmdMessageQuery.ExecuteScalar() == null)
                            cmdMessageInsert.ExecuteNonQuery();
                        }
                        speciesList.Add(species, (int)speciesId);
                    }

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
                        cmdInventoriedPlot.Parameters["@inventory_id"].Value = inventoryId;
                        cmdInventoriedPlot.Parameters["@plot_code"].Value = plot.Code;
                        cmdInventoriedPlot.Parameters["@management_type"].Value = managementTypeList[plot.ManagementType.ToString()];
                        cmdInventoriedPlot.Parameters["@plot_type"].Value = plot.PlotType;
                        int inventoriedPlotId = (int)cmdInventoriedPlot.ExecuteScalar();
                        inventoriedPlotIds.Add(plot.Code, inventoriedPlotId);

                        foreach (KeyValuePair<string, MessageType> message in plot.Errors)
                        {
                            cmdMessageInsert.Parameters["@objectId"].Value = inventoriedPlotId;
                            cmdMessageInsert.Parameters["@objectType"].Value = nameof(plot);
                            cmdMessageInsert.Parameters["@description"].Value = message.Key;
                            cmdMessageInsert.Parameters["@messageType"].Value = message.Value.ToString();
                            cmdMessageInsert.Parameters["@inventoryId"].Value = inventoryId;

                            cmdMessageInsert.ExecuteNonQuery();
                        }
                    }

                    foreach (Measurement measurement in inventory.Measurements)
                    {
                        cmdMeasurement.Parameters["@inventoried_plot_id"].Value = inventoriedPlotIds[measurement.Plot.Code];
                        cmdMeasurement.Parameters["@species_id"].Value = speciesList[measurement.Species];
                        cmdMeasurement.Parameters["@coverage"].Value = measurement.Coverage;
                        int measurementId = (int)cmdMeasurement.ExecuteScalar();

                        foreach (KeyValuePair<string, MessageType> message in measurement.Errors)
                        {
                            cmdMessageInsert.Parameters["@objectId"].Value = measurementId;
                            cmdMessageInsert.Parameters["@objectType"].Value = nameof(measurement);
                            cmdMessageInsert.Parameters["@description"].Value = message.Key;
                            cmdMessageInsert.Parameters["@messageType"].Value = message.Value.ToString();
                            cmdMessageInsert.Parameters["@inventoryId"].Value = inventoryId;

                            cmdMessageInsert.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                    return inventoryId;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public List<InventoryDTO> GetInventoryDTOs()
        {
            const string query = "SELECT id, date, name FROM inventory";
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
                        inventories.Add(new InventoryDTO(reader.GetInt32(0), reader.GetDateTime(1), reader.GetString(2)));
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
        public MeasurementDTO InsertMeasurement(string plotCode, string species, string coverage, int inventoryId)
        {
            const string queryInventoriedPlot = "select id from inventoried_plot where inventory_id = @inventory_id and plot_code = @plot_code";
            const string querySpecies = "select * from species where name = @name";
            const string queryMeasurement = "insert into measurement (inventoried_plot_id, species_id, coverage) OUTPUT Inserted.Id VALUES (@inventoried_plot_id,@species_id,@coverage)";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmdInventoriedPlot = conn.CreateCommand())
            using (SqlCommand cmdSpecies = conn.CreateCommand())
            using (SqlCommand cmdMeasurement = conn.CreateCommand())
            {
                cmdInventoriedPlot.CommandText = queryInventoriedPlot;
                cmdSpecies.CommandText = querySpecies;
                cmdMeasurement.CommandText = queryMeasurement;

                cmdInventoriedPlot.Parameters.AddWithValue("@inventory_id", inventoryId);
                cmdInventoriedPlot.Parameters.AddWithValue("@plot_code", plotCode);

                cmdSpecies.Parameters.AddWithValue("@name", species);

                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();
                cmdInventoriedPlot.Transaction = transaction;
                cmdSpecies.Transaction = transaction;
                cmdMeasurement.Transaction = transaction;
                try
                {
                    int inventoriedPlotId = (int)cmdInventoriedPlot.ExecuteScalar();

                    Species foundSpecies = null;
                    using (SqlDataReader reader = cmdSpecies.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int? nectarValue = reader.IsDBNull(reader.GetOrdinal("nectar_production")) ? null : reader.GetInt32(reader.GetOrdinal("nectar_production"));
                            int? biodiversity = reader.IsDBNull(reader.GetOrdinal("biodiversity_relevance")) ? null : reader.GetInt32(reader.GetOrdinal("biodiversity_relevance"));
                            foundSpecies = new Species(reader.GetInt32(reader.GetOrdinal("id")),
                                reader.GetString(reader.GetOrdinal("name")),
                                reader.GetInt32(reader.GetOrdinal("moisture")),
                                reader.GetInt32(reader.GetOrdinal("ph")),
                                reader.GetInt32(reader.GetOrdinal("nitrogen")),
                                nectarValue,
                                biodiversity,
                                Species.ParseRating(reader.GetString(reader.GetOrdinal("rating"))));
                        }
                    }


                    if (foundSpecies != null)
                    {
                        cmdMeasurement.Parameters.AddWithValue("@inventoried_plot_id", inventoriedPlotId);
                        cmdMeasurement.Parameters.AddWithValue("@species_id", foundSpecies.Id);
                        cmdMeasurement.Parameters.AddWithValue("@coverage", coverage);
                        int measurementId = (int)cmdMeasurement.ExecuteScalar();

                        transaction.Commit();
                        return new MeasurementDTO(measurementId, foundSpecies, coverage);
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }
        public List<MeasurementDTO> GetMeasurementsDTOForPlot(int inventoryID, string code)
        {
            List<MeasurementDTO> measurementDTOList = new List<MeasurementDTO>();

            string queryMeasurement = "SELECT m.id, m.coverage, m.species_id, s.name, s.rating, s.moisture, s.ph, s.nitrogen, " +
                "s.nectar_production, s.biodiversity_relevance FROM measurement m " +
                "JOIN inventoried_plot ip ON m.inventoried_plot_id = ip.id " +
                "JOIN species s on s.id = m.species_id " +
                //Select only those where inventory_id = @inventoryID AND plot_code = @code
                "WHERE ip.inventory_id = @inventoryID AND ip.plot_code =@code";

            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmdMeasurementDTO = new SqlCommand(queryMeasurement, con))
            {
                //Parameters
                cmdMeasurementDTO.Parameters.AddWithValue("@inventoryID", inventoryID);
                cmdMeasurementDTO.Parameters.AddWithValue("@code", code);

                //Open conection 
                con.Open();

                //read!!
                using (SqlDataReader reader = cmdMeasurementDTO.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int speciesId = reader.GetInt32(reader.GetOrdinal("species_id"));
                        string speciesName = reader.GetString(reader.GetOrdinal("name"));
                        string ratingString = reader.GetString(reader.GetOrdinal("rating"));
                        Rating rating = Species.ParseRating(ratingString);
                        int moisture = reader.GetInt32(reader.GetOrdinal("moisture"));
                        int ph = reader.GetInt32(reader.GetOrdinal("ph"));
                        int nitrogen = reader.GetInt32(reader.GetOrdinal("nitrogen"));
                        int? nectarValue = reader.IsDBNull(reader.GetOrdinal("nectar_production")) ? null : reader.GetInt32(reader.GetOrdinal("nectar_production"));
                        int? biodiversity = reader.IsDBNull(reader.GetOrdinal("biodiversity_relevance")) ? null : reader.GetInt32(reader.GetOrdinal("biodiversity_relevance"));
                        string coverage = reader.GetString(reader.GetOrdinal("coverage"));
                        int measurementId = reader.GetInt32(reader.GetOrdinal("id"));

                        Species species = new Species(speciesId, speciesName, moisture, ph, nitrogen, nectarValue, biodiversity, rating);
                        MeasurementDTO measurementDTO = new MeasurementDTO(measurementId, species, coverage);
                        measurementDTOList.Add(measurementDTO);
                    }
                }
                return measurementDTOList;
            }
        }

        public bool DeleteInventory(int inventoryId)
        {
            // Queries
            string inventoriedPlotSelectQuery = "SELECT id FROM inventoried_plot WHERE inventory_id = @inventoryId";
            string measurementQuery = "DELETE FROM measurement WHERE inventoried_plot_id = @inventoriedPlotId";
            string inventoriedPlotDeleteQuery = "DELETE FROM inventoried_plot WHERE inventory_id = @inventoryId";
            string inventoryQuery = "DELETE FROM inventory WHERE id = @inventoryId";
            string messageQuery = "DELETE FROM message WHERE inventory_id = @inventoryId";

            List<int> inventoriedPlotIds = new List<int>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand inventoriedPlotSelectCommand = connection.CreateCommand())
                {
                    using (SqlCommand measurementCommand = connection.CreateCommand())
                    {
                        using (SqlCommand inventoriedPlotDeleteCommand = connection.CreateCommand())
                        {
                            using (SqlCommand inventoryCommand = connection.CreateCommand())
                            {
                                using (SqlCommand messageCommand = connection.CreateCommand())
                                {
                                    // CommandText
                                    inventoriedPlotSelectCommand.CommandText = inventoriedPlotSelectQuery;
                                    measurementCommand.CommandText = measurementQuery;
                                    inventoriedPlotDeleteCommand.CommandText = inventoriedPlotDeleteQuery;
                                    inventoryCommand.CommandText = inventoryQuery;
                                    messageCommand.CommandText = messageQuery;

                                    // Parameters
                                    inventoriedPlotSelectCommand.Parameters.AddWithValue("@inventoryId", inventoryId);
                                    measurementCommand.Parameters.Add(new SqlParameter("@inventoriedPlotId", SqlDbType.Int));
                                    inventoriedPlotDeleteCommand.Parameters.AddWithValue("@inventoryId", inventoryId);
                                    inventoryCommand.Parameters.AddWithValue("@inventoryId", inventoryId);
                                    messageCommand.Parameters.AddWithValue("@inventoryId", inventoryId);

                                    connection.Open();

                                    // Transaction
                                    SqlTransaction transaction = connection.BeginTransaction();
                                    inventoriedPlotSelectCommand.Transaction = transaction;
                                    measurementCommand.Transaction = transaction;
                                    inventoriedPlotDeleteCommand.Transaction = transaction;
                                    inventoryCommand.Transaction = transaction;
                                    messageCommand.Transaction = transaction;

                                    try
                                    {
                                        // Get all inventoried_plot_ids of inventory
                                        using (SqlDataReader reader = inventoriedPlotSelectCommand.ExecuteReader())
                                        {
                                            while (reader.Read())
                                            {
                                                inventoriedPlotIds.Add(reader.GetInt32(reader.GetOrdinal("id")));
                                            }
                                        }

                                        foreach (int id in inventoriedPlotIds)
                                        {
                                            measurementCommand.Parameters["@inventoriedPlotId"].Value = id;

                                            measurementCommand.ExecuteNonQuery();
                                        }

                                        inventoriedPlotDeleteCommand.ExecuteNonQuery();
                                        inventoryCommand.ExecuteNonQuery();
                                        messageCommand.ExecuteNonQuery();

                                        transaction.Commit();
                                        return true;
                                    }

                                    catch
                                    {
                                        transaction.Rollback();
                                        return false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void InsertSpecies(Species species)
        {
            const string query = "insert into species(name, rating, moisture, ph, nitrogen, nectar_production, biodiversity_relevance)" +
                                 "VALUES(@name, @rating, @moisture, @ph, @nitrogen, @nectar_production, @biodiversity)";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@name", species.Name);
                cmd.Parameters.AddWithValue("@rating", species.Rating);
                cmd.Parameters.AddWithValue("@moisture", species.Moisture);
                cmd.Parameters.AddWithValue("@ph", species.Ph);
                cmd.Parameters.AddWithValue("@nitrogen", species.Nitrogen);
                cmd.Parameters.AddWithValue("@nectar_production", species.Nectarvalue != null ? species.Nectarvalue : DBNull.Value);
                cmd.Parameters.AddWithValue("@biodiversity", species.Biodiversity != null ? species.Biodiversity : DBNull.Value);
                cmd.ExecuteNonQuery();
            }
        }

        public void InsertMessages(int inventoryID, Dictionary<string, MessageType> messages)
        {
            string queryInsertMessage = "INSERT INTO message(inventory_id, description, message_type) VALUES(@inventory_id, @description, @messageType)";

            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmdInsertMessages = con.CreateCommand())
            {
                //PARA
                cmdInsertMessages.CommandText = queryInsertMessage;
                cmdInsertMessages.Parameters.Add(new SqlParameter("@inventory_id", SqlDbType.Int));
                cmdInsertMessages.Parameters.Add(new SqlParameter("@description", SqlDbType.NVarChar));
                cmdInsertMessages.Parameters.Add(new SqlParameter("@messageType", SqlDbType.NVarChar));

                //Open connection and transaction
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();
                cmdInsertMessages.Transaction = transaction;

                try
                {   //For each key (description) value(messagetype) add them
                    foreach (KeyValuePair<string, MessageType> messageType in messages)
                    {
                        cmdInsertMessages.Parameters["@inventory_id"].Value = inventoryID;
                        cmdInsertMessages.Parameters["@description"].Value = messageType.Key;
                        cmdInsertMessages.Parameters["@messageType"].Value = messageType.Value;
                        cmdInsertMessages.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

       

        public void DeleteMeasurement(int measurementDTO_id)
        {
            string deleetMeasurementQuery = "DELETE FROM measurement WHERE id = @measurementDTO_id";

            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand deleteMeasurementCommand = con.CreateCommand())
            {
                deleteMeasurementCommand.CommandText = deleetMeasurementQuery;
                deleteMeasurementCommand.Parameters.AddWithValue("@measurementDTO_id", measurementDTO_id);

                con.Open();

                deleteMeasurementCommand.ExecuteNonQuery();
            }
        }

        public Plot InsertInventoriedPlot(int inventoryID, string code, ManagementType managementType, string plot_Type)
        {
            //get management type id
            string queryGetManagementTypeID = "SELECT id FROM management_type WHERE type = @management_type";

            //insert inventoried plot with given management type id
            string queryInsertInventoriedPlot = "INSERT INTO inventoried_plot(inventory_id, plot_code, management_type, plot_type) VALUES(@inventoryID, @code, @management_type_id, @plot_type)";

            //If plot exsists return data of plot else return null
            string queryGetPlot = "SELECT campus, area_sq_meter FROM grass_plot WHERE code = @code";

            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmdInsertInventoriedPlot = con.CreateCommand())
            using (SqlCommand cmdCheckManagementType = con.CreateCommand())
            using (SqlCommand cmdGetPlot = con.CreateCommand())
            {
                cmdCheckManagementType.CommandText = queryGetManagementTypeID;
                cmdInsertInventoriedPlot.CommandText = queryInsertInventoriedPlot;
                cmdGetPlot.CommandText = queryGetPlot;

                cmdCheckManagementType.Parameters.AddWithValue("@management_type", managementType.ToString());

                cmdInsertInventoriedPlot.Parameters.AddWithValue("@inventoryID", inventoryID);
                cmdInsertInventoriedPlot.Parameters.AddWithValue("@code", code);
                cmdInsertInventoriedPlot.Parameters.AddWithValue("@plot_type", plot_Type);

                cmdGetPlot.Parameters.AddWithValue("@code", code);

                //open connection and transaction
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();
                cmdCheckManagementType.Transaction = transaction;
                cmdInsertInventoriedPlot.Transaction = transaction;
                cmdGetPlot.Transaction = transaction;
                try
                {
                    Plot plot = null;
                    //public Plot(string code, double areaSqMeters, string campus, ManagementType managementType, string plotType, Dictionary<string, MessageType> errors = null)
                    using (SqlDataReader reader = cmdGetPlot.ExecuteReader())
                    {
                        
                        while (reader.Read())
                        {
                            string campus = reader.GetString(0);
                            double area_sq_meterOrdinal = reader.GetDouble(1);
                            plot = new Plot(code, area_sq_meterOrdinal, campus, managementType, plot_Type);
                        }
                        if (plot == null) return plot;
                    }

                    //get the id of management type
                    int managementType_id = (int)cmdCheckManagementType.ExecuteScalar();
                    cmdInsertInventoriedPlot.Parameters.AddWithValue("@management_type_id", managementType_id);
                    cmdInsertInventoriedPlot.ExecuteNonQuery();
                    transaction.Commit();
                    return plot;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Dictionary<string, Dictionary<string, MessageType>> GetAllMessages()
        {
            Dictionary<string, Dictionary<string, MessageType>> allMessagesSorted = new Dictionary<string, Dictionary<string, MessageType>>();


            string queryGetAllMessages = "SELECT i.name, m.inventory_id, m.description, m.message_type FROM message m " +
            "LEFT OUTER JOIN inventory i ON m.inventory_id = i.id";

            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmdGetMessagesDTO = con.CreateCommand())
            {
                //Open conection 
                cmdGetMessagesDTO.CommandText = queryGetAllMessages;
                con.Open();
                using (SqlDataReader reader = cmdGetMessagesDTO.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string inventory_name;
                        int? inventory_id = reader.IsDBNull(1) ? null : reader.GetInt32(1);
                        if (inventory_id != null)
                        {
                            inventory_name = reader.GetString(0);
                        }
                        else
                        {
                            inventory_name = "Algemeen";
                        }

                        string description = reader.GetString(2);
                        string messageTypeString = reader.GetString(3);
                        MessageType messagetype = messagetype = messageTypeString switch
                        {
                            "Error" => MessageType.Error,
                            "Remark" => MessageType.Remark,
                            _ => throw new ArgumentException(),
                        };

                        Dictionary<string, MessageType> messageDesc = new Dictionary<string, MessageType>();
                        if (allMessagesSorted.ContainsKey(inventory_name))
                        {
                            allMessagesSorted[inventory_name].Add(description,messagetype);
                        }
                        else
                        {
                            messageDesc.Add(description, messagetype);
                            allMessagesSorted.Add(inventory_name,messageDesc);
                        }
                    }
                    return allMessagesSorted;
                }
            }
        }
    }
}
