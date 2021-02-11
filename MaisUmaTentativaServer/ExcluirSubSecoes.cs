using MaisUmaTentativaServer.ServiceReference1;
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
    public partial class ExcluirSubSecoes : Form
    {
        private ServiceSoapClient WebService { get; set; }
        private string mensagemDeErro = string.Empty;

        public ExcluirSubSecoes()
        {
            InitializeComponent();
            PopulaListaSubSecoes();
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

        public void PopulaListaSubSecoes()
        {
            try
            {
                WebService = CriaObjetoConexaoWS();

                comboBoxSubSecao.DataSource = WebService.wsListagemDeSubSecoes();
            }
            catch(Exception ex)
            {
                mensagemDeErro = ex.Message;
            }
        }

        private void btnExcluir_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(comboBoxSubSecao.Text))
                {
                    var resposta = MessageBox.Show($"A sub seção {comboBoxSubSecao.Text} será excluída! Tem certeza disso?", "ATENÇÃO", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button3);

                    if (resposta.ToString() == "Yes")
                    {
                        bool subSecaoExcluida = false;

                        WebService = CriaObjetoConexaoWS();
                        subSecaoExcluida = WebService.wsExcluirSubSecao(comboBoxSubSecao.Text);

                        if (subSecaoExcluida)
                        {
                            notifyIcon1.Icon = this.Icon;
                            notifyIcon1.BalloonTipTitle = "Exclusão de sub seções";
                            notifyIcon1.BalloonTipText = $"Sub seção excluída com sucesso!";
                            notifyIcon1.ShowBalloonTip(20);
                        }
                        else
                        {
                            notifyIcon1.Icon = this.Icon;
                            notifyIcon1.BalloonTipTitle = "Exclusão de sub seções";
                            notifyIcon1.BalloonTipText = $"Erro! Sub seção não excluída!";
                            notifyIcon1.ShowBalloonTip(20);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                mensagemDeErro = ex.Message;
            }

            this.Close();
        }
    }
}
