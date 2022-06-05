using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace CorruptionModel{
    //����� ���� ��������� ��������
    public static class Actions1{
        public static string bribery = "���������� ������";
        public static string notBriding = "�� ���������� ������";
        public static string reciprocating = "������� ������";
        public static string notReciprocating = "��������� ������";
        public static string inspect = "�������� ��������";
        public static string notToInspect = "�� ��������� ��������";
    }
    public class Settings1{
        public double v, b, pl, ph, q, r, delx, x, dely, y, delz, z;
    }
    //������� ���� ��� ������������� ������
    public class GameRooles1 : IGameRooles{
        Settings1 s;
        public GameRooles1(Settings1 settings){
            this.s = settings;
        }
        public int NumOfPlayers { get; } = 3;
        //������� ��� �������� �������� ������
        public GameTreeNode GenerateRoot(){
            GameTreeNode terminal = new GameTreeNode(NumOfPlayers, NodeType.TerminalNode, �1, null);
            terminal.Name = "������������ ����";
            GameTreeNode inspectorCh = new GameTreeNode(NumOfPlayers, NodeType.PlayNode, 2,
                new[] { Actions1.inspect, Actions1.notToInspect }
            );
            inspectorCh.PossibleChilds = new List<GameTreeNode>(new[] { terminal });
            inspectorCh.Name = "2:����� ����������";
            GameTreeNode officialCh = new GameTreeNode(NumOfPlayers, NodeType.PlayNode, 1,
                new[] { Actions1.reciprocating, Actions1.notReciprocating }
            );
            officialCh.PossibleChilds = new List<GameTreeNode>(new[] { inspectorCh });
            officialCh.Name = "1:����� ���������";
            GameTreeNode clientCh = new GameTreeNode(NumOfPlayers, NodeType.PlayNode, 0,
                new[] { Actions1.bribery, Actions1.notBriding }
            );
            clientCh.PossibleChilds = new List<GameTreeNode>(new[] { officialCh, inspectorCh });
            clientCh.Name = "0:����� �������";
            return clientCh;
        }
        //����� ��� ���������� ������
        public void UpdateChilds(GameTreeNode node){
            node.Childs.Clear();
            if (node.History.Count == 0){
                node.Childs.Add(node.PossibleChilds[0]);
                node.Childs.Add(node.PossibleChilds[1]);
            }
            else{
                node.Childs.Add(node.PossibleChilds[0]);
                if (node.Actions.Count > 1)
                    node.Childs.Add(node.PossibleChilds[0]);
            }
        }
        private bool HistoryEquals(List<string> a, string[] b){
            if (a.Count != b.Length)
                return false;
            for (int i = 0; i < a.Count; i++)
                if (a[i] != b[i]) return false;
            return true;
        }
        private string[] brc = new[] { Actions1.bribery, Actions1.reciprocating, Actions1.inspect };
        private string[] brnc = new[] { Actions1.bribery, Actions1.reciprocating, Actions1.notToInspect };
        private string[] bnrc = new[] { Actions1.bribery, Actions1.notReciprocating, Actions1.inspect };
        private string[] bnrnc = new[] { Actions1.bribery, Actions1.notReciprocating, Actions1.notToInspect };
        private string[] nbc = new[] { Actions1.notBriding, Actions1.inspect };
        private string[] nbnc = new[] { Actions1.notBriding, Actions1.notToInspect };
        //������� ��� ���������� ������������ ������
        public void SetTerminalEquity(GameTreeNode terminalNode){
            var cfVal = terminalNode.CfValues;
            Array.Clear(cfVal, 0, terminalNode.CfValues.Length);
            var history = terminalNode.History;
            if (HistoryEquals(history, brc))
            { cfVal[0] = s.v � s.b � s.pl � s.ph; cfVal[1] = s.b � s.q; cfVal[2] = s.x + s.delx; }
            else if (HistoryEquals(history, brnc))
            { cfVal[0] = s.v � s.b; cfVal[1] = s.b; cfVal[2] = s.x; }
            else if (HistoryEquals(history, bnrc))
            { cfVal[0] = �s.pl; cfVal[1] = s.r; cfVal[2] = s.y + s.dely; }
            else if (HistoryEquals(history, bnrnc))
            { cfVal[0] = 0; cfVal[1] = s.r; cfVal[2] = s.y; }
            else if (HistoryEquals(history, nbc))
            { cfVal[0] = 0; cfVal[1] = 0; cfVal[2] = s.z; }
            else if (HistoryEquals(history, nbnc))
            { cfVal[0] = 0; cfVal[1] = 0; cfVal[2] = s.z + s.delz; }
        }
    }
    public class Program{
        static void Demo1(){
            Console.WriteLine("������ ������ ����������");
            Settings1 settings1 = new Settings1() { v = 6, b = 4, pl = 3, ph = 3, q = 5, r = 1, delx = 6, x = �3, dely = 4, y = �2, z = �1, delz = 2 };
            GameRooles1 r1 = new GameRooles1(settings1);
            Cfr cfr1 = new Cfr(r1, external: false); cfr1.Init(); 
            cfr1.Iterate(1);
            Console.WriteLine("�������: " + Tools.StrategyToStr(cfr1.ReturnUtil()));
            Console.WriteLine("����� ���������: " + Tools.GetImmRegSum(cfr1).ToString());
            Cfr cfr2 = new Cfr(r1, external: false); cfr2.Init();
            cfr2.Iterate(100000);
            Console.WriteLine("�������: " + Tools.StrategyToStr(cfr2.ReturnUtil()));
            Console.WriteLine("C���� ���������: " + Tools.GetImmRegSum(cfr2).ToString());
            Console.WriteLine("������ ������ ����������");
            Settings1 settings2 = new Settings1() { v = 12, b = 3, pl = 8, ph = 8, q = 4, r = 2, delx = 9, x = 1, dely = 1, y = 7, z = 5, delz = 3 };
            GameRooles1 r2 = new GameRooles1(settings2);
            Cfr cfr3 = new Cfr(r2, external: false);
            cfr3.Init();
            cfr3.Iterate(10000);
            Console.WriteLine("�������: " + Tools.StrategyToStr(cfr3.ReturnUtil()));
            Console.WriteLine("C���� ���������: " + Tools.GetImmRegSum(cfr3).ToString());
        }
        static void Main(string[] args){
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en�US");
            Demo1();
        }
    }
}
