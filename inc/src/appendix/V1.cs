using System;
using System.Linq;
using System.Numerics;


namespace GR3410_2012
{
    //�������� ����� �������� �������
    public class GR3410_2012_Main
    {
        //��������� ������������� ������
        private BigInteger p, a, b, m, q;
        //����� P
        private BigInteger xp, yp;
        //����� ����
        private int l;
        //��������� ��������� �����
        private Random rand;
        //����������� � �����������
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
        //������ ������� ������������ � �����������
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
        //����������� �������� �������
        private BigInteger[] EGCD(BigInteger a, BigInteger b)
        {
            if (b == 0)
                return new BigInteger[] { a, 1, 0 };
            BigInteger[] t = EGCD(b, a % b);
            return new BigInteger[] { t[0], t[2], t[1] � t[2] * (a / b) };
        }
        //���������� ��������� � a �������� �� ������ p. ���������� 0 � ������ �������������
        private BigInteger Inverse(BigInteger a, BigInteger p)
        {
            BigInteger[] t = EGCD(a, p);
            if (t[0] != 1)
                return 0;
            return ModP(t[1], p);
        }
        //���������� a �� ������ p
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
                throw new Exception("������� �� ����������� �������");
            return ModP(a * c, p);
        }
        //��������� �������� ����� (xa, ya) � (xb, yb) ������������� ������ � ���������� ��������� � (xc, yc)
        //�� ���������� ��������� ����� ����������� ����� (�1, �1)
        private void EllipticSum(BigInteger xa, BigInteger ya, BigInteger xb, BigInteger yb, ref BigInteger xc, ref BigInteger yc)
        {
            if (xa == �1 && xb == �1)
            {
                xc = �1;
                yc = �1;
            }
            else if (xa == �1)
            {
                xc = xb;
                yc = yb;
            }
            else if (xb == �1)
            {
                xc = xa;
                yc = ya;
            }
            else if (xa != xb)
            {
                BigInteger lambda = DivModP(ModP(yb � ya, p), ModP(xb � xa, p), p);
                BigInteger xct = ModP(lambda * lambda � xa � xb, p);
                yc = ModP(lambda * (xa � xct) � ya, p);
                xc = xct;
            }
            else if (xa == xb && ModP(ya � yb, p) == 0)
            {
                BigInteger lambda = DivModP(ModP(3 * xa * xa + a, p), ModP(2 * ya, p), p);
                BigInteger xct = ModP(lambda * lambda � xa � xa, p);
                yc = ModP(lambda * (xa � xct) � ya, p);
                xc = xct;
            }
            else
            {
                xc = �1;
                yc = �1;
            }
        }
        //��������� �������� �������� ���������� ����������� ����� (x, y) �� k � ���������� ��������� � (x1, y1)
        private void ScalarProd(BigInteger k, BigInteger x, BigInteger y, ref BigInteger x1, ref BigInteger y1)
        {
            byte[] kBytes = k.ToByteArray();
            BigInteger p2x = x, p2y = y;
            x1 = �1; y1 = �1;
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
        //���������� ��������� ����� � ��������� �� 0 �� p � 1
        private BigInteger RandomBelow(BigInteger p)
        {
            byte[] bytes = p.ToByteArray();
            rand.NextBytes(bytes);
            bytes[bytes.Length � 1] &= (byte)0x7F;
            return new BigInteger(bytes) % p;
        }
        //����������� ������� ���� � ��������������� ��� ����� �����
        private BigInteger GetPositive(byte[] bytes)
        {
            byte[] bytes1;
            if ((bytes[bytes.Length � 1] & 0x80) > 0)
            {
                bytes1 = new byte[bytes.Length + 1];
                Array.Copy(bytes, bytes1, bytes.Length);
                bytes1[bytes1.Length � 1] = 0x00;
            }
            else
                bytes1 = bytes;
            return new BigInteger(bytes1);
        }
        //������������ ������� � �������� ����� � ������������ ����� a � b
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
        //��������� �������� r � s �� �������� �������
        private void GetRSFromVector(byte[] zeta, out BigInteger r, out BigInteger s)
        {
            byte[] bytesR = new byte[l / 8];
            Array.Copy(zeta, 0, bytesR, 0, l / 8);
            r = GetPositive(bytesR);

            byte[] bytesS = new byte[l / 8];
            Array.Copy(zeta, l / 8, bytesS, 0, l / 8);
            s = GetPositive(bytesS);
        }
        //���������� �������� � �������� �����
        public void GenerateKeys(out GR3410_2012_SignKey signKey, out GR3410_2012_VerifyKey verifyKey)
        {
            BigInteger xc = �1, yc = �1, k, r;
            do
            {
                k = RandomBelow(q � 1) + 1;
                ScalarProd(k, xp, yp, ref xc, ref yc);
                r = ModP(xc, q);
            }
            while (r == 0);
            signKey = new GR3410_2012_SignKey(k.ToByteArray());
            verifyKey = new GR3410_2012_VerifyKey(xc.ToByteArray(), yc.ToByteArray());
        }
        //������ ������� ��������� �������
        public GR3410_2012_Sign GenerateSign(byte[] hash, GR3410_2012_SignKey signKey)
        {
            return new GR3410_2012_Sign(GenerateSign(hash, signKey.D));
        }
        //������ ������� ��������� �������
        public byte[] GenerateSign(byte[] hash, byte[] signKey)
        {
            BigInteger h = GetPositive(hash);
            BigInteger d = GetPositive(signKey);
            BigInteger e = ModP(h, q);
            if (e == 0)
                e = 1;
            BigInteger xc = �1, yc = �1, k, r, s;
            do
            {
                do
                {
                    k = RandomBelow(q � 1) + 1;
                    ScalarProd(k, xp, yp, ref xc, ref yc);
                    r = ModP(xc, q);
                }
                while (r == 0);
                s = ModP(r * d + k * e, q);
            }
            while (s == 0);
            return BinVectorConc(r, s);
        }
        //������ ������� �������� �������
        public bool verifySign(byte[] hash, GR3410_2012_Sign sign, GR3410_2012_VerifyKey verifyKey)
        {
            return verifySign(hash, sign.Sign, verifyKey.Xq, verifyKey.Yq);
        }
        //������ ������� �������� �������
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
            BigInteger z2 = ModP(�r * v, q);

            BigInteger xc = �1, yc = �1, xt = �1, yt = �1;
            ScalarProd(z1, xp, yp, ref xc, ref yc);
            ScalarProd(z2, xq, yq, ref xt, ref yt);
            EllipticSum(xc, yc, xt, yt, ref xc, ref yc);
            BigInteger R = ModP(xc, q);

            return R == r;
        }
    }
}
