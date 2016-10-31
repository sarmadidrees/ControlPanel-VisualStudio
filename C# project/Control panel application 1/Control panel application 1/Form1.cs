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



namespace Control_panel_application_1
{
    public partial class Form1 : Form
    {
        private bool debug = false;
        private bool pressed = false;

        int mapX = 15;
        int mapY = 6;

        //initialize MAP (1=walkable, 0=wall)
        public bool[,] map = new bool[15, 6]{{true,true,false,true,true,true},
                          {true,true,true,true,true,true},
                          {true,true,false,true,true,true},
                          {true,true,false,false,false,true},
                          {true,true,true,true,true,true},
                          {true,false,false,false,true,true},
                          {true,true,true,false,true,true},
                          {true,true,true,false,true,true},
                          {true,true,true,false,true,true},
                          {false,true,false,false,true,true},
                          {true,true,true,true,true,true},
                          {true,true,false,false,false,true},
                          {true,true,false,true,true,true},
                          {true,true,true,true,true,true},
                          {true,true,false,true,true,true}
                         };

        public struct NODE
        {
            public bool isWalkable;
            public bool onOpen;
            public bool onClosed;
            public int parentX;
            public int parentY;
            public int G;
            public int H;
            public int F;
        }
        NODE[,] node = new NODE[15, 6];

        public int count = 0;

        public struct PATH
        {
            public int X;
            public int Y;
            public int F;
        }
        PATH[] path = new PATH[100];

        public struct newPATH
        {
            public int X;
            public int Y;
            public int F;
        }
        newPATH[] finalPath = new newPATH[100];

        public Form1()
        {
            InitializeComponent();
            Form1.CheckForIllegalCrossThreadCalls = false;
            Form1.CheckForIllegalCrossThreadCalls = false;
            Form1.CheckForIllegalCrossThreadCalls = false;

        }
        void getAvailablePorts()
        {
            serialPort1.Close();
            comboBox1.Items.Clear();
            string[] ports = SerialPort.GetPortNames();
            comboBox1.Items.AddRange(ports);
            button1.Enabled = true;
            //button2.Enabled = false;
            //button3.Enabled = false;
            // button4.Enabled = true;
            //button5.Enabled = false;
            local_btn.Enabled = false;
            path_btn.Enabled = true;
            pidL_Apply_btn.Enabled = false;
            readEnc_btn.Enabled = false;
            resetEnc_btn.Enabled = false;
            pidR_Apply_btn.Enabled = false;
            //button11.Enabled = false;
            button14.Enabled = false;
            button15.Enabled = false;
            button1.Text = "Connect";
            //textBox4.Text = Convert.ToString(vScrollBar1.Value);
            //textBox3.Text = Convert.ToString(vScrollBar2.Value);
            //textBox6.Text = Convert.ToString(vScrollBar4.Value);
            //textBox5.Text = Convert.ToString(vScrollBar3.Value);
            textBox7.Text = Convert.ToString(vScrollBar5.Value);
            textBox8.Text = Convert.ToString(vScrollBar6.Value);
            textBox9.Text = Convert.ToString(vScrollBar7.Value);
            textBox10.Text = Convert.ToString(vScrollBar8.Value);

            textBox11.Text = Convert.ToString(hScrollBar4.Value);
            textBox5.Text = Convert.ToString(hScrollBar2.Value);
            textBox6.Text = Convert.ToString(hScrollBar1.Value);
            textBoxRL.Text = Convert.ToString(hScrollBar3.Value);

            textBox13.Text = Convert.ToString(hScrollBar5.Value);
            textBox14.Text = Convert.ToString(hScrollBar6.Value);
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            getAvailablePorts();
        }

