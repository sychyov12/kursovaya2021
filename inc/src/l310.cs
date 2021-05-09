//Преобразование массива byte в массив ulong
public void Byte64ToUlong8(byte[] a, ulong[] b)
{
    for (int i = 0; i < 8; i++)
    {
        b[i] = (ulong)a[i * 8] | ((ulong)a[i * 8 + 1] << 8) |
          ((ulong)a[i * 8 + 2] << 16) | ((ulong)a[i * 8 + 3] << 24) |
          ((ulong)a[i * 8 + 4] << 32) | ((ulong)a[i * 8 + 5] << 40) |
          ((ulong)a[i * 8 + 6] << 48) | ((ulong)a[i * 8 + 7] << 56);
    }
}
//Преобразование массива ulong в массив byte
public void Ulong8ToByte64(ulong[] a, byte[] b)
{
    for (int i = 0; i < 64; i++)
    {
        b[i] = (byte)(a[i / 8] >> (i % 8) * 8);
    }
}