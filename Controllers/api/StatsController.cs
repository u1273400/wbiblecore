using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using wbible.Models;
using wbible.DataContexts;
using Microsoft.EntityFrameworkCore;

namespace wbible.Controllers.api
{
    public class StatsController : Controller
    {
        private WBibleContext db = new WBibleContext();

        [HttpGet("api/search")]
        public IQueryable<BookStats> GetSearch(int top, int skip, String query)
        {
            //Console.WriteLine("top="+top);
            IQueryable<BookStats> songs2 = from songs in db.Stats
                select songs;
            //Console.WriteLine("skip="+skip);
            return songs2.Skip(top * skip).Take(top);
        }

        [HttpGet("api/search/count")]
        public IActionResult GetSearchCount(String query)
        {
            //Console.WriteLine("query="+query);
            //return Ok(query);
            IQueryable<BookStats> songs2 = from songs in db.Stats
                //orderby songs.Title
                //where songs.Title.ToString().ToLower().Contains(query.ToLower())
                select songs;
            Console.WriteLine("count="+songs2.Count());
            return Ok(songs2.Count());
        }

        [HttpGet("api/songs/count")]
        public IActionResult GetSongCount()
        {
            //Console.WriteLine("here");
            return Ok(db.Stats.Count());
        }
        
       [HttpGet("api/allbooks")]
        public IActionResult GetStats()
        {
            //(from xx in db.Stats where xx.BookStatsId==c.BookStatsId select xx.BookCode).SingleOrDefault()
            //var bookids =(from xx in db.Corpus select xx.BookStatsId).Distinct().ToList();
            IQueryable<BookStats> stats = from stat in db.Stats join book in db.Corpus on stat.BookStatsId equals book.BookStatsId select stat ;
            var res = from x in stats.Distinct() orderby x.BookStatsId ascending
            select new  { BookStatsId = x.BookStatsId, BookCode = x.BookCode, 
            pc=db.Corpus.Count(e => e.BookStatsId==x.BookStatsId && String.IsNullOrEmpty(e.PText))*1.0 /
            x.VerseCount*100
             };
            return Ok(res);
        }

        // GET: api/bookchapts/id
        [HttpGet("api/bookchapts")]
        //public IQueryable<BookStats> GetBookChapts(int id)
        public IActionResult GetBookChapts(int id)
        {
            //Console.WriteLine($"id={id}");
            var chapts = (from xx in db.Corpus where xx.BookStatsId==id select xx.Chapt).Distinct();
            //var v = (from xx in db.Stats where xx.BookStatsId==id select xx.VerseCount).SingleOrDefault();
            var res=(from x in chapts orderby x ascending select new {chapt=x, 
            pc=db.Corpus.Count(c => c.BookStatsId==id && !String.IsNullOrEmpty(c.PText) && c.Chapt==x)*1.0/
            db.Corpus.Count(c => c.BookStatsId==id && c.Chapt==x) *100
            });
            //var res=new List<KeyValuePair<int,int>>();
            //var res=new List<int,int>();
            if (chapts == null || res == null)
            {
                return NotFound();
            }
            return Ok(res);
        }
        // PUT: api/bookchapts/id/chapt
        [HttpGet("api/verses")]
        //public IQueryable<BookStats> GetBookVerses(int id)
        public IActionResult GetBookVs(int id,int chapt)
        {
            Console.WriteLine($"id={id} chapt={chapt}");
            //return Ok();
            var v = (from xx in db.Corpus where xx.BookStatsId==id && xx.Chapt==chapt && xx.Verified==false orderby xx.Verse ascending select xx);
            if (v == null)
            {
                return NotFound();
            }
            Console.WriteLine($"Found {v.Count()} items..");
            return Ok(v);
        }

        // PUT: api/saveverse/id
        [HttpPut("api/saveverse")]
        //public IQueryable<BookStats> SaveVerse(int id)
        public IActionResult SaveVerse(int id,[FromBody] Corpus ptext)
        //public IActionResult SaveVerse(int id,Corpus ptext)
        {
            Console.WriteLine($"StatsController.SaveVerse(?): id={id} text={ptext} isnull="+(ptext==null?"true":"false"));
            //db.Corpus.Find(id).PText=ptext;
            db.Entry(ptext).State = EntityState.Modified;
            //return Ok($"test complete {ptext}");
            try
            {
                db.SaveChanges();
            }
            catch (Exception x)
            {
                if (!VerseExists(id))
                {
                    return NotFound();
                }
                else
                {
                    return BadRequest(x.Message);
                }
            }
            //return StatusCode(HttpStatusCode.NoContent);
            return Ok("Save Successful!");
        }

        // // PUT: api/Songs2/5
        // [ResponseType(typeof(void))]
        // public IHttpActionResult PutSong(int id, Song song)
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         return BadRequest(ModelState);
        //     }

        //     if (id != song.Id)
        //     {
        //         return BadRequest();
        //     }

        //     db.Entry(song).State = EntityState.Modified;

        //     try
        //     {
        //         db.SaveChanges();
        //     }
        //     catch (DbUpdateConcurrencyException)
        //     {
        //         if (!SongExists(id))
        //         {
        //             return NotFound();
        //         }
        //         else
        //         {
        //             throw;
        //         }
        //     }

        //     return StatusCode(HttpStatusCode.NoContent);
        // }

        // // POST: api/Songs2
        // [ResponseType(typeof(Song))]
        // public IHttpActionResult PostSong(Song song)
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         return BadRequest(ModelState);
        //     }

        //     db.Songs.Add(song);
        //     db.SaveChanges();

        //     return CreatedAtRoute("DefaultApi", new { id = song.Id }, song);
        // }

        // DELETE: api/delete
        [HttpDelete("api/delete")]
        public IActionResult DeleteVerse(long id)
        {
            Corpus v = db.Corpus.Find(id);
            Console.WriteLine($"StatsController.DeleteVerse(?): id={id} text={v} isnull="+(v==null?"true":"false"));
            if (v == null)
            {
                return NotFound();
            }

            db.Corpus.Remove(v);
            db.SaveChanges();

            return Ok("Deleted!");
        }

        // protected override void Dispose(bool disposing)
        // {
        //     if (disposing)
        //     {
        //         db.Dispose();
        //     }
        //     base.Dispose(disposing);
        // }

        private bool VerseExists(int id)
        {
            return db.Corpus.Count(e => e.CorpusId == id) > 0;
        }
    }
}