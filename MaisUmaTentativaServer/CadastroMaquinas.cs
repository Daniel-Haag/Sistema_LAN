using OABLanServer.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaisUmaTentativaServer.ServiceReference1;

namespace OABLanServer
{
    public partial class CadastroMaquinas : Form
    {
        Conexao conexao = new Conexao();
        SqlCommand sqlCommand = new SqlCommand();
        GroupBox groupBox;
        Label lblStatus;
        Button btnCadastrarMaquina;
        string mensagemDeErro;


        public CadastroMaquinas(GroupBox groupBox, Label lblStatus, Button btnCadastrarMaquina)
        {
            this.groupBox = groupBox;
            this.lblStatus = lblStatus;
            this.btnCadastrarMaquina = btnCadastrarMaquina;

            InitializeComponent();
        }

        public void btnCadastrarMaquinas_Click(object sender, EventArgs e)
        {

            if (CadastrarMaquina())
            {
                groupBox.BackColor = Color.LightGreen;
                lblStatus.Text = Enum.GetName(typeof(StatusMaquinas), 1);
                btnCadastrarMaquina.Visible = false;
            }
            else
            {
                
            }
        }

        private bool CadastrarMaquina()
        {
            if (string.IsNullOrEmpty(textIPmaquina.Text) || string.IsNullOrEmpty(comboBoxSubSecao.Text) || string.IsNullOrEmpty(textTempoSessao.Text))
            {
                notifyIcon1.Icon = this.Icon;
                notifyIcon1.BalloonTipTitle = "Cadastro de Máquinas";
                notifyIcon1.BalloonTipText = $"Há pelo menos um campo em branco!";
                notifyIcon1.ShowBalloonTip(20);
                return false;
            }
            else
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

                string mensagem = string.Empty;

                mensagem = WebService.wsCadastrarMaquinaLanServerOAB(textIPmaquina.Text, comboBoxSubSecao.Text, textTempoSessao.Text, Convert.ToInt32(StatusMaquinas.Livre), groupBox.Text);

                try
                {
                    if (mensagem == "Máquina já cadastrada.")
                    {
                        //Se tem linhas, então esta máquina já está cadastrada

                        notifyIcon1.Icon = this.Icon;
                        notifyIcon1.BalloonTipTitle = "Cadastro de Máquinas";
                        notifyIcon1.BalloonTipText = $"Esta máquina já está cadastrada!";
                        notifyIcon1.ShowBalloonTip(20);

                        Close();

                        return false;
                    }
                    else
                    {
                        if(mensagem == "Tipo de dado inválido.")
                        {
                            notifyIcon1.Icon = this.Icon;
                            notifyIcon1.BalloonTipTitle = "Cadastro de Máquinas";
                            notifyIcon1.BalloonTipText = $"Tipo de dados inserido, inválido!";
                            notifyIcon1.ShowBalloonTip(20);

                            return false;
                        }

                        if(mensagem == "Cadastrado com sucesso.")
                        {
                            notifyIcon1.Icon = this.Icon;
                            notifyIcon1.BalloonTipTitle = "Cadastro de Máquinas";
                            notifyIcon1.BalloonTipText = $"Máquina cadastrada com sucesso!";
                            notifyIcon1.ShowBalloonTip(20);
                        }
                        
                        Close();

                        return true;
                    }

                }
                catch (Exception s)
                {
                    mensagemDeErro = s.Message;
                    return false;
                }
            }
        }
    }
}
