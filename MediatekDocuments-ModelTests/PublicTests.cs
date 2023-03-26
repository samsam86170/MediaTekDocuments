using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using MediaTekDocuments.model;

namespace MediaTekDocuments_ModelTests
{
    /// <summary>
    /// Classe de test unitaire pour la classe métier Public
    /// </summary>
    [TestClass]
    public class PublicTests
    {
        private const string id = "00001";
        private const string libelle = "Jeunesse";

        private static readonly Public lePublic = new Public(id, libelle);

        /// <summary>
        /// Teste le constructeur de la classe Public
        /// </summary>
        [TestMethod()]
        public void PublicTest()
        {
            Assert.AreEqual(id, lePublic.Id, "devrait réussir : id valorisé");
            Assert.AreEqual(libelle, lePublic.Libelle, "devrait réussir : libellé valorisé");
        }
    }
}