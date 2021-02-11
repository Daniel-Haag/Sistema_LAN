using System;
using System.Windows.Forms;
using System.ServiceModel;
using System.Net;
using MaisUmaTentativaServer.ServiceReference1;

namespace OABLanServer
{
    public partial class ExcluirUsuarios : Form
    {
        private ServiceSoapClient WebService { get; set; }

        public ExcluirUsuarios()
        {
            InitializeComponent();
            PopulaListaDeSubSecoes();
        }

        public void PopulaListaDeSubSecoes()
        {
            //WebService = CriaObjetoConexaoWS();
            WebService = CriaObjetoConexaoWS();

            comboBoxNomeUsuario.DataSource = WebService.wsListagemDeUsuarios();
        }

        private ServiceSoapClient CriaObjetoConexaoWS()
        {
            BasicHttpsBinding binding = new BasicHttpsBinding();
            binding.Security.Mode = BasicHttpsSecurityMode.Transport;
            EndpointAddress endpoint = new EndpointAddress("https://wsoab.oabrs.org.br/Service.asmx");
            ServiceSoapClient WebService = new ServiceSoapClient(binding, endpoint);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            //BasicHttpBinding bindingLocal = new BasicHttpBinding();
            //bindingLocal.Security.Mode = BasicHttpSecurityMode.None;
            //bindingLocal.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            //EndpointAddress endpointLocal = new EndpointAddress("http://localhost:38115");
            //MaisUmaTentativaServer.ServiceReference2.ServiceSoapClient WebServiceLocal = new MaisUmaTentativaServer.ServiceReference2.ServiceSoapClient(bindingLocal, endpointLocal);
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            return WebService;
        }

        private void btnExcluir_Click(object sender, EventArgs e)
        {
            bool usuarioExcluido = false;
            try
            {
                //Primeiro conferir se um dos dados não está em branco
                if (string.IsNullOrEmpty(comboBoxNomeUsuario.Text))
                {
                    notifyIcon1.Icon = this.Icon;
                    notifyIcon1.BalloonTipTitle = "Excluir Usuários";
                    notifyIcon1.BalloonTipText = $"Há pelo menos um campo em branco!";
                    notifyIcon1.ShowBalloonTip(20);
                }
                else
                {
                    //WebService = CriaObjetoConexaoWS();

                    WebService = CriaObjetoConexaoWS();

                    string usuario = string.Empty;
                    string subSecao = string.Empty;

                    string[] arrayUsuarioSubSecao = comboBoxNomeUsuario.Text.Split('-');
                    usuario = arrayUsuarioSubSecao[0].Trim();
                    subSecao = arrayUsuarioSubSecao[1].Trim();

                    usuarioExcluido = WebService.wsExcluirUsuario(usuario.Trim(), subSecao.Trim());

                    if (usuarioExcluido)
                    {
                        notifyIcon1.Icon = this.Icon;
                        notifyIcon1.BalloonTipTitle = "Excluir Usuários";
                        notifyIcon1.BalloonTipText = $"Usuário excluído com sucesso!";
                        notifyIcon1.ShowBalloonTip(20);
                    }
                    else
                    {
                        notifyIcon1.Icon = this.Icon;
                        notifyIcon1.BalloonTipTitle = "Excluir Usuários";
                        notifyIcon1.BalloonTipText = $"Erro, usuário não excluído!";
                        notifyIcon1.ShowBalloonTip(20);
                    }   
                }

                this.Close();
            }
            catch (Exception except)
            {
                string erro = except.Message;
            }
        }
    }
}
