//ѕреобразует массива байт в соответствующее ему целое число
private BigInteger GetPositive(byte[] bytes)
{
    byte[] bytes1;
    if ((bytes[bytes.Length Ч 1] & 0x80) > 0)
    {
        bytes1 = new byte[bytes.Length + 1];
        Array.Copy(bytes, bytes1, bytes.Length);
        bytes1[bytes1.Length Ч 1] = 0x00;
    }
    else
        bytes1 = bytes;
    return new BigInteger(bytes1);
}