        private void button17_Click(object sender, EventArgs e)     // Refresh button
        {
            getAvailablePorts();
            scatterGraph1.ClearData();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.Text == "")
                {
                    mainTextBox.Text = "Select correct ComPort and BaudRate!";

                }
                else if (comboBox2.Text == "")
                {

                }
                else
                {
                    if (button1.Text == "Connect")
                    {
                        serialPort1.PortName = comboBox1.Text;
                        serialPort1.BaudRate = Convert.ToInt32(comboBox2.Text);
                        serialPort1.Open();
                        button1.Enabled = true;
                        // button2.Enabled = true;
                        // button3.Enabled = true;
                        // button4.Enabled = true;
                        // button5.Enabled = true;
                        local_btn.Enabled = true;
                        path_btn.Enabled = true;
                        pidL_Apply_btn.Enabled = true;
                        readEnc_btn.Enabled = true;
                        resetEnc_btn.Enabled = true;
                        pidR_Apply_btn.Enabled = true;
                        // button11.Enabled = true;
                        button14.Enabled = true;
                        button15.Enabled = true;
                        button1.Text = "Disconnect";
                        mainTextBox.Text = "";
                        serialPort1.NewLine = "\n";
                    }
                    else
                    {
                        getAvailablePorts();
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                mainTextBox.Text = "Unauthorized Access Exception";
            }
        }

        private void button7_Click(object sender, EventArgs e) // sending PID parameters for left motor
        {
            serialPort1.DiscardOutBuffer(); //clear the TX line
            serialPrint("M,"+"P," + Convert.ToString(numericUpDown1.Value) + "," + Convert.ToString(numericUpDown2.Value) + "," + Convert.ToString(numericUpDown3.Value) + ",1");  //H,kp,ki,kd,1/2
            if (debug)
            {
                mainTextBox.Text = serialPort1.ReadLine();
            }
        }

        private void button10_Click(object sender, EventArgs e)// sending PID parameters for Right motor
        {
            serialPort1.DiscardOutBuffer(); //clear the TX line
            serialPrint("M,"+"P," + Convert.ToString(numericUpDown6.Value) + "," + Convert.ToString(numericUpDown5.Value) + "," + Convert.ToString(numericUpDown4.Value) + ",2");  //H,kp,ki,kd,1/2
            if (debug)
            {
                mainTextBox.Text = serialPort1.ReadLine();
            }
        }

        private void button15_Click(object sender, EventArgs e)  //Driving through Pwm
        {
            serialPort1.DiscardOutBuffer(); //clear the TX line
            serialPrint("M,"+"L," + Convert.ToString(vScrollBar8.Value) + "," + Convert.ToString(vScrollBar7.Value)); //L,Left Right Motor
            if (debug)
            {
                mainTextBox.Text = serialPort1.ReadLine();
            }
        }

        private void button14_Click(object sender, EventArgs e) //Drive through mm/s
        {
            serialPort1.DiscardOutBuffer(); //clear the TX line
            serialPrint("M,"+"V," + Convert.ToString(vScrollBar6.Value) + "," + Convert.ToString(vScrollBar5.Value)); //D,Left Right Motor
            if (debug)
            {
                mainTextBox.Text = serialPort1.ReadLine();
            }
        }

        private void vScrollBar8_Scroll(object sender, ScrollEventArgs e)
        {
            textBox10.Text = Convert.ToString(vScrollBar8.Value);
        }

        private void vScrollBar7_Scroll(object sender, ScrollEventArgs e)
        {
            textBox9.Text = Convert.ToString(vScrollBar7.Value);
        }

        private void vScrollBar6_Scroll(object sender, ScrollEventArgs e)
        {
            textBox8.Text = Convert.ToString(vScrollBar6.Value);
        }

        private void vScrollBar5_Scroll(object sender, ScrollEventArgs e)
        {
            textBox7.Text = Convert.ToString(vScrollBar5.Value);
        }
        private void serialPrint(String text)
        {
            mainTextBox.AppendText(">>" + text + "\n");
            serialPort1.WriteLine(text);
        }
        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            String dataRead = serialPort1.ReadLine();
            mainTextBox.Invoke(new EventHandler(delegate
            {
                mainTextBox.AppendText("--" + dataRead + "\n");
            }));
            int a, b, c, d;
            String s1 = "", s2 = "", s3 = "", s4 = "";

