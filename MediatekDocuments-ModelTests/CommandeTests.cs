using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using MediaTekDocuments.model;

namespace MediaTekDocuments_ModelTests
{
    /// <summary>
    /// Classe de test unitaire pour la classe métier Commande
    /// </summary>
    [TestClass]
    public class CommandeTests
    {
        private const string id = "00060";
        private static readonly DateTime dateCommande = new DateTime(2023, 3, 22);
        private const double montant = 50;

        private static readonly Commande commande = new Commande(id, dateCommande, montant);

        /// <summary>
        /// Teste le constructeur de la classe Commande
        /// </summary>
        [TestMethod()]
        public void CommandeTest()
        {
            Assert.AreEqual(id, commande.Id, "devrait réussir : id valorisé");
            Assert.AreEqual(dateCommande, commande.DateCommande, "devrait réussir : date de commande valorisée");
            Assert.AreEqual(montant, commande.Montant, "devrait réussir : montant valorisé");
        }
    }
}