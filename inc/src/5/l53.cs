//��������� �������� ����� (xa, ya) � (xb, yb) ������������� ������ � ���������� ��������� � (xc; yc)
//�� ���������� ��������� ����� ����������� ����� (�1, �1)
private void EllipticSum(BigInteger xa, BigInteger ya, BigInteger xb, BigInteger yb, ref BigInteger xc, ref BigInteger yc)
{
    if (xa == �1 && xb == �1)
    {
        xc = �1;
        yc = �1;
    }
    else if (xa == �1)
    {
        xc = xb;
        yc = yb;
    }
    else if (xb == �1)
    {
        xc = xa;
        yc = ya;
    }
    else if (xa != xb)
    {
        BigInteger lambda = DivModP(ModP(yb � ya, p), ModP(xb � xa, p), p);
        BigInteger xct = ModP(lambda * lambda � xa � xb, p);
        yc = ModP(lambda * (xa � xct) � ya, p);
        xc = xct;
    }
    else if (xa == xb && ModP(ya � yb, p) == 0)
    {
        BigInteger lambda = DivModP(ModP(3 * xa * xa + a, p), ModP(2 * ya, p), p);
        BigInteger xct = ModP(lambda * lambda � xa � xa, p);
        yc = ModP(lambda * (xa � xct) � ya, p);
        xc = xct;
    }
    else
    {
        xc = �1;
        yc = �1;
    }
}