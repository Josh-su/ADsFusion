using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ADsFusion
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void Settings_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Create a string array to store the textbox values
            string[] server1Values = {
                textBox1.Text, textBox2.Text, textBox3.Text, textBox4.Text,
                textBox5.Text, textBox6.Text, textBox7.Text
            };

            string[] server2Values = {
                textBox8.Text, textBox9.Text, textBox10.Text, textBox11.Text,
                textBox12.Text, textBox13.Text, textBox14.Text
            };
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            checkBox4.Checked = radioButton1.Checked;
            checkBox11.Checked = radioButton1.Checked;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            checkBox1.Checked = radioButton2.Checked;
            checkBox14.Checked = radioButton2.Checked;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
