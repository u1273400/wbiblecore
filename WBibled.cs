using System;
using System.IO;
using System.Threading;
using System.Linq;
using wbible.Models;
using wbible.DataContexts;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;

//using System.IO;
//using Newtonsoft.Json;

namespace wbible
{
    class WBibled
    {
        static WBibleContext db = new WBibleContext();
        public static void webthread()
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls("http://0.0.0.0:8082")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
        public static void Verses()
        {
            using (db)
            {
                // foreach (var c in db.Corpus)
                // {
                //     Console.WriteLine("{0} {1} ", c.Chapt,(from xx in db.Stats where xx.BookStatsId==c.BookStatsId select xx.BookCode).SingleOrDefault()); 
                // }

                // foreach (var c in db.WXD)
                // {
                //     //for(int ts=0;ts<c.ch.Length;ts++)Console.WriteLine("chars={0} ",c.ch.Substring(ts,1)); 
                //     for(int ts=0;ts<c.ch.Length;ts++)Console.WriteLine("chars= {0} found= {1}",c.ch.Substring(ts,1),"éèēīíìóòōúūùāẹịọụiouâḅḍ".Contains(c.ch.Substring(ts,1))); 
                // }

                var bookStats=new List<BookStats>();
                int i=0;
                foreach (var stat in db.Stats.Where(x=>db.Corpus.Count(v=>v.BookStatsId==x.BookStatsId)>0)) 
                {
                    Console.WriteLine("({0}) {1} ",i, stat.BookCode);
                    bookStats.Add(stat);
                    i++;   
                }
                //check book
                //Console.WriteLine(bookStats[0].WordCount+bookStats[0].BookCode);
                Console.Write("Select Book: ");
                string j=Console.ReadLine();
                int n; 
                List<string> ct,pt,st=new List<string>();
                bool isNumeric = int.TryParse(j.Trim(), out n);
                int id= bookStats[n].BookStatsId;
                // var chapts = (from xx in db.Corpus where xx.BookStatsId==id select xx.Chapt).Distinct();
                // Console.WriteLine((from xx in db.Stats where xx.BookStatsId==id select xx.VerseCount).SingleOrDefault());
                // var v = (from xx in db.Stats where xx.BookStatsId==id select xx.VerseCount).SingleOrDefault();
                //foreach(var t in res){Console.WriteLine($"{t.chapt} {t.pc}");}return;
                //Console.WriteLine(db.Corpus.Count(c => c.BookStatsId==id && String.IsNullOrEmpty(c.PText) && c.Chapt==28));
                //Console.WriteLine(db.Corpus.Count(c => c.BookStatsId==id && c.Chapt==28));
                if(isNumeric && n<(from xx in db.Stats select xx).ToList().Count){
                    Console.WriteLine("bookstats corpus {0}",bookStats[n].BookCode);
                    foreach (var c in db.Corpus)(from xx in db.Stats where xx.BookStatsId==c.BookStatsId select xx.BookCode).SingleOrDefault();
                    var chapters=(from xx in bookStats[n].Corpus select xx.Chapt.ToString()).Distinct().ToList();
                    int chapt=selection(chapters,"Select Chapter");
                    if(chapt==-1){
                        Console.WriteLine("Invalid Selection");
                        return;
                    }
                    var verses=(from xx in bookStats[n].Corpus where xx.Chapt==Int32.Parse(chapters[chapt]) orderby xx.Verse ascending select xx).ToList();
                    foreach(var c in verses){//for each verse
                        i=0;
                        if(c.PText==null)c.PText="";
                        ct=new List<string>(c.CText.Split());
                        pt=new List<string>(c.PText.Split());
                        for(int t=0;t<pt.Count;t++)ct[t]=pt[t];
                        Console.Write(@"{1} chapter {2} Verse {0} of having {4} words is {3:0.00}% Complete. \nPress <Enter> to skip verse or any other key to continue... ",c.Verse,
                                    (from xx in db.Stats where xx.BookStatsId==c.BookStatsId select xx.BookCode).SingleOrDefault(),
                                    c.Chapt,pt.Count*100.0/(ct.Count*1.0),ct.Count);
                        if(Console.ReadKey().Key == ConsoleKey.Enter) continue;
                        for(int k=0;k<ct.Count;k++){
                            Console.Write("\n({0}) ",c.Verse);
                            printVerse(ct,i);
                            string sword=ct[i];
                            int o=0;
                            for(int cc=0;cc<ct[i].Length;cc++){
                                //Console.Write("Processing '{0}' ...",ct[i].ToLower().Substring(cc,1));
                                var xch=(from x in db.WXD where x.ch == ct[i].ToLower().Substring(cc,1) select x).ToList();
                                //Console.WriteLine("...found ({0})",xch.Count);
                                if(xch.Count>0){
                                    permuted(sword,st,xch[0].allch.Split(','),cc);
                                    sword=SelectedWord(st,cc,ct[i],out o);
                                }
                                st.Clear();
                                if(o==-1)break;
                            }
                            ct[i]=sword;
                            if(i<pt.Count)pt[i]=sword; else pt.Add(sword);
                            db.Corpus.Find(c.CorpusId).PText=String.Join(" ",pt.ToArray());
                            db.SaveChanges();
                            i++;
                        }
                        //pt=new ArrayList(c.PText.Split());
                    }
                }
            }
        }
        static int selection(List<String> list, string msg){
            int i=0;
            foreach (var line in list)
            {
                Console.WriteLine("({0}) {1} ",i, line);
                i++;   
            }
            Console.Write("{0} ",msg);
            string j=Console.ReadLine();
            int n; 
            bool isNumeric = int.TryParse(j.Trim(), out n);
            if(isNumeric && n<list.Count) return n; else return -1;
        }
        static string SelectedWord(List<string> words, int cc, string ow, out int o){
            Console.WriteLine();
            for(int i=0;i<words.Count;i++){
                string[] tmp=diap(words[i]);
                Console.Write("({0})",i);
                for(int kd=0;kd<tmp.Length;kd++)
                    if(kd==cc){
                        var pcol=Console.ForegroundColor;
                        Console.ForegroundColor=ConsoleColor.Red;
                        Console.Write("{0}",tmp[kd]);
                        Console.ForegroundColor=pcol;
                    }else{
                        Console.Write("{0}",tmp[kd]);
                    }
                Console.Write(" ");
            }
            string j="";
            if(words.Count>10) j=Console.ReadLine();
            else  j=Console.ReadKey().KeyChar.ToString();
            if(j.Equals(" ")||j.Equals("\n"))o=-1; else o=0;
            int n; 
            bool isNumeric = int.TryParse(j.Trim(), out n);
            if(isNumeric && n<words.Count) 
                return words[n];
            else
                return ow;
        }
        static void printVerse(List<string> ct, int i){
            for(int k=0;k<ct.Count;k++)
                if(k==i){
                    var pcol=Console.ForegroundColor;
                    Console.ForegroundColor=ConsoleColor.Red;
                    Console.Write("{0} ",ct[k]);
                    Console.ForegroundColor=pcol;
                }else{
                    Console.Write("{0} ",ct[k]);
                }
            Console.WriteLine();
        }

