using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Configuration
{
    public class Files
    {
        [Required(ErrorMessage = "You must provide a path to the file for logging")]
        [RegularExpression(@"^Logs\/[\w-]+\.txt$", ErrorMessage = "Invalid path")]
        public string LoggingFile { get; init; } = string.Empty;
    }
}
