using System;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.ServiceModel;
using System.Net;
using MaisUmaTentativaServer.ServiceReference1;
using MaisUmaTentativaServer;

namespace OABLanServer
{
    public partial class Login : Form
    {
        Conexao conexao = new Conexao();
        SqlCommand sqlCommand = new SqlCommand();


        public Login()
        {
            InitializeComponent();
            populaListaDeSubSecoes();
        }

        public void populaListaDeSubSecoes()
        {
            string mensagemDeErro = string.Empty;
            try
            {
                BasicHttpsBinding binding = new BasicHttpsBinding();
                binding.Security.Mode = BasicHttpsSecurityMode.Transport;
                EndpointAddress endpoint = new EndpointAddress("https://wsoab.oabrs.org.br/Service.asmx");
                ServiceSoapClient WebService = new ServiceSoapClient(binding, endpoint);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                comboBoxSubSecao.DataSource = WebService.wsListagemDeSubSecoes();
            }
            catch(Exception ex)
            {
                mensagemDeErro = ex.Message;
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                BasicHttpsBinding binding = new BasicHttpsBinding();
                binding.Security.Mode = BasicHttpsSecurityMode.Transport;
                EndpointAddress endpoint = new EndpointAddress("https://wsoab.oabrs.org.br/Service.asmx");
                ServiceSoapClient WebService = new ServiceSoapClient(binding, endpoint);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                var Admin = 0;
                var hasRows = false;

                ExecutarLogin(WebService, ref Admin, ref hasRows);
            }
            catch (Exception ex)
            {
                string mensagemDeErro = ex.Message;
            }
        }

        public void ExecutarLogin(ServiceSoapClient WebService, ref int Admin, ref bool hasRows)
        {
            if (txtNomeUsuario.Text == "Administrador")
            {
                if (string.IsNullOrEmpty(txtNomeUsuario.Text) || string.IsNullOrEmpty(txtSenha.Text))
                {
                    notifyIcon1.Icon = this.Icon;
                    notifyIcon1.BalloonTipTitle = "LOGIN";
                    notifyIcon1.BalloonTipText = "Há pelo menos um campo não preenchido!";
                    notifyIcon1.ShowBalloonTip(20);
                }
                else
                {
                    if (txtNomeUsuario.Text == "Administrador")
                    {
                        //Lógica para login de usuário administrador
                        LoginUsuarioAdministrador(WebService, out Admin, out hasRows);
                    }
                    else
                    {
                        //Lógica para usuário comum
                        LoginUsuarioComum(WebService, out Admin, out hasRows);
                    }
                }
            }
            else
            {
                if (string.IsNullOrEmpty(txtNomeUsuario.Text) || string.IsNullOrEmpty(txtSenha.Text) || string.IsNullOrEmpty(comboBoxSubSecao.Text))
                {
                    notifyIcon1.Icon = this.Icon;
                    notifyIcon1.BalloonTipTitle = "LOGIN";
                    notifyIcon1.BalloonTipText = "Há pelo menos um campo não preenchido!";
                    notifyIcon1.ShowBalloonTip(20);
                }
                else
                {
                    if (txtNomeUsuario.Text == "Administrador")
                    {
                        //Lógica para login de usuário administrador
                        LoginUsuarioAdministrador(WebService, out Admin, out hasRows);
                    }
                    else
                    {
                        //Lógica para usuário comum
                        LoginUsuarioComum(WebService, out Admin, out hasRows);
                    }
                }
            }
        }

        public void LoginUsuarioAdministrador(ServiceSoapClient WebService, out int Admin, out bool hasRows)
        {
            WebService.wsLoginLanServerOAB(txtNomeUsuario.Text, txtSenha.Text, "", out Admin, out hasRows);

            if (Admin == 1)
            {
                //Pode logar como administrador
                Form1 form1 = new Form1(txtNomeUsuario.Text, Admin);
                form1.ShowDialog();
                Close();
            }
        }

        private void LoginUsuarioComum(ServiceSoapClient WebService, out int Admin, out bool hasRows)
        {
            WebService.wsLoginLanServerOAB(txtNomeUsuario.Text, txtSenha.Text, comboBoxSubSecao.Text, out Admin, out hasRows);

            if (hasRows)
            {
                IPAddress enderecoIP = Form1.obterIP();
                string IP = enderecoIP.MapToIPv4().ToString();
                //Cadastrar ou atualizar IP na tabela de servidores

                WebService.wsAtualizarIPservidor(comboBoxSubSecao.Text, IP);
                //Se tem linha, pode logar
                Form1 form1 = new Form1(txtNomeUsuario.Text, comboBoxSubSecao.Text);
                form1.ShowDialog();
                this.Visible = false;

            }
            else
            {
                notifyIcon1.Icon = this.Icon;
                notifyIcon1.BalloonTipTitle = "LOGIN";
                notifyIcon1.BalloonTipText = "Usuário não cadastrado!";
                notifyIcon1.ShowBalloonTip(20);
            }
        }

        private void btnCadastro_Click(object sender, EventArgs e)
        {
            Cadastros cadastros = new Cadastros();
            cadastros.ShowDialog();

            //CadastroUsuarios cadastroUsuarios = new CadastroUsuarios();
            //cadastroUsuarios.ShowDialog();
        }

        public void AtualizarIPservidorNoBanco()
        {
            //Atualizar os dados do servidor na tabela ServidoresSubSecoes
            //Na primeira vez que o servidor for inicializado sera feito um INSERT nos campos SubSecao e
            //IPServidorSubSecao
            //O campo subSecao, será informado pelo usuário
            //O campo IPServidorSubSecao será o retorno do método "obterIP"
        }

        /// <summary>
        /// Método chamado para desabilitar o comboBox de subSeções, caso o usuário esteja realizando login como Administrador
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DesabilitaComboBoxSubSecao(object sender, EventArgs e)
        {
            if (txtNomeUsuario.Text == "Administrador")
            {
                comboBoxSubSecao.Enabled = false;
            }
            else
            {
                comboBoxSubSecao.Enabled = true;
            }
        }
    }
}
