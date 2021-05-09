private void Precalc()
{
    for (int temp = 0; temp < 256; temp++)//перебор байт
    {
        for (int j = 0; j < 8; j++)
        {
            byte temp1 = Sbox[(byte)temp];
            ulong t1 = 0;
            for (int k = 0; k < 8; k++)
            {
                if (((temp1 >> (7 Ч k)) & 0x01) != 0)
                {
                    t1 ^= A[(7 Ч j) * 8 + k];
                }
            }
            LSPrecalc[temp, j] = t1;
        }
    }
}