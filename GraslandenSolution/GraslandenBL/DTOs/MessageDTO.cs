namespace GraslandenBL.DTOs
{
    public class MessageDTO
    {
        public int? ObjectId { get; set; }

        public int InventoryId { get; set; }

        public string Description { get; set; }

        public string MessageType { get; set; }

        public MessageDTO(int? objectId,  int inventoryId, string description, string messageType)
        {
            ObjectId = objectId;
            InventoryId = inventoryId;
            Description = description;
            MessageType = messageType;
        }
    }
}
