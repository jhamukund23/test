using System.ComponentModel.DataAnnotations;
namespace Dss.Domain.Models;
public class User
    {
        [Key]        
        public int Id { get; set; }
        
        public Guid UserGuid { get; set; }        
        public string EmailId { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string ContactNo { get; set; }       
        public string AlternateContactNo { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTimeOffset? LastLoggedInDate { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }

        public User()
        {
            UserGuid = Guid.NewGuid();
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
            IsActive = true;
            IsDeleted = false;
        }
    }