        static void permuted(string csrc, List<string> d, string[] cenum,int idx){
            string[] tmp=diap(csrc);
            foreach(var x in cenum){
                string dest="";
                for(int i=0;i<tmp.Length;i++){
                    if(i==idx)
                        dest+=x;
                    else
                        dest+=tmp[i].Trim();
                }d.Add(dest);
            }
        }
        public static string[] diap(string w){
            string tmp="";
            for(var k=0;k<w.Length;k++){
                //Console.Write("Searching for '{0}'  to replace with {2}{1}...",words[i].ToLower().Substring(k,1),cenum[0],idx);
                //var xch=(from x in db.WXD where x.ch == words[i].ToLower().Substring(k,1) select x).ToList();
                if(k>0 && 
                        ("éèēīíìóòōúūùāẹịọụiouâḅḍ-".Contains(w.ToLower().Substring(k,1))
                        ||Regex.Match(w.Substring(k,1), @"^[a-zA-Z]*$", RegexOptions.IgnoreCase).Success))
                    tmp+=','+w.Substring(k,1);
                else
                    tmp+=w.Substring(k,1);
                //Console.WriteLine("...found ({0})","éèēīíìóòōúūùāáàéèēíìīóòōúūù".Contains(words[i].ToLower().Substring(k,1)));
            }//Console.WriteLine("{0}",tmp);
            return tmp.Split(',');
        }
    }
}