using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using MediaTekDocuments.model;

namespace MediaTekDocuments_ModelTests
{
    /// <summary>
    /// Classe de tests unitaires pour la classe métier Categorie
    /// </summary>
    [TestClass]
    public class CategorieTests
    {
        private const string id = "10020";
        private const string libelle = "Horreur";

        private static readonly Categorie categorie = new Categorie(id, libelle);

        /// <summary>
        /// Teste les propriétés id et libelle 
        /// </summary>
        [TestMethod()]
        public void CategorieTest()
        {
            Assert.AreEqual(id, categorie.Id, "devrait réussir : id valorisé");
            Assert.AreEqual(libelle, categorie.Libelle, "devrait réussir : libellé valorisé");
        }

        /// <summary>
        /// Teste la méthode ToString
        /// </summary>
        [TestMethod()]
        public void ToStringTest()
        {
            Assert.AreEqual(libelle, categorie.Libelle, "devrait réussir : libellé au format string");
        }
    }
}