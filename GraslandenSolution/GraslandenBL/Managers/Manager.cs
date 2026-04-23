using GraslandenBL.Domain;
using GraslandenBL.DTOs;
using GraslandenBL.Enums;
using GraslandenBL.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace GraslandenBL.Managers
{
    public class Manager
    {
        private IRepository _repository;

        public Manager(IRepository repository)
        {
            _repository = repository;
        }

        public bool DeleteInventory(int inventoryId)
        {
            return _repository.DeleteInventory(inventoryId);
        }

        public List<CampusDTO> GetAllCampusesDTO(int inventoryID)
        {
            return _repository.GetAllCampusesDTO(inventoryID);
        }

        public List<InventoryDTO> GetInventoryDTOs()
        {
            return _repository.GetInventoryDTOs();
        }

        public int ImportEmptyInventory(InventoryDTO inventoryDTO)
        {
            return _repository.ImportEmptyInventory(inventoryDTO);
        }
        // returns false if speciesName not found
        public MeasurementDTO InsertMeasurement(string plotCode, string species, string coverage, int inventoryId)
        {
            return _repository.InsertMeasurement(plotCode, species, coverage, inventoryId);
        }

        public List<MeasurementDTO> GetSpeciesOfPlot(Plot currentPlot, int currentInventoryId)
        {
            return _repository.GetMeasurementsDTOForPlot(currentInventoryId, currentPlot.Code);
        }

        public void DeleteMeasurement(int measurementId)
        {
            _repository.DeleteMeasurement(measurementId);
        }

        public Plot AddPlotToInventory(int inventoryID, string code, ManagementType managementType, string plotType)
        {
            return _repository.InsertInventoriedPlot(inventoryID, code, managementType, plotType);
        }

        public Dictionary<string, Dictionary<string, MessageType>> GetAllMessages()
        {
            return _repository.GetAllMessages();
        }
    }
}
