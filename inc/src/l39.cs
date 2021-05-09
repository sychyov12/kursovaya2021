//Вспомогательные переменные функции сжатия
private byte[] newh = new byte[64];
private ulong[] N64 = new ulong[8];
private ulong[] h64 = new ulong[8];
private ulong[] m64 = new ulong[8];
private ulong[] lResultG = new ulong[8];
private ulong[] lResultK1 = new ulong[8];
private ulong[] lResultK2 = new ulong[8];
private ulong[] lResultE1 = new ulong[8];
private ulong[] lResultE2 = new ulong[8];
private ulong[] t = new ulong[8];
private ulong[] newh64 = new ulong[8];
private ulong[] state = new ulong[8];
//Матрица предпросчета
private ulong[,] LSPrecalc = new ulong[256, 8];