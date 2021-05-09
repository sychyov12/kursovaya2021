using System;
using System.IO;
using System.Linq;
//Класс реализующий функцию хеширования
namespace GR3411_2012
{
    public class GR3411_2012_Hash
    {
        // Матрица A для L функции
        private readonly ulong[] A = {
            0x8E20FAA72BA0B470, 0x47107DDD9B505A38, 
            0xAD08B0E0C3282D1C, 0xD8045870EF14980E,
            0x6C022C38F90A4C07, 0x3601161CF205268D, 
            0x1B8E0B0E798C13C8, 0x83478B07B2468764,
            0xA011D380818E8F40, 0x5086E740CE47C920, 
            0x2843FD2067ADEA10, 0x14AFF010BDD87508,
            0x0AD97808D06CB404, 0x05E23C0468365A02, 
            0x8C711E02341B2D01, 0x46B60F011A83988E,
            0x90DAB52A387AE76F, 0x486DD4151C3DFDB9, 
            0x24B86A840E90F0D2, 0x125C354207487869,
            0x092E94218D243CBA, 0x8A174A9EC8121E5D, 
            0x4585254F64090FA0, 0xACCC9CA9328A8950,
            0x9D4DF05D5F661451, 0xC0A878A0A1330AA6, 
            0x60543C50DE970553, 0x302A1E286FC58CA7,
            0x18150F14B9EC46DD, 0x0C84890AD27623E0, 
            0x0642CA05693B9F70, 0x0321658CBA93C138,
            0x86275DF09CE8AAA8, 0x439DA0784E745554, 
            0xAFC0503C273AA42A, 0xD960281E9D1D5215,
            0xE230140FC0802984, 0x71180A8960409A42, 
            0xB60C05CA30204D21, 0x5B068C651810A89E,
            0x456C34887A3805B9, 0xAC361A443D1C8CD2, 
            0x561B0D22900E4669, 0x2B838811480723BA,
            0x9BCF4486248D9F5D, 0xC3E9224312C8C1A0, 
            0xEFFA11AF0964EE50, 0xF97D86D98A327728,
            0xE4FA2054A80B329C, 0x727D102A548B194E, 
            0x39B008152ACB8227, 0x9258048415EB419D,
            0x492C024284FBAEC0, 0xAA16012142F35760, 
            0x550B8E9E21F7A530, 0xA48B474F9EF5DC18,
            0x70A6A56E2440598E, 0x3853DC371220A247, 
            0x1CA76E95091051AD, 0x0EDD37C48A08A6D8,
            0x07E095624504536C, 0x8D70C431AC02A736, 
            0xC83862965601DD1B, 0x641C314B2B8EE083
        };
        // Подстановка байт
        private readonly byte[] Sbox ={
            0xFC, 0xEE, 0xDD, 0x11, 0xCF, 0x6E, 0x31, 0x16, 0xFB, 
            0xC4, 0xFA, 0xDA, 0x23, 0xC5, 0x04, 0x4D, 0xE9, 0x77, 
            0xF0, 0xDB, 0x93, 0x2E, 0x99, 0xBA, 0x17, 0x36, 0xF1, 
            0xBB, 0x14, 0xCD, 0x5F, 0xC1, 0xF9, 0x18, 0x65, 0x5A, 
            0xE2, 0x5C, 0xEF, 0x21, 0x81, 0x1C, 0x3C, 0x42, 0x8B, 
            0x01, 0x8E, 0x4F, 0x05, 0x84, 0x02, 0xAE, 0xE3, 0x6A, 
            0x8F, 0xA0, 0x06, 0x0B, 0xED, 0x98, 0x7F, 0xD4, 0xD3, 
            0x1F, 0xEB, 0x34, 0x2C, 0x51, 0xEA, 0xC8, 0x48, 0xAB, 
            0xF2, 0x2A, 0x68, 0xA2, 0xFD, 0x3A, 0xCE, 0xCC, 0xB5, 
            0x70, 0x0E, 0x56, 0x08, 0x0C, 0x76, 0x12, 0xBF, 0x72, 
            0x13, 0x47, 0x9C, 0xB7, 0x5D, 0x87, 0x15, 0xA1, 0x96, 
            0x29, 0x10, 0x7B, 0x9A, 0xC7, 0xF3, 0x91, 0x78, 0x6F, 
            0x9D, 0x9E, 0xB2, 0xB1, 0x32, 0x75, 0x19, 0x3D, 0xFF, 
            0x35, 0x8A, 0x7E, 0x6D, 0x54, 0xC6, 0x80, 0xC3, 0xBD, 
            0x0D, 0x57, 0xDF, 0xF5, 0x24, 0xA9, 0x3E, 0xA8, 0x43, 
            0xC9, 0xD7, 0x79, 0xD6, 0xF6, 0x7C, 0x22, 0xB9, 0x03,
            0xE0, 0x0F, 0xEC, 0xDE, 0x7A, 0x94, 0xB0, 0xBC, 0xDC, 
            0xE8, 0x28, 0x50, 0x4E, 0x33, 0x0A, 0x4A, 0xA7, 0x97, 
            0x60, 0x73, 0x1E, 0x00, 0x62, 0x44, 0x1A, 0xB8, 0x38, 
            0x82, 0x64, 0x9F, 0x26, 0x41, 0xAD, 0x45, 0x46, 0x92, 
            0x27, 0x5E, 0x55, 0x2F, 0x8C, 0xA3, 0xA5, 0x7D, 0x69, 
            0xD5, 0x95, 0x3B, 0x07, 0x58, 0xB3, 0x40, 0x86, 0xAC, 
            0x1D, 0xF7, 0x30, 0x37, 0x6B, 0xE4, 0x88, 0xD9, 0xE7, 
            0x89, 0xE1, 0x1B, 0x83, 0x49, 0x4C, 0x3F, 0xF8, 0xFE, 
            0x8D, 0x53, 0xAA, 0x90, 0xCA, 0xD8, 0x85, 0x61, 0x20, 
            0x71, 0x67, 0xA4, 0x2D, 0x2B, 0x09, 0x5B, 0xCB, 0x9B, 
            0x25, 0xD0, 0xBE, 0xE5, 0x6C, 0x52, 0x59, 0xA6, 0x74, 
            0xD2, 0xE6, 0xF4, 0xB4, 0xC0, 0xD1, 0x66, 0xAF, 0xC2, 
            0x39, 0x4B, 0x63, 0xB6
        };
        //Итерационные константы
        private readonly ulong[][] C = {
            new ulong[8]{
                0xDD806559F2A64507, 0x5767436CC744D23, 
                0xA2422A08A460D315, 0x4B7CE09192676901, 
                0x714EB88D7585C4FC, 0x2F6A76432E45D016, 
                0xEBCB2F81C0657C1F, 0xB1085BDA1ECADAE9
            },
            new ulong[8]{
                0xE679047021B19BB7, 0x55DDA21BD7CBCD56, 
                0x5CB561C2DB0AA7CA, 0x9AB5176B12D69958, 
                0x61D55E0F16B50131, 0xF3FEEA720A232B98, 
                0x4FE39D460F70B5D7, 0x6FA3B58AA99D2F1A
            },
            new ulong[8]{
                0x991E96F50ABA0AB2, 0xC2B6F443867ADB31, 
                0xC1C93A376062DB09, 0xD3E20FE490359EB1, 
                0xF2EA7514B1297B7B, 0x6F15E5F529C1F8B, 
                0xA39FC286A3D8435, 0xF574DCAC2BCE2FC7
            },
            new ulong[8]{
                0x220CBEBC84E3D12E, 0x3453EAA193E837F1, 
                0xD8B71333935203BE, 0xA9D72C82ED03D675, 
                0x9D721CAD685E353F, 0x488E857E335C3C7D, 
                0xF948E1A05D71E4DD, 0xEF1FDFB3E81566D2
            },
            new ulong[8]{
                0x601758FD7C6CFE57, 0x7A56A27EA9EA63F5, 
                0xDFFF00B723271A16, 0xBFCD1747253AF5A3, 
                0x359E35D7800FFFBD, 0x7F151C1F1686104A, 
                0x9A3F410C6CA92363, 0x4BEA6BACAD474799
            },
            new ulong[8]{
                0xFA68407A46647D6E, 0xBF71C57236904F35, 
                0xAF21F66C2BEC6B6, 0xCFFAA6B71C9AB7B4, 
                0x187F9AB49AF08EC6, 0x2D66C4F95142A46C, 
                0x6FA4C33B7A3039C0, 0xAE4FAEAE1D3AD3D9
            },
            new ulong[8]{
                0x8886564D3A14D493, 0x3517454CA23C4AF3, 
                0x6476983284A0504, 0x992ABC52D822C37, 
                0xD3473E33197A93C9, 0x399EC6C7E6BF87C9, 
                0x51AC86FEBF240954, 0xF4C70E16EEAAC5EC
            },
            new ulong[8]{
                0xA47F0DD4BF02E71E, 0x36ACC2355951A8D9, 
                0x69D18D2BD1A5C42F, 0xF4892BCB929B0690, 
                0x89B4443B4DDBC49A, 0x4EB7F8719C36DE1E, 
                0x3E7AA020C6E4141, 0x9B1F5B424D93C9A7
            },
            new ulong[8]{
                0x7261445183235ADB, 0xE38DC92CB1F2A60, 
                0x7B2B8A9AA6079C54, 0x800A440BDBB2CEB1, 
                0x3CD955B7E00D0984, 0x3A7D3A1B25894224, 
                0x944C9AD8EC165FDE, 0x378F5A541631229B
            },
            new ulong[8]{
                0x74B4C7FB98459CED, 0x3698FAD1153BB6C3, 
                0x7A1E6C303B7652F4, 0x9FE76702AF69334B, 
                0x1FFFE18A1B336103, 0x8941E71CFF8A78DB, 
                0x382AE548B2E4F3F3, 0xABBEDEA680056F52
            },
            new ulong[8]{
                0x6BCAA4CD81F32D1B, 0xDEA2594AC06FD85D, 
                0xEFBACD1D7D476E98, 0x8A1D71EFEA48B9CA, 
                0x2001802114846679, 0xD8FA6BBBEBAB0761, 
                0x3002C6CD635AFE94, 0x7BCD9ED0EFC889FB
            },
            new ulong[8]{
                0x48BC924AF11BD720, 0xFAF417D5D9B21B99, 
                0xE71DA4AA88E12852, 0x5D80EF9D1891CC86, 
                0xF82012D430219F9B, 0xCDA43C32BCDF1D77, 
                0xD21380B00449B17A, 0x378EE767F11631BA
            }
        };
        //Делегат обработчика события прогресса вычисления (для обратной связи)
        public delegate void sendProgressDel(long progress);
        //Событие отражающее увеличение прогресса вычисления на 1мБит (для обратной связи)
        public event sendProgressDel Hash1MBitStep;
        //Прогресс вычисления (для обратной связи)
        public long progress;
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
        //Длина хеша
        public int OutLen { get; private set; }
        //Конструктор с пареметром длины
        public GR3411_2012_Hash(int outputLenght)
        {
            if (outputLenght != 256 && outputLenght != 512)
                throw new Exception("Указанный размер вывода не поддерживается");
            OutLen = outputLenght;
            Precalc();
        }
        //Предпросчет
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
                        if (((temp1 >> (7 — k)) & 0x01) != 0)
                        {
                            t1 ^= A[(7 — j) * 8 + k];
                        }
                    }
                    LSPrecalc[temp, j] = t1;
                }
            }
        }
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
        //Побитовое сложение по модулю 2 (X)
        private void X(ulong[] a, ulong[] b, ulong[] c)
        {
            c[0] = a[0] ^ b[0];
            c[1] = a[1] ^ b[1];
            c[2] = a[2] ^ b[2];
            c[3] = a[3] ^ b[3];
            c[4] = a[4] ^ b[4];
            c[5] = a[5] ^ b[5];
            c[6] = a[6] ^ b[6];
            c[7] = a[7] ^ b[7];
        }

        private void LPSX(ulong[] stateA, ulong[] stateB, ulong[] result)
        {
            ulong
            t1 = stateA[0] ^ stateB[0],
            t2 = stateA[1] ^ stateB[1],
            t3 = stateA[2] ^ stateB[2],
            t4 = stateA[3] ^ stateB[3],
            t5 = stateA[4] ^ stateB[4],
            t6 = stateA[5] ^ stateB[5],
            t7 = stateA[6] ^ stateB[6],
            t8 = stateA[7] ^ stateB[7];
            int i1;
            for (int i = 0; i < 8; i++)//перебор восьмерок байт
            {
                i1 = i * 8;
                //Преобразование P осуществляется смещением индекса
                result[i] = LSPrecalc[(byte)(t1 >> i1), 0] ^
                LSPrecalc[(byte)(t2 >> i1), 1] ^
                LSPrecalc[(byte)(t3 >> i1), 2] ^
                LSPrecalc[(byte)(t4 >> i1), 3] ^
                LSPrecalc[(byte)(t5 >> i1), 4] ^
                LSPrecalc[(byte)(t6 >> i1), 5] ^
                LSPrecalc[(byte)(t7 >> i1), 6] ^
                LSPrecalc[(byte)(t8 >> i1), 7];
            }
        }

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
        //Функция сжатия
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
        //Хешфункция от массива байт
        public byte[] GetHash(byte[] message)
        {
            //Этап 1
            byte[] paddedMes = new byte[64];
            byte[] h = new byte[64];
            byte[] N_0 = new byte[64];
            byte[] N = new byte[64];
            byte[] Sigma = new byte[64];
            if (OutLen == 256)
            {
                for (int i = 0; i < 64; i++)
                {
                    h[i] = 0x01;
                }
            }
            byte[] N_512 = new byte[64];
            Array.Copy(BitConverter.GetBytes(512), 0, N_512, 0, 4);
            int inc = 0;
            byte[] tempMes = new byte[64];
            int progress = 0;
            //Этап 2
            int len = message.Length * 8;
            while (len >= 512)
            {
                Array.Copy(message, inc * 64, tempMes, 0, 64);
                h = G_n(N, h, tempMes);
                AddModulo512(N, N_512, N);
                AddModulo512(Sigma, tempMes, Sigma);
                len —= 512;
                inc++;
                progress++;
                if (progress % 2048 == 0 && Hash1MBitStep != null)
                    Hash1MBitStep(progress);
            }
            //Этап 3
            byte[] message1 = new byte[message.Length — inc * 64];
            Array.Copy(message, inc * 64, message1, 0, message.Length — inc * 64);
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
            //Конвертация длины усеченного  сообщения в байты, и запись в массив
            MesLen[0] = (byte)(message1.Length * 8);
            MesLen[1] = (byte)((message1.Length * 8) >> 8);

            AddModulo512(N, MesLen.ToArray(), N);
            AddModulo512(Sigma, paddedMes, Sigma);
            h = G_n(N_0, h, N);
            h = G_n(N_0, h, Sigma);

            if (OutLen == 512)
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

        //Поточная реализация хешфункции
        public byte[] GetHash(Stream st)
        {
            //Этап 1
            byte[] paddedMes = new byte[64];
            byte[] h = new byte[64];
            byte[] N_0 = new byte[64];
            byte[] N = new byte[64];
            byte[] Sigma = new byte[64];
            if (OutLen == 256)
            {
                for (int i = 0; i < 64; i++)
                {
                    h[i] = 0x01;
                }
            }
            byte[] N_512 = new byte[64];
            Array.Copy(BitConverter.GetBytes(512), 0, N_512, 0, 4);
            byte[] tempMes = new byte[64];
            int blockLen = st.Read(tempMes, 0, 64);
            int progress = 0;
            //Этап 2
            while (blockLen == 64)
            {
                h = G_n(N, h, tempMes);
                AddModulo512(N, N_512, N);
                AddModulo512(Sigma, tempMes, Sigma);
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
            //Конвертация длины усеченного  сообщения в байты, и запись в массив
            MesLen[0] = (byte)(message1.Length * 8);
            MesLen[1] = (byte)((message1.Length * 8) >> 8);

            AddModulo512(N, MesLen.ToArray(), N);
            AddModulo512(Sigma, paddedMes, Sigma);
            h = G_n(N_0, h, N);
            h = G_n(N_0, h, Sigma);

            if (OutLen == 512)
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
    }
}