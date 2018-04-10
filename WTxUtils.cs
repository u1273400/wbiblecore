using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Diagnostics;
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
    class WTxUtils
    {
        static WBibleContext db = new WBibleContext();
        public static void tp(string ar)
        {
            //Console.WriteLine(ar);
            ///Initialise variables
            using (db){
                switch(ar){
                    case "v2db":
                        verses2db();
                        break;
                    case "basech":
                        basechtp();
                        break;
                    case "base2":
                        basechtp2();
                        break;
                    case "xorr":
                        xokrnnres();
                        break;
                    case "p2s":
                        punc2sents();
                        break;
                    
                }
            }
        }
        static void basechtp(){
            var files = Directory.GetFiles(Path.Combine(new string[]{Directory.GetCurrentDirectory(),"txts2"}));
            StringBuilder stringBuilder=new StringBuilder();
            foreach(var f in files){
                var lines = File.ReadAllLines(f);
                foreach(var line in lines){
                    foreach(var word in line.Split()){
                        if (word.Trim().Length==0 || Char.IsDigit(word,0))continue;
                        var getchars = WBibled.diap(word);
                        for(int i=0;i<getchars.Length;i++){
                            var bch=(from x in db.WXD where x.ch == getchars[i].ToLower() select x.basech).ToList();
                            if(bch.Count>0){
                                //if(!Regex.Match(getchars[i], @"^[a-zA-Z]*$", RegexOptions.IgnoreCase).Success)
                                    //Console.WriteLine($"Found {getchars[i]}..Replacing with {bch[0]} ...");
                                getchars[i]=bch[0];                           
                            }
                        }var new_word1=String.Join("",getchars)+" ";
                        var new_word="";
                        for(int i=0;i<new_word1.Length;i++){
                            if(Char.IsDigit(new_word1,i))continue;
                            new_word+=new_word1.Substring(i,1);
                        }
                        stringBuilder.Append(Regex.Replace(new_word, @"[^\u0000-\u007F]+", string.Empty));
                    }
                }
            }Console.WriteLine(stringBuilder.ToString());
        }
        static void basechtp2(){
            var files = Directory.GetFiles(Path.Combine(new string[]{Directory.GetCurrentDirectory(),"txts2"}));
            var outfiles = Path.Combine(new string[]{Directory.GetCurrentDirectory(),"txts"});
            StringBuilder stringBuilder=new StringBuilder();
            foreach(var f in files){
                var lines = File.ReadAllLines(f);
                var outfile= Path.Combine(new string[]{outfiles,f.Split('/')[f.Split('/').Length-1]});
                Console.WriteLine($"Writing to {outfile} ..");
                foreach(var line in lines){
                    foreach(var word in line.Split()){
                        //if (word.Trim().Length==0 || Char.IsDigit(word,0))continue;
                        if (word.Trim().Length==0 )continue;
                        var getchars = WBibled.diap(word);
                        for(int i=0;i<getchars.Length;i++){
                            var bch=(from x in db.WXD where x.ch == getchars[i].ToLower() select x.basech).ToList();
                            if(bch.Count>0){
                                //if(!Regex.Match(getchars[i], @"^[a-zA-Z]*$", RegexOptions.IgnoreCase).Success)
                                    //Console.WriteLine($"Found {getchars[i]}..Replacing with {bch[0]} ...");
                                getchars[i]=bch[0];                           
                            }
                        }var new_word1=String.Join("",getchars)+" ";
                        var new_word="";
                        for(int i=0;i<new_word1.Length;i++){
                            if(Char.IsDigit(new_word1,i))continue;
                            new_word+=new_word1.Substring(i,1);
                        }
                        stringBuilder.Append(Regex.Replace(new_word, @"[^\u0000-\u007F]+", string.Empty));
                    }
                }
                File.WriteAllText(outfile, stringBuilder.ToString());
                stringBuilder.Clear();
            }
        }
        static void xokrnnres(){
            string[] lines=File.ReadAllLines("web-result-1.txt");
            var sb = new StringBuilder();
            var processing=false;
            foreach (var line in lines)
            {
                //var adding=false;
                if(line.StartsWith("TRAINING STATS:"))
                    sb.AppendLine(line);
                if(line.Contains("Generating random text from learned"))
                    processing=true;
                if(line.Contains("End of generation"))
                    processing=false;
                if(processing)
                    sb.AppendLine(line);
            }
            //Console.Write(data.Count+", "+(990-data.Count));
            Console.Write(sb.ToString());
        }
        static void punc2sents(){
            string[] lines=File.ReadAllLines("okall_upper.txt");
            var sb = new StringBuilder();
            var data=new List<string>();
            foreach (var line in lines)
            {
                var arr=line.Split('!');
                foreach(var l in arr){
                    data.AddRange(new List<string>(line.Split('.')));
                    // var t=l.Split('.');
                    // foreach(var n in t){
                    //     data.Add(n);
                    // }
                }
            }
            foreach (var line in data){
                sb.AppendLine();
            }
            Console.Write(sb.ToString());

        }
        static void verses2db(){
            bool WriteDB=false; int sver=1, ch=1; string book="",img="",img2="";
            var lines = File.ReadAllLines("corpus.txt");//verse2db
            var conf = File.ReadAllLines("v2db.conf");
            foreach(var line in conf){
                //Console.Write(line);
                var vars=line.Split('=');
                switch(vars[0].ToLower()){
                    case "save2db":
                        WriteDB=(vars[1].ToLower().Equals("true"))?true:false;break;
                    case "book":
                        book=vars[1];break;
                    case "ch":
                        ch=Convert.ToInt32(vars[1]);break;
                    case "sver":
                        sver=Convert.ToInt32(vars[1]);break;
                    case "img":
                        img=vars[1];break;
                    case "img2":
                        img2=vars[1];break;
                }
            }
            StringBuilder sb= new StringBuilder();
            var data=new List<string>();
            foreach (var line in lines)
            {
                //Console.WriteLine($"{line.Split().Length} words");
	            foreach (var word in line.Split())
       		    {
                    if (word.Trim().Length==0)continue;
                    if(Char.IsDigit(word,0)){
                        data.Add(sb.ToString());
                        sb.Clear();
                    }else{
                        sb.Append(word+" ");
                    }
                }
            }data.Add(sb.ToString());
            int id = (from xx in db.Stats where xx.BookCode==book select xx.BookStatsId).SingleOrDefault();
            //Console.WriteLine($"id={id}");
            if (id==null){
                Console.WriteLine("Invalid ID!");
                return;
            }
            foreach (var verse in data)
            {
                Console.WriteLine($"{book} {ch}:{sver} >> {verse.Substring(0,verse.Length>25?25:verse.Length)} [{img}, {img2}]");
                var corpus = new Corpus { 
                    BookStatsId = id,
                    CText = verse,
                    Chapt = ch,
                    Verse = sver++,
                    ReadersId=1,
                    Image=img,
                    Image2=img2
                    };
                db.Corpus.Add(corpus);
                if(WriteDB)db.SaveChanges();                
            }
        }
        static void sphinx_adaptt(string[] lines){
            StringBuilder sb= new StringBuilder();
            // var test="(chp01_00010 \"after much thought i decided that i was a person fitted to furnish to mankind this spectacle . \")";
            // Console.WriteLine(test.Split()[0].Substring(1)+"\n"+test.Split()[1].Substring(1));
            // Console.WriteLine(test.Split('"')[0]+"\n"+test.Split('"')[1]);
            //var i=0;
            foreach (var line in lines)
            {
                sb.AppendLine("<s>"+line.Split('"')[1]+"</s>"+line.Split('"')[0].Trim()+")");
                //sb.AppendLine(line.Split('"')[0].Trim().Substring(1));
                //Console.WriteLine(line);
            }
            Console.Write(sb.ToString());
        }
        static void LIUMConvert(string[] lines){
            //StringBuilder sb= new StringBuilder();
            double f2s=1.0/53.6519071;
            var spt=new List<double>();
            var dur=new List<double>();
            var fn=new List<string>();
            int i=0;
            foreach (var line in lines)
            {
                //sb.AppendLine("<s>"+line.Split('"')[1]+"</s>"+line.Split('"')[0].Trim()+")");
                if(line.StartsWith("palisa")){
                    i++;
                    var arr=line.Split();
                    spt.Add(Convert.ToDouble(arr[2])*f2s);
                    dur.Add(Convert.ToDouble(arr[3])*f2s);
                    fn.Add("u_"+i.ToString("D4")+"_"+arr[4]+"_"+arr[7].Substring(1).PadLeft(4,'0'));
                }
                for(var j=0;j<spt.Count ;j++){
                    Console.WriteLine("sox palisa.wav dtrim/"+ fn[j]+".wav trim "+spt[j]+" "+dur[j]);
                    Process myProcess = new Process();
                    //break;
                    try
                    {
                        myProcess.StartInfo.UseShellExecute = false;
                        // You can start any process, HelloWorld is a do-nothing example.
                        myProcess.StartInfo.FileName = "sox";
                        myProcess.StartInfo.CreateNoWindow = true;
                        myProcess.StartInfo.Arguments="palisa.wav dtrim/"+ fn[j]+".wav trim "+spt[j]+" "+dur[j]+" >> dtrim.log";
                        myProcess.StartInfo.RedirectStandardOutput = true;
                        myProcess.Start();
                        // This code assumes the process you are starting will terminate itself. 
                        // Given that is is started without a window so you cannot terminate it 
                        // on the desktop, it must terminate itself or you can do it programmatically
                        // from this application using the Kill method.
                        string output = myProcess.StandardOutput.ReadToEnd();
                        myProcess.WaitForExit();
                        Console.WriteLine(output);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        //myProcess.StartInfo.FileName="rm";
                        //myProcess.St
                    }
                }
            }
        }
    }

}
