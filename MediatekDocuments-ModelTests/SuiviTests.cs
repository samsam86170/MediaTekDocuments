using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using MediaTekDocuments.model;

namespace MediaTekDocuments_ModelTests
{
    /// <summary>
    /// Classe de test unitaire pour la classe métier Suivi
    /// </summary>
    [TestClass]
    public class SuiviTests
    {
        private const string id = "00001";
        private const string libelle = "en cours";

        private static readonly Suivi suivi = new Suivi(id, libelle);

        /// <summary>
        /// Teste le constructeur de la classe Suivi
        /// </summary>
        [TestMethod()]
        public void SuiviTest()
        {
            Assert.AreEqual(id, suivi.Id, "devrait réussir : id valorisé");
            Assert.AreEqual(libelle, suivi.Libelle, "devrait réussir : libellé valorisé");
        }
    }
}