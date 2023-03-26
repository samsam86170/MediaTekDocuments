using MediaTekDocuments.model;
using MediaTekDocuments.view;
using NUnit.Framework;
using System;
using System.Windows.Forms;
using TechTalk.SpecFlow;

namespace SpecFlowMediatekDocuments.Steps
{
    /// <summary>
    /// Classe de tests fonctionnels SpecFlow
    /// </summary>
    [Binding]
    public class SpecFlowMediatekDocumentsSteps
    {
        private readonly FrmMediatek frmMediatek = new FrmMediatek(null);

        /// <summary>
        /// Récupère la valeur de l'id du livre saisi
        /// </summary>
        /// <param name="valeur"></param>
        [Given(@"Je saisis la valeur (.*) dans txbLivresNumRecherche")]
        public void GivenJeSaisisLaValeurDansTxbLivresNumRecherche(string valeur)
        {
            TextBox TxtValeur = (TextBox)frmMediatek.Controls["tabOngletsApplication"].Controls["tabLivres"].Controls["grpLivresRecherche"].Controls["txbLivresNumRecherche"];
            frmMediatek.Visible = true;
            TxtValeur.Text = valeur;
        }
        
        /// <summary>
        /// Récupère le nombre de lignes affichées dans la liste des livres
        /// Selon la valeur sélectionnée dans la comboBox du Genre
        /// </summary>
        /// <param name="listeAttendue">Nombre de lignes attendues dans la datagrid</param>
        [Given(@"Je sélectionne la valeur '(.*)' dans cbxLivresGenres")]
        public void GivenJeSelectionneLaValeurDansCbxLivresGenres(string listeAttendue)
        {
            ComboBox cbxLivresGenres = (ComboBox)frmMediatek.Controls["tabOngletsApplication"].Controls["tabLivres"].Controls["grpLivresInfos"].Controls["cbxLivresGenres"];
            cbxLivresGenres.SelectedItem = listeAttendue;
        }

        /// <summary>
        /// Récupère le nombre de lignes affichées dans la liste des livres
        /// Selon la valeur sélectionnée dans la comboBox du Public
        /// </summary>
        /// <param name="listeAttendue">Nombre de lignes attendues dans la datagrid</param>
        [Given(@"Je sélectionne la valeur '(.*)' dans cbxLivresPublics")]
        public void GivenJeSelectionneLaValeurDansCbxLivresPublics(string listeAttendue)
        {
            ComboBox cbxLivresPublics = (ComboBox)frmMediatek.Controls["tabOngletsApplication"].Controls["tabLivres"].Controls["grpLivresInfos"].Controls["cbxLivresPublics"];
            cbxLivresPublics.SelectedItem = listeAttendue;
        }

        /// <summary>
        /// Récupère le nombre de lignes affichées dans la liste des livres
        /// Selon la valeur sélectionnée dans la comboBox du Rayon
        /// </summary>
        /// <param name="listeAttendue">Nombre de lignes attendues dans la datagrid</param>
        [Given(@"Je sélectionne la valeur '(.*)' dans cbxLivresRayons")]
        public void GivenJeSelectionneLaValeurDansCbxLivresRayons(string listeAttendue)
        {
            ComboBox cbxLivresRayons = (ComboBox)frmMediatek.Controls["tabOngletsApplication"].Controls["tabLivres"].Controls["grpLivresInfos"].Controls["cbxLivresRayons"];
            cbxLivresRayons.SelectedItem = listeAttendue;
        }

        /// <summary>
        /// Récupère le titre du livre saisi
        /// </summary>
        /// <param name="titreAttendu">Récupère le titre attendu</param>
        [Given(@"Je saisis la valeur '(.*)' dans txbLivresTitreRecherche")]
        public void GivenJeSaisisLaValeurDansTxbLivresTitreRecherche(string titreAttendu)
        {
            TextBox txbLivresTitre = (TextBox)frmMediatek.Controls["tabOngletsApplication"].Controls["tabLivres"].Controls["grpLivresInfos"].Controls["txbLivresTitre"];
            string titreObtenu = txbLivresTitre.Text;
            Assert.AreEqual(titreAttendu, titreObtenu);
        }
        
        /// <summary>
        /// Evènement du clic sur le bouton de recherche
        /// </summary>
        [When(@"Je clic sur le bouton Rechercher")]
        public void WhenJeClicSurLeBoutonRechercher()
        {
            Button btnLivresNumRecherche = (Button)frmMediatek.Controls["tabOngletsApplication"].Controls["tabLivres"].Controls["grpLivresRecherche"].Controls["btnLivresNumRecherche"];
            frmMediatek.Visible = true;
            btnLivresNumRecherche.PerformClick();
        }
        
        /// <summary>
        /// Affichage du titre dans les informations détaillées 
        /// </summary>
        /// <param name="titreAttendu">Récupère le titre attendu</param>
        [Then(@"Les informations détaillées affichent le titre '(.*)'")]
        public void ThenLesInformationsDetailleesAffichentLeTitre(string titreAttendu)
        {
            TextBox txbLivresTitre = (TextBox)frmMediatek.Controls["tabOngletsApplication"].Controls["tabLivres"].Controls["grpLivresInfos"].Controls["txbLivresTitre"];
            string titreObtenu = txbLivresTitre.Text;
            Assert.AreEqual(titreAttendu, titreObtenu);
        }
        
        /// <summary>
        /// Récupère le nombre de lignes attendues 
        /// </summary>
        /// <param name="listeAttendue">Nombre de lignes attendues dans la datagrid</param>
        [Then(@"Le résultat est (.*) livres dans dgvLivresListe")]
        public void ThenLeResultatEstLivresDansDgvLivresListe(int listeAttendue)
        {
            DataGridView dgvLivresListe = (DataGridView)frmMediatek.Controls["tabOngletsApplication"].Controls["tabLivres"].Controls["grpLivresRecherche"].Controls["cbxLivresRecherchesGenres"];
            int listeObtenue = dgvLivresListe.Rows.Count;
            Assert.AreEqual(listeAttendue, listeObtenue);
        }

        /// <summary>
        /// Récupère le nombre de lignes attendues 
        /// </summary>
        /// <param name="listeAttendue">Nombre de lignes attendues dans la datagrid</param>
        [Then(@"Le résultat est (.*) livre")]
        public void ThenLeResultatEstLivre(int listeAttendue)
        {
            DataGridView dgvLivresListe = (DataGridView)frmMediatek.Controls["tabOngletsApplication"].Controls["tabLivres"].Controls["grpLivresRecherche"].Controls["cbxLivresRayons"];
            int listeObtenue = dgvLivresListe.Rows.Count;
            Assert.AreEqual(listeAttendue, listeObtenue);
        }
    }
}
