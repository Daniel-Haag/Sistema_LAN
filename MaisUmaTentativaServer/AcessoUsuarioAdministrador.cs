using MaisUmaTentativaServer.ServiceReference1;
using OABLanServer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MaisUmaTentativaServer
{
    public partial class AcessoUsuarioAdministrador : Form
    {
        private ServiceSoapClient WebService { get; set; }
        public AcessoUsuarioAdministrador()
        {
            InitializeComponent();
        }

        private void btnAcesso_Click(object sender, EventArgs e)
        {
            int Admin = 0;
            bool hasRows = false;

            Login login = new Login();

            WebService = CriaObjetoConexaoWS();
            WebService.wsLoginLanServerOAB(txtNomeUsuario.Text, txtSenha.Text, "", out Admin, out hasRows);

            if(Admin == 1)
            {
                //Pode acessar o gerenciador de máquinas
                this.Close();
                ExcluirMaquinasCadastradas excluirMaquinas = new ExcluirMaquinasCadastradas();
                excluirMaquinas.ShowDialog();
            }
        }

        private ServiceSoapClient CriaObjetoConexaoWS()
        {
            //BasicHttpBinding bindingLocal = new BasicHttpBinding();
            //bindingLocal.Security.Mode = BasicHttpSecurityMode.None;
            //bindingLocal.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            //EndpointAddress endpointLocal = new EndpointAddress("http://localhost:38115");
            //MaisUmaTentativaServer.ServiceReference2.ServiceSoapClient WebServiceLocal = new MaisUmaTentativaServer.ServiceReference2.ServiceSoapClient(bindingLocal, endpointLocal);
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            BasicHttpsBinding binding = new BasicHttpsBinding();
            binding.Security.Mode = BasicHttpsSecurityMode.Transport;
            EndpointAddress endpoint = new EndpointAddress("https://wsoab.oabrs.org.br/Service.asmx");
            ServiceSoapClient WebService = new ServiceSoapClient(binding, endpoint);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            return WebService;
        }

    }
}
