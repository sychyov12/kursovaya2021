//Расширенный алгоритм Евклида
private BigInteger[] EGCD(BigInteger a, BigInteger b)
{
    if (b == 0)
        return new BigInteger[] { a, 1, 0 };
    BigInteger[] t = EGCD(b, a % b);
    return new BigInteger[] { t[0], t[2], t[1] — t[2] * (a / b) };
}
//Вычисление обратного к a элемента по модулю p. Возвращает 0 в случае необратимости
private BigInteger Inverse(BigInteger a, BigInteger p)
{
    BigInteger[] t = EGCD(a, p);
    if (t[0] != 1)
        return 0;
    return ModP(t[1], p);
}
//Возвращает a по модулю p
private BigInteger ModP(BigInteger a, BigInteger p)
{
    if (a >= 0)
        return a % p;
    else
        return a % p + p;
}
//a/b mod p
private BigInteger DivModP(BigInteger a, BigInteger b, BigInteger p)
{
    BigInteger c = Inverse(b, p);
    if (c == 0)
        throw new Exception("Деление на необратимый элемент");
    return ModP(a * c, p);
}