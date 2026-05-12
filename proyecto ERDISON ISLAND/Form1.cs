using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static System.ComponentModel.Design.ObjectSelectorEditor;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.Design.AxImporter;
namespace proyecto_ERDISON_ISLAND
{
    public partial class Form1 : Form
    {
        SqlConnection conexion;
        int hh;
        int rr;
        bool accion;

        decimal tp = 0;
        int cp = 0;

        //contenedores de factura
        decimal total;
        int nn;
        bool enFac = false;
        bool enFac2 = false;  //ahora es personal

        //contenedores de factura
        decimal totalR;
        int nnR;
        bool enFacR = false;
        bool enFac2R = false;  //ahora es personal

        DataTable dtD = new DataTable();
        DataTable dtDR = new DataTable();




        public Form1()
        {
            InitializeComponent();
            accion = false;

            this.AutoScaleMode = AutoScaleMode.Dpi;



            try
            {
                conexion = new SqlConnection(

                    "Server=(localdb)\\MSSQLLocalDB;Database=BDDProyecto;Trusted_Connection=True;"

                );

                conexion.Open();
            }
            catch
            {
                conexion = new SqlConnection(

                    "Server=(localdb)\\MSSQLLocalDB;Database=loQueComo;Trusted_Connection=True;"

                );

                conexion.Open();
            }



        }

        private void Form1_Load(object sender, EventArgs e)

        {
            hh = 1;
            rr = 1;
            asignarEnlaces();
            CargarDatos();
            CargarStock();
            //RedondearPaneles

            ActualizarAlertas();
            RecorrerControles(this);




        }





