using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace wbible.Models
{    public class Readers
    {
        public int ReadersId { get; set; }
        public string Surname { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public int AgeRange { get; set; }
        public List<Corpus> Corpus {get; set;}
    }
}