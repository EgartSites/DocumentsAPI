using DocumentsAPI.Enums;
using System.ComponentModel.DataAnnotations;

namespace DocumentsAPI.Models
{
    public class Status
    {
        public STATUS StatusId { get; set; }

        public required string Name { get; set; }

    }

}
