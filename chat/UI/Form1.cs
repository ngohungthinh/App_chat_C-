using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Shaping;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Reflection;
using chat.UI;
using System.Threading;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using FireSharp;
using Newtonsoft.Json;
using chat.Class;
using System.Threading.Tasks;

namespace chat
{
    public partial class Form1 : BeautyForm //Inherited from Beauty, which a custom form.
    {
        public Form1()
        {
            InitializeComponent();
        }
        public string MyUsername { get; set; }
        Users[] List_user;
        int so_tin = 0;
        bool canh = true;
        string id;
        int slluong = 0;
        //--------------------------------------------------------------------------------------
        IFirebaseConfig ifc = new FirebaseConfig()
        {
            AuthSecret = "bC1RA3io6cLdFGl92erWcp8puMQ8Ye3zdMK7Xygh",
            BasePath = "https://test-firebase-2806f-default-rtdb.asia-southeast1.firebasedatabase.app/"
        };
        IFirebaseClient client;
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                client = new FirebaseClient(ifc);
            }
            catch
            {
                MessageBox.Show("Error Internet!");
            }
            //------
            
            textBox_showName.Text = MyUsername;
            this.chatHeader1.UserTitle = "";

            List_user = new Users[11] {users1,users2, users3,users4, users5, users6,
                users7, users8, users9, users10,users11 };
            foreach (Users u in List_user)
            {
                u.Hide();
            }
            //-------
            //clone tất cả về
            FirebaseResponse res = client.Get(@"Users");
            Dictionary<string, MyUser> data = JsonConvert.DeserializeObject<Dictionary<string, MyUser>>(res.Body.ToString());
            //
            int i = 0;
            foreach (var item in data)
            {
                if (item.Key == MyUsername) continue;
                List_user[i%11].Username = item.Value.Username;
                List_user[i%11].Show();
                i++;
            }    
            
        }//-------------------------------------------------------------------------------------
        //Move Form with the mouse. This method is available in BeautyForm that this form inherits
        protected override void OnMouseDownMoveForm(object sender, MouseEventArgs e)
        {
            base.OnMouseDownMoveForm(sender, e);
        }

        private void label1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void typingBox1_OnTypingTextChanged(string newVal)
        {
            // When you type something this fires... From 'typingBox11.OnTypingTextChanged'
            //'NewVal' is the new value... also can be gotten from 'typingBox1.Value'

        }

        private void typingBox1_OnHitEnter(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(typingBox1.Value))
            {
                //-------------------------
                Mess mes = new Mess()
                {
                    User = MyUsername,
                    Content = typingBox1.Value,
                };

                
                string order = "1";
                order += (char)(so_tin + 65);
                var setter = client.Set("UserChat/" + id + "/" + order , mes);

                so_tin = (so_tin + 1) % 10;
                SoTin s = new SoTin()
                { Number = so_tin };
                
                var set_so_tin = client.Set("SoTin/" + id,s);
                
                //=-------------------------
                MeBubble bubble = new MeBubble();
                bubble.Dock = DockStyle.Bottom;//Dock to bottom  so that the bubbles can align themselves in a horizontal grid. You dont have to worry about responsiveness when window resizes.
                bubble.SendToBack();//Send back so that it will be lowest control... Use bubble.BringToFront() if u r docking up.
               
                bubble.Body = typingBox1.Value;

                panel4.Controls.Add(bubble);

                typingBox1.Value = "";
                //FakeRecieving()

            }
        }

        private void FakeRecieving()
        {
            YouBubble bubble = new YouBubble();
            bubble.Dock = DockStyle.Bottom;
            bubble.SendToBack();
            bubble.Body = "This is a message received.";
            panel4.Controls.Add(bubble);
        }
        
        private void OnUserClick(object sender, EventArgs e)
        {
            panel4.Controls.Clear();

            var ClickedUser = ((Users)sender);

            string name = ClickedUser.Username;
            string statusText = ClickedUser.StatusMessage;
            Image profileImage = ClickedUser.UserImage;

            this.chatHeader1.UserTitle = name;
            this.chatHeader1.UserStatusText = statusText;
            this.chatHeader1.UserImage = profileImage;

            id = ID_chat.Xor(MyUsername,name);
            //---------
            FirebaseResponse get_so_tin = client.Get("SoTin/" + id);
            SoTin so_tinn = get_so_tin.ResultAs<SoTin>();
            
            if (so_tinn == null)
            {
                so_tin = 0;
            }
            else
            {
                so_tin = so_tinn.Number;
                FirebaseResponse res = client.Get("UserChat/" + id);
                Dictionary<string, Mess> data = JsonConvert.DeserializeObject<Dictionary<string, Mess>>(res.Body.ToString());

                //--------------------------------
                //update panel4
                panel4.Controls.Clear();
                int length = data.Count;
                int dem = 0;
                int i = so_tin % length;
                while (dem < length)
                {
                    if (data.ElementAt(i).Value.User == MyUsername)
                    {
                        MeBubble bubble = new MeBubble();
                        bubble.Dock = DockStyle.Bottom;
                        bubble.SendToBack();
                        bubble.Body = data.ElementAt(i).Value.Content;
                        panel4.Controls.Add(bubble);
                    }
                    else
                    {
                        YouBubble bubble = new YouBubble();
                        bubble.Dock = DockStyle.Bottom;
                        bubble.SendToBack();
                        bubble.Body = data.ElementAt(i).Value.Content;
                        panel4.Controls.Add(bubble);
                    }
                    i = (i + 1) % length;
                    dem++;
                }

                slluong++;
                LiveCall(slluong);
            }  
            //----------
        }
        async Task LiveCall(int index)
        {
            //await Task.Delay(2000); // delay để luồng live call cũ thức dậy thấy được canh == false.
            //canh = true;
            while (true)
            {
                
                await Task.Delay(1000);
                if (index != slluong) { break; }
                //---------
                FirebaseResponse get_so_tin = client.Get("SoTin/" + id);
                SoTin so_tinn = get_so_tin.ResultAs<SoTin>();
                
                if( so_tinn == null || so_tin == so_tinn.Number)
                {
                    
                }  
                else
                {
                    so_tin = so_tinn.Number;
                    string order = "1";
                    if (so_tin == 0) order += (char)(9 + 65);
                    else order += (char)(so_tin - 1 + 65); 

                    var get = client.Get("UserChat/" + id + "/" + order);
                    Mess mess = get.ResultAs<Mess>();

                    //---------------------
                    YouBubble bubble = new YouBubble();
                    bubble.Dock = DockStyle.Bottom;
                    bubble.SendToBack();
                    bubble.Body = mess.Content;
                    panel4.Controls.Add(bubble);
                }    
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Login_Register_Form formLG = new Login_Register_Form();
            formLG.ShowDialog();
            this.Close();
        }


        //-----------------
        

        private void searchBox1_OnHitEnter(object sender, EventArgs e)
        {
            FirebaseResponse res = client.Get("Users/"+searchBox1.Value);
            MyUser usn = res.ResultAs<MyUser>();
            if (usn != null)
            {
                foreach (Users u in List_user)
                {
                    u.Hide();
                }
                users1.Username = usn.Username;
                users1.Show();
            }
            else
            {
                MessageBox.Show("Không có username nào phù hợp!");
            }
        }
    }
}
