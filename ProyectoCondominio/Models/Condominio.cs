using Google.Cloud.Firestore;

namespace ProyectoCondominio.Models
{
    [FirestoreData]
    public class Condominio
    {
        [FirestoreDocumentId]
        public string? idProyectoHabitacional { get; set; }
        [FirestoreProperty]
        public string? logo { get; set; }
        [FirestoreProperty]
        public string? codigo { get; set; }
        [FirestoreProperty]
        public string? nombre { get; set; }
        [FirestoreProperty]
        public string? direccion { get; set; }
        [FirestoreProperty]
        public string? telefonoOficina { get; set; }
    }
}
