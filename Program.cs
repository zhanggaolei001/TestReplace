using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace TestReplace
{
    class Program
    {
       
        static Stopwatch sw;//计时器
        static void Main(string[] args)
        {
            int length = 10000;
            RunTest(length);
            length *=10;
            RunTest(length);
            length *= 10;
            RunTest(length);
            length *= 10;
            RunTest(length);
            length *= 10;
            RunTest(length);
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static void RunTest(int length)
        {
            sw = new Stopwatch();
            //数据准备:
            var data = GenerateData(length);
            Console.WriteLine($"测试数据长度:{length}");
            var str1 = data.ToString(); //用于方法一测试
            var str2 = data.ToString(); //用于方法二测试

            //第一种方法测试
            sw.Start(); //开始计时
            for (int i = 0; i < 10; i++)
            {
                str1 = str1.Replace(i.ToString(), "");
            }

            Console.WriteLine($"去除数字的字符串长度为:{str1.Length}");
            sw.Stop(); //停止计时
            Console.WriteLine($"0~10for循环使用Replace方法耗时:{sw.Elapsed.TotalMilliseconds}毫秒");

            //第二种方法测试
            sw.Restart(); //重新开始计时
            var result = new StringBuilder(str2.Length);
            foreach (var chr in str2)
            {
                if (chr < 48 || chr > 57)
                {
                    result.Append(chr);
                }
            }

            str2 = result.ToString(); //对比一致性,也转为string,参与计时
            Console.WriteLine($"去除数字的字符串长度为:{str2.Length}");
            sw.Stop(); //停止计时
            Console.WriteLine($"每个字符for循环使用判定耗时:{sw.Elapsed.TotalMilliseconds}毫秒");
        }

        private static StringBuilder GenerateData(int length)
        {
            StringBuilder data = new StringBuilder(length);
            var random = new ThreadSafeRandom();
            for (int i = 0; i < length; i++)
            {
                data.Append((char)random.Next(48, 122));//0~z
            }
            //Console.WriteLine(data.ToString());
            return data;
        }
    }
    //随机数生成器(转自"_学而时习之_"博客:https://blog.csdn.net/xxdddail/article/details/16980743)
    public class ThreadSafeRandom : Random
    { 
        private static readonly RNGCryptoServiceProvider _global = new RNGCryptoServiceProvider();
         
        private ThreadLocal<Random> _local = new ThreadLocal<Random>(() =>
        {
            //This is the valueFactory function
            //This code will run for each thread to initialize each independent instance of Random.
            var buffer = new byte[4];
            //Calls the GetBytes method for RNGCryptoServiceProvider because this class is thread-safe
            //for this usage.
            _global.GetBytes(buffer);
            //Return the new thread-local Random instance initialized with the generated seed.
            return new Random(BitConverter.ToInt32(buffer, 0));
        });

        public override int Next()
        {
            return _local.Value.Next();
        }

        public override int Next(int maxValue)
        {
            return _local.Value.Next(maxValue);
        } 
        public override int Next(int minValue, int maxValue)
        {
            return _local.Value.Next(minValue, maxValue);
        } 
        public override double NextDouble()
        {
            return _local.Value.NextDouble();
        } 
        public override void NextBytes(byte[] buffer)
        {
            _local.Value.NextBytes(buffer);
        }
    }
}
