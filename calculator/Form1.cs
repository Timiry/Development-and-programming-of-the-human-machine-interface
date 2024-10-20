using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MathNet.Numerics;

namespace calculator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        enum operations { plus, minus, multipl, div,
                          pow2, powxy, sin, cos, tan,
                          sqrt, pow10x, log, exp, mod, 
                          faktorial};

        double curNum;
        double res = 0;
        operations curOper = operations.plus;
        double coefDegRad = 1;

        // кнопки цифр
        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = textBox1.Text + "1";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = textBox1.Text + "2";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text = textBox1.Text + "3";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.Text = textBox1.Text + "4";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox1.Text = textBox1.Text + "5";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            textBox1.Text = textBox1.Text + "6";
        }

        private void button7_Click(object sender, EventArgs e)
        {
            textBox1.Text = textBox1.Text + "7";
        }

        private void button8_Click(object sender, EventArgs e)
        {
            textBox1.Text = textBox1.Text + "8";
        }

        private void button9_Click(object sender, EventArgs e)
        {
            textBox1.Text = textBox1.Text + "9";
        }

        private void button0_Click(object sender, EventArgs e)
        {
            textBox1.Text = textBox1.Text + "0";
        }


        private void buttonComa_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" & !textBox1.Text.Contains(','))
            {
                textBox1.Text = textBox1.Text + ",";
            }
        }

        private void buttonClean_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            curNum = 0;
            res = 0;
        }

        // кнопки операций
        private void buttonPlus_Click(object sender, EventArgs e)
        {
            if(!double.TryParse(textBox1.Text, out curNum)) MessageBox.Show("Введите число");
            solve();
            curOper = operations.plus;
            textBox1.Text = "";
        }

        private void buttonMinus_Click(object sender, EventArgs e)
        {
            if (!double.TryParse(textBox1.Text, out curNum)) MessageBox.Show("Введите число");
            solve();
            curOper = operations.minus;
            textBox1.Text = "";
        }

        private void buttonMultipl_Click(object sender, EventArgs e)
        {
            if (!double.TryParse(textBox1.Text, out curNum)) MessageBox.Show("Введите число");
            solve();
            curOper = operations.multipl;
            textBox1.Text = "";
        }

        private void buttonDiv_Click(object sender, EventArgs e)
        {
            if (!double.TryParse(textBox1.Text, out curNum)) MessageBox.Show("Введите число");
            solve();
            curOper = operations.div;
            textBox1.Text = "";
        }

        // кнопка равно
        private void buttonSolv_Click(object sender, EventArgs e)
        {
            if (!double.TryParse(textBox1.Text, out curNum)) MessageBox.Show("Введите число");
            solve();
            textBox1.Text = res.ToString();

            res = 0;
            curOper = operations.plus;
        }

        public void solve()
        {
            switch (curOper)
            {
                case operations.plus:
                    res = res + curNum; break;
                case operations.minus:
                    res = res - curNum; break;
                case operations.multipl:
                    res = res * curNum; break;
                case operations.div:
                    res = res / curNum; break;
                case operations.pow2:
                    res = curNum * curNum; break;
                case operations.powxy:
                    res = Math.Pow(res, curNum); break;
                case operations.sin:
                    res = Math.Sin(curNum * coefDegRad); break;
                case operations.cos:
                    res = Math.Cos(curNum * coefDegRad); break;
                case operations.tan:
                    res = Math.Tan(curNum * coefDegRad); break;
                case operations.sqrt:
                    res = Math.Sqrt(curNum); break;
                case operations.pow10x:
                    res = Math.Pow(10, curNum); break;
                case operations.log:
                    res = Math.Log10(curNum); break;
                case operations.exp:
                    res = Math.Exp(curNum); break;
                case operations.mod:
                    res = Math.Abs(curNum); break;
                case operations.faktorial:
                    try
                    {
                        if (curNum - Math.Floor(curNum) == 0) {
                            res = Factorial((int)curNum);
                        }
                        else
                        {
                            res = Factorial(curNum);
                        }
                    }
                    catch(Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                    break;
                default: break;
            }
        }

        public long Factorial(int n)
        {
            if (n < 0)
                throw new ArgumentException("Факториал не определен для отрицательных чисел.");
            if (n == 0 || n == 1)
                return 1;
            return n * Factorial(n - 1);
        }
        double Factorial(double n)
        {
            return SpecialFunctions.Gamma(n + 1);
        }

        private void buttonPow2_Click(object sender, EventArgs e)
        {
            if (!double.TryParse(textBox1.Text, out curNum)) MessageBox.Show("Введите число");
            solve();
            curOper = operations.pow2;
            solve();
            textBox1.Text = res.ToString();
        }

        private void buttonPowy_Click(object sender, EventArgs e)
        {
            if (!double.TryParse(textBox1.Text, out curNum)) MessageBox.Show("Введите число");
            solve();
            curOper = operations.powxy;
            textBox1.Text = "";
        }

        private void buttonSin_Click(object sender, EventArgs e)
        {
            if (!double.TryParse(textBox1.Text, out curNum)) MessageBox.Show("Введите число");
            solve();
            curOper = operations.sin;
            solve();
            textBox1.Text = res.ToString();
        }

        private void buttonCos_Click(object sender, EventArgs e)
        {
            if (!double.TryParse(textBox1.Text, out curNum)) MessageBox.Show("Введите число");
            solve();
            curOper = operations.cos;
            solve();
            textBox1.Text = res.ToString();
        }

        private void buttonTan_Click(object sender, EventArgs e)
        {
            if (!double.TryParse(textBox1.Text, out curNum)) MessageBox.Show("Введите число");
            solve();
            curOper = operations.tan;
            solve();
            textBox1.Text = res.ToString();
        }

        private void buttonSqrt_Click(object sender, EventArgs e)
        {
            if (!double.TryParse(textBox1.Text, out curNum)) MessageBox.Show("Введите число");
            solve();
            curOper = operations.sqrt;
            solve();
            textBox1.Text = res.ToString();
        }

        private void buttonPow10x_Click(object sender, EventArgs e)
        {
            if (!double.TryParse(textBox1.Text, out curNum)) MessageBox.Show("Введите число");
            solve();
            curOper = operations.pow10x;
            solve();
            textBox1.Text = res.ToString();
        }

        private void buttonLog_Click(object sender, EventArgs e)
        {
            if (!double.TryParse(textBox1.Text, out curNum)) MessageBox.Show("Введите число");
            solve();
            curOper = operations.log;
            solve();
            textBox1.Text = res.ToString();
        }

        private void buttonExp_Click(object sender, EventArgs e)
        {
            if (!double.TryParse(textBox1.Text, out curNum)) MessageBox.Show("Введите число");
            solve();
            curOper = operations.log;
            solve();
            textBox1.Text = res.ToString();
        }

        private void buttonMod_Click(object sender, EventArgs e)
        {
            if (!double.TryParse(textBox1.Text, out curNum)) MessageBox.Show("Введите число");
            solve();
            curOper = operations.mod;
            solve();
            textBox1.Text = res.ToString();
        }

        private void buttonFaktorial_Click(object sender, EventArgs e)
        {
            if (!double.TryParse(textBox1.Text, out curNum)) MessageBox.Show("Введите число");
            solve();
            curOper = operations.faktorial;
            solve();
            textBox1.Text = res.ToString();
        }

        private void buttonPi_Click(object sender, EventArgs e)
        {
            textBox1.Text = Math.PI.ToString();
        }

        private void buttonNeg_Click(object sender, EventArgs e)
        {
            if (!double.TryParse(textBox1.Text, out curNum)) MessageBox.Show("Введите число");
            textBox1.Text = (-curNum).ToString();
        }

        private void buttonDel_Click(object sender, EventArgs e)
        {
            textBox1.Text = textBox1.Text.Substring(0, textBox1.Text.Length - 1);
        }

        // обработка смены градусов и радианов
        private void rbDeg_CheckedChanged(object sender, EventArgs e)
        {
            coefDegRad = Math.PI / 180;
        }

        private void rbRad_CheckedChanged(object sender, EventArgs e)
        {
            coefDegRad = 1;
        }
    }
}
