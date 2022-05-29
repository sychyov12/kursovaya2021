using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace CorruptionModel{
    //����� ���� ��������� ��������
    public static class A2{
        public static string DontSteal = "��C����",
            Steal = "�����", DontCheckOfficial = "���������",
            CheckOfficial = "�������", OfferBride = "�������",
            Agree = "������", ExposeBoss = "������",
            TakeBride = "������", RejectBride = "������",
            CheckInspector = "�������", DontCheckInspector = "���������";
    }
    //���������
    public class Settings2{
        public int NumOfPlayers;
        public Employee Hierarchy;
        public int numOfOfficials { get { return NumOfPlayers � 2; } }
        public int inspectorNumber { get { return NumOfPlayers � 2; } }
        public int controllerNumber { get { return NumOfPlayers � 1; } }

        public double co, ci, fs, fb, fq, fns, fe, fbc, rs, rb, rq;
        public Settings2(Employee hierarchy){
            Hierarchy = hierarchy;
            NumOfPlayers = hierarchy.GetCount() + 2;
            hierarchy.UpdateBoss();
        }
    }
    //���������� � ���������� � �����������
    public class Employee{
        public string Name { get; set; }
        public double S, B;
        public Employee Boss = null;
        public Employee[] Subordinates = null;
        public int Number = �1;
        public Employee(string name, double s, double b, Employee[] subordinates = null){
            Name = name; S = s; B = b;
            if (subordinates != null)
                Subordinates = (Employee[])subordinates.Clone();
        }
        public int GetCount(){
            if (Subordinates == null)
                return 1;
            else
                return Subordinates.Sum(x => x.GetCount()) + 1;
        }
        public void UpdateBoss(){
            if (Subordinates != null){
                foreach (var o in Subordinates){
                    o.Boss = this;
                    o.UpdateBoss();
                }
            }
        }
    }
    //���������� ������ ���� ��� ������������� ������ ���������
    public class GameRooles2 : IGameRooles{
        public int NumOfPlayers => s.NumOfPlayers;
        public Settings2 s;
        public GameRooles2(Settings2 settings){
            s = settings;
        }
        bool[] steals = null;
        //������� ��� �������� �������� ������
        public GameTreeNode GenerateRoot(){
            if (steals == null) steals = new bool[s.NumOfPlayers � 2];
            Array.Clear(steals, 0, steals.Length);
            Employee hierarchy = s.Hierarchy;
            Queue<Employee> officials = new Queue<Employee>();
            officials.Enqueue(hierarchy);
            GameTreeNode offch = null;
            GameTreeNode root = null;
            int offNumber = 0;
            while (officials.Count > 0){
                Employee tmpOfficial = officials.Dequeue();
                tmpOfficial.Number = offNumber;
                var nextOffch = new GameTreeNode(s.NumOfPlayers, NodeType.PlayNode, tmpOfficial.Number, new[] { A2.Steal, A2.DontSteal });
                nextOffch.Name = string.Format("���� 2. ������� ���������� {0} � �����", offNumber);
                if (offch != null)
                    offch.PossibleChilds = new List<GameTreeNode>(new[] { nextOffch });
                else{
                    GameTreeNode iich = new GameTreeNode(s.NumOfPlayers, NodeType.PlayNode, s.controllerNumber,
                        new[] { A2.DontCheckInspector, A2.CheckInspector });
                    iich.Name = string.Format("���� 1. ������� ���������� � ��������");
                    iich.PossibleChilds = new List<GameTreeNode>(new[] { nextOffch });
                    root = iich;
                }
                offch = nextOffch;
                if (tmpOfficial.Subordinates != null)
                    foreach (var s in tmpOfficial.Subordinates)
                        officials.Enqueue(s);
                offNumber++;
            }
            //������������ ����
            GameTreeNode terminal = new GameTreeNode(s.NumOfPlayers, NodeType.TerminalNode, �1, null);
            terminal.Name = string.Format("���� 5: ������������ ����");
            var inspch = new GameTreeNode(s.NumOfPlayers, NodeType.PlayNode, s.inspectorNumber,
                new[] { A2.CheckOfficial, A2.DontCheckOfficial });
            inspch.Name = string.Format("���� 3: ������� ��������� � �������� ���������� {0}", 0);
            //�������� �������� ������� ����
            offch.PossibleChilds = new List<GameTreeNode>(new[] { inspch });
            //���������� ������ ���� ��������� �� �������
            GameTreeNode off0SI = new GameTreeNode(s.NumOfPlayers, NodeType.PlayNode, 0, new[] { A2.OfferBride, A2.Agree });
            off0SI.Name = string.Format("���� 4: ����� ���������� {0} ��������", 0);
            GameTreeNode inp0SIB = new GameTreeNode(s.NumOfPlayers, NodeType.PlayNode, s.inspectorNumber,
                new[] { A2.TakeBride, A2.RejectBride });
            inp0SIB.Name = string.Format("���� 4: ����� ���������� {0} ��������. ���������� ������", 0);
            off0SI.PossibleChilds = new List<GameTreeNode>(new[] { inp0SIB, terminal });
            //�������� ���� ���������� ������
            inp0SIB.PossibleChilds = new List<GameTreeNode>(new[] { terminal });
            var offISI = off0SI;
            if (hierarchy.Subordinates != null)
                foreach (var s in hierarchy.Subordinates)
                    officials.Enqueue(s);
            while (officials.Count > 0){
                Employee tmpOfficial = officials.Dequeue();
                var nextInspch = new GameTreeNode(s.NumOfPlayers, NodeType.PlayNode, s.inspectorNumber,
                    new[] { A2.CheckOfficial, A2.DontCheckOfficial });
                nextInspch.Name = string.Format("���� 3: ������� ��������� � �������� ���������� {0}", tmpOfficial.Number);
                //������������ ���������� ������
                GameTreeNode inpSI2EB = new GameTreeNode(s.NumOfPlayers, NodeType.PlayNode, s.inspectorNumber,
                    new[] { A2.TakeBride, A2.RejectBride });
                inpSI2EB.Name = string.Format("���� 4: ����� ���������� {0} �������� ����� ������������. ���������� ������",
                    tmpOfficial.Boss.Number);
                inpSI2EB.PossibleChilds = new List<GameTreeNode>(new[] { terminal });
                //��������� ������� �� ������������, � �� ���� �������� �����
                GameTreeNode offIBossSIE = new GameTreeNode(s.NumOfPlayers, NodeType.PlayNode, tmpOfficial.Boss.Number,
                    new[] { A2.OfferBride, A2.Agree });
                offIBossSIE.Name = string.Format("���� 4: ����� ���������� {0} �������� ����� ������������", tmpOfficial.Boss.Number);
                offIBossSIE.PossibleChilds = new List<GameTreeNode>(new[] { inpSI2EB, terminal });
                //������ ������������
                GameTreeNode inpISB = new GameTreeNode(s.NumOfPlayers, NodeType.PlayNode, s.inspectorNumber,
                    new[] { A2.TakeBride, A2.RejectBride });
                inpISB.Name = string.Format("���� 4: ����� ���������� {0} ��������. ���������� ������", tmpOfficial.Number);
                inpISB.PossibleChilds = new List<GameTreeNode>(new[] { terminal });
                //����������� ����� ����������� ��������
                GameTreeNode nextOffISI = new GameTreeNode(s.NumOfPlayers, NodeType.PlayNode, tmpOfficial.Number,
                    new[] { A2.ExposeBoss, A2.OfferBride, A2.Agree });
                nextOffISI.Name = string.Format("���� 4: ����� ���������� {0} ��������", tmpOfficial.Number);
                nextOffISI.PossibleChilds = new List<GameTreeNode>(new[] { inpISB, offIBossSIE, terminal });
                //�������� ����������� ������� � �������� �������� ������������ ���� ������� � ���������� ������������
                inspch.PossibleChilds = new List<GameTreeNode>(new[] { nextInspch, offISI, terminal });
                inspch = nextInspch;
                offISI = nextOffISI;
                if (tmpOfficial.Subordinates != null)
                    foreach (var s in tmpOfficial.Subordinates)
                        officials.Enqueue(s);
            }
            inspch.PossibleChilds = new List<GameTreeNode>(new[] { terminal, offISI, terminal });
            return root;
        }
        //����� ��� ���������� ������
        public void UpdateChilds(GameTreeNode node){
            node.Childs.Clear();
            int offCh = 0, checkedOfficial = 0;
            bool officialChecked = false;
            var history = node.History;
            int i = 0;
            if (history.Count == 0){
                node.Childs.Add(node.PossibleChilds[0]);
                node.Childs.Add(node.PossibleChilds[0]);
            }
            else{
                i++;
                while (offCh < Math.Min(history.Count � 1, s.NumOfPlayers � 2)){
                    if (history[i] == A2.Steal)
                        steals[offCh] = true;
                    else
                        steals[offCh] = false;
                    i++;
                    offCh++;
                }
                if (offCh < s.NumOfPlayers � 2){
                    node.Childs.Add(node.PossibleChilds[0]);
                    node.Childs.Add(node.PossibleChilds[0]);
                }
                else{
                    while (checkedOfficial < Math.Min(history.Count � 1 � (s.NumOfPlayers � 2), s.NumOfPlayers � 2)){
                        checkedOfficial++;
                        if (history[i] == A2.CheckOfficial){
                            officialChecked = true; i++; break;
                        }
                        i++;
                    }
                    if (!officialChecked && checkedOfficial < s.NumOfPlayers � 2){
                        //������ ����������� ����� ��������/��������� � ����������
                        if (steals[checkedOfficial])
                            node.Childs.Add(node.PossibleChilds[1]);
                        else
                            node.Childs.Add(node.PossibleChilds[2]);
                        node.Childs.Add(node.PossibleChilds[0]);
                    }
                    else if (officialChecked && steals[checkedOfficial � 1]){
                        //�������� ������� ����� (����� terminal)
                        Employee chOfficial = findByNumber(s.Hierarchy, checkedOfficial � 1);
                        while (i < history.Count && history[i] == A2.ExposeBoss){
                            chOfficial = chOfficial.Boss;
                            i++;
                        }
                        if (i == history.Count){
                            //���������� ������, ��������� �����, ����������� ������������
                            if (chOfficial.Number == 0){
                                node.Childs.Add(node.PossibleChilds[0]);
                                node.Childs.Add(node.PossibleChilds[1]);
                            }
                            else{
                                if (steals[chOfficial.Boss.Number])
                                    node.Childs.Add(node.PossibleChilds[1]);
                                else
                                    node.Childs.Add(node.PossibleChilds[2]);
                                node.Childs.Add(node.PossibleChilds[0]);
                                node.Childs.Add(node.PossibleChilds[2]);
                            }
                        }
                        else{
                            //������� ��� ��������� ������
                            node.Childs.Add(node.PossibleChilds[0]);
                            node.Childs.Add(node.PossibleChilds[0]);
                        }
                    }
                }
            }
        }
        private List<Employee> checkedOfficials = new List<Employee>();
        //������� ��� ���������� ������������ ������
        public void SetTerminalEquity(GameTreeNode terminalNode){
            terminalNode.CfValues = terminalNode.CfValues == null ? new double[s.NumOfPlayers] : terminalNode.CfValues;
            var cfVal = terminalNode.CfValues;
            Array.Clear(cfVal, 0, terminalNode.CfValues.Length);
            var history = terminalNode.History;
            int i = 0;
            //������� ���������� � ��������
            bool inpCh = history[i] == A2.CheckInspector;
            if (inpCh) cfVal[s.controllerNumber] = �s.ci;
            i++;
            for (int off = 0; off < s.numOfOfficials; off++){
                if (history[i] == A2.Steal){
                    steals[off] = true;
                    cfVal[off] += findByNumber(s.Hierarchy, off).S;
                }
                else
                    steals[off] = false;
                i++;
            }
            int checkedOff = 0;
            while (i < history.Count && history[i++] == A2.DontCheckOfficial)
                checkedOff++;
            //���������� ���������� ������ ��������� �� ��� ����������� �����
            cfVal[s.inspectorNumber] �= steals.Count(x => x) * s.fns;
            if (checkedOff != s.numOfOfficials){
                //�������� ����������
                cfVal[s.inspectorNumber] �= s.co;
                if (steals[checkedOff]){
                    //����������� ��������� �������� �����
                    double CActionckedSteal = findByNumber(s.Hierarchy, checkedOff).S;
                    if (history[i] == A2.Agree){
                        //��������� �������� �� �������
                        cfVal[checkedOff] += �CActionckedSteal � s.fs;
                        cfVal[s.inspectorNumber] += s.rs + s.fns;
                    }
                    else if (history[i] == A2.OfferBride){
                        //��������� ��������� ������
                        i++;
                        double offBride = findByNumber(s.Hierarchy, checkedOff).B;
                        cfVal[checkedOff] �= offBride;
                        if (history[i] == A2.RejectBride){
                            //�������� �������� ������
                            cfVal[checkedOff] += �CActionckedSteal � s.fs � s.fb;
                            cfVal[s.inspectorNumber] += s.rs + s.rb + s.fns;
                        }
                        else if (history[i] == A2.TakeBride){
                            //�������� ������ ������
                            if (!inpCh){
                                //��������� �� �������� ��������
                                cfVal[s.inspectorNumber] += offBride;
                                cfVal[s.controllerNumber] += �s.fbc;
                            }
                            else{
                                //��������� �������� ��������
                                cfVal[checkedOff] += �CActionckedSteal � s.fs � s.fb;
                                cfVal[s.inspectorNumber] += �s.fq;
                                cfVal[s.controllerNumber] += s.rq;
                            }
                        }
                    }
                    else if (history[i] == A2.ExposeBoss){
                        //��������� ����� �� ������������
                        checkedOfficials.Clear();
                        checkedOfficials.Add(findByNumber(s.Hierarchy, checkedOff));
                        checkedOfficials.Add(findByNumber(s.Hierarchy, checkedOff));
                        int bossNumber = findByNumber(s.Hierarchy, checkedOff).Boss.Number;
                        i++;
                        if (!steals[bossNumber]){
                            //������������ �� �������� �����
                            checkedOfficials.ForEach((x) => cfVal[x.Number] �= x.S);
                            cfVal[checkedOfficials.Last().Number] += �s.fs � s.fe;
                            cfVal[s.inspectorNumber] += checkedOfficials.Count * (s.rs + s.fns);
                        }
                        else{
                            //������������ �������� �����
                            double cActionckedBossSteal = findByNumber(s.Hierarchy, bossNumber).S;
                            if (history[i] == A2.Agree){
                                //������������ �������� �� �������
                                checkedOfficials.ForEach((x) => cfVal[x.Number] �= x.S);
                                cfVal[bossNumber] += �cActionckedBossSteal � s.fs;
                                cfVal[s.inspectorNumber] += (s.rs + s.fns) * (checkedOfficials.Count + 1);
                            }
                            else if (history[i] == A2.OfferBride){
                                //������������ ��������� ������
                                i++;
                                double offBossBride = findByNumber(s.Hierarchy, bossNumber).B;
                                cfVal[bossNumber] �= offBossBride;

                                if (history[i] == A2.RejectBride){
                                    //�������� �������� ������
                                    checkedOfficials.ForEach((x) => cfVal[x.Number] �= x.S);
                                    cfVal[bossNumber] += �cActionckedBossSteal � s.fs � s.fb;
                                    cfVal[s.inspectorNumber] += (s.rs + s.fns) * (checkedOfficials.Count + 1) + s.rb;
                                }
                                else if (history[i] == A2.TakeBride){
                                    //�������� ������ ������
                                    if (!inpCh){
                                        //��������� �� �������� ��������
                                        cfVal[s.inspectorNumber] += offBossBride;
                                        cfVal[s.controllerNumber] += �s.fbc;
                                    }
                                    else{
                                        //��������� �������� ��������
                                        checkedOfficials.ForEach((x) => cfVal[x.Number] �= x.S);
                                        cfVal[bossNumber] += �CActionckedSteal � s.fs � s.fb;
                                        cfVal[s.inspectorNumber] += �s.fq;
                                        cfVal[s.controllerNumber] += s.rq;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        private Employee findByNumber(Employee boss, int number){
            if (boss.Number == number)
                return boss;
            if (boss.Subordinates != null)
                foreach (var s in boss.Subordinates){
                    var ans = findByNumber(s, number);
                    if (ans != null)
                        return ans;
                }
            return null;
        }
    }
    public class Program{
        static void Demo2(){
            Employee hierarchy = new Employee("official1", 80, 24,
                new[] { new Employee("official1", 40, 12, null) }
            );
            Settings2 settings2 = new Settings2(hierarchy)
            { co = 2, ci = 2, fs = 110, fb = 15, fq = 4, fns = 2, fe = 1, fbc = 10, rs = 4, rb = 1, rq = 10 };
            GameRooles2 gr2 = new GameRooles2(settings2);
            Cfr cfr1 = new Cfr(gr2, false);
            cfr1.Init();
            cfr1.Iterate(1);
            Console.WriteLine("�������: " + Tools.StrategyToStr(cfr1.ReturnUtil()));
            Tools.printTree(Console.Out, Tools.getTree(cfr1.root, gr2));
            Tools.getTree2(cfr1.root).ToList().ForEach(x => Tools.printTree(Console.Out, x));
            Console.WriteLine("C���� ���������: " + Tools.GetImmRegSum(cfr1).ToString());
            Cfr cfr2 = new Cfr(gr2, false);
            cfr2.Init();
            cfr2.Iterate(10000);
            Console.WriteLine("�������: " + Tools.StrategyToStr(cfr2.ReturnUtil()));
            Tools.printTree(Console.Out, Tools.getTree(cfr2.root, gr2));
            Tools.getTree2(cfr2.root).ToList().ForEach(x => Tools.printTree(Console.Out, x));
            Console.WriteLine("C���� ���������: " + Tools.GetImmRegSum(cfr2).ToString());
        }
        static void Main(string[] args){
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en�US");
            Demo2();
        }
    }
}
