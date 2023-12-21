using Google.Cloud.Firestore;

namespace ProyectoCondominio.FirebaseServices;

public class FirestoreRepository
{
    private readonly FirestoreDb _firestoreDB;
    public FirestoreRepository(FirebaseInicializer firestoreInstance)
    {
        _firestoreDB = firestoreInstance.GetFirestoreInstance();
    }

    /// <summary>
    /// Obtiene un documento específico de Firestore basado en su ID.
    /// </summary>
    /// <typeparam name="T">El tipo del objeto que se espera obtener.</typeparam>
    /// <param name="collectionName">El nombre de la colección</param>
    /// <param name="id">El ID del documento a obtener.</param>
    /// <returns>
    /// Un objeto del tipo especificado <typeparamref name="T"/>.
    /// Si el documento no existe, devuelve null.
    /// </returns>
    public async Task<T?> Get<T>(string collectionName, string id) where T : class
    {
        try
        {
            var document = _firestoreDB.Collection(collectionName).Document(id);
            var snapshot = await document.GetSnapshotAsync();

            if (!snapshot.Exists)
            {
                return null;
            }

            return snapshot.ConvertTo<T>();

        }
        catch (System.Exception)
        {
            throw;
        }

    }

    /// <summary>
    /// Obtiene todos los documentos de una colección de Firestore.
    /// </summary>
    /// <typeparam name="T">El tipo de los objetos en la colección.</typeparam>
    /// <param name="collectionName">El nombre de la colección</param>
    /// <returns>
    /// Lista de objetos de tipo T si la colección no está vacía; de lo contrario, retorna null.
    /// </returns>
    /// <remarks>
    /// Este método obtiene todos los documentos de una colección en Firestore y los convierte a una lista de objetos del tipo especificado.
    /// Si la colección está vacía, retorna null.
    /// </remarks>
    public async Task<QuerySnapshot> GetAll<T>(string collectionName) where T : class
    {
        try
        {
            var collection = _firestoreDB.Collection(collectionName);

            return await collection.GetSnapshotAsync();
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public async Task Insert<T>(string collectionName, T entity)
    {
        try
        {
            DocumentReference addedDocRef = await _firestoreDB.Collection(collectionName).AddAsync(entity);

            Console.WriteLine("{0} agregado con ID: {0}.", typeof(T).Name, addedDocRef.Id);
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public async Task Update<T>(string collectionName, string documentId, T entity)
    {
        try
        {
            var document = _firestoreDB.Collection(collectionName).Document(documentId);

            await document.SetAsync(entity, SetOptions.MergeAll);
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public async Task Delete<T>(string collectionName, string id)
    {
        try
        {
            var document = _firestoreDB.Collection(collectionName).Document(id);
            await document.DeleteAsync();
        }
        catch (System.Exception)
        {
            throw;
        }
    }
}
