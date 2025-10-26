using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoSpin.BusinessObjects.Common
{
    /// <summary>
    /// OTP 
    /// </summary>
    public class busOtp
    {

        public const int
            MIN_PINLENGTH = 4,
            SECRET_LENGTH = 20;
        private const string
            MSG_WRONGPIN = "Wrong PIN",
            MSG_SECRETLENGTH = "Secret must be at least 20 bytes",
            MSG_VERIFYPIN = "PIN must be verified",
            MSG_PIN4DIGITS = "PIN must be at least four digits",
            MSG_COUNTER_MINVALUE = "Counter min value is 1";

        public busOtp()
        {
        }

        public busOtp(ulong counter = 1, byte[] secretKey = null)
        {
            if (secretKey != null)
            {
                if (secretKey.Length < SECRET_LENGTH)
                {
                    throw new Exception(MSG_SECRETLENGTH);
                }

                this.secretKey = secretKey;
            }

            if (counter < 1)
            {
                throw new Exception(MSG_COUNTER_MINVALUE);
            }

            this.counter = counter;
        }

        private static int[] dd = new int[10] { 0, 2, 4, 6, 8, 1, 3, 5, 7, 9 };

        private byte[] secretKey = new byte[SECRET_LENGTH] 
        {
			0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39,
			0x3A, 0x3B, 0x3C, 0x3D, 0x3E, 0x3F, 0x40, 0x41, 0x42, 0x43
        };

        private ulong counter = 0x0000000000000001;

        private static int checksum(int Code_Digits)
        {
            //int d1 = (Code_Digits / 1000000) % 10;
            //int d2 = (Code_Digits / 100000) % 10;
            int d3 = (Code_Digits / 10000) % 10;
            int d4 = (Code_Digits / 1000) % 10;
            int d5 = (Code_Digits / 100) % 10;
            int d6 = (Code_Digits / 10) % 10;
            int d7 = Code_Digits % 10;
            return (10 - ((dd[d3] + d4 + dd[d5] + d6 + dd[d7]) % 10)) % 10; //dd[d1] + d2 +
        }

        /// <summary>
        /// Formats the OTP. This is the OTP algorithm.
        /// </summary>
        /// <param name="hmac">HMAC value</param>
        /// <returns>8 digits OTP</returns>
        private static string FormatOTP(byte[] hmac)
        {
            int offset = hmac[19] & 0xf;
            int bin_code = (hmac[offset] & 0x7f) << 24
                | (hmac[offset + 1] & 0xff) << 16
                | (hmac[offset + 2] & 0xff) << 8
                | (hmac[offset + 3] & 0xff);

            int Code_Digits = bin_code % 100000;
            int csum = checksum(Code_Digits);
            int OTP = Code_Digits * 10 + csum;

            return string.Format("{0:d06}", OTP);
        }

        public static byte[] ToByteArray(string otp)
        {
            byte[] baOTP = new byte[otp.Length];
            char[] arOTP = otp.ToCharArray();

            for (int nI = 0; nI < otp.Length; nI++)
            {
                baOTP[nI] = (byte)arOTP[nI];
            }

            return baOTP;
        }

        public byte[] CounterArray
        {
            get
            {
                return BitConverter.GetBytes(counter);
            }

            set
            {
                counter = BitConverter.ToUInt64(value, 0);
            }
        }

        /// <summary>
        /// Set the OTP secret
        /// </summary>
        /// <param name="secret"></param>
        public byte[] Secret
        {
            set
            {
                if (value.Length < SECRET_LENGTH)
                {
                    throw new Exception(MSG_SECRETLENGTH);
                }

                secretKey = value;
            }
        }

        /// <summary>
        /// Get the current OTP value
        /// </summary>
        /// <returns></returns>
        public string GetCurrentOTP()
        {
            busHmacSha1Algorithm ibusHmacSha1Algorithm = new busHmacSha1Algorithm();

            ibusHmacSha1Algorithm.Init(secretKey);
            ibusHmacSha1Algorithm.Update(CounterArray);

            byte[] hmac_result = ibusHmacSha1Algorithm.Final();

            return FormatOTP(hmac_result);
        }

        /// <summary>
        /// Get the next OTP value
        /// </summary>
        /// <returns></returns>
        public string GetNextOTP()
        {
            // increment the counter
            ++counter;

            return GetCurrentOTP();
        }

        /// <summary>
        /// Get the counter value
        /// </summary>
        /// <returns></returns>
        public ulong Counter
        {
            get
            {
                return counter;
            }

            set
            {
                counter = value;
            }
        }
    }
    /// <summary>
    /// SHA1 Hash Algoritham
    /// </summary>
   public class busSha1HashAlgorithm
    {

        struct SHA_TRANSF
        {
            public long A;
            public long B;
            public long C;
            public long D;
            public long E;

            public long T;
            public long[] W;
            public int idxW;
        };

        #region SHA f()-functions

        static long f1(long x, long y, long z)
        {
            return ((x & y) | (~x & z));
        }

        static long f2(long x, long y, long z)
        {
            return (x ^ y ^ z);
        }

        static long f3(long x, long y, long z)
        {
            return ((x & y) | (x & z) | (y & z));
        }

        static long f4(long x, long y, long z)
        {
            return (x ^ y ^ z);
        }

        static long f(long n, long x, long y, long z)
        {
            switch (n)
            {
                case 1:
                    {
                        return f1(x, y, z);
                    }

                case 2:
                    {
                        return f2(x, y, z);
                    }

                case 3:
                    {
                        return f3(x, y, z);
                    }

                case 4:
                    {
                        return f4(x, y, z);
                    }

                default:
                    throw new Exception("Wrong parameter");
            }
        }

        #endregion

        #region SHA constants

        static UInt32[] CONST = new UInt32[4] 
		{
			0x5a827999, 
			0x6ed9eba1,
			0x8f1bbcdc,
			0xca62c1d6
		};

        #endregion

        static long T32(long x)
        {
            unchecked
            {
                return (x & 0xFFFFFFFF);
            }
        }

        static long R32(long x, int n)
        {
            return T32(((x << n) | (x >> (32 - n))));
        }

        #region Unraveled Rotation functions

        static void FA(Int32 n, ref SHA_TRANSF t)
        {
            t.T = T32(R32(t.A, 5) + f(n, t.B, t.C, t.D) + t.E + t.W[t.idxW++] + CONST[n - 1]);
            t.B = R32(t.B, 30);
        }

        static void FB(Int32 n, ref SHA_TRANSF t)
        {
            t.E = T32(R32(t.T, 5) + f(n, t.A, t.B, t.C) + t.D + t.W[t.idxW++] + CONST[n - 1]);
            t.A = R32(t.A, 30);
        }

        static void FC(Int32 n, ref SHA_TRANSF t)
        {
            t.D = T32(R32(t.E, 5) + f(n, t.T, t.A, t.B) + t.C + t.W[t.idxW++] + CONST[n - 1]);
            t.T = R32(t.T, 30);
        }

        static void FD(Int32 n, ref SHA_TRANSF t)
        {
            t.C = T32(R32(t.D, 5) + f(n, t.E, t.T, t.A) + t.B + t.W[t.idxW++] + CONST[n - 1]);
            t.E = R32(t.E, 30);
        }

        static void FE(Int32 n, ref SHA_TRANSF t)
        {
            t.B = T32(R32(t.C, 5) + f(n, t.D, t.E, t.T) + t.A + t.W[t.idxW++] + CONST[n - 1]);
            t.D = R32(t.D, 30);
        }

        static void FT(Int32 n, ref SHA_TRANSF t)
        {
            t.A = T32(R32(t.B, 5) + f(n, t.C, t.D, t.E) + t.T + t.W[t.idxW++] + CONST[n - 1]);
            t.C = R32(t.C, 30);
        }

        #endregion

        private void sha_transform()
        {
            int
                i,
                idx = 0;

            SHA_TRANSF tf = new SHA_TRANSF();
            tf.W = new long[80];

            /* SHA_BYTE_ORDER == 12345678 */
            //			for (i = 0; i < 16; i += 2) 
            //			{
            //				tf.T = data[idx];
            //				idx += 8;
            //				tf.W[i] =  ((tf.T << 24) & 0xff000000) | ((tf.T <<  8) & 0x00ff0000) |
            //					((tf.T >>  8) & 0x0000ff00) | ((tf.T >> 24) & 0x000000ff);
            //				tf.T >>= 32;
            //				tf.W[i+1] = ((tf.T << 24) & 0xff000000) | ((tf.T <<  8) & 0x00ff0000) |
            //					((tf.T >>  8) & 0x0000ff00) | ((tf.T >> 24) & 0x000000ff);
            //			}

            /* SHA_BYTE_ORDER == 1234 */
            for (i = 0; i < 16; ++i)
            {
                tf.T = ((long)data[idx++]) & 0x000000ff;
                tf.T += (((long)data[idx++]) << 8) & 0x0000ff00;
                tf.T += (((long)data[idx++]) << 16) & 0x00ff0000;
                tf.T += (((long)data[idx++]) << 24) & 0xff000000;

                tf.W[i] = ((tf.T << 24) & 0xff000000) | ((tf.T << 8) & 0x00ff0000) |
                          ((tf.T >> 8) & 0x0000ff00) | ((tf.T >> 24) & 0x000000ff);
            }

            for (i = 16; i < 80; ++i)
            {
                tf.W[i] = tf.W[i - 3] ^ tf.W[i - 8] ^ tf.W[i - 14] ^ tf.W[i - 16];
                tf.W[i] = R32(tf.W[i], 1);
            }

            tf.A = digest[0];
            tf.B = digest[1];
            tf.C = digest[2];
            tf.D = digest[3];
            tf.E = digest[4];
            tf.idxW = 0;

            // UNRAVEL
            FA(1, ref tf); FB(1, ref tf); FC(1, ref tf); FD(1, ref tf); FE(1, ref tf); FT(1, ref tf); FA(1, ref tf); FB(1, ref tf); FC(1, ref tf); FD(1, ref tf);
            FE(1, ref tf); FT(1, ref tf); FA(1, ref tf); FB(1, ref tf); FC(1, ref tf); FD(1, ref tf); FE(1, ref tf); FT(1, ref tf); FA(1, ref tf); FB(1, ref tf);
            FC(2, ref tf); FD(2, ref tf); FE(2, ref tf); FT(2, ref tf); FA(2, ref tf); FB(2, ref tf); FC(2, ref tf); FD(2, ref tf); FE(2, ref tf); FT(2, ref tf);
            FA(2, ref tf); FB(2, ref tf); FC(2, ref tf); FD(2, ref tf); FE(2, ref tf); FT(2, ref tf); FA(2, ref tf); FB(2, ref tf); FC(2, ref tf); FD(2, ref tf);
            FE(3, ref tf); FT(3, ref tf); FA(3, ref tf); FB(3, ref tf); FC(3, ref tf); FD(3, ref tf); FE(3, ref tf); FT(3, ref tf); FA(3, ref tf); FB(3, ref tf);
            FC(3, ref tf); FD(3, ref tf); FE(3, ref tf); FT(3, ref tf); FA(3, ref tf); FB(3, ref tf); FC(3, ref tf); FD(3, ref tf); FE(3, ref tf); FT(3, ref tf);
            FA(4, ref tf); FB(4, ref tf); FC(4, ref tf); FD(4, ref tf); FE(4, ref tf); FT(4, ref tf); FA(4, ref tf); FB(4, ref tf); FC(4, ref tf); FD(4, ref tf);
            FE(4, ref tf); FT(4, ref tf); FA(4, ref tf); FB(4, ref tf); FC(4, ref tf); FD(4, ref tf); FE(4, ref tf); FT(4, ref tf); FA(4, ref tf); FB(4, ref tf);
            digest[0] = T32(digest[0] + tf.E);
            digest[1] = T32(digest[1] + tf.T);
            digest[2] = T32(digest[2] + tf.A);
            digest[3] = T32(digest[3] + tf.B);
            digest[4] = T32(digest[4] + tf.C);
        }

        public const ushort LITTLE_INDIAN = 1234;
        public const ushort BYTE_ORDER = LITTLE_INDIAN;
        public const int SHA_BLOCKSIZE = 64;
        public const int SHA_DIGESTSIZE = 20;

        #region Replaces the SHA_INFO structure

        private long[] digest;				/* message digest */
        private long count_lo, count_hi;	/* 64-bit bit count */
        private byte[] data;				/* SHA data buffer */
        private int local;					/* unprocessed amount in data */

        #endregion

        public busSha1HashAlgorithm()
        {
        }

        /// <summary>
        /// Initialize the SHA digest
        /// </summary>
        public void Init()
        {
            data = new byte[SHA_BLOCKSIZE];
            digest = new long[5];

            digest[0] = 0x67452301L;
            digest[1] = 0xefcdab89L;
            digest[2] = 0x98badcfeL;
            digest[3] = 0x10325476L;
            digest[4] = 0xc3d2e1f0L;
            count_lo = 0L;
            count_hi = 0L;
            local = 0;
        }

        /// <summary>
        /// Update the SHA digest
        /// </summary>
        /// <param name="buffer">Data to be processed</param>
        public void Update(byte[] buffer)
        {
            int i;
            long clo;
            int count = buffer.Length;
            int buf_idx = 0;

            clo = T32(count_lo + ((long)count << 3));
            if (clo < count_lo)
            {
                ++count_hi;
            }
            count_lo = clo;
            count_hi += (long)count >> 29;
            if (local != 0)
            {
                i = SHA_BLOCKSIZE - local;
                if (i > count)
                {
                    i = count;
                }

                busMemberUtil._cpy(ref data, local, buffer, buf_idx, i);
                count -= i;
                buf_idx += i;

                local += i;
                if (local == SHA_BLOCKSIZE)
                {
                    sha_transform();
                }
                else
                {
                    return;
                }
            }
            while (count >= SHA_BLOCKSIZE)
            {
                busMemberUtil._cpy(ref data, 0, buffer, buf_idx, SHA_BLOCKSIZE);
                buf_idx += SHA_BLOCKSIZE;
                count -= SHA_BLOCKSIZE;
                sha_transform();
            }

            busMemberUtil._cpy(ref data, 0, buffer, buf_idx, count);
            local = count;
        }

        /// <summary>
        /// Finish computing the SHA digest
        /// </summary>
        /// <param name="result"></param>
        public byte[] Final()
        {
            byte[] result = new byte[SHA_DIGESTSIZE];

            int count;
            long lo_bit_count, hi_bit_count;

            lo_bit_count = count_lo;
            hi_bit_count = count_hi;
            count = (int)((lo_bit_count >> 3) & 0x3f);
            data[count++] = 0x80;
            if (count > SHA_BLOCKSIZE - 8)
            {
                busMemberUtil._set(ref data, count, 0, SHA_BLOCKSIZE - count);
                sha_transform();
                busMemberUtil._set(ref data, 0, 0, SHA_BLOCKSIZE - 8);
            }
            else
            {
                busMemberUtil._set(ref data, count, 0, SHA_BLOCKSIZE - 8 - count);
            }

            data[56] = (byte)((hi_bit_count >> 24) & 0xff);
            data[57] = (byte)((hi_bit_count >> 16) & 0xff);
            data[58] = (byte)((hi_bit_count >> 8) & 0xff);
            data[59] = (byte)((hi_bit_count >> 0) & 0xff);
            data[60] = (byte)((lo_bit_count >> 24) & 0xff);
            data[61] = (byte)((lo_bit_count >> 16) & 0xff);
            data[62] = (byte)((lo_bit_count >> 8) & 0xff);
            data[63] = (byte)((lo_bit_count >> 0) & 0xff);
            sha_transform();
            result[0] = (byte)((digest[0] >> 24) & 0xff);
            result[1] = (byte)((digest[0] >> 16) & 0xff);
            result[2] = (byte)((digest[0] >> 8) & 0xff);
            result[3] = (byte)((digest[0]) & 0xff);
            result[4] = (byte)((digest[1] >> 24) & 0xff);
            result[5] = (byte)((digest[1] >> 16) & 0xff);
            result[6] = (byte)((digest[1] >> 8) & 0xff);
            result[7] = (byte)((digest[1]) & 0xff);
            result[8] = (byte)((digest[2] >> 24) & 0xff);
            result[9] = (byte)((digest[2] >> 16) & 0xff);
            result[10] = (byte)((digest[2] >> 8) & 0xff);
            result[11] = (byte)((digest[2]) & 0xff);
            result[12] = (byte)((digest[3] >> 24) & 0xff);
            result[13] = (byte)((digest[3] >> 16) & 0xff);
            result[14] = (byte)((digest[3] >> 8) & 0xff);
            result[15] = (byte)((digest[3]) & 0xff);
            result[16] = (byte)((digest[4] >> 24) & 0xff);
            result[17] = (byte)((digest[4] >> 16) & 0xff);
            result[18] = (byte)((digest[4] >> 8) & 0xff);
            result[19] = (byte)((digest[4]) & 0xff);

            return result;
        }

        public byte[] Final_dss_padding()
        {
            byte[] result = new byte[SHA_DIGESTSIZE];

            int count;
            long lo_bit_count, hi_bit_count;

            lo_bit_count = count_lo;
            hi_bit_count = count_hi;
            count = (int)((lo_bit_count >> 3) & 0x3f);
            if (count > SHA_BLOCKSIZE)
            {
                busMemberUtil._set(ref data, count, 0, SHA_BLOCKSIZE - count);
                sha_transform();
                busMemberUtil._set(ref data, 0, 0, SHA_BLOCKSIZE);
            }
            else
            {
                busMemberUtil._set(ref data, count, 0, SHA_BLOCKSIZE - count);
            }

            sha_transform();
            result[0] = (byte)((digest[0] >> 24) & 0xff);
            result[1] = (byte)((digest[0] >> 16) & 0xff);
            result[2] = (byte)((digest[0] >> 8) & 0xff);
            result[3] = (byte)((digest[0]) & 0xff);
            result[4] = (byte)((digest[1] >> 24) & 0xff);
            result[5] = (byte)((digest[1] >> 16) & 0xff);
            result[6] = (byte)((digest[1] >> 8) & 0xff);
            result[7] = (byte)((digest[1]) & 0xff);
            result[8] = (byte)((digest[2] >> 24) & 0xff);
            result[9] = (byte)((digest[2] >> 16) & 0xff);
            result[10] = (byte)((digest[2] >> 8) & 0xff);
            result[11] = (byte)((digest[2]) & 0xff);
            result[12] = (byte)((digest[3] >> 24) & 0xff);
            result[13] = (byte)((digest[3] >> 16) & 0xff);
            result[14] = (byte)((digest[3] >> 8) & 0xff);
            result[15] = (byte)((digest[3]) & 0xff);
            result[16] = (byte)((digest[4] >> 24) & 0xff);
            result[17] = (byte)((digest[4] >> 16) & 0xff);
            result[18] = (byte)((digest[4] >> 8) & 0xff);
            result[19] = (byte)((digest[4]) & 0xff);

            return result;
        }

        /// <summary>
        /// Returns the version
        /// </summary>
        /// <returns></returns>
        public static string version()
        {
            return "SHA-1";
        }
    }

    /// <summary>
    /// Otp Counter
    /// </summary>
    public class busOTPCounter
    {
        public const int NB_BYTES_COUNTER = 6;

        private ulong counter;

        #region Constructors

        public busOTPCounter()
        {
            counter = 0;
        }

        public busOTPCounter(ulong val)
        {
            counter = val;
        }

        public busOTPCounter(byte[] val)
        {
            Array = val;
        }

        #endregion

        public byte[] Array
        {
            get
            {
                byte[] baCounter = new byte[NB_BYTES_COUNTER];

                for (int nI = 0; nI < NB_BYTES_COUNTER; nI++)
                {
                    baCounter[nI] = (byte)((counter >> (56 - nI * NB_BYTES_COUNTER)) & 0x00000000000000ff);
                }

                return baCounter;
            }

            set
            {
                byte[] baCounter = value;
                counter = 0;

                for (int nI = 0; nI < NB_BYTES_COUNTER; nI++)
                {
                    counter += ((ulong)baCounter[nI]) << (56 - nI * NB_BYTES_COUNTER);
                }
            }
        }

        public ulong Ulong
        {
            get
            {
                return counter;
            }

            set
            {
                counter = value;
            }
        }
    }

    /// <summary>
    /// HMAC SHA1 Algo
    /// </summary>
   public class busHmacSha1Algorithm
    {
        private const int HMAC_SHA1_PAD_SIZE = 64;
        private const int HMAC_SHA1_DIGEST_SIZE = 20;
        private const int HMAC_SHA1_128_DIGEST_SIZE = 16;

        private busSha1HashAlgorithm sha_ctx;
        private byte[] key_ctx;
        private int key_len_ctx;
        private byte[] temp_key_ctx = new byte[busSha1HashAlgorithm.SHA_DIGESTSIZE];  /* in case key exceeds 64 bytes  */

        public busHmacSha1Algorithm()
        {
        }

        public void Init(byte[] key)
        {
            byte[] k_ipad = new byte[HMAC_SHA1_PAD_SIZE];
            int i, key_len = key.Length;

            sha_ctx = new busSha1HashAlgorithm();

            /* if key is longer than 64 bytes reset it to key=SHA-1(key) */
            if (key_len > HMAC_SHA1_PAD_SIZE)
            {
                sha_ctx.Init();
                sha_ctx.Update(key);
                temp_key_ctx = sha_ctx.Final();

                key = temp_key_ctx;
                key_len = HMAC_SHA1_DIGEST_SIZE;
            }

            /*
            * the HMAC_SHA1 transform looks like:
            *
            * SHA1(K XOR opad, SHA1(K XOR ipad, text))
            *
            * where K is an n byte key
            * ipad is the byte 0x36 repeated 64 times
            * opad is the byte 0x5c repeated 64 times
            * and text is the data being protected
            */

            /* start out by storing key in pads */
            busMemberUtil._set(ref k_ipad, 0, 0, k_ipad.Length);
            busMemberUtil._cpy(ref k_ipad, 0, key, 0, key_len);

            /* XOR key with ipad and opad values */
            for (i = 0; i < k_ipad.Length; i++)
            {
                k_ipad[i] ^= 0x36;
            }

            /*
             * perform inner SHA1
             */
            sha_ctx.Init();               /* init context for 1st pass */
            /* start with inner pad      */
            sha_ctx.Update(k_ipad);

            /* Stash the key and it's length into the context. */
            key_ctx = key;
            key_len_ctx = key_len;
        }

        public void Update(byte[] text)
        {
            sha_ctx.Update(text);
        }

        public byte[] Final()
        {
            byte[] digest;

            /* outer padding -  key XORd with opad */
            byte[] k_opad = new byte[HMAC_SHA1_PAD_SIZE];
            int i;

            busMemberUtil._set(ref k_opad, 0, 0, k_opad.Length);
            busMemberUtil._cpy(ref k_opad, 0, key_ctx, 0, key_len_ctx);

            /* XOR key with ipad and opad values */
            for (i = 0; i < k_opad.Length; i++)
            {
                k_opad[i] ^= 0x5c;
            }

            digest = sha_ctx.Final();         /* finish up 1st pass */

            /*
             * perform outer SHA1
             */
            sha_ctx.Init();                  /* init context for 2nd pass */
            /* start with outer pad      */
            sha_ctx.Update(k_opad);

            /* then results of 1st hash  */
            sha_ctx.Update(digest);
            digest = sha_ctx.Final();         /* finish up 2nd pass        */

            return digest;
        }
    }
    /// <summary>
    /// Member Util
    /// </summary>
   public class busMemberUtil
    {
        public static void _set(ref byte[] data, int first, byte val, int count)
        {
            for (int nI = 0; nI < count; nI++)
            {
                data[nI + first] = val;
            }
        }

        public static void _cpy(ref byte[] dest, int dest_first, byte[] srce, int srce_first, int count)
        {
            for (int nI = 0; nI < count; nI++)
            {
                dest[dest_first + nI] = srce[nI + srce_first];
            }
        }
    }

}
