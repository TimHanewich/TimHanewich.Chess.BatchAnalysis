using System;
using TimHanewich.Chess.BatchAnalysis;
using TimHanewich.Chess;
using System.IO;
using TimHanewich.Chess.BatchAnalysis.MoveNodeTree;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace testing
{
    class Program
    {
        static void Main(string[] args)
        {
            
            Stream s = System.IO.File.OpenRead(@"C:\Users\tahan\Downloads\wbli21.pgn");
            
            MoveNodeTreeConstructor mntc = new MoveNodeTreeConstructor();
            mntc.MassivePgnFileStream = s;
            mntc.StartConstruction();
            Task.Delay(10000).Wait();
            mntc.StopConstructionAsync().Wait();

            Console.WriteLine(JsonConvert.SerializeObject(mntc.ResultMoveNodeTree));


        }
    }
}
