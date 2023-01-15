
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Testing.Models;

namespace Testing.Controllers
{
    public class ClientsController : Controller
    {

        List<Cliente> clientes = new List<Cliente>();
        String connectionString = "Data Source=CHRISLUAN; Initial catalog=clientes; integrated security=SSPI; TrustServerCertificate=True;";
        public IActionResult Index()=> View();
        public IActionResult Register(Cliente cl)
        {

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                DateTime date = DateTime.Now;
                cl.Data = date.ToString("dd/MM/yyyy HH:mm:ss");
                SqlCommand sqlcmd = new("ClientesProcedure", sqlConnection);
                sqlcmd.CommandType = System.Data.CommandType.StoredProcedure;

                sqlcmd.Parameters.AddWithValue("@Valor", cl.Valor);
                sqlcmd.Parameters.AddWithValue("@Nome", cl.Nome == null ? "Sem nome" : cl.Nome);
                sqlcmd.Parameters.AddWithValue("@Telefone", cl.Telefone == null ? "Sem telefone" : cl.Telefone);
                sqlcmd.Parameters.AddWithValue("@Servicos", cl.Servicos == null ? "Servicos não registrados" : cl.Servicos);
                sqlcmd.Parameters.AddWithValue("@Data", cl.Data);

                sqlConnection.Open();
                sqlcmd.ExecuteNonQuery();
                sqlConnection.Close();
            }
            FetchData();
            return RedirectToAction("ClientList");
        }
        public IActionResult ClientList()
        {
            FetchData();
            return View("ClientList", clientes);
        }
        public void FetchData()
        {
            SqlDataReader dr;



            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = @"SELECT TOP (10000) [Nome],[Telefone],[Servicos],[Valor],[Data], [ClienteID] FROM [clientes].[dbo].[tabelaClientes]";
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    clientes.Add(new Cliente()
                    {
                        ID = Convert.ToInt32(dr["ClienteID"].ToString()),
                        Nome = dr["Nome"].ToString(),
                        Telefone = dr["Telefone"].ToString(),
                        Servicos = dr["Servicos"].ToString(),
                        Valor = dr["Valor"].ToString(),
                        Data = dr["Data"].ToString()

                    });
                }
                con.Close();
                
            };
            
        }
        public IActionResult SavePage() => View();
    }
}
