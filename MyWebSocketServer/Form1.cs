using JczMark;
using MyWebSocketServer.json;
using MyWebSocketServer.JSON;
using MyWebSocketServer.Server;
using MyWebSocketServer.Sys;
using SuperSocket.SocketBase.Config;
using SuperWebSocket;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
namespace MyWebSocketServer
{
    public partial class Form1 : Form
    {
        //声明委托,更新显示正在打印的字符

        public delegate void UpdatePrinting();
        public delegate void UpdatePrintingTextLabel(String text);
        public delegate void UpdateServerState(String msg);
        public delegate void UpdateConnectedState(String msg);
        public delegate void UpdateMsgReceivedLabel(int hideorshow);

        //创建一个委托变量
        public UpdatePrinting UpdatePrintingDelegate { get; private set; }
        public UpdatePrintingTextLabel UpdatePrintingTextLabelDelegate { get; private set; }

        private UpdateServerState UpdateServerStateDelegate;
        private UpdateConnectedState UpdateConnectedStateDelegate;


        private Boolean isRedLightStart = false;
        private Boolean isMarking = false;
        private Thread consumer;
        private Thread serverThread;
        public DataTable dt;
        private WebSocketServer server;
        private JSONSerializer jsonSerializer;

        public Form1()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.waiting_dataGridView.Columns.Clear();
            dt = new DataTable();
            dt.Columns.Add("编码", System.Type.GetType("System.String"));
            dt.Columns.Add("入队时间", System.Type.GetType("System.String"));
            dt.Columns.Add("状态", System.Type.GetType("System.String"));

            BindingSource bs = new BindingSource();
            bs.DataSource = dt;
            this.waiting_dataGridView.DataSource = bs;
            waiting_dataGridView.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            waiting_dataGridView.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            waiting_dataGridView.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;

            //注意第二个参数,现场实际运行时一定要改成false
            var ret = MarkJcz.InitLaser(this.Handle,true);
            //var ret = MarkJcz.InitLaser();
            Console.WriteLine(ret);
            if (ret) {
                //初始化成功
               // MarkJcz.ShowDevCfgForm();
            }

            //实例化委托
            UpdatePrintingDelegate = new UpdatePrinting(AddUpdatePrintingMethod);
            UpdatePrintingTextLabelDelegate = new UpdatePrintingTextLabel(UpdateLabel);
            UpdateServerStateDelegate = new UpdateServerState(UpdateServerStateFunc);
            UpdateConnectedStateDelegate = new UpdateConnectedState(UpdateConnectedStateFunc);

            Console.WriteLine("主线程:"+Thread.CurrentThread.GetHashCode());

            //创建websocket服务器线程
            serverThread = new Thread(new ThreadStart(createServerFunc));
            serverThread.Start();
            
            //实例化委托
            //webSocketService.updateTxtDelegate = new UpdatePrinting(UpdatePrintingMethod);

            //创建消费者线程(不用,现在采用手工点击事件处理标刻)
            //consumer = new Thread(new ThreadStart(markEzdFunc));
            //consumer.Start();
        }

        //创建服务器
        public void createServerFunc() {

            Console.WriteLine("websocketserver服务器启动..." + Thread.CurrentThread.GetHashCode());
            server = new WebSocketServer();
            jsonSerializer = new JSONSerializer();
            server.NewMessageReceived += (sock, message) =>
            {
                try
                {
                    MsgList msgJson = jsonSerializer.Deserialize<MsgList>(message);
                    Console.WriteLine("收到新的消息..." + message);
                    if (msgJson != null&& msgJson.msgs.Count>0)
                    {
                            lock (this.waiting_dataGridView)
                            {
                                foreach (var item in msgJson.msgs)
                                {
                                    Console.WriteLine("添加数据到DT中的服务线程..." + Thread.CurrentThread.GetHashCode());
                                    DataRow dr = dt.NewRow();
                                    dr["编码"] = item.PrintCode;
                                    dr["入队时间"] = DateTime.Now.ToLocalTime().ToString();
                                    dr["状态"] = "排队等待";
                                    dt.Rows.Add(dr);
                                    Monitor.PulseAll(this.waiting_dataGridView);
                                    this.waiting_dataGridView.BeginInvoke(UpdatePrintingDelegate);
                                    sock.Send(item.PrintCode + "已加入到打印队列");
                                }
                        }

                    }

                    Console.WriteLine("服务线程..." + Thread.CurrentThread.GetHashCode());
                    //sock.Send(message+"已加入到打印队列");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    sock.Close();
                }
            };

            server.SessionClosed += (sock, message) =>
            {
                Console.WriteLine("session连接断开...");
                this.connected_client_count_label.BeginInvoke(UpdateConnectedStateDelegate, "当前连接数 " + server.SessionCount);
            };


            server.NewSessionConnected += (sock) =>
            {
                Console.WriteLine("打开新的连接...");
                
                this.connected_client_count_label.BeginInvoke(UpdateConnectedStateDelegate, "当前连接数 "+server.SessionCount);
            };
            this.server_state_label.BeginInvoke(UpdateServerStateDelegate, "正在运行");
            if (!server.Setup(new ServerConfig { Port = 8881, MaxConnectionNumber = 100 }) || !server.Start())
            {
                this.server_state_label.BeginInvoke(UpdateServerStateDelegate, "启动失败...");
                WebSocketException.ThrowServerError("启动失败...");             
            }

        }

