using System.ComponentModel.DataAnnotations;

namespace DocumentsAPI.Requests
{
    public class AddDocumentRequest
    {
        [Required]
        public required int Amount { get; set; }

        [Required]
        [StringLength(1000)]
        public required string Description { get; set; }

    }
}
