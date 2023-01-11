using Microsoft.AspNetCore.Mvc;
using Testing.Models;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using NuGet.Protocol.Core.Types;

namespace Testing.Controllers
{
    public class ClientsController : Controller
    {
       
        List<Cliente> clientes = new List<Cliente>();
        String connectionString = "Data Source=CHRISLUAN; Initial catalog=clientes; integrated security=SSPI; TrustServerCertificate=True;";
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Register(Cliente cl)
        {
           
            using(SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                DateTime date = DateTime.Now;
                cl.Data = date.ToString("dd/MM/yyyy HH:mm:ss");
                SqlCommand sqlcmd = new("ClientesProcedure", sqlConnection);
                sqlcmd.CommandType = System.Data.CommandType.StoredProcedure;

                sqlcmd.Parameters.AddWithValue("@Valor", cl.Valor);
                sqlcmd.Parameters.AddWithValue("@Nome", cl.Nome);
                sqlcmd.Parameters.AddWithValue("@Telefone", cl.Telefone);
                sqlcmd.Parameters.AddWithValue("@Servicos", cl.Servicos);
                sqlcmd.Parameters.AddWithValue("@Data", cl.Data);
                
                sqlConnection.Open();
                sqlcmd.ExecuteNonQuery();
                sqlConnection.Close();
            }
            FetchData();
            return View("ClientList", clientes);
        }
        public IActionResult Save()
        {
            return View();
        }
        public IActionResult ClientList()
        {
            FetchData();
            return View("ClientList", clientes);
         }
        public void FetchData()
        {
            SqlDataReader dr;

            if (clientes.Count > 0)
            {
                clientes.Clear();
            }
            using (SqlConnection con = new SqlConnection(connectionString))
            {

                con.Open();
                
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = @"SELECT TOP (1000) [Nome],[Telefone],[Servicos],[Valor],[Data] FROM [clientes].[dbo].[tabelaClientes]";
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    clientes.Add(new Cliente()
                    {

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
        public IActionResult SavePage()
        {
            return View();
        }
    }
}
