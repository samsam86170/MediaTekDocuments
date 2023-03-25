using MediaTekDocuments.model;
using MediaTekDocuments.dal;

namespace MediaTekDocuments.controller
{
   
    public class FrmAuthentificationController
    {
        /// <summary>
        /// Objet d'accès aux données
        /// </summary>
        private readonly Access access;

        /// <summary>
        /// Récupération de l'instance unique d'accès aux données
        /// </summary>
        public FrmAuthentificationController()
        {
            access = Access.GetInstance();
        }

        /// <summary>
        /// récupere les identifiants d'un utilisateur
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <returns>True si utilisateur et mot de passe trouvés</returns>
        public Service GetUtilisateur(string login, string password)
        {
            Utilisateur utilisateur = access.GetUtilisateur(login);

            if (utilisateur == null)
            {
                return null;
            }
            if (utilisateur.Password.Equals(utilisateur.Password))
            {
                Service service = new Service(utilisateur.IdService, utilisateur.Libelle);
                service.Id = utilisateur.IdService;
                service.Libelle = utilisateur.Libelle;
                return service;
            }
            return null;
        }

    }
}
