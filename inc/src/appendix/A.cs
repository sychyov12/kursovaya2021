using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace CorruptionModel{
    //���� �������� ������
    public class GameTreeNode : IComparable<GameTreeNode>{
        public string Name { get; set; }//���
        public NodeType NodeType { get; set; }//���
        public List<GameTreeNode> Childs { get; set; }//�������� ����
        //��������� �������� ����
        public List<GameTreeNode> PossibleChilds { get; set; }
        public List<string> Actions { get; set; }//��������
        public double[] TStrategy { get; set; }//���������
        public double[] RegretSum { get; set; }//���������
        public double[] tRegretSum { get; set; }
        public double[] StrategySum { get; set; }
        public double[,] Util { get; set; }//��������
        //����������������� ��������
        public double[] CfValues { get; set; }
        public double[] P { get; set; }
        public int Player { get; set; }
        public List<string> History { get; set; }
        public int NumOfPlayers { get; set; }
        public GameTreeNode(int numOfPlayers, NodeType nodeType, int player, IEnumerable<string> actions){
            NumOfPlayers = numOfPlayers;
            NodeType = nodeType;
            Player = player;
            if (nodeType != NodeType.TerminalNode)
                Actions = new List<string>(actions);
            InitValues();
        }
        public void InitValues(){
            CfValues = new double[NumOfPlayers];
            P = new double[NumOfPlayers];
            History = new List<string>();
            if (NodeType == NodeType.TerminalNode) return;
            TStrategy = new double[Actions.Count];
            StrategySum = new double[Actions.Count];
            RegretSum = new double[Actions.Count];
            tRegretSum = new double[Actions.Count];
            Util = new double[Actions.Count, NumOfPlayers];
            Childs = new List<GameTreeNode>();
        }
        //������������ ���������
        private void NormalizePositiveSum(double[] source, double[] dest){
            double normSum = 0;
            for (int a = 0; a < source.Length; a++)
            {
                if (source[a] > 0)
                {
                    dest[a] = source[a];
                    normSum += source[a];
                }
                else
                    dest[a] = 0;
            }
            for (int a = 0; a < source.Length; a++)
            {
                if (normSum > 0)
                    dest[a] /= normSum;
                else
                    dest[a] = 1.0 / Actions.Count;
            }
        }
        public double[] GetAvgStrategy(){
            if (NodeType == NodeType.ChanceNode)
                return TStrategy;
            double[] avgStrategy = new double[Actions.Count];
            NormalizePositiveSum(StrategySum, avgStrategy);
            return avgStrategy;
        }
        public void CalcTStrategy(double weight){
            NormalizePositiveSum(tRegretSum, TStrategy);
            for (int a = 0; a < Actions.Count; a++)
                StrategySum[a] += TStrategy[a] * weight;
        }
        public int CompareTo(GameTreeNode other){
            return Name.CompareTo(other.Name);
        }
    }
    public interface IGameRooles{
        int NumOfPlayers { get; }
        GameTreeNode GenerateRoot();
        void UpdateChilds(GameTreeNode node);
        void SetTerminalEquity(GameTreeNode terminalNode);
    }
    public enum NodeType{
        PlayNode,//������� ����
        ChanceNode,//��������� �����
        TerminalNode//����������� ����
    }
    public static class TSRandom{
        [ThreadStatic] public static Random rand = new Random();
    }
    //����� ��������
    public static class Strategy{
        public static int GetRandomChoice(double[] strategy){
            double ch = TSRandom.rand.NextDouble();
            int a = 0;
            while (a < strategy.Length � 1 && strategy[a] <= ch) a++;
            return a;
        }
    }
    //���������� ���������
    public class Cfr{
        double[] resultUtil;//�������
        public int iterSum { get; private set; }
        public GameTreeNode root;
        public int NumOfPlayers;
        public bool External;//������� �������
        public IGameRooles gameRooles;//������� ����
        public Cfr(IGameRooles gameRooles, bool external = false){
            NumOfPlayers = gameRooles.NumOfPlayers;
            this.gameRooles = gameRooles;
            External = external;
        }
        public void Init(){
            if (root == null)
                root = gameRooles.GenerateRoot();
            for (int player = 0; player < NumOfPlayers; player++)
                root.P[player] = 1.0;
            resultUtil = new double[NumOfPlayers];
            iterSum = 0;
        }
        int player;
        public void Iterate(int iterations){
            int count = iterations;
            while (count�� > 0){
                root.History.Clear();
                SaveTRegretsRec(root);
                player = count % NumOfPlayers;
                RegRec(root);
                for (int player = 0; player < NumOfPlayers; player++)
                {
                    resultUtil[player] += root.CfValues[player];
                }
            }
            iterSum += iterations;
        }
        public double[] ReturnUtil(){
            double[] resUtil = resultUtil.Clone() as double[];

            for (int player = 0; player < NumOfPlayers; player++){
                resUtil[player] /= iterSum;
            }

            return resUtil;
        }
        //������� ���������
        public void RegRec(GameTreeNode node){
            if (node.NodeType == NodeType.TerminalNode){
                gameRooles.SetTerminalEquity(node);
                return;
            }
            if (node.NodeType != NodeType.ChanceNode){
                node.CalcTStrategy(node.P[node.Player]);
            }
            gameRooles.UpdateChilds(node);

            if (!External || node.Player == player)
                CalcUtil(node);
            else
                CalcUtilExternal(node);
        }
        //������� ���������
        public void CalcUtil(GameTreeNode node){
            for (int player = 0; player < NumOfPlayers; player++)
                node.CfValues[player] = 0;
            for (int a = 0; a < node.Childs.Count; a++){
                GameTreeNode child = node.Childs[a];
                child.History.Clear();
                child.History.AddRange(node.History);
                child.History.Add(node.Actions[a]);
                for (int player = 0; player < NumOfPlayers; player++){
                    if (player != node.Player)
                        child.P[player] = node.P[player];
                    else
                        child.P[player] = node.TStrategy[a] * node.P[player];
                }
                RegRec(child);
                for (int player = 0; player < NumOfPlayers; player++){
                    node.Util[a, player] = child.CfValues[player];
                    node.CfValues[player] += node.TStrategy[a] * node.Util[a, player];
                }
            }
            //���������� ���������
            if (node.NodeType != NodeType.ChanceNode){
                for (int a = 0; a < node.Childs.Count; a++){
                    double regret = node.Util[a, node.Player] � node.CfValues[node.Player];
                    if (!External)
                        node.RegretSum[a] += GetPiMinusI(node.P, node.Player) * regret;
                    else
                        node.RegretSum[a] += regret;
                }
            }
        }
        //������� �������
        public void CalcUtilExternal(GameTreeNode node){
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
        public double GetPiMinusI(double[] p, int i){
            double ans = 1.0;
            for (int player = 0; player < NumOfPlayers; player++)
                if (player != i)
                    ans *= p[player];
            return ans;
        }
        //���������� ���������
        public void SaveTRegretsRec(GameTreeNode node){
            if (node.NodeType == NodeType.TerminalNode)
                return;
            for (int a = 0; a < node.Childs.Count; a++)
                node.tRegretSum[a] = node.RegretSum[a];
            foreach (var c in node.PossibleChilds)
                SaveTRegretsRec(c);
        }
    }
    public static class Tools{
        public static ICollection<GameTreeNode> GetInfosets(GameTreeNode node){
            var ans = new SortedSet<GameTreeNode>();
            var q = new Queue<GameTreeNode>();
            q.Enqueue(node);
            while (q.Count > 0){
                var tnode = q.Dequeue();
                if (!ans.Contains(tnode)){
                    ans.Add(tnode);
                    if (tnode.PossibleChilds != null)
                        for (int a = 0; a < tnode.PossibleChilds.Count; a++)
                            if (tnode.PossibleChilds[a].NodeType != NodeType.TerminalNode)
                                q.Enqueue(tnode.PossibleChilds[a]);
                }
            }
            return ans;
        }
        //���������� ����� ���������
        public static double GetImmRegSum(Cfr cfr){
            var infosets = GetInfosets(cfr.root);
            return infosets.Select(x => x.RegretSum.Select(r => Math.Max(0, r)).Max()).Sum() / cfr.iterSum;
        }
    }
}
