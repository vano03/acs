using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using ExcelDataReader;
using System.Numerics;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using System.Windows.Forms.DataVisualization.Charting;
using System.Text.RegularExpressions;

namespace KursovayaISIS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        int[] rdata = new int[3];
        string punkt = "";
        //double[,] massiv2 = { { 1, 2, 3, 4, 5, 6, 7, 8 }, { -1, -2, -3, -4, -5, -6, -7, -8 } };
        string lin = "", pok = "", par = "", mas = "", godi = "";
        int stroki = 0, stolbci = 0, r = 0;
        int mgod = 0;
        string put = "";
        int k = 0;
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            punkt = comboBox1.SelectedItem.ToString();
        }

        double Poisk_Kriteriya(int stepen_svobodi1, int stepen_svobodi2, double[,] arr, int ii, int jj)
        {
            int pomosh1 = 0, pomosh2 = 0;

            for (int i = 1; i < ii; i++)
            {
                if (arr[i, 0] == stepen_svobodi2)
                {
                    pomosh2 = i;
                }
            }
            if (pomosh2 == 0)
            {
                int otklonenie, motklonenie = Math.Abs((int)arr[1, 0] - stepen_svobodi2);
                for (int i = 2; i < ii; i++)
                {
                    otklonenie = Math.Abs((int)arr[i, 0] - stepen_svobodi2);
                    //Console.WriteLine("M" + otklonenie);
                    if (motklonenie > otklonenie)
                    {
                        motklonenie = otklonenie;
                        pomosh2 = i;
                    }

                }
            }

            for (int j = 1; j < jj; j++)
            {
                if (arr[0, j] == stepen_svobodi2)
                {
                    pomosh1 = j;
                }
            }
            if (pomosh1 == 0)
            {
                int otklonenie, motklonenie = Math.Abs((int)arr[0, 1] - stepen_svobodi1);
                for (int j = 2; j < jj; j++)
                {
                    otklonenie = Math.Abs((int)arr[0, j] - stepen_svobodi1);
                    if (motklonenie > otklonenie)
                    {
                        motklonenie = otklonenie;
                        pomosh1 = j;
                    }
                }
            }

            return arr[pomosh2, pomosh1];
        }
        string Stacionarnost(string massivvh)
        {
            string[] massiv2s = massivvh.Split(' ');
            int dlina_m2 = massiv2s.Length;
            double[] massiv = new double[dlina_m2];
            for (int i = 0; i < dlina_m2 - 1; i++) massiv[i] = Double.Parse(massiv2s[i]);

            int n1, n2;
            if (massiv.Length % 2 == 1)
            {
                n1 = massiv.Length / 2;
                n2 = massiv.Length / 2 + 1;
            }
            else n1 = n2 = massiv.Length / 2;

            double[] polovina1 = new double[n1];
            double[] polovina2 = new double[n2];

            for (int i = 0; i < n1; i++) polovina1[i] = massiv[i];
            for (int i = n1; i < massiv.Length; i++) polovina2[i - n1] = massiv[i];

            for (int i = 0; i < n1; i++) Console.WriteLine("Первая половина " + polovina1[i]);
            for (int i = 0; i < n2; i++) Console.WriteLine("Вторая половина " + polovina2[i]);

            // вычисление средних значений выборок
            double y_sr1 = 0;
            double y_sr2 = 0;

            for (int i = 0; i < n1; i++)
            {
                y_sr1 += polovina1[i];
            }
            y_sr1 = y_sr1 / polovina1.Length;

            Console.WriteLine("Среднее значение первой половины " + y_sr1);

            for (int i = 0; i < n2; i++)
            {
                y_sr2 += polovina2[i];
            }
            y_sr2 = y_sr2 / polovina2.Length;

            Console.WriteLine("Среднее значение второй половины " + y_sr2);

            // вычисление дисперсий выборок
            double dispersiya1 = 0;
            double dispersiya2 = 0;

            for (int i = 0; i < n1; i++)
            {
                dispersiya1 += (polovina1[i] - y_sr1) * (polovina1[i] - y_sr1);
                //Console.WriteLine("Дисперсия первой половины " + dispersiya1);
            }
            dispersiya1 = dispersiya1 / (n1 - 1);

            Console.WriteLine("Дисперсия первой половины " + dispersiya1);

            for (int i = 0; i < n2; i++)
            {
                dispersiya2 += (polovina2[i] - y_sr2) * (polovina2[i] - y_sr2);
            }
            dispersiya2 = dispersiya2 / (n2 - 1);

            Console.WriteLine("Дисперсия второй половины " + dispersiya2);

            double[,] Zagruzka_tablici(int ii, int jj, string nazvanie)
            {
                double[,] arr = new double[ii, jj];
                StreamReader sr = new StreamReader(nazvanie);
                for (int i = 0; i < ii; i++)
                {
                    string stroka = sr.ReadLine();
                    string[] znacheniya = stroka.Split(' ');
                    //Console.WriteLine(znacheniya[19]);
                    for (int j = 0; j < jj; j++)
                    {
                        arr[i, j] = Double.Parse(znacheniya[j]);
                        //Console.WriteLine(arr[i, j]);
                    }
                }
                return arr;
            }

            // загрузка в программу критериев Фишера из файла
            //double[,] arr = new double[29, 20];
            double[,] fisher = Zagruzka_tablici(29, 20, "Kriterii.txt");

            // вычисление критерия Фишера выборки
            double kriterij_fishera = dispersiya2 / dispersiya1;
            Console.WriteLine("F-критерий Фишера " + kriterij_fishera);

            int stepen_svobodi1 = n1 - 2;
            int stepen_svobodi2 = n2 - 2;

            //int stepen_svobodi1 = 51;
            //int stepen_svobodi2 = 27;

            double kriterij_fishera_tablichnij = Poisk_Kriteriya(stepen_svobodi1, stepen_svobodi2, fisher, 29, 20);

            Console.WriteLine("Табличный F-критерий Фишера " + kriterij_fishera_tablichnij);
            //Console.WriteLine(pomosh2 + " " + pomosh1);
            //Console.WriteLine(arr[23, 18]);

            if (kriterij_fishera_tablichnij > kriterij_fishera)
            {
                // критерий Стьюдента
                double dispersiya = Math.Sqrt((dispersiya1 + dispersiya2) / 2);
                Console.WriteLine("Дисперсия равна " + dispersiya);
                double t = Math.Abs(y_sr1 - y_sr2) / (Math.Sqrt(Math.Pow(n1, -1) + Math.Pow(n2, -1)) * dispersiya);
                Console.WriteLine("Коэффициент Стьюдента " + t);
                double f = n1 + n2;
                double[,] styudent = Zagruzka_tablici(91, 2, "Styudent.txt");
                int stepen_svobodi_s = (int)(f - 2);
                int Poisk_Kriteriya_S(int stepen_svobodi, double[,] arr, int ii)
                {
                    int pomosh_s = 0;
                    for (int i = 0; i < ii; i++)
                    {
                        if (styudent[i, 0] == stepen_svobodi)
                        {
                            pomosh_s = i;
                        }
                    }
                    if (pomosh_s == 0)
                    {
                        int otklonenie, motklonenie = Math.Abs((int)styudent[0, 0] - stepen_svobodi);
                        for (int i = 0; i < ii; i++)
                        {
                            otklonenie = Math.Abs((int)styudent[i, 0] - stepen_svobodi);
                            if (motklonenie > otklonenie)
                            {
                                motklonenie = otklonenie;
                                pomosh_s = i;
                            }
                        }
                    }
                    return pomosh_s;
                }
                double kriterij_styudenta_tablichnij = styudent[Poisk_Kriteriya_S(stepen_svobodi_s, styudent, 91), 1];
                Console.WriteLine("Табличный критерий Стьюдента " + kriterij_styudenta_tablichnij);

                if (kriterij_styudenta_tablichnij > t)
                {
                    // Средний уровень ряда и среднее квадратическое отклонение
                    double y_sr = (y_sr1 + y_sr2) / 2;
                    Console.WriteLine("Среднее " + y_sr);

                    int n = massiv.Length;

                    double dispersiyak = 0;
                    for (int i = 0; i < n1 + n2; i++)
                    {
                        dispersiyak += (massiv[i] - y_sr) * (massiv[i] - y_sr);
                    }
                    dispersiyak = Math.Sqrt(dispersiyak / (n1 + n2 - 1));
                    Console.WriteLine("Дисперсия " + dispersiyak);

                    // Новый критерий Стьюдента
                    double kstyudenta = styudent[Poisk_Kriteriya_S(n - 1, styudent, 91), 1];
                    Console.WriteLine("Табличный критерий Стьюдента " + kstyudenta);

                    // Предсказание следующего значения
                    string vivod = "";
                    double y1 = y_sr + kstyudenta * dispersiyak * Math.Sqrt(1 + Math.Pow(n, -1));
                    vivod += "Ряд стационарный. Максимальное число аварий " + Math.Round(y1, 2);
                    double y2 = y_sr - kstyudenta * dispersiyak * Math.Sqrt(1 + Math.Pow(n, -1));
                    if (y2 < 0) { y2 = 0; }
                    vivod += ", минимальное число аварий " + Math.Round(y2, 2);

                    return vivod;
                }
                else return "Ряд не стационарный.";
            }
            else return "Ряд не стационарный.";
        }

        string Funkcii (string massiv)
        {
            string[] massiv2s = massiv.Split(' ');
            int dlina_m2 = massiv2s.Length;
            double[] massiv2 = new double[dlina_m2];
            //for (int i = 0; i < dlina_m2; i++) textBox3.AppendText(massiv2s[i]);
            for (int i = 0; i < dlina_m2 - 1; i++) massiv2[i] = Double.Parse(massiv2s[i]);
            //textBox3.AppendText("/");
            double vm = 0;
            for (int i = 0; i < dlina_m2; i++) vm += massiv2[i];
            Console.WriteLine("Сумма элементов " + vm);

            // Линейная функция

            double vt = 0, vt2 = 0;
            int t2 = 0;
            for (int i = 1; i < dlina_m2 + 1; i++)
            {
                t2 = i * i;
                vt += i;
                vt2 += t2;
            }
            Console.WriteLine("Сумма номеров элементов " + vt);
            Console.WriteLine("Сумма номеров элементов в квадрате " + vt2);

            double yt = 0;
            double vyt = 0;
            for (int i = 0; i < dlina_m2; i++)
            {
                yt = massiv2[i] * (i + 1);
                vyt += yt;
            }
            Console.WriteLine("Сумма из умножений номеров на элементы " + vyt);

            // Решение системы методом Крамера
            double delta = vt2 * dlina_m2 - vt * vt;

            double deltaa = vyt * dlina_m2 - vm * vt;
            double la = deltaa / delta;
            Console.WriteLine("А " + la);
            lin += la + " ";

            double deltab = vt2 * vm - vt * vyt;
            double lb = deltab / delta;
            Console.WriteLine("В " + lb);
            lin += lb + " ";

            // Теоретические уровни
            double[] y = new double[dlina_m2];
            double vy = 0;
            for (int i = 0; i < dlina_m2; i++)
            {
                y[i] = lb + la * (i + 1);
                vy += y[i];
            }

            // Разница теории и реальности (отклонения)
            double[] otkl = new double[dlina_m2];
            double votkl = 0;
            for (int i = 0; i < dlina_m2; i++)
            {
                otkl[i] = (massiv2[i] - y[i]) * (massiv2[i] - y[i]);
                votkl += otkl[i];
            }
            Console.WriteLine("Сумма отклонений " + votkl);

            // Средняя квадратическая ошибка уравнения тренда
            double sr_k_oshibka1 = Math.Sqrt(votkl / (dlina_m2 - 2));
            Console.WriteLine("Средняя квадратическая ошибка уравнения тренда " + sr_k_oshibka1);

            // Показательная функция

            double vlny = 0, lny = 0, vtlny = 0;
            int osh = 0;
            for (int i = 0; i < dlina_m2; i++)
            {
                lny = Math.Log(massiv2[i]);
                if (lny == double.NegativeInfinity) osh = 1;
                vlny += lny;
                vtlny += lny * (i + 1);
            }
            //textBox3.AppendText("Сумма натуральных логарифмов элементов " + vlny.ToString());
            //textBox3.AppendText("Сумма произведений натуральных логарифмов элементов на номера " + vtlny.ToString());
            double sr_k_oshibka2 = 0;
            double pb = 0, pa = 0;
            if (osh == 0)
            {
                // Решение системы методом Крамера
                delta = vt2 * dlina_m2 - vt * vt;

                deltaa = vtlny * dlina_m2 - vlny * vt;
                pa = deltaa / delta;
                pa = Math.Exp(pa);
                Console.WriteLine("А " + pa);
                pok += pa + " ";

                deltab = vt2 * vlny - vt * vtlny;
                pb = deltab / delta;
                pb = Math.Exp(pb);
                Console.WriteLine("В " + pb);
                pok += pb + " ";


                // Теоретические уровни
                vy = 0;
                for (int i = 0; i < dlina_m2; i++)
                {
                    y[i] = pb * Math.Pow(pa, i + 1);
                    vy += y[i];
                }

                // Разница теории и реальности (отклонения)
                votkl = 0;
                for (int i = 0; i < dlina_m2; i++)
                {
                    otkl[i] = (massiv2[i] - y[i]) * (massiv2[i] - y[i]);
                    votkl += otkl[i];
                }
                Console.WriteLine("Сумма отклонений " + votkl);

                // Средняя квадратическая ошибка уравнения тренда
                sr_k_oshibka2 = Math.Sqrt(votkl / (dlina_m2 - 2));
                Console.WriteLine("Средняя квадратическая ошибка уравнения тренда " + sr_k_oshibka2);
            }
            else
            {
                pok += "[не_определено] [не_определено] ";
                sr_k_oshibka2 = double.PositiveInfinity - 1;
            }

            // Парабола второго порядка

            double vt2y = 0, vt3 = 0, vt4 = 0;
            for (int i = 0; i < dlina_m2; i++)
            {
                int nomer = (i + 1);
                vt2y += nomer * nomer * massiv2[i];
                vt3 += nomer * nomer * nomer;
                vt4 += nomer * nomer * nomer * nomer;
            }
            Console.WriteLine("Сумма квадратов номеров на элементы " + vt2y);
            Console.WriteLine("Сумма номеров в третьей степени " + vt3);
            Console.WriteLine("Сумма номеров в четвёртой степени " + vt4);

            // Решение системы методом Крамера

            delta = dlina_m2 * vt2 * vt4 + vt * vt3 * vt2 + vt2 * vt * vt3 - vt2 * vt2 * vt2 - vt * vt * vt4 - dlina_m2 * vt3 * vt3;
            Console.WriteLine("delta " + delta);

            deltaa = vm * vt2 * vt4 + vt * vt3 * vt2y + vt2 * vyt * vt3 - vt2 * vt2 * vt2y - vt * vyt * vt4 - vm * vt3 * vt3;
            double pra = deltaa / delta;
            Console.WriteLine("А " + pra);
            par += pra + " ";

            deltab = dlina_m2 * vyt * vt4 + vm * vt3 * vt2 + vt2 * vt * vt2y - vt2 * vyt * vt2 - vm * vt * vt4 - dlina_m2 * vt3 * vt2y;
            double prb = deltab / delta;
            Console.WriteLine("В " + prb);
            par += prb + " ";

            double deltaz = dlina_m2 * vt2 * vt2y + vt * vyt * vt2 + vm * vt * vt3 - vm * vt2 * vt2 - vt * vt * vt2y - dlina_m2 * vyt * vt3;
            double z = deltaz / delta;
            Console.WriteLine("Z " + z);
            par += z + " ";

            // Теоретические уровни
            vy = 0;
            for (int i = 0; i < dlina_m2; i++)
            {
                y[i] = pra + prb * (i + 1) + z * (i + 1) * (i + 1);
                vy += y[i];
            }

            // Разница теории и реальности (отклонения)
            votkl = 0;
            for (int i = 0; i < dlina_m2; i++)
            {
                otkl[i] = (massiv2[i] - y[i]) * (massiv2[i] - y[i]);
                votkl += otkl[i];
            }
            Console.WriteLine("Сумма отклонений " + votkl);

            // Средняя квадратическая ошибка уравнения тренда
            double sr_k_oshibka3 = Math.Sqrt(votkl / (dlina_m2 - 3));
            Console.WriteLine("Средняя квадратическая ошибка уравнения тренда " + sr_k_oshibka3);

            string vivod = "";// sr_k_oshibka1.ToString() + " " + sr_k_oshibka2.ToString() + " " + sr_k_oshibka3.ToString();
            //textBox3.AppendText(vivod);

            int god = rdata[2] - mgod;
            int rr = r + god;

            double min = Math.Min(sr_k_oshibka1, Math.Min(sr_k_oshibka2, sr_k_oshibka3));
            if (min == sr_k_oshibka1) { vivod = "Наилучшим образом описывает фактические уровни ряда линейная функция. Прогноз составляет " + Math.Round((lb + la * (rr + 1)), 2) + " аварии(й)."; }
            if (min == sr_k_oshibka2) { vivod = "Наилучшим образом описывает фактические уровни ряда показательная функция. Прогноз составляет " + Math.Round((pb * Math.Pow(pa, rr))) + " аварии(й)."; }
            if (min == sr_k_oshibka3) { vivod = "Наилучшим образом описывает фактические уровни ряда парабола второго порядка. Прогноз составляет " + Math.Round((pra + prb * rr + z * rr * rr)) + " аварии(й)."; }

            return vivod;
        }
        private void открытьДанныеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                // Регистрация провайдера кодовых страниц
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.ShowDialog();

                if (openFileDialog.FileName != "")
                {
                    put = openFileDialog.FileName;
                }
                else throw new Exception("Файл не выбран");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии файла: {ex.Message}");
            }
            k = 0;

        }

        private void посмотретьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (k != 0)
            {
                string s = "";
                string[] linz = lin.Split(' ');
                string[] pokz = pok.Split(' ');
                string[] parz = par.Split(' ');
                for (int i = 0; i < comboBox2.Items.Count; i++)
                {
                    s += "Уравнения для " + comboBox2.Items[i].ToString() + " участка:\n1) для линейной функции: " + Math.Round(Double.Parse(linz[2 * i]), 2) + " + " + Math.Round(Double.Parse(linz[2 * i + 1]), 2) + "t\n" + "2) для показательной функции: " + pokz[2 * i] + " * " + pokz[2 * i + 1] + "^t\n" + "3) для параболы второго порядка: " + Math.Round(Double.Parse(parz[3 * i]), 2) + " + " + Math.Round(Double.Parse(parz[3 * i + 1]), 2) + "t + " + Math.Round(Double.Parse(parz[3 * i + 2]), 2) + "t^2\n\n";
                }
                MessageBox.Show(s, "Просмотр уравнений", MessageBoxButtons.OK, MessageBoxIcon.Question);
            }
            else MessageBox.Show("Пожалуйста, нажмите на опцию прогнозирования, нечего выводить", "Ошибка вывода", MessageBoxButtons.OK, MessageBoxIcon.Question);
        }

        /*private void проверкаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] godiz = godi.Split(' ');
            double[] go = new double[godiz.Length - 1];
            for (int i = 0; i < godiz.Length - 1; i++) { go[i] = Double.Parse(godiz[i]); textBox3.AppendText("/" + go[i] + "/");}
        }*/

        private void сделатьПрогнозToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((put != null) && (rdata[1] != 0))
            {
                using (var stream = File.Open(put, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        DataSet result = reader.AsDataSet(new ExcelDataSetConfiguration()
                        {
                            ConfigureDataTable = _ => new ExcelDataTableConfiguration
                            {
                                UseHeaderRow = false
                            }
                        });

                        DataTable table = result.Tables[0];

                        stroki = table.Rows.Count - 1;
                        stolbci = table.Columns.Count - 1;

                        int[,] massiv = new int[stroki, stolbci];

                        // Заполнение матрицы данными из таблицы
                        for (int i = 1; i <= stroki; i++) // Начинаем с 1, так как первая строка содержит заголовки
                        {
                            for (int j = 1; j <= stolbci; j++) // Начинаем с 1, так как первый столбец содержит названия месяцев
                            {
                                massiv[i - 1, j - 1] = Convert.ToInt32(table.Rows[i][j]);
                                //textBox3.AppendText(massiv[i - 1, j - 1].ToString() + " ");
                            }
                        }
                        //textBox3.Text = ""; //транспортная компания отдел логистики

                        godi += massiv[0, 0] + " ";
                        for (int i = 1; i < stroki; i++)
                        {
                            if (massiv[i, 0] == massiv[0, 0])
                            {
                                r = i;
                                mgod = massiv[i - 1, 0];
                                break;
                            }
                            godi += massiv[i, 0] + " ";
                        }

                        comboBox2.Items.Clear();
                        comboBox3.Items.Clear();
                        //rdata[1] = 2;
                        //rdata[2] = 2022;
                        int mesyac = rdata[1];
                        for (int j = 1; j < stolbci; j++)
                        {
                            string massivv = "";
                            for (int i = (mesyac - 1) * r; i < mesyac * r; i++)
                            {
                                massivv += massiv[i, j] + " ";
                                mas += massiv[i, j] + " ";
                            }
                            //textBox3.AppendText(massivv + " ");
                            string v = Stacionarnost(massivv);
                            comboBox3.Items.Add(j);
                            if (v.Contains("не стационарный"))
                            {
                                textBox3.AppendText(j + ". " + Stacionarnost(massivv) + " " + Funkcii(massivv) + "\r\n");
                                comboBox2.Items.Add(j);
                            }
                            else textBox3.AppendText(j + ". " + Stacionarnost(massivv) + "\r\n");
                        }
                    }
                }
                k = 1;
            }
            else MessageBox.Show("Пожалуйста, введите дату и выберите данные, отсутствует информация для прогнозирования", "Ошибка прогнозирования", MessageBoxButtons.OK, MessageBoxIcon.Question);
        }

        private void сохранитьВыводToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (k != 0)
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                if (saveFileDialog1.ShowDialog() == DialogResult.Cancel) return;
                string nazvanie = saveFileDialog1.FileName;
                if (!nazvanie.Contains(".txt")) nazvanie += ".txt";
                File.WriteAllText(nazvanie, textBox3.Text);
                MessageBox.Show("Файл успешно сохранён!");
            }
            else MessageBox.Show("Пожалуйста, нажмите на опцию прогнозирования, нечего сохранять", "Ошибка сохранения", MessageBoxButtons.OK, MessageBoxIcon.Question);
        }

        private void теоретическиеСведенияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filename = "Л_9_Ряды динамики.pdf";
            new Process
            {
                StartInfo = new ProcessStartInfo(filename)
                {
                    UseShellExecute = true
                }
            }.Start();
        }

        private void инструкцияПоИспользованиюToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Инструкция:\n 1. Откройте файл с данными (Файл->Открыть данные); \n 2. Введите желаемую дату (Опции->Ввести дату); \n 3. Ознакомьтесь с появившимся текстом и графиками, перебирая для показа последних параметры, задаваемые выплывающими списками, расположенными в правом нижнем углу; \n 4. При желании сохраните полученные сведения (Файл->Сохранить вывод)", "Инструкция по использованию", MessageBoxButtons.OK, MessageBoxIcon.Question);
        }

        private void ввестиДатуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
               string data = Microsoft.VisualBasic.Interaction.InputBox("Пожалуйста, введите желаемую дату в формате число.месяц.год", "Окно для ввода даты", "", -1, -1);
               string[] numbers = data.Split('.');
                if (numbers.Length != 3) throw new Exception("Некорректная дата");
               for (int i = 0; i < 3; i++)
                {
                    rdata[i] = Int32.Parse(numbers[i]);
                }
                if ((rdata[0] > 31) || (rdata[0] < 1)) throw new Exception("Некорректная дата");
                if ((rdata[1] > 12) || (rdata[1] < 1)) throw new Exception("Некорректная дата");
                k = 0;
            }
            catch
            { 
                MessageBox.Show("Дата некорректна или не была введена", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Question);
            }
        }
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (k != 0)
            {
                chart1.Series["Series2"].Points.Clear();
                chart1.Series["Series5"].Points.Clear();

                string[] godiz = godi.Split(' ');
                double[] go = new double[godiz.Length - 1];
                for (int i = 0; i < godiz.Length - 1; i++) go[i] = Double.Parse(godiz[i]);
                chart1.ChartAreas[0].AxisX.Minimum = go[0];
                chart1.ChartAreas[0].AxisX.Maximum = go[go.Length - 1];

                string[] masz = mas.Split(' ');
                int uchastok2 = (int)comboBox3.SelectedItem;

                if (punkt == "График ряда данных")
                {
                    for (int i = 0; i < r; i++)
                    {
                        this.chart1.Series["Series5"].Points.AddXY(go[i], Double.Parse(masz[(uchastok2 - 1) * r + i]));
                        //textBox3.AppendText(masz[(uchastok - 1) * r + i] + "/");
                    }
                }
            }
            else MessageBox.Show("Пожалуйста, нажмите на опцию прогнозирования, нечего выводить", "Ошибка вывода", MessageBoxButtons.OK, MessageBoxIcon.Question);

        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (k != 0)
            {
                chart1.Series["Series2"].Points.Clear();
                chart1.Series["Series5"].Points.Clear();

                string[] godiz = godi.Split(' ');
                double[] go = new double[godiz.Length - 1];
                for (int i = 0; i < godiz.Length - 1; i++) go[i] = Double.Parse(godiz[i]);
                chart1.ChartAreas[0].AxisX.Minimum = go[0];
                chart1.ChartAreas[0].AxisX.Maximum = go[go.Length - 1];
                int uchastok = (int)comboBox2.SelectedIndex;
                int uchastokg = (int)comboBox2.SelectedItem;
                //double[,] massiv2 = { { 22.8, 24.9, 31.0, 29.5, 30.5, 35.6, 36.4, 42.6 }, { 42.6, 36.4, 35.6, 30.5, 29.5, 31.0, 24.9, 22.8 } };
                string[] masz = mas.Split(' ');

                if (punkt == "График линейной функции")
                {
                    string[] linz = lin.Split(' ');
                    double a = Double.Parse(linz[(uchastok) * 2]);
                    double b = Double.Parse(linz[(uchastok) * 2 + 1]);

                    for (int i = 0; i < r; i++)
                    {
                        double zn = b + a * (i + 1);
                        this.chart1.Series["Series2"].Points.AddXY(go[i], zn);
                        this.chart1.Series["Series5"].Points.AddXY(go[i], Double.Parse(masz[(uchastokg - 1) * r + i]));
                    }
                }
                if (punkt == "График показательной функции")
                {
                    try
                    {
                        string[] pokz = pok.Split(' ');
                        double a = Double.Parse(pokz[(uchastok) * 2]);
                        double b = Double.Parse(pokz[(uchastok) * 2 + 1]);

                        for (int i = 0; i < r; i++)
                        {
                            double zn = b * Math.Pow(a, i + 1);
                            this.chart1.Series["Series2"].Points.AddXY(go[i], zn);
                            this.chart1.Series["Series5"].Points.AddXY(go[i], Double.Parse(masz[(uchastokg - 1) * r + i]));
                        }
                    }
                    catch { MessageBox.Show("К сожалению, натуральный логарифм от 0 взять не получилось, поэтому функция не может быть построена. Пожалуйста, пробуйте другие виды", "Ошибка построения графика", MessageBoxButtons.OK, MessageBoxIcon.Question); }
                }
                if (punkt == "График параболы второго порядка")
                {
                    string[] parz = par.Split(' ');
                    double a = Double.Parse(parz[(uchastok) * 3]);
                    double b = Double.Parse(parz[(uchastok) * 3 + 1]);
                    double z = Double.Parse(parz[(uchastok) * 3 + 2]);

                    for (int i = 0; i < r; i++)
                    {
                        double zn = a + b * (i + 1) + z * (i + 1) * (i + 1);
                        this.chart1.Series["Series2"].Points.AddXY(go[i], zn);
                        this.chart1.Series["Series5"].Points.AddXY(go[i], Double.Parse(masz[(uchastokg - 1) * r + i]));
                    }
                }
                //if (punkt == "Все графики") { MessageBox.Show("5"); }
            }
            else MessageBox.Show("Пожалуйста, нажмите на опцию прогнозирования, нечего выводить", "Ошибка вывода", MessageBoxButtons.OK, MessageBoxIcon.Question);
        }
    }
}
