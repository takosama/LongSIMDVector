using System;
using System.Diagnostics;
using System.Numerics;
using System.Xml.Serialization;

namespace LongSIMDVector
{
    namespace ConsoleApp19
    {
  unsafe      internal class Program
        {
       unsafe     private static void Main(string[] args)
            {
                var arr = new float[1001];
                for (var i = 0; i < arr.Length; i++)
                    arr[i] = i;
                var arr2 = new float[1001];
                for (var i = 0; i < arr.Length; i++)
                    arr2[i] = i + 1;


                var sw = new Stopwatch();

                var lv = new LongVector(arr);
                var lv2 = new LongVector(arr2);

                LongVector tmp;
                float[] tmpa;
                float sum = 0;
                sw.Start();

                for (var i = 0; i < 1000000; i++)
                {
                    //       tmpa= Arra.add(arr, arr2);
                     //        sum = Arra.dot(arr, arr2); //1002001 3.34334E+08

                    //    tmp = LongVector.Add(lv, lv2); 3.34334E+08
                        sum = LongVector.Dot(lv, lv2);
                    
                }
                sw.Stop();
                Console.WriteLine(sw.ElapsedMilliseconds);
            }
        }

        internal class Arra
        {
            public static float[] add(float[] arr, float[] arr1)
            {
                var rtn = new float[arr.Length];
                for (var j = 0; j < arr.Length; j++)
                    rtn[j] = arr[j] + arr1[j];
                return rtn;
            }

            public static float sum(float[] arr)
            {
                float sum = 0;
                for (var j = 0; j < arr.Length; j++)
                    sum += arr[j];
                return sum;
            }

            public static float dot(float[] arr, float[] arr1)
            {
                float sum = 0;
                for (var i = 0; i < arr.Length; i++)
                    sum += arr[i] * arr1[i];
                return sum;
            }
        }

  unsafe      internal class LongVector
        {
            public Vector4[] Vector4s;
            public int Lng;


            public float this[int n]
            {
                get
                {
                    switch (n % 4)
                    {
                        case 0:
                            return Vector4s[n / 4].X;
                        case 1:
                            return Vector4s[n / 4].Y;
                        case 2:
                            return Vector4s[n / 4].Z;
                        case 3:
                            return Vector4s[n / 4].W;
                    }
                    throw new Exception();
                }
                set
                {
                    switch (n % 4)
                    {
                        case 0:
                            Vector4s[n / 4].X = value;
                            return;
                        case 1:
                            Vector4s[n / 4].Y = value;
                            return;
                        case 2:
                            Vector4s[n / 4].Z = value;
                            return;
                        case 3:
                            Vector4s[n / 4].W = value;
                            return;
                    }
                }
            }

            public LongVector(int n)
            {
                this.Lng = n;
                var num = n % 4 == 0 ? n / 4 : n / 4 + 1;
                this.Vector4s=new Vector4[num];
            }

            public LongVector(Vector4[] v, int Lng)
            {
                Vector4s = v;
                this.Lng = Lng;
            }

            public LongVector(float[] v)
            {
                Lng = v.Length;
                var end = 4 * (Lng / 4);
                var num = Lng % 4 == 0 ? end / 4 : end / 4 + 1;
                Vector4s = new Vector4[num];

                for (var i = 0; i < end; ++i)
                {
                    var k = i / 4;
                    Vector4s[k].X = v[i];
                    Vector4s[k].Y = v[++i];
                    Vector4s[k].Z = v[++i];
                    Vector4s[k].W = v[++i];
                }
                try
                {
                    var i = end;
                    Vector4s[end / 4].X = v[i];
                    Vector4s[end / 4].Y = v[++i];
                    Vector4s[end / 4].Z = v[++i];
                    Vector4s[end / 4].W = v[++i];
                }
                catch (Exception e)
                {
                }
            }

            public float Sum()
            {
                var isAmari = Vector4s.Length % 2 == 1;
                var last = Vector4s.Length / 2;
                var tmp = new Vector4[Vector4s.Length / 2];

                for (var i = 0; i < last; i++)
                    tmp[i] = Vector4s[i] + Vector4s[last + i];

                if (isAmari)
                    tmp[0] = Vector4.Add(tmp[0], Vector4s[Vector4s.Length - 1]);
                return Sum_s(last, tmp);
            }

            private float Sum_s(int lng, Vector4[] tmp)
            {
                if (lng == 1)
                {
                    fixed (float* p = &tmp[0].X)
                    {
                        float* pp = p;
                        return *pp + *(++pp) + *(++pp) + *(++pp);
                    }
                }


                var isAmari = lng % 2 == 1;
                var last = lng / 2;
                for (var i = 0; i < last; i++)
                    tmp[i] = tmp[i] + tmp[last + i];

                if (isAmari)
                    tmp[0] = Vector4.Add(tmp[0], tmp[lng - 1]);
                return Sum_s(last, tmp);
            }

       unsafe     public static float Dot(LongVector v0, LongVector v1)
            {
               var tmp = new LongVector(v0.Lng);
                fixed (float* p = &tmp.Vector4s[0].X)
                fixed (Vector4* vvv0 = &v0.Vector4s[0])
                fixed (Vector4* vvv1 = &v1.Vector4s[0])
                {
                    float* pp = p;
                    Vector4* vv0 = vvv0;
                    Vector4* vv1 = vvv1;

                    for (var i = 0; i < v0.Vector4s.Length; i++)
                        *pp++ = Vector4.Dot(*vv0++,*vv1++);
                }
                //float sum = 0;
                //foreach (var t in tmp)
                //    sum += t;
                return tmp.Sum();
            }

            public static LongVector Add(LongVector v0, LongVector v1)
            {
                var rtn = new Vector4[v0.Vector4s.Length];
                for (var i = 0; i < v0.Vector4s.Length; i++)
                    rtn[i] = Vector4.Add(v0.Vector4s[i], v1.Vector4s[i]);
                return new LongVector(rtn, v0.Lng);
            }
        }
    }
}