using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using MediaTekDocuments.model;

namespace MediaTekDocuments_ModelTests
{
    /// <summary>
    /// Classe de test unitaire pour la classe métier Etat
    /// </summary>
    [TestClass]
    public class EtatTests
    {
        private const string id = "00001";
        private const string libelle = "neuf";

        private static readonly Etat etat = new Etat(id, libelle);

        /// <summary>
        /// Teste le constructeur de la classe Etat
        /// </summary>
        [TestMethod()]
        public void EtatTest()
        {
            Assert.AreEqual(id, etat.Id, "devrait réussir : id valorisé");
            Assert.AreEqual(libelle, etat.Libelle, "devrait réussir : libellé valorisé");
        }
    }
}