        public void CargarDatos()
        {
            string query = "SELECT Id, Cliente, Total FROM inicio";


            try
            {
                SqlDataAdapter da = new SqlDataAdapter(query, conexion);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;

                // Cambiar nombres visibles
                dataGridView1.Columns["Id"].HeaderText = "ID";
                dataGridView1.Columns["Cliente"].HeaderText = "Cliente";
                dataGridView1.Columns["Total"].HeaderText = "Total ($)";

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

        private void CargarStock()
        {
            //using (SqlConnection conexion = new SqlConnection("Server=(localdb)\\MSSQLLocalDB;Database=BDDProyecto;Trusted_Connection=True;"))

            string consulta = "SELECT nombre, stock FROM productos WHERE Stock <= 5";

            SqlDataAdapter da = new SqlDataAdapter(consulta, conexion);
            DataTable dt = new DataTable();
            da.Fill(dt);

            dataGridView2.DataSource = dt;

        }

        private void ActualizarAlertas()
        {
            string consulta = "SELECT COUNT(*) FROM productos WHERE Stock <= 5";

            SqlCommand cmd = new SqlCommand(consulta, conexion);

            int cantidadAlertas = (int)cmd.ExecuteScalar();

            label23.Text = cantidadAlertas.ToString();

        }













        //------------------------------------------------------------------------------------//------------------------------------------------------------------------------------
        //------------------------------------Redondear---------------------------------------//------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------//------------------------------------------------------------------------------------
        public void RedondearControl(Control control, int radio)
        {
            GraphicsPath path = new GraphicsPath();

            path.AddArc(0, 0, radio, radio, 180, 90);
            path.AddArc(control.Width - radio, 0, radio, radio, 270, 90);
            path.AddArc(control.Width - radio, control.Height - radio, radio, radio, 0, 90);
            path.AddArc(0, control.Height - radio, radio, radio, 90, 90);

            path.CloseFigure();
            control.Region = new Region(path);
        }

        //------------------------------------------------------------------------------------//------------------------------------------------------------------------------------


        private void RecorrerControles(Control padre)
        {
            foreach (Control cc in padre.Controls)
            {
                if ((cc.Tag as string) == "radio") //(cc is Panel || cc is Button) &&
                {
                    RedondearControl(cc, 20);
                }

                if (cc.HasChildren)
                {
                    RecorrerControles(cc);
                }
            }
        }
        //------------------------------------------------------------------------------------//------------------------------------------------------------------------------------
        //-------------------------------------Redondear--------------------------------------//------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------//------------------------------------------------------------------------------------


        //------------------------------------------------------------------------------------//------------------------------------------------------------------------------------
        //-------------------------Menu de navegacion Principal-------------------------------//------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------//------------------------------------------------------------------------------------
        private void asignarEnlaces()
        {
            foreach (Control c in Menu.Controls)
            {
                asignarEvento(c, CambiarPestaÒa_click);
            }

        }

        //------------------------------------------------------------------------------------//------------------------------------------------------------------------------------


        private void asignarEvento(Control c, EventHandler e)
        {
            c.Click += e;

            foreach (Control cc in c.Controls)
            {
                cc.Click += e;
            }
        }

        //------------------------------------------------------------------------------------//------------------------------------------------------------------------------------


        private void CambiarPestaÒa_click(object sender, EventArgs e)
        {
            Control c = (Control)sender;
            if (accion == false)
            {
                switch (c.Tag)
                {
                    case "facturacion":
                        ptFacturacion.BringToFront();
                        break;

                    case "inventario":
                        ptInventario.BringToFront();
                        break;

                    case "inicio":
                        ptInicio.BringToFront();
                        break;

                    case "analisis":
                        ptAnalisis.BringToFront();
                        break;

                }
            }
            else
            {
                MessageBox.Show("se esta ejecutando una accion");
            }

        }
        //------------------------------------------------------------------------------------//------------------------------------------------------------------------------------
        //-------------------------Menu de navegacion Principal-------------------------------//------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------//------------------------------------------------------------------------------------



        //------------------------------------------------------------------------------------//------------------------------------------------------------------------------------
        //--------------------------------Crear Grafico---------------------------------------//------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------//------------------------------------------------------------------------------------
        private Chart CrearGrafico(string n1, int i1, string n2, int i2, string n3, int i3)
        {
            Chart chart = new Chart();
            chart.Dock = DockStyle.Fill;

            ChartArea area = new ChartArea();
            chart.ChartAreas.Add(area);

            Series serie = new Series();
            serie.ChartType = SeriesChartType.Pie;

            serie.Points.AddXY(n1, i1);
            serie.Points.AddXY(n2, i2);
            serie.Points.AddXY(n3, i3);

            chart.Series.Add(serie);

            return chart;

        }
        //------------------------------------------------------------------------------------//------------------------------------------------------------------------------------
        //--------------------------------Crear Grafico---------------------------------------//------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------//------------------------------------------------------------------------------------



        //------------------------------------------------------------------------------------//------------------------------------------------------------------------------------
        //--------------------------------Crear Tabla-----------------------------------------//------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------//------------------------------------------------------------------------------------

        private DataGridView CTabla(string nombre_tabla, string filtro = null, string t1 = null, DataGridViewCellEventHandler evento = null)
        {
            //-----------------------------------------//
            string query;
            if (filtro == null)
            {
                query = $"SELECT {t1} FROM {nombre_tabla}";
            }
            else
            {
                query = $"SELECT {t1} FROM {nombre_tabla} WHERE nombre LIKE @t";
            }



            SqlDataAdapter da = new SqlDataAdapter(query, conexion);
            da.SelectCommand.Parameters.AddWithValue("@t", "%" + (filtro ?? "") + "%");

            DataTable dt = new DataTable();
            da.Fill(dt);

            //-----------------------------------------//

            DataGridView dgv = new DataGridView()
            {
                SelectionMode = DataGridViewSelectionMode.FullRowSelect, //En vez de seleccionar una celda azul. selecciona toda la fila.
                MultiSelect = false, //solo deja seleccionar una fila
                RowHeadersVisible = false, //elimina la primera columna de la izquierda
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, //auto relleno
                Font = new System.Drawing.Font("Arial", 14),
                BackgroundColor = Color.White,
                ColumnHeadersHeight = 40,
                AllowUserToAddRows = false,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                AllowUserToResizeColumns = false,
                AllowUserToResizeRows = false,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle()
                {
                    Font = new System.Drawing.Font("Arial", 18, FontStyle.Bold),
                },

                Location = new Point(0, 0),
                Dock = DockStyle.Fill,
                DataSource = dt
            };
            if (evento != null)
            {
                dgv.CellClick += evento;
            }

            return dgv;

        }

        //------------------------------------------------------------------------------------//------------------------------------------------------------------------------------
        //--------------------------------Crear Tabla-----------------------------------------//------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------//------------------------------------------------------------------------------------



        private void ptFacturacion_Paint(object sender, PaintEventArgs e)
        {

        }
        //------------------------------------------------------------------------------------//------------------------------------------------------------------------------------
        //--------------------------------PestaÒa Factura-------------------------------------//------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------//------------------------------------------------------------------------------------


        private void Buscador1_Click(object sender, EventArgs e)
        {


            if (hh == 1)
            {
                crearFactura(1);
                Buscador1.Text = "Cancelar";
                Buscador1.BackColor = Color.Black;
                textBox1.Visible = true;
                textBox1.Focus();
                tt1.Width = tt1.Width / 2;
                tt2.Width = tt2.Width / 2;
                hh = 2;
                RedondearControl(tt1, 20);
                ttFacturacion.Location = new Point(20, 20);
                ptProductos.Location = new Point(tt1.Left + tt1.Width + 20, 37);
                ptProductos.Width = 600;
                tablaF.Width = 590;
                SubirFactura.Visible = true;
                tablaF.Controls.Add(CTabla("productos", textBox1.Text, "nombre, precio", CSubirProducto));
                accion = true;
            }
            else
            {
                Buscador1.Text = "";
                Buscador1.BackColor = Color.White;
                textBox1.Visible = false;
                tt1.Width = tt1.Width * 2;
                tt2.Width = tt2.Width * 2;
                hh = 1;
                RedondearControl(tt1, 20);
                ttFacturacion.Location = new Point(300, 20);
                ptProductos.Location = new Point(1100, 37);
                ptProductos.Width = 48;
                tablaF.Width = 40;
                SubirFactura.Visible = false;
                tablaF.Controls.Clear();
                accion = false;
                crearFactura(4);
                DtId.Text = "00";
                Dtcantidad.Text = "0000";
                Dttotal.Text = "00000.00";
                tp = 0;
                cp = 0;
            }

        }

        //------------------------------------------------------------------------------------//------------------------------------------------------------------------------------


        private void CSubirProducto(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            string t = ((DataGridView)sender).Rows[e.RowIndex].Cells["nombre"].Value.ToString();
            string j = ((DataGridView)sender).Rows[e.RowIndex].Cells[1].Value.ToString();

            int cantidadP;

            SqlCommand cmd = new SqlCommand(
                "SELECT stock FROM productos WHERE nombre = @nombre",
                conexion
            );

            cmd.Parameters.AddWithValue("@nombre", t);

            cantidadP = Convert.ToInt32(cmd.ExecuteScalar());


            Panel pnl = new Panel()
            {
                Size = new Size(300, 150),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Tag = "pnlC"
            };

            pnl.Location = new Point(
            (this.ClientSize.Width - pnl.Width) / 2,
            (this.ClientSize.Height - pnl.Height) / 2);

            TextBox txt = new TextBox()
            {
                Size = new Size(135, 23),
                TextAlign = HorizontalAlignment.Center,
                Location = new Point(115, 70)
            };

            Label lbl1 = new Label()
            {
                AutoSize = false,
                Text = $"Se a seleccionado {t} para subir a la factura actual",
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(200, 35),
                Location = new Point(50, 25)
            };

            Label lbl2 = new Label()
            {
                AutoSize = false,
                Text = "Cantidad",
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(60, 23),
                Location = new Point(50, 70)
            };

            Button btn = new Button()
            {
                Text = "Confirmar",
                Size = new Size(200, 29),
                Location = new Point(50, 100)
            };

            Button btns = new Button()
            {
                Text = "Confirmar",
                Size = new Size(20, 20),
                Location = new Point(280, 0)
            };

            btn.Click += (s, e) =>
            {

                if (!int.TryParse(txt.Text, out int numero) || txt.Text == "0")
                {
                    MessageBox.Show("Valor no permitido");
                    txt.Text = "";
                    txt.Focus();
                    return;
                }

                if (cantidadP - int.Parse(txt.Text) <= 1)
                {
                    MessageBox.Show($"no hay suficiente stock. \n revise inventario \nST: {cantidadP}, {int.Parse(txt.Text)}");
                    txt.Text = "";
                    txt.Focus();
                    return;
                }





                string texto;
                string cantidad = txt.Text;
                string tt = t + " |" + cantidad;

                texto = tt.PadRight(30 - j.Length, '-')
                + j + "\n \n";

                lblViewFactura.Text += texto;

                ptFacturacion.Controls.Remove(pnl);

                pnl.Dispose();

                crearFactura(2, t, decimal.Parse(j), int.Parse(cantidad));
                cp = cp + 1;
                Dtcantidad.Text = "0000";
                Dtcantidad.Text += cp;

                tp = tp + decimal.Parse(j);

                Dttotal.Text = tp.ToString();
            };

            btns.Click += (s, e) =>
            {
                ptFacturacion.Controls.Remove(pnl);
                pnl.Dispose();
            };

            pnl.Controls.Add(txt);
            pnl.Controls.Add(btn);
            pnl.Controls.Add(lbl1);
            pnl.Controls.Add(lbl2);
            pnl.Controls.Add(btns);

            ptFacturacion.Controls.Add(pnl);

            pnl.BringToFront();

            txt.Focus();

        }

        //------------------------------------------------------------------------------------//------------------------------------------------------------------------------------


        private void crearFactura(int ecena, string nF = null, decimal? pF = null, int? cF = null)
        {
            //-----crear Factura----------------------------------------
            if (ecena == 1)
            {
                enFac = true;

                SqlCommand cmd = new SqlCommand(
                    "select top 1  idFacturas from facturas order by idFacturas desc;",
                    conexion
                );

                nn = Convert.ToInt32(cmd.ExecuteScalar()) + 1;



                dtD.Columns.Add("IdFactura");
                dtD.Columns.Add("nombre");
                dtD.Columns.Add("precio");
                dtD.Columns.Add("cantidad");

                total = 0;

                DtId.Text = "";
                DtId.Text += "00." + nn;
            }

            //-----editar Factura---------------------------------------
            if (ecena == 2 & enFac == true)
            {
                dtD.Rows.Add(nn, nF, pF, cF);
                decimal i = (pF ?? 0) * (cF ?? 0);

                total += i;
                enFac2 = true;
            }

            //-----confirmar Factura------------------------------------
            if (ecena == 3 & enFac == true & enFac2 == true)
            {



                foreach (DataRow fila in dtD.Rows)
                {
                    SqlCommand cmdD = new SqlCommand(
                        "insert into DetallesF(IdDetallesF, IdFactura, Nombre, Precio, Cantidad) values(next value for seq_idDetalleF, @i, @nombre, @precio, @cantidad)",
                        conexion
                    );

                    cmdD.Parameters.AddWithValue("@i", fila["IdFactura"]);
                    cmdD.Parameters.AddWithValue("@nombre", fila["nombre"]);
                    cmdD.Parameters.AddWithValue("@precio", fila["precio"]);
                    cmdD.Parameters.AddWithValue("@cantidad", fila["cantidad"]);

                    cmdD.ExecuteNonQuery();

                    //---------------------------------------------------------------------------


                    SqlCommand cmdP = new SqlCommand(
                        "UPDATE productos SET stock = stock - @cantidad WHERE nombre = @nombre",
                        conexion
                    );

                    cmdP.Parameters.AddWithValue("@nombre", fila["nombre"]);
                    cmdP.Parameters.AddWithValue("@cantidad", fila["cantidad"]);

                    cmdP.ExecuteNonQuery();
                }

                SqlCommand cmdF = new SqlCommand(
                        "insert into Facturas(idFacturas, Total, Fecha) values(NEXT VALUE FOR seq_idFactura, @Total, GETDATE());",
                        conexion
                );

                cmdF.Parameters.AddWithValue("@Total", total);

                cmdF.ExecuteNonQuery();

                total = 0;
                nn = 0;
                dtD?.Reset();
                enFac = false;
                enFac2 = false;
                lblViewFactura.Text = "------------Factura-----------                               ";
                crearFactura(1);

                foreach (Control p in ptFacturacion.Controls)
                {
                    if (p != null && p.Tag?.ToString() == "pnlC")
                    {
                        ptFacturacion.Controls.Remove(p);
                        p.Dispose();
                        break;
                    }
                }
            }

            if (ecena == 4)
            {
                total = 0;
                nn = 0;
                dtD?.Reset();
                enFac = false;
                enFac2 = false;
                lblViewFactura.Text = "------------Factura-----------                               ";

                foreach (Control p in ptFacturacion.Controls)
                {
                    if (p.Tag?.ToString() == "pnlC")
                    {
                        ptFacturacion.Controls.Remove(p);
                        p.Dispose();
                        break;
                    }
                }
            }

        }

        //------------------------------------------------------------------------------------//------------------------------------------------------------------------------------
        //--------------------------------PestaÒa Factura-------------------------------------//------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------//------------------------------------------------------------------------------------



        //------------------------------------------------------------------------------------//------------------------------------------------------------------------------------
        //--------------------------------PestaÒa Reporte-------------------------------------//------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------//------------------------------------------------------------------------------------

        private void txtBuscador_TextChanged(object sender, EventArgs e)
        {
            lblProductos.Text = txtBuscador.Text;
            PnlInventario.Controls.Clear();

            if (accion == true)
            {
                PnlInventario.Controls.Add(CTabla("productos", txtBuscador.Text, "*", CSubirReporte));
            }
            else
            {
                PnlInventario.Controls.Add(CTabla("productos", txtBuscador.Text, "*"));
            }


            if (txtBuscador.Text == "")
            {
                lblProductos.Text = "Productos";
            }
        }

        //------------------------------------------------------------------------------------//------------------------------------------------------------------------------------


        private void btnReporte_Click(object sender, EventArgs e)
        {
            if (rr == 1)
            {
                PnlInventario.Controls.Clear();
                PnlInventario.Controls.Add(CTabla("productos", txtBuscador.Text, "*", CSubirReporte));
                crearReporte(1);
                btnReporte.Text = "Cancelar";
                txtBuscador.Location = new Point(10, 8);
                btnReporte.ForeColor = Color.White;
                btnReporte.BackColor = Color.Black;
                txtBuscador.Focus();
                pnlinv.Width = pnlinv.Width / 2;
                PnlInventario.Width = PnlInventario.Width / 2;
                rr = 2;
                RedondearControl(pnlinv, 20);
                pnlR.Location = new Point(pnlinv.Left + pnlinv.Width + 20, 204);
                pnlR.Visible = true;
                button1.Visible = true;
                accion = true;
                RedondearControl(pnlR, 20);
                CProductos.Visible = false;
            }
            else
            {
                btnReporte.Text = "Nuevo Reporte";
                txtBuscador.Location = new Point(299, 8);
                btnReporte.ForeColor = Color.Black;
                btnReporte.BackColor = Color.White;
                pnlinv.Width = pnlinv.Width * 2;
                PnlInventario.Width = PnlInventario.Width * 2;
                rr = 1;
                RedondearControl(pnlinv, 20);
                pnlR.Location = new Point(1140, 204);

                accion = false;
                crearReporte(4);
                RedondearControl(pnlR, 20);
                pnlR.Visible = false;
                button1.Visible = false;
                CProductos.Visible = true;

            }
        }

        //------------------------------------------------------------------------------------//------------------------------------------------------------------------------------


        private void CSubirReporte(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            string t = ((DataGridView)sender).Rows[e.RowIndex].Cells["nombre"].Value.ToString();
            string j = ((DataGridView)sender).Rows[e.RowIndex].Cells[2].Value.ToString();

            Panel pnl = new Panel()
            {
                Size = new Size(300, 150),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Tag = "pnlC"
            };

            pnl.Location = new Point(
            (this.ClientSize.Width - pnl.Width) / 2,
            (this.ClientSize.Height - pnl.Height) / 2);

            TextBox txt = new TextBox()
            {
                Size = new Size(135, 23),
                TextAlign = HorizontalAlignment.Center,
                Location = new Point(115, 70)
            };

            Label lbl1 = new Label()
            {
                AutoSize = false,
                Text = $"Se a seleccionado |{t}| para subir al reporte actual",
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(200, 35),
                Location = new Point(50, 25)
            };

            Label lbl2 = new Label()
            {
                AutoSize = false,
                Text = "Cantidad",
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(60, 23),
                Location = new Point(50, 70)
            };

            Button btn = new Button()
            {
                Text = "Confirmar",
                Size = new Size(200, 29),
                Location = new Point(50, 100)
            };

            Button btns = new Button()
            {
                Text = "Confirmar",
                Size = new Size(20, 20),
                Location = new Point(280, 0)
            };

            btn.Click += (s, e) =>
            {
                if (!int.TryParse(txt.Text, out int numero) || txt.Text == "0")
                {
                    MessageBox.Show("Valor no permitido");
                    txt.Text = "";
                    txt.Focus();
                    return;
                }



                string texto;
                string cantidad = txt.Text;
                string tt = t + " |" + cantidad;

                texto = tt.PadRight(30 - j.Length, '-')
                + j + "\n \n";

                verReporte.Text += texto;

                ptInventario.Controls.Remove(pnl);

                pnl.Dispose();

                crearReporte(2, t, decimal.Parse(j), int.Parse(cantidad));


            };

            btns.Click += (s, e) =>
            {
                ptInventario.Controls.Remove(pnl);
                pnl.Dispose();
            };

            pnl.Controls.Add(txt);
            pnl.Controls.Add(btn);
            pnl.Controls.Add(lbl1);
            pnl.Controls.Add(lbl2);
            pnl.Controls.Add(btns);

            ptInventario.Controls.Add(pnl);

            pnl.BringToFront();

            txt.Focus();

        }

        //------------------------------------------------------------------------------------//------------------------------------------------------------------------------------


        private void crearReporte(int ecena, string nF = null, decimal? pF = null, int? cF = null)
        {
            //-----crear Reporte----------------------------------------
            if (ecena == 1)
            {
                enFacR = true;

                SqlCommand cmd = new SqlCommand(
                    "select top 1  idReporte from Reportes order by idReporte desc;",
                    conexion
                );

                nnR = Convert.ToInt32(cmd.ExecuteScalar()) + 1;



                dtDR.Columns.Add("IdReporte");
                dtDR.Columns.Add("nombre");
                dtDR.Columns.Add("precio");
                dtDR.Columns.Add("cantidad");

                totalR = 0;
            }

            //-----editar Reporte---------------------------------------
            if (ecena == 2 & enFacR == true)
            {
                dtDR.Rows.Add(nnR, nF, pF, cF);
                decimal i = (pF ?? 0) * (cF ?? 0);

                totalR += i;
                enFac2R = true;
            }

            //-----confirmar Reporte------------------------------------
            if (ecena == 3 & enFacR == true & enFac2R == true)
            {


                foreach (DataRow fila in dtDR.Rows)
                {
                    SqlCommand cmdD = new SqlCommand(
                        "insert into DetallesR(IdDetallesR, IdReporte, Nombre,Cantidad) values(next value for seq_idDetalleR, @i, @nombre, @cantidad)",
                        conexion
                    );

                    cmdD.Parameters.AddWithValue("@i", fila["IdReporte"]);
                    cmdD.Parameters.AddWithValue("@nombre", fila["nombre"]);
                    cmdD.Parameters.AddWithValue("@cantidad", fila["cantidad"]);

                    cmdD.ExecuteNonQuery();

                    //---------------------------------------------------------------------------


                    SqlCommand cmdP = new SqlCommand(
                        "UPDATE productos SET stock = stock + @cantidad WHERE nombre = @nombre",
                        conexion
                    );

                    cmdP.Parameters.AddWithValue("@nombre", fila["nombre"]);
                    cmdP.Parameters.AddWithValue("@cantidad", fila["cantidad"]);

                    cmdP.ExecuteNonQuery();
                }

                SqlCommand cmdF = new SqlCommand(
                        "insert into Reportes(idReporte, Fecha) values(NEXT VALUE FOR seq_idReporte, GETDATE());",
                        conexion
                );

                cmdF.ExecuteNonQuery();

                totalR = 0;
                nnR = 0;
                dtDR?.Reset();
                enFacR = false;
                enFac2R = false;
                verReporte.Text = "------------Reporte-----------                               ";
                crearReporte(1);

                foreach (Control p in ptInventario.Controls)
                {
                    if (p != null && p.Tag?.ToString() == "pnlC")
                    {
                        ptInventario.Controls.Remove(p);
                        p.Dispose();
                        break;
                    }
                }
            }

            if (ecena == 4)
            {
                totalR = 0;
                nnR = 0;
                dtDR?.Reset();
                enFacR = false;
                enFac2R = false;
                verReporte.Text = "------------Reporte-----------                               ";

                foreach (Control p in ptInventario.Controls)
                {
                    if (p.Tag?.ToString() == "pnlC")
                    {
                        ptInventario.Controls.Remove(p);
                        p.Dispose();
                        break;
                    }
                }
            }

        }
        //------------------------------------------------------------------------------------//------------------------------------------------------------------------------------
        //--------------------------------PestaÒa Reporte-------------------------------------//------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------//------------------------------------------------------------------------------------




        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            tablaF.Controls.Clear();
            tablaF.Controls.Add(CTabla("productos", textBox1.Text, "nombre, precio"));

        }


        private void SDatos(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                MessageBox.Show($"Fila clickeada: {e.RowIndex}");

            }
        }







