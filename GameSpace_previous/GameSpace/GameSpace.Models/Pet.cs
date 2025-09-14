using System;

namespace GameSpace.Models
{
    public class Pet
    {
        public int PetID { get; set; }
        public int UserID { get; set; }
        public string PetName { get; set; } = string.Empty;
        public string PetType { get; set; } = string.Empty;
        public int Level { get; set; }
        public int Experience { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
