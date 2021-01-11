#nullable enable
using System;
using System.Threading;

namespace AutoAITalk
{
    public class Sugar
    {
        public static TR? TryFunc<T, TR>(Func<T, TR> func, T? t, TR? fail)
        {
            if (t == null) return fail;
            try
            {
                return func(t);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return fail;
            }
        }

        public static bool TryAction<T>(Action<T> func, T? t)
        {
            if (t == null) return false;
            try
            {
                func(t);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public static bool WaitFor(Func<bool> func, int timeout)
        {
            while (func()) Thread.Sleep(timeout);
            return true;
        }
    }
}