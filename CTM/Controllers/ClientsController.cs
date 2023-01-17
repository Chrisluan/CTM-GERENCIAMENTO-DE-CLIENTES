
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Testing.Models;

namespace Testing.Controllers
{
    public class ClientsController : Controller
    {

        List<Cliente> clientes = new List<Cliente>();
        string defaultSearchCmd = @"SELECT TOP (10000) [Nome],[Telefone],[Servicos],[Valor],[Data], [ClienteID] FROM [clientes].[dbo].[tabelaClientes]";
        String connectionString = "Data Source=CHRISLUAN; Initial catalog=clientes; integrated security=SSPI; TrustServerCertificate=True;";
        public IActionResult Index()=> View();
        public IActionResult Register(Cliente cl)
        {

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                
                SqlCommand sqlcmd = new("ClientesProcedure", sqlConnection);
                sqlcmd.CommandType = System.Data.CommandType.StoredProcedure;

                DateTime date = DateTime.Now;
                cl.Data = date.ToString("dd/MM/yyyy HH:mm:ss");
                sqlcmd.Parameters.AddWithValue("@Valor", cl.Valor == null ? "Valor não informado" : cl.Valor);
                sqlcmd.Parameters.AddWithValue("@Nome", cl.Nome == null ? "Sem nome" : cl.Nome);
                sqlcmd.Parameters.AddWithValue("@Telefone", cl.Telefone == null ? "Sem telefone" : cl.Telefone);
                sqlcmd.Parameters.AddWithValue("@Servicos", cl.Servicos == null ? "Servicos não registrados" : cl.Servicos);
                sqlcmd.Parameters.AddWithValue("@Data", cl.Data);

                sqlConnection.Open();
                sqlcmd.ExecuteNonQuery();
                sqlConnection.Close();
            }
            FetchData();
            return RedirectToAction("ClientList", clientes);
        }
        public IActionResult ClientList(string search = "", InfoType filter = InfoType.Nome)
        {
            FetchData();
            return View(FilterClient(search, filter));
        }
        public List<Cliente> FilterClient(string search = "", InfoType filterBy = InfoType.Nome)
        {
            search = search.ToUpper();
            List<Cliente> filteredClientsList = new();
            switch (filterBy)
            {
                case InfoType.Nome:
                    filteredClientsList = search == "" ? clientes : clientes.Where(x => x.Nome.ToUpper().Contains(search)).ToList();
                    break;
                case InfoType.Telefone:
                    filteredClientsList = search == "" ? clientes : clientes.Where(x => x.Telefone.ToUpper().Contains(search)).ToList();
                    break;
                case InfoType.Servicos:
                    filteredClientsList = search == "" ? clientes : clientes.Where(x => x.Servicos.ToUpper().Contains(search)).ToList();
                    break;
                case InfoType.Data:
                    filteredClientsList = search == "" ? clientes : clientes.Where(x => x.Data.ToUpper().StartsWith(search)).ToList();
                    break;
            }
           
            return filteredClientsList;
        }
        public void FetchData()
        {
            SqlDataReader dr;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = defaultSearchCmd;
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
