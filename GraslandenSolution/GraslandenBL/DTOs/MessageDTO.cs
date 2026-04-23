namespace GraslandenBL.DTOs
{
    public class MessageDTO
    {

        public string Description { get; set; }

        public string MessageType { get; set; }

        public MessageDTO(string description, string messageType)
        {
            Description = description;
            MessageType = messageType;
        }
    }
}