            if (dataRead.StartsWith("s"))
            {

                a = dataRead.IndexOf(",");
                b = dataRead.IndexOf(",", a + 1);
                c = dataRead.IndexOf(",", b + 1);
                d = dataRead.IndexOf(",", c + 1);
                s1 = dataRead.Substring(a + 1, b - a - 1);
                s2 = dataRead.Substring(b + 1, c - b - 1);
                s3 = dataRead.Substring(c + 1, d - c - 1);
                s4 = dataRead.Substring(d + 1);
                if (s4.StartsWith("1"))
                {
                    numericUpDown1.Invoke(new EventHandler(delegate
                    {
                        numericUpDown1.Value = Convert.ToDecimal(s1);
                    }));
                    numericUpDown2.Invoke(new EventHandler(delegate
                    {
                        numericUpDown2.Value = Convert.ToDecimal(s2);
                    }));
                    numericUpDown3.Invoke(new EventHandler(delegate
                    {
                        numericUpDown3.Value = Convert.ToDecimal(s3);
                    }));
                }
                else if (s4.StartsWith("2"))
                {
                    numericUpDown6.Invoke(new EventHandler(delegate
                    {
                        numericUpDown6.Value = Convert.ToDecimal(s1);
                    }));
                    numericUpDown5.Invoke(new EventHandler(delegate
                    {
                        numericUpDown5.Value = Convert.ToDecimal(s2);
                    }));
                    numericUpDown4.Invoke(new EventHandler(delegate
                    {
                        numericUpDown4.Value = Convert.ToDecimal(s3);
                    }));
                }
            }

            else if (dataRead.StartsWith("S"))
            {
                if (dataRead.Length > 4)
                {
                    //string[] inputSplit = dataRead.Split(',');
                    //textBox1.Text = dataRead[2].ToString();
                    //textBox2.Text = dataRead[4].ToString();
                    mainTextBox.Text = dataRead;
                }
            }

