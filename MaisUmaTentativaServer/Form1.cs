using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using OABLanServer.Enums;
using System.Diagnostics;
using System.ServiceModel;
using MaisUmaTentativaServer.ServiceReference1;
using MaisUmaTentativaServer.Enums;

namespace OABLanServer
{
    public partial class Form1 : Form
    {
        Conexao conexao = new Conexao();
        SqlCommand sqlCommand = new SqlCommand();
        string nomeUsuario = string.Empty;
        string subSecao = string.Empty;
        public string mensagemDeErro;
        public int status;
        public int admin;
        const int MF_BYCOMMAND = 0X400;
        [DllImport("user32")]
        static extern int RemoveMenu(IntPtr hMenu, int nPosition, int wFlags);
        [DllImport("user32")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32")]
        static extern int GetMenuItemCount(IntPtr hWnd);

        //Declarar e inicializar o endereço IP
        //static IPAddress ipAd = IPAddress.Parse("192.168.15.23");

        //Declarar e iniciarlizar a porta
        static int PortNumber = 8001;

        /* Inicializar o listener */
        TcpListener ServerListener = new TcpListener(obterIP(), PortNumber);
        TcpClient clientSocket = default(TcpClient);

        public Form1(string nomeUsuario, string subSecao)
        {
            this.nomeUsuario = nomeUsuario;
            this.subSecao = subSecao;

            InitializeComponent();

            //Pintar as caixas das máquinas aqui conforme o status
            PintaGroupBoxesConformeStatus();
            AtualizaTextoLabelsDeStatus();
            DesabilitaOuHabilitaBotaoDeCadastrarMaquina();
            //ProtocolosDeSeguranca();
        }

        public Form1(string nomeUsuario, int admin)
        {
            this.nomeUsuario = nomeUsuario;
            this.admin = admin;

            InitializeComponent();

            //Pintar as caixas das máquinas aqui conforme o status
            PintaGroupBoxesConformeStatus();
            AtualizaTextoLabelsDeStatus();
            DesabilitaOuHabilitaBotaoDeCadastrarMaquina(admin);
            DesabilitaBotoesFuncionalidades(admin);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            IntPtr hMenu = GetSystemMenu(this.Handle, false);
            int CountMenu = GetMenuItemCount(hMenu) - 1;
            RemoveMenu(hMenu, CountMenu, MF_BYCOMMAND);

            notifyIcon1.Icon = this.Icon;
            notifyIcon1.BalloonTipTitle = "OABLAN";
            notifyIcon1.BalloonTipText = $"Bem vindo {nomeUsuario}, servidor online!";
            notifyIcon1.ShowBalloonTip(20);

            Thread ThreadingServer = new Thread(IniciarErodarServidor);
            ThreadingServer.Start();
        }

        public static IPAddress obterIP()
        {
            IPAddress ipAd;
            string IP = string.Empty;
            Regex regex = new Regex(@"(192\.168\.\d{1,3}\.\d{1,3})|(172\.16\.\d{1,3}\.{1,3})|(172\.31\.\d{1,3}\.{1,3})|(172\.16\.\d{1,3}\.{1,3}|(10\.\d{1,3}\.\d{1,3}\.{1,3}))");

            string strHostName = Dns.GetHostName();
            IPHostEntry iPHostEntry = Dns.GetHostEntry(strHostName);

            foreach (var item in iPHostEntry.AddressList)
            {
                Match match = regex.Match(item.ToString());

                if (match.Success)
                {
                    IP = match.Value;
                }
            }

            ipAd = IPAddress.Parse(IP);

            return ipAd;
        }

        private void DesabilitaOuHabilitaBotaoDeCadastrarMaquina(int admin = 0)
        {
            BasicHttpsBinding binding = new BasicHttpsBinding();
            binding.Security.Mode = BasicHttpsSecurityMode.Transport;
            EndpointAddress endpoint = new EndpointAddress("https://wsoab.oabrs.org.br/Service.asmx");
            ServiceSoapClient WebService = new ServiceSoapClient(binding, endpoint);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            //BasicHttpBinding binding = new BasicHttpBinding();
            //binding.Security.Mode = BasicHttpSecurityMode.None;
            //binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            //EndpointAddress endpoint = new EndpointAddress("http://localhost:38115");
            //MaisUmaTentativaServer.ServiceReference2.ServiceSoapClient WebServiceLocal = new MaisUmaTentativaServer.ServiceReference2.ServiceSoapClient(binding, endpoint);
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            int numeroMaquina = 0;

            List<Button> listaBotoesCadastroDeMaquinas = new List<Button>();
            listaBotoesCadastroDeMaquinas.Add(btnCadastrarMaquina01);
            listaBotoesCadastroDeMaquinas.Add(btnCadastrarMaquina02);
            listaBotoesCadastroDeMaquinas.Add(btnCadastrarMaquina03);
            listaBotoesCadastroDeMaquinas.Add(btnCadastrarMaquina04);
            listaBotoesCadastroDeMaquinas.Add(btnCadastrarMaquina05);
            listaBotoesCadastroDeMaquinas.Add(btnCadastrarMaquina06);
            listaBotoesCadastroDeMaquinas.Add(btnCadastrarMaquina07);
            listaBotoesCadastroDeMaquinas.Add(btnCadastrarMaquina08);
            listaBotoesCadastroDeMaquinas.Add(btnCadastrarMaquina09);
            listaBotoesCadastroDeMaquinas.Add(btnCadastrarMaquina10);
            listaBotoesCadastroDeMaquinas.Add(btnCadastrarMaquina11);
            listaBotoesCadastroDeMaquinas.Add(btnCadastrarMaquina12);
            listaBotoesCadastroDeMaquinas.Add(btnCadastrarMaquina13);
            listaBotoesCadastroDeMaquinas.Add(btnCadastrarMaquina14);
            listaBotoesCadastroDeMaquinas.Add(btnCadastrarMaquina15);

            foreach (var item in listaBotoesCadastroDeMaquinas)
            {
                numeroMaquina++;

                item.Visible = WebService.wsDesabilitaOuHabilitaBotaoCadastrarMaquinaLanServerOAB(numeroMaquina, subSecao);

                if (admin != 1)
                {
                    item.Enabled = false;
                    item.Visible = false;
                }
            }
        }

        private void DesabilitaBotoesFuncionalidades(int admin)
        {
            if (admin == 1)
            {
                btnConfigurarSessoes.Enabled = false;
                btnDesligarMaquina.Enabled = false;
                btnUsuariosSistema.Enabled = false;
                button1.Enabled = false;
                lblTipAdm.Visible = true;
            }

        }

        private void AtualizaTextoLabelsDeStatus(Label lblStatusMaquina = null)
        {
            BasicHttpsBinding binding = new BasicHttpsBinding();
            binding.Security.Mode = BasicHttpsSecurityMode.Transport;
            EndpointAddress endpoint = new EndpointAddress("https://wsoab.oabrs.org.br/Service.asmx");
            ServiceSoapClient WebService = new ServiceSoapClient(binding, endpoint);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            //BasicHttpBinding binding = new BasicHttpBinding();
            //binding.Security.Mode = BasicHttpSecurityMode.None;
            //binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            //EndpointAddress endpoint = new EndpointAddress("http://localhost:38115");
            //MaisUmaTentativaServer.ServiceReference2.ServiceSoapClient WebServiceLocal = new MaisUmaTentativaServer.ServiceReference2.ServiceSoapClient(binding, endpoint);
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            NomeUsuarioServidor.Text = nomeUsuario;
            NomeSubSecaoServidor.Text = subSecao;

            int numeroMaquina = 0;

            List<Label> listaDeStatusLabels = new List<Label>();
            listaDeStatusLabels.Add(lblStatus1);
            listaDeStatusLabels.Add(lblStatus2);
            listaDeStatusLabels.Add(lblStatus3);
            listaDeStatusLabels.Add(lblStatus4);
            listaDeStatusLabels.Add(lblStatus5);
            listaDeStatusLabels.Add(lblStatus6);
            listaDeStatusLabels.Add(lblStatus7);
            listaDeStatusLabels.Add(lblStatus8);
            listaDeStatusLabels.Add(lblStatus9);
            listaDeStatusLabels.Add(lblStatus10);
            listaDeStatusLabels.Add(lblStatus11);
            listaDeStatusLabels.Add(lblStatus12);
            listaDeStatusLabels.Add(lblStatus13);
            listaDeStatusLabels.Add(lblStatus14);
            listaDeStatusLabels.Add(lblStatus15);

            foreach (var item in listaDeStatusLabels)
            {
                numeroMaquina++;

                status = WebService.wsAtualizaTextoElabelsDeStatus(numeroMaquina, subSecao);

                if (status == 0)
                {
                    //Mantem a cor cinza...
                    item.Text = Enum.GetName(typeof(StatusMaquinas), 0);
                }
                else if (status == 1)
                {
                    //A máquina está cadastrada e livre, pinta de LightGreen
                    item.BackColor = Color.LightGreen;
                    item.Text = Enum.GetName(typeof(StatusMaquinas), 1);

                }
                else if (status == 2)
                {
                    //A máquina está ocupada, pinta de Red
                    item.BackColor = Color.Red;
                    item.Text = Enum.GetName(typeof(StatusMaquinas), 2);
                }

            }

        }

        private void PintaGroupBoxesConformeStatus()
        {
            BasicHttpsBinding binding = new BasicHttpsBinding();
            binding.Security.Mode = BasicHttpsSecurityMode.Transport;
            EndpointAddress endpoint = new EndpointAddress("https://wsoab.oabrs.org.br/Service.asmx");
            ServiceSoapClient WebService = new ServiceSoapClient(binding, endpoint);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            //BasicHttpBinding binding = new BasicHttpBinding();
            //binding.Security.Mode = BasicHttpSecurityMode.None;
            //binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            //EndpointAddress endpoint = new EndpointAddress("http://localhost:38115");
            //MaisUmaTentativaServer.ServiceReference2.ServiceSoapClient WebServiceLocal = new MaisUmaTentativaServer.ServiceReference2.ServiceSoapClient(binding, endpoint);
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            List<GroupBox> listaDeGroupBoxes = new List<GroupBox>();
            listaDeGroupBoxes.Add(groupBox1);
            listaDeGroupBoxes.Add(groupBox2);
            listaDeGroupBoxes.Add(groupBox3);
            listaDeGroupBoxes.Add(groupBox6);
            listaDeGroupBoxes.Add(groupBox5);
            listaDeGroupBoxes.Add(groupBox4);
            listaDeGroupBoxes.Add(groupBox9);
            listaDeGroupBoxes.Add(groupBox8);
            listaDeGroupBoxes.Add(groupBox7);
            listaDeGroupBoxes.Add(groupBox12);
            listaDeGroupBoxes.Add(groupBox11);
            listaDeGroupBoxes.Add(groupBox10);
            listaDeGroupBoxes.Add(groupBox15);
            listaDeGroupBoxes.Add(groupBox14);
            listaDeGroupBoxes.Add(groupBox13);

            foreach (var item in listaDeGroupBoxes)
            {
                status = WebService.wsPintaGroupBoxesConformeStatus(item.Text, subSecao);

                if (status == 0)
                {
                    //Mantem a cor cinza...
                }
                else if (status == 1)
                {
                    //A máquina está cadastrada e livre, pinta de LightGreen
                    item.BackColor = Color.LightGreen;

                }
                else if (status == 2)
                {
                    //A máquina está ocupada, pinta de Red
                    item.BackColor = Color.Red;
                }
            }
        }

        private void THREAD_MOD(string teste)
        {
            txtStatus.Text += Environment.NewLine + teste;
        }

        private void IniciarErodarServidor()
        {
            BasicHttpsBinding binding = new BasicHttpsBinding();
            binding.Security.Mode = BasicHttpsSecurityMode.Transport;
            EndpointAddress endpoint = new EndpointAddress("https://wsoab.oabrs.org.br/Service.asmx");
            ServiceSoapClient WebService = new ServiceSoapClient(binding, endpoint);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            BasicHttpBinding bindingLocal = new BasicHttpBinding();
            bindingLocal.Security.Mode = BasicHttpSecurityMode.None;
            bindingLocal.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            EndpointAddress endpointLocal = new EndpointAddress("http://localhost:38115");
            MaisUmaTentativaServer.ServiceReference2.ServiceSoapClient WebServiceLocal = new MaisUmaTentativaServer.ServiceReference2.ServiceSoapClient(bindingLocal, endpointLocal);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Action<string> DelegateTeste_ModifyText = THREAD_MOD;
            ServerListener.Start();
            Invoke(DelegateTeste_ModifyText, "Servidor aguardando conexões!");
            clientSocket = ServerListener.AcceptTcpClient();
            Invoke(DelegateTeste_ModifyText, "Servidor rodando!");

            while (true)
            {
                try
                {
                    NetworkStream networkStream = clientSocket.GetStream();
                    byte[] bytesFrom = new byte[500];
                    networkStream.Read(bytesFrom, 0, bytesFrom.Length);
                    string dadosDoCliente = Encoding.ASCII.GetString(bytesFrom);
                    dadosDoCliente = dadosDoCliente.Substring(0, dadosDoCliente.IndexOf("$"));
                    string valor = string.Empty;
                    int valorInt = 0;
                    string maquina = string.Empty;
                    string IPCliente = string.Empty;//((IPEndPoint)clientSocket.Client.RemoteEndPoint).Address.ToString();

                    string[] arrayDadosDoCliente = dadosDoCliente.Split(':');

                    if (arrayDadosDoCliente[0] == "Obter tempo sessao")
                    {
                        IPCliente = arrayDadosDoCliente[2].Trim();

                        WebService.wsNomearMaquina(arrayDadosDoCliente[1].Trim(), IPCliente, subSecao);

                        maquina = WebService.wsBuscarTempoSessaoMaquinas(arrayDadosDoCliente[1].Trim(), IPCliente, subSecao, out valor);

                        if (valor == "IP nao encontrado na base de dados.")
                        {
                            string serverResponseIP = valor;
                            Byte[] sendBytesIP = Encoding.ASCII.GetBytes(serverResponseIP);
                            networkStream.Write(sendBytesIP, 0, sendBytesIP.Length);
                            networkStream.Flush();
                        }
                        else
                        {
                            valorInt = int.Parse(valor);
                        }

                        if (valorInt > 0)
                        {
                            //Sinalizar o GroupBox de vermelho e atualizar o labelStatus
                            if (!string.IsNullOrEmpty(maquina))
                            {
                                Invoke(DelegateTeste_ModifyText, $"Sessão iniciada para {maquina}, ({valorInt} Minutos...)");

                                int statusMaquina = 0;

                                statusMaquina = WebService.wsSinalizarMaquinasInicioSessao(Convert.ToInt32(StatusMaquinas.Ocupado), maquina, subSecao);

                                if (statusMaquina == Convert.ToInt32(StatusMaquinas.Ocupado))
                                {
                                    //Pintar GroupBoxes conforme o status...
                                    //Atualizar texto dos labels de status conforme o status...
                                    switch (maquina)
                                    {
                                        case "Máquina 01":
                                            groupBox1.Invoke((MethodInvoker)delegate
                                            {
                                                groupBox1.BackColor = Color.Red;
                                            });
                                            break;
                                        case "Máquina 02":
                                            groupBox2.Invoke((MethodInvoker)delegate
                                            {
                                                groupBox2.BackColor = Color.Red;
                                            });
                                            break;
                                        case "Máquina 03":
                                            groupBox3.Invoke((MethodInvoker)delegate
                                            {
                                                groupBox3.BackColor = Color.Red;
                                            });
                                            break;
                                        case "Máquina 04":
                                            groupBox4.Invoke((MethodInvoker)delegate
                                            {
                                                groupBox4.BackColor = Color.Red;
                                            });
                                            break;
                                        case "Máquina 05":
                                            groupBox5.Invoke((MethodInvoker)delegate
                                            {
                                                groupBox5.BackColor = Color.Red;
                                            });
                                            break;
                                        case "Máquina 06":
                                            groupBox6.Invoke((MethodInvoker)delegate
                                            {
                                                groupBox6.BackColor = Color.Red;
                                            });
                                            break;
                                        case "Máquina 07":
                                            groupBox7.Invoke((MethodInvoker)delegate
                                            {
                                                groupBox7.BackColor = Color.Red;
                                            });
                                            break;
                                        case "Máquina 08":
                                            groupBox8.Invoke((MethodInvoker)delegate
                                            {
                                                groupBox8.BackColor = Color.Red;
                                            });
                                            break;
                                        case "Máquina 09":
                                            groupBox9.Invoke((MethodInvoker)delegate
                                            {
                                                groupBox9.BackColor = Color.Red;
                                            });
                                            break;
                                        case "Máquina 10":
                                            groupBox10.Invoke((MethodInvoker)delegate
                                            {
                                                groupBox10.BackColor = Color.Red;
                                            });
                                            break;
                                        case "Máquina 11":
                                            groupBox11.Invoke((MethodInvoker)delegate
                                            {
                                                groupBox11.BackColor = Color.Red;
                                            });
                                            break;
                                        case "Máquina 12":
                                            groupBox12.Invoke((MethodInvoker)delegate
                                            {
                                                groupBox12.BackColor = Color.Red;
                                            });
                                            break;
                                        case "Máquina 13":
                                            groupBox13.Invoke((MethodInvoker)delegate
                                            {
                                                groupBox13.BackColor = Color.Red;
                                            });
                                            break;
                                        case "Máquina 14":
                                            groupBox14.Invoke((MethodInvoker)delegate
                                            {
                                                groupBox14.BackColor = Color.Red;
                                            });
                                            break;
                                        case "Máquina 15":
                                            groupBox15.Invoke((MethodInvoker)delegate
                                            {
                                                groupBox15.BackColor = Color.Red;
                                            });
                                            break;
                                    }

                                    switch (maquina)
                                    {
                                        case "Máquina 01":
                                            lblStatus1.Invoke((MethodInvoker)delegate
                                            {
                                                lblStatus1.Text = Enum.GetName(typeof(StatusMaquinas), 2);
                                                lblStatus1.BackColor = Color.Red;
                                            });
                                            break;
                                        case "Máquina 02":
                                            lblStatus2.Invoke((MethodInvoker)delegate
                                            {
                                                lblStatus2.Text = Enum.GetName(typeof(StatusMaquinas), 2);
                                                lblStatus2.BackColor = Color.Red;
                                            });
                                            break;
                                        case "Máquina 03":
                                            lblStatus3.Invoke((MethodInvoker)delegate
                                            {
                                                lblStatus3.Text = Enum.GetName(typeof(StatusMaquinas), 2);
                                                lblStatus3.BackColor = Color.Red;
                                            });
                                            break;
                                        case "Máquina 04":
                                            lblStatus4.Invoke((MethodInvoker)delegate
                                            {
                                                lblStatus4.Text = Enum.GetName(typeof(StatusMaquinas), 2);
                                                lblStatus4.BackColor = Color.Red;
                                            });
                                            break;
                                        case "Máquina 05":
                                            lblStatus5.Invoke((MethodInvoker)delegate
                                            {
                                                lblStatus5.Text = Enum.GetName(typeof(StatusMaquinas), 2);
                                                lblStatus5.BackColor = Color.Red;
                                            });
                                            break;
                                        case "Máquina 06":
                                            lblStatus6.Invoke((MethodInvoker)delegate
                                            {
                                                lblStatus6.Text = Enum.GetName(typeof(StatusMaquinas), 2);
                                                lblStatus6.BackColor = Color.Red;
                                            });
                                            break;
                                        case "Máquina 07":
                                            lblStatus7.Invoke((MethodInvoker)delegate
                                            {
                                                lblStatus7.Text = Enum.GetName(typeof(StatusMaquinas), 2);
                                                lblStatus7.BackColor = Color.Red;
                                            });
                                            break;
                                        case "Máquina 08":
                                            lblStatus8.Invoke((MethodInvoker)delegate
                                            {
                                                lblStatus8.Text = Enum.GetName(typeof(StatusMaquinas), 2);
                                                lblStatus8.BackColor = Color.Red;
                                            });
                                            break;
                                        case "Máquina 09":
                                            lblStatus9.Invoke((MethodInvoker)delegate
                                            {
                                                lblStatus9.Text = Enum.GetName(typeof(StatusMaquinas), 2);
                                                lblStatus9.BackColor = Color.Red;
                                            });
                                            break;
                                        case "Máquina 10":
                                            lblStatus10.Invoke((MethodInvoker)delegate
                                            {
                                                lblStatus10.Text = Enum.GetName(typeof(StatusMaquinas), 2);
                                                lblStatus10.BackColor = Color.Red;
                                            });
                                            break;
                                        case "Máquina 11":
                                            lblStatus11.Invoke((MethodInvoker)delegate
                                            {
                                                lblStatus11.Text = Enum.GetName(typeof(StatusMaquinas), 2);
                                                lblStatus11.BackColor = Color.Red;
                                            });
                                            break;
                                        case "Máquina 12":
                                            lblStatus12.Invoke((MethodInvoker)delegate
                                            {
                                                lblStatus12.Text = Enum.GetName(typeof(StatusMaquinas), 2);
                                                lblStatus12.BackColor = Color.Red;
                                            });
                                            break;
                                        case "Máquina 13":
                                            lblStatus13.Invoke((MethodInvoker)delegate
                                            {
                                                lblStatus13.Text = Enum.GetName(typeof(StatusMaquinas), 2);
                                                lblStatus13.BackColor = Color.Red;
                                            });
                                            break;
                                        case "Máquina 14":
                                            lblStatus14.Invoke((MethodInvoker)delegate
                                            {
                                                lblStatus14.Text = Enum.GetName(typeof(StatusMaquinas), 2);
                                                lblStatus14.BackColor = Color.Red;
                                            });
                                            break;
                                        case "Máquina 15":
                                            lblStatus15.Invoke((MethodInvoker)delegate
                                            {
                                                lblStatus15.Text = Enum.GetName(typeof(StatusMaquinas), 2);
                                                lblStatus15.BackColor = Color.Red;
                                            });
                                            break;
                                    }

                                    notifyIcon1.Icon = this.Icon;
                                    notifyIcon1.BalloonTipTitle = "Controle de sessões";
                                    notifyIcon1.BalloonTipText = $"Sessão iniciada para {maquina}";
                                    notifyIcon1.ShowBalloonTip(10);

                                    //MessageBox.Show($"Sessão iniciada para {maquina}");
                                }

                            }

                            string serverResponse = valor;
                            Byte[] sendBytes = Encoding.ASCII.GetBytes(serverResponse);
                            networkStream.Write(sendBytes, 0, sendBytes.Length);
                            networkStream.Flush();
                        }
                        else
                        {
                            string serverResponse = "Tempo de sessao nao definido para esta maquina.";
                            Byte[] sendBytes = Encoding.ASCII.GetBytes(serverResponse);
                            networkStream.Write(sendBytes, 0, sendBytes.Length);
                            networkStream.Flush();
                        }
                    }
                    else if (arrayDadosDoCliente[0] == "Contagem finalizada")
                    {
                        //Se entrou aqui deve sinalizar o GroupBox e o label de status
                        //Junto com isto, deve atualizar o status da tabela maquinas

                        //Se a sessão acabou o tempo de sessão deve ser RESETADO para zero 

                        maquina = WebService.wsBuscarNumeroMaquina(arrayDadosDoCliente[1].Trim(), subSecao);

                        if (!string.IsNullOrEmpty(maquina))
                        {
                            int statusMaquina = 0;

                            statusMaquina = WebService.wsSinalizarMaquinasFimSessao(Convert.ToInt32(StatusMaquinas.Livre), maquina, subSecao);

                            if (statusMaquina == Convert.ToInt32(StatusMaquinas.Livre))
                            {
                                //Pintar GroupBoxes conforme o status...
                                //Atualizar texto dos labels de status conforme o status...

                                switch (maquina)
                                {
                                    case "Máquina 01":
                                        groupBox1.Invoke((MethodInvoker)delegate
                                        {
                                            groupBox1.BackColor = Color.LightGreen;
                                        });
                                        break;
                                    case "Máquina 02":
                                        groupBox2.Invoke((MethodInvoker)delegate
                                        {
                                            groupBox2.BackColor = Color.LightGreen;
                                        });
                                        break;
                                    case "Máquina 03":
                                        groupBox3.Invoke((MethodInvoker)delegate
                                        {
                                            groupBox3.BackColor = Color.LightGreen;
                                        });
                                        break;
                                    case "Máquina 04":
                                        groupBox4.Invoke((MethodInvoker)delegate
                                        {
                                            groupBox4.BackColor = Color.LightGreen;
                                        });
                                        break;
                                    case "Máquina 05":
                                        groupBox5.Invoke((MethodInvoker)delegate
                                        {
                                            groupBox5.BackColor = Color.LightGreen;
                                        });
                                        break;
                                    case "Máquina 06":
                                        groupBox6.Invoke((MethodInvoker)delegate
                                        {
                                            groupBox6.BackColor = Color.LightGreen;
                                        });
                                        break;
                                    case "Máquina 07":
                                        groupBox7.Invoke((MethodInvoker)delegate
                                        {
                                            groupBox7.BackColor = Color.LightGreen;
                                        });
                                        break;
                                    case "Máquina 08":
                                        groupBox8.Invoke((MethodInvoker)delegate
                                        {
                                            groupBox8.BackColor = Color.LightGreen;
                                        });
                                        break;
                                    case "Máquina 09":
                                        groupBox9.Invoke((MethodInvoker)delegate
                                        {
                                            groupBox9.BackColor = Color.LightGreen;
                                        });
                                        break;
                                    case "Máquina 10":
                                        groupBox10.Invoke((MethodInvoker)delegate
                                        {
                                            groupBox10.BackColor = Color.LightGreen;
                                        });
                                        break;
                                    case "Máquina 11":
                                        groupBox11.Invoke((MethodInvoker)delegate
                                        {
                                            groupBox11.BackColor = Color.LightGreen;
                                        });
                                        break;
                                    case "Máquina 12":
                                        groupBox12.Invoke((MethodInvoker)delegate
                                        {
                                            groupBox12.BackColor = Color.LightGreen;
                                        });
                                        break;
                                    case "Máquina 13":
                                        groupBox13.Invoke((MethodInvoker)delegate
                                        {
                                            groupBox13.BackColor = Color.LightGreen;
                                        });
                                        break;
                                    case "Máquina 14":
                                        groupBox14.Invoke((MethodInvoker)delegate
                                        {
                                            groupBox14.BackColor = Color.LightGreen;
                                        });
                                        break;
                                    case "Máquina 15":
                                        groupBox15.Invoke((MethodInvoker)delegate
                                        {
                                            groupBox15.BackColor = Color.LightGreen;
                                        });
                                        break;
                                }

                                switch (maquina)
                                {
                                    case "Máquina 01":
                                        lblStatus1.Invoke((MethodInvoker)delegate
                                        {
                                            lblStatus1.Text = Enum.GetName(typeof(StatusMaquinas), 1);
                                            lblStatus1.BackColor = Color.LightGreen;
                                        });
                                        break;
                                    case "Máquina 02":
                                        lblStatus2.Invoke((MethodInvoker)delegate
                                        {
                                            lblStatus2.Text = Enum.GetName(typeof(StatusMaquinas), 1);
                                            lblStatus2.BackColor = Color.LightGreen;
                                        });
                                        break;
                                    case "Máquina 03":
                                        lblStatus3.Invoke((MethodInvoker)delegate
                                        {
                                            lblStatus3.Text = Enum.GetName(typeof(StatusMaquinas), 1);
                                            lblStatus3.BackColor = Color.LightGreen;
                                        });
                                        break;
                                    case "Máquina 04":
                                        lblStatus4.Invoke((MethodInvoker)delegate
                                        {
                                            lblStatus4.Text = Enum.GetName(typeof(StatusMaquinas), 1);
                                            lblStatus4.BackColor = Color.LightGreen;
                                        });
                                        break;
                                    case "Máquina 05":
                                        lblStatus5.Invoke((MethodInvoker)delegate
                                        {
                                            lblStatus5.Text = Enum.GetName(typeof(StatusMaquinas), 1);
                                            lblStatus5.BackColor = Color.LightGreen;
                                        });
                                        break;
                                    case "Máquina 06":
                                        lblStatus6.Invoke((MethodInvoker)delegate
                                        {
                                            lblStatus6.Text = Enum.GetName(typeof(StatusMaquinas), 1);
                                            lblStatus6.BackColor = Color.LightGreen;
                                        });
                                        break;
                                    case "Máquina 07":
                                        lblStatus7.Invoke((MethodInvoker)delegate
                                        {
                                            lblStatus7.Text = Enum.GetName(typeof(StatusMaquinas), 1);
                                            lblStatus7.BackColor = Color.LightGreen;
                                        });
                                        break;
                                    case "Máquina 08":
                                        lblStatus8.Invoke((MethodInvoker)delegate
                                        {
                                            lblStatus8.Text = Enum.GetName(typeof(StatusMaquinas), 1);
                                            lblStatus8.BackColor = Color.LightGreen;
                                        });
                                        break;
                                    case "Máquina 09":
                                        lblStatus9.Invoke((MethodInvoker)delegate
                                        {
                                            lblStatus9.Text = Enum.GetName(typeof(StatusMaquinas), 1);
                                            lblStatus9.BackColor = Color.LightGreen;
                                        });
                                        break;
                                    case "Máquina 10":
                                        lblStatus10.Invoke((MethodInvoker)delegate
                                        {
                                            lblStatus10.Text = Enum.GetName(typeof(StatusMaquinas), 1);
                                            lblStatus10.BackColor = Color.LightGreen;
                                        });
                                        break;
                                    case "Máquina 11":
                                        lblStatus11.Invoke((MethodInvoker)delegate
                                        {
                                            lblStatus11.Text = Enum.GetName(typeof(StatusMaquinas), 1);
                                            lblStatus11.BackColor = Color.LightGreen;
                                        });
                                        break;
                                    case "Máquina 12":
                                        lblStatus12.Invoke((MethodInvoker)delegate
                                        {
                                            lblStatus12.Text = Enum.GetName(typeof(StatusMaquinas), 1);
                                            lblStatus12.BackColor = Color.LightGreen;
                                        });
                                        break;
                                    case "Máquina 13":
                                        lblStatus13.Invoke((MethodInvoker)delegate
                                        {
                                            lblStatus13.Text = Enum.GetName(typeof(StatusMaquinas), 1);
                                            lblStatus13.BackColor = Color.LightGreen;
                                        });
                                        break;
                                    case "Máquina 14":
                                        lblStatus14.Invoke((MethodInvoker)delegate
                                        {
                                            lblStatus14.Text = Enum.GetName(typeof(StatusMaquinas), 1);
                                            lblStatus14.BackColor = Color.LightGreen;
                                        });
                                        break;
                                    case "Máquina 15":
                                        lblStatus15.Invoke((MethodInvoker)delegate
                                        {
                                            lblStatus15.Text = Enum.GetName(typeof(StatusMaquinas), 1);
                                            lblStatus15.BackColor = Color.LightGreen;
                                        });
                                        break;
                                }

                                notifyIcon1.Icon = this.Icon;
                                notifyIcon1.BalloonTipTitle = "Controle de sessões";
                                notifyIcon1.BalloonTipText = $"Sessão finalizada para {maquina}";
                                notifyIcon1.ShowBalloonTip(20);

                                //MessageBox.Show($"Sessão finalizada para {maquina}");
                            }

                            string serverResponse = "Recebido! ";
                            Byte[] sendBytes = Encoding.ASCII.GetBytes(serverResponse);
                            networkStream.Write(sendBytes, 0, sendBytes.Length);
                            networkStream.Flush();
                        }
                    }
                    else if (arrayDadosDoCliente[0] == "Tarefa")
                    {
                        //notifyIcon1.Icon = this.Icon;
                        //notifyIcon1.BalloonTipTitle = "TAREFA";
                        //notifyIcon1.BalloonTipText = $"{maquina} SOLICITOU UMA TAREFA";
                        //notifyIcon1.ShowBalloonTip(5);



                        //Responder a tarefa que o client deve efetuar, aqui
                        //No caso atual, ele deve ler o novo campo TAREFA da tabela MAQUINAS

                        //Devo fazer um select na tabela maquinas com os dados que recebi do client e retornar a tarefa.

                        string maquinaIP = arrayDadosDoCliente[2].Trim();
                        string nomeMaquina = arrayDadosDoCliente[1].Trim();

                        string tarefa = WebServiceLocal.wsBuscarTarefa(maquinaIP, subSecao, nomeMaquina);


                        if (!string.IsNullOrEmpty(tarefa))
                        {
                            if (tarefa == Enum.GetName(typeof(Tarefas), 2))
                            {
                                int tempoExtra = WebServiceLocal.wsObterTempoExtra(subSecao, nomeMaquina, maquinaIP);

                                tarefa = tarefa + ":" + tempoExtra.ToString();

                                string serverResponse = tarefa;
                                Byte[] sendBytes = Encoding.ASCII.GetBytes(serverResponse);
                                networkStream.Write(sendBytes, 0, sendBytes.Length);
                                networkStream.Flush();
                            }
                            else if (tarefa == Enum.GetName(typeof(Tarefas), 0))
                            {
                                //Enviar comando para desligar máquina
                                string serverResponse = "Recebido! " + dadosDoCliente;
                                Byte[] sendBytes = Encoding.ASCII.GetBytes(serverResponse);
                                networkStream.Write(sendBytes, 0, sendBytes.Length);
                                networkStream.Flush();
                            }
                            else if (tarefa == Enum.GetName(typeof(Tarefas), 1))
                            {
                                //Enviar comando para encerrar a sessão
                                string serverResponse = "Recebido! " + dadosDoCliente;
                                Byte[] sendBytes = Encoding.ASCII.GetBytes(serverResponse);
                                networkStream.Write(sendBytes, 0, sendBytes.Length);
                                networkStream.Flush();
                            }
                            else
                            {
                                string serverResponse = "Recebido! " + dadosDoCliente;
                                Byte[] sendBytes = Encoding.ASCII.GetBytes(serverResponse);
                                networkStream.Write(sendBytes, 0, sendBytes.Length);
                                networkStream.Flush();
                            }

                        }
                        else
                        {
                            string serverResponse = "Recebido! ";
                            Byte[] sendBytes = Encoding.ASCII.GetBytes(serverResponse);
                            networkStream.Write(sendBytes, 0, sendBytes.Length);
                            networkStream.Flush();
                        }
                    }
                    else
                    {
                        string serverResponse = "Recebido! " + dadosDoCliente;
                        Byte[] sendBytes = Encoding.ASCII.GetBytes(serverResponse);
                        networkStream.Write(sendBytes, 0, sendBytes.Length);
                        networkStream.Flush();
                    }

                }
                catch (Exception e)
                {
                    string erro = e.Message;
                    ServerListener.Stop();
                    ServerListener.Start();
                    Invoke(DelegateTeste_ModifyText, "Servidor aguardando conexões!");
                    clientSocket = ServerListener.AcceptTcpClient();
                    Invoke(DelegateTeste_ModifyText, "Servidor pronto!");
                }

            }
        }

        private void btnCadastrarMaquina01_Click(object sender, EventArgs e)
        {
            CadastroMaquinas cadastroMaquinas = new CadastroMaquinas(groupBox1, lblStatus1, btnCadastrarMaquina01);
            cadastroMaquinas.ShowDialog();
        }

        private void btnCadastrarMaquina02_Click(object sender, EventArgs e)
        {
            CadastroMaquinas cadastroMaquinas = new CadastroMaquinas(groupBox2, lblStatus2, btnCadastrarMaquina02);
            cadastroMaquinas.ShowDialog();
        }

        private void btnCadastrarMaquina03_Click(object sender, EventArgs e)
        {
            CadastroMaquinas cadastroMaquinas = new CadastroMaquinas(groupBox3, lblStatus3, btnCadastrarMaquina03);
            cadastroMaquinas.ShowDialog();
        }

        private void btnCadastrarMaquina04_Click(object sender, EventArgs e)
        {
            CadastroMaquinas cadastroMaquinas = new CadastroMaquinas(groupBox4, lblStatus4, btnCadastrarMaquina04);
            cadastroMaquinas.ShowDialog();
        }

        private void btnCadastrarMaquina05_Click(object sender, EventArgs e)
        {
            CadastroMaquinas cadastroMaquinas = new CadastroMaquinas(groupBox5, lblStatus5, btnCadastrarMaquina05);
            cadastroMaquinas.ShowDialog();
        }

        private void btnCadastrarMaquina06_Click(object sender, EventArgs e)
        {
            CadastroMaquinas cadastroMaquinas = new CadastroMaquinas(groupBox6, lblStatus6, btnCadastrarMaquina06);
            cadastroMaquinas.ShowDialog();
        }

        private void btnCadastrarMaquina07_Click(object sender, EventArgs e)
        {
            CadastroMaquinas cadastroMaquinas = new CadastroMaquinas(groupBox7, lblStatus7, btnCadastrarMaquina07);
            cadastroMaquinas.ShowDialog();
        }

        private void btnCadastrarMaquina08_Click(object sender, EventArgs e)
        {
            CadastroMaquinas cadastroMaquinas = new CadastroMaquinas(groupBox8, lblStatus8, btnCadastrarMaquina08);
            cadastroMaquinas.ShowDialog();
        }

        private void btnCadastrarMaquina09_Click(object sender, EventArgs e)
        {
            CadastroMaquinas cadastroMaquinas = new CadastroMaquinas(groupBox9, lblStatus9, btnCadastrarMaquina09);
            cadastroMaquinas.ShowDialog();
        }

        private void btnCadastrarMaquina10_Click(object sender, EventArgs e)
        {
            CadastroMaquinas cadastroMaquinas = new CadastroMaquinas(groupBox10, lblStatus10, btnCadastrarMaquina10);
            cadastroMaquinas.ShowDialog();
        }

        private void btnCadastrarMaquina11_Click(object sender, EventArgs e)
        {
            CadastroMaquinas cadastroMaquinas = new CadastroMaquinas(groupBox11, lblStatus11, btnCadastrarMaquina11);
            cadastroMaquinas.ShowDialog();
        }

        private void btnCadastrarMaquina12_Click(object sender, EventArgs e)
        {
            CadastroMaquinas cadastroMaquinas = new CadastroMaquinas(groupBox12, lblStatus12, btnCadastrarMaquina12);
            cadastroMaquinas.ShowDialog();
        }

        private void btnCadastrarMaquina13_Click(object sender, EventArgs e)
        {
            CadastroMaquinas cadastroMaquinas = new CadastroMaquinas(groupBox13, lblStatus13, btnCadastrarMaquina13);
            cadastroMaquinas.ShowDialog();
        }

        private void btnCadastrarMaquina14_Click(object sender, EventArgs e)
        {
            CadastroMaquinas cadastroMaquinas = new CadastroMaquinas(groupBox14, lblStatus14, btnCadastrarMaquina14);
            cadastroMaquinas.ShowDialog();
        }

        private void btnCadastrarMaquina15_Click(object sender, EventArgs e)
        {
            CadastroMaquinas cadastroMaquinas = new CadastroMaquinas(groupBox15, lblStatus15, btnCadastrarMaquina15);
            cadastroMaquinas.ShowDialog();
        }

        private void btnConfigurarSessoes_Click(object sender, EventArgs e)
        {
            List<string> listaDeMaquinas = new List<string>();

            BasicHttpsBinding binding = new BasicHttpsBinding();
            binding.Security.Mode = BasicHttpsSecurityMode.Transport;
            EndpointAddress endpoint = new EndpointAddress("https://wsoab.oabrs.org.br/Service.asmx");
            ServiceSoapClient WebService = new ServiceSoapClient(binding, endpoint);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            //BasicHttpBinding binding = new BasicHttpBinding();
            //binding.Security.Mode = BasicHttpSecurityMode.None;
            //binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            //EndpointAddress endpoint = new EndpointAddress("http://localhost:38115");
            //MaisUmaTentativaServer.ServiceReference2.ServiceSoapClient WebServiceLocal = new MaisUmaTentativaServer.ServiceReference2.ServiceSoapClient(binding, endpoint);
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            listaDeMaquinas = WebService.wsListagemDeMaquinas(subSecao).ToList();

            ConfigurarSessao configurarSessao = new ConfigurarSessao(listaDeMaquinas, subSecao);
            configurarSessao.ShowDialog();
        }

        private void btnFecharServidor_Click(object sender, EventArgs e)
        {
            var processes = Process.GetProcessesByName("MaisUmaTentativaServer");
            foreach (var item in processes)
            {
                item.Kill();
            }
        }

        private void btnUsuariosSistema_Click(object sender, EventArgs e)
        {

            //string endereco = "https://192.168.2.61/Service.asmx/"; //isso aqui esta certo, funciona... //Esta parte funciona com HTTPS
            //BasicHttpsBinding binding = new BasicHttpsBinding(BasicHttpsSecurityMode.Transport);
            //MaisUmaTentativaServer.ServiceReference2.ServiceSoapClient WebServiceLocal = new MaisUmaTentativaServer.ServiceReference2.ServiceSoapClient(binding, new EndpointAddress(endereco));

            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            //string aaa = string.Empty;
            //var tste = WebServiceLocal.wsNoticias(aaa);



            //BasicHttpBinding binding = new BasicHttpBinding();
            //binding.Security.Mode = BasicHttpSecurityMode.None;
            //binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            //EndpointAddress endpoint = new EndpointAddress("http://localhost:38115/");
            //MaisUmaTentativaServer.ServiceReference2.ServiceSoapClient WebServiceLocal = new MaisUmaTentativaServer.ServiceReference2.ServiceSoapClient(binding, endpoint);

            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            //string aaa = string.Empty;
            //string tste = WebServiceLocal.wsTeste(aaa);

        }
    }
}