        //------------------------------------------------------------------------------------//------------------------------------------------------------------------------------
        //--------------------------------Menu Analisis---------------------------------------//------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------//------------------------------------------------------------------------------------


        //--------------------Grafico----------------------------------------------
        private void btnGrafico_Click(object sender, EventArgs e)
        {

            panelGrafico.Controls.Clear();

            panelGrafico.Controls.Add(CrearGrafico("johan", 10, "jose", 20, "eddison", 50));
        }


        //---------------------Facturas----------------------------------------------
        private void btnFacturas_Click(object sender, EventArgs e)
        {
            panelGrafico.Controls.Clear();

            panelGrafico.Controls.Add(CTabla("facturas", null, "*", vewFactura));
        }


        //----------------------Reportes----------------------------------------------
        private void btnReportes_Click(object sender, EventArgs e)
        {
            panelGrafico.Controls.Clear();

            panelGrafico.Controls.Add(CTabla("reportes", null, "*", vewReporte));
        }

        //------------------------------------------------------------------------------------//------------------------------------------------------------------------------------
        //--------------------------------Menu Analisis---------------------------------------//------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------//------------------------------------------------------------------------------------







        private void vewFactura(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;
            accion = true;
            string f = ((DataGridView)sender).Rows[e.RowIndex].Cells[2].Value.ToString();
            string Tt = ((DataGridView)sender).Rows[e.RowIndex].Cells[1].Value.ToString();
            int t = Convert.ToInt32(((DataGridView)sender).Rows[e.RowIndex].Cells[0].Value);


            Panel pnF = new Panel()
            {
                Size = new Size(400, 500),
                BackColor = Color.White,
                Location = new Point(this.Width / 2, 50)
            };


            Label lbF = new Label()
            {
                AutoSize = false,
                Size = new Size(380, 490),
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(5, 5),
                Font = new System.Drawing.Font("Consolas", 12),
                Text = "-----------------Factura---------------- \n \n Id |00" + t + "\n fecha |" + f + "\n\n-----------------Productos--------------\n\n"
            };
            SqlCommand cmd = new SqlCommand(
                $"select nombre, precio, cantidad from DetallesF where idfactura = @id",
                conexion);

            cmd.Parameters.AddWithValue("@id", t);

            SqlDataReader dr = cmd.ExecuteReader();

            string texto = "\n";

            while (dr.Read())
            {
                string nombre = dr["Nombre"].ToString();
                string precio = dr["Precio"].ToString();
                string cantidad = dr["Cantidad"].ToString();




                string tt = nombre + " |" + cantidad;

                texto += tt.PadRight(40 - precio.Length, '-') + precio + "\n \n";



            }
            texto += "-----------------Detalle---------------- \n \n total |" + Tt;
            lbF.Text += texto;
            dr.Close();

            //-----------------------------------

            pnF.Controls.Add(lbF);
            this.Controls.Add(pnF);

            pnF.BringToFront();

            pnF.Click += (s, e) =>
            {
                ptFacturacion.Controls.Remove(pnF);
                pnF.Dispose();
                accion = false;
            };

            lbF.Click += (s, e) =>
            {
                ptFacturacion.Controls.Remove(pnF);
                pnF.Dispose();
                accion = false;
            };

            //MessageBox.Show("ver factura");
        }






