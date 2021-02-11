using OABLanServer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MaisUmaTentativaServer
{
    public partial class Cadastros : Form
    {
        public Cadastros()
        {
            InitializeComponent();
        }

        private void btnCadastrarUsuarios_Click(object sender, EventArgs e)
        {
            CadastroUsuarios cadastroUsuarios = new CadastroUsuarios();
            cadastroUsuarios.ShowDialog();
            this.Close();
        }

        private void btnCadastrarSubSecoes_Click(object sender, EventArgs e)
        {
            CadastroSubSecoes cadastroSubSecoes = new CadastroSubSecoes();
            cadastroSubSecoes.ShowDialog();
            this.Close();
        }

        private void btnExcluirSubSecoes_Click(object sender, EventArgs e)
        {
            ExcluirSubSecoes excluirSubSecoes = new ExcluirSubSecoes();
            excluirSubSecoes.ShowDialog();
            this.Close();
        }

        private void btnExcluirUsuarios_Click(object sender, EventArgs e)
        {
            ExcluirUsuarios excluirUsuarios = new ExcluirUsuarios();
            excluirUsuarios.ShowDialog();
            this.Close();
        }

        private void btnExcluirMaquinas_Click(object sender, EventArgs e)
        {
            AcessoUsuarioAdministrador acesso = new AcessoUsuarioAdministrador();
            acesso.ShowDialog();

        }
    }
}
