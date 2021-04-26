using System;
using static System.Convert;

namespace WebAPI.Database.Generation
{
    public static class GenerationToken
    {
        private static char RandNum(Random random)
        {
            return ToChar(random.Next(48, 58));
        }

        private static char RandSymb(Random random)
        {
            return (random.Next(0, 2) == 0) ? ToChar(random.Next(65, 91))
                                          : ToChar(random.Next(97, 123));
        }

        public static string Get(int length)
        {
            Random random = new Random();
            char[] resoult = new char[length];
            for (int i = 0; i < length; i++)
                resoult[i] = (random.Next(0, 2) == 0) ? RandNum(random) : RandSymb(random);
            return new string(resoult);
        }
    }
}