        private void vewReporte(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;
            accion = true;
            string f = ((DataGridView)sender).Rows[e.RowIndex].Cells[1].Value.ToString();
            int t = Convert.ToInt32(((DataGridView)sender).Rows[e.RowIndex].Cells[0].Value);


            Panel pnF = new Panel()
            {
                Size = new Size(400, 500),
                BackColor = Color.White,
                Location = new Point(this.Width / 2, 50)
            };


            Label lbF = new Label()
            {
                AutoSize = false,
                Size = new Size(380, 490),
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(5, 5),
                Font = new System.Drawing.Font("Consolas", 12),
                Text = "-----------------Reporte---------------- \n \n Id |00" + t + "\n fecha |" + f + "\n\n-----------------Productos--------------\n\n"
            };
            SqlCommand cmd = new SqlCommand(
                $"select nombre, cantidad from DetallesR where idReporte = @id",
                conexion);

            cmd.Parameters.AddWithValue("@id", t);

            SqlDataReader dr = cmd.ExecuteReader();

            string texto = "\n";

            while (dr.Read())
            {
                string nombre = dr["Nombre"].ToString();
                string cantidad = dr["Cantidad"].ToString();




                string tt = nombre + " |";

                texto += tt.PadRight(40 - 2 - cantidad.Length, '-') + "| " + cantidad + "\n \n";



            }
            texto += "-----------------Detalle---------------- \n \n ...";
            lbF.Text += texto;
            dr.Close();

            //-----------------------------------

            pnF.Controls.Add(lbF);
            this.Controls.Add(pnF);

            pnF.BringToFront();

            pnF.Click += (s, e) =>
            {
                ptFacturacion.Controls.Remove(pnF);
                pnF.Dispose();
                accion = false;
            };

            lbF.Click += (s, e) =>
            {
                ptFacturacion.Controls.Remove(pnF);
                pnF.Dispose();
                accion = false;
            };

            //MessageBox.Show("ver factura");
        }



