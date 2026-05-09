using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing.Drawing2D;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static System.ComponentModel.Design.ObjectSelectorEditor;
using static System.Windows.Forms.Design.AxImporter;
namespace proyecto_ERDISON_ISLAND
{
    public partial class Form1 : Form
    {
        SqlConnection conexion;
        int hh;
        bool accion;


        //contenedores de factura
        decimal total;
        int nn;
        bool enFac = false;
        bool enFac2 = false;  //ahora es personal

        DataTable dtD = new DataTable();


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

        public void CargarStock()
        {
            string query = "SELECT nombre, stock FROM productos";

            SqlDataAdapter da = new SqlDataAdapter(query, conexion);
            DataTable dt = new DataTable();
            da.Fill(dt);

            dataGridView2.DataSource = dt;

            // Cambiar nombres visibles
            dataGridView2.Columns["nombre"].HeaderText = "Nombre";
            dataGridView2.Columns["stock"].HeaderText = "Stock";
        }

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


        private void Form1_Load(object sender, EventArgs e)

        {
            hh = 1;
            CrearGrafico("johan", 10, "jose", 20, "eddison", 50);
            asignarEnlaces();
            CargarDatos();
            CargarStock();
            //RedondearPaneles

            RecorrerControles(this);




        }

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

        private void asignarEnlaces()
        {
            foreach (Control c in Menu.Controls)
            {
                asignarEvento(c, CambiarPestaña_click);
            }

        }
        private void asignarEvento(Control c, EventHandler e)
        {
            c.Click += e;

            foreach (Control cc in c.Controls)
            {
                cc.Click += e;
            }
        }
        private void CambiarPestaña_click(object sender, EventArgs e)
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



        private void CrearGrafico(string n1, int i1, string n2, int i2, string n3, int i3)
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

            panelGrafico.Controls.Add(chart);
        }



        private DataGridView CTabla(string nombre_tabla, string filtro = null, string t1 = null)
        {
            //-----------------------------------------//


            string query = $"SELECT {t1} FROM {nombre_tabla} WHERE nombre LIKE @t";

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
                Font = new Font("Arial", 14),
                BackgroundColor = Color.White,
                ColumnHeadersHeight = 40,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                AllowUserToResizeColumns = false,
                AllowUserToResizeRows = false,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle()
                {
                    Font = new Font("Arial", 18, FontStyle.Bold)
                },
                //dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 16, FontStyle.Bold);
                Location = new Point(0, 0),
                Dock = DockStyle.Fill,
                DataSource = dt
            };

            dgv.CellClick += CSubirProducto;
            return dgv;

        }

        private void txtBuscador_TextChanged(object sender, EventArgs e)
        {
            lblProductos.Text = txtBuscador.Text;
            PnlInventario.Controls.Clear();

            PnlInventario.Controls.Add(CTabla("productos", txtBuscador.Text, "*"));

            if (txtBuscador.Text == "")
            {
                lblProductos.Text = "Productos";
            }
        }

        private void ptFacturacion_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            crearFactura(3);
        }

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

                tablaF.Controls.Add(CTabla("productos", textBox1.Text, "nombre, precio"));
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

                tablaF.Controls.Clear();
                accion = false;
                crearFactura(4);
            }

        }

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

        private void panelGrafico_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panelint5_Paint(object sender, PaintEventArgs e)
        {

        }
        private void CSubirProducto(object sender, DataGridViewCellEventArgs e)
        {

            string t = ((DataGridView)sender).Rows[e.RowIndex].Cells["nombre"].Value.ToString();
            string j = ((DataGridView)sender).Rows[e.RowIndex].Cells[1].Value.ToString();

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
                string texto;
                string cantidad = txt.Text;
                string tt = t + " |" + cantidad;
                //MessageBox.Show("Escribiste: " + texto);

                texto = tt.PadRight(30 - j.Length, '-')
                + j + "\n \n";

                lblViewFactura.Text += texto;

                ptFacturacion.Controls.Remove(pnl);

                pnl.Dispose();

                crearFactura(2, t, decimal.Parse(j), int.Parse(cantidad));


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

            if(ecena == 4)
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





            // se crea una nueva factura
            //nn = 1;
            //nombreF = new string[] { "juan", "pedro" };
            //nombreF[nn] = "";


            
        }

        
    }
}

//todo bien, todo correcto 
