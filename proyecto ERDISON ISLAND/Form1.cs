using Microsoft.Data.SqlClient;
using System.Data.SqlClient;
using System.Data.SqlTypes;
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


        private void Form1_Load(object sender, EventArgs e)
        {
            CrearGrafico("johan", 10, "jose", 20, "eddison", 50);
            asignarEnlaces();
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
            hola.Text = c.Name.ToString();

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

        private void SacProductos()
        {
            SqlCommand cmd = new SqlCommand(

                "select * from productos where id = 1",
                conexion

            );

            SqlDataReader reader = cmd.ExecuteReader();

            string res = "";

            if (reader.Read())
            {
                res = reader[1].ToString();
            }
            reader.Close();

            prueba.Text = res;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SacProductos();
        }

        private void bttnVer_Click(object sender, EventArgs e)
        {

            PnlInventario.Controls.Clear();

            PnlInventario.Controls.Add(tableProductos(PnlInventario, true, tboxFiltro.Text));
        }

        private Panel tableProductos(Panel md, bool filtro, string t = null)
        {
            int Al = md.Height;
            int An = md.Width;

            Panel Pnl = new Panel()
            {
                Location = new Point(5, 100),
                Size = new Size(An, Al),
                BorderStyle = BorderStyle.FixedSingle,
                AutoScroll = true,
                BackColor = Color.BlueViolet
            };


            Point i = sacFyC(filtro);

            Pnl.Controls.Add(crearTabla(An, i.X, i.Y, t));

            return Pnl;
        }

        private TableLayoutPanel crearTabla(int An, int Fi, int Col, string t = null)
        {
            TableLayoutPanel Tbl = new TableLayoutPanel()
            {
                ColumnCount =Col,
                RowCount = Fi,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                Size = new Size(An - 4, 40 * Fi),
                Location = new Point(1, 1)
            };

            for (int i = 0; i < Fi; i++)
            {
                Tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 40f));
            }

            for (int i = 0; i < Col; i++)
            {
                Tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, An / Fi));
            }

            for (int f = 0; f < Fi; f++)
            {
                for (int c = 0; c < Col; c++)
                {
                    Tbl.Controls.Add(crearLabel(sacProductos(f, c, t)), c, f);
                }
            }

            return Tbl;
        }
        
        private Label crearLabel(string t)
        {
            return new Label()
            {
                Dock = DockStyle.Fill,
                Text = t,
                Font = new Font("Arial", 20)
            };
        }

        private string sacProductos(int f, int c, string t = null)
        {
            string[] columna = { "id", "nombre", "precio", "stock", "ultimafecha" };

            SqlCommand cmd = new SqlCommand(
                $@"SELECT {columna[c]} 
                FROM productos 
                WHERE nombre LIKE @v 
                ORDER BY id ASC
                OFFSET @offset ROWS FETCH NEXT 1 ROWS ONLY",
                conexion
            );

            cmd.Parameters.AddWithValue("@v", "%" + (t ?? "") + "%");
            cmd.Parameters.AddWithValue("@offset", f);

            SqlDataReader leer = cmd.ExecuteReader();
            
            string res = "";

            if (leer.Read())
            {
                res = leer[0].ToString();
            }

            leer.Close();

            return res;
        }

        private Point sacFyC(bool filtro, string t = null)
        {
            SqlCommand cmdF = new SqlCommand(
            
                $"select count(*) from productos where nombre like '%' + @v + '%'",
                conexion
            );

            cmdF.Parameters.AddWithValue("@v", t ?? "");

            SqlCommand cmdC = new SqlCommand(

                "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'productos';",
                conexion
            );
            

            int f = Convert.ToInt32(cmdF.ExecuteScalar());

            int c = Convert.ToInt32(cmdC.ExecuteScalar());
            return new Point(f, c);
        }
    }
}

//todo bien, todo correcto 
