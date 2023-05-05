using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DocumentsAPI.Models
{
    public class Document
    {
        public int DocumentId { get; set; }

        public required int Amount { get; set; }

        [StringLength(1000)]
        public required string Description { get; set; }

        [JsonIgnore]
        public DocumentStatus DocumentStatus { get; set; } = new DocumentStatus();
    }
}
