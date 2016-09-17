using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;

namespace TernaryCore
{
    public enum Trit { False = -1, Unknown = 0, True = 1 }
    public class Tryte 
    {
        public const short Size = 9;
        private Trit[] tryteBase = new Trit[Size] {0, 0, 0, 0, 0, 0, 0, 0, 0}; 
        public Trit[] TryteBase {get; }
        private Trit carry = Trit.Unknown;
        public Trit[] transfer = new Trit[Size]{0, 0, 0, 0, 0, 0, 0, 0, 0};

        public Trit Carry
        {
            get { return carry; }
            set { carry = value; }
        }

        public Trit this[int index]
        {
            get { return tryteBase[index]; }
            set { tryteBase[index] = value; }
        }

        public Tryte() { }

        public Tryte(int[] a) : this()
        {
            if (a.Length != 9)
                throw new OverflowException("При создании объекта Tryte из массива int произошла ошибка:\nПревышен допустимый размер (размер трайта: " + Size + ")");
            for (int i = 0; i < Size; i++)
            {
                tryteBase[i] = (Trit) a[i];
            }
        } 

        public Tryte(Tryte copy) : this()
        {
            for (int i = 0; i < Size; i++)
            {
                tryteBase[i] = copy.tryteBase[i];
            }
            for (int i = 0; i < Size; i++)
            {
                transfer[i] = copy.transfer[i];
            }
        }

        public Tryte(int obj)
        {
            if (obj == 0) return;

            bool neg = obj < 0;

            if (neg) obj = -obj;

            int[] result = new int[Size] {0,0,0,0,0,0,0,0,0};
            int mod; int index = result.Length - 1;
            while (obj >= 3)
            {
                mod = obj % 3;
                if (mod < 2) { result[index] = mod; obj /= 3; }
                else { result[index] = -1; obj = obj / 3 + 1; }
                --index;
                if (index < 0)
                    throw new OverflowException("При создании объекта Integer из объекта Long произошло переполнение\nЗначение превышает допустимый диапозон значений");
            }
            if (obj != 2)
                result[index] = obj;
            else { result[index] = -1; result[index - 1] = 1; }
            for (int i = 0; i < Size; i++)
            {
                if(neg)
                    result[i] =  -result[i];
                tryteBase[i] = (Trit) result[i];
            }
        }


        #region Операторы преобразования

        public static implicit operator int(Tryte obj)
        {
            int result = 0;
            for (int i = Size - 1; i >= 0; i--)
            {
                result += (int)obj.tryteBase[i] * (int)Math.Pow(3, Size - 1 - i);
            }
            return result;
        }

        public static explicit operator Tryte(int obj)
        {
            return new Tryte(obj);
        }

        #endregion

        #region Операции
        private static Trit TritSum(Trit left, Trit right, ref Trit carry)
        {
            Trit res = Trit.Unknown;
            if (carry == Trit.Unknown)
            {
                if (left == right)
                {
                    if (left != Trit.Unknown)
                    {
                        res = (Trit) (- (int) (left));
                        carry = left;
                    }
                }
                else
                {
                    res = (Trit) ((int)left + (int)right);
                    carry = Trit.Unknown;
                }
            }
            else
            {
                if (left == right && carry == left)
                {
                    res = Trit.Unknown;
                }
                else if (left != Trit.Unknown && left == right && carry != left)
                {
                    res = left;
                    carry = Trit.Unknown;
                }
                else if(left == right && left == Trit.Unknown && carry != left)
                {
                    res = carry;
                    carry = Trit.Unknown;
                }
                else if ((left == Trit.Unknown && carry == right) || (right == Trit.Unknown && carry == left))
                {
                    res = (Trit) (-(int) (carry));
                }
                else { res = (Trit) ((int) left + (int) right) + (int) carry; carry = Trit.Unknown; }
            }
            return res;
        }

        public static Trit TritMul(Trit left, Trit right)
        {
            return (Trit) ((int)left * (int)right);
        }

        public static Tryte operator +(Tryte left, Tryte right)
        {
            Tryte result = new Tryte();
            for (int i = Size - 1; i >= 0; i--)
            {
                result[i] = TritSum(left[i], right[i], ref result.carry);
            }
            return result;
        }

        public static Tryte operator -(Tryte left, Tryte right)
        {
            Tryte res = new Tryte(left);
            res = -res + right;
            return res;
        }

        public static Tryte operator -(Tryte right)
        {
            Tryte result = new Tryte(right);
            for (int i = 0; i < Size; i++)
            {
                if(result.tryteBase[i] == Trit.True)
                    result.tryteBase[i] = Trit.False;
                else if (result.tryteBase[i] == Trit.False)
                    result.tryteBase[i] = Trit.True;
            }
            return result;
        }

        public static Tryte operator <<(Tryte left, int n )
        {
            if(n > 8 || n < 0)
                return new Tryte();
            else if (n == 0) return new Tryte(left);            
            Tryte result = new Tryte();

            for (int i = 0; i < Size - n; i++)
            {
                result[i] = left[i + n];
            }
            int s = Size-1;
            for (int i = n-1; i >= 0; i--)
            {
                result.transfer[s] = left[i];
                s--;
                
            }
            for(int i = Size - n; i < Size; i++) result[i] = Trit.Unknown;
            return result;
        }

        public static Tryte operator >>(Tryte left, int n)
        {
            if (n > 8 || n < 0)
                return new Tryte();
            else if (n == 0) return left;
            Tryte result = new Tryte();
            for (int i = n; i < Size; i++)            
                result[i] = left[i - n];
            int s = 0;
            for (int i = Size - n; i < Size; i++)
            {
                result.transfer[s] = left[i];
                s++;
            }
            for (int i = 0; i < n; i++) result[i] = Trit.Unknown;
            return result;
        }

        #endregion

        #region Вспомогательные функции


        public int FirstIndex()
        {
            int index = 0;
            for (; index <= Size - 1; index++)
                if (tryteBase[index] != Trit.Unknown)
                    return index;
            return index = 8;

        }
        
        public override string ToString()
        {
            StringBuilder s = new StringBuilder(Size);
            for (int i = 0; i < Size; i++)
                s.Append(tryteBase[i].ToString("D"));
            return s.ToString();
        }

        public string ToString(string format)
        {
            StringBuilder s = new StringBuilder(Size);
            for (int i = 0; i < Size; i++)
                s.Append(tryteBase[i].ToString(format));
            return s.ToString();
        }



        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            Tryte t = obj as Tryte;
            if ((Object) t == null)
                return false;
            for (int i = 0; i < Size; i++)
            {
                if (tryteBase[i] != t.tryteBase[i])
                    return false;
            }
            return true;
        }



        public override int GetHashCode()
        {
            return (tryteBase != null ? tryteBase.GetHashCode() : 0);
        }



        public void DebugWrite()
        {
            for (int i = 0; i < Size; i++)
            {
                Console.Write(tryteBase[i]);
            }
            Console.WriteLine();
        }
        #endregion

    }
}
