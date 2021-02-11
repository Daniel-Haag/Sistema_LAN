using System;
using System.Windows.Forms;
using System.ServiceModel;
using System.Net;
using MaisUmaTentativaServer.ServiceReference1;

namespace OABLanServer
{
    public partial class CadastroUsuarios : Form
    {
        private ServiceSoapClient WebService { get; set; }

        public CadastroUsuarios()
        {
            InitializeComponent();
            PopulaListaDeSubSecoes();
        }

        public void PopulaListaDeSubSecoes()
        {
            WebService = CriaObjetoConexaoWS();

            comboBoxSubSecao.DataSource = WebService.wsListagemDeSubSecoes();
        }

        private ServiceSoapClient CriaObjetoConexaoWS()
        {
            BasicHttpsBinding binding = new BasicHttpsBinding();
            binding.Security.Mode = BasicHttpsSecurityMode.Transport;
            EndpointAddress endpoint = new EndpointAddress("https://wsoab.oabrs.org.br/Service.asmx");
            ServiceSoapClient WebService = new ServiceSoapClient(binding, endpoint);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            return WebService;
        }

        private void btnCadastrar_Click(object sender, EventArgs e)
        {
            try
            {
                //Primeiro conferir se um dos dados não está em branco
                if (string.IsNullOrEmpty(txtNomeUsuario.Text) || string.IsNullOrEmpty(txtSenha.Text) || string.IsNullOrEmpty(comboBoxSubSecao.Text))
                {
                    notifyIcon1.Icon = this.Icon;
                    notifyIcon1.BalloonTipTitle = "Cadastro de Usuários";
                    notifyIcon1.BalloonTipText = $"Há pelo menos um campo em branco!";
                    notifyIcon1.ShowBalloonTip(20);
                }
                else
                {
                    WebService = CriaObjetoConexaoWS();

                    bool hasRows = false;
                    string mensagem = string.Empty;

                    mensagem = WebService.wsCadastrarUsuarioLanServerOAB(txtNomeUsuario.Text, txtSenha.Text, comboBoxSubSecao.Text, out hasRows);

                    if (hasRows)
                    {
                        //O registro que o usuário tentou cadastrar, já existe
                        notifyIcon1.Icon = this.Icon;
                        notifyIcon1.BalloonTipTitle = "Cadastro de Usuários";
                        notifyIcon1.BalloonTipText = $"Este usuário já está cadastrado!";
                        notifyIcon1.ShowBalloonTip(20);
                    }
                    else if (mensagem == "Cadastrado com sucesso.")
                    {
                        notifyIcon1.Icon = this.Icon;
                        notifyIcon1.BalloonTipTitle = "Cadastro de Usuários";
                        notifyIcon1.BalloonTipText = $"Novo usuário cadastrado com sucesso!";
                        notifyIcon1.ShowBalloonTip(20);
                    }
                }
            }
            catch (Exception except)
            {
                string erro = except.Message;
            }
        }
    }
}
