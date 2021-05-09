//Осуществляет перевод в двоичную форму и конкатенацию чисел a и b
private byte[] BinVectorConc(BigInteger a, BigInteger b)
{
    int zetaL = l * 2 / 8;
    byte[] zeta = new byte[zetaL];
    byte[] bytesA = a.ToByteArray();
    byte[] bytesB = b.ToByteArray();
    Array.Copy(bytesA, 0, zeta, 0, Math.Min(bytesA.Length, l / 8));
    Array.Copy(bytesB, 0, zeta, zetaL / 2, Math.Min(bytesB.Length, l / 8));
    return zeta;
}