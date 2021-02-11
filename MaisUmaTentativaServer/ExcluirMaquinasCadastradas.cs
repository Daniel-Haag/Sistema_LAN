using MaisUmaTentativaServer.ServiceReference2;
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
    public partial class ExcluirMaquinasCadastradas : Form
    {
        private ServiceSoapClient WebService { get; set; }

        public ExcluirMaquinasCadastradas()
        {
            InitializeComponent();

            WebService = CriaObjetoConexaoWS();

            string[] listagemCompleta = WebService.wsListagemDeMaquinasCompleta();

            for (int i = 0; i < listagemCompleta.Length; i++)
            {

                string[] arrayDados = listagemCompleta[i].Split(':');

                ListViewItem listViewItem = new ListViewItem(new string[] { arrayDados[0].Trim(), arrayDados[1].Trim(), arrayDados[2].Trim(), arrayDados[3].Trim(), arrayDados[4].Trim(), arrayDados[5].Trim() });

                listaDeMaquinas.Items.Add(listViewItem);
            }


        }

        private void btnExcluir_Click(object sender, EventArgs e)
        {
            string executarExclusao = string.Empty;

            if (listaDeMaquinas.SelectedItems.Count > 0)
            {
                executarExclusao = MessageBox.Show("A máquina selecionada será excluída! Se esta máquina estiver ativa no sistema, a exclusão da mesma poderá ocasionar erros! Você tem certeza disso?", "ATENÇÃO", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1).ToString();

                if (executarExclusao == "Yes")
                {
                    string IPMaquina = string.Empty;
                    string subSecao = string.Empty;
                    string numeroMaquina = string.Empty;
                    string nomeMaquina = string.Empty;
                    string dadosConcatenados = string.Empty;
                    bool linhaExcluida = false;

                    for (int i = 0; i < listaDeMaquinas.SelectedItems[0].SubItems.Count; i++)
                    {
                        dadosConcatenados += listaDeMaquinas.SelectedItems[0].SubItems[i].Text + ":";
                    }

                    string[] arrayItens = dadosConcatenados.Split(':');

                    IPMaquina = arrayItens[0];
                    subSecao = arrayItens[1];
                    numeroMaquina = arrayItens[4];
                    nomeMaquina = arrayItens[5];

                    WebService = CriaObjetoConexaoWS();
                    linhaExcluida = WebService.wsExcluirMaquina(IPMaquina.Trim(), subSecao.Trim(), numeroMaquina.Trim(), nomeMaquina.Trim());

                    if (linhaExcluida)
                    {
                        notifyIcon1.Icon = this.Icon;
                        notifyIcon1.BalloonTipTitle = "Excluir Máquinas";
                        notifyIcon1.BalloonTipText = $"Máquina excluida com sucesso!";
                        notifyIcon1.ShowBalloonTip(20);

                        listaDeMaquinas.Items.Remove(listaDeMaquinas.SelectedItems[0]);

                        this.Dispose();
                    }
                    else
                    {
                        notifyIcon1.Icon = this.Icon;
                        notifyIcon1.BalloonTipTitle = "Excluir Máquinas";
                        notifyIcon1.BalloonTipText = $"Erro! Máquina não excluída.";
                        notifyIcon1.ShowBalloonTip(20);
                        this.Dispose();
                    }
                }
            }
        }

        private ServiceSoapClient CriaObjetoConexaoWS()
        {
            BasicHttpBinding bindingLocal = new BasicHttpBinding();
            bindingLocal.Security.Mode = BasicHttpSecurityMode.None;
            bindingLocal.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            EndpointAddress endpointLocal = new EndpointAddress("http://localhost:38115");
            MaisUmaTentativaServer.ServiceReference2.ServiceSoapClient WebServiceLocal = new MaisUmaTentativaServer.ServiceReference2.ServiceSoapClient(bindingLocal, endpointLocal);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            //BasicHttpsBinding binding = new BasicHttpsBinding();
            //binding.Security.Mode = BasicHttpsSecurityMode.Transport;
            //EndpointAddress endpoint = new EndpointAddress("https://wsoab.oabrs.org.br/Service.asmx");
            //ServiceSoapClient WebService = new ServiceSoapClient(binding, endpoint);
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            return WebServiceLocal;
        }
    }
}
