
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace wbible.Models
{    public class  WxD
    {
        public int id{get; set; }
        public string ch {get; set;}
        public string basech {get;set;}
        public string allch {get; set;}
        public bool vowel {get; set;}
    }
}
