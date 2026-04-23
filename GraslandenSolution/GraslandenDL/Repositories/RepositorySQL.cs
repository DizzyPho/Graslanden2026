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
                                                "output inserted.id " +
                                                "VALUES (@inventory_id, @plot_code, @management_type, @plot_type)";
            string queryMeasurement = "INSERT INTO measurement (inventoried_plot_id, species_id, coverage)" +
                                    "OUTPUT Inserted.id " +
                                      "VALUES (@inventoried_plot_id,@species_id,@coverage)";
            string insertMessage = "INSERT INTO message (object_id, object_type, inventory_id, description, message_type) " +
                                    "VALUES (@objectId, @objectType, @inventoryId, @description, @messageType)";

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
            using (SqlCommand cmdMessage = conn.CreateCommand())
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

                cmdMessage.CommandText = insertMessage;
                cmdMessage.Parameters.Add("@objectId", SqlDbType.Int);
                cmdMessage.Parameters.Add("@objectType", SqlDbType.NVarChar);
                cmdMessage.Parameters.Add("@inventoryId", SqlDbType.Int);
                cmdMessage.Parameters.Add("@description", SqlDbType.NVarChar);
                cmdMessage.Parameters.Add("@messageType", SqlDbType.NVarChar);

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
                cmdMessage.Transaction = transaction;

                try
                {
                    int inventoryId = (int)cmdInventory.ExecuteScalar();

                    cmdMessage.Parameters["@inventoryId"].Value = inventoryId;

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
                            cmdMessage.Parameters["@objectId"].Value = (int)speciesId;
                            cmdMessage.Parameters["@objectType"].Value = nameof(species);
                            cmdMessage.Parameters["@description"].Value = message.Key;
                            cmdMessage.Parameters["@messageType"].Value = message.Value.ToString();
                            cmdMessage.ExecuteNonQuery();
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
                            cmdMessage.Parameters["@objectId"].Value = inventoriedPlotId;
                            cmdMessage.Parameters["@objectType"].Value = nameof(plot);
                            cmdMessage.Parameters["@description"].Value = message.Key;
                            cmdMessage.Parameters["@messageType"].Value = message.Value.ToString();
                            cmdMessage.ExecuteNonQuery();
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
                            cmdMessage.Parameters["@objectId"].Value = measurementId;
                            cmdMessage.Parameters["@objectType"].Value = nameof(measurement);
                            cmdMessage.Parameters["@description"].Value = message.Key;
                            cmdMessage.Parameters["@messageType"].Value = message.Value.ToString();
                            cmdMessage.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                    return inventoryId;
                }
                catch
                {
                    transaction.Rollback();
                    throw new Exception();
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
        public bool InsertMeasurement(string plotCode, string species, string coverage, int inventoryId)
        {
            const string queryInventoriedPlot = "select id from inventoried_plot where inventory_id = @inventory_id and plot_code = @plot_code";
            const string querySpecies = "select id from species where name = @name";
            const string queryMeasurement = "insert into measurement (inventoried_plot_id, species_id, coverage) VALUES (@inventoried_plot_id,@species_id,@coverage)";

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
                    int? speciesId = (int?)cmdSpecies.ExecuteScalar();

                    if (speciesId != null)
                    {
                        cmdMeasurement.Parameters.AddWithValue("@inventoried_plot_id", inventoriedPlotId);
                        cmdMeasurement.Parameters.AddWithValue("@species_id", speciesId);
                        cmdMeasurement.Parameters.AddWithValue("@coverage", coverage);
                        cmdMeasurement.ExecuteNonQuery();
                        transaction.Commit();
                        return true;
                    }

                    return false;
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
        public List<MeasurementDTO> GetMeasurementsDTOForPlot(int inventoryID, string code)
        {
            List<MeasurementDTO> measurementDTOList = new List<MeasurementDTO>();

            string queryMeasurement = "SELECT m.coverage, m.species_id FROM measurement m " +
                "JOIN inventoried_plot ip ON m.inventoried_plot_id = ip.id " +
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
                SqlDataReader reader = cmdMeasurementDTO.ExecuteReader();
                //read!!
                while (reader.Read())
                {
                    int speciesId = reader.GetInt32(reader.GetOrdinal("species_id"));
                    string coverage = reader.GetString(reader.GetOrdinal("coverage"));

                    //public MeasurementDTO(int speciesid, string coverage)
                    MeasurementDTO measurementDTO = new MeasurementDTO(speciesId, coverage);
                    measurementDTOList.Add(measurementDTO);
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
                                        SqlDataReader reader = inventoriedPlotSelectCommand.ExecuteReader();
                                        while (reader.Read())
                                        {
                                            inventoriedPlotIds.Add(reader.GetInt32(reader.GetOrdinal("id")));
                                        }
                                        reader.Close();

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

                                    catch (Exception ex)
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
        public CampusDTO GetCampusDTO(int inventoryID, string campus)
        {
            //public CampusDTO(List<Plot> plots, Dictionary<string, PlotValue> plotTypes
            List<Plot> plots = new List<Plot>();

            //Join grass_plot, inventoried_plot
            const string queryGetPlots = "SELECT i.plot_code, gp.area_sq_meter, gp.campus, mt.type, i.plot_type FROM inventoried_plot i " +
                "JOIN grass_plot gp ON i.plot_code = gp.code " +
                "JOIN management_type mt on mt.id = i.management_type " +
                "WHERE i.inventory_id = @inventoryID AND gp.campus = @campus";
            const string queryPlotTypeValues = "select plot_type, count(plot_type) as count, sum(g.area_sq_meter) as area_sum from inventoried_plot i " +
                "join grass_plot g on i.plot_code = g.code group by plot_type,inventory_id,campus " +
                "having campus = @campus and inventory_id = @inventoryID";

            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmdCampusDTO = con.CreateCommand())
            using (SqlCommand cmdPlotTypeValues = con.CreateCommand())
            {
                //Parameters
                cmdCampusDTO.Parameters.AddWithValue("@inventoryID", inventoryID);
                cmdCampusDTO.Parameters.AddWithValue("@campus", campus);

                cmdPlotTypeValues.Parameters.AddWithValue("@inventoryID", inventoryID);
                cmdPlotTypeValues.Parameters.AddWithValue("@campus", campus);

                //Open connection
                con.Open();
                cmdCampusDTO.CommandText = queryGetPlots;
                cmdPlotTypeValues.CommandText = queryPlotTypeValues;
                using (SqlDataReader reader = cmdCampusDTO.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        //public Plot(string code, double areaSqMeters, string campus, ManagementType managementType, string plotType)
                        string code = reader.GetString(reader.GetOrdinal("plot_code"));
                        double areaSqMeterString = reader.GetDouble(reader.GetOrdinal("area_sq_meter"));
                        string campusValue = reader.GetString(reader.GetOrdinal("campus"));
                        string managementType = reader.GetString(reader.GetOrdinal("type"));
                        ManagementType managementTypeEnum = managementType switch
                        {
                            "Netheidsboord" => ManagementType.Netheidsboord,
                            "Schapenweide" => ManagementType.Schapenweide,
                            "Intensief" => ManagementType.Intensief,
                            "Extensief" => ManagementType.Extensief,
                            _ => throw new Exception("Invalid management type")
                        };

                        string plotTypeCode = reader.GetString(reader.GetOrdinal("plot_type"));
                        Plot plot = new Plot(code, areaSqMeterString, campusValue, managementTypeEnum, plotTypeCode);
                        plots.Add(plot);
                    }
                }
                Dictionary<string, PlotValue> plotTypes = new Dictionary<string, PlotValue>();
                using (SqlDataReader reader = cmdPlotTypeValues.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        plotTypes.Add(reader.GetString(0), new PlotValue(reader.GetInt32(1), reader.GetDouble(2)));
                    }
                }

                CampusDTO campusDTO = new CampusDTO(plots, plotTypes);
                return campusDTO;
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
                    foreach (KeyValuePair<string,MessageType> messageType in messages)
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
    }
}
