using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Providers.Random
{
    internal class RandomByteArray : Variable
    {
        private readonly long mLength;

        public RandomByteArray(long length)
        {
            mLength = length;
        }

        public override IEnumerable<byte> ToByteArray(Encoding encoding, TestStatus? status)
        {
            for (var i = 0; i < mLength; i++)
            { 
                yield return (byte)System.Random.Shared.Next(256);
            }
        }

        public override string? ToString(TestStatus? status)
        {
            var bytes = ToByteArray(Encoding.UTF8, status).ToArray();
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
