using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ENateRandom {
    static ulong mask = ((ulong) 1L << 63) - 1;
    static long addend = 0xBL;
    static long multiplier = 0x5DEECE66DL;
    private long m_nRandom;

    public long RandomSeed {
        get { return m_nRandom; }
    }
    public ENateRandom () {
        createSeed ();
    }

    public void createSeed () {
        m_nRandom = (long)((ulong)((long) DateTime.Now.ToFileTime () ^ multiplier) & mask);
    }

    public long random (long lMix, long lMax) {
        if (lMix == lMax) {
            return 0;
        }
        long nextseed = (long)((ulong)(m_nRandom * multiplier + addend) & mask);
        m_nRandom = nextseed;
        return Math.Abs (m_nRandom) % (lMax - lMix) + lMix;
    }

}