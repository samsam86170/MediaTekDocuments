using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using MediaTekDocuments.model;

namespace MediaTekDocuments_ModelTests
{
    /// <summary>
    /// Classe de test unitaire pour la classe métier Exemplaire
    /// </summary>
    [TestClass]
    public class ExemplaireTests
    {
        private const int numero = 23;
        private static readonly DateTime dateAchat = new DateTime(2023, 3, 22);
        private const string photo = "";
        private const string idEtat = "00001";
        private const string id = "00003";
        private const string libelle = "neuf";

        private static readonly Exemplaire exemplaire = new Exemplaire(numero, dateAchat, photo, idEtat, id, libelle);

        /// <summary>
        /// Teste le constructeur de la classe Exemplaire
        /// </summary>
        [TestMethod()]
        public void ExemplaireTest()
        {
            Assert.AreEqual(numero, exemplaire.Numero, "devrait réussir : numéro valorisé");
            Assert.AreEqual(dateAchat, exemplaire.DateAchat, "devrait réussir : date d'achat valorisée");
            Assert.AreEqual(photo, exemplaire.Photo, "devrait réussir : photo valorisée");
            Assert.AreEqual(idEtat, exemplaire.IdEtat, "devrait réussir : idEtat valorisé");
            Assert.AreEqual(id, exemplaire.Id, "devrait réussir : id du document valorisé");
            Assert.AreEqual(libelle, exemplaire.Libelle, "devrait réussir : libellé valorisé");
        }
    }
}