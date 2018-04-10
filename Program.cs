using System;
using System.Threading;
using System.Linq;

//using System.IO;
//using Newtonsoft.Json;

namespace wbible
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length==0)
            {            
                new Thread(new ThreadStart(WBibled.webthread)).Start();
                try{
                    WBibled.Verses();                
                }catch(Exception x){
                    Console.WriteLine(x.Message);
                }
            }else
            {
                WTxUtils.tp(args[0]);
            }
        }
    }
}
