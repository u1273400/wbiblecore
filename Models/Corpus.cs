using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace wbible.Models
{    public class Corpus
    {
        public int CorpusId { get; set; }
        public int BookStatsId { get; set; }
        public int ReadersId { get; set; }
        public int Chapt { get; set; }
        public int Verse { get; set; }
        public string CText { get; set; }
        public string UText { get; set; }
        public string PText { get; set; }
        public string Audio { get; set; }
        public string Image { get; set; }
        public string Image2 { get; set; }
        public bool Verified {get; set; }
        
        public override string ToString(){
            return PText;
        }
    }
}