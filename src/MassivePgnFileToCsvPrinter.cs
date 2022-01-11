using System;
using System.IO;
using System.Collections.Generic;
using TimHanewich.Chess.Pgn;

namespace TimHanewich.Chess.BatchAnalysis
{
    public class MassivePgnFileToCsvPrinter
        {

            public void PrintToCsv(Stream MassivePgnFileStream, Stream TargetCsvFileStream, int PrintCount, int PrintNumberOfMoves)
            {
                MassivePgnFileSplitter mpfs = new MassivePgnFileSplitter(MassivePgnFileStream);
                StreamWriter sw = new StreamWriter(TargetCsvFileStream);


                //Write the header
                List<string> Header = new List<string>();
                Header.Add("Event");
                Header.Add("Site");
                Header.Add("Date");
                Header.Add("Round");
                Header.Add("White");
                Header.Add("Black");
                Header.Add("Result");
                Header.Add("UtcDateTime");
                Header.Add("WhiteElo");
                Header.Add("BlackElo");
                Header.Add("WhiteRatingDifference");
                Header.Add("BlackRatingDifference");
                Header.Add("ECO");
                Header.Add("OpeningName");
                Header.Add("TimeControl");
                Header.Add("TimeIncrement");
                Header.Add("Termination");


                int t = 0;
                for (t = 1; t <= PrintNumberOfMoves; t++)
                {
                    Header.Add("Move" + t.ToString());
                }
                sw.WriteLine(StringListToCsvLine(Header));




                //Get the logs
                int GameIndex = 0;
                for (GameIndex = 0; GameIndex <= PrintCount; GameIndex++)
                {
                    string ThisGame = mpfs.GetNextGame();

                    try
                    {
                        List<string> ThisRow = new List<string>();
                        PgnParserLite ppl = PgnParserLite.ParsePgn(ThisGame);
                        ThisRow.Add(ppl.Event);
                        ThisRow.Add(ppl.Site);
                        ThisRow.Add(ppl.Date.ToShortDateString());
                        ThisRow.Add(ppl.Round);
                        ThisRow.Add(ppl.White);
                        ThisRow.Add(ppl.Black);
                        ThisRow.Add(ppl.Result);
                        ThisRow.Add(ppl.UtcDateTime.ToString());
                        ThisRow.Add(ppl.WhiteElo.ToString());
                        ThisRow.Add(ppl.BlackElo.ToString());
                        ThisRow.Add(ppl.WhiteRatingDifference.ToString());
                        ThisRow.Add(ppl.BlackRatingDifference.ToString());
                        ThisRow.Add(ppl.ECO);
                        ThisRow.Add(ppl.OpeningName);
                        ThisRow.Add(ppl.TimeControl.TotalSeconds.ToString());
                        ThisRow.Add(ppl.TimeIncrement.TotalSeconds.ToString());
                        ThisRow.Add(ppl.Termination);

                        //Write moves
                        t = 0;
                        for (t = 0; t <= PrintNumberOfMoves - 1; t++)
                        {
                            if (ppl.Moves.Length > t)
                            {
                                ThisRow.Add(ppl.Moves[t]);
                            }
                            else
                            {
                                ThisRow.Add("");
                            }
                        }

                        //Write this line
                        sw.WriteLine(StringListToCsvLine(ThisRow));
                    }
                    catch
                    {

                    }

                    float percentdone = (float)GameIndex / (float)PrintCount;
                    Console.WriteLine("Writing game " + GameIndex.ToString("#,##0") + " out of " + PrintCount.ToString("#,##0") + " (" + percentdone.ToString("#0.00%") + ")...");
                }



                Console.WriteLine("Game writing complete!");

            }

            private string StringListToCsvLine(List<string> CsvRowContent)
            {
                if (CsvRowContent.Count == 0)
                {
                    throw new Exception("The supplied CSV row content did not have any values.");
                }
                string a = "";
                foreach (string s in CsvRowContent)
                {
                    string ThisOne = s;
                    if (ThisOne.Contains(","))
                    {
                        ThisOne = "\"" + ThisOne + "\"";
                    }
                    a = a + ThisOne + ",";
                }
                a = a.Substring(0, a.Length - 1);
                return a;
            }
        }
}