        private void SubirFactura_Click_1(object sender, EventArgs e)
        {
            DtId.Text = "00";
            Dtcantidad.Text = "0000";
            tp = 0;
            cp = 0;
            Dttotal.Text = "00000.00";
            crearFactura(3);
        }





        private void button1_Click(object sender, EventArgs e)
        {
            crearReporte(3);
        }

        private void panel10_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel11_Paint(object sender, PaintEventArgs e)
        {

        }

        private void CProductos_Click(object sender, EventArgs e)
        {
            accion = true;
            ptControlP.BringToFront();
            ptControlP.Location = new Point(100, 20);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ptEditarP.BringToFront();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ptAgregarP.BringToFront();
        }


        //------------------------------------------------------------------------------------//------------------------------------------------------------------------------------
        //------------------------agregar Productos-------------------------------------------//------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------//------------------------------------------------------------------------------------
        private void button6_Click(object sender, EventArgs e)
        {

            string nombre = txt3.Text;
            string precio = txt2.Text;
            string cantidad = txt1.Text;

            if (!decimal.TryParse(precio, out decimal dec) || precio == "0" || precio == "" || !Regex.IsMatch(precio, @"^[0-9.]+$"))
            {
                MessageBox.Show("Valor no valido en Precio");
                txt2.Focus();
                return;
            }

            if (!int.TryParse(cantidad, out int cn) || cantidad == "0" || cantidad == "" || !Regex.IsMatch(cantidad, @"^[0-9]+$"))
            {
                MessageBox.Show("Valor no valido en Cantidad");
                txt1.Focus();
                return;
            }

            if (nombre == "" || !Regex.IsMatch(nombre, @"^[a-zA-Z0-9·ÈÌÛ˙¡…Õ”⁄Ò—_ )(]+$"))
            {
                MessageBox.Show("Valor no valido en Nombre");
                txt3.Focus();
                return;
            }


            SqlCommand cmd = new SqlCommand(
                "INSERT INTO productos (id, nombre, precio, stock) VALUES (next value for seq_idProductos, @nombre, @precio, @stock)",
                conexion
            );

            cmd.Parameters.AddWithValue("@nombre", nombre);
            cmd.Parameters.AddWithValue("@precio", precio);
            cmd.Parameters.AddWithValue("@stock", cantidad);

            cmd.ExecuteNonQuery();

            MessageBox.Show($"Has ingresado una nueva tabla:\nNombre: {nombre}\nPrecio: {precio}\nCantidade: {cantidad}");


        }

        private void button3_Click(object sender, EventArgs e)
        {
            txt3.Text = "";
            txt2.Text = "";
            txt1.Text = "";
        }

        private void cerrarCP_Click(object sender, EventArgs e)
        {
            accion = false;
            txt3.Text = "";
            txt2.Text = "";
            txt1.Text = "";
            ptInventario.BringToFront();
        }
    }
}

//todo bien, todo correcto 
