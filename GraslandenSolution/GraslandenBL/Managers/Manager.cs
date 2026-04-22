using GraslandenBL.DTOs;
using GraslandenBL.Interfaces;
using System;
using System.Collections.Generic;
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

        public HashSet<String> GetAllCampuses()
        {
            return _repository.GetAllCampuses();
        }

        public List<InventoryDTO> GetInventoryDTOs()
        {
            return _repository.GetInventoryDTOs();
        }

        public int ImportEmptyInventory(InventoryDTO inventoryDTO)
        {
            return _repository.ImportEmptyInventory(inventoryDTO);
        }
    }
}
