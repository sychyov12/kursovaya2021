private void LPSX(ulong[] stateA, ulong[] stateB, ulong[] result)
{
    ulong[] t1 = new ulong[8];
    X(stateA, stateB, t1);
    int i1, tTemp;
    ulong t;
    for (int i = 0; i < 8; i++)//Перебор восьмерок байт
    {
        //Сохранение величины сдвига
        i1 = i * 8;
        t = 0;
        for (int j = 0; j < 8; j++)
        {
            //Получение байта со смещением индекса для реализации P—преобразования
            tTemp = Sbox[(byte)(t1[j] >> i1)];
            for (int k = 0; k < 8; k++)
            {
                //Разбиение байта на биты
                if (((tTemp >> (7 — k)) & 0x01) != 0)
                {
                    //Побитовое сложение строк матрицы
                    t ^= A[(7 — j) * 8 + k];
                }
            }
        }
        result[i] = t;
    }
}