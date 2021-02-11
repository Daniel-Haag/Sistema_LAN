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
using MaisUmaTentativaServer.Enums;
using MaisUmaTentativaServer.ServiceReference1;
using OABLanServer.Enums;

namespace OABLanServer
{
    public partial class ConfigurarSessao : Form
    {
        Conexao conexao = new Conexao();
        SqlCommand sqlcommand = new SqlCommand();
        string subSecao = string.Empty;


        public ConfigurarSessao(List<string> listaDeMaquinas, string subSecao)
        {
            InitializeComponent();

            this.subSecao = subSecao;
            this.comboBoxMaquinas.DataSource = listaDeMaquinas;
        }

        private void btnConfigurar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(comboBoxMaquinas.Text) || string.IsNullOrEmpty(txtMinutosSessao.Text))
            {
                notifyIcon1.Icon = this.Icon;
                notifyIcon1.BalloonTipTitle = "Configurador de sessão";
                notifyIcon1.BalloonTipText = "Há pelo menos um campo em branco!";
                notifyIcon1.ShowBalloonTip(20);
            }
            else
            {
                string mensagemErro = string.Empty;

                try
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

                    int minutosSessao = int.Parse(txtMinutosSessao.Text);
                    int minutosSessaoAtualizado = 0;
                    bool cadastroEfetuado = false;
                    int statusMaquina = 0;

                    //Primeiro testar se a máquina não encontra-se em status 2 - Box ocupado

                    statusMaquina = WebServiceLocal.wsRetornaStatusMaquina(subSecao, comboBoxMaquinas.Text);

                    if (statusMaquina == Convert.ToInt32(StatusMaquinas.Ocupado))
                    {
                        //Máquina ocupada, perguntar se deseja inserir mais tempo para a sessão
                        string resposta = MessageBox.Show("A máquina selecionada encontra-se ocupada, deseja configurar mais tempo para esta mesma sessão?", "Tempo extra", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information).ToString();

                        if (resposta == "Yes")
                        {
                            //Aqui deve-se efetuar um UPDATE na tabela MAQUINAS, campo TEMPOSESSAO
                            //Será inserido um novo campo na tabela MAQUINAS correspondente a TAREFA que ele deve realizar lblStatus.Text = Enum.GetName(typeof(StatusMaquinas), 1);
                            //Este UPDATE de tempo de sessão extra atualiza o campo TAREFA para a tarefa de inserir mais tempo
                            

                            cadastroEfetuado = WebServiceLocal.wsConfigurarSessaoTempoExtra(minutosSessao, comboBoxMaquinas.Text, Enum.GetName(typeof(Tarefas), 2), subSecao);


                            if (cadastroEfetuado)
                            {
                                notifyIcon1.Icon = this.Icon;
                                notifyIcon1.BalloonTipTitle = "Configurador de sessão";
                                notifyIcon1.BalloonTipText = "Tempo extra para sessão configurado com sucesso!";
                                notifyIcon1.ShowBalloonTip(20);
                                Close();
                            }
                            else
                            {
                                notifyIcon1.Icon = this.Icon;
                                notifyIcon1.BalloonTipTitle = "Configurador de sessão";
                                notifyIcon1.BalloonTipText = "Houve um erro na configuração.";
                                notifyIcon1.ShowBalloonTip(20);
                            }
                        }
                    }
                    else
                    {
                        cadastroEfetuado = WebService.wsConfigurarSessao(minutosSessao, comboBoxMaquinas.Text, subSecao, out minutosSessaoAtualizado);

                        if (cadastroEfetuado)
                        {
                            notifyIcon1.Icon = this.Icon;
                            notifyIcon1.BalloonTipTitle = "Configurador de sessão";
                            notifyIcon1.BalloonTipText = "Sessão configurada com sucesso!";
                            notifyIcon1.ShowBalloonTip(20);
                            Close();
                        }
                        else
                        {
                            notifyIcon1.Icon = this.Icon;
                            notifyIcon1.BalloonTipTitle = "Configurador de sessão";
                            notifyIcon1.BalloonTipText = "Houve um erro na configuração.";
                            notifyIcon1.ShowBalloonTip(20);
                        }
                    }

                }
                catch (Exception excpt)
                {
                    mensagemErro = excpt.Message;
                }

            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }
    }
}
