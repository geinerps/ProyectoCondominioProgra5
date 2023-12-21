using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProyectoCondominio.Database;
using ProyectoCondominio.Models;
using System.Data.SqlClient;
using System.Data;
using ProyectoCondominio.FirebaseServices;
using Google.Cloud.Firestore;
using Firebase.Storage;

namespace ProyectoCondominio.Controllers
{
    public class ViviendasController : Controller
    {
        private readonly FirestoreRepository _firestoreRepo;
        public ViviendasController(FirestoreRepository firestoreRepo)
        {
            _firestoreRepo = firestoreRepo;
        }
        // GET: ViviendasController
        public async Task<IActionResult> Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("userSession")))
                return RedirectToAction("Index", "Login");

            return await getViviendas();
        }

        private async Task<IActionResult> getViviendas()
        {
            List<Vivienda> viviendasList = new List<Vivienda>();

            // Query query = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId).Collection("Condominios");
            QuerySnapshot? querySnapshot = await _firestoreRepo.GetAll<Models.Vivienda>("Viviendas");

            // Valida si se encontraron las Viviendas
            if (querySnapshot.Count == 0)
            {
                ViewBag.Viviendas = null;
                return View("Index");
            }

            foreach (var item in querySnapshot)
            {
                Dictionary<string, object> data = item.ToDictionary();


                viviendasList.Add(new Vivienda
                {
                    idVivienda = item.Id,
                    numeroVivienda = data.ContainsKey("numeroVivienda") ? data["numeroVivienda"]?.ToString() : null,
                    descripcion = data.ContainsKey("descripcion") ? data["descripcion"]?.ToString() : null,
                    numeroHabitaciones = data.ContainsKey("numeroHabitaciones") ? data["numeroHabitaciones"]?.ToString() : null,
                    cochera = data.ContainsKey("cochera") ? data["cochera"]?.ToString() : null,
                    idProyectoHabitacional = item.Id,
                    idPersona = data.ContainsKey("idPersona") ? data["idPersona"]?.ToString() : null               
                });
            }

            ViewBag.Viviendas = viviendasList;

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
        public async Task<IActionResult> Create(int idVivienda, string numeroVivienda, string descripcion, int numeroHabitaciones, int cochera, int idProyectoHabitacional, int idPersona)
        {
            try
            {

                Vivienda viviendaToInsert = new Vivienda()
                {
                    idVivienda = idVivienda,
                    numeroVivienda = numeroVivienda,
                    descripcion = descripcion,
                    numeroHabitaciones = numeroHabitaciones,
                    cochera = cochera,
                    idProyectoHabitacional = idProyectoHabitacional,
                    idPersona = idPersona
                };

                //Aqui creamos el usuario en la collection de Firestore Database
                await _firestoreRepo.Insert<Vivienda>("Viviendas", viviendaToInsert);

                return await getViviendas();
            }
            catch (FirebaseStorageException ex)
            {
                ViewBag.Error = new ErrorHandler()
                {
                    Title = ex.Message,
                    ErrorMessage = ex.InnerException?.Message,
                    ActionMessage = "Go to Viviendas",
                    Path = "/Viviendas"
                };

                return View("ErrorHandler");
            }
        }


        public async Task<ActionResult> Editar(string idProyectoHabitacional)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("userSession")))
                return RedirectToAction("Index", "Login");

            Vivienda? vivienda = await _firestoreRepo.Get<Vivienda>("Viviendas", idProyectoHabitacional);

            ViewBag.condominio = vivienda;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int idVivienda, string numeroVivienda, string descripcion, int numeroHabitaciones, int cochera, int idProyectoHabitacional, int idPersona)
        {
            try
            {

                Vivienda viviendaToUpdate = new Vivienda()
                {
                    idVivienda = idVivienda,
                    numeroVivienda = numeroVivienda,
                    descripcion = descripcion,
                    numeroHabitaciones = numeroHabitaciones,
                    cochera = cochera,
                    idProyectoHabitacional = idProyectoHabitacional,
                    idPersona = idPersona
                };

                //Aqui creamos el usuario en la collection de Firestore Database
                await _firestoreRepo.Update<Vivienda>("Viviendas", idProyectoHabitacional, viviendaToUpdate);

                return await getViviendas();
            }
            catch (FirebaseStorageException ex)
            {
                ViewBag.Error = new ErrorHandler()
                {
                    Title = ex.Message,
                    ErrorMessage = ex.InnerException?.Message,
                    ActionMessage = "Go to Viviendas",
                    Path = "/Viviendas"
                };

                return View("ErrorHandler");
            }
        }

        public async Task<ActionResult> EliminarVivienda(string idProyectoHabitacional)
        {
            await _firestoreRepo.Delete<Vivienda>("Viviendas", idProyectoHabitacional);

            return RedirectToAction("Index", "Condominios");
        }
    }

}