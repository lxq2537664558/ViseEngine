namespace CSUtility.Support
{
    public class UniHash
    {
        public static uint DefaultHash(string str)
        {
            return JSHash(str);
        }

        public static uint RSHash(string str)
        {
            uint b = 378551;
            uint a = 63689;
            uint hash = 0;

            for (int i = 0; i < str.Length; i++)
            {
                hash = hash * a + str[i];
                a = a * b;
            }

            return hash;
        }

        public static uint JSHash(string str)
        {
            uint hash = 1315423911;

            for (int i = 0; i < str.Length; i++)
            {
                hash ^= ((hash << 5) + str[i] + (hash >> 2));
            }

            return hash;
        }

        public static uint ELFHash(string str)
        {
            uint hash = 0;
            uint x = 0;

            for (int i = 0; i < str.Length; i++)
            {
                hash = (hash << 4) + str[i];

                if ((x = hash & 0xF0000000) != 0)
                {
                    hash ^= (x >> 24);
                }
                hash &= ~x;
            }
            return hash;
        }

        public static uint BKDRHash(string str)
        {
            uint seed = 131; // 31 131 1313 13131 131313 etc..   
            uint hash = 0;

            for (int i = 0; i < str.Length; i++)
            {
                hash = (hash * seed) + str[i];
            }

            return hash;
        }
        /* End Of BKDR Hash Function */


        public static uint SDBMHash(string str)
        {
            uint hash = 0;

            for (int i = 0; i < str.Length; i++)
            {
                hash = str[i] + (hash << 6) + (hash << 16) - hash;
            }

            return hash;
        }
        /* End Of SDBM Hash Function */


        public static uint DJBHash(string str)
        {
            uint hash = 5381;

            for (int i = 0; i < str.Length; i++)
            {
                hash = ((hash << 5) + hash) + str[i];
            }

            return hash;
        }
        /* End Of DJB Hash Function */


        public static uint DEKHash(string str)
        {
            int hash = str.Length;

            for (int i = 0; i < str.Length; i++)
            {
                hash = ((hash << 5) ^ (hash >> 27)) ^ str[i];
            }

            return (uint)hash;
        }
        /* End Of DEK Hash Function */


        public static uint BPHash(string str)
        {
            uint hash = 0;

            for (int i = 0; i < str.Length; i++)
            {
                hash = hash << 7 ^ str[i];
            }

            return hash;
        }
        /* End Of BP Hash Function */


        public static uint FNVHash(string str)
        {
            uint fnv_prime = 0x811C9DC5;
            uint hash = 0;

            for (int i = 0; i < str.Length; i++)
            {
                hash *= fnv_prime;
                hash ^= str[i];
            }

            return hash;
        }
        /* End Of FNV Hash Function */


        public static uint APHash(string str)
        {
            uint hash = 0xAAAAAAAA;

            for (int i = 0; i < str.Length; i++)
            {
                if ((i & 1) == 0)
                {
                    hash ^= ((hash << 7) ^ str[i] * (hash >> 3));
                }
                else
                {
                    hash ^= (~((hash << 11) + str[i] ^ (hash >> 5)));
                }
            }

            return hash;
        }

    }

}
