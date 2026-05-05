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
            txtBuscador.Text = c.Name.ToString();

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



        private DataGridView CTabla(string nombre_tabla, string filtro = null)
        {
            //-----------------------------------------//

            string query = $"SELECT * FROM {nombre_tabla} WHERE nombre LIKE @t";

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
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle()
                {
                    Font = new Font("Arial", 18, FontStyle.Bold)
                },
                //dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 16, FontStyle.Bold);
                Location = new Point(0, 0),
                Dock = DockStyle.Fill,
                DataSource = dt
            };


            return dgv;
        }

        private void txtBuscador_TextChanged(object sender, EventArgs e)
        {
            lblProductos.Text = txtBuscador.Text;
            PnlInventario.Controls.Clear();

            PnlInventario.Controls.Add(CTabla("productos", txtBuscador.Text));

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

        }
    }
}

//todo bien, todo correcto 
