//Класс параметров схемы цифровой подписи
[Serializable]
public class GR3410_2012_Parameters
{
    public byte[] P { get; set; }
    public byte[] A { get; set; }
    public byte[] B { get; set; }
    public byte[] M { get; set; }
    public byte[] Q { get; set; }
    public byte[] Xp { get; set; }
    public byte[] Yp { get; set; }
    public int L { get; set; }

    public GR3410_2012_Parameters() { }

    public GR3410_2012_Parameters(byte[] p, byte[] a, byte[] b, byte[] m, byte[] q, byte[] xp, byte[] yp, int l)
    {
        P = (byte[])p.Clone();
        A = (byte[])a.Clone();
        B = (byte[])b.Clone();
        M = (byte[])m.Clone();
        Q = (byte[])q.Clone();
        Xp = (byte[])xp.Clone();
        Yp = (byte[])yp.Clone();
        L = l;
    }
}
//Класс ключа подписи
[Serializable]
public class GR3410_2012_SignKey
{
    public byte[] D { get; set; }

    public GR3410_2012_SignKey() { }

    public GR3410_2012_SignKey(byte[] d)
    {
        D = (byte[])d.Clone();
    }
}
//Класс ключа проверки подписи
[Serializable]
public class GR3410_2012_VerifyKey
{
    public byte[] Xq { get; set; }
    public byte[] Yq { get; set; }

    public GR3410_2012_VerifyKey() { }

    public GR3410_2012_VerifyKey(byte[] xq, byte[] yq)
    {
        Xq = (byte[])xq.Clone();
        Yq = (byte[])yq.Clone();
    }
}
//Класс подписи
[Serializable]
public class GR3410_2012_Sign
{
    public byte[] Sign { get; set; }

    public GR3410_2012_Sign() { }

    public GR3410_2012_Sign(byte[] sign)
    {
        Sign = (byte[])sign.Clone();
    }
}