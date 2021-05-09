using System;
using System.Linq;
using System.Numerics;


namespace GR3410_2012
{
    //ќсновной класс цифровой подписи
    public class GR3410_2012_Main
    {
        //ѕараметры эллиптической кривой
        private BigInteger p, a, b, m, q;
        //“очка P
        private BigInteger xp, yp;
        //ƒлина хэша
        private int l;
        //√енератор случайных чисел
        private Random rand;
        // онструктор с параметрами
        public GR3410_2012_Main(byte[] p, byte[] a, byte[] b, byte[] m, byte[] q, byte[] xp, byte[] yp, int l)
        {
            this.p = GetPositive(p);
            this.a = GetPositive(a);
            this.b = GetPositive(b);
            this.m = GetPositive(m);
            this.q = GetPositive(q);
            this.xp = GetPositive(xp);
            this.yp = GetPositive(yp);
            this.l = l;
            rand = new Random();
        }
        //ƒругой вариант конструктора с параметрами
        public GR3410_2012_Main(GR3410_2012_Parameters parameters)
        {
            this.p = GetPositive(parameters.P);
            this.a = GetPositive(parameters.A);
            this.b = GetPositive(parameters.B);
            this.m = GetPositive(parameters.M);
            this.q = GetPositive(parameters.Q);
            this.xp = GetPositive(parameters.Xp);
            this.yp = GetPositive(parameters.Yp);
            this.l = parameters.L;
            rand = new Random();
        }
        //–асширенный алгоритм ≈вклида
        private BigInteger[] EGCD(BigInteger a, BigInteger b)
        {
            if (b == 0)
                return new BigInteger[] { a, 1, 0 };
            BigInteger[] t = EGCD(b, a % b);
            return new BigInteger[] { t[0], t[2], t[1] Ч t[2] * (a / b) };
        }
        //¬ычисление обратного к a элемента по модулю p. ¬озвращает 0 в случае необратимости
        private BigInteger Inverse(BigInteger a, BigInteger p)
        {
            BigInteger[] t = EGCD(a, p);
            if (t[0] != 1)
                return 0;
            return ModP(t[1], p);
        }
        //¬озвращает a по модулю p
        private BigInteger ModP(BigInteger a, BigInteger p)
        {
            if (a >= 0)
                return a % p;
            else
                return a % p + p;
        }
        //a/b mod p
        private BigInteger DivModP(BigInteger a, BigInteger b, BigInteger p)
        {
            BigInteger c = Inverse(b, p);
            if (c == 0)
                throw new Exception("ƒеление на необратимый элемент");
            return ModP(a * c, p);
        }
        //–еализует сложение точек (xa, ya) и (xb, yb) эллиптической кривой и записывает результат в (xc, yc)
        //«а бесконечно удаленную точку принимаетс€ точка (Ч1, Ч1)
        private void EllipticSum(BigInteger xa, BigInteger ya, BigInteger xb, BigInteger yb, ref BigInteger xc, ref BigInteger yc)
        {
            if (xa == Ч1 && xb == Ч1)
            {
                xc = Ч1;
                yc = Ч1;
            }
            else if (xa == Ч1)
            {
                xc = xb;
                yc = yb;
            }
            else if (xb == Ч1)
            {
                xc = xa;
                yc = ya;
            }
            else if (xa != xb)
            {
                BigInteger lambda = DivModP(ModP(yb Ч ya, p), ModP(xb Ч xa, p), p);
                BigInteger xct = ModP(lambda * lambda Ч xa Ч xb, p);
                yc = ModP(lambda * (xa Ч xct) Ч ya, p);
                xc = xct;
            }
            else if (xa == xb && ModP(ya Ч yb, p) == 0)
            {
                BigInteger lambda = DivModP(ModP(3 * xa * xa + a, p), ModP(2 * ya, p), p);
                BigInteger xct = ModP(lambda * lambda Ч xa Ч xa, p);
                yc = ModP(lambda * (xa Ч xct) Ч ya, p);
                xc = xct;
            }
            else
            {
                xc = Ч1;
                yc = Ч1;
            }
        }
        //–еализует механизм быстрого скал€рного роизведени€ точки (x, y) на k и записывает результат в (x1, y1)
        private void ScalarProd(BigInteger k, BigInteger x, BigInteger y, ref BigInteger x1, ref BigInteger y1)
        {
            byte[] kBytes = k.ToByteArray();
            BigInteger p2x = x, p2y = y;
            x1 = Ч1; y1 = Ч1;
            for (int i = 0; i < kBytes.Count<byte>(); i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if ((kBytes[i] >> j & 0x01) > 0)
                    {
                        EllipticSum(x1, y1, p2x, p2y, ref x1, ref y1);
                    }
                    EllipticSum(p2x, p2y, p2x, p2y, ref p2x, ref p2y);
                }
            }
        }
        //¬озвращает случайное число в диапазоне от 0 до p Ч 1
        private BigInteger RandomBelow(BigInteger p)
        {
            byte[] bytes = p.ToByteArray();
            rand.NextBytes(bytes);
            bytes[bytes.Length Ч 1] &= (byte)0x7F;
            return new BigInteger(bytes) % p;
        }
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
        //ќсуществл€ет перевод в двоичную форму и конкатенацию чисел a и b
        private byte[] BinVectorConc(BigInteger a, BigInteger b)
        {
            int zetaL = l * 2 / 8;
            byte[] zeta = new byte[zetaL];
            byte[] bytesA = a.ToByteArray();
            byte[] bytesB = b.ToByteArray();
            Array.Copy(bytesA, 0, zeta, 0, Math.Min(bytesA.Length, l / 8));
            Array.Copy(bytesB, 0, zeta, zetaL / 2, Math.Min(bytesB.Length, l / 8));
            return zeta;
        }
        //ѕолучение значений r и s из цифровой подписи
        private void GetRSFromVector(byte[] zeta, out BigInteger r, out BigInteger s)
        {
            byte[] bytesR = new byte[l / 8];
            Array.Copy(zeta, 0, bytesR, 0, l / 8);
            r = GetPositive(bytesR);

            byte[] bytesS = new byte[l / 8];
            Array.Copy(zeta, l / 8, bytesS, 0, l / 8);
            s = GetPositive(bytesS);
        }
        //√енерирует открытый и закрытый ключи
        public void GenerateKeys(out GR3410_2012_SignKey signKey, out GR3410_2012_VerifyKey verifyKey)
        {
            BigInteger xc = Ч1, yc = Ч1, k, r;
            do
            {
                k = RandomBelow(q Ч 1) + 1;
                ScalarProd(k, xp, yp, ref xc, ref yc);
                r = ModP(xc, q);
            }
            while (r == 0);
            signKey = new GR3410_2012_SignKey(k.ToByteArray());
            verifyKey = new GR3410_2012_VerifyKey(xc.ToByteArray(), yc.ToByteArray());
        }
        //¬торой вариант генерации подписи
        public GR3410_2012_Sign GenerateSign(byte[] hash, GR3410_2012_SignKey signKey)
        {
            return new GR3410_2012_Sign(GenerateSign(hash, signKey.D));
        }
        //ѕервый вариант генерации подписи
        public byte[] GenerateSign(byte[] hash, byte[] signKey)
        {
            BigInteger h = GetPositive(hash);
            BigInteger d = GetPositive(signKey);
            BigInteger e = ModP(h, q);
            if (e == 0)
                e = 1;
            BigInteger xc = Ч1, yc = Ч1, k, r, s;
            do
            {
                do
                {
                    k = RandomBelow(q Ч 1) + 1;
                    ScalarProd(k, xp, yp, ref xc, ref yc);
                    r = ModP(xc, q);
                }
                while (r == 0);
                s = ModP(r * d + k * e, q);
            }
            while (s == 0);
            return BinVectorConc(r, s);
        }
        //¬торой вариант проверки подписи
        public bool verifySign(byte[] hash, GR3410_2012_Sign sign, GR3410_2012_VerifyKey verifyKey)
        {
            return verifySign(hash, sign.Sign, verifyKey.Xq, verifyKey.Yq);
        }
        //ѕервый вариант проверки подписи
        public bool verifySign(byte[] hash, byte[] sign, byte[] xqb, byte[] yqb)
        {
            BigInteger r, s;
            GetRSFromVector(sign, out r, out s);

            if (r >= q || s >= q)
                return false;

            BigInteger xq = GetPositive(xqb);
            BigInteger yq = GetPositive(yqb);

            BigInteger h = GetPositive(hash);
            BigInteger e = ModP(h, q);
            if (e == 0)
                e = 1;
            BigInteger v = Inverse(e, q);

            BigInteger z1 = ModP(s * v, q);
            BigInteger z2 = ModP(Чr * v, q);

            BigInteger xc = Ч1, yc = Ч1, xt = Ч1, yt = Ч1;
            ScalarProd(z1, xp, yp, ref xc, ref yc);
            ScalarProd(z2, xq, yq, ref xt, ref yt);
            EllipticSum(xc, yc, xt, yt, ref xc, ref yc);
            BigInteger R = ModP(xc, q);

            return R == r;
        }
    }
}
