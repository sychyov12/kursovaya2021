//���������� ��������� ����� � ��������� �� 0 �� p � 1
private BigInteger RandomBelow(BigInteger p)
{
    byte[] bytes = p.ToByteArray();
    rand.NextBytes(bytes);
    bytes[bytes.Length � 1] &= (byte)0x7F;
    return new BigInteger(bytes) % p;
}