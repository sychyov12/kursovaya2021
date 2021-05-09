//¬озвращает случайное число в диапазоне от 0 до p Ч 1
private BigInteger RandomBelow(BigInteger p)
{
    byte[] bytes = p.ToByteArray();
    rand.NextBytes(bytes);
    bytes[bytes.Length Ч 1] &= (byte)0x7F;
    return new BigInteger(bytes) % p;
}