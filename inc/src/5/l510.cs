//Получение значений r и s из цифровой подписи
private void GetRSFromVector(byte[] zeta, out BigInteger r, out BigInteger s)
{
    byte[] bytesR = new byte[l / 8];
    Array.Copy(zeta, 0, bytesR, 0, l / 8);
    r = GetPositive(bytesR);

    byte[] bytesS = new byte[l / 8];
    Array.Copy(zeta, l / 8, bytesS, 0, l / 8);
    s = GetPositive(bytesS);
}