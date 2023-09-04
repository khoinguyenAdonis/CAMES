using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using ZedGraph;

namespace BLT_FORM
{
    public partial class Console : Form
    {
        public Console()
        {
            InitializeComponent();
        }
        string[] databaud = { "1200", "2400", "4800", "9600", "19200", "115200" };

        private void Console_Load(object sender, EventArgs e)
        {
            string [] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                com.Items.Add(port);
            }
            baud.Items.AddRange(databaud);
            timer1.Start();
            daytime();
            bieudo();
        }
        #region button connect disconnect exit
        private void connect_Click(object sender, EventArgs e)
        {
            connect1();
        }

        private void connect1()
        {
            if (com.Text == "")
            {
                MessageBox.Show("Vui lòng chọn cổng COM","Thông báo",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            else if (baud.Text == "")
            {
                MessageBox.Show("Vui lòng chọn tốc độ Baud", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (serialPort1.IsOpen == true)
            {
                serialPort1.Close();
            }
            if (serialPort1.IsOpen == false)
            {
                try
                {
                    serialPort1.PortName = com.Text;
                    serialPort1.BaudRate = int.Parse(baud.Text);
                    serialPort1.Open();
                    status.Text = "Connected";
                    status.ForeColor = Color.GreenYellow;
                    connect.Enabled = false;
                    disconnect.Enabled = true;
                    baud.Enabled = false;   
                    com.Enabled = false;
                    increase.Enabled = true;
                    decrease.Enabled = true;
                    backward.Enabled = true;
                }
                catch (Exception)
                {
                    MessageBox.Show("Cổng COM không khả dụng", "Warring", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void disconnect_Click(object sender, EventArgs e)
        {
            disconnect1();
        }

        private void disconnect1()
        {
            serialPort1.Close();
            status.Text = "Disconnected";
            status.ForeColor= Color.DarkRed;
            disconnect.Enabled= false;
            baud.Enabled= true;
            com.Enabled = true;
            connect.Enabled= true;
            increase.Enabled = false;
            decrease.Enabled = false;
            stop.Enabled = false;
            backward.Enabled = false;
            vantoc.Text = "";
            rpm.Text = "";
        }

        private void exit_Click(object sender, EventArgs e)
        {
            DialogResult dr;
            dr = MessageBox.Show("Bạn có muốn thoát chương trình","Thông báo",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                Application.Exit();
            }
            else
            {

            }
        }
        #endregion
        #region đồ thị  
        private void bieudo()
        {
            GraphPane mypanel = zedGraphControl1.GraphPane;
            mypanel.Title.Text = "Vận tốc theo thời gian";
            mypanel.YAxis.Title.Text = "Vận tốc";
            mypanel.XAxis.Title.Text = "Thời gian";
            RollingPointPairList List1 = new RollingPointPairList(60000);
            LineItem line1 = mypanel.AddCurve("Vận tốc",List1,Color.Blue,SymbolType.None);

            

            zedGraphControl1.AxisChange();

        }
        int sum = 0;
        public void draw (double line1)
        {
            LineItem duongline = zedGraphControl1.GraphPane.CurveList[0] as LineItem;
            if (duongline == null) return;
            IPointListEdit list1 = duongline.Points as IPointListEdit;
            if (list1 == null) return;

            list1.Add(sum, line1);
            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
            sum += 1;

        }
        #endregion
        private void timer1_Tick(object sender, EventArgs e)
        {
            daytime();
        }


        private void daytime()
        {
            day.Text = DateTime.Now.ToString("dddd  dd/MM/yyyy");
            time.Text = DateTime.Now.ToString("HH:mm:ss");
        }
        #region button control
        private void button1_Click(object sender, EventArgs e)
        {
            if (rotationdirection.Text == "Forward") serialPort1.WriteLine("\nt");
            if (rotationdirection.Text == "Backward") serialPort1.WriteLine("\ni");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (rotationdirection.Text == "Forward") serialPort1.WriteLine("\ng");
            if (rotationdirection.Text == "Backward") serialPort1.WriteLine("\nd");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            serialPort1.WriteLine("\ns");
            rotationdirection.Text = "";
            value.Enabled = false;
            tgtd.Enabled = false;
            dcq.Enabled = false;
            stop.Enabled = false;
        }

        private void sendvalute_Click(object sender, EventArgs e)
        {
            
            if (Convert.ToDouble(textvalue.Text) > 255 & Convert.ToDouble(textvalue.Text) <0 ) 
            { MessageBox.Show("Giá trị Không đúng\n >0 & <=255", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            if (rotationdirection.Text == "Backward") { serialPort1.Write("\nsl"+textvalue.Text); }
            if (rotationdirection.Text == "Forward") { serialPort1.Write("\nst" + textvalue.Text); }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            serialPort1.WriteLine("\nb");
            rotationdirection.Text = "Backward";
            rotationdirection.ForeColor = Color.GreenYellow;
            forw.Enabled = true;
            backward.Enabled = false;
        }
        private void button2_Click_1(object sender, EventArgs e)
        {
            serialPort1.WriteLine("\nc");
            tgtd.Enabled = true;
            dcq.Enabled = true;
            value.Enabled = true;
            rotationdirection.Text = "Forward";
            rotationdirection.ForeColor= Color.GreenYellow;
            stop.Enabled = true;
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            serialPort1.WriteLine("\na");
            rotationdirection.Text = "Forward";
            rotationdirection.ForeColor= Color.GreenYellow;
            backward.Enabled = true;
            forw.Enabled = false;
        }
        #endregion
        string data2 = "";
        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (serialPort1.IsOpen == true)
            {
               try
                {
                    string data = serialPort1.ReadLine();
                    double data1 = Convert.ToDouble(data);
                    data2 += Convert.ToString((data1 / 60) * 0.6 * 3.14);
                    Invoke(new MethodInvoker(() => rpm.Text = data));
                    Invoke(new MethodInvoker(() => vantoc.Text = Convert.ToString((data1 / 60) * 0.6 * 3.14) ));
                    if (data2.Length > 2 )
                    {
                        Invoke(new MethodInvoker(() => draw((data1 /60) * 0.6 * 3.14)));
                        data2 = "";
                    }
                   
                }
                catch (Exception)
                {

                }
            }
        }

    }
}
