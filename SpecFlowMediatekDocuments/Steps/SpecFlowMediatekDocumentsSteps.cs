using MediaTekDocuments.model;
using MediaTekDocuments.view;
using NUnit.Framework;
using System;
using System.Windows.Forms;
using TechTalk.SpecFlow;

namespace SpecFlowMediatekDocuments.Steps
{
    [Binding]
    public class SpecFlowMediatekDocumentsSteps
    {
        private readonly FrmMediatek frmMediatek = new FrmMediatek(null);

        [Given(@"Je saisis la valeur (.*) dans txbLivresNumRecherche")]
        public void GivenJeSaisisLaValeurDansTxbLivresNumRecherche(string valeur)
        {
            TextBox TxtValeur = (TextBox)frmMediatek.Controls["tabOngletsApplication"].Controls["tabLivres"].Controls["grpLivresRecherche"].Controls["txbLivresNumRecherche"];
            frmMediatek.Visible = true;
            TxtValeur.Text = valeur;
        }
        
        [Given(@"Je sélectionne la valeur '(.*)' dans cbxLivresGenres")]
        public void GivenJeSelectionneLaValeurDansCbxLivresGenres(string listeAttendue)
        {
            ComboBox cbxLivresGenres = (ComboBox)frmMediatek.Controls["tabOngletsApplication"].Controls["tabLivres"].Controls["grpLivresInfos"].Controls["cbxLivresGenres"];
            cbxLivresGenres.SelectedItem = listeAttendue;
        }
        
        [Given(@"Je sélectionne la valeur '(.*)' dans cbxLivresPublics")]
        public void GivenJeSelectionneLaValeurDansCbxLivresPublics(string listeAttendue)
        {
            ComboBox cbxLivresPublics = (ComboBox)frmMediatek.Controls["tabOngletsApplication"].Controls["tabLivres"].Controls["grpLivresInfos"].Controls["cbxLivresPublics"];
            cbxLivresPublics.SelectedItem = listeAttendue;
        }
        
        [Given(@"Je sélectionne la valeur '(.*)' dans cbxLivresRayons")]
        public void GivenJeSelectionneLaValeurDansCbxLivresRayons(string listeAttendue)
        {
            ComboBox cbxLivresRayons = (ComboBox)frmMediatek.Controls["tabOngletsApplication"].Controls["tabLivres"].Controls["grpLivresInfos"].Controls["cbxLivresRayons"];
            cbxLivresRayons.SelectedItem = listeAttendue;
        }
        
        [Given(@"Je saisis la valeur '(.*)' dans txbLivresTitreRecherche")]
        public void GivenJeSaisisLaValeurDansTxbLivresTitreRecherche(string titreAttendu)
        {
            TextBox txbLivresTitre = (TextBox)frmMediatek.Controls["tabOngletsApplication"].Controls["tabLivres"].Controls["grpLivresInfos"].Controls["txbLivresTitre"];
            string titreObtenu = txbLivresTitre.Text;
            Assert.AreEqual(titreAttendu, titreObtenu);
        }
        
        [When(@"Je clic sur le bouton Rechercher")]
        public void WhenJeClicSurLeBoutonRechercher()
        {
            Button btnLivresNumRecherche = (Button)frmMediatek.Controls["tabOngletsApplication"].Controls["tabLivres"].Controls["grpLivresRecherche"].Controls["btnLivresNumRecherche"];
            frmMediatek.Visible = true;
            btnLivresNumRecherche.PerformClick();
        }
        
        [Then(@"Les informations détaillées affichent le titre '(.*)'")]
        public void ThenLesInformationsDetailleesAffichentLeTitre(string titreAttendu)
        {
            TextBox txbLivresTitre = (TextBox)frmMediatek.Controls["tabOngletsApplication"].Controls["tabLivres"].Controls["grpLivresInfos"].Controls["txbLivresTitre"];
            string titreObtenu = txbLivresTitre.Text;
            Assert.AreEqual(titreAttendu, titreObtenu);
        }
        
        [Then(@"Le résultat est (.*) livres dans dgvLivresListe")]
        public void ThenLeResultatEstLivresDansDgvLivresListe(int listeAttendue)
        {
            DataGridView dgvLivresListe = (DataGridView)frmMediatek.Controls["tabOngletsApplication"].Controls["tabLivres"].Controls["grpLivresRecherche"].Controls["cbxLivresRecherchesGenres"];
            int listeObtenue = dgvLivresListe.Rows.Count;
            Assert.AreEqual(listeAttendue, listeObtenue);
        }
        
        [Then(@"Le résultat est (.*) livre")]
        public void ThenLeResultatEstLivre(int listeAttendue)
        {
            DataGridView dgvLivresListe = (DataGridView)frmMediatek.Controls["tabOngletsApplication"].Controls["tabLivres"].Controls["grpLivresRecherche"].Controls["cbxLivresRayons"];
            int listeObtenue = dgvLivresListe.Rows.Count;
            Assert.AreEqual(listeAttendue, listeObtenue);
        }
    }
}
