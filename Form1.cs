using FormsUser.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FormsUser.Model;
using System.Net.Http;
using System.Data.Entity;
using System.Security;
using System.Security.Cryptography;

namespace FormsUser
{
    public partial class Form1 : Form
    {
        public tipodocumento tipoDoc { get; set; }
        enum Estado
        {
            nuevo,
            modificar,

        }
        string hash = "f0xle@rn";
        Estado valor = Estado.nuevo;
        public Form1()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cargarTipoDocu();
        }
        private void cargarTipoDocu()
        {
            using (crudEntities2 db = new crudEntities2())
            {
                var lst = from d in db.tipodocumento select d;
                cmbTipoDoc.ValueMember = "tipodoc";
                cmbTipoDoc.DataSource = lst.ToList();
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                var val = Convert.ToInt32(txtNumDoc.Text);
                using (crudEntities2 db = new crudEntities2())
                {
                    var user = from d in db.suscriptor
                               where d.numerodocumento == val
                               select d;
                    if (user.Any())
                    {
                        txtApellido.Text = user.FirstOrDefault().apellido;
                        txtNombre.Text = user.FirstOrDefault().nombre;
                        txtContrasenia.Text = desenscriptarContrasenia(user.FirstOrDefault().passowrd);
                        txtDireccion.Text = user.FirstOrDefault().direccion;
                        txtNumDoc.Text = user.FirstOrDefault().numerodocumento.ToString();
                        txtMail.Text = user.FirstOrDefault().mail;
                        txtTelefono.Text = user.FirstOrDefault().telefono;
                        txtNomUs.Text = user.FirstOrDefault().usuario;
                        deshabilitarBotones();


                    }
                    else
                    {
                        MessageBox.Show("No existe el usuario");
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            txtNumDoc.ReadOnly = false;
            valor = Estado.nuevo;
            txtApellido.Text = "";
            txtNombre.Text = "";
            txtDireccion.Text = "";
            txtContrasenia.Text = "";
            txtTelefono.Text = "";
            txtMail.Text = "";
            txtNomUs.Text = "";
            habilitarBotones();


        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {

            if (valor == Estado.modificar)
            {
                modificar();
                MessageBox.Show("Se modifico correctamente el suscriptor.");
            }
            else if (valor == Estado.nuevo)
            {
                guardar();

            }

        }
        public void modificar()
        {
            var susc = buscarSuscriptor();
            actualizarSuscriptor(ref susc);
            using (crudEntities2 db = new crudEntities2())
            {
                db.Entry(susc).State = EntityState.Modified;
                db.SaveChanges();
            }


        }
        public void guardar()
        {
            try
            {

                suscripcion validar = validarSuscriptor();
                if (validar != null)
                {
                    using (crudEntities2 db = new crudEntities2())
                    {
                        using (var dbt = db.Database.BeginTransaction())
                        {
                            crearSuscripcion(validar, db, dbt);
                        }

                    }
                }
                else
                {
                    MessageBox.Show("Tiene una suscripción vigente.");

                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
        public void deshabilitarBotones()
        {
            foreach (Control cont in grpBox.Controls)
            {
                if (cont is TextBox)
                {
                    ((TextBox)cont).ReadOnly = true;
                    ((TextBox)cont).BackColor = SystemColors.Control;
                }
                else if (cont is ComboBox)
                {
                    ((ComboBox)cont).Enabled = false;
                    ((ComboBox)cont).BackColor = SystemColors.Control;
                }

            }
        }
        public void habilitarBotones()
        {
            foreach (Control cont in grpBox.Controls)
            {
                if (cont is TextBox)
                {
                    ((TextBox)cont).ReadOnly = false;
                    ((TextBox)cont).BackColor = SystemColors.Control;
                }
                else if (cont is ComboBox)
                {
                    ((ComboBox)cont).Enabled = true;
                    ((ComboBox)cont).BackColor = SystemColors.Control;
                }

            }
        }
        public suscripcion validarSuscriptor()
        {
            using (crudEntities2 db = new crudEntities2())
            {
                var numeroDoc = Convert.ToInt32(txtNumDoc.Text);
                var spr = new suscripcion();
                var res = from d in db.suscriptor where d.numerodocumento == numeroDoc select d;
                if (res.Any())
                {
                    spr = db.suscripcion.Where(m => m.idsuscriptor == res.FirstOrDefault().idsuscriptor)
                    .OrderByDescending(m => m.idasociacion)
                    .FirstOrDefault();

                    if (spr.idsuscriptor != 0 && spr.fechabaja != null)
                    {
                        return spr;//devolvemos la asociacion no vigente del suscriptor
                    }
                    else
                    {

                        return null;
                    }

                }
                suscriptor susc = crearSuscriptor();// suscriptor que no existe por lo tanto se crea una nueva suscripcion
                spr.suscriptor = susc;
                return spr;
            }
        }
        public int? devolverIdSuscriptor(int? idSus, crudEntities2 dbe)
        {

            var res = from d in dbe.suscriptor where d.idsuscriptor == idSus select d;
            if (res.Any())
            {
                return res.FirstOrDefault().idsuscriptor;
            }
            return null;

        }
        public void crearSuscripcion(suscripcion s, crudEntities2 dbe, DbContextTransaction dbt)
        {
            suscripcion susc = new suscripcion
            {
                fechaalta = DateTime.UtcNow,
                idsuscriptor = s.idsuscriptor,
                fechabaja = null,
                motivo = null,
                suscriptor = null
            };
            try
            {
                // Validamos que no exista el suscriptor en caso contrario se guarda el mismo en la Base de datos.
                s.suscriptor = s.idsuscriptor == 0 && s.suscriptor.numerodocumento != 0 ? dbe.suscriptor.Add(susc.suscriptor = s.suscriptor) : null;

                dbe.suscripcion.Add(susc);
                dbe.SaveChanges();
                dbt.Commit();

                MessageBox.Show("Se registró el usuario correctamente");
                deshabilitarBotones();

            }
            catch (Exception ex)
            {
                dbt.Rollback();
                throw ex;
            }




        }
        public suscriptor crearSuscriptor()
        {
            var susc = new suscriptor
            {
                nombre = txtNombre.Text,
                apellido = txtApellido.Text,
                mail = txtMail.Text,
                direccion = txtDireccion.Text,
                usuario = txtNomUs.Text,
                passowrd = encriptarContrasenia(txtContrasenia.Text),
                numerodocumento = Convert.ToInt32(txtNumDoc.Text),
                tipodoc = cmbTipoDoc.SelectedValue.ToString(),
                telefono = txtTelefono.Text,

            };
            return susc;
        }
        public string encriptarContrasenia(string con)
        {
            byte[] data = UTF8Encoding.UTF8.GetBytes(con);
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] keys = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(hash));
                using (TripleDESCryptoServiceProvider tdc = new TripleDESCryptoServiceProvider() { Key = keys, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                {
                    ICryptoTransform transform = tdc.CreateEncryptor();
                    byte[] results = transform.TransformFinalBlock(data, 0, data.Length);
                    return Convert.ToBase64String(results);
                }
            }
        }

        public string desenscriptarContrasenia(string con)
        {
            byte[] data = Convert.FromBase64String(con);
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] keys = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(hash));
                using (TripleDESCryptoServiceProvider tdc = new TripleDESCryptoServiceProvider() { Key = keys, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                {
                    ICryptoTransform transform = tdc.CreateDecryptor();
                    byte[] results = transform.TransformFinalBlock(data, 0, data.Length);
                    return UTF8Encoding.UTF8.GetString(results);
                }
            }
        }
        public suscriptor actualizarSuscriptor(ref suscriptor s)
        {

            s.nombre = txtNombre.Text;
            s.apellido = txtApellido.Text;
            s.mail = txtMail.Text;
            s.direccion = txtDireccion.Text;
            s.usuario = txtNomUs.Text;
            s.passowrd = encriptarContrasenia(txtContrasenia.Text);
            s.telefono = txtTelefono.Text;


            return s;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            valor = Estado.modificar;
            habilitarBotones();
            txtNumDoc.ReadOnly = true;
            cmbTipoDoc.Enabled = false;

        }
        public suscriptor buscarSuscriptor()
        {
            using (crudEntities2 db = new crudEntities2())
            {
                int numDoc = Convert.ToInt32(txtNumDoc.Text);
                var res = from d in db.suscriptor where d.numerodocumento == numDoc select d;
                if (res.Any())
                {
                    return res.FirstOrDefault();
                }
            }

            return null;
        }
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }

    public class Estado
    {
        public string Modificar { get; set; }
        public string Nuevo { get; set; }
    }
}
