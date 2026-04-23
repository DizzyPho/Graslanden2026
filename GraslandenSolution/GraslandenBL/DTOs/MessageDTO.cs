namespace GraslandenBL.DTOs
{
    public class MessageDTO
    {
        public string InventoryName { get; set; }

        public string Description { get; set; }

        public string MessageType { get; set; }

        public MessageDTO(string inventoryName, string description, string messageType)
        {
            InventoryName = inventoryName;
            Description = description;
            MessageType = messageType;
        }
    }
}
