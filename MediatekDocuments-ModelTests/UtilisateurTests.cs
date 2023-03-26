using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using MediaTekDocuments.model;

namespace MediaTekDocuments_ModelTests
{
    /// <summary>
    /// Classe de test unitaire pour la classe métier Utilisateur
    /// </summary>
    [TestClass]
    public class UtilisateurTests
    {
        private const string login = "ZakariZotto";
        private const string password = "ZakariZotto430";
        private const string idService = "2";
        private const string libelle = "administratif";

        private static readonly Utilisateur utilisateur = new Utilisateur(login, password, idService, libelle);

        /// <summary>
        /// Teste le constructeur de la classe Utilisateur
        /// </summary>
        [TestMethod()]
        public void UtilisateurTest()
        {
            Assert.AreEqual(login, utilisateur.Login, "devrait réussir : user valorisé");
            Assert.AreEqual(password, utilisateur.Password, "devrait réussir : pwd valorisé");
            Assert.AreEqual(idService, utilisateur.IdService, "devrait réussir : idService valorisé");
            Assert.AreEqual(libelle, utilisateur.Libelle, "devrait réussir : libellé valorisé");
        }
    }
}