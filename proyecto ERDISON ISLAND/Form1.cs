using System.Security.Cryptography.X509Certificates;
using static System.Windows.Forms.Design.AxImporter;
using System.Windows.Forms.DataVisualization.Charting;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.Data.SqlClient;
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

        private void hola_TextChanged(object sender, EventArgs e)
        {

        }

        private void TbTodo_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel15_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}

