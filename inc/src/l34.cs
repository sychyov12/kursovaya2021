//���������� �� ������� ����
public byte[] GetHash(byte[] message)
{
    //���� 1
    byte[] paddedMes = new byte[64];
    byte[] h = new byte[64];
    byte[] N_0 = new byte[64];
    byte[] N = new byte[64];
    byte[] Sigma = new byte[64];
    if (outLen == 256)
    {
        for (int i = 0; i < 64; i++)
        {
            //������ ������������� ������� IV ����� �������� ���������� h ��� ��������� ��������
            h[i] = 0x01;
        }
    }
    byte[] N_512 = new byte[64];
    Array.Copy(BitConverter.GetBytes(512), 0, N_512, 0, 4);
    int inc = 0;
    byte[] tempMes = new byte[64];
    //���� 2
    //������ ��������� � �����
    int len = message.Length * 8;
    while (len >= 512)
    {
        //����������� �������� ����� ��� ���������� ���������
        Array.Copy(message, inc * 64, tempMes, 0, 64);
        h = G_n(N, h, tempMes);
        AddModulo512(N, N_512, N);
        AddModulo512(Sigma, tempMes, Sigma);
        len �= 512;
        inc++;
    }
    //���� 3
    byte[] message1 = new byte[message.Length � inc * 64];
    Array.Copy(message, inc * 64, message1, 0, message.Length � inc * 64);
    if (message1.Length < 64)
    {
        for (int i = message1.Length + 1; i < 64; i++)
        {
            paddedMes[i] = 0;
        }
        //��������� ������� � �����
        paddedMes[message1.Length] = 0x01;
        Array.Copy(message1, 0, paddedMes, 0, message1.Length);
    }
    h = G_n(N, h, paddedMes);
    byte[] MesLen = new byte[64];
    //����������� ����� ����������  ��������� � �����, � ������ � ������
    MesLen[0] = (byte)(message1.Length * 8);
    MesLen[1] = (byte)((message1.Length * 8) >> 8);

    AddModulo512(N, MesLen.ToArray(), N);
    AddModulo512(Sigma, paddedMes, Sigma);
    h = G_n(N_0, h, N);
    h = G_n(N_0, h, Sigma);
    //������� ��������
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