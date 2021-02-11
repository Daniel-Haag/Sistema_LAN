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
    public partial class CadastroSubSecoes : Form
    {
        public CadastroSubSecoes()
        {
            InitializeComponent();
        }

        private void btnCadastrar_Click(object sender, EventArgs e)
        {
            string mensagemDeErro = string.Empty;
            bool cadastrado = false;
            bool subSecaoExistente = false;

            if (!string.IsNullOrEmpty(txtNomeSubSecao.Text))
            {
                try
                {
                    BasicHttpsBinding binding = new BasicHttpsBinding();
                    binding.Security.Mode = BasicHttpsSecurityMode.Transport;
                    EndpointAddress endpoint = new EndpointAddress("https://wsoab.oabrs.org.br/Service.asmx");
                    ServiceSoapClient WebService = new ServiceSoapClient(binding, endpoint);
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    cadastrado = WebService.wsCadastrarSubSecao(txtNomeSubSecao.Text.Trim(), out subSecaoExistente);

                    if (cadastrado)
                    {
                        notifyIcon1.Icon = this.Icon;
                        notifyIcon1.BalloonTipTitle = "Cadastro de Sub Seções";
                        notifyIcon1.BalloonTipText = $"Cadastro realizado com sucesso!";
                        notifyIcon1.ShowBalloonTip(20);

                        cadastrado = false;

                        this.Close();
                    }
                    else if (subSecaoExistente)
                    {
                        notifyIcon1.Icon = this.Icon;
                        notifyIcon1.BalloonTipTitle = "Cadastro de Sub Seções";
                        notifyIcon1.BalloonTipText = $"Sub Seção já cadastrada!";
                        notifyIcon1.ShowBalloonTip(20);

                        subSecaoExistente = false;
                    }
                    else
                    {
                        notifyIcon1.Icon = this.Icon;
                        notifyIcon1.BalloonTipTitle = "Cadastro de Sub Seções";
                        notifyIcon1.BalloonTipText = $"Erro. Cadastro não efetuado.";
                        notifyIcon1.ShowBalloonTip(20);
                    }
                }
                catch (Exception ex)
                {
                    mensagemDeErro = ex.Message;
                }
            }
            else
            {
                notifyIcon1.Icon = this.Icon;
                notifyIcon1.BalloonTipTitle = "Cadastro de Sub Seções";
                notifyIcon1.BalloonTipText = $"Nenhum nome de sub seção foi inserido!";
                notifyIcon1.ShowBalloonTip(20);
            }
        }
    }
}
