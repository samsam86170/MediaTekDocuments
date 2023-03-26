using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using MediaTekDocuments.model;

namespace MediaTekDocuments_ModelTests
{
    /// <summary>
    /// Classe de test unitaire pour la classe métier Dvd
    /// </summary>
    [TestClass]
    public class DvdTests
    {
        private const string id = "20008";
        private const string titre = "Police Academy";
        private const string image = "";
        private const int duree = 96;
        private const string realisateur = "Hugh Wilson";
        private const string synopsis = "Académie de police ouverte à tout citoyen qui veut s'y inscrire, sans examen d'entrée préalable.";
        private const string idGenre = "10013";
        private const string genre = "Comédie";
        private const string idPublic = "00003";
        private const string lePublic = "Tous publics";
        private const string idRayon = "DF001";
        private const string rayon = "DVD films";

        private static readonly Dvd dvd = new Dvd(id, titre, image, duree, realisateur, synopsis, idGenre, genre, idPublic, lePublic, idRayon, rayon);

        /// <summary>
        /// Teste le constructeur de la classe Dvd
        /// </summary>
        [TestMethod()]
        public void DvdTest()
        {
            Assert.AreEqual(id, dvd.Id, "devrait réussir : id valorisé");
            Assert.AreEqual(titre, dvd.Titre, "devrait réussir : titre valorisé");
            Assert.AreEqual(image, dvd.Image, "devrait réussir : chemin de l'image valorisé");
            Assert.AreEqual(duree, dvd.Duree, "devrait réussir : durée valorisée");
            Assert.AreEqual(realisateur, dvd.Realisateur, "devrait réussir : réalisateur valorisé");
            Assert.AreEqual(synopsis, dvd.Synopsis, "devrait réussir : synopsis valorisé");
            Assert.AreEqual(idGenre, dvd.IdGenre, "devrait réussir : idGenre valorisé");
            Assert.AreEqual(genre, dvd.Genre, "devrait réussir : genre valorisé");
            Assert.AreEqual(idPublic, dvd.IdPublic, "devrait réussir : idPublic valorisé");
            Assert.AreEqual(lePublic, dvd.Public, "devrait réussir : public valorisé");
            Assert.AreEqual(idRayon, dvd.IdRayon, "devrait réussir : idRayon valorisé");
            Assert.AreEqual(rayon, dvd.Rayon, "devrait réussir : rayon valorisé");
        }
    }
}