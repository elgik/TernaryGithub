using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace TernaryCore
{
    public class Integer
    {
        private const int Size = 3;
        private Tryte[] ibase = {new Tryte(), new Tryte(), new Tryte()};

        public Integer(){ }

        public Integer(long obj) : this()
        {
            if (obj == 0) return;

            bool neg = obj < 0;

            if (neg) obj = -obj;

            long[] result = new long[Size*Tryte.Size];
            long mod; int index = result.Length - 1;
            while (obj >= 3)
            {
                mod = obj%3;
                if (mod < 2) { result[index] = mod; obj/= 3; }
                else { result[index] = -1; obj = obj/3 + 1; }
                --index;
                if (index < 0)
                {
                    ++index;
                    break;
                }
            }
            if(obj != 2)
                result[index] = obj;
            else { result[index] = -1; result[index - 1] = 1; }
            index = 0;
            int[] temp = new int[9];
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Tryte.Size; j++)
                {
                    temp[j] = (int) result[index];
                    index++;
                }
                if(neg) ibase[i] = -new Tryte(temp);
                else ibase[i] = new Tryte(temp);
            }
        }

        public Integer(Integer copy)
        {
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Tryte.Size; j++)
                    ibase[i] = new Tryte(copy.ibase[i]);
            }
        }

        #region Операторы преобразования

        public static implicit operator long(Integer obj)
        {
            long result = 0;
            int temp = 0;
            for (int i = Size - 1; i >= 0; i--)
            {
                for (int j = Tryte.Size - 1; j >= 0; j--)
                {
                    result += (long) obj.ibase[i][j] * (long)Math.Pow(3,temp);
                    temp++;
                }
            }
            return result;
        }

        public static explicit operator int(Integer obj)
        {
            int result = 0;
            int temp = 0;
            for (int i = Size - 1; i >= 0; i--)
            {
                for (int j = Tryte.Size - 1; j >= 0; j--)
                {
                    result += (int)obj.ibase[i][j] * (int)Math.Pow(3, temp);
                    temp++;
                }
            }
            return result;
        }

        public static implicit operator Integer(int obj)
        {
            return new Integer((long) obj);
        }

        public static implicit operator Integer(long obj)
        {
            return new Integer(obj);
        }

        #endregion

        #region Алгебраические операции

        public static Integer operator +(Integer left, Integer right)
        {
            Integer result = new Integer();
            Tryte temp = new Tryte();
            for (int i = Size - 1; i >= 0; i--)
            {
                result.ibase[i] = temp + left.ibase[i] + right.ibase[i] ;
                temp = new Tryte(new []{0,0,0,0,0,0,0,0,(int)result.ibase[i].Carry});
            }
            for (int i = 0; i < Size; i++)
            {
                result.ibase[i].Carry = Trit.Unknown;
                left.ibase[i].Carry = Trit.Unknown;
                right.ibase[i].Carry = Trit.Unknown;
            }

            return result;
        }

        public static Integer operator -(Integer right)
        {
            Integer result = new Integer(right);
            for (int i = 0; i < Size; i++)
                result.ibase[i] = -result.ibase[i];
            return result;
        }

        public static Integer operator -(Integer left, Integer right)
        {
            var result = left + -right;
            return result;
        }

        public static Integer operator *(Integer left, Integer right)
        {
            Integer result = new Integer();
            Integer temp = new Integer();
            int[] leftind = left.FindIndex(); int[] rightind = right.FindIndex();
            
            bool chooseright;
            if (leftind[0] == rightind[0]) chooseright = leftind[1] > rightind[1];
            else chooseright = leftind[0] >= rightind[0];
            
            int index1, index2, offset = 0;
            if (chooseright)
            {                                
                index1 = Size - 1;                
                while (index1 >= leftind[0])
                {                     
                    index2 = Tryte.Size - 1;
                    while ((index1 == leftind[0] && index2 >= leftind[1]) || (index1 != leftind[0] && index2 >= 0))
                    {                        
                        for (int i = Size - 1; i >= rightind[0]; i--)                        
                            for (int j = Tryte.Size - 1; j >= 0; j--)                            
                                temp.ibase[i][j] = Tryte.TritMul(right.ibase[i][j], left.ibase[index1][index2]);                                                                                                     
                        temp = temp << offset;                                              
                        result += temp;                        
                        temp = new Integer();
                        offset++; 
                        index2--;
                    }
                    index1--;
                }
            }
            else
            {                
                index1 = Size - 1;                
                while (index1 >= rightind[0])
                {                    
                    index2 = Tryte.Size - 1;
                    while ((index1 == rightind[0] && index2 >= rightind[1]) || (index1 != rightind[0] && index2 >= 0))
                    {
                        for (int i = Size - 1; i >= leftind[0]; i--)
                            for (int j = Tryte.Size - 1; j >= 0; j--)                            
                                temp.ibase[i][j] = Tryte.TritMul(left.ibase[i][j], right.ibase[index1][index2]);                                              
                        temp = temp << offset;                        
                        result += temp;                       
                        temp = new Integer();
                        offset++; 
                        index2--;
                    }
                    index1--;
                }
            }
            return result;
        }

        public static Integer operator /(Integer left, Integer right)
        {
            Integer result = new Integer(left);
            Integer temp = new Integer(right);
            long count = 0;
            bool neg = false;
            if (right == 0)
                throw new DivideByZeroException();
            if (left == 0)
                return new Integer();
            if (left < 0 && right > 0)
            {
                result = -result;
                neg = true;
            }
            else if (left > 0 && right < 0)
            { 
                temp = new Integer(-right);
                neg = true;
            }
            else if (left < 0 && right < 0)
            {
                result = new Integer(-left);
                temp = new Integer(-right);
                neg = true;
            }

            while (result > 0)
            {
                result = result - temp;
                count++;
            }
            if (result < 0) count--;
            if (neg) return result = -count;
            return result = count;
        }

        public static Integer operator %(Integer left, Integer right)
        {
            Integer result = new Integer(left);
            Integer temp = new Integer(right);
            long count = 0;
            bool neg = false;
            if (right == 0)
                throw new DivideByZeroException();
            if (left == 0)
                return result = 0;
            if (left < 0 && right > 0)
            {
                result = new Integer(-left);
                neg = true;
            }
            else if (left > 0 && right < 0)
            {
                temp = new Integer(-right);
                neg = true;
            }
            else if (left < 0 && right < 0)
            {
                result = new Integer(-left);
                temp = new Integer(-right);
                neg = true;
            }

            while (result > 0)
            {
                result = result - temp;
                count++;
            }
            if (result == 0) return new Integer();
            if (neg) return -(result + temp);
            return result + temp;
        }

        public static Integer operator <<(Integer left, int n)
        {
            Integer result = new Integer(left);
            if (n < 9)
            {
                for (int i = Size - 1; i >= 0; i--)
                    result.ibase[i] = result.ibase[i] << n;
                for (int i = Size - 2; i >= 0; i--)
                    for (int j = 0; j < Tryte.Size; j++)
                        result.ibase[i][j] = (Trit)((int)result.ibase[i + 1].transfer[j] + (int)result.ibase[i][j]);
            }
            else if (n >= 9 && n < 18)
            { 
                for (int i = 0; i <= Size - 2; i++)
                {
                    result.ibase[i] = new Tryte(result.ibase[i + 1] << n - Tryte.Size);
                }
                for (int i = Tryte.Size -1; i >= 0; i--)
                {
                    result.ibase[2][i] = Trit.Unknown;
                    
                    result.ibase[0][i] = (Trit)((int)result.ibase[1].transfer[i] + (int)result.ibase[0][i]);
                }
            }
            else if (n >= 18 && n < 27)
            {
                result.ibase[0] = result.ibase[Size - 1] << n - Tryte.Size - Tryte.Size;
                for (int i = 1; i < Size; i++) for (int j = 0; j < Tryte.Size; j++) result.ibase[i][j] = Trit.Unknown;
            }
            for(int i = 0; i < Size; i++) for(int j = 0; j < Tryte.Size; j++) result.ibase[i].transfer[j] = Trit.Unknown;
            return result;
        }
        //[TODO] Реализовать по принципу сдвига слева. После этого использовать в делении  
        public static Integer operator >>(Integer left, int n)
        {
            Integer result = new Integer(left);
            for (int i = Size - 1; i >= 0; i--)
                result.ibase[i] = result.ibase[i] >> n;
            for (int i = 1; i < Size; i++)            
                for (int j = 0; j < Tryte.Size; j++)                
                    result.ibase[i][j] = (Trit)((int)result.ibase[i - 1].transfer[j] + (int)result.ibase[i][j]);

            for (int i = 0; i < Size; i++) for (int j = 0; j < Tryte.Size; j++) result.ibase[i].transfer[j] = Trit.Unknown;
            return result;
        }

        #endregion

        #region Вспомогательные фукнции

        public void DebugWrite()
        {
            foreach (var tryte1 in ibase)
            {
                Console.Write(tryte1.ToString("D"));
            }
            Console.WriteLine();
        }

        public void SetTrytes(Tryte[] trytes)
        {
            if (trytes.Length != Size)
                throw new Exception();
            for (int i = 0; i < Size; i++)
                trytes[i] = ibase[i];
        }

        private int[] FindIndex()
        {
            for(int i = 0; i < Size; i++)
                for (int j = 0; j < Tryte.Size; j++)
                {
                    if (ibase[i][j] != Trit.Unknown)
                        return new[] {i, j};
                }
            return new []{2, 8};
        }
       
        #endregion

    }


}
