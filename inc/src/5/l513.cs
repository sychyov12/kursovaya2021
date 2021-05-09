//Конструктор с параметрами
public GR3410_2012_Main(byte[] p, byte[] a, byte[] b, byte[] m, byte[] q, byte[] xp, byte[] yp, int l)
{
    this.p = GetPositive(p);
    this.a = GetPositive(a);
    this.b = GetPositive(b);
    this.m = GetPositive(m);
    this.q = GetPositive(q);
    this.xp = GetPositive(xp);
    this.yp = GetPositive(yp);
    this.l = l;
    rand = new Random();
}
//Другой вариант конструктора с параметрами
public GR3410_2012_Main(GR3410_2012_Parameters parameters)
{
    this.p = GetPositive(parameters.P);
    this.a = GetPositive(parameters.A);
    this.b = GetPositive(parameters.B);
    this.m = GetPositive(parameters.M);
    this.q = GetPositive(parameters.Q);
    this.xp = GetPositive(parameters.Xp);
    this.yp = GetPositive(parameters.Yp);
    this.l = parameters.L;
    rand = new Random();
}