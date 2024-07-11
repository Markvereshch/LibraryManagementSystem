using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Configuration
{
    public class ConnectionStrings
    {
        [Required(ErrorMessage = "You must provide a connection string to your SQL Database")]
        [RegularExpression(@"^Server=[\w-\\]+;Database=[\w]+;TrustServerCertificate=(True|False);Trusted_Connection=True;MultipleActiveResultSets=(True|False)$",
            ErrorMessage = "Invalid SQL SERVER connection string")]
        public string DefaultSQLConnection { get; init; } = string.Empty;
    }
}
