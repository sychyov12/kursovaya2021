using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace CorruptionModel{
    //узел игрового дерева
    public class GameTreeNode : IComparable<GameTreeNode>{
        public string Name { get; set; }//им€
        public NodeType NodeType { get; set; }//тип
        public List<GameTreeNode> Childs { get; set; }//дочерние узлы
        //возможные дочерние узлы
        public List<GameTreeNode> PossibleChilds { get; set; }
        public List<string> Actions { get; set; }//действи€
        public double[] TStrategy { get; set; }//стратегии
        public double[] RegretSum { get; set; }//сожалени€
        public double[] tRegretSum { get; set; }
        public double[] StrategySum { get; set; }
        public double[,] Util { get; set; }//значение
        //контрафактические значени€
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
        //нормализаци€ стратегии
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
        PlayNode,//игровой узел
        ChanceNode,//случайный выбор
        TerminalNode//терминальый узел
    }
    public static class TSRandom{
        [ThreadStatic] public static Random rand = new Random();
    }
    //выбор действи€
    public static class Strategy{
        public static int GetRandomChoice(double[] strategy){
            double ch = TSRandom.rand.NextDouble();
            int a = 0;
            while (a < strategy.Length Ц 1 && strategy[a] <= ch) a++;
            return a;
        }
    }
    //реализаци€ алгоритма
    public class Cfr{
        double[] resultUtil;//выплаты
        public int iterSum { get; private set; }
        public GameTreeNode root;
        public int NumOfPlayers;
        public bool External;//внешние выплаты
        public IGameRooles gameRooles;//правила игры
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
            while (countЦЦ > 0){
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
        //подсчет сожалений
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
        //подсчет сожалений
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
            //обновление сожалений
            if (node.NodeType != NodeType.ChanceNode){
                for (int a = 0; a < node.Childs.Count; a++){
                    double regret = node.Util[a, node.Player] Ц node.CfValues[node.Player];
                    if (!External)
                        node.RegretSum[a] += GetPiMinusI(node.P, node.Player) * regret;
                    else
                        node.RegretSum[a] += regret;
                }
            }
        }
        //внешн€€ выборка
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
        //сохранение сожалений
        public void SaveTRegretsRec(GameTreeNode node){
            if (node.NodeType == NodeType.TerminalNode)
                return;
            for (int a = 0; a < node.Childs.Count; a++)
                node.tRegretSum[a] = node.RegretSum[a];
            foreach (var c in node.PossibleChilds)
                SaveTRegretsRec(c);
        }
    }
    public class StringTree{
        public string[] strs;
        public int type = 0;
        public StringTree[] childs;
    }
    public static class Tools{
        private static string FillStr(string str, int len){
            while (str.Length < len) str += " ";
            return str;
        }
        //отрисовка игрового дерева
        public static void printTree(TextWriter tw, StringTree node, string indent = ""){
            string leftIndent = indent != "" ? (indent.Substring(0, indent.Length Ц 3) + " | ") : "";
            string midleIndent = indent != "" ? indent.Substring(0, indent.Length Ц 3) + (indent.EndsWith(" | ") ? " +Ч" : " +Ч") : "";
            int maxLen = node.strs.Select(x => x.Length).Max();
            char line = node.type == 0 ? 'Ч' : '=';
            tw.WriteLine("{0}+{1}+", leftIndent, new String(line, maxLen));
            for (int i = 0; i < node.strs.Length; i++){
                if (i == node.strs.Length / 2 && indent != ""){
                    tw.WriteLine("{0}+{1}|", midleIndent, FillStr(node.strs[i], maxLen));
                    leftIndent = indent;
                }
                else
                    tw.WriteLine("{0}|{1}|", leftIndent, FillStr(node.strs[i], maxLen));
            }
            tw.WriteLine("{0}+{1}{2}+", indent, node.childs.Length > 0 ? "+" : line.ToString(), new String(line, maxLen Ц 1));
            if (node.childs != null)
                for (int a = 0; a < node.childs.Length; a++){
                    var child = node.childs[a];
                    if (a != node.childs.Length Ц 1)
                        printTree(tw, child, indent + " | ");
                    else
                        printTree(tw, child, indent + "   ");
                }
        }
        //генераци€ представлени€ дерева
        public static StringTree getTree(GameTreeNode node, IGameRooles gameRooles, double p = 1.0, double minP = 0, int depth = 100){
            if (node.NodeType != NodeType.TerminalNode)
                gameRooles.UpdateChilds(node);
            else
                gameRooles.SetTerminalEquity(node);
            StringTree ans = new StringTree();
            var strs = new List<string>();
            if (node.NodeType == NodeType.PlayNode){
                ans.type = 1;
                strs.Add(node.Name);
                strs.Add(string.Format("»стори€: {0}. ¬еро€тность: {1:0.00000000}", HistoryToStr(node.History), p));
                strs.Add(string.Format("ƒействи€ игрока {0}: {1}", node.Player, StrategyToStr(node.GetAvgStrategy(), node.Actions)));
            }
            else if (node.NodeType == NodeType.TerminalNode){
                ans.type = 0;
                strs.Add(node.Name);
                strs.Add(string.Format("»стори€: {0}", HistoryToStr(node.History)));
                strs.Add(string.Format("“ерминальные выплаты: {0}. ¬еро€тность: {1:0.00000000}", StrategyToStr(node.CfValues), p));
            }
            else{
                ans.type = 1;
                strs.Add(node.Name);
                strs.Add(string.Format("»стори€: {0}. ¬еро€тность: {1:0.00000000}", HistoryToStr(node.History), p));
                strs.Add(string.Format("—лучайное распределение: {0}", StrategyToStr(node.GetAvgStrategy(), node.Actions)));
            }
            ans.strs = strs.ToArray();
            var childs = new List<StringTree>();
            if (node.PossibleChilds != null){
                var avgStrategy = node.GetAvgStrategy();
                for (int a = 0; a < node.Actions.Count; a++){
                    var child = node.Childs[a];
                    child.History = new List<string>(node.History);
                    child.History.Add(node.Actions[a]);
                    if (p * avgStrategy[a] >= minP && depth > 0)
                        childs.Add(getTree(child, gameRooles, p * avgStrategy[a], minP, depth Ц 1));
                }
            }
            ans.childs = childs.ToArray();
            return ans;
        }
        //генераци€ информационных состо€ний
        public static StringTree[] getTree2(GameTreeNode node, int depth = 0){
            return GetInfosets(node).Select(tnode =>{
                StringTree ans1 = new StringTree();
                ans1.type = 1;
                var strs = new List<string>();
                if (node.NodeType == NodeType.PlayNode){
                    strs.Add(tnode.Name);
                    strs.Add(string.Format("ƒействи€ игрока {0}: {1}", tnode.Player, StrategyToStr(tnode.GetAvgStrategy(), tnode.Actions)));
                }
                else{
                    strs.Add(tnode.Name);
                    strs.Add(string.Format("—лучайное распределение: {0}", StrategyToStr(tnode.GetAvgStrategy(), tnode.Actions)));
                }
                ans1.childs = new StringTree[0];
                ans1.strs = strs.ToArray();
                return ans1;
            }).ToArray();
        }
        public static string HistoryToStr(IEnumerable<string> history){
            return string.Join(", ", history);
        }
        public static string StrategyToStr(double[] strategy, IEnumerable<string> actions = null){
            StringBuilder sb = new StringBuilder();
            for (int a = 0; a < strategy.Length; a++){
                if (actions != null)
                    sb.Append(String.Format("{0} {1:0.0000}, ", actions.ToArray()[a], strategy[a]));
                else
                    sb.Append(String.Format("{0:0.0000}, ", strategy[a]));
            }
            if (sb.Length > 1)
                sb.Remove(sb.Length Ц 2, 2);
            return sb.ToString();
        }
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
        //вычисление суммы сожалений
        public static double GetImmRegSum(Cfr cfr){
            var infosets = GetInfosets(cfr.root);
            return infosets.Select(x => x.RegretSum.Select(r => Math.Max(0, r)).Max()).Sum() / cfr.iterSum;
        }
    }
}
