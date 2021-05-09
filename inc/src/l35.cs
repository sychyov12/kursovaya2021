//Поточная реализация хешфункции
public byte[] GetHash(Stream st)
{
    //Этап 1
    byte[] paddedMes = new byte[64];
    byte[] h = new byte[64];
    byte[] N_0 = new byte[64];
    byte[] N = new byte[64];
    byte[] Sigma = new byte[64];
    if (outLen == 256)
    {
        for (int i = 0; i < 64; i++)
        {
            h[i] = 0x01;
        }
    }
    byte[] N_512 = new byte[64];
    Array.Copy(BitConverter.GetBytes(512), 0, N_512, 0, 4);
    byte[] tempMes = new byte[64];
    //Чтение блока из потока
    int blockLen = st.Read(tempMes, 0, 64);
    int progress = 0;
    //Этап 2
    while (blockLen == 64)
    {
        h = G_n(N, h, tempMes);
        AddModulo512(N, N_512, N);
        AddModulo512(Sigma, tempMes, Sigma);
        //Чтение блока из потока
        blockLen = st.Read(tempMes, 0, 64);
        progress++;
        if (progress % 2048 == 0 && Hash1MBitStep != null)
            Hash1MBitStep(progress);
    }
    //Этап 3
    byte[] message1 = new byte[blockLen];
    Array.Copy(tempMes, 0, message1, 0, blockLen);
    if (message1.Length < 64)
    {
        for (int i = message1.Length + 1; i < 64; i++)
        {
            paddedMes[i] = 0;
        }
        paddedMes[message1.Length] = 0x01;
        Array.Copy(message1, 0, paddedMes, 0, message1.Length);
    }
    h = G_n(N, h, paddedMes);
    byte[] MesLen = new byte[64];
    MesLen[0] = (byte)(message1.Length * 8);
    MesLen[1] = (byte)((message1.Length * 8) >> 8);

    AddModulo512(N, MesLen.ToArray(), N);
    AddModulo512(Sigma, paddedMes, Sigma);
    h = G_n(N_0, h, N);
    h = G_n(N_0, h, Sigma);

    if (outLen == 512)
    {
        return h;
    }
    else
    {
        byte[] h256 = new byte[32];
        Array.Copy(h, 32, h256, 0, 32);
        return h256;
    }
}