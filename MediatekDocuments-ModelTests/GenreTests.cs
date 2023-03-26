using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using MediaTekDocuments.model;

namespace MediaTekDocuments_ModelTests
{
    /// <summary>
    /// Classe de test unitaire pour la classe métier Genre
    /// </summary>
    [TestClass]
    public class GenreTests
    {
        private const string id = "10000";
        private const string libelle = "Humour";

        private static readonly Genre genre = new Genre(id, libelle);

        /// <summary>
        /// Teste le constructeur de la classe Genre
        /// </summary>
        [TestMethod()]
        public void GenreTest()
        {
            Assert.AreEqual(id, genre.Id, "devrait réussir : id valorisé");
            Assert.AreEqual(libelle, genre.Libelle, "devrait réussir : libellé valorisé");
        }
    }
}