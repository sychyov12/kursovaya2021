//Расчет Ki
private ulong[] KeySchedule(ulong[] K, int i)
{
    //Проверка на запись в себя
    if (K.Equals(lResultK1))
    {
        LPSX(K, C[i], lResultK2);
        return lResultK2;
    }
    else
    {
        LPSX(K, C[i], lResultK1);
        return lResultK1;
    }
}

private ulong[] E(ulong[] K, ulong[] m)
{
    Array.Copy(m, state, 8);
    for (int i = 0; i < 12; i++)
    {
        //Проверка на запись в себя
        if (state.Equals(lResultE1))
        {
            LPSX(state, K, lResultE2);
            state = lResultE2;
        }
        else
        {
            LPSX(state, K, lResultE1);
            state = lResultE1;
        }
        K = KeySchedule(K, i);
    }
    X(state, K, state);
    return state;
}

private byte[] G_n(byte[] N, byte[] h, byte[] m)
{
    Byte64ToUlong8(N, N64);
    Byte64ToUlong8(h, h64);
    Byte64ToUlong8(m, m64);
    LPSX(h64, N64, lResultG);
    t = E(lResultG, m64);
    X(t, h64, t);
    X(t, m64, newh64);
    Ulong8ToByte64(newh64, newh);
    return newh;
}