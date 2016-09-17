using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TernaryCore
{
    public class Float
    {
        private Tryte fbase = new Tryte();
        private Integer mantisa = new Integer();

        public Float() { }

        public Float(float obj)
        {
            bool isInt = false;
            int commaPos = obj.ToString().IndexOf(",");
            if (commaPos == -1)
                isInt = true;
            if (!isInt)
            {
                int order = -(obj.ToString().Length - commaPos - 1);
                fbase = (Tryte) order;
                obj *= (float) Math.Pow(10, Math.Abs(order));
            }
            mantisa = (long) obj;
        }

        public Float(Float obj)
        {
            fbase = new Tryte(obj.fbase);
            mantisa = new Integer(obj.mantisa);
        }

        public static implicit operator float(Float obj)
        {
            float result = obj.mantisa;
            return result * (float) Math.Pow(10, obj.fbase);
        }

        public static implicit operator Float(float obj)
        {
            return new Float(obj);
        }

        #region Алгебраические операции

        public static Float operator +(Float left, Float right)
        {
            Float result = new Float();
            if (Math.Abs(left.fbase) < Math.Abs(right.fbase))
            {
                left.mantisa *= (long) Math.Pow(10, Math.Abs(right.fbase) - Math.Abs(left.fbase));
                result.fbase = right.fbase;
            }
            else if (Math.Abs(right.fbase) < Math.Abs(left.fbase))
            {
                right.mantisa *= (long) Math.Pow(10, Math.Abs(left.fbase) - Math.Abs(right.fbase));
                result.fbase = left.fbase;
            }
            result.mantisa = left.mantisa + right.mantisa;
            return result;
        }

        public static Float operator -(Float right)
        {
            Float result = new Float();
            result.fbase = right.fbase;
            result.mantisa = -right.mantisa;
            return result;
        }

        public static Float operator -(Float left, Float right)
        {
            Float result = new Float();
            return result = left + -right;
        }

        public static Float operator *(Float left, Float right)
        {
            Float result = new Float();
            result.fbase = left.fbase + right.fbase;
            result.mantisa = left.mantisa * right.mantisa;
            return result;
        }

        public static Float operator /(Float left, Float right)
        {
            Float result = new Float();
            Float temp = new Float(1 / right);
            result.fbase = left.fbase + temp.fbase;
            result.mantisa = left.mantisa * temp.mantisa;
            return result;
        }

        #endregion
    }
}
