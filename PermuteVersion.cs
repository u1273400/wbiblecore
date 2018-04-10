using System;
using System.Threading;
using System.Linq;
using wbible.Models;
using wbible.DataContexts;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

//using System.IO;
//using Newtonsoft.Json;

namespace wbible
{
    class PermuteVersion
    {
        static WBibleContext db = new WBibleContext();
        static void read_keys()
        {
            ConsoleKeyInfo keyinfo;
            do
            {
                keyinfo = Console.ReadKey();
                Console.WriteLine(keyinfo.Key + " was pressed");
            }
            while (keyinfo.Key != ConsoleKey.X);
            //to multithread this say -> new Thread(new ThreadStart())
        }
        static void read_keyb()
        {
            new Thread(new ThreadStart(read_keys)).Start();
        }
        static void Mains(string[] args)
        {
            //read_keyb();
            using (db)
            {
                //db.Stats.Add(new BookStats { BookCode = "Matthew", ChaptCount=28 });
                //var count = db.SaveChanges();
                //Console.WriteLine("{0} records saved to database", count);

                // foreach (var c in db.WXD)
                // {
                //     //Console.WriteLine("ch={0} basech={1}  allch={2} {3}",c.ch, c.basech, c.allch, c.ch=="o"?"found":"not found"); 
                //     for(int ts=0;ts<c.ch.Length;ts++)Console.WriteLine("chars={0} ",c.ch.Substring(ts,1)); 
                // }
                var bookStats=new List<BookStats>();
                int i=0;
                foreach (var stat in db.Stats)
                {
                    Console.WriteLine("({0}) {1} ",i, stat.BookCode);
                    bookStats.Add(stat);
                    i++;   
                }//éèēīíìóòōúūùāáàéèēíìīóòōúūù
                Console.WriteLine("Select Book");
                string j=Console.ReadLine();
                int n; 
                List<string> ct,pt=new List<string>(),st=new List<string>();
                bool isNumeric = int.TryParse(j.Trim(), out n);
                if(isNumeric && n<(from xx in db.Stats select xx).ToList().Count){
                    Console.WriteLine((from xx in db.Corpus select xx).ToList().Count);//count verses
                    foreach(var c in bookStats[n].Corpus){//for each verse
                        i=0;
                        ct=new List<string>(c.CText.Split());
                        Console.WriteLine("word count="+ct.Count);
                        for(int k=0;k<ct.Count;k++){
                            Console.Write("\n({0}) ",c.Verse);
                            printVerse(ct,i);
                            for(int cc=0;cc<ct[i].Length;cc++){
                                Console.Write("Processing '{0}' ...",ct[i].ToLower().Substring(cc,1));
                                var xch=(from x in db.WXD where x.ch == ct[i].ToLower().Substring(cc,1) select x).ToList();
                                Console.WriteLine("...found ({0})",xch.Count);
                                if(xch.Count>0)
                                    permuted(ct[i],st,xch[0].allch.Split(','),cc);
                            }
                            i++;
                            pt.Add(SelectedWord(st));
                            st.Clear();
                        }
                        //pt=new ArrayList(c.PText.Split());
                    }
                }
            }
        }
        static string SelectedWord(List<string> words){
            for(int i=0;i<words.Count;i++){
                Console.WriteLine("({0}) {1}",i,words[i]);
            }
            Console.WriteLine("Select Transcription");
            string j=Console.ReadLine();
            int n; 
            bool isNumeric = int.TryParse(j.Trim(), out n);
            if(isNumeric && n<words.Count) 
                return words[n];
            else
                return words[0];
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
            if(d.Count==0){
                foreach(var x in cenum){
                    string dest="";
                    for(int i=0;i<csrc.Length;i++){
                        if(i==idx)
                            dest+=x;
                        else
                            dest+=csrc.Substring(i,1);
                    }d.Add(dest);
                    //Console.WriteLine("Word Added: {0}",dest);                
                }
            }
            else{ 
                d.AddRange(diap(d,cenum,idx));
            }
        }
        static List<string> diap(List<string> d, string[] cenum, int idx){
            var dlist=new List<string>();
            int j=0;
            //return dlist;
            foreach(var word in d){
                string tmp="";
                for(var k=0;k<word.Length;k++){
                    //Console.Write("Searching for '{0}'  to replace with {2}{1}...",word.ToLower().Substring(k,1),cenum[0],idx);
                    //var xch=(from x in db.WXD where x.ch == word.ToLower().Substring(k,1) select x).ToList();
                    if(k>0 && 
                            //(xch.Count>0
                            ("éèēīíìóòōúūùā".Contains(word.ToLower().Substring(k,1))
                            ||Regex.Match(word.Substring(k,1), @"^[a-zA-Z]*$", RegexOptions.IgnoreCase).Success))
                        tmp+=','+word.Substring(k,1);
                    else
                        tmp+=word.Substring(k,1);
                    //Console.WriteLine("...found ({0})","éèēīíìóòōúūùāáàéèēíìīóòōúūù".Contains(word.ToLower().Substring(k,1)));
                }Console.WriteLine("{0} dlist={1} d={2}",tmp,dlist.Count, d.Count);
                foreach(var x in cenum){
                    string dest="";
                    for(int i=0;i<tmp.Split(',').Length;i++){
                        if(i==idx)
                            dest+=x;
                        else
                            dest+=tmp.Split(',')[i];
                        if(j++>10000)return dlist;
                   }
                    dlist.Add(dest);
                }
            }
            return dlist;
        }
        static string getwdic(){
            return @"[
                {
                    'id': 1,
                    'ch': 'a',
                    'basech': 'a',
                    'allch': 'a,ā,á,à',
                    'vowel': 'TRUE'
                },
                {
                    'id': 2,
                    'ch': 'à',
                    'basech': 'a',
                    'allch': 'a,ā,á,à',
                    'vowel': 'TRUE'
                },
                {
                    'id': 3,
                    'ch': 'ā',
                    'basech': 'a',
                    'allch': 'a,ā,á,à',
                    'vowel': 'TRUE'
                },
                {
                    'id': 4,
                    'ch': 'á',
                    'basech': 'a',
                    'allch': 'a,ā,á,à',
                    'vowel': 'TRUE'
                },
                {
                    'id': 5,
                    'ch': 'e',
                    'basech': 'e',
                    'allch': 'e,é,è,ē,ẹ́,è̩,ẹ̄',
                    'vowel': 'TRUE'
                },
                {
                    'id': 6,
                    'ch': 'ē',
                    'basech': 'e',
                    'allch': 'e,é,è,ē,ẹ́,è̩,ẹ̄',
                    'vowel': 'TRUE'
                },
                {
                    'id': 7,
                    'ch': 'é',
                    'basech': 'e',
                    'allch': 'e,é,è,ē,ẹ́,è̩,ẹ̄',
                    'vowel': 'TRUE'
                },
                {
                    'id': 8,
                    'ch': 'è',
                    'basech': 'e',
                    'allch': 'e,é,è,ē,ẹ́,è̩,ẹ̄',
                    'vowel': 'TRUE'
                },
                {
                    'id': 9,
                    'ch': 'ẹ́',
                    'basech': 'e',
                    'allch': 'e,é,è,ē,ẹ́,è̩,ẹ̄',
                    'vowel': 'TRUE'
                },
                {
                    'id': 10,
                    'ch': 'è̩',
                    'basech': 'e',
                    'allch': 'e,é,è,ē,ẹ́,è̩,ẹ̄',
                    'vowel': 'TRUE'
                },
                {
                    'id': 11,
                    'ch': 'ẹ̄',
                    'basech': 'e',
                    'allch': 'e,é,è,ē,ẹ́,è̩,ẹ̄',
                    'vowel': 'TRUE'
                },
                {
                    'id': 12,
                    'ch': 'i',
                    'basech': 'i',
                    'allch': 'i,í,ì,ī,ị̄,ị́,ị̀',
                    'vowel': 'TRUE'
                },
                {
                    'id': 13,
                    'ch': 'í',
                    'basech': 'i',
                    'allch': 'i,í,ì,ī,ị̄,ị́,ị̀',
                    'vowel': 'TRUE'
                },
                {
                    'id': 14,
                    'ch': 'ì',
                    'basech': 'i',
                    'allch': 'i,í,ì,ī,ị̄,ị́,ị̀',
                    'vowel': 'TRUE'
                },
                {
                    'id': 15,
                    'ch': 'ī',
                    'basech': 'i',
                    'allch': 'i,í,ì,ī,ị̄,ị́,ị̀',
                    'vowel': 'TRUE'
                },
                {
                    'id': 16,
                    'ch': 'ị̄',
                    'basech': 'i',
                    'allch': 'i,í,ì,ī,ị̄,ị́,ị̀',
                    'vowel': 'TRUE'
                },
                {
                    'id': 17,
                    'ch': 'ị́',
                    'basech': 'i',
                    'allch': 'i,í,ì,ī,ị̄,ị́,ị̀',
                    'vowel': 'TRUE'
                },
                {
                    'id': 18,
                    'ch': 'ị̀',
                    'basech': 'i',
                    'allch': 'i,í,ì,ī,ị̄,ị́,ị̀',
                    'vowel': 'TRUE'
                },
                {
                    'id': 19,
                    'ch': 'o',
                    'basech': 'o',
                    'allch': 'o,ó,ò,ō,ọ́,ọ̀,ọ̄',
                    'vowel': 'TRUE'
                },
                {
                    'id': 20,
                    'ch': 'ó',
                    'basech': 'o',
                    'allch': 'o,ó,ò,ō,ọ́,ọ̀,ọ̄',
                    'vowel': 'TRUE'
                },
                {
                    'id': 21,
                    'ch': 'ō',
                    'basech': 'o',
                    'allch': 'o,ó,ò,ō,ọ́,ọ̀,ọ̄',
                    'vowel': 'TRUE'
                },
                {
                    'id': 22,
                    'ch': 'ò',
                    'basech': 'o',
                    'allch': 'o,ó,ò,ō,ọ́,ọ̀,ọ̄',
                    'vowel': 'TRUE'
                },
                {
                    'id': 23,
                    'ch': 'ọ́',
                    'basech': 'o',
                    'allch': 'o,ó,ò,ō,ọ́,ọ̀,ọ̄',
                    'vowel': 'TRUE'
                },
                {
                    'id': 24,
                    'ch': 'ọ̀',
                    'basech': 'o',
                    'allch': 'o,ó,ò,ō,ọ́,ọ̀,ọ̄',
                    'vowel': 'TRUE'
                },
                {
                    'id': 25,
                    'ch': 'ọ̄',
                    'basech': 'o',
                    'allch': 'o,ó,ò,ō,ọ́,ọ̀,ọ̄',
                    'vowel': 'TRUE'
                },
                {
                    'id': 26,
                    'ch': 'u',
                    'basech': 'u',
                    'allch': 'u,ú,ū,ù,ụ́,ụ̄,ụ̀',
                    'vowel': 'TRUE'
                },
                {
                    'id': 27,
                    'ch': 'ú',
                    'basech': 'u',
                    'allch': 'u,ú,ū,ù,ụ́,ụ̄,ụ̀',
                    'vowel': 'TRUE'
                },
                {
                    'id': 28,
                    'ch': 'ū',
                    'basech': 'u',
                    'allch': 'u,ú,ū,ù,ụ́,ụ̄,ụ̀',
                    'vowel': 'TRUE'
                },
                {
                    'id': 29,
                    'ch': 'ù',
                    'basech': 'u',
                    'allch': 'u,ú,ū,ù,ụ́,ụ̄,ụ̀',
                    'vowel': 'TRUE'
                },
                {
                    'id': 30,
                    'ch': 'ụ́',
                    'basech': 'u',
                    'allch': 'u,ú,ū,ù,ụ́,ụ̄,ụ̀',
                    'vowel': 'TRUE'
                },
                {
                    'id': 31,
                    'ch': 'ụ̄',
                    'basech': 'u',
                    'allch': 'u,ú,ū,ù,ụ́,ụ̄,ụ̀',
                    'vowel': 'TRUE'
                },
                {
                    'id': 32,
                    'ch': 'ụ̀',
                    'basech': 'u',
                    'allch': 'u,ú,ū,ù,ụ́,ụ̄,ụ̀',
                    'vowel': 'TRUE'
                },
                {
                    'id': 33,
                    'ch': 'b',
                    'basech': 'b',
                    'allch': 'b, ḅ',
                    'vowel': 'FALSE'
                },
                {
                    'id': 34,
                    'ch': 'ḅ',
                    'basech': 'b',
                    'allch': 'b, ḅ',
                    'vowel': 'FALSE'
                },
                {
                    'id': 35,
                    'ch': 'd',
                    'basech': 'd',
                    'allch': 'd, ḍ',
                    'vowel': 'FALSE'
                },
                {
                    'id': 36,
                    'ch': 'ḍ',
                    'basech': 'd',
                    'allch': 'd, ḍ',
                    'vowel': 'FALSE'
                }
            ]";
        }
    }
}
