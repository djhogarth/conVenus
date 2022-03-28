using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace API.DataTransferObjects
{
    public class MessageDTO
    {
        public int Id { get; set; }

        public int SenderId { get; set; }
        public string SenderUsername { get; set; }
        public string SenderPhotoUrl { get; set; }

        [JsonIgnore]
        public bool SenderDeleted { get; set; }

        public int RecipientId { get; set; }
        public string RecipientUsername { get; set; }
        public string RecipientPhotoUrl { get; set; }

        [JsonIgnore]
        public bool RecipientDeleted { get; set; }

        public string Content { get; set; }

        public DateTime? DateRead { get; set; }
        public DateTime MessageSent { get; set; }


    }
}
