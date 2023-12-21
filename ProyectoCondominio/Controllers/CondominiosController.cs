using Firebase.Storage;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProyectoCondominio.Models;
using System.Data;
using ProyectoCondominio.FirebaseServices;

namespace ProyectoCondominio.Controllers
{
    public class CondominiosController : Controller
    {
        private readonly FirestoreRepository _firestoreRepo;
        public CondominiosController(FirestoreRepository firestoreRepo)
        {
            _firestoreRepo = firestoreRepo;
        }
        // GET: CondominiosController
        public async Task<IActionResult> Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("userSession")))
                return RedirectToAction("Index", "Login");

            return await getCondominios();
        }

        private async Task<IActionResult> getCondominios()
        {
            List<Condominio> condominiosList = new List<Condominio>();

            // Query query = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId).Collection("Condominios");
            QuerySnapshot? querySnapshot = await _firestoreRepo.GetAll<Models.Condominio>("Condominios");

            // Valida si se encontraron condominios
            if (querySnapshot.Count == 0)
            {
                ViewBag.Condominios = null;
                return View("Index");
            }

            foreach (var item in querySnapshot)
            {
                Dictionary<string, object> data = item.ToDictionary();


                condominiosList.Add(new Condominio
                {
                    idProyectoHabitacional = item.Id,
                    logo = data.ContainsKey("logo") ? data["logo"]?.ToString() : null,
                    codigo = data.ContainsKey("codigo") ? data["codigo"]?.ToString() : null,
                    nombre = data.ContainsKey("nombre") ? data["nombre"]?.ToString() : null,
                    direccion = data.ContainsKey("direccion") ? data["direccion"]?.ToString() : null,
                    telefonoOficina = data.ContainsKey("telefonoOficina") ? data["telefonoOficina"]?.ToString() : null
                });
            }

            ViewBag.Condominios = condominiosList;

            return View("Index");
        }

        public ActionResult Agregar()
        {

            if (string.IsNullOrEmpty(HttpContext.Session.GetString("userSession")))
                return RedirectToAction("Index", "Login");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string Logo, string Codigo, string Nombre, string Direccion, string TelefonoOficina)
        {
            try
            {

                Condominio condominioToInsert = new Condominio()
                {
                    logo = Logo,
                    codigo = Codigo,
                    nombre = Nombre,
                    direccion = Direccion,
                    telefonoOficina = TelefonoOficina
                };

                //Aqui creamos el usuario en la collection de Firestore Database
                await _firestoreRepo.Insert<Condominio>("Condominios", condominioToInsert);

                return await getCondominios();
            }
            catch (FirebaseStorageException ex)
            {
                ViewBag.Error = new ErrorHandler()
                {
                    Title = ex.Message,
                    ErrorMessage = ex.InnerException?.Message,
                    ActionMessage = "Go to Condominios",
                    Path = "/Condominios"
                };

                return View("ErrorHandler");
            }
        }

        public async Task<ActionResult> Editar(string idProyectoHabitacional)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("userSession")))
                return RedirectToAction("Index", "Login");

            Condominio? condominio = await _firestoreRepo.Get<Condominio>("Condominios", idProyectoHabitacional);

            ViewBag.condominio = condominio;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(string idProyectoHabitacional, string Nombre, string Direccion, string TelefonoOficina)
        {
            try
            {

                Condominio condominioToUpdate = new Condominio()
                {
                    nombre = Nombre,
                    direccion = Direccion,
                    telefonoOficina = TelefonoOficina
                };

                //Aqui creamos el usuario en la collection de Firestore Database
                await _firestoreRepo.Update<Condominio>("Condominios", idProyectoHabitacional, condominioToUpdate);

                return await getCondominios();
            }
            catch (FirebaseStorageException ex)
            {
                ViewBag.Error = new ErrorHandler()
                {
                    Title = ex.Message,
                    ErrorMessage = ex.InnerException?.Message,
                    ActionMessage = "Go to Condominios",
                    Path = "/Condominios"
                };

                return View("ErrorHandler");
            }
        }

        public async Task<ActionResult> EliminarCondominio(string idProyectoHabitacional)
        {
            await _firestoreRepo.Delete<Condominio>("Condominios", idProyectoHabitacional);

            return RedirectToAction("Index", "Condominios");
        }
    }

}



