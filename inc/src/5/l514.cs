//Второй вариант генерации подписи
public GR3410_2012_Sign GenerateSign(byte[] hash, GR3410_2012_SignKey signKey)
{
    return new GR3410_2012_Sign(GenerateSign(hash, signKey.D));
}
//Второй вариант проверки подписи
public bool verifySign(byte[] hash, GR3410_2012_Sign sign, GR3410_2012_VerifyKey verifyKey)
{
    return verifySign(hash, sign.Sign, verifyKey.Xq, verifyKey.Yq);
}