using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TernaryCore
{
    public class Logic
    {
        private Tryte value;

        public Logic()
        {
            value = new Tryte();
        }

        public Logic(bool obj)
        {
            value = obj ? value = new Tryte(1) : value = new Tryte(-1);
        }

        public Logic(int obj)
        {
            if (obj == 0) value = new Tryte(0);
            else
                value = obj > 0 ? value = new Tryte(1) : value = new Tryte(-1);
        }

        public static Logic operator !(Logic right)
        {
            Logic result = new Logic();
            result.value = -right.value;
            return result;
        }

        public static explicit operator bool(Logic right)
        {
            bool result = right.value == 1;
            return result;
        }

        public static Logic operator &(Logic left, Logic right)
        {
            Logic result = new Logic(Math.Min(left.value, right.value));
            return result;
        }

        public static Logic operator |(Logic left, Logic right)
        {
            Logic result = new Logic(Math.Max(left.value, right.value));
            return result;
        }

        public override string ToString()
        {
            return value[Tryte.Size - 1].ToString();
        }
    }
}

