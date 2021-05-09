//��������� �������� �������� ���������� ����������� ����� (x, y) �� k � ���������� ��������� � (x1, y1)
private void ScalarProd(BigInteger k, BigInteger x, BigInteger y, ref BigInteger x1, ref BigInteger y1)
{
    byte[] kBytes = k.ToByteArray();
    BigInteger p2x = x, p2y = y;
    x1 = �1; y1 = �1;
    for (int i = 0; i < kBytes.Count<byte>(); i++)
    {
        for (int j = 0; j < 8; j++)
        {
            if ((kBytes[i] >> j & 0x01) > 0)
            {
                EllipticSum(x1, y1, p2x, p2y, ref x1, ref y1);
            }
            EllipticSum(p2x, p2y, p2x, p2y, ref p2x, ref p2y);
        }
    }
}