//        private Condominio CargarCondominio(int idProyectoHabitacional)
//        {
//            List<SqlParameter> param = new List<SqlParameter>()
//            {
//                new SqlParameter("@idProyectoHabitacional", idProyectoHabitacional)
//            };

//            DataTable ds = FirebaseAuthHelper.ExecuteNonQuery("SP_ObtenerCondominio", param);

//            Condominio condomi = new Condominio()
//            {
//                idProyectoHabitacional = Convert.ToInt32(ds.Rows[0]["idProyectoHabitacional"]),
//                logo = ds.Rows[0]["logo"].ToString(),
//                codigo = ds.Rows[0]["codigo"].ToString(),
//                nombre = ds.Rows[0]["nombre"].ToString(),
//                direccion = ds.Rows[0]["direccion"].ToString(),
//                telefonoOficina = ds.Rows[0]["telefonoOficina"].ToString(),
//            };

//            return condomi;
//        }

//        public ActionResult UpdateCondominio(IFormFile inputPhoto, int idProyectoHabitacional, string nombre, string direccion, string telefonoOficina)
//        {

//            string? photoPath = null;

//            if (inputPhoto != null)
//            {
//                photoPath =
//                    "/images/fotos_condominios/"
//                    + Guid.NewGuid().ToString()
//                    + new FileInfo(inputPhoto.FileName).Extension;

//                using (
//                    var stream = new FileStream(
//                        Directory.GetCurrentDirectory() + "/wwwroot/" + photoPath,
//                        FileMode.Create
//                    )
//                )
//                {
//                    inputPhoto.CopyTo(stream);
//                }
//            }

//            List<SqlParameter> param = new List<SqlParameter>()
//            {
//                new SqlParameter("@idProyectoHabitacional", idProyectoHabitacional),
//                new SqlParameter("@logo", photoPath),
//                new SqlParameter("@nombre", nombre),
//                new SqlParameter("@direccion", direccion),
//                new SqlParameter("@telefonoOficina", telefonoOficina)
//            };

//            Database.DatabaseHelper.ExecuteQuery("SP_UpdateCondominio", param);

//            return RedirectToAction("Index", "Condominios");
//        }




//        public ActionResult AgregarCondominio(IFormFile inputPhoto, string codigo, string nombre, string direccion, string telefonoOficina, string selectNumViviendas)
//        {
//            string photoPath;

//            if (inputPhoto != null)
//            {
//                photoPath =
//                    "/images/fotos_condominios/"
//                    + Guid.NewGuid().ToString()
//                    + new FileInfo(inputPhoto.FileName).Extension;

//                using (
//                    var stream = new FileStream(
//                        Directory.GetCurrentDirectory() + "/wwwroot/" + photoPath,
//                        FileMode.Create
//                    )
//                )
//                {
//                    inputPhoto.CopyTo(stream);
//                }
//            }
//            else
//            {
//                photoPath = "/images/fotos_condominios/defaultCondominio.png";
//            }

//            List<SqlParameter> param = new List<SqlParameter>()
//            {
//                new SqlParameter("@logo", photoPath),
//                new SqlParameter("@codigo", codigo),
//                new SqlParameter("@nombre", nombre),
//                new SqlParameter("@direccion", direccion),
//                new SqlParameter("@telefonoOficina", telefonoOficina),
//                new SqlParameter("@numeroViviendas", selectNumViviendas),
//            };

//            Database.DatabaseHelper.ExecuteQuery("SP_AgregarCondominio", param);


//            return RedirectToAction("Index", "Condominios");
//        }
//    }
//}