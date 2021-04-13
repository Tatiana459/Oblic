using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;


namespace Oblic_Tovariv
{
    public partial class Form1 : Form
    {
        string connectString = @"Data Source=.\SQLEXPRESS;
Initial Catalog=Облік руху товарів;Integrated Security=True";

        public Form1()
        {
            InitializeComponent();
        }

        private void вихідToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Вийти з програми?", " ", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            { this.Close(); }
        }

        public void Zapit(string Sql_Z)
        {
            try
            {
                string sql = Sql_Z;
                using (SqlConnection connection = new SqlConnection(connectString))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(sql, connection);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    dataGridView1.DataSource = ds.Tables[0];
                    connection.Close();
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        private void зберегтиToolStripMenuItem_Click(object sender, EventArgs e)
        { DialogResult result = MessageBox.Show("Зберегти дані в файл?", " ", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            { SaveFileDialog savefile = new SaveFileDialog();
                savefile.DefaultExt = ".txt";
                savefile.Filter = "test files|*.txt";
                if (savefile.ShowDialog() == System.Windows.Forms.DialogResult.OK && savefile.FileName.Length > 0)
                {
                    using (StreamWriter sw = new StreamWriter(savefile.FileName, true))
                    {
                        try
                        {
                            for (int j = 0; j < dataGridView1.Rows.Count; j++)
                            {
                                for (int i = 0; i < dataGridView1.Rows[j].Cells.Count; i++)
                                { sw.Write(dataGridView1.Rows[j].Cells[i].Value + " "); }
                                sw.WriteLine();
                            }
                            sw.Close();
                            MessageBox.Show("Файл успішно збережений");
                        }
                        catch
                        {
                            MessageBox.Show("Помилка при збережені файла!");
                            sw.Close();
                        }
                    }
                }

            }

        }

        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                string sel = comboBox1.SelectedItem.ToString();
                if (sel == "Групи")
                { Zapit("SELECT* from Групи"); }
                else if (sel == "Товари")
                { Zapit("SELECT* from Товари"); }
                else if (sel == "Постачальники")
                { Zapit("SELECT* from Постачальники"); }
                else if (sel == "Покупці")
                { Zapit("SELECT* from Покупці"); }
                else if (sel == "Закупівлі")
                { Zapit("SELECT* from Закупівлі"); }
                else if (sel == "Продажі")
                { Zapit("SELECT* from Продажі"); }
                else { MessageBox.Show("Невірний вибір"); }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int index = dataGridView1.CurrentRow.Index;
            if (index != 0)
            {
                dataGridView1.Rows[index].Selected = true;
                dataGridView1.CurrentCell = dataGridView1[0, 0];
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int index = dataGridView1.CurrentRow.Index;
           if( index!= dataGridView1.Rows.Count)
                {
                dataGridView1.Rows[index].Selected = true;
                dataGridView1.CurrentCell = dataGridView1[0, dataGridView1.Rows.Count-1];
                }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                richTextBox1.Text = "Select Г.Id_Gr as Група, Г.Name_Gr as Назва, З.Kolichestvo from Групи as Г join Закупівлі as З" +
                       " on З.Id_Gr=Г.Id_Gr join Товари as Т on Т.Id_Tov=З.Id_Tov " + richTextBox2.Text + " group by Г.Id_Gr,Г.Name_Gr,З.Kolichestvo";
                Zapit(richTextBox1.Text);
                comboBox1.Text = "Групи";
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }


        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                richTextBox1.Text = "Select П.FIO as Постачальник, sum(З.Price_Z) as Сума from Постачальники as П join Закупівлі as З" +
                       " on З.Id_Post=П.Id_Post " + richTextBox2.Text + " group by П.FIO";
                Zapit(richTextBox1.Text);
                comboBox1.Text = "Постачальники";
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                richTextBox1.Text = "Select П.FIO as Постачальник, З.Data_Zak as Дата_закупки, sum(З.Price_Z*З.Kolichestvo) as Сума from Постачальники as П join Закупівлі as З" +
                       " on З.Id_Post=П.Id_Post " + richTextBox2.Text + " group by П.FIO, З.Data_Zak";
                Zapit(richTextBox1.Text);
                comboBox1.Text = "Закупівлі";
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                richTextBox1.Text = "Select П.FIO_Pok as Покупець, (sum(Пр.Price_Pr)*Пр.Kolichestvo_Pr) " +
       "as Сума_Продажі from Покупці as П join Продажі as Пр on (Пр.Id_Pok=П.Id_Pok and Data_Pr between \'16.09.2019\' and \'1.11.2019\' ) "
          + richTextBox2.Text + " group by П.FIO_Pok, Пр.Kolichestvo_Pr";
                Zapit(richTextBox1.Text);
                comboBox1.Text = "Продажі";
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                richTextBox1.Text = "Select П.FIO_Pok as Покупець, max(Пр.Price_Pr*Пр.Kolichestvo_Pr) " +
       "as Сума_Продажі from Покупці as П join Продажі as Пр on (Пр.Id_Pok=П.Id_Pok and Data_Pr between \'16.09.2019\' and \'1.11.2019\' ) "
          + richTextBox2.Text + " group by П.FIO_Pok";
                Zapit(richTextBox1.Text);
                comboBox1.Text = "Продажі";
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Zapit(richTextBox1.Text);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
        }
    }
    }

 
 

 