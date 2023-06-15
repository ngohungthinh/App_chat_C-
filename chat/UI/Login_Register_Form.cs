using chat.Class;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace chat.UI
{
    public partial class Login_Register_Form : BeautyForm
    {
        public Login_Register_Form()
        {
            InitializeComponent();
        }
        //--------
        IFirebaseConfig ifc = new FirebaseConfig()
        {
            AuthSecret = "bC1RA3io6cLdFGl92erWcp8puMQ8Ye3zdMK7Xygh",
            BasePath = "https://test-firebase-2806f-default-rtdb.asia-southeast1.firebasedatabase.app/"
        };
        IFirebaseClient client;
        private void Login_Register_Form_Load(object sender, EventArgs e)
        {
            try
            {
                client = new FireSharp.FirebaseClient(ifc);
            }

            catch
            {
                MessageBox.Show("No Internet or Connection Problem");
            }
        }
        //--------
        protected override void OnMouseDownMoveForm(object sender, MouseEventArgs e)
        {
            base.OnMouseDownMoveForm(sender, e);
        }

        Color select_color = Color.FromArgb(46, 49, 49);
        private void button_GotoLogin_Click(object sender, EventArgs e)
        {
            panel_Login.BringToFront();
            panel1.BackColor = Color.Yellow;
            panel_Register_Bar.BackColor = select_color;
            button_GotoLogin.BackColor = select_color;
            //panel_Login.BackColor = select_color;
            button_GoToRegister.BackColor = Color.Black;
        }

        private void button_GoToRegister_Click(object sender, EventArgs e)
        {
            panel_Register.BringToFront();
            panel1.BackColor = select_color;
            panel_Register_Bar.BackColor = Color.Yellow;
            button_GoToRegister.BackColor = select_color;
            //panel_Register.BackColor = select_color;
            button_GotoLogin.BackColor = Color.Black;
        }

        private void pictureBox_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FirebaseResponse res = client.Get(@"Users/" + textBox1.Text);
            MyUser ResUser = res.ResultAs<MyUser>();// database result

            MyUser CurUser = new MyUser() // USER GIVEN INFO
            {
                Username = textBox1.Text,
                Password = Security.ComputeSHA256Hash(textBox2.Text)
            };

            if (MyUser.IsEqual(ResUser, CurUser))
            {
                Form1 f = new Form1();
                f.MyUsername = textBox1.Text;
                this.Hide();
                f.ShowDialog();
                this.Close();
                MessageBox.Show("Login Success!");
            }
            else
            {
                MyUser.ShowError();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string ex = "MAT KHAU KHONG KHOP";
            if (textBox6.Text != textBox5.Text)
            {
                MessageBox.Show(ex);
            }
            else
            {
                MyUser user = new MyUser()
                {
                    Fullname = textBox7.Text + " " + textBox4.Text,
                    Username = textBox3.Text,
                    Password = Security.ComputeSHA256Hash(textBox5.Text),
                };

                SetResponse set = client.Set(@"Users/" + textBox3.Text, user);
                MessageBox.Show("Successfully registered!");
            }
        }
    }
}
