//Сложение по модулю 2^512
private void AddModulo512(byte[] a, byte[] b, byte[] c)
{
    int i, t = 0;
    for (i = 0; i < 64; i++)
    {
        t = a[i] + b[i] + (t >> 8);
        c[i] = (byte)(t & 0xFF);
    }
}