using System.ComponentModel.DataAnnotations;

namespace BenchStoreMVC.ViewModels
{
    public class UploadResultFileViewModel
    {
        [Display(Name = "Result File", Description = "(.xml or .xml.bz2)")]
        public required IFormFile ResultFile { get; set; }

        [Display(Name = "Logfiles", Description = "(Optional, .zip)")]
        public IFormFile? LogFiles { get; set; }
    }
}

