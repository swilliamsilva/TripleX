using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjetoTripleX
{
    public partial class FrmConsultarCeps : Form
    {
        public FrmConsultarCeps()
        {
            InitializeComponent();
        }

        private void FrmConsultarCeps_Load(object sender, EventArgs e)
        {

        }

        private void btnConsultar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCEP.Text))
            {
                MessageBox.Show("Informe um CEP válido...", Text,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            }
            else
            {
                string strURL = string.Format("https://viacep.com.br/ws/{0}/json/", txtCEP.Text.Trim());
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        var response = client.GetAsync(strURL).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            var result = response.Content.ReadAsStringAsync().Result;
                            Resultado res = JsonConvert.DeserializeObject<Resultado>(result);
                            if (res != null)
                            {
                                txtEstado.Text = res.Uf;
                                txtCidade.Text = res.Localidade;
                                txtBairro.Text = res.Bairro;
                                txtRua.Text = res.Logradouro;

                            }
                            if (res.CEP == null)
                            {
                                MessageBox.Show("CEP não encontrado.", Text,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, this.Text,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                }



            }
        }
    }
}
