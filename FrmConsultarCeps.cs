using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
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

        private void btnLimpar_Click(object sender, EventArgs e)
        {
            txtCEP.Text = string.Empty;
            txtEstado.Text = string.Empty;
            txtCidade.Text = string.Empty;
            txtBairro.Text = string.Empty;
            txtRua.Text = string.Empty;


        }

        private void btnFechar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection("Data Source=DESKTOP-7GR2V41;Initial Catalog=ResultadoConsultaViaCepDb;Integrated Security=True;Pooling=False");
            con.Open();
            var sql = "insert into RESCONSULTACEP(CEP, ESTADO, CIDADE, BAIRRO, RUA) values " + "(@CEP, @Estado, @Cidade, @Bairro, @Rua)";
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {

                    cmd.Parameters.AddWithValue("@CEP", txtCEP.Text);
                    cmd.Parameters.AddWithValue("@Estado", txtEstado.Text);
                    cmd.Parameters.AddWithValue("@Cidade", txtCidade.Text);
                    cmd.Parameters.AddWithValue("@Bairro", txtBairro.Text);
                    cmd.Parameters.AddWithValue("@Rua", txtRua.Text);
                    int b = cmd.ExecuteNonQuery();
                    if (b != 0)
                    {
                        MessageBox.Show(" O CEP foi cadastrado com exito!");
                    }
                    else
                    {
                        MessageBox.Show("Erro, no cadastro do CEP!");
                    }
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("Este CEP já existe gravado no arquivo!", Text,
                        // MessageBox.Show(ex.Message, this.Text,
                        MessageBoxButtons.OK);
            }
        
        }
    }
}
