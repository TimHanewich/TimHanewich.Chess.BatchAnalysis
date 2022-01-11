using System;
using System.Collections.Generic;

namespace TimHanewich.Chess.BatchAnalysis.MoveNodeTree
{
    public class MoveNodeTree
            {
                public List<MoveNode> MoveTree { get; set; }

                public MoveNode GetBranchMoveNode(string[] moves)
                {
                    //Get the first move
                    MoveNode CurrentNode = null;
                    foreach (MoveNode mn in MoveTree)
                    {
                        if (mn.Move == moves[0])
                        {
                            CurrentNode = mn;
                        }
                    }

                    //Check for errors
                    if (CurrentNode == null)
                    {
                        throw new Exception("There were no scenarios in which first move '" + moves + "' was played.");
                    }

                    //Get to the end of this!
                    int t = 1;
                    for (t = 1; t <= moves.Length - 1; t++)
                    {
                        MoveNode next_node = null;
                        foreach (MoveNode mn in CurrentNode.ChildNodes)
                        {
                            if (mn.Move == moves[t])
                            {
                                next_node = mn;
                            }
                        }

                        if (next_node == null)
                        {
                            throw new Exception("There were no scenarios in which the sequence of moves was played.");
                        }

                        CurrentNode = next_node;
                    }


                    return CurrentNode;
                }

            }
}