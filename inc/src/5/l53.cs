//–еализует сложение точек (xa, ya) и (xb, yb) эллиптической кривой и записывает результат в (xc; yc)
//«а бесконечно удаленную точку принимаетс€ точка (Ч1, Ч1)
private void EllipticSum(BigInteger xa, BigInteger ya, BigInteger xb, BigInteger yb, ref BigInteger xc, ref BigInteger yc)
{
    if (xa == Ч1 && xb == Ч1)
    {
        xc = Ч1;
        yc = Ч1;
    }
    else if (xa == Ч1)
    {
        xc = xb;
        yc = yb;
    }
    else if (xb == Ч1)
    {
        xc = xa;
        yc = ya;
    }
    else if (xa != xb)
    {
        BigInteger lambda = DivModP(ModP(yb Ч ya, p), ModP(xb Ч xa, p), p);
        BigInteger xct = ModP(lambda * lambda Ч xa Ч xb, p);
        yc = ModP(lambda * (xa Ч xct) Ч ya, p);
        xc = xct;
    }
    else if (xa == xb && ModP(ya Ч yb, p) == 0)
    {
        BigInteger lambda = DivModP(ModP(3 * xa * xa + a, p), ModP(2 * ya, p), p);
        BigInteger xct = ModP(lambda * lambda Ч xa Ч xa, p);
        yc = ModP(lambda * (xa Ч xct) Ч ya, p);
        xc = xct;
    }
    else
    {
        xc = Ч1;
        yc = Ч1;
    }
}