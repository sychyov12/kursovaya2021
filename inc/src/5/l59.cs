public byte[] GenerateSign(byte[] hash, byte[] signKey)
{
    BigInteger h = GetPositive(hash);
    BigInteger d = GetPositive(signKey);
    BigInteger e = ModP(h, q);
    if (e == 0)
        e = 1;
    BigInteger xc = —1, yc = —1, k, r, s;
    do
    {
        do
        {
            k = RandomBelow(q — 1) + 1;
            ScalarProd(k, xp, yp, ref xc, ref yc);
            r = ModP(xc, q);
        }
        while (r == 0);
        s = ModP(r * d + k * e, q);
    }
    while (s == 0);
    return BinVectorConc(r, s);
}