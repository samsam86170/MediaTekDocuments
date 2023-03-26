using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using MediaTekDocuments.model;

namespace MediaTekDocuments_ModelTests
{
    /// <summary>
    /// Classe de test unitaire pour la classe métier Rayon
    /// </summary>
    [TestClass]
    public class RayonTests
    {
        private const string id = "BD001";
        private const string libelle = "BD Adultes";

        private static readonly Rayon rayon = new Rayon(id, libelle);

        /// <summary>
        /// Teste le constructeur de la classe Rayon
        /// </summary>
        [TestMethod()]
        public void RayonTest()
        {
            Assert.AreEqual(id, rayon.Id, "devrait réussir : id valorisé");
            Assert.AreEqual(libelle, rayon.Libelle, "devrait réussir : libellé valorisé");
        }
    }
}