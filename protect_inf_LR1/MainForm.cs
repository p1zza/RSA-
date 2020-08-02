using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Numerics;

namespace protect_inf_LR1
{
    public partial class MainForm : Form
    {
        char[] characters = new char[] { '#', 'А', 'Б', 'В', 'Г', 'Д', 'Е', 'Ё', 'Ж', 'З', 'И',
                                                        'Й', 'К', 'Л', 'М', 'Н', 'О', 'П', 'Р', 'С', 
                                                        'Т', 'У', 'Ф', 'Х', 'Ц', 'Ч', 'Ш', 'Щ', 'Ь', 'Ы', 'Ъ',
                                                        'Э', 'Ю', 'Я', ' ', '1', '2', '3', '4', '5', '6', '7',
                                                        '8', '9', '0' };
        public long e_1;
        public long e_2;

        public long d_1;
        public long d_2;
        public MainForm()
        {
            InitializeComponent();
        }

        //зашифровать
        private void buttonEncrypt_Click(object sender, EventArgs e)
        {
            
            if ((!String.IsNullOrEmpty(textBox_p.Text)) && (!String.IsNullOrEmpty(textBox_q.Text)))
            {
                long p = Convert.ToInt64(textBox_p.Text);
                long q = Convert.ToInt64(textBox_q.Text);
                long p2 = Convert.ToInt64(textBox_p2.Text);
                long q2 = Convert.ToInt64(textBox_q2.Text);


                if (IsTheNumberSimple(p) && IsTheNumberSimple(q))
                {
                    string s = "";

                    using(StreamReader sr = new StreamReader("in.txt"))
                    {
                        while (!sr.EndOfStream)
                        {
                            s += sr.ReadLine();
                        }

                        sr.Close();
                    }

                    s = s.ToUpper();
        
                    long n = p * q;
                    long m = (p - 1) * (q - 1);


                    long m_2 = (p2 - 1) * (q2 - 1); 
                    
                    
                    d_1 = Calculate_d(m);
                    d_2 = Calculate_d(m_2);

                    e_1 = Calculate_e(d_1, m);
                    e_2 = Calculate_e(d_2, m_2);

                    List<string> result = RSA_Encode(s, e_1, n);
                    result = RSA_Encode(s, e_2, n);

                    using (StreamWriter sw = new StreamWriter("out1.txt"))
                    {
                        foreach (string item in result)
                        {
                            sw.WriteLine(item);
                        }
                        sw.Close();
                    }
                    

                    textBox_d.Text = d_1.ToString();
                    textBox_n.Text = n.ToString();

                    textBox_d2.Text = d_2.ToString();
                    textBox_n2.Text = textBox_n.Text;

                    Process.Start("out1.txt");

                    label7.Text = "e_1 = " + e_1 + "\n" + "e_2 = " + e_2 + "\n d_1 = " + d_1 + "\n" + "d_2 = " + d_2;
                }
                else
                    MessageBox.Show("p или q - не простые числа!");
            }
            else
                MessageBox.Show("Введите p и q!");
        }

        //расшифровать
        private void buttonDecipher_Click(object sender, EventArgs e)
        {
            if ((!String.IsNullOrEmpty(textBox_d.Text)) && (!String.IsNullOrEmpty(textBox_n.Text)))
            {
                long d = Convert.ToInt64(textBox_d.Text);
                long n = Convert.ToInt64(textBox_n.Text);

                List<string> input = new List<string>();

                using (StreamReader sr = new StreamReader("out1.txt"))
                {
                    while (!sr.EndOfStream)
                    {
                        input.Add(sr.ReadLine());
                    }

                    sr.Close();
                }

                string result = RSA_Decode(input, d_2, n);
                result = RSA_Decode(input, d_1, n);
                using (StreamWriter sw = new StreamWriter("out2.txt"))
                {
                    sw.WriteLine(result);
                    sw.Close();
                }
                Process.Start("out2.txt");
            }
            else
                MessageBox.Show("Введите секретный ключ!");
        }

        //проверка: простое ли число?
        private bool IsTheNumberSimple(long n)
        {
            if (n < 2)
                return false;

            if (n == 2)
                return true;

            for (long i = 2; i < n; i++)
                if (n % i == 0)
                    return false;

            return true;
        }

        //зашифровать
        private List<string> RSA_Encode(string s, long e, long n)
        {
            List<string> result = new List<string>();

            BigInteger bi;

            for (int i = 0; i < s.Length; i++)
            {
                int index = Array.IndexOf(characters, s[i]);

                bi = new BigInteger(index);
                bi = BigInteger.Pow(bi, (int)e);

                BigInteger n_ = new BigInteger((int)n);

                bi = bi % n_;
                
                if(bi>0)
                {
                    result.Add(bi.ToString());
                }
                
            }

            return result;
        }


        //расшифровать
        private string RSA_Decode(List<string> input, long d, long n)
        {
            string result = "";

            BigInteger bi;
            foreach (string item in input)
            {
                bi = new BigInteger(Convert.ToDouble(item));
                bi = BigInteger.Pow(bi, (int)d);

                BigInteger n_ = new BigInteger((int)n);

                bi = bi % n_;

                int index = Convert.ToInt32(bi.ToString());

                if(index <= characters.Length)
                {
                    result += characters[index].ToString();
                }
                
            }

            return result;
        }

        //вычисление параметра d. d должно быть взаимно простым с m
        private long Calculate_d(long m)
        {
            long d = m - 1;

            for (long i = 2; i <= m; i++)
                if ((m % i == 0) && (d % i == 0) && (d!=d_1)) //если имеют общие делители
                {
                    d--;
                    i = 1;
                }

            return d;
        }

        //вычисление параметра e
        private long Calculate_e(long d, long m)
        {
            //long e = 10;
            long e = 5;

            while (true)
            {
                if (((e * d) % m == 1)&&(e!=d_1)&& (e != d_2))
                    break;
                else
                    e++;
            }
            return e;
        }

        private void EditTXT_Click(object sender, EventArgs e)
        {
            Process.Start("in.txt");
        }
    }

}
