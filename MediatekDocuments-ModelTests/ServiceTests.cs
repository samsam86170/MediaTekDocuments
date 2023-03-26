using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using MediaTekDocuments.model;

namespace MediaTekDocuments_ModelTests
{
    /// <summary>
    /// Classe de test unitaire pour la classe métier Service
    /// </summary>
    [TestClass]
    public class ServiceTests
    {
        private const string id = "00001";
        private const string libelle = "Administratif";

        private static readonly Service service = new Service(id, libelle);

        /// <summary>
        ///  Teste le constructeur de la classe Service
        /// </summary>
        [TestMethod()]
        public void ServiceTest()
        {
            Assert.AreEqual(id, service.Id, "devrait réussir : id valorisé");
            Assert.AreEqual(libelle, service.Libelle, "devrait réussir : libellé valorisé");
        }
    }
}