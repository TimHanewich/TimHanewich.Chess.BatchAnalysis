using System;
using System.Collections.Generic;

namespace TimHanewich.Chess.BatchAnalysis.MoveNodeTree
{
    public class MoveNode
            {
                public string Move { get; set; }
                public long OccuranceCount { get; set; }
                public List<MoveNode> ChildNodes { get; set; }
                public float StatisticalProbability { get; set; }

                public static MoveNode Create()
                {
                    MoveNode ReturnInstance = new MoveNode();
                    ReturnInstance.ChildNodes = new List<MoveNode>();
                    return ReturnInstance;
                }

                public void CalculateChildNodesStatisticalProbability()
                {

                    List<MoveNode> ToExecute = ChildNodes;

                    //Error check
                    if (ToExecute.Count == 0)
                    {
                        throw new Exception("Cannot calculate statistical probability as there were no child nodes.");
                    }

                    //Get total count
                    long TotalCount = 0;
                    foreach (MoveNode mn in ToExecute)
                    {
                        TotalCount = TotalCount + mn.OccuranceCount;
                    }

                    //Get statistical probability
                    foreach (MoveNode mn in ToExecute)
                    {
                        mn.StatisticalProbability = (float)mn.OccuranceCount / (float)TotalCount;
                    }

                }

                public void DeepCalculateChildNodesStatisticalProbability()
                {
                    List<MoveNode> ToCalculate = new List<MoveNode>();
                    ToCalculate.Add(this);

                    do
                    {
                        List<MoveNode> ToAdd = new List<MoveNode>();

                        //Calculate all of them
                        foreach (MoveNode mn in ToCalculate)
                        {
                            if (mn.ChildNodes.Count > 0)
                            {
                                mn.CalculateChildNodesStatisticalProbability();

                                foreach (MoveNode cmn in mn.ChildNodes)
                                {
                                    ToAdd.Add(cmn);
                                }

                            }
                        }

                        //Clear the list
                        ToCalculate.Clear();

                        //Add it to the list
                        foreach (MoveNode tamn in ToAdd)
                        {
                            if (tamn.ChildNodes.Count > 0)
                            {
                                ToCalculate.Add(tamn);
                            }
                        }


                    } while (ToCalculate.Count > 0);

                }

                public string PrintNextMoveProbabilities()
                {
                    CalculateChildNodesStatisticalProbability();

                    //Prepare to arrange
                    List<MoveNode> ToPullFrom = new List<MoveNode>();
                    List<MoveNode> Arranged = new List<MoveNode>();
                    foreach (MoveNode mn in ChildNodes)
                    {
                        ToPullFrom.Add(mn);
                    }

                    //Arrange them all
                    do
                    {
                        MoveNode winner = ToPullFrom[0];
                        foreach (MoveNode mn in ToPullFrom)
                        {
                            if (mn.StatisticalProbability > winner.StatisticalProbability)
                            {
                                winner = mn;
                            }
                        }
                        Arranged.Add(winner);
                        ToPullFrom.Remove(winner);
                    } while (ToPullFrom.Count > 0);


                    //Print
                    string ALL = "";
                    foreach (MoveNode mn in Arranged)
                    {
                        ALL = ALL + mn.Move + " - " + mn.StatisticalProbability.ToString("#0.0%") + Environment.NewLine;
                    }
                    ALL = ALL.Substring(0, ALL.Length - 1);

                    return ALL;
                }

            }
}