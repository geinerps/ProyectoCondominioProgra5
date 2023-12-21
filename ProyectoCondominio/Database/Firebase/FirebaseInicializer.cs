using Firebase.Auth;
using Firebase.Auth.Providers;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;

namespace ProyectoCondominio.FirebaseServices;

public class FirebaseInicializer
{
    private FirebaseAuthClient _authInstance;
    private FirestoreDb _firestoreInstance;

    public FirebaseInicializer(string projectId)
    {
        string credentialsPath = Environment.CurrentDirectory + "/condominio-config.json";
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialsPath);

        _authInstance = new FirebaseAuthClient(new FirebaseAuthConfig
        {
            ApiKey = "AIzaSyDWqb1VCreD9Ot9lsGyRRDj5Gt8HS9hlN8",
            AuthDomain = $"{projectId}.firebaseapp.com",
            Providers = new FirebaseAuthProvider[]
            {
                new EmailProvider()
            }
        });

        _firestoreInstance = FirestoreDb.Create(projectId);
    }

    public FirestoreDb GetFirestoreInstance()
    {
        return _firestoreInstance;
    }

    public FirebaseAuthClient GetFirebaseAuthInstance()
    {
        return _authInstance;
    }
}
