using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace CorruptionModel{
    //набор всех возможных действий
    public static class A2{
        public static string DontSteal = "ЌеCов р",
            Steal = "—ов р", DontCheckOfficial = "Ќеѕров—от",
            CheckOfficial = "ѕров—от", OfferBride = "ѕредл¬з",
            Agree = "¬ыплЎт", ExposeBoss = "–аз–ук",
            TakeBride = "ѕрин¬з", RejectBride = "ќткл¬з",
            CheckInspector = "ѕров„ин", DontCheckInspector = "Ќеѕров„ин";
    }
    //параметры
    public class Settings2{
        public int NumOfPlayers;
        public Employee Hierarchy;
        public int numOfOfficials { get { return NumOfPlayers Ц 2; } }
        public int inspectorNumber { get { return NumOfPlayers Ц 2; } }
        public int controllerNumber { get { return NumOfPlayers Ц 1; } }

        public double co, ci, fs, fb, fq, fns, fe, fbc, rs, rb, rq;
        public Settings2(Employee hierarchy){
            Hierarchy = hierarchy;
            NumOfPlayers = hierarchy.GetCount() + 2;
            hierarchy.UpdateBoss();
        }
    }
    //информаци€ о сотруднике и подчиненных
    public class Employee{
        public string Name { get; set; }
        public double S, B;
        public Employee Boss = null;
        public Employee[] Subordinates = null;
        public int Number = Ц1;
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
    //реализаци€ правил игры дл€ иерархической модели коррупции
    public class GameRooles2 : IGameRooles{
        public int NumOfPlayers => s.NumOfPlayers;
        public Settings2 s;
        public GameRooles2(Settings2 settings){
            s = settings;
        }
        bool[] steals = null;
        //функци€ дл€ создани€ игрового дерева
        public GameTreeNode GenerateRoot(){
            if (steals == null) steals = new bool[s.NumOfPlayers Ц 2];
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
                nextOffch.Name = string.Format("Ётап 2. –ешение сотрудника {0} о краже", offNumber);
                if (offch != null)
                    offch.PossibleChilds = new List<GameTreeNode>(new[] { nextOffch });
                else{
                    GameTreeNode iich = new GameTreeNode(s.NumOfPlayers, NodeType.PlayNode, s.controllerNumber,
                        new[] { A2.DontCheckInspector, A2.CheckInspector });
                    iich.Name = string.Format("Ётап 1. –ешение инспектора о проверке");
                    iich.PossibleChilds = new List<GameTreeNode>(new[] { nextOffch });
                    root = iich;
                }
                offch = nextOffch;
                if (tmpOfficial.Subordinates != null)
                    foreach (var s in tmpOfficial.Subordinates)
                        officials.Enqueue(s);
                offNumber++;
            }
            //терминальный узел
            GameTreeNode terminal = new GameTreeNode(s.NumOfPlayers, NodeType.TerminalNode, Ц1, null);
            terminal.Name = string.Format("Ётап 5: “ерминальный узел");
            var inspch = new GameTreeNode(s.NumOfPlayers, NodeType.PlayNode, s.inspectorNumber,
                new[] { A2.CheckOfficial, A2.DontCheckOfficial });
            inspch.Name = string.Format("Ётап 3: –ешение чиновника о проверке сотрудника {0}", 0);
            //возможна проверка первого лица
            offch.PossibleChilds = new List<GameTreeNode>(new[] { inspch });
            //предложить вл€тку либо смиритьс€ со штрафом
            GameTreeNode off0SI = new GameTreeNode(s.NumOfPlayers, NodeType.PlayNode, 0, new[] { A2.OfferBride, A2.Agree });
            off0SI.Name = string.Format("Ётап 4:  ража сотрудника {0} раскрыта", 0);
            GameTreeNode inp0SIB = new GameTreeNode(s.NumOfPlayers, NodeType.PlayNode, s.inspectorNumber,
                new[] { A2.TakeBride, A2.RejectBride });
            inp0SIB.Name = string.Format("Ётап 4:  ража сотрудника {0} раскрыта. ѕредложена вз€тка", 0);
            off0SI.PossibleChilds = new List<GameTreeNode>(new[] { inp0SIB, terminal });
            //прин€тие либо отклонение вз€тки
            inp0SIB.PossibleChilds = new List<GameTreeNode>(new[] { terminal });
            var offISI = off0SI;
            if (hierarchy.Subordinates != null)
                foreach (var s in hierarchy.Subordinates)
                    officials.Enqueue(s);
            while (officials.Count > 0){
                Employee tmpOfficial = officials.Dequeue();
                var nextInspch = new GameTreeNode(s.NumOfPlayers, NodeType.PlayNode, s.inspectorNumber,
                    new[] { A2.CheckOfficial, A2.DontCheckOfficial });
                nextInspch.Name = string.Format("Ётап 3: –ешение чиновника о проверке сотрудника {0}", tmpOfficial.Number);
                //руководитель предлагает вз€тку
                GameTreeNode inpSI2EB = new GameTreeNode(s.NumOfPlayers, NodeType.PlayNode, s.inspectorNumber,
                    new[] { A2.TakeBride, A2.RejectBride });
                inpSI2EB.Name = string.Format("Ётап 4:  ража сотрудника {0} раскрыта после разоблачени€. ѕредложена вз€тка",
                    tmpOfficial.Boss.Number);
                inpSI2EB.PossibleChilds = new List<GameTreeNode>(new[] { terminal });
                //сотрудник доносит на руководител€, и он тоже совершил кражу
                GameTreeNode offIBossSIE = new GameTreeNode(s.NumOfPlayers, NodeType.PlayNode, tmpOfficial.Boss.Number,
                    new[] { A2.OfferBride, A2.Agree });
                offIBossSIE.Name = string.Format("Ётап 4:  ража сотрудника {0} раскрыта после разоблачени€", tmpOfficial.Boss.Number);
                offIBossSIE.PossibleChilds = new List<GameTreeNode>(new[] { inpSI2EB, terminal });
                //вз€тка подчиненного
                GameTreeNode inpISB = new GameTreeNode(s.NumOfPlayers, NodeType.PlayNode, s.inspectorNumber,
                    new[] { A2.TakeBride, A2.RejectBride });
                inpISB.Name = string.Format("Ётап 4:  ража сотрудника {0} раскрыта. ѕредложена вз€тка", tmpOfficial.Number);
                inpISB.PossibleChilds = new List<GameTreeNode>(new[] { terminal });
                //совершивший кражу подчиненный проверен
                GameTreeNode nextOffISI = new GameTreeNode(s.NumOfPlayers, NodeType.PlayNode, tmpOfficial.Number,
                    new[] { A2.ExposeBoss, A2.OfferBride, A2.Agree });
                nextOffISI.Name = string.Format("Ётап 4:  ража сотрудника {0} раскрыта", tmpOfficial.Number);
                nextOffISI.PossibleChilds = new List<GameTreeNode>(new[] { inpISB, offIBossSIE, terminal });
                //прин€тие инспектором решени€ о проверке текущего подчиненного либо переход к следующему подчиненному
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
        //метод дл€ обновлени€ истори
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
                while (offCh < Math.Min(history.Count Ц 1, s.NumOfPlayers Ц 2)){
                    if (history[i] == A2.Steal)
                        steals[offCh] = true;
                    else
                        steals[offCh] = false;
                    i++;
                    offCh++;
                }
                if (offCh < s.NumOfPlayers Ц 2){
                    node.Childs.Add(node.PossibleChilds[0]);
                    node.Childs.Add(node.PossibleChilds[0]);
                }
                else{
                    while (checkedOfficial < Math.Min(history.Count Ц 1 Ц (s.NumOfPlayers Ц 2), s.NumOfPlayers Ц 2)){
                        checkedOfficial++;
                        if (history[i] == A2.CheckOfficial){
                            officialChecked = true; i++; break;
                        }
                        i++;
                    }
                    if (!officialChecked && checkedOfficial < s.NumOfPlayers Ц 2){
                        //случай продолжени€ цикла проверок/переходов к следующему
                        if (steals[checkedOfficial])
                            node.Childs.Add(node.PossibleChilds[1]);
                        else
                            node.Childs.Add(node.PossibleChilds[2]);
                        node.Childs.Add(node.PossibleChilds[0]);
                    }
                    else if (officialChecked && steals[checkedOfficial Ц 1]){
                        //проверка вы€вила кражу (иначе terminal)
                        Employee chOfficial = findByNumber(s.Hierarchy, checkedOfficial Ц 1);
                        while (i < history.Count && history[i] == A2.ExposeBoss){
                            chOfficial = chOfficial.Boss;
                            i++;
                        }
                        if (i == history.Count){
                            //предложить вз€тку, выплатить штраф, разоблачить руководител€
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
                            //прин€ть или отклонить вз€тку
                            node.Childs.Add(node.PossibleChilds[0]);
                            node.Childs.Add(node.PossibleChilds[0]);
                        }
                    }
                }
            }
        }
        private List<Employee> checkedOfficials = new List<Employee>();
        //функци€ дл€ начислени€ терминальных выплат
        public void SetTerminalEquity(GameTreeNode terminalNode){
            terminalNode.CfValues = terminalNode.CfValues == null ? new double[s.NumOfPlayers] : terminalNode.CfValues;
            var cfVal = terminalNode.CfValues;
            Array.Clear(cfVal, 0, terminalNode.CfValues.Length);
            var history = terminalNode.History;
            int i = 0;
            //решение инспектора о проверке
            bool inpCh = history[i] == A2.CheckInspector;
            if (inpCh) cfVal[s.controllerNumber] = Цs.ci;
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
            //происходит начисление штрафа чиновнику за все совершенные кражи
            cfVal[s.inspectorNumber] Ц= steals.Count(x => x) * s.fns;
            if (checkedOff != s.numOfOfficials){
                //проверка сотрудника
                cfVal[s.inspectorNumber] Ц= s.co;
                if (steals[checkedOff]){
                    //провер€емый сотрудник совершил кражу
                    double CActionckedSteal = findByNumber(s.Hierarchy, checkedOff).S;
                    if (history[i] == A2.Agree){
                        //сотрудник согласен со штрафом
                        cfVal[checkedOff] += ЦCActionckedSteal Ц s.fs;
                        cfVal[s.inspectorNumber] += s.rs + s.fns;
                    }
                    else if (history[i] == A2.OfferBride){
                        //сотрудник предложил вз€тку
                        i++;
                        double offBride = findByNumber(s.Hierarchy, checkedOff).B;
                        cfVal[checkedOff] Ц= offBride;
                        if (history[i] == A2.RejectBride){
                            //чиновник отклонил вз€тку
                            cfVal[checkedOff] += ЦCActionckedSteal Ц s.fs Ц s.fb;
                            cfVal[s.inspectorNumber] += s.rs + s.rb + s.fns;
                        }
                        else if (history[i] == A2.TakeBride){
                            //чиновник прин€л вз€тку
                            if (!inpCh){
                                //инспектор не проводит проверку
                                cfVal[s.inspectorNumber] += offBride;
                                cfVal[s.controllerNumber] += Цs.fbc;
                            }
                            else{
                                //инспектор проводит проверку
                                cfVal[checkedOff] += ЦCActionckedSteal Ц s.fs Ц s.fb;
                                cfVal[s.inspectorNumber] += Цs.fq;
                                cfVal[s.controllerNumber] += s.rq;
                            }
                        }
                    }
                    else if (history[i] == A2.ExposeBoss){
                        //сотрудник донес на руководител€
                        checkedOfficials.Clear();
                        checkedOfficials.Add(findByNumber(s.Hierarchy, checkedOff));
                        checkedOfficials.Add(findByNumber(s.Hierarchy, checkedOff));
                        int bossNumber = findByNumber(s.Hierarchy, checkedOff).Boss.Number;
                        i++;
                        if (!steals[bossNumber]){
                            //руководитель не совершал кражу
                            checkedOfficials.ForEach((x) => cfVal[x.Number] Ц= x.S);
                            cfVal[checkedOfficials.Last().Number] += Цs.fs Ц s.fe;
                            cfVal[s.inspectorNumber] += checkedOfficials.Count * (s.rs + s.fns);
                        }
                        else{
                            //руководитель совершил кражу
                            double cActionckedBossSteal = findByNumber(s.Hierarchy, bossNumber).S;
                            if (history[i] == A2.Agree){
                                //руководитель согласен со штрафом
                                checkedOfficials.ForEach((x) => cfVal[x.Number] Ц= x.S);
                                cfVal[bossNumber] += ЦcActionckedBossSteal Ц s.fs;
                                cfVal[s.inspectorNumber] += (s.rs + s.fns) * (checkedOfficials.Count + 1);
                            }
                            else if (history[i] == A2.OfferBride){
                                //руководитель предложил вз€тку
                                i++;
                                double offBossBride = findByNumber(s.Hierarchy, bossNumber).B;
                                cfVal[bossNumber] Ц= offBossBride;

                                if (history[i] == A2.RejectBride){
                                    //чиновник отклонил вз€тку
                                    checkedOfficials.ForEach((x) => cfVal[x.Number] Ц= x.S);
                                    cfVal[bossNumber] += ЦcActionckedBossSteal Ц s.fs Ц s.fb;
                                    cfVal[s.inspectorNumber] += (s.rs + s.fns) * (checkedOfficials.Count + 1) + s.rb;
                                }
                                else if (history[i] == A2.TakeBride){
                                    //чиновник прин€л вз€тку
                                    if (!inpCh){
                                        //инспектор не проводит проверку
                                        cfVal[s.inspectorNumber] += offBossBride;
                                        cfVal[s.controllerNumber] += Цs.fbc;
                                    }
                                    else{
                                        //инспектор проводит проверку
                                        checkedOfficials.ForEach((x) => cfVal[x.Number] Ц= x.S);
                                        cfVal[bossNumber] += ЦCActionckedSteal Ц s.fs Ц s.fb;
                                        cfVal[s.inspectorNumber] += Цs.fq;
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
            Console.WriteLine("¬ыплаты: " + Tools.StrategyToStr(cfr1.ReturnUtil()));
            Tools.printTree(Console.Out, Tools.getTree(cfr1.root, gr2));
            Tools.getTree2(cfr1.root).ToList().ForEach(x => Tools.printTree(Console.Out, x));
            Console.WriteLine("Cумма сожалений: " + Tools.GetImmRegSum(cfr1).ToString());
            Cfr cfr2 = new Cfr(gr2, false);
            cfr2.Init();
            cfr2.Iterate(10000);
            Console.WriteLine("¬ыплаты: " + Tools.StrategyToStr(cfr2.ReturnUtil()));
            Tools.printTree(Console.Out, Tools.getTree(cfr2.root, gr2));
            Tools.getTree2(cfr2.root).ToList().ForEach(x => Tools.printTree(Console.Out, x));
            Console.WriteLine("Cумма сожалений: " + Tools.GetImmRegSum(cfr2).ToString());
        }
        static void Main(string[] args){
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("enЦUS");
            Demo2();
        }
    }
}
