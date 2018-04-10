using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace wbible.Models
{    public class BookStats
    {
        public int BookStatsId { get; set; }
        public string BookCode { get; set; }
        public int ChaptCount {get; set; }
        public int VerseCount {get; set; }
        public int WordCount {get; set; }
        public int Testament {get; set; }
        public List<Corpus> Corpus {get; set;}

        public override string ToString(){
            return BookCode;
        }
    }
}