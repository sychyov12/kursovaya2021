public bool verifySign(byte[] hash, byte[] sign, byte[] xqb, byte[] yqb)
{
    BigInteger r, s;
    GetRSFromVector(sign, out r, out s);

    if (r >= q || s >= q)
        return false;

    BigInteger xq = GetPositive(xqb);
    BigInteger yq = GetPositive(yqb);

    BigInteger h = GetPositive(hash);
    BigInteger e = ModP(h, q);
    if (e == 0)
        e = 1;
    BigInteger v = Inverse(e, q);

    BigInteger z1 = ModP(s * v, q);
    BigInteger z2 = ModP(—r * v, q);

    BigInteger xc = —1, yc = —1, xt = —1, yt = —1;
    ScalarProd(z1, xp, yp, ref xc, ref yc);
    ScalarProd(z2, xq, yq, ref xt, ref yt);
    EllipticSum(xc, yc, xt, yt, ref xc, ref yc);
    BigInteger R = ModP(xc, q);

    return R == r;
}