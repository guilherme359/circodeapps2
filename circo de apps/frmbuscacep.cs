﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace circideapps
{
    public partial class frmbuscacep : Form
    {
        public frmbuscacep()
        {
            InitializeComponent();
        }

        private void btnfechar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnpesquisar_Click(object sender, EventArgs e)
        {
            String cep = txtcep.Text.Trim();
            if (cep.Length > 8 || cep.Length < 8)
            {
                MessageBox.Show("CEP invalido!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtcep.Text = string.Empty;
                this.ActiveControl = txtcep;
                return;

            }


            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://viacep.com.br/ws/" + txtcep.Text + "/json");
            request.AllowAutoRedirect = false;
            HttpWebResponse ChecaServidor = (HttpWebResponse)request.GetResponse();
            if (ChecaServidor.StatusCode != HttpStatusCode.OK)
            {
                MessageBox.Show("Servidor indisponivel!", "Erro HTTP", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            using (Stream webstream = ChecaServidor.GetResponseStream())
            {
                if (webstream != null)
                {
                    using (StreamReader responseReader = new StreamReader(webstream))
                    {
                        string response = responseReader.ReadToEnd();
                        response = Regex.Replace(response, "[{},]", string.Empty);
                        response = response.Replace("\"", "");
                        string[] substrings = response.Split('\n');
                        int cont = 0;
                        foreach (var substring in substrings)
                        {
                            if (cont == 1)
                            {
                                if (substring.Contains(":"))
                                {
                                    string[] valor = substring.Split(":".ToCharArray());
                                    if (valor.Length >= 2 && valor[0].Trim().Replace("\"", "") == "erro")
                                    {
                                        MessageBox.Show("CEP não encontrado!");
                                        txtcep.Text = string.Empty;
                                        txtcep.Focus();
                                        return;
                                    }
                                }
                            }
                            if (cont == 2)
                            {
                                string[] valor = substring.Split(":".ToCharArray());
                                lblendereco.Text = valor[1];
                            }
                            if (cont == 3)
                            {
                                string[] valor = substring.Split(":".ToCharArray());
                                lblcomplemento.Text = valor[1];
                            }
                            if (cont == 5)
                            {
                                string[] valor = substring.Split(":".ToCharArray());
                                lblbairro.Text = valor[1];
                            }
                            if (cont == 6)
                            {
                                string[] valor = substring.Split(":".ToCharArray());
                                lblcidade.Text = valor[1];
                            }
                            if (cont == 7)
                            {
                                string[] valor = substring.Split(":".ToCharArray());
                                lbluf.Text = valor[1];
                            }
                            cont++;
                        }
                    }
                }
            }
        }

        private void txtcep_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
                MessageBox.Show("Este campo aceita apenas números");
            }
        }
    }
}
