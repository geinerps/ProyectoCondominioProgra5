using Google.Cloud.Firestore;

namespace ProyectoCondominio.Models
{
    [FirestoreData]
    public class User
    {
        [FirestoreDocumentId]
        public string? DocumentId { get; set; }
        [FirestoreProperty]
        public string? Name { get; set; }
        [FirestoreProperty]
        public string? Email { get; set; }
        [FirestoreProperty]
        public string? PhotoPath { get; set; }

    }
}
