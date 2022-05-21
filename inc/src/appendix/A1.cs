using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DominoSolver
{
    public class GameTreeNode
    {
        public string Name { get; set; }
        public GameTreeNodeType NodeType { get; set; }
        public List<GameTreeNode> Childs { get; set; }
        public List<GameTreeNode> PossibleChilds { get; set; }
        public List<string> Actions { get; set; }
        public double[] TStrategy { get; set; }
        public double[] RegretSum { get; set; }
        public double[] tRegretSum { get; set; }
        public double[] StrategySum { get; set; }
        public double[] RegretSum_e { get; set; }
        public double[] StrategySum_e { get; set; }
        public double[,] Util { get; set; }
        public double[] CfValues { get; set; }
        public double[] P { get; set; }
        public int Player { get; set; }
        public List<string> History { get; set; }

        public int NumOfPlayers { get; set; }

        public GameTreeNode() { }

        public GameTreeNode(int numOfPlayers, GameTreeNodeType nodeType, int player, IEnumerable<string> actions)
        {
            NumOfPlayers = numOfPlayers;
            NodeType = nodeType;
            Player = player;
            if (actions != null)
                Actions = new List<string>(actions);
            InitValues();
        }

        public void InitValues()
        {
            if (NodeType == GameTreeNodeType.TerminalNode)
            {
                CfValues = new double[NumOfPlayers];
                P = new double[NumOfPlayers];
                History = new List<string>();
                return;
            }
            TStrategy = new double[Actions.Count];
            StrategySum = new double[Actions.Count];
            RegretSum = new double[Actions.Count];
            tRegretSum = new double[Actions.Count];

            StrategySum_e = new double[Actions.Count];
            RegretSum_e = new double[Actions.Count];

            Util = new double[Actions.Count, NumOfPlayers];

            CfValues = new double[NumOfPlayers];
            P = new double[NumOfPlayers];

            Childs = new List<GameTreeNode>();

            History = new List<string>();
        }

        public double[] GetAvgStrategy()
        {
            if (NodeType == GameTreeNodeType.ChanceNode)
                return TStrategy;
            double[] avgStrategy = new double[Childs.Count];
            double normSum = 0;
            for (int a = 0; a < Actions.Count; a++)
            {
                if (StrategySum[a] > 0)
                {
                    avgStrategy[a] = StrategySum[a];
                    normSum += StrategySum[a];
                }
                else
                    avgStrategy[a] = 0;
            }
            for (int a = 0; a < Actions.Count; a++)
            {
                if (normSum > 0)
                    avgStrategy[a] /= normSum;
                else
                    avgStrategy[a] = 1.0 / Actions.Count;
            }
            return avgStrategy;
        }

        public void CalcTStrategy(double weight)
        {
            double normSum = 0;
            for (int rI = 0; rI < Actions.Count; rI++)
            {
                if (RegretSum[rI] > 0)
                {
                    TStrategy[rI] = tRegretSum[rI];
                    normSum += tRegretSum[rI];
                }
                else
                    TStrategy[rI] = 0;
            }
            for (int a = 0; a < Actions.Count; a++)
            {
                if (normSum > 0)
                    TStrategy[a] /= normSum;
                else
                    TStrategy[a] = 1.0 / Actions.Count;
                StrategySum[a] += TStrategy[a] * weight;
            }
        }

        public void SetAvgStrategy()
        {
            TStrategy = GetAvgStrategy();
        }
    }

    public interface IGameRooles
    {
        int NumOfPlayers { get; }
        GameTreeNode GenerateRoot();
        void UpdateChilds(GameTreeNode node);
        void SetTerminalEquity(GameTreeNode terminalNode);
    }

    public enum GameTreeNodeType
    {
        PlayNode,
        ChanceNode,
        TerminalNode
    }

    public static class TSRandom
    {
        [ThreadStatic] private static Random _local = new Random();

        public static double NextDouble()
        {
            return _local.NextDouble();
        }
        public static int Next()
        {
            return _local.Next();
        }
    }

    public static class Strategy
    {
        public static int GetRandomChoice(double[] strategy)
        {
            double ch = TSRandom.NextDouble();
            int a = 0;
            while (a < strategy.Length - 1 && strategy[a] <= ch) a++;
            return a;
        }
    }

    //Класс, реализующий итерации cfr и mccfr алгоритмов
    public class Cfr
    {
        double[] resultUtil;
        double[] resultUtil_e;

        public int iterSum { get; private set; }
        public int iterSum_e { get; private set; }

        public GameTreeNode root;

        public int NumOfPlayers;
        public bool External;
        public IGameRooles gameRooles;

        public HashSet<int> activePlayers = null;

        public Cfr(IGameRooles gameRooles, bool external = false)
        {
            NumOfPlayers = gameRooles.NumOfPlayers;
            this.gameRooles = gameRooles;
            External = external;
        }

        public void Init()
        {
            if (root == null)
                root = gameRooles.GenerateRoot() as GameTreeNode;

            for (int player = 0; player < NumOfPlayers; player++)
                root.P[player] = 1.0;
            resultUtil = new double[NumOfPlayers];
            iterSum = 0;
        }

        int player;

        public void Iterate(int iterations)
        {
            int count = iterations;
            while (count-- > 0)
            {
                root.History.Clear();
                SaveTRegretsRec(root);
                player = count % NumOfPlayers;
                RegRec(root);
                for (int player = 0; player < NumOfPlayers; player++)
                {
                    if (!ExploitMode)
                        resultUtil[player] += root.CfValues[player];
                    else
                        resultUtil_e[player] += root.CfValues[player];
                }
            }
            if (!ExploitMode)
                iterSum += iterations;
            else
                iterSum_e += iterations;
        }

        public double[] ReturnUtil()
        {
            double[] resUtil = !ExploitMode ? resultUtil.Clone() as double[] : resultUtil_e.Clone() as double[];

            for (int player = 0; player < NumOfPlayers; player++)
            {
                resUtil[player] /= !ExploitMode ? iterSum : iterSum_e;
            }

            return resUtil;
        }

        public void RegRec(GameTreeNode node)
        {
            if (node.NodeType == GameTreeNodeType.TerminalNode)
            {
                gameRooles.SetTerminalEquity(node);
                return;
            }
            if (node.Childs.Count == 0)
            {
                gameRooles.UpdateChilds(node);
            }
            if (node.NodeType != GameTreeNodeType.ChanceNode)
            {
                if ((!ExploitMode || activePlayers.Contains(node.Player)))
                {
                    //динамическая стратегия
                    node.CalcTStrategy(node.P[node.Player]);
                }
                else
                {
                    node.SetAvgStrategy();
                }
            }
            gameRooles.UpdateChilds(node);

            if (!External || node.Player == player)
                CalcUtil(node);
            else
                CalcUtilExternal(node);
        }

        //в теле данного метода происходит ветвление. Было бы неплохо выполнять данные операции асинхронно
        public void CalcUtil(GameTreeNode node)
        {
            for (int player = 0; player < NumOfPlayers; player++)
                node.CfValues[player] = 0;

            for (int a = 0; a < node.Childs.Count; a++)
            {
                GameTreeNode child = node.Childs[a];
                child.History.Clear();
                child.History.AddRange(node.History);
                child.History.Add(node.Actions[a]);

                for (int player = 0; player < NumOfPlayers; player++)
                {
                    if (player != node.Player)
                        child.P[player] = node.P[player];
                    else
                        child.P[player] = node.TStrategy[a] * node.P[player];
                }

                RegRec(child);
                for (int player = 0; player < NumOfPlayers; player++)
                {
                    node.Util[a, player] = child.CfValues[player];
                    node.CfValues[player] += node.TStrategy[a] * node.Util[a, player];
                }
            }
            //update regrets
            if (node.NodeType != GameTreeNodeType.ChanceNode && (!ExploitMode || activePlayers.Contains(node.Player)))
            {
                for (int a = 0; a < node.Childs.Count; a++)
                {
                    double regret = node.Util[a, node.Player] - node.CfValues[node.Player];
                    if (!External)
                        node.RegretSum[a] += GetPiMinusI(node.P, node.Player) * regret;
                    else
                        node.RegretSum[a] += regret;
                }
            }
        }

        public void SaveTRegretsRec(GameTreeNode node)
        {
            if (node.NodeType == GameTreeNodeType.TerminalNode)
                return;
            for (int a = 0; a < node.Childs.Count; a++)
                node.tRegretSum[a] = node.RegretSum[a];
            foreach (var c in node.PossibleChilds)
                SaveTRegretsRec(c);
        }

        public void CalcUtilExternal(GameTreeNode node)
        {
            int a = Strategy.GetRandomChoice(node.TStrategy);

            GameTreeNode child = node.Childs[a];
            child.History.Clear();
            child.History.AddRange(node.History);
            child.History.Add(node.Actions[a]);

            for (int player = 0; player < NumOfPlayers; player++)
                child.P[player] = node.P[player];

            RegRec(child);

            for (int player = 0; player < NumOfPlayers; player++)
                node.CfValues[player] = child.CfValues[player];
        }

        public double GetPiMinusI(double[] p, int i)
        {
            double ans = 1.0;
            for (int player = 0; player < NumOfPlayers; player++)
                if (player != i)
                    ans *= p[player];
            return ans;
        }

        public bool ExploitMode { get { return activePlayers != null; } }

        public void InitExploit(HashSet<int> activePlayers)
        {
            if (root == null)
                throw new Exception("not initialized");

            this.activePlayers = activePlayers;

            resultUtil_e = new double[NumOfPlayers];
            iterSum_e = 0;

            SetENode(root);
        }

        private void SetENode(GameTreeNode node)
        {
            if (activePlayers.Contains(node.Player))
            {
                node.RegretSum_e = node.RegretSum;
                node.RegretSum = new double[node.RegretSum_e.Length];
                node.StrategySum_e = node.StrategySum;
                node.StrategySum = new double[node.StrategySum_e.Length];
            }
            if (node.PossibleChilds != null)
                foreach (var c in node.PossibleChilds)
                    SetENode(c);
        }

        private void UnsetENode(GameTreeNode node)
        {
            if (activePlayers.Contains(node.Player))
            {
                node.RegretSum = node.RegretSum_e;
                node.StrategySum = node.StrategySum_e;
            }
            if (node.PossibleChilds != null)
                foreach (var c in node.PossibleChilds)
                    UnsetENode(c);
        }

        public void EndExploit()
        {
            UnsetENode(root);
            activePlayers = null;
        }
    }
}

