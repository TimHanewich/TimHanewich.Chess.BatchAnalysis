using System;
using System.IO;
using System.Collections.Generic;
using TimHanewich.Chess.Pgn;
using System.Threading.Tasks;

namespace TimHanewich.Chess.BatchAnalysis.MoveNodeTree
{
    public class MoveNodeTreeConstructor
            {
                //Inputs
                public Stream MassivePgnFileStream { get; set; }

                //Progress
                public long GamesAdded;
                public TimeSpan ProcessingTime;

                //Outputs
                public MoveNodeTree ResultMoveNodeTree { get; set; }

                //Other information
                private bool HasBeenStarted = false;
                private bool ContinueConstructing = false;
                private DateTime StartedProcessingTime;
                private bool ResultIsReady = false;


                public void StartConstruction()
                {
                    //Error checking 
                    if (MassivePgnFileStream == null)
                    {
                        throw new Exception("A massive pgn file stream was not provided to the constructor.");
                    }
                    if (HasBeenStarted == true)
                    {
                        throw new Exception("Cannot start construction as construction has already been started for this instance.");
                    }

                    //Set up
                    StartedProcessingTime = DateTime.Now;
                    HasBeenStarted = true;
                    ContinueConstructing = true;
                    ResultIsReady = false;


                    MassivePgnFileSplitter mpfs = new MassivePgnFileSplitter(MassivePgnFileStream);

                    List<MoveNode> RootNodes = new List<MoveNode>();


                    do
                    {
                        try
                        {
                            string ng = mpfs.GetNextGame();

                            //PgnFileParser pfp = PgnFileParser.CreateFromPgnContent(ng);
                            PgnParserLite ppl = PgnParserLite.ParsePgn(ng);
                            //Get current node
                            MoveNode CurrentNode = null;
                            foreach (MoveNode mn in RootNodes)
                            {
                                if (mn.Move == ppl.Moves[0])
                                {
                                    CurrentNode = mn;
                                    CurrentNode.OccuranceCount = CurrentNode.OccuranceCount + 1;
                                }
                            }
                            if (CurrentNode == null)
                            {
                                CurrentNode = MoveNode.Create();
                                CurrentNode.Move = ppl.Moves[0];
                                CurrentNode.OccuranceCount = 1;
                                RootNodes.Add(CurrentNode);
                            }


                            int t = 1;
                            for (t = 1; t <= ppl.Moves.Length - 1; t++)
                            {

                                //See if we can find the next one
                                bool FoundIt = false;
                                foreach (MoveNode mn in CurrentNode.ChildNodes)
                                {
                                    if (mn.Move == ppl.Moves[t])
                                    {
                                        CurrentNode = mn;
                                        CurrentNode.OccuranceCount = CurrentNode.OccuranceCount + 1;
                                        FoundIt = true;
                                    }
                                }


                                //If we can't find it
                                if (FoundIt == false)
                                {
                                    MoveNode nmn = MoveNode.Create();
                                    nmn.Move = ppl.Moves[t];
                                    nmn.OccuranceCount = 1;
                                    CurrentNode.ChildNodes.Add(nmn);
                                    CurrentNode = nmn;
                                }

                            }

                            //Update the progress
                            GamesAdded = GamesAdded + 1;
                            ProcessingTime = DateTime.Now - StartedProcessingTime;
                        }
                        catch
                        {

                        }
                    } while (ContinueConstructing);


                    //The below happen once "Continue Constructing" is false
                    MoveNodeTree MNT = new MoveNodeTree();
                    MNT.MoveTree = RootNodes;
                    ResultMoveNodeTree = MNT;
                    ResultIsReady = true;


                }

                public async Task StopConstructionAsync()
                {
                    ContinueConstructing = false;
                    do
                    {
                        await Task.Delay(25);
                    } while (ResultIsReady == false);
                }

            }
}