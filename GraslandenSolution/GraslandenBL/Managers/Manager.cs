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

        public List<InventoryDTO> GetInventoryDTOs()
        {
            return _repository.GetInventorieDTOs();
        }
    }
}