            /* else if (dataRead.StartsWith("r"))
             {
                 a = dataRead.IndexOf(",");
                 b = dataRead.IndexOf(",", a + 1);
                 s1 = dataRead.Substring(a + 1, b - a - 1);
                 s2 = dataRead.Substring(b + 1);
                 textBox1.Invoke(new EventHandler(delegate
                 {
                     textBox1.Text = s1;
                 }));
                 textBox2.Invoke(new EventHandler(delegate
                 {
                     textBox2.Text = s2;
                 }));
             }
             else if (dataRead.StartsWith("acc"))
             {

                 a = dataRead.IndexOf(",");
                 b = dataRead.IndexOf(",", a + 1);
                 c = dataRead.IndexOf(",", b + 1);
                 s1 = dataRead.Substring(a + 1, b - a - 1);
                 s2 = dataRead.Substring(b + 1, c - b - 1);
                 s3 = dataRead.Substring(c + 1);
                 textBox11.Invoke(new EventHandler(delegate
                 {
                     textBox11.Text = s1;
                 }));
                 textBox14.Invoke(new EventHandler(delegate
                 {
                     textBox14.Text = s2;
                 }));
                 textBox15.Invoke(new EventHandler(delegate
                 {
                     textBox15.Text = s3;
                 }));
             }
             else if (dataRead.StartsWith("gyro"))
             {

                 a = dataRead.IndexOf(",");
                 b = dataRead.IndexOf(",", a + 1);
                 c = dataRead.IndexOf(",", b + 1);
                 s1 = dataRead.Substring(a + 1, b - a - 1);
                 s2 = dataRead.Substring(b + 1, c - b - 1);
                 s3 = dataRead.Substring(c + 1);
                 textBox18.Invoke(new EventHandler(delegate
                 {
                     textBox18.Text = s1;
                 }));
                 textBox17.Invoke(new EventHandler(delegate
                 {
                     textBox17.Text = s2;
                 }));
                 textBox16.Invoke(new EventHandler(delegate
                 {
                     textBox16.Text = s3;
                 }));
             }
             else if (dataRead.StartsWith("mag"))
             {

                 a = dataRead.IndexOf(",");
                 b = dataRead.IndexOf(",", a + 1);
                 c = dataRead.IndexOf(",", b + 1);
                 s1 = dataRead.Substring(a + 1, b - a - 1);
                 s2 = dataRead.Substring(b + 1, c - b - 1);
                 s3 = dataRead.Substring(c + 1);
                 textBox19.Invoke(new EventHandler(delegate
                 {
                     textBox19.Text = s1;
                 }));
                 textBox20.Invoke(new EventHandler(delegate
                 {
                     textBox20.Text = s2;
                 }));
                 textBox21.Invoke(new EventHandler(delegate
                 {
                     textBox21.Text = s3;
                 }));
             }
             else if (dataRead.StartsWith("temp"))
             {
                 a = dataRead.IndexOf(",");
                 s1 = dataRead.Substring(a + 1);
                 textBox22.Invoke(new EventHandler(delegate
                 {
                     textBox22.Text = s1;
                 }));
             }
             * */
            serialPort1.DiscardInBuffer(); //clear the RX line

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (pressed)
            { return; }
            if (serialPort1.IsOpen)
            {
                int correctKey = 0;
                int pwm = 125;
                int offset = 0;
                switch (e.KeyCode)
                {
                    case Keys.Up:
                        vScrollBar8.Value = pwm;
                        vScrollBar7.Value = pwm - offset;
                        correctKey = 1;
                        break;
                    case Keys.Down:
                        vScrollBar8.Value = -pwm;
                        vScrollBar7.Value = -(pwm - offset);
                        correctKey = 1;
                        break;
                    case Keys.Left:
                        vScrollBar8.Value = -pwm;
                        vScrollBar7.Value = pwm - offset;
                        correctKey = 1;
                        break;
                    case Keys.Right:
                        vScrollBar8.Value = pwm;
                        vScrollBar7.Value = -(pwm - offset);
                        correctKey = 1;
                        break;
                }
                if (correctKey == 1)
                    button15_Click(sender, e);
            }
            pressed = true;

        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                switch (e.KeyCode)
                {
                    case Keys.Up:
                    case Keys.Down:
                    case Keys.Left:
                    case Keys.Right:
                        vScrollBar8.Value = 0;
                        vScrollBar7.Value = 0;
                        button15_Click(sender, e);
                        break;
                }
            }
            pressed = false;

        }

        private void local_btn_Click(object sender, EventArgs e)
        {
            serialPort1.DiscardOutBuffer();
            if (serialPort1.IsOpen) serialPrint("SONAR," + "L");
        }

        private void path_btn_Click(object sender, EventArgs e)
        {
            scatterGraph1.ClearData();
            try
            {
                //initialize nodes with all values ZERO
                for (int x = 0; x < mapX; x++)
                {
                    for (int y = 0; y < mapY; y++)
                    {
                        node[x, y].isWalkable = true;
                        node[x, y].onOpen = false;
                        node[x, y].onClosed = false;
                        node[x, y].G = 0;
                        node[x, y].H = 0;
                        node[x, y].F = 0;
                        node[x, y].parentX = 0;
                        node[x, y].parentY = 0;
                    }
                }
                //end initialization of nodes

                //find isWalkable for every node
                for (int x = 0; x < mapX; x++)
                {
                    for (int y = 0; y < mapY; y++)
                    {
                        if (map[x, y] == false) node[x, y].isWalkable = false;
                    }
                }
                //end finding isWalkable for every node

                //set starting point
                int startX = int.Parse(textBox2.Text);
                int startY = int.Parse(textBox1.Text);

                if (startY >= 2)
                {
                    startY += 1;
                }

                if (startX == 3) startX += 1;
                else if ((startX >= 4) && (startX <= 6)) startX += 2;
                else if (startX == 7) startX += 3;
                else if ((startX >= 8) && (startX <= 10)) startX += 4;


                //set ending point
                int endX = int.Parse(textBox3.Text);
                int endY = int.Parse(textBox4.Text);

                if (endX == 3) endX += 1;
                else if ((endX >= 4) && (endX <= 6)) endX += 2;
                else if (endX == 7) endX += 3;
                else if ((endX >= 8) && (endX <= 10)) endX += 4;

                if (endY >= 2)
                {
                    endY += 1;
                }

                node[startX, startY].onClosed = true;      //make starting point as closed

                int parentG = node[startX, startY].G;

                int currentX = startX;
                int currentY = startY;

                //start finding path
                //run path finding until it reaches end point
                while ((currentX != endX) || (currentY != endY))
                {
                    //first find adjacent nodes wrt to current node
                    //adjacent nodes in horizontal
                    for (int i = -1; i < 2; i += 2)
                    {
                        if ((currentY + i >= 0) && (currentY + i < mapY))
                        {
                            if ((node[currentX, currentY + i].isWalkable) && !(node[currentX, currentY + i].onClosed))
                            {
                                if (!(node[currentX, currentY + i].onOpen))
                                {
                                    node[currentX, currentY + i].parentX = currentX;
                                    node[currentX, currentY + i].parentY = currentY;
                                    node[currentX, currentY + i].G = parentG + 1;        //TODO: make parentG = node[currentX,currentY].G;
                                    node[currentX, currentY + i].H = Math.Abs(currentX - endX) + Math.Abs(currentY - endY);
                                    node[currentX, currentY + i].F = node[currentX, currentY + i].G + node[currentX, currentY + i].H;   //MANHATHAN method
                                    node[currentX, currentY + i].onOpen = true;
                                }
                            }
                        }
                    }

                    //adjacent nodes in vertical
                    for (int i = -1; i < 2; i += 2)
                    {
                        if ((currentX + i >= 0) && (currentX + i < mapX))
                        {
                            if ((node[currentX + i, currentY].isWalkable) && !(node[currentX + i, currentY].onClosed))
                            {

                                if (!(node[currentX + i, currentY].onOpen))
                                {
                                    node[currentX + i, currentY].parentX = currentX;
                                    node[currentX + i, currentY].parentY = currentY;
                                    node[currentX + i, currentY].G = parentG + 1;        //TODO: make parentG = node[currentX,currentY].G;
                                    node[currentX + i, currentY].H = Math.Abs(currentX - endX) + Math.Abs(currentY - endY);
                                    node[currentX + i, currentY].F = node[currentX + i, currentY].G + node[currentX + i, currentY].H;   //MANHATHAN method
                                    node[currentX + i, currentY].onOpen = true;
                                }
                            }
                        }
                    }

                    int lowestF = 100;
                    int lowestG = 100;

                    //check for lowest F in open list nodes
                    //and if more than 2 have same F than decide on its G value
                    for (int x = 0; x < mapX; x++)
                    {
                        for (int y = 0; y < mapY; y++)
                        {
                            if ((node[x, y].onOpen) && !(node[x, y].onClosed))
                            {
                                if (node[x, y].F < lowestF)
                                {
                                    currentX = x;
                                    currentY = y;
                                    lowestF = node[x, y].F;
                                    lowestG = node[x, y].G;
                                }
                                else if ((node[x, y].F == lowestF) && (node[x, y].G < lowestG))
                                {
                                    currentX = x;
                                    currentY = y;
                                    lowestF = node[x, y].F;
                                    lowestG = node[x, y].G;
                                }
                            }
                        }
                    }

                    //make current node as closed
                    node[currentX, currentY].onClosed = true;
                    node[currentX, currentY].onOpen = false;

                    parentG = node[currentX, currentY].G;
                }
                //textBox13.Text = "while1";


                currentX = endX;
                currentY = endY;
                //to store the found path, start from ending point and get all parent node back to the starting point
                while ((currentX != startX) || (currentY != startY))
                {

                    path[count].X = node[currentX, currentY].parentX;
                    path[count].Y = node[currentX, currentY].parentY;
                    path[count].F = node[currentX, currentY].F;
                    currentX = path[count].X;
                    currentY = path[count].Y;
                    count++;
                }

                int _count = count - 1;
                int ia = 0;
                count = 0;

                while (_count > -1)
                {

                    // printf("while\n");
                    // printf("%d\n",_count);
                    if ((path[_count].Y == 2) || (path[_count].X == 3) || (path[_count].X == 5) || (path[_count].X == 9) || (path[_count].X == 11))
                    {
                        _count -= 1;

                        //printf("%d\n",_count);
                    }
                    else
                    {

                        if (path[_count].Y >= 3)
                        {
                            finalPath[ia].Y = path[_count].Y - 1;
                        }
                        else
                        { finalPath[ia].Y = path[_count].Y; }

                        if (path[_count].X == 4)
                            finalPath[ia].X = path[_count].X - 1;
                        else if ((path[_count].X >= 5) && (path[_count].X <= 9))
                            finalPath[ia].X = path[_count].X - 2;
                        else if (path[_count].X == 10)
                            finalPath[ia].X = path[_count].X - 3;
                        else if ((path[_count].X >= 11) && (path[_count].X <= 14))
                            finalPath[ia].X = path[_count].X - 4;
                        else
                            finalPath[ia].X = path[_count].X;

                        _count -= 1;
                        ia++;
                    }

                }
                _count = 0;
                scatterGraph1.ClearData();

                for (int i = 0; i < ia; i++)
                {
                    scatterGraph1.PlotXYAppend(finalPath[i].Y, finalPath[i].X);
                }

                for (int i = 0; i < 100; i++)
                {
                    path[i].X = 0;
                    path[i].Y = 0;
                    finalPath[i].X = 0;
                    finalPath[i].Y = 0;
                }

            }
            catch(Exception ae){
                mainTextBox.Text = "Error while Path Planning";
            }
        }


        private void readSonar_btn_Click(object sender, EventArgs e)
        {
            serialPort1.DiscardOutBuffer();
            serialPrint("SONAR,"+"D,");
        }

        private void readEnc_L_Click(object sender, EventArgs e)
        {
            serialPort1.DiscardOutBuffer();
            serialPrint("M," + "R");
        }

        private void resetEnc_btn_Click(object sender, EventArgs e)
        {
            serialPort1.DiscardOutBuffer();
            serialPrint("M," + "X");
        }

        private void pidRotate_Apply_btn_Click(object sender, EventArgs e)
        {
            serialPort1.DiscardOutBuffer();
            serialPrint("M," + "P," + Convert.ToString(numericUpDown9.Value) + "," + Convert.ToString(numericUpDown8.Value) + "," + Convert.ToString(numericUpDown7.Value) + ",3");
        }

        private void RotateRight_btn_Click(object sender, EventArgs e)
        {
             serialPort1.DiscardOutBuffer();
             serialPrint("M," + "G," + "R," + Convert.ToString(hScrollBar3.Value) + "," + Convert.ToString(hScrollBar4.Value));     //M,G,R/L   (abbv: Motor,Ghoom,Right/Left)
        }

        private void RotateLeft_btn_Click(object sender, EventArgs e)
        {
            serialPort1.DiscardOutBuffer();
            serialPrint("M," + "G," + "L," + Convert.ToString(hScrollBar2.Value) + "," + Convert.ToString(hScrollBar1.Value));     //M,G,R/L
        }

        private void hScrollBar2_Scroll(object sender, ScrollEventArgs e)
        {
            textBox5.Text = Convert.ToString(hScrollBar2.Value);
        }
        
        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            textBox6.Text = Convert.ToString(hScrollBar1.Value);
        }

        private void hScrollBar3_Scroll(object sender, ScrollEventArgs e)
        {
            textBoxRL.Text = Convert.ToString(hScrollBar3.Value);
        }

        private void hScrollBar4_Scroll(object sender, ScrollEventArgs e)
        {
            textBox11.Text = Convert.ToString(hScrollBar4.Value);
        }

        private void hScrollBar5_Scroll(object sender, ScrollEventArgs e)
        {
            textBox13.Text = Convert.ToString(hScrollBar5.Value);
        }

        private void hScrollBar6_Scroll(object sender, ScrollEventArgs e)
        {
            textBox14.Text = Convert.ToString(hScrollBar6.Value);
        }
    }
}




