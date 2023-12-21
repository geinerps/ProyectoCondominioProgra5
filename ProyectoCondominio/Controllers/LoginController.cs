using Firebase.Auth;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProyectoCondominio.FirebaseServices;
using ProyectoCondominio.Models;

namespace ProyectoCondominio.Controllers
{
    public class LoginController : Controller
    {
        private readonly FirebaseAuthClient _firebaseAuth;
        private readonly FirestoreRepository _firestoreRepo;
        public LoginController(FirebaseInicializer firebaseInicializer, FirestoreRepository firestoreRepo)
        {
            _firebaseAuth = firebaseInicializer.GetFirebaseAuthInstance();
            _firestoreRepo = firestoreRepo;
        }

        // GET: LoginController
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Logout()
        {
            return View();
        }

        public async Task<IActionResult> Login(string loginUsername, string loginPassword)
        {
            try
            {
                //Aqui Firebase Auth nos devuelve si el usuario existe y esta activo
                UserCredential userLogged = await _firebaseAuth.SignInWithEmailAndPasswordAsync(loginUsername, loginPassword);

                // Crea una consulta en Firestore Database que busca el usuario
                Models.User? user = await _firestoreRepo.Get<Models.User>("Users", userLogged.User.Uid);

                //Aqui creamos el objeto del usuario
                Models.User userSession = new Models.User
                {
                    DocumentId = userLogged.User.Uid,
                    Name = userLogged.User.Info.DisplayName,
                    Email = userLogged.User.Info.Email,
                    PhotoPath = userLogged.User.Info.PhotoUrl
                };

                //Aqui guardamos los datos del usuario en la session en formato json
                HttpContext.Session.SetString("userSession", JsonConvert.SerializeObject(userSession));

                return RedirectToAction("Index", "Home");
            }
            catch (FirebaseAuthHttpException ex)
            {
                return FirebaseAuthHttpExceptionHandler(ex);
            }


            /* try
            {
                //Aqui Firebase Auth nos devuelve si el usuario existe y esta activo
                UserCredential taskUser = await FirebaseAuthHelper.setFirebaseAuthClient().SignInWithEmailAndPasswordAsync(loginUsername, loginPassword);

                // Crea una consulta en Firestore Database que busca el usuario
                Query query = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId).Collection("Users").WhereEqualTo("Id", taskUser.User.Uid);

                // Ejecuta la consulta y obtenemos el dato
                QuerySnapshot querySnapshot = await query.GetSnapshotAsync();
                Dictionary<string, object> data = querySnapshot.Documents[0].ToDictionary();

                //Aqui creamos el objeto del usuario
                Models.User user = new Models.User
                {
                    DocumentId = data["DocumentId"].ToString(),
                    Id = taskUser.User.Uid,
                    Name = taskUser.User.Info.DisplayName,
                    Email = taskUser.User.Info.Email,
                    PhotoPath = data["PhotoPath"].ToString()
                };

                //Aqui guardamos los datos del usuario en la session en formato json
                HttpContext.Session.SetString("userSession", JsonConvert.SerializeObject(user));

                return RedirectToAction("Index", "Home");
            }
            catch (FirebaseAuthHttpException ex)
            {
                return FirebaseAuthHttpExceptionHandler(ex);
            } */
        }

        public async Task<IActionResult> SignUp(string signupUsername, string signupPassword, string signupDisplayName)
        {
            try
            {
                //Aqui Firebase crea el usuario en Firebase Auth 
                UserCredential userCreated = await _firebaseAuth.CreateUserWithEmailAndPasswordAsync(signupUsername, signupPassword, signupDisplayName);
                Console.WriteLine(userCreated.User.Uid);

                Models.User userToInsert = new Models.User()
                {
                    DocumentId = userCreated.User.Uid,
                    Name = signupDisplayName,
                    Email = signupUsername,
                    PhotoPath = ""
                };

                //Aqui creamos el usuario en la collection de Firestore Database
                await _firestoreRepo.Insert<Models.User>("Users", userToInsert);

                /* DocumentReference addedDocRef =
                    await FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId)
                        .Collection("Users").AddAsync(new Dictionary<string, object>
                            {
                                { "DocumentId", "" },
                                { "Id", userCreated.User.Uid },
                                { "Name", userCreated.User.Info.DisplayName },
                                { "Email", userCreated.User.Info.Email },
                                { "PhotoPath", "" },
                            }); */

                //Aqui actualizamos el usuario en Firestore Database para actualizar el DocumentId
                /* FirestoreDb db = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId);
                DocumentReference docRef = db.Collection("Users").Document(addedDocRef.Id);
                Dictionary<string, object> dataToUpdate = new Dictionary<string, object>
                {
                    { "DocumentId", addedDocRef.Id },
                };
                WriteResult result = await docRef.UpdateAsync(dataToUpdate); */

                return View("Index");
            }
            catch (FirebaseAuthHttpException ex)
            {
                return FirebaseAuthHttpExceptionHandler(ex);
            }

            /*try
            {
                //Aqui Firebase crea el usuario en Firebase Auth 
                UserCredential taskUser = await FirebaseAuthHelper.setFirebaseAuthClient().
                    CreateUserWithEmailAndPasswordAsync(signupUsername, signupPassword, signupDisplayName);

                //Aqui creamos el usuario en la collection de Firestore Database
                DocumentReference addedDocRef =
                    await FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId)
                        .Collection("Users").AddAsync(new Dictionary<string, object>
                            {
                                { "DocumentId", "" },
                                { "Id", taskUser.User.Uid },
                                { "Name", taskUser.User.Info.DisplayName },
                                { "Email", taskUser.User.Info.Email },
                                { "PhotoPath", "" },
                            });

                //Aqui actualizamos el usuario en Firestore Database para actualizar el DocumentId
                FirestoreDb db = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId);
                DocumentReference docRef = db.Collection("Users").Document(addedDocRef.Id);
                Dictionary<string, object> dataToUpdate = new Dictionary<string, object>
                {
                    { "DocumentId", addedDocRef.Id },
                };
                WriteResult result = await docRef.UpdateAsync(dataToUpdate);

                return View("Index");
            }
            catch (FirebaseAuthHttpException ex)
            {
                return FirebaseAuthHttpExceptionHandler(ex);
            }*/
        }

        public async Task<IActionResult> ForgotPwd(string signupUsername)
        {
            try
            {
                await _firebaseAuth.ResetEmailPasswordAsync(signupUsername);

                return View("Index");
            }
            catch (FirebaseAuthHttpException ex)
            {
                return FirebaseAuthHttpExceptionHandler(ex);
            }
        }

        public IActionResult FirebaseAuthHttpExceptionHandler(FirebaseAuthHttpException ex)
        {
            ViewBag.Error = new ErrorHandler()
            {
                Title = ex.Reason.ToString(),
                ErrorMessage = ex.InnerException?.Message,
                ActionMessage = "Go to login",
                Path = "/Login"
            };

            return View("ErrorHandler");
        }
    }
}