        //执行激光打码
        public void markEzdFunc()
        {
            string markNum = null;
            if (dt.Rows.Count > 0)
            {
                markNum = printing_text_label.Text;

                if (markNum != null&& markNum.Trim().Length!=0)
                {
                    //处理打码操作
                    Thread.Sleep(200);

                    isMarking = true;
                    markBtn.Text = "停止标刻";
                    markBtn.BackColor = System.Drawing.Color.Khaki;
                    this.server_state_label.BeginInvoke(UpdateServerStateDelegate, "正在标刻:"+ markNum);

                    bool res = MarkJcz.Mark();//此函数一直等待设备加工完毕后才返回
                    Thread.Sleep(100);
                    stopMark();
                    if (res)
                    {
                        //标刻成功完成
                        this.server_state_label.BeginInvoke(UpdateServerStateDelegate, markNum + "标刻完成");
                        DialogResult dr = MessageBox.Show("标刻成功,是否需要重新标刻\n"+ markNum + "?", "标刻完成", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                        if (dr == DialogResult.OK)
                        {
                            return;
                        }
                        dt.Rows.RemoveAt(0);
                    }
                    else
                    {
                        MessageBox.Show("标刻失败");
                        this.server_state_label.BeginInvoke(UpdateServerStateDelegate, markNum + "标刻失败");
                        MessageBox.Show(MarkJcz.GetLastError());
                        return;
                    }  
                    this.waiting_dataGridView.BeginInvoke(UpdatePrintingDelegate);//刷新正在排队等待打印的字符串的列表
                }
                
                //Monitor.PulseAll(this.waiting_dataGridView);
            }
            else
            {
                MessageBox.Show("没有要标刻的数据");
                this.printing_text_label.BeginInvoke(UpdatePrintingTextLabelDelegate, "");
                //Monitor.Wait(this.waiting_dataGridView);
            }
        }

            //有新打印数据,加入到datagrid中等待
            public void AddUpdatePrintingMethod()
        {

            /*
                DataGridViewRow dgvr = new DataGridViewRow();
                dgvr.CreateCells(waiting_dataGridView);
                dgvr.Cells[0].Value = msg;
                dgvr.Cells[1].Value = DateTime.Now.ToLocalTime().ToString();
                dgvr.Cells[2].Value = "排队等待";

                //插入到第一行
                // this.waiting_dataGridView.Rows.Insert(0, dgvr);
                //附加到最后
                this.waiting_dataGridView.Rows.Add(dgvr); 
                */

                //waiting_dataGridView.DataSource = dt;
                if (dt.Rows.Count > 0)
                {
                    waiting_dataGridView.ScrollBars = ScrollBars.Both;
                }
                else
                {
                    waiting_dataGridView.ScrollBars = ScrollBars.None;
                }
                waiting_dataGridView.Refresh();
                waiting_dataGridView.Update();
            //如果printing_text_label上内容为空,把第一行rows[0]放到里面打码
                string markNum = dt.Rows.Count>0?dt.Rows[0]["编码"] as string:"";
                if (printing_text_label.Text==null|| printing_text_label.Text.Trim().Length==0|| printing_text_label.Text!= markNum)
                {
                    if (dt.Rows.Count > 0)
                    {
                        dt.Rows[0]["状态"] = "正在标刻";
                    }                    
                    this.printing_text_label.BeginInvoke(UpdatePrintingTextLabelDelegate, markNum);//label上显示正在打码的数据
                }

            //Console.WriteLine(Thread.CurrentThread.GetHashCode() + "---" + "刷新到UI grid中");       
        }

        //更新显示正在打印的编码,同时更新二维码信息
        public void UpdateLabel(string txt) {
            this.printing_text_label.Text = txt;

            if (printing_text_label.Text != null && printing_text_label.Text.Trim().Length != 0)
            {
                MarkJcz.ChangeTextByName("no", printing_text_label.Text.Trim());//模板中的变量名字必须是no
                MarkJcz.ShowPreviewBmp(printingQcode_pictureBox);
            }
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            //consumer.Abort();
            //consumer = null;
            server.Stop();
            server.Dispose();

            
            if (serverThread!=null) {
                serverThread.Abort();
                serverThread = null;
            }

            base.Dispose(disposing);
        }


        //更新服务器状态
        private void UpdateServerStateFunc(string msg)
        {
            this.server_state_label.Text = msg;
        }

        //更新连接客户端个数
        private void UpdateConnectedStateFunc(string msg)
        {
            this.connected_client_count_label.Text = msg;
        }

        private void closedForm(object sender, FormClosedEventArgs e)
        {
            MarkJcz.Close();
        }
        

        private void button4_Click(object sender, EventArgs e)
        {
            //获取数据库中的文本对象内容
            String val=null;
            MarkJcz.GetTextByName("no", ref val);
            MessageBox.Show(val);

            MarkJcz.ChangeTextByName("no","123123123123");

            MarkJcz.GetTextByName("no", ref val);
            MessageBox.Show(val);

            MessageBox.Show(MarkJcz.SaveEntLibToFile().ToString());
            
        }

        private void loadTemplateBtn_Click(object sender, EventArgs e)
        {
            String path = @"D:\1.ezd";//下面第二个参数为true时可以随便写
            bool loaded = MarkJcz.LoadEzdFile(ref path, true);//加载模板
            if (loaded)
            {
                //如果printing_text_label上有当前需要打印的内容,那么直接替换模板二维码中的内容
                if (printing_text_label.Text != null && printing_text_label.Text.Trim().Length != 0)
                {
                    MarkJcz.ChangeTextByName("no", printing_text_label.Text.Trim());//模板中的变量名字必须是no
                }

                MarkJcz.ShowPreviewBmp(printingQcode_pictureBox);
                path = path.Substring(path.LastIndexOf(@"\") + 1);
                currentTemplateName_label.Text = path;

                //加载完模板停止红光和标刻
                stopMark();
                stopRedLight();

                this.server_state_label.BeginInvoke(UpdateServerStateDelegate, "加载模板完成");
            }
            else
            {
                this.server_state_label.BeginInvoke(UpdateServerStateDelegate, "加载模板失败");
            }
        }

        private void redLightBtn_Click(object sender, EventArgs e)
        {     
            if (!isRedLightStart)
            {
                //必须已经加载模板
                if (currentTemplateName_label.Text == null || currentTemplateName_label.Text.Trim().Length == 0)
                {
                    MessageBox.Show("请先加载模板");
                    return;
                }

                bool res = MarkJcz.StartRed();//开启红光
                if (res)
                {
                    isRedLightStart = true;
                    redLightBtn.Text = "停止红光";
                    redLightBtn.BackColor = System.Drawing.Color.Khaki;
                    this.server_state_label.BeginInvoke(UpdateServerStateDelegate, "红光指示中...");
                }
                else
                {
                    MessageBox.Show("启动红光失败");
                    this.server_state_label.BeginInvoke(UpdateServerStateDelegate, "红光启动失败");
                }
               
            }
            else
            {
                stopRedLight();
            }
        }

        private void stopRedLight()
        {
            if (isRedLightStart)
            {
                MarkJcz.StopRed();//关闭红光
                isRedLightStart = false;
                redLightBtn.Text = "红光";
                redLightBtn.BackColor = System.Drawing.Color.Yellow;
                this.server_state_label.BeginInvoke(UpdateServerStateDelegate, "红光已关闭");
            }
        }

        private void stopMark()
        {
            if (isMarking)
            {
                //停止标刻
                bool res = MarkJcz.StopMark();
                if (res)
                {
                    isMarking = false;
                    markBtn.Text = "标刻";
                    markBtn.BackColor = System.Drawing.Color.Green;
                    this.server_state_label.BeginInvoke(UpdateServerStateDelegate, "标刻已停止");
                }
                else
                {
                    MessageBox.Show("停止标刻失败");
                    this.server_state_label.BeginInvoke(UpdateServerStateDelegate, "标刻停止失败");
                }
            }
        }

        private void markBtn_Click(object sender, EventArgs e)
        {
            if (!isMarking)
            {
                //标刻前必须已经加载模板
                if (currentTemplateName_label.Text==null|| currentTemplateName_label.Text.Trim().Length==0)
                {
                    MessageBox.Show("请先加载模板");
                    return;
                }
                //标刻前必须停止红光
                stopRedLight();

                //标刻
                markEzdFunc();
            
            }
            else
            {
                stopMark();
            }
        }

        private void deleteGridItem(object sender, EventArgs e)
        {
            int curIndex = waiting_dataGridView.CurrentRow.Index;
            this.dt.Rows.RemoveAt(curIndex);
            //更新状态
            this.waiting_dataGridView.BeginInvoke(UpdatePrintingDelegate);
        }
    }
}
