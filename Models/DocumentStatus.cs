using DocumentsAPI.Enums;
using System.ComponentModel.DataAnnotations;

namespace DocumentsAPI.Models
{
    public class DocumentStatus
    {
        public int DocumentStatusId { get; set; }

        public int DocumentId { get; set; }

        public STATUS StatusId { get; set; } = STATUS.CREATED;

        public DateTime DateTime { get; set; } = DateTime.UtcNow;

    }
}
