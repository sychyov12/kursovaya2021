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
    for (int i = 0; i < 8; i++)
    {
        i1 = i * 8;
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